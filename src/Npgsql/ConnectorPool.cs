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
        readonly NpgsqlConnector?[] _open;

        readonly ConcurrentQueue<(TaskCompletionSource<NpgsqlConnector?> TaskCompletionSource, bool IsAsync)> _waiting;

        [StructLayout(LayoutKind.Explicit)]
        internal struct PoolState
        {
            [FieldOffset(0)] public int Open;
            [FieldOffset(4)] public int Idle;

            [FieldOffset(0)] public long All;

            // Busy can actually be read and written non atomically, it would introduce a benign race
            // between readers of Busy and the writer(s), connector Close, when Idle is close to zero.
            // The writer would first decrement Open then Idle to prevent readers racing and concluding Busy == _max.
            // However with that order a race of the Idle read and decrement could happen, having readers read and
            // conclude Idle > 0, causing readers to loop for a non existent connector until Idle is also decremented.
            public void Deconstruct(out int open, out int idle, out int busy)
            {
                var copy = new PoolState { All = Volatile.Read(ref All) };
                open = copy.Open;
                idle = copy.Idle;
                busy = copy.Open - copy.Idle;
            }
        }

        // Mutable struct, do not make this readonly.
        PoolState State;

        internal (int Open, int Idle, int Busy, int Waiters) Statistics {
            get
            {
                var (open, idle, busy) = State;
                return (open, idle, busy, _waiting.Count);
            }
        }

        /// <summary>
        /// Incremented every time this pool is cleared via <see cref="NpgsqlConnection.ClearPool"/> or
        /// <see cref="NpgsqlConnection.ClearAllPools"/>. Allows us to identify connections which were
        /// created before the clear.
        /// </summary>
        int _clearCounter;

        // TODO move all this out of the pool
        static readonly TimerCallback PruningTimerCallback = PruneIdleConnectors;
        readonly Timer _pruningTimer;
        readonly TimeSpan _pruningSamplingInterval;
        readonly int _pruningSampleSize;
        readonly int[] _pruningSamples;
        readonly int _pruningMedianIndex;
        volatile bool _pruningTimerEnabled;
        int _pruningSampleIndex;

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

            if (settings.ConnectionPruningInterval == 0)
                throw new ArgumentException("ConnectionPruningInterval can't be 0.");
            var connectionIdleLifetime = TimeSpan.FromSeconds(settings.ConnectionIdleLifetime);
            var pruningSamplingInterval = TimeSpan.FromSeconds(settings.ConnectionPruningInterval);
            if (connectionIdleLifetime < pruningSamplingInterval)
                throw new ArgumentException($"Connection can't have ConnectionIdleLifetime {connectionIdleLifetime} under ConnectionPruningInterval {_pruningSamplingInterval}");

            _pruningTimer = new Timer(PruningTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
            _pruningSampleSize = DivideRoundingUp(settings.ConnectionIdleLifetime, settings.ConnectionPruningInterval);
            _pruningMedianIndex = DivideRoundingUp(_pruningSampleSize, 2) - 1; // - 1 to go from length to index
            _pruningSamplingInterval = pruningSamplingInterval;
            _pruningSamples = new int[_pruningSampleSize];
            _pruningTimerEnabled = false;

            _max = settings.MaxPoolSize;
            _min = settings.MinPoolSize;
            _idle = new NpgsqlConnector[_max];
            _open = new NpgsqlConnector[_max];
            _waiting = new ConcurrentQueue<(TaskCompletionSource<NpgsqlConnector?> TaskCompletionSource, bool IsAsync)>();

            UserFacingConnectionString = settings.PersistSecurityInfo
                ? connString
                : settings.ToStringWithoutPassword();

            Settings = settings;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryAllocateFast(NpgsqlConnection conn, [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            Counters.SoftConnectsPerSecond.Increment();

            // Idle may indicate that there are idle connectors, with the subsequent scan failing to find any.
            // This can happen because of race conditions with Release(), which updates Idle before actually putting
            // the connector in the list, or because of other allocation attempts, which remove the connector from
            // the idle list before updating Idle.
            // Loop until either State.Idle is 0 or you manage to remove a connector.
            connector = null;
            var spinner = new SpinWait();
            var idle = _idle;
            while (Volatile.Read(ref State.Idle) > 0)
            {
                for (var i = 0; connector == null && i < idle.Length; i++)
                {
                    // First check without an Interlocked operation, it's faster
                    if (Volatile.Read(ref idle[i]) == null)
                        continue;

                    // If we saw a connector in this slot, atomically exchange it with a null.
                    // Either we get a connector out which we can use, or we get null because
                    // someone has taken it in the meanwhile. Either way put a null in its place.
                    connector = Interlocked.Exchange(ref idle[i], null);
                }

                if (connector == null)
                {
                    spinner.SpinOnce();
                    continue;
                }

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
                Interlocked.Decrement(ref State.Idle);
                CheckInvariants(State);
                return true;
            }

            connector = null;
            return false;
        }

        internal async ValueTask<NpgsqlConnector> AllocateLong(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            // No idle connector was found in the pool.
            // We now loop until one of three things happen:
            // 1. The pool isn't at max capacity (Open < Max), so we can create a new physical connection.
            // 2. The pool is at maximum capacity and there are no idle connectors (Open - Idle == Max),
            // so we enqueue an open attempt into the waiting queue, so that the next release will unblock it.
            // 3. An connector makes it into the idle list (race condition with another Release()).
            while (true)
            {
                NpgsqlConnector? connector;

                var (openCount, idleCount, busyCount) = State;
                if (openCount < _max)
                {
                    // We're under the pool's max capacity, "allocate" a slot for a new physical connection.
                    // Don't spin for this https://github.com/dotnet/coreclr/pull/21437
                    var prevOpenCount = openCount;
                    while (true)
                    {
                        var currentOpenCount = prevOpenCount;
                        prevOpenCount = Interlocked.CompareExchange(ref State.Open, currentOpenCount + 1, currentOpenCount);
                        // Either we succeeded or someone else did and we're at max opens, break.
                        if (prevOpenCount == currentOpenCount || prevOpenCount == _max) break;
                    }
                    // Restart the outer loop if we're at max opens.
                    if (prevOpenCount == _max) continue;

                    try
                    {
                        // We've managed to increase the open counter, open a physical connections.
                        connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
                        await connector.Open(timeout, async, cancellationToken);
                    }
                    catch
                    {
                        // Physical open failed, decrement the open and busy counter back down.
                        conn.Connector = null;
                        Interlocked.Decrement(ref State.Open);
                        ReleaseOneWaiter();
                        throw;
                    }

                    // We immediately store the connector as well, assigning it an index
                    // that will be used during the lifetime of the connector for both _idle and _open.
                    for (var i = 0; i < _open.Length; i++)
                    {
                        if (Interlocked.CompareExchange(ref _open[i], connector, null) == null)
                        {
                            connector.PoolIndex = i;
                            break;
                        }
                    }
                    Debug.Assert(connector.PoolIndex != int.MaxValue);

                    // Only start pruning if it was this thread that incremented open count past _min.
                    if (prevOpenCount == _min)
                        EnablePruning();
                    Counters.NumberOfPooledConnections.Increment();
                    Counters.NumberOfActiveConnections.Increment();
                    CheckInvariants(State);

                    return connector;
                }

                if (busyCount == _max)
                {
                    // Pool is exhausted.
                    // Enqueue an allocate attempt into the waiting queue so that the next release will unblock us.
                    var tcs = new TaskCompletionSource<NpgsqlConnector?>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _waiting.Enqueue((tcs, async));

                    // Scenario: pre-empted waiter
                    // Say there's a pre-emption of the thread right between our State.Busy read and our Enqueue.
                    // If that happens and the waiter queue is empty before we enqueue we couldn't signal to any
                    // releases we are a new waiter, causing any to add their connectors back into the idle pool.
                    // We do a correction for that right here after our own enqueue by re-checking Idle.
                    // We also check Open as we may have raced a connector close.
                    var (racedOpen, racedIdle, _) = State;
                    if (racedIdle > 0 || racedOpen < _max)
                    {
                        // If setting this fails we have been raced to completion by a Release().
                        // Otherwise we have an idle connector or open slot to try and race to.
                        if (tcs.TrySetCanceled())
                            continue;

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
                                    if (timeLeft <= TimeSpan.Zero || await Task.WhenAny(tcs.Task, Task.Delay(timeLeft, delayCancellationToken.Token)) != tcs.Task)
                                    {
                                        // Delay task completed first, either because of a user cancellation or an actual timeout
                                        cancellationToken.ThrowIfCancellationRequested();
                                        throw new NpgsqlException(
                                            $"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {Settings.Timeout} seconds)");
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
                                    throw new NpgsqlException(
                                        $"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {Settings.Timeout} seconds)");
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

                    // Note that we don't update counters since the connector is being
                    // handed off from one open connection to another.
                    Debug.Assert(tcs.Task.IsCompleted);
                    connector = tcs.Task.Result;

                    if (connector == null)
                        continue;

                    connector.Connection = conn;

                    return connector;
                }

                // We didn't create a new connector or start waiting, which means there's a new idle connector,
                // or we raced a connector close, loop again as we could potentially open a new connector.
                if (idleCount > 0 && TryAllocateFast(conn, out connector))
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

            // If there are any pending waiters we hand the connector off to them directly.
            while (_waiting.TryDequeue(out var waitingOpenAttempt))
            {
                var tcs = waitingOpenAttempt.TaskCompletionSource;

                // We have a pending waiter. "Complete" it, handing off the connector.
                if (tcs.TrySetResult(connector))
                    return;

                // If the open attempt timed out, the Task's state will be set to Canceled and our
                // TrySetResult fails. Try again.
                Debug.Assert(tcs.Task.IsCanceled);
            }

            // Scenario: pre-empted release
            // Right here between our check for waiters and our signalling decrement for storing
            // a connector there could have been a new waiter enqueueing, we compensate at the end.

            // If we're here, we put the connector back in the idle list
            // We increment Idle, any allocate that is racing us will not match Busy == _max
            // and will not enqueue but try to get our connector.
            Interlocked.Increment(ref State.Idle);
            Volatile.Write(ref _idle[connector.PoolIndex], connector);
            CheckInvariants(State);

            // Scenario: pre-empted release
            // We checked at the start of release if there were any waiters.
            // Unblock any new waiter that raced us by handing it a null result.
            // We try to complete exactly one waiter as long as there are any in the queue, if any came in at all.
            // The performance of trying this after each _idle release is fine as the queue is very uncontended.
            // In the .Net Core BCL, 3.0 as of writing, TryDequeue for the empty path is as fast as doing IsEmpty.
            ReleaseOneWaiter();

            // Scenario: pre-empted waiter
            // Could have a pre-empted waiter, that didn't enqueue yet it wakes up right after
            // our correcting dequeue, it will do its own check after that Enqueue for Idle > 0.
        }

        void CloseConnector(NpgsqlConnector connector, bool wasIdle)
        {
            try
            {
                connector.Close();
            }
            catch (Exception e)
            {
                Log.Warn("Exception while closing outdated connector", e, connector.Id);
            }

            _open[connector.PoolIndex] = null;

            int openCount;
            if (wasIdle)
            {
                var prevAll = Volatile.Read(ref State.All);
                var prevState = new PoolState { All = prevAll };
                while (true)
                {
                    var state = new PoolState { Open = prevState.Open - 1, Idle = prevState.Idle - 1 };
                    prevAll = Interlocked.CompareExchange(ref State.All, state.All, prevState.All);
                    if (prevAll == prevState.All) break;

                    prevState = new PoolState { All = prevAll };
                }
                openCount = prevState.Open - 1;
            }
            else
                openCount = Interlocked.Decrement(ref State.Open);

            // Unblock a single waiter, if any, to get the slot that just opened up.
            ReleaseOneWaiter();

            // Only turn off the timer one time, when it was this Close that brought Open back to _min.
            if (openCount == _min)
                DisablePruning();
            Counters.NumberOfPooledConnections.Decrement();
            CheckInvariants(State);
        }

        /// <summary>
        /// Dequeues a single waiter and signals that it should re-attempt to allocate again. Needed in various
        /// race conditions.
        /// </summary>
        void ReleaseOneWaiter()
        {
            while (_waiting.TryDequeue(out var waiter))
                if (waiter.TaskCompletionSource.TrySetResult(null))
                    break;
        }

        // Manual reactivation of timer happens in callback
        void EnablePruning()
        {
            lock (_pruningTimer)
            {
                _pruningTimerEnabled = true;
                _pruningTimer.Change(_pruningSamplingInterval, Timeout.InfiniteTimeSpan);
            }
        }

        void DisablePruning()
        {
            lock (_pruningTimer)
            {
                _pruningTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _pruningSampleIndex = 0;
                _pruningTimerEnabled = false;
            }
        }

        static void PruneIdleConnectors(object? state)
        {
            var pool = (ConnectorPool)state!;
            var samples = pool._pruningSamples;
            int toPrune;
            lock (pool._pruningTimer)
            {
                // Check if we might have been contending with DisablePruning.
                if (!pool._pruningTimerEnabled)
                    return;

                var sampleIndex = pool._pruningSampleIndex;
                samples[sampleIndex] = pool.State.Idle;

                if (sampleIndex != pool._pruningSampleSize - 1)
                {
                    pool._pruningSampleIndex = sampleIndex + 1;
                    pool._pruningTimer.Change(pool._pruningSamplingInterval, Timeout.InfiniteTimeSpan);

                    return;
                }

                // Calculate median value for pruning, reset index and timer, and release the lock.
                Array.Sort(samples);
                toPrune = samples[pool._pruningMedianIndex];
                pool._pruningSampleIndex = 0;
                pool._pruningTimer.Change(pool._pruningSamplingInterval, Timeout.InfiniteTimeSpan);
            }

            var idle = pool._idle;
            for (var i = 0; i < idle.Length; i++)
            {
                if (Volatile.Read(ref pool.State.Open) <= pool._min || toPrune == 0)
                    return;

                var connector = Interlocked.Exchange(ref idle[i], null);
                if (connector == null) continue;

                toPrune -= 1;
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

        static int DivideRoundingUp(int value, int divisor) => 1 + (value - 1) / divisor;

        [Conditional("DEBUG")]
        void CheckInvariants(PoolState state)
        {
            if (state.Open > _max)
                throw new NpgsqlException($"Pool is over capacity (Total={state.Open}, Max={_max})");
            if (state.Open < 0)
                throw new NpgsqlException("Open is negative");
            if (state.Idle < 0)
                throw new NpgsqlException("Idle is negative");
            if (state.Open - state.Idle < 0)
                throw new NpgsqlException("Busy is negative");
        }

        public void Dispose() => _pruningTimer?.Dispose();

        public override string ToString()
        {
            var (open, idle, busy, waiters) = Statistics;
            return $"[{open} total, {idle} idle, {busy} busy, {waiters} waiters]";
        }

        #endregion Misc
    }
}
