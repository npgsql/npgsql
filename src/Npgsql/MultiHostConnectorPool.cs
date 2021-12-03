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

        readonly int[] _roundRobinIndex;

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

            _roundRobinIndex = new int[7];
            for (var i = 0; i < _roundRobinIndex.Length; i++)
            {
                _roundRobinIndex[i] = -1;
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

            var preferredType = GetTargetSessionAttributes(conn);
            var poolIndex = conn.Settings.LoadBalanceHosts ? GetRoundRobinIndex(preferredType) : 0;

            var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
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

        int GetRoundRobinIndex(TargetSessionAttributes preferredType)
        {
            var preferredTypeIndex = (int)preferredType;
            Debug.Assert(preferredTypeIndex >= 0 && preferredTypeIndex < _roundRobinIndex.Length);
            // We have 20 attempts to get an index
            for (var i = 0; i < 20; i++)
            {
                var index = GetNewIndex();
                var previousHostIndex = index == 0 ? _pools.Length - 1 : index - 1;
                var clusterState = GetClusterState(_pools[previousHostIndex]);
                if (IsPreferred(clusterState, preferredType))
                    return index;

                // The previous host isn't valid - there is a high chance someone else already took the host we're attempting to get
                // Try again
            }

            // Either we're very unlucky, or every single host isn't valid
            // Just return the new index and hope for the best
            return GetNewIndex();

            int GetNewIndex() => (int)(unchecked((uint)Interlocked.Increment(ref _roundRobinIndex[preferredTypeIndex])) % _pools.Length);
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
