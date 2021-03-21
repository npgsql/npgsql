using Npgsql.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    sealed class MultiHostConnectorPool : ConnectorSource
    {
        readonly ConnectorPool[] _pools;

        public MultiHostConnectorPool(NpgsqlConnectionStringBuilder settings, string connString) : base(settings, connString)
        {
            var hosts = settings.Host!.Split(',');
            _pools = new ConnectorPool[hosts.Length];
            for (var i = 0; i < hosts.Length; i++)
            {
                var host = hosts[i];
                var port = settings.Port;
                // TODO: Add support for host:port to the default pool (and conn without pooling)
                var portSeparator = host.IndexOf(':');
                if (portSeparator != -1)
                {
                    host = host.Substring(0, portSeparator);
                    port = int.Parse(host.Substring(portSeparator + 1));
                }
                var poolSettings = settings.Clone();
                poolSettings.Host = host;
                poolSettings.Port = port;
                _pools[i] = new ConnectorPool(poolSettings, poolSettings.ConnectionString, this);
            }
        }

        static bool IsPreferred(ClusterState state, TargetSessionAttributes preferredType)
            => state switch
            {
                ClusterState.Offline => false,
                ClusterState.Unknown => true, // We will check compatibility again after refreshing the cluster state
                ClusterState.Primary when preferredType == TargetSessionAttributes.Primary || preferredType == TargetSessionAttributes.PreferPrimary => true,
                ClusterState.Secondary when preferredType == TargetSessionAttributes.Secondary || preferredType == TargetSessionAttributes.PreferSecondary => true,
                _ => preferredType == TargetSessionAttributes.Any
            };

        static bool IsFallbackOrPreferred(ClusterState state, TargetSessionAttributes preferredType)
            => state switch
            {
                ClusterState.Unknown => true, // We will check compatibility again after refreshing the cluster state
                ClusterState.Primary when preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferSecondary => true,
                ClusterState.Secondary when preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferSecondary => true,
                _ => false
            };

        static ClusterState GetClusterState(ConnectorPool pool)
            => ClusterStateCache.GetClusterState(pool.Settings.Host!, pool.Settings.Port);

        async ValueTask<NpgsqlConnector?> TryGetIdle(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetSessionAttributes preferredType,
            Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferredType))
                    continue;

                if (pool.TryGetIdleConnector(out var connector))
                {
                    conn.Connector = connector;
                    connector.Connection = conn;

                    try
                    {
                        if (clusterState == ClusterState.Unknown)
                        {
                            clusterState = await connector.QueryClusterState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                            Debug.Assert(clusterState != ClusterState.Unknown);
                            if (!clusterValidator(clusterState, preferredType))
                            {
                                conn.Connector = null;
                                connector.Connection = null;
                                pool.Return(connector);
                                continue;
                            }
                        }

                        return connector;
                    }
                    catch (Exception ex)
                    {
                        conn.Connector = null;
                        connector.Connection = null;
                        pool.Return(connector);
                        exceptions.Add(new NpgsqlException($"Unable to get an idle connection to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                    }
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryOpenNew(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetSessionAttributes preferedType,
            Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferedType))
                    continue;

                try
                {
                    var connector = await pool.OpenNewConnector(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    if (connector is not null)
                    {
                        // Opening a new physical connection refreshed the cluster state, check again
                        clusterState = GetClusterState(pool);
                        Debug.Assert(clusterState != ClusterState.Unknown);
                        if (!clusterValidator(clusterState, preferedType))
                        {
                            conn.Connector = null;
                            connector.Connection = null;
                            pool.Return(connector);
                            continue;
                        }

                        conn.Connector = connector;
                        connector.Connection = conn;
                        return connector;
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(new NpgsqlException($"Unable to open a new connection to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryGet(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetSessionAttributes preferedType,
            Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferedType))
                    continue;

                NpgsqlConnector? connector = null;

                try
                {
                    connector = await pool.Get(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    // Get may be have opened a new physical connection and refreshed the cluster state, check again
                    if (clusterState == ClusterState.Unknown)
                    {
                        clusterState = await connector.QueryClusterState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                        Debug.Assert(clusterState != ClusterState.Unknown);
                        if (!clusterValidator(clusterState, preferedType))
                        {
                            conn.Connector = null;
                            connector.Connection = null;
                            pool.Return(connector);
                            continue;
                        }
                    }

                    conn.Connector = connector;
                    connector.Connection = conn;
                    return connector;
                }
                catch (Exception ex)
                {
                    if (connector is not null)
                    {
                        conn.Connector = null;
                        connector.Connection = null;
                        pool.Return(connector);
                    }

                    exceptions.Add(new NpgsqlException($"Unable to connect to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                }
            }

            return null;
        }

        internal override async ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var exceptions = new List<Exception>();

            var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
            var preferredType = conn.Settings.TargetSessionAttributes;

            var idlePreferredConnector = await TryGetIdle(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken);
            if (idlePreferredConnector is not null)
                return idlePreferredConnector;

            var newPreferredConnector = await TryOpenNew(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken);
            if (newPreferredConnector is not null)
                return newPreferredConnector;

            if (preferredType == TargetSessionAttributes.Any)
            {
                var rentedAnyConnector = await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken);
                if (rentedAnyConnector is null)
                    throw NoSuitableHostsException(exceptions);
                return rentedAnyConnector;
            }

            if (preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferSecondary)
            {
                var idleUnpreferedConnector = await TryGetIdle(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, exceptions, cancellationToken);
                if (idleUnpreferedConnector is not null)
                    return idleUnpreferedConnector;

                var newUnpreferedConnector = await TryOpenNew(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, exceptions, cancellationToken);
                if (newUnpreferedConnector is not null)
                    return newUnpreferedConnector;
            }

            // TODO: add a queue to wait for the connector
            var rentedPreferedConnector = await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken);
            if (rentedPreferedConnector is not null)
                return rentedPreferedConnector;

            if (preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferSecondary)
            {
                var rentedUnpreferedConnector = await TryGet(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, exceptions, cancellationToken);
                if (rentedUnpreferedConnector is not null)
                    return rentedUnpreferedConnector;
            }

            if (exceptions.Count == 0)
                exceptions.Add(new NpgsqlException("No suitable host has been found."));

            throw NoSuitableHostsException(exceptions);
        }

        static NpgsqlException NoSuitableHostsException(IList<Exception> exceptions)
            => new("Unable to connect to a suitable host. Check inner exception for more details.", new AggregateException(exceptions));

        internal override void Return(NpgsqlConnector connector)
            => throw new NpgsqlException("Npgsql bug: a connector was returned to " + nameof(MultiHostConnectorPool));

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
                    numConnectors += pool.NumConnectors;
                    idleCount += pool.IdleCount;
                }

                return (numConnectors, idleCount, numConnectors - idleCount);
            }
        }
    }
}
