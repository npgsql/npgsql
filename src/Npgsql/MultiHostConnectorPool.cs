using Npgsql.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    sealed class MultiHostConnectorPool : ConnectorPoolBase
    {
        readonly ConnectorPool[] _pools;

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
                // TODO: should be safely removed
                /*
                // Setting the default settings due to PoolManager.TryGetValue
                _pools[i] = new ConnectorPool(poolSettings, settings.ConnectionString, this);
                */
            }
        }

        static bool IsValidHost(ConnectorPool pool, TargetServerType preferedType)
        {
            var currentState = GetHostState(pool);
            if (currentState == HostState.Offline)
                return false;
            return preferedType == TargetServerType.Any || currentState == HostState.Unknown ||
                preferedType.HasFlag(TargetServerType.Primary) && currentState == HostState.Primary ||
                preferedType.HasFlag(TargetServerType.Secondary) && currentState == HostState.Secondary;
        }

        static bool IsValidUnpreferedHost(ConnectorPool pool, TargetServerType preferedType)
        {
            var currentState = GetHostState(pool);
            if (currentState == HostState.Offline)
                return false;
            return currentState == HostState.Unknown ||
                preferedType == TargetServerType.PreferSecondary && currentState == HostState.Primary ||
                preferedType == TargetServerType.PreferPrimary && currentState == HostState.Secondary;
        }

        static HostState GetHostState(ConnectorPool pool)
            => HostsCache.GetHostState($"{pool.Settings.Host}:{pool.Settings.Port}");

        async ValueTask<NpgsqlConnector?> TryGetIdle(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetServerType preferedType,
            Func<ConnectorPool, TargetServerType, bool> validHostFunc, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                if (!validHostFunc(pool, preferedType))
                    continue;

                if (pool.TryGetIdleConnector(out var connector))
                {
                    conn.Connector = connector;
                    connector.Connection = conn;

                    try
                    {
                        if (GetHostState(pool) == HostState.Unknown)
                            await connector.UpdateServerType(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);

                        if (!validHostFunc(pool, preferedType))
                        {
                            conn.Connector = null;
                            connector.Connection = null;
                            pool.Return(connector);
                            continue;
                        }

                        return connector;
                    }
                    catch (Exception ex)
                    {
                        conn.Connector = null;
                        connector.Connection = null;
                        pool.Return(connector);
                        exceptions.Add(new NpgsqlException($"Unable to connect to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                    }
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryOpenNew(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetServerType preferedType,
            Func<ConnectorPool, TargetServerType, bool> validHostFunc, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                if (!validHostFunc(pool, preferedType))
                    continue;

                try
                {
                    var connector = await pool.OpenNewConnector(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    if (connector is not null)
                    {
                        if (!validHostFunc(pool, preferedType))
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
                    exceptions.Add(new NpgsqlException($"Unable to connect to {pool.Settings.Host}:{pool.Settings.Port}", ex));
                }
            }

            return null;
        }

        async ValueTask<NpgsqlConnector?> TryRent(NpgsqlConnection conn, TimeSpan timeoutPerHost, bool async, TargetServerType preferedType,
            Func<ConnectorPool, TargetServerType, bool> validHostFunc, IList<Exception> exceptions,
            CancellationToken cancellationToken)
        {
            foreach (var pool in _pools)
            {
                if (!validHostFunc(pool, preferedType))
                    continue;

                NpgsqlConnector? connector = null;

                try
                {
                    connector = await pool.Rent(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                    if (GetHostState(pool) == HostState.Unknown)
                        await connector.UpdateServerType(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);

                    if (!validHostFunc(pool, preferedType))
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

        internal override async ValueTask<NpgsqlConnector> Rent(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            var exceptions = new List<Exception>();

            var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
            var preferedType = conn.Settings.TargetServerType;

            var idlePreferedConnector = await TryGetIdle(conn, timeoutPerHost, async, preferedType, IsValidHost, exceptions, cancellationToken);
            if (idlePreferedConnector is not null)
                return idlePreferedConnector;

            var newPreferedConnector = await TryOpenNew(conn, timeoutPerHost, async, preferedType, IsValidHost, exceptions, cancellationToken);
            if (newPreferedConnector is not null)
                return newPreferedConnector;

            if (preferedType == TargetServerType.Any)
            {
                var rentedAnyConnector = await TryRent(conn, timeoutPerHost, async, preferedType, IsValidHost, exceptions, cancellationToken);
                if (rentedAnyConnector is null)
                    throw NoSuitableHostsException(exceptions);
                return rentedAnyConnector;
            }

            if (preferedType.HasFlag(TargetServerType.Any))
            {
                var idleUnpreferedConnector = await TryGetIdle(conn, timeoutPerHost, async, preferedType, IsValidUnpreferedHost, exceptions, cancellationToken);
                if (idleUnpreferedConnector is not null)
                    return idleUnpreferedConnector;

                var newUnpreferedConnector = await TryOpenNew(conn, timeoutPerHost, async, preferedType, IsValidUnpreferedHost, exceptions, cancellationToken);
                if (newUnpreferedConnector is not null)
                    return newUnpreferedConnector;
            }

            // TODO: add a queue to wait for the connector
            var rentedPreferedConnector = await TryRent(conn, timeoutPerHost, async, preferedType, IsValidHost, exceptions, cancellationToken);
            if (rentedPreferedConnector is not null)
                return rentedPreferedConnector;

            if (preferedType.HasFlag(TargetServerType.Any))
            {
                var rentedUnpreferedConnector = await TryRent(conn, timeoutPerHost, async, preferedType, IsValidUnpreferedHost, exceptions, cancellationToken);
                if (rentedUnpreferedConnector is not null)
                    return rentedUnpreferedConnector;
            }

            throw NoSuitableHostsException(exceptions);
        }

        static NpgsqlException NoSuitableHostsException(IList<Exception> exceptions)
            => new NpgsqlException("Unable to connect to a suitable host. Check inner exception for more details.", new AggregateException(exceptions));

        internal override void Clear()
        {
            foreach (var pool in _pools)
                pool.Clear();
        }
    }
}
