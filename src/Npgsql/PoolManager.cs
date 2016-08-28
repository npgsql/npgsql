using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using Npgsql.Logging;

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

        internal static ConnectorPool Get(NpgsqlConnectionStringBuilder connString)
        {
            Debug.Assert(connString != null);

            return Pools[connString];
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

        internal NpgsqlConnectionStringBuilder ConnectionString;

        /// <summary>
        /// Open connectors waiting to be requested by new connections
        /// </summary>
        internal IdleConnectorList Idle;

        readonly int _max;
        internal int Min { get; }
        internal int Busy { get; private set; }

        Queue<WaitingOpenAttempt> Waiting;

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

        Timer _pruningTimer;
        readonly TimeSpan _pruningInterval;
        readonly List<NpgsqlConnector> _prunedConnectors;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        internal ConnectorPool(NpgsqlConnectionStringBuilder csb)
        {
            _max = csb.MaxPoolSize;
            Min = csb.MinPoolSize;

            ConnectionString = csb;
            _pruningInterval = TimeSpan.FromSeconds(ConnectionString.ConnectionPruningInterval);
            _prunedConnectors = new List<NpgsqlConnector>();
            Idle = new IdleConnectorList();
            Waiting = new Queue<WaitingOpenAttempt>();
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
                    Log.Warn("Exception while closing outdated connector", e, connector.Id);
                }

                lock (this)
                    DecrementBusy();
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
                while (Waiting.Count > 0)
                {
                    var waitingOpenAttempt = Waiting.Dequeue();
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
                        Task.Run(() => tcs.SetResult(connector));
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
                missing = Min - (Busy + Idle.Count);
                if (missing <= 0)
                    return;
                Busy += missing;
            }

            for (; missing > 0; missing--)
            {

                try
                {
#if NET451 || NET45
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
                    Log.Warn("Connection error while attempting to ensure MinPoolSize", e);
                    return;
                }
            }
        }

        void EnsurePruningTimerState()
        {
            Debug.Assert(Monitor.IsEntered(this));

            if (Idle.Count + Busy <= Min)
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

        internal void PruneIdleConnectors(object state)
        {
            if (Idle.Count + Busy <= Min)
                return;

            if (!Monitor.TryEnter(_prunedConnectors))
                return; // Pruning thread already running

            try
            {
                var idleLifetime = ConnectionString.ConnectionIdleLifetime;
                lock (this)
                {
                    var totalConnections = Idle.Count + Busy;
                    int i;
                    for (i = 0; i < Idle.Count; i++)
                    {
                        var connector = Idle[i];
                        if (totalConnections - i <= Min || (DateTime.UtcNow - connector.ReleaseTimestamp).TotalSeconds < idleLifetime)
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
                        Log.Warn("Exception while closing pruned connector", e, connector.Id);
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
                    Log.Warn("Exception while closing connector during clear", e, connector.Id);
                }
            }
            _clearCounter++;
        }

        // This method (and its async counterpart below) are a hack to allow us to distinguish between whether
        // we're executing in sync or async code - this is necessary to properly set IsAsync on the
        // WaitingOpenAttempt. AsyncRewriter will automatically choose the right method.
        void EnqueueWaitingOpenAttempt(TaskCompletionSource<NpgsqlConnector> tcs)
        {
            Waiting.Enqueue(new WaitingOpenAttempt { TaskCompletionSource = tcs, IsAsync =  false});
        }

        Task EnqueueWaitingOpenAttemptAsync(TaskCompletionSource<NpgsqlConnector> tcs, CancellationToken cancellationToken)
        {
            Waiting.Enqueue(new WaitingOpenAttempt { TaskCompletionSource = tcs, IsAsync = true });
            return PGUtil.CompletedTask;
        }

        void WaitForTask(Task task, TimeSpan timeout)
        {
            if (!task.Wait(timeout))
                throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {ConnectionString.Timeout} seconds)");
        }

        async Task WaitForTaskAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var timeoutTask = Task.Delay(timeout, cancellationToken);
            if (task != await Task.WhenAny(task, timeoutTask).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {ConnectionString.Timeout} seconds)");
            }
        }

        public void Dispose()
        {
            _pruningTimer?.Dispose();
        }

        public override string ToString() => $"[{Busy} busy, {Idle.Count} idle, {Waiting.Count} waiting]";
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
