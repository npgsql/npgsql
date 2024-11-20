using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

/// <summary>
/// Tests on PostgreSQL numeric types
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-net-types.html
/// </remarks>
class NetworkTypeTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public Task Inet_v4_as_IPAddress()
        => AssertType(IPAddress.Parse("192.168.1.1"), "192.168.1.1/32", "inet", NpgsqlDbType.Inet, skipArrayCheck: true);

    [Test]
    public Task Inet_v4_array_as_IPAddress_array()
        => AssertType(
            new[]
            {
                IPAddress.Parse("192.168.1.1"),
                IPAddress.Parse("192.168.1.2")
            },
            "{192.168.1.1,192.168.1.2}", "inet[]", NpgsqlDbType.Inet | NpgsqlDbType.Array);

    [Test]
    public Task Inet_v6_as_IPAddress()
        => AssertType(
            IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"),
            "2001:1db8:85a3:1142:1000:8a2e:1370:7334/128",
            "inet",
            NpgsqlDbType.Inet,
            skipArrayCheck: true);

    [Test]
    public Task Inet_v6_array_as_IPAddress_array()
        => AssertType(
            new[]
            {
                IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"),
                IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7335")
            },
            "{2001:1db8:85a3:1142:1000:8a2e:1370:7334,2001:1db8:85a3:1142:1000:8a2e:1370:7335}", "inet[]", NpgsqlDbType.Inet | NpgsqlDbType.Array);

    [Test, IssueLink("https://github.com/dotnet/corefx/issues/33373")]
    public Task IPAddress_Any()
        => AssertTypeWrite(IPAddress.Any, "0.0.0.0/32", "inet", NpgsqlDbType.Inet, skipArrayCheck: true);

    [Test]
    public Task IPNetwork_as_cidr()
        => AssertType(
            new IPNetwork(IPAddress.Parse("192.168.1.0"), 24),
            "192.168.1.0/24",
            "cidr",
            NpgsqlDbType.Cidr);

#pragma warning disable CS0618 // NpgsqlCidr is obsolete
    [Test]
    public Task NpgsqlCidr_as_Cidr()
        => AssertType(
            new NpgsqlCidr(IPAddress.Parse("192.168.1.0"), netmask: 24),
            "192.168.1.0/24",
            "cidr",
            NpgsqlDbType.Cidr,
            isDefaultForReading: false);
#pragma warning restore CS0618

    [Test]
    public Task Inet_v4_as_NpgsqlInet()
        => AssertType(
            new NpgsqlInet(IPAddress.Parse("192.168.1.1"), 24),
            "192.168.1.1/24",
            "inet",
            NpgsqlDbType.Inet,
            isDefaultForReading: false);

    [Test]
    public Task Inet_v6_as_NpgsqlInet()
        => AssertType(
            new NpgsqlInet(IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"), 24),
            "2001:1db8:85a3:1142:1000:8a2e:1370:7334/24",
            "inet",
            NpgsqlDbType.Inet,
            isDefaultForReading: false);

    [Test]
    public Task Macaddr()
        => AssertType(PhysicalAddress.Parse("08-00-2B-01-02-03"), "08:00:2b:01:02:03", "macaddr", NpgsqlDbType.MacAddr);

    [Test]
    public async Task Macaddr8()
    {
        await using var conn = await OpenConnectionAsync();
        if (conn.PostgreSqlVersion < new Version(10, 0))
            Assert.Ignore("macaddr8 only supported on PostgreSQL 10 and above");

        await AssertType(PhysicalAddress.Parse("08-00-2B-01-02-03-04-05"), "08:00:2b:01:02:03:04:05", "macaddr8", NpgsqlDbType.MacAddr8,
            isDefaultForWriting: false);
    }

    [Test]
    public async Task Macaddr8_write_with_6_bytes()
    {
        await using var conn = await OpenConnectionAsync();
        if (conn.PostgreSqlVersion < new Version(10, 0))
            Assert.Ignore("macaddr8 only supported on PostgreSQL 10 and above");

        await AssertTypeWrite(PhysicalAddress.Parse("08-00-2B-01-02-03"), "08:00:2b:ff:fe:01:02:03", "macaddr8", NpgsqlDbType.MacAddr8,
            isDefault: false);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/835")]
    public async Task Macaddr_multiple()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT unnest(ARRAY['08-00-2B-01-02-03'::MACADDR, '08-00-2B-01-02-04'::MACADDR])", conn);
        await using var r = await cmd.ExecuteReaderAsync();
        r.Read();
        var p1 = (PhysicalAddress)r[0];
        r.Read();
        var p2 = (PhysicalAddress)r[0];
        Assert.That(p1, Is.EqualTo(PhysicalAddress.Parse("08-00-2B-01-02-03")));
        Assert.That(p2, Is.EqualTo(PhysicalAddress.Parse("08-00-2B-01-02-04")));
    }

    [Test]
    public async Task Macaddr_write_validation()
    {
        await using var conn = await OpenConnectionAsync();
        if (conn.PostgreSqlVersion < new Version(10, 0))
            Assert.Ignore("macaddr8 only supported on PostgreSQL 10 and above");

        await AssertTypeUnsupportedWrite<PhysicalAddress, ArgumentException>(PhysicalAddress.Parse("08-00-2B-01-02-03-04-05"), "macaddr");
    }
}
