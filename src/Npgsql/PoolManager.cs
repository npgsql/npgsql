using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Npgsql.Logging;
#if NET45 || NET451
using System.Transactions;
#endif

namespace Npgsql
{
    static class PoolManager
    {
        /// <summary>
        /// Holds connector pools indexed by their connection strings.
        /// </summary>
        internal static ConcurrentDictionary<NpgsqlConnectionStringBuilder, ConnectorPool> Pools { get; }

        /// <summary>
        /// Maximum number of possible connections in the pool.
        /// </summary>
        internal const int PoolSizeLimit = 1024;

        static PoolManager()
        {
            Pools = new ConcurrentDictionary<NpgsqlConnectionStringBuilder, ConnectorPool>();

#if NET45 || NET451
            // When the appdomain gets unloaded (e.g. web app redeployment) attempt to nicely
            // close idle connectors to prevent errors in PostgreSQL logs (#491).
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => ClearAll();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => ClearAll();
#endif
        }

        internal static ConnectorPool GetOrAdd(NpgsqlConnectionStringBuilder connString)
        {
            Debug.Assert(connString != null);

            return Pools.GetOrAdd(connString, cs =>
            {
                if (cs.MaxPoolSize < cs.MinPoolSize)
                    throw new ArgumentException($"Connection can't have MaxPoolSize {cs.MaxPoolSize} under MinPoolSize {cs.MinPoolSize}");
                return new ConnectorPool(cs);
            });
        }

        internal static void Clear(NpgsqlConnectionStringBuilder connString)
        {
            Debug.Assert(connString != null);

            ConnectorPool pool;
            if (Pools.TryGetValue(connString, out pool))
                pool.Clear();
        }

        internal static void ClearAll()
        {
            foreach (var kvp in Pools)
                kvp.Value.Clear();
        }
    }

    sealed partial class ConnectorPool : IDisposable
    {
        #region Fields

        readonly NpgsqlConnectionStringBuilder _connectionString;

        /// <summary>
        /// Open connectors waiting to be requested by new connections
        /// </summary>
        internal readonly IdleConnectorList Idle;

        readonly int _max;
        readonly int _min;
        internal int Busy { get; private set; }

        readonly Queue<WaitingOpenAttempt> _waiting;

        struct WaitingOpenAttempt
        {
            internal TaskCompletionSource<NpgsqlConnector> TaskCompletionSource;
            internal bool IsAsync;
        }

        /// <summary>
        /// Incremented every time this pool is cleared via <see cref="NpgsqlConnection.ClearPool"/> or
        /// <see cref="NpgsqlConnection.ClearAllPools"/>. Allows us to identify connections which were
        /// created before the clear.
        /// </summary>
        int _clearCounter;

        [CanBeNull]
        Timer _pruningTimer;
        readonly TimeSpan _pruningInterval;
        readonly List<NpgsqlConnector> _prunedConnectors;

        #endregion

        internal ConnectorPool(NpgsqlConnectionStringBuilder csb)
        {
            _max = csb.MaxPoolSize;
            _min = csb.MinPoolSize;

            _connectionString = csb;
            _pruningInterval = TimeSpan.FromSeconds(_connectionString.ConnectionPruningInterval);
            _prunedConnectors = new List<NpgsqlConnector>();
            Idle = new IdleConnectorList();
            _waiting = new Queue<WaitingOpenAttempt>();
            Counters.NumberOfActiveConnectionPools.Increment();
        }

        void IncrementBusy()
        {
            Busy++;
            Counters.NumberOfActiveConnections.Increment();
        }

        void DecrementBusy()
        {
            Busy--;
            Counters.NumberOfActiveConnections.Decrement();
        }

        [RewriteAsync]
        internal NpgsqlConnector Allocate(NpgsqlConnection conn, NpgsqlTimeout timeout)
        {
            NpgsqlConnector connector;
            Monitor.Enter(this);

            while (Idle.Count > 0)
            {
                connector = Idle.Pop();
                // An idle connector could be broken because of a keepalive
                if (connector.IsBroken)
                    continue;
                connector.Connection = conn;
                IncrementBusy();
                EnsurePruningTimerState();
                Monitor.Exit(this);
                return connector;
            }

            Debug.Assert(Busy <= _max);
            if (Busy == _max)
            {
                // TODO: Async cancellation
                var tcs = new TaskCompletionSource<NpgsqlConnector>();
                EnqueueWaitingOpenAttempt(tcs);
                Monitor.Exit(this);
                try
                {
                    WaitForTask(tcs.Task, timeout.TimeLeft);
                }
                catch
                {
                    // We're here if the timeout expired or the cancellation token was triggered
                    // Re-lock and check in case the task was set to completed after coming out of the Wait
                    lock (this)
                    {
                        if (!tcs.Task.IsCompleted)
                        {
                            tcs.SetCanceled();
                            throw;
                        }
                    }
                }
                connector = tcs.Task.Result;
                connector.Connection = conn;
                return connector;
            }

            // No idle connectors are available, and we're under the pool's maximum capacity.
            IncrementBusy();
            Monitor.Exit(this);

            try
            {
                connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
                connector.Open(timeout);
                Counters.NumberOfPooledConnections.Increment();
                EnsureMinPoolSize(conn);
                return connector;
            }
            catch
            {
                lock (this)
                    DecrementBusy();
                throw;
            }
        }

        internal void Release(NpgsqlConnector connector)
        {
            // If Clear/ClearAll has been been called since this connector was first opened,
            // throw it away.
            if (connector.ClearCounter < _clearCounter)
            {
                try
                {
                    connector.Close();
                }
                catch (Exception e)
                {
                    Log.Logger.LogWarning(NpgsqlEventId.ExceptionClosingOutdatedConnector, e, "[{ConnectorId}] Exception while closing outdated connector", connector.Id);
                }

                lock (this)
                    DecrementBusy();
                Counters.SoftDisconnectsPerSecond.Increment();
                Counters.NumberOfPooledConnections.Decrement();
                return;
            }

            if (connector.IsBroken)
            {
                lock (this)
                    DecrementBusy();
                Counters.NumberOfPooledConnections.Decrement();
                return;
            }

            connector.Reset();
            lock (this)
            {
                // If there are any pending open attempts in progress hand the connector off to
                // them directly.
                while (_waiting.Count > 0)
                {
                    var waitingOpenAttempt = _waiting.Dequeue();
                    var tcs = waitingOpenAttempt.TaskCompletionSource;
                    // Some attempts may be in the queue but in cancelled state, since they've already timed out.
                    // Simply dequeue these and move on.
                    if (tcs.Task.IsCanceled)
                        continue;

                    // We have a pending open attempt. "Complete" it, handing off the connector.
                    if (waitingOpenAttempt.IsAsync)
                    {
                        // If the waiting open attempt is asynchronous (i.e. OpenAsync()), we can't simply
                        // call SetResult on its TaskCompletionSource, since it would execute the open's
                        // continuation in our thread (the closing thread). Instead we schedule the completion
                        // to run in the TP

                        // We copy tcs2 and especially connector2 to avoid allocations caused by the closure, see
                        // http://stackoverflow.com/questions/41507166/closure-heap-allocation-happening-at-start-of-method
                        var tcs2 = tcs;
                        var connector2 = connector;

                        Task.Run(() => tcs2.SetResult(connector2));
                    }
                    else
                        tcs.SetResult(connector);
                    return;
                }

                Idle.Push(connector);
                DecrementBusy();
                EnsurePruningTimerState();
                Debug.Assert(Idle.Count <= _max);
            }
        }

        /// <summary>
        /// Attempts to ensure, on a best-effort basis, that there are enough connections to meet MinPoolSize.
        /// This method never throws an exception.
        /// </summary>
        void EnsureMinPoolSize(NpgsqlConnection conn)
        {
            int missing;
            lock (this)
            {
                missing = _min - (Busy + Idle.Count);
                if (missing <= 0)
                    return;
                Busy += missing;
            }

            for (; missing > 0; missing--)
            {

                try
                {
#if NET45 || NET451
                    var connector = new NpgsqlConnector((NpgsqlConnection) ((ICloneable) conn).Clone())
#else
                    var connector = new NpgsqlConnector(conn.Clone())
#endif
                    {
                        ClearCounter = _clearCounter
                    };
                    connector.Open();
                    connector.Reset();
                    Counters.NumberOfPooledConnections.Increment();
                    lock (this)
                    {
                        Idle.Push(connector);
                        EnsurePruningTimerState();
                        Busy--;
                    }
                }
                catch (Exception e)
                {
                    lock (this)
                        Busy -= missing;
                    Log.Logger.LogWarning(NpgsqlEventId.ExceptionEnsuringMinPoolSize, e, "Connection error while attempting to ensure MinPoolSize");
                    return;
                }
            }
        }

        void EnsurePruningTimerState()
        {
            Debug.Assert(Monitor.IsEntered(this));

            if (Idle.Count + Busy <= _min)
            {
                if (_pruningTimer != null)
                {
                    _pruningTimer.Dispose();
                    _pruningTimer = null;
                }
            }
            else if (_pruningTimer == null)
                _pruningTimer = new Timer(PruneIdleConnectors, null, _pruningInterval, _pruningInterval);
        }

        void PruneIdleConnectors(object state)
        {
            if (Idle.Count + Busy <= _min)
                return;

            if (!Monitor.TryEnter(_prunedConnectors))
                return; // Pruning thread already running

            try
            {
                var idleLifetime = _connectionString.ConnectionIdleLifetime;
                lock (this)
                {
                    var totalConnections = Idle.Count + Busy;
                    int i;
                    for (i = 0; i < Idle.Count; i++)
                    {
                        var connector = Idle[i];
                        if (totalConnections - i <= _min || (DateTime.UtcNow - connector.ReleaseTimestamp).TotalSeconds < idleLifetime)
                            break;
                        _prunedConnectors.Add(connector);
                    }

                    if (i == 0)   // nothing to prune
                        return;

                    Idle.RemoveRange(0, i);
                    EnsurePruningTimerState();
                }

                foreach (var connector in _prunedConnectors)
                {
                    Counters.NumberOfPooledConnections.Decrement();
                    Counters.NumberOfFreeConnections.Decrement();
                    try { connector.Close(); }
                    catch (Exception e)
                    {
                        Log.Logger.LogWarning(NpgsqlEventId.ExceptionClosingPrunedConnector, e, "[{ConnectorId}] Exception while closing pruned connector", connector.Id);
                    }
                }

                _prunedConnectors.Clear();
            }
            finally
            {
                Monitor.Exit(_prunedConnectors);
            }
        }

        internal void Clear()
        {
            NpgsqlConnector[] idleConnectors;
            lock (this)
            {
                idleConnectors = Idle.ToArray();
                Idle.Clear();
                EnsurePruningTimerState();
            }

            foreach (var connector in idleConnectors)
            {
                Counters.NumberOfPooledConnections.Decrement();
                Counters.NumberOfFreeConnections.Decrement();
                try { connector.Close(); }
                catch (Exception e)
                {
                    Log.Logger.LogWarning(NpgsqlEventId.ExceptionClearingConnector, e, "[{ConnectorId}] Exception while closing connector during clear", connector.Id);
                }
            }
            _clearCounter++;
        }

        // This method (and its async counterpart below) are a hack to allow us to distinguish between whether
        // we're executing in sync or async code - this is necessary to properly set IsAsync on the
        // WaitingOpenAttempt. AsyncRewriter will automatically choose the right method.
        void EnqueueWaitingOpenAttempt(TaskCompletionSource<NpgsqlConnector> tcs)
        {
            _waiting.Enqueue(new WaitingOpenAttempt { TaskCompletionSource = tcs, IsAsync =  false});
        }

        // ReSharper disable once UnusedParameter.Local
        Task EnqueueWaitingOpenAttemptAsync(TaskCompletionSource<NpgsqlConnector> tcs, CancellationToken cancellationToken)
        {
            _waiting.Enqueue(new WaitingOpenAttempt { TaskCompletionSource = tcs, IsAsync = true });
            return PGUtil.CompletedTask;
        }

        void WaitForTask(Task task, TimeSpan timeout)
        {
            if (!task.Wait(timeout))
                throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {_connectionString.Timeout} seconds)");
        }

        async Task WaitForTaskAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var timeoutTask = Task.Delay(timeout, cancellationToken);
            if (task != await Task.WhenAny(task, timeoutTask))
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {_connectionString.Timeout} seconds)");
            }
        }

        #region Pending Enlisted Connections

#if NET45 || NET451
        internal void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                List<NpgsqlConnector> list;
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out list))
                    list = _pendingEnlistedConnectors[transaction] = new List<NpgsqlConnector>();
                list.Add(connector);
            }
        }

        internal void RemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
                _pendingEnlistedConnectors[transaction].Remove(connector);
        }

        [CanBeNull]
        internal NpgsqlConnector TryAllocateEnlistedPending(Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                List<NpgsqlConnector> list;
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out list))
                    return null;
                var connector = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return connector;
            }
        }

        // Note that while the dictionary is threadsafe, we assume that the lists it contains don't need to be
        // (i.e. access to connectors of a specific transaction won't be concurrent)
        readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
            = new Dictionary<Transaction, List<NpgsqlConnector>>();
#endif

        #endregion


        public void Dispose()
        {
            _pruningTimer?.Dispose();
        }

        public override string ToString() => $"[{Busy} busy, {Idle.Count} idle, {_waiting.Count} waiting]";
    }

    class IdleConnectorList : List<NpgsqlConnector>
    {
        internal void Push(NpgsqlConnector connector)
        {
            connector.ReleaseTimestamp = DateTime.UtcNow;
            Add(connector);
            Counters.NumberOfFreeConnections.Increment();
        }

        internal NpgsqlConnector Pop()
        {
            var connector = this[Count - 1];
            connector.ReleaseTimestamp = DateTime.UtcNow;
            RemoveAt(Count - 1);
            Counters.NumberOfFreeConnections.Decrement();
            return connector;
        }
    }
}
