﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL types which don't fit elsewhere
    /// </summary>
    class MiscTypeTests : TestBase
    {
        /// <summary>
        /// http://www.postgresql.org/docs/current/static/datatype-boolean.html
        /// </summary>
        /// <param name="prepare"></param>
        [Test, Description("Roundtrips a bool")]
        public void Bool()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn))
            {
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
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    Assert.That(reader.GetBoolean(0), Is.False);

                    for (var i = 1; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetBoolean(i), Is.True);
                        Assert.That(reader.GetValue(i), Is.True);
                        Assert.That(reader.GetProviderSpecificValue(i), Is.True);
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof (bool)));
                        Assert.That(reader.GetDataTypeName(i), Is.EqualTo("bool"));
                    }
                }
            }
        }

        /// <summary>
        /// http://www.postgresql.org/docs/current/static/datatype-money.html
        /// </summary>
        [Test]
        public void Money()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var expected1 = 12345.12m;
                var expected2 = -10.5m;
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Money, expected1);
                cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Currency) {Value = expected2});
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDecimal(0), Is.EqualTo(12345.12m));
                    Assert.That(reader.GetValue(0), Is.EqualTo(12345.12m));
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(12345.12m));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (decimal)));

                    Assert.That(reader.GetDecimal(1), Is.EqualTo(-10.5m));
                }
            }
        }

        /// <summary>
        /// http://www.postgresql.org/docs/current/static/datatype-uuid.html
        /// </summary>
        [Test, Description("Roundtrips a UUID")]
        public void Uuid()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                var expected = new Guid("a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11");
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Uuid);
                var p2 = new NpgsqlParameter("p2", DbType.Guid);
                var p3 = new NpgsqlParameter {ParameterName = "p3", Value = expected};
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetGuid(i), Is.EqualTo(expected));
                        Assert.That(reader.GetFieldValue<Guid>(i), Is.EqualTo(expected));
                        Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                        Assert.That(reader.GetString(i), Is.EqualTo(expected.ToString()));
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof (Guid)));
                    }
                }
            }
        }

        [Test]
        public void ReadInternalChar()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT typdelim FROM pg_type WHERE typname='int4'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetChar(0), Is.EqualTo(','));
                Assert.That(reader.GetValue(0), Is.EqualTo(','));
                Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(','));
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(char)));
            }
        }

        [Test, Description("Makes sure that the PostgreSQL 'unknown' type (OID 705) is read properly")]
        public void ReadUnknown()
        {
            const string expected = "some_text";
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($"SELECT '{expected}'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetString(0), Is.EqualTo(expected));
                Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<char[]>(0), Is.EqualTo(expected.ToCharArray()));
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));
            }
        }

        [Test, Description("Roundtrips a null value")]
        public void Null()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::INT4", conn))
            {
                cmd.Parameters.AddWithValue("p", DBNull.Value);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.IsDBNull(0));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (int)));
                }
            }
        }

        [Test]
        [MinPgVersion(9, 2, 0, "JSON data type not yet introduced")]
        public void Json()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                const string expected = @"{ ""Key"" : ""Value"" }";
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Json, expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetString(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));

                    using (var textReader = reader.GetTextReader(0))
                        Assert.That(textReader.ReadToEnd(), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        [MinPgVersion(9, 4, 0, "JSONB data type not yet introduced")]
        public void Jsonb()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var sb = new StringBuilder();
                sb.Append(@"{""Key"": """);
                sb.Append('x', conn.BufferSize);
                sb.Append(@"""}");
                var value = sb.ToString();
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Jsonb, value);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetString(0), Is.EqualTo(value));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(string)));

                    using (var textReader = reader.GetTextReader(0))
                        Assert.That(textReader.ReadToEnd(), Is.EqualTo(value));
                }
            }
        }

        [Test]
        [MinPgVersion(9, 1, 0, "HSTORE data type not yet introduced")]
        public void Hstore()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE EXTENSION IF NOT EXISTS hstore");
                conn.ReloadTypes();

                var expected = new Dictionary<string, string> {
                    {"a", "3"},
                    {"b", null},
                    {"cd", "hello"}
                };

                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Hstore, expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (IDictionary<string, string>)));
                        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                        Assert.That(reader.GetString(0), Is.EqualTo(@"""a""=>""3"",""b""=>NULL,""cd""=>""hello"""));
                    }
                }
            }

        }

        [Test]
        public void RegType()
        {
            const uint expected = 8u;
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Regtype, expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(uint)));
                    Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                }
            }
        }

        [Test, Description("PostgreSQL records should be returned as arrays of objects")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/724")]
        public void Record()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE FUNCTION pg_temp.foo () RETURNS RECORD AS $$ SELECT 1,2 $$ LANGUAGE SQL");
                using (var cmd = new NpgsqlCommand("SELECT pg_temp.foo()", conn))
                {
                    var record = cmd.ExecuteScalar();
                    Assert.That(record, Is.TypeOf<object[]>());
                    var array = (object[]) record;
                    Assert.That(array[0], Is.EqualTo(1));
                    Assert.That(array[1], Is.EqualTo(2));
                }
            }
        }

        [Test, Description("Makes sure that setting DbType.Object makes Npgsql infer the type")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/694")]
        public void DbTypeCausesInference()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName="p", DbType = DbType.Object, Value = 3 });
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(3));
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
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT typinput FROM pg_type WHERE typname='bool'", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<NotSupportedException>());
                }
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Retrieves a type as an unknown type, i.e. untreated string")]
        public void AllResultTypesAreUnknown()
        {
            CheckUnrecognizedType();
            using (var conn = OpenConnection())
            {
                // Fetch as text to have something the value to assert against
                var expected = (string)conn.ExecuteScalar("SELECT typinput::TEXT FROM pg_type WHERE typname='bool'");

                using (var cmd = new NpgsqlCommand("SELECT typinput FROM pg_type WHERE typname='bool'", conn))
                {
                    cmd.AllResultTypesAreUnknown = true;
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (string)));
                        Assert.That(reader.GetString(0), Is.EqualTo(expected));
                    }
                }
            }
        }

        [Test, Description("Mixes and matches an unknown type with a known type")]
        public void UnknownResultTypeList()
        {
            CheckUnrecognizedType();
            using (var conn = OpenConnection())
            {
                // Fetch as text to have something the value to assert against
                var expected = (string) conn.ExecuteScalar("SELECT typinput::TEXT FROM pg_type WHERE typname='bool'");

                using (var cmd = new NpgsqlCommand("SELECT typinput, 8 FROM pg_type WHERE typname='bool'", conn))
                {
                    cmd.UnknownResultTypeList = new[] {true, false};
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (string)));
                        Assert.That(reader.GetString(0), Is.EqualTo(expected));
                        Assert.That(reader.GetInt32(1), Is.EqualTo(8));
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/711")]
        public void KnownTypeAsUnknown()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 8", conn))
            {
                cmd.AllResultTypesAreUnknown = true;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo("8"));
            }
        }

        [Test, Description("Sends a null value parameter with no NpgsqlDbType or DbType, but with context for the backend to handle it")]
        public void UnrecognizedNull()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::TEXT", conn))
            {
                var p = new NpgsqlParameter("p", DBNull.Value);
                cmd.Parameters.Add(p);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.IsDBNull(0));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (string)));
                }
            }
        }

        [Test, Description("Sends a value parameter with an explicit NpgsqlDbType.Unknown, but with context for the backend to handle it")]
        public void SendUnknown()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::INT4", conn))
            {
                var p = new NpgsqlParameter("p", "8");
                cmd.Parameters.Add(p);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (int)));
                    Assert.That(reader.GetInt32(0), Is.EqualTo(8));
                }
            }
        }

        #endregion

        [Test, MinPgVersion(9, 1, 0)]
        public void Int2Vector()
        {
            var expected = new short[] { 4, 5, 6 };
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT @p::int2vector";
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Int2Vector, expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldValue<short[]>(0), Is.EqualTo(expected));
                }
            }
        }

        // Older tests

        [Test]
        public void Bug1011085()
        {
            // Money format is not set in accordance with the system locale format
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select :moneyvalue", conn))
            {
                var expectedValue = 8.99m;
                command.Parameters.Add("moneyvalue", NpgsqlDbType.Money).Value = expectedValue;
                var result = (Decimal) command.ExecuteScalar();
                Assert.AreEqual(expectedValue, result);

                expectedValue = 100m;
                command.Parameters[0].Value = expectedValue;
                result = (Decimal) command.ExecuteScalar();
                Assert.AreEqual(expectedValue, result);

                expectedValue = 72.25m;
                command.Parameters[0].Value = expectedValue;
                result = (Decimal) command.ExecuteScalar();
                Assert.AreEqual(expectedValue, result);
            }
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
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select @PrecisionXML", conn))
            {
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
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", conn))
            {
                var p0 = new NpgsqlParameter(":a", true);
                // with setting pramater type
                p0.DbType = DbType.Boolean;
                command.Parameters.Add(p0);
                command.ExecuteScalar();
            }
        }

        [Test]
        public void TestBoolParameter2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", conn))
            {
                var p0 = new NpgsqlParameter(":a", true);
                // not setting parameter type
                command.Parameters.Add(p0);
                command.ExecuteScalar();
            }
        }

        private void TestBoolParameter_Internal(bool prepare)
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select :boolValue", conn))
            {
                // Add test for prepared queries with bool parameter.
                // This test was created based on a report from Andrus Moor in the help forum:
                // http://pgfoundry.org/forum/forum.php?thread_id=15672&forum_id=519&group_id=1000140

                command.Parameters.Add(":boolValue", NpgsqlDbType.Boolean);

                if (prepare)
                {
                    command.Prepare();
                }

                command.Parameters["boolvalue"].Value = false;

                Assert.IsFalse((bool) command.ExecuteScalar());

                command.Parameters["boolvalue"].Value = true;

                Assert.IsTrue((bool) command.ExecuteScalar());
            }
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
        [Ignore("")]
        public void TestBoolParameterPrepared2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select :boolValue", conn))
            {
                var p0 = new NpgsqlParameter(":boolValue", false);
                // not setting parameter type
                command.Parameters.Add(p0);
                command.Prepare();

                Assert.IsFalse((bool) command.ExecuteScalar());
            }
        }

        [Test]
        public void TestUUIDDataType()
        {
            using (var conn = OpenConnection())
            {
                const string createTable =
                    @"CREATE TEMP TABLE person (
                        person_id serial PRIMARY KEY NOT NULL,
                        person_uuid uuid NOT NULL
                      ) WITH(OIDS=FALSE);";
                var command = new NpgsqlCommand(createTable, conn);
                command.ExecuteNonQuery();

                NpgsqlParameter uuidDbParam = new NpgsqlParameter(":param1", NpgsqlDbType.Uuid);
                uuidDbParam.Value = Guid.NewGuid();

                command = new NpgsqlCommand(@"INSERT INTO person (person_uuid) VALUES (:param1);", conn);
                command.Parameters.Add(uuidDbParam);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand("SELECT person_uuid::uuid FROM person LIMIT 1", conn);
                var result = command.ExecuteScalar();
                Assert.AreEqual(typeof (Guid), result.GetType());
            }
        }

        [Test]
        public void OidVector()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
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
