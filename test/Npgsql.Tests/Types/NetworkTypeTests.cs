using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

#pragma warning disable 618  // For NpgsqlInet

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL numeric types
    /// </summary>
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    class NetworkTypeTests : MultiplexingTestBase
    {
        [Test]
        public async Task Inet_v4()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6", conn);
            var expectedIp = IPAddress.Parse("192.168.1.1");
            var expectedTuple = (Address: expectedIp, Subnet: 24);
            var expectedNpgsqlInet = new NpgsqlInet(expectedIp, 24);
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Inet) { Value = expectedIp });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = expectedIp });
            cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.Inet) { Value = expectedTuple });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4", Value = expectedTuple });
            cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlDbType.Inet) { Value = expectedNpgsqlInet });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p6", Value = expectedNpgsqlInet });

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            // Address only, no subnet
            for (var i = 0; i < 2; i++)
            {
                // Regular type (IPAddress)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));
                Assert.That(reader.GetFieldValue<IPAddress>(i), Is.EqualTo(expectedIp));
                Assert.That(reader[i], Is.EqualTo(expectedIp));
                Assert.That(reader.GetValue(i), Is.EqualTo(expectedIp));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));

                // Provider-specific type (ValueTuple<IPAddress, int>)
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof((IPAddress, int))));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo((expectedIp, 32)));
                Assert.That(reader.GetFieldValue<NpgsqlInet>(i), Is.EqualTo(new NpgsqlInet(expectedIp)));
            }

            // Address and subnet
            for (var i = 2; i < 6; i++)
            {
                // Regular type (IPAddress)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));
                Assert.That(reader.GetFieldValue<IPAddress>(i), Is.EqualTo(expectedIp));
                Assert.That(reader[i], Is.EqualTo(expectedIp));
                Assert.That(reader.GetValue(i), Is.EqualTo(expectedIp));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));

                // Provider-specific type (NpgsqlInet)
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof((IPAddress, int))));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(expectedTuple));
                Assert.That(reader.GetFieldValue<NpgsqlInet>(i), Is.EqualTo(expectedNpgsqlInet));
            }
        }

        [Test]
        public async Task Inet_v6()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6", conn);
            const string addr = "2001:1db8:85a3:1142:1000:8a2e:1370:7334";
            var expectedIp = IPAddress.Parse(addr);
            var expectedTuple = (Address: expectedIp, Subnet: 24);
            var expectedNpgsqlInet = new NpgsqlInet(expectedIp, 24);
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Inet) { Value = expectedIp });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = expectedIp });
            cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.Inet) { Value = expectedTuple });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4", Value = expectedTuple });
            cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlDbType.Inet) { Value = expectedNpgsqlInet });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p6", Value = expectedNpgsqlInet });

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            // Address only, no subnet
            for (var i = 0; i < 2; i++)
            {
                // Regular type (IPAddress)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));
                Assert.That(reader.GetFieldValue<IPAddress>(i), Is.EqualTo(expectedIp));
                Assert.That(reader[i], Is.EqualTo(expectedIp));
                Assert.That(reader.GetValue(i), Is.EqualTo(expectedIp));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));

                // Provider-specific type (ValueTuple<IPAddress, int>)
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof((IPAddress, int))));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo((expectedIp, 128)));
                Assert.That(reader.GetFieldValue<NpgsqlInet>(i), Is.EqualTo(new NpgsqlInet(expectedIp)));
            }

            // Address and subnet
            for (var i = 2; i < 6; i++)
            {
                // Regular type (IPAddress)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));
                Assert.That(reader.GetFieldValue<IPAddress>(i), Is.EqualTo(expectedIp));
                Assert.That(reader[i], Is.EqualTo(expectedIp));
                Assert.That(reader.GetValue(i), Is.EqualTo(expectedIp));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(IPAddress)));

                // Provider-specific type (NpgsqlInet)
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof((IPAddress, int))));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(expectedTuple));
                Assert.That(reader.GetFieldValue<NpgsqlInet>(i), Is.EqualTo(expectedNpgsqlInet));
            }
        }

        [Test, Description("Tests support for ReadOnlyIPAddress, see https://github.com/dotnet/corefx/issues/33373")]
        public async Task IPAddress_Any()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Inet) { Value = IPAddress.Any });
            cmd.Parameters.Add(new NpgsqlParameter<IPAddress>("p2", NpgsqlDbType.Inet) { TypedValue = IPAddress.Any });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3", Value = IPAddress.Any });
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            for (var i = 0; i < reader.FieldCount; i++)
                Assert.That(reader.GetFieldValue<IPAddress>(i), Is.EqualTo(IPAddress.Any));
        }

        [Test]
        public async Task Cidr()
        {
            var expected = (Address: IPAddress.Parse("192.168.1.0"), Subnet: 24);
            //var expectedInet = new NpgsqlInet("192.168.1.0/24");
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT '192.168.1.0/24'::CIDR", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            // Regular type (IPAddress)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof((IPAddress, int))));
            Assert.That(reader.GetFieldValue<(IPAddress, int)>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<NpgsqlInet>(0), Is.EqualTo(new NpgsqlInet(expected.Address, expected.Subnet)));
            Assert.That(reader[0], Is.EqualTo(expected));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        }

        [Test]
        public async Task Macaddr()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            var expected = PhysicalAddress.Parse("08-00-2B-01-02-03");
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.MacAddr) { Value = expected };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldValue<PhysicalAddress>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(PhysicalAddress)));
            }
        }

        [Test]
        public async Task Macaddr8()
        {
            using var conn = await OpenConnectionAsync();
            if (conn.PostgreSqlVersion < new Version(10, 0))
                Assert.Ignore("macaddr8 only supported on PostgreSQL 10 and above");

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            var send6 = PhysicalAddress.Parse("08-00-2B-01-02-03");
            var expected6 = PhysicalAddress.Parse("08-00-2B-FF-FE-01-02-03"); // 6-byte macaddr8 gets FF and FE inserted in the middle
            var expected8 = PhysicalAddress.Parse("08-00-2B-01-02-03-04-05");
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.MacAddr8) { Value = send6 });
            cmd.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.MacAddr8) { Value = expected8 });
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetFieldValue<PhysicalAddress>(0), Is.EqualTo(expected6));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected6));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(PhysicalAddress)));

            Assert.That(reader.GetFieldValue<PhysicalAddress>(1), Is.EqualTo(expected8));
            Assert.That(reader.GetValue(1), Is.EqualTo(expected8));
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(PhysicalAddress)));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/835")]
        public async Task Macaddr_multiple()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT unnest(ARRAY['08-00-2B-01-02-03'::MACADDR, '08-00-2B-01-02-04'::MACADDR])", conn);
            using var r = await cmd.ExecuteReaderAsync();
            r.Read();
            var p1 = (PhysicalAddress)r[0];
            r.Read();
            var p2 = (PhysicalAddress)r[0];
            Assert.That(p1, Is.EqualTo(PhysicalAddress.Parse("08-00-2B-01-02-03")));
            Assert.That(p2, Is.EqualTo(PhysicalAddress.Parse("08-00-2B-01-02-04")));
        }

        [Test]
        public async Task Macaddr_validation()
        {
            using var conn = await OpenConnectionAsync();
            if (conn.PostgreSqlVersion < new Version(10, 0))
                Assert.Ignore("macaddr8 only supported on PostgreSQL 10 and above");

            using var cmd = new NpgsqlCommand("SELECT @p1", conn);
            // 6-byte macaddr8 gets FF and FE inserted in the middle
            var send8 = PhysicalAddress.Parse("08-00-2B-01-02-03-04-05");
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.MacAddr) { Value = send8 });

            var exception = Assert.ThrowsAsync<PostgresException>(() => cmd.ExecuteReaderAsync())!;
            Assert.That(exception.Message, Does.StartWith("22P03:").And.Contain("1"));
        }

        public NetworkTypeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
