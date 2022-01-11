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
class NetworkTypeTests : MultiplexingTestBase
{
    [Test]
    public Task Inet_v4_as_IPAddress()
        => AssertType(IPAddress.Parse("192.168.1.1"), "192.168.1.1/32", "inet", NpgsqlDbType.Inet);

    [Test]
    public Task Inet_v6_as_IPAddress()
        => AssertType(
            IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"),
            "2001:1db8:85a3:1142:1000:8a2e:1370:7334/128",
            "inet",
            NpgsqlDbType.Inet);

    [Test]
    public Task Inet_v4_as_tuple()
        => AssertType((IPAddress.Parse("192.168.1.1"), 24), "192.168.1.1/24", "inet", NpgsqlDbType.Inet, isDefaultForReading: false);

    [Test]
    public Task Inet_v6_as_tuple()
        => AssertType(
            (IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"), 24),
            "2001:1db8:85a3:1142:1000:8a2e:1370:7334/24",
            "inet",
            NpgsqlDbType.Inet,
            isDefaultForReading: false);

    [Test, IssueLink("https://github.com/dotnet/corefx/issues/33373")]
    public Task IPAddress_Any()
        => AssertTypeWrite(IPAddress.Any, "0.0.0.0/32", "inet", NpgsqlDbType.Inet);

    [Test]
    public Task Cidr()
        => AssertType(
            (Address: IPAddress.Parse("192.168.1.0"), Subnet: 24),
            "192.168.1.0/24",
            "cidr",
            NpgsqlDbType.Cidr,
            isDefaultForWriting: false);

#pragma warning disable 618  // For NpgsqlInet
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
#pragma warning restore 618  // For NpgsqlInet

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

        var exception = await AssertTypeUnsupportedWrite<PhysicalAddress, PostgresException>(
            PhysicalAddress.Parse("08-00-2B-01-02-03-04-05"), "macaddr");

        Assert.That(exception.Message, Does.StartWith("22P03:").And.Contain("1"));
    }

    public NetworkTypeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}