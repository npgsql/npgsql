using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using static Npgsql.Tests.TestUtil;

#pragma warning disable 618 // NpgsqlDateTime, NpgsqlDate and NpgsqlTimespan are obsolete, remove in 7.0

namespace Npgsql.Tests.Types
{
    // Since this test suite manipulates TimeZone, it is incompatible with multiplexing
    public class DateTimeTests : TestBase
    {
        #region Date

        [Test]
        public async Task Date()
        {
            using var conn = await OpenConnectionAsync();
            var dateTime = new DateTime(2002, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified);
            var npgsqlDate = new NpgsqlDate(dateTime);

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Date) {Value = npgsqlDate};
            var p2 = new NpgsqlParameter {ParameterName = "p2", Value = npgsqlDate};
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Date));
            Assert.That(p2.DbType, Is.EqualTo(DbType.Date));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                // Regular type (DateTime)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetDateTime(i), Is.EqualTo(dateTime));
                Assert.That(reader.GetFieldValue<DateTime>(i), Is.EqualTo(dateTime));
                Assert.That(reader[i], Is.EqualTo(dateTime));
                Assert.That(reader.GetValue(i), Is.EqualTo(dateTime));

                // Provider-specific type (NpgsqlDate)
                Assert.That(reader.GetDate(i), Is.EqualTo(npgsqlDate));
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlDate)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(npgsqlDate));
                Assert.That(reader.GetFieldValue<NpgsqlDate>(i), Is.EqualTo(npgsqlDate));

                // Internal PostgreSQL representation, for out-of-range values.
                Assert.That(() => reader.GetInt32(0), Throws.Nothing);
            }
        }

#if NET6_0_OR_GREATER
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

            // Provider-specific type (NpgsqlTimeStamp)
            var npgsqlDateTime = new NpgsqlDateTime(dateTime);
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlDateTime)));
            Assert.That(reader.GetTimeStamp(0), Is.EqualTo(npgsqlDateTime));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(npgsqlDateTime));
            Assert.That(reader.GetFieldValue<NpgsqlDateTime>(0), Is.EqualTo(npgsqlDateTime));

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
                    new() { Value = new NpgsqlDateTime(dateTime.Ticks, DateTimeKind.Unspecified) },
                    new() { Value = new NpgsqlDateTime(dateTime.Ticks, DateTimeKind.Local) },
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
                new() { Value = new NpgsqlDateTime(0, DateTimeKind.Utc), NpgsqlDbType = NpgsqlDbType.Timestamp },
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

            // Provider-specific type (NpgsqlTimeStamp)
            var npgsqlDateTime = new NpgsqlDateTime(dateTime.Ticks, DateTimeKind.Utc);
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlDateTime)));
            Assert.That(reader.GetTimeStamp(0), Is.EqualTo(npgsqlDateTime));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(npgsqlDateTime));
            Assert.That(reader.GetFieldValue<NpgsqlDateTime>(0), Is.EqualTo(npgsqlDateTime));
            Assert.That(reader.GetTimeStamp(0).Kind, Is.EqualTo(DateTimeKind.Utc));

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
            new TestCaseData(NpgsqlDateTime.Infinity, "infinity")
                .SetName("TimestampTzNpgsqlDateTimeInfinity"),
            new TestCaseData(NpgsqlDateTime.NegativeInfinity, "-infinity")
                .SetName("TimestampTzNpgsqlDateTimeNegativeInfinity"),
            new TestCaseData(new NpgsqlDateTime(-5, 3, 3, 1, 0, 0, DateTimeKind.Utc), "0005-03-03 01:00:00 BC")
                .SetName("TimestampTzBC"),
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
                    new() { Value = new NpgsqlDateTime(dateTime.Ticks, DateTimeKind.Utc), NpgsqlDbType = NpgsqlDbType.TimestampTz },
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
                new() { Value = new NpgsqlDateTime(0, DateTimeKind.Unspecified), NpgsqlDbType = NpgsqlDbType.TimestampTz },
                new() { Value = new NpgsqlDateTime(0, DateTimeKind.Local), NpgsqlDbType = NpgsqlDbType.TimestampTz },
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

        [Test]
        public async Task Interval()
        {
            using var conn = await OpenConnectionAsync();
            var expectedNpgsqlTimeSpan = new NpgsqlTimeSpan(1, 2, 3, 4, 5);
            var expectedTimeSpan = new TimeSpan(1, 2, 3, 4, 5);
            var expectedNpgsqlInterval = new NpgsqlInterval(0, 1, 7384005000);

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Interval);
            var p2 = new NpgsqlParameter("p2", expectedTimeSpan);
            var p3 = new NpgsqlParameter("p3", expectedNpgsqlTimeSpan);
            var p4 = new NpgsqlParameter("p4", expectedNpgsqlInterval);
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p2.DbType, Is.EqualTo(DbType.Object));
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Object));
            Assert.That(p4.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p4.DbType, Is.EqualTo(DbType.Object));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            p1.Value = expectedNpgsqlTimeSpan;

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

                // Provider-specific type (NpgsqlInterval)
                Assert.That(reader.GetInterval(i), Is.EqualTo(expectedNpgsqlTimeSpan));
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlTimeSpan)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(expectedNpgsqlTimeSpan));
                Assert.That(reader.GetFieldValue<NpgsqlTimeSpan>(i), Is.EqualTo(expectedNpgsqlTimeSpan));

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
