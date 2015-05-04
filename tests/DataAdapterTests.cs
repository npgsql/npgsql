// created on 3/5/2003 at 14:29
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
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{

    [TestFixture]
    public class DataAdapterTests : TestBase
    {
        public DataAdapterTests(string backendVersion) : base(backendVersion) { }

        [Test]
        [MonoIgnore("Bug in mono, submitted pull request: https://github.com/mono/mono/pull/1172")]
        public void InsertWithDataSet()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("SELECT * FROM data", Conn);

            da.InsertCommand = new NpgsqlCommand("INSERT INTO data (field_int2, field_timestamp, field_numeric) VALUES (:a, :b, :c)", Conn);

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

            var dr2 = new NpgsqlCommand("SELECT field_int2, field_numeric, field_timestamp FROM data", Conn).ExecuteReader();
            dr2.Read();

            Assert.AreEqual(4, dr2[0]);
            Assert.AreEqual(7.3000000M, dr2[1]);
            dr2.Close();
        }

        [Test]
        public void DataAdapterUpdateReturnValue()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("SELECT * FROM data", Conn);

            da.InsertCommand = new NpgsqlCommand(@"INSERT INTO data (field_int2, field_timestamp, field_numeric) VALUES (:a, :b, :c)", Conn);

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

        [Test]
        [Ignore]
        public void DataAdapterUpdateReturnValue2()
        {
            var cmd = Conn.CreateCommand();
            var da = new NpgsqlDataAdapter("select * from tabled", Conn);
            var cb = new NpgsqlCommandBuilder(da);
            var ds = new DataSet();
            da.Fill(ds);

            //## Insert a new row with id = 1
            ds.Tables[0].Rows.Add(new Object[] {0.4, 0.5});
            da.Update(ds);

            //## change id from 1 to 2
            cmd.CommandText = "update tabled set field_float4 = 0.8 where id = (select max(field_serial) from tabled)";
            cmd.ExecuteNonQuery();

            //## change value to newvalue
            ds.Tables[0].Rows[0][1] = 0.7;
            //## update should fail, and make a DBConcurrencyException
            var count = da.Update(ds);
            //## count is 1, even if the isn't updated in the database
            Assert.AreEqual(0, count);
        }

        [Test]
        public void FillWithEmptyResultset()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("SELECT field_serial, field_int2, field_timestamp, field_numeric FROM data WHERE field_serial = -1", Conn);

            da.Fill(ds);

            Assert.AreEqual(1, ds.Tables.Count);
            Assert.AreEqual(4, ds.Tables[0].Columns.Count);
            Assert.AreEqual("field_serial", ds.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual("field_int2", ds.Tables[0].Columns[1].ColumnName);
            Assert.AreEqual("field_timestamp", ds.Tables[0].Columns[2].ColumnName);
            Assert.AreEqual("field_numeric", ds.Tables[0].Columns[3].ColumnName);
        }

        [Test]
        [Ignore]
        public void FillAddWithKey()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("select field_serial, field_int2, field_timestamp, field_numeric from tableb", Conn);

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
            // version 2 of the protocol doesn't know how to populate the unique field
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

        [Test]
        public void FillAddColumns()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter(@"SELECT field_serial, field_int2, field_timestamp, field_numeric FROM data", Conn);

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

        [Test]
        [MonoIgnore("Bug in mono, submitted pull request: https://github.com/mono/mono/pull/1172")]
        public void UpdateLettingNullFieldValue()
        {
            var command = new NpgsqlCommand(@"INSERT INTO data (field_int2) VALUES (2)", Conn);
            command.ExecuteNonQuery();

            var ds = new DataSet();

            var da = new NpgsqlDataAdapter("SELECT * FROM data", Conn);
            da.InsertCommand = new NpgsqlCommand(";", Conn);
            da.UpdateCommand = new NpgsqlCommand("UPDATE data SET field_int2 = :a, field_timestamp = :b, field_numeric = :c WHERE field_serial = :d", Conn);

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

            using (var dr2 = new NpgsqlCommand(@"SELECT field_int2 FROM data", Conn).ExecuteReader())
            {
                dr2.Read();
                Assert.AreEqual(4, dr2["field_int2"]);
            }
        }

        [Test]
        public void FillWithDuplicateColumnName()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("SELECT field_serial, field_serial FROM data", Conn);
            da.Fill(ds);
        }

        [Test]
        [Ignore]
        public void UpdateWithDataSet()
        {
            DoUpdateWithDataSet();
        }

        public virtual void DoUpdateWithDataSet()
        {
            var command = new NpgsqlCommand("insert into tableb(field_int2) values (2)", Conn);
            command.ExecuteNonQuery();

            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("select * from tableb where field_serial = (select max(field_serial) from tableb)", Conn);
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

            using (var dr2 = new NpgsqlCommand("select * from tableb where field_serial = (select max(field_serial) from tableb)", Conn).ExecuteReader())
            {
                dr2.Read();
                Assert.AreEqual(4, dr2["field_int2"]);
            }
        }

        [Test]
        [Ignore]
        public void InsertWithCommandBuilderCaseSensitive()
        {
            DoInsertWithCommandBuilderCaseSensitive();
        }
        public virtual void DoInsertWithCommandBuilderCaseSensitive()
        {
            var ds = new DataSet();
            var da = new NpgsqlDataAdapter("select * from tablei", Conn);
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

            using (var dr2 = new NpgsqlCommand("select * from tablei", Conn).ExecuteReader())
            {
                dr2.Read();
                Assert.AreEqual(4, dr2[1]);
            }
        }

        [Test]
        public void IntervalAsTimeSpan()
        {
            var dt = new DataTable();
            var c = dt.Columns.Add("dauer", typeof(TimeSpan));
            // DataColumn c = dt.Columns.Add("dauer", typeof(NpgsqlInterval));
            c.AllowDBNull = true;
            var command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = Conn;
            var da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                Console.Out.WriteLine(dr["dauer"]);
            }
        }

        [Test]
        public void IntervalAsTimeSpan2()
        {
            var dt = new DataTable();
            //DataColumn c = dt.Columns.Add("dauer", typeof(TimeSpan));
            // DataColumn c = dt.Columns.Add("dauer", typeof(NpgsqlInterval));
            //c.AllowDBNull = true;
            var command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = Conn;
            var da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            da.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                Console.Out.WriteLine(dr["dauer"]);
            }
        }

        [Test]
        public void DbDataAdapterCommandAccess()
        {
            var command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = Conn;
            var da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            System.Data.Common.DbDataAdapter common = da;
            Assert.IsNotNull(common.SelectCommand);
        }

        [Test, Description("Makes sure that the INSERT/UPDATE/DELETE commands are auto-populated on NpgsqlDataAdapter (issue #179)")]
        public void AutoPopulateAdapterCommands()
        {
            var da = new NpgsqlDataAdapter("SELECT field_pk,field_int4 FROM data", Conn);
            var builder = new NpgsqlCommandBuilder(da);
            var ds = new DataSet();
            da.Fill(ds);

            var table = ds.Tables[0];
            var row = table.NewRow();
            row["field_pk"] = 1;
            row["field_int4"] = 8;
            table.Rows.Add(row);
            da.Update(ds);
            Assert.That(ExecuteScalar(@"SELECT field_int4 FROM data"), Is.EqualTo(8));

            row["field_int4"] = 9;
            da.Update(ds);
            Assert.That(ExecuteScalar(@"SELECT field_int4 FROM data"), Is.EqualTo(9));

            row.Delete();
            da.Update(ds);
            Assert.That(ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
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

        [Test]
        public void Bug352() {
            var ds = new DataSet();
            ds.Reset();
            // System.InvalidCastException will occur if it contains any of:
            // field_date
            // field_timestamp
            // field_time
            // field_timestamp_with_timezone
            // field_inet
            var da = new NpgsqlDataAdapter("SELECT field_pk,field_date,field_int4 FROM data", Conn);
            da.Fill(ds, "data");

            da.RowUpdating += (sender2, e2) => {
                // this workaround needs to be before you getting NpgsqlCommandBuilder.
                e2.Command.Parameters.Clear();
            };

            var cb = new NpgsqlCommandBuilder(da as NpgsqlDataAdapter);
            //cb.SetAllValues = false; // default is false
            //cb.SetAllValues = true; // System.InvalidCastException won't be raised if set to true.
            cb.ConflictOption = ConflictOption.OverwriteChanges;
            da.InsertCommand = cb.GetInsertCommand();
            da.DeleteCommand = cb.GetDeleteCommand();
            da.UpdateCommand = cb.GetUpdateCommand();
            da.RowUpdated += (sender2, e2) => {
                if (e2.StatementType == StatementType.Insert) {
                    var cm = new NpgsqlCommand("SELECT currval('data_field_pk_seq')", Conn);
                    e2.Row["field_pk"] = cm.ExecuteScalar(); // rewrite pk on DataTable
                }
            };
            var table = ds.Tables["data"];

            var row = table.NewRow();
            row["field_int4"] = 8;
            table.Rows.Add(row);

            da.Update(ds, "data"); // insert has no problem.

            row["field_int4"] = 7;
            da.Update(ds, "data"); // <-- update has problem: System.InvalidCastException

            row.Delete();
            da.Update(ds, "data"); // delete has no problem.
        }
    }
    /*
    [TestFixture]
    public class DataAdapterTestsV2 : DataAdapterTests
    {
        public DataAdapterTestsV2(int backendProtocolVersion) : base(backendProtocolVersion) {}

        public override void DoInsertWithCommandBuilderCaseSensitive()
        {
            //Not possible with V2?
        }
        public override void DoUpdateWithDataSet()
        {
            //Not possible with V2?
        }
    }
     */
}
