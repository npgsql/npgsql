using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class ConnectionTests : MultiplexingTestBase
{
    [Test, Description("Makes sure the connection goes through the proper state lifecycle")]
    public async Task Basic_lifecycle()
    {
        await using var conn = CreateConnection();

        var eventOpen = false;
        var eventClosed = false;

        conn.StateChange += (s, e) =>
        {
            if (e.OriginalState == ConnectionState.Closed &&
                e.CurrentState == ConnectionState.Open)
                eventOpen = true;

            if (e.OriginalState == ConnectionState.Open &&
                e.CurrentState == ConnectionState.Closed)
                eventClosed = true;
        };

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));

        await conn.OpenAsync();

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        Assert.That(eventOpen, Is.True);

        await using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            await reader.ReadAsync();

            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open | ConnectionState.Fetching));
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        }

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

        await conn.CloseAsync();

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
        Assert.That(eventClosed, Is.True);
    }

    [Test, Description("Makes sure the connection goes through the proper state lifecycle")]
    public async Task Broken_lifecycle([Values] bool openFromClose)
    {
        if (IsMultiplexing)
            return;

        await using var dataSource = CreateDataSource();
        await using var conn = dataSource.CreateConnection();

        var eventOpen = false;
        var eventClosed = false;

        conn.StateChange += (s, e) =>
        {
            if (e.OriginalState == ConnectionState.Closed &&
                e.CurrentState == ConnectionState.Open)
                eventOpen = true;

            if (e.OriginalState == ConnectionState.Open &&
                e.CurrentState == ConnectionState.Closed)
                eventClosed = true;
        };

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));

        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        Assert.That(eventOpen, Is.True);

        var sleep = conn.ExecuteNonQueryAsync("SELECT pg_sleep(5)");

        // Wait for a query
        await Task.Delay(1000);
        await using (var killingConn = await OpenConnectionAsync())
            killingConn.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

        Assert.ThrowsAsync<PostgresException>(() => sleep);

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
        Assert.That(eventClosed, Is.True);
        Assert.That(conn.Connector is null);
        Assert.AreEqual(0, conn.NpgsqlDataSource.Statistics.Total);

        if (openFromClose)
        {
            await conn.CloseAsync();

            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            Assert.That(eventClosed, Is.True);
        }

        Assert.DoesNotThrowAsync(conn.OpenAsync);
        Assert.AreEqual(1, await conn.ExecuteScalarAsync("SELECT 1"));
        Assert.AreEqual(1, conn.NpgsqlDataSource.Statistics.Total);
        Assert.DoesNotThrowAsync(conn.CloseAsync);
    }

    [Test]
    [Platform(Exclude = "MacOsX", Reason = "Flaky on MacOS")]
    public async Task Break_while_open()
    {
        if (IsMultiplexing)
            return;

        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        using (var conn2 = await OpenConnectionAsync())
            conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

        // Allow some time for the pg_terminate to kill our connection
        using (var cmd = CreateSleepCommand(conn, 10))
            Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
                .AssignableTo<NpgsqlException>());

        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
    }

    #region Connection Errors

#if IGNORE
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Connection_refused(bool pooled)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Port = 44444, Pooling = pooled };
        using (var conn = new NpgsqlConnection(csb)) {
            Assert.That(() => conn.Open(), Throws.Exception
                .TypeOf<SocketException>()
                // CoreCLR currently has an issue which causes the wrong SocketErrorCode to be set on Linux:
                // https://github.com/dotnet/corefx/issues/8464
                .With.Property(nameof(SocketException.SocketErrorCode)).EqualTo(SocketError.ConnectionRefused)
            );
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Connection_refused_async(bool pooled)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Port = 44444, Pooling = pooled };
        using (var conn = new NpgsqlConnection(csb))
        {
            Assert.That(async () => await conn.OpenAsync(), Throws.Exception
                .TypeOf<SocketException>()
                .With.Property(nameof(SocketException.SocketErrorCode)).EqualTo(SocketError.ConnectionRefused)
            );
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
        }
    }
#endif

    [Test]
    [Ignore("Fails in a non-determinstic manner and only on the build server... investigate...")]
    public void Invalid_Username()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Username = "unknown", Pooling = false
        }.ToString();
        using var conn = new NpgsqlConnection(connString);
        Assert.That(conn.Open, Throws.Exception
            .TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.InvalidPassword)
        );
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
    }

    [Test]
    public void Bad_database()
    {
        using var dataSource = CreateDataSource(csb => csb.Database = "does_not_exist");
        using var conn = dataSource.CreateConnection();

        Assert.That(() => conn.Open(),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.InvalidCatalogName)
        );
    }

    [Test, Description("Tests that mandatory connection string parameters are indeed mandatory")]
    public void Mandatory_connection_string_params()
        => Assert.Throws<ArgumentException>(() =>
            new NpgsqlConnection("User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests"));

    [Test, Description("Reuses the same connection instance for a failed connection, then a successful one")]
    public async Task Fail_connect_then_succeed([Values] bool pooling)
    {
        if (IsMultiplexing && !pooling) // Multiplexing doesn't work without pooling
            return;

        var dbName = GetUniqueIdentifier(nameof(Fail_connect_then_succeed));
        await using var conn1 = await OpenConnectionAsync();
        await conn1.ExecuteNonQueryAsync($"DROP DATABASE IF EXISTS \"{dbName}\"");
        try
        {
            await using var dataSource = CreateDataSource(csb =>
            {
                csb.Database = dbName;
                csb.Pooling = pooling;
            });

            await using var conn2 = dataSource.CreateConnection();
            var pgEx = Assert.ThrowsAsync<PostgresException>(conn2.OpenAsync)!;
            Assert.That(pgEx.SqlState, Is.EqualTo(PostgresErrorCodes.InvalidCatalogName)); // database doesn't exist
            Assert.That(conn2.FullState, Is.EqualTo(ConnectionState.Closed));

            await conn1.ExecuteNonQueryAsync($"CREATE DATABASE \"{dbName}\" TEMPLATE template0");

            Assert.DoesNotThrowAsync(conn2.OpenAsync);
            Assert.DoesNotThrowAsync(conn2.CloseAsync);
        }
        finally
        {
            await conn1.ExecuteNonQueryAsync($"DROP DATABASE IF EXISTS \"{dbName}\"");
        }
    }

    [Test]
    public void Open_timeout_unknown_ip([Values(true, false)] bool async)
    {
        const int timeoutSeconds = 2;

        var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
        if (unknownIp is null)
        {
            Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");
            return;
        }

        using var dataSource = CreateDataSource(csb =>
        {
            csb.Host = unknownIp;
            csb.Timeout = timeoutSeconds;
        });
        using var conn = dataSource.CreateConnection();

        var sw = Stopwatch.StartNew();
        if (async)
        {
            Assert.That(async () => await conn.OpenAsync(), Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<TimeoutException>());
        }
        else
        {
            Assert.That(() => conn.Open(), Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<TimeoutException>());
        }

        Assert.That(sw.Elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(timeoutSeconds * 1000 - 100),
            $"Timeout was supposed to happen after {timeoutSeconds} seconds, but fired after {sw.Elapsed.TotalSeconds}");
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    [Test]
    public void Connect_timeout_cancel()
    {
        var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
        if (unknownIp is null)
        {
            Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection cancellation tests");
            return;
        }

        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Host = unknownIp,
            Pooling = false,
            Timeout = 30
        }.ToString();
        using var conn = new NpgsqlConnection(connString);
        var cts = new CancellationTokenSource(1000);
        Assert.That(async () => await conn.OpenAsync(cts.Token), Throws.Exception.TypeOf<OperationCanceledException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    #endregion

    #region Client Encoding

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
    public async Task Client_encoding_is_UTF8_by_default()
    {
        using var conn = await OpenConnectionAsync();
        Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.EqualTo("UTF8"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
    [NonParallelizable] // Sets environment variable
    public async Task Client_encoding_env_var()
    {
        using (var testConn = await OpenConnectionAsync())
            Assert.That(await testConn.ExecuteScalarAsync("SHOW client_encoding"), Is.Not.EqualTo("SQL_ASCII"));

        // Note that the pool is unaware of the environment variable, so if a connection is
        // returned from the pool it may contain the wrong client_encoding
        using var _ = SetEnvironmentVariable("PGCLIENTENCODING", "SQL_ASCII");
        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();
        Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.EqualTo("SQL_ASCII"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
    public async Task Client_encoding_connection_param()
    {
        using (var conn = await OpenConnectionAsync())
            Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.Not.EqualTo("SQL_ASCII"));
        await using var dataSource = CreateDataSource(csb => csb.ClientEncoding = "SQL_ASCII");
        using (var conn = await dataSource.OpenConnectionAsync())
            Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.EqualTo("SQL_ASCII"));
    }

    #endregion Client Encoding

    #region Timezone

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1634")]
    [NonParallelizable] // Sets environment variable
    public async Task Timezone_env_var()
    {
        string newTimezone;
        using (var conn1 = await OpenConnectionAsync())
        {
            newTimezone = (string?)await conn1.ExecuteScalarAsync("SHOW TIMEZONE") == "Africa/Bamako"
                ? "Africa/Lagos"
                : "Africa/Bamako";
        }

        // Note that the pool is unaware of the environment variable, so if a connection is
        // returned from the pool it may contain the wrong timezone
        using var _ = SetEnvironmentVariable("PGTZ", newTimezone);
        await using var dataSource = CreateDataSource();
        using var conn2 = await dataSource.OpenConnectionAsync();
        Assert.That(await conn2.ExecuteScalarAsync("SHOW TIMEZONE"), Is.EqualTo(newTimezone));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1634")]
    public async Task Timezone_connection_param()
    {
        string newTimezone;
        using (var conn = await OpenConnectionAsync())
        {
            newTimezone = (string?)await conn.ExecuteScalarAsync("SHOW TIMEZONE") == "Africa/Bamako"
                ? "Africa/Lagos"
                : "Africa/Bamako";
        }

        await using var dataSource = CreateDataSource(csb => csb.Timezone = newTimezone);
        using (var conn = await dataSource.OpenConnectionAsync())
            Assert.That(await conn.ExecuteScalarAsync("SHOW TIMEZONE"), Is.EqualTo(newTimezone));
    }

    #endregion Timezone

    #region ConnectionString - Host

    [TestCase("127.0.0.1", ExpectedResult = new [] { "127.0.0.1:5432" })]
    [TestCase("127.0.0.1:5432", ExpectedResult = new [] { "127.0.0.1:5432" })]
    [TestCase("::1", ExpectedResult = new [] { "::1:5432" })]
    [TestCase("[::1]", ExpectedResult = new [] { "[::1]:5432" })]
    [TestCase("[::1]:5432", ExpectedResult = new [] { "[::1]:5432" })]
    [TestCase("localhost", ExpectedResult = new [] { "localhost:5432" })]
    [TestCase("localhost:5432", ExpectedResult = new [] { "localhost:5432" })]
    [TestCase("127.0.0.1,127.0.0.1:5432,::1,[::1],[::1]:5432,localhost,localhost:5432",
        ExpectedResult = new []
        {
            "127.0.0.1:5432",
            "127.0.0.1:5432",
            "::1:5432",
            "[::1]:5432",
            "[::1]:5432",
            "localhost:5432",
            "localhost:5432"
        })]
    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3802")]
    public string[] ConnectionString_Host(string host)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder = { Host = host }
        };
        using var dataSource = dataSourceBuilder.BuildMultiHost();
        return dataSource.Pools.Select(ds => $"{ds.Settings.Host}:{ds.Settings.Port}").ToArray()!;
    }

    #endregion ConnectionString - Host

    [Test]
    public async Task Unix_domain_socket()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (Environment.OSVersion.Version.Major < 10 || Environment.OSVersion.Version.Build < 17093)
                Assert.Ignore("Unix-domain sockets support was introduced in Windows build 17093");

            // On Windows we first need a classic IP connection to make sure we're running against the
            // right backend version
            using var versionConnection = await OpenConnectionAsync();
            MinimumPgVersion(versionConnection, "13.0", "Unix-domain sockets support on Windows was introduced in PostgreSQL 13");
        }

        var port = new NpgsqlConnectionStringBuilder(ConnectionString).Port;
        var candidateDirectories = new[] { "/var/run/postgresql", "/tmp", Environment.GetEnvironmentVariable("TMP") ?? "C:\\" };
        var dir = candidateDirectories.FirstOrDefault(d => File.Exists(Path.Combine(d, $".s.PGSQL.{port}")));
        if (dir == null)
        {
            IgnoreExceptOnBuildServer("No PostgreSQL unix domain socket was found");
            return;
        }

        try
        {
            await using var dataSource = CreateDataSource(csb => csb.Host = dir);
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var tx = await conn.BeginTransactionAsync();
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1", tx), Is.EqualTo(1));
            Assert.That(conn.DataSource, Is.EqualTo(Path.Combine(dir, $".s.PGSQL.{port}")));
        }
        catch (Exception ex)
        {
            IgnoreExceptOnBuildServer($"Connection via unix domain socket failed: {ex}");
        }
    }

    [Test]
    [Platform(Exclude = "MacOsX", Reason = "Fails only on mac, needs to be investigated")]
    public async Task Unix_abstract_domain_socket()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore("Abstract unix-domain sockets are not supported on windows");
        }

        // We first need a classic IP connection to make sure we're running against the
        // right backend version
        using var versionConnection = await OpenConnectionAsync();
        MinimumPgVersion(versionConnection, "14.0", "Abstract unix-domain sockets support was introduced in PostgreSQL 14");

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Host = "@/npgsql_unix"
        };

        try
        {
            await using var dataSource = CreateDataSource(csb.ToString());
            await using var conn = await dataSource.OpenConnectionAsync();
            await using var tx = await conn.BeginTransactionAsync();
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1", tx), Is.EqualTo(1));
            Assert.That(conn.DataSource, Is.EqualTo(Path.Combine(csb.Host, $".s.PGSQL.{csb.Port}")));
        }
        catch (Exception ex)
        {
            IgnoreExceptOnBuildServer($"Connection via abstract unix domain socket failed: {ex}");
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/903")]
    public void DataSource_property()
    {
        using var conn = new NpgsqlConnection();
        Assert.That(conn.DataSource, Is.EqualTo(string.Empty));

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString);

        conn.ConnectionString = csb.ConnectionString;
        Assert.That(conn.DataSource, Is.EqualTo($"tcp://{csb.Host}:{csb.Port}"));

        // Multiplexing isn't supported with multiple hosts
        if (IsMultiplexing)
            return;

        csb.Host = "127.0.0.1, 127.0.0.2";
        conn.ConnectionString = csb.ConnectionString;
        Assert.That(conn.DataSource, Is.EqualTo(string.Empty));
    }

    #region Server version

    [Test]
    public async Task PostgreSqlVersion_ServerVersion()
    {
        await using var c = new NpgsqlConnection(ConnectionString);

        Assert.That(() => c.PostgreSqlVersion, Throws.InvalidOperationException
            .With.Message.EqualTo("Connection is not open"));

        Assert.That(() => c.ServerVersion, Throws.InvalidOperationException
            .With.Message.EqualTo("Connection is not open"));

        await c.OpenAsync();
        var backendVersionString = (string)(await c.ExecuteScalarAsync("SHOW server_version"))!;

        Assert.That(backendVersionString, Is.EqualTo(c.ServerVersion));

        Assert.That(backendVersionString, Does.Contain(
            new[] { "rc", "beta", "devel" }.Any(x => backendVersionString.Contains(x))
                ? c.PostgreSqlVersion.Major.ToString()
                : c.PostgreSqlVersion.ToString()));
    }

    [TestCase("X13.0")]
    [TestCase("13.")]
    [TestCase("13.1.")]
    [TestCase("13.1.1.")]
    [TestCase("13.1.1.1.")]
    [TestCase("13.1.1.1.1")]
    public void ParseVersion_fails(string versionString)
        => Assert.That(() => TestDbInfo.ParseServerVersion(versionString), Throws.Exception);

    [TestCase("13.3", ExpectedResult = "13.3")]
    [TestCase("13.3X", ExpectedResult = "13.3")]
    [TestCase("9.6.4", ExpectedResult = "9.6.4")]
    [TestCase("9.6.4X", ExpectedResult = "9.6.4")]
    [TestCase("9.5alpha2", ExpectedResult = "9.5")]
    [TestCase("9.5alpha2X", ExpectedResult = "9.5")]
    [TestCase("9.5devel", ExpectedResult = "9.5")]
    [TestCase("9.5develX", ExpectedResult = "9.5")]
    [TestCase("9.5deveX", ExpectedResult = "9.5")]
    [TestCase("9.4beta3", ExpectedResult = "9.4")]
    [TestCase("9.4rc1", ExpectedResult = "9.4")]
    [TestCase("9.4rc1X", ExpectedResult = "9.4")]
    [TestCase("13devel", ExpectedResult = "13.0")]
    [TestCase("13beta1", ExpectedResult = "13.0")]
    // The following should not occur as PostgreSQL version string in the wild these days but we support it.
    [TestCase("13", ExpectedResult = "13.0")]
    [TestCase("13X", ExpectedResult = "13.0")]
    [TestCase("13alpha1", ExpectedResult = "13.0")]
    [TestCase("13alpha", ExpectedResult = "13.0")]
    [TestCase("13alphX", ExpectedResult = "13.0")]
    [TestCase("13beta", ExpectedResult = "13.0")]
    [TestCase("13betX", ExpectedResult = "13.0")]
    [TestCase("13rc1", ExpectedResult = "13.0")]
    [TestCase("13rc", ExpectedResult = "13.0")]
    [TestCase("13rX", ExpectedResult = "13.0")]
    [TestCase("99999.99999.99999.99999", ExpectedResult = "99999.99999.99999.99999")]
    [TestCase("99999.99999.99999.99999X", ExpectedResult = "99999.99999.99999.99999")]
    [TestCase("99999.99999.99999.99999devel", ExpectedResult = "99999.99999.99999.99999")]
    [TestCase("99999.99999.99999.99999alpha99999", ExpectedResult = "99999.99999.99999.99999")]
    [TestCase("99999.99999.99999alpha99999", ExpectedResult = "99999.99999.99999")]
    [TestCase("99999.99999.99999.99999beta99999", ExpectedResult = "99999.99999.99999.99999")]
    [TestCase("99999.99999.99999beta99999", ExpectedResult = "99999.99999.99999")]
    [TestCase("99999.99999.99999.99999rc99999", ExpectedResult = "99999.99999.99999.99999")]
    [TestCase("99999.99999.99999rc99999", ExpectedResult = "99999.99999.99999")]
    public string ParseVersion_succeeds(string versionString)
        => TestDbInfo.ParseServerVersion(versionString).ToString();

    class TestDbInfo : NpgsqlDatabaseInfo
    {
        public TestDbInfo(string host, int port, string databaseName, Version version) : base(host, port, databaseName, version)
            => throw new NotImplementedException();

        protected override IEnumerable<PostgresType> GetTypes()
            => throw new NotImplementedException();

        public new static Version ParseServerVersion(string versionString)
            => NpgsqlDatabaseInfo.ParseServerVersion(versionString);
    }

    #endregion Server version

    [Test]
    public void Setting_connection_string_while_open_throws()
    {
        using var conn = new NpgsqlConnection();
        conn.ConnectionString = ConnectionString;
        conn.Open();
        Assert.That(() => conn.ConnectionString = "", Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Empty_constructor()
    {
        var conn = new NpgsqlConnection();
        Assert.That(conn.ConnectionTimeout, Is.EqualTo(NpgsqlConnectionStringBuilder.DefaultTimeout));
        Assert.That(conn.ConnectionString, Is.SameAs(string.Empty));
        Assert.That(() => conn.Open(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Constructor_with_null_connection_string()
    {
        var conn = new NpgsqlConnection(null);
        Assert.That(conn.ConnectionString, Is.SameAs(string.Empty));
        Assert.That(() => conn.Open(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Constructor_with_empty_connection_string()
    {
        var conn = new NpgsqlConnection("");
        Assert.That(conn.ConnectionString, Is.SameAs(string.Empty));
        Assert.That(() => conn.Open(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Set_connection_string_to_null()
    {
        var conn = new NpgsqlConnection(ConnectionString);
        conn.ConnectionString = null;
        Assert.That(conn.ConnectionString, Is.SameAs(string.Empty));
        Assert.That(conn.Settings.Host, Is.Null);
        Assert.That(() => conn.Open(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Set_connection_string_to_empty()
    {
        var conn = new NpgsqlConnection(ConnectionString);
        conn.ConnectionString = "";
        Assert.That(conn.ConnectionString, Is.SameAs(string.Empty));
        Assert.That(conn.Settings.Host, Is.Null);
        Assert.That(() => conn.Open(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/703")]
    public async Task No_database_defaults_to_username()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Database = null };
        using var conn = new NpgsqlConnection(csb.ToString());
        Assert.That(conn.Database, Is.EqualTo(csb.Username));
        conn.Open();
        Assert.That(await conn.ExecuteScalarAsync("SELECT current_database()"), Is.EqualTo(csb.Username));
        Assert.That(conn.Database, Is.EqualTo(csb.Username));
    }

    [Test, Description("Breaks a connector while it's in the pool, with a keepalive and without")]
    [Platform(Exclude = "MacOsX", Reason = "Fails only on mac, needs to be investigated")]
    [TestCase(false, TestName = nameof(Break_connector_in_pool) + "_without_keep_alive")]
    [TestCase(true, TestName = nameof(Break_connector_in_pool) + "_with_keep_alive")]
    public async Task Break_connector_in_pool(bool keepAlive)
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing, hanging");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.MaxPoolSize = 1;
        if (keepAlive)
            dataSourceBuilder.ConnectionStringBuilder.KeepAlive = 1;
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();
        var connector = conn.Connector;
        Assert.That(connector, Is.Not.Null);
        await conn.CloseAsync();

        // Use another connection to kill the connector currently in the pool
        await using (var conn2 = await OpenConnectionAsync())
            await conn2.ExecuteNonQueryAsync($"SELECT pg_terminate_backend({connector!.BackendProcessId})");

        // Allow some time for the terminate to occur
        await Task.Delay(3000);

        await conn.OpenAsync();
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        if (keepAlive)
        {
            Assert.That(conn.Connector, Is.Not.SameAs(connector));
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }
        else
        {
            Assert.That(conn.Connector, Is.SameAs(connector));
            Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 1"), Throws.Exception
                .AssignableTo<NpgsqlException>());
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4603")]
    public async Task Reload_types_keepalive_concurrent()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing doesn't support keepalive");

        await using var dataSource = CreateDataSource(csb => csb.KeepAlive = 1);
        await using var conn = await dataSource.OpenConnectionAsync();

        var startTimestamp = Stopwatch.GetTimestamp();
        // Give a few seconds for a KeepAlive to possibly perform
        while (GetElapsedTime(startTimestamp).TotalSeconds < 2)
            Assert.DoesNotThrow(conn.ReloadTypes);

        // dotnet 3.1 doesn't have Stopwatch.GetElapsedTime method.
        static TimeSpan GetElapsedTime(long startingTimestamp) =>
            new((long)((Stopwatch.GetTimestamp() - startingTimestamp) * ((double)10000000 / Stopwatch.Frequency)));
    }

    #region ChangeDatabase

    [Test]
    public async Task ChangeDatabase()
    {
        using var conn = await OpenConnectionAsync();
        conn.ChangeDatabase("template1");
        using var cmd = new NpgsqlCommand("select current_database()", conn);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("template1"));
    }

    [Test]
    public async Task ChangeDatabase_does_not_affect_other_connections()
    {
        using var conn1 = new NpgsqlConnection(ConnectionString);
        using var conn2 = new NpgsqlConnection(ConnectionString);
        // Connection 1 changes database
        conn1.Open();
        conn1.ChangeDatabase("template1");
        Assert.That(await conn1.ExecuteScalarAsync("SELECT current_database()"), Is.EqualTo("template1"));

        // Connection 2's database should not changed
        conn2.Open();
        Assert.That(await conn2.ExecuteScalarAsync("SELECT current_database()"), Is.Not.EqualTo(conn1.Database));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1331")]
    public void ChangeDatabase_connection_on_closed_connection_throws()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        Assert.That(() => conn.ChangeDatabase("template1"), Throws.Exception
            .TypeOf<InvalidOperationException>()
            .With.Message.EqualTo("Connection is not open"));
    }

    #endregion

    [Test, Description("Tests closing a connector while a reader is open")]
    public async Task Close_during_read([Values(PooledOrNot.Pooled, PooledOrNot.Unpooled)] PooledOrNot pooled)
    {
        if (IsMultiplexing && pooled == PooledOrNot.Unpooled)
            return; // Multiplexing requires pooling

        await using var dataSource = CreateDataSource(csb => csb.Pooling = pooled == PooledOrNot.Pooled);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            conn.Close();
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            Assert.That(reader.IsClosed);
        }

        conn.Open();
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task Search_path()
    {
        await using var dataSource = CreateDataSource(csb => csb.SearchPath = "foo");
        await using var conn = await dataSource.OpenConnectionAsync();
        Assert.That(await conn.ExecuteScalarAsync("SHOW search_path"), Contains.Substring("foo"));
    }

    [Test]
    public async Task Set_options()
    {
        await using var dataSource = CreateDataSource(csb =>
            csb.Options =
                "-c default_transaction_isolation=serializable -c default_transaction_deferrable=on -c foo.bar=My\\ Famous\\\\Thing");
        await using var conn = await dataSource.OpenConnectionAsync();

        Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_isolation"), Is.EqualTo("serializable"));
        Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_deferrable"), Is.EqualTo("on"));
        Assert.That(await conn.ExecuteScalarAsync("SHOW foo.bar"), Is.EqualTo("My Famous\\Thing"));
    }

    [Test]
    public async Task Connector_not_initialized_exception()
    {
        var command = new NpgsqlCommand();
        command.CommandText = @"SELECT 123";

        for (var i = 0; i < 2; i++)
        {
            await using var connection = await OpenConnectionAsync();
            command.Connection = connection;
            await using var tx = await connection.BeginTransactionAsync();
            await command.ExecuteScalarAsync();
            await tx.CommitAsync();
        }
    }

    [Test]
    public void Bug1011001()
    {
        //[#1011001] Bug in NpgsqlConnectionStringBuilder affects on cache and connection pool

        var csb1 = new NpgsqlConnectionStringBuilder(@"Server=server;Port=5432;User Id=user;Password=passwor;Database=database;");
        var cs1 = csb1.ToString();
        var csb2 = new NpgsqlConnectionStringBuilder(cs1);
        var cs2 = csb2.ToString();
        Assert.IsTrue(cs1 == cs2);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/pull/164")]
    public void Connection_State_is_Closed_when_disposed()
    {
        var c = new NpgsqlConnection();
        c.Dispose();
        Assert.AreEqual(ConnectionState.Closed, c.State);
    }

    [Test]
    public void Change_ApplicationName_with_connection_string_builder()
    {
        // Test for issue #165 on github.
        var builder = new NpgsqlConnectionStringBuilder();
        builder.ApplicationName = "test";
    }

    [Test, Description("Makes sure notices are probably received and emitted as events")]
    public async Task Notice()
    {
        // Make sure messages are in English
        await using var dataSource = CreateDataSource(csb => csb.Options = "-c lc_messages=en_US.UTF-8");
        await using var conn = await dataSource.OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);
        await conn.ExecuteNonQueryAsync($@"
CREATE OR REPLACE FUNCTION {function}() RETURNS VOID AS
'BEGIN RAISE NOTICE ''testnotice''; END;'
LANGUAGE 'plpgsql'");

        var mre = new ManualResetEvent(false);
        PostgresNotice? notice = null;
        NoticeEventHandler action = (sender, args) =>
        {
            notice = args.Notice;
            mre.Set();
        };
        conn.Notice += action;
        try
        {
            // See docs for CreateSleepCommand
            await conn.ExecuteNonQueryAsync($"SELECT {function}()::TEXT");
            mre.WaitOne(5000);
            Assert.That(notice, Is.Not.Null, "No notice was emitted");
            Assert.That(notice!.MessageText, Is.EqualTo("testnotice"));
            Assert.That(notice.Severity, Is.EqualTo("NOTICE"));
        }
        finally
        {
            conn.Notice -= action;
        }
    }

    [Test, Description("Makes sure that concurrent use of the connection throws an exception")]
    public async Task Concurrent_use_throws()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing: fails");
        using var conn = await OpenConnectionAsync();
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        using (await cmd.ExecuteReaderAsync())
            Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 2"),
                Throws.Exception.TypeOf<NpgsqlOperationInProgressException>()
                    .With.Property(nameof(NpgsqlOperationInProgressException.CommandInProgress)).SameAs(cmd));

        await conn.ExecuteNonQueryAsync("CREATE TEMP TABLE foo (bar INT)");
        using (conn.BeginBinaryImport("COPY foo (bar) FROM STDIN BINARY"))
        {
            Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 2"),
                Throws.Exception.TypeOf<NpgsqlOperationInProgressException>()
                    .With.Message.Contains("Copy"));
        }
    }

    #region PersistSecurityInfo

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
    public void PersistSecurityInfo_is_true([Values(true, false)] bool pooling)
    {
        if (IsMultiplexing && !pooling)
            return;

        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            PersistSecurityInfo = true,
            Pooling = pooling
        }.ToString();
        using var conn = new NpgsqlConnection(connString);
        var passwd = new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password;
        Assert.That(passwd, Is.Not.Null);
        conn.Open();
        Assert.That(new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password, Is.EqualTo(passwd));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
    public void No_password_without_PersistSecurityInfo([Values(true, false)] bool pooling)
    {
        if (IsMultiplexing && !pooling)
            return;

        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = pooling
        }.ToString();
        using var conn = new NpgsqlConnection(connString);
        var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
        Assert.That(csb.PersistSecurityInfo, Is.False);
        Assert.That(csb.Password, Is.Not.Null);
        conn.Open();
        Assert.That(new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password, Is.Null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2725")]
    public void Clone_with_PersistSecurityInfo()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            PersistSecurityInfo = true
        };
        using var _ = CreateTempPool(builder, out var connStringWithPersist);

        using var connWithPersist = new NpgsqlConnection(connStringWithPersist);

        // First un-persist, should work
        builder.PersistSecurityInfo = false;
        var connStringWithoutPersist = builder.ToString();
        using var clonedWithoutPersist = connWithPersist.CloneWith(connStringWithoutPersist);
        clonedWithoutPersist.Open();

        Assert.That(clonedWithoutPersist.ConnectionString, Does.Not.Contain("Password="));

        // Then attempt to re-persist, should not work
        using var clonedConn = clonedWithoutPersist.CloneWith(connStringWithPersist);
        clonedConn.Open();

        Assert.That(clonedConn.ConnectionString, Does.Not.Contain("Password="));
    }

    [Test]
    public async Task CloneWith_and_data_source_with_password()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
        // Set the password via the data source property later to make sure that's picked up by CloneWith
        var password = dataSourceBuilder.ConnectionStringBuilder.Password!;
        dataSourceBuilder.ConnectionStringBuilder.Password = null;
        await using var dataSource = dataSourceBuilder.Build();

        await using var connection = dataSource.CreateConnection();
        dataSource.Password = password;

        // Test that the up-to-date password gets copied to the clone, as if we opened the original connection instead of cloning it
        using var _ = CreateTempPool(new NpgsqlConnectionStringBuilder(ConnectionString) { Password = null }, out var tempConnectionString);
        await using var clonedConnection = connection.CloneWith(tempConnectionString);
        await clonedConnection.OpenAsync();
    }

    [Test]
    public async Task CloneWith_and_data_source_with_auth_callbacks()
    {
        var (userCertificateValidationCallbackCalled, clientCertificatesCallbackCalled) = (false, false);

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseUserCertificateValidationCallback(UserCertificateValidationCallback);
        dataSourceBuilder.UseClientCertificatesCallback(ClientCertificatesCallback);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = dataSource.CreateConnection();

        using var _ = CreateTempPool(ConnectionString, out var tempConnectionString);
        await using var clonedConnection = connection.CloneWith(tempConnectionString);

        clonedConnection.UserCertificateValidationCallback!(null!, null, null, SslPolicyErrors.None);
        Assert.True(userCertificateValidationCallbackCalled);
        clonedConnection.ProvideClientCertificatesCallback!(null!);
        Assert.True(clientCertificatesCallbackCalled);

        bool UserCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors errors)
            => userCertificateValidationCallbackCalled = true;

        void ClientCertificatesCallback(X509CertificateCollection certs)
            => clientCertificatesCallbackCalled = true;
    }

    #endregion PersistSecurityInfo

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/743")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
    public void Clone()
    {
        using var pool = CreateTempPool(ConnectionString, out var connectionString);
        using var conn = new NpgsqlConnection(connectionString);
        ProvideClientCertificatesCallback callback1 = certificates => { };
        conn.ProvideClientCertificatesCallback = callback1;
        RemoteCertificateValidationCallback callback2 = (sender, certificate, chain, errors) => true;
        conn.UserCertificateValidationCallback = callback2;

        conn.Open();
        Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

        using var conn2 = (NpgsqlConnection)((ICloneable)conn).Clone();
        Assert.That(conn2.ConnectionString, Is.EqualTo(conn.ConnectionString));
        Assert.That(conn2.ProvideClientCertificatesCallback, Is.SameAs(callback1));
        Assert.That(conn2.UserCertificateValidationCallback, Is.SameAs(callback2));
        conn2.Open();
        Assert.That(async () => await conn2.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task Clone_with_data_source()
    {
        await using var connection = await SharedDataSource.OpenConnectionAsync();
        await using var clonedConnection = (NpgsqlConnection)((ICloneable)connection).Clone();

        Assert.That(clonedConnection.NpgsqlDataSource, Is.SameAs(SharedDataSource));
        Assert.DoesNotThrowAsync(() => clonedConnection.OpenAsync());
    }

    [Test]
    public async Task DatabaseInfo_is_shared()
    {
        if (IsMultiplexing)
            return;
        // Create a temp pool to make sure the second connection will be new and not idle
        await using var dataSource = CreateDataSource();
        await using var conn1 = await dataSource.OpenConnectionAsync();
        // Call RealoadTypes to force reload DatabaseInfo
        conn1.ReloadTypes();
        await using var conn2 = await dataSource.OpenConnectionAsync();
        Assert.That(conn1.Connector!.DatabaseInfo, Is.SameAs(conn2.Connector!.DatabaseInfo));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/736")]
    public async Task ManyOpenClose()
    {
        await using var dataSource = CreateDataSource();
        // The connector's _sentRfqPrependedMessages is a byte, too many open/closes made it overflow
        for (var i = 0; i < 255; i++)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
        }
        await using (var conn = dataSource.CreateConnection())
        {
            await conn.OpenAsync();
        }
        await using (var conn = dataSource.CreateConnection())
        {
            await conn.OpenAsync();
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/736")]
    public async Task Many_open_close_with_transaction()
    {
        await using var dataSource = CreateDataSource();
        // The connector's _sentRfqPrependedMessages is a byte, too many open/closes made it overflow
        for (var i = 0; i < 255; i++)
        {
            await using var conn = await dataSource.OpenConnectionAsync();
            await conn.BeginTransactionAsync();
        }
        await using (var conn = await dataSource.OpenConnectionAsync())
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/927")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/736")]
    [Ignore("Fails when running the entire test suite but not on its own...")]
    public async Task Rollback_on_close()
    {
        // Npgsql 3.0.0 to 3.0.4 prepended a rollback for the next time the connector is used, as an optimization.
        // This caused some issues (#927) and was removed.

        await using var dataSource = CreateDataSource();

        int processId;
        await using (var conn = await dataSource.OpenConnectionAsync())
        {
            processId = conn.Connector!.BackendProcessId;
            await conn.BeginTransactionAsync();
            await conn.ExecuteNonQueryAsync("SELECT 1");
            Assert.That(conn.Connector.TransactionStatus, Is.EqualTo(TransactionStatus.InTransactionBlock));
        }

        await using (var conn = await dataSource.OpenConnectionAsync())
        {
            Assert.That(conn.Connector!.BackendProcessId, Is.EqualTo(processId));
            Assert.That(conn.Connector.TransactionStatus, Is.EqualTo(TransactionStatus.Idle));
        }
    }

    [Test, Description("Tests an exception happening when sending the Terminate message while closing a ready connector")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/777")]
    public async Task Exception_during_close()
    {
        // Pooling must be on to use multiplexing
        if (IsMultiplexing)
            return;

        await using var dataSource = CreateDataSource(csb => csb.Pooling = false);
        await using var conn = await dataSource.OpenConnectionAsync();
        var connectorId = conn.ProcessID;

        using (var conn2 = await OpenConnectionAsync())
            await conn2.ExecuteNonQueryAsync($"SELECT pg_terminate_backend({connectorId})");

        conn.Close();
    }

    [Test, Description("Some pseudo-PG database don't support pg_type loading, we have a minimal DatabaseInfo for this")]
    public async Task NoTypeLoading()
    {
        await using var dataSource = CreateDataSource(csb => csb.ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading);
        await using var conn = await dataSource.OpenConnectionAsync();

        Assert.That(await conn.ExecuteScalarAsync("SELECT 8"), Is.EqualTo(8));
        Assert.That(await conn.ExecuteScalarAsync("SELECT 'foo'"), Is.EqualTo("foo"));
        Assert.That(await conn.ExecuteScalarAsync("SELECT TRUE"), Is.EqualTo(true));
        Assert.That(await conn.ExecuteScalarAsync("SELECT INET '192.168.1.1'"), Is.EqualTo(IPAddress.Parse("192.168.1.1")));

        Assert.That(await conn.ExecuteScalarAsync("SELECT '{1,2,3}'::int[]"), Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(await conn.ExecuteScalarAsync("SELECT '[1,10)'::int4range"), Is.EqualTo(new NpgsqlRange<int>(1, true, 10, false)));

        if (conn.PostgreSqlVersion >= new Version(14, 0))
        {
            var multirangeArray = (NpgsqlRange<int>[])(await conn.ExecuteScalarAsync("SELECT '{[3,7), (8,]}'::int4multirange"))!;
            Assert.That(multirangeArray.Length, Is.EqualTo(2));
            Assert.That(multirangeArray[0], Is.EqualTo(new NpgsqlRange<int>(3, true, false, 7, false, false)));
            Assert.That(multirangeArray[1], Is.EqualTo(new NpgsqlRange<int>(9, true, false, 0, false, true)));
        }
        else
        {
            using var cmd = new NpgsqlCommand("SELECT $1", conn)
            {
                Parameters = { new() { Value = DBNull.Value, NpgsqlDbType = NpgsqlDbType.IntegerMultirange } }
            };

            Assert.That(async () => await cmd.ExecuteScalarAsync(),
                Throws.Exception.TypeOf<NpgsqlException>()
                    .With.Message.EqualTo("The NpgsqlDbType 'IntegerMultirange' isn't present in your database. You may need to install an extension or upgrade to a newer version."));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1158")]
    public async Task Table_named_record()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing, ReloadTypes");

        using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync(@"

DROP TABLE IF EXISTS record;
CREATE TABLE record ()");
        try
        {
            conn.ReloadTypes();
            Assert.That(await conn.ExecuteScalarAsync("SELECT COUNT(*) FROM record"), Is.Zero);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync("DROP TABLE record");
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/392")]
    [NonParallelizable]
    [Platform(Exclude = "MacOsX", Reason = "Flaky in CI on Mac")]
    public async Task Non_UTF8_Encoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        await using var adminConn = await OpenConnectionAsync();

        // Create the database with server encoding sql-ascii
        // Starting with PG16, the default locale provider is icu, which does not support encoding sql_ascii. Specify libc explicitly as the
        // locale provider (except for older versions where specifying explicitly isn't supported, and libc is the only possibility).
        await adminConn.ExecuteNonQueryAsync("DROP DATABASE IF EXISTS sqlascii");
        await adminConn.ExecuteNonQueryAsync(
            adminConn.PostgreSqlVersion >= new Version(15, 0)
                ? "CREATE DATABASE sqlascii ENCODING 'sql_ascii' LOCALE_PROVIDER libc TEMPLATE template0"
                : "CREATE DATABASE sqlascii ENCODING 'sql_ascii' TEMPLATE template0");

        try
        {
            // Insert some win1252 data
            await using var goodDataSource = CreateDataSource(csb =>
            {
                csb.Database = "sqlascii";
                csb.Encoding = "windows-1252";
                csb.ClientEncoding = "sql-ascii";
            });

            await using (var conn = await goodDataSource.OpenConnectionAsync())
            {
                const string value = "éàç";
                await conn.ExecuteNonQueryAsync("CREATE TABLE foo (bar TEXT)");
                await conn.ExecuteNonQueryAsync($"INSERT INTO foo (bar) VALUES ('{value}')");

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM foo";
                await using var reader = await cmd.ExecuteReaderAsync();
                Assert.IsTrue(await reader.ReadAsync());

                using (var textReader = await reader.GetTextReaderAsync(0))
                    Assert.That(textReader.ReadToEnd(), Is.EqualTo(value));
                Assert.That(reader.GetString(0), Is.EqualTo(value));
            }

            // A normal connection with the default UTF8 encoding and client_encoding should fail
            await using var badDataSource = CreateDataSource(csb => csb.Database = "sqlascii");
            await using (var conn = await badDataSource.OpenConnectionAsync())
            {
                Assert.That(async () => await conn.ExecuteScalarAsync("SELECT * FROM foo"),
                    Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.CharacterNotInRepertoire)
                        .Or.TypeOf<DecoderFallbackException>()
                );
            }
        }
        finally
        {
            await adminConn.ExecuteNonQueryAsync("DROP DATABASE IF EXISTS sqlascii");
        }
    }

    [Test]
    public async Task Oversize_buffer()
    {
        if (IsMultiplexing)
            return;

        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString);

        Assert.That(conn.Connector!.ReadBuffer.Size, Is.EqualTo(csb.ReadBufferSize));

        // Read a big row, we should now be using an oversize buffer
        var bigString1 = new string('x', conn.Connector.ReadBuffer.Size + 1);
        using (var cmd = new NpgsqlCommand($"SELECT '{bigString1}'", conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo(bigString1));
        }
        var size1 = conn.Connector.ReadBuffer.Size;
        Assert.That(conn.Connector.ReadBuffer.Size, Is.GreaterThan(csb.ReadBufferSize));

        // Even bigger oversize buffer
        var bigString2 = new string('x', conn.Connector.ReadBuffer.Size + 1);
        using (var cmd = new NpgsqlCommand($"SELECT '{bigString2}'", conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo(bigString2));
        }
        Assert.That(conn.Connector.ReadBuffer.Size, Is.GreaterThan(size1));

        var processId = conn.ProcessID;
        conn.Close();
        conn.Open();
        Assert.That(conn.ProcessID, Is.EqualTo(processId));
        Assert.That(conn.Connector.ReadBuffer.Size, Is.EqualTo(csb.ReadBufferSize));
    }

    #region Keepalive

    [Test, Explicit, Description("Turns on TCP keepalive and sleeps forever, good for wiresharking")]
    public async Task TcpKeepaliveTime()
    {
        await using var dataSource = CreateDataSource(csb => csb.TcpKeepAliveTime = 2);
        using (await dataSource.OpenConnectionAsync())
            Thread.Sleep(Timeout.Infinite);
    }

    [Test, Explicit, Description("Turns on TCP keepalive and sleeps forever, good for wiresharking")]
    public async Task TcpKeepalive()
    {
        await using var dataSource = CreateDataSource(csb => csb.TcpKeepAlive = true);
        await using (await dataSource.OpenConnectionAsync())
            Thread.Sleep(Timeout.Infinite);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3511")]
    public async Task Keepalive_with_failed_transaction()
    {
        if (IsMultiplexing)
            return;

        await using var dataSource = CreateDataSource(csb => csb.KeepAlive = 1);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();

        Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteScalarAsync("SELECT non_existent_table"));
        // Connection is now in a failed transaction state. Wait a bit to allow for the keepalive to execute.
        Thread.Sleep(3000);

        await tx.RollbackAsync();

        // Confirm that the connection is still open and usable
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    #endregion Keepalive

    [Test]
    public async Task Change_parameter()
    {
        if (IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        var defaultApplicationName = conn.PostgresParameters["application_name"];
        await conn.ExecuteNonQueryAsync("SET application_name = 'some_test_value'");
        Assert.That(conn.PostgresParameters["application_name"], Is.EqualTo("some_test_value"));
        await conn.ExecuteNonQueryAsync("SET application_name = 'some_test_value2'");
        Assert.That(conn.PostgresParameters["application_name"], Is.EqualTo("some_test_value2"));
        await conn.ExecuteNonQueryAsync($"SET application_name = '{defaultApplicationName}'");
        Assert.That(conn.PostgresParameters["application_name"], Is.EqualTo(defaultApplicationName));
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Connect_OptionsFromEnvironment_Succeeds()
    {
        using (SetEnvironmentVariable("PGOPTIONS", "-c default_transaction_isolation=serializable -c default_transaction_deferrable=on -c foo.bar=My\\ Famous\\\\Thing"))
        {
            await using var dataSource = CreateDataSource();
            await using var conn = await dataSource.OpenConnectionAsync();
            Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_isolation"), Is.EqualTo("serializable"));
            Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_deferrable"), Is.EqualTo("on"));
            Assert.That(await conn.ExecuteScalarAsync("SHOW foo.bar"), Is.EqualTo("My Famous\\Thing"));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3030")]
    [TestCase(true, TestName = "NoResetOnClose")]
    [TestCase(false, TestName = "NoNoResetOnClose")]
    public async Task NoResetOnClose(bool noResetOnClose)
    {
        var originalApplicationName = new NpgsqlConnectionStringBuilder(ConnectionString).ApplicationName ?? "";

        await using var dataSource = CreateDataSource(csb =>
        {
            csb.MaxPoolSize = 1;
            csb.NoResetOnClose = noResetOnClose;
        });

        await using var conn = await dataSource.OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync("SET application_name = 'modified'");
        await conn.CloseAsync();
        await conn.OpenAsync();
        Assert.That(await conn.ExecuteScalarAsync("SHOW application_name"), Is.EqualTo(
            noResetOnClose || IsMultiplexing
                ? "modified"
                : originalApplicationName));
    }

    #region Physical connection initialization

    [Test]
    public async Task PhysicalConnectionInitializer_sync()
    {
        if (IsMultiplexing) // Sync I/O
            return;

        await using var adminConn = await OpenConnectionAsync();
        var table = await CreateTempTable(adminConn, "ID INTEGER");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UsePhysicalConnectionInitializer(
            conn => conn.ExecuteNonQuery($"INSERT INTO {table} VALUES (1)"),
            _ => throw new NotSupportedException());
        await using var dataSource = dataSourceBuilder.Build();

        await using (var conn = dataSource.OpenConnection())
        {
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM \"{table}\""), Is.EqualTo(1));
        }

        // Opening a second time should get us an idle connection, which should not cause the initializer to get executed
        await using (var conn = dataSource.OpenConnection())
        {
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM \"{table}\""), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PhysicalConnectionInitializer_async()
    {
        // With multiplexing the connector might become idle at undetermined point after the query is executed.
        // Which is why we ignore it.
        if (IsMultiplexing)
            return;

        await using var adminConn = await OpenConnectionAsync();
        var table = await CreateTempTable(adminConn, "ID INTEGER");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UsePhysicalConnectionInitializer(
            _ => throw new NotSupportedException(),
            async conn => await conn.ExecuteNonQueryAsync($"INSERT INTO {table} VALUES (1)"));
        await using var dataSource = dataSourceBuilder.Build();

        await using (var conn = await dataSource.OpenConnectionAsync())
        {
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM \"{table}\""), Is.EqualTo(1));
        }

        // Opening a second time should get us an idle connection, which should not cause the initializer to get executed
        await using (var conn = await dataSource.OpenConnectionAsync())
        {
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM \"{table}\""), Is.EqualTo(1));
        }
    }

    [Test]
    public async Task PhysicalConnectionInitializer_sync_with_break()
    {
        if (IsMultiplexing) // Sync I/O
            return;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UsePhysicalConnectionInitializer(
            conn =>
            {
                // Use another connection to kill the connector currently in the pool
                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

                conn.ExecuteScalar("SELECT 1");
            },
            _ => throw new NotSupportedException());
        await using var dataSource = dataSourceBuilder.Build();

        Assert.That(() => dataSource.OpenConnection(), Throws.Exception.InstanceOf<NpgsqlException>());
        Assert.That(dataSource.Statistics, Is.EqualTo((0, 0, 0)));
    }

    [Test]
    public async Task PhysicalConnectionInitializer_async_with_break()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UsePhysicalConnectionInitializer(
            _ => throw new NotSupportedException(),
            async conn =>
            {
                // Use another connection to kill the connector currently in the pool
                await using (var conn2 = await OpenConnectionAsync())
                    await conn2.ExecuteNonQueryAsync($"SELECT pg_terminate_backend({conn.ProcessID})");

                await conn.ExecuteScalarAsync("SELECT 1");
            });
        await using var dataSource = dataSourceBuilder.Build();

        Assert.That(async () => await dataSource.OpenConnectionAsync(), Throws.Exception.InstanceOf<NpgsqlException>());
        Assert.That(dataSource.Statistics, Is.EqualTo((0, 0, 0)));
    }

    [Test]
    public async Task PhysicalConnectionInitializer_async_throws_on_second_open()
    {
        // With multiplexing a physical connection might open on NpgsqlConnection.OpenAsync (if there was no completed bootstrap beforehand)
        // or on NpgsqlCommand.ExecuteReaderAsync.
        // We've already tested the first case in PhysicalConnectionInitializer_async_throws above, testing the second one below.
        var count = 0;
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UsePhysicalConnectionInitializer(
            _ => throw new NotSupportedException(),
            _ =>
            {
                if (++count == 1)
                    return Task.CompletedTask;
                throw new Exception("INTENTIONAL FAILURE");
            });
        await using var dataSource = dataSourceBuilder.Build();

        await using var conn1 = dataSource.CreateConnection();
        Assert.DoesNotThrowAsync(async () => await conn1.OpenAsync());

        // We start a transaction specifically for multiplexing (to bind a connector to the connection)
        await using var tx = await conn1.BeginTransactionAsync();

        await using var conn2 = dataSource.CreateConnection();
        Exception exception;
        if (IsMultiplexing)
        {
            await conn2.OpenAsync();
            exception = Assert.ThrowsAsync<Exception>(async () => await conn2.BeginTransactionAsync())!;
        }
        else
            exception = Assert.ThrowsAsync<Exception>(async () => await conn2.OpenAsync())!;
        Assert.That(exception.Message, Is.EqualTo("INTENTIONAL FAILURE"));
    }

    [Test]
    public async Task PhysicalConnectionInitializer_disposes_connection()
    {
        NpgsqlConnection? initializerConnection = null;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UsePhysicalConnectionInitializer(
            _ => throw new NotSupportedException(),
            conn =>
            {
                initializerConnection = conn;
                return Task.CompletedTask;
            });
        await using var dataSource = dataSourceBuilder.Build();

        await using var conn = await dataSource.OpenConnectionAsync();

        Assert.That(initializerConnection, Is.Not.Null);
        Assert.That(conn, Is.Not.SameAs(initializerConnection));
        Assert.That(() => initializerConnection!.Open(), Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    #endregion Physical connection initialization

    [Test]
    [NonParallelizable] // Modifies global database info factories
    [IssueLink("https://github.com/npgsql/npgsql/issues/4425")]
    public async Task Breaking_connection_while_loading_database_info()
    {
        if (IsMultiplexing)
            return;

        await using var dataSource = CreateDataSource();

        await using var firstConn = dataSource.CreateConnection();
        NpgsqlDatabaseInfo.RegisterFactory(new BreakingDatabaseInfoFactory());
        try
        {
            // Test the first time we load the database info
            Assert.ThrowsAsync<IOException>(firstConn.OpenAsync);
        }
        finally
        {
            NpgsqlDatabaseInfo.ResetFactories();
        }

        await firstConn.OpenAsync();
        await using var secondConn = await dataSource.OpenConnectionAsync();
        await secondConn.CloseAsync();
        await firstConn.ReloadTypesAsync();

        NpgsqlDatabaseInfo.RegisterFactory(new BreakingDatabaseInfoFactory());
        try
        {
            // Make sure that the database info is now cached and won't be reloaded
            Assert.DoesNotThrowAsync(secondConn.OpenAsync);
        }
        finally
        {
            NpgsqlDatabaseInfo.ResetFactories();
        }
    }

    class BreakingDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        public Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
            => throw conn.Break(new IOException());
    }

    #region Logging tests

    [Test]
    public async Task Log_Open_Close_pooled()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = dataSource.CreateConnection();

        // Open and close to have an idle connection in the pool - we don't want to test physical open/close
        await conn.OpenAsync();
        await conn.CloseAsync();

        int processId, port;
        string host, database;
        using (listLoggerProvider.Record())
        {
            await conn.OpenAsync();

            var tx = await conn.BeginTransactionAsync();
            (processId, host, port, database) = (conn.ProcessID, conn.Host!, conn.Port, conn.Database);
            await tx.CommitAsync();

            await conn.CloseAsync();
        }

        var openingConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.OpeningConnection);
        AssertLoggingConnectionString(conn, openingConnectionEvent.State);
        AssertLoggingStateContains(openingConnectionEvent, "Host", host);
        AssertLoggingStateContains(openingConnectionEvent, "Port", port);
        AssertLoggingStateContains(openingConnectionEvent, "Database", database);

        var openedConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.OpenedConnection);
        AssertLoggingConnectionString(conn, openedConnectionEvent.State);
        AssertLoggingStateContains(openedConnectionEvent, "Host", host);
        AssertLoggingStateContains(openedConnectionEvent, "Port", port);
        AssertLoggingStateContains(openedConnectionEvent, "Database", database);

        var closingConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.ClosingConnection);
        AssertLoggingConnectionString(conn, closingConnectionEvent.State);
        AssertLoggingStateContains(closingConnectionEvent, "Host", host);
        AssertLoggingStateContains(closingConnectionEvent, "Port", port);
        AssertLoggingStateContains(closingConnectionEvent, "Database", database);

        var closedConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.ClosedConnection);
        AssertLoggingConnectionString(conn, closedConnectionEvent.State);
        AssertLoggingStateContains(closedConnectionEvent, "Host", host);
        AssertLoggingStateContains(closedConnectionEvent, "Port", port);
        AssertLoggingStateContains(closedConnectionEvent, "Database", database);

        if (!IsMultiplexing)
        {
            AssertLoggingStateContains(openedConnectionEvent, "ConnectorId", processId);
            AssertLoggingStateContains(closingConnectionEvent, "ConnectorId", processId);
            AssertLoggingStateContains(closedConnectionEvent, "ConnectorId", processId);
        }

        var ids = new[]
        {
            NpgsqlEventId.OpeningPhysicalConnection,
            NpgsqlEventId.OpenedPhysicalConnection,
            NpgsqlEventId.ClosingPhysicalConnection,
            NpgsqlEventId.ClosedPhysicalConnection
        };

        foreach (var id in ids)
            Assert.That(listLoggerProvider.Log.Count(l => l.Id == id), Is.Zero);
    }

    [Test]
    public async Task Log_Open_Close_physical()
    {
        if (IsMultiplexing)
            return;

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider, csb.ToString());
        await using var conn = dataSource.CreateConnection();

        int processId, port;
        string host, database;
        using (listLoggerProvider.Record())
        {
            await conn.OpenAsync();
            (processId, host, port, database) = (conn.ProcessID, conn.Host!, conn.Port, conn.Database);
            await conn.CloseAsync();
        }

        var openingConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.OpeningPhysicalConnection);
        AssertLoggingConnectionString(conn, openingConnectionEvent.State);
        AssertLoggingStateContains(openingConnectionEvent, "Host", host);
        AssertLoggingStateContains(openingConnectionEvent, "Port", port);
        AssertLoggingStateContains(openingConnectionEvent, "Database", database);

        var openedConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.OpenedPhysicalConnection);
        AssertLoggingConnectionString(conn, openedConnectionEvent.State);
        AssertLoggingStateContains(openedConnectionEvent, "ConnectorId", processId);
        AssertLoggingStateContains(openingConnectionEvent, "Host", host);
        AssertLoggingStateContains(openingConnectionEvent, "Port", port);
        AssertLoggingStateContains(openingConnectionEvent, "Database", database);
        AssertLoggingStateContains(openedConnectionEvent, "DurationMs");

        var closingConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.ClosingPhysicalConnection);
        AssertLoggingConnectionString(conn, closingConnectionEvent.State);
        AssertLoggingStateContains(closingConnectionEvent, "ConnectorId", processId);
        AssertLoggingStateContains(closingConnectionEvent, "Host", host);
        AssertLoggingStateContains(closingConnectionEvent, "Port", port);
        AssertLoggingStateContains(closingConnectionEvent, "Database", database);

        var closededConnectionEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.ClosedPhysicalConnection);
        AssertLoggingConnectionString(conn, closededConnectionEvent.State);
        AssertLoggingStateContains(closededConnectionEvent, "ConnectorId", processId);
        AssertLoggingStateContains(closededConnectionEvent, "Host", host);
        AssertLoggingStateContains(closededConnectionEvent, "Port", port);
        AssertLoggingStateContains(closededConnectionEvent, "Database", database);
    }

    void AssertLoggingConnectionString(NpgsqlConnection connection, object? logState)
    {
        var keyValuePairs = (IEnumerable<KeyValuePair<string, object?>>)logState!;
        var connectionString = keyValuePairs.Single(kvp => kvp.Key == "ConnectionString").Value;
        Assert.That(connectionString, Is.EqualTo(connection.ConnectionString));
        Assert.That(connectionString, Does.Not.Contain("Password"));
    }

    #endregion Logging tests

    public ConnectionTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
