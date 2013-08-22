// CommandTests.cs created with MonoDevelop
// User: fxjr at 11:40 PM 4/9/2008
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
using System.Data;
using System.Globalization;
using System.Net;
using NpgsqlTypes;
using System.Resources;
using System.Threading;
using System.Reflection;

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
        protected override NpgsqlConnection TheConnection
        {
            get { return _conn; }
        }

        protected override NpgsqlTransaction TheTransaction
        {
            get { return _t; }
            set { _t = value; }
        }

        protected virtual string TheConnectionString
        {
            get { return _connString; }
        }

        // Make sure SuppressBinaryBackendEncoding is initialized.
        // Try to make this test run first by prepending '__' for sorting.  This test should run before any other tests
        // that use binary backend suppression.
        [Test]
        public void __SuppressBinaryBackendEncodingInitTest()
        {
            if (SuppressBinaryBackendEncoding == null)
            {
                try
                {
                    InitBinaryBackendSuppression();

                    throw new Exception("Unknown error occurred previously");
                }
                catch (Exception e)
                {
                    throw new Exception("BaseClassTests.SuppressBinaryBackendEncoding is not bound via reflection to NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding", e);
                }
            }
        }

        [Test]
        public void ParametersGetName()
        {
            var command = new NpgsqlCommand();

            // Add parameters.
            command.Parameters.Add(new NpgsqlParameter(":Parameter1", DbType.Boolean));
            command.Parameters.Add(new NpgsqlParameter(":Parameter2", DbType.Int32));
            command.Parameters.Add(new NpgsqlParameter(":Parameter3", DbType.DateTime));
            command.Parameters.Add(new NpgsqlParameter("Parameter4", DbType.DateTime));

            var idbPrmtr = command.Parameters["Parameter1"];
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
            var command = new NpgsqlCommand();

            // Add parameters.
            command.Parameters.Add(new NpgsqlParameter(":Parameter1 ", DbType.Boolean));

            Assert.AreEqual(":Parameter1", command.Parameters[0].ParameterName);
        }

        [Test]
        public void EmptyQuery()
        {
            var command = new NpgsqlCommand(";", TheConnection);
            command.ExecuteNonQuery();
        }


        [Test]
        public void NoNameParameterAdd()
        {
            var command = new NpgsqlCommand();
            command.Parameters.Add(new NpgsqlParameter());
            command.Parameters.Add(new NpgsqlParameter());
            Assert.AreEqual(":Parameter1", command.Parameters[0].ParameterName);
            Assert.AreEqual(":Parameter2", command.Parameters[1].ParameterName);
        }


        [Test]
        public void FunctionCallFromSelect()
        {
            var command = new NpgsqlCommand("select * from funcB()", TheConnection);
            NpgsqlDataReader reader = command.ExecuteReader();
            Assert.IsNotNull(reader);
            reader.Close();
            //reader.FieldCount
        }

        [Test]
        public void ExecuteScalar()
        {
            var command = new NpgsqlCommand("select count(*) from tablea", TheConnection);
            Object result = command.ExecuteScalar();
            Assert.AreEqual(6, result);
            //reader.FieldCount
        }

        [Test]
        public void TransactionSetOk()
        {
            var command = new NpgsqlCommand("select count(*) from tablea", TheConnection);
            command.Transaction = _t;
            Object result = command.ExecuteScalar();
            Assert.AreEqual(6, result);
        }


        [Test]
        public void InsertStringWithBackslashes()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            command.Parameters["p0"].Value = @"\test";
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            result = command2.ExecuteScalar();
            Assert.AreEqual(@"\test", result);
            //reader.FieldCount
        }


        [Test]
        public void UseStringParameterWithNoNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", "test"));
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Text);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.String);
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            result = command2.ExecuteScalar();
            Assert.AreEqual("test", result);
            //reader.FieldCount
        }


        [Test]
        public void UseIntegerParameterWithNoNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", 5));
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand( "select field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            result = command2.ExecuteScalar();
            Assert.AreEqual(5, result);
            //reader.FieldCount
        }


        //[Test]
        public void UseSmallintParameterWithNoNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", (Int16) 5));
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Smallint);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.Int16);
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand("select field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            result = command2.ExecuteScalar();
            Assert.AreEqual(5, result);
            //reader.FieldCount
        }


        [Test]
        public void FunctionCallReturnSingleValue()
        {
            var command = new NpgsqlCommand("funcC();", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var result = command.ExecuteScalar();
            Assert.AreEqual(6, result);
            //reader.FieldCount
        }


        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void RollbackWithNoTransaction()
        {
            TheTransaction.Rollback();
            TheTransaction.Rollback();
        }


        [Test]
        public void FunctionCallReturnSingleValueWithPrepare()
        {
            var command = new NpgsqlCommand("funcC()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(6, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValue()
        {
            var command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValueNpgsqlDbType()
        {
            var command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }


        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValue()
        {
            var command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            Assert.AreEqual(1, command.Parameters.Count);

            command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValue_SuppressBinary()
        {
            using (SuppressBackendBinary())
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
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType()
        {
            var command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            Assert.AreEqual(1, command.Parameters.Count);

            command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType_SuppressBinary()
        {
            using (SuppressBackendBinary())
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
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2()
        {
            var command = new NpgsqlCommand("funcC(@a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            Assert.AreEqual(1, command.Parameters.Count);
            //command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2_SuppressBinary()
        {
            using (SuppressBackendBinary())
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
        }

        [Test]
        public void FunctionCallReturnResultSet()
        {
            var command = new NpgsqlCommand("funcB()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        public void CursorStatement()
        {
            Int32 i = 0;
            var command = new NpgsqlCommand("declare te cursor for select * from tablea;", TheConnection);
            command.ExecuteNonQuery();
            command.CommandText = "fetch forward 3 in te;";
            var dr = command.ExecuteReader();

            while (dr.Read())
                i++;
            Assert.AreEqual(3, i);

            i = 0;
            command.CommandText = "fetch backward 1 in te;";
            var dr2 = command.ExecuteReader();
            while (dr2.Read())
                i++;
            Assert.AreEqual(1, i);

            command.CommandText = "close te;";
            command.ExecuteNonQuery();
        }

        [Test]
        public void PreparedStatementNoParameters()
        {
            var command = new NpgsqlCommand("select * from tablea;", TheConnection);
            command.Prepare();
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }


        [Test]
        public void PreparedStatementInsert()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0);", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            command.Parameters["p0"].Value = "test";
            command.Prepare();
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }

        [Test]
        public void RTFStatementInsert()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0);", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            command.Parameters["p0"].Value = @"{\rtf1\ansi\ansicpg1252\uc1 \deff0\deflang1033\deflangfe1033{";
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);

            var result = (String)new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea);", TheConnection).ExecuteScalar();
            Assert.AreEqual(@"{\rtf1\ansi\ansicpg1252\uc1 \deff0\deflang1033\deflangfe1033{", result);
        }



        [Test]
        public void PreparedStatementInsertNullValue()
        {
            var command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0);", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Integer));
            command.Parameters["p0"].Value = DBNull.Value;
            command.Prepare();

            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }

        [Test]
        public void PreparedStatementWithParameters()
        {
            var command = new NpgsqlCommand("select * from tablea where field_int4 = :a and field_int8 = :b;", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters.Add(new NpgsqlParameter("b", DbType.Int64));
            Assert.AreEqual(2, command.Parameters.Count);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            command.Prepare();
            command.Parameters[0].Value = 3;
            command.Parameters[1].Value = 5;
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        public void PreparedStatementWithParametersNpgsqlDbType()
        {
            var command = new NpgsqlCommand("select * from tablea where field_int4 = :a and field_int8 = :b;", TheConnection);

            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("b", NpgsqlDbType.Bigint));
            Assert.AreEqual(2, command.Parameters.Count);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            command.Prepare();
            command.Parameters[0].Value = 3;
            command.Parameters[1].Value = 5;
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        public void FunctionCallWithImplicitParameters()
        {
            var command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("@a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            command.Parameters.Add(p);
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }


        [Test]
        public void PreparedFunctionCallWithImplicitParameters()
        {
            var command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            command.Parameters.Add(p);
            command.Prepare();
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }


        [Test]
        public void FunctionCallWithImplicitParametersWithNoParameters()
        {
            var command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var result = command.ExecuteScalar();
            Assert.AreEqual(6, result);
            //reader.FieldCount

        }

        [Test]
        public void FunctionCallOutputParameter()
        {
            var command = new NpgsqlCommand("funcC()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(6, command.Parameters["a"].Value);
        }

        [Test]
        public void FunctionCallOutputParameter2()
        {
            var command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("@a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(6, command.Parameters["@a"].Value);
        }

        [Test]
        public void OutputParameterWithoutName()
        {
            var command = new NpgsqlCommand("funcC", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var p = command.CreateParameter();
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(6, command.Parameters[0].Value);
        }

        [Test]
        public void FunctionReturnVoid()
        {
            var command = new NpgsqlCommand("testreturnvoid()", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        [Test]
        public void StatementOutputParameters()
        {
            var command = new NpgsqlCommand("values (4,5), (6,7)", TheConnection);
            var p = new NpgsqlParameter("a", DbType.Int32);
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

            // Should bear the values of the first tuple.
            Assert.AreEqual(4, command.Parameters["a"].Value);
            Assert.AreEqual(5, command.Parameters["b"].Value);
            Assert.AreEqual(-1, command.Parameters["c"].Value);
        }

        [Test]
        public void StringEscapeSyntax()
        {
            //on protocol version 2 connections, standard_conforming_strings is always assumed off by Npgsql.
            //regardless of this setting, Npgsql will always use the E string prefix when possible,
            //therefore, this test is not fully functional on version 2.

            //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            try
            {
                new NpgsqlCommand("set standard_conforming_strings=off;set escape_string_warning=off", TheConnection).ExecuteNonQuery();
            }
            catch
            {
            }
            string cmdTxt = "select :par";
            var command = new NpgsqlCommand(cmdTxt, TheConnection);
            var arrCommand = new NpgsqlCommand(cmdTxt, TheConnection);
            string testStrPar = "This string has a 'literal' backslash \\";
            string[,] testArrPar = new string[,] {{testStrPar, ""}, {testStrPar, testStrPar}};
            command.Parameters.AddWithValue(":par", testStrPar);
            using (var rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(rdr.GetString(0), testStrPar);
            }
            arrCommand.Parameters.AddWithValue(":par", testArrPar);
            using (var rdr = arrCommand.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(((string[,]) rdr.GetValue(0))[0, 0], testStrPar);
            }

            try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=on;set escape_string_warning=on", TheConnection)
                    .ExecuteNonQuery();
            }
            catch
            {
            }
            using (var rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(rdr.GetString(0), testStrPar);
            }
            using (var rdr = arrCommand.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(((string[,]) rdr.GetValue(0))[0, 0], testStrPar);
            }
        }

        [Test]
        public void FunctionCallStringEscape()
        {
            //on protocol version 2 connections, standard_conforming_strings is always assumed off by Npgsql.
            //regardless of this setting, Npgsql will always use the E string prefix when possible,
            //therefore, this test is not fully functional on version 2.

            int warnings = 0;
            NoticeEventHandler countWarn = delegate(Object c, NpgsqlNoticeEventArgs e) { warnings += 1; };
            TheConnection.Notice += countWarn;

            var testStrPar = "This string has a 'literal' backslash \\";
            var command = new NpgsqlCommand("trim", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter());
            command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            command.Parameters[0].Value = testStrPar;

            try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set escape_string_warning=on", TheConnection).ExecuteNonQuery();
                new NpgsqlCommand("set standard_conforming_strings=off", TheConnection).ExecuteNonQuery();
            }
            catch
            {
            }
            using (IDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(testStrPar, rdr.GetString(0));
            }

            try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=on", TheConnection).ExecuteNonQuery();
            }
            catch
            {
            }
            using (IDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(testStrPar, rdr.GetString(0));
            }

            TheConnection.Notice -= countWarn;
            Assert.AreEqual(0, warnings);
        }


        [Test]
        public void ParameterAndOperatorUnclear()
        {
            //Without parenthesis the meaning of [, . and potentially other characters is
            //a syntax error. See comment in NpgsqlCommand.GetClearCommandText() on "usually-redundant parenthesis".
            var command = new NpgsqlCommand("select :arr[2]", TheConnection);
            command.Parameters.AddWithValue(":arr", new int[] {5, 4, 3, 2, 1});
            using (var rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(rdr.GetInt32(0), 4);
            }
        }

        [Test]
        public void FunctionCallInputOutputParameter()
        {
            var command = new NpgsqlCommand("funcC(:a)", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Direction = ParameterDirection.InputOutput;
            p.Value = 4;
            command.Parameters.Add(p);
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }


        [Test]
        public void StatementMappedOutputParameters()
        {
            var command = new NpgsqlCommand("select 3, 4 as param1, 5 as param2, 6;", TheConnection);

            var p = new NpgsqlParameter("param2", NpgsqlDbType.Integer);
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
            Assert.IsFalse(ReceivedNotification); //Test we start correctly.

            var command = new NpgsqlCommand("listen notifytest;", TheConnection);
            command.ExecuteNonQuery();

            TheConnection.Notification += new NotificationEventHandler(NotificationSupportHelper);

            command = new NpgsqlCommand("notify notifytest;", TheConnection);
            command.ExecuteNonQuery();

            Assert.IsTrue(ReceivedNotification);

        }

        public bool ReceivedNotification = false;

        private void NotificationSupportHelper(Object sender, NpgsqlNotificationEventArgs args)
        {
            ReceivedNotification = true;
        }


        [Test]
        public void ByteSupport()
        {
            var command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Byte));
            command.Parameters[0].Value = 2;
            Int32 rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);
            command.Parameters.Clear();
        }


        [Test]
        public void ByteaSupport()
        {
            var command = new NpgsqlCommand("select field_bytea from tablef where field_serial = 1", TheConnection);
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void ByteaLargeSupport()
        {
            var buff = new byte[100000];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", TheConnection);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        [Test]
        public void ByteaInsertSupport1()
        {
            Byte[] toStore = {0, 1, 255, 254};

            var cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);
            var result = (Byte[]) cmd.ExecuteScalar();
            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaInsertSupport2()
        {
            Byte[] toStore = {1, 2, 127, 126};

            var cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);
            var result = (Byte[]) cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaInsertWithPrepareSupport1()
        {
            Byte[] toStore = {0};

            var cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);

            cmd.Prepare();
            var result = (Byte[]) cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaInsertWithPrepareSupport2()
        {
            Byte[] toStore = {1};

            var cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);

            cmd.Prepare();
            var result = (Byte[]) cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaParameterSupport()
        {
            var command = new NpgsqlCommand("select field_bytea from tablef where field_bytea = :bytesData", TheConnection);
            var bytes = new Byte[] {45, 44};
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ByteaParameterWithPrepareSupport()
        {
            var command = new NpgsqlCommand("select field_bytea from tablef where field_bytea = :bytesData", TheConnection);

            var bytes = new Byte[] {45, 44};
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            command.Prepare();
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }


        [Test]
        public void EnumSupport()
        {
            var command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Smallint));
            command.Parameters[0].Value = EnumTest.Value1;
            Int32 rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);
        }

        [Test]
        public void DateTimeSupport()
        {
            var command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);
            DateTime d = (DateTime) command.ExecuteScalar();

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
            var command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

            var culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            var dt = DateTime.Parse("2004-06-04 09:48:00", culture);
            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = dt;
            command.ExecuteScalar();
        }

        [Test]
        public void DateSupport()
        {
            var command = new NpgsqlCommand("select field_date from tablec where field_serial = 1;", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("2002-03-04", d.ToString("yyyy-MM-dd"));
        }

        [Test]
        public void TimeSupport()
        {
            var command = new NpgsqlCommand("select field_time from tablec where field_serial = 2;", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("10:03:45.345", d.ToString("HH:mm:ss.fff"));
        }

        [Test]
        [Ignore]
        public void TimeSupportTimezone()
        {
            var command = new NpgsqlCommand("select '13:03:45.001-05'::timetz", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("18:03:45.001", d.ToUniversalTime().ToString("HH:mm:ss.fff"));
        }

        [Test]
        public void DateTimeSupportTimezone()
        {
            var command = new NpgsqlCommand("set time zone 5;select field_timestamp_with_timezone from tableg where field_serial = 1;", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezone2()
        {
            //Changed the comparison. Did thisassume too much about ToString()?
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5; select field_timestamp_with_timezone from tableg where field_serial = 1;", TheConnection);
            var s = ((DateTime) command.ExecuteScalar()).ToUniversalTime().ToString();
            Assert.AreEqual(new DateTime(2002, 02, 02, 09, 00, 23).ToString(), s);
        }

        [Test]
        public void DateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select timestamptz'2009-11-11 20:15:43.019-03:30';", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2009-11-11 23:45:43Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezoneEuropeAmsterdam()
        {
            //1929-08-19 00:00:00+01:19:32
            // This test was provided by Christ Akkermans.
            var command = new NpgsqlCommand("SET TIME ZONE 'Europe/Amsterdam';SELECT '1929-08-19 00:00:00'::timestamptz;", TheConnection);
            var d = (DateTime) command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateTimeSupport()
        {
            var command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);

            NpgsqlTimeStamp ts;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStamp(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.ToString());

            var culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            var ts1 = NpgsqlTimeStamp.Parse("2004-06-04 09:48:00");

            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }


        [Test]
        public void ProviderDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = 2;", TheConnection);

            NpgsqlTimeStamp ts;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStamp(0);
            }

            Assert.AreEqual("2002-02-02 09:00:23.345", ts.ToString());

            var culture = new DateTimeFormatInfo();
            culture.TimeSeparator = ":";
            var ts1 = NpgsqlTimeStamp.Parse("2004-06-04 09:48:00");

            command.CommandText = "insert into tableb(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateSupport()
        {
            var command = new NpgsqlCommand("select field_date from tablec where field_serial = 1;", TheConnection);

            NpgsqlDate d;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                d = reader.GetDate(0);
            }

            Assert.AreEqual("2002-03-04", d.ToString());
        }

        [Test]
        public void ProviderTimeSupport()
        {
            var command = new NpgsqlCommand("select field_time from tablec where field_serial = 2;", TheConnection);

            NpgsqlTime t;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                t = reader.GetTime(0);
            }


            Assert.AreEqual("10:03:45.345", t.ToString());
        }

        [Test]
        public void ProviderTimeSupportTimezone()
        {
            var command = new NpgsqlCommand("select '13:03:45.001-05'::timetz", TheConnection);

            NpgsqlTimeTZ t;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                t = reader.GetTimeTZ(0);
            }

            Assert.AreEqual("18:03:45.001", t.AtTimeZone(NpgsqlTimeZone.UTC).LocalTime.ToString());
        }

        [Test]
        public void ProviderDateTimeSupportTimezone()
        {
            var command = new NpgsqlCommand("set time zone 5;select field_timestamp_with_timezone from tableg where field_serial = 1;", TheConnection);

            NpgsqlTimeStampTZ ts;
            using (var reader = command.ExecuteReader())
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
            var command = new NpgsqlCommand("set time zone 5;select timestamptz'2009-11-11 20:15:43.019-03:30';", TheConnection);

            NpgsqlTimeStampTZ ts;
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                ts = reader.GetTimeStampTZ(0);
            }

            Assert.AreEqual("2009-11-12 04:45:43.019+05", ts.ToString());
        }

        [Test]
        public void NumericSupport()
        {
            var command = new NpgsqlCommand("insert into tableb(field_numeric) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Decimal));
            command.Parameters[0].Value = 7.4M;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tableb where field_numeric = :a";
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(3);
                Assert.AreEqual(7.4000000M, result);
            }
        }

        [Test]
        public void NumericSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tableb(field_numeric) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Numeric));
            command.Parameters[0].Value = 7.4M;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tableb where field_numeric = :a";
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(3);
                Assert.AreEqual(7.4000000M, result);
            }
        }

        [Test]
        public void InsertSingleValue()
        {
            var command = new NpgsqlCommand("insert into tabled(field_float4) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", DbType.Single));
            command.Parameters[0].Value = 7.4F;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tabled where field_float4 = :a";
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetFloat(1);
                Assert.AreEqual(7.4F, result);
            }
        }

        [Test]
        public void InsertSingleValueNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tabled(field_float4) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Real));
            command.Parameters[0].Value = 7.4F;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tabled where field_float4 = :a";
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetFloat(1);
                Assert.AreEqual(7.4F, result);
            }
        }


        [Test]
        public void DoubleValueSupportWithExtendedQuery()
        {
            var command = new NpgsqlCommand("select count(*) from tabled where field_float8 = :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Double));
            command.Parameters[0].Value = 0.123456789012345D;
            command.Prepare();
            var rows = command.ExecuteScalar();
            Assert.AreEqual(1, rows);
        }

        [Test]
        public void Bug1010992DoubleValueSupport()
        {
            var command = new NpgsqlCommand("select :field_float8", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":field_float8", NpgsqlDbType.Double));
            double x = 1d/7d;
            //double value = 0.12345678901234561D;
            command.Parameters[0].Value = x;
            var valueReturned = command.ExecuteScalar();
            Assert.AreEqual(x, valueReturned);
        }

        [Test]
        public void InsertDoubleValue()
        {
            var command = new NpgsqlCommand("insert into tabled(field_float8) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", DbType.Double));
            command.Parameters[0].Value = 7.4D;
            var rowsAdded = command.ExecuteNonQuery();

            command.CommandText = "select * from tabled where field_float8 = :a";
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDouble(2);
                Assert.AreEqual(1, rowsAdded);
                Assert.AreEqual(7.4D, result);
            }
        }


        [Test]
        public void InsertDoubleValueNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tabled(field_float8) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Double));
            command.Parameters[0].Value = 7.4D;
            var rowsAdded = command.ExecuteNonQuery();

            command.CommandText = "select * from tabled where field_float8 = :a";
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDouble(2);
                Assert.AreEqual(1, rowsAdded);
                Assert.AreEqual(7.4D, result);
            }
        }


        [Test]
        public void NegativeNumericSupport()
        {
            var command = new NpgsqlCommand("select * from tableb where field_serial = 4", TheConnection);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(3);
                Assert.AreEqual(-4.3000000M, result);
            }
        }


        [Test]
        public void PrecisionScaleNumericSupport()
        {
            var command = new NpgsqlCommand("select * from tableb where field_serial = 4", TheConnection);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(3);
                Assert.AreEqual(-4.3000000M, result);
                //Assert.AreEqual(11, result.Precision);
                //Assert.AreEqual(7, result.Scale);
            }
        }

        [Test]
        public void InsertNullString()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.String));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_text is null";
            command.Parameters.Clear();
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullStringNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Text));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_text is null";
            command.Parameters.Clear();
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullDateTime()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_timestamp is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullDateTimeNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_timestamp is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullInt16()
        {
            var command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_int2 is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullInt16NpgsqlDbType()
        {
            NpgsqlCommand command = new NpgsqlCommand("insert into tableb(field_int2) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Smallint));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_int2 is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(4, result);
        }

        [Test]
        public void InsertNullInt32()
        {
            var command = new NpgsqlCommand("insert into tablea(field_int4) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_int4 is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(6, result);
        }

        [Test]
        public void InsertNullNumeric()
        {
            var command = new NpgsqlCommand("insert into tableb(field_numeric) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Decimal));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tableb where field_numeric is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(3, result);
        }

        [Test]
        public void InsertNullBoolean()
        {
            var command = new NpgsqlCommand("insert into tablea(field_bool) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Boolean));
            command.Parameters[0].Value = DBNull.Value;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select count(*) from tablea where field_bool is null";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(6, result);

        }

        [Test]
        public void InsertBoolean()
        {
            var command = new NpgsqlCommand("insert into tablea(field_bool) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Boolean));
            command.Parameters[0].Value = false;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select field_bool from tablea where field_serial = (select max(field_serial) from tablea)";
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(false, result);

        }

        [Test]
        public void AnsiStringSupport()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.AnsiString));
            command.Parameters[0].Value = "TesteAnsiString";
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = String.Format("select count(*) from tablea where field_text = '{0}'", command.Parameters[0].Value);
            command.Parameters.Clear();
            var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(1, result);
        }


        [Test]
        public void MultipleQueriesFirstResultsetEmpty()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values ('a'); select count(*) from tablea;", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(7, result);
        }

        [Test]
        [ExpectedException(typeof (NpgsqlException))]
        public void ConnectionStringWithInvalidParameterValue()
        {
            var conn = new NpgsqlConnection(TheConnectionString + ";userid=npgsql_tes;pooling=false");
            var command = new NpgsqlCommand("select * from tablea", conn);

            try
            {
                command.Connection.Open();
                command.ExecuteReader();
            }
            finally
            {
                command.Connection.Close();
            }
        }

        [Test]
        [ExpectedException(typeof (ArgumentException))]
        public void InvalidConnectionString()
        {
            var conn = new NpgsqlConnection("Server=127.0.0.1;User Id=npgsql_tests;Pooling:false");
            conn.Open();
        }

        [Test]
        public void AmbiguousFunctionParameterType()
        {
            //NpgsqlConnection conn = new NpgsqlConnection(TheConnectionString);
            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", TheConnection);
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

            command.ExecuteScalar();
        }

        [Test]
        public void AmbiguousFunctionParameterTypePrepared()
        {
            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", TheConnection);
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

            command.Prepare();
            command.ExecuteScalar();
        }

        // The following two methods don't need checks because what is being tested is the 
        // execution of parameter replacing which happens on ExecuteNonQuery method. So, if these
        // methods don't throw exception, they are ok.
        [Test]
        public void TestParameterReplace()
        {
            const string sql = @"select * from tablea where field_serial = :a";
            var command = new NpgsqlCommand(sql, TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 2;
            command.ExecuteNonQuery();
        }

        [Test]
        public void TestParameterReplace2()
        {
            const string sql = @"select * from tablea where field_serial = :a+1";
            var command = new NpgsqlCommand(sql, TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 1;
            command.ExecuteNonQuery();
        }

        [Test]
        public void TestParameterNameInParameterValue()
        {
            const string sql = "insert into tablea(field_text, field_int4) values ( :a, :b );";
            const string aValue = "test :b";

            var command = new NpgsqlCommand(sql, TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Text));
            command.Parameters[":a"].Value = aValue;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[":b"].Value = 1;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);

            var command2 = new NpgsqlCommand("select field_text, field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", TheConnection);
            using (var dr = command2.ExecuteReader())
            {
                dr.Read();
                var a = dr.GetString(0);
                var b = dr.GetInt32(1);
                Assert.AreEqual(aValue, a);
                Assert.AreEqual(1, b);
            }
        }

        [Test]
        public void TestBoolParameter1()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", TheConnection);
            var p0 = new NpgsqlParameter(":a", true);
            // with setting pramater type
            p0.DbType = DbType.Boolean;
            command.Parameters.Add(p0);
            command.ExecuteScalar();
        }

        [Test]
        public void TestBoolParameter2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", TheConnection);
            var p0 = new NpgsqlParameter(":a", true);
            // not setting parameter type
            command.Parameters.Add(p0);
            command.ExecuteScalar();
        }

        [Test]
        public void TestPointSupport()
        {
            var command = new NpgsqlCommand("select field_point from tablee where field_serial = 1", TheConnection);
            var p = (NpgsqlPoint) command.ExecuteScalar();
            Assert.AreEqual(4, p.X);
            Assert.AreEqual(3, p.Y);
        }

        [Test]
        public void TestBoxSupport()
        {
            var command = new NpgsqlCommand("select field_box from tablee where field_serial = 2", TheConnection);
            var box = (NpgsqlBox) command.ExecuteScalar();
            Assert.AreEqual(5, box.UpperRight.X);
            Assert.AreEqual(4, box.UpperRight.Y);
            Assert.AreEqual(4, box.LowerLeft.X);
            Assert.AreEqual(3, box.LowerLeft.Y);
        }

        [Test]
        public void TestLSegSupport()
        {
            var command = new NpgsqlCommand("select field_lseg from tablee where field_serial = 3", TheConnection);
            var lseg = (NpgsqlLSeg) command.ExecuteScalar();
            Assert.AreEqual(4, lseg.Start.X);
            Assert.AreEqual(3, lseg.Start.Y);
            Assert.AreEqual(5, lseg.End.X);
            Assert.AreEqual(4, lseg.End.Y);
        }

        [Test]
        public void TestClosedPathSupport()
        {
            var command = new NpgsqlCommand("select field_path from tablee where field_serial = 4", TheConnection);
            var path = (NpgsqlPath) command.ExecuteScalar();
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
            var command = new NpgsqlCommand("select field_path from tablee where field_serial = 5", TheConnection);
            var path = (NpgsqlPath) command.ExecuteScalar();
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
            var command = new NpgsqlCommand("select field_polygon from tablee where field_serial = 6", TheConnection);
            var polygon = (NpgsqlPolygon) command.ExecuteScalar();
            Assert.AreEqual(2, polygon.Count);
            Assert.AreEqual(4, polygon[0].X);
            Assert.AreEqual(3, polygon[0].Y);
            Assert.AreEqual(5, polygon[1].X);
            Assert.AreEqual(4, polygon[1].Y);
        }

        [Test]
        public void TestCircleSupport()
        {
            var command = new NpgsqlCommand("select field_circle from tablee where field_serial = 7", TheConnection);
            var circle = (NpgsqlCircle) command.ExecuteScalar();
            Assert.AreEqual(4, circle.Center.X);
            Assert.AreEqual(3, circle.Center.Y);
            Assert.AreEqual(5, circle.Radius);
        }

        [Test]
        public void SetParameterValueNull()
        {
            var cmd = new NpgsqlCommand("insert into tablef(field_bytea) values (:val)", TheConnection);
            var param = cmd.CreateParameter();
            param.ParameterName = "val";
            param.NpgsqlDbType = NpgsqlDbType.Bytea;
            param.Value = DBNull.Value;
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from tablef where field_serial = (select max(field_serial) from tablef)", TheConnection);
            var result = cmd.ExecuteScalar();
            Assert.AreEqual(DBNull.Value, result);
        }


        [Test]
        public void TestCharParameterLength()
        {
            const string sql = "insert into tableh(field_char5) values ( :a );";
            const string aValue = "atest";
            var command = new NpgsqlCommand(sql, TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Char));
            command.Parameters[":a"].Value = aValue;
            command.Parameters[":a"].Size = 5;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);

            var command2 = new NpgsqlCommand("select field_char5 from tableh where field_serial = (select max(field_serial) from tableh)", TheConnection);
            using (var dr = command2.ExecuteReader())
            {
                dr.Read();
                String a = dr.GetString(0);
                Assert.AreEqual(aValue, a);
            }
        }

        [Test]
        public void ParameterHandlingOnQueryWithParameterPrefix()
        {
            var command = new NpgsqlCommand("select to_char(field_time, 'HH24:MI') from tablec where field_serial = :a;", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Value = 2;
            command.Parameters.Add(p);
            var d = (String) command.ExecuteScalar();
            Assert.AreEqual("10:03", d);
        }

        [Test]
        public void MultipleRefCursorSupport()
        {
            var command = new NpgsqlCommand("testmultcurfunc", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var one = dr.GetInt32(0);
                dr.NextResult();
                dr.Read();
                var two = dr.GetInt32(0);
                Assert.AreEqual(1, one);
                Assert.AreEqual(2, two);
            }
        }

        [Test]
        public void ProcedureNameWithSchemaSupport()
        {
            var command = new NpgsqlCommand("public.testreturnrecord", TheConnection);
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
            var command = new NpgsqlCommand("testreturnrecord", TheConnection);
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
        public void ReturnSetofRecord()
        {
            var command = new NpgsqlCommand("testreturnsetofrecord", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Direction = ParameterDirection.Output;
            command.ExecuteNonQuery();

            Assert.AreEqual(8, command.Parameters[0].Value);
            Assert.AreEqual(9, command.Parameters[1].Value);
        }

        [Test]
        public void ReturnRecordSupportWithResultset()
        {

            var command = new NpgsqlCommand("testreturnrecordresultset", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Integer));
            command.Parameters[0].Value = 1;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[1].Value = 4;

            NpgsqlDataReader dr = null;

            try
            {
                dr = command.ExecuteReader();
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
        }

        [Test]
        public void ParameterTypeBoolean()
        {
            var p = new NpgsqlParameter();
            p.ParameterName = "test";
            p.Value = true;
            Assert.AreEqual(NpgsqlDbType.Boolean, p.NpgsqlDbType);
        }

        [Test]
        public void ParameterTypeTimestamp()
        {
            var p = new NpgsqlParameter();
            p.ParameterName = "test";
            p.Value = DateTime.Now;
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType);
        }

        [Test]
        public void ParameterTypeText()
        {
            var p = new NpgsqlParameter();
            p.ParameterName = "test";
            p.Value = "teste";
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType);
        }

        [Test]
        public void ReadUncommitedTransactionSupport()
        {
            const string sql = "select 1 as test";
            using (var c = new NpgsqlConnection(TheConnectionString))
            {
                c.Open();
                var t = c.BeginTransaction(IsolationLevel.ReadUncommitted);
                Assert.IsNotNull(t);

                var command = new NpgsqlCommand(sql, TheConnection);
                command.ExecuteReader().Close();
            }
        }

        [Test]
        public void RepeatableReadTransactionSupport()
        {
            const string sql = "select 1 as test";
            using (var c = new NpgsqlConnection(TheConnectionString))
            {
                c.Open();
                var t = c.BeginTransaction(IsolationLevel.RepeatableRead);
                Assert.IsNotNull(t);

                var command = new NpgsqlCommand(sql, TheConnection);
                command.ExecuteReader().Close();
                c.Close();
            }
        }

        [Test]
        public void SetTransactionToSerializable()
        {
            const string sql = "show transaction_isolation;";
            using (var c = new NpgsqlConnection(TheConnectionString))
            {
                c.Open();
                var t = c.BeginTransaction(IsolationLevel.Serializable);
                Assert.IsNotNull(t);
                var command = new NpgsqlCommand(sql, c);
                var isolation = (String) command.ExecuteScalar();
                c.Close();
                Assert.AreEqual("serializable", isolation);
            }
        }

        [Test]
        public void TestParameterNameWithDot()
        {
            const string sql = "insert into tableh(field_char5) values ( :a.parameter );";
            const string aValue = "atest";
            var command = new NpgsqlCommand(sql, TheConnection);
            command.Parameters.Add(new NpgsqlParameter(":a.parameter", NpgsqlDbType.Char));
            command.Parameters[":a.parameter"].Value = aValue;
            command.Parameters[":a.parameter"].Size = 5;

            var rowsAdded = -1;
            try
            {
                rowsAdded = command.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine(e.ErrorSql);
            }

            Assert.AreEqual(rowsAdded, 1);

            var command2 = new NpgsqlCommand("select field_char5 from tableh where field_serial = (select max(field_serial) from tableh)", TheConnection);
            var a = (String) command2.ExecuteScalar();
            Assert.AreEqual(aValue, a);
        }

        [Test]
        public void LastInsertedOidSupport()
        {
            var insertCommand = new NpgsqlCommand("insert into tablea(field_text) values ('a');", TheConnection);
            // Insert this dummy row, just to enable us to see what was the last oid in order we can assert it later.
            insertCommand.ExecuteNonQuery();

            var selectCommand = new NpgsqlCommand("select max(oid) from tablea;", TheConnection);
            var previousOid = (Int64) selectCommand.ExecuteScalar();

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
                var invalidCommandName = new NpgsqlCommand("invalidfunctionname", TheConnection);
                NpgsqlCommandBuilder.DeriveParameters(invalidCommandName);
            }
            catch (InvalidOperationException e)
            {
                var resman = new ResourceManager(typeof (NpgsqlCommandBuilder));
                var expected = string.Format(resman.GetString("Exception_InvalidFunctionName"), "invalidfunctionname");
                Assert.AreEqual(expected, e.Message);
            }
        }

        [Test]
        public void DoubleSingleQuotesValueSupport()
        {
            var command = new NpgsqlCommand("insert into tablea(field_text) values (:a)", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Text));
            command.Parameters[0].Value = "''";
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from tablea where field_text = :a";
            using (var dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.Read());
            }
        }

        [Test]
        public void ReturnInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values ('infinity'::timestamp);", TheConnection);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        [Test]
        public void ReturnMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values ('-infinity'::timestamp);", TheConnection);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void InsertInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Timestamp);
            p.Value = DateTime.MaxValue;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        [Test]
        public void InsertMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Timestamp);
            p.Value = DateTime.MinValue;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void InsertMinusInfinityDateTimeSupport()
        {
            var command = new NpgsqlCommand("insert into tableb(field_timestamp) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", DateTime.MinValue);
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from tableb where field_serial = (select max(field_serial) from tableb);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void MinusInfinityDateTimeSupport()
        {
            var command = TheConnection.CreateCommand();
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MinValue));
            command.CommandText = "select 1 where current_date=:p0";
            var result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void PlusInfinityDateTimeSupport()
        {
            var command = TheConnection.CreateCommand();
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MaxValue));
            command.CommandText = "select 1 where current_date=:p0";
            var result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void InetTypeSupport()
        {
            var command = new NpgsqlCommand("insert into tablej(field_inet) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Inet);
            p.Value = new NpgsqlInet("127.0.0.1");
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_inet from tablej where field_serial = (select max(field_serial) from tablej);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual((IPAddress) new NpgsqlInet("127.0.0.1"), (IPAddress) result);
        }

        [Test]
        public void IPAddressTypeSupport()
        {
            var command = new NpgsqlCommand("insert into tablej(field_inet) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Inet);
            p.Value = IPAddress.Parse("127.0.0.1");
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_inet from tablej where field_serial = (select max(field_serial) from tablej);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(IPAddress.Parse("127.0.0.1"), result);
        }

        [Test]
        public void BitTypeSupportWithPrepare()
        {
            var command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
            p.Value = true;
            command.Parameters.Add(p);
            command.Prepare();
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BitTypeSupport()
        {
            var command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
            p.Value = true;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BitTypeSupport2()
        {
            var command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
            p.Value = 3;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BitTypeSupport3()
        {
            var command = new NpgsqlCommand("insert into tablek(field_bit) values (:a);", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
            p.Value = 6;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_bit from tablek where field_serial = (select max(field_serial) from tablek);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(false, result);
        }

        //[Test]
        public void FunctionReceiveCharParameter()
        {
            var command = new NpgsqlCommand("d/;", TheConnection);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Inet);
            p.Value = IPAddress.Parse("127.0.0.1");
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_inet from tablej where field_serial = (select max(field_serial) from tablej);", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(new NpgsqlInet("127.0.0.1"), result);
        }

        [Test]
        public void FunctionCaseSensitiveName()
        {
            var command = new NpgsqlCommand("\"FunctionCaseSensitive\"", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            var result = command.ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FunctionCaseSensitiveNameWithSchema()
        {
            var command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            var result = command.ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParameters()
        {
            var command = new NpgsqlCommand("\"FunctionCaseSensitive\"", TheConnection);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParametersWithSchema()
        {
            var command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", TheConnection);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        [Test]
        public void CaseSensitiveParameterNames()
        {
            var command = new NpgsqlCommand("select :p1", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("P1", NpgsqlDbType.Integer)).Value = 5;
            var result = command.ExecuteScalar();
            Assert.AreEqual(5, result);
        }

        [Test]
        public void FunctionTestTimestamptzParameterSupport()
        {
            var command = new NpgsqlCommand("testtimestamptzparameter", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimestampTZ));
            using (var dr = command.ExecuteReader())
            {
                var count = 0;
                while (dr.Read())
                    count++;
                Assert.IsTrue(count > 1);
            }
        }

        [Test]
        public void GreaterThanInQueryStringWithPrepare()
        {
            var command = new NpgsqlCommand("select count(*) from tablea where field_serial >:param1", TheConnection);
            command.Parameters.AddWithValue(":param1", 1);
            command.Prepare();
            command.ExecuteScalar();
        }

        [Test]
        public void CharParameterValueSupport()
        {
            const String query = @"create temp table test ( tc char(1) );
                                   insert into test values(' ');
                                   select * from test where tc=:charparam";
            var command = new NpgsqlCommand(query, TheConnection);
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "charparam";

            // Exception Can't cast System.Char into any valid DbType.
            sqlParam.Value = ' ';
            command.Parameters.Add(sqlParam);
            var res = (String) command.ExecuteScalar();

            Assert.AreEqual(" ", res);
        }

        [Test]
        public void ConnectionStringCommandTimeout()
        {
            /* NpgsqlConnection connection = new NpgsqlConnection("Server=localhost; Database=test; User=postgres; Password=12345;
 CommandTimeout=180");
 NpgsqlCommand command = new NpgsqlCommand("\"Foo\"", connection);
 connection.Open();*/
            using (var conn = new NpgsqlConnection(TheConnectionString + ";CommandTimeout=180;pooling=false"))
            {
                var command = new NpgsqlCommand("\"Foo\"", conn);
                conn.Open();
                Assert.AreEqual(180, command.CommandTimeout);
            }
        }

        [Test]
        public void ParameterExplicitType()
        {
            object param = 1;

            using (NpgsqlCommand cmd = new NpgsqlCommand("select a, max(b) from (select :param as a, 1 as b) x group by a", TheConnection))
            {
                cmd.Parameters.AddWithValue("param", param);
                cmd.Parameters[0].DbType = DbType.Int32;

                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    rdr.Read();
                }

                param = "text";
                cmd.Parameters[0].DbType = DbType.String;
                using (var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    rdr.Read();
                }
            }
        }


        [Test]
        public void ParameterExplicitType2()
        {
            var command = new NpgsqlCommand(@"create temp table test ( tc date );  select * from test where tc=:param", TheConnection);
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = "param";
            sqlParam.Value = "2008-1-1";
            //sqlParam.DbType = DbType.Object;
            command.Parameters.Add(sqlParam);
            command.ExecuteScalar();
        }

        [Test]
        public void ParameterExplicitType2DbTypeObject()
        {
            var command = new NpgsqlCommand(@"create temp table test ( tc date );  select * from test where tc=:param", TheConnection);
            var sqlParam = command.CreateParameter();
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
            var command = new NpgsqlCommand(@"select * from test where tc=:param", TheConnection);

            var sqlParam = command.CreateParameter();
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
            var command = new NpgsqlCommand(@"select * from test where tc=:param or tc=:param2", TheConnection);

            var sqlParam = command.CreateParameter();
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
            var a = new NpgsqlCommand("select 'polygon ((:a :b))' ", TheConnection);
            a.Parameters.Add(new NpgsqlParameter("a", 1));
            a.Parameters.Add(new NpgsqlParameter("b", 1));
            a.ExecuteScalar();
        }

        [Test]
        public void Int32WithoutQuotesPolygon2()
        {
            var a = new NpgsqlCommand("select 'polygon ((:a :b))' ", TheConnection);
            a.Parameters.Add(new NpgsqlParameter("a", 1)).DbType = DbType.Int32;
            a.Parameters.Add(new NpgsqlParameter("b", 1)).DbType = DbType.Int32;
            a.ExecuteScalar();
        }

        [Test]
        public void TestUUIDDataType()
        {
            const string createTable =
                @"DROP TABLE if exists public.person;
                  CREATE TABLE public.person ( 
                    person_id serial PRIMARY KEY NOT NULL,
                    person_uuid uuid NOT NULL
                  ) WITH(OIDS=FALSE);";
            var command = new NpgsqlCommand(createTable, TheConnection);
            command.ExecuteNonQuery();

            NpgsqlParameter uuidDbParam = new NpgsqlParameter(":param1", NpgsqlDbType.Uuid);
            uuidDbParam.Value = Guid.NewGuid();

            command = new NpgsqlCommand(@"INSERT INTO person (person_uuid) VALUES (:param1);", TheConnection);
            command.Parameters.Add(uuidDbParam);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("SELECT person_uuid::uuid FROM person LIMIT 1", TheConnection);
            var result = command.ExecuteScalar();
            Assert.AreEqual(typeof (Guid), result.GetType());
        }

        [Test]
        public void TestBug1006158OutputParameters()
        {
            const string createFunction =
                @"CREATE OR REPLACE FUNCTION more_params(OUT a integer, OUT b boolean) AS
            $BODY$DECLARE
                BEGIN
                    a := 3;
                    b := true;
                END;$BODY$
              LANGUAGE 'plpgsql' VOLATILE;";

            var command = new NpgsqlCommand(createFunction, TheConnection);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("more_params", TheConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Direction = ParameterDirection.Output;
            command.Parameters.Add(new NpgsqlParameter("b", DbType.Boolean));
            command.Parameters[1].Direction = ParameterDirection.Output;

            var result = command.ExecuteScalar();

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

            var result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection).ExecuteScalar();
            Assert.AreEqual(1, result);

            TheTransaction.Rollback(theSavePoint);

            result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection).ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestSavePointWithSemicolon()
        {
            if (TheConnection.PostgreSqlVersion < new Version("8.0.0"))
                Assert.Ignore("Not supported on Postgres version {0} (< 8.0.0)", TheConnection.PostgreSqlVersion);

            const String theSavePoint = "theSavePoint;";

            TheTransaction.Save(theSavePoint);

            new NpgsqlCommand("insert into tablea (field_text) values ('savepointtest')", TheConnection).ExecuteNonQuery();

            var result = new NpgsqlCommand("select count(*) from tablea where field_text = 'savepointtest'", TheConnection) .ExecuteScalar();
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
            var cmd = new NpgsqlCommand("select C1 from testpreparedstatementparametercast where :p0 is null or  C1 = :p0 ", TheConnection);

            var paramP0 = cmd.CreateParameter();
            paramP0.ParameterName = "p0";
            paramP0.DbType = DbType.Int32;
            cmd.Parameters.Add(paramP0);
            cmd.Prepare(); // This cause a runtime exception // Tested with PostgreSQL 8.3 //
        }

        [Test]
        [ExpectedException(typeof (NpgsqlException))]
        public void TestErrorInPreparedStatementCausesReleaseConnectionToThrowException()
        {
            // This is caused by having an error with the prepared statement and later, Npgsql is trying to release the plan as it was successful created.             
            var cmd = new NpgsqlCommand("sele", TheConnection);
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

            using (var cmd = new NpgsqlCommand("NullTest", TheConnection))
            {
                var parameter = new NpgsqlParameter("", NpgsqlDbType.Integer | NpgsqlDbType.Array);
                parameter.Value = new object[] {5, 5, DBNull.Value};
                cmd.Parameters.Add(parameter);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void TestBug1010675ArrayParameterWithNullValue()
        {
            new NpgsqlCommand(@"CREATE OR REPLACE FUNCTION NullTest (input INT4[]) RETURNS VOID                             
            AS $$
            DECLARE
            BEGIN
            END
            $$ LANGUAGE plpgsql;", TheConnection).ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand("NullTest", TheConnection))
            {
                NpgsqlParameter parameter = new NpgsqlParameter("", NpgsqlDbType.Integer | NpgsqlDbType.Array);
                parameter.Value = new object[] {5, 5, null};
                cmd.Parameters.Add(parameter);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void VarCharArrayHandling()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Varchar | NpgsqlDbType.Array);
                parameter.Value = new object[] {"test", "test"};
                cmd.Parameters.Add(parameter);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void DoubleArrayHandling()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var inVal = new[] {1.2d, 1.3d};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                var retVal = (Double[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void DoubleArrayHandlingZeroItem()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var inVal = new Double[] {};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                var retVal = (Double[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
            }
        }

        [Test]
        public void DoubleArrayHandlingPrepared()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var inVal = new[] {1.2d, 1.3d};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (Double[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void DoubleArrayHandlingZeroItemPrepared()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var inVal = new Double[] {};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (Double[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
            }
        }

        [Test]
        public void Bug1010521NpgsqlIntervalShouldBeQuoted()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Interval);
                parameter.Value = new NpgsqlInterval(DateTime.Now.TimeOfDay);
                cmd.Parameters.Add(parameter);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void Bug1010543Int32MinValueThrowException()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
                parameter.Value = Int32.MinValue;
                cmd.Parameters.Add(parameter);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void Bug1010543Int16MinValueThrowException()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var parameter = new NpgsqlParameter("p1", DbType.Int16);
                parameter.Value = Int16.MinValue;
                cmd.Parameters.Add(parameter);
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void Bug1010543Int16MinValueThrowExceptionPrepared()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var parameter = new NpgsqlParameter("p1", DbType.Int16);
                parameter.Value = Int16.MinValue;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void HandleInt16MinValueParameter()
        {
            var command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = Int16.MinValue;
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int16.MinValue, result);
        }

        [Test]
        public void HandleInt32MinValueParameter()
        {
            var command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = Int32.MinValue;
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int32.MinValue, result);
        }

        [Test]
        public void HandleInt64MinValueParameter()
        {
            var command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int64));
            command.Parameters[0].Value = Int64.MinValue;
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int64.MinValue, result);
        }

        [Test]
        public void HandleInt16MinValueParameterPrepared()
        {
            var command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = Int16.MinValue;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int16.MinValue, result);
        }

        [Test]
        public void HandleInt32MinValueParameterPrepared()
        {
            var command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = Int32.MinValue;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int32.MinValue, result);
        }

        [Test]
        public void HandleInt64MinValueParameterPrepared()
        {
            var command = new NpgsqlCommand("select :a", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int64));
            command.Parameters[0].Value = Int64.MinValue;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int64.MinValue, result);
        }

        [Test]
        public void Bug1010557BackslashGetDoubled()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Text);
                parameter.Value = "test\\str";
                cmd.Parameters.Add(parameter);
                var result = cmd.ExecuteScalar();
                Assert.AreEqual("test\\str", result);
            }
        }

        [Test]
        public void NumberConversionWithCulture()
        {
            using (var cmd = new NpgsqlCommand("select :p1", TheConnection))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double);
                parameter.Value = 5.5;
                cmd.Parameters.Add(parameter);
                var result = cmd.ExecuteScalar();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("");
                Assert.AreEqual(5.5, result);
            }
        }

        [Test]
        public void TestNullParameterValueInStatement()
        {
            // Test by Andrus Moor
            var cmd = TheConnection.CreateCommand();
            int? i = null;
            cmd.Parameters.Add(new NpgsqlParameter("p0", i));
            cmd.CommandText = "select :p0 is null or :p0=0 ";
            cmd.ExecuteScalar();
        }

        [Test]
        public void PreparedStatementWithParametersWithSize()
        {
            using (var cmd = new NpgsqlCommand("select :p0, :p1;", TheConnection))
            {
                var parameter = new NpgsqlParameter("p0", NpgsqlDbType.Varchar);
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
            var cmd = new NpgsqlCommand();
            cmd.CommandTimeout = Int32.MaxValue;
            cmd.Connection = TheConnection;
            Assert.AreEqual(Int32.MaxValue, cmd.CommandTimeout);
        }

        [Test]
        public void SelectInfinityValueDateDataType()
        {
            var cmd = TheConnection.CreateCommand();
            cmd.CommandText = "create temp table test (dt date); insert into test values ('-infinity'::date);select * from test";
            using (var dr = cmd.ExecuteReader())
            {
                dr.Read();
                // InvalidCastException was unhandled
                // at Npgsql.ForwardsOnlyDataReader.GetValue(Int32 Index)
                //  at Npgsql.NpgsqlDataReader.GetDateTime(Int32 i) ..
                dr.GetDateTime(0);
            }
        }

        [Test]
        public void DeriveParametersWithParameterNameFromFunction()
        {
            var command = new NpgsqlCommand("testoutparameter2", TheConnection);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(":x", command.Parameters[0].ParameterName);
            Assert.AreEqual(":y", command.Parameters[1].ParameterName);
        }

        [Test]
        public void NegativeMoneySupport()
        {
            var command = new NpgsqlCommand("select '-10.5'::money", TheConnection);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(0);
                Assert.AreEqual(-10.5, result);
            }
        }

        [Test]
        public void Bug1011085()
        {
            // Money format is not set in accordance with the system locale format
            var command = new NpgsqlCommand("select :moneyvalue", TheConnection);
            var expectedValue = 8.99m;
            command.Parameters.Add("moneyvalue", NpgsqlDbType.Money).Value = expectedValue;
            var result = (Decimal) command.ExecuteScalar();
            Assert.AreEqual(expectedValue, result);

            expectedValue = 100m;
            command.Parameters[0].Value = expectedValue;
            result = (Decimal) command.ExecuteScalar();
            Assert.AreEqual(expectedValue, result);

            expectedValue = 72.25m;
            command.Parameters[0].Value = expectedValue;
            result = (Decimal) command.ExecuteScalar();
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void Bug1010714AndPatch1010715()
        {
            var command = new NpgsqlCommand("select field_bytea from tablef where field_bytea = :bytesData", TheConnection);
            var bytes = new Byte[] {45, 44};
            command.Parameters.AddWithValue(":bytesData", bytes);
            Assert.AreEqual(DbType.Binary, command.Parameters[0].DbType);
            var result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void TestNpgsqlSpecificTypesCLRTypesNpgsqlInet()
        {
            // Please, check http://pgfoundry.org/forum/message.php?msg_id=1005483
            // for a discussion where an NpgsqlInet type isn't shown in a datagrid
            // This test tries to check if the type returned is an IPAddress when using
            // the GetValue() of NpgsqlDataReader and NpgsqlInet when using GetProviderValue();

            var command = new NpgsqlCommand("select '192.168.10.10'::inet;", TheConnection);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetValue(0);
                Assert.AreEqual(typeof (IPAddress), result.GetType());
            }
        }

        [Test]
        public void Bug1011100NpgsqlParameterandDBNullValue()
        {
            using (var cmd = new NpgsqlCommand("select :BLOB"))
            {
                cmd.Connection = TheConnection;

                var paramBLOB = new NpgsqlParameter();
                paramBLOB.ParameterName = "BLOB";
                cmd.Parameters.Add(paramBLOB);
                cmd.Parameters[0].Value = DBNull.Value;
                Assert.AreEqual(DbType.String, cmd.Parameters[0].DbType);

                cmd.Parameters[0].Value = new byte[] {1, 2, 3};
                Assert.AreEqual(DbType.Binary, cmd.Parameters[0].DbType);
            }
        }

        [Test]
        public void TestNpgsqlSpecificTypesCLRTypesNpgsqlTimeStamp()
        {
            // Please, check http://pgfoundry.org/forum/message.php?msg_id=1005483
            // for a discussion where an NpgsqlInet type isn't shown in a datagrid
            // This test tries to check if the type returned is an IPAddress when using
            // the GetValue() of NpgsqlDataReader and NpgsqlInet when using GetProviderValue();

            var command = new NpgsqlCommand("select '2010-01-17 15:45'::timestamp;", TheConnection);
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetValue(0);
                var result2 = dr.GetProviderSpecificValue(0);
                Assert.AreEqual(typeof (DateTime), result.GetType());
                Assert.AreEqual(typeof (NpgsqlTimeStamp), result2.GetType());
            }
        }

        [Test]
        public void TimeoutFirstParameters()
        {
            //on protocol version 2 connections, standard_conforming_strings is always assumed off by Npgsql.
            //regardless of this setting, Npgsql will always use the E string prefix when possible,
            //therefore, this test is not fully functional on version 2.
 
            var conn = new NpgsqlConnection(TheConnectionString);
            conn.Open();
            try//the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=off", conn).ExecuteNonQuery();
            }
            catch{}

            using (conn = new NpgsqlConnection(TheConnectionString)) {
                conn.Open();
                try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
                {
                    new NpgsqlCommand("set standard_conforming_strings=off", conn).ExecuteNonQuery();
                }
                catch
                {
                }

                var command = new NpgsqlCommand("SELECT :dummy, pg_sleep(1.5)", conn);
                command.Parameters.Add(new NpgsqlParameter("dummy", NpgsqlDbType.Text));
                command.Parameters[0].Value = "foo";
                command.CommandTimeout = 1;
                try
                {
                    command.ExecuteNonQuery();
                    Assert.Fail("1.5s command survived a 1s timeout");
                }
                catch (NpgsqlException)
                {
                    // We cannot currently identify that the exception was a timeout
                    // in a locale-independent fashion, so just assume so.
                }
            }
        }

        [Test]
        public void TimeoutFirstFunctionCall()
        {
            // Function calls entail internal queries; verify that they do not
            // sabotage the requested timeout.

            using (var conn = new NpgsqlConnection(TheConnectionString)) {
                conn.Open();

                var command = new NpgsqlCommand("pg_sleep", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new NpgsqlParameter());
                command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Double;
                command.Parameters[0].Value = 1.5;
                command.CommandTimeout = 1;
                try
                {
                    command.ExecuteNonQuery();
                    Assert.Fail("1.5s function call survived a 1s timeout");
                }
                catch (NpgsqlException)
                {
                    // We cannot currently identify that the exception was a timeout
                    // in a locale-independent fashion, so just assume so.
                }
            }
        }

        [Test]
        public void Bug1010788UpdateRowSource()
        {
            using (var conn = new NpgsqlConnection(TheConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("select * from tableB", conn);
                Assert.AreEqual(UpdateRowSource.Both, command.UpdatedRowSource);

                var cmdBuilder = new NpgsqlCommandBuilder();
                var da = new NpgsqlDataAdapter(command);
                cmdBuilder.DataAdapter = da;
                Assert.IsNotNull(da.SelectCommand);
                Assert.IsNotNull(cmdBuilder.DataAdapter);

                NpgsqlCommand updateCommand = cmdBuilder.GetUpdateCommand();
                Assert.AreEqual(UpdateRowSource.None, updateCommand.UpdatedRowSource);
            }
        }

        [Test]
        public void VerifyFunctionWithNoParametersWithDeriveParameters()
        {
            var command = new NpgsqlCommand("funcb", TheConnection);
            NpgsqlCommandBuilder.DeriveParameters(command);
        }

        [Test]
        public void DataTypeTests()
        {
            // Test all types according to this table:
            // http://www.postgresql.org/docs/9.1/static/datatype.html

            // bigint
            var cmd = new NpgsqlCommand("select 1::bigint", TheConnection);
            var result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Int64), result.GetType());

            // bit
            cmd.CommandText = "select '1'::bit(1)";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Boolean), result.GetType());

            // bit(2)
            cmd.CommandText = "select '11'::bit(2)";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (BitString), result.GetType());

            // boolean
            cmd.CommandText = "select 1::boolean";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Boolean), result.GetType());

            // box
            cmd.CommandText = "select '((7,4),(8,3))'::box";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (NpgsqlBox), result.GetType());

            // bytea 
            cmd.CommandText = @"SELECT E'\\xDEADBEEF'::bytea;";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Byte[]), result.GetType());

            // varchar(2)
            cmd.CommandText = "select 'aa'::varchar(2);";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (String), result.GetType());

            // char(2)
            cmd.CommandText = "select 'aa'::char(2);";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (String), result.GetType());

            // cidr
            cmd.CommandText = "select '192.168/24'::cidr";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (String), result.GetType());

            // int4
            cmd.CommandText = "select 1::int4";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Int32), result.GetType());

            // int8
            cmd.CommandText = "select 1::int8";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Int64), result.GetType());


            /*
            // time
            cmd.CommandText = "select current_time::time";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof(NpgsqlTime), result.GetType());

            // timetz
            cmd.CommandText = "select current_time::timetz";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof(NpgsqlTimeTZ), result.GetType());
            */
        }
    }


    [TestFixture]
    public class CommandTestsV2 : CommandTests
    {
        protected override NpgsqlConnection TheConnection
        {
            get { return _connV2; }
        }

        protected override NpgsqlTransaction TheTransaction
        {
            get { return _tV2; }
            set { _tV2 = value; }
        }

        protected override string TheConnectionString
        {
            get { return _connV2String; }
        }
    }
}

