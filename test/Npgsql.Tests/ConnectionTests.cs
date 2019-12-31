using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;
using static Npgsql.Util.Statics;

#if NET461
using System.Text;
#endif

namespace Npgsql.Tests
{
    public class ConnectionTests : MultiplexingTestBase
    {
        [Test, Description("Makes sure the connection goes through the proper state lifecycle")]
        //[Timeout(5000)]
        public async Task BasicLifecycle()
        {
            using var conn = new NpgsqlConnection(ConnectionString);

            bool eventOpen = false, eventClosed = false;
            conn.StateChange += (s, e) =>
            {
                if (e.OriginalState == ConnectionState.Closed && e.CurrentState == ConnectionState.Open)
                    eventOpen = true;
                if (e.OriginalState == ConnectionState.Open && e.CurrentState == ConnectionState.Closed)
                    eventClosed = true;
            };

            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));

            conn.Open();
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
            Assert.That(eventOpen, Is.True);

            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open | ConnectionState.Fetching));
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
            }

            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));

            conn.Close();
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            Assert.That(eventClosed, Is.True);
        }

        [Test]
        public async Task BreakWhileOpen()
        {
            if (IsMultiplexing)
                return;

            using var conn = new NpgsqlConnection(ConnectionString);

            conn.Open();

            using (var conn2 = await OpenConnectionAsync())
                conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

            // Allow some time for the pg_terminate to kill our connection
            using (var cmd = CreateSleepCommand(conn, 10))
                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
                    .AssignableTo<NpgsqlException>()
                );

            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
        }

        #region Connection Errors

#if IGNORE
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task ConnectionRefused(bool pooled)
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
        public async Task ConnectionRefusedAsync(bool pooled)
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
        public void InvalidUserId()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Username = "unknown", Pooling = false
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                Assert.That(conn.Open, Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("28P01")
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test, Description("Connects with a bad password to ensure the proper error is thrown")]
        public void AuthenticationFailure()
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

        #region ProvidePasswordCallback Tests

        [Test, Description("ProvidePasswordCallback is used when password is not supplied in connection string")]
        public async Task ProvidePasswordCallbackDelegateIsUsed()
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
        public void ProvidePasswordCallbackDelegateIsNotUsed()
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
        public void ProvidePasswordCallbackDelegateExceptionsAreWrapped()
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
        public void ProvidePasswordCallbackDelegateGetsCorrectArguments()
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
        #endregion

        [Test]
        public void BadDatabase()
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = "does_not_exist"
            };
            using (CreateTempPool(builder, out var connectionString))
            using (var conn = new NpgsqlConnection(connectionString))
                Assert.That(() => conn.Open(),
                    Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("3D000")
                );
        }

        [Test, Description("Tests that mandatory connection string parameters are indeed mandatory")]
        public void MandatoryConnectionStringParams()
        {
            Assert.That(() => new NpgsqlConnection("User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests").Open(), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test, Description("Reuses the same connection instance for a failed connection, then a successful one")]
        public async Task FailConnectThenSucceed()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");

            var dbName = GetUniqueIdentifier(nameof(FailConnectThenSucceed));
            using (var conn1 = await OpenConnectionAsync())
            {
                conn1.ExecuteNonQuery($"DROP DATABASE IF EXISTS \"{dbName}\"");
                try
                {
                    var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = dbName,
                        Pooling = false
                    }.ToString();

                    using (var conn2 = new NpgsqlConnection(connString))
                    {
                        Assert.That(() => conn2.Open(),
                            Throws.Exception.TypeOf<PostgresException>()
                            .With.Property(nameof(PostgresException.SqlState)).EqualTo("3D000") // database doesn't exist
                        );
                        Assert.That(conn2.FullState, Is.EqualTo(ConnectionState.Closed));

                        conn1.ExecuteNonQuery($"CREATE DATABASE \"{dbName}\" TEMPLATE template0");

                        conn2.Open();
                        conn2.Close();
                    }
                }
                finally
                {
                    //conn1.ExecuteNonQuery($"DROP DATABASE IF EXISTS \"{dbName}\"");
                }
            }
        }

        [Test]
        [Timeout(10000)]
        public void  ConnectTimeout()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                return; // https://github.com/nunit/nunit/issues/3282
            //Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) {
                Host = unknownIp,
                Pooling = false,
                Timeout = 2
            };
            using (var conn = new NpgsqlConnection(csb.ToString()))
            {
                var sw = Stopwatch.StartNew();
                Assert.That(() => conn.Open(), Throws.Exception.TypeOf<TimeoutException>());
                Assert.That(sw.Elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo((csb.Timeout * 1000) - 100),
                    $"Timeout was supposed to happen after {csb.Timeout} seconds, but fired after {sw.Elapsed.TotalSeconds}");
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        [Timeout(10000)]
        public void ConnectTimeoutAsync()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                return; // https://github.com/nunit/nunit/issues/3282
            // Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = unknownIp,
                Pooling = false,
                Timeout = 2
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                Assert.That(async () => await conn.OpenAsync(), Throws.Exception.TypeOf<TimeoutException>());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        [Timeout(10000)]
        public void ConnectTimeoutCancel()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                return; // https://github.com/nunit/nunit/issues/3282
            //Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection cancellation tests");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = unknownIp,
                Pooling = false,
                Timeout = 30
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                var cts = new CancellationTokenSource(1000);
                Assert.That(async () => await conn.OpenAsync(cts.Token), Throws.Exception.TypeOf<OperationCanceledException>());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        #endregion

        #region Client Encoding

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
        public async Task ClientEncodingIsUTF8ByDefault()
        {
            using (var conn = await OpenConnectionAsync())
                Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.EqualTo("UTF8"));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
        [NonParallelizable]
        public async Task ClientEncodingEnvVar()
        {
            using (var testConn = await OpenConnectionAsync())
                Assert.That(await testConn.ExecuteScalarAsync("SHOW client_encoding"), Is.Not.EqualTo("SQL_ASCII"));

            // Note that the pool is unaware of the environment variable, so if a connection is
            // returned from the pool it may contain the wrong client_encoding
            using var _ = SetEnvironmentVariable("PGCLIENTENCODING", "SQL_ASCII");
            using var __ = CreateTempPool(ConnectionString, out var connectionString);

            var connString = new NpgsqlConnectionStringBuilder(connectionString);
            using var conn = await OpenConnectionAsync(connString);
            Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.EqualTo("SQL_ASCII"));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
        public async Task ClientEncodingConnectionParam()
        {
            using (var conn = await OpenConnectionAsync())
                Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.Not.EqualTo("SQL_ASCII"));
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) { ClientEncoding = "SQL_ASCII" };
            using (var conn = await OpenConnectionAsync(connString))
                Assert.That(await conn.ExecuteScalarAsync("SHOW client_encoding"), Is.EqualTo("SQL_ASCII"));
        }

        #endregion Client Encoding

        #region Timezone

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1634")]
        [NonParallelizable]
        public async Task TimezoneEnvVar()
        {
            string newTimezone;
            using (var conn1 = await OpenConnectionAsync())
            {
                newTimezone = (string)await conn1.ExecuteScalarAsync("SHOW TIMEZONE") == "Africa/Bamako"
                    ? "Africa/Lagos"
                    : "Africa/Bamako";
            }

            // Note that the pool is unaware of the environment variable, so if a connection is
            // returned from the pool it may contain the wrong timezone
            using var _ = SetEnvironmentVariable("PGTZ", newTimezone);
            using var __ = CreateTempPool(ConnectionString, out var connectionString);
            using var conn2 = await OpenConnectionAsync(connectionString);
            Assert.That(await conn2.ExecuteScalarAsync("SHOW TIMEZONE"), Is.EqualTo(newTimezone));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1634")]
        public async Task TimezoneConnectionParam()
        {
            string newTimezone;
            using (var conn = await OpenConnectionAsync())
            {
                newTimezone = (string)await conn.ExecuteScalarAsync("SHOW TIMEZONE") == "Africa/Bamako"
                    ? "Africa/Lagos"
                    : "Africa/Bamako";
            }

            var _ = CreateTempPool(ConnectionString, out var connString);
            var builder = new NpgsqlConnectionStringBuilder(connString)
            {
                Timezone = newTimezone
            };
            using (var conn = await OpenConnectionAsync(builder.ConnectionString))
                Assert.That(await conn.ExecuteScalarAsync("SHOW TIMEZONE"), Is.EqualTo(newTimezone));
        }

        #endregion Timezone

        [Test]
        public async Task UnixDomainSocket()
        {
            if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                if (Environment.OSVersion.Version.Major < 10 || Environment.OSVersion.Version.Build < 17093)
                    Assert.Ignore("Unix-domain sockets support was introduced in Windows build 17093");

                // On Windows we first need a classic IP connection to make sure we're running against the
                // right backend version
                using var versionConnection = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(versionConnection, "13.0",
                    "Unix-domain sockets support on Windows was introduced in PostgreSQL 13");
            }

            var port = new NpgsqlConnectionStringBuilder(ConnectionString).Port;
            var candidateDirectories = new[] { "/var/run/postgresql", "/tmp", Environment.GetEnvironmentVariable("TMP") ?? "C:\\" };
            var dir = candidateDirectories.FirstOrDefault(d => File.Exists(Path.Combine(d, $".s.PGSQL.{port}")));
            if (dir == null)
            {
                IgnoreExceptOnBuildServer("No PostgreSQL unix domain socket was found");
                return;
            }

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = dir
            };

            try
            {
                using var conn = await OpenConnectionAsync(csb);
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
                Assert.That(conn.DataSource, Is.EqualTo(Path.Combine(csb.Host, $".s.PGSQL.{port}")));
            }
            catch (PostgresException e) when (e.SqlState.StartsWith("28"))
            {
                if (TestUtil.IsOnBuildServer)
                    throw;
                Assert.Ignore("Connection via unix domain socket failed");
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/903")]
        public void DataSource()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
                Assert.That(conn.DataSource, Is.EqualTo($"tcp://{conn.Host}:{conn.Port}"));

            var bld = new NpgsqlConnectionStringBuilder(ConnectionString);
            bld.Host = "Otherhost";

            using (var conn = new NpgsqlConnection(bld.ToString()))
                Assert.That(conn.DataSource, Is.EqualTo($"tcp://{conn.Host}:{conn.Port}"));

            bld = new NpgsqlConnectionStringBuilder(ConnectionString);
            bld.Port = 5435;

            using (var conn = new NpgsqlConnection(bld.ToString()))
                Assert.That(conn.DataSource, Is.EqualTo($"tcp://{conn.Host}:{conn.Port}"));
        }


        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2763")]
        public void DataSourceDefault()
        {
            using (var conn = new NpgsqlConnection())
            {
                Assert.That(conn.DataSource, Is.EqualTo(string.Empty));

                conn.ConnectionString = ConnectionString;
                Assert.That(conn.DataSource, Is.EqualTo($"tcp://{conn.Host}:{conn.Port}"));
            }
        }

        [Test]
        public void SetConnectionString()
        {
            using (var conn = new NpgsqlConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
                Assert.That(() => conn.ConnectionString = "", Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void EmptyCtor()
        {
            var conn = new NpgsqlConnection();
            Assert.That(conn.ConnectionTimeout, Is.EqualTo(NpgsqlConnectionStringBuilder.DefaultTimeout));
            Assert.That(conn.ConnectionString, Is.SameAs(string.Empty));
            Assert.That(() => conn.Open(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/703")]
        public async Task NoDatabaseDefaultsToUsername()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Database = null };
            using (var conn = new NpgsqlConnection(csb.ToString()))
            {
                Assert.That(conn.Database, Is.EqualTo(csb.Username));
                conn.Open();
                Assert.That(await conn.ExecuteScalarAsync("SELECT current_database()"), Is.EqualTo(csb.Username));
                Assert.That(conn.Database, Is.EqualTo(csb.Username));
            }
        }

        [Test, Description("Breaks a connector while it's in the pool, with a keepalive and without")]
        [TestCase(false, TestName = "BreakConnectorInPoolWithoutKeepAlive")]
        [TestCase(true, TestName = "BreakConnectorInPoolWithKeepAlive")]
        public async Task BreakConnectorInPool(bool keepAlive)
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, hanging");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { MaxPoolSize = 1 };
            if (keepAlive)
                csb.KeepAlive = 1;
            using (var conn = new NpgsqlConnection(csb.ToString()))
            {
                conn.Open();
                var connectorId = conn.ProcessID;
                conn.Close();

                // Use another connection to kill the connector currently in the pool
                using (var conn2 = await OpenConnectionAsync())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({connectorId})");

                // Allow some time for the terminate to occur
                Thread.Sleep(2000);

                conn.Open();
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                if (keepAlive)
                {
                    Assert.That(conn.ProcessID, Is.Not.EqualTo(connectorId));
                    Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
                }
                else
                {
                    Assert.That(conn.ProcessID, Is.EqualTo(connectorId));
                    Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 1"), Throws.Exception
                        .AssignableTo<NpgsqlException>());
                }
            }
        }

        #region ChangeDatabase

        [Test]
        public async Task ChangeDatabase()
        {
            using (var conn = await OpenConnectionAsync())
            {
                conn.ChangeDatabase("template1");
                using (var cmd = new NpgsqlCommand("select current_database()", conn))
                    Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("template1"));
            }
        }

        [Test]
        public async Task ChangeDatabaseDoesNotAffectOtherConnections()
        {
            using (var conn1 = new NpgsqlConnection(ConnectionString))
            using (var conn2 = new NpgsqlConnection(ConnectionString))
            {
                // Connection 1 changes database
                conn1.Open();
                conn1.ChangeDatabase("template1");
                Assert.That(await conn1.ExecuteScalarAsync("SELECT current_database()"), Is.EqualTo("template1"));

                // Connection 2's database should not changed
                conn2.Open();
                Assert.That(await conn2.ExecuteScalarAsync("SELECT current_database()"), Is.Not.EqualTo(conn1.Database));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1331")]
        public void ChangeDatabaseConnectionNotOpen()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
                Assert.That(() => conn.ChangeDatabase("template1"), Throws.Exception
                    .TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Connection is not open"));
        }

        #endregion

        [Test, Description("Tests closing a connector while a reader is open")]
        [Timeout(10000)]
        public async Task CloseDuringRead([Values(PooledOrNot.Pooled, PooledOrNot.Unpooled)] PooledOrNot pooled)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            if (pooled == PooledOrNot.Unpooled)
            {
                if (IsMultiplexing)
                    return; // Multiplexing requires pooling
                csb.Pooling = false;
            }

            using (var conn = await OpenConnectionAsync(csb))
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
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
        }

        [Test]
        public async Task SearchPath()
        {
            using (var conn = await OpenConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString) { SearchPath = "foo" }))
                Assert.That(await conn.ExecuteScalarAsync("SHOW search_path"), Contains.Substring("foo"));
        }

        [Test]
        public async Task SetOptions()
        {
            using var _ = CreateTempPool(new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Options = "default_transaction_isolation=serializable  default_transaction_deferrable=on foo.bar=My\\ Famous\\\\Thing"
            }, out var connectionString);

            using var conn = await OpenConnectionAsync(connectionString);

            Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_isolation"), Is.EqualTo("serializable"));
            Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_deferrable"), Is.EqualTo("on"));
            Assert.That(await conn.ExecuteScalarAsync("SHOW foo.bar"), Is.EqualTo("My Famous\\Thing"));
        }

        [Test]
        public async Task ConnectorNotInitializedException1000581()
        {
            var command = new NpgsqlCommand();
            command.CommandText = @"SELECT 123";

            for (var i = 0; i < 2; i++)
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    var tx = connection.BeginTransaction();
                    await command.ExecuteScalarAsync();
                    await tx.CommitAsync();
                }
            }
        }

        [Test]
        [Ignore("")]
        public void NpgsqlErrorRepro1()
        {
            throw new NotImplementedException();
#if WHAT_TO_DO_WITH_THIS
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var largeObjectMgr = new LargeObjectManager(connection);
                    try
                    {
                        var largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                        transaction.Commit();
                    }
                    catch
                    {
                        // ignore the LO failure
                    }
                } // *1* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // *2* this fails if the initial connection is used
                    }
                }
            } // *3* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"
#endif
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

        [Test]
        public void NpgsqlErrorRepro2()
        {
#if WHAT_TO_DO_WITH_THIS
            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            var largeObjectMgr = new LargeObjectManager(connection);
            try
            {
                var largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                transaction.Commit();
            }
            catch
            {
                // ignore the LO failure
                try
                {
                    transaction.Dispose();
                }
                catch
                {
                    // ignore dispose failure
                }
                try
                {
                    connection.Dispose();
                }
                catch
                {
                    // ignore dispose failure
                }
            }

            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        // *1* this fails if the connection for the pool happens to be the bad one from above
                        Assert.IsTrue(!String.IsNullOrEmpty((string)reader["datname"]));
                    }
                }
            }
#endif
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/pull/164")]
        public void voidConnectionStateWhenDisposed()
        {
            var c = new NpgsqlConnection();
            c.Dispose();
            Assert.AreEqual(ConnectionState.Closed, c.State);
        }

        [Test]
        public void ChangeApplicationNameWithConnectionStringBuilder()
        {
            // Test for issue #165 on github.
            var builder = new NpgsqlConnectionStringBuilder();
            builder.ApplicationName = "test";
        }

        [Test, Description("Makes sure notices are probably received and emitted as events")]
        public async Task Notice()
        {
            using (var conn = await OpenConnectionAsync())
            await using (GetTempFunctionName(conn, out var function))
            {
                // Make sure messages are in English
                await conn.ExecuteNonQueryAsync($@"
SET lc_messages='en_US.UTF-8';

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
        }

        [Test, Description("Makes sure that concurrent use of the connection throws an exception")]
        public async Task ConcurrentUse()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");
            using (var conn = await OpenConnectionAsync())
            {
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
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void PersistSecurityInfoIsOn([Values(true, false)] bool pooling)
        {
            if (IsMultiplexing && !pooling)
                return;

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                PersistSecurityInfo = true,
                Pooling = pooling
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                var passwd = new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password;
                Assert.That(passwd, Is.Not.Null);
                conn.Open();
                Assert.That(new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password, Is.EqualTo(passwd));
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void NoPasswordWithoutPersistSecurityInfo([Values(true, false)] bool pooling)
        {
            if (IsMultiplexing && !pooling)
                return;

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = pooling
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
                Assert.That(csb.PersistSecurityInfo, Is.False);
                Assert.That(csb.Password, Is.Not.Null);
                conn.Open();
                Assert.That(new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password, Is.Null);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2725")]
        public void CloneWithAndPersistSecurityInfo()
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
        [IssueLink("https://github.com/npgsql/npgsql/issues/743")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void Clone()
        {
            using (CreateTempPool(ConnectionString, out var connectionString))
            using (var conn = new NpgsqlConnection(connectionString))
            {
                ProvideClientCertificatesCallback callback1 = certificates => { };
                conn.ProvideClientCertificatesCallback = callback1;
                RemoteCertificateValidationCallback callback2 = (sender, certificate, chain, errors) => true;
                conn.UserCertificateValidationCallback = callback2;

                conn.Open();
                Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));

                using (var conn2 = (NpgsqlConnection)((ICloneable)conn).Clone())
                {
                    Assert.That(conn2.ConnectionString, Is.EqualTo(conn.ConnectionString));
                    Assert.That(conn2.ProvideClientCertificatesCallback, Is.SameAs(callback1));
                    Assert.That(conn2.UserCertificateValidationCallback, Is.SameAs(callback2));
                    conn2.Open();
                    Assert.That(async () => await conn2.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/824")]
        public async Task ReloadTypes()
        {
            if (IsMultiplexing)
                return;

            using (CreateTempPool(ConnectionString, out var connectionString))
            using (var conn = await OpenConnectionAsync(connectionString))
            using (var conn2 = await OpenConnectionAsync(connectionString))
            {
                Assert.That(await conn.ExecuteScalarAsync("SELECT EXISTS (SELECT * FROM pg_type WHERE typname='reload_types_enum')"),
                    Is.False);
                await conn.ExecuteNonQueryAsync("CREATE TYPE pg_temp.reload_types_enum AS ENUM ('First', 'Second')");
                Assert.That(() => conn.TypeMapper.MapEnum<ReloadTypesEnum>(), Throws.Exception.TypeOf<ArgumentException>());
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<ReloadTypesEnum>();

                // Make sure conn2 picks up the new type after a pooled close
                var connId = conn2.ProcessID;
                conn2.Close();
                conn2.Open();
                Assert.That(conn2.ProcessID, Is.EqualTo(connId), "Didn't get the same connector back");
                conn2.TypeMapper.MapEnum<ReloadTypesEnum>();
            }
        }
        enum ReloadTypesEnum { First, Second };

        [Test]
        public async Task DatabaseInfoIsShared()
        {
            if (IsMultiplexing)
                return;
            using (var conn1 = await OpenConnectionAsync())
            using (var conn2 = await OpenConnectionAsync())
                Assert.That(conn1.Connector!.DatabaseInfo, Is.SameAs(conn2.Connector!.DatabaseInfo));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/736")]
        public async Task ManyOpenClose()
        {
            // The connector's _sentRfqPrependedMessages is a byte, too many open/closes made it overflow
            for (var i = 0; i < 255; i++)
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/736")]
        public async Task ManyOpenCloseWithTransaction()
        {
            // The connector's _sentRfqPrependedMessages is a byte, too many open/closes made it overflow
            for (var i = 0; i < 255; i++)
            {
                using (var conn = await OpenConnectionAsync())
                    conn.BeginTransaction();
            }
            using (var conn = await OpenConnectionAsync())
                Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/927")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/736")]
        [Ignore("Fails when running the entire test suite but not on its own...")]
        public async Task RollbackOnClose()
        {
            // Npgsql 3.0.0 to 3.0.4 prepended a rollback for the next time the connector is used, as an optimization.
            // This caused some issues (#927) and was removed.

            // Clear connections in pool as we're going to need to reopen the same connection
            var dummyConn = new NpgsqlConnection(ConnectionString);
            NpgsqlConnection.ClearPool(dummyConn);

            int processId;
            using (var conn = await OpenConnectionAsync())
            {
                processId = conn.Connector!.BackendProcessId;
                conn.BeginTransaction();
                await conn.ExecuteNonQueryAsync("SELECT 1");
                Assert.That(conn.Connector.TransactionStatus, Is.EqualTo(TransactionStatus.InTransactionBlock));
            }
            using (var conn = await OpenConnectionAsync())
            {
                Assert.That(conn.Connector!.BackendProcessId, Is.EqualTo(processId));
                Assert.That(conn.Connector.TransactionStatus, Is.EqualTo(TransactionStatus.Idle));
            }
        }

        [Test, Description("Tests an exception happening when sending the Terminate message while closing a ready connector")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/777")]
        [Ignore("Flaky")]
        public async Task ExceptionDuringClose()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };
            using (var conn = await OpenConnectionAsync(csb))
            {
                var connectorId = conn.ProcessID;

                using (var conn2 = await OpenConnectionAsync())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({connectorId})");

                conn.Close();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1180")]
        public void PoolByPassword()
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

        [Test, Description("Some pseudo-PG database don't support pg_type loading, we have a minimal DatabaseInfo for this")]
        public async Task NoTypeLoading()
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ServerCompatibilityMode = ServerCompatibilityMode.NoTypeLoading
            };

            using var _ = CreateTempPool(builder, out var connectionString);
            using var conn = await OpenConnectionAsync(connectionString);
            // Arrays should not be supported in this mode
            Assert.That(async () => await conn.ExecuteScalarAsync("SELECT '{1,2,3}'::INTEGER[]"),
                Throws.Exception.TypeOf<NotSupportedException>());
            // Test that some basic types do work
            Assert.That(await conn.ExecuteScalarAsync("SELECT 8"), Is.EqualTo(8));
            Assert.That(await conn.ExecuteScalarAsync("SELECT 'foo'"), Is.EqualTo("foo"));
            Assert.That(await conn.ExecuteScalarAsync("SELECT TRUE"), Is.EqualTo(true));
            Assert.That(await conn.ExecuteScalarAsync("SELECT INET '192.168.1.1'"), Is.EqualTo(IPAddress.Parse("192.168.1.1")));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1158")]
        public async Task TableNamedRecord()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "", out var table);

                conn.ReloadTypes();

                Assert.That(await conn.ExecuteScalarAsync("SELECT COUNT(*) FROM record"), Is.Zero);
            }
        }

#if NET461
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/392")]
        public async Task NonUTF8Encoding()
        {
            using (var adminConn = await OpenConnectionAsync())
            {
                // Create the database with server encoding sql-ascii
                adminConn.ExecuteNonQuery("DROP DATABASE IF EXISTS sqlascii");
                adminConn.ExecuteNonQuery("CREATE DATABASE sqlascii ENCODING 'sql_ascii' TEMPLATE template0");
                try
                {
                    // Insert some win1252 data
                    var goodCsb = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = "sqlascii",
                        Encoding = "windows-1252",
                        ClientEncoding = "sql-ascii",
                        Pooling = false
                    };
                    using (var conn = await OpenConnectionAsync(goodCsb))
                    {
                        await conn.ExecuteNonQueryAsync("CREATE TABLE foo (bar TEXT)");
                        await conn.ExecuteNonQueryAsync("INSERT INTO foo (bar) VALUES ('éàç')");
                        Assert.That(await conn.ExecuteScalarAsync("SELECT * FROM foo"), Is.EqualTo("éàç"));
                    }

                    // A normal connection with the default UTF8 encoding and client_encoding should fail
                    var badCsb = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = "sqlascii",
                        Pooling = false
                    };
                    using (var conn = await OpenConnectionAsync(badCsb))
                    {
                        Assert.That(async () => await conn.ExecuteScalarAsync("SELECT * FROM foo"),
                            Throws.Exception.TypeOf<PostgresException>()
                                .With.Property(nameof(PostgresException.SqlState)).EqualTo("22021")
                                .Or.TypeOf<DecoderFallbackException>()
                        );
                    }
                }
                finally
                {
                    adminConn.ExecuteNonQuery("DROP DATABASE IF EXISTS sqlascii");
                }
            }
        }
#endif

        [Test]
        public async Task OversizeBuffer()
        {
            if (IsMultiplexing)
                return;

            using (CreateTempPool(ConnectionString, out var connectionString))
            using (var conn = await OpenConnectionAsync(connectionString))
            {
                var csb = new NpgsqlConnectionStringBuilder(connectionString);

                Assert.That(conn.Connector!.ReadBuffer.Size, Is.EqualTo(csb.ReadBufferSize));

                // Read a big row, we should now be using an oversize buffer
                var bigString1 = new string('x', csb.ReadBufferSize + 10);
                using (var cmd = new NpgsqlCommand($"SELECT '{bigString1}'", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetString(0), Is.EqualTo(bigString1));
                }
                var size1 = conn.Connector.ReadBuffer.Size;
                Assert.That(conn.Connector.ReadBuffer.Size, Is.GreaterThan(csb.ReadBufferSize));

                // Even bigger oversize buffer
                var bigString2 = new string('x', csb.ReadBufferSize + 20);
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
        }

        [Test, Explicit, Description("Turns on TCP keepalive and sleeps forever, good for wiresharking")]
        public async Task TcpKeepaliveTime()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                TcpKeepAliveTime = 2000
            };
            using (await OpenConnectionAsync(csb))
                Thread.Sleep(Timeout.Infinite);
        }

        [Test, Explicit, Description("Turns on TCP keepalive and sleeps forever, good for wiresharking")]
        public async Task TcpKeepalive()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                TcpKeepAlive = true
            };
            using (await OpenConnectionAsync(csb))
                Thread.Sleep(Timeout.Infinite);
        }

        [Test]
        public async Task ChangeParameter()
        {
            if (IsMultiplexing)
                return;

            using (var conn = await OpenConnectionAsync())
            {
                await conn.ExecuteNonQueryAsync("SET application_name = 'some_test_value'");
                Assert.That(conn.PostgresParameters["application_name"], Is.EqualTo("some_test_value"));
                await conn.ExecuteNonQueryAsync("SET application_name = 'some_test_value2'");
                Assert.That(conn.PostgresParameters["application_name"], Is.EqualTo("some_test_value2"));
            }
        }

        [Test]
        [NonParallelizable]
        public async Task Connect_UserNameFromEnvironment_Succeeds()
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { IntegratedSecurity = false };
            using var _ = SetEnvironmentVariable("PGUSER", builder.Username);
            builder.Username = null;
            using var __ = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var ___ = await OpenConnectionAsync(connectionString);
        }

        [Test]
        [NonParallelizable]
        public async Task Connect_PasswordFromEnvironment_Succeeds()
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { IntegratedSecurity = false };
            using var _ = SetEnvironmentVariable("PGPASSWORD", builder.Password);
            builder.Password = null;
            using var __ = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var ___ = await OpenConnectionAsync(connectionString);
        }

        [Test]
        [NonParallelizable]
        public async Task Connect_OptionsFromEnvironment_Succeeds()
        {
            using (SetEnvironmentVariable("PGOPTIONS", "default_transaction_isolation=serializable  default_transaction_deferrable=on foo.bar=My\\ Famous\\\\Thing"))
            {
                using var _ = CreateTempPool(ConnectionString, out var connectionString);
                using var conn = await OpenConnectionAsync(connectionString);
                Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_isolation"), Is.EqualTo("serializable"));
                Assert.That(await conn.ExecuteScalarAsync("SHOW default_transaction_deferrable"), Is.EqualTo("on"));
                Assert.That(await conn.ExecuteScalarAsync("SHOW foo.bar"), Is.EqualTo("My Famous\\Thing"));
            }
        }

        #region pgpass

        [Test]
        [NonParallelizable]
        public async Task UsePgPassFile()
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                IntegratedSecurity = false
            };

            var pgpassFile = Path.GetTempFileName();
            using var _ = Defer(() => File.Delete(pgpassFile));
            File.WriteAllText(pgpassFile, $"*:*:*:{builder.Username}:{builder.Password}");
            using var __ = SetEnvironmentVariable("PGPASSFILE", pgpassFile);

            builder.Password = null;
            using var ___ = CreateTempPool(builder.ConnectionString, out var connectionString);
            using var ____ = await OpenConnectionAsync(connectionString);
        }

        #endregion

        public ConnectionTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
