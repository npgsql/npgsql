using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class SecurityTests : TestBase
    {
        public SecurityTests(string backendVersion) : base(backendVersion) {}

        [Test, Description("Establishes an SSL connection")]
        public void BasicSsl()
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";SSL=true;SslMode=Require"))
            {
                conn.Open();
                Assert.That(conn.IsSecure, Is.True);
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
                TestUtil.Inconclusive("SSL support isn't enabled at the backend");
        }

        #endregion
    }
}
