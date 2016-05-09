#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class SecurityTests : TestBase
    {
        [Test, Description("Establishes an SSL connection, assuming a self-signed server certificate")]
        [TestCase(false, TestName = "TlsClientStream")]
        [TestCase(true,  TestName = "SslStream")]
        public void BasicSsl(bool useSslStream)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                UseSslStream = useSslStream
            };

            using (var conn = OpenConnection(csb))
                Assert.That(conn.IsSecure, Is.True);
        }

        [Test, Description("Makes sure a certificate whose root CA isn't known isn't accepted")]
        [TestCase(false, TestName = "TlsClientStream")]
        [TestCase(true,  TestName = "SslStream")]
        public void RejectSelfSignedCertificate(bool useSslStream)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SslMode = SslMode.Require,
                UseSslStream = useSslStream
            };

            using (var conn = new NpgsqlConnection(csb))
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

        [Test]
        public void IntegratedSecurity()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) {
                IntegratedSecurity = true,
                Username = null,
                Password = null,
            };
            using (var conn = new NpgsqlConnection(csb))
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

        #region Partial Trust

#if NET46
        [Test, Description("Makes sure Npgsql works when running under pseudo-medium trust")]
        [Ignore("Doesn't work via DNX testing")]
        public void RestrictedTrust()
        {
            var domainSetup = new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory };
            var permissions = new PermissionSet(null);
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            var domain = AppDomain.CreateDomain("Partial Trust AppDomain", null, domainSetup, permissions);

            try
            {
                var test = (TrustTestClass) domain.CreateInstanceAndUnwrap(
                    typeof (TrustTestClass).Assembly.FullName,
                    typeof (TrustTestClass).FullName
                    );
                test.Go(ConnectionString);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        [Serializable]
        public class TrustTestClass
        {
            public void Go(string connString)
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    {
                        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                    }
                }
            }
        }
#endif
        #endregion

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
