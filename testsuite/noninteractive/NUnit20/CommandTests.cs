// CommandTests.cs created with MonoDevelop
// User: fxjr at 11:40 PM 4/9/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

// created on 30/11/2002 at 22:35
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

using Npgsql;
using NUnit.Framework;
using NUnit.Core;
using System.Data;
using System.Globalization;
using System.Net;
using NpgsqlTypes;
using System.Resources;
using System.Threading;

namespace NpgsqlTests
{

    public enum EnumTest : short
    {
        Value1 = 0,
        Value2 = 1
    };

    [TestFixture]
    public class CommandTests : BaseClassTests
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
        [Test]
        public void ParametersGetName()
        {
            NpgsqlCommand command = new NpgsqlCommand();

            // Add parameters.
            command.Parameters.Add(new NpgsqlParameter(":Parameter1", DbType.Boolean));
            command.Parameters.Add(new NpgsqlParameter(":Parameter2", DbType.Int32));
            command.Parameters.Add(new NpgsqlParameter(":Parameter3", DbType.DateTime));
            command.Parameters.Add(new NpgsqlParameter("Parameter4", DbType.DateTime));

            IDbDataParameter idbPrmtr = command.Parameters["Parameter1"];
            Assert.IsNotNull(idbPrmtr);
            command.Parameters[0].Value = 1;

            // Get by indexers.

            Assert.AreEqual(":Parameter1", command.Parameters[":Parameter1"].ParameterName);
            Assert.AreEqual(":Parameter2", command.Parameters[":Parameter2"].ParameterName);
            Assert.AreEqual(":Parameter3", command.Parameters[":Parameter3"].ParameterName);
            //Assert.AreEqual(":Parameter4", command.Parameters["Parameter4"].ParameterName); //Should this work?

            Assert.AreEqual(":Parameter1", command.Parameters[0].ParameterName);
            Assert.AreEqual(":Parameter2", command.Parameters[1].ParameterName);
            Assert.AreEqual(":Parameter3", command.Parameters[2].ParameterName);
            Assert.AreEqual("Parameter4", command.Parameters[3].ParameterName);
        }
        
        [Test]
        public void ParameterNameWithSpace()
        {
            NpgsqlCommand command = new NpgsqlCommand();

            // Add parameters.
            command.Parameters.Add(new NpgsqlParameter(":Parameter1 ", DbType.Boolean));
            
            Assert.AreEqual(":Parameter1", command.Parameters[0].ParameterName);
            
        }

        [Test]
        public void EmptyQuery()
        {
            
            NpgsqlCommand command = new NpgsqlCommand(";", TheConnection);
            command.ExecuteNonQuery();
        }
        
        
        [Test]
        public void NoNameParameterAdd()
        {
            NpgsqlCommand command = new NpgsqlCommand();

            command.Parameters.Add(new NpgsqlParameter());
            command.Parameters.Add(new NpgsqlParameter());
            
            
            Assert.AreEqual(":Parameter1", command.Parameters[0].ParameterName);
            Assert.AreEqual(":Parameter2", command.Parameters[1].ParameterName);
        }
        

        [Test]
        public void FunctionCallFromSelect()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from funcB()", TheConnection);

            NpgsqlDataReader reader = command.ExecuteReader();

            Assert.IsNotNull(reader);
            reader.Close();
            //reader.FieldCount
        }

        [Test]
        public void ExecuteScalar()
        {
            NpgsqlCommand command = new NpgsqlCommand("select count(*) from tablea", TheConnection);

            Object result = command.ExecuteScalar();

            Assert.AreEqual(6, result);
            //reader.FieldCount
        }
        
        [Test]
        public void TransactionSetOk()
        {
            NpgsqlCommand command = new NpgsqlCommand("select count(*) from tablea", TheConnection);
            
            command.Transaction = _t;
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(6, result);
        }
        
        
        [Test]
        public void InsertStringWithBackslashes()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", TheConnection);
            
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            
            command.Parameters["p0"].Value = @"\test";

            Object result = command.ExecuteNonQuery();

            Assert.AreEqual(1, result);
            
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            

            result = command2.ExecuteScalar();
            
            Assert.AreEqual(@"\test", result);
            
            
            
            //reader.FieldCount
        }
               
        
        [Test]
        public void UseStringParameterWithNoNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", TheConnection);
            
            command.Parameters.Add(new NpgsqlParameter("p0", "test"));
            
            
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Text);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.String);
            
            Object result = command.ExecuteNonQuery();

            Assert.AreEqual(1, result);
            
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            

            result = command2.ExecuteScalar();
            
            
            
            Assert.AreEqual("test", result);
            
            
            
            //reader.FieldCount
        }
        
        
        [Test]
        public void UseIntegerParameterWithNoNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0)", TheConnection);
            
            command.Parameters.Add(new NpgsqlParameter("p0", 5));
            
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
            
            
            Object result = command.ExecuteNonQuery();

            Assert.AreEqual(1, result);
            
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            

            result = command2.ExecuteScalar();
            
            
            Assert.AreEqual(5, result);
            
            
           //reader.FieldCount
        }
        
        
        //[Test]
        public void UseSmallintParameterWithNoNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0)", TheConnection);
            
            command.Parameters.Add(new NpgsqlParameter("p0", (Int16)5));
            
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Smallint);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.Int16);
            
            
            Object result = command.ExecuteNonQuery();

            Assert.AreEqual(1, result);
            
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);

            result = command2.ExecuteScalar();
            
            
            Assert.AreEqual(5, result);
            
            
            //reader.FieldCount
        }
        
        
        [Test]
        public void FunctionCallReturnSingleValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC();", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            Object result = command.ExecuteScalar();

            Assert.AreEqual(6, result);
            //reader.FieldCount
        }
        
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RollbackWithNoTransaction()
        {
            
            TheTransaction.Rollback();
            TheTransaction.Rollback();
        }


        [Test]
        public void FunctionCallReturnSingleValueWithPrepare()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Prepare();
            Object result = command.ExecuteScalar();

            Assert.AreEqual(6, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));

            command.Parameters[0].Value = 4;

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValueNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));

            command.Parameters[0].Value = 4;

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }


        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;


            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));

            Assert.AreEqual(1, command.Parameters.Count);
            command.Prepare();


            command.Parameters[0].Value = 4;

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;


            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));

            Assert.AreEqual(1, command.Parameters.Count);
            command.Prepare();


            command.Parameters[0].Value = 4;

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }


        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC(@a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;


            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));

            Assert.AreEqual(1, command.Parameters.Count);
            //command.Prepare();


            command.Parameters[0].Value = 4;

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }


        [Test]
        public void FunctionCallReturnResultSet()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcB()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }


        [Test]
        public void CursorStatement()
        {
            Int32 i = 0;

            
            NpgsqlCommand command = new NpgsqlCommand("declare te cursor for select * from tablea;", TheConnection);

            command.ExecuteNonQuery();

            command.CommandText = "fetch forward 3 in te;";

            NpgsqlDataReader dr = command.ExecuteReader();


            while (dr.Read())
            {
                i++;
            }

            Assert.AreEqual(3, i);


            i = 0;

            command.CommandText = "fetch backward 1 in te;";

            NpgsqlDataReader dr2 = command.ExecuteReader();

            while (dr2.Read())
            {
                i++;
            }

            Assert.AreEqual(1, i);

            command.CommandText = "close te;";

            command.ExecuteNonQuery();
        }

        [Test]
        public void PreparedStatementNoParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea;", TheConnection);

            command.Prepare();
            
            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            
            dr.Close();
        }
        
        
        [Test]
        public void PreparedStatementInsert()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:p0);", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            command.Parameters["p0"].Value = "test";
            
            command.Prepare();
            
            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }
        
        [Test]
        public void RTFStatementInsert()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:p0);", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            command.Parameters["p0"].Value = @"{\rtf1\ansi\ansicpg1252\uc1 \deff0\deflang1033\deflangfe1033{";
                       
            
            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            
            
            String result = (String)new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea);", TheConnection).ExecuteScalar();
            
            Assert.AreEqual(@"{\rtf1\ansi\ansicpg1252\uc1 \deff0\deflang1033\deflangfe1033{", result);
        }
        
        
        
        [Test]
        public void PreparedStatementInsertNullValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0);", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Integer));
            command.Parameters["p0"].Value = DBNull.Value;

            command.Prepare();

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }       

        [Test]
        public void PreparedStatementWithParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_int4 = :a and field_int8 = :b;", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters.Add(new NpgsqlParameter("b", DbType.Int64));

            Assert.AreEqual(2, command.Parameters.Count);

            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            command.Prepare();

            command.Parameters[0].Value = 3;
            command.Parameters[1].Value = 5;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            
            dr.Close();
        }

        [Test]
        public void PreparedStatementWithParametersNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tablea where field_int4 = :a and field_int8 = :b;", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("b", NpgsqlDbType.Bigint));

            Assert.AreEqual(2, command.Parameters.Count);

            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            command.Prepare();

            command.Parameters[0].Value = 3;
            command.Parameters[1].Value = 5;

            NpgsqlDataReader dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }
        
        [Test]
        public void FunctionCallWithImplicitParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;


            NpgsqlParameter p = new NpgsqlParameter("@a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            
            command.Parameters.Add(p);
            

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }
        
        
        [Test]
        public void PreparedFunctionCallWithImplicitParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;


            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            
            command.Parameters.Add(p);
            
            command.Prepare();

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }
        
        
        [Test]
        public void FunctionCallWithImplicitParametersWithNoParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            Object result = command.ExecuteScalar();

            Assert.AreEqual(6, result);
            //reader.FieldCount

        }
        
        [Test]
        public void FunctionCallOutputParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            NpgsqlParameter p = new NpgsqlParameter("a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
                        
            command.ExecuteNonQuery();
            
            Assert.AreEqual(6, command.Parameters["a"].Value);
        }
        
        [Test]
        public void FunctionCallOutputParameter2()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            NpgsqlParameter p = new NpgsqlParameter("@a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
                        
            command.ExecuteNonQuery();
            
            Assert.AreEqual(6, command.Parameters["@a"].Value);
        }
        
        [Test]
        public void OutputParameterWithoutName()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            NpgsqlParameter p = command.CreateParameter();
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
                        
            command.ExecuteNonQuery();
            
            Assert.AreEqual(6, command.Parameters[0].Value);
        }
        
        [Test]
        public void FunctionReturnVoid()
        {
            NpgsqlCommand command = new NpgsqlCommand("testreturnvoid()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }
        
        [Test]
        public void StatementOutputParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("select 4, 5;", TheConnection);
                        
            NpgsqlParameter p = new NpgsqlParameter("a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
            p = new NpgsqlParameter("b", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
            
            p = new NpgsqlParameter("c", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
                        
            command.ExecuteNonQuery();
            
            Assert.AreEqual(4, command.Parameters["a"].Value);
            Assert.AreEqual(5, command.Parameters["b"].Value);
            Assert.AreEqual(-1, command.Parameters["c"].Value);
        }
        
        [Test]
        public void StringEscapeSyntax()
        {
            try//the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=off;set escape_string_warning=off", TheConnection).ExecuteNonQuery();
            }
            catch{}
            string cmdTxt = "select :par";
            NpgsqlCommand command = new NpgsqlCommand(cmdTxt, TheConnection);
            NpgsqlCommand arrCommand = new NpgsqlCommand(cmdTxt, TheConnection);
            string testStrPar = "This string has a 'literal' backslash \\";
            string[,] testArrPar = new string[,]{{testStrPar, ""}, {testStrPar, testStrPar}};
            command.Parameters.Add(":par", testStrPar);
            using(IDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(rdr.GetString(0), testStrPar);
            }
            arrCommand.Parameters.Add(":par", testArrPar);
            using(IDataReader rdr = arrCommand.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(((string[,])rdr.GetValue(0))[0,0], testStrPar);
            }
            
            try//the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=on;set escape_string_warning=on", TheConnection).ExecuteNonQuery();
            }
            catch{}
            using(IDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(rdr.GetString(0), testStrPar);
            }
            using(IDataReader rdr = arrCommand.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(((string[,])rdr.GetValue(0))[0,0], testStrPar);
            }
        }
        
        [Test]
        public void ParameterAndOperatorUnclear()
        {
            //Without parenthesis the meaning of [, . and potentially other characters is
            //a syntax error. See comment in NpgsqlCommand.GetClearCommandText() on "usually-redundant parenthesis".
            NpgsqlCommand command = new NpgsqlCommand("select :arr[2]", TheConnection);
            command.Parameters.Add(":arr", new int[]{5,4,3,2,1});
            using(IDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(rdr.GetInt32(0), 4);
            }
        }
        
        [Test]
        public void FunctionCallInputOutputParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;


            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            
            command.Parameters.Add(p);
            

            Int64 result = (Int64) command.ExecuteScalar();

            Assert.AreEqual(1, result);
        }
        
        
        [Test]
        public void StatementMappedOutputParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("select 3, 4 as param1, 5 as param2, 6;", TheConnection);
                        
            NpgsqlParameter p = new NpgsqlParameter("param2", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
            p = new NpgsqlParameter("param1", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
            p = new NpgsqlParameter("p", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            
            command.Parameters.Add(p);
            
            
            command.ExecuteNonQuery();
            
            Assert.AreEqual(4, command.Parameters["param1"].Value);
            Assert.AreEqual(5, command.Parameters["param2"].Value);
            //Assert.AreEqual(-1, command.Parameters["p"].Value); //Which is better, not filling this or filling this with an unmapped value?
        }


        [Test]
        public void ListenNotifySupport()
        {
            // Notify messages are only sent from server after a transaction is finished.
            // So, finish now the implicit transaction.
            
            TheTransaction.Rollback();
            
            Assert.IsFalse(RecievedNotification);//Test we start correctly.

            NpgsqlCommand command = new NpgsqlCommand("listen notifytest;", TheConnection);
            command.ExecuteNonQuery();

            TheConnection.Notification += new NotificationEventHandler(NotificationSupportHelper);


            command = new NpgsqlCommand("notify notifytest;", TheConnection);
            command.ExecuteNonQuery();

            Assert.IsTrue(RecievedNotification);
            
        }

        public bool RecievedNotification = false;
        private void NotificationSupportHelper(Object sender, NpgsqlNotificationEventArgs args)
        {
            RecievedNotification = true;
        }

        
            [Test]
        public void ByteSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Byte));

            command.Parameters[0].Value = 2;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.Parameters.Clear();
        }
        
        
            [Test]
        public void ByteaSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1", TheConnection);


            Byte[] result = (Byte[]) command.ExecuteScalar();
            

            Assert.AreEqual(2, result.Length);
        }
        
        [Test]
        public void ByteaInsertSupport()
        {
            Byte[] toStore = { 1 };

                  NpgsqlCommand cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
              cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
                  cmd.Parameters[0].Value = toStore;
                  cmd.ExecuteNonQuery();

                  cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);
            
                  Byte[] result = (Byte[])cmd.ExecuteScalar();
            
            Assert.AreEqual(1, result.Length);

        }
        
        [Test]
        public void ByteaInsertWithPrepareSupport()
        {
            


            Byte[] toStore = { 1 };

            NpgsqlCommand cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);
            
            cmd.Prepare();
            Byte[] result = (Byte[])cmd.ExecuteScalar();
            
            
            Assert.AreEqual(toStore, result);
        }
        
        
        
        [Test]
        public void ByteaParameterSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_bytea = :bytesData", TheConnection);
            
            Byte[] bytes = new Byte[]{45,44};
            
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
                  command.Parameters[":bytesData"].Value = bytes;


            Object result = command.ExecuteNonQuery();
            

            Assert.AreEqual(-1, result);
        }
        
        [Test]
        public void ByteaParameterWithPrepareSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_bytea = :bytesData", TheConnection);
            
            Byte[] bytes = new Byte[]{45,44};
            
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
                  command.Parameters[":bytesData"].Value = bytes;


            command.Prepare();
            Object result = command.ExecuteNonQuery();
            

            Assert.AreEqual(-1, result);
        }
        
        
            [Test]
        public void EnumSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Smallint));

            command.Parameters[0].Value = EnumTest.Value1;
            

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);
        }

        [Test]
        public void DateTimeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

            DateTimeFormatInfo culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            DateTime dt = System.DateTime.Parse("2004-06-04 09:48:00", culture);

            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
            command.Parameters[0].Value = dt;

            command.ExecuteScalar();
       }


        [Test]
        public void DateTimeSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

            DateTimeFormatInfo culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            DateTime dt = System.DateTime.Parse("2004-06-04 09:48:00", culture);

            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = dt;

            command.ExecuteScalar();
        }

        [Test]
        public void DateSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_date from tablec where field_serial = 1;", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("2002-03-04", d.ToString("yyyy-MM-dd"));
        }

        [Test]
        public void TimeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_time from tablec where field_serial = 2;", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("10:03:45.345", d.ToString("HH:mm:ss.fff"));
        }

        [Test]
        public void TimeSupportTimezone()
        {
            NpgsqlCommand command = new NpgsqlCommand("select '13:03:45.001-05'::timetz", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("18:03:45.001", d.ToUniversalTime().ToString("HH:mm:ss.fff"));
        }
        
        [Test]
        public void DateTimeSupportTimezone()
        {
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select field_timestamp_with_timezone from tableg where field_serial = 1;", TheConnection);
            
            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezone2()
        {
            //Changed the comparison. Did thisassume too much about ToString()?
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5; select field_timestamp_with_timezone from tableg where field_serial = 1;", TheConnection);

            String s = ((DateTime)command.ExecuteScalar()).ToUniversalTime().ToString();
           
            Assert.AreEqual(new DateTime(2002,02,02,09,00,23).ToString() , s);
        }

        [Test]
        public void DateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select timestamptz'2009-11-11 20:15:43.019-03:30';", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();


            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2009-11-11 23:45:43Z", d.ToUniversalTime().ToString("u"));

        }
        
        [Test]
        public void DateTimeSupportTimezoneEuropeAmsterdam()
        {
            //1929-08-19 00:00:00+01:19:32
            // This test was provided by Christ Akkermans.
            
            NpgsqlCommand command = new NpgsqlCommand("SET TIME ZONE 'Europe/Amsterdam';SELECT '1929-08-19 00:00:00'::timestamptz;", TheConnection);

            DateTime d = (DateTime)command.ExecuteScalar();
            
           

        }



        [Test]
        public void ProviderDateTimeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);

            NpgsqlTimeStamp ts;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStamp(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.ToString());

            DateTimeFormatInfo culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            NpgsqlTimeStamp ts1 = NpgsqlTimeStamp.Parse("2004-06-04 09:48:00");

            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }


        [Test]
        public void ProviderDateTimeSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);

            NpgsqlTimeStamp ts;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStamp(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.ToString());

            DateTimeFormatInfo culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            NpgsqlTimeStamp ts1 = NpgsqlTimeStamp.Parse("2004-06-04 09:48:00");

            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_date from tablec where field_serial = 1;", TheConnection);

            NpgsqlDate d;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                d = reader.GetDate(0);
            }

            Assert.AreEqual("2002-03-04", d.ToString());
        }

        [Test]
        public void ProviderTimeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_time from tablec where field_serial = 2;", TheConnection);

            NpgsqlTime t;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                t = reader.GetTime(0);
            }


            Assert.AreEqual("10:03:45.345", t.ToString());
        }

        [Test]
        public void ProviderTimeSupportTimezone()
        {
            NpgsqlCommand command = new NpgsqlCommand("select '13:03:45.001-05'::timetz", TheConnection);

            NpgsqlTimeTZ t;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                t = reader.GetTimeTZ(0);
            }

            Assert.AreEqual("18:03:45.001", t.AtTimeZone(NpgsqlTimeZone.UTC).LocalTime.ToString());
        }

        [Test]
        public void ProviderDateTimeSupportTimezone()
        {
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select field_timestamp_with_timezone from tableg where field_serial = 1;", TheConnection);

            NpgsqlTimeStampTZ ts;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStampTZ(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.AtTimeZone(NpgsqlTimeZone.UTC).ToString());
        }

        [Test]
        public void ProviderDateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select timestamptz'2009-11-11 20:15:43.019-03:30';", TheConnection);

            NpgsqlTimeStampTZ ts;
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStampTZ(0);
            }

            Assert.AreEqual("2009-11-12 04:45:43.019+05", ts.ToString());

        }

        [Test]
        public void NumericSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_numeric) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Decimal));

            command.Parameters[0].Value = 7.4M;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tableb where field_numeric = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Decimal result = dr.GetDecimal(3);


            Assert.AreEqual(7.4000000M, result);
            dr.Close();
        }

        [Test]
        public void NumericSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_numeric) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Numeric));

            command.Parameters[0].Value = 7.4M;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tableb where field_numeric = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Decimal result = dr.GetDecimal(3);


            Assert.AreEqual(7.4000000M, result);
            
            dr.Close();
        }


        [Test]
        public void InsertSingleValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tabled(field_float4) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", DbType.Single));

            command.Parameters[0].Value = 7.4F;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tabled where field_float4 = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Single result = dr.GetFloat(1);


            Assert.AreEqual(7.4F, result);
            
            dr.Close();
        }


        [Test]
        public void InsertSingleValueNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tabled(field_float4) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Real));

            command.Parameters[0].Value = 7.4F;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tabled where field_float4 = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Single result = dr.GetFloat(1);


            Assert.AreEqual(7.4F, result);
            
            dr.Close();
        }
        
        
        [Test]
        public void DoubleValueSupportWithExtendedQuery()
        {
            NpgsqlCommand command = new NpgsqlCommand("select count(*) from tabled where field_float8 = :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Double));

            command.Parameters[0].Value = 0.123456789012345D;
            
            command.Prepare();

            Object rows = command.ExecuteScalar();

            
            Assert.AreEqual(1, rows);
        }

        [Test]
        public void InsertDoubleValue()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tabled(field_float8) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", DbType.Double));

            command.Parameters[0].Value = 7.4D;

            Int32 rowsAdded = command.ExecuteNonQuery();

            command.CommandText = "select * from tabled where field_float8 = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Double result = dr.GetDouble(2);


            Assert.AreEqual(1, rowsAdded);
            Assert.AreEqual(7.4D, result);
            
            dr.Close();
        }


        [Test]
        public void InsertDoubleValueNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tabled(field_float8) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Double));

            command.Parameters[0].Value = 7.4D;

            Int32 rowsAdded = command.ExecuteNonQuery();

            command.CommandText = "select * from tabled where field_float8 = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Double result = dr.GetDouble(2);


            Assert.AreEqual(1, rowsAdded);
            Assert.AreEqual(7.4D, result);
            
            dr.Close();
        }


        [Test]
        public void NegativeNumericSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 4", TheConnection);


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Decimal result = dr.GetDecimal(3);

            Assert.AreEqual(-4.3000000M, result);
            
            dr.Close();
        }


        [Test]
        public void PrecisionScaleNumericSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select * from tableb where field_serial = 4", TheConnection);


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Decimal result = dr.GetDecimal(3);

            Assert.AreEqual(-4.3000000M, result);
            //Assert.AreEqual(11, result.Precision);
            //Assert.AreEqual(7, result.Scale);
            
            dr.Close();
        }

        [Test]
        public void InsertNullString()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.String));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_text is null";
            command.Parameters.Clear();

            Int64 result = (Int64)command.ExecuteScalar();

            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullStringNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Text));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_text is null";
            command.Parameters.Clear();

            Int64 result = (Int64)command.ExecuteScalar();

            Assert.AreEqual(4, result);
        }



        [Test]
        public void InsertNullDateTime()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_timestamp is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar();

            Assert.AreEqual(4, result);
        }


        [Test]
        public void InsertNullDateTimeNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_timestamp is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar();

            Assert.AreEqual(4, result);
        }



        [Test]
        public void InsertNullInt16()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_int2 is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(4, result);
        }


        [Test]
        public void InsertNullInt16NpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Smallint));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_int2 is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(4, result);
        }


        [Test]
        public void InsertNullInt32()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_int4) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_int4 is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(6, result);
        }


        [Test]
        public void InsertNullNumeric()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_numeric) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Decimal));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_numeric is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(3, result);
        }

        [Test]
        public void InsertNullBoolean()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_bool) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Boolean));

            command.Parameters[0].Value = DBNull.Value;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_bool is null";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(6, result);

        }

        [Test]
        public void InsertBoolean()
        {
            


            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_bool) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Boolean));

            command.Parameters[0].Value = false;

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select field_bool from tablea where field_serial = (select max(field_serial) from tablea)";
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(false, result);

        }

        [Test]
        public void AnsiStringSupport()
        {
         
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.AnsiString));

            command.Parameters[0].Value = "TesteAnsiString";

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = String.Format("select count(*) from tablea where field_text = '{0}'", command.Parameters[0].Value);
            command.Parameters.Clear();

            Object result = command.ExecuteScalar(); // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64

            Assert.AreEqual(1, result);
        }


        [Test]
        public void MultipleQueriesFirstResultsetEmpty()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values ('a'); select count(*) from tablea;", TheConnection);

            Object result = command.ExecuteScalar();

            Assert.AreEqual(7, result);
        }

        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void ConnectionStringWithInvalidParameterValue()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=npgsql_tets;Password=j");

            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", conn);

            command.Connection.Open();
            command.ExecuteReader();
            command.Connection.Close();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidConnectionString()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1;User Id=npgsql_tests;Pooling:false");

            NpgsqlCommand command = new NpgsqlCommand("select * from tablea", conn);

            command.Connection.Open();
            command.ExecuteReader();
            command.Connection.Close();
        }


        [Test]
        public void AmbiguousFunctionParameterType()
        {
            NpgsqlConnection conn = new NpgsqlConnection(TheConnectionString);


            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlParameter p = new NpgsqlParameter("a", DbType.Int16);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("b", DbType.Int32);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("c", DbType.Int64);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("d", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("e", NpgsqlDbType.Char);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("f", NpgsqlDbType.Varchar);
            p.Value = "a";
            command.Parameters.Add(p);


            command.Connection.Open();
            command.ExecuteScalar();
            command.Connection.Close();
        }
        
        [Test]
        public void AmbiguousFunctionParameterTypePrepared()
        {
            NpgsqlConnection conn = new NpgsqlConnection(TheConnectionString);


            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlParameter p = new NpgsqlParameter("a", DbType.Int16);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("b", DbType.Int32);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("c", DbType.Int64);
            p.Value = 2;
            command.Parameters.Add(p);
            p = new NpgsqlParameter("d", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("e", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);
            p = new NpgsqlParameter("f", DbType.String);
            p.Value = "a";
            command.Parameters.Add(p);


            command.Connection.Open();
            command.Prepare();
            command.ExecuteScalar();
            command.Connection.Close();
        }


        
        // The following two methods don't need checks because what is being tested is the 
        // execution of parameter replacing which happens on ExecuteNonQuery method. So, if these
        // methods don't throw exception, they are ok.
        [Test]
        public void TestParameterReplace()
        {
            String sql = @"select * from tablea where
                            field_serial = :a
                         ";


            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));

            command.Parameters[0].Value = 2;

            command.ExecuteNonQuery();
        }
        
        [Test]
        public void TestParameterReplace2()
        {
            String sql = @"select * from tablea where
                         field_serial = :a+1
                         ";


            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));

            command.Parameters[0].Value = 1;

            command.ExecuteNonQuery();
        }
        
        [Test]
        public void TestParameterNameInParameterValue()
        {
            String sql = "insert into tablea(field_text, field_int4) values ( :a, :b );" ;

            String aValue = "test :b";

            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);

            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Text));

            command.Parameters[":a"].Value = aValue;
            
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));

            command.Parameters[":b"].Value = 1;

            Int32 rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);
            
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_text, field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            
            NpgsqlDataReader dr = command2.ExecuteReader();
            
            dr.Read();
            
            String a = dr.GetString(0);;
            Int32 b = dr.GetInt32(1);
            
            dr.Close();
            
            
            
            Assert.AreEqual(aValue, a);
            Assert.AreEqual(1, b);
        }

        [Test]
        public void TestBoolParameter1()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            NpgsqlCommand command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", TheConnection);
            NpgsqlParameter p0 = new NpgsqlParameter(":a", true);
            // with setting pramater type
            p0.DbType = DbType.Boolean;
            command.Parameters.Add(p0);

            command.ExecuteScalar();
        }

        [Test]
        public void TestBoolParameter2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            NpgsqlCommand command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", TheConnection);
            NpgsqlParameter p0 = new NpgsqlParameter(":a", true);
            // not setting parameter type
            command.Parameters.Add(p0);

            command.ExecuteScalar();
        }

        [Test]
        public void TestPointSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_point from tablee where field_serial = 1", TheConnection);

            NpgsqlPoint p = (NpgsqlPoint) command.ExecuteScalar();

            Assert.AreEqual(4, p.X);
            Assert.AreEqual(3, p.Y);
        }


        [Test]
        public void TestBoxSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_box from tablee where field_serial = 2", TheConnection);

            NpgsqlBox box = (NpgsqlBox) command.ExecuteScalar();

            Assert.AreEqual(5, box.UpperRight.X);
            Assert.AreEqual(4, box.UpperRight.Y);
            Assert.AreEqual(4, box.LowerLeft.X);
            Assert.AreEqual(3, box.LowerLeft.Y);
        }

        [Test]
        public void TestLSegSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_lseg from tablee where field_serial = 3", TheConnection);

            NpgsqlLSeg lseg = (NpgsqlLSeg) command.ExecuteScalar();

            Assert.AreEqual(4, lseg.Start.X);
            Assert.AreEqual(3, lseg.Start.Y);
            Assert.AreEqual(5, lseg.End.X);
            Assert.AreEqual(4, lseg.End.Y);
        }

        [Test]
        public void TestClosedPathSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_path from tablee where field_serial = 4", TheConnection);

            NpgsqlPath path = (NpgsqlPath) command.ExecuteScalar();

            Assert.AreEqual(false, path.Open);
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(4, path[0].X);
            Assert.AreEqual(3, path[0].Y);
            Assert.AreEqual(5, path[1].X);
            Assert.AreEqual(4, path[1].Y);
        }

        [Test]
        public void TestOpenPathSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_path from tablee where field_serial = 5", TheConnection);

            NpgsqlPath path = (NpgsqlPath) command.ExecuteScalar();

            Assert.AreEqual(true, path.Open);
            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(4, path[0].X);
            Assert.AreEqual(3, path[0].Y);
            Assert.AreEqual(5, path[1].X);
            Assert.AreEqual(4, path[1].Y);
        }



        [Test]
        public void TestPolygonSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_polygon from tablee where field_serial = 6", TheConnection);

            NpgsqlPolygon polygon = (NpgsqlPolygon) command.ExecuteScalar();

            Assert.AreEqual(2, polygon.Count);
            Assert.AreEqual(4, polygon[0].X);
            Assert.AreEqual(3, polygon[0].Y);
            Assert.AreEqual(5, polygon[1].X);
            Assert.AreEqual(4, polygon[1].Y);
        }


        [Test]
        public void TestCircleSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_circle from tablee where field_serial = 7", TheConnection);

            NpgsqlCircle circle = (NpgsqlCircle) command.ExecuteScalar();

            Assert.AreEqual(4, circle.Center.X);
            Assert.AreEqual(3, circle.Center.Y);
            Assert.AreEqual(5, circle.Radius);
        }
        
        [Test]
        public void SetParameterValueNull()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
                  NpgsqlParameter param = cmd.CreateParameter();
                  param.ParameterName="val";
            param.NpgsqlDbType = NpgsqlDbType.Bytea;
                  param.Value = DBNull.Value;
            
                  cmd.Parameters.Add(param);
            
                  cmd.ExecuteNonQuery();

                  cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);
            
                  Object result = cmd.ExecuteScalar();
            
            
            Assert.AreEqual(DBNull.Value, result);
        }
        
        
        [Test]
        public void TestCharParameterLength()
        {
            String sql = "insert into tableh(field_char5) values ( :a );" ;
    
            String aValue = "atest";
    
            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);
    
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Char));
    
            command.Parameters[":a"].Value = aValue;
            command.Parameters[":a"].Size = 5;
            
            Int32 rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_char5 from tableh where field_serial = (select max(field_serial) from tableh)", TheConnection);
            
            NpgsqlDataReader dr = command2.ExecuteReader();
            
            dr.Read();
            
            String a = dr.GetString(0);;
                        
            dr.Close();
            
            
            Assert.AreEqual(aValue, a);
        }
        
        [Test]
        public void ParameterHandlingOnQueryWithParameterPrefix()
        {
            NpgsqlCommand command = new NpgsqlCommand("select to_char(field_time, 'HH24:MI') from tablec where field_serial = :a;", TheConnection);
            
            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Value = 2;
            
            command.Parameters.Add(p);

            String d = (String)command.ExecuteScalar();


            Assert.AreEqual("10:03", d);
        }
        
        [Test]
        public void MultipleRefCursorSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("testmultcurfunc", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            NpgsqlDataReader dr = command.ExecuteReader();
            
            dr.Read();
            
            Int32 one = dr.GetInt32(0);
            
            dr.NextResult();
            
            dr.Read();
            
            Int32 two = dr.GetInt32(0);
            
            dr.Close();
            
            
            Assert.AreEqual(1, one);
            Assert.AreEqual(2, two);
        }
        
        [Test]
        public void ProcedureNameWithSchemaSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("public.testreturnrecord", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Direction = ParameterDirection.Output;

            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Direction = ParameterDirection.Output;

            command.ExecuteNonQuery();
            
            Assert.AreEqual(4, command.Parameters[0].Value);
            Assert.AreEqual(5, command.Parameters[1].Value);
        }
        
        [Test]
        public void ReturnRecordSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("testreturnrecord", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Direction = ParameterDirection.Output;

            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Direction = ParameterDirection.Output;

            command.ExecuteNonQuery();
            
            Assert.AreEqual(4, command.Parameters[0].Value);
            Assert.AreEqual(5, command.Parameters[1].Value);
        }
        
        
        
        [Test]
        public void ParameterTypeBoolean()
        {
            NpgsqlParameter p = new NpgsqlParameter();
            
            p.ParameterName = "test";
            
            p.Value = true;
            
            Assert.AreEqual(NpgsqlDbType.Boolean, p.NpgsqlDbType);
        }

        
        [Test]
        public void ParameterTypeTimestamp()
        {
            NpgsqlParameter p = new NpgsqlParameter();
            
            p.ParameterName = "test";
            
            p.Value = DateTime.Now;
            
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType);
        }
        
        
        [Test]
        public void ParameterTypeText()
        {
            NpgsqlParameter p = new NpgsqlParameter();
            
            p.ParameterName = "test";
            
            p.Value = "teste";
            
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType);
        }
        
        [Test]
        public void ProblemSqlInsideException()
        {
            String sql = "selec 1 as test";
            try
            {
                NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);
                
                command.ExecuteReader();
            }
            catch (NpgsqlException ex)
            {
                Assert.AreEqual(sql, ex.ErrorSql);
            }
        }

        [Test]
        public void ReadUncommitedTransactionSupport()
        {
            String sql = "select 1 as test";
            
            NpgsqlConnection c = new NpgsqlConnection(TheConnectionString);
            
            c.Open();
            
            NpgsqlTransaction t = c.BeginTransaction(IsolationLevel.ReadUncommitted);
            Assert.IsNotNull(t);
            
            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);
                
            command.ExecuteReader().Close();
            
        }
        
        [Test]
        public void RepeatableReadTransactionSupport()
        {
            String sql = "select 1 as test";
            
            NpgsqlConnection c = new NpgsqlConnection(TheConnectionString);
            
            c.Open();
            
            NpgsqlTransaction t = c.BeginTransaction(IsolationLevel.RepeatableRead);
            Assert.IsNotNull(t);
            
            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);
                
            command.ExecuteReader().Close();
            
            c.Close();
            
        }
        
        [Test]
        public void SetTransactionToSerializable()
        {
            String sql = "show transaction_isolation;";
            
            NpgsqlConnection c = new NpgsqlConnection(TheConnectionString);
            
            c.Open();
            
            NpgsqlTransaction t = c.BeginTransaction(IsolationLevel.Serializable);
            Assert.IsNotNull(t);
            
            NpgsqlCommand command = new NpgsqlCommand(sql, c);
            
            String isolation = (String)command.ExecuteScalar();
            
            c.Close();
                
            Assert.AreEqual("serializable", isolation);
        }
        
        
        [Test]
        public void TestParameterNameWithDot()
        {
            String sql = "insert into tableh(field_char5) values ( :a.parameter );" ;
    
            String aValue = "atest";
    
            NpgsqlCommand command = new NpgsqlCommand(sql, TheConnection);
    
            command.Parameters.Add(new NpgsqlParameter(":a.parameter", NpgsqlDbType.Char));
    
            command.Parameters[":a.parameter"].Value = aValue;
            command.Parameters[":a.parameter"].Size = 5;
            
            
            Int32 rowsAdded = -1;
            try
            {
                rowsAdded = command.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.ErrorSql);
            }

            Assert.AreEqual(rowsAdded, 1);
            
            NpgsqlCommand command2 = new NpgsqlCommand("select field_char5 from tableh where field_serial = (select max(field_serial) from tableh)", TheConnection);
            
            String a = (String)command2.ExecuteScalar();
            
            Assert.AreEqual(aValue, a);
        }


        [Test]
        public void LastInsertedOidSupport()
        {

            NpgsqlCommand insertCommand = new NpgsqlCommand("insert into tablea(field_text) values ('a');", TheConnection);
            // Insert this dummy row, just to enable us to see what was the last oid in order we can assert it later.
            insertCommand.ExecuteNonQuery();

            NpgsqlCommand selectCommand = new NpgsqlCommand("select max(oid) from tablea;", TheConnection);     


            Int64 previousOid = (Int64) selectCommand.ExecuteScalar();

            insertCommand.ExecuteNonQuery();

            Assert.AreEqual(previousOid + 1, insertCommand.LastInsertedOID);

            
        }
        
        /*[Test]
        public void SetServerVersionToNull()
        {

            ServerVersion o = TheConnection.ServerVersion;
            
            if(o == null)
              return;
        }*/
        
        [Test]
        public void VerifyFunctionNameWithDeriveParameters()
        {
            try
            {
                NpgsqlCommand invalidCommandName = new NpgsqlCommand("invalidfunctionname", TheConnection);
                
                NpgsqlCommandBuilder.DeriveParameters(invalidCommandName);
            }
            catch (InvalidOperationException e)
            {
                ResourceManager resman = new ResourceManager(typeof(NpgsqlCommandBuilder));
                string expected = string.Format(resman.GetString("Exception_InvalidFunctionName"), "invalidfunctionname");
                Assert.AreEqual(expected, e.Message);
            }
        }
        
        
        [Test]
        public void DoubleSingleQuotesValueSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Text));

            command.Parameters[0].Value = "''";

            Int32 rowsAdded = command.ExecuteNonQuery();

            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tablea where field_text = :a";


            NpgsqlDataReader dr = command.ExecuteReader();
            
            Assert.IsTrue(dr.Read());
            
            dr.Close();
        }
        
        [Test]
        public void ReturnInfinityDateTimeSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values ('infinity'::timestamp);", TheConnection);
            

            command.ExecuteNonQuery();
            
            
            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        [Test]
        public void ReturnMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values ('-infinity'::timestamp);", TheConnection);
            

            command.ExecuteNonQuery();
            
            
            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void InsertInfinityDateTimeSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Timestamp);

            p.Value = DateTime.MaxValue;
            
            command.Parameters.Add(p);

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        [Test]
        public void InsertMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Timestamp);

            p.Value = DateTime.MinValue;
            
            command.Parameters.Add(p);

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(DateTime.MinValue, result);
        }
        
        [Test]
        public void InsertMinusInfinityDateTimeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", DateTime.MinValue);

            command.Parameters.Add(p);

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void MinusInfinityDateTimeSupport()
        {
            NpgsqlCommand command = TheConnection.CreateCommand();
                       
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MinValue));

            command.CommandText = "select 1 where current_date=:p0";
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(null, result);
        }
        
        
        [Test]
        public void PlusInfinityDateTimeSupport()
        {
            NpgsqlCommand command = TheConnection.CreateCommand();
                       
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MaxValue));

            command.CommandText = "select 1 where current_date=:p0";
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(null, result);
        }


        [Test]
        public void InetTypeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablej(field_inet) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Inet);

            p.Value = new NpgsqlInet("127.0.0.1");
            
            command.Parameters.Add(p);

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_inet from tablej where field_serial = (select max(field_serial) from tablej);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(new NpgsqlInet("127.0.0.1"), result);
        }

        [Test]
        public void IPAddressTypeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablej(field_inet) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Inet);

            p.Value = IPAddress.Parse("127.0.0.1");
            
            command.Parameters.Add(p);

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_inet from tablej where field_serial = (select max(field_serial) from tablej);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(new NpgsqlInet("127.0.0.1"), result);
        }

        [Test]
        public void BitTypeSupportWithPrepare()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Bit);

            p.Value = true;
            
            command.Parameters.Add(p);

            command.Prepare();

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BitTypeSupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Bit);

            p.Value = true;
            
            command.Parameters.Add(p);


            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);

            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BitTypeSupport2()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Bit);

            p.Value = 3;
            
            command.Parameters.Add(p);


            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(true, result);
        }


        [Test]
        public void BitTypeSupport3()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Bit);

            p.Value = 6;
            
            command.Parameters.Add(p);


            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(false, result);
        }

        //[Test]
        public void FunctionReceiveCharParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("d/;", TheConnection);
            

            NpgsqlParameter p = new NpgsqlParameter("a", NpgsqlDbType.Inet);

            p.Value = IPAddress.Parse("127.0.0.1");
            
            command.Parameters.Add(p);

            command.ExecuteNonQuery();
            
            command = new NpgsqlCommand("select field_inet from tablej where field_serial = (select max(field_serial) from tablej);", TheConnection);
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(new NpgsqlInet("127.0.0.1"), result);
        }

        [Test]
        public void FunctionCaseSensitiveName()
        {
            NpgsqlCommand command = new NpgsqlCommand("\"FunctionCaseSensitive\"", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));

            Object result = command.ExecuteScalar();

            Assert.AreEqual(0, result);
            
        }

        [Test]
        public void FunctionCaseSensitiveNameWithSchema()
        {
            NpgsqlCommand command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(0, result);
            
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParameters()
        {
            NpgsqlCommand command = new NpgsqlCommand("\"FunctionCaseSensitive\"", TheConnection);

            

            NpgsqlCommandBuilder.DeriveParameters(command);
            
            
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
            
        }
        
        [Test]
        public void FunctionCaseSensitiveNameDeriveParametersWithSchema()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", TheConnection);
            
            NpgsqlCommandBuilder.DeriveParameters(command);
            
            
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);

            
            
        }
        
        [Test]
        public void CaseSensitiveParameterNames()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :p1", TheConnection);
            
            command.Parameters.Add(new NpgsqlParameter("P1", NpgsqlDbType.Integer)).Value = 5;

            
            Object result = command.ExecuteScalar();
            
            Assert.AreEqual(5, result);
            
        }


        [Test]
        public void FunctionTestTimestamptzParameterSupport()
        {
            
            NpgsqlCommand command = new NpgsqlCommand("testtimestamptzparameter", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimestampTZ));
            
            NpgsqlDataReader dr = command.ExecuteReader();

            Int32 count = 0;
            
            while (dr.Read())
                count++;

            Assert.IsTrue(count > 1);
            
            
            
            
            
            
        }
        
        [Test]
        public void GreaterThanInQueryStringWithPrepare()
        {
            NpgsqlCommand command = new NpgsqlCommand("select count(*) from tablea where field_serial >:param1", TheConnection);
            
            command.Parameters.Add(":param1", 1);
            

            command.Prepare();
            command.ExecuteScalar();
            
            
        }
        
        [Test]
        public void CharParameterValueSupport()
        {
            const String query = @"create temp table test ( tc char(1) );
            insert into test values(' ');
            select * from test where tc=:charparam";

            NpgsqlCommand command = new NpgsqlCommand(query, TheConnection);

            IDbDataParameter sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "charparam";
            
            // Exception Can't cast System.Char into any valid DbType.
            sqlParam.Value = ' ';
            command.Parameters.Add(sqlParam);

            String res = (String)command.ExecuteScalar();
            
            Assert.AreEqual(" ", res);                    
            
            
        }
        [Test]
        public void ConnectionStringCommandTimeout()
        {
           /* NpgsqlConnection connection = new NpgsqlConnection("Server=localhost; Database=test; User=postgres; Password=12345;
CommandTimeout=180");
NpgsqlCommand command = new NpgsqlCommand("\"Foo\"", connection);
connection.Open();*/

        NpgsqlConnection conn = new NpgsqlConnection(TheConnectionString + ";CommandTimeout=180");
        NpgsqlCommand command = new NpgsqlCommand("\"Foo\"", conn);
        conn.Open();
        
        Assert.AreEqual(180, command.CommandTimeout);
            

            
            
        }
        
         [Test]
        public void ParameterExplicitType()
        {
            
            object param = 1;
            
            using(NpgsqlCommand cmd = new NpgsqlCommand("select a, max(b) from (select :param as a, 1 as b) x group by a", TheConnection))
            {
                cmd.Parameters.Add("param", param);
                cmd.Parameters[0].DbType = DbType.Int32;
                
                using(IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    rdr.Read();
                }

                param = "text";
                cmd.Parameters[0].DbType = DbType.String;
                using(IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    rdr.Read();
                }
            
            }
        }
        

        [Test]
        public void ParameterExplicitType2()
        {

            const string query = @"create temp table test ( tc date );  select * from test where tc=:param";
        
            NpgsqlCommand command = new NpgsqlCommand(query, TheConnection);

           IDbDataParameter sqlParam = command.CreateParameter();
           sqlParam.ParameterName = "param";
           sqlParam.Value = "2008-1-1";
           //sqlParam.DbType = DbType.Object;
           command.Parameters.Add(sqlParam);
           
          
           command.ExecuteScalar();
        }
        
        [Test]
        public void ParameterExplicitType2DbTypeObject()
        {

            const string query = @"create temp table test ( tc date );  select * from test where tc=:param";
        
            NpgsqlCommand command = new NpgsqlCommand(query, TheConnection);

           IDbDataParameter sqlParam = command.CreateParameter();
           sqlParam.ParameterName = "param";
           sqlParam.Value = "2008-1-1";
           sqlParam.DbType = DbType.Object;
           command.Parameters.Add(sqlParam);
           
           
           command.ExecuteScalar();
        }
        
        [Test]
        public void ParameterExplicitType2DbTypeObjectWithPrepare()
        {

            new NpgsqlCommand("create temp table test ( tc date )", TheConnection).ExecuteNonQuery();
        
            const string query = @"select * from test where tc=:param";
        
            NpgsqlCommand command = new NpgsqlCommand(query, TheConnection);

            IDbDataParameter sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "param";
            sqlParam.Value = "2008-1-1";
            sqlParam.DbType = DbType.Object;
            command.Parameters.Add(sqlParam);
           
            command.Prepare();
           
            command.ExecuteScalar();
        }
        
        [Test]
        public void ParameterExplicitType2DbTypeObjectWithPrepare2()
        {

            new NpgsqlCommand("create temp table test ( tc date )", TheConnection).ExecuteNonQuery();
            
            const string query = @"select * from test where tc=:param or tc=:param2";
        
            NpgsqlCommand command = new NpgsqlCommand(query, TheConnection);

            IDbDataParameter sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "param";
            sqlParam.Value = "2008-1-1";
            sqlParam.DbType = DbType.Object;
            command.Parameters.Add(sqlParam);
            
            sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "param2";
            sqlParam.Value = DateTime.Now;
            sqlParam.DbType = DbType.Date;
            command.Parameters.Add(sqlParam);
            
            command.Prepare();
            
            command.ExecuteScalar();
        }

        [Test]
        public void Int32WithoutQuotesPolygon()
        {

            NpgsqlCommand a = new NpgsqlCommand("select 'polygon ((:a :b))' ", TheConnection);
            a.Parameters.Add(new NpgsqlParameter("a", 1));
            a.Parameters.Add(new NpgsqlParameter("b", 1));
            
            a.ExecuteScalar();
                      
                 
        }
        
        [Test]
        public void Int32WithoutQuotesPolygon2()
        {

            NpgsqlCommand a = new NpgsqlCommand("select 'polygon ((:a :b))' ", TheConnection);
            a.Parameters.Add(new NpgsqlParameter("a", 1)).DbType = DbType.Int32;
            a.Parameters.Add(new NpgsqlParameter("b", 1)).DbType = DbType.Int32;
            
            a.ExecuteScalar();
                      
                 
        }
        
        [Test]
        public void TestUUIDDataType()
        {

            string createTable =
            @"DROP TABLE if exists public.person;
            CREATE TABLE public.person ( 
            person_id serial PRIMARY KEY NOT NULL,
            person_uuid uuid NOT NULL
            ) WITH(OIDS=FALSE);";
            NpgsqlCommand command = new NpgsqlCommand(createTable, TheConnection);
            command.ExecuteNonQuery();

            string insertSql = "INSERT INTO person (person_uuid) VALUES (:param1);";
            NpgsqlParameter uuidDbParam = new NpgsqlParameter(":param1", NpgsqlDbType.Uuid);
            uuidDbParam.Value = Guid.NewGuid();

            command = new NpgsqlCommand(insertSql, TheConnection);
            command.Parameters.Add(uuidDbParam);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("SELECT person_uuid::uuid FROM person LIMIT 1", TheConnection);


            object result = command.ExecuteScalar();
            Assert.AreEqual(typeof(Guid), result.GetType());
        }
        
        [Test]
        public void TestBug1006158OutputParameters()
        {

            string createFunction =
            @"CREATE OR REPLACE FUNCTION more_params(OUT a integer, OUT b boolean) AS
            $BODY$DECLARE
                BEGIN
                    a := 3;
                    b := true;
                END;$BODY$
              LANGUAGE 'plpgsql' VOLATILE;";
              
            NpgsqlCommand command = new NpgsqlCommand(createFunction, TheConnection);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("more_params", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters.Add(new NpgsqlParameter("b", DbType.Boolean));
            command.Parameters[1].Direction = ParameterDirection.Output;

            Object result = command.ExecuteScalar();

            Assert.AreEqual(3, command.Parameters[0].Value);
            Assert.AreEqual(true, command.Parameters[1].Value);
        }
        
        
        [Test]
        public void TestSavePoint()
        {
            
            if (TheConnection.PostgreSqlVersion < new Version("8.0.0"))
                return;
                
            const String theSavePoint = "theSavePoint";
            
            TheTransaction.Save(theSavePoint);
            
            new NpgsqlCommand("insert into tablea (field_text) values ('savepointtest')", TheConnection).ExecuteNonQuery();
            
            object result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection).ExecuteScalar();
            
            Assert.AreEqual(1, result);
            
            TheTransaction.Rollback(theSavePoint);
            
            result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection).ExecuteScalar();
            
            Assert.AreEqual(0, result);
            
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSavePointWithSemicolon()
        {
            if (TheConnection.PostgreSqlVersion < new Version("8.0.0"))
                // Fake exception just to make test pass;
                throw new InvalidOperationException();
            
            const String theSavePoint = "theSavePoint;";
            
            TheTransaction.Save(theSavePoint);
            
            new NpgsqlCommand("insert into tablea (field_text) values ('savepointtest')", TheConnection).ExecuteNonQuery();
            
            object result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection).ExecuteScalar();
            
            Assert.AreEqual(1, result);
            
            TheTransaction.Rollback(theSavePoint);
            
            result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection).ExecuteScalar();
            
            Assert.AreEqual(0, result);
            
        }
        
        [Test]
        public void TestPreparedStatementParameterCastIsNotAdded()
        {
            // Test by Waldemar Bergstreiser            

            new NpgsqlCommand("create table testpreparedstatementparametercast ( C1 int );", TheConnection).ExecuteNonQuery();
            IDbCommand cmd = new NpgsqlCommand("select C1 from testpreparedstatementparametercast where :p0 is null or  C1 = :p0 ", TheConnection);
            
            IDbDataParameter paramP0 = cmd.CreateParameter();
            paramP0.ParameterName = "p0";
            paramP0.DbType = DbType.Int32;
        cmd.Parameters.Add(paramP0);
            cmd.Prepare();    // This cause a runtime exception // Tested with PostgreSQL 8.3 //
             
           
            
        }
        
        [Test]
        [ExpectedException(typeof(NpgsqlException))]
        public void TestErrorInPreparedStatementCausesReleaseConnectionToThrowException()
        {
            // This is caused by having an error with the prepared statement and later, Npgsql is trying to release the plan as it was successful created.             

            IDbCommand cmd = new NpgsqlCommand("sele", TheConnection);
            
                cmd.Prepare();    
             
            
        
        }
        
        [Test]
        public void TestBug1010488ArrayParameterWithNullValue()
        {
            // Test by Christ Akkermans       
            
            new NpgsqlCommand(@"CREATE OR REPLACE FUNCTION NullTest (input INT4[]) RETURNS VOID                             
            AS $$
            DECLARE
            BEGIN
            END
            $$ LANGUAGE plpgsql;", TheConnection).ExecuteNonQuery();
            
            using (NpgsqlCommand cmd = new NpgsqlCommand("NullTest", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("", NpgsqlDbType.Integer | NpgsqlDbType.Array);
                parameter.Value = new object[] { 5, 5, DBNull.Value };
                cmd.Parameters.Add(parameter);
 
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void VarCharArrayHandling()
        {
            
            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p1", NpgsqlDbType.Varchar | NpgsqlDbType.Array);
                parameter.Value = new object[] { "test", "test"};
                cmd.Parameters.Add(parameter);
 
                cmd.ExecuteNonQuery();
            }
            
            
        }
        
        [Test]
        public void Bug1010521NpgsqlIntervalShouldBeQuoted()
        {
            
            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p1", NpgsqlDbType.Interval);
                parameter.Value = new NpgsqlInterval(DateTime.Now.TimeOfDay);
                cmd.Parameters.Add(parameter);
 
                cmd.ExecuteNonQuery();
            }
            
            
        }

        [Test]
        public void Bug1010543Int32MinValueThrowException()
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
                parameter.Value = Int32.MinValue;
                cmd.Parameters.Add(parameter);

                cmd.ExecuteNonQuery();
            }


        }

        [Test]
        public void Bug1010543Int16MinValueThrowException()
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p1", DbType.Int16);
                parameter.Value = Int16.MinValue;
                cmd.Parameters.Add(parameter);

                cmd.ExecuteNonQuery();
            }



        }
        [Test]
        public void Bug1010543Int16MinValueThrowExceptionPrepared()
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p1", DbType.Int16);
                parameter.Value = Int16.MinValue;
                cmd.Parameters.Add(parameter);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }



        }

        [Test]
        public void HandleInt16MinValueParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = Int16.MinValue;

            Object result = command.ExecuteScalar();
            Assert.AreEqual(Int16.MinValue, result);
        }

        [Test]
        public void HandleInt32MinValueParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = Int32.MinValue;

            Object result = command.ExecuteScalar();
            Assert.AreEqual(Int32.MinValue, result);
        }

        [Test]
        public void HandleInt64MinValueParameter()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int64));
            command.Parameters[0].Value = Int64.MinValue;
            
            Object result = command.ExecuteScalar();
            Assert.AreEqual(Int64.MinValue, result);
        }

        [Test]
        public void HandleInt16MinValueParameterPrepared()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = Int16.MinValue;
            command.Prepare();

            Object result = command.ExecuteScalar();
            Assert.AreEqual(Int16.MinValue, result);
        }

        [Test]
        public void HandleInt32MinValueParameterPrepared()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = Int32.MinValue;
            command.Prepare();

            Object result = command.ExecuteScalar();
            Assert.AreEqual(Int32.MinValue, result);
        }

        [Test]
        public void HandleInt64MinValueParameterPrepared()
        {
            NpgsqlCommand command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int64));
            command.Parameters[0].Value = Int64.MinValue;
            command.Prepare();

            Object result = command.ExecuteScalar();
            Assert.AreEqual(Int64.MinValue, result);
        }


        [Test]
        public void Bug1010557BackslashGetDoubled()
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p1", NpgsqlDbType.Text);
                parameter.Value = "test\\str";
                cmd.Parameters.Add(parameter);

                object result = cmd.ExecuteScalar();
                Assert.AreEqual("test\\str", result); 
            }


        }

        [Test]
        public void NumberConversionWithCulture()
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p1", TheConnection))
            {

                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");

                NpgsqlParameter parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double);
                parameter.Value = 5.5;
                cmd.Parameters.Add(parameter);

                object result = cmd.ExecuteScalar();

                Thread.CurrentThread.CurrentCulture = new CultureInfo("");
                Assert.AreEqual(5.5, result);

            }


        }
        
        [Test]
        public void TestNullParameterValueInStatement()
        {
            // Test by Andrus Moor
            IDbCommand cmd = TheConnection.CreateCommand();
            int? i = null;
            cmd.Parameters.Add(new NpgsqlParameter("p0", i));
            cmd.CommandText = "select :p0 is null or :p0=0 ";
            
            cmd.ExecuteScalar();
        }
        
        [Test]
        public void PreparedStatementWithParametersWithSize()
        {

            using (NpgsqlCommand cmd = new NpgsqlCommand("select :p0, :p1;", TheConnection))
            {

                NpgsqlParameter parameter = new NpgsqlParameter("p0", NpgsqlDbType.Varchar);
                parameter.Value = "test";
                parameter.Size = 10;
                cmd.Parameters.Add(parameter);
                
                parameter = new NpgsqlParameter("p1", NpgsqlDbType.Varchar);
                parameter.Value = "test";
                parameter.Size = 10;
                cmd.Parameters.Add(parameter);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }



        }

        [Test]
        public void CommandTimeoutReset()
        {

            NpgsqlCommand cmd = new NpgsqlCommand();

            cmd.CommandTimeout = Int32.MaxValue;

            cmd.Connection = TheConnection;

            Assert.AreEqual(Int32.MaxValue, cmd.CommandTimeout);



        }
        
        [Test]
        public void SelectInfinityValueDateDataType()
        {
            IDbCommand cmd = TheConnection.CreateCommand();
            cmd.CommandText = "create temp table test (dt date); insert into test values ('-infinity'::date);select * from test";
            var dr = cmd.ExecuteReader();
            dr.Read();
           // InvalidCastException was unhandled
          // at Npgsql.ForwardsOnlyDataReader.GetValue(Int32 Index)
          //  at Npgsql.NpgsqlDataReader.GetDateTime(Int32 i) ..

            dr.GetDateTime(0);

            dr.Close();

        }



        [Test]

        public void DeriveParametersWithParameterNameFromFunction()

        {

            NpgsqlCommand command = new NpgsqlCommand("testoutparameter2", TheConnection);

            command.CommandType = CommandType.StoredProcedure;



            NpgsqlCommandBuilder.DeriveParameters(command);



            Assert.AreEqual(":x", command.Parameters[0].ParameterName);

            Assert.AreEqual(":y", command.Parameters[1].ParameterName);

            



        }
        
        [Test]
        public void NegativeMoneySupport()
        {
            NpgsqlCommand command = new NpgsqlCommand("select '-10.5'::money", TheConnection);


            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Decimal result = dr.GetDecimal(0);

            Assert.AreEqual(-10.5, result);
            
            dr.Close();
        }
       
        [Test]
        public void Bug1010714AndPatch1010715()
        {
            NpgsqlCommand command = new NpgsqlCommand("select field_bytea from tablef where field_bytea = :bytesData", TheConnection);
            
            Byte[] bytes = new Byte[]{45,44};
            
            command.Parameters.AddWithValue(":bytesData", bytes);
            
            Assert.AreEqual(DbType.Binary, command.Parameters[0].DbType);
            
            Object result = command.ExecuteNonQuery();
            

            Assert.AreEqual(-1, result);
        }
        
        [Test]
        public void TestNpgsqlSpecificTypesCLRTypesNpgsqlInet()
        {
            // Please, check http://pgfoundry.org/forum/message.php?msg_id=1005483
            // for a discussion where an NpgsqlInet type isn't shown in a datagrid
            // This test tries to check if the type returned is an IPAddress when using
            // the GetValue() of NpgsqlDataReader and NpgsqlInet when using GetProviderValue();
            
            NpgsqlCommand command = new NpgsqlCommand("select '192.168.10.10'::inet;", TheConnection);
            
            
            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Object result = dr.GetValue(0);
            
            dr.Close();
            

            Assert.AreEqual(typeof(IPAddress), result.GetType());
            
            
        }
        
        [Test]
        public void TestNpgsqlSpecificTypesCLRTypesNpgsqlTimeStamp()
        {
            // Please, check http://pgfoundry.org/forum/message.php?msg_id=1005483
            // for a discussion where an NpgsqlInet type isn't shown in a datagrid
            // This test tries to check if the type returned is an IPAddress when using
            // the GetValue() of NpgsqlDataReader and NpgsqlInet when using GetProviderValue();
            
            NpgsqlCommand command = new NpgsqlCommand("select '2010-01-17 15:45'::timestamp;", TheConnection);
            
            
            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            Object result = dr.GetValue(0);
            Object result2 = dr.GetProviderSpecificValue(0);
            
            dr.Close();
            

            Assert.AreEqual(typeof(DateTime), result.GetType());
            Assert.AreEqual(typeof(NpgsqlTimeStamp), result2.GetType());
            
            
        }


    }
    

    [TestFixture]
    public class CommandTestsV2 : CommandTests
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
    }
}

