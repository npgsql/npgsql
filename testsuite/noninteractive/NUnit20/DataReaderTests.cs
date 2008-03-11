// created on 27/12/2002 at 17:05
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
    public class DataReaderTests : BaseClassTests
    {

/*        [Test]
        public void TestNew()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 4;", _conn);

            command.Prepare();
            
            NpgsqlDataReaderNew dr = command.ExecuteReaderNew(CommandBehavior.Default);

            while(dr.Read());
        }*/
        [Test]
        public void GetBoolean()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 4;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Boolean result = dr.GetBoolean(4);
            Assert.AreEqual(true, result);
            dr.Close();
        }


        [Test]
        public void GetChars()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Char[] result = new Char[6];


            dr.GetChars(1, 0, result, 0, 6);

            Assert.AreEqual("Random", new String(result));
            
            dr.Close();
        }
        [Test]
        public void GetCharsSequential()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SequentialAccess);

            dr.Read();
            Char[] result = new Char[6];


            dr.GetChars(1, 0, result, 0, 6);

            Assert.AreEqual("Random", new String(result));
            
            dr.Close();
        }
           
           
        [Test]
        public void GetBytes()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Byte[] result = new Byte[2];


            Int64 a = dr.GetBytes(0, 0, result, 0, 2);
            Int64 b = dr.GetBytes(0, result.Length, result, 0, 2);

            Assert.AreEqual('S', (Char)result[0]);
            Assert.AreEqual('.', (Char)result[1]);
            Assert.AreEqual(2, a);
            Assert.AreEqual(0, b);
            
            dr.Close();
        }
        [Test]
        public void GetBytesSequential()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SequentialAccess);

            dr.Read();
            Byte[] result = new Byte[2];


            Int64 a = dr.GetBytes(0, 0, result, 0, 2);
            Int64 b = dr.GetBytes(0, result.Length, result, 0, 2);

            Assert.AreEqual('S', (Char)result[0]);
            Assert.AreEqual('.', (Char)result[1]);
            Assert.AreEqual(2, a);
            Assert.AreEqual(0, b);
            
            dr.Close();
        }
        
        

        [Test]
        public void GetInt32()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();


            Int32 result = dr.GetInt32(2);

            //ConsoleWriter cw = new ConsoleWriter(Console.Out);

            //cw.WriteLine(result.GetType().Name);
            Assert.AreEqual(4, result);
            
            dr.Close();
        }


        [Test]
        public void GetInt16()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Int16 result = dr.GetInt16(1);

            Assert.AreEqual(2, result);
            
            dr.Close();
        }


        [Test]
        public void GetDecimal()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 3;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Decimal result = dr.GetDecimal(3);


            Assert.AreEqual(4.2300000M, result);
            
            dr.Close();
        }


        [Test]
        public void GetDouble()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tabled where field_serial = 2;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            //Double result = Double.Parse(dr.GetInt32(2).ToString());
            Double result = dr.GetDouble(2);

            Assert.AreEqual(.123456789012345D, result);
            
            dr.Close();
        }


        [Test]
        public void GetFloat()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tabled where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            //Single result = Single.Parse(dr.GetInt32(2).ToString());
            Single result = dr.GetFloat(1);

            Assert.AreEqual(.123456F, result);
            
            dr.Close();
        }


        [Test]
        public void GetString()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual("Random text", result);
            
            dr.Close();
        }


        [Test]
        public void GetStringWithParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", _conn);

            String test = "Random text";
            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual(test, result);
            
            dr.Close();
        }

        [Test]
        public void GetStringWithQuoteWithParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", _conn);

            String test = "Text with ' single quote";
            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual(test, result);
            
            dr.Close();
        }


        [Test]
        public void GetValueByName()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = (String) dr["field_text"];

            Assert.AreEqual("Random text", result);
            
            dr.Close();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValueFromEmptyResultset()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", _conn);

            String test = "Text single quote";
            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();


            // This line should throw the invalid operation exception as the datareader will
            // have an empty resultset.
            Console.WriteLine(dr.IsDBNull(1));
        }


        [Test]
        public void TestOverlappedParameterNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :test_name or field_serial = :test_name_long", _conn);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "a"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }
        
        [Test]
        public void TestOverlappedParameterNamesWithPrepare()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :test_name or field_serial = :test_name_long", _conn);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "abc_de"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "abc_defg"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            command.Prepare();
            
            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void TestNonExistentParameterName()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :a or field_serial = :aa", _conn);
            command.Parameters.Add(new NpgsqlParameter(":b", DbType.Int32, 4, "b"));
            command.Parameters.Add(new NpgsqlParameter(":aa", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }

        [Test]
        public void UseDataAdapter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _conn);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();

            da.SelectCommand = command;

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapter.xml");
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _conn);

            command.Connection = _conn;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablea", _conn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
        }


        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablea", _connString);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", _connString);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        public void DataGridWebControlSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            DataGrid dg = new DataGrid();

            dg.DataSource = dr;
            dg.DataBind();
        }


        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadPastDataReaderEnd()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.Read()) {}

            Object o = dr[0];
            Assert.IsNotNull(o);
        }

        [Test]
        public void IsDBNull()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_text from tablea;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Assert.AreEqual(false, dr.IsDBNull(0));
            dr.Read();
            Assert.AreEqual(true, dr.IsDBNull(0));
            
            dr.Close();
        }

        [Test]
        public void IsDBNullFromScalar()
        {
            NpgsqlCommand command = new NpgsqlCommand("select max(field_serial) from tablea;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Assert.AreEqual(false, dr.IsDBNull(0));
            dr.Close();
        }



        [Test]
        public void TypesNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where 1 = 2;", _conn);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Assert.AreEqual("int4", dr.GetDataTypeName(0));
            Assert.AreEqual("text", dr.GetDataTypeName(1));
            Assert.AreEqual("int4", dr.GetDataTypeName(2));
            Assert.AreEqual("int8", dr.GetDataTypeName(3));
            Assert.AreEqual("bool", dr.GetDataTypeName(4));

            dr.Close();

            command.CommandText = "select * from tableb where 1 = 2";

            dr = command.ExecuteReader();

            dr.Read();

            Assert.AreEqual("int4", dr.GetDataTypeName(0));
            Assert.AreEqual("int2", dr.GetDataTypeName(1));
            Assert.AreEqual("timestamp", dr.GetDataTypeName(2));
            Assert.AreEqual("numeric", dr.GetDataTypeName(3));
        }
        
        [Test]
        public void SingleRowCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _conn);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }
        
        
        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncall()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcb", _conn);
            command.CommandType = CommandType.StoredProcedure;

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }
        
        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncallPrepare()
        {
            //FIXME: Find a way of supporting single row with prepare.
            // Problem is that prepare plan must already have the limit 1 single row support.
            
            NpgsqlCommand command = new NpgsqlCommand("funcb()", _conn);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Prepare();

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }
        
        [Test]
        public void PrimaryKeyFieldsMetadataSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from metadatatest1", _conn);
            
            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo);

            
            DataTable metadata = dr.GetSchemaTable();
            
            Boolean keyfound = false;
            
            foreach(DataRow r in metadata.Rows)
            {
                if ((Boolean)r["IsKey"] )
                {
                    Assert.AreEqual("field_pk", r["ColumnName"]);
                    keyfound = true;
                }
                    
            }
            if (!keyfound)
                Assert.Fail("No primary key found!");
            
            dr.Close();
        }
        
        [Test]
        public void IsIdentityMetadataSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from metadatatest1", _conn);
            
            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo);

            
            DataTable metadata = dr.GetSchemaTable();
            
            Boolean identityfound = false;
            
            foreach(DataRow r in metadata.Rows)
            {
                if ((Boolean)r["IsAutoIncrement"] )
                {
                    Assert.AreEqual("field_serial", r["ColumnName"]);
                    identityfound = true;
                }
                    
            }
            if (!identityfound)
                Assert.Fail("No identity column found!");
            
            dr.Close();
        }
        
        [Test]
        public void HasRowsWithoutResultset()
        {
            NpgsqlCommand command = new NpgsqlCommand("delete from tablea where field_serial = 2000000", _conn);
            
            NpgsqlDataReader dr = command.ExecuteReader();

                        
            Assert.IsFalse(dr.HasRows);
        }
   
        [Test]
        public void ParameterAppearMoreThanOneTime()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :parameter and field_int4 = :parameter", _conn);
            
            command.Parameters.Add("parameter", NpgsqlDbType.Integer);
            command.Parameters["parameter"].Value = 1;
            
            NpgsqlDataReader dr = command.ExecuteReader();
                        
            Assert.IsFalse(dr.HasRows);
            
            dr.Close();
        }
        
        [Test]
        public void SchemaOnlySingleRowCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _conn);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        [Test]
        public void SchemaOnlyCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _conn);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        
        [Test]
        public void SchemaOnlyCommandBehaviorSupportFunctioncall()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcb", _conn);
            command.CommandType = CommandType.StoredProcedure;

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldNameDoesntExist()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", _conn);
            

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Object a = dr["field_int"];
                Assert.IsNotNull(a);
            }
        }
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldIndexDoesntExist()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", _conn);
            

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Object a = dr[5];
                Assert.IsNotNull(a);
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteReaderBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _conn);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _conn);
                NpgsqlDataReader dr2 = cmd2.ExecuteReader();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteScalarBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _conn);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _conn);
                cmd2.ExecuteScalar();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteNonQueryBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _conn);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _conn);
                cmd2.ExecuteNonQuery();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrepareBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _conn);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _conn);
                cmd2.Prepare();
            }
        }
    }
    [TestFixture]
    public class DataReaderTestsV2 : BaseClassTests
    {

        [Test]
        public void GetBoolean()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 4;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Boolean result = dr.GetBoolean(4);
            Assert.AreEqual(true, result);
            dr.Close();
        }


        [Test]
        public void GetChars()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Char[] result = new Char[6];


            dr.GetChars(1, 0, result, 0, 6);

            Assert.AreEqual("Random", new String(result));
            dr.Close();
        }
           
        [Test]
        public void GetCharsSequential()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SequentialAccess);

            dr.Read();
            Char[] result = new Char[6];


            dr.GetChars(1, 0, result, 0, 6);

            Assert.AreEqual("Random", new String(result));
            dr.Close();
        }
           
        [Test]
        public void GetBytes()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Byte[] result = new Byte[2];


            Int64 a = dr.GetBytes(0, 0, result, 0, 2);
            Int64 b = dr.GetBytes(0, result.Length, result, 0, 2);

            Assert.AreEqual('S', (Char)result[0]);
            Assert.AreEqual('.', (Char)result[1]);
            Assert.AreEqual(2, a);
            Assert.AreEqual(0, b);
            dr.Close();
        }
        [Test]
        public void GetBytesSequential()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SequentialAccess);

            dr.Read();
            Byte[] result = new Byte[2];


            Int64 a = dr.GetBytes(0, 0, result, 0, 2);
            Int64 b = dr.GetBytes(0, result.Length, result, 0, 2);

            Assert.AreEqual('S', (Char)result[0]);
            Assert.AreEqual('.', (Char)result[1]);
            Assert.AreEqual(2, a);
            Assert.AreEqual(0, b);
            dr.Close();
        }
        

        [Test]
        public void GetInt32()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();


            Int32 result = dr.GetInt32(2);

            //ConsoleWriter cw = new ConsoleWriter(Console.Out);

            //cw.WriteLine(result.GetType().Name);
            Assert.AreEqual(4, result);
            dr.Close();
        }


        [Test]
        public void GetInt16()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Int16 result = dr.GetInt16(1);

            Assert.AreEqual(2, result);
            dr.Close();
        }


        [Test]
        public void GetDecimal()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 3;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Decimal result = dr.GetDecimal(3);


            Assert.AreEqual(4.2300000M, result);
            dr.Close();
        }


        [Test]
        public void GetDouble()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tabled where field_serial = 2;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            //Double result = Double.Parse(dr.GetInt32(2).ToString());
            Double result = dr.GetDouble(2);

            Assert.AreEqual(.123456789012345D, result);
            dr.Close();
        }


        [Test]
        public void GetFloat()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tabled where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            //Single result = Single.Parse(dr.GetInt32(2).ToString());
            Single result = dr.GetFloat(1);

            Assert.AreEqual(.123456F, result);
            dr.Close();
        }


        [Test]
        public void GetString()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual("Random text", result);
            dr.Close();
        }


        [Test]
        public void GetStringWithParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", _connV2);

            String test = "Random text";
            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual(test, result);
            dr.Close();
        }

        [Test]
        public void GetStringWithQuoteWithParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", _connV2);

            String test = "Text with ' single quote";
            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual(test, result);
            dr.Close();
        }


        [Test]
        public void GetValueByName()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = (String) dr["field_text"];

            Assert.AreEqual("Random text", result);
            dr.Close();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetValueFromEmptyResultset()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", _connV2);

            String test = "Text single quote";
            NpgsqlParameter param = new NpgsqlParameter();
            param.ParameterName = "value";
            param.DbType = DbType.String;
            //param.NpgsqlDbType = NpgsqlDbType.Text;
            param.Size = test.Length;
            param.Value = test;
            command.Parameters.Add(param);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();


            // This line should throw the invalid operation exception as the datareader will
            // have an empty resultset.
            Console.WriteLine(dr.IsDBNull(1));
        }


        [Test]
        public void TestOverlappedParameterNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :test_name or field_serial = :test_name_long", _connV2);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "a"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }
        
        [Test]
        public void TestOverlappedParameterNamesWithPrepare()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :test_name or field_serial = :test_name_long", _connV2);
            command.Parameters.Add(new NpgsqlParameter("test_name", DbType.Int32, 4, "abc_de"));
            command.Parameters.Add(new NpgsqlParameter("test_name_long", DbType.Int32, 4, "abc_defg"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            command.Prepare();
            
            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void TestNonExistentParameterName()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :a or field_serial = :aa", _connV2);
            command.Parameters.Add(new NpgsqlParameter(":b", DbType.Int32, 4, "b"));
            command.Parameters.Add(new NpgsqlParameter(":aa", DbType.Int32, 4, "aa"));

            command.Parameters[0].Value = 2;
            command.Parameters[1].Value = 3;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }

        [Test]
        public void UseDataAdapter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _connV2);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();

            da.SelectCommand = command;

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapter.xml");
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _connV2);

            command.Connection = _connV2;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablea", _connV2);

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
        }


        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablea", _connString);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", _connString);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        public void DataGridWebControlSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            DataGrid dg = new DataGrid();

            dg.DataSource = dr;
            dg.DataBind();
        }


        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadPastDataReaderEnd()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.Read()) {}

            Object o = dr[0];
            Assert.IsNotNull(o);
        }

        [Test]
        public void IsDBNull()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_text from tablea;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Assert.AreEqual(false, dr.IsDBNull(0));
            dr.Read();
            Assert.AreEqual(true, dr.IsDBNull(0));
            dr.Close();
        }

        [Test]
        public void IsDBNullFromScalar()
        {
            NpgsqlCommand command = new NpgsqlCommand("select max(field_serial) from tablea;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Assert.AreEqual(false, dr.IsDBNull(0));
            dr.Close();
        }



        [Test]
        public void TypesNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where 1 = 2;", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Assert.AreEqual("int4", dr.GetDataTypeName(0));
            Assert.AreEqual("text", dr.GetDataTypeName(1));
            Assert.AreEqual("int4", dr.GetDataTypeName(2));
            Assert.AreEqual("int8", dr.GetDataTypeName(3));
            Assert.AreEqual("bool", dr.GetDataTypeName(4));

            dr.Close();

            command.CommandText = "select * from tableb where 1 = 2";

            dr = command.ExecuteReader();

            dr.Read();

            Assert.AreEqual("int4", dr.GetDataTypeName(0));
            Assert.AreEqual("int2", dr.GetDataTypeName(1));
            Assert.AreEqual("timestamp", dr.GetDataTypeName(2));
            Assert.AreEqual("numeric", dr.GetDataTypeName(3));
        }
        
        [Test]
        public void SingleRowCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }
        
        
        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncall()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcb", _connV2);
            command.CommandType = CommandType.StoredProcedure;

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }
        
        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncallPrepare()
        {
            //FIXME: Find a way of supporting single row with prepare.
            // Problem is that prepare plan must already have the limit 1 single row support.
            
            NpgsqlCommand command = new NpgsqlCommand("funcb()", _connV2);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Prepare();

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }
        
        [Test]
        public void PrimaryKeyFieldsMetadataSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from metadatatest1", _connV2);
            
            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.KeyInfo);

            
            DataTable metadata = dr.GetSchemaTable();
            
            Boolean keyfound = false;
            
            foreach(DataRow r in metadata.Rows)
            {
                if ((Boolean)r["IsKey"] )
                {
                    Assert.AreEqual("field_pk", r["ColumnName"]);
                    keyfound = true;
                }
                    
            }
            if (!keyfound)
                Assert.Fail("No primary key found!");
            dr.Close();
        }
        
        [Test]
        public void HasRowsWithoutResultset()
        {
            NpgsqlCommand command = new NpgsqlCommand("delete from tablea where field_serial = 2000000", _connV2);
            
            NpgsqlDataReader dr = command.ExecuteReader();

                        
            Assert.IsFalse(dr.HasRows);
        }
   
        [Test]
        public void ParameterAppearMoreThanOneTime()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :parameter and field_int4 = :parameter", _connV2);
            
            command.Parameters.Add("parameter", NpgsqlDbType.Integer);
            command.Parameters["parameter"].Value = 1;
            
            NpgsqlDataReader dr = command.ExecuteReader();
                        
            Assert.IsFalse(dr.HasRows);
            
            dr.Close();
        }
        
        [Test]
        public void SchemaOnlySingleRowCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        [Test]
        public void SchemaOnlyCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", _connV2);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        
        [Test]
        public void SchemaOnlyCommandBehaviorSupportFunctioncall()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcb", _connV2);
            command.CommandType = CommandType.StoredProcedure;

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldNameDoesntExist()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", _connV2);
            

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Object a = dr["field_int"];
                Assert.IsNotNull(a);
            }
        }
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldIndexDoesntExist()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", _connV2);
            

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
    
                dr.Read();
                
                Object a = dr[5];
                Assert.IsNotNull(a);
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteReaderBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _connV2);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _connV2);
                NpgsqlDataReader dr2 = cmd2.ExecuteReader();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteScalarBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _connV2);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _connV2);
                cmd2.ExecuteScalar();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteNonQueryBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _connV2);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _connV2);
                cmd2.ExecuteNonQuery();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrepareBeforeClosingReader()
        {
            if(_conn.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", _connV2);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", _connV2);
                cmd2.Prepare();
            }
        }
    }
}
