using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web.UI.WebControls;
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

        #region Partial Trust

        [Test, Description("Makes sure Npgsql works when running under pseudo-medium trust")]
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

        #endregion

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
