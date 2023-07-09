using System;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;
using Npgsql.NodaTime.Internal;

namespace Npgsql.PluginTests;

[NonParallelizable] // Since this test suite manipulates an AppContext switch
public class LegacyNodaTimeTests : TestBase, IDisposable
{
    const string TimeZone = "Europe/Berlin";

    [Test]
    public async Task Timestamp_as_ZonedDateTime()
    {
        await AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InZoneLeniently(DateTimeZoneProviders.Tzdb[TimeZone]),
            "1998-04-12 13:26:38.789+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTimeOffset,
            isNpgsqlDbTypeInferredFromClrType: false, isDefault: false);
    }

    [Test]
    public Task Timestamp_as_Instant()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone",
            NpgsqlDbType.Timestamp,
            DbType.DateTime,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Timestamp_as_LocalDateTime()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone",
            NpgsqlDbType.Timestamp,
            DbType.DateTime,
            isDefaultForReading: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Timestamptz_as_Instant()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
            "1998-04-12 15:26:38.789+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTimeOffset,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Timestamptz_ZonedDateTime_infinite_values_are_not_supported()
        => AssertTypeUnsupported(Instant.MaxValue.InZone(DateTimeZone.Utc), "infinity", "timestamptz");

    [Test]
    public Task Timestamptz_OffsetDateTime_infinite_values_are_not_supported()
        => AssertTypeUnsupported(Instant.MaxValue.WithOffset(Offset.Zero), "infinity", "timestamptz");

    #region Support

    protected override NpgsqlDataSource DataSource { get; }

    public LegacyNodaTimeTests()
    {
#if DEBUG
        NodaTimeUtils.LegacyTimestampBehavior = true;
        Util.Statics.LegacyTimestampBehavior = true;

        var builder = CreateDataSourceBuilder();
        builder.UseNodaTime();
        builder.ConnectionStringBuilder.Timezone = TimeZone;
        DataSource = builder.Build();
#else
        Assert.Ignore(
            "Legacy NodaTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif
    }

    public void Dispose()
    {
#if DEBUG
        NodaTimeUtils.LegacyTimestampBehavior = false;
        Util.Statics.LegacyTimestampBehavior = false;

        DataSource.Dispose();
#endif
    }

    #endregion Support
}
