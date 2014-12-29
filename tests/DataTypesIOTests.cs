using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NpgsqlTests
{
    [TestFixture]
    public class DataTypesIOTests : TestBase
    {
        public DataTypesIOTests(string backendVersion) : base(backendVersion) { }

        [Test]
        public void TestDates([Values(true, false)] bool prepare)
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
                if (prepare)
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

        [Test]
        public void TestGuid([Values(true, false)] bool prepare)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "select '00010203-0405-0607-0809-0a0b0c0d0e0f'::uuid";
                if (prepare)
                    cmd.Prepare();
                var uuid = cmd.ExecuteScalar();
                Assert.AreEqual(new Guid("00010203-0405-0607-0809-0a0b0c0d0e0f"), uuid);
            }
        }

        [Test]
        public void TestMoney([Values(true, false)] bool prepare)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "select 1::money, 123.45::money, 1234567890123.45::money";
                if (prepare)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(1M, reader.GetValue(0));
                    Assert.AreEqual(123.45M, reader.GetValue(1));
                    Assert.AreEqual(1234567890123.45M, reader.GetValue(2));
                }
            }
        }
    }
}
