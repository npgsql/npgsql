using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class SecurityTests : TestBase
    {
        [Test, Description("Establishes an SSL connection, assuming a self-signed server certificate")]
        public void BasicSsl()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };

            using var conn = OpenConnection(csb);
            Assert.That(conn.IsSecure, Is.True);
        }

        [Test, Description("Default user must run with md5 password encryption")]
        public void DefaultUserUsesMd5Password()
        {
            if (!TestUtil.IsOnBuildServer)
                Assert.Ignore("Only executed in CI");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };

            using var conn = OpenConnection(csb);
            Assert.That(conn.IsScram, Is.False);
            Assert.That(conn.IsScramPlus, Is.False);
        }

        [Test, Description("Makes sure a certificate whose root CA isn't known isn't accepted")]
        public void RejectSelfSignedCertificate([Values(SslMode.VerifyCA, SslMode.VerifyFull)] SslMode sslMode)
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = sslMode
            }.ToString();

            using var conn = new NpgsqlConnection(connString);
            // The following is necessary since a pooled connector may exist from a previous
            // SSL test
            NpgsqlConnection.ClearPool(conn);

            // TODO: Specific exception, align with SslStream
            Assert.That(() => conn.Open(), Throws.Exception);
        }

        [Test, Description("Makes sure that ssl_renegotiation_limit is always 0, renegotiation is buggy")]
        public void NoSslRenegotiation()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };

            using var conn = OpenConnection(csb);
            Assert.That(conn.ExecuteScalar("SHOW ssl_renegotiation_limit"), Is.EqualTo("0"));
            conn.ExecuteNonQuery("DISCARD ALL");
            Assert.That(conn.ExecuteScalar("SHOW ssl_renegotiation_limit"), Is.EqualTo("0"));
        }

        [Test, Description("Makes sure that when SSL is disabled IsSecure returns false")]
        public void NonSecure()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Disable,
            };
            using var conn = OpenConnection(csb);
            Assert.That(conn.IsSecure, Is.False);
        }

        [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
        public void IntegratedSecurityWithUsername()
        {
            var username = Environment.UserName;
            if (username == null)
                throw new Exception("Could find username");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                IntegratedSecurity = true,
                Username = username,
                Password = null
            }.ToString();
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                if (TestUtil.IsOnBuildServer)
                    throw;
                Console.WriteLine(e);
                Assert.Ignore("Integrated security (GSS/SSPI) doesn't seem to be set up");
            }
        }

        [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
        public void IntegratedSecurityWithoutUsername()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                IntegratedSecurity = true,
                Username = null,
                Password = null
            }.ToString();
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                if (TestUtil.IsOnBuildServer)
                    throw;
                Console.WriteLine(e);
                Assert.Ignore("Integrated security (GSS/SSPI) doesn't seem to be set up");
            }
        }

        [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
        public void ConnectionDatabasePopulatedOnConnect()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                IntegratedSecurity = true,
                Username = null,
                Password = null,
                Database = null
            }.ToString();
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                if (TestUtil.IsOnBuildServer)
                    throw;
                Console.WriteLine(e);
                Assert.Ignore("Integrated security (GSS/SSPI) doesn't seem to be set up");
            }
            Assert.That(conn.Database, Is.Not.Null);
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1718")]
        [Timeout(12000)]
        public void Bug1718()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
            };

            using var conn = OpenConnection(csb);
            using var cmd = CreateSleepCommand(conn, 10000);
            var cts = new CancellationTokenSource(1000).Token;
            Assert.That(async () => await cmd.ExecuteNonQueryAsync(cts), Throws.Exception
                .TypeOf<OperationCanceledException>()
                .With.InnerException.TypeOf<PostgresException>()
                .With.InnerException.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.QueryCanceled));
        }

        [Test]
        [Timeout(2000)]
        public void ConnectToDatabaseUsingScramPlus()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                Username = "npgsql_tests_scram",
                Password = "npgsql_tests_scram",
                TrustServerCertificate = true,
            };

            try
            {
                using var conn = OpenConnection(csb);
                // scram-sha-256-plus only works begining from PostgreSQL 11
                if (conn.PostgreSqlVersion.Major >= 11)
                {
                    Assert.That(conn.IsScramPlus, Is.True);
                }
            }
            catch (Exception e) when (!TestUtil.IsOnBuildServer)
            {
                Console.WriteLine(e);
                Assert.Ignore("scram-sha-256-plus doesn't seem to be set up");
            }
        }

        [Test]
        public async Task ConnectWithOnlySslAllowedUser([Values] bool multiplexing, [Values] bool keepAlive)
        {
            if (multiplexing && keepAlive)
            {
                Assert.Ignore("Multiplexing doesn't support keepalive");
            }

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Allow,
                Username = "npgsql_tests_ssl",
                Password = "npgsql_tests_ssl",
                Multiplexing = multiplexing,
                KeepAlive = keepAlive ? 10 : 0,
            };

            try
            {
                await using var conn = await OpenConnectionAsync(csb);
                Assert.IsTrue(conn.IsSecure);
            }
            catch (Exception e) when (!TestUtil.IsOnBuildServer)
            {
                Console.WriteLine(e);
                Assert.Ignore("Only ssl user doesn't seem to be set up");
            }
        }

        [Test]
        public void SslModeRequireThrowsWithoutTSC()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
            };

            var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await OpenConnectionAsync(csb))!;
            Assert.That(ex.Message, Is.EqualTo("SslMode.Require requires TrustServerCertificate to be explicitly set. Please see https://www.npgsql.org/doc/release-notes/6.0.html"));
        }

        [Test]
        public async Task ConnectWithOnlyNonSslAllowedUser([Values] bool multiplexing, [Values] bool keepAlive)
        {
            if (multiplexing && keepAlive)
            {
                Assert.Ignore("Multiplexing doesn't support keepalive");
            }

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Prefer,
                Username = "npgsql_tests_nossl",
                Password = "npgsql_tests_nossl",
                Multiplexing = multiplexing,
                KeepAlive = keepAlive ? 10 : 0,
            };

            try
            {
                await using var conn = await OpenConnectionAsync(csb);
                Assert.IsFalse(conn.IsSecure);
            }
            catch (Exception e) when (!TestUtil.IsOnBuildServer)
            {
                Console.WriteLine(e);
                Assert.Ignore("Only nonssl user doesn't seem to be set up");
            }
        }

        #region Setup / Teardown / Utils

        [OneTimeSetUp]
        public void CheckSslSupport()
        {
            using var conn = OpenConnection();
            var sslSupport = (string)conn.ExecuteScalar("SHOW ssl")!;
            if (sslSupport == "off")
                TestUtil.IgnoreExceptOnBuildServer("SSL support isn't enabled at the backend");
        }

        #endregion
    }
}
