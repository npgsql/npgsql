// created on 3/5/2003 at 14:29
//
// Author:
// 	Francisco Figueiredo Jr. <fxjrlists@yahoo.com>
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
using System.Web.UI.WebControls;
using Npgsql;

using NpgsqlTypes;

using NUnit.Framework;
using NUnit.Core;

namespace NpgsqlTests
{

    [TestFixture]
    public class DataAdapterTests : BaseClassTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _conn;}
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _t; }
            set { _t = value; }
        }
        [Test]
        public void InsertWithDataSet()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", TheConnection);

            da.InsertCommand = new NpgsqlCommand("insert into tableb(field_int2, field_timestamp, field_numeric) values (:a, :b, :c)", TheConnection);

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


            DataTable dt = ds.Tables[0];

            DataRow dr = dt.NewRow();
            dr["field_int2"] = 4;
            dr["field_timestamp"] = new DateTime(2003, 01, 30, 14, 0, 0);
            dr["field_numeric"] = 7.3M;
            

            dt.Rows.Add(dr);


            DataSet ds2 = ds.GetChanges();
            

            da.Update(ds2);
            
            
            ds.Merge(ds2);
            ds.AcceptChanges();


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tableb where field_serial > 4", TheConnection).ExecuteReader();
            dr2.Read();


            Assert.AreEqual(4, dr2[1]);
            Assert.AreEqual(7.3000000M, dr2[3]);
            dr2.Close();
        }
        
        [Test]
        public void DataAdapterUpdateReturnValue()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", TheConnection);

            da.InsertCommand = new NpgsqlCommand("insert into tableb(field_int2, field_timestamp, field_numeric) values (:a, :b, :c)", TheConnection);

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


            DataTable dt = ds.Tables[0];

            DataRow dr = dt.NewRow();
            dr["field_int2"] = 4;
            dr["field_timestamp"] = new DateTime(2003, 01, 30, 14, 0, 0);
            dr["field_numeric"] = 7.3M;
            

            dt.Rows.Add(dr);
            
            dr = dt.NewRow();
            dr["field_int2"] = 4;
            dr["field_timestamp"] = new DateTime(2003, 01, 30, 14, 0, 0);
            dr["field_numeric"] = 7.3M;
            
            dt.Rows.Add(dr);


            DataSet ds2 = ds.GetChanges();
            

            int daupdate = da.Update(ds2);
            
            Assert.AreEqual(2, daupdate);
            
            

         
        }
        
        [Test]
        public void DataAdapterUpdateReturnValue2()
        {
            
            NpgsqlCommand cmd = TheConnection.CreateCommand();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tabled", TheConnection);
            NpgsqlCommandBuilder cb = new NpgsqlCommandBuilder(da);
            DataSet ds = new DataSet();
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
            int count = da.Update(ds);
            //## count is 1, even if the isn't updated in the database
            Assert.AreEqual(0, count);
        }

        [Test]
        public void FillWithEmptyResultset()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb where field_serial = -1", TheConnection);


            da.Fill(ds);

            Assert.AreEqual(1, ds.Tables.Count);
            Assert.AreEqual(4, ds.Tables[0].Columns.Count);
            Assert.AreEqual("field_serial", ds.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual("field_int2", ds.Tables[0].Columns[1].ColumnName);
            Assert.AreEqual("field_timestamp", ds.Tables[0].Columns[2].ColumnName);
            Assert.AreEqual("field_numeric", ds.Tables[0].Columns[3].ColumnName);
        }

        [Test]
        public void FillAddWithKey()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select field_serial, field_int2, field_timestamp, field_numeric from tableb", TheConnection);

            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds);

            DataColumn field_serial = ds.Tables[0].Columns[0];
            DataColumn field_int2 = ds.Tables[0].Columns[1];
            DataColumn field_timestamp = ds.Tables[0].Columns[2];
            DataColumn field_numeric = ds.Tables[0].Columns[3];

            Assert.IsFalse(field_serial.AllowDBNull);
            Assert.IsTrue(field_serial.AutoIncrement);
            Assert.AreEqual("field_serial", field_serial.ColumnName);
            Assert.AreEqual(typeof(int), field_serial.DataType);
            Assert.AreEqual(0, field_serial.Ordinal);
            // version 2 of the protocol doesn't know how to populate the unique field
            if (TheConnection.BackendProtocolVersion != ProtocolVersion.Version2)
            {
                Assert.IsTrue(field_serial.Unique);
            }

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
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select field_serial, field_int2, field_timestamp, field_numeric from tableb", TheConnection);

            da.MissingSchemaAction = MissingSchemaAction.Add;
            da.Fill(ds);

            DataColumn field_serial = ds.Tables[0].Columns[0];
            DataColumn field_int2 = ds.Tables[0].Columns[1];
            DataColumn field_timestamp = ds.Tables[0].Columns[2];
            DataColumn field_numeric = ds.Tables[0].Columns[3];

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
        public void UpdateLettingNullFieldValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (2)", TheConnection);
            command.ExecuteNonQuery();
            

            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb where field_serial = (select max(field_serial) from tableb)", TheConnection);
            da.InsertCommand = new NpgsqlCommand(";", TheConnection);
      			da.UpdateCommand = new NpgsqlCommand("update tableb set field_int2 = :a, field_timestamp = :b, field_numeric = :c where field_serial = :d", TheConnection);

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
            
            DataTable dt = ds.Tables[0];
            Assert.IsNotNull(dt);

            DataRow dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
            
            dr["field_int2"] = 4;
            
            DataSet ds2 = ds.GetChanges();

            da.Update(ds2);

            ds.Merge(ds2);
            ds.AcceptChanges();


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tableb where field_serial = (select max(field_serial) from tableb)", TheConnection).ExecuteReader();
            dr2.Read();
            
            Assert.AreEqual(4, dr2["field_int2"]);
            
            dr2.Close();
        }

        [Test]
        public void FillWithDuplicateColumnName()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select field_serial, field_serial from tableb", TheConnection);

            da.Fill(ds);
        }
        
        [Test]
        public void UpdateWithDataSet()
        {
            DoUpdateWithDataSet();
        }
        public virtual void DoUpdateWithDataSet()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (2)", TheConnection);
            command.ExecuteNonQuery();
            

            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb where field_serial = (select max(field_serial) from tableb)", TheConnection);
            
            NpgsqlCommandBuilder cb = new NpgsqlCommandBuilder(da);
            Assert.IsNotNull(cb);
            
            da.Fill(ds);
            
            DataTable dt = ds.Tables[0];
            Assert.IsNotNull(dt);

            DataRow dr = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1];
            
            dr["field_int2"] = 4;
            
            DataSet ds2 = ds.GetChanges();

            da.Update(ds2);

            ds.Merge(ds2);
            ds.AcceptChanges();


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tableb where field_serial = (select max(field_serial) from tableb)", TheConnection).ExecuteReader();
            dr2.Read();
            
            Assert.AreEqual(4, dr2["field_int2"]);
            
            dr2.Close();
        }
        
        [Test]
        public void InsertWithCommandBuilderCaseSensitive()
        {
            DoInsertWithCommandBuilderCaseSensitive();
        }
        public virtual void DoInsertWithCommandBuilderCaseSensitive()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablei", TheConnection);
            NpgsqlCommandBuilder builder = new NpgsqlCommandBuilder(da);
            Assert.IsNotNull(builder);

            da.Fill(ds);


            DataTable dt = ds.Tables[0];

            DataRow dr = dt.NewRow();
            dr["Field_Case_Sensitive"] = 4;
            
            dt.Rows.Add(dr);


            DataSet ds2 = ds.GetChanges();

            da.Update(ds2);

            ds.Merge(ds2);
            ds.AcceptChanges();


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tablei", TheConnection).ExecuteReader();
            dr2.Read();


            Assert.AreEqual(4, dr2[1]);
            dr2.Close();
        }

        [Test]
        public void IntervalAsTimeSpan()
        {
            DataTable dt = new DataTable();
            DataColumn c = dt.Columns.Add("dauer", typeof(TimeSpan));
            // DataColumn c = dt.Columns.Add("dauer", typeof(NpgsqlInterval));
            c.AllowDBNull = true;
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = TheConnection;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
            DataTable dt = new DataTable();
            //DataColumn c = dt.Columns.Add("dauer", typeof(TimeSpan));
            // DataColumn c = dt.Columns.Add("dauer", typeof(NpgsqlInterval));
            //c.AllowDBNull = true;
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = TheConnection;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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

            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = TheConnection;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            da.SelectCommand = command;
            System.Data.Common.DbDataAdapter common = da;
            Assert.IsNotNull(common.SelectCommand);
        }
    }
    [TestFixture]
    public class DataAdapterTestsV2 : DataAdapterTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _connV2; }
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _tV2; }
            set { _tV2 = value; }
        }
        public override void DoInsertWithCommandBuilderCaseSensitive()
        {
            //Not possible with V2?
        }
        public override void DoUpdateWithDataSet()
        {
            //Not possible with V2?
        }
    }
}
