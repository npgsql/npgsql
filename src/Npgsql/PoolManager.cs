using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using System.Transactions;

namespace Npgsql
{
    static class PoolManager
    {
        /// <summary>
        /// Holds connector pools indexed by their connection strings.
        /// </summary>
        internal static ConcurrentDictionary<string, ConnectorPool> Pools { get; }
            = new ConcurrentDictionary<string, ConnectorPool>();

        /// <summary>
        /// Maximum number of possible connections in the pool.
        /// </summary>
        internal const int PoolSizeLimit = 1024;

        static PoolManager()
        {
            // When the appdomain gets unloaded (e.g. web app redeployment) attempt to nicely
            // close idle connectors to prevent errors in PostgreSQL logs (#491).
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => ClearAll();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => ClearAll();
        }

        internal static void Clear(string connString)
        {
            Debug.Assert(connString != null);

            if (Pools.TryGetValue(connString, out var pool))
                pool.Clear();
        }

        internal static void ClearAll()
        {
            foreach (var kvp in Pools)
                kvp.Value.Clear();
        }
    }

    /// <summary>
    /// Connection pool for PostgreSQL physical connections. Implementation is completely lock-free
    /// to avoid contention.
    /// </summary>
    /// <remarks>
    /// When the number of physical connections reaches MaxPoolSize, further attempts to allocate
    /// connections will block until an existing connection is released. If multiple waiters
    /// exist, they will receive connections in FIFO manner to ensure fairness and prevent old waiters
    /// to time out.
    /// </remarks>
    sealed class ConnectorPool : IDisposable
    {
        #region Fields

        internal NpgsqlConnectionStringBuilder Settings { get; }

        /// <summary>
        /// Contains the connection string returned to the user from <see cref="NpgsqlConnection.ConnectionString"/>
        /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
        /// </summary>
        internal string UserFacingConnectionString { get; }

        readonly int _max;
        readonly int _min;

        /// <summary>
        /// The total number of physical connections in existence, whether idle or busy out of the pool.
        /// </summary>
        /// <remarks>
        /// Internal for tests only
        /// </remarks>
        internal int Total;

        [ItemCanBeNull]
        readonly NpgsqlConnector[] _idle;

        readonly ConcurrentQueue<WaitingOpenAttempt> _waiting;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        internal ConnectorPool(NpgsqlConnectionStringBuilder settings, string connString)
        {
            if (settings.MaxPoolSize < settings.MinPoolSize)
                throw new ArgumentException($"Connection can't have MaxPoolSize {settings.MaxPoolSize} under MinPoolSize {settings.MinPoolSize}");

            Settings = settings;

            _max = settings.MaxPoolSize;
            _min = settings.MinPoolSize;

            UserFacingConnectionString = settings.PersistSecurityInfo
                ? connString
                : settings.ToStringWithoutPassword();

            _pruningInterval = TimeSpan.FromSeconds(Settings.ConnectionPruningInterval);
            _idle = new NpgsqlConnector[_max];
            _waiting = new ConcurrentQueue<WaitingOpenAttempt>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ValueTask<NpgsqlConnector> Allocate(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            Counters.SoftConnectsPerSecond.Increment();

            NpgsqlConnector connector;
            var len = _idle.Length;
            // We start scanning for an idle connector in "random" places in the array, to avoid
            // too much interlocked operations "contention" at the beginning.
            var start = Thread.CurrentThread.ManagedThreadId % len;
            for (var i = start; i < len; i++)
            {
                connector = GetIdleConnector(i, conn);
                if (connector != null)
                    return new ValueTask<NpgsqlConnector>(connector);
            }

            // We got to the end of the array but started in the middle, complete from the beginning.
            for (var i = 0; i < start; i++)
            {
                connector = GetIdleConnector(i, conn);
                if (connector != null)
                    return new ValueTask<NpgsqlConnector>(connector);
            }

            // No idle connector was found in the pool, we have to either create a new physical
            // connection or block if MaxPoolSize has been reached.
            while (true)
            {
                var oldTotal = Total;
                if (oldTotal >= _max)   // Pool is exhausted, wait for a close
                    return new ValueTask<NpgsqlConnector>(WaitForConnector(timeout, conn, async));
                if (Interlocked.CompareExchange(ref Total, oldTotal + 1, oldTotal) == oldTotal)
                {
                    // We increased the total number of physical connections and are still under
                    // MaxPoolSize, so we can create a new physical connection and return it.
                    return new ValueTask<NpgsqlConnector>(OpenConnector(conn, timeout, async, cancellationToken));
                }
            }

            // Cannot be here
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), CanBeNull]
        NpgsqlConnector GetIdleConnector(int index, NpgsqlConnection conn)
        {
            // First check without an Interlocked operation, it's faster
            if (_idle[index] == null)
                return null;

            // If we saw a connector in this slot, atomically exchange it with a null.
            // Either we get a connector out which we can use, or we get null because
            // someone has taken it in the meanwhile. Either way put a null in its place.
            var connector = Interlocked.Exchange(ref _idle[index], null);
            if (connector == null)
                return null;

            Counters.NumberOfFreeConnections.Decrement();

            // An connector could be broken because of a keepalive that occurred while it was
            // idling in the pool
            if (connector.IsBroken)
            {
                CloseConnector(connector);
                return null;
            }

            connector.Connection = conn;
            Counters.NumberOfActiveConnections.Increment();
            return connector;
        }

        /// <summary>
        /// Opens a new physical connection (connector).
        /// </summary>
        /// <remarks>
        /// This method is distinct from <see cref="Allocate"/> because this method is async (slow)
        /// whereas <see cref="Allocate"/> implements the fast path (isn't async).
        /// </remarks>
        async Task<NpgsqlConnector> OpenConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
            try
            {
                await connector.Open(timeout, async, cancellationToken);
            }
            catch
            {
                // Total has already been incremented outside the function, decrement it back
                Interlocked.Decrement(ref Total);
                throw;
            }

            Counters.NumberOfActiveConnections.Increment();
            Counters.NumberOfPooledConnections.Increment();

            // Start the pruning timer if we're above MinPoolSize
            if (_pruningTimer == null && Total > _min)
            {
                var newPruningTimer = new Timer(PruneIdleConnectors);
                if (Interlocked.CompareExchange(ref _pruningTimer, newPruningTimer, null) == null)
                    newPruningTimer.Change(_pruningInterval, _pruningInterval);
                else
                {
                    // Someone beat us to it
                    newPruningTimer.Dispose();
                }
            }

            return connector;
        }

        async Task<NpgsqlConnector> WaitForConnector(NpgsqlTimeout timeout, NpgsqlConnection conn, bool async)
        {
            // TODO: Async cancellation
            var tcs = new TaskCompletionSource<NpgsqlConnector>();
            _waiting.Enqueue(new WaitingOpenAttempt { TaskCompletionSource = tcs, IsAsync = async });

            try
            {
                if (async)
                {
                    if (timeout.IsSet)
                    {
                        var timeLeft = timeout.TimeLeft;
                        if (timeLeft <= TimeSpan.Zero || tcs.Task != await Task.WhenAny(tcs.Task, Task.Delay(timeLeft)))
                            throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {Settings.Timeout} seconds)");
                    }
                    else
                        await tcs.Task;
                }
                else
                {
                    if (timeout.IsSet)
                    {
                        var timeLeft = timeout.TimeLeft;
                        if (timeLeft <= TimeSpan.Zero || !tcs.Task.Wait(timeLeft))
                            throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {Settings.Timeout} seconds)");
                    }
                    else
                        tcs.Task.Wait();
                }
            }
            catch
            {
                // We're here if the timeout expired or the cancellation token was triggered.
                // Transition our Task to cancelled, so that the next time someone releases
                // a connection they'll skip over it.
                if (tcs.TrySetCanceled())
                    throw;
                // If we've failed to cancel, someone has released a connection in the meantime
                // and we're good to go.
            }

            Debug.Assert(tcs.Task.IsCompleted);
            var connector = tcs.Task.Result;
            // Note that we don't update counters or any state since the connector is being
            // handed off from one open connection to another.
            connector.Connection = conn;
            return connector;
        }

        internal void Release(NpgsqlConnector connector)
        {
            Counters.SoftDisconnectsPerSecond.Increment();
            Counters.NumberOfActiveConnections.Decrement();

            // If Clear/ClearAll has been been called since this connector was first opened,
            // throw it away. The same if it's broken (in which case CloseConnector is only
            // used to update state/perf counter).
            if (connector.ClearCounter < _clearCounter || connector.IsBroken)
            {
                CloseConnector(connector);
                return;
            }

            connector.Reset();

            // If there are any pending open attempts in progress hand the connector off to
            // them directly.
            // TODO: Check the efficiency, we may be able to improve by tracking the number
            // of waiters in our own int, or removing the IsEmpty check and just trying to dequeue
            while (!_waiting.IsEmpty && _waiting.TryDequeue(out var waitingOpenAttempt))
            {
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

                    Task.Run(() =>
                    {
                        if (!tcs2.TrySetResult(connector2))
                        {
                            // Race condition: the waiter timed out between our IsCanceled check above and here
                            // Recursively call Release again, this will dequeue another open attempt and retry.
                            Debug.Assert(tcs2.Task.IsCanceled);
                            Release(connector2);
                        }
                    });
                }
                else if (!tcs.TrySetResult(connector))
                {
                    // Race condition: the waiter timed out between our IsCanceled check above and here
                    // Recursively call Release again, this will dequeue another open attempt and retry.
                    Debug.Assert(tcs.Task.IsCanceled);
                    continue;
                }

                return;
            }

            // There were no pending open attempts, simply place the connector back in the idle list
            for (var i = 0; i < _idle.Length; i++)
            {
                if (Interlocked.CompareExchange(ref _idle[i], connector, null) == null)
                {
                    Counters.NumberOfFreeConnections.Increment();
                    connector.ReleaseTimestamp = DateTime.UtcNow;
                    return;
                }
            }

            // Should not be here
            Log.Error("The idle list was full when releasing, there are more than MaxPoolSize connectors! Please file an issue.");
            CloseConnector(connector);
        }

        void CloseConnector(NpgsqlConnector connector)
        {
            try
            {
                connector.Close();
            }
            catch (Exception e)
            {
                Log.Warn("Exception while closing outdated connector", e, connector.Id);
            }

            Interlocked.Decrement(ref Total);
            Counters.NumberOfPooledConnections.Decrement();

            while (_pruningTimer != null && Total <= _min)
            {
                var oldTimer = _pruningTimer;
                if (Interlocked.CompareExchange(ref _pruningTimer, null, oldTimer) == oldTimer)
                {
                    oldTimer.Dispose();
                    break;
                }
            }
        }

#pragma warning disable CA1801 // Review unused parameters
        void PruneIdleConnectors(object state)
#pragma warning restore CA1801 // Review unused parameters
        {
            var now = DateTime.UtcNow;
            var idleLifetime = Settings.ConnectionIdleLifetime;

            for (var i = 0; i < _idle.Length; i++)
            {
                if (Total <= _min)
                    return;

                var connector = _idle[i];
                if (connector == null || (now - connector.ReleaseTimestamp).TotalSeconds < idleLifetime)
                    continue;
                if (Interlocked.CompareExchange(ref _idle[i], null, connector) == connector)
                    CloseConnector(connector);
            }
        }

        internal void Clear()
        {
            var toClose = new List<NpgsqlConnector>(_max);

            for (var i = 0; i < _idle.Length; i++)
            {
                var connector = Interlocked.Exchange(ref _idle[i], null);
                if (connector != null)
                {
                    toClose.Add(connector);
                }
            }

            foreach (var connector in toClose)
                CloseConnector(connector);

            _clearCounter++;
        }

        #region Pending Enlisted Connections

        internal void AddPendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                    list = _pendingEnlistedConnectors[transaction] = new List<NpgsqlConnector>();
                list.Add(connector);
            }
        }

        internal void TryRemovePendingEnlistedConnector(NpgsqlConnector connector, Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                    return;
                list.Remove(connector);
                if (list.Count == 0)
                    _pendingEnlistedConnectors.Remove(transaction);
            }
        }

        [CanBeNull]
        internal NpgsqlConnector TryAllocateEnlistedPending(Transaction transaction)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                    return null;
                var connector = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                if (list.Count == 0)
                    _pendingEnlistedConnectors.Remove(transaction);
                return connector;
            }
        }

        // Note that while the dictionary is threadsafe, we assume that the lists it contains don't need to be
        // (i.e. access to connectors of a specific transaction won't be concurrent)
        readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
            = new Dictionary<Transaction, List<NpgsqlConnector>>();

        #endregion

        public void Dispose() => _pruningTimer?.Dispose();

        public override string ToString() => $"[{Total} total, ${_idle.Count(i => i != null)} idle, {_waiting.Count} waiting]";

        /// <summary>
        /// Returns the number of idle connector in the pool, for testing purposes only.
        /// </summary>
        internal int IdleCount => _idle.Count(i => i != null);
    }
}
