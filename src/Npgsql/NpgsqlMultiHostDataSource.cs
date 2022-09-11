﻿using Npgsql.Internal;
using Npgsql.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Npgsql;

#pragma warning disable CS1591
#pragma warning disable RS0016

public sealed class NpgsqlMultiHostDataSource : NpgsqlDataSource
{
    internal override bool OwnsConnectors => false;

    readonly NpgsqlDataSource[] _pools;

    internal NpgsqlDataSource[] Pools => _pools;

    readonly MultiHostDataSourceWrapper[] _wrappers;

    volatile int _roundRobinIndex = -1;

    internal NpgsqlMultiHostDataSource(NpgsqlConnectionStringBuilder settings, NpgsqlDataSourceConfiguration dataSourceConfig)
        : base(settings, dataSourceConfig)
    {
        var hosts = settings.Host!.Split(',');
        _pools = new NpgsqlDataSource[hosts.Length];
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
                ? new PoolingDataSource(poolSettings, dataSourceConfig, this)
                : new UnpooledDataSource(poolSettings, dataSourceConfig);
        }

        var targetSessionAttributeValues = Enum.GetValues(typeof(TargetSessionAttributes)).Cast<TargetSessionAttributes>().ToArray();
        _wrappers = new MultiHostDataSourceWrapper[targetSessionAttributeValues.Max(t => (int)t) + 1];
        foreach (var targetSessionAttribute in targetSessionAttributeValues)
        {
            _wrappers[(int)targetSessionAttribute] = new(this, targetSessionAttribute);
        }
    }

    /// <summary>
    /// Returns a new, unopened connection from this data source.
    /// </summary>
    /// <param name="targetSessionAttributes">Specifies the server type (e.g. primary, standby).</param>
    public NpgsqlConnection CreateConnection(TargetSessionAttributes targetSessionAttributes)
        => NpgsqlConnection.FromDataSource(_wrappers[(int)targetSessionAttributes]);

    /// <summary>
    /// Returns a new, opened connection from this data source.
    /// </summary>
    /// <param name="targetSessionAttributes">Specifies the server type (e.g. primary, standby).</param>
    public NpgsqlConnection OpenConnection(TargetSessionAttributes targetSessionAttributes)
    {
        var connection = CreateConnection(targetSessionAttributes);

        try
        {
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Returns a new, opened connection from this data source.
    /// </summary>
    /// <param name="targetSessionAttributes">Specifies the server type (e.g. primary, standby).</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    public async ValueTask<NpgsqlConnection> OpenConnectionAsync(
        TargetSessionAttributes targetSessionAttributes,
        CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection(targetSessionAttributes);

        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>
    /// Returns an <see cref="NpgsqlDataSource" /> that wraps this multi-host one with the given server type.
    /// </summary>
    /// <param name="targetSessionAttributes">Specifies the server type (e.g. primary, standby).</param>
    public NpgsqlDataSource For(TargetSessionAttributes targetSessionAttributes)
        => _wrappers[(int)targetSessionAttributes];

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

    static bool IsOnline(ClusterState state, TargetSessionAttributes preferredType)
    {
        Debug.Assert(preferredType is TargetSessionAttributes.PreferPrimary or TargetSessionAttributes.PreferStandby);
        return state != ClusterState.Offline;
    }

    static ClusterState GetClusterState(NpgsqlDataSource pool, bool ignoreExpiration = false)
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
                            // While opening a new connector we might have refreshed the cluster state, check again
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
        CheckDisposed();

        var exceptions = new List<Exception>();

        var poolIndex = conn.Settings.LoadBalanceHosts ? GetRoundRobinIndex() : 0;

        var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
        var preferredType = GetTargetSessionAttributes(conn);
        var checkUnpreferred =
            preferredType == TargetSessionAttributes.PreferPrimary ||
            preferredType == TargetSessionAttributes.PreferStandby;

        var connector = await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
                        (checkUnpreferred ?
                            await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsOnline, poolIndex, exceptions, cancellationToken)
                            : null) ??
                        await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
                        (checkUnpreferred ?
                            await TryGet(conn, timeoutPerHost, async, preferredType, IsOnline, poolIndex, exceptions, cancellationToken)
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
        => throw new NpgsqlException("Npgsql bug: a connector was returned to " + nameof(NpgsqlMultiHostDataSource));

    internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        => throw new NpgsqlException("Npgsql bug: trying to get an idle connector from " + nameof(NpgsqlMultiHostDataSource));

    internal override ValueTask<NpgsqlConnector?> OpenNewConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        => throw new NpgsqlException("Npgsql bug: trying to open a new connector from " + nameof(NpgsqlMultiHostDataSource));

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

            var preferredType = GetTargetSessionAttributes(connection);
            // First try to get a valid preferred connector.
            if (TryGetValidConnector(list, preferredType, IsPreferred, out connector))
            {
                return true;
            }

            // Can't get valid preferred connector. Try to get an unpreferred connector, if supported.
            if ((preferredType == TargetSessionAttributes.PreferPrimary || preferredType == TargetSessionAttributes.PreferStandby)
                && TryGetValidConnector(list, preferredType, IsOnline, out connector))
            {
                return true;
            }

            connector = null;
            return false;
        }

        bool TryGetValidConnector(List<NpgsqlConnector> list, TargetSessionAttributes preferredType,
            Func<ClusterState, TargetSessionAttributes, bool> validationFunc, [NotNullWhen(true)] out NpgsqlConnector? connector)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                connector = list[i];
                var lastKnownState = GetClusterState(connector.Host, connector.Port, ignoreExpiration: true);
                Debug.Assert(lastKnownState != ClusterState.Unknown);
                if (validationFunc(lastKnownState, preferredType))
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
           (PostgresEnvironment.TargetSessionAttributes is { } s
               ? NpgsqlConnectionStringBuilder.ParseTargetSessionAttributes(s)
               : TargetSessionAttributes.Any);
}