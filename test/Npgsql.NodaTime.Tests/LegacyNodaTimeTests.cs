using System;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.NodaTime.Tests;

// Since this test suite manipulates TimeZone, it is incompatible with multiplexing
[NonParallelizable]
public class LegacyNodaTimeTests : TestBase
{
    [Test]
    public Task Timestamp_as_Instant()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone",
            NpgsqlDbType.Timestamp,
            DbType.DateTime);

    [Test]
    public Task Timestamp_as_LocalDateTime()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789),
            "1998-04-12 13:26:38.789",
            "timestamp without time zone",
            NpgsqlDbType.Timestamp,
            DbType.DateTime,
            isDefaultForReading: false);

    [Test]
    public Task Timestamptz_as_Instant()
        => AssertType(
            new LocalDateTime(1998, 4, 12, 13, 26, 38, 789).InUtc().ToInstant(),
            "1998-04-12 15:26:38.789+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTimeOffset,
            isDefault: false);

    [Test]
    public Task Timestamptz_ZonedDateTime_infinite_values_are_not_supported()
        => AssertTypeUnsupported(Instant.MaxValue.InZone(DateTimeZone.Utc), "infinity", "timestamptz");

    [Test]
    public Task Timestamptz_OffsetDateTime_infinite_values_are_not_supported()
        => AssertTypeUnsupported(Instant.MaxValue.WithOffset(Offset.Zero), "infinity", "timestamptz");

    #region Support

    protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
    {
        var conn = new NpgsqlConnection(connectionString ?? ConnectionString);
        await conn.OpenAsync();
        await conn.ExecuteNonQueryAsync("SET TimeZone='Europe/Berlin'");
        return conn;
    }

    protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        => throw new NotSupportedException();

#pragma warning disable CS1998 // Release code blocks below lack await
#pragma warning disable CS0618 // GlobalTypeMapper is obsolete
    [OneTimeSetUp]
    public async Task Setup()
    {
#if DEBUG
        Internal.NodaTimeUtils.LegacyTimestampBehavior = true;
        Util.Statics.LegacyTimestampBehavior = true;

        // Clear any previous cached mappings/handlers in case tests were executed before the legacy flag was set.
        NpgsqlConnection.GlobalTypeMapper.Reset();
        NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
        await using var connection = await OpenConnectionAsync();
        await connection.ReloadTypesAsync();
#else
        Assert.Ignore(
            "Legacy NodaTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif

    }

    [OneTimeTearDown]
    public async Task Teardown()
    {
#if DEBUG
        Internal.NodaTimeUtils.LegacyTimestampBehavior = false;
        Util.Statics.LegacyTimestampBehavior = false;

        // Clear any previous cached mappings/handlers to not affect test which will run later without the legacy flag
        NpgsqlConnection.GlobalTypeMapper.Reset();
        NpgsqlConnection.GlobalTypeMapper.UseNodaTime();

        await using var connection = await OpenConnectionAsync();
        await connection.ReloadTypesAsync();
#endif
    }
#pragma warning restore CS1998
#pragma warning restore CS0618 // GlobalTypeMapper is obsolete

    #endregion Support
}
