#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Npgsql;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Tests
{
    [TestFixture(CommandBehavior.Default)]
    [TestFixture(CommandBehavior.SequentialAccess)]
    public class ReaderTests : TestBase
    {
        [Test]
        public void SeekColumns()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1,2,3", conn))
            using (var reader = cmd.ExecuteReader(Behavior))
            {
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                if (IsSequential)
                    Assert.That(() => reader.GetInt32(0), Throws.Exception.TypeOf<InvalidOperationException>());
                else
                    Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                Assert.That(reader.GetInt32(1), Is.EqualTo(2));
                if (IsSequential)
                    Assert.That(() => reader.GetInt32(0), Throws.Exception.TypeOf<InvalidOperationException>());
                else
                    Assert.That(reader.GetInt32(0), Is.EqualTo(1));
            }
        }

        [Test]
        public void NoResultSet()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id INT)");
                using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (8)", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    Assert.That(() => reader.GetOrdinal("foo"), Throws.Exception.TypeOf<InvalidOperationException>());
                    Assert.That(reader.Read(), Is.False);
                    Assert.That(() => reader.GetOrdinal("foo"), Throws.Exception.TypeOf<InvalidOperationException>());
                    Assert.That(reader.FieldCount, Is.EqualTo(0));
                    Assert.That(reader.NextResult(), Is.False);
                    Assert.That(() => reader.GetOrdinal("foo"), Throws.Exception.TypeOf<InvalidOperationException>());
                }

                using (var cmd = new NpgsqlCommand("SELECT 1; INSERT INTO data VALUES (8)", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.NextResult();
                    Assert.That(() => reader.GetOrdinal("foo"), Throws.Exception.TypeOf<InvalidOperationException>());
                    Assert.That(reader.Read(), Is.False);
                    Assert.That(() => reader.GetOrdinal("foo"), Throws.Exception.TypeOf<InvalidOperationException>());
                    Assert.That(reader.FieldCount, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void EmptyResultSet()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1 AS foo WHERE FALSE", conn))
            using (var reader = cmd.ExecuteReader(Behavior))
            {
                Assert.That(reader.Read(), Is.False);
                Assert.That(reader.FieldCount, Is.EqualTo(1));
                Assert.That(reader.GetOrdinal("foo"), Is.EqualTo(0));
                Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void FieldCount()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2,3", conn))
                {
                    using (var reader = cmd.ExecuteReader(Behavior))
                    {
                        Assert.That(reader.FieldCount, Is.EqualTo(1));
                        Assert.That(reader.Read(), Is.True);
                        Assert.That(reader.FieldCount, Is.EqualTo(1));
                        Assert.That(reader.Read(), Is.False);
                        Assert.That(reader.FieldCount, Is.EqualTo(1));
                        Assert.That(reader.NextResult(), Is.True);
                        Assert.That(reader.FieldCount, Is.EqualTo(2));
                        Assert.That(reader.NextResult(), Is.False);
                        Assert.That(reader.FieldCount, Is.EqualTo(0));
                    }

                    cmd.CommandText = "INSERT INTO data (int) VALUES (1)";
                    using (var reader = cmd.ExecuteReader(Behavior))
                    {
                        // Note MSDN docs that seem to say we should case -1 in this case: http://msdn.microsoft.com/en-us/library/system.data.idatarecord.fieldcount(v=vs.110).aspx
                        // But SqlClient returns 0
                        Assert.That(() => reader.FieldCount, Is.EqualTo(0));

                    }
                }
            }
        }

        [Test]
        public void RecordsAffected()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");

                var sb = new StringBuilder();
                for (var i = 0; i < 15; i++)
                    sb.Append($"INSERT INTO data (int) VALUES ({i});");
                var cmd = new NpgsqlCommand(sb.ToString(), conn);
                var reader = cmd.ExecuteReader(Behavior);
                reader.Close();
                Assert.That(reader.RecordsAffected, Is.EqualTo(15));

                cmd = new NpgsqlCommand("SELECT * FROM data", conn);
                reader = cmd.ExecuteReader(Behavior);
                reader.Close();
                Assert.That(reader.RecordsAffected, Is.EqualTo(-1));

                cmd = new NpgsqlCommand("UPDATE data SET int=int+1 WHERE int > 10", conn);
                reader = cmd.ExecuteReader(Behavior);
                reader.Close();
                Assert.That(reader.RecordsAffected, Is.EqualTo(4));

                cmd = new NpgsqlCommand("UPDATE data SET int=8 WHERE int=666", conn);
                reader = cmd.ExecuteReader(Behavior);
                reader.Close();
                Assert.That(reader.RecordsAffected, Is.EqualTo(0));

                cmd = new NpgsqlCommand("DELETE FROM data WHERE int > 10", conn);
                reader = cmd.ExecuteReader(Behavior);
                reader.Close();
                Assert.That(reader.RecordsAffected, Is.EqualTo(4));
            }
        }

        [Test]
        public void Statements()
        {
            // See also CommandTests.Statements()
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT) WITH OIDS");
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO data (name) VALUES ('a');" +
                    "UPDATE data SET name='b' WHERE name='doesnt_exist'",
                    conn)
                    )
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    Assert.That(reader.Statements, Has.Count.EqualTo(2));
                    Assert.That(reader.Statements[0].SQL, Is.EqualTo("INSERT INTO data (name) VALUES ('a')"));
                    Assert.That(reader.Statements[0].StatementType, Is.EqualTo(StatementType.Insert));
                    Assert.That(reader.Statements[0].Rows, Is.EqualTo(1));
                    Assert.That(reader.Statements[0].OID, Is.Not.EqualTo(0));
                    Assert.That(reader.Statements[1].SQL,
                        Is.EqualTo("UPDATE data SET name='b' WHERE name='doesnt_exist'"));
                    Assert.That(reader.Statements[1].StatementType, Is.EqualTo(StatementType.Update));
                    Assert.That(reader.Statements[1].Rows, Is.EqualTo(0));
                    Assert.That(reader.Statements[1].OID, Is.EqualTo(0));
                }

                using (var cmd = new NpgsqlCommand("SELECT name FROM data; DELETE FROM data", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.NextResult(); // Consume SELECT result set
                    Assert.That(reader.Statements, Has.Count.EqualTo(2));
                    Assert.That(reader.Statements[0].SQL, Is.EqualTo("SELECT name FROM data"));
                    Assert.That(reader.Statements[0].StatementType, Is.EqualTo(StatementType.Select));
                    Assert.That(reader.Statements[0].Rows, Is.EqualTo(1));
                    Assert.That(reader.Statements[0].OID, Is.EqualTo(0));
                    Assert.That(reader.Statements[1].SQL, Is.EqualTo("DELETE FROM data"));
                    Assert.That(reader.Statements[1].StatementType, Is.EqualTo(StatementType.Delete));
                    Assert.That(reader.Statements[1].Rows, Is.EqualTo(1));
                    Assert.That(reader.Statements[1].OID, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void GetStringWithParameter()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                const string text = "Random text";
                conn.ExecuteNonQuery($@"INSERT INTO data (name) VALUES ('{text}')");

                var command = new NpgsqlCommand("SELECT name FROM data WHERE name = :value;", conn);
                var param = new NpgsqlParameter
                {
                    ParameterName = "value",
                    DbType = DbType.String,
                    Size = text.Length,
                    Value = text
                };
                //param.NpgsqlDbType = NpgsqlDbType.Text;
                command.Parameters.Add(param);

                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    var result = dr.GetString(0);
                    Assert.AreEqual(text, result);
                }
            }
        }

        [Test]
        public void GetStringWithQuoteWithParameter()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('Text with '' single quote')");

                const string test = "Text with ' single quote";
                var command = new NpgsqlCommand("SELECT name FROM data WHERE name = :value;", conn);

                var param = new NpgsqlParameter();
                param.ParameterName = "value";
                param.DbType = DbType.String;
                //param.NpgsqlDbType = NpgsqlDbType.Text;
                param.Size = test.Length;
                param.Value = test;
                command.Parameters.Add(param);

                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    var result = dr.GetString(0);
                    Assert.AreEqual(test, result);
                }
            }
        }

        [Test]
        public void GetValueByName()
        {
            using (var conn = OpenConnection())
            {
                using (var command = new NpgsqlCommand(@"SELECT 'Random text' AS real_column", conn))
                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    Assert.That(dr["real_column"], Is.EqualTo("Random text"));
                    Assert.That(() => dr["non_existing"], Throws.Exception.TypeOf<IndexOutOfRangeException>());
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/794")]
        public void GetFieldType()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(int)));
                }
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", conn))
                {
                    cmd.AllResultTypesAreUnknown = true;
                    using (var reader = cmd.ExecuteReader(Behavior))
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(string)));
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1096")]
        public void GetFieldTypeSchemaOnly()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    reader.Read();
                    Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(int)));
                }
            }
        }

        [Test]
        public void GetPostgresType()
        {
            using (var conn = OpenConnection())
            {
                PostgresType intType;
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INTEGER AS some_column", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    intType = (PostgresBaseType)reader.GetPostgresType(0);
                    Assert.That(intType.Namespace, Is.EqualTo("pg_catalog"));
                    Assert.That(intType.Name, Is.EqualTo("integer"));
                    Assert.That(intType.FullName, Is.EqualTo("pg_catalog.integer"));
                    Assert.That(intType.DisplayName, Is.EqualTo("integer"));
                    Assert.That(intType.InternalName, Is.EqualTo("int4"));
                }

                using (var cmd = new NpgsqlCommand(@"SELECT '{1}'::INTEGER[] AS some_column", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    var intArrayType = (PostgresArrayType)reader.GetPostgresType(0);
                    Assert.That(intArrayType.Name, Is.EqualTo("integer[]"));
                    Assert.That(intArrayType.Element, Is.SameAs(intType));
                    Assert.That(intArrayType.DisplayName, Is.EqualTo("integer[]"));
                    Assert.That(intArrayType.InternalName, Is.EqualTo("_int4"));
                    Assert.That(intType.Array, Is.SameAs(intArrayType));
                }
            }
        }

        /// <seealso cref="ReaderNewSchemaTests.DataTypeName"/>
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/787")]
        [TestCase("integer")]
        [TestCase("real")]
        [TestCase("integer[]")]
        [TestCase("character varying(10)")]
        [TestCase("character varying")]
        [TestCase("character varying(10)[]")]
        [TestCase("character(10)")]
        [TestCase("character")]
        [TestCase("numeric(1000, 2)")]
        [TestCase("numeric(1000)")]
        [TestCase("numeric")]
        [TestCase("timestamp")]
        [TestCase("timestamp(2)")]
        [TestCase("timestamp(2) with time zone")]
        [TestCase("time")]
        [TestCase("time(2)")]
        [TestCase("time(2) with time zone")]
        [TestCase("interval")]
        [TestCase("interval(2)")]
        [TestCase("bit")]
        [TestCase("bit(3)")]
        [TestCase("bit varying")]
        [TestCase("bit varying(3)")]
        public void GetDataTypeName(string typeName)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($"SELECT NULL::{typeName} AS some_column", conn))
            using (var reader = cmd.ExecuteReader(Behavior))
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo(typeName));
            }
        }

        [Test]
        public void GetDataTypeNameEnum()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.my_enum AS ENUM ('one')");
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT 'one'::my_enum", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".my_enum"));
                }
            }
        }

        [Test]
        public void GetDataTypeNameDomain()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE DOMAIN pg_temp.my_domain AS VARCHAR(10)");
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT 'one'::my_domain", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    // In the RowDescription, PostgreSQL sends the type OID of the underlying type and not of the domain.
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("character varying(10)"));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/794")]
        public void GetDataTypeNameTypesUnknown()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INTEGER AS some_column", conn))
                {
                    cmd.AllResultTypesAreUnknown = true;
                    using (var reader = cmd.ExecuteReader(Behavior))
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer"));
                    }
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/791")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/794")]
        public void GetDataTypeOID()
        {
            using (var conn = OpenConnection())
            {
                var int4OID = conn.ExecuteScalar("SELECT oid FROM pg_type WHERE typname = 'int4'");
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeOID(0), Is.EqualTo(int4OID));
                }
                using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", conn))
                {
                    cmd.AllResultTypesAreUnknown = true;
                    using (var reader = cmd.ExecuteReader(Behavior))
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeOID(0), Is.EqualTo(int4OID));
                    }
                }
            }
        }

        [Test]
        public void GetName()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand(@"SELECT 1 AS some_column", conn))
            using (var dr = command.ExecuteReader(Behavior))
            {
                dr.Read();
                Assert.That(dr.GetName(0), Is.EqualTo("some_column"));
            }

        }

        [Test]
        public void GetFieldValueAsObject()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 'foo'::TEXT", conn))
            using (var reader = cmd.ExecuteReader(Behavior))
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<object>(0), Is.EqualTo("foo"));
            }
        }

        [Test]
        public void GetValues()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand(@"SELECT 'hello', 1, '2014-01-01'::DATE", conn))
            {
                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    var values = new object[4];
                    Assert.That(dr.GetValues(values), Is.EqualTo(3));
                    Assert.That(values, Is.EqualTo(new object[] { "hello", 1, new DateTime(2014, 1, 1), null }));
                }
                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    var values = new object[2];
                    Assert.That(dr.GetValues(values), Is.EqualTo(2));
                    Assert.That(values, Is.EqualTo(new object[] { "hello", 1 }));
                }
            }
        }

        [Test]
        public void GetProviderSpecificValues()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand(@"SELECT 'hello', 1, '2014-01-01'::DATE", conn))
            {
                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    var values = new object[4];
                    Assert.That(dr.GetProviderSpecificValues(values), Is.EqualTo(3));
                    Assert.That(values, Is.EqualTo(new object[] { "hello", 1, new NpgsqlDate(2014, 1, 1), null }));
                }
                using (var dr = command.ExecuteReader(Behavior))
                {
                    dr.Read();
                    var values = new object[2];
                    Assert.That(dr.GetProviderSpecificValues(values), Is.EqualTo(2));
                    Assert.That(values, Is.EqualTo(new object[] { "hello", 1 }));
                }
            }
        }

        [Test]
        public void ExecuteReaderGettingEmptyResultSetWithOutputParameter()
        {
            if (IsSequential)
                Assert.Pass("Not supported in sequential mode");
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var command = new NpgsqlCommand("SELECT * FROM data WHERE name = NULL;", conn);
                var param = new NpgsqlParameter("some_param", NpgsqlDbType.Varchar);
                param.Direction = ParameterDirection.Output;
                command.Parameters.Add(param);
                using (var dr = command.ExecuteReader(Behavior))
                    Assert.IsFalse(dr.NextResult());
            }
        }

        [Test]
        public void GetValueFromEmptyResultset()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                using (var command = new NpgsqlCommand("SELECT * FROM data WHERE name = :value;", conn))
                {
                    const string test = "Text single quote";
                    var param = new NpgsqlParameter();
                    param.ParameterName = "value";
                    param.DbType = DbType.String;
                    //param.NpgsqlDbType = NpgsqlDbType.Text;
                    param.Size = test.Length;
                    param.Value = test;
                    command.Parameters.Add(param);

                    using (var dr = command.ExecuteReader(Behavior))
                    {
                        dr.Read();
                        // This line should throw the invalid operation exception as the datareader will
                        // have an empty resultset.
                        Assert.That(() => Console.WriteLine(dr.IsDBNull(1)),
                            Throws.Exception.TypeOf<InvalidOperationException>());
                    }
                }
            }
        }

        [Test]
        public void ReadPastDataReaderEnd()
        {
            using (var conn = OpenConnection())
            {
                var command = new NpgsqlCommand("SELECT 1", conn);
                using (var dr = command.ExecuteReader(Behavior))
                {
                    while (dr.Read()) {}
                    Assert.That(() => dr[0], Throws.Exception.TypeOf<InvalidOperationException>());
                }
            }
        }

        [Test]
        public void SingleResult()
        {
            using (var conn = OpenConnection())
            {
                var cmd = new NpgsqlCommand(@"SELECT 1; SELECT 2", conn);
                var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);
                Assert.That(rdr.Read(), Is.True);
                Assert.That(rdr.GetInt32(0), Is.EqualTo(1));
                Assert.That(rdr.NextResult(), Is.False);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/400")]
        public void ExceptionThrownFromExecuteQuery([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"
                     CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
                        'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                     LANGUAGE 'plpgsql';
                ");

                using (var cmd = new NpgsqlCommand("SELECT pg_temp.emit_exception()", conn))
                {
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    Assert.That(() => cmd.ExecuteReader(Behavior), Throws.Exception.TypeOf<PostgresException>());
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1032")]
        public void ExceptionThrownFromNextResult([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"
                     CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
                        'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                     LANGUAGE 'plpgsql';
                ");

                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT pg_temp.emit_exception()", conn))
                {
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    using (var reader = cmd.ExecuteReader(Behavior))
                        Assert.That(() => reader.NextResult(), Throws.Exception.TypeOf<PostgresException>());
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/967")]
        public void NpgsqlExceptionReferencesStatement()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"
                     CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
                        'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                     LANGUAGE 'plpgsql';
                ");

                // Exception in single-statement command
                using (var cmd = new NpgsqlCommand("SELECT pg_temp.emit_exception()", conn))
                {
                    try
                    {
                        cmd.ExecuteReader(Behavior);
                        Assert.Fail();
                    }
                    catch (PostgresException e)
                    {
                        Assert.That(e.Statement, Is.SameAs(cmd.Statements[0]));
                    }
                }

                // Exception in multi-statement command
                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT pg_temp.emit_exception()", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    try
                    {
                        reader.NextResult();
                        Assert.Fail();
                    }
                    catch (PostgresException e)
                    {
                        Assert.That(e.Statement, Is.SameAs(cmd.Statements[1]));
                    }
                }
            }
        }

        [Test]
        public void SchemaOnlyReturnsNoData()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                Assert.That(reader.Read(), Is.False);
        }

        [Test]
        public void SchemaOnlyCommandBehaviorSupportFunctioncall()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION pg_temp.funcB() RETURNS SETOF integer as 'SELECT 1;' LANGUAGE 'sql';");
                var command = new NpgsqlCommand("pg_temp.funcb", conn) { CommandType = CommandType.StoredProcedure };
                using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var i = 0;
                    while (dr.Read())
                        i++;
                    Assert.AreEqual(0, i);
                }
            }
        }

        #region GetOrdinal

        [Test]
        public void GetOrdinal()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand(@"SELECT 0, 1 AS some_column WHERE 1=0", conn))
            using (var reader = command.ExecuteReader(Behavior))
            {
                Assert.That(reader.GetOrdinal("some_column"), Is.EqualTo(1));
                Assert.That(() => reader.GetOrdinal("doesn't_exist"), Throws.Exception.TypeOf<IndexOutOfRangeException>());
            }
        }

        [Test]
        public void GetOrdinalInsensitivity()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select 123 as FIELD1", conn))
            using (var reader = command.ExecuteReader(Behavior))
            {
                reader.Read();
                Assert.That(reader.GetOrdinal("fieLd1"), Is.EqualTo(0));
            }
        }

        [Test]
        public void GetOrdinalKanaInsensitive()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select 123 as ｦｧｨｩｪｫｬ", conn))
            using (var reader = command.ExecuteReader(Behavior))
            {
                reader.Read();
                Assert.That(reader["ヲァィゥェォャ"], Is.EqualTo(123));
            }
        }

        #endregion GetOrdinal

        [Test]
        public void FieldIndexDoesntExist()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT 1", conn))
            using (var dr = command.ExecuteReader(Behavior))
            {
                dr.Read();
                Assert.That(() => dr[5], Throws.Exception.TypeOf<IndexOutOfRangeException>());
            }
        }

        [Test, Description("Performs some operations while a reader is still open and checks for exceptions")]
        public void ReaderIsStillOpen()
        {
            using (var conn = OpenConnection())
            using (var cmd1 = new NpgsqlCommand("SELECT 1", conn))
            using (var reader1 = cmd1.ExecuteReader(Behavior))
            {
                Assert.That(() => conn.ExecuteNonQuery("SELECT 1"), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
                Assert.That(() => conn.ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());

                using (var cmd2 = new NpgsqlCommand("SELECT 2", conn))
                {
                    Assert.That(() => cmd2.ExecuteReader(Behavior), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
                    Assert.That(() => cmd2.Prepare(), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
                }
            }
        }

        [Test]
        public void CleansupOkWithDisposeCalls()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT 1", conn))
            using (var dr = command.ExecuteReader(Behavior))
            {
                dr.Read();
                dr.Close();

                using (var upd = conn.CreateCommand())
                {
                    upd.CommandText = "SELECT 1";
                    upd.Prepare();
                }
            }
        }

        [Test]
        public void Null([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2::TEXT", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", DbType.String) { Value = DBNull.Value });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = DBNull.Value });

                using (var reader = cmd.ExecuteReader(behavior))
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.IsDBNull(i), Is.True);
                        Assert.That(reader.IsDBNullAsync(i).Result, Is.True);
                        Assert.That(reader.GetValue(i), Is.EqualTo(DBNull.Value));
                        Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(DBNull.Value));
                        Assert.That(() => reader.GetString(i), Throws.Exception.TypeOf<InvalidCastException>());
                    }
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/742")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/800")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/1234")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/1898")]
        public void HasRows([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var command = new NpgsqlCommand("SELECT 1; SELECT * FROM data WHERE name='does_not_exist'", conn);
                if (prepare == PrepareOrNot.Prepared)
                    command.Prepare();
                using (var reader = command.ExecuteReader(Behavior))
                {
                    Assert.That(reader.HasRows, Is.True);
                    Assert.That(reader.HasRows, Is.True);
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader.HasRows, Is.True);
                    Assert.That(reader.Read(), Is.False);
                    Assert.That(reader.HasRows, Is.True);
                    reader.NextResult();
                    Assert.That(reader.HasRows, Is.False);
                }

                command.CommandText = "SELECT * FROM data";
                if (prepare == PrepareOrNot.Prepared)
                    command.Prepare();
                using (var reader = command.ExecuteReader(Behavior))
                {
                    reader.Read();
                    Assert.That(reader.HasRows, Is.False);
                }

                command.CommandText = "SELECT 1";
                if (prepare == PrepareOrNot.Prepared)
                    command.Prepare();
                using (var reader = command.ExecuteReader(Behavior))
                {
                    reader.Read();
                    reader.Close();
                    Assert.That(() => reader.HasRows, Throws.Exception.TypeOf<InvalidOperationException>());
                }

                command.CommandText = "INSERT INTO data (name) VALUES ('foo'); SELECT * FROM data";
                if (prepare == PrepareOrNot.Prepared)
                    command.Prepare();
                using (var reader = command.ExecuteReader())
                {
                    Assert.That(reader.HasRows, Is.True);
                    reader.Read();
                    Assert.That(reader.GetString(0), Is.EqualTo("foo"));
                }

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void HasRowsWithoutResultset()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                using (var command = new NpgsqlCommand("DELETE FROM data WHERE name = 'unknown'", conn))
                using (var dr = command.ExecuteReader(Behavior))
                    Assert.IsFalse(dr.HasRows);
            }
        }

        [Test]
        public void IntervalAsTimeSpan()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT CAST('1 hour' AS interval) AS dauer", conn))
            using (var dr = command.ExecuteReader(Behavior))
            {
                Assert.IsTrue(dr.HasRows);
                Assert.IsTrue(dr.Read());
                Assert.IsTrue(dr.HasRows);
                var ts = dr.GetTimeSpan(0);
            }
        }

        [Test]
        public void CloseConnectionInMiddleOfRow()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1, 2", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/pull/1266")]
        [Description("NextResult was throwing an ArgumentOutOfRangeException when trying to determine the statement to associate with the PostgresException")]
        public void ReaderNextResultExceptionHandling()
        {
            const string initializeTablesSql = @"
CREATE TEMP TABLE A(value int NOT NULL);
CREATE TEMP TABLE B(value int UNIQUE);
ALTER TABLE ONLY A ADD CONSTRAINT fkey FOREIGN KEY (value) REFERENCES B(value) DEFERRABLE INITIALLY DEFERRED;
CREATE FUNCTION pg_temp.C(_value int) RETURNS int AS $BODY$
BEGIN
    INSERT INTO A(value) VALUES(_value);
    RETURN _value;
END;
$BODY$
LANGUAGE plpgsql VOLATILE";

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(initializeTablesSql);
                using (var cmd = new NpgsqlCommand("SELECT pg_temp.C(1)", conn))
                using (var reader = cmd.ExecuteReader(Behavior)) {
                    Assert.That(() => reader.NextResult(),
                        Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("23503"));
                }
            }
        }

        [Test]
        public void InvalidCast()
        {
            using (var conn = OpenConnection())
            {
                // Chunking type handler
                using (var cmd = new NpgsqlCommand("SELECT 'foo'", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(() => reader.GetInt32(0), Throws.Exception.TypeOf<InvalidCastException>());
                }
                // Simple type handler
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(() => reader.GetDate(0), Throws.Exception.TypeOf<InvalidCastException>());
                }
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Reads a lot of rows to make sure the long unoptimized path for Read() works")]
        public void ManyReads()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($"SELECT generate_series(1, {conn.Settings.ReadBufferSize})", conn))
            using (var reader = cmd.ExecuteReader())
            {
                for (var i = 1; i <= conn.Settings.ReadBufferSize; i++)
                {
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader.GetInt32(0), Is.EqualTo(i));
                }
                Assert.That(reader.Read(), Is.False);
            }
        }


        [Test]
        public void NullableScalar()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var p1 = new NpgsqlParameter { ParameterName = "p1", Value = DBNull.Value, NpgsqlDbType = NpgsqlDbType.Smallint };
                var p2 = new NpgsqlParameter { ParameterName = "p2", Value = (short)8 };
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Smallint));
                Assert.That(p2.DbType, Is.EqualTo(DbType.Int16));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(short)));
                        Assert.That(reader.GetDataTypeName(i), Is.EqualTo("smallint"));
                    }

                    Assert.That(() => reader.GetFieldValue<object>(0), Throws.TypeOf<InvalidCastException>());
                    Assert.That(() => reader.GetFieldValue<int>(0), Throws.TypeOf<InvalidCastException>());
                    Assert.That(() => reader.GetFieldValue<int?>(0), Throws.Nothing);
                    Assert.That(reader.GetFieldValue<int?>(0), Is.Null);

                    Assert.That(() => reader.GetFieldValue<object>(1), Throws.Nothing);
                    Assert.That(() => reader.GetFieldValue<int>(1), Throws.Nothing);
                    Assert.That(() => reader.GetFieldValue<int?>(1), Throws.Nothing);
                    Assert.That(reader.GetFieldValue<object>(1), Is.EqualTo(8));
                    Assert.That(reader.GetFieldValue<int>(1), Is.EqualTo(8));
                    Assert.That(reader.GetFieldValue<int?>(1), Is.EqualTo(8));
                }
            }
        }

        #region GetBytes / GetStream

        [Test]
        public void GetBytes()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (bytes BYTEA)");

                // TODO: This is too small to actually test any interesting sequential behavior
                byte[] expected = { 1, 2, 3, 4, 5 };
                var actual = new byte[expected.Length];
                conn.ExecuteNonQuery($"INSERT INTO data (bytes) VALUES ({TestUtil.EncodeByteaHex(expected)})");

                const string queryText = @"SELECT bytes, 'foo', bytes, 'bar', bytes, bytes FROM data";
                using (var cmd = new NpgsqlCommand(queryText, conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();

                    Assert.That(reader.GetBytes(0, 0, actual, 0, 2), Is.EqualTo(2));
                    Assert.That(actual[0], Is.EqualTo(expected[0]));
                    Assert.That(actual[1], Is.EqualTo(expected[1]));
                    Assert.That(reader.GetBytes(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");
                    if (IsSequential)
                        Assert.That(() => reader.GetBytes(0, 0, actual, 4, 1),
                            Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                    {
                        Assert.That(reader.GetBytes(0, 0, actual, 4, 1), Is.EqualTo(1));
                        Assert.That(actual[4], Is.EqualTo(expected[0]));
                    }
                    Assert.That(reader.GetBytes(0, 2, actual, 2, 3), Is.EqualTo(3));
                    Assert.That(actual, Is.EqualTo(expected));
                    Assert.That(reader.GetBytes(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");

                    Assert.That(() => reader.GetBytes(1, 0, null, 0, 0), Throws.Exception.TypeOf<InvalidCastException>(),
                        "GetBytes on non-bytea");
                    Assert.That(() => reader.GetBytes(1, 0, actual, 0, 1),
                        Throws.Exception.TypeOf<InvalidCastException>(),
                        "GetBytes on non-bytea");
                    Assert.That(reader.GetString(1), Is.EqualTo("foo"));
                    reader.GetBytes(2, 0, actual, 0, 2);
                    // Jump to another column from the middle of the column
                    reader.GetBytes(4, 0, actual, 0, 2);
                    Assert.That(reader.GetBytes(4, expected.Length - 1, actual, 0, 2), Is.EqualTo(1),
                        "Length greater than data length");
                    Assert.That(actual[0], Is.EqualTo(expected[expected.Length - 1]), "Length greater than data length");
                    Assert.That(() => reader.GetBytes(4, 0, actual, 0, actual.Length + 1),
                        Throws.Exception.TypeOf<IndexOutOfRangeException>(), "Length great than output buffer length");
                    // Close in the middle of a column
                    reader.GetBytes(5, 0, actual, 0, 2);
                }

                //var result = (byte[]) cmd.ExecuteScalar();
                //Assert.AreEqual(2, result.Length);
            }
        }

        [Test]
        public async Task GetStream([Values(true, false)] bool isAsync)
        {
            var streamGetter = BuildStreamGetter(isAsync);

            using (var conn = OpenConnection())
            {
                // TODO: This is too small to actually test any interesting sequential behavior
                byte[] expected = { 1, 2, 3, 4, 5 };
                var actual = new byte[expected.Length];
                using (var cmd = new NpgsqlCommand($@"SELECT {TestUtil.EncodeByteaHex(expected)}::bytea, {TestUtil.EncodeByteaHex(expected)}::bytea", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();

                    using (var stream = await streamGetter(reader, 0))
                    {
                        Assert.That(stream.CanSeek, Is.EqualTo(Behavior == CommandBehavior.Default));
                        Assert.That(stream.Length, Is.EqualTo(expected.Length));
                        stream.Read(actual, 0, 2);
                        Assert.That(actual[0], Is.EqualTo(expected[0]));
                        Assert.That(actual[1], Is.EqualTo(expected[1]));
                        Assert.That(async () => await streamGetter(reader, 0),
                            Throws.Exception.TypeOf<InvalidOperationException>());
                        stream.Read(actual, 2, 1);
                        Assert.That(actual[2], Is.EqualTo(expected[2]));
                    }

                    if (IsSequential)
                        Assert.That(() => reader.GetBytes(0, 0, actual, 4, 1), Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                    {
                        Assert.That(reader.GetBytes(0, 0, actual, 4, 1), Is.EqualTo(1));
                        Assert.That(actual[4], Is.EqualTo(expected[0]));
                    }

                    using (var stream2 = await streamGetter(reader, 1))
                    {
                        Assert.That(stream2.ReadByte(), Is.EqualTo(1));
                    }
                }
            }
        }

        [Test]
        public async Task OpenStreamWhenChangingColumns([Values(true, false)] bool isAsync)
        {
            var streamGetter = BuildStreamGetter(isAsync);

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p, @p", conn))
            {
                var data = new byte[] { 1, 2, 3 };
                cmd.Parameters.Add(new NpgsqlParameter("p", data));
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    var stream = await streamGetter(reader, 0);
                    // ReSharper disable once UnusedVariable
                    var v = reader.GetValue(1);
                    Assert.That(() => stream.ReadByte(), Throws.Exception.TypeOf<ObjectDisposedException>());
                }
            }
        }

        [Test]
        public async Task OpenStreamWhenChangingRows([Values(true, false)] bool isAsync)
        {
            var streamGetter = BuildStreamGetter(isAsync);

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT @p", conn))
            {
                var data = new byte[] { 1, 2, 3 };
                cmd.Parameters.Add(new NpgsqlParameter("p", data));
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    var s1 = await streamGetter(reader, 0);
                    reader.Read();
                    Assert.That(() => s1.ReadByte(), Throws.Exception.TypeOf<ObjectDisposedException>());
                }
            }
        }

        [Test]
        public void GetBytesWithNull([Values(true, false)] bool isAsync)
        {
            var streamGetter = BuildStreamGetter(isAsync);

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (bytes BYTEA)");
                var buf = new byte[8];
                conn.ExecuteNonQuery(@"INSERT INTO data (bytes) VALUES (NULL)");
                using (var cmd = new NpgsqlCommand("SELECT bytes FROM data", conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();
                    Assert.That(reader.IsDBNull(0), Is.True);
                    Assert.That(() => reader.GetBytes(0, 0, buf, 0, 1), Throws.Exception.TypeOf<InvalidCastException>(), "GetBytes");
                    Assert.That(async () => await streamGetter(reader, 0), Throws.Exception.TypeOf<InvalidCastException>(), "GetStream");
                    Assert.That(() => reader.GetBytes(0, 0, null, 0, 0), Throws.Exception.TypeOf<InvalidCastException>(), "GetBytes with null buffer");
                }
            }
        }

        static Func<NpgsqlDataReader, int, Task<Stream>> BuildStreamGetter(bool isAsync)
            => isAsync
                ? (Func<NpgsqlDataReader, int, Task<Stream>>)((r, index) => r.GetStreamAsync(index))
                : (r, index) => Task.FromResult(r.GetStream(index));

        #endregion GetBytes / GetStream

        #region GetChars / GetTextReader

        [Test]
        public void GetChars()
        {
            using (var conn = OpenConnection())
            {
                // TODO: This is too small to actually test any interesting sequential behavior
                const string str = "ABCDE";
                var expected = str.ToCharArray();
                var actual = new char[expected.Length];

                var queryText = $@"SELECT '{str}', 3, '{str}', 4, '{str}', '{str}', '{str}'";
                using (var cmd = new NpgsqlCommand(queryText, conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();

                    Assert.That(reader.GetChars(0, 0, actual, 0, 2), Is.EqualTo(2));
                    Assert.That(actual[0], Is.EqualTo(expected[0]));
                    Assert.That(actual[1], Is.EqualTo(expected[1]));
                    Assert.That(reader.GetChars(0, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");
                    // Note: Unlike with bytea, finding out the length of the column consumes it (variable-width
                    // UTF8 encoding)
                    Assert.That(reader.GetChars(2, 0, actual, 0, 2), Is.EqualTo(2));
                    if (IsSequential)
                        Assert.That(() => reader.GetChars(2, 0, actual, 4, 1), Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                    {
                        Assert.That(reader.GetChars(2, 0, actual, 4, 1), Is.EqualTo(1));
                        Assert.That(actual[4], Is.EqualTo(expected[0]));
                    }
                    Assert.That(reader.GetChars(2, 2, actual, 2, 3), Is.EqualTo(3));
                    Assert.That(actual, Is.EqualTo(expected));
                    //Assert.That(reader.GetChars(2, 0, null, 0, 0), Is.EqualTo(expected.Length), "Bad column length");

                    Assert.That(() => reader.GetChars(3, 0, null, 0, 0), Throws.Exception.TypeOf<InvalidCastException>(), "GetChars on non-text");
                    Assert.That(() => reader.GetChars(3, 0, actual, 0, 1), Throws.Exception.TypeOf<InvalidCastException>(), "GetChars on non-text");
                    Assert.That(reader.GetInt32(3), Is.EqualTo(4));
                    reader.GetChars(4, 0, actual, 0, 2);
                    // Jump to another column from the middle of the column
                    reader.GetChars(5, 0, actual, 0, 2);
                    Assert.That(reader.GetChars(5, expected.Length - 1, actual, 0, 2), Is.EqualTo(1), "Length greater than data length");
                    Assert.That(actual[0], Is.EqualTo(expected[expected.Length - 1]), "Length greater than data length");
                    Assert.That(() => reader.GetChars(5, 0, actual, 0, actual.Length + 1), Throws.Exception.TypeOf<IndexOutOfRangeException>(), "Length great than output buffer length");
                    // Close in the middle of a column
                    reader.GetChars(6, 0, actual, 0, 2);
                }
            }
        }

        [Test]
        public async Task GetTextReader([Values(true, false)] bool isAsync)
        {
            Func<NpgsqlDataReader, int, Task<TextReader>> textReaderGetter;
            if (isAsync)
                textReaderGetter = (r, index) => r.GetTextReaderAsync(index);
            else
                textReaderGetter = (r, index) => Task.FromResult(r.GetTextReader(index));

            using (var conn = OpenConnection())
            {
                // TODO: This is too small to actually test any interesting sequential behavior
                const string str = "ABCDE";
                var expected = str.ToCharArray();
                var actual = new char[expected.Length];
                //ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", str));

                var queryText = $@"SELECT '{str}', 'foo'";
                using (var cmd = new NpgsqlCommand(queryText, conn))
                using (var reader = cmd.ExecuteReader(Behavior))
                {
                    reader.Read();

                    var textReader = await textReaderGetter(reader, 0);
                    textReader.Read(actual, 0, 2);
                    Assert.That(actual[0], Is.EqualTo(expected[0]));
                    Assert.That(actual[1], Is.EqualTo(expected[1]));
                    Assert.That(async () => await textReaderGetter(reader, 0),
                        Throws.Exception.TypeOf<InvalidOperationException>(),
                        "Sequential text reader twice on same column");
                    textReader.Read(actual, 2, 1);
                    Assert.That(actual[2], Is.EqualTo(expected[2]));
                    textReader.Dispose();

                    if (IsSequential)
                        Assert.That(() => reader.GetChars(0, 0, actual, 4, 1),
                            Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                    {
                        Assert.That(reader.GetChars(0, 0, actual, 4, 1), Is.EqualTo(1));
                        Assert.That(actual[4], Is.EqualTo(expected[0]));
                    }
                    Assert.That(reader.GetString(1), Is.EqualTo("foo"));
                }
            }
        }

        [Test]
        public void OpenTextReaderWhenChangingColumns()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT 'some_text', 'some_text'", conn))
            using (var reader = cmd.ExecuteReader(Behavior))
            {
                reader.Read();
                var textReader = reader.GetTextReader(0);
                // ReSharper disable once UnusedVariable
                var v = reader.GetValue(1);
                Assert.That(() => textReader.Peek(), Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void OpenReaderWhenChangingRows()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand(@"SELECT 'some_text', 'some_text'", conn))
            using (var reader = cmd.ExecuteReader(Behavior))
            {
                reader.Read();
                var tr1 = reader.GetTextReader(0);
                reader.Read();
                Assert.That(() => tr1.Peek(), Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test]
        public void GetCharsWhenNull([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            var buf = new char[8];
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT NULL::TEXT", conn))
            using (var reader = cmd.ExecuteReader(behavior))
            {
                reader.Read();
                Assert.That(reader.IsDBNull(0), Is.True);
                Assert.That(() => reader.GetChars(0, 0, buf, 0, 1), Throws.Exception.TypeOf<InvalidCastException>(), "GetChars");
                Assert.That(() => reader.GetTextReader(0), Throws.Exception.TypeOf<InvalidCastException>(), "GetTextReader");
                Assert.That(() => reader.GetChars(0, 0, null, 0, 0), Throws.Exception.TypeOf<InvalidCastException>(), "GetChars with null buffer");
            }
        }

        [Test]
        public void ReaderIsReused()
        {
            using (var conn = OpenConnection())
            {
                NpgsqlDataReader reader1;

                using (var cmd = new NpgsqlCommand("SELECT 8", conn))
                using (reader1 = cmd.ExecuteReader(Behavior))
                {
                    reader1.Read();
                    Assert.That(reader1.GetInt32(0), Is.EqualTo(8));
                }

                using (var cmd = new NpgsqlCommand("SELECT 9", conn))
                using (var reader2 = cmd.ExecuteReader(Behavior))
                {
                    Assert.That(reader2, Is.SameAs(reader1));
                    reader2.Read();
                    Assert.That(reader2.GetInt32(0), Is.EqualTo(9));
                }
            }
        }

        #endregion GetChars / GetTextReader

#if DEBUG
        [Test, Description("Tests that everything goes well when a type handler generates a NpgsqlSafeReadException")]
        [Timeout(5000)]
        public void SafeReadException()
        {
            using (var conn = OpenConnection())
            {
                // Temporarily reroute integer to go to a type handler which generates SafeReadExceptions
                conn.TypeMapper.AddMapping(new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = "integer",
                    TypeHandlerFactory = new ExplodingTypeHandlerFactory(true)
                }.Build());
                using (var cmd = new NpgsqlCommand(@"SELECT 1, 'hello'", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(() => reader.GetInt32(0),
                        Throws.Exception.With.Message.EqualTo("Safe read exception as requested"));
                    Assert.That(reader.GetString(1), Is.EqualTo("hello"));
                }
            }
        }

        [Test, Description("Tests that when a type handler generates an exception that isn't a NpgsqlSafeReadException, the connection is properly broken")]
        [Timeout(5000)]
        public void NonSafeReadException()
        {
            using (var conn = OpenConnection())
            {
                // Temporarily reroute integer to go to a type handler which generates some exception
                conn.TypeMapper.AddMapping(new NpgsqlTypeMappingBuilder()
                {
                    PgTypeName = "integer",
                    TypeHandlerFactory = new ExplodingTypeHandlerFactory(false)
                }.Build());
                using (var cmd = new NpgsqlCommand(@"SELECT 1, 'hello'", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(() => reader.GetInt32(0),
                        Throws.Exception.With.Message.EqualTo("Non-safe read exception as requested"));
                    Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
                    Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
                }
            }
        }
#endif

        #region Initialization / setup / teardown

        // ReSharper disable InconsistentNaming
        readonly bool IsSequential;
        readonly CommandBehavior Behavior;
        // ReSharper restore InconsistentNaming

        public ReaderTests(CommandBehavior behavior)
        {
            Behavior = behavior;
            IsSequential = (Behavior & CommandBehavior.SequentialAccess) != 0;
        }

        #endregion
    }

    #region Mock Type Handlers

    class ExplodingTypeHandlerFactory : NpgsqlTypeHandlerFactory<int>
    {
        readonly bool _safe;
        internal ExplodingTypeHandlerFactory(bool safe) { _safe = safe; }
        protected override NpgsqlTypeHandler<int> Create(NpgsqlConnection conn)
            => new ExplodingTypeHandler(_safe);
    }

    class ExplodingTypeHandler : NpgsqlSimpleTypeHandler<int>
    {
        readonly bool _safe;
        internal ExplodingTypeHandler(bool safe) { _safe = safe; }

        public override int Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            buf.ReadInt32();
            throw _safe
                ? new NpgsqlSafeReadException(new Exception("Safe read exception as requested"))
                : throw new Exception("Non-safe read exception as requested");
        }

        public override int ValidateAndGetLength(int value, NpgsqlParameter parameter) { throw new NotSupportedException(); }
        public override void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter) { throw new NotSupportedException(); }
    }

    #endregion
}
