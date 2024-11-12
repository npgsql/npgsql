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
using Npgsql.Properties;
using static Npgsql.Tests.Support.MockState;
using static Npgsql.Tests.TestUtil;
using IsolationLevel = System.Transactions.IsolationLevel;
using TransactionStatus = Npgsql.Internal.TransactionStatus;

namespace Npgsql.Tests;

#pragma warning disable CS0618

public class MultipleHostsTests : TestBase
{
    static readonly object[] MyCases =
    [
        new object[] { TargetSessionAttributes.Standby,        new[] { Primary,         Standby         }, 1 },
        new object[] { TargetSessionAttributes.Standby,        new[] { PrimaryReadOnly, Standby         }, 1 },
        new object[] { TargetSessionAttributes.PreferStandby,  new[] { Primary,         Standby         }, 1 },
        new object[] { TargetSessionAttributes.PreferStandby,  new[] { PrimaryReadOnly, Standby         }, 1 },
        new object[] { TargetSessionAttributes.PreferStandby,  new[] { Primary,         Primary         }, 0 },
        new object[] { TargetSessionAttributes.Primary,        new[] { Standby,         Primary         }, 1 },
        new object[] { TargetSessionAttributes.Primary,        new[] { Standby,         PrimaryReadOnly }, 1 },
        new object[] { TargetSessionAttributes.PreferPrimary,  new[] { Standby,         Primary         }, 1 },
        new object[] { TargetSessionAttributes.PreferPrimary,  new[] { Standby,         PrimaryReadOnly }, 1 },
        new object[] { TargetSessionAttributes.PreferPrimary,  new[] { Standby,         Standby         }, 0 },
        new object[] { TargetSessionAttributes.Any,            new[] { Standby,         Primary         }, 0 },
        new object[] { TargetSessionAttributes.Any,            new[] { Primary,         Standby         }, 0 },
        new object[] { TargetSessionAttributes.Any,            new[] { PrimaryReadOnly, Standby         }, 0 },
        new object[] { TargetSessionAttributes.ReadWrite,      new[] { Standby,         Primary         }, 1 },
        new object[] { TargetSessionAttributes.ReadWrite,      new[] { PrimaryReadOnly, Primary         }, 1 },
        new object[] { TargetSessionAttributes.ReadOnly,       new[] { Primary,         Standby         }, 1 },
        new object[] { TargetSessionAttributes.ReadOnly,       new[] { PrimaryReadOnly, Standby         }, 0 }
    ];

    [Test]
    [TestCaseSource(nameof(MyCases))]
    public async Task Connect_to_correct_host_pooled(TargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            Pooling = true
        };

        await using var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString)
            .BuildMultiHost();
        await using var conn = await dataSource.OpenConnectionAsync(targetSessionAttributes);

        Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

        for (var i = 0; i <= expectedServer; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test]
    [TestCaseSource(nameof(MyCases))]
    public async Task Connect_to_correct_host_unpooled(TargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            Pooling = false
        };

        await using var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString)
            .BuildMultiHost();
        await using var conn = await dataSource.OpenConnectionAsync(targetSessionAttributes);

        Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

        for (var i = 0; i <= expectedServer; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test]
    [TestCaseSource(nameof(MyCases))]
    public async Task Connect_to_correct_host_with_available_idle(
        TargetSessionAttributes targetSessionAttributes, MockState[] servers, int expectedServer)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        // First, open and close a connection with the TargetSessionAttributes matching the first server.
        // This ensures wew have an idle connection in the pool.
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
        };

        await using var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString)
            .BuildMultiHost();
        var idleConnTargetSessionAttributes = servers[0] switch
        {
            Primary => TargetSessionAttributes.ReadWrite,
            PrimaryReadOnly => TargetSessionAttributes.ReadOnly,
            Standby => TargetSessionAttributes.Standby,
            _ => throw new ArgumentOutOfRangeException()
        };
        await using (_ = await dataSource.OpenConnectionAsync(idleConnTargetSessionAttributes))
        {
            // Do nothing, close to have an idle connection in the pool.
        }

        // Now connect with the test TargetSessionAttributes

        await using var conn = await dataSource.OpenConnectionAsync(targetSessionAttributes);

        Assert.That(conn.Port, Is.EqualTo(postmasters[expectedServer].Port));

        for (var i = 0; i <= expectedServer; i++)
            _ = await postmasters[i].WaitForServerConnection();
    }

    [Test]
    [TestCase(TargetSessionAttributes.Standby,   new[] { Primary,         Primary })]
    [TestCase(TargetSessionAttributes.Primary,   new[] { Standby,         Standby })]
    [TestCase(TargetSessionAttributes.ReadWrite, new[] { PrimaryReadOnly, Standby })]
    [TestCase(TargetSessionAttributes.ReadOnly,  new[] { Primary,         Primary })]
    public async Task Valid_host_not_found(TargetSessionAttributes targetSessionAttributes, MockState[] servers)
    {
        var postmasters = servers.Select(s => PgPostmasterMock.Start(state: s)).ToArray();
        await using var __ = new DisposableWrapper(postmasters);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = MultipleHosts(postmasters),
            ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
        };

        await using var dataSource = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString)
            .BuildMultiHost();

        var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await dataSource.OpenConnectionAsync(targetSessionAttributes))!;
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
        using var dataSource = new NpgsqlDataSourceBuilder(connectionString).BuildMultiHost();

        var exception = Assert.ThrowsAsync<NpgsqlException>(async () => await dataSource.OpenConnectionAsync(TargetSessionAttributes.Any))!;
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
            Pooling = pooling,
        };

        await using var dataSource = new NpgsqlDataSourceBuilder(builder.ConnectionString).BuildMultiHost();

        var ex = Assert.ThrowsAsync<PostgresException>(async () => await dataSource.OpenConnectionAsync(TargetSessionAttributes.Any))!;
        Assert.That(ex.SqlState, Is.EqualTo(errorCode));
    }

    [Test]
    [Platform(Exclude = "MacOsX", Reason = "Flaky in CI on Mac")]
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

        await using var dataSource = new NpgsqlDataSourceBuilder(connectionString).BuildMultiHost();

        await using var conn = await dataSource.OpenConnectionAsync(TargetSessionAttributes.Any);
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
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            _ = await postmasterMock.WaitForServerConnection();
        }
        else
        {
            Assert.That(() => new NpgsqlConnection(connectionString), Throws.Exception.TypeOf<NotSupportedException>());
        }
    }

    [Test]
    public void TargetSessionAttributes_default_is_null()
        => Assert.That(new NpgsqlConnectionStringBuilder().TargetSessionAttributes, Is.Null);

    [Test]
    [NonParallelizable] // Sets environment variable
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

        await using var dataSource = new NpgsqlDataSourceBuilder(builder.ConnectionString)
            .BuildMultiHost();

        await using var conn = await dataSource.OpenConnectionAsync();
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

        await using var dataSource = new NpgsqlDataSourceBuilder(defaultCsb.ConnectionString)
            .BuildMultiHost();

        NpgsqlConnector firstConnector;
        NpgsqlConnector secondConnector;

        await using (var firstConnection = await dataSource.OpenConnectionAsync())
        {
            firstConnector = firstConnection.Connector!;
        }

        await using (var secondConnection = await dataSource.OpenConnectionAsync())
        {
            secondConnector = secondConnection.Connector!;
        }

        Assert.AreNotSame(firstConnector, secondConnector);

        await using (var firstBalancedConnection = await dataSource.OpenConnectionAsync())
        {
            Assert.AreSame(firstConnector, firstBalancedConnection.Connector);
        }

        await using (var secondBalancedConnection = await dataSource.OpenConnectionAsync())
        {
            Assert.AreSame(secondConnector, secondBalancedConnection.Connector);
        }

        await using (var thirdBalancedConnection = await dataSource.OpenConnectionAsync())
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

        await using var dataSource = new NpgsqlDataSourceBuilder(defaultCsb.ConnectionString)
            .BuildMultiHost();

        NpgsqlConnector firstConnector;
        NpgsqlConnector secondConnector;

        await using (var firstConnection = await dataSource.OpenConnectionAsync())
        {
            firstConnector = firstConnection.Connector!;
        }
        await using (var secondConnection = await dataSource.OpenConnectionAsync())
        {
            Assert.AreSame(firstConnector, secondConnection.Connector);
        }
        await using (var firstConnection = await dataSource.OpenConnectionAsync())
        await using (var secondConnection = await dataSource.OpenConnectionAsync())
        {
            secondConnector = secondConnection.Connector!;
        }

        Assert.AreNotSame(firstConnector, secondConnector);

        await using (var firstUnbalancedConnection = await dataSource.OpenConnectionAsync())
        {
            Assert.AreSame(firstConnector, firstUnbalancedConnection.Connector);
        }

        await using (var secondUnbalancedConnection = await dataSource.OpenConnectionAsync())
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
            NoResetOnClose = true,
        };

        await using var dataSource = new NpgsqlDataSourceBuilder(defaultCsb.ConnectionString)
            .BuildMultiHost();

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

        await using (var firstConnection = await dataSource.OpenConnectionAsync(TargetSessionAttributes.PreferPrimary))
        await using (var secondConnection = await dataSource.OpenConnectionAsync(TargetSessionAttributes.PreferPrimary))
        {
            firstConnector = firstConnection.Connector!;
            secondConnector = secondConnection.Connector!;
        }

        await using var thirdConnection = await dataSource.OpenConnectionAsync(TargetSessionAttributes.PreferPrimary);
        Assert.AreSame(alwaysCheckHostState ? secondConnector : firstConnector, thirdConnection.Connector);

        await firstServerTask;
        await secondServerTask;
    }

    [Test]
    public void Database_state_cache_basic()
    {
        using var dataSource = CreateDataSource();
        var timeStamp = DateTime.UtcNow;

        dataSource.UpdateDatabaseState(DatabaseState.PrimaryReadWrite, timeStamp, TimeSpan.Zero);
        Assert.AreEqual(DatabaseState.PrimaryReadWrite, dataSource.GetDatabaseState());

        // Update with the same timestamp - shouldn't change anything
        dataSource.UpdateDatabaseState(DatabaseState.Standby, timeStamp, TimeSpan.Zero);
        Assert.AreEqual(DatabaseState.PrimaryReadWrite, dataSource.GetDatabaseState());

        // Update with a new timestamp
        timeStamp = timeStamp.AddSeconds(1);
        dataSource.UpdateDatabaseState(DatabaseState.PrimaryReadOnly, timeStamp, TimeSpan.Zero);
        Assert.AreEqual(DatabaseState.PrimaryReadOnly, dataSource.GetDatabaseState());

        // Expired state returns as Unknown (depending on ignoreExpiration)
        timeStamp = timeStamp.AddSeconds(1);
        dataSource.UpdateDatabaseState(DatabaseState.PrimaryReadWrite, timeStamp, TimeSpan.FromSeconds(-1));
        Assert.AreEqual(DatabaseState.Unknown, dataSource.GetDatabaseState(ignoreExpiration: false));
        Assert.AreEqual(DatabaseState.PrimaryReadWrite, dataSource.GetDatabaseState(ignoreExpiration: true));
    }

    [Test]
    public async Task Offline_state_on_connection_failure()
    {
        await using var server = PgPostmasterMock.Start(ConnectionString, startupErrorCode: PostgresErrorCodes.ConnectionFailure);
        await using var dataSource = server.CreateDataSource();
        await using var conn = dataSource.CreateConnection();

        var ex = Assert.ThrowsAsync<PostgresException>(conn.OpenAsync)!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.ConnectionFailure));

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Offline));
    }

    [Test]
    public async Task Unknown_state_on_connection_authentication_failure()
    {
        await using var server = PgPostmasterMock.Start(ConnectionString, startupErrorCode: PostgresErrorCodes.InvalidAuthorizationSpecification);
        await using var dataSource = server.CreateDataSource();
        await using var conn = dataSource.CreateConnection();

        var ex = Assert.ThrowsAsync<PostgresException>(conn.OpenAsync)!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.InvalidAuthorizationSpecification));

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
    }

    [Test]
    public async Task Offline_state_on_query_execution_pg_critical_failure()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = postmaster.CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var anotherConn = await dataSource.OpenConnectionAsync();
        await anotherConn.CloseAsync();

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(2));

        var server = await postmaster.WaitForServerConnection();
        await server.WriteErrorResponse(PostgresErrorCodes.CrashShutdown).FlushAsync();

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.CrashShutdown));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Offline));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(0));
    }

    [Test, NonParallelizable]
    public async Task Offline_state_on_query_execution_pg_non_critical_failure()
    {
        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        // Starting with PG14 we get the cluster's state from PG automatically
        var expectedState = conn.PostgreSqlVersion.Major > 13 ? DatabaseState.PrimaryReadWrite : DatabaseState.Unknown;

        var state = dataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(expectedState));
        Assert.That(dataSource.Statistics.Total, Is.EqualTo(1));

        var ex = Assert.ThrowsAsync<PostgresException>(() => conn.ExecuteNonQueryAsync("SELECT abc"))!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.UndefinedColumn));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        state = dataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(expectedState));
        Assert.That(dataSource.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Offline_state_on_query_execution_IOException()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = postmaster.CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var anotherConn = await dataSource.OpenConnectionAsync();
        await anotherConn.CloseAsync();

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(2));

        var server = await postmaster.WaitForServerConnection();
        server.Close();

        var ex = Assert.ThrowsAsync<NpgsqlException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.InnerException, Is.InstanceOf<IOException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Offline));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task Offline_state_on_query_execution_TimeoutException()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = postmaster.CreateDataSource(builder =>
        {
            builder.ConnectionStringBuilder.CommandTimeout = 1;
            builder.ConnectionStringBuilder.CancellationTimeout = 1;
        });

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var anotherConn = await dataSource.OpenConnectionAsync();
        await anotherConn.CloseAsync();

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(2));

        var ex = Assert.ThrowsAsync<NpgsqlException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Offline));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task Unknown_state_on_query_execution_TimeoutException_with_disabled_cancellation()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = postmaster.CreateDataSource(builder =>
        {
            builder.ConnectionStringBuilder.CommandTimeout = 1;
            builder.ConnectionStringBuilder.CancellationTimeout = -1;
        });

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var anotherConn = await dataSource.OpenConnectionAsync();
        await anotherConn.CloseAsync();

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(2));

        var ex = Assert.ThrowsAsync<NpgsqlException>(() => conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Unknown_state_on_query_execution_cancellation_with_disabled_cancellation_timeout()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = postmaster.CreateDataSource(builder =>
        {
            builder.ConnectionStringBuilder.CommandTimeout = 30;
            builder.ConnectionStringBuilder.CancellationTimeout = -1;
        });

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var anotherConn = await dataSource.OpenConnectionAsync();
        await anotherConn.CloseAsync();

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(2));

        using var cts = new CancellationTokenSource();

        var query = conn.ExecuteNonQueryAsync("SELECT 1", cancellationToken: cts.Token);
        cts.Cancel();
        var ex = Assert.ThrowsAsync<OperationCanceledException>(async () => await query)!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));

        state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Unknown_state_on_query_execution_TimeoutException_with_cancellation_failure()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = postmaster.CreateDataSource(builder =>
        {
            builder.ConnectionStringBuilder.CommandTimeout = 1;
            builder.ConnectionStringBuilder.CancellationTimeout = 0;
        });

        await using var conn = await dataSource.OpenConnectionAsync();

        var state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(1));

        var server = await postmaster.WaitForServerConnection();

        var query = conn.ExecuteNonQueryAsync("SELECT 1");

        await postmaster.WaitForCancellationRequest();
        await server.WriteCancellationResponse().WriteReadyForQuery().FlushAsync();

        var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await query)!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        state = conn.NpgsqlDataSource.GetDatabaseState();
        Assert.That(state, Is.EqualTo(DatabaseState.Unknown));
        Assert.That(conn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(1));
    }

    [Test]
    public async Task Clear_pool_one_host_only_on_admin_shutdown()
    {
        await using var primaryPostmaster = PgPostmasterMock.Start(ConnectionString, state: Primary);
        await using var standbyPostmaster = PgPostmasterMock.Start(ConnectionString, state: Standby);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder =
            {
                Host = MultipleHosts(primaryPostmaster, standbyPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                MaxPoolSize = 2
            }
        };
        await using var multiHostDataSource = dataSourceBuilder.BuildMultiHost();
        await using var preferPrimaryDataSource = multiHostDataSource.WithTargetSession(TargetSessionAttributes.PreferPrimary);

        await using var primaryConn = await preferPrimaryDataSource.OpenConnectionAsync();
        await using var anotherPrimaryConn = await preferPrimaryDataSource.OpenConnectionAsync();
        await using var standbyConn = await preferPrimaryDataSource.OpenConnectionAsync();
        var primaryDataSource = primaryConn.Connector!.DataSource;
        var standbyDataSource = standbyConn.Connector!.DataSource;
        await anotherPrimaryConn.CloseAsync();
        await standbyConn.CloseAsync();

        Assert.That(primaryDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.PrimaryReadWrite));
        Assert.That(standbyDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Standby));
        Assert.That(primaryConn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(3));

        var server = await primaryPostmaster.WaitForServerConnection();
        await server.WriteErrorResponse(PostgresErrorCodes.AdminShutdown).FlushAsync();

        var ex = Assert.ThrowsAsync<PostgresException>(() => primaryConn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.AdminShutdown));
        Assert.That(primaryConn.State, Is.EqualTo(ConnectionState.Closed));

        Assert.That(primaryDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Offline));
        Assert.That(standbyDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Standby));
        Assert.That(primaryConn.NpgsqlDataSource.Statistics.Total, Is.EqualTo(1));

        multiHostDataSource.ClearDatabaseStates();
        Assert.That(primaryDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Unknown));
        Assert.That(standbyDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Unknown));
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
        var dataSourceBuilder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder =
            {
                Host = MultipleHosts(firstPostmaster, secondPostmaster),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
                HostRecheckSeconds = 5
            }
        };
        await using var multiHostDataSource = dataSourceBuilder.BuildMultiHost();
        var (firstDataSource, secondDataSource) = (multiHostDataSource.Pools[0], multiHostDataSource.Pools[1]);
        await using var primaryDataSource = multiHostDataSource.WithTargetSession(TargetSessionAttributes.Primary);

        await using var conn = await primaryDataSource.OpenConnectionAsync();
        Assert.That(conn.Port, Is.EqualTo(firstPostmaster.Port));
        var firstServer = await firstPostmaster.WaitForServerConnection();
        await firstServer
            .WriteErrorResponse(PostgresErrorCodes.AdminShutdown)
            .FlushAsync();

        var failoverEx = Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteNonQueryAsync("SELECT 1"))!;
        Assert.That(failoverEx.SqlState, Is.EqualTo(PostgresErrorCodes.AdminShutdown));

        var noHostFoundEx = Assert.ThrowsAsync<NpgsqlException>(async () => await conn.OpenAsync())!;
        Assert.That(noHostFoundEx.Message, Is.EqualTo("No suitable host was found."));

        Assert.That(firstDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Offline));
        Assert.That(secondDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Standby));

        firstPostmaster.State = Standby;
        secondPostmaster.State = Primary;
        var secondServer = await secondPostmaster.WaitForServerConnection();
        await secondServer.SendMockState(Primary);

        await Task.Delay(TimeSpan.FromSeconds(10));
        Assert.That(firstDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Unknown));
        Assert.That(secondDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Unknown));

        await conn.OpenAsync();
        Assert.That(conn.Port, Is.EqualTo(secondPostmaster.Port));
        Assert.That(firstDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.Standby));
        Assert.That(secondDataSource.GetDatabaseState(), Is.EqualTo(DatabaseState.PrimaryReadWrite));
    }

    [Test, NonParallelizable]
    public void IntegrationTest([Values] bool loadBalancing, [Values] bool alwaysCheckHostState)
    {
        PoolManager.Reset();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString)
        {
            ConnectionStringBuilder =
            {
                Host = "localhost,127.0.0.1",
                Pooling = true,
                MaxPoolSize = 2,
                LoadBalanceHosts = loadBalancing,
                HostRecheckSeconds = alwaysCheckHostState ? 0 : 10,
            }
        };
        using var dataSource = dataSourceBuilder.BuildMultiHost();

        var queriesDone = 0;

        var clientsTask = Task.WhenAll(
            Client(dataSource, TargetSessionAttributes.Any),
            Client(dataSource, TargetSessionAttributes.Primary),
            Client(dataSource, TargetSessionAttributes.PreferPrimary),
            Client(dataSource, TargetSessionAttributes.PreferStandby),
            Client(dataSource, TargetSessionAttributes.ReadWrite));

        var onlyStandbyClient = Client(dataSource, TargetSessionAttributes.Standby);
        var readOnlyClient = Client(dataSource, TargetSessionAttributes.ReadOnly);

        Assert.DoesNotThrowAsync(() => clientsTask);
        Assert.ThrowsAsync<NpgsqlException>(() => onlyStandbyClient);
        Assert.ThrowsAsync<NpgsqlException>(() => readOnlyClient);
        Assert.AreEqual(125, queriesDone);

        Task Client(NpgsqlMultiHostDataSource multiHostDataSource, TargetSessionAttributes targetSessionAttributes)
        {
            var dataSource = multiHostDataSource.WithTargetSession(targetSessionAttributes);
            var tasks = new List<Task>(5);

            for (var i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(() => Query(dataSource)));
            }

            return Task.WhenAll(tasks);
        }

        async Task Query(NpgsqlDataSource dataSource)
        {
            await using var conn = dataSource.CreateConnection();
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

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString)
        {
            ConnectionStringBuilder =
            {
                Host = "localhost,127.0.0.1",
                Pooling = true,
                HostRecheckSeconds = 0
            }
        };
        await using var dataSource = dataSourceBuilder.BuildMultiHost();
        await using var conn = await dataSource.OpenConnectionAsync();
    }

    [Test]
    public async Task DataSource_with_wrappers()
    {
        await using var primaryPostmasterMock = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmasterMock = PgPostmasterMock.Start(state: Standby);

        var builder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder =
            {
                Host = MultipleHosts(primaryPostmasterMock, standbyPostmasterMock),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            }
        };

        await using var dataSource = builder.BuildMultiHost();
        await using var primaryDataSource = dataSource.WithTargetSession(TargetSessionAttributes.Primary);
        await using var standbyDataSource = dataSource.WithTargetSession(TargetSessionAttributes.Standby);

        await using var primaryConnection = await primaryDataSource.OpenConnectionAsync();
        Assert.That(primaryConnection.Port, Is.EqualTo(primaryPostmasterMock.Port));

        await using var standbyConnection = await standbyDataSource.OpenConnectionAsync();
        Assert.That(standbyConnection.Port, Is.EqualTo(standbyPostmasterMock.Port));
    }

    [Test]
    public async Task DataSource_without_wrappers()
    {
        await using var primaryPostmasterMock = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmasterMock = PgPostmasterMock.Start(state: Standby);

        var builder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder =
            {
                Host = MultipleHosts(primaryPostmasterMock, standbyPostmasterMock),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            }
        };

        await using var dataSource = builder.BuildMultiHost();

        await using var primaryConnection = await dataSource.OpenConnectionAsync(TargetSessionAttributes.Primary);
        Assert.That(primaryConnection.Port, Is.EqualTo(primaryPostmasterMock.Port));

        await using var standbyConnection = await dataSource.OpenConnectionAsync(TargetSessionAttributes.Standby);
        Assert.That(standbyConnection.Port, Is.EqualTo(standbyPostmasterMock.Port));
    }

    [Test]
    public void DataSource_with_TargetSessionAttributes_is_not_supported()
    {
        var builder = new NpgsqlDataSourceBuilder("Host=foo,bar;Target Session Attributes=primary");

        Assert.That(() => builder.BuildMultiHost(), Throws.Exception.TypeOf<InvalidOperationException>()
            .With.Message.EqualTo(NpgsqlStrings.CannotSpecifyTargetSessionAttributes));
    }

    [Test]
    public async Task BuildMultiHost_with_single_host_is_supported()
    {
        var builder = new NpgsqlDataSourceBuilder(ConnectionString);
        await using var dataSource = builder.BuildMultiHost();
        await using var connection = await dataSource.OpenConnectionAsync();
        Assert.That(await connection.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task Build_with_multiple_hosts_is_supported()
    {
        await using var primaryPostmasterMock = PgPostmasterMock.Start(state: Primary);
        await using var standbyPostmasterMock = PgPostmasterMock.Start(state: Standby);

        var builder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder =
            {
                Host = MultipleHosts(primaryPostmasterMock, standbyPostmasterMock),
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading,
            }
        };

        await using var dataSource = builder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4181")]
    [Explicit("Fails until #4181 is fixed.")]
    public async Task LoadBalancing_is_fair_if_first_host_is_down([Values]TargetSessionAttributes targetSessionAttributes)
    {
        await using var pDown = PgPostmasterMock.Start(state: Primary, startupErrorCode: PostgresErrorCodes.CannotConnectNow);
        await using var pRw1 = PgPostmasterMock.Start(state: Primary);
        await using var pR1 = PgPostmasterMock.Start(state: PrimaryReadOnly);
        await using var s1 = PgPostmasterMock.Start(state: Standby);
        await using var pRw2 = PgPostmasterMock.Start(state: Primary);
        await using var pR2 = PgPostmasterMock.Start(state: PrimaryReadOnly);
        await using var s2 = PgPostmasterMock.Start(state: Standby);

        var hostList = $"{pDown.Host}:{pDown.Port}," +
                       $"{pRw1.Host}:{pRw1.Port}," +
                       $"{pR1.Host}:{pR1.Port}," +
                       $"{s1.Host}:{s1.Port}," +
                       $"{pRw2.Host}:{pRw2.Port}," +
                       $"{pR2.Host}:{pR2.Port}," +
                       $"{s2.Host}:{s2.Port}";

        await using var dataSource = CreateDataSource(builder =>
        {
            builder.Host = hostList;
            builder.ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading;
            builder.LoadBalanceHosts = true;
            builder.TargetSessionAttributesParsed = targetSessionAttributes;

        });
        var connections = Enumerable.Repeat(0, 12).Select(_ => dataSource.OpenConnection()).ToArray();
        await using var __ = new DisposableWrapper(connections);

        switch (targetSessionAttributes)
        {
        case TargetSessionAttributes.Any:
            Assert.That(connections[0].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[1].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[2].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[3].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[4].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[5].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[6].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[7].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[8].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[9].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[10].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[11].Port, Is.EqualTo(s2.Port));
            break;
        case TargetSessionAttributes.ReadWrite:
            Assert.That(connections[0].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[1].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[2].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[3].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[4].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[5].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[6].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[7].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[8].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[9].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[10].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[11].Port, Is.EqualTo(pRw2.Port));
            break;
        case TargetSessionAttributes.ReadOnly:
            Assert.That(connections[0].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[1].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[2].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[3].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[4].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[5].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[6].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[7].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[8].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[9].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[10].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[11].Port, Is.EqualTo(s2.Port));
            break;
        case TargetSessionAttributes.Primary:
        case TargetSessionAttributes.PreferPrimary:
            Assert.That(connections[0].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[1].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[2].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[3].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[4].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[5].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[6].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[7].Port, Is.EqualTo(pR2.Port));
            Assert.That(connections[8].Port, Is.EqualTo(pRw1.Port));
            Assert.That(connections[9].Port, Is.EqualTo(pR1.Port));
            Assert.That(connections[10].Port, Is.EqualTo(pRw2.Port));
            Assert.That(connections[11].Port, Is.EqualTo(pR2.Port));
            break;
        case TargetSessionAttributes.Standby:
        case TargetSessionAttributes.PreferStandby:
            Assert.That(connections[0].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[1].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[2].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[3].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[4].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[5].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[6].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[7].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[8].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[9].Port, Is.EqualTo(s2.Port));
            Assert.That(connections[10].Port, Is.EqualTo(s1.Port));
            Assert.That(connections[11].Port, Is.EqualTo(s2.Port));
            break;
        }
    }

    static string MultipleHosts(params PgPostmasterMock[] postmasters)
        => string.Join(",", postmasters.Select(p => $"{p.Host}:{p.Port}"));

    class DisposableWrapper(IEnumerable<IAsyncDisposable> disposables) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            foreach (var disposable in disposables)
                await disposable.DisposeAsync();
        }
    }
}
