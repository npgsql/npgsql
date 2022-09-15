using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Util.Statics;

namespace Npgsql.Tests.Types;

// Since this test suite manipulates TimeZone, it is incompatible with multiplexing
[NonParallelizable]
public class LegacyDateTimeTests : TestBase
{
    [Test]
    public Task Timestamp_with_all_DateTime_kinds([Values] DateTimeKind kind)
        => AssertType(
            new DateTime(1998, 4, 12, 13, 26, 38, 789, kind),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone",
            NpgsqlDbType.Timestamp,
            DbType.DateTime);

    [Test]
    [TestCase(DateTimeKind.Utc, TestName = "Timestamptz_write_utc_DateTime_does_not_convert")]
    [TestCase(DateTimeKind.Unspecified, TestName = "Timestamptz_write_unspecified_DateTime_does_not_convert")]
    public Task Timestamptz_write_utc_DateTime_does_not_convert(DateTimeKind kind)
        => AssertTypeWrite(
            new DateTime(1998, 4, 12, 13, 26, 38, 789, kind),
            "1998-04-12 15:26:38.789+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTimeOffset,
            isDefault: false);

    [Test]
    public Task Timestamptz_local_DateTime_converts()
    {
        // In legacy mode, we convert local DateTime to UTC when writing, and convert to local when reading,
        // using the machine time zone.
        var dateTime = new DateTime(1998, 4, 12, 13, 26, 38, 789, DateTimeKind.Utc).ToLocalTime();

        return AssertType(
            dateTime,
            "1998-04-12 15:26:38.789+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTimeOffset,
            isDefaultForWriting: false);
    }

    protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
    {
        var conn = await base.OpenConnectionAsync(connectionString);
        await conn.ExecuteNonQueryAsync("SET TimeZone='Europe/Berlin'");
        return conn;
    }

    protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        => throw new NotSupportedException();

    [OneTimeSetUp]
    public void Setup()
    {
#if DEBUG
        LegacyTimestampBehavior = true;
#else
        Assert.Ignore(
            "Legacy DateTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif
    }

#if DEBUG
    [OneTimeTearDown]
    public void Teardown() => LegacyTimestampBehavior = false;
#endif
}
