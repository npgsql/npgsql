using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL types which don't fit elsewhere
    /// </summary>
    class MiscTypeTests : TestBase
    {
        /// <summary>
        /// http://www.postgresql.org/docs/9.4/static/datatype-boolean.html
        /// </summary>
        /// <param name="prepare"></param>
        [Test, Description("Roundtrips a bool")]
        public void Bool()
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Boolean);
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Boolean);
            var p3 = new NpgsqlParameter("p3", DbType.Boolean);
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = true };
            Assert.That(p4.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Boolean));
            Assert.That(p4.DbType, Is.EqualTo(DbType.Boolean));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            p1.Value = false;
            p2.Value = p3.Value = true;
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader.GetBoolean(0), Is.False);

            for (var i = 1; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetBoolean(i),               Is.True);
                Assert.That(reader.GetValue(i),                 Is.True);
                Assert.That(reader.GetProviderSpecificValue(i), Is.True);
                Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof (bool)));
            }

            reader.Close();
            cmd.Dispose();
        }

        /// <summary>
        /// http://www.postgresql.org/docs/9.4/static/datatype-money.html
        /// </summary>
        [Test]
        public void ReadMoney()
        {
            var cmd = new NpgsqlCommand("SELECT 12345.12::MONEY, '-10.5'::MONEY", Conn);
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

        /// <summary>
        /// http://www.postgresql.org/docs/9.4/static/datatype-uuid.html
        /// </summary>
        [Test, Description("Roundtrips a UUID")]
        public void Uuid()
        {
            var expected = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Uuid);
            var p2 = new NpgsqlParameter("p2", DbType.Guid);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            p1.Value = p2.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetGuid(i),             Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<Guid>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i),            Is.EqualTo(expected));
                Assert.That(reader.GetString(i),           Is.EqualTo(expected.ToString()));
                Assert.That(reader.GetFieldType(i),        Is.EqualTo(typeof(Guid)));
            }

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void ReadInternalChar()
        {
            var cmd = new NpgsqlCommand("SELECT typdelim FROM pg_type WHERE typname='int4'", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetChar(0), Is.EqualTo(','));
            Assert.That(reader.GetValue(0), Is.EqualTo(','));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(','));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(char)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test, Description("Makes sure that the PostgreSQL 'unknown' type (OID 705) is read properly")]
        public void ReadUnknown()
        {
            const string expected = "some_text";
            var cmd = new NpgsqlCommand(String.Format("SELECT '{0}'", expected), Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo(expected));
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<char[]>(0), Is.EqualTo(expected.ToCharArray()));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test, Description("Roundtrips a null value")]
        public void Null()
        {
            var cmd = new NpgsqlCommand("SELECT @p::INT4", Conn);
            var p = new NpgsqlParameter("p", NpgsqlDbType.Integer);
            cmd.Parameters.Add(p);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.IsDBNull(0));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(int)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        [MinPgVersion(9, 2, 0, "JSON data type not yet introduced")]
        public void Json()
        {
            const string expected = @"{ ""Key"" : ""Value"" }";
            using (var cmd = new NpgsqlCommand("SELECT @p", Conn))
            {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Json, expected);
                var reader = cmd.ExecuteReader();
                reader.Read();
                Assert.That(reader.GetString(0), Is.EqualTo(expected));
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
                reader.Close();
            }
        }

        [Test]
        [MinPgVersion(9, 4, 0, "JSONB data type not yet introduced")]
        public void Jsonb()
        {
            const string expected = @"{""Key"": ""Value""}";
            using (var cmd = new NpgsqlCommand("SELECT @p", Conn)) {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Jsonb, expected);
                var reader = cmd.ExecuteReader();
                reader.Read();
                Assert.That(reader.GetString(0), Is.EqualTo(expected));
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
                reader.Close();
            }
        }

        [Test]
        [MinPgVersion(9, 1, 0, "HSTORE data type not yet introduced")]
        public void Hstore()
        {
            ExecuteNonQuery(@"CREATE EXTENSION IF NOT EXISTS hstore");

            var expected = new Dictionary<string, string> {
              {"a", "3"},
              {"b", null},
              {"cd", "hello"}
            };

            using (var cmd = new NpgsqlCommand("SELECT @p", Conn)) {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Hstore, expected);
                var reader = cmd.ExecuteReader();
                reader.Read();
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(IDictionary<string, string>)));
                Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                Assert.That(reader.GetString(0), Is.EqualTo(@"""a""=>""3"",""b""=>NULL,""cd""=>""hello"""));
                reader.Close();
            }
        }

        #region Unrecognized types

        static void CheckUnrecognizedType()
        {
            Assert.That(TypeHandlerRegistry.HandlerTypes.Values.All(x => x.Mapping.PgName != "regproc"), "Test requires an unrecognized type to work");
        }

        [Test, Description("Attempts to retrieve an unrecognized type without marking it as unknown, triggering an exception")]
        public void UnrecognizedBinary()
        {
            CheckUnrecognizedType();
            var cmd = new NpgsqlCommand("SELECT typinput FROM pg_type WHERE typname='bool'", Conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<NotSupportedException>());
        }

        [Test, Description("Retrieves a type as an unknown type, i.e. untreated string")]
        public void AllResultTypesAreUnknown()
        {
            CheckUnrecognizedType();

            // Fetch as text to have something the value to assert against
            var expected = (string)ExecuteScalar("SELECT typinput::TEXT FROM pg_type WHERE typname='bool'");

            var cmd = new NpgsqlCommand("SELECT typinput FROM pg_type WHERE typname='bool'", Conn);
            cmd.AllResultTypesAreUnknown = true;
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
            Assert.That(reader.GetString(0), Is.EqualTo(expected));
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("Mixes and matches an unknown type with a known type")]
        public void UnknownResultTypeList()
        {
            CheckUnrecognizedType();

            // Fetch as text to have something the value to assert against
            var expected = (string)ExecuteScalar("SELECT typinput::TEXT FROM pg_type WHERE typname='bool'");

            var cmd = new NpgsqlCommand("SELECT typinput, 8 FROM pg_type WHERE typname='bool'", Conn);
            cmd.UnknownResultTypeList = new[] { true, false };
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
            Assert.That(reader.GetString(0), Is.EqualTo(expected));
            Assert.That(reader.GetInt32(1), Is.EqualTo(8));
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("Sends a null value parameter with no NpgsqlDbType or DbType, but with context for the backend to handle it")]
        public void UnrecognizedNull()
        {
            var cmd = new NpgsqlCommand("SELECT @p::TEXT", Conn);
            var p = new NpgsqlParameter("p", null);
            cmd.Parameters.Add(p);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.IsDBNull(0));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
            reader.Dispose();
            cmd.Dispose();
        }

        [Test, Description("Sends a value parameter with an explicit NpgsqlDbType.Unknown, but with context for the backend to handle it")]
        public void SendUnknown()
        {
            var cmd = new NpgsqlCommand("SELECT @p::INT4", Conn);
            var p = new NpgsqlParameter("p", "8");
            cmd.Parameters.Add(p);
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(int)));
            Assert.That(reader.GetInt32(0), Is.EqualTo(8));
            reader.Dispose();
            cmd.Dispose();
        }

        #endregion

        // Older tests

        [Test]
        public void Bug1011085()
        {
            // Money format is not set in accordance with the system locale format
            var command = new NpgsqlCommand("select :moneyvalue", Conn);
            var expectedValue = 8.99m;
            command.Parameters.Add("moneyvalue", NpgsqlDbType.Money).Value = expectedValue;
            var result = (Decimal)command.ExecuteScalar();
            Assert.AreEqual(expectedValue, result);

            expectedValue = 100m;
            command.Parameters[0].Value = expectedValue;
            result = (Decimal)command.ExecuteScalar();
            Assert.AreEqual(expectedValue, result);

            expectedValue = 72.25m;
            command.Parameters[0].Value = expectedValue;
            result = (Decimal)command.ExecuteScalar();
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void TestXmlParameter()
        {
            TestXmlParameter_Internal(false);
        }

        [Test]
        public void TestXmlParameterPrepared()
        {
            TestXmlParameter_Internal(true);
        }

        private void TestXmlParameter_Internal(bool prepare)
        {
            using (var command = new NpgsqlCommand("select @PrecisionXML", Conn)) {
                var sXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <strings type=\"array\"> <string> this is a test with ' single quote </string></strings>";
                var parameter = command.CreateParameter();
                parameter.DbType = DbType.Xml;  // To make it work we need to use DbType.String; and then CAST it in the sSQL: cast(@PrecisionXML as xml)
                parameter.ParameterName = "@PrecisionXML";
                parameter.Value = sXML;
                command.Parameters.Add(parameter);

                if (prepare)
                    command.Prepare();
                command.ExecuteScalar();
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

        [Test]
        public void Range()
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Range | NpgsqlDbType.Integer) { Value = NpgsqlRange<int>.Empty() };
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = new NpgsqlRange<int>(1, 10) };
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = new NpgsqlRange<int>(1, false, 10, false) };
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = new NpgsqlRange<int>(0, false, true, 10, false, false) };
            Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Range | NpgsqlDbType.Integer));
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader[0].ToString(), Is.EqualTo("empty"));
            Assert.That(reader[1].ToString(), Is.EqualTo("[1,11)"));
            Assert.That(reader[2].ToString(), Is.EqualTo("[2,10)"));
            Assert.That(reader[3].ToString(), Is.EqualTo("(,10)"));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void TestRange()
        {
            using (var cmd = Conn.CreateCommand())
            {
                object obj;

                cmd.CommandText = "select '[2,10)'::int4range";
                cmd.Prepare();
                obj = cmd.ExecuteScalar();
                Assert.AreEqual(new NpgsqlRange<int>(2, true, false, 10, false, false), obj);

                cmd.CommandText = "select array['[2,10)'::int4range, '[3,9)'::int4range]";
                cmd.Prepare();
                obj = cmd.ExecuteScalar();
                Assert.AreEqual(new NpgsqlRange<int>(3, true, false, 9, false, false), ((NpgsqlRange<int>[])obj)[1]);
            }
        }

        [Test]
        public void OidVector()
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select '1 2 3'::oidvector, :p1";
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Oidvector, new uint[] { 4, 5, 6 });
                using (var rdr = cmd.ExecuteReader())
                {
                    rdr.Read();
                    Assert.AreEqual(typeof(uint[]), rdr.GetValue(0).GetType());
                    Assert.AreEqual(typeof(uint[]), rdr.GetValue(1).GetType());
                    Assert.IsTrue(rdr.GetFieldValue<uint[]>(0).SequenceEqual(new uint[] { 1, 2, 3 }));
                    Assert.IsTrue(rdr.GetFieldValue<uint[]>(1).SequenceEqual(new uint[] { 4, 5, 6 }));
                }
            }
        }

        [Test]
        public void TsVector()
        {
            using (var cmd = Conn.CreateCommand())
            {
                var inputVec = NpgsqlTsVector.Parse(" a:12345C  a:24D a:25B b c d 1 2 a:25A,26B,27,28");

                cmd.CommandText = "Select :p";
                cmd.Parameters.AddWithValue("p", inputVec);
                var outputVec = cmd.ExecuteScalar();
                Assert.AreEqual(inputVec.ToString(), outputVec.ToString());
            }
        }

        [Test]
        public void TsQuery()
        {
            using (var cmd = Conn.CreateCommand())
            {
                var query = NpgsqlTsQuery.Parse("(a & !(c | d)) & (!!a&b) | ä | d | e");

                cmd.CommandText = "Select :p";
                cmd.Parameters.AddWithValue("p", query);
                var output = cmd.ExecuteScalar();
                Assert.AreEqual(query.ToString(), output.ToString());
            }
        }

        public MiscTypeTests(string backendVersion) : base(backendVersion) {}
    }
}
