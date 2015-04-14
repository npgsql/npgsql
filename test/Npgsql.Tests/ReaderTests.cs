// created on 27/12/2002 at 17:05
//
// Author:
//     Francisco Figueiredo Jr. <fxjrlists@yahoo.com>
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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

            cmd.CommandText = "INSERT INTO data (field_int4) VALUES (1)";
            reader = cmd.ExecuteReader();
            // Note MSDN docs that seem to say we should case -1 in this case: http://msdn.microsoft.com/en-us/library/system.data.idatarecord.fieldcount(v=vs.110).aspx
            // But SqlClient returns 0
            Assert.That(() => reader.FieldCount, Is.EqualTo(0));
            cmd.Dispose();
        }

        [Test]
        public void RecordsAffected()
        {
            var cmd = new NpgsqlCommand("INSERT INTO data (field_int4) VALUES (7); INSERT INTO data (field_int4) VALUES (8)", Conn);
            var reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(2));

            cmd = new NpgsqlCommand("SELECT * FROM data", Conn);
            reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(-1));

            cmd = new NpgsqlCommand("UPDATE data SET field_int4=8", Conn);
            reader = cmd.ExecuteReader();
            reader.Close();
            Assert.That(reader.RecordsAffected, Is.EqualTo(2));
        }

        [Test]
        public void LastInsertedOID()
        {
            var insertCmd = new NpgsqlCommand("INSERT INTO data (field_text) VALUES ('a')", Conn);
            insertCmd.ExecuteNonQuery();

            var selectCmd = new NpgsqlCommand("SELECT MAX(oid) FROM data", Conn);
            var previousOid = (uint)selectCmd.ExecuteScalar();

            var reader = insertCmd.ExecuteReader();
            Assert.That(reader.LastInsertedOID, Is.EqualTo(previousOid + 1));
        }

        [Test]
        public void GetStringWithParameter()
        {
            const string text = "Random text";
            ExecuteNonQuery(String.Format(@"INSERT INTO data (field_text) VALUES ('{0}')", text));

            var command = new NpgsqlCommand("SELECT field_text FROM data WHERE field_text = :value;", Conn);
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
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Text with '' single quote')");

            const string test = "Text with ' single quote";
            var command = new NpgsqlCommand("SELECT field_text FROM data WHERE field_text = :value;", Conn);

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
        public void GetDataTypeName()
        {
            var command = new NpgsqlCommand(@"SELECT 1::INT4 AS some_column", Conn);
            var dr = command.ExecuteReader();
            dr.Read();
            Assert.That(dr.GetDataTypeName(0), Is.EqualTo("int4"));
            command.Dispose();
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
            var command = new NpgsqlCommand(@"SELECT 0, 1 AS some_column", Conn);
            var dr = command.ExecuteReader();
            dr.Read();
            Assert.That(dr.GetOrdinal("some_column"), Is.EqualTo(1));
            Assert.That(() => dr.GetOrdinal("doesn't_exist"), Throws.Exception.TypeOf<IndexOutOfRangeException>());
            command.Dispose();
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
            var command = new NpgsqlCommand("select * from data where field_text = NULL;", Conn);
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
            var command = new NpgsqlCommand("select * from data where field_text = :value;", Conn);

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
        public void TestOverlappedParameterNames()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :test_name or field_serial = :test_name_long", Conn);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "a"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            using (var dr = command.ExecuteReader())
            {
                Assert.IsNotNull(dr);
            }
        }

        [Test]
        public void TestOverlappedParameterNamesWithPrepare()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :test_name or field_serial = :test_name_long", Conn);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "abc_de"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "abc_defg"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            command.Prepare();

            using (var dr = command.ExecuteReader())
            {
                Assert.IsNotNull(dr);
            }
        }

        [Test]
        public void NonExistentParameterName()
        {
            var cmd = new NpgsqlCommand("SELECT * FROM data WHERE field_text=@x", Conn);
            Assert.That(() => cmd.ExecuteReader(), Throws.Exception);
        }

        [Test]
        public void UseDataAdapter()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            var da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapter.xml");
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
            command.Connection = Conn;
            var da = new NpgsqlDataAdapter(command);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            var da = new NpgsqlDataAdapter("select * from data", Conn);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            var da = new NpgsqlDataAdapter("select * from data", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            var da = new NpgsqlDataAdapter("select * from data", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        public void DataGridWebControlSupport()
        {
            var command = new NpgsqlCommand("select * from data;", Conn);
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
            var command = new NpgsqlCommand("select * from data;", Conn);
            var dr = command.ExecuteReader();
            while (dr.Read()) {}
            Object o = dr[0];
            Assert.IsNotNull(o);
        }

        [Test]
        public void TypesNames()
        {
            var command = new NpgsqlCommand("SELECT field_int2, field_int4, field_int8, field_numeric, field_text, field_bool, field_timestamp FROM data WHERE 1 = 2;", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual("int2", dr.GetDataTypeName(0));
                Assert.AreEqual("int4", dr.GetDataTypeName(1));
                Assert.AreEqual("int8", dr.GetDataTypeName(2));
                Assert.AreEqual("numeric", dr.GetDataTypeName(3));
                Assert.AreEqual("text", dr.GetDataTypeName(4));
                Assert.AreEqual("bool", dr.GetDataTypeName(5));
                Assert.AreEqual("timestamp", dr.GetDataTypeName(6));
            }
        }

        [Test]
        public void SchemaOnly([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand(
                "SELECT 1 AS some_column;" +
                "UPDATE data SET field_text='yo' WHERE 1=0;" +
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
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            var command = new NpgsqlCommand(@"SELECT * FROM data", Conn);
            var reader = command.ExecuteReader(CommandBehavior.SingleRow);
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.Read(), Is.False);
            reader.Close();
            command.Dispose();
        }

        [Test]
        public void SingleRowSequentialCommandBehaviorSupport()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text, field_bool) VALUES ('X', true)");
            ExecuteNonQuery(@"INSERT INTO data (field_text, field_bool) VALUES ('Y', false)");
            var command = new NpgsqlCommand("SELECT field_int4, field_text, field_char5, field_float4, field_date, field_bool FROM data ORDER BY field_text", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess))
            {
                Assert.That(dr.Read(), Is.True);
                Assert.IsTrue(dr.IsDBNull(0));  // field_int4
                Assert.IsFalse(dr.IsDBNull(1)); // field_text
                Assert.That(dr.GetString(1), Is.EqualTo("X"));
                Assert.IsTrue(dr.IsDBNull(2));  // field_char5
                // field_float4 - do not read this field to test skip
                Assert.IsTrue(dr.IsDBNull(4));  // field_date
                Assert.IsFalse(dr.IsDBNull(5)); // field_bool
                Assert.That(dr.GetValue(5), Is.True);

                Assert.That(dr.Read(), Is.False);
            }
        }

        [Test, Description("In sequential access, performing a null check on a non-first field would check the first field")]
        public void SequentialNullCheckOnNonFirstField()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            using (var cmd = new NpgsqlCommand("SELECT field_text, field_int4 FROM data", Conn))
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
            ExecuteNonQuery(@"CREATE TEMPORARY TABLE DATA2 (
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
            using (var command = new NpgsqlCommand("SELECT * FROM data", Conn))
            {
                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    dr.Read();
                    var metadata = dr.GetSchemaTable();
                    var key = metadata.Rows.Cast<DataRow>().Single(r => (bool) r["IsKey"]);
                    Assert.That(key["ColumnName"], Is.EqualTo("field_pk"));
                }
            }
        }

        [Test]
        public void IsAutoIncrementMetadataSupport()
        {
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
            ExecuteNonQuery("CREATE OR REPLACE VIEW DataView (field_pk, field_int2) AS select field_pk, field_int2 + field_int2 as field_int2 from data");

            var command = new NpgsqlCommand("SELECT * FROM DataView", Conn);

            using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
            {
                var metadata = dr.GetSchemaTable();

                foreach (DataRow r in metadata.Rows)
                {
                    switch ((string)r["ColumnName"])
                    {
                        case "field_pk":
                            Assert.IsFalse((bool)r["IsReadonly"]);
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
        public void HasRowsWithoutResultset()
        {
            var command = new NpgsqlCommand("delete from data where field_serial = 2000000", Conn);
            var dr = command.ExecuteReader();
            Assert.IsFalse(dr.HasRows);
        }

        [Test]
        public void ParameterAppearMoreThanOneTime()
        {
            var command = new NpgsqlCommand("select * from data where field_serial = :parameter and field_int4 = :parameter", Conn);
            command.Parameters.Add("parameter", NpgsqlDbType.Integer);
            command.Parameters["parameter"].Value = 1;
            using (var dr = command.ExecuteReader())
            {
                Assert.IsFalse(dr.HasRows);
            }
        }

        [Test]
        public void SchemaOnlySingleRowCommandBehaviorSupport()
        {
            var command = new NpgsqlCommand("select * from data", Conn);
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
            var command = new NpgsqlCommand("select * from data", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() returns setof data as '
                              select * from data;
                              ' language 'sql';");
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

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteReaderBeforeClosingReader()
        {
            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);
            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            using (var dr2 = cmd2.ExecuteReader())
            {
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteScalarBeforeClosingReader()
        {
            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);

            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            {
                cmd2.ExecuteScalar();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteNonQueryBeforeClosingReader()
        {
            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);
            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            {
                cmd2.ExecuteNonQuery();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrepareBeforeClosingReader()
        {
            var cmd1 = new NpgsqlCommand("select field_serial from data", Conn);
            using (var dr1 = cmd1.ExecuteReader())
            using (var cmd2 = new NpgsqlCommand("select * from data", Conn))
            {
                cmd2.Prepare();
            }
        }

        [Test]
        public void LoadDataTable()
        {
            using (var command = new NpgsqlCommand("SELECT field_char5, field_varchar5 FROM data", Conn))
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
            var command = new NpgsqlCommand("select * from data", Conn);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                dr.Close();

                using (var upd = Conn.CreateCommand())
                {
                    upd.CommandText = "select * from data";
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
        public void HasRows()
        {
            var command = new NpgsqlCommand("SELECT 1; SELECT * FROM data WHERE field_text='does_not_exist'", Conn);
            using (var dr = command.ExecuteReader())
            {
                Assert.That(dr.HasRows, Is.True);
                Assert.That(dr.Read(), Is.True);
                Assert.That(dr.HasRows, Is.True);
                Assert.That(dr.Read(), Is.False);
                dr.NextResult();
                Assert.That(dr.HasRows, Is.False);
            }
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
    internal class SafeExceptionGeneratingHandler : TypeHandler<int>, ISimpleTypeReader<int>, ISimpleTypeWriter
    {
        public int Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            buf.ReadInt32();
            throw new SafeReadException(new Exception("Safe read exception as requested"));
        }

        public int ValidateAndGetLength(object value) { throw new NotSupportedException(); }
        public void Write(object value, NpgsqlBuffer buf) { throw new NotSupportedException(); }
    }

    internal class NonSafeExceptionGeneratingHandler : TypeHandler<int>, ISimpleTypeReader<int>, ISimpleTypeWriter
    {
        public int Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            throw new Exception("Non-safe read exception as requested");
        }

        public int ValidateAndGetLength(object value) { throw new NotSupportedException(); }
        public void Write(object value, NpgsqlBuffer buf) { throw new NotSupportedException();}
    }
#endif
    #endregion
}
