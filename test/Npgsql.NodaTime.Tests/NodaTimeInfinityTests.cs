using System;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Tests;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Tests;

[TestFixture(false)]
#if DEBUG
[TestFixture(true)]
#endif
[NonParallelizable]
public class NodaTimeInfinityTests : TestBase
{
    [Test] // #4715
    public async Task DateRange_with_upper_bound_infinity()
    {
        if (DisableDateTimeInfinityConversions)
            return;

        await AssertType(
            new DateInterval(LocalDate.MinIsoValue, LocalDate.MaxIsoValue),
            "[-infinity,infinity]",
            "daterange",
            NpgsqlDbType.DateRange);
    }

    [Test]
    public async Task Timestamptz_read_values()
    {
        if (DisableDateTimeInfinityConversions)
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
        if (DisableDateTimeInfinityConversions)
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

        if (DisableDateTimeInfinityConversions)
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

        Assert.That(await cmd2.ExecuteScalarAsync(), Is.EqualTo(DisableDateTimeInfinityConversions ? "9999-12-31 23:59:59.999999" : "infinity"));
    }

    [Test]
    public async Task Timestamptz_read()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
            "SELECT '-infinity'::timestamp with time zone, 'infinity'::timestamp with time zone", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (DisableDateTimeInfinityConversions)
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

        // TODO: Switch to use LocalDateTime.MinMaxValue when available (#4061)

        await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
        {
            Parameters = { new() { Value = LocalDate.MinIsoValue + LocalTime.MinValue, NpgsqlDbType = NpgsqlDbType.Timestamp } }
        };

        if (DisableDateTimeInfinityConversions)
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
            Parameters = { new() { Value = LocalDate.MaxIsoValue + LocalTime.MaxValue, NpgsqlDbType = NpgsqlDbType.Timestamp } }
        };

        Assert.That(await cmd2.ExecuteScalarAsync(), Is.EqualTo(DisableDateTimeInfinityConversions
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

        if (DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            // TODO: Switch to use LocalDateTime.MinMaxValue when available (#4061)
            Assert.That(reader[0], Is.EqualTo(LocalDate.MinIsoValue + LocalTime.MinValue));
            Assert.That(reader[1], Is.EqualTo(LocalDate.MaxIsoValue + LocalTime.MaxValue));
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
        if (DisableDateTimeInfinityConversions)
            Assert.That(async () => await cmd.ExecuteScalarAsync(),
                Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.DatetimeFieldOverflow));
        else
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("-infinity"));

        cmd.Parameters[0].Value = LocalDate.MaxIsoValue;

        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(DisableDateTimeInfinityConversions ? "9999-12-31" : "infinity"));
    }

    [Test]
    public async Task Date_read()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT '-infinity'::date, 'infinity'::date", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (DisableDateTimeInfinityConversions)
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
        if (DisableDateTimeInfinityConversions)
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

    protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
    {
        var conn = await base.OpenConnectionAsync(connectionString);
        await conn.ExecuteNonQueryAsync("SET TimeZone='Europe/Berlin'");
        return conn;
    }

    protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        => throw new NotSupportedException();

    public NodaTimeInfinityTests(bool disableDateTimeInfinityConversions)
    {
#if DEBUG
        DisableDateTimeInfinityConversions = disableDateTimeInfinityConversions;
        Statics.DisableDateTimeInfinityConversions = disableDateTimeInfinityConversions;
#else
        if (disableDateTimeInfinityConversions)
        {
            Assert.Ignore(
                "NodaTimeInfinityTests rely on the Npgsql.DisableDateTimeInfinityConversions AppContext switch and can only be run in DEBUG builds");
        }
#endif
    }

    public void Dispose()
    {
#if DEBUG
        DisableDateTimeInfinityConversions = false;
        Statics.DisableDateTimeInfinityConversions = false;
#endif
    }
}
