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

namespace NpgsqlTests
{

    [TestFixture]
    public class DataReaderTests : BaseClassTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _conn;}
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _t; }
            set { _t = value; }
        }
        protected virtual string TheConnectionString {
            get { return _connString; }
        }

/*        [Test]
        public void TestNew()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 4;", TheConnection);

            command.Prepare();
            
            NpgsqlDataReaderNew dr = command.ExecuteReaderNew(CommandBehavior.Default);

            while(dr.Read());
        }*/
        [Test]
        public void RecordsAffecte()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_int4) values (7); insert into tablea(field_int4) values (8)", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();
            try
            {
                Assert.AreEqual(2, dr.RecordsAffected);
            }
            finally
            {
                dr.Close();
            }
        }

        [Test]
        public void GetBoolean()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 4;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Boolean result = dr.GetBoolean(4);
            Assert.AreEqual(true, result);
            dr.Close();
        }


        [Test]
        public void GetChars()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", TheConnection);

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
        [Ignore]
        public void GetBytesSequential()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1;", TheConnection);

            using (NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SequentialAccess))
            {

                dr.Read();
                Byte[] result = new Byte[2];


                Int64 a = dr.GetBytes(0, 0, result, 0, 2);
                Int64 b = dr.GetBytes(0, result.Length, result, 0, 2);

                Assert.AreEqual('S', (Char)result[0]);
                Assert.AreEqual('.', (Char)result[1]);
                Assert.AreEqual(2, a);
                Assert.AreEqual(0, b);

            }
        }
        
        

        [Test]
        public void GetInt32()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 2;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 1;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Int16 result = dr.GetInt16(1);

            Assert.AreEqual(2, result);
            
            dr.Close();
        }


        [Test]
        public void GetDecimal()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 3;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            Decimal result = dr.GetDecimal(3);


            Assert.AreEqual(4.2300000M, result);
            
            dr.Close();
        }


        [Test]
        public void GetDouble()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tabled where field_serial = 2;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tabled where field_serial = 1;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            //Single result = Single.Parse(dr.GetInt32(2).ToString());
            Single result = dr.GetFloat(1);

            Assert.AreEqual(.123456F, result);
            
            dr.Close();
        }

        [Test]
        public void GetMoney()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("select :param", TheConnection);
            decimal monAmount = 1234.5m;
            cmd.Parameters.Add("param", NpgsqlDbType.Money).Value = monAmount;
            using(IDataReader rdr = cmd.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(monAmount, rdr[0]);
            }
        }

        [Test]
        public void GetString()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();

            String result = dr.GetString(1);

            Assert.AreEqual("Random text", result);
            
            dr.Close();
        }


        [Test]
        public void GetStringWithParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = 1;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_text = :value;", TheConnection);

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
        public void GetInt32ArrayFieldType()
        {
            NpgsqlCommand command = new NpgsqlCommand("select cast(null as integer[])", TheConnection);
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                Assert.AreEqual(typeof(int[]), dr.GetFieldType(0));
            }
        }
        
        [Test]
        public void TestMultiDimensionalArray()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :i", TheConnection);
            command.Parameters.Add(":i", (new decimal[,]{{0,1,2},{3,4,5}}));
            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(2, (dr[0] as Array).Rank);
                decimal[,] da = dr[0] as decimal[,];
                Assert.AreEqual(da.GetUpperBound(0), 1);
                Assert.AreEqual(da.GetUpperBound(1), 2);
                decimal cmp = 0m;
                foreach(decimal el in da)
                    Assert.AreEqual(el, cmp++);
            }
        }
        
        [Test]
        public void TestArrayOfBytea()
        {
            NpgsqlCommand command = new NpgsqlCommand("select get_byte(:i[1], 2)", TheConnection);
            command.Parameters.Add(":i", new byte[][]{new byte[]{0,1,2}, new byte[]{3,4,5}});
            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr[0], 2);
            }
        }

        [Test]
        public void TestOverlappedParameterNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :test_name or field_serial = :test_name_long", TheConnection);
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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :test_name or field_serial = :test_name_long", TheConnection);
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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :a or field_serial = :aa", TheConnection);
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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();

            da.SelectCommand = command;

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapter.xml");
        }

        [Test]
        public void UseDataAdapterNpgsqlConnectionConstructor()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);

            command.Connection = TheConnection;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapterNpgsqlConnectionConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringNpgsqlConnectionConstructor()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablea", TheConnection);

            DataSet ds = new DataSet();

            da.Fill(ds);

            //ds.WriteXml("TestUseDataAdapterStringNpgsqlConnectionConstructor.xml");
        }


        [Test]
        public void UseDataAdapterStringStringConstructor()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tablea", TheConnectionString);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ds.WriteXml("TestUseDataAdapterStringStringConstructor.xml");
        }

        [Test]
        public void UseDataAdapterStringStringConstructor2()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from tableb", TheConnectionString);

            DataSet ds = new DataSet();

            da.Fill(ds);

            ds.WriteXml("TestUseDataAdapterStringStringConstructor2.xml");
        }

        [Test]
        public void DataGridWebControlSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();
            
            //Console.WriteLine(dr.FieldCount);
            

            DataGrid dg = new DataGrid();

            dg.DataSource = dr;
            dg.DataBind();
        }


        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadPastDataReaderEnd()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            while (dr.Read()) {}

            Object o = dr[0];
            Assert.IsNotNull(o);
        }

        [Test]
        public void IsDBNull()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_text from tablea;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select max(field_serial) from tablea;", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader();

            dr.Read();
            Assert.AreEqual(false, dr.IsDBNull(0));
            dr.Close();
        }



        [Test]
        public void TypesNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where 1 = 2;", TheConnection);

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
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(1, i);
        }

        [Test]
        public void SingleRowForwardOnlyCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);
            Int32 i = 0;

            using (NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SequentialAccess))
            {

                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        dr.GetValue(0);
                    if (!dr.IsDBNull(1))
                        dr.GetValue(1);
                    i++;
                }
            }

            Assert.AreEqual(1, i);
        }
        
        
        [Test]
        public void SingleRowCommandBehaviorSupportFunctioncall()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcb", TheConnection);
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
            
            NpgsqlCommand command = new NpgsqlCommand("funcb()", TheConnection);
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
            NpgsqlCommand command = new NpgsqlCommand("select * from metadatatest1", TheConnection);
            
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
            DoIsIdentityMetadataSupport();
        }
        public virtual void DoIsIdentityMetadataSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from metadatatest1", TheConnection);
            
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
            NpgsqlCommand command = new NpgsqlCommand("delete from tablea where field_serial = 2000000", TheConnection);
            
            NpgsqlDataReader dr = command.ExecuteReader();

                        
            Assert.IsFalse(dr.HasRows);
        }
   
        [Test]
        public void ParameterAppearMoreThanOneTime()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_serial = :parameter and field_int4 = :parameter", TheConnection);
            
            command.Parameters.Add("parameter", NpgsqlDbType.Integer);
            command.Parameters["parameter"].Value = 1;
            
            NpgsqlDataReader dr = command.ExecuteReader();
                        
            Assert.IsFalse(dr.HasRows);
            
            dr.Close();
        }
        
        [Test]
        public void SchemaOnlySingleRowCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.SingleRow);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        [Test]
        public void SchemaOnlyCommandBehaviorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        
        [Test]
        public void SchemaOnlyCommandBehaviorSupportFunctioncall()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcb", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            NpgsqlDataReader dr = command.ExecuteReader(CommandBehavior.SchemaOnly);

            
            Int32 i = 0;
            
            while (dr.Read())
                i++;
            
            Assert.AreEqual(0, i);
        }
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldNameDoesntExistOnGetOrdinal()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                int idx =  dr.GetOrdinal("field_int");
            }
        }
        
        [Test]
        public void FieldNameDoesntExistBackwardsCompat()
        {
            using(NpgsqlConnection backCompatCon = new NpgsqlConnection(TheConnectionString + ";Compatible=2.0.2"))
                using(NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", backCompatCon))
                {
                    backCompatCon.Open();
                    using(IDataReader rdr = command.ExecuteReader())
                        Assert.AreEqual(rdr.GetOrdinal("field_int"), -1);
                }
        }
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldNameDoesntExist()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Object a = dr["field_int"];
                Assert.IsNotNull(a);
            }
        }
        
        [Test]
        public void FieldNameKanaWidthWideRequestForNarrowFieldName()
        {//Should ignore Kana width and hence find the first of these two fields
            NpgsqlCommand command = new NpgsqlCommand("select 123 as ｦｧｨｩｪｫｬ, 124 as ヲァィゥェォャ", TheConnection);

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Assert.AreEqual(dr["ｦｧｨｩｪｫｬ"], 123);
                Assert.AreEqual(dr["ヲァィゥェォャ"], 123);// Wide version.
            }
        }
        
        [Test]
        public void FieldNameKanaWidthNarrowRequestForWideFieldName()
        {//Should ignore Kana width and hence find the first of these two fields
            NpgsqlCommand command = new NpgsqlCommand("select 123 as ヲァィゥェォャ, 124 as ｦｧｨｩｪｫｬ", TheConnection);

            using(NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Assert.AreEqual(dr["ヲァィゥェォャ"], 123);
                Assert.AreEqual(dr["ｦｧｨｩｪｫｬ"], 123);// Narrow version.
            }
        }
        
        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void FieldIndexDoesntExist()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            

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
            if (TheConnection.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", TheConnection);
                NpgsqlDataReader dr2 = cmd2.ExecuteReader();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteScalarBeforeClosingReader()
        {
            if (TheConnection.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", TheConnection);
                cmd2.ExecuteScalar();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExecuteNonQueryBeforeClosingReader()
        {
            if (TheConnection.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", TheConnection);
                cmd2.ExecuteNonQuery();
            }
        }
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PrepareBeforeClosingReader()
        {
            if (TheConnection.PreloadReader)//this behavior won't happen in this case so we fake it for the sake of the test.
                throw new InvalidOperationException();
            NpgsqlCommand cmd1 = new NpgsqlCommand("select field_serial from tablea", TheConnection);
            using(NpgsqlDataReader dr1 = cmd1.ExecuteReader())
            {
                NpgsqlCommand cmd2 = new NpgsqlCommand("select * from tablea", TheConnection);
                cmd2.Prepare();
            }
        }

        [Test]
        public void LoadDataTable()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableh", TheConnection);
            NpgsqlDataReader dr = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();

            Assert.AreEqual(5, dt.Columns[1].MaxLength);
            Assert.AreEqual(5, dt.Columns[2].MaxLength);
        }
        [Test]
        public void CleansupOkWithDisposeCalls()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", TheConnection);
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                dr.Close();
                using (NpgsqlCommand upd = TheConnection.CreateCommand())
                {
                    upd.CommandText = "select * from tablea";
                    upd.Prepare();
                }
                
           
            }
            
            
            
            
        }
        
        
        [Test]
        public void TestOutParameter2()
        {
            NpgsqlCommand command = new NpgsqlCommand("testoutparameter2", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Parameters.Add(new NpgsqlParameter("@x", NpgsqlDbType.Integer)).Value = 1;
            command.Parameters.Add(new NpgsqlParameter("@y", NpgsqlDbType.Integer)).Value = 2;
            command.Parameters.Add(new NpgsqlParameter("@sum", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("@product", NpgsqlDbType.Refcursor));
            
            command.Parameters["@sum"].Direction = ParameterDirection.Output;
            command.Parameters["@product"].Direction = ParameterDirection.Output;
            
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();
                
                Assert.AreEqual(3, command.Parameters["@sum"].Value);
                Assert.AreEqual(2, command.Parameters["@product"].Value);
                
                
           
            }
            
        }

        [Test]
        public void GetValueWithNullFields()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb", TheConnection);
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                dr.Read();

                Boolean result = dr.IsDBNull(2);

                Assert.IsTrue(result);



            }




        }

        
        [Test]
        public void HasRowsGetValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("select 1", TheConnection);
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.HasRows);
                Assert.IsTrue(dr.Read());
                Assert.AreEqual(1, dr.GetValue(0));
            }
        }
        
        
        [Test]
        public void IntervalAsTimeSpan()
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT CAST('1 hour' AS interval) AS dauer";
            command.Connection = TheConnection;
            
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.HasRows);
                Assert.IsTrue(dr.Read());
                TimeSpan ts = dr.GetTimeSpan(0);
            }
        }

        
    }
    [TestFixture]
    public class DataReaderTestsV2 : DataReaderTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _connV2; }
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _tV2; }
            set { _tV2 = value; }
        }
        protected override string TheConnectionString {
            get { return _connV2String; }
        }
        public override void DoIsIdentityMetadataSupport()
        {
            //Not possible with V2?
        }
    }
}
