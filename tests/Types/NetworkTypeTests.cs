using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL numeric types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    class NetworkTypeTests : TestBase
    {
        [Test]
        public void ReadInetV4([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var expectedIp = IPAddress.Parse("192.168.1.1");
            var expectedInet = new NpgsqlInet(expectedIp, 24);
            var cmd = new NpgsqlCommand("SELECT '192.168.1.1/24'::INET", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Regular type (IPAddress)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));
            Assert.That(reader.GetFieldValue<IPAddress>(0), Is.EqualTo(expectedIp));
            Assert.That(reader[0], Is.EqualTo(expectedIp));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedIp));
            Assert.That(reader.GetString(0), Is.EqualTo("192.168.1.1/24"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));

            // Provider-specific type (NpgsqlInet)
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetFieldValue<NpgsqlInet>(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetString(0), Is.EqualTo(expectedInet.ToString()));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadInetV6([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            const string addr = "2001:1db8:85a3:1142:1000:8a2e:1370:7334";
            var expectedIp = IPAddress.Parse(addr);
            var expectedInet = new NpgsqlInet(expectedIp, 24);
            var cmd = new NpgsqlCommand(String.Format("SELECT '{0}/24'::INET", addr), Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Regular type (IPAddress)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));
            Assert.That(reader.GetFieldValue<IPAddress>(0), Is.EqualTo(expectedIp));
            Assert.That(reader[0], Is.EqualTo(expectedIp));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedIp));
            Assert.That(reader.GetString(0), Is.EqualTo(addr + "/24"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));

            // Provider-specific type (NpgsqlInet)
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetFieldValue<NpgsqlInet>(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetString(0), Is.EqualTo(expectedInet.ToString()));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadCidr([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var expectedIp = IPAddress.Parse("192.168.1.0");
            var expectedInet = new NpgsqlInet(expectedIp, 24);
            var cmd = new NpgsqlCommand("SELECT '192.168.1.0/24'::CIDR", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Regular type (IPAddress)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));
            Assert.That(reader.GetFieldValue<IPAddress>(0), Is.EqualTo(expectedIp));
            Assert.That(reader[0], Is.EqualTo(expectedIp));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedIp));
            Assert.That(reader.GetString(0), Is.EqualTo("192.168.1.0/24"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));

            // Provider-specific type (NpgsqlInet)
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetFieldValue<NpgsqlInet>(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetString(0), Is.EqualTo(expectedInet.ToString()));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadMacaddr([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var expected = PhysicalAddress.Parse("08-00-2B-01-02-03");
            var cmd = new NpgsqlCommand("SELECT '08-00-2b-01-02-03'::MACADDR", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldValue<PhysicalAddress>(0), Is.EqualTo(expected));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetString(0), Is.EqualTo(expected.ToString()));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(PhysicalAddress)));

            reader.Dispose();
            cmd.Dispose();
        }

        public NetworkTypeTests(string backendVersion) : base(backendVersion) {}
    }
}
