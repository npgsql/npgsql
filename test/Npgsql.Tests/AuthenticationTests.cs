using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;
using Npgsql.Tests.Support;
using NUnit.Framework;
using static Npgsql.Util.Statics;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class AuthenticationTests : MultiplexingTestBase
{
    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Connect_UserNameFromEnvironment_Succeeds()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { IntegratedSecurity = false };
        using var _ = SetEnvironmentVariable("PGUSER", builder.Username);
        builder.Username = null;
        using var __ = CreateTempPool(builder.ConnectionString, out var connectionString);
        using var ___ = await OpenConnectionAsync(connectionString);
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Connect_PasswordFromEnvironment_Succeeds()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { IntegratedSecurity = false };
        using var _ = SetEnvironmentVariable("PGPASSWORD", builder.Password);
        builder.Password = null;
        using var __ = CreateTempPool(builder.ConnectionString, out var connectionString);
        using var ___ = await OpenConnectionAsync(connectionString);
    }

    [Test]
    public async Task Set_Password_on_NpgsqlDataSource()
    {
        var dataSourceBuilder = GetPasswordlessDataSourceBuilder();
        await using var dataSource = dataSourceBuilder.Build();

        // No password provided
        Assert.That(() => dataSource.OpenConnectionAsync(), Throws.Exception.TypeOf<NpgsqlException>());

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(TestUtil.ConnectionString);
        dataSource.Password = connectionStringBuilder.Password!;

        await using var connection1 = await dataSource.OpenConnectionAsync();
        await using var connection2 = dataSource.OpenConnection();
    }

    [Test]
    public async Task Periodic_password_provider()
    {
        var dataSourceBuilder = GetPasswordlessDataSourceBuilder();
        var password = new NpgsqlConnectionStringBuilder(TestUtil.ConnectionString).Password!;

        var mre = new ManualResetEvent(false);
        dataSourceBuilder.UsePeriodicPasswordProvider((_, _) =>
        {
            mre.Set();
            return new(password);
        }, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10));

        await using (var dataSource = dataSourceBuilder.Build())
        {
            await using var connection1 = await dataSource.OpenConnectionAsync();
            await using var connection2 = dataSource.OpenConnection();

            mre.Reset();
            if (!mre.WaitOne(TimeSpan.FromSeconds(30)))
                Assert.Fail("Periodic password refresh did not occur");
        }

        mre.Reset();
        if (mre.WaitOne(TimeSpan.FromSeconds(1)))
            Assert.Fail("Periodic password refresh occurred after disposal of the data source");
    }

    [Test]
    public async Task Periodic_password_provider_with_first_time_exception()
    {
        var dataSourceBuilder = GetPasswordlessDataSourceBuilder();
        dataSourceBuilder.UsePeriodicPasswordProvider(
            (_, _) => throw new Exception("FOO"), TimeSpan.FromDays(30), TimeSpan.FromSeconds(10));
        await using var dataSource = dataSourceBuilder.Build();

        Assert.That(() => dataSource.OpenConnectionAsync(), Throws.Exception.TypeOf<NpgsqlException>()
            .With.InnerException.With.Message.EqualTo("FOO"));
        Assert.That(() => dataSource.OpenConnection(), Throws.Exception.TypeOf<NpgsqlException>()
            .With.InnerException.With.Message.EqualTo("FOO"));
    }

    [Test]
    public async Task Periodic_password_provider_with_second_time_exception()
    {
        var dataSourceBuilder = GetPasswordlessDataSourceBuilder();
        var password = new NpgsqlConnectionStringBuilder(TestUtil.ConnectionString).Password!;

        var times = 0;
        var mre = new ManualResetEvent(false);

        dataSourceBuilder.UsePeriodicPasswordProvider(
            (_, _) =>
            {
                if (times++ > 1)
                {
                    mre.Set();
                    throw new Exception("FOO");
                }

                return new(password);
            },
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(10));
        await using var dataSource = dataSourceBuilder.Build();

        mre.WaitOne();

        // The periodic timer threw, but previously returned a password. Make sure we keep using that last known one.
        using (await dataSource.OpenConnectionAsync()) {}
        using (dataSource.OpenConnection()) {}
    }

    [Test]
    public void Both_password_and_password_provider_is_not_supported()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(TestUtil.ConnectionString);
        dataSourceBuilder.UsePeriodicPasswordProvider((_, _) => new("foo"), TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
        Assert.That(() => dataSourceBuilder.Build(), Throws.Exception.TypeOf<NotSupportedException>()
            .With.Message.EqualTo(NpgsqlStrings.CannotSetBothPasswordProviderAndPassword));
    }

    #region pgpass

    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Use_pgpass_from_connection_string()
    {
        using var resetPassword = SetEnvironmentVariable("PGPASSWORD", null);
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);

        var password = builder.Password;
        builder.Password = null;

        var passFile = Path.GetTempFileName();
        File.WriteAllText(passFile, $"*:*:*:{builder.Username}:{password}");
        builder.Passfile = passFile;

        try
        {
            using var pool = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var conn = await OpenConnectionAsync(connectionString);
        }
        finally
        {
            File.Delete(passFile);
        }
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Use_pgpass_from_environment_variable()
    {
        using var resetPassword = SetEnvironmentVariable("PGPASSWORD", null);
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);

        var password = builder.Password;
        builder.Password = null;

        var passFile = Path.GetTempFileName();
        File.WriteAllText(passFile, $"*:*:*:{builder.Username}:{password}");
        using var passFileVariable = SetEnvironmentVariable("PGPASSFILE", passFile);

        try
        {
            using var pool = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var conn = await OpenConnectionAsync(connectionString);
        }
        finally
        {
            File.Delete(passFile);
        }
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Use_pgpass_from_homedir()
    {
        using var resetPassword = SetEnvironmentVariable("PGPASSWORD", null);
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);

        var password = builder.Password;
        builder.Password = null;

        string? dirToDelete = null;
        string passFile;
        string? previousPassFile = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var dir = Path.Combine(Environment.GetEnvironmentVariable("APPDATA")!, "postgresql");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                dirToDelete = dir;
            }
            passFile = Path.Combine(dir, "pgpass.conf");
        }
        else
        {
            passFile = Path.Combine(Environment.GetEnvironmentVariable("HOME")!, ".pgpass");
        }

        if (File.Exists(passFile))
        {
            previousPassFile = Path.GetTempFileName();
            File.Move(passFile, previousPassFile);
        }

        try
        {
            File.WriteAllText(passFile, $"*:*:*:{builder.Username}:{password}");
            using var pool = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var conn = await OpenConnectionAsync(connectionString);
        }
        finally
        {
            File.Delete(passFile);
            if (dirToDelete is not null)
                Directory.Delete(dirToDelete);
            if (previousPassFile is not null)
                File.Move(previousPassFile, passFile);
        }
    }

    #endregion pgpass

    [Test]
    [NonParallelizable] // Sets environment variable
    public void Password_source_precedence()
    {
        using var resetPassword = SetEnvironmentVariable("PGPASSWORD", null);
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);

        var password = builder.Password;
        var passwordBad = password + "_bad";

        var passFile = Path.GetTempFileName();
        var passFileBad = passFile + "_bad";

        using var deletePassFile = Defer(() => File.Delete(passFile));
        using var deletePassFileBad = Defer(() => File.Delete(passFileBad));

        File.WriteAllText(passFile, $"*:*:*:{builder.Username}:{password}");
        File.WriteAllText(passFileBad, $"*:*:*:{builder.Username}:{passwordBad}");

        using (var passFileVariable = SetEnvironmentVariable("PGPASSFILE", passFileBad))
        {
            // Password from the connection string goes first
            using (var passwordVariable = SetEnvironmentVariable("PGPASSWORD", passwordBad))
                Assert.That(OpenConnection(password, passFileBad), Throws.Nothing);

            // Password from the environment variable goes second
            using (var passwordVariable = SetEnvironmentVariable("PGPASSWORD", password))
                Assert.That(OpenConnection(password: null, passFileBad), Throws.Nothing);

            // Passfile from the connection string goes third
            Assert.That(OpenConnection(password: null, passFile: passFile), Throws.Nothing);
        }

        // Passfile from the environment variable goes fourth
        using (var passFileVariable = SetEnvironmentVariable("PGPASSFILE", passFile))
            Assert.That(OpenConnection(password: null, passFile: null), Throws.Nothing);

        Func<ValueTask> OpenConnection(string? password, string? passFile) => async () =>
        {
            builder.Password = password;
            builder.Passfile = passFile;
            builder.IntegratedSecurity = false;
            builder.ApplicationName = $"{nameof(Password_source_precedence)}:{Guid.NewGuid()}";

            using var pool = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var connection = await OpenConnectionAsync(connectionString);
        };
    }

    [Test, Description("Connects with a bad password to ensure the proper error is thrown")]
    public void Authentication_failure()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Password = "bad"
        };
        using (CreateTempPool(builder, out var connectionString))
        using (var conn = new NpgsqlConnection(connectionString))
        {
            Assert.That(() => conn.OpenAsync(), Throws.Exception
                .TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).StartsWith("28")
            );
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
        }
    }

    [Test, Description("Simulates a timeout during the authentication phase")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3227")]
    public async Task Timeout_during_authentication()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { Timeout = 1 };
        await using var postmasterMock = new PgPostmasterMock(builder.ConnectionString);
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);

        var __ = postmasterMock.AcceptServer();

        // The server will accept a connection from the client, but will not respond to the client's authentication
        // request. This should trigger a timeout
        Assert.That(async () => await OpenConnectionAsync(connectionString),
            Throws.Exception.TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<TimeoutException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1180")]
    public void Pool_by_password()
    {
        using var _ = CreateTempPool(ConnectionString, out var connectionString);
        using (var goodConn = new NpgsqlConnection(connectionString))
            goodConn.Open();

        var badConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Password = "badpasswd"
        }.ConnectionString;
        using (var conn = new NpgsqlConnection(badConnectionString))
            Assert.That(conn.Open, Throws.Exception.TypeOf<PostgresException>());
    }

    [Test, Explicit("Requires user specific local setup")]
    public async Task AuthenticateIntegratedSecurity()
    {
        await using var dataSource = NpgsqlDataSource.Create(new NpgsqlConnectionStringBuilder(ConnectionString)
            { IntegratedSecurity = true, Username = null, Password = null });
        await using var c = await  dataSource.OpenConnectionAsync();
        Assert.That(c.State, Is.EqualTo(ConnectionState.Open));
    }

    #region ProvidePasswordCallback Tests

#pragma warning disable CS0618 // ProvidePasswordCallback is Obsolete

    [Test, Description("ProvidePasswordCallback is used when password is not supplied in connection string")]
    public async Task ProvidePasswordCallback_is_used()
    {
        using var _ = CreateTempPool(ConnectionString, out var connString);
        var builder = new NpgsqlConnectionStringBuilder(connString);
        var goodPassword = builder.Password;
        var getPasswordDelegateWasCalled = false;
        builder.Password = null;

        Assume.That(goodPassword, Is.Not.Null);

        using (var conn = new NpgsqlConnection(builder.ConnectionString) { ProvidePasswordCallback = ProvidePasswordCallback })
        {
            conn.Open();
            Assert.True(getPasswordDelegateWasCalled, "ProvidePasswordCallback delegate not used");

            // Do this again, since with multiplexing the very first connection attempt is done via
            // the non-multiplexing path, to surface any exceptions.
            NpgsqlConnection.ClearPool(conn);
            conn.Close();
            getPasswordDelegateWasCalled = false;
            conn.Open();
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            Assert.True(getPasswordDelegateWasCalled, "ProvidePasswordCallback delegate not used");
        }

        string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            getPasswordDelegateWasCalled = true;
            return goodPassword!;
        }
    }

    [Test, Description("ProvidePasswordCallback is not used when password is supplied in connection string")]
    public void ProvidePasswordCallback_is_not_used()
    {
        using var _ = CreateTempPool(ConnectionString, out var connString);

        using (var conn = new NpgsqlConnection(connString) { ProvidePasswordCallback = ProvidePasswordCallback })
        {
            conn.Open();

            // Do this again, since with multiplexing the very first connection attempt is done via
            // the non-multiplexing path, to surface any exceptions.
            NpgsqlConnection.ClearPool(conn);
            conn.Close();
            conn.Open();
        }

        string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            throw new Exception("password should come from connection string, not delegate");
        }
    }

    [Test, Description("Exceptions thrown from client application are wrapped when using ProvidePasswordCallback Delegate")]
    public void ProvidePasswordCallback_exceptions_are_wrapped()
    {
        using var _ = CreateTempPool(ConnectionString, out var connString);
        var builder = new NpgsqlConnectionStringBuilder(connString)
        {
            Password = null
        };

        using (var conn = new NpgsqlConnection(builder.ConnectionString) { ProvidePasswordCallback = ProvidePasswordCallback })
        {
            Assert.That(() => conn.Open(), Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.Message.EqualTo("inner exception from ProvidePasswordCallback")
            );
        }

        string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            throw new Exception("inner exception from ProvidePasswordCallback");
        }
    }

    [Test, Description("Parameters passed to ProvidePasswordCallback delegate are correct")]
    public void ProvidePasswordCallback_gets_correct_arguments()
    {
        using var _ = CreateTempPool(ConnectionString, out var connString);
        var builder = new NpgsqlConnectionStringBuilder(connString);
        var goodPassword = builder.Password;
        builder.Password = null;

        Assume.That(goodPassword, Is.Not.Null);

        string? receivedHost = null;
        int? receivedPort = null;
        string? receivedDatabase = null;
        string? receivedUsername = null;

        using (var conn = new NpgsqlConnection(builder.ConnectionString) { ProvidePasswordCallback = ProvidePasswordCallback })
        {
            conn.Open();
            Assert.AreEqual(builder.Host, receivedHost);
            Assert.AreEqual(builder.Port, receivedPort);
            Assert.AreEqual(builder.Database, receivedDatabase);
            Assert.AreEqual(builder.Username, receivedUsername);
        }

        string ProvidePasswordCallback(string host, int port, string database, string username)
        {
            receivedHost = host;
            receivedPort = port;
            receivedDatabase = database;
            receivedUsername = username;

            return goodPassword!;
        }
    }

#pragma warning restore CS0618 // ProvidePasswordCallback is Obsolete

    #endregion

    NpgsqlDataSourceBuilder GetPasswordlessDataSourceBuilder()
        => new(TestUtil.ConnectionString)
        {
            ConnectionStringBuilder =
            {
                Password = null
            }
        };

    public AuthenticationTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
