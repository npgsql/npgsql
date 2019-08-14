using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql.Logging;
using Npgsql.Util;

namespace Npgsql
{
    /// <summary>
    /// Connection pool for PostgreSQL physical connections. Attempts to allocate connections over MaxPoolSize will
    /// block until someone releases. Implementation is completely lock-free to avoid contention, and ensure FIFO
    /// for open attempts waiting (because the pool is at capacity).
    /// </summary>
    sealed class ConnectorPool : IDisposable
    {
        #region Implementation notes

        // General
        //
        // * When we're at capacity (Busy==Max) further open attempts wait until someone releases.
        //   This must happen in FIFO (first to block on open is the first to release), otherwise some attempts may get
        //   starved and time out. This is why we use a ConcurrentQueue.
        // * We must avoid a race condition whereby an open attempt starts waiting at the same time as another release
        //   puts a connector back into the idle list. This would potentially make the waiter wait forever/time out.
        //
        // Rules
        // * You *only* create a new connector if Total < Max.
        // * You *only* go into waiting if Busy == Max (which also implies Idle == 0)

        #endregion Implementation notes

        #region Fields

        internal NpgsqlConnectionStringBuilder Settings { get; }

        /// <summary>
        /// Contains the connection string returned to the user from <see cref="NpgsqlConnection.ConnectionString"/>
        /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
        /// </summary>
        internal string UserFacingConnectionString { get; }

        readonly int _max;
        readonly int _min;

        readonly NpgsqlConnector?[] _idle;

        readonly ConcurrentQueue<(TaskCompletionSource<NpgsqlConnector?> TaskCompletionSource, bool IsAsync)> _waiting;

        [StructLayout(LayoutKind.Explicit)]
        internal struct PoolState
        {
            [FieldOffset(0)]
            internal short Idle;
            [FieldOffset(2)]
            internal short Busy;
            [FieldOffset(4)]
            internal int Waiting;
            [FieldOffset(0)]
            internal long All;

            internal int Total => Idle + Busy;

            internal PoolState Copy() => new PoolState { All = Volatile.Read(ref All) };

            public override string ToString()
            {
                var state = Copy();
                return $"[{state.Total} total, {state.Idle} idle, {state.Busy} busy, {state.Waiting} waiting]";
            }
        }

        internal PoolState State;

        /// <summary>
        /// Incremented every time this pool is cleared via <see cref="NpgsqlConnection.ClearPool"/> or
        /// <see cref="NpgsqlConnection.ClearAllPools"/>. Allows us to identify connections which were
        /// created before the clear.
        /// </summary>
        int _clearCounter;

        static readonly TimerCallback PruningTimerCallback = PruneIdleConnectors;
        Timer? _pruningTimer;
        readonly TimeSpan _pruningInterval;

        /// <summary>
        /// Maximum number of possible connections in any pool.
        /// </summary>
        internal const int PoolSizeLimit = 1024;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ConnectorPool));

        #endregion

        internal ConnectorPool(NpgsqlConnectionStringBuilder settings, string connString)
        {
            Debug.Assert(PoolSizeLimit <= short.MaxValue,
                "PoolSizeLimit cannot be larger than short.MaxValue unless PoolState is refactored to hold larger values.");

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
            _waiting = new ConcurrentQueue<(TaskCompletionSource<NpgsqlConnector?> TaskCompletionSource, bool IsAsync)>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryAllocateFast(NpgsqlConnection conn, [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            Counters.SoftConnectsPerSecond.Increment();

            // We start scanning for an idle connector in "random" places in the array, to avoid
            // too much interlocked operations "contention" at the beginning.
            var start = Thread.CurrentThread.ManagedThreadId % _max;

            // Idle may indicate that there are idle connectors, with the subsequent scan failing to find any.
            // This can happen because of race conditions with Release(), which updates Idle before actually putting
            // the connector in the list, or because of other allocation attempts, which remove the connector from
            // the idle list before updating Idle.
            // Loop until either State.Idle is 0 or you manage to remove a connector.
            connector = null;
            while (Volatile.Read(ref State.Idle) > 0)
            {
                for (var i = start; connector == null && i < _max; i++)
                {
                    // First check without an Interlocked operation, it's faster
                    if (_idle[i] == null)
                        continue;

                    // If we saw a connector in this slot, atomically exchange it with a null.
                    // Either we get a connector out which we can use, or we get null because
                    // someone has taken it in the meanwhile. Either way put a null in its place.
                    connector = Interlocked.Exchange(ref _idle[i], null);
                }

                for (var i = 0; connector == null && i < start; i++)
                {
                    // Same as above
                    if (_idle[i] == null)
                        continue;
                    connector = Interlocked.Exchange(ref _idle[i], null);
                }

                if (connector == null)
                    return false;

                Counters.NumberOfFreeConnections.Decrement();

                // An connector could be broken because of a keepalive that occurred while it was
                // idling in the pool
                // TODO: Consider removing the pool from the keepalive code. The following branch is simply irrelevant
                // if keepalive isn't turned on.
                if (connector.IsBroken)
                {
                    CloseConnector(connector, true);
                    continue;
                }

                connector.Connection = conn;

                // We successfully extracted an idle connector, update state
                Counters.NumberOfActiveConnections.Increment();
                var sw = new SpinWait();
                while (true)
                {
                    var state = State.Copy();
                    var newState = state;
                    newState.Busy++;
                    newState.Idle--;
                    CheckInvariants(newState);
                    if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) == state.All)
                        return true;
                    sw.SpinOnce();
                }
            }

            connector = null;
            return false;
        }

        internal async ValueTask<NpgsqlConnector> AllocateLong(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            // No idle connector was found in the pool.
            // We now loop until one of three things happen:
            // 1. The pool isn't at max capacity (Total < Max), so we can create a new physical connection.
            // 2. The pool is at maximum capacity and there are no idle connectors (Busy == Max),
            // so we enqueue an open attempt into the waiting queue, so that the next release will unblock it.
            // 3. An connector makes it into the idle list (race condition with another Release().
            while (true)
            {
                NpgsqlConnector? connector;
                var state = State.Copy();
                var newState = state;

                if (state.Total < _max)
                {
                    // We're under the pool's max capacity, try to "allocate" a slot for a new physical connection.
                    newState.Busy++;
                    CheckInvariants(newState);
                    if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) != state.All)
                    {
                        // Our attempt to increment the busy count failed, Loop again and retry.
                        continue;
                    }

                    try
                    {
                        // We've managed to increase the busy counter, open a physical connections
                        connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
                        await connector.Open(timeout, async, cancellationToken);
                    }
                    catch
                    {
                        // Physical open failed, decrement busy back down
                        conn.Connector = null;

                        var sw = new SpinWait();
                        while (true)
                        {
                            state = State.Copy();
                            newState = state;
                            newState.Busy--;
                            if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) != state.All)
                            {
                                // Our attempt to increment the busy count failed, Loop again and retry.
                                sw.SpinOnce();
                                continue;
                            }

                            break;
                        }

                        // There may be waiters because we raised the busy count (and failed). Release one
                        // waiter if there is one.
                        if (_waiting.TryDequeue(out var waitingOpenAttempt))
                        {
                            if (!waitingOpenAttempt.TaskCompletionSource.TrySetResult(null))
                            {
                                // TODO: Release more??
                            }
                        }

                        throw;
                    }

                    Counters.NumberOfActiveConnections.Increment();
                    Counters.NumberOfPooledConnections.Increment();

                    // Start the pruning timer if we're above MinPoolSize
                    if (_pruningTimer == null && newState.Total > _min)
                    {
                        var newPruningTimer = new Timer(PruningTimerCallback, this, -1, -1);
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

                if (state.Busy == _max)
                {
                    // Pool is exhausted. Increase the waiting count while atomically making sure the busy count
                    // doesn't decrease (otherwise we have a new idle connector).
                    checked
                    {
                        newState.Waiting++;
                    }

                    CheckInvariants(newState);
                    if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) != state.All)
                    {
                        // Our attempt to increment the waiting count failed, either because a connector became idle (busy
                        // changed) or the waiting count changed. Loop again and retry.
                        continue;
                    }

                    // At this point the waiting count is non-zero, so new release calls are blocking on the waiting
                    // queue. This avoids a race condition where we wait while another connector is put back in the
                    // idle list - we know the idle list is empty and will stay empty.

                    try
                    {
                        // Enqueue an open attempt into the waiting queue so that the next release attempt will unblock us.
                        var tcs = new TaskCompletionSource<NpgsqlConnector?>(TaskCreationOptions.RunContinuationsAsynchronously);
                        _waiting.Enqueue((tcs, async));

                        try
                        {
                            if (async)
                            {
                                if (timeout.IsSet)
                                {
                                    // Use Task.Delay to implement the timeout, but cancel the timer if we actually
                                    // do complete successfully
                                    var delayCancellationToken = new CancellationTokenSource();
                                    using (cancellationToken.Register(s => ((CancellationTokenSource)s!).Cancel(), delayCancellationToken))
                                    {
                                        var timeLeft = timeout.TimeLeft;
                                        if (timeLeft <= TimeSpan.Zero ||
                                            await Task.WhenAny(tcs.Task, Task.Delay(timeLeft, delayCancellationToken.Token)) != tcs.Task)
                                        {
                                            // Delay task completed first, either because of a user cancellation or an actual timeout
                                            cancellationToken.ThrowIfCancellationRequested();
                                            throw new NpgsqlException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {Settings.Timeout} seconds)");
                                        }
                                    }
                                    delayCancellationToken.Cancel();
                                }
                                else
                                {
                                    using (cancellationToken.Register(s => ((TaskCompletionSource<NpgsqlConnector?>)s!).SetCanceled(), tcs))
                                        await tcs.Task;
                                }
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
                            tcs.TrySetCanceled();

                            // There's still a chance of a race condition, whereby the task was transitioned to
                            // completed in the meantime.
                            if (tcs.Task.Status != TaskStatus.RanToCompletion)
                                throw;
                        }

                        Debug.Assert(tcs.Task.IsCompleted);
                        connector = tcs.Task.Result;

                        // Our task completion may contain a null in order to unblock us, allowing us to try
                        // allocating again.
                        if (connector == null)
                            continue;

                        // Note that we don't update counters or any state since the connector is being
                        // handed off from one open connection to another.
                        connector.Connection = conn;
                        return connector;
                    }
                    finally
                    {
                        // The allocation attempt succeeded or timed out, decrement the waiting count
                        var sw = new SpinWait();
                        while (true)
                        {
                            state = State.Copy();
                            newState = state;
                            newState.Waiting--;
                            CheckInvariants(newState);
                            if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) == state.All)
                                break;
                            sw.SpinOnce();
                        }
                    }
                }

                // We didn't create a new connector or start waiting, which means there's a new idle connector, try
                // getting it
                Debug.Assert(state.Idle > 0);
                if (TryAllocateFast(conn, out connector))
                    return connector;
            }

            // Cannot be here
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
                CloseConnector(connector, false);
                return;
            }

            connector.Reset();

            var sw = new SpinWait();

            while (true)
            {
                var state = State.Copy();

                // If there are any pending open attempts in progress hand the connector off to them directly.
                // Note that in this case, state changes (i.e. decrementing State.Waiting) happens at the allocating
                // side.
                if (state.Waiting > 0)
                {
                    if (!_waiting.TryDequeue(out var waitingOpenAttempt))
                    {
                        // _waitingCount has been increased, but there's nothing in the queue yet - someone is in the
                        // process of enqueuing an open attempt. Wait and retry.
                        sw.SpinOnce();
                        continue;
                    }

                    var tcs = waitingOpenAttempt.TaskCompletionSource;

                    // We have a pending open attempt. "Complete" it, handing off the connector.
                    if (!tcs.TrySetResult(connector))
                    {
                        // If the open attempt timed out, the Task's state will be set to Canceled and our
                        // TrySetResult fails. Try again.
                        Debug.Assert(tcs.Task.IsCanceled);
                        continue;
                    }

                    return;
                }

                // There were no waiting attempts. However, there's a race condition where a new waiting attempt
                // may occur as we're putting our connector into the idle list. Decrement the busy
                // count, while atomically make sure the waiting count isn't increased.
                // Note that we also must update the state *before* putting the connector back in the idle list.
                var newState = state;
                newState.Idle++;
                newState.Busy--;
                CheckInvariants(newState);
                if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) != state.All)
                {
                    // Our attempt to decrement the busy count failed, either because a waiting attempt has been added
                    // or busy has changed. Loop again and retry.
                    continue;
                }

                // If we're here, we successfully applied the new state above and can put the connector back in the idle
                // list (there were no pending open attempts).

                connector.ReleaseTimestamp = DateTime.UtcNow;

                // We start scanning for an empty slot in "random" places in the array, to avoid
                // too much interlocked operations "contention" at the beginning.
                var start = Thread.CurrentThread.ManagedThreadId % _max;

                sw = new SpinWait();
                while (true)
                {
                    for (var i = start; i < _idle.Length; i++)
                    {
                        if (Interlocked.CompareExchange(ref _idle[i], connector, null) == null)
                        {
                            Counters.NumberOfFreeConnections.Increment();
                            return;
                        }
                    }

                    for (var i = 0; i < start; i++)
                    {
                        if (Interlocked.CompareExchange(ref _idle[i], connector, null) == null)
                        {
                            Counters.NumberOfFreeConnections.Increment();
                            return;
                        }
                    }
                    sw.SpinOnce();
                }
            }
        }

        void CloseConnector(NpgsqlConnector connector, bool wasIdle)
        {
            try
            {
                connector.Close();

                var sw = new SpinWait();
                while (true)
                {
                    var state = State.Copy();
                    var newState = state;
                    if (wasIdle)
                        newState.Idle--;
                    else
                        newState.Busy--;
                    CheckInvariants(newState);
                    if (Interlocked.CompareExchange(ref State.All, newState.All, state.All) == state.All)
                        break;
                    sw.SpinOnce();
                }
            }
            catch (Exception e)
            {
                Log.Warn("Exception while closing outdated connector", e, connector.Id);
            }

            Counters.NumberOfPooledConnections.Decrement();

            while (_pruningTimer != null && State.Total <= _min)
            {
                var oldTimer = _pruningTimer;
                if (Interlocked.CompareExchange(ref _pruningTimer, null, oldTimer) == oldTimer)
                {
                    oldTimer.Dispose();
                    break;
                }
            }
        }

        static void PruneIdleConnectors(object? state)
        {
            var pool = (ConnectorPool)state!;
            var idle = pool._idle;
            var now = DateTime.UtcNow;
            var idleLifetime = pool.Settings.ConnectionIdleLifetime;

            for (var i = 0; i < idle.Length; i++)
            {
                if (pool.State.Total <= pool._min)
                    return;

                var connector = idle[i];
                if (connector == null || (now - connector.ReleaseTimestamp).TotalSeconds < idleLifetime)
                    continue;
                if (Interlocked.CompareExchange(ref idle[i], null, connector) == connector)
                    pool.CloseConnector(connector, true);
            }
        }

        internal void Clear()
        {
            for (var i = 0; i < _idle.Length; i++)
            {
                var connector = Interlocked.Exchange(ref _idle[i], null);
                if (connector != null)
                    CloseConnector(connector, true);
            }

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

        internal NpgsqlConnector? TryAllocateEnlistedPending(Transaction transaction)
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

        // Note that while the dictionary is thread-safe, we assume that the lists it contains don't need to be
        // (i.e. access to connectors of a specific transaction won't be concurrent)
        readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
            = new Dictionary<Transaction, List<NpgsqlConnector>>();

        #endregion

        #region Misc

        [Conditional("DEBUG")]
        void CheckInvariants(PoolState state)
        {
            if (state.Total > _max)
                throw new NpgsqlException($"Pool is over capacity (Total={state.Total}, Max={_max})");
            if (state.Waiting > 0 && state.Idle > 0)
                throw new NpgsqlException($"Can't have waiters ({state.Waiting}) while there are idle connections ({state.Idle}");
            if (state.Idle < 0)
                throw new NpgsqlException("Idle is negative");
            if (state.Busy < 0)
                throw new NpgsqlException("Busy is negative");
            if (state.Waiting < 0)
                throw new NpgsqlException("Waiting is negative");
        }

        public void Dispose() => _pruningTimer?.Dispose();

        public override string ToString() => State.ToString();

        #endregion Misc
    }
}
