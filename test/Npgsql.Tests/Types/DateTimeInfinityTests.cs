using System;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Util.Statics;

namespace Npgsql.Tests.Types;

[TestFixture(true)]
#if DEBUG
[TestFixture(false)]
#endif
[NonParallelizable]
public class DateTimeInfinityTests : TestBase, IDisposable
{
    [Test]
    public async Task TimestampTz_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig("SELECT ($1 AT TIME ZONE 'UTC')::text", conn)
        {
            Parameters =
            {
                new() { Value = DateTime.MinValue, NpgsqlDbType = NpgsqlDbType.TimestampTz },
            }
        };

        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(DisableDateTimeInfinityConversions ? "0001-01-01 00:00:00" : "-infinity"));

        cmd.Parameters[0].Value = DateTime.MaxValue;

        if (DisableDateTimeInfinityConversions)
            Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<InvalidCastException>());
        else
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("infinity"));
    }

    [Test]
    public async Task TimestampTz_read()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig(
            "SELECT '-infinity'::timestamp with time zone, 'infinity'::timestamp with time zone",
            conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(DateTime.MinValue));
            Assert.That(reader[1], Is.EqualTo(DateTime.MaxValue));
        }
    }

    [Test]
    public async Task Timestamp_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text, $2::text", conn)
        {
            Parameters =
            {
                new() { Value = DateTime.MinValue, NpgsqlDbType = NpgsqlDbType.Timestamp },
                new() { Value = DateTime.MaxValue, NpgsqlDbType = NpgsqlDbType.Timestamp },
            }
        };
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            await reader.ReadAsync();

            Assert.That(reader[0], Is.EqualTo(DisableDateTimeInfinityConversions ? "0001-01-01 00:00:00" : "-infinity"));
            Assert.That(reader[1], Is.EqualTo(DisableDateTimeInfinityConversions ? "9999-12-31 23:59:59.999999" : "infinity"));
        }
    }

    [Test]
    public async Task Timestamp_read()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig(
            "SELECT '-infinity'::timestamp without time zone, 'infinity'::timestamp without time zone",
            conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(DateTime.MinValue));
            Assert.That(reader[1], Is.EqualTo(DateTime.MaxValue));
        }
    }

    [Test, NonParallelizable]
    public async Task Date_DateTime_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text, $2::text", conn)
        {
            Parameters =
            {
                new() { Value = DateTime.MinValue, NpgsqlDbType = NpgsqlDbType.Date },
                new() { Value = DateTime.MaxValue, NpgsqlDbType = NpgsqlDbType.Date }
            }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader[0], Is.EqualTo(DisableDateTimeInfinityConversions ? "0001-01-01" : "-infinity"));
        Assert.That(reader[1], Is.EqualTo(DisableDateTimeInfinityConversions ? "9999-12-31" : "infinity"));
    }

    [Test]
    public async Task Date_DateTime_read()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig("SELECT '-infinity'::date, 'infinity'::date", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader[0], Is.EqualTo(DateTime.MinValue));
            Assert.That(reader[1], Is.EqualTo(DateTime.MaxValue));
        }
    }

#if NET6_0_OR_GREATER
    [Test]
    public async Task Date_DateOnly_write()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig("SELECT $1::text, $2::text", conn)
        {
            Parameters =
            {
                new() { Value = DateOnly.MinValue, NpgsqlDbType = NpgsqlDbType.Date },
                new() { Value = DateOnly.MaxValue, NpgsqlDbType = NpgsqlDbType.Date }
            }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader[0], Is.EqualTo(DisableDateTimeInfinityConversions ? "0001-01-01" : "-infinity"));
        Assert.That(reader[1], Is.EqualTo(DisableDateTimeInfinityConversions ? "9999-12-31" : "infinity"));
    }

    [Test]
    public async Task Date_DateOnly_read()
    {
        await using var conn = await OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig("SELECT '-infinity'::date, 'infinity'::date", conn);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        if (DisableDateTimeInfinityConversions)
        {
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader[1], Throws.Exception.TypeOf<InvalidCastException>());
        }
        else
        {
            Assert.That(reader.GetFieldValue<DateOnly>(0), Is.EqualTo(DateOnly.MinValue));
            Assert.That(reader.GetFieldValue<DateOnly>(1), Is.EqualTo(DateOnly.MaxValue));
        }
    }
#endif

    public DateTimeInfinityTests(bool disableDateTimeInfinityConversions)
    {
#if DEBUG
        DisableDateTimeInfinityConversions = disableDateTimeInfinityConversions;
#else
        if (disableDateTimeInfinityConversions)
        {
            Assert.Ignore(
                "DateTimeInfinityTests rely on the Npgsql.DisableDateTimeInfinityConversions AppContext switch and can only be run in DEBUG builds");
        }
#endif
    }

    public void Dispose()
    {
#if DEBUG
        DisableDateTimeInfinityConversions = false;
#endif
    }
}
