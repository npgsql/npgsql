using Npgsql.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

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
                var host = hosts[i].Trim();
                var port = settings.Port;
                var portSeparator = host.IndexOf(':');
                if (portSeparator != -1)
                {
                    port = int.Parse(host.Substring(portSeparator + 1));
                    host = host.Substring(0, portSeparator);
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

        static ClusterState GetClusterState(ConnectorPool pool)
            => GetClusterState(pool.Settings.Host!, pool.Settings.Port, ignoreExpiration: false);

        static ClusterState GetClusterState(string host, int port, bool ignoreExpiration)
            => ClusterStateCache.GetClusterState(host, port, ignoreExpiration);

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
                        conn.FullState = ConnectionState.Connecting;
                        conn.Connector = null;
                        connector.Connection = null;
                        pool.Return(connector);
                        exceptions.Add(new NpgsqlException($"Unable to get an idle connection to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                    }
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryOpenNew(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetSessionAttributes preferredType,
            Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferredType))
                    continue;

                try
                {
                    var connector = await pool.OpenNewConnector(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    if (connector is not null)
                    {
                        if (clusterState == ClusterState.Unknown)
                        {
                            // Opening a new physical connection refreshed the cluster state, check again
                            clusterState = GetClusterState(pool);
                            Debug.Assert(clusterState != ClusterState.Unknown);
                            if (!clusterValidator(clusterState, preferredType))
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
                }
                catch (Exception ex)
                {
                    conn.FullState = ConnectionState.Connecting;
                    exceptions.Add(ex);
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryGet(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetSessionAttributes preferredType,
            Func<ClusterState, TargetSessionAttributes, bool> clusterValidator, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                var clusterState = GetClusterState(pool);
                if (!clusterValidator(clusterState, preferredType))
                    continue;

                NpgsqlConnector? connector = null;

                try
                {
                    connector = await pool.Get(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    // Get may be have opened a new physical connection and refreshed the cluster state, check again
                    if (clusterState == ClusterState.Unknown)
                    {
                        clusterState = GetClusterState(pool);
                        if (clusterState == ClusterState.Unknown)
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

                    conn.FullState = ConnectionState.Connecting;
                    exceptions.Add(new NpgsqlException($"Unable to connect to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                }
            }

            return null;
        }

        internal override async ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var exceptions = new List<Exception>();

            var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
            var preferredType = conn.Settings.TargetSessionAttributesParsed;
            var checkUnpreferred =
                preferredType == TargetSessionAttributes.PreferPrimary ||
                preferredType == TargetSessionAttributes.PreferStandby;

            var connector = await TryGetIdle(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken) ??
                            await TryOpenNew(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken) ??
                            (checkUnpreferred ?
                                await TryGetIdle(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, exceptions, cancellationToken) ??
                                await TryOpenNew(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, exceptions, cancellationToken)
                            : null) ??
                            await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, exceptions, cancellationToken) ??
                            (checkUnpreferred ?
                                await TryGet(conn, timeoutPerHost, async, preferredType, IsFallbackOrPreferred, exceptions, cancellationToken)
                            : null);

            if (connector is not null)
                return connector;

            conn.FullState = ConnectionState.Broken;
            throw NoSuitableHostsException(exceptions);
        }

        static NpgsqlException NoSuitableHostsException(IList<Exception> exceptions)
            => exceptions.Count == 0
                ? new NpgsqlException("No suitable host was found.")
                : new("Unable to connect to a suitable host. Check inner exception for more details.",
                    new AggregateException(exceptions));

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
                    var preferredType = connection.Settings.TargetSessionAttributesParsed;
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
    }
}
