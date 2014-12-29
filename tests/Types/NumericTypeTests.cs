using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL numeric types
    /// </summary>
    /// <summary>
    /// http://www.postgresql.org/docs/9.4/static/datatype-numeric.html
    /// </summary>
    public class NumericTypeTests : TestBase
    {
        [Test]
        public void ReadInt16([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::SMALLINT", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            Assert.That(reader.GetFieldType(0),             Is.EqualTo(typeof(short)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadInt32([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::INTEGER", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            Assert.That(reader.GetFieldType(0),             Is.EqualTo(typeof(int)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test, Description("Tests some types which are aliased to UInt32")]
        [TestCase("oid")]
        [TestCase("xid")]
        [TestCase("cid")]
        public void ReadUInt32Aliases(string typename)
        {
            const uint expected = 8;
            var cmd = new NpgsqlCommand(String.Format("SELECT '{0}'::{1}", expected, typename), Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadInt64([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::BIGINT", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            Assert.That(reader.GetFieldType(0),             Is.EqualTo(typeof(long)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadDouble([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            const double expected = 4.123456789012345;
            var cmd = new NpgsqlCommand("SELECT 4.123456789012345::DOUBLE PRECISION", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetDouble(0), Is.EqualTo(expected).Within(10E-07));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(double)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void GetFloat([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            const float expected = .123456F;
            var cmd = new NpgsqlCommand("SELECT .123456::REAL", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFloat(0), Is.EqualTo(expected).Within(10E-07));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(float)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadNumeric([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 8::NUMERIC", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt64(0),                 Is.EqualTo(8));
            Assert.That(reader.GetInt16(0),                 Is.EqualTo(8));
            Assert.That(reader.GetByte(0),                  Is.EqualTo(8));
            Assert.That(reader.GetFloat(0),                 Is.EqualTo(8.0f));
            Assert.That(reader.GetDouble(0),                Is.EqualTo(8.0d));
            Assert.That(reader.GetDecimal(0),               Is.EqualTo(8.0m));
            Assert.That(reader.GetValue(0),                 Is.EqualTo(8));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(8));
            Assert.That(reader.GetFieldType(0),             Is.EqualTo(typeof(decimal)));
            reader.Dispose();
            cmd.Dispose();
        }

        // Older tests

        [Test]
        public void ByteSupport()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_int2) VALUES (:a)", Conn))
            {
                command.Parameters.Add(new NpgsqlParameter("a", DbType.Byte));
                command.Parameters[0].Value = 2;
                var rowsAdded = command.ExecuteNonQuery();
                Assert.AreEqual(1, rowsAdded);
                command.Parameters.Clear();
            }
        }

        [Test]
        public void DoubleWithoutPrepared()
        {
            var command = new NpgsqlCommand("select :field_float8", Conn);
            command.Parameters.Add(new NpgsqlParameter(":field_float8", NpgsqlDbType.Double));
            double x = 1d / 7d; ;
            command.Parameters[0].Value = x;
            var valueReturned = command.ExecuteScalar();
            Assert.That(valueReturned, Is.EqualTo(x).Within(100).Ulps);
            Console.WriteLine("Actual=  {0}", valueReturned);
            Console.WriteLine("Expected={0}", x);
        }

        [Test]
        public void DoubleValueSupportWithExtendedQuery()
        {
            ExecuteNonQuery("INSERT INTO data(field_float8) VALUES (.123456789012345)");
            using (var command = new NpgsqlCommand("select count(*) from data where field_float8 = :a", Conn))
            {
                command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Double));
                command.Parameters[0].Value = 0.123456789012345D;
                command.Prepare();
                var rows = command.ExecuteScalar();
                Assert.AreEqual(1, rows);
            }
        }

        [Test]
        public void PrecisionScaleNumericSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_numeric) VALUES (-4.3)");

            using (var command = new NpgsqlCommand("SELECT field_numeric FROM data", Conn))
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(0);
                Assert.AreEqual(-4.3000000M, result);
                //Assert.AreEqual(11, result.Precision);
                //Assert.AreEqual(7, result.Scale);
            }
        }

        [Test]
        public void NumberConversionWithCulture()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double);
                parameter.Value = 5.5;
                cmd.Parameters.Add(parameter);
                var result = cmd.ExecuteScalar();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("");
                Assert.AreEqual(5.5, result);
            }
        }

        [Test]
        public void TestMoney([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "select 1::money, 123.45::money, 1234567890123.45::money";
                if (prepare == PrepareOrNot.Prepared)
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

        public NumericTypeTests(string backendVersion) : base(backendVersion) { }
    }
}
