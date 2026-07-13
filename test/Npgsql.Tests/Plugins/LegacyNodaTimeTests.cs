using System;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.NodaTime.Internal;
using NUnit.Framework;

namespace Npgsql.Tests.Plugins;

[NonParallelizable] // Since this test suite manipulates an AppContext switch
public class LegacyNodaTimeTests : TestBase
{
    const string TimeZone = "Europe/Berlin";

    [Test]
    public async Task Timestamp_as_ZonedDateTime()
        => await AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InZoneLeniently(DateTimeZoneProviders.Tzdb[TimeZone]),
            "1998-04-12 13:26:38.789+02",
            "timestamp with time zone", dataTypeInference: DataTypeInference.Nothing,
            dbType: new(DbType.DateTimeOffset, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Timestamp_as_Instant()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone", dataTypeInference: DataTypeInference.Nothing,
            dbType: new(DbType.DateTime, DbType.Object));

    [Test]
    public Task Timestamp_as_LocalDateTime()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone", dataTypeInference: DataTypeInference.Nothing,
            dbType: new(DbType.DateTime, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Timestamptz_as_Instant()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
            "1998-04-12 15:26:38.789+02",
            "timestamp with time zone", dataTypeInference: DataTypeInference.Nothing,
            dbType: new(DbType.DateTimeOffset, DbType.Object));

    [Test]
    public async Task Timestamptz_ZonedDateTime_infinite_values_are_not_supported()
    {
        await AssertTypeUnsupportedRead<OffsetDateTime, InvalidCastException>("infinity", "timestamptz");
        await AssertTypeUnsupportedWrite<OffsetDateTime, ArgumentException>(Instant.MaxValue.WithOffset(Offset.Zero), "timestamptz");
    }

    [Test]
    public async Task Timestamptz_OffsetDateTime_infinite_values_are_not_supported()
    {
        await AssertTypeUnsupportedRead<OffsetDateTime, InvalidCastException>("infinity", "timestamptz");
        await AssertTypeUnsupportedWrite<OffsetDateTime, ArgumentException>(Instant.MaxValue.WithOffset(Offset.Zero), "timestamptz");
    }

    #region Support

    NpgsqlDataSource _dataSource = null!;
    protected override NpgsqlDataSource DataSource => _dataSource;

    [OneTimeSetUp]
    public void Setup()
    {
#if DEBUG
        NodaTimeUtils.LegacyTimestampBehavior = true;
        Util.Statics.LegacyTimestampBehavior = true;

        var builder = CreateDataSourceBuilder();
        builder.UseNodaTime();
        builder.ConnectionStringBuilder.Timezone = TimeZone;
        _dataSource = builder.Build();
        NpgsqlDataSourceBuilder.ResetGlobalMappings(overwrite: true);
#else
        Assert.Ignore(
            "Legacy NodaTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif
    }

#if DEBUG
    [OneTimeTearDown]
    public void Teardown()
    {
        NodaTimeUtils.LegacyTimestampBehavior = false;
        Util.Statics.LegacyTimestampBehavior = false;

        _dataSource.Dispose();
        NpgsqlDataSourceBuilder.ResetGlobalMappings(overwrite: true);
    }
#endif

    #endregion Support
}
