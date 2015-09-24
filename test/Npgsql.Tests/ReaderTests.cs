#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Linq;
using System.Web.UI.WebControls;

using Npgsql;
using Npgsql.BackendMessages;
using NpgsqlTypes;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests
{
    [TestFixture]
    public class ReaderTests : TestBase
    {
        public ReaderTests(string backendVersion) : base(backendVersion) { }

        [Test]
        public void EmptyResultSet()
        {
            var cmd = new NpgsqlCommand("SELECT 1 WHERE FALSE", Conn);
            var reader = cmd.ExecuteReader();
            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>());
            cmd.Dispose();
        }

        [Test]
        public void FieldCount()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
            var cmd = new NpgsqlCommand("SELECT 1; SELECT 2,3", Conn);
            var reader = cmd.ExecuteReader();
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.FieldCount, Is.EqualTo(1));
            Assert.That(reader.NextResult(), Is.True);
            Assert.That(reader.FieldCount, Is.EqualTo(2));
            Assert.That(reader.NextResult(), Is.False);
            Assert.That(reader.FieldCount, Is.EqualTo(0));
            reader.Close();

            cmd.CommandText = "INSERT INTO data (int) VALUES (1)";
            reader = cmd.ExecuteReader();
            // Note MSDN docs that seem to say we should case -1 in this case: http://msdn.microsoft.com/en-us/library/system.data.idatarecord.fieldcount(v=vs.110).aspx
            // But SqlClient returns 0
            Assert.That(() => reader.FieldCount, Is.EqualTo(0));
            cmd.Dispose();
        }

        [Test]
        public void RecordsAffected()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
            var cmd = new NpgsqlCommand("INSERT INTO data (int) VALUES (7); INSERT INTO data (int) VALUES (8)", Conn);
            var reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(2));

            cmd = new NpgsqlCommand("SELECT * FROM data", Conn);
            reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(-1));

            cmd = new NpgsqlCommand("UPDATE data SET int=8", Conn);
            reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(2));

            cmd = new NpgsqlCommand("UPDATE data SET int=8 WHERE int=666", Conn);
            reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(0));
        }

        [Test]
        public void Statements()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT) WITH OIDS");
            using (var cmd = new NpgsqlCommand(
                "INSERT INTO data (name) VALUES ('a');" +
                "UPDATE data SET name='b' WHERE name='doesnt_exist'",
                Conn)
            )
            using (var reader = cmd.ExecuteReader())
            {
                Assert.That(reader.Statements[0].SQL, Is.EqualTo("INSERT INTO data (name) VALUES ('a')"));
                Assert.That(reader.Statements[0].StatementType, Is.EqualTo(StatementType.Insert));
                Assert.That(reader.Statements[0].Rows, Is.EqualTo(1));
                Assert.That(reader.Statements[0].OID, Is.Not.EqualTo(0));
                Assert.That(reader.Statements[1].SQL, Is.EqualTo("UPDATE data SET name='b' WHERE name='doesnt_exist'"));
                Assert.That(reader.Statements[1].StatementType, Is.EqualTo(StatementType.Update));
                Assert.That(reader.Statements[1].Rows, Is.EqualTo(0));
                Assert.That(reader.Statements[1].OID, Is.EqualTo(0));
            }

            using (var cmd = new NpgsqlCommand("SELECT name FROM data; DELETE FROM data", Conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.NextResult();  // Consume SELECT result set
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

        [Test]
        public void GetStringWithParameter()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            const string text = "Random text";
            ExecuteNonQuery(String.Format(@"INSERT INTO data (name) VALUES ('{0}')", text));

            var command = new NpgsqlCommand("SELECT name FROM data WHERE name = :value;", Conn);
            var param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = text.Length;
            param.Value = text;
            command.Parameters.Add(param);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetString(0);
                Assert.AreEqual(text, result);
            }
        }

        [Test]
        public void GetStringWithQuoteWithParameter()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('Text with '' single quote')");

            const string test = "Text with ' single quote";
            var command = new NpgsqlCommand("SELECT name FROM data WHERE name = :value;", Conn);

            var param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetString(0);
                Assert.AreEqual(test, result);
            }
        }

        [Test]
        public void GetValueByName()
        {
            var command = new NpgsqlCommand(@"SELECT 'Random text' AS real_column", Conn);
            var dr = command.ExecuteReader();
            dr.Read();
            Assert.That(dr["real_column"], Is.EqualTo("Random text"));
            Assert.That(() => dr["non_existing"], Throws.Exception.TypeOf<IndexOutOfRangeException>());
            command.Dispose();
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/794")]
        public void GetFieldType()
        {
            using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(int)));
            }
            using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn))
            {
                cmd.AllResultTypesAreUnknown = true;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(string)));
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/787")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/794")]
        public void GetDataTypeName()
        {
            using (var command = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn))
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4"));
            }
            using (var command = new NpgsqlCommand(@"SELECT '{1}'::INT4[] AS some_column", Conn))
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("_int4"));
            }
            using (var command = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn))
            {
                command.AllResultTypesAreUnknown = true;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("int4"));
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/791")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/794")]
        public void GetDataTypeOID()
        {
            var int4OID = ExecuteScalar("SELECT oid FROM pg_type WHERE typname = 'int4'");
            using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeOID(0), Is.EqualTo(int4OID));
            }
            using (var cmd = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn))
            {
                cmd.AllResultTypesAreUnknown = true;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeOID(0), Is.EqualTo(int4OID));
                }
            }
        }

        [Test]
        public void GetName()
        {
            var command = new NpgsqlCommand(@"SELECT 1 AS some_column", Conn);
            var dr = command.ExecuteReader();
            dr.Read();
            Assert.That(dr.GetName(0), Is.EqualTo("some_column"));
            command.Dispose();
        }

        [Test]
        public void GetOrdinal()
        {
            using (var command = new NpgsqlCommand(@"SELECT 0, 1 AS some_column WHERE 1=0", Conn))
            using (var dr = command.ExecuteReader())
            {
                Assert.That(dr.GetOrdinal("some_column"), Is.EqualTo(1));
                Assert.That(() => dr.GetOrdinal("doesn't_exist"), Throws.Exception.TypeOf<IndexOutOfRangeException>());
            }
        }

        [Test]
        public void GetFieldValueAsObject()
        {
            using (var cmd = new NpgsqlCommand("SELECT 'foo'::TEXT", Conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<object>(0), Is.EqualTo("foo"));
            }
        }

        [Test]
        public void GetValues()
        {
            var command = new NpgsqlCommand(@"SELECT 'hello', 1, '2014-01-01'::DATE", Conn);
            var dr = command.ExecuteReader();
            dr.Read();
            var values = new object[4];
            Assert.That(dr.GetValues(values), Is.EqualTo(3));
            Assert.That(values, Is.EqualTo(new object[] { "hello", 1, new DateTime(2014, 1, 1), null }));
            values = new object[2];
            Assert.That(dr.GetValues(values), Is.EqualTo(2));
            Assert.That(values, Is.EqualTo(new object[] { "hello", 1 }));
            command.Dispose();
        }

        [Test]
        public void GetProviderSpecificValues()
        {
            var command = new NpgsqlCommand(@"SELECT 'hello', 1, '2014-01-01'::DATE", Conn);
            var dr = command.ExecuteReader();
            dr.Read();
            var values = new object[4];
            Assert.That(dr.GetProviderSpecificValues(values), Is.EqualTo(3));
            Assert.That(values, Is.EqualTo(new object[] { "hello", 1, new NpgsqlDate(2014, 1, 1), null }));
            values = new object[2];
            Assert.That(dr.GetProviderSpecificValues(values), Is.EqualTo(2));
            Assert.That(values, Is.EqualTo(new object[] { "hello", 1 }));
            command.Dispose();
        }

        [Test]
        public void ExecuteReaderGettingEmptyResultSetWithOutputParameter()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var command = new NpgsqlCommand("SELECT * FROM data WHERE name = NULL;", Conn);
            var param = new NpgsqlParameter("some_param", NpgsqlDbType.Varchar);
            param.Direction = ParameterDirection.Output;
            command.Parameters.Add(param);
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                Assert.IsFalse(dr.NextResult());
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValueFromEmptyResultset()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var command = new NpgsqlCommand("SELECT * FROM data WHERE name = :value;", Conn);

            const string test = "Text single quote";
            var param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                // This line should throw the invalid operation exception as the datareader will
                // have an empty resultset.
                Console.WriteLine(dr.IsDBNull(1));
            }
        }

        [Test]
        public void NonExistentParameterName()
        {
            var cmd = new NpgsqlCommand("SELECT @x::TEXT", Conn);
            Assert.That(() => cmd.ExecuteReader(), Throws.Exception);
        }

        [Test]
        public void UseDataAdapter()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            var da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapter.xml");
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            command.Connection = Conn;
            var da = new NpgsqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            var da = new NpgsqlDataAdapter("SELECT 1", Conn);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            var da = new NpgsqlDataAdapter("SELECT 1", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            var da = new NpgsqlDataAdapter("SELECT 1", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        public void DataGridWebControlSupport()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            var dr = command.ExecuteReader();
            //Console.WriteLine(dr.FieldCount);
            var dg = new DataGrid();
            dg.DataSource = dr;
            dg.DataBind();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadPastDataReaderEnd()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            var dr = command.ExecuteReader();
            while (dr.Read()) {}
            Object o = dr[0];
            Assert.IsNotNull(o);
        }

        [Test]
        public void SchemaOnly([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var cmd = new NpgsqlCommand(
                "SELECT 1 AS some_column;" +
                "UPDATE data SET name='yo' WHERE 1=0;" +
                "SELECT 1 AS some_other_column",
                Conn);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
            Assert.That(reader.Read(), Is.False);
            var t = reader.GetSchemaTable();
            Assert.That(t.Rows[0]["ColumnName"], Is.EqualTo("some_column"));
            Assert.That(reader.NextResult(), Is.True);
            Assert.That(reader.Read(), Is.False);
            t = reader.GetSchemaTable();
            Assert.That(t.Rows[0]["ColumnName"], Is.EqualTo("some_other_column"));
            Assert.That(reader.NextResult(), Is.False);
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void SingleResult()
        {
            var cmd = new NpgsqlCommand(@"SELECT 1; SELECT 2", Conn);
            var rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);
            Assert.That(rdr.Read(), Is.True);
            Assert.That(rdr.GetInt32(0), Is.EqualTo(1));
            Assert.That(rdr.NextResult(), Is.False);
            rdr.Close();
        }

        [Test]
        public void SingleRowCommandBehaviorSupport()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('Y')");
            var command = new NpgsqlCommand(@"SELECT * FROM data", Conn);
            var reader = command.ExecuteReader(CommandBehavior.SingleRow);
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.Read(), Is.False);
            reader.Close();
            command.Dispose();
        }

        [Test, Description("In sequential access, performing a null check on a non-first field would check the first field")]
        public void SequentialNullCheckOnNonFirstField()
        {
            using (var cmd = new NpgsqlCommand("SELECT 'X', NULL", Conn))
            using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                dr.Read();
                Assert.That(dr.IsDBNull(1), Is.True);
            }
        }

        #region GetSchemaTable

        [Test]
        public void PrimaryKeyFieldsMetadataSupport()
        {
            ExecuteNonQuery("DROP TABLE IF EXISTS DATA2 CASCADE");
            ExecuteNonQuery(@"CREATE TEMP TABLE DATA2 (
                                field_pk1                      INT2 NOT NULL,
                                field_pk2                      INT2 NOT NULL,
                                field_serial                   SERIAL,
                                CONSTRAINT data2_pkey PRIMARY KEY (field_pk1, field_pk2)
                                ) WITH OIDS");

            using (var command = new NpgsqlCommand("SELECT * FROM DATA2", Conn))
            {
                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    dr.Read();
                    var keyColumns = dr.GetSchemaTable().Rows.Cast<DataRow>().Where(r => (bool)r["IsKey"]).ToArray();
                    Assert.That(keyColumns, Has.Length.EqualTo(2));
                    Assert.That(keyColumns.Count(c => (string)c["ColumnName"] == "field_pk1"), Is.EqualTo(1));
                    Assert.That(keyColumns.Count(c => (string)c["ColumnName"] == "field_pk2"), Is.EqualTo(1));
                }
            }
        }

        [Test]
        public void PrimaryKeyFieldMetadataSupport()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY, serial SERIAL)");
            using (var command = new NpgsqlCommand("SELECT * FROM data", Conn))
            {
                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    dr.Read();
                    var metadata = dr.GetSchemaTable();
                    var key = metadata.Rows.Cast<DataRow>().Single(r => (bool) r["IsKey"]);
                    Assert.That(key["ColumnName"], Is.EqualTo("id"));
                }
            }
        }

        [Test]
        public void IsAutoIncrementMetadataSupport()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY)");
            var command = new NpgsqlCommand("SELECT * FROM data", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
            {
                var metadata = dr.GetSchemaTable();
                Assert.That(metadata.Rows.Cast<DataRow>()
                    .Where(r => ((string)r["ColumnName"]).Contains("serial"))
                    .All(r => (bool)r["IsAutoIncrement"]));
            }
        }

        [Test]
        public void IsReadOnlyMetadataSupport()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY, int2 SMALLINT)");
            ExecuteNonQuery("CREATE OR REPLACE TEMPORARY VIEW DataView (id, int2) AS SELECT id, int2 + int2 AS int2 FROM data");

            var command = new NpgsqlCommand("SELECT * FROM DataView", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
            {
                var metadata = dr.GetSchemaTable();

                foreach (DataRow r in metadata.Rows)
                {
                    switch ((string)r["ColumnName"])
                    {
                        case "field_pk":
                            if (Conn.PostgreSqlVersion < new Version("9.4"))
                            {
                                // 9.3 and earlier: IsUpdatable = False
                                Assert.IsTrue((bool)r["IsReadonly"], "field_pk");
                            }
                            else
                            {
                                // 9.4: IsUpdatable = True
                                Assert.IsFalse((bool)r["IsReadonly"], "field_pk");
                            }
                            break;
                        case "field_int2":
                            Assert.IsTrue((bool)r["IsReadonly"]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        [Test]
        public void SchemaOnlySingleRowCommandBehaviorSupport()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.SingleRow))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(0, i);
            }
        }

        [Test]
        public void SchemaOnlyCommandBehaviorSupport()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(0, i);
            }
        }

        [Test]
        public void SchemaOnlyCommandBehaviorSupportFunctioncall()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() RETURNS SETOF integer as '
                              SELECT 1;
                              ' LANGUAGE 'sql';");
            var command = new NpgsqlCommand("funcb", Conn);
            command.CommandType = CommandType.StoredProcedure;
            using (var dr = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                var i = 0;
                while (dr.Read())
                    i++;
                Assert.AreEqual(0, i);
            }
        }

        [Test]
        public void FieldNameKanaWidthWideRequestForNarrowFieldName()
        {//Should ignore Kana width and hence find the first of these two fields
            var command = new NpgsqlCommand("select 123 as ｦｧｨｩｪｫｬ, 124 as ヲァィゥェォャ", Conn);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr["ｦｧｨｩｪｫｬ"], 123);
                Assert.AreEqual(dr["ヲァィゥェォャ"], 123);// Wide version.
            }
        }

        [Test]
        public void FieldNameKanaWidthNarrowRequestForWideFieldName()
        {//Should ignore Kana width and hence find the first of these two fields
            var command = new NpgsqlCommand("select 123 as ヲァィゥェォャ, 124 as ｦｧｨｩｪｫｬ", Conn);

            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr["ヲァィゥェォャ"], 123);
                Assert.AreEqual(dr["ｦｧｨｩｪｫｬ"], 123);// Narrow version.
            }
        }

        [Test]
        public void FieldIndexDoesntExist()
        {
            using (var command = new NpgsqlCommand("SELECT 1", Conn))
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.That(() => dr[5], Throws.Exception.TypeOf<IndexOutOfRangeException>());
            }
        }

        [Test, Description("Performs some operations while a reader is still open and checks for exceptions")]
        public void ReaderIsStillOpen()
        {
            using (var cmd1 = new NpgsqlCommand("SELECT 1", Conn))
            using (var reader1 = cmd1.ExecuteReader())
            {
                Assert.That(() => ExecuteNonQuery("SELECT 1"), Throws.Exception.TypeOf<InvalidOperationException>());
                Assert.That(() => ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<InvalidOperationException>());

                using (var cmd2 = new NpgsqlCommand("SELECT 2", Conn))
                {
                    Assert.That(() => cmd2.ExecuteReader(), Throws.Exception.TypeOf<InvalidOperationException>());
                    Assert.That(() => cmd2.Prepare(), Throws.Exception.TypeOf<InvalidOperationException>());
                }
            }
        }

        [Test]
        public void LoadDataTable()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (char5 CHAR(5), varchar5 VARCHAR(5))");

            using (var command = new NpgsqlCommand("SELECT char5, varchar5 FROM data", Conn))
            using (var dr = command.ExecuteReader())
            {
                var dt = new DataTable();
                dt.Load(dr);
                dr.Close();

                Assert.AreEqual(5, dt.Columns[0].MaxLength);
                Assert.AreEqual(5, dt.Columns[1].MaxLength);
            }
        }

        [Test]
        public void CleansupOkWithDisposeCalls()
        {
            var command = new NpgsqlCommand("SELECT 1", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                dr.Close();

                using (var upd = Conn.CreateCommand())
                {
                    upd.CommandText = "SELECT 1";
                    upd.Prepare();
                }
            }
        }

        [Test]
        public void Null()
        {
            using (var cmd = new NpgsqlCommand("SELECT @p::TEXT", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", DbType.String) { Value = DBNull.Value });

                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    Assert.That(reader.IsDBNull(0), Is.True);
                    Assert.That(reader.IsDBNullAsync(0).Result, Is.True);
                    Assert.That(reader.GetValue(0), Is.EqualTo(DBNull.Value));
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(DBNull.Value));
                    Assert.That(() => reader.GetString(0), Throws.Exception.TypeOf<InvalidCastException>());
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/742")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/800")]
        public void HasRows([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var command = new NpgsqlCommand("SELECT 1; SELECT * FROM data WHERE name='does_not_exist'", Conn);
            if (prepare == PrepareOrNot.Prepared)
                command.Prepare();
            using (var dr = command.ExecuteReader())
            {
                Assert.That(dr.HasRows, Is.True);
                Assert.That(dr.HasRows, Is.True);
                Assert.That(dr.Read(), Is.True);
                Assert.That(dr.HasRows, Is.True);
                Assert.That(dr.Read(), Is.False);
                dr.NextResult();
                Assert.That(dr.HasRows, Is.False);
            }
        }

        [Test]
        public void HasRowsWithoutResultset()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var command = new NpgsqlCommand("DELETE FROM data WHERE name = 'unknown'", Conn);
            var dr = command.ExecuteReader();
            Assert.IsFalse(dr.HasRows);
        }

        [Test]
        public void IntervalAsTimeSpan()
        {
            var command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = Conn;

            using (var dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.HasRows);
                Assert.IsTrue(dr.Read());
                Assert.IsTrue(dr.HasRows);
                var ts = dr.GetTimeSpan(0);
            }
        }

        [Test]
        public void SequentialConsumeWithNull()
        {
            var command = new NpgsqlCommand("SELECT 1, NULL", Conn);
            var reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
            reader.Read();
            reader.Close();
            command.Dispose();
        }

        [Test]
        public void CloseConnectionInMiddleOfRow()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT 1, 2", conn);
                var reader = cmd.ExecuteReader();
                reader.Read();
                Console.WriteLine(reader.GetInt32(0));
            }
        }

#if DEBUG
        [Test, Description("Tests that everything goes well when a type handler generates a SafeReadException")]
        [Timeout(5000)]
        public void SafeReadException()
        {
            // Temporarily reroute integer to go to a type handler which generates SafeReadExceptions
            var registry = Conn.Connector.TypeHandlerRegistry;
            var intHandler = registry[typeof(int)];
            registry.OIDIndex[intHandler.OID] = new SafeExceptionGeneratingHandler();
            try
            {
                using (var cmd = new NpgsqlCommand(@"SELECT 1, 'hello'", Conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(() => reader.GetInt32(0),
                        Throws.Exception.With.Message.EqualTo("Safe read exception as requested"));
                    Assert.That(reader.GetString(1), Is.EqualTo("hello"));
                }
            }
            finally
            {
                registry.OIDIndex[intHandler.OID] = intHandler;
            }
        }

        [Test, Description("Tests that when a type handler generates an exception that isn't a SafeReadException, the connection is properly broken")]
        [Timeout(5000)]
        public void NonSafeReadException()
        {
            // Temporarily reroute integer to go to a type handler which generates some exception
            var registry = Conn.Connector.TypeHandlerRegistry;
            var intHandler = registry[typeof(int)];
            registry.OIDIndex[intHandler.OID] = new NonSafeExceptionGeneratingHandler();
            try {
                using (var cmd = new NpgsqlCommand(@"SELECT 1, 'hello'", Conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)) {
                    reader.Read();
                    Assert.That(() => reader.GetInt32(0),
                        Throws.Exception.With.Message.EqualTo("Non-safe read exception as requested"));
                    Assert.That(Conn.FullState, Is.EqualTo(ConnectionState.Broken));
                    Assert.That(Conn.State, Is.EqualTo(ConnectionState.Closed));
                }
            } finally {
                registry.OIDIndex[intHandler.OID] = intHandler;
            }
        }
#endif
    }

    #region Mock Type Handlers
#if DEBUG
    internal class SafeExceptionGeneratingHandler : SimpleTypeHandler<int>
    {
        public override int Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            buf.ReadInt32();
            throw new SafeReadException(new Exception("Safe read exception as requested"));
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter) { throw new NotSupportedException(); }
        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter) { throw new NotSupportedException(); }
    }

    internal class NonSafeExceptionGeneratingHandler : SimpleTypeHandler<int>
    {
        public override int Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            throw new Exception("Non-safe read exception as requested");
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter) { throw new NotSupportedException(); }
        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter) { throw new NotSupportedException();}
    }
#endif
    #endregion
}
