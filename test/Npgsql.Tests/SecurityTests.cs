﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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
                TrustServerCertificate = true
            };

            using (var conn = OpenConnection(csb))
                Assert.That(conn.IsSecure, Is.True);
        }

        [Test, Description("Makes sure a certificate whose root CA isn't known isn't accepted")]
        public void RejectSelfSignedCertificate()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require
            }.ToString();

            using (var conn = new NpgsqlConnection(connString))
            {
                // The following is necessary since a pooled connector may exist from a previous
                // SSL test
                NpgsqlConnection.ClearPool(conn);

                // TODO: Specific exception, align with SslStream
                Assert.That(() => conn.Open(), Throws.Exception);
            }
        }

        [Test, Description("Makes sure that ssl_renegotiation_limit is always 0, renegotiation is buggy")]
        public void NoSslRenegotiation()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            using (var conn = OpenConnection(csb))
            {
                Assert.That(conn.ExecuteScalar("SHOW ssl_renegotiation_limit"), Is.EqualTo("0"));
                conn.ExecuteNonQuery("DISCARD ALL");
                Assert.That(conn.ExecuteScalar("SHOW ssl_renegotiation_limit"), Is.EqualTo("0"));
            }
        }

        [Test, Description("Makes sure that when SSL is disabled IsSecure returns false")]
        public void NonSecure()
        {
            using (var conn = OpenConnection())
                Assert.That(conn.IsSecure, Is.False);
        }

        [Test, Explicit("Needs to be set up (and run with with Kerberos credentials on Linux)")]
        public void IntegratedSecurityWithUsername()
        {
            var username = Environment.GetEnvironmentVariable("USERNAME") ??
                           Environment.GetEnvironmentVariable("USER");
            if (username == null)
                throw new Exception("Could find username");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                IntegratedSecurity = true,
                Username = username,
                Password = null
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
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
            using (var conn = new NpgsqlConnection(connString))
            {
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
            using (var conn = new NpgsqlConnection(connString))
            {
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
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1718")]
        [Timeout(12000)]
        public void Bug1718()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            using (var conn = OpenConnection(csb))
            using (var cmd = CreateSleepCommand(conn, 10000))
            {
                var cts = new CancellationTokenSource(1000).Token;
                Assert.That(async () => await cmd.ExecuteNonQueryAsync(cts), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("57014"));
            }
        }

        #region Setup / Teardown / Utils

        [SetUp]
        public void CheckSslSupport()
        {
            using (var conn = OpenConnection())
            {
                var sslSupport = (string)conn.ExecuteScalar("SHOW ssl");
                if (sslSupport == "off")
                    TestUtil.IgnoreExceptOnBuildServer("SSL support isn't enabled at the backend");
            }
        }

        #endregion
    }
}
