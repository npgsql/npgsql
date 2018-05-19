#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL date/time types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class DateTimeTests : TestBase
    {
        #region Date

        [Test]
        public void Date()
        {
            using (var conn = OpenConnection())
            {
                var dateTime = new DateTime(2002, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified);
                var npgsqlDate = new NpgsqlDate(dateTime);

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                {
                    var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Date) {Value = npgsqlDate};
                    var p2 = new NpgsqlParameter {ParameterName = "p2", Value = npgsqlDate};
                    Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Date));
                    Assert.That(p2.DbType, Is.EqualTo(DbType.Date));
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    using (var reader = cmd.ExecuteReader())
                    {
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
                        }
                    }
                }
            }
        }

        static readonly TestCaseData[] DateSpecialCases = {
            new TestCaseData(NpgsqlDate.Infinity).SetName(nameof(DateSpecial) + "Infinity"),
            new TestCaseData(NpgsqlDate.NegativeInfinity).SetName(nameof(DateSpecial) + "NegativeInfinity"),
            new TestCaseData(new NpgsqlDate(-5, 3, 3)).SetName(nameof(DateSpecial) +"BC"),
        };

        [Test, TestCaseSource(nameof(DateSpecialCases))]
        public void DateSpecial(NpgsqlDate value)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn)) {
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = value });
                using (var reader = cmd.ExecuteReader()) {
                    reader.Read();
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(value));
                    Assert.That(() => reader.GetDateTime(0), Throws.Exception.TypeOf<InvalidCastException>());
                }
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
        public void DateConvertInfinity()
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";ConvertInfinityDateTime=true"))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn)) {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Date, DateTime.MaxValue);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Date, DateTime.MinValue);
                    using (var reader = cmd.ExecuteReader()) {
                        reader.Read();
                        Assert.That(reader.GetFieldValue<NpgsqlDate>(0), Is.EqualTo(NpgsqlDate.Infinity));
                        Assert.That(reader.GetFieldValue<NpgsqlDate>(1), Is.EqualTo(NpgsqlDate.NegativeInfinity));
                        Assert.That(reader.GetDateTime(0), Is.EqualTo(DateTime.MaxValue));
                        Assert.That(reader.GetDateTime(1), Is.EqualTo(DateTime.MinValue));
                    }
                }
            }
        }

        #endregion

        #region Time

        [Test]
        public void Time()
        {
            using (var conn = OpenConnection())
            {
                var expected = new TimeSpan(0, 10, 45, 34, 500);

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Time) {Value = expected});
                    cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Time) {Value = expected});
                    using (var reader = cmd.ExecuteReader())
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
        public void TimeTz()
        {
            using (var conn = OpenConnection())
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

                    using (var reader = cmd.ExecuteReader())
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
        public void TimeWithTimeZoneBeforeUtcZero()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT TIME WITH TIME ZONE '01:00:00+02'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<DateTimeOffset>(0), Is.EqualTo(new DateTimeOffset(1, 1, 2, 1, 0, 0, new TimeSpan(0, 2, 0, 0))));
            }
        }

        #endregion

        #region Timestamp

        static readonly TestCaseData[] TimeStampCases = {
            new TestCaseData(new DateTime(1998, 4, 12, 13, 26, 38)).SetName(nameof(Timestamp) + "Pre2000"),
            new TestCaseData(new DateTime(2015, 1, 27, 8, 45, 12, 345)).SetName(nameof(Timestamp) + "Post2000"),
            new TestCaseData(new DateTime(2013, 7, 25)).SetName(nameof(Timestamp) + "DateOnly"),
        };

        [Test, TestCaseSource(nameof(TimeStampCases))]
        public void Timestamp(DateTime dateTime)
        {
            using (var conn = OpenConnection())
            {
                var npgsqlDateTime = new NpgsqlDateTime(dateTime.Ticks);

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6", conn))
                {
                    var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Timestamp);
                    var p2 = new NpgsqlParameter("p2", DbType.DateTime);
                    var p3 = new NpgsqlParameter("p3", DbType.DateTime2);
                    var p4 = new NpgsqlParameter { ParameterName = "p4", Value = npgsqlDateTime };
                    var p5 = new NpgsqlParameter { ParameterName = "p5", Value = dateTime };
                    var p6 = new NpgsqlParameter<DateTime> { ParameterName = "p6", TypedValue = dateTime };
                    Assert.That(p4.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
                    Assert.That(p4.DbType, Is.EqualTo(DbType.DateTime));
                    Assert.That(p5.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
                    Assert.That(p5.DbType, Is.EqualTo(DbType.DateTime));
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    cmd.Parameters.Add(p3);
                    cmd.Parameters.Add(p4);
                    cmd.Parameters.Add(p5);
                    cmd.Parameters.Add(p6);
                    p1.Value = p2.Value = p3.Value = npgsqlDateTime;
                    using (var reader = cmd.ExecuteReader())
                    {
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
                            Assert.That(reader.GetTimeStamp(i), Is.EqualTo(npgsqlDateTime));
                            Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlDateTime)));
                            Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(npgsqlDateTime));
                            Assert.That(reader.GetFieldValue<NpgsqlDateTime>(i), Is.EqualTo(npgsqlDateTime));

                            // DateTimeOffset
                            Assert.That(() => reader.GetFieldValue<DateTimeOffset>(i), Throws.Exception.TypeOf<InvalidCastException>());
                        }
                    }
                }
            }
        }

        static readonly TestCaseData[] TimeStampSpecialCases = {
            new TestCaseData(NpgsqlDateTime.Infinity).SetName(nameof(TimeStampSpecial) + "Infinity"),
            new TestCaseData(NpgsqlDateTime.NegativeInfinity).SetName(nameof(TimeStampSpecial) + "NegativeInfinity"),
            new TestCaseData(new NpgsqlDateTime(-5, 3, 3, 1, 0, 0)).SetName(nameof(TimeStampSpecial) + "BC"),
        };

        [Test, TestCaseSource(nameof(TimeStampSpecialCases))]
        public void TimeStampSpecial(NpgsqlDateTime value)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn)) {
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = value });
                using (var reader = cmd.ExecuteReader()) {
                    reader.Read();
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(value));
                    Assert.That(() => reader.GetDateTime(0), Throws.Exception.TypeOf<InvalidCastException>());
                }
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Makes sure that when ConvertInfinityDateTime is true, infinity values are properly converted")]
        public void TimeStampConvertInfinity()
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";ConvertInfinityDateTime=true"))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Timestamp, DateTime.MaxValue);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Timestamp, DateTime.MinValue);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldValue<NpgsqlDateTime>(0), Is.EqualTo(NpgsqlDateTime.Infinity));
                        Assert.That(reader.GetFieldValue<NpgsqlDateTime>(1), Is.EqualTo(NpgsqlDateTime.NegativeInfinity));
                        Assert.That(reader.GetDateTime(0), Is.EqualTo(DateTime.MaxValue));
                        Assert.That(reader.GetDateTime(1), Is.EqualTo(DateTime.MinValue));
                    }
                }
            }
        }

        #endregion

        #region Timestamp with timezone

        [Test]
        public void TimestampTz()
        {
            using (var conn = OpenConnection())
            {
                var tzOffset = TimeZoneInfo.Local.BaseUtcOffset;
                if (tzOffset == TimeSpan.Zero)
                    Assert.Ignore("Test cannot run when machine timezone is UTC");

                var dateTimeUtc = new DateTime(2015, 6, 27, 8, 45, 12, 345, DateTimeKind.Utc);
                var dateTimeLocal = dateTimeUtc.ToLocalTime();
                var dateTimeUnspecified = new DateTime(dateTimeUtc.Ticks, DateTimeKind.Unspecified);

                var nDateTimeUtc = new NpgsqlDateTime(dateTimeUtc);
                var nDateTimeLocal = nDateTimeUtc.ToLocalTime();
                var nDateTimeUnspecified = new NpgsqlDateTime(nDateTimeUtc.Ticks, DateTimeKind.Unspecified);

                //var dateTimeOffset = new DateTimeOffset(dateTimeLocal, dateTimeLocal - dateTimeUtc);
                var dateTimeOffset = new DateTimeOffset(dateTimeLocal);

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5, @p6, @p7", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.TimestampTz, dateTimeUtc);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.TimestampTz, dateTimeLocal);
                    cmd.Parameters.AddWithValue("p3", NpgsqlDbType.TimestampTz, dateTimeUnspecified);
                    cmd.Parameters.AddWithValue("p4", NpgsqlDbType.TimestampTz, nDateTimeUtc);
                    cmd.Parameters.AddWithValue("p5", NpgsqlDbType.TimestampTz, nDateTimeLocal);
                    cmd.Parameters.AddWithValue("p6", NpgsqlDbType.TimestampTz, nDateTimeUnspecified);
                    cmd.Parameters.AddWithValue("p7", dateTimeOffset);
                    Assert.That(cmd.Parameters["p7"].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.TimestampTz));

                    using (var reader = cmd.ExecuteReader())
                    {
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
                            Assert.That(reader.GetTimeStamp(i), Is.EqualTo(nDateTimeLocal));
                            Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(NpgsqlDateTime)));
                            Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(nDateTimeLocal));
                            Assert.That(reader.GetFieldValue<NpgsqlDateTime>(i), Is.EqualTo(nDateTimeLocal));

                            // DateTimeOffset
                            Assert.That(reader.GetFieldValue<DateTimeOffset>(i), Is.EqualTo(dateTimeOffset));
                            var x = reader.GetFieldValue<DateTimeOffset>(i);
                        }
                    }
                }

                Assert.AreEqual(nDateTimeUtc, nDateTimeLocal.ToUniversalTime());
                Assert.AreEqual(nDateTimeUtc, new NpgsqlDateTime(nDateTimeLocal.Ticks, DateTimeKind.Unspecified).ToUniversalTime());
                Assert.AreEqual(nDateTimeLocal, nDateTimeUnspecified.ToLocalTime());
            }
        }

        #endregion

        #region Interval

        [Test]
        public void Interval()
        {
            using (var conn = OpenConnection())
            {
                var expectedNpgsqlInterval = new NpgsqlTimeSpan(1, 2, 3, 4, 5);
                var expectedTimeSpan = new TimeSpan(1, 2, 3, 4, 5);

                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
                {
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
                    p1.Value = expectedNpgsqlInterval;

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        // Regular type (TimeSpan)
                        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(TimeSpan)));
                        Assert.That(reader.GetTimeSpan(0), Is.EqualTo(expectedTimeSpan));
                        Assert.That(reader.GetFieldValue<TimeSpan>(0), Is.EqualTo(expectedTimeSpan));
                        Assert.That(reader[0], Is.EqualTo(expectedTimeSpan));
                        Assert.That(reader.GetValue(0), Is.EqualTo(expectedTimeSpan));

                        // Provider-specific type (NpgsqlInterval)
                        Assert.That(reader.GetInterval(0), Is.EqualTo(expectedNpgsqlInterval));
                        Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlTimeSpan)));
                        Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlInterval));
                        Assert.That(reader.GetFieldValue<NpgsqlTimeSpan>(0), Is.EqualTo(expectedNpgsqlInterval));
                    }
                }
            }
        }

        #endregion
    }
}
