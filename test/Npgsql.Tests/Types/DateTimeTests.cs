using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL date/time types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    public class DateTimeTests : MultiplexingTestBase
    {
        #region Date

        [Test]
        public async Task Date()
        {
            await using var conn = await OpenConnectionAsync();
            var dateTime = new DateTime(2002, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified);
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            var npgsqlDate = new NpgsqlDate(dateTime);
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

            using var cmd = new NpgsqlCommand("SELECT @p1::date, @p2::date", conn);
#if LegacyProviderSpecificDateTimeTypes
            cmd.CommandText += ", @p3";
#endif // LegacyProviderSpecificDateTimeTypes
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Text) {Value = dateTime.ToString("yyyy-MM-dd")};
            cmd.Parameters.Add(p1);
            //Assert.That(p1.DbType, Is.EqualTo(DbType.Date));
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Text) {Value = dateTime.ToString("yyyy-MM-dd")};
            cmd.Parameters.Add(p2);
#if LegacyProviderSpecificDateTimeTypes
            var p3 = new NpgsqlParameter {ParameterName = "p3", Value = npgsqlDate};
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Date));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Date));
            cmd.Parameters.Add(p3);
#endif // LegacyProviderSpecificDateTimeTypes
            await using var reader = await cmd.ExecuteReaderAsync();
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
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
                Assert.That(reader.GetDate(i), Is.EqualTo(npgsqlDate));
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlDate)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(npgsqlDate));
                Assert.That(reader.GetFieldValue<NpgsqlDate>(i), Is.EqualTo(npgsqlDate));
#pragma warning restore 618
#else
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(dateTime));
#endif // LegacyProviderSpecificDateTimeTypes
            }
        }

        static readonly TestCaseData[] DateSpecialCases = {
            new TestCaseData("-infinity").SetName(nameof(DateSpecial) + "NegativeInfinity"),
            new TestCaseData("infinity").SetName(nameof(DateSpecial) + "Infinity"),
            new TestCaseData("0005-03-03 BC").SetName(nameof(DateSpecial) +"BC"),
        };

        [Test, TestCaseSource(nameof(DateSpecialCases))]
        public async Task DateSpecial(string value)
        {
            await using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p::date", conn);
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = value });
            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            var npgsqlDate = NpgsqlDate.Parse(value);
            Assert.That(reader.GetDate(0), Is.EqualTo(npgsqlDate));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlDate)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(npgsqlDate));
            Assert.That(reader.GetFieldValue<NpgsqlDate>(0), Is.EqualTo(npgsqlDate));
#pragma warning restore 618
#else
            Assert.That(() => reader.GetProviderSpecificValue(0), Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader.GetDateTime(0), Throws.Exception.TypeOf<InvalidCastException>());
#endif // LegacyProviderSpecificDateTimeTypes
        }

        [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
        public async Task DateConvertInfinity()
        {
            await using var conn = await OpenConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString)
                { ConvertInfinityDateTime = true });
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Date, DateTime.MaxValue);
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Date, DateTime.MinValue);
            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            Assert.That(reader.GetFieldValue<NpgsqlDate>(0), Is.EqualTo(NpgsqlDate.Infinity));
            Assert.That(reader.GetFieldValue<NpgsqlDate>(1), Is.EqualTo(NpgsqlDate.NegativeInfinity));
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes
            Assert.That(reader.GetDateTime(0), Is.EqualTo(DateTime.MaxValue));
            Assert.That(reader.GetDateTime(1), Is.EqualTo(DateTime.MinValue));
        }

        #endregion

        #region Time

        [Test]
        public async Task Time()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var expected = new TimeSpan(0, 10, 45, 34, 500);

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Time) {Value = expected});
                    cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Time) {Value = expected});
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
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
                }
            }
        }

        #endregion

        #region Time with timezone

        [Test]
        [MonoIgnore]
        public async Task TimeTz()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var tzOffset = TimeZoneInfo.Local.BaseUtcOffset;
                if (tzOffset == TimeSpan.Zero)
                    Assert.Ignore("Test cannot run when machine timezone is UTC");

                // Note that the date component of the below is ignored
                var dto = new DateTimeOffset(5, 5, 5, 13, 3, 45, 510, tzOffset);
                var dtUtc = new DateTime(dto.Year, dto.Month, dto.Day, dto.Hour, dto.Minute, dto.Second, dto.Millisecond, DateTimeKind.Utc) - tzOffset;
                var dtLocal = new DateTime(dto.Year, dto.Month, dto.Day, dto.Hour, dto.Minute, dto.Second, dto.Millisecond, DateTimeKind.Local);
                var dtUnspecified = new DateTime(dto.Year, dto.Month, dto.Day, dto.Hour, dto.Minute, dto.Second, dto.Millisecond, DateTimeKind.Unspecified);
                var ts = dto.TimeOfDay;

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.TimeTz, dto);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.TimeTz, dtUtc);
                    cmd.Parameters.AddWithValue("p3", NpgsqlDbType.TimeTz, dtLocal);
                    cmd.Parameters.AddWithValue("p4", NpgsqlDbType.TimeTz, dtUnspecified);
                    cmd.Parameters.AddWithValue("p5", NpgsqlDbType.TimeTz, ts);
                    Assert.That(cmd.Parameters.All(p => p.DbType == DbType.Object));

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        reader.Read();

                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTimeOffset)));

                            Assert.That(reader.GetFieldValue<DateTimeOffset>(i), Is.EqualTo(new DateTimeOffset(1, 1, 2, dto.Hour, dto.Minute, dto.Second, dto.Millisecond, dto.Offset)));
                            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTimeOffset)));
                            Assert.That(reader.GetFieldValue<DateTime>(i).Kind, Is.EqualTo(DateTimeKind.Local));
                            Assert.That(reader.GetFieldValue<DateTime>(i), Is.EqualTo(reader.GetFieldValue<DateTimeOffset>(i).LocalDateTime));
                            Assert.That(reader.GetFieldValue<TimeSpan>(i), Is.EqualTo(reader.GetFieldValue<DateTimeOffset>(i).LocalDateTime.TimeOfDay));
                        }
                    }
                }
            }
        }

        [Test]
        public async Task TimeWithTimeZoneBeforeUtcZero()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT TIME WITH TIME ZONE '01:00:00+02'", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<DateTimeOffset>(0), Is.EqualTo(new DateTimeOffset(1, 1, 2, 1, 0, 0, new TimeSpan(0, 2, 0, 0))));
            }
        }

        #endregion

        #region Timestamp

        static readonly TestCaseData[] TimeStampCases = {
            new TestCaseData(DateTime.MinValue).SetName(nameof(Timestamp) + "DateTime.MinValue"),
            new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38)).SetName(nameof(Timestamp) + "Pre2000"),
            new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345)).SetName(nameof(Timestamp) + "Post2000"),
            new TestCaseData(new DateTime(2013, 7, 25)).SetName(nameof(Timestamp) + "DateOnly"),
            new TestCaseData(new DateTime(DateTime.MaxValue.Ticks - 9)).SetName(nameof(Timestamp) + "DateTime.MaxValue (rounded)"),
        };

        [Test, TestCaseSource(nameof(TimeStampCases))]
        public async Task Timestamp(DateTime dateTime)
        {
            await using var conn = await OpenConnectionAsync();
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            var npgsqlDateTime = new NpgsqlDateTime(dateTime.Ticks);
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6::timestamp", conn);
#if LegacyProviderSpecificDateTimeTypes
            cmd.CommandText += ", @p7";
#endif // LegacyProviderSpecificDateTimeTypes
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Timestamp);
            var p2 = new NpgsqlParameter("p2", DbType.DateTime);
            var p3 = new NpgsqlParameter("p3", DbType.DateTime2);
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = dateTime };
            Assert.That(p4.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
            Assert.That(p4.DbType, Is.EqualTo(DbType.DateTime));
            var p5 = new NpgsqlParameter<DateTime> { ParameterName = "p5", TypedValue = dateTime };
            var p6 = new NpgsqlParameter("p6", NpgsqlDbType.Text){Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF")};
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
#if LegacyProviderSpecificDateTimeTypes
            var p7 = new NpgsqlParameter { ParameterName = "p7", Value = npgsqlDateTime };
            Assert.That(p7.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
            Assert.That(p7.DbType, Is.EqualTo(DbType.DateTime));
            cmd.Parameters.Add(p7);
            p1.Value = p2.Value = p3.Value = npgsqlDateTime;
#else
            p1.Value = p2.Value = p3.Value = dateTime;
#endif // LegacyProviderSpecificDateTimeTypes
            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                // Regular type (DateTime)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetDateTime(i), Is.EqualTo(dateTime));
                Assert.That(reader.GetDateTime(i).Kind, Is.EqualTo(DateTimeKind.Unspecified));
                Assert.That(reader.GetFieldValue<DateTime>(i), Is.EqualTo(dateTime));
                Assert.That(reader[i], Is.EqualTo(dateTime));
                Assert.That(reader.GetValue(i), Is.EqualTo(dateTime));

                // Provider-specific type (NpgsqlTimeStamp)
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
                Assert.That(reader.GetTimeStamp(i), Is.EqualTo(npgsqlDateTime));
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlDateTime)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(npgsqlDateTime));
                Assert.That(reader.GetFieldValue<NpgsqlDateTime>(i), Is.EqualTo(npgsqlDateTime));
#pragma warning restore 618
#else
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(dateTime));
#endif // LegacyProviderSpecificDateTimeTypes

                // DateTimeOffset
                Assert.That(() => reader.GetFieldValue<DateTimeOffset>(i), Throws.Exception.TypeOf<InvalidCastException>());
            }
        }

        static readonly TestCaseData[] TimeStampSpecialCases = {
            new TestCaseData("-infinity").SetName(nameof(TimeStampSpecial) + "NegativeInfinity"),
            new TestCaseData("infinity").SetName(nameof(TimeStampSpecial) + "Infinity"),
            new TestCaseData("0005-03-03 01:00:00 BC").SetName(nameof(TimeStampSpecial) +"BC"),
        };

        [Test, TestCaseSource(nameof(TimeStampSpecialCases))]
        public async Task TimeStampSpecial(string value)
        {
            await using var conn = await OpenConnectionAsync();
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            var npgsqlDateTime = NpgsqlDateTime.Parse(value);
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = npgsqlDateTime });
#pragma warning restore 618
#else
            using var cmd = new NpgsqlCommand("SELECT @p::timestamp", conn);
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = value });
#endif // LegacyProviderSpecificDateTimeTypes
            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
#if LegacyProviderSpecificDateTimeTypes
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(npgsqlDateTime));
            Assert.That(() => reader.GetDateTime(0), Throws.Exception.TypeOf<InvalidCastException>());
#else
            Assert.That(() => reader.GetProviderSpecificValue(0), Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(() => reader.GetDateTime(0), Throws.Exception.TypeOf<InvalidCastException>());
#endif // LegacyProviderSpecificDateTimeTypes
        }

        [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
        public async Task TimeStampConvertInfinity()
        {
            await using var conn = await OpenConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString)
                { ConvertInfinityDateTime = true });
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Timestamp, DateTime.MaxValue);
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Timestamp, DateTime.MinValue);
            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            Assert.That(reader.GetFieldValue<NpgsqlDateTime>(0), Is.EqualTo(NpgsqlDateTime.Infinity));
            Assert.That(reader.GetFieldValue<NpgsqlDateTime>(1), Is.EqualTo(NpgsqlDateTime.NegativeInfinity));
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes
            Assert.That(reader.GetDateTime(0), Is.EqualTo(DateTime.MaxValue));
            Assert.That(reader.GetDateTime(1), Is.EqualTo(DateTime.MinValue));
        }

        #endregion

        #region Timestamp with timezone

        [Test]
        public async Task TimestampTz()
        {
            await using var conn = await OpenConnectionAsync();
            var tzOffset = TimeZoneInfo.Local.BaseUtcOffset;
            if (tzOffset == TimeSpan.Zero)
                Assert.Ignore("Test cannot run when machine timezone is UTC");

            var dateTimeUtc = new DateTime(2015, 6, 27, 8, 45, 12, 345, DateTimeKind.Utc);
            var dateTimeLocal = dateTimeUtc.ToLocalTime();
            var dateTimeUnspecified = new DateTime(dateTimeUtc.Ticks, DateTimeKind.Unspecified);

#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            var nDateTimeUtc = new NpgsqlDateTime(dateTimeUtc);
            var nDateTimeLocal = nDateTimeUtc.ToLocalTime();
            var nDateTimeUnspecified = new NpgsqlDateTime(nDateTimeUtc.Ticks, DateTimeKind.Unspecified);

            Assert.AreEqual(nDateTimeUtc, nDateTimeLocal.ToUniversalTime());
            Assert.AreEqual(nDateTimeUtc, new NpgsqlDateTime(nDateTimeLocal.Ticks, DateTimeKind.Unspecified).ToUniversalTime());
            Assert.AreEqual(nDateTimeLocal, nDateTimeUnspecified.ToLocalTime());
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

            var dateTimeOffset = new DateTimeOffset(dateTimeLocal);

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
#if LegacyProviderSpecificDateTimeTypes
            cmd.CommandText += ", @p5, @p6, @p7";
#endif // LegacyProviderSpecificDateTimeTypes
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.TimestampTz, dateTimeUtc);
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.TimestampTz, dateTimeLocal);
            cmd.Parameters.AddWithValue("p3", NpgsqlDbType.TimestampTz, dateTimeUnspecified);
            cmd.Parameters.AddWithValue("p4", dateTimeOffset);
            Assert.That(cmd.Parameters["p4"].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.TimestampTz));
#if LegacyProviderSpecificDateTimeTypes
            cmd.Parameters.AddWithValue("p5", NpgsqlDbType.TimestampTz, nDateTimeUtc);
            cmd.Parameters.AddWithValue("p6", NpgsqlDbType.TimestampTz, nDateTimeLocal);
            cmd.Parameters.AddWithValue("p7", NpgsqlDbType.TimestampTz, nDateTimeUnspecified);
#endif // LegacyProviderSpecificDateTimeTypes

            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                // Regular type (DateTime)
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetDateTime(i), Is.EqualTo(dateTimeLocal));
                Assert.That(reader.GetFieldValue<DateTime>(i).Kind, Is.EqualTo(DateTimeKind.Local));
                Assert.That(reader[i], Is.EqualTo(dateTimeLocal));
                Assert.That(reader.GetValue(i), Is.EqualTo(dateTimeLocal));

                // Provider-specific type (NpgsqlDateTime)
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
                Assert.That(reader.GetTimeStamp(i), Is.EqualTo(nDateTimeLocal));
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlDateTime)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(nDateTimeLocal));
                Assert.That(reader.GetFieldValue<NpgsqlDateTime>(i), Is.EqualTo(nDateTimeLocal));
#pragma warning restore 618
#else
                Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(dateTimeLocal));
#endif // LegacyProviderSpecificDateTimeTypes

                // DateTimeOffset
                Assert.That(reader.GetFieldValue<DateTimeOffset>(i), Is.EqualTo(dateTimeOffset));
                var x = reader.GetFieldValue<DateTimeOffset>(i);
            }

        }

        #endregion

        #region Interval

        [Test]
        public async Task Interval()
        {
            await using var conn = await OpenConnectionAsync();
            var expectedTimeSpan = new TimeSpan(1, 2, 3, 4, 5);
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            var expectedNpgsqlInterval = new NpgsqlTimeSpan(1, 2, 3, 4, 5);
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

            using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
#if LegacyProviderSpecificDateTimeTypes
            cmd.CommandText += ", @p3";
#endif // LegacyProviderSpecificDateTimeTypes
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Interval);
            var p2 = new NpgsqlParameter("p2", expectedTimeSpan);
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p2.DbType, Is.EqualTo(DbType.Object));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
#if LegacyProviderSpecificDateTimeTypes
            var p3 = new NpgsqlParameter("p3", expectedNpgsqlInterval);
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Interval));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Object));
            cmd.Parameters.Add(p3);
            p1.Value = expectedNpgsqlInterval;
#else
            p1.Value = expectedTimeSpan;
#endif // LegacyProviderSpecificDateTimeTypes

            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            // Regular type (TimeSpan)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(TimeSpan)));
            Assert.That(reader.GetTimeSpan(0), Is.EqualTo(expectedTimeSpan));
            Assert.That(reader.GetFieldValue<TimeSpan>(0), Is.EqualTo(expectedTimeSpan));
            Assert.That(reader[0], Is.EqualTo(expectedTimeSpan));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedTimeSpan));

            // Provider-specific type (NpgsqlInterval)
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
            Assert.That(reader.GetInterval(0), Is.EqualTo(expectedNpgsqlInterval));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlTimeSpan)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlInterval));
            Assert.That(reader.GetFieldValue<NpgsqlTimeSpan>(0), Is.EqualTo(expectedNpgsqlInterval));
#pragma warning restore 618
#else
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(TimeSpan)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedTimeSpan));
#endif // LegacyProviderSpecificDateTimeTypes
        }

        #endregion

        public DateTimeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
