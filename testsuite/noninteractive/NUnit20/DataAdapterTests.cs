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

        [Test]
        public void InsertWithDataSet()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", _conn);

            da.InsertCommand = new NpgsqlCommand("insert into tableb(field_int2, field_timestamp, field_numeric) values (:a, :b, :c)", _conn);

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


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tableb where field_serial > 4", _conn).ExecuteReader();
            dr2.Read();


            Assert.AreEqual(4, dr2[1]);
            Assert.AreEqual(7.3000000M, dr2[3]);
        }

        [Test]
        public void FillWithEmptyResultset()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb where field_serial = -1", _conn);


            da.Fill(ds);

            Assert.AreEqual(1, ds.Tables.Count);
            Assert.AreEqual(4, ds.Tables[0].Columns.Count);
            Assert.AreEqual("field_serial", ds.Tables[0].Columns[0].ColumnName);
            Assert.AreEqual("field_int2", ds.Tables[0].Columns[1].ColumnName);
            Assert.AreEqual("field_timestamp", ds.Tables[0].Columns[2].ColumnName);
            Assert.AreEqual("field_numeric", ds.Tables[0].Columns[3].ColumnName);
        }

        [Test]
        public void UpdateLettingNullFieldValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (2)", _conn);
            command.ExecuteNonQuery();
            

            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb where field_serial = (select max(field_serial) from tableb)", _conn);
            da.InsertCommand = new NpgsqlCommand(";", _conn);
      			da.UpdateCommand = new NpgsqlCommand("update tableb set field_int2 = :a, field_timestamp = :b, field_numeric = :c where field_serial = :d", _conn);

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


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tableb where field_serial = (select max(field_serial) from tableb)", _conn).ExecuteReader();
            dr2.Read();
            
            Assert.AreEqual(4, dr2["field_int2"]);
        }

        [Test]
        public void FillWithDuplicateColumnName()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select field_serial, field_serial from tableb", _conn);

            da.Fill(ds);
        }
        
        [Test]
        public void UpdateWithDataSet()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (2)", _conn);
            command.ExecuteNonQuery();
            

            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb where field_serial = (select max(field_serial) from tableb)", _conn);
            
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


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tableb where field_serial = (select max(field_serial) from tableb)", _conn).ExecuteReader();
            dr2.Read();
            
            Assert.AreEqual(4, dr2["field_int2"]);
        }
        
        [Test]
        public void InsertWithCommandBuilderCaseSensitive()
        {
            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablei", _conn);
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


            NpgsqlDataReader dr2 = new NpgsqlCommand("select * from tablei", _conn).ExecuteReader();
            dr2.Read();


            Assert.AreEqual(4, dr2[1]);
        }
    }
}
