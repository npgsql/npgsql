using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL date/time types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-datetime.html
    /// </remarks>
    class DateTimeTests : TestBase
    {
        public DateTimeTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public void ReadDate()
        {
            // TODO: Decide on the DateTime kind (#346)
            var expectedDateTime = new DateTime(2002, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified);
            var expectedNpgsqlDate = new NpgsqlDate(expectedDateTime);
            ExecuteNonQuery("INSERT INTO data (field_date) VALUES ('2002-03-04')");
            var cmd = new NpgsqlCommand("SELECT '2002-03-04'::DATE", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Regular type (DateTime)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (DateTime)));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(expectedDateTime));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expectedDateTime));
            Assert.That(reader[0], Is.EqualTo(expectedDateTime));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedDateTime));

            // Provider-specific type (NpgsqlDate)
            Assert.That(reader.GetDate(0), Is.EqualTo(expectedNpgsqlDate));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlDate)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlDate));
            Assert.That(reader.GetFieldValue<NpgsqlDate>(0), Is.EqualTo(expectedNpgsqlDate));

            cmd.Dispose();
        }

        [Test]
        public void ReadTime()
        {
            // TODO: Decide on the DateTime kind (#346)
            var expectedNpgsqlTime = new NpgsqlTime(10, 3, 45, 345000);
            var expectedDateTime = new DateTime(expectedNpgsqlTime.Ticks, DateTimeKind.Unspecified);
            var cmd = new NpgsqlCommand("SELECT '10:03:45.345'::TIME", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Regular type (DateTime)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));
            Assert.That(reader.GetDateTime(0), Is.EqualTo(expectedDateTime));
            Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expectedDateTime));
            Assert.That(reader[0], Is.EqualTo(expectedDateTime));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedDateTime));

            // Provider-specific type (NpgsqlTime)
            Assert.That(reader.GetTime(0), Is.EqualTo(expectedNpgsqlTime));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlTime)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlTime));
            Assert.That(reader.GetFieldValue<NpgsqlTime>(0), Is.EqualTo(expectedNpgsqlTime));

            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void ReadTimeTz()
        {
            // TODO: Decide on the DateTime kind (#346)
            var expectedNpgsqlTimeTz = new NpgsqlTimeTZ(13, 3, 45, 001000, new NpgsqlTimeZone(-5, 0));
            var expectedDateTime = new DateTime(expectedNpgsqlTimeTz.AtTimeZone(NpgsqlTimeZone.UTC).Ticks, DateTimeKind.Utc).ToLocalTime();
            using (var cmd = new NpgsqlCommand("SELECT '13:03:45.001-05'::TIMETZ", Conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();

                // Regular type (DateTime)
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetDateTime(0), Is.EqualTo(expectedDateTime));
                Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expectedDateTime));
                Assert.That(reader[0], Is.EqualTo(expectedDateTime));
                Assert.That(reader.GetValue(0), Is.EqualTo(expectedDateTime));

                // Provider-specific type (NpgsqlTimeTZ)
                Assert.That(reader.GetTimeTZ(0), Is.EqualTo(expectedNpgsqlTimeTz));
                Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlTimeTZ)));
                Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlTimeTz));
                Assert.That(reader.GetFieldValue<NpgsqlTimeTZ>(0), Is.EqualTo(expectedNpgsqlTimeTz));
            }
        }

        [Test]
        public void Timestamp()
        {
            // TODO: Decide on the DateTime kind (#346)
            var expectedNpgsqlTimeStamp = new NpgsqlTimeStamp(new NpgsqlDate(2002, 2, 2), new NpgsqlTime(9, 0, 23.345));
            var expectedDateTime = new DateTime(expectedNpgsqlTimeStamp.Ticks, DateTimeKind.Utc);
            using (var cmd = new NpgsqlCommand("SELECT '2002-02-02 09:00:23.345'::TIMESTAMP", Conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();

                // Regular type (DateTime)
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(DateTime)));
                Assert.That(reader.GetDateTime(0), Is.EqualTo(expectedDateTime));
                Assert.That(reader.GetFieldValue<DateTime>(0), Is.EqualTo(expectedDateTime));
                Assert.That(reader[0], Is.EqualTo(expectedDateTime));
                Assert.That(reader.GetValue(0), Is.EqualTo(expectedDateTime));

                // Provider-specific type (NpgsqlTimeStamp)
                Assert.That(reader.GetTimeStamp(0), Is.EqualTo(expectedNpgsqlTimeStamp));
                Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlTimeStamp)));
                Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlTimeStamp));
                Assert.That(reader.GetFieldValue<NpgsqlTimeStamp>(0), Is.EqualTo(expectedNpgsqlTimeStamp));
            }
        }

        [Test]
        public void Interval()
        {
            var expectedNpgsqlInterval = new NpgsqlInterval(1, 2, 3, 4, 5);
            var expectedTimeSpan = new TimeSpan(1, 2, 3, 4, 5);
            using (var cmd = new NpgsqlCommand("SELECT '1 days 2 hours 3 minutes 4 seconds 5 milliseconds'::INTERVAL", Conn))
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
                Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInterval)));
                Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedNpgsqlInterval));
                Assert.That(reader.GetFieldValue<NpgsqlInterval>(0), Is.EqualTo(expectedNpgsqlInterval));
            }
        }

        // Older tests

        [Test]
        public void TestNpgsqlSpecificTypesCLRTypesNpgsqlTimeStamp()
        {
            // Please, check http://pgfoundry.org/forum/message.php?msg_id=1005483
            // for a discussion where an NpgsqlInet type isn't shown in a datagrid
            // This test tries to check if the type returned is an IPAddress when using
            // the GetValue() of NpgsqlDataReader and NpgsqlInet when using GetProviderValue();

            var command = new NpgsqlCommand("select '2010-01-17 15:45'::timestamp;", Conn);
            using (var dr = command.ExecuteReader()) {
                dr.Read();
                var result = dr.GetValue(0);
                var result2 = dr.GetProviderSpecificValue(0);
                Assert.AreEqual(typeof(DateTime), result.GetType());
                Assert.AreEqual(typeof(NpgsqlTimeStamp), result2.GetType());
            }
        }

        [Test]
        public void SelectInfinityValueDateDataType()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_date) VALUES ('-infinity'::date)");
            using (var cmd = new NpgsqlCommand(@"SELECT field_date FROM data", Conn))
            using (var dr = cmd.ExecuteReader()) {
                dr.Read();
                // InvalidCastException was unhandled
                // at Npgsql.ForwardsOnlyDataReader.GetValue(Int32 Index)
                //  at Npgsql.NpgsqlDataReader.GetDateTime(Int32 i) ..
                dr.GetDateTime(0);
            }
        }

        [Test]
        public void ReturnInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_timestamp) values ('infinity'::timestamp);", Conn);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from data where field_serial = (select max(field_serial) from data);", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        [Test]
        public void ReturnMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_timestamp) values ('-infinity'::timestamp);", Conn);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from data where field_serial = (select max(field_serial) from data);", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void MinusInfinityDateTimeSupport()
        {
            var command = Conn.CreateCommand();
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MinValue));
            command.CommandText = "select 1 where current_date=:p0";
            var result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void PlusInfinityDateTimeSupport()
        {
            var command = Conn.CreateCommand();
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MaxValue));
            command.CommandText = "select 1 where current_date=:p0";
            var result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void DateTimeSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_timestamp) VALUES ('2002-02-02 09:00:23.345')");
            using (var command = new NpgsqlCommand("SELECT field_timestamp FROM data", Conn))
            {
                var d = (DateTime)command.ExecuteScalar();

                Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
                Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

                var culture = new DateTimeFormatInfo();
                culture.TimeSeparator = ":";
                var dt = System.DateTime.Parse("2004-06-04 09:48:00", culture);

                command.CommandText = "insert into data(field_timestamp) values (:a);";
                command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
                command.Parameters[0].Value = dt;

                command.ExecuteScalar();
            }
        }

        [Test]
        public void DateTimeSupportNpgsqlDbType()
        {
            ExecuteNonQuery("INSERT INTO data (field_timestamp) VALUES ('2002-02-02 09:00:23.345')");

            using (var command = new NpgsqlCommand("SELECT field_timestamp FROM data;", Conn))
            {
                var d = (DateTime)command.ExecuteScalar();
                Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

                var culture = new DateTimeFormatInfo();
                culture.TimeSeparator = ":";
                var dt = DateTime.Parse("2004-06-04 09:48:00", culture);
                command.CommandText = "insert into data(field_timestamp) values (:a);";
                command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
                command.Parameters[0].Value = dt;
                command.ExecuteScalar();
            }
        }

        [Test]
        public void DateTimeSupportTimezone()
        {
            ExecuteNonQuery("INSERT INTO data(field_timestamp_with_timezone) VALUES ('2002-02-02 09:00:23.345Z')");
            var d = (DateTime)ExecuteScalar("SET TIME ZONE 5; SELECT field_timestamp_with_timezone FROM data");
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezone2()
        {
            ExecuteNonQuery("INSERT INTO data(field_timestamp_with_timezone) VALUES ('2002-02-02 09:00:23.345Z')");
            //Changed the comparison. Did thisassume too much about ToString()?
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5; select field_timestamp_with_timezone from data", Conn);
            var s = ((DateTime)command.ExecuteScalar()).ToUniversalTime().ToString();
            Assert.AreEqual(new DateTime(2002, 02, 02, 09, 00, 23).ToString(), s);
        }

        [Test]
        public void DateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select timestamptz'2009-11-11 20:15:43.019-03:30';", Conn);
            var d = (DateTime)command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2009-11-11 23:45:43Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezoneEuropeAmsterdam()
        {
            //1929-08-19 00:00:00+01:19:32
            // This test was provided by Christ Akkermans.
            var command = new NpgsqlCommand("SET TIME ZONE 'Europe/Amsterdam';SELECT '1929-08-19 00:00:00'::timestamptz;", Conn);
            var d = (DateTime)command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateTimeSupport()
        {
            ExecuteNonQuery(@"insert into data (field_timestamp) values ('2002-02-02 09:00:23.345')");
            var command = new NpgsqlCommand("select field_timestamp from data", Conn);

            NpgsqlTimeStamp ts;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStamp(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.ToString());

            var culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            var ts1 = NpgsqlTimeStamp.Parse("2004-06-04 09:48:00");

            command.CommandText = "insert into data(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateTimeSupportNpgsqlDbType()
        {
            ExecuteNonQuery(@"insert into data (field_timestamp) values ('2002-02-02 09:00:23.345')");
            var command = new NpgsqlCommand("select field_timestamp from data", Conn);

            NpgsqlTimeStamp ts;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStamp(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.ToString());

            var culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            var ts1 = NpgsqlTimeStamp.Parse("2004-06-04 09:48:00");

            command.CommandText = "insert into data(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateTimeSupportTimezone()
        {
            ExecuteNonQuery("SET TIME ZONE 5");
            ExecuteNonQuery("INSERT INTO data(field_timestamp_with_timezone) VALUES ('2002-02-02 09:00:23.345Z')");
            using (var command = new NpgsqlCommand("SELECT field_timestamp_with_timezone FROM data", Conn))
            {
                NpgsqlTimeStampTZ ts;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    ts = reader.GetTimeStampTZ(0);
                }

                Assert.AreEqual("2002-02-02 09:00:23.345", ts.AtTimeZone(NpgsqlTimeZone.UTC).ToString());
            }
        }

        [Test]
        public void ProviderDateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            ExecuteNonQuery("SET TIME ZONE 5");
            using (var command = new NpgsqlCommand("select timestamptz'2009-11-11 20:15:43.019-03:30';", Conn))
            {
                NpgsqlTimeStampTZ ts;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    ts = reader.GetTimeStampTZ(0);
                }

                Assert.AreEqual("2009-11-12 04:45:43.019+05", ts.ToString());
            }
        }

        [Test]
        public void ProviderDateTimeSupportTimezone4()
        {
            ExecuteNonQuery("SET TIME ZONE 5"); //Should not be equal to your local time zone !

            NpgsqlTimeStampTZ tsInsert = new NpgsqlTimeStampTZ(2014, 3, 28, 10, 0, 0, NpgsqlTimeZone.UTC);

            using (var command = new NpgsqlCommand("INSERT INTO data(field_timestamp_with_timezone) VALUES (:p1)", Conn))
            {
                var p1 = command.Parameters.Add("p1", NpgsqlDbType.TimestampTZ);
                p1.Direction = ParameterDirection.Input;
                p1.Value = tsInsert;

                command.ExecuteNonQuery();
            }


            using (var command = new NpgsqlCommand("SELECT field_timestamp_with_timezone FROM data", Conn))
            {
                NpgsqlTimeStampTZ tsSelect;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    tsSelect = reader.GetTimeStampTZ(0);
                }

                Assert.AreEqual(tsInsert.AtTimeZone(NpgsqlTimeZone.UTC), tsSelect.AtTimeZone(NpgsqlTimeZone.UTC));
            }
        }

        private void TimeStampHandlingInternal(bool prepare)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                DateTime inVal = DateTime.Parse("02/28/2000 02:20:20 PM", CultureInfo.InvariantCulture);
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Timestamp);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                if (prepare)
                {
                    cmd.Prepare();
                }

                var retVal = (DateTime)cmd.ExecuteScalar();
                Assert.AreEqual(inVal, retVal);
            }
        }

        [Test]
        public void TimeStampHandling()
        {
            TimeStampHandlingInternal(false);
        }

        [Test]
        public void TimeStampHandlingPrepared()
        {
            TimeStampHandlingInternal(true);
        }

        [Test]
        public void TestDates([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = @"select
                    '294277-01-09 04:00:54.775806'::timestamp,
                    '4714-11-24 00:00:00 BC'::timestamp,
                    '1999-12-31 23:59:59'::timestamp,
                    '2000-01-01 00:00:00'::timestamp,
                    '0001-01-01 00:00:00'::timestamp,
                    '0001-01-01 00:00:01'::timestamp,
                    'infinity'::timestamp,
                    '-infinity'::timestamp,
                    '2100-01-01T00:00:00Z'::timestamptz,
                    '01:02:03.456789'::time,
                    '01:02:03.456789-13:14:11'::timetz,
                    '00:00:00.000000+12:13:14'::timetz";
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(new NpgsqlTimeStamp(new NpgsqlDate(294277, 1, 9), new NpgsqlTime(4, 0, 54.775806M)), reader.GetProviderSpecificValue(0));
                    Assert.AreEqual(new NpgsqlTimeStamp(-4714, 11, 24, 0, 0, 0), reader.GetProviderSpecificValue(1));
                    Assert.AreEqual(new NpgsqlTimeStamp(1999, 12, 31, 23, 59, 59), reader.GetProviderSpecificValue(2));
                    Assert.AreEqual(new NpgsqlTimeStamp(2000, 1, 1, 0, 0, 0), reader.GetProviderSpecificValue(3));
                    Assert.AreEqual(new NpgsqlTimeStamp(0001, 1, 1, 0, 0, 0), reader.GetProviderSpecificValue(4));
                    Assert.AreEqual(new NpgsqlTimeStamp(0001, 1, 1, 0, 0, 1), reader.GetProviderSpecificValue(5));
                    Assert.AreEqual(NpgsqlTimeStamp.Infinity, reader.GetProviderSpecificValue(6));
                    Assert.AreEqual(NpgsqlTimeStamp.MinusInfinity, reader.GetProviderSpecificValue(7));
                    Assert.AreEqual(new DateTime(2100, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), reader.GetValue(8));
                    Assert.AreEqual(DateTimeKind.Utc, ((DateTime)reader.GetValue(8)).Kind);
                    Assert.AreEqual(new NpgsqlTime(1, 2, 3.456789), reader.GetProviderSpecificValue(9));
                    Assert.AreEqual(new NpgsqlTimeTZ(1, 2, 3.456789, -new NpgsqlTimeZone(13, 14, 11)), reader.GetProviderSpecificValue(10));
                    Assert.AreEqual(new NpgsqlTimeTZ(0, 0, 0, new NpgsqlTimeZone(12, 13, 14)), reader.GetProviderSpecificValue(11));
                }
            }
        }
    }
}
