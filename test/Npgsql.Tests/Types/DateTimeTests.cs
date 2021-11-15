using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types
{
    // Since this test suite manipulates TimeZone, it is incompatible with multiplexing
    public class DateTimeTests : TestBase
    {
        #region Date

        [Test]
        public async Task Date_read()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT '2020-10-01'::date", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("date"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));

            var expected = new DateTime(2020, 10, 1);
            Assert.That(reader[0], Is.EqualTo(expected));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(expected));
            Assert.That(reader.GetDateTime(0).Kind, Is.EqualTo(DateTimeKind.Unspecified));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expected));

            // Internal PostgreSQL representation, for out-of-range values.
            Assert.That(() => reader.GetInt32(0), Throws.Nothing);
        }

        [Test]
        public async Task Date_write_values()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
            {
                Parameters = { new() { Value = new DateTime(2020, 10, 1), NpgsqlDbType = NpgsqlDbType.Date } }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("2020-10-01"));
        }

#if NET6_0_OR_GREATER
        [Test]
        public async Task Date_DateOnly_read()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT '2020-10-01'::date", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("date"));
            Assert.That(reader.GetFieldValue<DateOnly>(0), Is.EqualTo(new DateOnly(2020, 10, 1)));
        }

        [Test]
        public async Task Date_DateOnly_write_values()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
            {
                Parameters = { new() { Value = new DateOnly(2020, 10, 1), NpgsqlDbType = NpgsqlDbType.Date } }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("2020-10-01"));
        }

        [Test]
        public async Task Date_DateOnly()
        {
            using var conn = await OpenConnectionAsync();
            var dateOnly = new DateOnly(2002, 3, 4);
            var dateTime = dateOnly.ToDateTime(default);

            using var cmd = new NpgsqlCommand("SELECT @p1", conn);
            var p1 = new NpgsqlParameter { ParameterName = "p1", Value = dateOnly };
            Assert.That(p1.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Date));
            Assert.That(p1.DbType, Is.EqualTo(DbType.Date));
            cmd.Parameters.Add(p1);

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetFieldValue<DateOnly>(0), Is.EqualTo(dateOnly));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(dateTime));
            Assert.That(reader[0], Is.EqualTo(dateTime));
            Assert.That(reader.GetValue(0), Is.EqualTo(dateTime));
        }

        [Test]
        public async Task Date_DateOnly_range()
        {
            using var conn = await OpenConnectionAsync();
            var range = new NpgsqlRange<DateOnly>(new(2002, 3, 4), true, new(2002, 3, 6), false);

            using var cmd = new NpgsqlCommand("SELECT @p1", conn);
            var p1 = new NpgsqlParameter { ParameterName = "p1", Value = range };
            Assert.That(p1.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.DateRange));
            Assert.That(p1.DbType, Is.EqualTo(DbType.Object));
            cmd.Parameters.Add(p1);

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetFieldValue<NpgsqlRange<DateOnly>>(0), Is.EqualTo(range));
        }
#endif

        #endregion

        #region Time

        [Test]
        public async Task Time()
        {
            using var conn = await OpenConnectionAsync();
            var expected = new TimeSpan(0, 10, 45, 34, 500);

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Time) {Value = expected});
            cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Time) {Value = expected});
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(TimeSpan)));
                Assert.That(reader.GetTimeSpan(i), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<TimeSpan>(i), Is.EqualTo(expected));
                Assert.That(reader[i], Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            }
        }

#if NET6_0_OR_GREATER
        [Test]
        public async Task Time_TimeOnly()
        {
            using var conn = await OpenConnectionAsync();
            var timeOnly = new TimeOnly(10, 45, 34, 500);
            var timeSpan = timeOnly.ToTimeSpan();

            using var cmd = new NpgsqlCommand("SELECT @p1", conn);
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p1", Value = timeOnly });

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetFieldValue<TimeOnly>(0), Is.EqualTo(timeOnly));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(TimeSpan)));
            Assert.That(reader.GetTimeSpan(0), Is.EqualTo(timeSpan));
            Assert.That(reader.GetFieldValue<TimeSpan>(0), Is.EqualTo(timeSpan));
            Assert.That(reader[0], Is.EqualTo(timeSpan));
            Assert.That(reader.GetValue(0), Is.EqualTo(timeSpan));
        }
#endif

        #endregion

        #region Time with timezone

        [Test]
        [MonoIgnore]
        public async Task TimeTz()
        {
            using var conn = await OpenConnectionAsync();
            var tzOffset = TimeZoneInfo.Local.BaseUtcOffset;
            if (tzOffset == TimeSpan.Zero)
                Assert.Ignore("Test cannot run when machine timezone is UTC");

            // Note that the date component of the below is ignored
            var dto = new DateTimeOffset(5, 5, 5, 13, 3, 45, 510, tzOffset);

            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            cmd.Parameters.AddWithValue("p", NpgsqlDbType.TimeTz, dto);
            Assert.That(cmd.Parameters.All(p => p.DbType == DbType.Object));

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTimeOffset)));
                Assert.That(reader.GetFieldValue<DateTimeOffset>(i), Is.EqualTo(new DateTimeOffset(1, 1, 2, dto.Hour, dto.Minute, dto.Second, dto.Millisecond, dto.Offset)));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTimeOffset)));
            }
        }

        [Test]
        public async Task TimeTz_before_utc_zero()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT TIME WITH TIME ZONE '01:00:00+02'", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<DateTimeOffset>(0), Is.EqualTo(new DateTimeOffset(1, 1, 2, 1, 0, 0, new TimeSpan(0, 2, 0, 0))));
        }

        #endregion

        #region Timestamp

        static readonly TestCaseData[] TimestampValues =
        {
            new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Unspecified), "1998-04-12 13:26:38")
                .SetName("TimestampPre2000"),
            new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Unspecified), "2015-01-27 08:45:12.345")
                .SetName("TimestampPost2000"),
            new TestCaseData(new DateTime(2013, 7, 25, 0, 0, 0, DateTimeKind.Unspecified), "2013-07-25 00:00:00")
                .SetName("TimestampDateOnly")
        };

        [Test, TestCaseSource(nameof(TimestampValues))]
        public async Task Timestamp_read(DateTime dateTime, string s)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{s}'::timestamp without time zone", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("timestamp without time zone"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));

            Assert.That(reader[0], Is.EqualTo(dateTime));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(dateTime));
            Assert.That(reader.GetDateTime(0).Kind, Is.EqualTo(DateTimeKind.Unspecified));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(dateTime));

            // DateTimeOffset
            Assert.That(() => reader.GetFieldValue<DateTimeOffset>(0), Throws.Exception.TypeOf<InvalidCastException>());

            // Internal PostgreSQL representation, for out-of-range values.
            Assert.That(() => reader.GetInt64(0), Throws.Nothing);
        }

        [Test, TestCaseSource(nameof(TimestampValues))]
        public async Task Timestamp_write_values(DateTime dateTime, string expected)
        {
            Assert.That(dateTime.Kind, Is.EqualTo(DateTimeKind.Unspecified));

            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
            {
                Parameters =
                {
                    new() { Value = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), NpgsqlDbType = NpgsqlDbType.Timestamp }
                }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected));
        }

        static NpgsqlParameter[] TimestampParameters
        {
            get
            {
                var dateTime = new DateTime(1998, 4, 12, 13, 26, 38);

                return new NpgsqlParameter[]
                {
                    new() { Value = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified) },
                    new() { Value = DateTime.SpecifyKind(dateTime, DateTimeKind.Local) },
                    new() { Value = DateTime.SpecifyKind(dateTime, DateTimeKind.Local), NpgsqlDbType = NpgsqlDbType.Timestamp },
                    new() { Value = DateTime.SpecifyKind(dateTime, DateTimeKind.Local), DbType = DbType.DateTime2 },
                    new() { Value = -54297202000000L, NpgsqlDbType = NpgsqlDbType.Timestamp }
                };
            }
        }

        [Test, TestCaseSource(nameof(TimestampParameters))]
        public async Task Timestamp_resolution(NpgsqlParameter parameter)
        {
            await using var conn = await OpenConnectionAsync();
            conn.TypeMapper.Reset();

            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters = { parameter }
            };

            Assert.That(parameter.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
            Assert.That(parameter.DbType, Is.EqualTo(DbType.DateTime).Or.EqualTo(DbType.DateTime2));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("timestamp without time zone"));
            Assert.That(reader[1], Is.EqualTo("1998-04-12 13:26:38"));
        }

        static NpgsqlParameter[] TimestampInvalidParameters
            => new NpgsqlParameter[]
            {
                new() { Value = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc), NpgsqlDbType = NpgsqlDbType.Timestamp },
                new() { Value = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero), NpgsqlDbType = NpgsqlDbType.Timestamp }
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

        [Test]
        public async Task Timestamp_array_resolution()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters = { new() { Value = new[] { new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local) } } }
            };

            Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("timestamp without time zone[]"));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Timestamp));
            Assert.That(cmd.Parameters[0].DbType, Is.EqualTo(DbType.Object));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("timestamp without time zone[]"));
            Assert.That(reader[1], Is.EqualTo(@"{""1998-04-12 13:26:38""}"));
        }

        [Test]
        public async Task Timestamp_range_resolution()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new NpgsqlRange<DateTime>(
                            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local),
                            new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local))
                    }
                }
            };

            Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("tsrange"));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Range | NpgsqlDbType.Timestamp));
            Assert.That(cmd.Parameters[0].DbType, Is.EqualTo(DbType.Object));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("tsrange"));
            Assert.That(reader[1], Is.EqualTo(@"[""1998-04-12 13:26:38"",""1998-04-12 15:26:38""]"));
        }

        [Test]
        public async Task Timestamp_multirange_resolution()
        {
            await using var conn = await OpenConnectionAsync();
            MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new[]
                        {
                            new NpgsqlRange<DateTime>(
                                new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local),
                                new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Local)),
                            new NpgsqlRange<DateTime>(
                                new DateTime(1998, 4, 13, 13, 26, 38, DateTimeKind.Local),
                                new DateTime(1998, 4, 13, 15, 26, 38, DateTimeKind.Local)),
                        }
                    }
                }
            };

            Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("tsmultirange"));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Multirange | NpgsqlDbType.Timestamp));
            Assert.That(cmd.Parameters[0].DbType, Is.EqualTo(DbType.Object));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("tsmultirange"));
            Assert.That(reader[1], Is.EqualTo(@"{[""1998-04-12 13:26:38"",""1998-04-12 15:26:38""],[""1998-04-13 13:26:38"",""1998-04-13 15:26:38""]}"));
        }

        #endregion

        #region Timestamp with timezone

        static readonly TestCaseData[] TimestampTzReadValues =
        {
            new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc), "1998-04-12 13:26:38+00")
                .SetName("TimestampPre2000"),
            new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Utc), "2015-01-27 08:45:12.345+00")
                .SetName("TimestampPost2000"),
            new TestCaseData(new DateTime(2013, 7, 25, 0, 0, 0, DateTimeKind.Utc), "2013-07-25 00:00:00+00")
                .SetName("TimestampDateOnly")
        };

        [Test, TestCaseSource(nameof(TimestampTzReadValues))]
        public async Task Timestamptz_read(DateTime dateTime, string s)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand($"SELECT '{s}'::timestamp with time zone", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("timestamp with time zone"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));

            Assert.That(reader[0], Is.EqualTo(dateTime));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(dateTime));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(dateTime));
            Assert.That(reader.GetDateTime(0).Kind, Is.EqualTo(DateTimeKind.Utc));

            // DateTimeOffset
            Assert.That(reader.GetFieldValue<DateTimeOffset>(0), Is.EqualTo(new DateTimeOffset(dateTime, TimeSpan.Zero)));
            Assert.That(reader.GetFieldValue<DateTimeOffset>(0).Offset, Is.EqualTo(TimeSpan.Zero));

            // Internal PostgreSQL representation, for out-of-range values.
            Assert.That(() => reader.GetInt64(0), Throws.Nothing);
        }

        static readonly TestCaseData[] TimestampTzWriteValues =
        {
            new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc), "1998-04-12 13:26:38")
                .SetName("TimestampTzPre2000"),
            new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345, DateTimeKind.Utc), "2015-01-27 08:45:12.345")
                .SetName("TimestampTzPost2000"),
            new TestCaseData(new DateTime(2013, 7, 25, 0, 0, 0, DateTimeKind.Utc), "2013-07-25 00:00:00")
                .SetName("TimestampTzDateOnly"),
            new TestCaseData(DateTime.MinValue, "-infinity")
                .SetName("TimestampNegativeInfinity"),
            new TestCaseData(DateTime.MaxValue, "infinity")
                .SetName("TimestampInfinity")
        };

        [Test, TestCaseSource(nameof(TimestampTzWriteValues))]
        public async Task Timestamptz_write_values(object dateTime, string expected)
        {
            await using var conn = await OpenConnectionAsync();

            // PG sends local timestamptz *text* representations (according to TimeZone). Convert to a timestamp without time zone at UTC
            // for sensible assertions.
            await using var cmd = new NpgsqlCommand("SELECT ($1 AT TIME ZONE 'UTC')::text", conn)
            {
                Parameters = { new() { Value = dateTime, NpgsqlDbType = NpgsqlDbType.TimestampTz } }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected));
        }

        static NpgsqlParameter[] TimestamptzParameters
        {
            get
            {
                var dateTime = new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc);

                return new NpgsqlParameter[]
                {
                    new() { Value = dateTime },
                    new() { Value = dateTime, NpgsqlDbType = NpgsqlDbType.TimestampTz },
                    new() { Value = new DateTimeOffset(dateTime) },
                    new() { Value = -54297202000000L, NpgsqlDbType = NpgsqlDbType.TimestampTz }
                };
            }
        }

        [Test, TestCaseSource(nameof(TimestamptzParameters))]
        public async Task Timestamptz_resolution(NpgsqlParameter parameter)
        {
            await using var conn = await OpenConnectionAsync();
            conn.TypeMapper.Reset();
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters = { parameter }
            };

            Assert.That(parameter.DataTypeName, Is.EqualTo("timestamp with time zone"));
            Assert.That(parameter.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.TimestampTz));
            Assert.That(parameter.DbType, Is.EqualTo(DbType.DateTime).Or.EqualTo(DbType.DateTimeOffset));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("timestamp with time zone"));
            Assert.That(reader[1], Is.EqualTo("1998-04-12 15:26:38+02"));
        }

        static NpgsqlParameter[] TimestamptzInvalidParameters
            => new NpgsqlParameter[]
            {
                new() { Value = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), NpgsqlDbType = NpgsqlDbType.TimestampTz },
                new() { Value = DateTime.Now, NpgsqlDbType = NpgsqlDbType.TimestampTz },
                new() { Value = new DateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), TimeSpan.FromHours(2)) }
            };

        [Test, TestCaseSource(nameof(TimestamptzInvalidParameters))]
        public async Task Timestamptz_resolution_failure(NpgsqlParameter parameter)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
            {
                Parameters = { parameter }
            };

            Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidCastException>());
        }

        [Test]
        public async Task Timestamptz_array_resolution()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters = { new() { Value = new[] { new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc) } } }
            };

            Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("timestamp with time zone[]"));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.TimestampTz));
            Assert.That(cmd.Parameters[0].DbType, Is.EqualTo(DbType.Object));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("timestamp with time zone[]"));
            Assert.That(reader[1], Is.EqualTo(@"{""1998-04-12 15:26:38+02""}"));
        }

        [Test]
        public async Task Timestamptz_range_resolution()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new NpgsqlRange<DateTime>(
                            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                            new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc))
                    }
                }
            };

            Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("tstzrange"));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Range | NpgsqlDbType.TimestampTz));
            Assert.That(cmd.Parameters[0].DbType, Is.EqualTo(DbType.Object));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("tstzrange"));
            Assert.That(reader[1], Is.EqualTo(@"[""1998-04-12 15:26:38+02"",""1998-04-12 17:26:38+02""]"));
        }

        [Test]
        public async Task Timestamptz_multirange_resolution()
        {
            await using var conn = await OpenConnectionAsync();
            MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
            await using var cmd = new NpgsqlCommand("SELECT pg_typeof($1)::text, $1::text", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new[]
                        {
                            new NpgsqlRange<DateTime>(
                                new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                                new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc)),
                            new NpgsqlRange<DateTime>(
                                new DateTime(1998, 4, 13, 13, 26, 38, DateTimeKind.Utc),
                                new DateTime(1998, 4, 13, 15, 26, 38, DateTimeKind.Utc)),
                        }
                    }
                }
            };

            Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("tstzmultirange"));
            Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Multirange | NpgsqlDbType.TimestampTz));
            Assert.That(cmd.Parameters[0].DbType, Is.EqualTo(DbType.Object));

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo("tstzmultirange"));
            Assert.That(reader[1], Is.EqualTo(@"{[""1998-04-12 15:26:38+02"",""1998-04-12 17:26:38+02""],[""1998-04-13 15:26:38+02"",""1998-04-13 17:26:38+02""]}"));
        }

        [Test]
        public async Task Cannot_mix_DateTime_Kinds_in_array()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new[]
                        {
                            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local),
                        }
                    }
                }
            };

            Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<Exception>());
        }

        [Test]
        public async Task Cannot_mix_DateTime_Kinds_in_range()
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new NpgsqlRange<DateTime>(
                            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                            new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Local))
                    }
                }
            };

            Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidCastException>());
        }

        [Test]
        public async Task Cannot_mix_DateTime_Kinds_in_multirange()
        {
            await using var conn = await OpenConnectionAsync();
            MinimumPgVersion(conn, "14.0", "Multirange types were introduced in PostgreSQL 14");
            await using var cmd = new NpgsqlCommand("SELECT $1", conn)
            {
                Parameters =
                {
                    new()
                    {
                        Value = new[]
                        {
                            new NpgsqlRange<DateTime>(
                                new DateTime(1998, 4, 12, 13, 26, 38, DateTimeKind.Utc),
                                new DateTime(1998, 4, 12, 15, 26, 38, DateTimeKind.Utc)),
                            new NpgsqlRange<DateTime>(
                                new DateTime(1998, 4, 13, 13, 26, 38, DateTimeKind.Local),
                                new DateTime(1998, 4, 13, 15, 26, 38, DateTimeKind.Local)),
                        }
                    }
                }
            };

            Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<InvalidCastException>());
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

        static readonly TestCaseData[] IntervalReadValues =
        {
            new TestCaseData(new TimeSpan(0, 2, 3, 4, 5), "02:03:04.005")
                .SetName("TimeOnly"),
            new TestCaseData(new TimeSpan(1, 2, 3, 4, 5), "1 day 02:03:04.005")
                .SetName("WithDay"),
            new TestCaseData(new TimeSpan(61, 2, 3, 4, 5), "61 days 02:03:04.005")
                .SetName("WithManyDays"),
            new TestCaseData(new TimeSpan(new TimeSpan(2, 3, 4).Ticks + 10), "02:03:04.000001")
                .SetName("WithMicrosecond")
        };

        [Test, TestCaseSource(nameof(IntervalReadValues))]
        public async Task Interval_read(TimeSpan timeSpan, string s)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand($"SELECT '{s}'::interval", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("interval"));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(TimeSpan)));

            Assert.That(reader[0], Is.EqualTo(timeSpan));
            Assert.That(reader.GetTimeSpan(0), Is.EqualTo(timeSpan));
            Assert.That(reader.GetFieldValue<TimeSpan>(0), Is.EqualTo(timeSpan));

            // Internal PostgreSQL representation, for out-of-range values.
            var expectedNpgsqlInterval = new NpgsqlInterval(
                0,
                timeSpan.Days,
                (timeSpan.Ticks / 10) - timeSpan.Days * (TimeSpan.TicksPerDay / 10));

            Assert.That(() => reader.GetFieldValue<NpgsqlInterval>(0), Is.EqualTo(expectedNpgsqlInterval));
        }

        static readonly TestCaseData[] IntervalWriteValues =
        {
            new TestCaseData(new TimeSpan(0, 2, 3, 4, 5), "02:03:04.005")
                .SetName("TimeOnly"),
            new TestCaseData(new TimeSpan(1, 2, 3, 4, 5), "1 day 02:03:04.005")
                .SetName("WithDay"),
            new TestCaseData(new TimeSpan(61, 2, 3, 4, 5), "61 days 02:03:04.005")
                .SetName("WithManyDays"),
            new TestCaseData(new TimeSpan(new TimeSpan(2, 3, 4).Ticks + 10), "02:03:04.000001")
                .SetName("WithMicrosecond"),
            new TestCaseData(new TimeSpan(new TimeSpan(2, 3, 4).Ticks + 1), "02:03:04")  // Truncate
                .SetName("WithTick")
        };

        [Test, TestCaseSource(nameof(IntervalWriteValues))]
        public async Task Interval_write_values(TimeSpan timeSpan, string expected)
        {
            await using var conn = await OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("SELECT $1::text", conn)
            {
                Parameters = { new() { Value = timeSpan, NpgsqlDbType = NpgsqlDbType.Interval } }
            };

            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected));
        }

        [Test]
        public async Task Interval()
        {
            using var conn = await OpenConnectionAsync();
            var expectedTimeSpan = new TimeSpan(1, 2, 3, 4, 5);
            var expectedNpgsqlInterval = new NpgsqlInterval(0, 1, 7384005000);

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Interval);
            var p2 = new NpgsqlParameter("p2", expectedTimeSpan);
            var p3 = new NpgsqlParameter("p3", expectedNpgsqlInterval);
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p2.DbType, Is.EqualTo(DbType.Object));
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Object));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = expectedTimeSpan;

            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                // Regular type (TimeSpan)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(TimeSpan)));
                Assert.That(reader.GetTimeSpan(i), Is.EqualTo(expectedTimeSpan));
                Assert.That(reader.GetFieldValue<TimeSpan>(i), Is.EqualTo(expectedTimeSpan));
                Assert.That(reader[i], Is.EqualTo(expectedTimeSpan));
                Assert.That(reader.GetValue(i), Is.EqualTo(expectedTimeSpan));

                // Internal PostgreSQL representation, for out-of-range values.
                Assert.That(() => reader.GetFieldValue<NpgsqlInterval>(i), Is.EqualTo(expectedNpgsqlInterval));
            }
        }

        [Test]
        public async Task Interval_with_months_cannot_read_as_TimeSpan()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT '1 month 2 days'::interval", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            Assert.That(() => reader.GetTimeSpan(0), Throws.Exception.TypeOf<InvalidCastException>());
        }

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
}
