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

namespace NpgsqlTests
{
    public class SimpleTypeTests : TestBase
    {
        #region Numeric Types
        // http://www.postgresql.org/docs/9.3/static/datatype-numeric.html

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

        [Test, Description("Tests some types which are aliased to int32")]
        [TestCase("oid")]
        public void ReadInt32Aliases(string typename)
        {
            const int expected = 8;
            var cmd = new NpgsqlCommand(String.Format("SELECT {0}::{1}", expected, typename), Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetInt32(0), Is.EqualTo(expected));
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

        #endregion

        #region Boolean Type
        // http://www.postgresql.org/docs/9.3/static/datatype-boolean.html

        [Test]
        public void ReadBool([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT TRUE::BOOLEAN, FALSE::BOOLEAN", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetBoolean(0), Is.True);
            Assert.That(reader.GetBoolean(1), Is.False);
            Assert.That(reader.GetValue(0), Is.True);
            Assert.That(reader.GetProviderSpecificValue(0), Is.True);
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(bool)));
            reader.Close();
            cmd.Dispose();
        }

        #endregion

        #region Monetary Types
        // http://www.postgresql.org/docs/9.3/static/datatype-money.html

        [Test]
        public void ReadMoney([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 12345.12::MONEY, '-10.5'::MONEY", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetDecimal(0), Is.EqualTo(12345.12m));
            Assert.That(reader.GetValue(0), Is.EqualTo(12345.12m));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(12345.12m));
            Assert.That(reader.GetDecimal(1), Is.EqualTo(-10.5m));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(decimal)));
            reader.Close();
            cmd.Dispose();
        }

        #endregion

        #region Network Address Types
        // http://www.postgresql.org/docs/9.3/static/datatype-net-types.html

        [Test]
        public void ReadInet()
        {
            var expectedIp = IPAddress.Parse("192.168.1.1");
            var expectedInet = new NpgsqlInet(expectedIp, 24);
            var cmd = new NpgsqlCommand("SELECT '192.168.1.1/24'::INET", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Regular type (IPAddress)
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));
            Assert.That(reader.GetFieldValue<IPAddress>(0), Is.EqualTo(expectedIp));
            Assert.That(reader[0], Is.EqualTo(expectedIp));
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedIp));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IPAddress)));

            // Provider-specific type (NpgsqlInet)
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetFieldValue<NpgsqlInet>(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetString(0), Is.EqualTo(expectedInet.ToString()));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadMacaddr()
        {
            var expected = PhysicalAddress.Parse("08-00-2B-01-02-03");
            var cmd = new NpgsqlCommand("SELECT '08-00-2b-01-02-03'::MACADDR", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldValue<PhysicalAddress>(0), Is.EqualTo(expected));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetString(0), Is.EqualTo(expected.ToString()));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(PhysicalAddress)));

            reader.Dispose();
            cmd.Dispose();
        }

        #endregion

        #region UUID Type

        [Test]
        public void ReadUuid()
        {
            var expected = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
            var cmd = new NpgsqlCommand("SELECT 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11'::UUID", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetGuid(0),             Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<Guid>(0), Is.EqualTo(expected));
            Assert.That(reader.GetValue(0),            Is.EqualTo(expected));
            Assert.That(reader.GetString(0),           Is.EqualTo(expected.ToString()));
            Assert.That(reader.GetFieldType(0),        Is.EqualTo(typeof(Guid)));
            reader.Dispose();
            cmd.Dispose();
        }

        #endregion

        [Test]
        public void ReadInternalChar([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT typdelim FROM pg_type WHERE typname='int4'", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetChar(0), Is.EqualTo(','));
            Assert.That(reader.GetValue(0), Is.EqualTo(','));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(','));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(char)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test, Description("Makes sure that types without a handler can still be read as strings")]
        public void ReadUnknownType([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            const string expected = "(1,2)";
            var cmd = new NpgsqlCommand("SELECT '(1,2)'::POINT", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo(expected));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<char[]>(0), Is.EqualTo(expected.ToCharArray()));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        [MinPgVersion(9, 2, 0, "JSON data type not yet introduced")]
        public void InsertJsonValueDataType()
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_json) VALUES (:param)", Conn))
            {
                cmd.Parameters.AddWithValue("param", @"{ ""Key"" : ""Value"" }");
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Json;
                Assert.That(cmd.ExecuteNonQuery(), Is.EqualTo(1));
            }
        }

        [Test]
        [MinPgVersion(9, 4, 0, "JSONB data type not yet introduced")]
        public void InsertJsonbValueDataType()
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_jsonb) VALUES (:param)", Conn))
            {
                cmd.Parameters.AddWithValue("param", @"{ ""Key"" : ""Value"" }");
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Jsonb;
                Assert.That(cmd.ExecuteNonQuery(), Is.EqualTo(1));
            }
        }

        [Test]
        [MinPgVersion(9, 1, 0, "HSTORE data type not yet introduced")]
        public void InsertHstoreValueDataType()
        {
            CreateSchema("hstore");
            ExecuteNonQuery(@"CREATE EXTENSION IF NOT EXISTS hstore WITH SCHEMA hstore");
            ExecuteNonQuery(@"ALTER TABLE data DROP COLUMN IF EXISTS field_hstore");
            try
            {
                ExecuteNonQuery(@"ALTER TABLE data ADD COLUMN field_hstore hstore.HSTORE");
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "42704")
                    TestUtil.Inconclusive("HSTORE does not seem to be installed at the backend");
            }

            ExecuteNonQuery(@"SET search_path = public, hstore");
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_hstore) VALUES (:param)", Conn))
            {
                cmd.Parameters.AddWithValue("param", @"""a"" => 3, ""b"" => 4");
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Hstore;
                Assert.That(cmd.ExecuteNonQuery(), Is.EqualTo(1));
            }
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
        public void TestBoolParameter1()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", Conn);
            var p0 = new NpgsqlParameter(":a", true);
            // with setting pramater type
            p0.DbType = DbType.Boolean;
            command.Parameters.Add(p0);
            command.ExecuteScalar();
        }

        [Test]
        public void TestBoolParameter2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", Conn);
            var p0 = new NpgsqlParameter(":a", true);
            // not setting parameter type
            command.Parameters.Add(p0);
            command.ExecuteScalar();
        }

        private void TestBoolParameter_Internal(bool prepare)
        {
            // Add test for prepared queries with bool parameter.
            // This test was created based on a report from Andrus Moor in the help forum:
            // http://pgfoundry.org/forum/forum.php?thread_id=15672&forum_id=519&group_id=1000140

            var command = new NpgsqlCommand("select :boolValue", Conn);

            command.Parameters.Add(":boolValue", NpgsqlDbType.Boolean);

            if (prepare)
            {
                command.Prepare();
            }

            command.Parameters["boolvalue"].Value = false;

            Assert.IsFalse((bool)command.ExecuteScalar());

            command.Parameters["boolvalue"].Value = true;

            Assert.IsTrue((bool)command.ExecuteScalar());
        }

        [Test]
        public void TestBoolParameter()
        {
            TestBoolParameter_Internal(false);
        }

        [Test]
        public void TestBoolParameterPrepared()
        {
            TestBoolParameter_Internal(true);
        }

        [Test]
        public void TestBoolParameterPrepared_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                TestBoolParameter_Internal(true);
            }
        }

        [Test]
        [Ignore]
        public void TestBoolParameterPrepared2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            var command = new NpgsqlCommand("select :boolValue", Conn);
            var p0 = new NpgsqlParameter(":boolValue", false);
            // not setting parameter type
            command.Parameters.Add(p0);
            command.Prepare();

            Assert.IsFalse((bool)command.ExecuteScalar());
        }

        [Test]
        public void TestUUIDDataType()
        {
            const string createTable =
                @"DROP TABLE if exists public.person;
                  CREATE TABLE public.person (
                    person_id serial PRIMARY KEY NOT NULL,
                    person_uuid uuid NOT NULL
                  ) WITH(OIDS=FALSE);";
            var command = new NpgsqlCommand(createTable, Conn);
            command.ExecuteNonQuery();

            NpgsqlParameter uuidDbParam = new NpgsqlParameter(":param1", NpgsqlDbType.Uuid);
            uuidDbParam.Value = Guid.NewGuid();

            command = new NpgsqlCommand(@"INSERT INTO person (person_uuid) VALUES (:param1);", Conn);
            command.Parameters.Add(uuidDbParam);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("SELECT person_uuid::uuid FROM person LIMIT 1", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(typeof(Guid), result.GetType());
        }

        public SimpleTypeTests(string backendVersion) : base(backendVersion) { }
    }
}
