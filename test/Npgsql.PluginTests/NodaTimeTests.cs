using System;
using System.Data;
using System.Globalization;
using NodaTime;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable AccessToDisposedClosure

namespace Npgsql.PluginTests
{
    public class NodaTimeTests : TestBase
    {
        #region Timestamp

        static readonly TestCaseData[] TimestampCases = {
            new TestCaseData(new LocalDateTime(1998, 4, 12, 13, 26, 38, 789)).SetName(nameof(Timestamp) + "Pre2000"),
            new TestCaseData(new LocalDateTime(2015, 1, 27, 8, 45, 12, 345)).SetName(nameof(Timestamp) + "Post2000"),
            new TestCaseData(new LocalDateTime(1999, 12, 31, 23, 59, 59, 999).PlusNanoseconds(456000)).SetName(nameof(Timestamp) + "Microseconds"),
        };

        [Test, TestCaseSource(nameof(TimestampCases))]
        public void Timestamp(LocalDateTime localDateTime)
        {
            using (var conn = OpenConnection())
            {
                var instant = localDateTime.InUtc().ToInstant();
                var minTimestampPostgres = Instant.FromUtc(-4713, 12, 31, 00, 00, 00);
                var maxTimestampPostgres = Instant.MaxValue;
                var dateTime = new DateTime(2020, 03, 04, 12, 20, 44, 0, DateTimeKind.Utc);

                conn.ExecuteNonQuery("CREATE TEMP TABLE data (d1 TIMESTAMP, d2 TIMESTAMP, d3 TIMESTAMP, d4 TIMESTAMP, d5 TIMESTAMP, d6 TIMESTAMP, d7 TIMESTAMP, d8 TIMESTAMP)");

                using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Timestamp) { Value = instant });
                    cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.DateTime) { Value = instant });
                    cmd.Parameters.Add(new NpgsqlParameter("p3", DbType.DateTime2) { Value = instant });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4", Value = instant });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p5", Value = localDateTime });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p6", Value = minTimestampPostgres });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p7", Value = maxTimestampPostgres });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p8", Value = dateTime });
                    cmd.ExecuteNonQuery();
                }

                // Make sure the values inserted are the good ones, textually
                using (var cmd = new NpgsqlCommand("SELECT d1::TEXT, d2::TEXT, d3::TEXT, d4::TEXT, d5::TEXT FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    for (var i = 0; i < reader.FieldCount; i++)
                        Assert.That(reader.GetValue(i), Is.EqualTo(instant.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'FFFFFF", CultureInfo.InvariantCulture)));
                }

                using (var cmd = new NpgsqlCommand("SELECT d6::TEXT, d7::TEXT, d8::TEXT FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetValue(0), Is.EqualTo("4714-12-31 00:00:00 BC"));
                    Assert.That(reader.GetValue(1), Is.EqualTo(maxTimestampPostgres.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'FFFFFF", CultureInfo.InvariantCulture)));
                    Assert.That(reader.GetValue(2), Is.EqualTo("2020-03-04 12:20:44"));
                }

                using (var cmd = new NpgsqlCommand("SELECT d1, d2, d3, d4, d5 FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Instant)));
                        Assert.That(reader.GetFieldValue<Instant>(i), Is.EqualTo(instant));
                        Assert.That(reader.GetValue(i), Is.EqualTo(instant));
                        Assert.That(reader.GetFieldValue<LocalDateTime>(i), Is.EqualTo(localDateTime));
                        Assert.That(() => reader.GetFieldValue<ZonedDateTime>(i), Throws.TypeOf<InvalidCastException>());
                        Assert.That(() => reader.GetDateTime(i), Is.EqualTo(localDateTime.ToDateTimeUnspecified()));
                        Assert.That(() => reader.GetFieldValue<DateTime>(i), Is.EqualTo(localDateTime.ToDateTimeUnspecified()));
                        Assert.That(() => reader.GetDate(i), Throws.TypeOf<InvalidCastException>());
                    }
                }
            }
        }

        [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
        public void TimestampConvertInfinity()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { ConvertInfinityDateTime = true };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (d1 TIMESTAMP, d2 TIMESTAMP, d3 TIMESTAMP, d4 TIMESTAMP)");

                using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1, @p2, @p3, @p4)", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Timestamp, Instant.MaxValue);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Timestamp, Instant.MinValue);
                    cmd.Parameters.AddWithValue("p3", NpgsqlDbType.Timestamp, DateTime.MaxValue);
                    cmd.Parameters.AddWithValue("p4", NpgsqlDbType.Timestamp, DateTime.MinValue);
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
                    Assert.That(reader.GetFieldValue<Instant>(0), Is.EqualTo(Instant.MaxValue));
                    Assert.That(reader.GetFieldValue<Instant>(1), Is.EqualTo(Instant.MinValue));
                    Assert.That(reader.GetFieldValue<DateTime>(2), Is.EqualTo(DateTime.MaxValue));
                    Assert.That(reader.GetFieldValue<DateTime>(3), Is.EqualTo(DateTime.MinValue));
                }
            }
        }

        #endregion Timestamp

        #region Timestamp with time zone

        [Test]
        public void TimestampTz()
        {
            using (var conn = OpenConnection())
            {
                var timezone = "America/New_York";
                conn.ExecuteNonQuery($"SET TIMEZONE TO '{timezone}'");
                Assert.That(conn.Timezone, Is.EqualTo(timezone));
                // Nodatime provider should return timestamptz's as ZonedDateTime in the session timezone

                var instant = Instant.FromUtc(2015, 6, 27, 8, 45, 12) + Duration.FromMilliseconds(345);
                var utcZonedDateTime = instant.InUtc();
                var localZonedDateTime = utcZonedDateTime.WithZone(DateTimeZoneProviders.Tzdb[timezone]);
                var offsetDateTime = localZonedDateTime.ToOffsetDateTime();
                var dateTimeOffset = offsetDateTime.ToDateTimeOffset();
                var dateTime = dateTimeOffset.DateTime;
                var localDateTime = dateTimeOffset.LocalDateTime;

                conn.ExecuteNonQuery("CREATE TEMP TABLE data (d1 TIMESTAMPTZ, d2 TIMESTAMPTZ, d3 TIMESTAMPTZ, d4 TIMESTAMPTZ, d5 TIMESTAMPTZ, d6 TIMESTAMPTZ)");

                using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p1, @p2, @p3, @p4, @p5, @p6)", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimestampTz) { Value = instant });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = utcZonedDateTime });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3", Value = localZonedDateTime });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p4", Value = offsetDateTime });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p5", Value = dateTimeOffset });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p6", Value = dateTime });
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("SELECT d1::TEXT, d2::TEXT, d3::TEXT, d4::TEXT, d5::TEXT, d6::TEXT FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    // When converting timestamptz as a string as we're doing here, PostgreSQL automatically converts
                    // it to the session timezone
                    for (var i = 0; i < reader.FieldCount; i++)
                        Assert.That(reader.GetValue(i), Is.EqualTo(
                            localZonedDateTime.ToString("uuuu'-'MM'-'dd' 'HH':'mm':'ss'.'fff", CultureInfo.InvariantCulture) + "-04")
                        );
                }

                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Instant)));
                        Assert.That(reader.GetFieldValue<Instant>(i), Is.EqualTo(instant));
                        Assert.That(reader.GetValue(i), Is.EqualTo(instant));
                        Assert.That(reader.GetFieldValue<ZonedDateTime>(i), Is.EqualTo(localZonedDateTime));
                        Assert.That(reader.GetFieldValue<OffsetDateTime>(i), Is.EqualTo(offsetDateTime));
                        Assert.That(reader.GetFieldValue<DateTimeOffset>(i), Is.EqualTo(dateTimeOffset));
                        Assert.That(() => reader.GetFieldValue<LocalDateTime>(i), Throws.TypeOf<InvalidCastException>());
                        Assert.That(() => reader.GetDateTime(i), Is.EqualTo(localDateTime));
                        Assert.That(() => reader.GetDate(i), Throws.TypeOf<InvalidCastException>());
                    }
                }
            }
        }

        #endregion Timestamp with time zone

        #region Date

        [Test]
        public void Date()
        {
            using (var conn = OpenConnection())
            {
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
                }
            }
        }

        [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
        public void DateConvertInfinity()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { ConvertInfinityDateTime = true };
            using (var conn = OpenConnection(csb))
            {
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
        }

        #endregion Date

        #region Time

        [Test]
        public void Time()
        {
            using (var conn = OpenConnection())
            {
                var expected = new LocalTime(1, 2, 3, 4).PlusNanoseconds(5000);
                var timeSpan = new TimeSpan(0, 1, 2, 3, 4).Add(TimeSpan.FromTicks(50));

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Time) { Value = expected });
                    cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Time) { Value = expected });
                    cmd.Parameters.Add(new NpgsqlParameter("p3", DbType.Time) { Value = timeSpan });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(LocalTime)));
                            Assert.That(reader.GetFieldValue<LocalTime>(i), Is.EqualTo(expected));
                            Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                            Assert.That(() => reader.GetTimeSpan(i), Is.EqualTo(timeSpan));
                        }
                    }
                }
            }
        }

        #endregion Time

        #region Time with time zone

        [Test]
        public void TimeTz()
        {
            using (var conn = OpenConnection())
            {
                var time = new LocalTime(1, 2, 3, 4).PlusNanoseconds(5000);
                var offset = Offset.FromHoursAndMinutes(3, 30) + Offset.FromSeconds(5);
                var expected = new OffsetTime(time, offset);
                var dateTimeOffset = new DateTimeOffset(0001, 01, 02, 03, 43, 20, TimeSpan.FromHours(3));
                var dateTime = dateTimeOffset.DateTime;

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimeTz) { Value = expected });
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = expected });
                    cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.TimeTz) { Value = dateTimeOffset });
                    cmd.Parameters.Add(new NpgsqlParameter("p4", dateTimeOffset));
                    cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlDbType.TimeTz) { Value = dateTime });
                    cmd.Parameters.Add(new NpgsqlParameter("p6", dateTime));

                    using (var reader = cmd.ExecuteReader())
                    {
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
                        for (var i = 4; i < 6; i++)
                        {
                            Assert.That(reader.GetFieldValue<DateTime>(i), Is.EqualTo(dateTime));
                        }
                    }
                }
            }
        }

        #endregion Time with time zone

        #region Interval

        [Test]
        public void IntervalAsPeriod()
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

            using var conn = OpenConnection();
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Interval) { Value = expectedPeriod });
                cmd.Parameters.AddWithValue("p2", expectedPeriod);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < 2; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Period)));
                        Assert.That(reader.GetFieldValue<Period>(i), Is.EqualTo(expectedPeriod));
                        Assert.That(reader.GetValue(i), Is.EqualTo(expectedPeriod));
                    }
                }
            }
        }

        [Test]
        public void IntervalAsDuration()
        {
            using var conn = OpenConnection();
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

        [Test]
        public void IntervalAsTimeSpan()
        {
            var expected = new TimeSpan(1, 2, 3, 4, 5);
            using var conn = OpenConnection();
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
        public void IntervalAsDurationWithMonthsFails()
        {
            using var conn = OpenConnection();
            using var cmd = new NpgsqlCommand("SELECT make_interval(months => 2)", conn);
            using var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(() => reader.GetFieldValue<Duration>(0), Throws.Exception.TypeOf<NpgsqlException>().With.Message.EqualTo(
                "Cannot read PostgreSQL interval with non-zero months to NodaTime Duration. Try reading as a NodaTime Period instead."));
        }

        #endregion Interval

        #region Support

        protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        {
            var conn = new NpgsqlConnection(connectionString ?? ConnectionString);
            conn.Open();
            conn.TypeMapper.UseNodaTime();
            return conn;
        }

        #endregion Support
    }
}
