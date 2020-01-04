using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Transactions;
using Npgsql.Logging;
using Npgsql.Util;

namespace Npgsql
{
    sealed class ConnectorPool
    {
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ConnectorPool));

        internal NpgsqlConnectionStringBuilder Settings { get; }

        /// <summary>
        /// Contains the connection string returned to the user from <see cref="NpgsqlConnection.ConnectionString"/>
        /// after the connection has been opened. Does not contain the password unless Persist Security Info=true.
        /// </summary>
        internal string UserFacingConnectionString { get; }

        readonly int _max;
        readonly int _min;
        volatile int _numConnectors;

        readonly Channel<NpgsqlConnector> _idle;
        readonly ChannelReader<NpgsqlConnector> _idleConnectorReader;
        readonly ChannelWriter<NpgsqlConnector> _idleWriter;

        /// <summary>
        /// Incremented every time this pool is cleared via <see cref="NpgsqlConnection.ClearPool"/> or
        /// <see cref="NpgsqlConnection.ClearAllPools"/>. Allows us to identify connections which were
        /// created before the clear.
        /// </summary>
        int _clearCounter;

        // Note that while the dictionary is protected by locking, we assume that the lists it contains don't need to be
        // (i.e. access to connectors of a specific transaction won't be concurrent)
        readonly Dictionary<Transaction, List<NpgsqlConnector>> _pendingEnlistedConnectors
            = new Dictionary<Transaction, List<NpgsqlConnector>>();

        internal (int Open, int Idle, int Busy, int Waiters) Statistics => (0, 0, 0, 0); // TODO

        internal ConnectorPool(NpgsqlConnectionStringBuilder settings, string connString)
        {
            if (settings.MaxPoolSize < settings.MinPoolSize)
                throw new ArgumentException($"Connection can't have MaxPoolSize {settings.MaxPoolSize} under MinPoolSize {settings.MinPoolSize}");

            // We enforce Max Pool Size, so need to to create a bounded channel (which is less efficient)
            _idle = Channel.CreateUnbounded<NpgsqlConnector>(
                new UnboundedChannelOptions { AllowSynchronousContinuations = true });
            _idleConnectorReader = _idle.Reader;
            _idleWriter = _idle.Writer;

            _max = settings.MaxPoolSize;
            _min = settings.MinPoolSize;

            UserFacingConnectionString = settings.PersistSecurityInfo
                ? connString
                : settings.ToStringWithoutPassword();

            Settings = settings;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryAllocateFast(NpgsqlConnection conn, [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            if (TryGetIdleConnector(out connector))
            {
                connector.Connection = conn;
                return true;
            }

            return false;
        }

        internal async ValueTask<NpgsqlConnector> AllocateLong(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async,
            CancellationToken cancellationToken)
        {
            NpgsqlConnector? connector = null;

            // No idle connector was found in the pool.
            // We now loop until one of three things happen:
            // 1. The pool isn't at max capacity (Open < Max), so we can create a new physical connection.
            // 2. The pool is at maximum capacity and there are no idle connectors (Open - Idle == Max),
            // so we enqueue an open attempt into the waiting queue, so that the next release will unblock it.
            // 3. An connector makes it into the idle list (race condition with another Release()).
            while (true)
            {
                var numConnectors = _numConnectors;
                if (numConnectors < _max)
                {
                    // We're under the pool's max capacity, create a new physical connection.
                    // Note that we purposefully don't use SpinWait for this: https://github.com/dotnet/coreclr/pull/21437
                    if (Interlocked.CompareExchange(ref _numConnectors, numConnectors + 1, numConnectors) != numConnectors)
                        continue;

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
                        Interlocked.Decrement(ref _numConnectors);
                        throw;
                    }

                    // Only start pruning if it was this thread that incremented open count past _min.
                    if (numConnectors == _min)
                        EnablePruning();

                    return connector;
                }

                break;
            }

            // We're at max capacity. Asynchronously block on the idle channel with a timeout.

            // TODO: Potential issue: we only check to create new connections once above. In theory we could have
            // many attempts waiting on the idle channel forever, since all connections were broken by some network
            // event. Pretty sure this issue exists in the old lock-free implementation too, think about it (it would
            // be enough to retry the physical creation above from time to time).

            var timeoutSource = new CancellationTokenSource(timeout.TimeLeft);
            var timeoutToken = timeoutSource.Token;
            using var _ = cancellationToken.Register(cts => ((CancellationTokenSource)cts!).Cancel(), timeoutSource);

            try
            {
                while (true)
                {
                    // Block until a connector is released.
                    // Note that Channels guarantee fair FIFO behavior to callers of ReadAsync (first-come first-
                    // served), which is crucial to us.
                    connector = await _idleConnectorReader.ReadAsync(timeoutToken);
                    if (IsIdleConnectorValid(connector))
                        return connector;
                }
            }
            catch (OperationCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Debug.Assert(timeoutToken.IsCancellationRequested);
                throw new NpgsqlException(
                    $"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) " +
                    $"or Timeout (currently {Settings.Timeout} seconds)");
            }
            catch (ChannelClosedException)
            {
                // TODO: The channel has been completed, the pool is being disposed. Does this actually occur?
                throw new NpgsqlException("The connection pool has been shut down.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector connector)
        {
            while (_idleConnectorReader.TryRead(out connector))
                if (IsIdleConnectorValid(connector))
                    return true;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsIdleConnectorValid(NpgsqlConnector connector)
        {
            // An connector could be broken because of a keepalive that occurred while it was
            // idling in the pool
            // TODO: Consider removing the pool from the keepalive code. The following branch is simply irrelevant
            // if keepalive isn't turned on.
            if (connector.IsBroken)
            {
                CloseConnector(connector);
                return false;
            }

            return true;
        }

        internal void Release(NpgsqlConnector connector)
        {
            // If Clear/ClearAll has been been called since this connector was first opened,
            // throw it away. The same if it's broken (in which case CloseConnector is only
            // used to update state/perf counter).
            if (connector.ClearCounter < _clearCounter || connector.IsBroken)
            {
                CloseConnector(connector);
                return;
            }

            connector.Reset();

            var written = _idleWriter.TryWrite(connector);
            Debug.Assert(written);
        }

        internal void Clear()
        {
            _clearCounter++;
            // TODO
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

        #endregion

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

            var numConnectors = Interlocked.Decrement(ref _numConnectors);
            Debug.Assert(numConnectors >= 0);
            // Only turn off the timer one time, when it was this Close that brought Open back to _min.
            if (numConnectors == _min)
                DisablePruning();
        }

        void EnablePruning()
        {
            // TODO
        }

        void DisablePruning()
        {
            // TODO
        }
    }
}
