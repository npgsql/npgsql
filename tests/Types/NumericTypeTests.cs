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
        public void Int16()
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Smallint);
            var p2 = new NpgsqlParameter("p2", DbType.Int16);
            var p3 = new NpgsqlParameter("p3", DbType.Byte);
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = (short)8 };
            var p5 = new NpgsqlParameter { ParameterName = "p5", Value = (byte)8  };
            Assert.That(p4.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Smallint));
            Assert.That(p4.DbType, Is.EqualTo(DbType.Int16));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            p1.Value = p2.Value = p3.Value = (long)8;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetInt16(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt32(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt64(i),                 Is.EqualTo(8));
                Assert.That(reader.GetByte(i),                  Is.EqualTo(8));
                Assert.That(reader.GetFloat(i),                 Is.EqualTo(8.0f));
                Assert.That(reader.GetDouble(i),                Is.EqualTo(8.0d));
                Assert.That(reader.GetDecimal(i),               Is.EqualTo(8.0m));
                Assert.That(reader.GetValue(i),                 Is.EqualTo(8));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof(short)));
            }

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void Int32()
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
            var p2 = new NpgsqlParameter("p2", DbType.Int32);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = 8 };
            Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
            Assert.That(p3.DbType, Is.EqualTo(DbType.Int32));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = (long)8;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetInt32(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt64(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt16(i),                 Is.EqualTo(8));
                Assert.That(reader.GetByte(i),                  Is.EqualTo(8));
                Assert.That(reader.GetFloat(i),                 Is.EqualTo(8.0f));
                Assert.That(reader.GetDouble(i),                Is.EqualTo(8.0d));
                Assert.That(reader.GetDecimal(i),               Is.EqualTo(8.0m));
                Assert.That(reader.GetValue(i),                 Is.EqualTo(8));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof(int)));
            }

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
        public void Int64()
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Bigint);
            var p2 = new NpgsqlParameter("p2", DbType.Int64);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = (long)8 };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = (short)8;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetInt64(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt16(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt32(i),                 Is.EqualTo(8));
                Assert.That(reader.GetByte(i),                  Is.EqualTo(8));
                Assert.That(reader.GetFloat(i),                 Is.EqualTo(8.0f));
                Assert.That(reader.GetDouble(i),                Is.EqualTo(8.0d));
                Assert.That(reader.GetDecimal(i),               Is.EqualTo(8.0m));
                Assert.That(reader.GetValue(i),                 Is.EqualTo(8));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof(long)));
            }

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void Double()
        {
            const double expected = 4.123456789012345;
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Double);
            var p2 = new NpgsqlParameter("p2", DbType.Double);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetDouble(i),    Is.EqualTo(expected).Within(10E-07));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(double)));
            }
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void Float()
        {
            const float expected = .123456F;
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Real);
            var p2 = new NpgsqlParameter("p2", DbType.Single);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFloat(i),     Is.EqualTo(expected).Within(10E-07));
                Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(float)));
            }

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void Numeric()
        {
            var cmd = new NpgsqlCommand("SELECT '-1234567.890123'::numeric", Conn);
            var result = cmd.ExecuteScalar();
            Assert.AreEqual(-1234567.890123M, result);

            cmd = new NpgsqlCommand("SELECT '" + string.Join("", Enumerable.Range(0, 131072).Select(i => "1")) + "." + string.Join("", Enumerable.Range(0, 16383).Select(i => "1")) + "'::numeric::text", Conn);
            using (var rdr = cmd.ExecuteReader())
            {
                rdr.Read();
            }


            var decimals = new decimal[] { 0, 1, -1, 2, -2, decimal.MaxValue, decimal.MinValue, 9999, 10000, -0.0001M, 0.00001M, 0.00000000111143243221M, 4372894738294782934.5832947839247M, 7483927483400000000000M };

            cmd = new NpgsqlCommand("SELECT " + string.Join(", ", Enumerable.Range(0, decimals.Length).Select(i => "@p" + i.ToString())), Conn);
            for (var i = 0; i < decimals.Length; i++)
            {
                cmd.Parameters.Add(new NpgsqlParameter("p" + i, NpgsqlDbType.Numeric) { Value = decimals[i] });
            }
            using (var rdr = cmd.ExecuteReader())
            {
                rdr.Read();
                for (var i = 0; i < decimals.Length; i++)
                    Assert.AreEqual(decimals[i], rdr.GetValue(i));
            }
            
            cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Numeric);
            var p2 = new NpgsqlParameter("p2", DbType.Decimal);
            var p3 = new NpgsqlParameter("p3", DbType.VarNumeric);
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = (decimal)8 };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            p1.Value = p2.Value = p3.Value = 8;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetDecimal(i),               Is.EqualTo(8.0m));
                Assert.That(reader.GetInt32(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt64(i),                 Is.EqualTo(8));
                Assert.That(reader.GetInt16(i),                 Is.EqualTo(8));
                Assert.That(reader.GetByte(i),                  Is.EqualTo(8));
                Assert.That(reader.GetFloat(i),                 Is.EqualTo(8.0f));
                Assert.That(reader.GetDouble(i),                Is.EqualTo(8.0d));
                Assert.That(reader.GetValue(i),                 Is.EqualTo(8));
                Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof(decimal)));
            }

            reader.Dispose();
            cmd.Dispose();
        }

        // Older tests

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
