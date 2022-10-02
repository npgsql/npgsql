using System;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

// Since this test suite manipulates TimeZone, it is incompatible with multiplexing
public class DateTimeTests : TestBase
{
    #region Date

    [Test]
    public Task Date_as_DateTime()
        => AssertType(new DateTime(2020, 10, 1), "2020-10-01", "date", NpgsqlDbType.Date, DbType.Date, isDefaultForWriting: false);

    // Internal PostgreSQL representation (days since 2020-01-01), for out-of-range values.
    [Test]
    public Task Date_as_int()
        => AssertType(7579, "2020-10-01", "date", NpgsqlDbType.Date, DbType.Date, isDefault: false);

    [Test]
    public Task Daterange_as_NpgsqlRange_of_DateTime()
        => AssertType(
            new NpgsqlRange<DateTime>(new(2002, 3, 4), true, new(2002, 3, 6), false),
            "[2002-03-04,2002-03-06)",
            "daterange",
            NpgsqlDbType.DateRange,
            isDefaultForWriting: false);

    [Test]
    public async Task Datemultirange_as_array_of_NpgsqlRange_of_DateTime()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        await AssertType(
            new[]
            {
                new NpgsqlRange<DateTime>(new(2002, 3, 4), true, new(2002, 3, 6), false),
                new NpgsqlRange<DateTime>(new(2002, 3, 8), true, new(2002, 3, 11), false)
            },
            "{[2002-03-04,2002-03-06),[2002-03-08,2002-03-11)}",
            "datemultirange",
            NpgsqlDbType.DateMultirange,
            isDefaultForWriting: false);
    }

#if NET6_0_OR_GREATER
    [Test]
    public Task Date_as_DateOnly()
        => AssertType(new DateOnly(2020, 10, 1), "2020-10-01", "date", NpgsqlDbType.Date, DbType.Date, isDefaultForReading: false);

    [Test]
    public Task Daterange_as_NpgsqlRange_of_DateOnly()
        => AssertType(
            new NpgsqlRange<DateOnly>(new(2002, 3, 4), true, new(2002, 3, 6), false),
            "[2002-03-04,2002-03-06)",
            "daterange",
            NpgsqlDbType.DateRange,
            isDefaultForReading: false);

    [Test]
    public async Task Datemultirange_as_array_of_NpgsqlRange_of_DateOnly()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        await  AssertType(
            new[]
            {
                new NpgsqlRange<DateOnly>(new(2002, 3, 4), true, new(2002, 3, 6), false),
                new NpgsqlRange<DateOnly>(new(2002, 3, 8), true, new(2002, 3, 11), false)
            },
            "{[2002-03-04,2002-03-06),[2002-03-08,2002-03-11)}",
            "datemultirange",
            NpgsqlDbType.DateMultirange,
            isDefaultForReading: false);
    }
#endif

    #endregion

    #region Time

    [Test]
    public Task Time_as_TimeSpan()
        => AssertType(
            new TimeSpan(0, 10, 45, 34, 500),
            "10:45:34.5",
            "time without time zone",
            NpgsqlDbType.Time,
            DbType.Time,
            isDefaultForWriting: false);

#if NET6_0_OR_GREATER
    [Test]
    public Task Time_as_TimeOnly()
        => AssertType(
            new TimeOnly(10, 45, 34, 500),
            "10:45:34.5",
            "time without time zone",
            NpgsqlDbType.Time,
            DbType.Time,
            isDefaultForReading: false);
#endif

    #endregion

    #region Time with timezone

    [Test]
    public async Task TimeTz_as_DateTimeOffset()
    {
        await AssertTypeRead("13:03:45.51+02",
            "time with time zone", new DateTimeOffset(1, 1, 2, 13, 3, 45, 510, TimeSpan.FromHours(2)));

        await AssertTypeWrite(
            new DateTimeOffset(1, 1, 1, 13, 3, 45, 510, TimeSpan.FromHours(2)),
            "13:03:45.51+02",
            "time with time zone",
            NpgsqlDbType.TimeTz,
            isDefault: false);
    }

    [Test]
    public Task TimeTz_before_utc_zero()
        => AssertTypeRead("01:00:00+02",
            "time with time zone", new DateTimeOffset(1, 1, 2, 1, 0, 0, new TimeSpan(0, 2, 0, 0)));

    #endregion

    #region Timestamp

    static readonly TestCaseData[] TimestampValues =
    {
        new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified), "1998-04-12 13:26:38")
            .SetName("Timestamp_pre2000"),
        new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Unspecified), "2015-01-27 08:45:12.345")
            .SetName("Timestamp_post2000"),
        new TestCaseData(new DateTime(2013, 7, 25, 0, 0, 0, DateTimeKind.Unspecified), "2013-07-25 00:00:00")
            .SetName("Timestamp_date_only")
    };

    [Test, TestCaseSource(nameof(TimestampValues))]
    public Task Timestamp_as_DateTime(DateTime dateTime, string sqlLiteral)
        => AssertType(dateTime, sqlLiteral, "timestamp without time zone", NpgsqlDbType.Timestamp, DbType.DateTime2);

    [Test]
    public Task Timestamp_cannot_write_utc_DateTime()
        => AssertTypeUnsupportedWrite(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc), "timestamp without time zone");

    [Test]
    public Task Timestamp_as_long()
        => AssertType(
            -54297202000000,
            "1998-04-12 13:26:38",
            "timestamp without time zone",
            NpgsqlDbType.Timestamp,
            DbType.DateTime2,
            isDefault: false);

    [Test]
    public Task Timestamp_cannot_use_as_DateTimeOffset()
        => AssertTypeUnsupported(
            new DateTimeOffset(1998, 4, 12, 13, 26, 38, TimeSpan.Zero),
            "1998-04-12 13:26:38",
            "timestamp without time zone");

    [Test]
    public Task Tsrange_as_NpgsqlRange_of_DateTime()
        => AssertType(
            new NpgsqlRange<DateTime>(
                new(1998, 4, 12, 13, 26, 38, DateTimeKind.Local),
                new(1998, 4, 12, 15, 26, 38, DateTimeKind.Local)),
            @"[""1998-04-12 13:26:38"",""1998-04-12 15:26:38""]",
            "tsrange",
            NpgsqlDbType.TimestampRange);

    [Test]
    public async Task Tsmultirange_as_array_of_NpgsqlRange_of_DateTime()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        await AssertType(
            new[]
            {
                new NpgsqlRange<DateTime>(
                    new(1998, 4, 12, 13, 26, 38, DateTimeKind.Local),
                    new(1998, 4, 12, 15, 26, 38, DateTimeKind.Local)),
                new NpgsqlRange<DateTime>(
                    new(1998, 4, 13, 13, 26, 38, DateTimeKind.Local),
                    new(1998, 4, 13, 15, 26, 38, DateTimeKind.Local)),
            },
            @"{[""1998-04-12 13:26:38"",""1998-04-12 15:26:38""],[""1998-04-13 13:26:38"",""1998-04-13 15:26:38""]}",
            "tsmultirange",
            NpgsqlDbType.TimestampMultirange);
    }

    #endregion

    #region Timestamp with timezone

    // Note that the below text representations are local (according to TimeZone, which is set to Europe/Berlin in this test class),
    // because that's how PG does timestamptz *text* representation.
    static readonly TestCaseData[] TimestampTzWriteValues =
    {
        new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc), "1998-04-12 15:26:38+02")
            .SetName("Timestamptz_write_pre2000"),
        new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Utc), "2015-01-27 09:45:12.345+01")
            .SetName("Timestamptz_write_post2000"),
        new TestCaseData(new DateTime(2013, 7, 25, 0, 0, 0, DateTimeKind.Utc), "2013-07-25 02:00:00+02")
            .SetName("Timestamptz_write_date_only")
    };

    [Test, TestCaseSource(nameof(TimestampTzWriteValues))]
    public Task Timestamptz_as_DateTime(DateTime dateTime, string sqlLiteral)
        => AssertType(dateTime, sqlLiteral, "timestamp with time zone", NpgsqlDbType.TimestampTz, DbType.DateTime);

    [Test]
    public async Task Timestamptz_infinity_as_DateTime()
    {
        await AssertType(DateTime.MinValue, "-infinity", "timestamp with time zone", NpgsqlDbType.TimestampTz, DbType.DateTime,
            isDefault: false);
        await AssertType(DateTime.MaxValue, "infinity", "timestamp with time zone", NpgsqlDbType.TimestampTz, DbType.DateTime,
            isDefault: false);
    }

    [Test]
    public async Task Timestamptz_cannot_write_non_utc_DateTime()
    {
        await AssertTypeUnsupportedWrite(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified), "timestamp with time zone");
        await AssertTypeUnsupportedWrite(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local), "timestamp with time zone");
    }

    [Test]
    public async Task Timestamptz_as_DateTimeOffset_utc()
    {
        var dateTimeOffset = await AssertType(
            new DateTimeOffset(1998, 4, 12, 13, 26, 38, TimeSpan.Zero),
            "1998-04-12 15:26:38+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTime,
            isDefaultForReading: false);

        Assert.That(dateTimeOffset.Offset, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public Task Timestamptz_as_DateTimeOffset_utc_with_DbType_DateTimeOffset()
        => AssertTypeWrite(
            new DateTimeOffset(1998, 4, 12, 13, 26, 38, TimeSpan.Zero),
            "1998-04-12 15:26:38+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTimeOffset,
            inferredDbType: DbType.DateTime,
            isDefault: false);

    [Test]
    public Task Timestamptz_cannot_write_non_utc_DateTimeOffset()
        => AssertTypeUnsupportedWrite(new DateTimeOffset(1998, 4, 12, 13, 26, 38, TimeSpan.FromHours(2)));

    [Test]
    public Task Timestamptz_as_long()
        => AssertType(
            -54297202000000,
            "1998-04-12 15:26:38+02",
            "timestamp with time zone",
            NpgsqlDbType.TimestampTz,
            DbType.DateTime,
            isDefault: false);

    [Test]
    public Task Tstzrange_as_NpgsqlRange_of_DateTime()
        => AssertType(
            new NpgsqlRange<DateTime>(
                new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc)),
            @"[""1998-04-12 15:26:38+02"",""1998-04-12 17:26:38+02""]",
            "tstzrange",
            NpgsqlDbType.TimestampTzRange);

    [Test]
    public async Task Tstzmultirange_as_array_of_NpgsqlRange_of_DateTime()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        await AssertType(
            new[]
            {
                new NpgsqlRange<DateTime>(
                    new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                    new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc)),
                new NpgsqlRange<DateTime>(
                    new DateTime(1998, 4, 13, 13, 26, 38, DateTimeKind.Utc),
                    new DateTime(1998, 4, 13, 15, 26, 38, DateTimeKind.Utc)),
            },
            @"{[""1998-04-12 15:26:38+02"",""1998-04-12 17:26:38+02""],[""1998-04-13 15:26:38+02"",""1998-04-13 17:26:38+02""]}",
            "tstzmultirange",
            NpgsqlDbType.TimestampTzMultirange);
    }

    [Test]
    public Task Cannot_mix_DateTime_Kinds_in_array()
        => AssertTypeUnsupportedWrite<DateTime[], Exception>(new[]
        {
            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local),
        });


    [Test]
    public Task Cannot_mix_DateTime_Kinds_in_range()
        => AssertTypeUnsupportedWrite(new NpgsqlRange<DateTime>(
            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local)));

    [Test]
    public async Task Cannot_mix_DateTime_Kinds_in_multirange()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        await AssertTypeUnsupportedWrite(new[]
        {
            new NpgsqlRange<DateTime>(
                new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc)),
            new NpgsqlRange<DateTime>(
                new DateTime(1998, 4, 13, 13, 26, 38, DateTimeKind.Local),
                new DateTime(1998, 4, 13, 15, 26, 38, DateTimeKind.Local)),
        });
    }

    [Test]
    public void NpgsqlParameterDbType_is_value_dependent_datetime_or_datetime2()
    {
        var localtimestamp = new NpgsqlParameter { Value = DateTime.Now };
        var unspecifiedtimestamp = new NpgsqlParameter { Value = new DateTime() };
        Assert.AreEqual(DbType.DateTime2, localtimestamp.DbType);
        Assert.AreEqual(DbType.DateTime2, unspecifiedtimestamp.DbType);

        // We don't support any DateTimeOffset other than offset 0 which maps to timestamptz,
        // we might add an exception for offset == DateTimeOffset.Now.Offset (local offset) mapping to timestamp at some point.
        // var dtotimestamp = new NpgsqlParameter { Value = DateTimeOffset.Now };
        // Assert.AreEqual(DbType.DateTime2, dtotimestamp.DbType);

        var timestamptz = new NpgsqlParameter { Value = DateTime.UtcNow };
        var dtotimestamptz = new NpgsqlParameter { Value = DateTimeOffset.UtcNow };
        Assert.AreEqual(DbType.DateTime, timestamptz.DbType);
        Assert.AreEqual(DbType.DateTime, dtotimestamptz.DbType);
    }

    [Test]
    public void NpgsqlParameterNpgsqlDbType_is_value_dependent_timestamp_or_timestamptz()
    {
        var localtimestamp = new NpgsqlParameter { Value = DateTime.Now };
        var unspecifiedtimestamp = new NpgsqlParameter { Value = new DateTime() };
        Assert.AreEqual(NpgsqlDbType.Timestamp, localtimestamp.NpgsqlDbType);
        Assert.AreEqual(NpgsqlDbType.Timestamp, unspecifiedtimestamp.NpgsqlDbType);

        var timestamptz = new NpgsqlParameter { Value = DateTime.UtcNow };
        var dtotimestamptz = new NpgsqlParameter { Value = DateTimeOffset.UtcNow };
        Assert.AreEqual(NpgsqlDbType.TimestampTz, timestamptz.NpgsqlDbType);
        Assert.AreEqual(NpgsqlDbType.TimestampTz, dtotimestamptz.NpgsqlDbType);
    }

    #endregion

    #region Interval

    static readonly TestCaseData[] IntervalValues =
    {
        new TestCaseData(new TimeSpan(0, 2, 3, 4, 5), "02:03:04.005")
            .SetName("Interval_time_only"),
        new TestCaseData(new TimeSpan(1, 2, 3, 4, 5), "1 day 02:03:04.005")
            .SetName("Interval_with_day"),
        new TestCaseData(new TimeSpan(61, 2, 3, 4, 5), "61 days 02:03:04.005")
            .SetName("Interval_with_many_days"),
        new TestCaseData(new TimeSpan(new TimeSpan(2, 3, 4).Ticks + 10), "02:03:04.000001")
            .SetName("Interval_with_microsecond")
    };

    [Test, TestCaseSource(nameof(IntervalValues))]
    public Task Interval_as_TimeSpan(TimeSpan timeSpan, string sqlLiteral)
        => AssertType(timeSpan, sqlLiteral, "interval", NpgsqlDbType.Interval, isDefaultForWriting: false);

    [Test]
    public Task Interval_write_as_TimeSpan_truncates_ticks()
        => AssertTypeWrite(
            new TimeSpan(new TimeSpan(2, 3, 4).Ticks + 1),
            "02:03:04",
            "interval",
            NpgsqlDbType.Interval,
            isDefault: false);

    [Test]
    public Task Interval_as_NpgsqlInterval()
        => AssertType(
            new NpgsqlInterval(2, 15, 7384005000),
            "2 mons 15 days 02:03:04.005", "interval",
            NpgsqlDbType.Interval,
            isDefaultForReading: false,
            isDefaultForWriting: false);

    [Test]
    public Task Interval_with_months_cannot_read_as_TimeSpan()
        => AssertTypeUnsupportedRead("1 month 2 days", "interval");

    #endregion

    protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
    {
        var conn = await base.OpenConnectionAsync(connectionString);
        await conn.ExecuteNonQueryAsync("SET TimeZone='Europe/Berlin'");
        return conn;
    }

    protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        => throw new NotSupportedException();
}
