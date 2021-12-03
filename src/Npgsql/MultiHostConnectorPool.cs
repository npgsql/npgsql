using Npgsql.Internal;
using Npgsql.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Npgsql
{
    sealed class MultiHostConnectorPool : ConnectorSource
    {
        internal override bool OwnsConnectors => false;

        readonly ConnectorSource[] _pools;

        volatile int _roundRobinIndex = -1;

        public MultiHostConnectorPool(NpgsqlConnectionStringBuilder settings, string connString) : base(settings, connString)
        {
            var hosts = settings.Host!.Split(',');
            _pools = new ConnectorSource[hosts.Length];
            for (var i = 0; i < hosts.Length; i++)
            {
                var poolSettings = settings.Clone();
                Debug.Assert(!poolSettings.Multiplexing);
                var host = hosts[i].AsSpan().Trim();
                if (NpgsqlConnectionStringBuilder.TrySplitHostPort(host, out var newHost, out var newPort))
                {
                    poolSettings.Host = newHost;
                    poolSettings.Port = newPort;
                }
                else
                    poolSettings.Host = host.ToString();

                _pools[i] = settings.Pooling
                    ? new ConnectorPool(poolSettings, poolSettings.ConnectionString, this)
                    : new UnpooledConnectorSource(poolSettings, poolSettings.ConnectionString);
            }
        }

        static bool IsPreferred(ClusterState state, TargetSessionAttributes preferredType)
            => state switch
            {
                ClusterState.Offline => false,
                ClusterState.Unknown => true, // We will check compatibility again after refreshing the cluster state
                ClusterState.PrimaryReadWrite when preferredType == TargetSessionAttributes.Primary || preferredType == TargetSessionAttributes.PreferPrimary
                    || preferredType == TargetSessionAttributes.ReadWrite  => true,
                ClusterState.PrimaryReadOnly when preferredType == TargetSessionAttributes.Primary || preferredType == TargetSessionAttributes.PreferPrimary
                    || preferredType == TargetSessionAttributes.ReadOnly => true,
                ClusterState.Standby when preferredType == TargetSessionAttributes.Standby || preferredType == TargetSessionAttributes.PreferStandby
                    || preferredType == TargetSessionAttributes.ReadOnly => true,
                _ => preferredType == TargetSessionAttributes.Any
            };

        static bool IsFallbackOrPreferred(ClusterState state, TargetSessionAttributes preferredType)
            => state switch
            {
                ClusterState.Unknown => true, // We will check compatibility again after refreshing the cluster state
                ClusterState.PrimaryReadWrite when preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferStandby => true,
                ClusterState.PrimaryReadOnly when preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferStandby => true,
                ClusterState.Standby when preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferStandby => true,
                _ => false
            };

        static ClusterState GetClusterState(ConnectorSource pool, bool ignoreExpiration = false)
            => GetClusterState(pool.Settings.Host!, pool.Settings.Port, ignoreExpiration);

        static ClusterState GetClusterState(string host, int port, bool ignoreExpiration)
            => ClusterStateCache.GetClusterState(host, port, ignoreExpiration);

        async ValueTask<NpgsqlConnector?> TryGetIdleOrNew(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async,
            TargetSessionAttributes preferredType, Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, int poolIndex,
            IList<Exception> exceptions, CancellationToken cancellationToken)
        {
            var pools = _pools;
            for (var i = 0; i < pools.Length; i++)
            {
                var pool = pools[poolIndex];
                poolIndex++;
                if (poolIndex == pools.Length)
                    poolIndex = 0;

                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferredType))
                    continue;

                NpgsqlConnector? connector = null;

                try
                {
                    if (pool.TryGetIdleConnector(out connector))
                    {
                        if (clusterState == ClusterState.Unknown)
                        {
                            clusterState = await connector.QueryClusterState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                            Debug.Assert(clusterState != ClusterState.Unknown);
                            if (!clusterValidator(clusterState, preferredType))
                            {
                                pool.Return(connector);
                                continue;
                            }
                        }

                        return connector;
                    }
                    else
                    {
                        connector = await pool.OpenNewConnector(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                        if (connector is not null)
                        {
                            if (clusterState == ClusterState.Unknown)
                            {
                                clusterState = await connector.QueryClusterState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                                Debug.Assert(clusterState != ClusterState.Unknown);
                                if (!clusterValidator(clusterState, preferredType))
                                {
                                    pool.Return(connector);
                                    continue;
                                }
                            }

                            return connector;
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    if (connector is not null)
                        pool.Return(connector);
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryGet(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetSessionAttributes preferredType,
            Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, int poolIndex,
            IList<Exception> exceptions, CancellationToken cancellationToken)
        {
            var pools = _pools;
            for (var i = 0; i < pools.Length; i++)
            {
                var pool = pools[poolIndex];
                poolIndex++;
                if (poolIndex == pools.Length)
                    poolIndex = 0;

                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferredType))
                    continue;

                NpgsqlConnector? connector = null;

                try
                {
                    connector = await pool.Get(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    if (clusterState == ClusterState.Unknown)
                    {
                        // Get might have opened a new physical connection and refreshed the cluster state, check again
                        clusterState = GetClusterState(pool);
                        if (clusterState == ClusterState.Unknown)
                            clusterState = await connector.QueryClusterState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);

                        Debug.Assert(clusterState != ClusterState.Unknown);
                        if (!clusterValidator(clusterState, preferredType))
                        {
                            pool.Return(connector);
                            continue;
                        }
                    }

                    return connector;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    if (connector is not null)
                        pool.Return(connector);
                }
            }

            return null;
        }

        internal override async ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var exceptions = new List<Exception>();

            var poolIndex = conn.Settings.LoadBalanceHosts ? GetRoundRobinIndex() : 0;

            var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
            var preferredType = GetTargetSessionAttributes(conn);
            var checkUnpreferred =
                preferredType == TargetSessionAttributes.PreferPrimary ||
                preferredType == TargetSessionAttributes.PreferStandby;

            var connector = await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
                            (checkUnpreferred ?
                                await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, poolIndex, exceptions, cancellationToken)
                            : null) ??
                            await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
                            (checkUnpreferred ?
                                await TryGet(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, poolIndex, exceptions, cancellationToken)
                            : null);

            if (connector is not null)
                return connector;

            throw NoSuitableHostsException(exceptions);
        }

        static NpgsqlException NoSuitableHostsException(IList<Exception> exceptions)
            => exceptions.Count == 0
                ? new NpgsqlException("No suitable host was found.")
                : exceptions[0] is PostgresException firstException &&
                  exceptions.All(x => x is PostgresException ex && ex.SqlState == firstException.SqlState)
                    ? firstException
                    : new NpgsqlException("Unable to connect to a suitable host. Check inner exception for more details.",
                        new AggregateException(exceptions));

        int GetRoundRobinIndex()
        {
            while (true)
            {
                var index = Interlocked.Increment(ref _roundRobinIndex);
                if (index >= 0)
                    return index % _pools.Length;

                // Worst case scenario - we've wrapped around integer counter
                if (index == int.MinValue)
                {
                    // This is the thread which wrapped around the counter - reset it to 0
                    _roundRobinIndex = 0;
                    return 0;
                }

                // This is not the thread which wrapped around the counter - just wait until it's 0 or more
                var sw = new SpinWait();
                while (_roundRobinIndex < 0)
                    sw.SpinOnce();
            }
        }

        internal override void Return(NpgsqlConnector connector)
            => throw new NpgsqlException("Npgsql bug: a connector was returned to " + nameof(MultiHostConnectorPool));

        internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
            => throw new NpgsqlException("Npgsql bug: trying to get an idle connector from " + nameof(MultiHostConnectorPool));

        internal override ValueTask<NpgsqlConnector?> OpenNewConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
            => throw new NpgsqlException("Npgsql bug: trying to open a new connector from " + nameof(MultiHostConnectorPool));

        internal override void Clear()
        {
            foreach (var pool in _pools)
                pool.Clear();
        }

        internal override (int Total, int Idle, int Busy) Statistics
        {
            get
            {
                var numConnectors = 0;
                var idleCount = 0;

                foreach (var pool in _pools)
                {
                    var stat = pool.Statistics;
                    numConnectors += stat.Total;
                    idleCount += stat.Idle;
                }

                return (numConnectors, idleCount, numConnectors - idleCount);
            }
        }

        internal override bool TryRentEnlistedPending(Transaction transaction, NpgsqlConnection connection,
            [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            lock (_pendingEnlistedConnectors)
            {
                if (!_pendingEnlistedConnectors.TryGetValue(transaction, out var list))
                {
                    connector = null;
                    return false;
                }

                for (var i = list.Count - 1; i >= 0; i--)
                {
                    connector = list[i];
                    var lastKnownState = GetClusterState(connector.Host, connector.Port, ignoreExpiration: true);
                    Debug.Assert(lastKnownState != ClusterState.Unknown);
                    var preferredType = GetTargetSessionAttributes(connection);
                    if (IsFallbackOrPreferred(lastKnownState, preferredType))
                    {
                        list.RemoveAt(i);
                        if (list.Count == 0)
                            _pendingEnlistedConnectors.Remove(transaction);
                        return true;
                    }
                }

                connector = null;
                return false;
            }
        }

        static TargetSessionAttributes GetTargetSessionAttributes(NpgsqlConnection connection)
            => connection.Settings.TargetSessionAttributesParsed ??
               (PostgresEnvironment.TargetSessionAttributes is string s
                   ? NpgsqlConnectionStringBuilder.ParseTargetSessionAttributes(s)
                   : default);
    }
}
