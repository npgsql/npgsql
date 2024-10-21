using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.NodaTime.Properties;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable AccessToDisposedClosure

namespace Npgsql.NodaTime.Tests;

// Since this test suite manipulates TimeZone, it is incompatible with multiplexing
public class NodaTimeTests : TestBase
{
    #region Timestamp without time zone

    static readonly TestCaseData[] TimestampValues =
    {
        new TestCaseData(new LocalDateTime(1998, 4, 12, 13, 26, 38, 789), "1998-04-12 13:26:38.789")
            .SetName("TimestampPre2000"),
        new TestCaseData(new LocalDateTime(2015, 1, 27, 8, 45, 12, 345), "2015-01-27 08:45:12.345")
            .SetName("TimestampPost2000"),
        new TestCaseData(new LocalDateTime(1999, 12, 31, 23, 59, 59, 999).PlusNanoseconds(456000), "1999-12-31 23:59:59.999456")
            .SetName("TimestampMicroseconds")
    };

    [Test, TestCaseSource(nameof(TimestampValues))]
    public async Task Timestamp_read(LocalDateTime localDateTime, string s)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{s}'::timestamp without time zone", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("timestamp without time zone"));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(LocalDateTime)));

        Assert.That(reader[0], Is.EqualTo(localDateTime));
        Assert.That(reader.GetFieldValue<LocalDateTime>(0), Is.EqualTo(localDateTime));
        Assert.That(reader.GetDateTime(0), Is.EqualTo(localDateTime.ToDateTimeUnspecified()));
        Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(localDateTime.ToDateTimeUnspecified()));

        Assert.That(() => reader.GetFieldValue<Instant>(0), Throws.TypeOf<InvalidCastException>());
        Assert.That(() => reader.GetFieldValue<ZonedDateTime>(0), Throws.TypeOf<InvalidCastException>());

        // Internal PostgreSQL representation, for out-of-range values.
        Assert.That(() => reader.GetInt64(0), Throws.Nothing);
    }

    [Test, TestCaseSource(nameof(TimestampValues))]
    public async Task Timestamp_write_values(LocalDateTime localDateTime, string expected)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters =
            {
                new() { Value = localDateTime, NpgsqlDbType = NpgsqlDbType.Timestamp }
            }
        };

        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected));
    }

    static NpgsqlParameter[] TimestampParameters
    {
        get
        {
            var localDateTime = new LocalDateTime(1998, 4, 12, 13, 26, 38);

            return new NpgsqlParameter[]
            {
                new() { Value = localDateTime },
                new() { Value = localDateTime, NpgsqlDbType = NpgsqlDbType.Timestamp },
                new() { Value = localDateTime, DbType = DbType.DateTime2 },
                new() { Value = localDateTime.ToDateTimeUnspecified() },
                new() { Value = DateTime.SpecifyKind(localDateTime.ToDateTimeUnspecified(), DateTimeKind.Local) },
                new() { Value = -54297202000000L, NpgsqlDbType = NpgsqlDbType.Timestamp }
            };
        }
    }

    [Test, TestCaseSource(nameof(TimestampParameters))]
    public async Task Timestamp_resolution(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        conn.TypeMapper.Reset();
        conn.TypeMapper.UseNodaTime();

        await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
        {
            Parameters = { parameter }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo("timestamp without time zone"));
        Assert.That(reader[1], Is.EqualTo("1998-04-12 13:26:38"));
    }

    static NpgsqlParameter[] TimestampInvalidParameters
        => new NpgsqlParameter[]
        {
            new() { Value = new LocalDateTime().InUtc().ToInstant(), NpgsqlDbType = NpgsqlDbType.Timestamp },
            new() { Value = new DateTimeOffset(), NpgsqlDbType = NpgsqlDbType.Timestamp },
            new() { Value = DateTime.UtcNow, NpgsqlDbType = NpgsqlDbType.Timestamp }
        };

    [Test, TestCaseSource(nameof(TimestampInvalidParameters))]
    public async Task Timestamp_resolution_failure(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { parameter }
        };

        Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidCastException>());
    }

    #endregion Timestamp without time zone

    #region Timestamp with time zone

    [Test, TestCaseSource(nameof(TimestampValues))]
    public async Task Timestamptz_read(LocalDateTime expectedLocalDateTime, string s)
    {
        var expectedInstance = expectedLocalDateTime.InUtc().ToInstant();

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{s}+00'::timestamp with time zone", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("timestamp with time zone"));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Instant)));

        Assert.That(reader[0], Is.EqualTo(expectedInstance));
        Assert.That(reader.GetFieldValue<Instant>(0), Is.EqualTo(expectedInstance));
        Assert.That(reader.GetFieldValue<ZonedDateTime>(0), Is.EqualTo(expectedInstance.InUtc()));
        Assert.That(reader.GetFieldValue<OffsetDateTime>(0), Is.EqualTo(expectedInstance.WithOffset(Offset.Zero)));
        Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expectedInstance.ToDateTimeUtc()));
        Assert.That(reader.GetFieldValue<DateTimeOffset>(0), Is.EqualTo(expectedInstance.ToDateTimeOffset()));

        Assert.That(() => reader.GetFieldValue<LocalDateTime>(0), Throws.TypeOf<InvalidCastException>());
#if NET6_0_OR_GREATER
        Assert.That(() => reader.GetFieldValue<DateOnly>(0), Throws.TypeOf<InvalidCastException>());
#endif

        // Internal PostgreSQL representation, for out-of-range values.
        Assert.That(() => reader.GetInt64(0), Throws.Nothing);
    }

    static readonly TestCaseData[] TimestampTzValues =
    {
        new TestCaseData(new LocalDateTime(1998, 4, 12, 13, 26, 38, 789), "1998-04-12 15:26:38.789+02")
            .SetName("TimestampTzPre2000"),
        new TestCaseData(new LocalDateTime(2015, 1, 27, 8, 45, 12, 345), "2015-01-27 09:45:12.345+01")
            .SetName("TimestampTzPost2000"),
        new TestCaseData(new LocalDateTime(1999, 12, 31, 23, 59, 59, 999).PlusNanoseconds(456000), "2000-01-01 00:59:59.999456+01")
            .SetName("TimestampTzMicroseconds"),
    };

    [Test, TestCaseSource(nameof(TimestampTzValues))]
    public async Task Timestamptz_write_values(LocalDateTime localDateTime, string expected)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { Value = localDateTime.InUtc().ToInstant(), NpgsqlDbType = NpgsqlDbType.TimestampTz} }
        };

        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected));
    }

    static NpgsqlParameter[] TimestamptzParameters
    {
        get
        {
            var localDateTime = new LocalDateTime(1998, 4, 12, 13, 26, 38);
            var instance = localDateTime.InUtc().ToInstant();

            return new NpgsqlParameter[]
            {
                new() { Value = instance },
                new() { Value = instance, NpgsqlDbType = NpgsqlDbType.TimestampTz },
                new() { Value = instance, DbType = DbType.DateTimeOffset },
                new() { Value = instance.InUtc() },
                new() { Value = instance.WithOffset(Offset.Zero) },
                new() { Value = instance.InUtc().ToDateTimeUtc() },
                new() { Value = instance.ToDateTimeOffset() },
                new() { Value = -54297202000000L, NpgsqlDbType = NpgsqlDbType.TimestampTz }
            };
        }
    }

    [Test, TestCaseSource(nameof(TimestamptzParameters))]
    public async Task Timestamptz_resolution(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        conn.TypeMapper.Reset();
        conn.TypeMapper.UseNodaTime();

        await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
        {
            Parameters = { parameter }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo("timestamp with time zone"));
        Assert.That(reader[1], Is.EqualTo("1998-04-12 15:26:38+02")); // We set TimeZone to Europe/Berlin below
    }

    static readonly TestCaseData[] TimestamptzInvalidParameters =
    {
        new TestCaseData(new LocalDateTime())
            .SetName("TimestamptzInvalidParameters_LocalDateTime"),
        new TestCaseData(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Local))
            .SetName("TimestamptzInvalidParameters_DateTime_Local"),
        new TestCaseData(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Unspecified))
            .SetName("TimestamptzInvalidParameters_DateTime_Unspecified"),
        new TestCaseData(new DateTimeOffset(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Unspecified), TimeSpan.FromHours(2)))
            .SetName("TimestamptzInvalidParameters_DateTimeOffset_non_UTC"),

        // We only support ZonedDateTime and OffsetDateTime in UTC
        new TestCaseData(new LocalDateTime().InUtc().ToInstant().InZone(DateTimeZoneProviders.Tzdb["America/New_York"]))
            .SetName("TimestamptzInvalidParameters_ZonedDateTime_non_UTC"),
        new TestCaseData(new LocalDateTime().WithOffset(Offset.FromHours(1)))
            .SetName("TimestamptzInvalidParameters_OffsetDateTime_non_UTC")
    };

    [Test, TestCaseSource(nameof(TimestamptzInvalidParameters))]
    public async Task Timestamptz_resolution_failure(object value)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { NpgsqlDbType = NpgsqlDbType.TimestampTz, Value = value } }
        };

        Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidCastException>());
    }

    static TestCaseData[] Timestamptz_range_read_values
    {
        get
        {
            var (start, end) = (
                new LocalDateTime(2020, 1, 1, 12, 0, 0).InUtc().ToInstant(),
                new LocalDateTime(2020, 1, 5, 12, 0, 0).InUtc().ToInstant());

            return new[]
            {
                new TestCaseData(
                        "[2020-01-01 12:00:00Z,2020-01-05 12:00:00Z)",
                        new Interval(start, end),
                        new NpgsqlRange<Instant>(start, true, end, false))
                    .SetName("InclusiveExclusive"),

                new TestCaseData(
                        "(2020-01-01 12:00:00Z,2020-01-05 12:00:00Z]",
                        new Interval(start + Duration.Epsilon, end + Duration.Epsilon),
                        new NpgsqlRange<Instant>(start, false, end, true))
                    .SetName("ExclusiveInclusive"),

                new TestCaseData(
                        "(,]",
                        new Interval(null, null),
                        new NpgsqlRange<Instant>(default, false, true, default, false, true))
                    .SetName("InfiniteInfinite")
            };
        }
    }

    [Test, TestCaseSource(nameof(Timestamptz_range_read_values))]
    public async Task Timestamptz_range_read(string sql, Interval interval, NpgsqlRange<Instant> range)
    {
        // NodaTime Interval is exclusive at the end, and PostgreSQL tstzrange is a non-discrete range.
        // This means that we need to manually patch up incoming ranges by adding/removing an epsilon.
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{sql}'::tstzrange", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("tstzrange"));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Interval)));
        Assert.That(reader.GetValue(0), Is.EqualTo(interval));
        Assert.That(reader.GetFieldValue<Interval>(0), Is.EqualTo(interval));
        Assert.That(reader.GetFieldValue<NpgsqlRange<Instant>>(0), Is.EqualTo(range));
        Assert.That(reader.GetFieldValue<NpgsqlRange<ZonedDateTime>>(0), Is.EqualTo(new NpgsqlRange<ZonedDateTime>(
            range.LowerBound.InUtc(), range.LowerBoundIsInclusive, range.LowerBoundInfinite,
            range.UpperBound.InUtc(), range.UpperBoundIsInclusive, range.UpperBoundInfinite)));
        Assert.That(reader.GetFieldValue<NpgsqlRange<OffsetDateTime>>(0),
            Is.EqualTo(new NpgsqlRange<OffsetDateTime>(
                range.LowerBound.WithOffset(Offset.Zero), range.LowerBoundIsInclusive, range.LowerBoundInfinite,
                range.UpperBound.WithOffset(Offset.Zero), range.UpperBoundIsInclusive, range.UpperBoundInfinite)));
        Assert.That(reader.GetFieldValue<NpgsqlRange<DateTime>>(0),
            Is.EqualTo(new NpgsqlRange<DateTime>(
                range.LowerBound.ToDateTimeUtc(), range.LowerBoundIsInclusive, range.LowerBoundInfinite,
                range.UpperBound.ToDateTimeUtc(), range.UpperBoundIsInclusive, range.UpperBoundInfinite)));
        Assert.That(reader.GetFieldValue<NpgsqlRange<DateTimeOffset>>(0),
            Is.EqualTo(new NpgsqlRange<DateTimeOffset>(
                range.LowerBound.ToDateTimeOffset(), range.LowerBoundIsInclusive, range.LowerBoundInfinite,
                range.UpperBound.ToDateTimeOffset(), range.UpperBoundIsInclusive, range.UpperBoundInfinite)));
    }

    static NpgsqlParameter[] Timestamptz_range_resolution_values
    {
        get
        {
            var (start, end) = (
                new LocalDateTime(2020, 1, 1, 12, 0, 0).InUtc().ToInstant(),
                new LocalDateTime(2020, 1, 5, 12, 0, 0).InUtc().ToInstant());

            return new NpgsqlParameter[]
            {
                new() { Value = new Interval(start, end) },
                new() { Value = new Interval(start, end), NpgsqlDbType = NpgsqlDbType.TimestampTzRange},
                new() { Value = new NpgsqlRange<Instant>(start, true, end, false) },
                new() { Value = new NpgsqlRange<ZonedDateTime>(start.InUtc(), true, end.InUtc(), false) },
                new() { Value = new NpgsqlRange<OffsetDateTime>(start.WithOffset(Offset.Zero), true, end.WithOffset(Offset.Zero), false) }
            };
        }
    }

    [Test, TestCaseSource(nameof(Timestamptz_range_resolution_values))]
    public async Task Timestamptz_range_resolution(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        conn.TypeMapper.Reset();
        conn.TypeMapper.UseNodaTime();

        await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
        {
            Parameters = { parameter }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo("tstzrange"));
        // We set TimeZone to Europe/Berlin below
        Assert.That(reader[1], Is.EqualTo(@"[""2020-01-01 13:00:00+01"",""2020-01-05 13:00:00+01"")"));
    }

    [Test]
    public async Task Timestamptz_multirange_read()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        var start1 = new LocalDateTime(2020, 1, 1, 12, 0, 0).InUtc().ToInstant();
        var end1 = new LocalDateTime(2020, 1, 5, 12, 0, 0).InUtc().ToInstant();

        var start2 = new LocalDateTime(2020, 1, 7, 12, 0, 0).InUtc().ToInstant();
        var end2 = new LocalDateTime(2020, 1, 10, 12, 0, 0).InUtc().ToInstant();

        await using var cmd = new NpgsqlCommand(@"
SELECT '{[""2020-01-01 12:00:00Z"",""2020-01-05 12:00:00Z""), (""2020-01-07 12:00:00Z"",""2020-01-10 12:00:00Z""]}'::tstzmultirange", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("tstzmultirange"));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Interval[])));

        var intervalMultirange = new Interval[]
        {
            new(start1, end1),
            new(start2 + Duration.Epsilon, end2 + Duration.Epsilon)
        };
        Assert.That(reader.GetValue(0), Is.EqualTo(intervalMultirange));
        Assert.That(reader.GetFieldValue<Interval[]>(0), Is.EqualTo(intervalMultirange));
        Assert.That(reader.GetFieldValue<List<Interval>>(0), Is.EqualTo(intervalMultirange));

        var instantMultirange = new NpgsqlRange<Instant>[]
        {
            new(start1, true, end1, false),
            new(start2, false, end2, true)
        };
        Assert.That(reader.GetFieldValue<NpgsqlRange<Instant>[]>(0), Is.EqualTo(instantMultirange));
        Assert.That(reader.GetFieldValue<List<NpgsqlRange<Instant>>>(0), Is.EqualTo(instantMultirange));

        var zonedDateTimeMultirange = new NpgsqlRange<ZonedDateTime>[]
        {
            new(start1.InUtc(), true, end1.InUtc(), false),
            new(start2.InUtc(), false, end2.InUtc(), true)
        };
        Assert.That(reader.GetFieldValue<NpgsqlRange<ZonedDateTime>[]>(0), Is.EqualTo(zonedDateTimeMultirange));
        Assert.That(reader.GetFieldValue<List<NpgsqlRange<ZonedDateTime>>>(0), Is.EqualTo(zonedDateTimeMultirange));

        var offsetDateTimeMultirange = new NpgsqlRange<OffsetDateTime>[]
        {
            new(start1.WithOffset(Offset.Zero), true, end1.WithOffset(Offset.Zero), false),
            new(start2.WithOffset(Offset.Zero), false, end2.WithOffset(Offset.Zero), true)
        };
        Assert.That(reader.GetFieldValue<NpgsqlRange<OffsetDateTime>[]>(0), Is.EqualTo(offsetDateTimeMultirange));
        Assert.That(reader.GetFieldValue<List<NpgsqlRange<OffsetDateTime>>>(0), Is.EqualTo(offsetDateTimeMultirange));
    }

    static NpgsqlParameter[] Timestamptz_multirange_resolution_parameters
    {
        get
        {
            var start1 = new LocalDateTime(2020, 1, 1, 12, 0, 0).InUtc().ToInstant();
            var end1 = new LocalDateTime(2020, 1, 5, 12, 0, 0).InUtc().ToInstant();

            var start2 = new LocalDateTime(2020, 1, 7, 12, 0, 0).InUtc().ToInstant();
            var end2 = new LocalDateTime(2020, 1, 10, 12, 0, 0).InUtc().ToInstant();

            var intervalMultirange = new Interval[]
            {
                new(start1, end1),
                new(start2, end2)
            };

            return new NpgsqlParameter[]
            {
                new() { Value = intervalMultirange },
                new() { Value = intervalMultirange, NpgsqlDbType = NpgsqlDbType.TimestampTzMultirange },
                new() { Value = new NpgsqlRange<Instant>[]
                {
                    new(start1, true, end1, false),
                    new(start2, true, end2, false)
                }},
                new() { Value = new NpgsqlRange<ZonedDateTime>[]
                {
                    new(start1.InUtc(), true, end1.InUtc(), false),
                    new(start2.InUtc(), true, end2.InUtc(), false)
                }},
                new() { Value = new NpgsqlRange<OffsetDateTime>[]
                {
                    new(start1.WithOffset(Offset.Zero), true, end1.WithOffset(Offset.Zero), false),
                    new(start2.WithOffset(Offset.Zero), true, end2.WithOffset(Offset.Zero), false)
                }}
            };
        }
    }

    [Test, TestCaseSource(nameof(Timestamptz_multirange_resolution_parameters))]
    public async Task Timestamptz_multirange_resolution(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
        conn.TypeMapper.Reset();
        conn.TypeMapper.UseNodaTime();

        await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
        {
            Parameters = { parameter }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo("tstzmultirange"));
        Assert.That(reader[1], Is.EqualTo(@"{[""2020-01-01 13:00:00+01"",""2020-01-05 13:00:00+01""),[""2020-01-07 13:00:00+01"",""2020-01-10 13:00:00+01"")}"));
    }

    #endregion Timestamp with time zone

    #region Date

#pragma warning disable 618 // NpgsqlDate is obsolete, remove in 7.0

    [Test]
    public async Task Date()
    {
        await using var conn = await OpenConnectionAsync();
        var localDate = new LocalDate(2002, 3, 4);
        var dateTime = new DateTime(localDate.Year, localDate.Month, localDate.Day);

        using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE data (d1 DATE, d2 DATE, d3 DATE, d4 DATE, d5 DATE)", conn))
            cmd.ExecuteNonQuery();

        using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1, @p2, @p3, @p4, @p5)", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Date) { Value = localDate });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = localDate });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3", Value = new LocalDate(-5, 3, 3) });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4", Value = dateTime });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p5", Value = dateTime, NpgsqlDbType = NpgsqlDbType.Date });
            cmd.ExecuteNonQuery();
        }

        using (var cmd = new NpgsqlCommand("SELECT d1::TEXT, d2::TEXT, d3::TEXT, d4::TEXT, d5::TEXT FROM data", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo("2002-03-04"));
            Assert.That(reader.GetValue(1), Is.EqualTo("2002-03-04"));
            Assert.That(reader.GetValue(2), Is.EqualTo("0006-03-03 BC"));
            Assert.That(reader.GetValue(3), Is.EqualTo("2002-03-04"));
            Assert.That(reader.GetValue(4), Is.EqualTo("2002-03-04"));
        }

        using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();

            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(LocalDate)));
            Assert.That(reader.GetFieldValue<LocalDate>(0), Is.EqualTo(localDate));
            Assert.That(reader.GetValue(0), Is.EqualTo(localDate));
            Assert.That(() => reader.GetDateTime(0), Is.EqualTo(dateTime));
            Assert.That(() => reader.GetDate(0), Is.EqualTo(new NpgsqlDate(localDate.Year, localDate.Month, localDate.Day)));
            Assert.That(reader.GetFieldValue<LocalDate>(2), Is.EqualTo(new LocalDate(-5, 3, 3)));
            Assert.That(reader.GetFieldValue<DateTime>(3), Is.EqualTo(dateTime));
            Assert.That(reader.GetDateTime(4), Is.EqualTo(dateTime));

            // Internal PostgreSQL representation, for out-of-range values.
            Assert.That(() => reader.GetInt32(0), Throws.Nothing);
        }
    }

#pragma warning restore 618 // NpgsqlDate is obsolete, remove in 7.0

#if NET6_0_OR_GREATER
    [Test]
    public async Task Date_DateOnly()
    {
        await using var conn = await OpenConnectionAsync();
        var localDate = new LocalDate(2002, 3, 4);
        var dateOnly = new DateOnly(2002, 3, 4);

        using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE data (d1 DATE)", conn))
            cmd.ExecuteNonQuery();

        using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1)", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1", Value = dateOnly });
            cmd.ExecuteNonQuery();
        }

        using (var cmd = new NpgsqlCommand("SELECT d1::TEXT FROM data", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo("2002-03-04"));
        }

        using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();

            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(LocalDate)));
            Assert.That(reader.GetValue(0), Is.EqualTo(localDate));
            Assert.That(reader.GetFieldValue<DateOnly>(0), Is.EqualTo(dateOnly));
        }
    }
#endif

    #endregion Date

    #region Time

    [Test]
    public async Task Time()
    {
        await using var conn = await OpenConnectionAsync();
        var expected = new LocalTime(1, 2, 3, 4).PlusNanoseconds(5000);
        var timeSpan = new TimeSpan(0, 1, 2, 3, 4).Add(TimeSpan.FromTicks(50));

        using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);
        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Time) { Value = expected });
        cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Time) { Value = expected });
        cmd.Parameters.Add(new NpgsqlParameter("p3", DbType.Time) { Value = timeSpan });
        using var reader = cmd.ExecuteReader();
        reader.Read();

        for (var i = 0; i < cmd.Parameters.Count; i++)
        {
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(LocalTime)));
            Assert.That(reader.GetFieldValue<LocalTime>(i), Is.EqualTo(expected));
            Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            Assert.That(() => reader.GetTimeSpan(i), Is.EqualTo(timeSpan));
        }
    }

#if NET6_0_OR_GREATER
    [Test]
    public async Task Time_TimeOnly()
    {
        await using var conn = await OpenConnectionAsync();
        var timeOnly = new TimeOnly(1, 2, 3, 500);
        var localTime = new LocalTime(1, 2, 3, 500);

        using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1", Value = timeOnly });

        using var reader = cmd.ExecuteReader();
        reader.Read();

        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(LocalTime)));
        Assert.That(reader.GetFieldValue<TimeOnly>(0), Is.EqualTo(timeOnly));
        Assert.That(reader.GetValue(0), Is.EqualTo(localTime));
    }
#endif

    #endregion Time

    #region Time with time zone

    [Test]
    public async Task TimeTz()
    {
        await using var conn = await OpenConnectionAsync();
        var time = new LocalTime(1, 2, 3, 4).PlusNanoseconds(5000);
        var offset = Offset.FromHoursAndMinutes(3, 30) + Offset.FromSeconds(5);
        var expected = new OffsetTime(time, offset);
        var dateTimeOffset = new DateTimeOffset(0001, 01, 02, 03, 43, 20, TimeSpan.FromHours(3));

        using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);
        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimeTz) { Value = expected });
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = expected });
        cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.TimeTz) { Value = dateTimeOffset });

        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();

            for (var i = 0; i < 2; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(OffsetTime)));
                Assert.That(reader.GetFieldValue<OffsetTime>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            }
        }

        cmd.CommandText = "SELECT @p";
        cmd.Parameters.Clear();
        cmd.Parameters.Add(new("p", NpgsqlDbType.TimeTz) { Value = DateTime.UtcNow });
        Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<InvalidCastException>());

        cmd.Parameters.Clear();
        cmd.Parameters.Add(new("p", NpgsqlDbType.TimeTz) { Value = TimeSpan.Zero });
        Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<InvalidCastException>());
    }

    #endregion Time with time zone

    #region Interval

    [Test]
    public async Task Interval_as_Period()
    {
        // PG has microsecond precision, so sub-microsecond values are stripped
        var expectedPeriod = new PeriodBuilder
        {
            Years = 1,
            Months = 2,
            Weeks = 3,
            Days = 4,
            Hours = 5,
            Minutes = 6,
            Seconds = 7,
            Milliseconds = 8,
            Nanoseconds = 9000
        }.Build().Normalize();

        await using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Interval) { Value = expectedPeriod });
        cmd.Parameters.AddWithValue("p2", expectedPeriod);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        for (var i = 0; i < 2; i++)
        {
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Period)));
            Assert.That(reader.GetFieldValue<Period>(i), Is.EqualTo(expectedPeriod));
            Assert.That(reader.GetValue(i), Is.EqualTo(expectedPeriod));
        }
    }

    [Test]
    public async Task Interval_as_Duration()
    {
        await using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);

        // PG has microsecond precision, so sub-microsecond values are stripped
        var expected = Duration.FromDays(5) + Duration.FromMinutes(4) + Duration.FromSeconds(3) + Duration.FromMilliseconds(2) +
                       Duration.FromNanoseconds(1500);

        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Interval) { Value = expected });
        cmd.Parameters.AddWithValue("p2", expected);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        for (var i = 0; i < 2; i++)
        {
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Period)));
            Assert.That(reader.GetFieldValue<Duration>(i), Is.EqualTo(expected - Duration.FromNanoseconds(500)));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3438")]
    public async Task Bug3438()
    {
        await using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);

        var expected = Duration.FromSeconds(2148);

        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Interval) { Value = expected });
        cmd.Parameters.AddWithValue("p2", expected);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        for (var i = 0; i < 2; i++)
        {
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Period)));
        }
    }

    [Test]
    public async Task Interval_as_TimeSpan()
    {
        var expected = new TimeSpan(1, 2, 3, 4, 5);
        await using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);

        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Interval) { Value = expected });
        cmd.Parameters.AddWithValue("p2", expected);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        for (var i = 0; i < 2; i++)
        {
            Assert.That(() => reader.GetTimeSpan(i), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<TimeSpan>(i), Is.EqualTo(expected));
        }
    }

    [Test]
    public async Task Interval_as_Duration_with_months_fails()
    {
        await using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT make_interval(months => 2)", conn);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        Assert.That(() => reader.GetFieldValue<Duration>(0), Throws.Exception.TypeOf<NpgsqlException>().With.Message.EqualTo(
            "Cannot read PostgreSQL interval with non-zero months to NodaTime Duration. Try reading as a NodaTime Period instead."));
    }

    [Test]
    public async Task Interval_as_NpgsqlInterval()
    {
        var expected = new NpgsqlInterval(0, 1, 7384005000);
        await using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);

        cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Interval) { Value = expected });
        cmd.Parameters.AddWithValue("p2", expected);
        using var reader = cmd.ExecuteReader();
        reader.Read();

        for (var i = 0; i < 2; i++)
        {
            Assert.That(reader.GetFieldValue<NpgsqlInterval>(i), Is.EqualTo(expected));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/5867")]
    public async Task Normalize_period_on_write()
    {
        var value = Period.FromTicks(-3675048768766);
        var expected = value.Normalize();
        var expectedAfterRoundtripBuilder = expected.ToBuilder();
        // Postgres doesn't support nanoseconds, trim them to microseconds
        expectedAfterRoundtripBuilder.Nanoseconds -= expected.Nanoseconds % 1000;
        var expectedAfterRoundtrip = expectedAfterRoundtripBuilder.Build();

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1, $2", conn);
        cmd.Parameters.AddWithValue(value);
        cmd.Parameters.AddWithValue(expected);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        var dbValue = reader.GetFieldValue<Period>(0);
        var dbExpected = reader.GetFieldValue<Period>(1);

        Assert.That(dbValue, Is.EqualTo(dbExpected));
        Assert.That(dbValue, Is.EqualTo(expectedAfterRoundtrip));
    }

    [Test]
    public async Task Period_write_throw_on_overflow()
    {
        var periodBuilder = new PeriodBuilder
        {
            Years = int.MaxValue
        };
        var ex = await AssertTypeUnsupportedWrite<Period, ArgumentException>(periodBuilder.Build(), "interval");
        Assert.That(ex.Message, Is.EqualTo(NpgsqlNodaTimeStrings.CannotWritePeriodDueToOverflow));
        Assert.That(ex.InnerException, Is.TypeOf<OverflowException>());
    }

    #endregion Interval

    #region DateInterval

    [Test]
    public async Task Date_range_read()
    {
        var dateInterval = new DateInterval(new(2020, 1, 1), new(2020, 1, 5));

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT '[2020-01-01,2020-01-05]'::daterange", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("daterange"));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateInterval)));
        Assert.That(reader.GetValue(0), Is.EqualTo(dateInterval));
        Assert.That(reader.GetFieldValue<DateInterval>(0), Is.EqualTo(dateInterval));
        Assert.That(reader.GetFieldValue<NpgsqlRange<LocalDate>>(0),
            Is.EqualTo(new NpgsqlRange<LocalDate>(new(2020, 1, 1), true, new(2020, 1, 6), false)));
#if NET6_0_OR_GREATER
        Assert.That(reader.GetFieldValue<NpgsqlRange<DateOnly>>(0),
            Is.EqualTo(new NpgsqlRange<DateOnly>(new(2020, 1, 1), true, new(2020, 1, 6), false)));
#endif
    }

    static NpgsqlParameter[] DaterangeParameters
    {
        get
        {
            var dateInterval = new DateInterval(new(2020, 1, 1), new(2020, 1, 5));
            var range = new NpgsqlRange<LocalDate>(new(2020, 1, 1), new(2020, 1, 5));

            return new NpgsqlParameter[]
            {
                new() { Value = dateInterval },
                new() { Value = range },
                new() { Value = dateInterval, NpgsqlDbType = NpgsqlDbType.DateRange },
                new() { Value = range, NpgsqlDbType = NpgsqlDbType.DateRange },
#if NET6_0_OR_GREATER
                new() { Value = new NpgsqlRange<DateOnly>(new(2020, 1, 1), new(2020, 1, 5)) },
                new() { Value = new NpgsqlRange<DateOnly>(new(2020, 1, 1), new(2020, 1, 5)), NpgsqlDbType = NpgsqlDbType.DateRange },
#endif
            };
        }
    }

    [Test, TestCaseSource(nameof(DaterangeParameters))]
    public async Task Date_range_resolution(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        conn.TypeMapper.Reset();
        conn.TypeMapper.UseNodaTime();

        await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
        {
            Parameters = { parameter }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo("daterange"));
        Assert.That(reader[1], Is.EqualTo("[2020-01-01,2020-01-06)"));
    }

    [Test]
    public async Task Date_multirange_read()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");

        var dateIntervalMultirange = new DateInterval[]
        {
            new(new(2020, 1, 1), new(2020, 1, 5)),
            new(new(2020, 1, 7), new(2020, 1, 10))
        };
        var rangeMultirange = new NpgsqlRange<LocalDate>[]
        {
            new(new(2020, 1, 1), true, new(2020, 1, 6), false),
            new(new(2020, 1, 7), true, new(2020, 1, 11), false)
        };

        await using var cmd = new NpgsqlCommand("SELECT '{[2020-01-01,2020-01-05], [2020-01-07,2020-01-10]}'::datemultirange", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("datemultirange"));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateInterval[])));
        Assert.That(reader.GetValue(0), Is.EqualTo(dateIntervalMultirange));
        Assert.That(reader.GetFieldValue<DateInterval[]>(0), Is.EqualTo(dateIntervalMultirange));
        Assert.That(reader.GetFieldValue<List<DateInterval>>(0), Is.EqualTo(dateIntervalMultirange));
        Assert.That(reader.GetFieldValue<NpgsqlRange<LocalDate>[]>(0), Is.EqualTo(rangeMultirange));
        Assert.That(reader.GetFieldValue<List<NpgsqlRange<LocalDate>>>(0), Is.EqualTo(rangeMultirange));
    }

    static NpgsqlParameter[] DatemultirangeParameters
    {
        get
        {
            var dateIntervalMultirange = new DateInterval[]
            {
                new(new(2020, 1, 1), new(2020, 1, 5)),
                new(new(2020, 1, 7), new(2020, 1, 10))
            };
            var rangeMultirange = new NpgsqlRange<LocalDate>[]
            {
                new(new(2020, 1, 1), true, new(2020, 1, 6), false),
                new(new(2020, 1, 7), true, new(2020, 1, 11), false)
            };

            return new NpgsqlParameter[]
            {
                new() { Value = dateIntervalMultirange },
                new() { Value = rangeMultirange },
                new() { Value = dateIntervalMultirange, NpgsqlDbType = NpgsqlDbType.DateMultirange },
                new() { Value = rangeMultirange, NpgsqlDbType = NpgsqlDbType.DateMultirange }
            };
        }
    }

    [Test, TestCaseSource(nameof(DatemultirangeParameters))]
    public async Task Date_multirange_resolution(NpgsqlParameter parameter)
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
        conn.TypeMapper.Reset();
        conn.TypeMapper.UseNodaTime();

        await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
        {
            Parameters = { parameter }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo("datemultirange"));
        Assert.That(reader[1], Is.EqualTo("{[2020-01-01,2020-01-06),[2020-01-07,2020-01-11)}"));
    }

    #endregion DateInterval

    #region Support

    protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
    {
        var conn = await base.OpenConnectionAsync(connectionString);
        conn.TypeMapper.UseNodaTime();
        await conn.ExecuteNonQueryAsync("SET TimeZone='Europe/Berlin'");
        return conn;
    }

    protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        => throw new NotSupportedException();

    #endregion Support
}
