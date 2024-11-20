using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class SecurityTests : TestBase
{
    [Test, Description("Establishes an SSL connection, assuming a self-signed server certificate")]
    public void Basic_ssl()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
        });
        using var conn = dataSource.OpenConnection();
        Assert.That(conn.IsSecure, Is.True);
    }

    [Test, Description("Default user must run with md5 password encryption")]
    public void Default_user_uses_md5_password()
    {
        if (!IsOnBuildServer)
            Assert.Ignore("Only executed in CI");

        using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
        });
        using var conn = dataSource.OpenConnection();
        Assert.That(conn.IsScram, Is.False);
        Assert.That(conn.IsScramPlus, Is.False);
    }

    [Test, Description("Makes sure a certificate whose root CA isn't known isn't accepted")]
    public void Reject_self_signed_certificate([Values(SslMode.VerifyCA, SslMode.VerifyFull)] SslMode sslMode)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            SslMode = sslMode,
            CheckCertificateRevocation = false,
        };

        using var _ = CreateTempPool(csb, out var connString);
        using var conn = new NpgsqlConnection(connString);

        var ex = Assert.Throws<NpgsqlException>(conn.Open)!;
        Assert.That(ex.InnerException, Is.TypeOf<AuthenticationException>());
    }

    [Test, Description("Makes sure that ssl_renegotiation_limit is always 0, renegotiation is buggy")]
    public void No_ssl_renegotiation()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
        });
        using var conn = dataSource.OpenConnection();
        Assert.That(conn.ExecuteScalar("SHOW ssl_renegotiation_limit"), Is.EqualTo("0"));
        conn.ExecuteNonQuery("DISCARD ALL");
        Assert.That(conn.ExecuteScalar("SHOW ssl_renegotiation_limit"), Is.EqualTo("0"));
    }

    [Test, Description("Makes sure that when SSL is disabled IsSecure returns false")]
    public void IsSecure_without_ssl()
    {
        using var dataSource = CreateDataSource(csb => csb.SslMode = SslMode.Disable);
        using var conn = dataSource.OpenConnection();
        Assert.That(conn.IsSecure, Is.False);
    }

    [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
    public void IntegratedSecurity_with_Username()
    {
        var username = Environment.UserName;
        if (username == null)
            throw new Exception("Could find username");

        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
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
            if (IsOnBuildServer)
                throw;
            Console.WriteLine(e);
            Assert.Ignore("Integrated security (GSS/SSPI) doesn't seem to be set up");
        }
    }

    [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
    public void IntegratedSecurity_without_Username()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
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
            if (IsOnBuildServer)
                throw;
            Console.WriteLine(e);
            Assert.Ignore("Integrated security (GSS/SSPI) doesn't seem to be set up");
        }
    }

    [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
    public void Connection_database_is_populated_on_Open()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
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
            if (IsOnBuildServer)
                throw;
            Console.WriteLine(e);
            Assert.Ignore("Integrated security (GSS/SSPI) doesn't seem to be set up");
        }
        Assert.That(conn.Database, Is.Not.Null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1718")]
    public void Bug1718()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
        });
        using var conn = dataSource.OpenConnection();
        using var tx = conn.BeginTransaction();
        using var cmd = CreateSleepCommand(conn, 10000);
        var cts = new CancellationTokenSource(1000).Token;
        Assert.That(async () => await cmd.ExecuteNonQueryAsync(cts), Throws.Exception
            .TypeOf<OperationCanceledException>()
            .With.InnerException.TypeOf<PostgresException>()
            .With.InnerException.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.QueryCanceled));
    }

    [Test]
    public void ScramPlus()
    {
        try
        {
            using var dataSource = CreateDataSource(csb =>
            {
                csb.SslMode = SslMode.Require;
                csb.Username = "npgsql_tests_scram";
                csb.Password = "npgsql_tests_scram";
            });
            using var conn = dataSource.OpenConnection();
            // scram-sha-256-plus only works beginning from PostgreSQL 11
            if (conn.PostgreSqlVersion.Major >= 11)
            {
                Assert.That(conn.IsScram, Is.False);
                Assert.That(conn.IsScramPlus, Is.True);
            }
            else
            {
                Assert.That(conn.IsScram, Is.True);
                Assert.That(conn.IsScramPlus, Is.False);
            }
        }
        catch (Exception e) when (!IsOnBuildServer)
        {
            Console.WriteLine(e);
            Assert.Ignore("scram-sha-256-plus doesn't seem to be set up");
        }
    }

    [Test]
    public void ScramPlus_channel_binding([Values] ChannelBinding channelBinding)
    {
        try
        {
            using var dataSource = CreateDataSource(csb =>
            {
                csb.SslMode = SslMode.Require;
                csb.Username = "npgsql_tests_scram";
                csb.Password = "npgsql_tests_scram";
                csb.ChannelBinding = channelBinding;
            });
            // scram-sha-256-plus only works beginning from PostgreSQL 11
            MinimumPgVersion(dataSource, "11.0");
            using var conn = dataSource.OpenConnection();

            if (channelBinding == ChannelBinding.Disable)
            {
                Assert.That(conn.IsScram, Is.True);
                Assert.That(conn.IsScramPlus, Is.False);
            }
            else
            {
                Assert.That(conn.IsScram, Is.False);
                Assert.That(conn.IsScramPlus, Is.True);
            }
        }
        catch (Exception e) when (!IsOnBuildServer)
        {
            Console.WriteLine(e);
            Assert.Ignore("scram-sha-256-plus doesn't seem to be set up");
        }
    }

    [Test]
    public async Task Connect_with_only_ssl_allowed_user([Values] bool multiplexing, [Values] bool keepAlive)
    {
        if (multiplexing && keepAlive)
        {
            Assert.Ignore("Multiplexing doesn't support keepalive");
        }

        try
        {
            await using var dataSource = CreateDataSource(csb =>
            {
                csb.SslMode = SslMode.Allow;
                csb.Username = "npgsql_tests_ssl";
                csb.Password = "npgsql_tests_ssl";
                csb.Multiplexing = multiplexing;
                csb.KeepAlive = keepAlive ? 10 : 0;
            });
            await using var conn = await dataSource.OpenConnectionAsync();
            Assert.IsTrue(conn.IsSecure);
        }
        catch (Exception e) when (!IsOnBuildServer)
        {
            Console.WriteLine(e);
            Assert.Ignore("Only ssl user doesn't seem to be set up");
        }
    }

    [Test]
    [Platform(Exclude = "Win", Reason = "Postgresql doesn't close connection correctly on windows which might result in missing error message")]
    public async Task Connect_with_only_non_ssl_allowed_user([Values] bool multiplexing, [Values] bool keepAlive)
    {
        if (multiplexing && keepAlive)
        {
            Assert.Ignore("Multiplexing doesn't support keepalive");
        }

        try
        {
            await using var dataSource = CreateDataSource(csb =>
            {
                csb.SslMode = SslMode.Prefer;
                csb.Username = "npgsql_tests_nossl";
                csb.Password = "npgsql_tests_nossl";
                csb.Multiplexing = multiplexing;
                csb.KeepAlive = keepAlive ? 10 : 0;
            });
            await using var conn = await dataSource.OpenConnectionAsync();
            Assert.IsFalse(conn.IsSecure);
        }
        catch (NpgsqlException ex) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && ex.InnerException is IOException)
        {
            // Windows server to windows client invites races that can cause the socket to be reset before all data can be read.
            // https://www.postgresql.org/message-id/flat/90b34057-4176-7bb0-0dbb-9822a5f6425b%40greiz-reinsdorf.de
            // https://www.postgresql.org/message-id/flat/16678-253e48d34dc0c376@postgresql.org
            Assert.Ignore();
        }
        catch (Exception e) when (!IsOnBuildServer)
        {
            Console.WriteLine(e);
            Assert.Ignore("Only nonssl user doesn't seem to be set up");
        }
    }

    [Test]
    public async Task DataSource_SslClientAuthenticationOptionsCallback_is_invoked([Values] bool acceptCertificate)
    {
        var callbackWasInvoked = false;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.SslMode = SslMode.Require;
        dataSourceBuilder.UseSslClientAuthenticationOptionsCallback(options =>
        {
            options.RemoteCertificateValidationCallback = (_, _, _, _) =>
            {
                callbackWasInvoked = true;
                return acceptCertificate;
            };
        });
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = dataSource.CreateConnection();

        if (acceptCertificate)
            Assert.DoesNotThrowAsync(async () => await connection.OpenAsync());
        else
        {
            var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await connection.OpenAsync())!;
            Assert.That(ex.InnerException, Is.TypeOf<AuthenticationException>());
        }

        Assert.IsTrue(callbackWasInvoked);
    }

    [Test]
    public async Task Connection_SslClientAuthenticationOptionsCallback_is_invoked([Values] bool acceptCertificate)
    {
        var callbackWasInvoked = false;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConnectionStringBuilder.SslMode = SslMode.Require;
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = dataSource.CreateConnection();
        connection.SslClientAuthenticationOptionsCallback = options =>
        {
            options.RemoteCertificateValidationCallback = (_, _, _, _) =>
            {
                callbackWasInvoked = true;
                return acceptCertificate;
            };
        };

        if (acceptCertificate)
            Assert.DoesNotThrowAsync(async () => await connection.OpenAsync());
        else
        {
            var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await connection.OpenAsync())!;
            Assert.That(ex.InnerException, Is.TypeOf<AuthenticationException>());
        }

        Assert.IsTrue(callbackWasInvoked);
    }

    [Test]
    public void Connect_with_Verify_and_callback_throws([Values(SslMode.VerifyCA, SslMode.VerifyFull)] SslMode sslMode)
    {
        using var dataSource = CreateDataSource(csb => csb.SslMode = sslMode);
        using var connection = dataSource.CreateConnection();
        connection.SslClientAuthenticationOptionsCallback = options =>
        {
            options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await connection.OpenAsync())!;
        Assert.That(ex.Message, Is.EqualTo(string.Format(NpgsqlStrings.CannotUseSslVerifyWithCustomValidationCallback, sslMode)));
    }

    [Test]
    public void Connect_with_RootCertificate_and_callback_throws()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
            csb.RootCertificate = "foo";
        });
        using var connection = dataSource.CreateConnection();
        connection.SslClientAuthenticationOptionsCallback = options =>
        {
            options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await connection.OpenAsync())!;
        Assert.That(ex.Message, Is.EqualTo(string.Format(NpgsqlStrings.CannotUseSslRootCertificateWithCustomValidationCallback)));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4305")]
    public async Task Bug4305_Secure([Values] bool async)
    {
        await using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
            csb.Username = "npgsql_tests_ssl";
            csb.Password = "npgsql_tests_ssl";
            csb.MaxPoolSize = 1;
        });

        NpgsqlConnection conn = default!;

        try
        {
            conn = await dataSource.OpenConnectionAsync();
            Assert.IsTrue(conn.IsSecure);
        }
        catch (Exception e) when (!IsOnBuildServer)
        {
            Console.WriteLine(e);
            Assert.Ignore("Only ssl user doesn't seem to be set up");
        }

        await using var __ = conn;
        await using var cmd = conn.CreateCommand();
        await using (var tx = await conn.BeginTransactionAsync())
        {
            var originalConnector = conn.Connector;

            cmd.CommandText = "select pg_sleep(30)";
            cmd.CommandTimeout = 3;
            var ex = async
                ? Assert.ThrowsAsync<NpgsqlException>(() => cmd.ExecuteNonQueryAsync())!
                : Assert.Throws<NpgsqlException>(() => cmd.ExecuteNonQuery())!;
            Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());

            await conn.CloseAsync();
            await conn.OpenAsync();

            Assert.AreSame(originalConnector, conn.Connector);
        }

        cmd.CommandText = "SELECT 1";
        if (async)
            Assert.DoesNotThrowAsync(async () => await cmd.ExecuteNonQueryAsync());
        else
            Assert.DoesNotThrow(() => cmd.ExecuteNonQuery());
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4305")]
    public async Task Bug4305_not_Secure([Values] bool async)
    {
        await using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Disable;
            csb.Username = "npgsql_tests_nossl";
            csb.Password = "npgsql_tests_nossl";
            csb.MaxPoolSize = 1;
        });

        NpgsqlConnection conn = default!;

        try
        {
            conn = await dataSource.OpenConnectionAsync();
            Assert.IsFalse(conn.IsSecure);
        }
        catch (Exception e) when (!IsOnBuildServer)
        {
            Console.WriteLine(e);
            Assert.Ignore("Only nossl user doesn't seem to be set up");
        }

        await using var __ = conn;
        var originalConnector = conn.Connector;

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "select pg_sleep(30)";
        cmd.CommandTimeout = 3;
        var ex = async
            ? Assert.ThrowsAsync<NpgsqlException>(() => cmd.ExecuteNonQueryAsync())!
            : Assert.Throws<NpgsqlException>(() => cmd.ExecuteNonQuery())!;
        Assert.That(ex.InnerException, Is.TypeOf<TimeoutException>());

        await conn.CloseAsync();
        await conn.OpenAsync();

        Assert.AreSame(originalConnector, conn.Connector);

        cmd.CommandText = "SELECT 1";
        if (async)
            Assert.DoesNotThrowAsync(async () => await cmd.ExecuteNonQueryAsync());
        else
            Assert.DoesNotThrow(() => cmd.ExecuteNonQuery());
    }

    [Test]
    public async Task Direct_ssl_negotiation()
    {
        await using var adminConn = await OpenConnectionAsync();
        MinimumPgVersion(adminConn, "17.0");

        await using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = SslMode.Require;
            csb.SslNegotiation = SslNegotiation.Direct;
        });
        await using var conn = await dataSource.OpenConnectionAsync();
        Assert.IsTrue(conn.IsSecure);
    }

    [Test]
    public void Direct_ssl_requires_correct_sslmode([Values] SslMode sslMode)
    {
        if (sslMode is SslMode.Disable or SslMode.Allow or SslMode.Prefer)
        {
            var ex = Assert.Throws<ArgumentException>(() =>
            {
                using var dataSource = CreateDataSource(csb =>
                {
                    csb.SslMode = sslMode;
                    csb.SslNegotiation = SslNegotiation.Direct;
                });
            })!;
            Assert.That(ex.Message, Is.EqualTo("SSL Mode has to be Require or higher to be used with direct SSL Negotiation"));
        }
        else
        {
            using var dataSource = CreateDataSource(csb =>
            {
                csb.SslMode = sslMode;
                csb.SslNegotiation = SslNegotiation.Direct;
            });
        }
    }

    [Test]
    [Platform(Exclude = "MacOsX", Reason = "Mac requires explicit opt-in to receive CA certificate in TLS handshake")]
    public async Task Connect_with_verify_and_ca_cert([Values(SslMode.VerifyCA, SslMode.VerifyFull)] SslMode sslMode)
    {
        if (!IsOnBuildServer)
            Assert.Ignore("Only executed in CI");

        await using var dataSource = CreateDataSource(csb =>
        {
            csb.SslMode = sslMode;
            csb.RootCertificate = "ca.crt";
        });

        await using var _ = await dataSource.OpenConnectionAsync();
    }

    [Test]
    [Platform(Exclude = "MacOsX", Reason = "Mac requires explicit opt-in to receive CA certificate in TLS handshake")]
    public async Task Connect_with_verify_check_host([Values(SslMode.VerifyCA, SslMode.VerifyFull)] SslMode sslMode)
    {
        if (!IsOnBuildServer)
            Assert.Ignore("Only executed in CI");

        await using var dataSource = CreateDataSource(csb =>
        {
            csb.Host = "127.0.0.1";
            csb.SslMode = sslMode;
            csb.RootCertificate = "ca.crt";
        });

        if (sslMode == SslMode.VerifyCA)
        {
            await using var _ = await dataSource.OpenConnectionAsync();
        }
        else
        {
            var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await dataSource.OpenConnectionAsync())!;
            Assert.That(ex.InnerException, Is.TypeOf<AuthenticationException>());
        }
    }

    [Test]
    [NonParallelizable] // Sets environment variable
    public async Task Direct_ssl_via_env_requires_correct_sslmode()
    {
        await using var adminConn = await OpenConnectionAsync();
        MinimumPgVersion(adminConn, "17.0");

        // NonParallelizable attribute doesn't work with parameters that well
        foreach (var sslMode in new[] { SslMode.Disable, SslMode.Allow, SslMode.Prefer, SslMode.Require })
        {
            using var _ = SetEnvironmentVariable("PGSSLNEGOTIATION", nameof(SslNegotiation.Direct));
            await using var dataSource = CreateDataSource(csb =>
            {
                csb.SslMode = sslMode;
            });
            if (sslMode is SslMode.Disable or SslMode.Allow or SslMode.Prefer)
            {
                var ex = Assert.ThrowsAsync<ArgumentException>(async () => await dataSource.OpenConnectionAsync())!;
                Assert.That(ex.Message, Is.EqualTo("SSL Mode has to be Require or higher to be used with direct SSL Negotiation"));
            }
            else
            {
                await using var conn = await dataSource.OpenConnectionAsync();
            }
        }
    }

    #region Setup / Teardown / Utils

    [OneTimeSetUp]
    public void CheckSslSupport()
    {
        using var conn = OpenConnection();
        var sslSupport = (string)conn.ExecuteScalar("SHOW ssl")!;
        if (sslSupport == "off")
            IgnoreExceptOnBuildServer("SSL support isn't enabled at the backend");
    }

    #endregion
}
