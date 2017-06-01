#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

#if !NETCOREAPP1_1

using System;
using System.Data;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;

namespace Npgsql.Tests
{
    public class DataAdapterTests : TestBase
    {
        [Test]
        public void UseDataAdapter()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT 1", conn))
            {
                var da = new NpgsqlDataAdapter();
                da.SelectCommand = command;
                var ds = new DataSet();
                da.Fill(ds);
                //ds.WriteXml("TestUseDataAdapter.xml");
            }
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT 1", conn))
            {
                command.Connection = conn;
                var da = new NpgsqlDataAdapter(command);
                var ds = new DataSet();
                da.Fill(ds);
                //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
            }
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            using (var conn = OpenConnection())
            {
                var da = new NpgsqlDataAdapter("SELECT 1", conn);
                var ds = new DataSet();
                da.Fill(ds);
                //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
            }
        }

        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            var da = new NpgsqlDataAdapter("SELECT 1", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            var da = new NpgsqlDataAdapter("SELECT 1", ConnectionString);
            var ds = new DataSet();
            da.Fill(ds);
            //ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        [MonoIgnore("Bug in mono, submitted pull request: https://github.com/mono/mono/pull/1172")]
        public void InsertWithDataSet()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("SELECT * FROM data", conn);

                da.InsertCommand = new NpgsqlCommand("INSERT INTO data (field_int2, field_timestamp, field_numeric) VALUES (:a, :b, :c)", conn);

                da.InsertCommand.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
                da.InsertCommand.Parameters.Add(new NpgsqlParameter("b", DbType.DateTime));
                da.InsertCommand.Parameters.Add(new NpgsqlParameter("c", DbType.Decimal));

                da.InsertCommand.Parameters[0].Direction = ParameterDirection.Input;
                da.InsertCommand.Parameters[1].Direction = ParameterDirection.Input;
                da.InsertCommand.Parameters[2].Direction = ParameterDirection.Input;

                da.InsertCommand.Parameters[0].SourceColumn = "field_int2";
                da.InsertCommand.Parameters[1].SourceColumn = "field_timestamp";
                da.InsertCommand.Parameters[2].SourceColumn = "field_numeric";

                da.Fill(ds);

                var dt = ds.Tables[0];
                var dr = dt.NewRow();
                dr["field_int2"] = 4;
                dr["field_timestamp"] = new DateTime(2003, 01, 30, 14, 0, 0);
                dr["field_numeric"] = 7.3M;
                dt.Rows.Add(dr);

                var ds2 = ds.GetChanges();
                da.Update(ds2);

                ds.Merge(ds2);
                ds.AcceptChanges();

                var dr2 = new NpgsqlCommand("SELECT field_int2, field_numeric, field_timestamp FROM data", conn).ExecuteReader();
                dr2.Read();

                Assert.AreEqual(4, dr2[0]);
                Assert.AreEqual(7.3000000M, dr2[1]);
                dr2.Close();
            }
        }

        [Test]
        public void DataAdapterUpdateReturnValue()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("SELECT * FROM data", conn);

                da.InsertCommand = new NpgsqlCommand(@"INSERT INTO data (field_int2, field_timestamp, field_numeric) VALUES (:a, :b, :c)", conn);

                da.InsertCommand.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
                da.InsertCommand.Parameters.Add(new NpgsqlParameter("b", DbType.DateTime));
                da.InsertCommand.Parameters.Add(new NpgsqlParameter("c", DbType.Decimal));

                da.InsertCommand.Parameters[0].Direction = ParameterDirection.Input;
                da.InsertCommand.Parameters[1].Direction = ParameterDirection.Input;
                da.InsertCommand.Parameters[2].Direction = ParameterDirection.Input;

                da.InsertCommand.Parameters[0].SourceColumn = "field_int2";
                da.InsertCommand.Parameters[1].SourceColumn = "field_timestamp";
                da.InsertCommand.Parameters[2].SourceColumn = "field_numeric";

                da.Fill(ds);

                var dt = ds.Tables[0];
                var dr = dt.NewRow();
                dr["field_int2"] = 4;
                dr["field_timestamp"] = new DateTime(2003, 01, 30, 14, 0, 0);
                dr["field_numeric"] = 7.3M;
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr["field_int2"] = 4;
                dr["field_timestamp"] = new DateTime(2003, 01, 30, 14, 0, 0);
                dr["field_numeric"] = 7.3M;
                dt.Rows.Add(dr);

                var ds2 = ds.GetChanges();
                var daupdate = da.Update(ds2);

                Assert.AreEqual(2, daupdate);
            }
        }

        [Test]
        [Ignore("")]
        public void DataAdapterUpdateReturnValue2()
        {
            using (var conn = OpenConnection())
            {
                var cmd = conn.CreateCommand();
                var da = new NpgsqlDataAdapter("select * from tabled", conn);
                var cb = new NpgsqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);

                //## Insert a new row with id = 1
                ds.Tables[0].Rows.Add(new Object[] {0.4, 0.5});
                da.Update(ds);

                //## change id from 1 to 2
                cmd.CommandText = "update tabled set field_float4 = 0.8";
                cmd.ExecuteNonQuery();

                //## change value to newvalue
                ds.Tables[0].Rows[0][1] = 0.7;
                //## update should fail, and make a DBConcurrencyException
                var count = da.Update(ds);
                //## count is 1, even if the isn't updated in the database
                Assert.AreEqual(0, count);
            }
        }

        [Test]
        public void FillWithEmptyResultset()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("SELECT field_serial, field_int2, field_timestamp, field_numeric FROM data WHERE field_serial = -1", conn);

                da.Fill(ds);

                Assert.AreEqual(1, ds.Tables.Count);
                Assert.AreEqual(4, ds.Tables[0].Columns.Count);
                Assert.AreEqual("field_serial", ds.Tables[0].Columns[0].ColumnName);
                Assert.AreEqual("field_int2", ds.Tables[0].Columns[1].ColumnName);
                Assert.AreEqual("field_timestamp", ds.Tables[0].Columns[2].ColumnName);
                Assert.AreEqual("field_numeric", ds.Tables[0].Columns[3].ColumnName);
            }
        }

        [Test]
        [Ignore("")]
        public void FillAddWithKey()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("select field_serial, field_int2, field_timestamp, field_numeric from tableb", conn);

                da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                da.Fill(ds);

                var field_serial = ds.Tables[0].Columns[0];
                var field_int2 = ds.Tables[0].Columns[1];
                var field_timestamp = ds.Tables[0].Columns[2];
                var field_numeric = ds.Tables[0].Columns[3];

                Assert.IsFalse(field_serial.AllowDBNull);
                Assert.IsTrue(field_serial.AutoIncrement);
                Assert.AreEqual("field_serial", field_serial.ColumnName);
                Assert.AreEqual(typeof(int), field_serial.DataType);
                Assert.AreEqual(0, field_serial.Ordinal);
                Assert.IsTrue(field_serial.Unique);

                Assert.IsTrue(field_int2.AllowDBNull);
                Assert.IsFalse(field_int2.AutoIncrement);
                Assert.AreEqual("field_int2", field_int2.ColumnName);
                Assert.AreEqual(typeof(short), field_int2.DataType);
                Assert.AreEqual(1, field_int2.Ordinal);
                Assert.IsFalse(field_int2.Unique);

                Assert.IsTrue(field_timestamp.AllowDBNull);
                Assert.IsFalse(field_timestamp.AutoIncrement);
                Assert.AreEqual("field_timestamp", field_timestamp.ColumnName);
                Assert.AreEqual(typeof(DateTime), field_timestamp.DataType);
                Assert.AreEqual(2, field_timestamp.Ordinal);
                Assert.IsFalse(field_timestamp.Unique);

                Assert.IsTrue(field_numeric.AllowDBNull);
                Assert.IsFalse(field_numeric.AutoIncrement);
                Assert.AreEqual("field_numeric", field_numeric.ColumnName);
                Assert.AreEqual(typeof(decimal), field_numeric.DataType);
                Assert.AreEqual(3, field_numeric.Ordinal);
                Assert.IsFalse(field_numeric.Unique);
            }
        }

        [Test]
        public void FillAddColumns()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter(@"SELECT field_serial, field_int2, field_timestamp, field_numeric FROM data", conn);

                da.MissingSchemaAction = MissingSchemaAction.Add;
                da.Fill(ds);

                var field_serial = ds.Tables[0].Columns[0];
                var field_int2 = ds.Tables[0].Columns[1];
                var field_timestamp = ds.Tables[0].Columns[2];
                var field_numeric = ds.Tables[0].Columns[3];

                Assert.AreEqual("field_serial", field_serial.ColumnName);
                Assert.AreEqual(typeof(int), field_serial.DataType);
                Assert.AreEqual(0, field_serial.Ordinal);

                Assert.AreEqual("field_int2", field_int2.ColumnName);
                Assert.AreEqual(typeof(short), field_int2.DataType);
                Assert.AreEqual(1, field_int2.Ordinal);

                Assert.AreEqual("field_timestamp", field_timestamp.ColumnName);
                Assert.AreEqual(typeof(DateTime), field_timestamp.DataType);
                Assert.AreEqual(2, field_timestamp.Ordinal);

                Assert.AreEqual("field_numeric", field_numeric.ColumnName);
                Assert.AreEqual(typeof(decimal), field_numeric.DataType);
                Assert.AreEqual(3, field_numeric.Ordinal);
            }
        }

        [Test]
        [MonoIgnore("Bug in mono, submitted pull request: https://github.com/mono/mono/pull/1172")]
        public void UpdateLettingNullFieldValue()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var command = new NpgsqlCommand(@"INSERT INTO data (field_int2) VALUES (2)", conn);
                command.ExecuteNonQuery();

                var ds = new DataSet();

                var da = new NpgsqlDataAdapter("SELECT * FROM data", conn);
                da.InsertCommand = new NpgsqlCommand(";", conn);
                da.UpdateCommand = new NpgsqlCommand("UPDATE data SET field_int2 = :a, field_timestamp = :b, field_numeric = :c WHERE field_serial = :d", conn);

                da.UpdateCommand.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
                da.UpdateCommand.Parameters.Add(new NpgsqlParameter("b", DbType.DateTime));
                da.UpdateCommand.Parameters.Add(new NpgsqlParameter("c", DbType.Decimal));
                da.UpdateCommand.Parameters.Add(new NpgsqlParameter("d", NpgsqlDbType.Bigint));

                da.UpdateCommand.Parameters[0].Direction = ParameterDirection.Input;
                da.UpdateCommand.Parameters[1].Direction = ParameterDirection.Input;
                da.UpdateCommand.Parameters[2].Direction = ParameterDirection.Input;
                da.UpdateCommand.Parameters[3].Direction = ParameterDirection.Input;

                da.UpdateCommand.Parameters[0].SourceColumn = "field_int2";
                da.UpdateCommand.Parameters[1].SourceColumn = "field_timestamp";
                da.UpdateCommand.Parameters[2].SourceColumn = "field_numeric";
                da.UpdateCommand.Parameters[3].SourceColumn = "field_serial";

                da.Fill(ds);

                var dt = ds.Tables[0];
                Assert.IsNotNull(dt);

                var dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
                dr["field_int2"] = 4;

                var ds2 = ds.GetChanges();
                da.Update(ds2);
                ds.Merge(ds2);
                ds.AcceptChanges();

                using (var dr2 = new NpgsqlCommand(@"SELECT field_int2 FROM data", conn).ExecuteReader())
                {
                    dr2.Read();
                    Assert.AreEqual(4, dr2["field_int2"]);
                }
            }
        }

        [Test]
        public void FillWithDuplicateColumnName()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("SELECT field_serial, field_serial FROM data", conn);
                da.Fill(ds);
            }
        }

        [Test]
        [Ignore("")]
        public void UpdateWithDataSet()
        {
            DoUpdateWithDataSet();
        }

        public virtual void DoUpdateWithDataSet()
        {
            using (var conn = OpenConnection())
            {
                var command = new NpgsqlCommand("insert into tableb(field_int2) values (2)", conn);
                command.ExecuteNonQuery();

                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("select * from tableb", conn);
                var cb = new NpgsqlCommandBuilder(da);
                Assert.IsNotNull(cb);

                da.Fill(ds);

                var dt = ds.Tables[0];
                Assert.IsNotNull(dt);

                var dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];

                dr["field_int2"] = 4;

                var ds2 = ds.GetChanges();
                da.Update(ds2);
                ds.Merge(ds2);
                ds.AcceptChanges();

                using (var dr2 = new NpgsqlCommand("select * from tableb", conn).ExecuteReader())
                {
                    dr2.Read();
                    Assert.AreEqual(4, dr2["field_int2"]);
                }
            }
        }

        [Test]
        [Ignore("")]
        public void InsertWithCommandBuilderCaseSensitive()
        {
            DoInsertWithCommandBuilderCaseSensitive();
        }

        public virtual void DoInsertWithCommandBuilderCaseSensitive()
        {
            using (var conn = OpenConnection())
            {
                var ds = new DataSet();
                var da = new NpgsqlDataAdapter("select * from tablei", conn);
                var builder = new NpgsqlCommandBuilder(da);
                Assert.IsNotNull(builder);

                da.Fill(ds);

                var dt = ds.Tables[0];
                var dr = dt.NewRow();
                dr["Field_Case_Sensitive"] = 4;
                dt.Rows.Add(dr);

                var ds2 = ds.GetChanges();
                da.Update(ds2);
                ds.Merge(ds2);
                ds.AcceptChanges();

                using (var dr2 = new NpgsqlCommand("select * from tablei", conn).ExecuteReader())
                {
                    dr2.Read();
                    Assert.AreEqual(4, dr2[1]);
                }
            }
        }

        [Test]
        public void IntervalAsTimeSpan()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE TEMP TABLE data (" +
                                      "  pk SERIAL PRIMARY KEY, " +
                                      "  interval INTERVAL" +
                                      ")");
                conn.ExecuteNonQuery(@"INSERT INTO data (interval) VALUES ('1 hour'::INTERVAL)");

                var dt = new DataTable("data");
                var command = new NpgsqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "SELECT interval FROM data",
                    Connection = conn
                };
                var da = new NpgsqlDataAdapter {SelectCommand = command};
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    //Console.Out.WriteLine(dr["interval"]);
                }
            }
        }

        [Test]
        public void IntervalAsTimeSpan2()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE TEMP TABLE data (" +
                                      "  pk SERIAL PRIMARY KEY, " +
                                      "  interval INTERVAL" +
                                      ")");
                conn.ExecuteNonQuery(@"INSERT INTO data (interval) VALUES ('1 hour'::INTERVAL)");

                var dt = new DataTable("data");
                //DataColumn c = dt.Columns.Add("dauer", typeof(TimeSpan));
                // DataColumn c = dt.Columns.Add("dauer", typeof(NpgsqlInterval));
                //c.AllowDBNull = true;
                var command = new NpgsqlCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT interval FROM data";
                command.Connection = conn;
                var da = new NpgsqlDataAdapter();
                da.SelectCommand = command;
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    //Console.Out.WriteLine(dr["interval"]);
                }
            }
        }

        [Test]
        public void DbDataAdapterCommandAccess()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT CAST('1 hour' AS interval) AS dauer", conn))
            {
                Setup(conn);
                var da = new NpgsqlDataAdapter();
                da.SelectCommand = command;
                System.Data.Common.DbDataAdapter common = da;
                Assert.IsNotNull(common.SelectCommand);
            }
        }

        [Test, Description("Makes sure that the INSERT/UPDATE/DELETE commands are auto-populated on NpgsqlDataAdapter")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/179")]
        [Ignore("Somehow related to us using a temporary table???")]
        public void AutoPopulateAdapterCommands()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                var da = new NpgsqlDataAdapter("SELECT field_pk,field_int4 FROM data", conn);
                var builder = new NpgsqlCommandBuilder(da);
                var ds = new DataSet();
                da.Fill(ds);

                var table = ds.Tables[0];
                var row = table.NewRow();
                row["field_pk"] = 1;
                row["field_int4"] = 8;
                table.Rows.Add(row);
                da.Update(ds);
                Assert.That(conn.ExecuteScalar(@"SELECT field_int4 FROM data"), Is.EqualTo(8));

                row["field_int4"] = 9;
                da.Update(ds);
                Assert.That(conn.ExecuteScalar(@"SELECT field_int4 FROM data"), Is.EqualTo(9));

                row.Delete();
                da.Update(ds);
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test]
        public void CommandBuilderQuoting()
        {
            var cb = new NpgsqlCommandBuilder();
            const string orig = "some\"column";
            var quoted = cb.QuoteIdentifier(orig);
            Assert.That(quoted, Is.EqualTo("\"some\"\"column\""));
            Assert.That(cb.UnquoteIdentifier(quoted), Is.EqualTo(orig));
        }

        [Test, Description("Makes sure a correct SQL string is built with GetUpdateCommand(true) using correct parameter names and placeholders")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/397")]
        [Ignore("Somehow related to us using a temporary table???")]
        public void GetUpdateCommand()
        {
            using (var conn = OpenConnection())
            {
                Setup(conn);
                using (var da = new NpgsqlDataAdapter("SELECT field_pk, field_int4 FROM data", conn))
                {
                    using (var cb = new NpgsqlCommandBuilder(da))
                    {
                        var updateCommand = cb.GetUpdateCommand(true);
                        da.UpdateCommand = updateCommand;

                        var ds = new DataSet();
                        da.Fill(ds);

                        var table = ds.Tables[0];
                        var row = table.Rows.Add();
                        row["field_pk"] = 1;
                        row["field_int4"] = 1;
                        da.Update(ds);

                        row["field_int4"] = 2;
                        da.Update(ds);

                        row.Delete();
                        da.Update(ds);
                    }
                }
            }
        }

        [Test]
        public void LoadDataTable()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (char5 CHAR(5), varchar5 VARCHAR(5))");
                using (var command = new NpgsqlCommand("SELECT char5, varchar5 FROM data", conn))
                using (var dr = command.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(dr);
                    dr.Close();

                    Assert.AreEqual(5, dt.Columns[0].MaxLength);
                    Assert.AreEqual(5, dt.Columns[1].MaxLength);
                }
            }
        }

        public void Setup(NpgsqlConnection conn)
        {
            conn.ExecuteNonQuery("CREATE TEMP TABLE data (" +
                                 "field_pk SERIAL PRIMARY KEY," +
                                 "field_serial SERIAL," +
                                 "field_int2 SMALLINT," +
                                 "field_int4 INTEGER," +
                                 "field_numeric NUMERIC," +
                                 "field_timestamp TIMESTAMP" +
                                 ")");
        }
    }
}

#endif