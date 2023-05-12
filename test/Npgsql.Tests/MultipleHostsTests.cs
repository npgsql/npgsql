using Npgsql.Internal;
using Npgsql.Tests.Support;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using static Npgsql.Tests.Support.MockState;
using static Npgsql.Tests.TestUtil;
using IsolationLevel = System.Transactions.IsolationLevel;
using TransactionStatus = Npgsql.Internal.TransactionStatus;

namespace Npgsql.Tests;

[Timeout(60000)]
public class MultipleHostsTests : TestBase
{
    static readonly object[] MyCases =
    {
        new object[] { "standby",        new[] { Primary,         Standby         }, 1 },
        new object[] { "standby",        new[] { PrimaryReadOnly, Standby         }, 1 },
        new object[] { "prefer-standby", new[] { Primary,         Standby         }, 1 },
        new object[] { "prefer-standby", new[] { PrimaryReadOnly, Standby         }, 1 },
        new object[] { "prefer-standby", new[] { Primary,         Primary         }, 0 },
        new object[] { "primary",        new[] { Standby,         Primary         }, 1 },
        new object[] { "primary",        new[] { Standby,         PrimaryReadOnly }, 1 },
        new object[] { "prefer-primary", new[] { Standby,         Primary         }, 1 },
        new object[] { "prefer-primary", new[] { Standby,         PrimaryReadOnly }, 1 },
        new object[] { "prefer-primary", new[] { Standby,         Standby         }, 0 },
        new object[] { "any",            new[] { Standby,         Primary         }, 0 },
        new object[] { "any",            new[] { Primary,         Standby         }, 0 },
        new object[] { "any",            new[] { PrimaryReadOnly, Standby         }, 0 },
        new object[] { "read-write",     new[] { Standby,         Primary         }, 1 },
        new object[] { "read-write",     new[] { PrimaryReadOnly, Primary         }, 1 },
        new object[] { "read-only",      new[] { Primary,         Standby         }, 1 },
        new object[] { "read-only",      new[] { PrimaryReadOnly, Standby         }, 0 }
    };

    [Test]
    [TestCaseSource(nameof(MyCases))]
    public async Task Connect_to_correct_host_pooled(string targetSessionAttributes, MockState[] servers, int expectedServer)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            TargetSessionAttributes = targetSessionAttributes,
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            Pooling = true
        };

        using var pool = CreateTempPool(connectionStringBuilder, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);

        Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

        for (var i = 0; i <= expectedServer; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test]
    [TestCaseSource(nameof(MyCases))]
    public async Task Connect_to_correct_host_unpooled(string targetSessionAttributes, MockState[] servers, int expectedServer)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            TargetSessionAttributes = targetSessionAttributes,
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            Pooling = false
        };

        using var pool = CreateTempPool(connectionStringBuilder, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);

        Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

        for (var i = 0; i <= expectedServer; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test]
    [TestCaseSource(nameof(MyCases))]
    public async Task Connect_to_correct_host_with_available_idle(
        string targetSessionAttributes, MockState[] servers, int expectedServer)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        // First, open and close a connection with the TargetSessionAttributes matching the first server.
        // This ensures wew have an idle connection in the pool.
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            TargetSessionAttributes = servers[0] switch
            {
                Primary => "read-write",
                PrimaryReadOnly => "read-only",
                Standby => "standby",
                _ => throw new ArgumentOutOfRangeException()
            },
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
        };

        using var pool = CreateTempPool(connectionStringBuilder, out var connectionString);
        await using (_ = await OpenConnectionAsync(connectionString))
        {
            // Do nothing, close to have an idle connection in the pool.
        }

        // Now connect with the test TargetSessionAttributes
        connectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            TargetSessionAttributes = targetSessionAttributes.ToString()
        }.ConnectionString;

        await using var conn = await OpenConnectionAsync(connectionString);

        Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

        for (var i = 0; i <= expectedServer; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test]
    [TestCase("standby",   new[] { Primary,         Primary })]
    [TestCase("primary",   new[] { Standby,         Standby })]
    [TestCase("read-write", new[] { PrimaryReadOnly, Standby })]
    [TestCase("read-only",  new[] { Primary,         Primary })]
    public async Task Valid_host_not_found(string targetSessionAttributes, MockState[] servers)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            TargetSessionAttributes = targetSessionAttributes
        };

        using var pool = CreateTempPool(connectionStringBuilder.ConnectionString, out var connectionString);

        var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await OpenConnectionAsync(connectionString))!;
        Assert.That(exception.Message, Is.EqualTo("No suitable host was found."));
        Assert.That(exception.InnerException, Is.Null);

        for (var i = 0; i < servers.Length; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test, Platform(Exclude = "MacOsX", Reason = "#3786")]
    public void All_hosts_are_down()
    {
        var endpoint = new IPEndPoint(IPAddress.Loopback, 0);

        using var socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket1.Bind(endpoint);
        var localEndPoint1 = (IPEndPoint)socket1.LocalEndPoint!;

        using var socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket2.Bind(endpoint);
        var localEndPoint2 = (IPEndPoint)socket2.LocalEndPoint!;

        // Note that we Bind (to reserve the port), but do not Listen - connection attempts will fail.

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = $"{localEndPoint1.Address}:{localEndPoint1.Port},{localEndPoint2.Address}:{localEndPoint2.Port}"
        }.ConnectionString;

        var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await OpenConnectionAsync(connectionString))!;
        var aggregateException = (AggregateException)exception.InnerException!;
        Assert.That(aggregateException.InnerExceptions, Has.Count.EqualTo(2));

        for (var i = 0; i < aggregateException.InnerExceptions.Count; i++)
        {
            Assert.That(aggregateException.InnerExceptions[i], Is.TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<SocketException>()
                .With.InnerException.Property(nameof(SocketException.SocketErrorCode)).EqualTo(SocketError.ConnectionRefused));
        }
    }

    [Test]
    public async Task All_hosts_are_unavailable(
        [Values] bool pooling,
        [Values(PostgresErrorCodes.InvalidCatalogName, PostgresErrorCodes.CannotConnectNow)] string errorCode)
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary, startupErrorCode: errorCode);
        await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby, startupErrorCode: errorCode);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            TargetSessionAttributes = "any",
            Pooling = pooling,
        };

        using var _ = CreateTempPool(builder.ConnectionString, out var connectionString);

        var ex = Assert.ThrowsAsync<PostgresException>(async () => await OpenConnectionAsync(connectionString))!;
        Assert.That(ex.SqlState, Is.EqualTo(errorCode));
    }

    [Test]
    public async Task First_host_is_down()
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var endpoint = new IPEndPoint(IPAddress.Loopback, 0);
        socket.Bind(endpoint);
        var localEndPoint = (IPEndPoint)socket.LocalEndPoint!;
        // Note that we Bind (to reserve the port), but do not Listen - connection attempts will fail.

        await using var postmaster = PgPostmasterMock.Start(state: Primary);

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = $"{localEndPoint.Address}:{localEndPoint.Port},{postmaster.Host}:{postmaster.Port}",
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading
        }.ConnectionString;

        await using var conn = await OpenConnectionAsync(connectionString);
        Assert.That(conn.Port, Is.EqualTo(postmaster.Port));
    }

    [Test]
    [TestCase("any")]
    [TestCase("primary")]
    [TestCase("standby")]
    [TestCase("prefer-primary")]
    [TestCase("prefer-standby")]
    [TestCase("read-write")]
    [TestCase("read-only")]
    public async Task TargetSessionAttributes_with_single_host(string targetSessionAttributes)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            TargetSessionAttributes = targetSessionAttributes
        }.ConnectionString;

        if (targetSessionAttributes == "any")
        {
            await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
            using var pool = CreateTempPool(postmasterMock.ConnectionString, out connectionString);
            await using var conn = await OpenConnectionAsync(connectionString);
            _ = await postmasterMock.WaitForServerConnection();
        }
        else
        {
            Assert.That(() => OpenConnectionAsync(connectionString), Throws.Exception.TypeOf<NotSupportedException>());
        }
    }

    [Test]
    public void TargetSessionAttributes_default_is_null()
        => Assert.That(new NpgsqlConnectionStringBuilder().TargetSessionAttributes, Is.Null);

    [Test, NonParallelizable]
    public async Task TargetSessionAttributes_uses_environment_variable()
    {
        using var envVarResetter = SetEnvironmentVariable("PGTARGETSESSIONATTRS", "prefer-standby");

        await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading
        };

        Assert.That(builder.TargetSessionAttributes, Is.Null);

        using var _ = CreateTempPool(builder.ConnectionString, out var connectionString);

        await using var conn = await OpenConnectionAsync(connectionString);
        Assert.That(conn.Port, Is.EqualTo(standbyPostmaster.Port));
    }

    [Test]
    public void TargetSessionAttributes_invalid_throws()
        => Assert.Throws<ArgumentException>(() =>
            new NpgsqlConnectionStringBuilder
            {
                TargetSessionAttributes = nameof(TargetSessionAttributes_invalid_throws)
            });

    [Test]
    public void HostRecheckSeconds_default_value()
    {
        var builder = new NpgsqlConnectionStringBuilder();
        Assert.That(builder.HostRecheckSeconds, Is.EqualTo(10));
        Assert.That(builder.HostRecheckSecondsTranslated, Is.EqualTo(TimeSpan.FromSeconds(10)));
    }

    [Test]
    public void HostRecheckSeconds_zero_value()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            HostRecheckSeconds = 0,
        };
        Assert.That(builder.HostRecheckSeconds, Is.EqualTo(0));
        Assert.That(builder.HostRecheckSecondsTranslated, Is.EqualTo(TimeSpan.FromSeconds(-1)));
    }

    [Test]
    public void HostRecheckSeconds_invalid_throws()
        => Assert.Throws<ArgumentException>(() =>
            new NpgsqlConnectionStringBuilder
            {
                HostRecheckSeconds = -1
            });

    [Test]
    public async Task Connect_with_load_balancing()
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

        var defaultCsb = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            MaxPoolSize = 1,
            LoadBalanceHosts = true,
        };

        using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

        NpgsqlConnector firstConnector;
        NpgsqlConnector secondConnector;

        await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            firstConnector = firstConnection.Connector!;
        }

        await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            secondConnector = secondConnection.Connector!;
        }

        Assert.AreNotSame(firstConnector, secondConnector);

        await using (var firstBalancedConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            Assert.AreSame(firstConnector, firstBalancedConnection.Connector);
        }

        await using (var secondBalancedConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            Assert.AreSame(secondConnector, secondBalancedConnection.Connector);
        }

        await using (var thirdBalancedConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            Assert.AreSame(firstConnector, thirdBalancedConnection.Connector);
        }
    }

    [Test]
    public async Task Connect_without_load_balancing()
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

        var defaultCsb = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            MaxPoolSize = 1,
            LoadBalanceHosts = false,
        };

        using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

        NpgsqlConnector firstConnector;
        NpgsqlConnector secondConnector;

        await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            firstConnector = firstConnection.Connector!;
        }
        await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            Assert.AreSame(firstConnector, secondConnection.Connector);
        }
        await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
        await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            secondConnector = secondConnection.Connector!;
        }

        Assert.AreNotSame(firstConnector, secondConnector);

        await using (var firstUnbalancedConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            Assert.AreSame(firstConnector, firstUnbalancedConnection.Connector);
        }

        await using (var secondUnbalancedConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            Assert.AreSame(firstConnector, secondUnbalancedConnection.Connector);
        }
    }

    [Test]
    public async Task Connect_state_changing_hosts([Values] bool alwaysCheckHostState)
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(state: Standby);

        var defaultCsb = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            MaxPoolSize = 1,
            HostRecheckSeconds = alwaysCheckHostState ? 0 : int.MaxValue,
            TargetSessionAttributes = "prefer-primary",
            NoResetOnClose = true,
        };

        using var _ = CreateTempPool(defaultCsb.ConnectionString, out var defaultConnectionString);

        NpgsqlConnector firstConnector;
        NpgsqlConnector secondConnector;
        var firstServerTask = Task.Run(async () =>
        {
            var server = await primaryPostmaster.WaitForServerConnection();
            if (!alwaysCheckHostState)
                return;

            // If we always check the host, we will send the request for the state
            // even though we got one while opening the connection
            await server.SendMockState(Primary);

            // Update the state after a 'failover'
            await server.SendMockState(Standby);
        });
        var secondServerTask = Task.Run(async () =>
        {
            var server = await standbyPostmaster.WaitForServerConnection();
            if (!alwaysCheckHostState)
                return;

            // If we always check the host, we will send the request for the state
            // even though we got one while opening the connection
            await server.SendMockState(Standby);

            // As TargetSessionAttributes is 'prefer', it does another cycle for the 'unpreferred'
            await server.SendMockState(Standby);
            // Update the state after a 'failover'
            await server.SendMockState(Primary);
        });

        await using (var firstConnection = await OpenConnectionAsync(defaultConnectionString))
        await using (var secondConnection = await OpenConnectionAsync(defaultConnectionString))
        {
            firstConnector = firstConnection.Connector!;
            secondConnector = secondConnection.Connector!;
        }

        await using var thirdConnection = await OpenConnectionAsync(defaultConnectionString);
        Assert.AreSame(alwaysCheckHostState ? secondConnector : firstConnector, thirdConnection.Connector);

        await firstServerTask;
        await secondServerTask;
    }

    [Test]
    public void Cluster_state_cache_basic()
    {
        const string host = nameof(Cluster_state_cache_basic);
        var timeStamp = DateTime.UtcNow;

        ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.PrimaryReadWrite, timeStamp, TimeSpan.Zero);
        Assert.AreEqual(ClusterState.PrimaryReadWrite, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));

        // Update with the same timestamp - shouldn't change anything
        ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.Standby, timeStamp, TimeSpan.Zero);
        Assert.AreEqual(ClusterState.PrimaryReadWrite, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));
        // Update with a new timestamp
        timeStamp = timeStamp.AddSeconds(1);
        ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.PrimaryReadOnly, timeStamp, TimeSpan.Zero);
        Assert.AreEqual(ClusterState.PrimaryReadOnly, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));

        // Expired state returns as Unknown (depending on ignoreExpiration)
        timeStamp = timeStamp.AddSeconds(1);
        ClusterStateCache.UpdateClusterState(host, 5432, ClusterState.PrimaryReadWrite, timeStamp, TimeSpan.FromSeconds(-1));
        Assert.AreEqual(ClusterState.Unknown, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: false));
        Assert.AreEqual(ClusterState.PrimaryReadWrite, ClusterStateCache.GetClusterState(host, 5432, ignoreExpiration: true));
    }

    [Test]
    public async Task Cluster_offline_state_on_connection_failure()
    {
        await using var server = PgPostmasterMock.Start(ConnectionString, startupErrorCode: PostgresErrorCodes.ConnectionFailure);
        var csb = new NpgsqlConnectionStringBuilder(server.ConnectionString);
        await using var conn = new NpgsqlConnection(csb.ConnectionString);

        var ex = Assert.ThrowsAsync<PostgresException>(conn.OpenAsync)!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.ConnectionFailure));

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Offline));
    }

    [Test]
    public async Task Cluster_unknown_state_on_connection_authentication_failure()
    {
        await using var server = PgPostmasterMock.Start(ConnectionString, startupErrorCode: PostgresErrorCodes.InvalidAuthorizationSpecification);
        var csb = new NpgsqlConnectionStringBuilder(server.ConnectionString);
        await using var conn = new NpgsqlConnection(csb.ConnectionString);

        var ex = Assert.ThrowsAsync<PostgresException>(conn.OpenAsync)!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.InvalidAuthorizationSpecification));

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
    }

    [Test]
    public async Task Cluster_offline_state_on_query_execution_pg_critical_failure()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmaster.ConnectionString);
        await using var conn = await OpenConnectionAsync(csb);
        await using var anotherConn = await OpenConnectionAsync(csb);
        await anotherConn.CloseAsync();

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(2));

        var server = await postmaster.WaitForServerConnection();
        await server.WriteErrorResponse(PostgresErrorCodes.CrashShutdown).FlushAsync();

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.CrashShutdown));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Offline));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(0));
    }

    [Test, NonParallelizable]
    public async Task Cluster_offline_state_on_query_execution_pg_non_critical_failure()
    {
        PoolManager.Reset();
        // We reset the cluster's state
        // Because other tests might have marked the host as disabled
        ClusterStateCache.Clear();

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
        await using var conn = await OpenConnectionAsync(csb);

        // Starting with PG14 we get the cluster's state from PG automatically
        var expectedState = conn.PostgreSqlVersion.Major > 13 ? ClusterState.PrimaryReadWrite : ClusterState.Unknown;

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(expectedState));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(1));

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync("SELECT abc"))!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.UndefinedColumn));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(expectedState));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Cluster_offline_state_on_query_execution_IOException()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmaster.ConnectionString);
        await using var conn = await OpenConnectionAsync(csb);
        await using var anotherConn = await OpenConnectionAsync(csb);
        await anotherConn.CloseAsync();

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(2));

        var server = await postmaster.WaitForServerConnection();
        server.Close();

        var ex = Assert.ThrowsAsync<NpgsqlException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.InnerException, Is.InstanceOf<IOException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Offline));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task Cluster_offline_state_on_query_execution_TimeoutException()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmaster.ConnectionString)
        {
            CommandTimeout = 1,
            CancellationTimeout = 1,
        };
        await using var conn = await OpenConnectionAsync(csb);
        await using var anotherConn = await OpenConnectionAsync(csb);
        await anotherConn.CloseAsync();

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(2));

        var ex = Assert.ThrowsAsync<NpgsqlException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Offline));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task Cluster_unknown_state_on_query_execution_TimeoutException_with_disabled_cancellation()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmaster.ConnectionString)
        {
            CommandTimeout = 1,
            CancellationTimeout = -1,
        };
        await using var conn = await OpenConnectionAsync(csb);
        await using var anotherConn = await OpenConnectionAsync(csb);
        await anotherConn.CloseAsync();

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(2));

        var ex = Assert.ThrowsAsync<NpgsqlException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Cluster_unknown_state_on_query_execution_cancellation_with_disabled_cancellation_timeout()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmaster.ConnectionString)
        {
            CommandTimeout = 30,
            CancellationTimeout = -1,
        };
        await using var conn = await OpenConnectionAsync(csb);
        await using var anotherConn = await OpenConnectionAsync(csb);
        await anotherConn.CloseAsync();

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(2));

        using var cts = new CancellationTokenSource();

        var query = conn.ExecuteNonQueryAsync("SELECT 1", cancellationToken: cts.Token);
        cts.Cancel();
        var ex = Assert.ThrowsAsync<OperationCanceledException>(async () => await query)!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Cluster_unknown_state_on_query_execution_TimeoutException_with_cancellation_failure()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmaster.ConnectionString)
        {
            CommandTimeout = 1,
            CancellationTimeout = 0,
        };
        await using var conn = await OpenConnectionAsync(csb);

        var state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(1));

        var server = await postmaster.WaitForServerConnection();

        var query = conn.ExecuteNonQueryAsync("SELECT 1");

        await postmaster.WaitForCancellationRequest();
        await server.WriteCancellationResponse().WriteReadyForQuery().FlushAsync();

        var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await query)!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        state = ClusterStateCache.GetClusterState(csb.Host!, csb.Port, ignoreExpiration: false);
        Assert.That(state, Is.EqualTo(ClusterState.Unknown));
        Assert.That(conn.Pool.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Clear_pool_one_host_only_on_admin_shutdown()
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(ConnectionString, state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(ConnectionString, state: Standby);
        var csb = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            TargetSessionAttributes = "prefer-primary",
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            MaxPoolSize = 2,
        };
        await using var primaryConn = await OpenConnectionAsync(csb);
        await using var anotherPrimaryConn = await OpenConnectionAsync(csb);
        await using var standbyConn = await OpenConnectionAsync(csb);
        await anotherPrimaryConn.CloseAsync();
        await standbyConn.CloseAsync();

        Assert.That(ClusterStateCache.GetClusterState(primaryPostmaster.Host, primaryPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.PrimaryReadWrite));
        Assert.That(ClusterStateCache.GetClusterState(standbyPostmaster.Host, standbyPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Standby));
        Assert.That(primaryConn.Pool.Statistics.Total, Is.EqualTo(3));

        var server = await primaryPostmaster.WaitForServerConnection();
        await server.WriteErrorResponse(PostgresErrorCodes.AdminShutdown).FlushAsync();

        var ex = Assert.ThrowsAsync<PostgresException>(() => primaryConn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.AdminShutdown));
        Assert.That(primaryConn.State, Is.EqualTo(ConnectionState.Closed));

        Assert.That(ClusterStateCache.GetClusterState(primaryPostmaster.Host, primaryPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Offline));
        Assert.That(ClusterStateCache.GetClusterState(standbyPostmaster.Host, standbyPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Standby));
        Assert.That(primaryConn.Pool.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    [TestCase("any", true)]
    [TestCase("primary", true)]
    [TestCase("standby", false)]
    [TestCase("prefer-primary", true)]
    [TestCase("prefer-standby", false)]
    [TestCase("read-write", true)]
    [TestCase("read-only", false)]
    public async Task Transaction_enlist_reuses_connection(string targetSessionAttributes, bool primary)
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(ConnectionString, state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(ConnectionString, state: Standby);
        var csb = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
            TargetSessionAttributes = targetSessionAttributes,
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            MaxPoolSize = 10,
        };

        using var _ = CreateTempPool(csb, out var connString);

        using var scope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled);

        var query1Task = Query(connString);

        var server = primary
            ? await primaryPostmaster.WaitForServerConnection()
            : await standbyPostmaster.WaitForServerConnection();

        await server
            .WriteCommandComplete()
            .WriteReadyForQuery(TransactionStatus.InTransactionBlock)
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteNoData()
            .WriteCommandComplete()
            .WriteReadyForQuery(TransactionStatus.InTransactionBlock)
            .FlushAsync();
        await query1Task;

        var query2Task = Query(connString);
        await server
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteNoData()
            .WriteCommandComplete()
            .WriteReadyForQuery(TransactionStatus.InTransactionBlock)
            .FlushAsync();
        await query2Task;

        await server
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .FlushAsync();
        scope.Complete();

        async Task Query(string connectionString)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            await cmd.ExecuteNonQueryAsync();
        }
    }

    [Test]
    public async Task Primary_host_failover_can_connect()
    {
        await using var firstPostmaster = PgPostmasterMock.Start(ConnectionString, state: Primary);
        await using var secondPostmaster = PgPostmasterMock.Start(ConnectionString, state: Standby);
        var csb = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(firstPostmaster, secondPostmaster),
            TargetSessionAttributes = "primary",
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            HostRecheckSeconds = 5
        };

        await using var conn = await OpenConnectionAsync(csb);
        Assert.That(conn.Port, Is.EqualTo(firstPostmaster.Port));
        var firstServer = await firstPostmaster.WaitForServerConnection();
        await firstServer
            .WriteErrorResponse(PostgresErrorCodes.AdminShutdown)
            .FlushAsync();

        var failoverEx = Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(failoverEx.SqlState, Is.EqualTo(PostgresErrorCodes.AdminShutdown));

        var noHostFoundEx = Assert.ThrowsAsync<NpgsqlException>(async () => await conn.OpenAsync())!;
        Assert.That(noHostFoundEx.Message, Is.EqualTo("No suitable host was found."));

        Assert.That(ClusterStateCache.GetClusterState(firstPostmaster.Host, firstPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Offline));
        Assert.That(ClusterStateCache.GetClusterState(secondPostmaster.Host, secondPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Standby));

        firstPostmaster.State = Standby;
        secondPostmaster.State = Primary;
        var secondServer = await secondPostmaster.WaitForServerConnection();
        await secondServer.SendMockState(Primary);

        await Task.Delay(TimeSpan.FromSeconds(10));
        Assert.That(ClusterStateCache.GetClusterState(firstPostmaster.Host, firstPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Unknown));
        Assert.That(ClusterStateCache.GetClusterState(secondPostmaster.Host, secondPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Unknown));

        await conn.OpenAsync();
        Assert.That(conn.Port, Is.EqualTo(secondPostmaster.Port));
        Assert.That(ClusterStateCache.GetClusterState(firstPostmaster.Host, firstPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.Standby));
        Assert.That(ClusterStateCache.GetClusterState(secondPostmaster.Host, secondPostmaster.Port, ignoreExpiration: false),
            Is.EqualTo(ClusterState.PrimaryReadWrite));
    }

    [Test, Timeout(10000), NonParallelizable]
    public void IntegrationTest([Values] bool loadBalancing, [Values] bool alwaysCheckHostState)
    {
        PoolManager.Reset();
        // We reset the cluster's state for multiple hosts
        // Because other tests might have marked some of the hosts as disabled
        ClusterStateCache.Clear();

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Host = "localhost,127.0.0.1",
            Pooling = true,
            MaxPoolSize = 2,
            LoadBalanceHosts = loadBalancing,
            HostRecheckSeconds = alwaysCheckHostState ? 0 : 10,
        };

        var queriesDone = 0;

        var clientsTask = Task.WhenAll(
            Client(csb, "any"),
            Client(csb, "primary"),
            Client(csb, "prefer-primary"),
            Client(csb, "prefer-standby"),
            Client(csb, "read-write"));

        var onlyStandbyClient = Client(csb, "standby");
        var readOnlyClient = Client(csb, "read-only");

        Assert.DoesNotThrowAsync(() => clientsTask);
        Assert.ThrowsAsync<NpgsqlException>(() => onlyStandbyClient);
        Assert.ThrowsAsync<NpgsqlException>(() => readOnlyClient);
        Assert.AreEqual(125, queriesDone);

        Assert.AreEqual(8, PoolManager.Pools.Count(x => x.Key is not null));

        PoolManager.Reset();

        Task Client(NpgsqlConnectionStringBuilder csb, string targetSessionAttributes)
        {
            csb = csb.Clone();
            csb.TargetSessionAttributes = targetSessionAttributes;
            var tasks = new List<Task>(5);

            for (var i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(() => Query(csb.ToString())));
            }

            return Task.WhenAll(tasks);
        }

        async Task Query(string connectionString)
        {
            await using var conn = new NpgsqlConnection(connectionString);
            for (var i = 0; i < 5; i++)
            {
                await conn.OpenAsync();
                await conn.ExecuteNonQueryAsync("SELECT 1");
                await conn.CloseAsync();
                Interlocked.Increment(ref queriesDone);
            }
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/5055")]
    [NonParallelizable] // Disables sql rewriting
    public async Task Multiple_hosts_with_disabled_sql_rewriting()
    {
        using var _ = DisableSqlRewriting();

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Host = "localhost,127.0.0.1",
            Pooling = true,
            HostRecheckSeconds = 0
        };
        await using var conn = await OpenConnectionAsync(csb);
    }

    static string MultipleHosts(params PgPostmasterMock[] postmasters)
        => string.Join(",", postmasters.Select(p => $"{p.Host}:{p.Port}"));

    class DisposableWrapper : IAsyncDisposable
    {
        readonly IEnumerable<IAsyncDisposable> _disposables;

        public DisposableWrapper(IEnumerable<IAsyncDisposable> disposables) => _disposables = disposables;

        public async ValueTask DisposeAsync()
        {
            foreach (var disposable in _disposables)
                await disposable.DisposeAsync();
        }
    }
}
