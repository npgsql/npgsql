using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Text;
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
        internal static ConcurrentDictionary<NpgsqlConnectionStringBuilder, ConnectorPool> Pools { get; private set; }

        static Timer _pruningTimer;

        /// <summary>
        /// Maximum number of possible connections in the pool.
        /// </summary>
        internal const int PoolSizeLimit = 1024;

        const int PruningInterval = 1000;

        static PoolManager()
        {
            Pools = new ConcurrentDictionary<NpgsqlConnectionStringBuilder, ConnectorPool>();
        }

        internal static ConnectorPool GetOrAdd(NpgsqlConnectionStringBuilder connString)
        {
            Contract.Requires(connString != null);
            Contract.Ensures(Contract.Result<ConnectorPool>() != null);

            if (_pruningTimer == null)
                _pruningTimer = new Timer(PruneIdleConnectors, null, PruningInterval, PruningInterval);

            return Pools.GetOrAdd(connString, cs =>
            {
                if (cs.MaxPoolSize < cs.MinPoolSize)
                    throw new ArgumentException($"Connection can't have MaxPoolSize {cs.MaxPoolSize} under MinPoolSize {cs.MinPoolSize}");
                return new ConnectorPool(cs);
            });
        }

        internal static ConnectorPool Get(NpgsqlConnectionStringBuilder connString)
        {
            Contract.Requires(connString != null);
            Contract.Ensures(Contract.Result<ConnectorPool>() != null);

            return Pools[connString];
        }

        static void PruneIdleConnectors(object state)
        {
            foreach (var pool in Pools.Values)
                pool.PruneIdleConnectors();
        }

        internal static void ClearAll()
        {
            foreach (var pool in Pools.Values)
                pool.Clear();
            _pruningTimer.Dispose();
            _pruningTimer = null;
        }
    }

    partial class ConnectorPool
    {
        internal NpgsqlConnectionStringBuilder ConnectionString;

        /// <summary>
        /// Open connectors waiting to be requested by new connections
        /// </summary>
        internal IdleConnectorList Idle;

        readonly int _max;
        internal int Min { get; }
        internal int Busy { get; private set; }

        internal Queue<TaskCompletionSource<NpgsqlConnector>> Waiting;

        /// <summary>
        /// Incremented every time this pool is cleared via <see cref="NpgsqlConnection.ClearPool"/> or
        /// <see cref="NpgsqlConnection.ClearAllPools"/>. Allows us to identify connections which were
        /// created before the clear.
        /// </summary>
        int _clearCounter;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal ConnectorPool(NpgsqlConnectionStringBuilder csb)
        {
            _max = csb.MaxPoolSize;
            Min = csb.MinPoolSize;

            ConnectionString = csb;
            Idle = new IdleConnectorList();
            Waiting = new Queue<TaskCompletionSource<NpgsqlConnector>>();
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
                Busy++;
                Monitor.Exit(this);
                return connector;
            }

            if (Busy >= _max)
            {
                // TODO: Async cancellation
                var tcs = new TaskCompletionSource<NpgsqlConnector>();
                Waiting.Enqueue(tcs);
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
            Busy++;
            Monitor.Exit(this);

            try
            {
                connector = new NpgsqlConnector(conn) { ClearCounter = _clearCounter };
                connector.Open(timeout);
                EnsureMinPoolSize(conn);
                return connector;
            }
            catch
            {
                lock (this)
                    Busy--;
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
                    Busy--;
                return;
            }

            if (connector.IsBroken)
            {
                lock (this)
                    Busy--;
                return;
            }

            connector.Reset();
            lock (this)
            {
                // If there are any pending open attempts in progress hand the connector off to
                // them directly.
                while (Waiting.Count > 0)
                {
                    var tcs = Waiting.Dequeue();
                    // Some attempts may be in the queue but in cancelled state, since they've already timed out.
                    // Simply dequeue these and move on.
                    if (tcs.Task.IsCanceled)
                        continue;
                    // We have a pending open attempt. "Complete" it, handing off the connector.
                    // We do this in another thread because we don't want to execute the continuation here.
                    Task.Run(() => tcs.SetResult(connector));
                    return;
                }

                Idle.Push(connector);
                Busy--;
                Contract.Assert(Idle.Count <= _max);
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
                    var connector = new NpgsqlConnector((NpgsqlConnection)((ICloneable)conn).Clone());
                    connector.ClearCounter = _clearCounter;
                    connector.Open();
                    connector.Reset();
                    lock (this)
                    {
                        Idle.Push(connector);
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

        internal void PruneIdleConnectors()
        {
            if (Idle.Count <= Min)
                return;

            var idleLifetime = ConnectionString.ConnectionIdleLifetime;
            lock (this)
            {
                while (Idle.Count > Min &&
                        (DateTime.UtcNow - Idle.Last.Value.ReleaseTimestamp).TotalSeconds >= idleLifetime)
                {
                    var connector = Idle.Pop();
                    try
                    {
                        connector.Close();
                    }
                    catch (Exception e)
                    {
                        Log.Warn("Exception while closing connector", e, connector.Id);
                    }
                }
            }
        }

        internal void Clear()
        {
            NpgsqlConnector[] idleConnectors;
            lock (this)
            {
                idleConnectors = Idle.ToArray();
                Idle.Clear();
            }

            foreach (var connector in idleConnectors)
            {
                try { connector.Close(); }
                catch (Exception e)
                {
                    Log.Warn("Exception while closing connector during clear", e, connector.Id);
                }
            }
            _clearCounter++;
        }

        void WaitForTask(Task task, TimeSpan timeout)
        {
            if (!task.Wait(timeout))
                throw new TimeoutException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {ConnectionString.Timeout} seconds)");
        }

        async Task WaitForTaskAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var timeoutTask = Task.Delay(timeout, cancellationToken);
            if (task != await Task.WhenAny(task, timeoutTask))
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw new TimeoutException($"The connection pool has been exhausted, either raise MaxPoolSize (currently {_max}) or Timeout (currently {ConnectionString.Timeout} seconds)");
            }
        }

        public override string ToString() => $"[{Busy} busy, {Idle.Count} idle, {Waiting.Count} waiting]";

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(Busy <= _max);
        }
    }

    class IdleConnectorList : LinkedList<NpgsqlConnector>
    {
        internal void Push(NpgsqlConnector connector)
        {
            connector.ReleaseTimestamp = DateTime.UtcNow;
            AddFirst(connector);
        }

        internal NpgsqlConnector Pop()
        {
            var connector = First.Value;
            connector.ReleaseTimestamp = DateTime.UtcNow;
            RemoveFirst();
            return connector;
        }
    }
}
