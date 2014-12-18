using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
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

            // Provider-specific type (NpgsqlInet)
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(NpgsqlInet)));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetFieldValue<NpgsqlInet>(0), Is.EqualTo(expectedInet));
            Assert.That(reader.GetString(0), Is.EqualTo(expectedInet.ToString()));

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

        public SimpleTypeTests(string backendVersion) : base(backendVersion) { }
    }
}
