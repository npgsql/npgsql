using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class SecurityTests : TestBase
    {
        public SecurityTests(string backendVersion) : base(backendVersion) {}

        [Test, Description("Establishes an SSL connection, assuming a self-signed server certificate")]
        [TestCase(false, TestName = "TlsClientStream")]
        [TestCase(true,  TestName = "SslStream")]
        public void BasicSsl(bool useSslStream)
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";SslMode=Require;TrustServerCertificate=true;" + (useSslStream ? ";UseSslStream=true" : "")))
            {
                conn.Open();
                Assert.That(conn.IsSecure, Is.True);
            }
        }

        [Test, Description("Makes sure a certificate whose root CA isn't known isn't accepted")]
        [TestCase(false, TestName = "TlsClientStream")]
        [TestCase(true,  TestName = "SslStream")]
        public void RejectSelfSignedCertificate(bool useSslStream)
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";SslMode=Require;" + (useSslStream ? ";UseSslStream=true" : "")))
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
            using (var conn = new NpgsqlConnection(ConnectionString + ";SslMode=Require;TrustServerCertificate=true"))
            {
                conn.Open();
                Assert.That(ExecuteScalar("SHOW ssl_renegotiation_limit", conn), Is.EqualTo("0"));
                ExecuteNonQuery("DISCARD ALL");
                Assert.That(ExecuteScalar("SHOW ssl_renegotiation_limit", conn), Is.EqualTo("0"));
            }
        }

        [Test, Description("Makes sure that when SSL is disabled IsSecure returns false")]
        public void NonSecure()
        {
            Assert.That(Conn.IsSecure, Is.False);
        }

        #region Setup / Teardown / Utils

        [SetUp]
        public void CheckSslSupport()
        {
            var sslSupport = (string) ExecuteScalar("SHOW ssl", Conn);
            if (sslSupport == "off")
                TestUtil.IgnoreExceptOnBuildServer("SSL support isn't enabled at the backend");
        }

        #endregion
    }
}
