using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests
{
    class DateTimeTests : TestBase
    {
        public DateTimeTests(string backendVersion) : base(backendVersion) {}

        [Test]
        public void ReadDate([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            // TODO: Decide on the DateTime kind (#346)
            var expectedDateTime = new DateTime(2002, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified);
            var expectedNpgsqlDate = new NpgsqlDate(expectedDateTime);
            ExecuteNonQuery("INSERT INTO data (field_date) VALUES ('2002-03-04')");
            var cmd = new NpgsqlCommand("SELECT '2002-03-04'::DATE", Conn);

            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
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
        public void ReadTime([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            // TODO: Decide on the DateTime kind (#346)
            var expectedNpgsqlTime = new NpgsqlTime(10, 3, 45, 345000);
            var expectedDateTime = new DateTime(expectedNpgsqlTime.Ticks, DateTimeKind.Unspecified);
            var cmd = new NpgsqlCommand("SELECT '10:03:45.345'::TIME", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
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
    }
}
