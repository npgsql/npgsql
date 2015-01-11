using System;
using System.Collections.Generic;
using System.Data;
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
        public void Bool([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Boolean);
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Boolean);
            var p3 = new NpgsqlParameter("p3", DbType.Boolean);
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = true };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
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

        /// <summary>
        /// http://www.postgresql.org/docs/9.4/static/datatype-uuid.html
        /// </summary>
        [Test, Description("Roundtrips a UUID")]
        public void Uuid([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            var expected = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Uuid);
            var p2 = new NpgsqlParameter("p2", DbType.Guid);
            var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
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
        public void TestOidVector()
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

        public MiscTypeTests(string backendVersion) : base(backendVersion) {}
    }
}
