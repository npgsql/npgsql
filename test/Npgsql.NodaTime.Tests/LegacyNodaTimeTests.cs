using System;
using System.Data;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.NodaTime.Tests
{
    // Since this test suite manipulates TimeZone, it is incompatible with multiplexing
    [NonParallelizable]
    public class LegacyNodaTimeTests : TestBase
    {
        static readonly TestCaseData[] TimestampValues =
        {
            new TestCaseData(new LocalDateTime(1998, 4, 12, 13, 26, 38, 789), "1998-04-12 13:26:38.789")
                .SetName("TimestampPre2000"),
            new TestCaseData(new LocalDateTime(2015, 1, 27, 8, 45, 12, 345), "2015-01-27 08:45:12.345")
                .SetName("TimestampPost2000"),
            new TestCaseData(new LocalDateTime(1999, 12, 31, 23, 59, 59, 999).PlusNanoseconds(456000), "1999-12-31 23:59:59.999456")
                .SetName("TimestampMicroseconds"),
        };

        [Test, TestCaseSource(nameof(TimestampValues))]
        public async Task Timestamp_read(LocalDateTime localDateTime, string s)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{s}'::timestamp without time zone", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("timestamp without time zone"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Instant)));

            Assert.That(reader[0], Is.EqualTo(localDateTime.InUtc().ToInstant()));
            Assert.That(reader.GetFieldValue<Instant>(0), Is.EqualTo(localDateTime.InUtc().ToInstant()));
            Assert.That(reader.GetFieldValue<LocalDateTime>(0), Is.EqualTo(localDateTime));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(localDateTime.ToDateTimeUnspecified()));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(localDateTime.ToDateTimeUnspecified()));

            Assert.That(() => reader.GetFieldValue<ZonedDateTime>(0), Throws.TypeOf<InvalidCastException>());
            Assert.That(() => reader.GetDate(0), Throws.TypeOf<InvalidCastException>());
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
                    new() { Value = localDateTime.InUtc().ToInstant() },
                    new() { Value = localDateTime, NpgsqlDbType = NpgsqlDbType.Timestamp },
                    new() { Value = localDateTime, DbType = DbType.DateTime },
                    new() { Value = localDateTime, DbType = DbType.DateTime2 },
                    new() { Value = localDateTime.ToDateTimeUnspecified() },
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

        [Test]
        public async Task Timestamp_read_infinity()
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString) { ConvertInfinityDateTime = true }.ConnectionString;
            await using var conn = await OpenConnectionAsync(connectionString);
            await using var cmd =
                new NpgsqlCommand("SELECT 'infinity'::timestamp without time zone, '-infinity'::timestamp without time zone", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetFieldValue<Instant>(0), Is.EqualTo(Instant.MaxValue));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(DateTime.MaxValue));
            Assert.That(reader.GetFieldValue<Instant>(1), Is.EqualTo(Instant.MinValue));
            Assert.That(reader.GetFieldValue<DateTime>(1), Is.EqualTo(DateTime.MinValue));
        }

        [Test]
        public async Task Timestamp_write_infinity()
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString) { ConvertInfinityDateTime = true }.ConnectionString;
            await using var conn = await OpenConnectionAsync(connectionString);
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

        [Test, TestCaseSource(nameof(TimestampValues))]
        public async Task Timestamptz_read(LocalDateTime expectedLocalDateTime, string s)
        {
            var expectedInstance = expectedLocalDateTime.InUtc().ToInstant();

            await using var conn = await OpenConnectionAsync();
            var timezone = "America/New_York";
            await conn.ExecuteNonQueryAsync($"SET TIMEZONE TO '{timezone}'");

            await using var cmd = new NpgsqlCommand($"SELECT '{s}+00'::timestamp with time zone", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("timestamp with time zone"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Instant)));

            Assert.That(reader[0], Is.EqualTo(expectedInstance));
            Assert.That(reader.GetFieldValue<Instant>(0), Is.EqualTo(expectedInstance));
            Assert.That(reader.GetFieldValue<ZonedDateTime>(0), Is.EqualTo(expectedInstance.InZone(DateTimeZoneProviders.Tzdb[timezone])));
            Assert.That(reader.GetFieldValue<OffsetDateTime>(0), Is.EqualTo(expectedInstance.InZone(DateTimeZoneProviders.Tzdb[timezone]).ToOffsetDateTime()));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expectedInstance.ToDateTimeUtc().ToLocalTime()));
            Assert.That(reader.GetFieldValue<DateTimeOffset>(0), Is.EqualTo(expectedInstance.ToDateTimeOffset().ToLocalTime()));

            Assert.That(() => reader.GetFieldValue<LocalDateTime>(0), Throws.TypeOf<InvalidCastException>());
            Assert.That(() => reader.GetDate(0), Throws.TypeOf<InvalidCastException>());
        }

        [Test, TestCaseSource(nameof(TimestampValues))]
        public async Task Timestamptz_write_values(LocalDateTime localDateTime, string expected)
        {
            await using var conn = await OpenConnectionAsync();
            await conn.ExecuteNonQueryAsync("SET TimeZone='UTC'");
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
            {
                Parameters = { new() { Value = localDateTime.InUtc().ToInstant(), NpgsqlDbType = NpgsqlDbType.TimestampTz} }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected + "+00"));
        }

        static NpgsqlParameter[] TimestamptzParameters
        {
            get
            {
                var localDateTime = new LocalDateTime(1998, 4, 12, 13, 26, 38);
                var instance = localDateTime.InUtc().ToInstant();

                return new NpgsqlParameter[]
                {
                    new() { Value = instance, NpgsqlDbType = NpgsqlDbType.TimestampTz },
                    new() { Value = instance, DbType = DbType.DateTimeOffset },
                    new() { Value = instance.InUtc() },
                    new() { Value = instance.WithOffset(Offset.Zero) },
                    new() { Value = instance.ToDateTimeOffset() },

                    // In legacy mode we support non-UTC ZonedDateTime and OffsetDateTime
                    new() { Value = instance.InZone(DateTimeZoneProviders.Tzdb["America/New_York"]), NpgsqlDbType = NpgsqlDbType.TimestampTz },
                    new() { Value = instance.WithOffset(Offset.FromHours(1)), NpgsqlDbType = NpgsqlDbType.TimestampTz }
                };
            }
        }

        [Test, TestCaseSource(nameof(TimestamptzParameters))]
        public async Task Timestamptz_resolution(NpgsqlParameter parameter)
        {
            await using var conn = await OpenConnectionAsync();
            await conn.ExecuteNonQueryAsync("SET TimeZone='UTC'");
            conn.TypeMapper.Reset();
            conn.TypeMapper.UseNodaTime();

            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters = { parameter }
            };

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("timestamp with time zone"));
            Assert.That(reader[1], Is.EqualTo("1998-04-12 13:26:38+00"));
        }

        [Test]
        public async Task Timestamptz_read_infinity()
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString) { ConvertInfinityDateTime = true }.ConnectionString;
            await using var conn = await OpenConnectionAsync(connectionString);
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
        public async Task Timestamptz_write_infinity()
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString) { ConvertInfinityDateTime = true }.ConnectionString;
            await using var conn = await OpenConnectionAsync(connectionString);
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
        public async Task TimeTz()
        {
            await using var conn = await OpenConnectionAsync();
            var time = new LocalTime(1, 2, 3, 4).PlusNanoseconds(5000);
            var offset = Offset.FromHoursAndMinutes(3, 30) + Offset.FromSeconds(5);
            var expected = new OffsetTime(time, offset);
            var dateTimeOffset = new DateTimeOffset(0001, 01, 02, 03, 43, 20, TimeSpan.FromHours(3));
            var dateTime = dateTimeOffset.DateTime;

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimeTz) { Value = expected });
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = expected });
            cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.TimeTz) { Value = dateTimeOffset });
            cmd.Parameters.Add(new NpgsqlParameter("p4", dateTimeOffset));

            using var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < 2; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(OffsetTime)));
                Assert.That(reader.GetFieldValue<OffsetTime>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            }
            for (var i = 2; i < 4; i++)
            {
                Assert.That(reader.GetFieldValue<DateTimeOffset>(i), Is.EqualTo(dateTimeOffset));
            }
        }

        #region Support

        protected override async ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
        {
            var conn = new NpgsqlConnection(connectionString ?? ConnectionString);
            await conn.OpenAsync();
            conn.TypeMapper.UseNodaTime();
            return conn;
        }

        protected override NpgsqlConnection OpenConnection(string? connectionString = null)
            => throw new NotSupportedException();

        [OneTimeSetUp]
        public void Setup()
        {
#if DEBUG
            Internal.NodaTimeUtils.LegacyTimestampBehavior = true;
            Util.Statics.LegacyTimestampBehavior = true;
#else
            Assert.Ignore(
                "Legacy NodaTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif

        }

        [OneTimeTearDown]
        public void Teardown()
        {
#if DEBUG
            Internal.NodaTimeUtils.LegacyTimestampBehavior = false;
            Util.Statics.LegacyTimestampBehavior = false;
#endif
        }

        #endregion Support
    }
}
