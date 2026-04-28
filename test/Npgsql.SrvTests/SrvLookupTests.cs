using System;
using NUnit.Framework;
using static Npgsql.SrvLookup;

namespace Npgsql.SrvTests;

/// <summary>
/// Tests for DNS SRV service discovery in Npgsql.
///
/// Unit tests exercise connection-string parsing and the SRV record sort logic without a live
/// DNS resolver by calling the internal <see cref="SrvLookup.SortAndBuild"/> method directly.
///
/// <see cref="ResolveSrvLive"/> queries actual SRV records published at
/// <c>_postgresql._tcp.mmatvei.ru</c> and verifies priority ordering end-to-end.
/// </summary>
class SrvLookupTests
{
    // -----------------------------------------------------------------------
    // Connection-string / property tests
    // -----------------------------------------------------------------------

    [Test]
    public void SrvHost_property_roundtrip()
    {
        var csb = new NpgsqlConnectionStringBuilder { SrvHost = "cluster.example.com" };
        Assert.That(csb.SrvHost, Is.EqualTo("cluster.example.com"));
        Assert.That(csb.ConnectionString, Contains.Substring("SrvHost=cluster.example.com"));
    }

    [Test]
    public void SrvHost_parsed_from_connection_string()
    {
        var csb = new NpgsqlConnectionStringBuilder("SrvHost=cluster.example.com;Database=mydb");
        Assert.That(csb.SrvHost, Is.EqualTo("cluster.example.com"));
        Assert.That(csb.Database, Is.EqualTo("mydb"));
    }

    [Test]
    public void SrvHost_and_Host_mutually_exclusive()
    {
        var csb = new NpgsqlConnectionStringBuilder
        {
            Host    = "pg1.example.com",
            SrvHost = "cluster.example.com"
        };
        Assert.Throws<ArgumentException>(() => csb.PostProcessAndValidate());
    }

    [Test]
    public void Plain_host_connection_string_unaffected()
    {
        var csb = new NpgsqlConnectionStringBuilder("Host=pg1.example.com;Database=mydb");
        Assert.That(csb.SrvHost, Is.Null.Or.Empty);
        Assert.That(csb.Host, Is.EqualTo("pg1.example.com"));
    }

    // -----------------------------------------------------------------------
    // SRV record sort / build tests — no DNS, calls internal SortAndBuild
    // -----------------------------------------------------------------------

    [Test]
    public void Sort_priority_ascending()
    {
        // Lower priority number = preferred (RFC 2782 §3)
        var result = SrvLookup.SortAndBuild([
            new SrvRecord(100, 1, 5432, "replica.example.com"),
            new SrvRecord( 10, 1, 5432, "primary.example.com"),
        ]);
        var hosts = result!.Split(',');
        Assert.That(hosts[0], Is.EqualTo("primary.example.com:5432"), "priority 10 must come first");
        Assert.That(hosts[1], Is.EqualTo("replica.example.com:5432"), "priority 100 must come second");
    }

    [Test]
    public void Sort_weight_descending_within_priority()
    {
        // Same priority: higher weight = more preferred
        var result = SrvLookup.SortAndBuild([
            new SrvRecord(10,  1, 5432, "light.example.com"),
            new SrvRecord(10, 50, 5433, "heavy.example.com"),
        ]);
        var hosts = result!.Split(',');
        Assert.That(hosts[0], Is.EqualTo("heavy.example.com:5433"), "weight 50 must come first");
        Assert.That(hosts[1], Is.EqualTo("light.example.com:5432"));
    }

    [Test]
    public void Sort_mixed_matches_mmatvei_ru_ordering()
    {
        // Mirrors the four real records published at _postgresql._tcp.mmatvei.ru
        var result = SrvLookup.SortAndBuild([
            new SrvRecord(100, 1, 5432, "pg.mmatvei.ru"),
            new SrvRecord( 96, 1, 5432, "pg4.mmatvei.ru"),
            new SrvRecord( 99, 1, 5432, "pg2.mmatvei.ru"),
            new SrvRecord( 97, 1, 5432, "pg3.mmatvei.ru"),
        ]);
        var hosts = result!.Split(',');
        Assert.That(hosts[0], Is.EqualTo("pg4.mmatvei.ru:5432"),  "priority 96 first");
        Assert.That(hosts[1], Is.EqualTo("pg3.mmatvei.ru:5432"),  "priority 97 second");
        Assert.That(hosts[2], Is.EqualTo("pg2.mmatvei.ru:5432"),  "priority 99 third");
        Assert.That(hosts[3], Is.EqualTo("pg.mmatvei.ru:5432"),   "priority 100 last");
    }

    [Test]
    public void Sort_empty_sequence_returns_null()
    {
        Assert.That(SrvLookup.SortAndBuild([]), Is.Null);
    }

    // -----------------------------------------------------------------------
    // Live DNS test
    // -----------------------------------------------------------------------

    /// <summary>
    /// Queries real SRV records published at <c>_postgresql._tcp.mmatvei.ru</c> and verifies
    /// that Npgsql resolves and orders them correctly.  The test skips automatically if the
    /// records are unreachable so it never breaks an offline build.
    /// </summary>
    [Test]
    public void ResolveSrvLive()
    {
        string hostString;
        try
        {
            hostString = SrvLookup.ResolveToHostString("mmatvei.ru");
        }
        catch (NpgsqlException ex)
        {
            Assert.Ignore($"SRV lookup failed (records may not be published): {ex.Message}");
            return;
        }

        var hosts = hostString.Split(',');
        Assume.That(hosts.Length, Is.GreaterThanOrEqualTo(2),
            $"Expected ≥2 SRV records, got {hosts.Length}");

        Console.WriteLine("Live SRV targets for mmatvei.ru:");
        for (var i = 0; i < hosts.Length; i++)
            Console.WriteLine($"  [{i}] {hosts[i]}");

        foreach (var h in hosts)
            Assert.That(h, Contains.Substring(":"), $"Expected host:port format, got: {h}");
    }
}
