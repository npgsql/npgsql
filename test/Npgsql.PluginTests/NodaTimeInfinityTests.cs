using System;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Tests;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.PluginTests;

[TestFixture(false)]
#if DEBUG
[TestFixture(true)]
[NonParallelizable] // Since this test suite manipulates an AppContext switch
#endif
public class NodaTimeInfinityTests : TestBase, IDisposable
{
    [Test] // #4715
    public async Task DateRange_with_upper_bound_infinity()
    {
        if (Statics.DisableDateTimeInfinityConversions)
            return;

        await AssertType(
            new DateInterval(LocalDate.MinIsoValue, LocalDate.MaxIsoValue),
            "[-infinity,infinity]",
            "daterange",
            NpgsqlDbType.DateRange,
            isNpgsqlDbTypeInferredFromClrType: false, skipArrayCheck: true); // NpgsqlRange<T>[] is mapped to multirange by default, not array; test separately

        await AssertType(
            new [] {new DateInterval(LocalDate.MinIsoValue, LocalDate.MaxIsoValue)},
            """{"[-infinity,infinity]"}""",
            "daterange[]",
            NpgsqlDbType.DateRange | NpgsqlDbType.Array,
            isDefault: false, skipArrayCheck: true);

        await using var conn = await OpenConnectionAsync();
        if (conn.PostgreSqlVersion < new Version(14, 0))
            return;

        await AssertType(
            new [] {new DateInterval(LocalDate.MinIsoValue, LocalDate.MaxIsoValue)},
            """{[-infinity,infinity]}""",
            "datemultirange",
            NpgsqlDbType.DateMultirange, isNpgsqlDbTypeInferredFromClrType: false, skipArrayCheck: true);
    }

    [Test]
    public async Task Timestamptz_read_values()
    {
        if (Statics.DisableDateTimeInfinityConversions)
            return;

        await using var conn = await OpenConnectionAsync();
        await using var cmd =
            new NpgsqlCommand("SELECT 'infinity'::timestamp with time zone, '-infinity'::timestamp with time zone", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetFieldValue<Instant>(0), Is.EqualTo(Instant.MaxValue));
        Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(DateTime.MaxValue));
        Assert.That(reader.GetFieldValue<Instant>(1), Is.EqualTo(Instant.MinValue));
        Assert.That(reader.GetFieldValue<DateTime>(1), Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public async Task Timestamptz_write_values()
    {
        if (Statics.DisableDateTimeInfinityConversions)
            return;

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1::text, $2::text, $3::text, $4::text", conn)
        {
            Parameters =
            {
                new() { Value = Instant.MaxValue },
                new() { Value = DateTime.MaxValue },
                new() { Value = Instant.MinValue },
                new() { Value = DateTime.MinValue }
            }
        };
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader[0], Is.EqualTo("infinity"));
        Assert.That(reader[1], Is.EqualTo("infinity"));
        Assert.That(reader[2], Is.EqualTo("-infinity"));
        Assert.That(reader[3], Is.EqualTo("-infinity"));
    }

    [Test]
    public async Task Timestamptz_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT ($1 AT TIME ZONE 'UTC')::text", conn)
        {
            Parameters = { new() { Value = Instant.MinValue, NpgsqlDbType = NpgsqlDbType.TimestampTz } }
        };

        if (Statics.DisableDateTimeInfinityConversions)
        {
            // NodaTime Instant.MinValue is outside the PG timestamp range.
            Assert.That(async () => await cmd.ExecuteScalarAsync(),
                Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.DatetimeFieldOverflow));
        }
        else
        {
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("-infinity"));
        }

        await using var cmd2 = new NpgsqlCommand("SELECT ($1 AT TIME ZONE 'UTC')::text", conn)
        {
            Parameters = { new() { Value = Instant.MaxValue, NpgsqlDbType = NpgsqlDbType.TimestampTz } }
        };

        Assert.That(await cmd2.ExecuteScalarAsync(), Is.EqualTo(Statics.DisableDateTimeInfinityConversions ? "9999-12-31 23:59:59.999999" : "infinity"));
    }

    [Test]
    public async Task Timestamptz_read()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT '-infinity'::timestamp with time zone, 'infinity'::timestamp with time zone", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (Statics.DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(Instant.MinValue));
            Assert.That(reader[1], Is.EqualTo(Instant.MaxValue));
        }
    }

    [Test]
    public async Task Timestamp_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { Value = LocalDateTime.MinIsoValue, NpgsqlDbType = NpgsqlDbType.Timestamp } }
        };

        if (Statics.DisableDateTimeInfinityConversions)
        {
            // NodaTime LocalDateTime.MinValue is outside the PG timestamp range.
            Assert.That(async () => await cmd.ExecuteScalarAsync(),
                Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.DatetimeFieldOverflow));
        }
        else
        {
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("-infinity"));
        }

        await using var cmd2 = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { Value = LocalDateTime.MaxIsoValue, NpgsqlDbType = NpgsqlDbType.Timestamp } }
        };

        Assert.That(await cmd2.ExecuteScalarAsync(), Is.EqualTo(Statics.DisableDateTimeInfinityConversions
            ? "9999-12-31 23:59:59.999999"
            : "infinity"));
    }

    [Test]
    public async Task Timestamp_read()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT '-infinity'::timestamp without time zone, 'infinity'::timestamp without time zone", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (Statics.DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(LocalDateTime.MinIsoValue));
            Assert.That(reader[1], Is.EqualTo(LocalDateTime.MaxIsoValue));
        }
    }

    [Test]
    public async Task Date_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { Value = LocalDate.MinIsoValue, NpgsqlDbType = NpgsqlDbType.Date } }
        };

        // LocalDate.MinIsoValue is outside of the PostgreSQL date range
        if (Statics.DisableDateTimeInfinityConversions)
            Assert.That(async () => await cmd.ExecuteScalarAsync(),
                Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.DatetimeFieldOverflow));
        else
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("-infinity"));

        cmd.Parameters[0].Value = LocalDate.MaxIsoValue;

        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(Statics.DisableDateTimeInfinityConversions ? "9999-12-31" : "infinity"));
    }

    [Test]
    public async Task Date_read()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT '-infinity'::date, 'infinity'::date", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (Statics.DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(LocalDate.MinIsoValue));
            Assert.That(reader[1], Is.EqualTo(LocalDate.MaxIsoValue));
        }
    }

    [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
    public async Task DateConvertInfinity()
    {
        if (Statics.DisableDateTimeInfinityConversions)
            return;

        await using var conn = await OpenConnectionAsync();
        conn.ExecuteNonQuery("CREATE TEMP TABLE data (d1 DATE, d2 DATE, d3 DATE, d4 DATE)");

        using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1, @p2, @p3, @p4)", conn))
        {
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Date, LocalDate.MaxIsoValue);
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Date, LocalDate.MinIsoValue);
            cmd.Parameters.AddWithValue("p3", NpgsqlDbType.Date, DateTime.MaxValue);
            cmd.Parameters.AddWithValue("p4", NpgsqlDbType.Date, DateTime.MinValue);
            cmd.ExecuteNonQuery();
        }

        using (var cmd = new NpgsqlCommand("SELECT d1::TEXT, d2::TEXT, d3::TEXT, d4::TEXT FROM data", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo("infinity"));
            Assert.That(reader.GetValue(1), Is.EqualTo("-infinity"));
            Assert.That(reader.GetValue(2), Is.EqualTo("infinity"));
            Assert.That(reader.GetValue(3), Is.EqualTo("-infinity"));
        }

        using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            Assert.That(reader.GetFieldValue<LocalDate>(0), Is.EqualTo(LocalDate.MaxIsoValue));
            Assert.That(reader.GetFieldValue<LocalDate>(1), Is.EqualTo(LocalDate.MinIsoValue));
            Assert.That(reader.GetFieldValue<DateTime>(2), Is.EqualTo(DateTime.MaxValue));
            Assert.That(reader.GetFieldValue<DateTime>(3), Is.EqualTo(DateTime.MinValue));
        }
    }

    [Test]
    public async Task Interval_write()
    {
        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "17.0", "Infinity values for intervals were introduced in PostgreSQL 17");
        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { Value = Period.MinValue, NpgsqlDbType = NpgsqlDbType.Interval } }
        };

        // While Period.MinValue technically isn't outside of supported values by postgres, we can't reasonably convert it
        if (Statics.DisableDateTimeInfinityConversions)
        {
            Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<OverflowException>());
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            await conn.OpenAsync();
        }
        else
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("-infinity"));

        cmd.Parameters[0].Value = Period.MaxValue;

        // While Period.MaxValue technically isn't outside of supported values by postgres, we can't reasonably convert it
        if (Statics.DisableDateTimeInfinityConversions)
            Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<OverflowException>());
        else
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("infinity"));
    }

    [Test]
    public async Task Interval_read()
    {
        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "17.0", "Infinity values for intervals were introduced in PostgreSQL 17");

        await using var cmd = new NpgsqlCommand("SELECT '-infinity'::interval, 'infinity'::interval", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (Statics.DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(Period.MinValue));
            Assert.That(reader[1], Is.EqualTo(Period.MaxValue));
        }
    }

    [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
    public async Task Interval_convert_infinity()
    {
        if (Statics.DisableDateTimeInfinityConversions)
            return;

        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "17.0", "Infinity values for intervals were introduced in PostgreSQL 17");
        await conn.ExecuteNonQueryAsync("CREATE TEMP TABLE data (i1 INTERVAL, i2 INTERVAL)");

        using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1, @p2)", conn))
        {
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Interval, Period.MaxValue);
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Interval, Period.MinValue);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = new NpgsqlCommand("SELECT i1::TEXT, i2::TEXT, i1, i2 FROM data", conn))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            await reader.ReadAsync();
            Assert.That(reader.GetValue(0), Is.EqualTo("infinity"));
            Assert.That(reader.GetValue(1), Is.EqualTo("-infinity"));
            Assert.That(reader.GetFieldValue<Period>(2), Is.EqualTo(Period.MaxValue));
            Assert.That(reader.GetFieldValue<Period>(3), Is.EqualTo(Period.MinValue));
        }
    }

    protected override NpgsqlDataSource DataSource { get; }

    public NodaTimeInfinityTests(bool disableDateTimeInfinityConversions)
    {
#if DEBUG
        Statics.DisableDateTimeInfinityConversions = disableDateTimeInfinityConversions;
#else
        if (disableDateTimeInfinityConversions)
        {
            Assert.Ignore(
                "NodaTimeInfinityTests rely on the Npgsql.DisableDateTimeInfinityConversions AppContext switch and can only be run in DEBUG builds");
        }
#endif

        var builder = CreateDataSourceBuilder();
        builder.UseNodaTime();
        builder.ConnectionStringBuilder.Options = "-c TimeZone=Europe/Berlin";
        DataSource = builder.Build();
    }

    public void Dispose()
    {
#if DEBUG
        Statics.DisableDateTimeInfinityConversions = false;
#endif

        DataSource.Dispose();
    }
}
