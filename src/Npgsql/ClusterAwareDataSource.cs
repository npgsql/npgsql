// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using Npgsql.Internal;
// using Npgsql.Util;
//
// namespace Npgsql;
//
// /// <summary>
// /// For Uniform Load Balancing
// /// </summary>
// public class ClusterAwareDataSource: NpgsqlDataSource
// {
//     readonly NpgsqlDataSource[] _pools;
//     internal NpgsqlDataSource[] Pools => _pools;
//     /// <summary>
//     /// list of yb_server hosts
//     /// </summary>
//     protected List<string> _hosts = new List<string>();
//     volatile int _roundRobinIndex = -1;
//
//     internal ClusterAwareDataSource(NpgsqlConnectionStringBuilder settings, NpgsqlDataSourceConfiguration dataSourceConfig)
//         : base(settings, dataSourceConfig)
//     {
//         Console.WriteLine("Inside ClusterAwareDatasource");
//         
//         try
//         {
//             NpgsqlDataSource control = new UnpooledDataSource(settings, dataSourceConfig);
//             NpgsqlConnection controlConnection = NpgsqlConnection.FromDataSource(control);
//             controlConnection.Open();
//             GetCurrentServers(controlConnection);
//             controlConnection.Close();
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//         }
//             
//         _pools = new NpgsqlDataSource[_hosts.Count];
//         var i = 0; 
//         foreach(var host in _hosts)
//         {
//             var poolSettings = settings.Clone();
//             poolSettings.Host = host.ToString();
//
//             _pools[i] = settings.Pooling
//                 ? new PoolingDataSource(poolSettings, dataSourceConfig)
//                 : new UnpooledDataSource(poolSettings, dataSourceConfig);
//             i++;
//         }
//
//     }
//
//     internal void GetCurrentServers(NpgsqlConnection conn)
//     {
//         NpgsqlCommand QUERY_SERVER = new NpgsqlCommand("Select * from yb_servers()",conn);
//         NpgsqlDataReader reader = QUERY_SERVER.ExecuteReader();
//         while (reader.Read())
//         {
//             // Console.WriteLine("Hosts : {0}", reader.GetString(0));
//             _hosts.Add(reader.GetString(0));
//         }
//
//         foreach (var host in _hosts)
//         {
//             Console.WriteLine("Hosts: {0}",host);
//         }
//
//     }
//
//     internal override (int Total, int Idle, int Busy) Statistics { get; }
//
//     internal override async ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async,
//         CancellationToken cancellationToken)
//     {
//         CheckDisposed();
//
//         var exceptions = new List<Exception>();
//
//         var poolIndex = conn.Settings.LoadBalanceHosts ? GetRoundRobinIndex() : 0;
//
//         var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
//         var preferredType = GetTargetSessionAttributes(conn);
//         var checkUnpreferred = preferredType is TargetSessionAttributes.PreferPrimary or TargetSessionAttributes.PreferStandby;
//
//         var connector = await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
//                         (checkUnpreferred ?
//                             await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsOnline, poolIndex, exceptions, cancellationToken)
//                             : null) ??
//                         await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
//                         (checkUnpreferred ?
//                             await TryGet(conn, timeoutPerHost, async, preferredType, IsOnline, poolIndex, exceptions, cancellationToken)
//                             : null);
//
//         return connector ?? throw NoSuitableHostsException(exceptions);
//     }
//     
//     
//
//     internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
//         => throw new NpgsqlException("Npgsql bug: trying to get an idle connector from " + nameof(ClusterAwareDataSource));
//
//     internal override ValueTask<NpgsqlConnector?> OpenNewConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken) => throw new NotImplementedException();
//
//     internal override void Return(NpgsqlConnector connector) => throw new NotImplementedException();
//
//     internal override void Clear()
//     {
//         foreach (var pool in _pools)
//             pool.Clear();
//     }
//
//     internal override bool OwnsConnectors { get; }
//     
//     static bool IsPreferred(DatabaseState state, TargetSessionAttributes preferredType)
//         => state switch
//         {
//             DatabaseState.Offline => false,
//             DatabaseState.Unknown => true, // We will check compatibility again after refreshing the database state
//
//             DatabaseState.PrimaryReadWrite when preferredType is
//                     TargetSessionAttributes.Primary or
//                     TargetSessionAttributes.PreferPrimary or
//                     TargetSessionAttributes.ReadWrite
//                 => true,
//
//             DatabaseState.PrimaryReadOnly when preferredType is
//                     TargetSessionAttributes.Primary or
//                     TargetSessionAttributes.PreferPrimary or
//                     TargetSessionAttributes.ReadOnly
//                 => true,
//
//             DatabaseState.Standby when preferredType is
//                     TargetSessionAttributes.Standby or
//                     TargetSessionAttributes.PreferStandby or
//                     TargetSessionAttributes.ReadOnly
//                 => true,
//
//             _ => preferredType == TargetSessionAttributes.Any
//         };
//     
//     static bool IsOnline(DatabaseState state, TargetSessionAttributes preferredType)
//     {
//         Debug.Assert(preferredType is TargetSessionAttributes.PreferPrimary or TargetSessionAttributes.PreferStandby);
//         return state != DatabaseState.Offline;
//     }
//     
//     static NpgsqlException NoSuitableHostsException(IList<Exception> exceptions)
//         => exceptions.Count == 0
//             ? new NpgsqlException("No suitable host was found.")
//             : exceptions[0] is PostgresException firstException &&
//               exceptions.All(x => x is PostgresException ex && ex.SqlState == firstException.SqlState)
//                 ? firstException
//                 : new NpgsqlException("Unable to connect to a suitable host. Check inner exception for more details.",
//                     new AggregateException(exceptions));
//     
//     int GetRoundRobinIndex()
//     {
//         while (true)
//         {
//             var index = Interlocked.Increment(ref _roundRobinIndex);
//             if (index >= 0)
//                 return index % _pools.Length;
//
//             // Worst case scenario - we've wrapped around integer counter
//             if (index == int.MinValue)
//             {
//                 // This is the thread which wrapped around the counter - reset it to 0
//                 _roundRobinIndex = 0;
//                 return 0;
//             }
//
//             // This is not the thread which wrapped around the counter - just wait until it's 0 or more
//             var sw = new SpinWait();
//             while (_roundRobinIndex < 0)
//                 sw.SpinOnce();
//         }
//     }
//     
//     static TargetSessionAttributes GetTargetSessionAttributes(NpgsqlConnection connection)
//         => connection.Settings.TargetSessionAttributesParsed ??
//            (PostgresEnvironment.TargetSessionAttributes is { } s
//                ? NpgsqlConnectionStringBuilder.ParseTargetSessionAttributes(s)
//                : TargetSessionAttributes.Any);
//     
//     async ValueTask<NpgsqlConnector?> TryGetIdleOrNew(
//         NpgsqlConnection conn,
//         TimeSpan timeoutPerHost,
//         bool async,
//         TargetSessionAttributes preferredType, Func<DatabaseState, TargetSessionAttributes, bool> stateValidator,
//         int poolIndex,
//         IList<Exception> exceptions,
//         CancellationToken cancellationToken)
//     {
//         var pools = _pools;
//         for (var i = 0; i < pools.Length; i++)
//         {
//             var pool = pools[poolIndex];
//             poolIndex++;
//             if (poolIndex == pools.Length)
//                 poolIndex = 0;
//
//             var databaseState = pool.GetDatabaseState();
//             if (!stateValidator(databaseState, preferredType))
//                 continue;
//
//             NpgsqlConnector? connector = null;
//
//             try
//             {
//                 if (pool.TryGetIdleConnector(out connector))
//                 {
//                     if (databaseState == DatabaseState.Unknown)
//                     {
//                         databaseState = await connector.QueryDatabaseState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
//                         Debug.Assert(databaseState != DatabaseState.Unknown);
//                         if (!stateValidator(databaseState, preferredType))
//                         {
//                             pool.Return(connector);
//                             continue;
//                         }
//                     }
//
//                     return connector;
//                 }
//                 else
//                 {
//                     connector = await pool.OpenNewConnector(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
//                     if (connector is not null)
//                     {
//                         if (databaseState == DatabaseState.Unknown)
//                         {
//                             // While opening a new connector we might have refreshed the database state, check again
//                             databaseState = pool.GetDatabaseState();
//                             if (databaseState == DatabaseState.Unknown)
//                                 databaseState = await connector.QueryDatabaseState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
//                             Debug.Assert(databaseState != DatabaseState.Unknown);
//                             if (!stateValidator(databaseState, preferredType))
//                             {
//                                 pool.Return(connector);
//                                 continue;
//                             }
//                         }
//
//                         return connector;
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 exceptions.Add(ex);
//                 if (connector is not null)
//                     pool.Return(connector);
//             }
//         }
//
//         return null;
//     }
//     
//     async ValueTask<NpgsqlConnector?> TryGet(
//         NpgsqlConnection conn,
//         TimeSpan timeoutPerHost,
//         bool async,
//         TargetSessionAttributes preferredType,
//         Func<DatabaseState, TargetSessionAttributes, bool> stateValidator,
//         int poolIndex,
//         IList<Exception> exceptions,
//         CancellationToken cancellationToken)
//     {
//         var pools = _pools;
//         for (var i = 0; i < pools.Length; i++)
//         {
//             var pool = pools[poolIndex];
//             poolIndex++;
//             if (poolIndex == pools.Length)
//                 poolIndex = 0;
//
//             var databaseState = pool.GetDatabaseState();
//             if (!stateValidator(databaseState, preferredType))
//                 continue;
//
//             NpgsqlConnector? connector = null;
//
//             try
//             {
//                 connector = await pool.Get(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
//                 if (databaseState == DatabaseState.Unknown)
//                 {
//                     // Get might have opened a new physical connection and refreshed the database state, check again
//                     databaseState = pool.GetDatabaseState();
//                     if (databaseState == DatabaseState.Unknown)
//                         databaseState = await connector.QueryDatabaseState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
//
//                     Debug.Assert(databaseState != DatabaseState.Unknown);
//                     if (!stateValidator(databaseState, preferredType))
//                     {
//                         pool.Return(connector);
//                         continue;
//                     }
//                 }
//
//                 return connector;
//             }
//             catch (Exception ex)
//             {
//                 exceptions.Add(ex);
//                 if (connector is not null)
//                     pool.Return(connector);
//             }
//         }
//
//         return null;
//     }
//
// }


/*
 * ------------------------------
 * Approach 2
 * ------------------------------
 */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql;

/// <summary>
/// For Uniform Load Balancing
/// </summary>
public class ClusterAwareDataSource: NpgsqlDataSource
{
    private static ClusterAwareDataSource? instance;
    List<NpgsqlDataSource> _pools = new List<NpgsqlDataSource>();
    internal List<NpgsqlDataSource> Pools => _pools;
    /// <summary>
    /// list of yb_server hosts
    /// </summary>
    protected List<string>? _hosts = null;
    volatile int _roundRobinIndex = -1;
    DateTime _lastServerFetchTime = new DateTime(0);
    readonly double REFRESH_LIST_SECONDS = 300;
    List<string> unreachableHosts = new List<string>();
    /// <summary>
    /// Todo
    /// </summary>
    protected bool? UseHostColumn = null;

    Dictionary<string, int> _hostToNumConnMap = new Dictionary<string, int>();
    NpgsqlConnectionStringBuilder settings;
    internal NpgsqlDataSourceConfiguration dataSourceConfig;

    internal ClusterAwareDataSource(NpgsqlConnectionStringBuilder settings, NpgsqlDataSourceConfiguration dataSourceConfig)
        : base(settings, dataSourceConfig)
    {
        this.settings = settings;
        this.dataSourceConfig = dataSourceConfig;
        instance = this;
        try
        {
            NpgsqlDataSource control = new UnpooledDataSource(settings, dataSourceConfig);
            NpgsqlConnection controlConnection = NpgsqlConnection.FromDataSource(control);
            controlConnection.Open();
            CreatePool(controlConnection);
            controlConnection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    internal static ClusterAwareDataSource? GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// Create a new pool
    /// </summary>
    internal void CreatePool(NpgsqlConnection conn)
    {
        _hosts = GetCurrentServers(conn);
        foreach(var host in _hosts)
        {
            var poolSettings = settings.Clone();
            poolSettings.Host = host.ToString();

            _pools.Add(settings.Pooling
                ? new PoolingDataSource(poolSettings, dataSourceConfig)
                : new UnpooledDataSource(poolSettings, dataSourceConfig));
        }
    }

    /// <summary>
    /// gets the list of hosts
    /// </summary>
    /// <param name="conn"></param>
    protected List<string> GetCurrentServers(NpgsqlConnection conn)
    {
        NpgsqlCommand QUERY_SERVER = new NpgsqlCommand("Select * from yb_servers()",conn);
        NpgsqlDataReader reader = QUERY_SERVER.ExecuteReader();
        List<string> currentPrivateIps = new List<string>();
        List<string> currentPublicIps = new List<string>();
        var hostConnectedTo = conn.Host;
        
        Debug.Assert(hostConnectedTo != null, nameof(hostConnectedTo) + " != null");
        var isIpv6Addresses = hostConnectedTo.Contains(":");
        if (isIpv6Addresses) {
            hostConnectedTo = hostConnectedTo.Replace("[", "").Replace("]", "");
        }
        while (reader.Read())
        {
            var host = reader.GetString(0);
            var publicHost = reader.GetString(7);
            currentPrivateIps.Add(host);
            currentPublicIps.Add(publicHost);
            if (hostConnectedTo.Equals(host))
            {
                UseHostColumn = true;
            } else if (hostConnectedTo.Equals(publicHost)) {
                UseHostColumn = false;
            }
        }
        return GetPrivateOrPublicServers(currentPrivateIps, currentPublicIps);
    }
    
    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="privateHosts"></param>
    /// <param name="publicHosts"></param>
    /// <returns></returns>
    protected List<string> GetPrivateOrPublicServers(List<string> privateHosts, List<string> publicHosts) {
        if (UseHostColumn == null) {
            if (!publicHosts.Any())
            {
                UseHostColumn = true;
            }

            return privateHosts;
        }
        var currentHosts = (bool)UseHostColumn ? privateHosts : publicHosts;
        return currentHosts;
    }

    internal override bool Refresh(NpgsqlConnection conn)
    {
        if (!NeedsRefresh())
            return true;
        DateTime currTime = DateTime.Now;
        CreatePool(conn);
        if (_hosts == null) return false;
        _lastServerFetchTime = currTime;
        unreachableHosts.Clear();
        foreach (var h in _hosts) {
            if (!_hostToNumConnMap.ContainsKey(h)) {
                _hostToNumConnMap.Add(h, 0);
            }
        }
        return true;
    }
    
    

    internal override (int Total, int Idle, int Busy) Statistics { get; }

    internal override async ValueTask<NpgsqlConnector> Get(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async,
        CancellationToken cancellationToken)
    {
        CheckDisposed();

        var exceptions = new List<Exception>();

        var poolIndex = conn.Settings.LoadBalanceHosts ? GetRoundRobinIndex() : 0;

        var timeoutPerHost = timeout.IsSet ? timeout.CheckAndGetTimeLeft() : TimeSpan.Zero;
        var preferredType = GetTargetSessionAttributes(conn);
        var checkUnpreferred = preferredType is TargetSessionAttributes.PreferPrimary or TargetSessionAttributes.PreferStandby;

        var connector = await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
                        (checkUnpreferred ?
                            await TryGetIdleOrNew(conn, timeoutPerHost, async, preferredType, IsOnline, poolIndex, exceptions, cancellationToken)
                            : null) ??
                        await TryGet(conn, timeoutPerHost, async, preferredType, IsPreferred, poolIndex, exceptions, cancellationToken) ??
                        (checkUnpreferred ?
                            await TryGet(conn, timeoutPerHost, async, preferredType, IsOnline, poolIndex, exceptions, cancellationToken)
                            : null);

        return connector ?? throw NoSuitableHostsException(exceptions);
    }
    
    

    internal override bool TryGetIdleConnector([NotNullWhen(true)] out NpgsqlConnector? connector)
        => throw new NpgsqlException("Npgsql bug: trying to get an idle connector from " + nameof(ClusterAwareDataSource));

    internal override ValueTask<NpgsqlConnector?> OpenNewConnector(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken) => throw new NotImplementedException();

    internal override void Return(NpgsqlConnector connector) => throw new NotImplementedException();

    internal override void Clear()
    {
        foreach (var pool in _pools)
            pool.Clear();
    }

    internal override bool OwnsConnectors { get; }
    
    static bool IsPreferred(DatabaseState state, TargetSessionAttributes preferredType)
        => state switch
        {
            DatabaseState.Offline => false,
            DatabaseState.Unknown => true, // We will check compatibility again after refreshing the database state

            DatabaseState.PrimaryReadWrite when preferredType is
                    TargetSessionAttributes.Primary or
                    TargetSessionAttributes.PreferPrimary or
                    TargetSessionAttributes.ReadWrite
                => true,

            DatabaseState.PrimaryReadOnly when preferredType is
                    TargetSessionAttributes.Primary or
                    TargetSessionAttributes.PreferPrimary or
                    TargetSessionAttributes.ReadOnly
                => true,

            DatabaseState.Standby when preferredType is
                    TargetSessionAttributes.Standby or
                    TargetSessionAttributes.PreferStandby or
                    TargetSessionAttributes.ReadOnly
                => true,

            _ => preferredType == TargetSessionAttributes.Any
        };
    
    static bool IsOnline(DatabaseState state, TargetSessionAttributes preferredType)
    {
        Debug.Assert(preferredType is TargetSessionAttributes.PreferPrimary or TargetSessionAttributes.PreferStandby);
        return state != DatabaseState.Offline;
    }
    
    static NpgsqlException NoSuitableHostsException(IList<Exception> exceptions)
        => exceptions.Count == 0
            ? new NpgsqlException("No suitable host was found.")
            : exceptions[0] is PostgresException firstException &&
              exceptions.All(x => x is PostgresException ex && ex.SqlState == firstException.SqlState)
                ? firstException
                : new NpgsqlException("Unable to connect to a suitable host. Check inner exception for more details.",
                    new AggregateException(exceptions));

    internal override bool NeedsRefresh()
    {
        var currentTime = DateTime.Now;
        var diff = (currentTime - _lastServerFetchTime).TotalMilliseconds;
        return (diff > REFRESH_LIST_SECONDS);
    }
    
    int GetRoundRobinIndex()
    {
        while (true)
        {
            var index = Interlocked.Increment(ref _roundRobinIndex);
            if (index >= 0)
                return index % _pools.Count;

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
    
    static TargetSessionAttributes GetTargetSessionAttributes(NpgsqlConnection connection)
        => connection.Settings.TargetSessionAttributesParsed ??
           (PostgresEnvironment.TargetSessionAttributes is { } s
               ? NpgsqlConnectionStringBuilder.ParseTargetSessionAttributes(s)
               : TargetSessionAttributes.Any);
    
    async ValueTask<NpgsqlConnector?> TryGetIdleOrNew(
        NpgsqlConnection conn,
        TimeSpan timeoutPerHost,
        bool async,
        TargetSessionAttributes preferredType, Func<DatabaseState, TargetSessionAttributes, bool> stateValidator,
        int poolIndex,
        IList<Exception> exceptions,
        CancellationToken cancellationToken)
    {
        var pools = _pools;
        for (var i = 0; i < pools.Count; i++)
        {
            var pool = pools[poolIndex];
            poolIndex++;
            if (poolIndex == pools.Count)
                poolIndex = 0;

            var databaseState = pool.GetDatabaseState();
            if (!stateValidator(databaseState, preferredType))
                continue;

            NpgsqlConnector? connector = null;

            try
            {
                if (pool.TryGetIdleConnector(out connector))
                {
                    if (databaseState == DatabaseState.Unknown)
                    {
                        databaseState = await connector.QueryDatabaseState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                        Debug.Assert(databaseState != DatabaseState.Unknown);
                        if (!stateValidator(databaseState, preferredType))
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
                        if (databaseState == DatabaseState.Unknown)
                        {
                            // While opening a new connector we might have refreshed the database state, check again
                            databaseState = pool.GetDatabaseState();
                            if (databaseState == DatabaseState.Unknown)
                                databaseState = await connector.QueryDatabaseState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                            Debug.Assert(databaseState != DatabaseState.Unknown);
                            if (!stateValidator(databaseState, preferredType))
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
    
    async ValueTask<NpgsqlConnector?> TryGet(
        NpgsqlConnection conn,
        TimeSpan timeoutPerHost,
        bool async,
        TargetSessionAttributes preferredType,
        Func<DatabaseState, TargetSessionAttributes, bool> stateValidator,
        int poolIndex,
        IList<Exception> exceptions,
        CancellationToken cancellationToken)
    {
        var pools = _pools;
        for (var i = 0; i < pools.Count; i++)
        {
            var pool = pools[poolIndex];
            poolIndex++;
            if (poolIndex == pools.Count)
                poolIndex = 0;

            var databaseState = pool.GetDatabaseState();
            if (!stateValidator(databaseState, preferredType))
                continue;

            NpgsqlConnector? connector = null;

            try
            {
                connector = await pool.Get(conn, new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);
                if (databaseState == DatabaseState.Unknown)
                {
                    // Get might have opened a new physical connection and refreshed the database state, check again
                    databaseState = pool.GetDatabaseState();
                    if (databaseState == DatabaseState.Unknown)
                        databaseState = await connector.QueryDatabaseState(new NpgsqlTimeout(timeoutPerHost), async, cancellationToken);

                    Debug.Assert(databaseState != DatabaseState.Unknown);
                    if (!stateValidator(databaseState, preferredType))
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

}
