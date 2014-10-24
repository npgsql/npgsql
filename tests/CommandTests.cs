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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Localization;
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
    public class CommandTests : TestBase
    {
        public CommandTests(string backendVersion) : base(backendVersion) { }

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
                    throw new Exception("TestBase.SuppressBinaryBackendEncoding is not bound via reflection to NpgsqlTypes.NpgsqlTypesHelper.SuppressBinaryBackendEncoding", e);
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
            var command = new NpgsqlCommand(";", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() returns setof data as 'select * from data;' language 'sql';");
            var command = new NpgsqlCommand("select * from funcB()", Conn);
            var reader = command.ExecuteReader();
            Assert.IsNotNull(reader);
            reader.Close();
            //reader.FieldCount
        }

        [Test]
        public void ExecuteScalar()
        {
            for (var i = 0; i < 6; i++)
                ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('X')");

            using (var command = new NpgsqlCommand("select count(*) from data", Conn))
            {
                Object result = command.ExecuteScalar();
                Assert.AreEqual(6, result);
                //reader.FieldCount
            }
        }

        [Test]
        public void TransactionSetOk()
        {
            ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('X')");
            using (var t = Conn.BeginTransaction())
            {
                var command = new NpgsqlCommand("select count(*) from data", Conn);
                command.Transaction = t;
                Object result = command.ExecuteScalar();
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void UseStringParameterWithNoNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_text) values (:p0)", Conn);
            command.Parameters.Add(new NpgsqlParameter("p0", "test"));
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Text);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.String);
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand("select field_text from data where field_serial = (select max(field_serial) from data)", Conn);
            result = command2.ExecuteScalar();
            Assert.AreEqual("test", result);
            //reader.FieldCount
        }

        [Test]
        public void UseIntegerParameterWithNoNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_int4) values (:p0)", Conn);
            command.Parameters.Add(new NpgsqlParameter("p0", 5));
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand( "select field_int4 from data where field_serial = (select max(field_serial) from data)", Conn);
            result = command2.ExecuteScalar();
            Assert.AreEqual(5, result);
            //reader.FieldCount
        }

        //[Test]
        public void UseSmallintParameterWithNoNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_int4) values (:p0)", Conn);
            command.Parameters.Add(new NpgsqlParameter("p0", (Int16) 5));
            Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Smallint);
            Assert.AreEqual(command.Parameters[0].DbType, DbType.Int16);
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(1, result);

            var command2 = new NpgsqlCommand("select field_int4 from data where field_serial = (select max(field_serial) from data)", Conn);
            result = command2.ExecuteScalar();
            Assert.AreEqual(5, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallReturnSingleValue()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
            //reader.FieldCount
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void RollbackWithNoTransaction()
        {
            var transaction = Conn.BeginTransaction();
            transaction.Rollback();
            transaction.Rollback();
        }

        [Test]
        public void FunctionCallReturnSingleValueWithPrepare()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValue()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersReturnSingleValueNpgsqlDbType()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        private void FunctionCallWithParametersPrepareReturnSingleValueInternal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            Assert.AreEqual(1, command.Parameters.Count);

            command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValue()
        {
            FunctionCallWithParametersPrepareReturnSingleValueInternal();
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValue_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                FunctionCallWithParametersPrepareReturnSingleValueInternal();
            }
        }

        private void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbTypeInternal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");

            var command = new NpgsqlCommand("funcC(:a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            Assert.AreEqual(1, command.Parameters.Count);

            command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType()
        {
            FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbTypeInternal();
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbTypeInternal();
            }
        }

        private void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2Internal()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");

            var command = new NpgsqlCommand("funcC(@a)", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Integer));
            Assert.AreEqual(1, command.Parameters.Count);
            //command.Prepare();
            command.Parameters[0].Value = 4;
            var result = (Int64) command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2()
        {
            FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2Internal();
        }

        [Test]
        public void FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                FunctionCallWithParametersPrepareReturnSingleValueNpgsqlDbType2Internal();
            }
        }

        [Test]
        public void FunctionCallReturnResultSet()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcB() returns setof data as 'select * from data;' language 'sql';");
            var command = new NpgsqlCommand("funcB()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test]
        public void CursorStatement()
        {
            using (var t = Conn.BeginTransaction())
            {
                for (var x = 0; x < 5; x++)
                    ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");

                Int32 i = 0;
                var command = new NpgsqlCommand("declare te cursor for select * from data;", Conn);
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
        }

        [Test]
        public void PreparedStatementNoParameters()
        {
            var command = new NpgsqlCommand("select * from data;", Conn);
            command.Prepare();
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
            dr.Close();
        }

        [Test, Description("Basic prepared system scenario. Checks proper backend deallocation of the statement.")]
        public void PreparedStatementInsert()
        {
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));

            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_text) VALUES (:p0);", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
                cmd.Parameters["p0"].Value = "test";
                cmd.Prepare();
                using (var dr = cmd.ExecuteReader())
                {
                    Assert.IsNotNull(dr);
                    dr.Close();
                    Assert.That(dr.RecordsAffected, Is.EqualTo(1));
                }
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
            }
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'test'"), Is.EqualTo(1));
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
        }

        [Test]
        public void RTFStatementInsert()
        {
            var command = new NpgsqlCommand("insert into data(field_text) values (:p0);", Conn);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
            command.Parameters["p0"].Value = @"{\rtf1\ansi\ansicpg1252\uc1 \deff0\deflang1033\deflangfe1033{";
            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);

            var result = (String)new NpgsqlCommand("select field_text from data where field_serial = (select max(field_serial) from data);", Conn).ExecuteScalar();
            Assert.AreEqual(@"{\rtf1\ansi\ansicpg1252\uc1 \deff0\deflang1033\deflangfe1033{", result);
        }

        [Test]
        public void PreparedStatementInsertNullValue()
        {
            var command = new NpgsqlCommand("insert into data(field_int4) values (:p0);", Conn);
            command.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Integer));
            command.Parameters["p0"].Value = DBNull.Value;
            command.Prepare();

            var dr = command.ExecuteReader();
            Assert.IsNotNull(dr);
        }

        [Test]
        public void PreparedStatementWithParameters()
        {
            var command = new NpgsqlCommand("select * from data where field_int4 = :a and field_int8 = :b;", Conn);
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
            var command = new NpgsqlCommand("select * from data where field_int4 = :a and field_int8 = :b;", Conn);

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

        [Test, Description("Makes sure that calling Prepare() twice on a command deallocates the first prepared statement")]
        public void PrepareStatementDoublePrepare()
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_text) VALUES (:p0)", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
                cmd.Parameters["p0"].Value = "test";
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO data (field_int4) VALUES (:p0)";
                cmd.Parameters.Clear();
                cmd.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Integer));
                cmd.Parameters["p0"].Value = 8;
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
        }

        [Test]
        public void FunctionCallWithImplicitParameters()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
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
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
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
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
            //reader.FieldCount
        }

        [Test]
        public void FunctionCallOutputParameter()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(1, command.Parameters["a"].Value);
        }

        [Test]
        public void FunctionCallOutputParameter2()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = new NpgsqlParameter("@a", DbType.Int32);
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(1, command.Parameters["@a"].Value);
        }

        [Test]
        public void OutputParameterWithoutName()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC() returns int8 as 'select count(*) from data;' language 'sql'");
            var command = new NpgsqlCommand("funcC", Conn);
            command.CommandType = CommandType.StoredProcedure;
            var p = command.CreateParameter();
            p.Direction = ParameterDirection.Output;
            p.Value = -1;
            command.Parameters.Add(p);
            command.ExecuteNonQuery();
            Assert.AreEqual(1, command.Parameters[0].Value);
        }

        [Test]
        public void FunctionReturnVoid()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnvoid() returns void as
                              '
                              begin
                                      insert into data(field_text) values (''testvoid'');
                                      return;
                              end;
                              ' language 'plpgsql';");
            var command = new NpgsqlCommand("testreturnvoid()", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        [Test]
        public void StatementOutputParameters()
        {
            var command = new NpgsqlCommand("values (4,5), (6,7)", Conn);
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
            //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            try
            {
                new NpgsqlCommand("set standard_conforming_strings=off;set escape_string_warning=off", Conn).ExecuteNonQuery();
            }
            catch
            {
            }
            string cmdTxt = "select :par";
            var command = new NpgsqlCommand(cmdTxt, Conn);
            var arrCommand = new NpgsqlCommand(cmdTxt, Conn);
            string testStrPar = "This string has a single quote: ', a double quote: \", and a backslash: \\";
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
                new NpgsqlCommand("set standard_conforming_strings=on;set escape_string_warning=on", Conn)
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
            int warnings = 0;
            NoticeEventHandler countWarn = delegate(Object c, NpgsqlNoticeEventArgs e) { warnings += 1; };
            Conn.Notice += countWarn;

            var testStrPar = "This string has a 'literal' backslash \\";
            var command = new NpgsqlCommand("trim", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter());
            command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            command.Parameters[0].Value = testStrPar;

            try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set escape_string_warning=on", Conn).ExecuteNonQuery();
                new NpgsqlCommand("set standard_conforming_strings=off", Conn).ExecuteNonQuery();
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
                new NpgsqlCommand("set standard_conforming_strings=on", Conn).ExecuteNonQuery();
            }
            catch
            {
            }
            using (IDataReader rdr = command.ExecuteReader())
            {
                rdr.Read();
                Assert.AreEqual(testStrPar, rdr.GetString(0));
            }

            Conn.Notice -= countWarn;
            Assert.AreEqual(0, warnings);
        }

        [Test]
        public void ParameterAndOperatorUnclear()
        {
            //Without parenthesis the meaning of [, . and potentially other characters is
            //a syntax error. See comment in NpgsqlCommand.GetClearCommandText() on "usually-redundant parenthesis".
            var command = new NpgsqlCommand("select :arr[2]", Conn);
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
            ExecuteNonQuery(@"INSERT INTO data (field_int4) VALUES (4)");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION funcC(int4) returns int8 as 'select count(*) from data where field_int4 = $1;' language 'sql'");
            var command = new NpgsqlCommand("funcC(:a)", Conn);
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
            var command = new NpgsqlCommand("select 3, 4 as param1, 5 as param2, 6;", Conn);

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
            var receivedNotification = false;
            ExecuteNonQuery("listen notifytest");
            Conn.Notification += (o, e) => receivedNotification = true;
            ExecuteNonQuery("notify notifytest");
            Assert.IsTrue(receivedNotification);
        }

        [Test]
        public void ByteSupport()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_int2) VALUES (:a)", Conn))
            {
                command.Parameters.Add(new NpgsqlParameter("a", DbType.Byte));
                command.Parameters[0].Value = 2;
                var rowsAdded = command.ExecuteNonQuery();
                Assert.AreEqual(1, rowsAdded);
                command.Parameters.Clear();
            }
        }

        [Test]
        public void ByteaSupport()
        {
            ExecuteNonQuery(string.Format(@"INSERT INTO data(field_bytea) VALUES (E'\123\056')"));
            var command = new NpgsqlCommand("SELECT field_bytea FROM data", Conn);
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void ByteaEmptySupport()
        {
            var buff = new byte[0];
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        private void ByteaEmptyWithPrepareSupport_Internal()
        {
            var buff = new byte[0];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            command.Prepare();
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        [Test]
        public void ByteaEmptyWithPrepareSupport()
        {
            ByteaEmptyWithPrepareSupport_Internal();
        }

        [Test]
        public void ByteaEmptyWithPrepareSupport_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ByteaEmptyWithPrepareSupport_Internal();
            }
        }

        [Test]
        public void ByteaLargeSupport()
        {
            var buff = new byte[100000];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        private void ByteaLargeWithPrepareSupport_Internal()
        {
            var buff = new byte[100000];
            new Random().NextBytes(buff);
            var command = new NpgsqlCommand("select :val", Conn);
            command.Parameters.Add("val", NpgsqlDbType.Bytea);
            command.Parameters["val"].Value = buff;
            command.Prepare();
            var result = (Byte[]) command.ExecuteScalar();
            Assert.AreEqual(buff, result);
        }

        [Test]
        public void ByteaLargeWithPrepareSupport()
        {
            ByteaLargeWithPrepareSupport_Internal();
        }

        [Test]
        public void ByteaLargeWithPrepareSupport_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ByteaLargeWithPrepareSupport_Internal();
            }
        }

        [Test]
        public void ByteaInsertSupport1()
        {
            Byte[] toStore = {0, 1, 255, 254};

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);
            var result = (Byte[]) cmd.ExecuteScalar();
            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaInsertSupport2()
        {
            Byte[] toStore = {1, 2, 127, 126};

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);
            var result = (Byte[]) cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaInsertWithPrepareSupport1()
        {
            Byte[] toStore = {0};

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);

            cmd.Prepare();
            var result = (Byte[]) cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaInsertWithPrepareSupport2()
        {
            Byte[] toStore = {1};

            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            cmd.Parameters.Add(new NpgsqlParameter("val", DbType.Binary));
            cmd.Parameters[0].Value = toStore;
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);

            cmd.Prepare();
            var result = (Byte[]) cmd.ExecuteScalar();

            Assert.AreEqual(toStore, result);
        }

        [Test]
        public void ByteaParameterSupport()
        {
            var command = new NpgsqlCommand("select field_bytea from data where field_bytea = :bytesData", Conn);
            var bytes = new byte[] {1,2,3,4,5,34,39,48,49,50,51,52,92,127,128,255,254,253,252,251};
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }

        private void ByteaParameterWithPrepareSupport_Internal()
        {
            var command = new NpgsqlCommand("select field_bytea from data where field_bytea = :bytesData", Conn);

            var bytes = new byte[] {1,2,3,4,5,34,39,48,49,50,51,52,92,127,128,255,254,253,252,251};
            command.Parameters.Add(":bytesData", NpgsqlTypes.NpgsqlDbType.Bytea);
            command.Parameters[":bytesData"].Value = bytes;
            command.Prepare();
            Object result = command.ExecuteNonQuery();
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ByteaParameterWithPrepareSupport()
        {
            ByteaParameterWithPrepareSupport_Internal();
        }

        [Test]
        public void ByteaParameterWithPrepareSupport_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ByteaParameterWithPrepareSupport_Internal();
            }
        }

        [Test]
        public void EnumSupport()
        {
            var command = new NpgsqlCommand("insert into data(field_int2) values (:a)", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Smallint));
            command.Parameters[0].Value = EnumTest.Value1;
            Int32 rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);
        }

        [Test]
        public void DateTimeSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_timestamp) VALUES ('2002-02-02 09:00:23.345')");
            using (var command = new NpgsqlCommand("SELECT field_timestamp FROM data", Conn))
            {
                var d = (DateTime) command.ExecuteScalar();

                Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
                Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

                var culture = new DateTimeFormatInfo();
                culture.TimeSeparator = ":";
                var dt = System.DateTime.Parse("2004-06-04 09:48:00", culture);

                command.CommandText = "insert into data(field_timestamp) values (:a);";
                command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
                command.Parameters[0].Value = dt;

                command.ExecuteScalar();
            }
        }

        [Test]
        public void DateTimeSupportNpgsqlDbType()
        {
            ExecuteNonQuery("INSERT INTO data (field_timestamp) VALUES ('2002-02-02 09:00:23.345')");

            using (var command = new NpgsqlCommand("SELECT field_timestamp FROM data;", Conn))
            {
                var d = (DateTime) command.ExecuteScalar();
                Assert.AreEqual("2002-02-02 09:00:23Z", d.ToString("u"));

                var culture = new DateTimeFormatInfo();
                culture.TimeSeparator = ":";
                var dt = DateTime.Parse("2004-06-04 09:48:00", culture);
                command.CommandText = "insert into data(field_timestamp) values (:a);";
                command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
                command.Parameters[0].Value = dt;
                command.ExecuteScalar();
            }
        }

        [Test]
        public void DateSupport()
        {
            ExecuteNonQuery("INSERT INTO data(field_date) VALUES ('2002-03-04')");
            var d = (DateTime)ExecuteScalar("SELECT field_date FROM data");
            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("2002-03-04", d.ToString("yyyy-MM-dd"));
        }

        [Test]
        public void TimeSupport()
        {
            ExecuteNonQuery("INSERT INTO data(field_time) VALUES ('10:03:45.345')");
            var d = (DateTime)ExecuteScalar("SELECT field_time FROM data");
            Assert.AreEqual(DateTimeKind.Unspecified, d.Kind);
            Assert.AreEqual("10:03:45.345", d.ToString("HH:mm:ss.fff"));
        }

        [Test]
        [Ignore]
        public void TimeSupportTimezone()
        {
            var command = new NpgsqlCommand("select '13:03:45.001-05'::timetz", Conn);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("18:03:45.001", d.ToUniversalTime().ToString("HH:mm:ss.fff"));
        }

        [Test]
        public void DateTimeSupportTimezone()
        {
            ExecuteNonQuery("INSERT INTO data(field_timestamp_with_timezone) VALUES ('2002-02-02 09:00:23.345Z')");
            var d = (DateTime) ExecuteScalar("SET TIME ZONE 5; SELECT field_timestamp_with_timezone FROM data");
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2002-02-02 09:00:23Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezone2()
        {
            ExecuteNonQuery("INSERT INTO data(field_timestamp_with_timezone) VALUES ('2002-02-02 09:00:23.345Z')");
            //Changed the comparison. Did thisassume too much about ToString()?
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5; select field_timestamp_with_timezone from data", Conn);
            var s = ((DateTime) command.ExecuteScalar()).ToUniversalTime().ToString();
            Assert.AreEqual(new DateTime(2002, 02, 02, 09, 00, 23).ToString(), s);
        }

        [Test]
        public void DateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            NpgsqlCommand command = new NpgsqlCommand("set time zone 5;select timestamptz'2009-11-11 20:15:43.019-03:30';", Conn);
            var d = (DateTime) command.ExecuteScalar();
            Assert.AreEqual(DateTimeKind.Local, d.Kind);
            Assert.AreEqual("2009-11-11 23:45:43Z", d.ToUniversalTime().ToString("u"));
        }

        [Test]
        public void DateTimeSupportTimezoneEuropeAmsterdam()
        {
            //1929-08-19 00:00:00+01:19:32
            // This test was provided by Christ Akkermans.
            var command = new NpgsqlCommand("SET TIME ZONE 'Europe/Amsterdam';SELECT '1929-08-19 00:00:00'::timestamptz;", Conn);
            var d = (DateTime) command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateTimeSupport()
        {
            ExecuteNonQuery(@"insert into data (field_timestamp) values ('2002-02-02 09:00:23.345')");
            var command = new NpgsqlCommand("select field_timestamp from data", Conn);

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

            command.CommandText = "insert into data(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", DbType.DateTime));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateTimeSupportNpgsqlDbType()
        {
            ExecuteNonQuery(@"insert into data (field_timestamp) values ('2002-02-02 09:00:23.345')");
            var command = new NpgsqlCommand("select field_timestamp from data", Conn);

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

            command.CommandText = "insert into data(field_timestamp) values (:a);";
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Timestamp));
            command.Parameters[0].Value = ts1;

            command.ExecuteScalar();
        }

        [Test]
        public void ProviderDateSupport()
        {
            ExecuteNonQuery("INSERT INTO data(field_date) VALUES ('2002-03-04')");
            using (var command = new NpgsqlCommand("select field_date from data", Conn))
            {
                NpgsqlDate d;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    d = reader.GetDate(0);
                }

                Assert.AreEqual("2002-03-04", d.ToString());
            }
        }

        [Test]
        public void ProviderTimeSupport()
        {
            ExecuteNonQuery("INSERT INTO data(field_time) VALUES ('10:03:45.345')");
            using (var command = new NpgsqlCommand("select field_time from data", Conn))
            {
                NpgsqlTime t;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    t = reader.GetTime(0);
                }

                Assert.AreEqual("10:03:45.345", t.ToString());
            }
        }

        [Test]
        public void ProviderTimeSupportTimezone()
        {
            using (var command = new NpgsqlCommand("select '13:03:45.001-05'::timetz", Conn))
            {
                NpgsqlTimeTZ t;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    t = reader.GetTimeTZ(0);
                }

                Assert.AreEqual("18:03:45.001", t.AtTimeZone(NpgsqlTimeZone.UTC).LocalTime.ToString());
            }
        }

        [Test]
        public void ProviderDateTimeSupportTimezone()
        {
            ExecuteNonQuery("SET TIME ZONE 5");
            ExecuteNonQuery("INSERT INTO data(field_timestamp_with_timezone) VALUES ('2002-02-02 09:00:23.345Z')");
            using (var command = new NpgsqlCommand("SELECT field_timestamp_with_timezone FROM data", Conn))
            {
                NpgsqlTimeStampTZ ts;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    ts = reader.GetTimeStampTZ(0);
                }

                Assert.AreEqual("2002-02-02 09:00:23.345", ts.AtTimeZone(NpgsqlTimeZone.UTC).ToString());
            }
        }

        [Test]
        public void ProviderDateTimeSupportTimezone3()
        {
            //2009-11-11 20:15:43.019-03:30
            ExecuteNonQuery("SET TIME ZONE 5");
            using (var command = new NpgsqlCommand("select timestamptz'2009-11-11 20:15:43.019-03:30';", Conn))
            {
                NpgsqlTimeStampTZ ts;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    ts = reader.GetTimeStampTZ(0);
                }

                Assert.AreEqual("2009-11-12 04:45:43.019+05", ts.ToString());
            }
        }

        [Test]
        public void ProviderDateTimeSupportTimezone4()
        {
            ExecuteNonQuery("SET TIME ZONE 5"); //Should not be equal to your local time zone !

            NpgsqlTimeStampTZ tsInsert = new NpgsqlTimeStampTZ(2014, 3, 28, 10, 0, 0, NpgsqlTimeZone.UTC);
            
            using (var command = new NpgsqlCommand("INSERT INTO data(field_timestamp_with_timezone) VALUES (:p1)", Conn))
            {
                var p1 = command.Parameters.Add("p1", NpgsqlDbType.TimestampTZ);
                p1.Direction = ParameterDirection.Input;
                p1.Value = tsInsert;
                
                command.ExecuteNonQuery();
            }
            

            using (var command = new NpgsqlCommand("SELECT field_timestamp_with_timezone FROM data", Conn))
            {
                NpgsqlTimeStampTZ tsSelect;
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    tsSelect = reader.GetTimeStampTZ(0);
                }

                Assert.AreEqual(tsInsert.AtTimeZone(NpgsqlTimeZone.UTC), tsSelect.AtTimeZone(NpgsqlTimeZone.UTC));
            }
        }

        [Test]
        public void DoubleValueSupportWithExtendedQuery()
        {
            ExecuteNonQuery("INSERT INTO data(field_float8) VALUES (.123456789012345)");
            using (var command = new NpgsqlCommand("select count(*) from data where field_float8 = :a", Conn))
            {
                command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Double));
                command.Parameters[0].Value = 0.123456789012345D;
                command.Prepare();
                var rows = command.ExecuteScalar();
                Assert.AreEqual(1, rows);
            }
        }

        [Test]
        public void DoubleWithoutPrepared()
        {
            var command = new NpgsqlCommand("select :field_float8", Conn);
            command.Parameters.Add(new NpgsqlParameter(":field_float8", NpgsqlDbType.Double));
            double x = 1d/7d;;
            command.Parameters[0].Value = x;
            var valueReturned = command.ExecuteScalar();
            Assert.That(valueReturned, Is.EqualTo(x).Within(100).Ulps);
            Console.WriteLine("Actual=  {0}", valueReturned);
            Console.WriteLine("Expected={0}", x);
        }

        [Test]
        public void PrecisionScaleNumericSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_numeric) VALUES (-4.3)");

            using (var command = new NpgsqlCommand("SELECT field_numeric FROM data", Conn))
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                var result = dr.GetDecimal(0);
                Assert.AreEqual(-4.3000000M, result);
                //Assert.AreEqual(11, result.Precision);
                //Assert.AreEqual(7, result.Scale);
            }
        }

        [Test]
        public void InsertMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            InsertValue(null, NpgsqlDbType.Timestamp, "field_timestamp", DateTime.MinValue);
        }

        [Test]
        public void InsertMinusInfinityDateTimeSupport()
        {
            InsertValue(DbType.DateTime, null, "field_timestamp", DateTime.MinValue);
        }

        [Test]
        public void InsertInfinityDateTimeSupportNpgsqlDbType()
        {
            InsertValue(null, NpgsqlDbType.Timestamp, "field_timestamp", DateTime.MaxValue);
        }

        [Test]
        [TestCase(DbType.Boolean, null,       "field_bool",    false,    TestName = "Boolean")]
        [TestCase(DbType.Double,  null,       "field_float8",  7.4D,     TestName = "Double")]
        [TestCase(DbType.Single,  null,       "field_float4",  7.4F,     TestName = "Single")]
        [TestCase(DbType.Decimal, null,       "field_numeric", 7.4,      TestName = "Numeric")]
        [TestCase(DbType.Decimal, null,       "field_numeric", -4.3,     TestName = "NegativeNumeric")]
        [TestCase(null, NpgsqlDbType.Numeric, "field_numeric", 7.4,      TestName = "NpsqlNumeric")]
        [TestCase(null, NpgsqlDbType.Double,  "field_float8",  7.4D,     TestName = "NpgsqlDouble")]
        [TestCase(null, NpgsqlDbType.Real,    "field_float4",  7.4F,     TestName = "NpgsqlSingle")]
        [TestCase(null, NpgsqlDbType.Text,    "field_text",    @"\test", TestName = "StringWithBackslashes")]
        [TestCase(null, NpgsqlDbType.Double,  "field_float8",  Double.NaN, TestName = "DoubleNaN")]
        [TestCase(null, NpgsqlDbType.Real,    "field_float4",  Single.NaN, TestName = "SingleNaN")]
        [TestCase(null, NpgsqlDbType.Double,  "field_float8", Double.PositiveInfinity, TestName = "DoubleInfinity")]
        [TestCase(null, NpgsqlDbType.Real,    "field_float4", Single.PositiveInfinity, TestName = "SingleInfinity")]
        public void InsertValue(DbType? dbType, NpgsqlDbType? npgsqlDbType, string fieldName, object value)
        {
            if (dbType.HasValue && npgsqlDbType.HasValue || (!dbType.HasValue && !npgsqlDbType.HasValue))
                Assert.Fail("Exactly one of dbType and npgsqlDbType must be specified");

            using (var command = new NpgsqlCommand(String.Format("INSERT INTO data ({0}) values (:a)", fieldName), Conn))
            {
                if (dbType.HasValue)
                    command.Parameters.Add(new NpgsqlParameter("a", dbType.Value));
                else
                    command.Parameters.Add(new NpgsqlParameter("a", npgsqlDbType.Value));
                command.Parameters[0].Value = value;
                var rowsAdded = command.ExecuteNonQuery();
                Assert.AreEqual(1, rowsAdded);
            }

            var result = ExecuteScalar(String.Format("SELECT {0} FROM data", fieldName));
            Assert.AreEqual(value, result);
        }

        [Test]
        [TestCase(DbType.String,   null,        "field_text",      TestName = "Text")]
        [TestCase(DbType.Boolean,  null,        "field_bool",      TestName = "Boolean")]
        [TestCase(DbType.Decimal,  null,        "field_numeric",   TestName = "Decimal")]
        [TestCase(DbType.Int16,    null,        "field_int2",      TestName = "Int16")]
        [TestCase(DbType.Int32,    null,        "field_int4",      TestName = "Int32")]
        [TestCase(DbType.DateTime, null,        "field_timestamp", TestName = "DateTime")]
        [TestCase(null, NpgsqlDbType.Text,      "field_text",      TestName = "NpgsqlText")]
        [TestCase(null, NpgsqlDbType.Smallint,  "field_int2",      TestName = "NpgsqlSmallInt")]
        [TestCase(null, NpgsqlDbType.Timestamp, "field_timestamp", TestName = "NpgsqlTimestamp")]
        public void InsertNullValue(DbType? dbType, NpgsqlDbType? npgsqlDbType, string fieldName)
        {
            if (dbType.HasValue && npgsqlDbType.HasValue || (!dbType.HasValue && !npgsqlDbType.HasValue))
                Assert.Fail("Exactly one of dbType and npgsqlDbType must be specified");

            using (var command = new NpgsqlCommand(String.Format("INSERT INTO data ({0}) values (:a)", fieldName), Conn))
            {
                if (dbType.HasValue)
                    command.Parameters.Add(new NpgsqlParameter("a", dbType.Value));
                else
                    command.Parameters.Add(new NpgsqlParameter("a", npgsqlDbType.Value));
                command.Parameters[0].Value = DBNull.Value;
                var rowsAdded = command.ExecuteNonQuery();
                Assert.AreEqual(1, rowsAdded);
            }

            var result = ExecuteScalar(String.Format("SELECT COUNT(*) FROM data WHERE {0} IS NULL", fieldName));
            // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
            Assert.AreEqual(1, result);
        }

        [Test]
        public void AnsiStringSupport()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_text) VALUES (:a)", Conn))
            {
                command.Parameters.Add(new NpgsqlParameter("a", DbType.AnsiString));
                command.Parameters[0].Value = "TesteAnsiString";
                var rowsAdded = command.ExecuteNonQuery();
                Assert.AreEqual(1, rowsAdded);

                command.CommandText = String.Format("SELECT COUNT(*) FROM data WHERE field_text = '{0}'",
                                                    command.Parameters[0].Value);
                command.Parameters.Clear();
                var result = command.ExecuteScalar();
                // The missed cast is needed as Server7.2 returns Int32 and Server7.3+ returns Int64
                Assert.AreEqual(1, result);
            }
        }

        [Test]
        public void MultipleQueriesFirstResultsetEmpty()
        {
            var command = new NpgsqlCommand("insert into data(field_text) values ('a'); select count(*) from data;", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(1, result);
        }

        [Test]
        public void AmbiguousFunctionParameterType()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ambiguousParameterType(int2, int4, int8, text, varchar(10), char(5)) returns int4 as '
                                select 4 as result;
                              ' language 'sql'");
            //NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ambiguousParameterType(int2, int4, int8, text, varchar(10), char(5)) returns int4 as '
                                select 4 as result;
                              ' language 'sql'");
            NpgsqlCommand command = new NpgsqlCommand("ambiguousParameterType(:a, :b, :c, :d, :e, :f)", Conn);
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
            const string sql = @"select * from data where field_serial = :a";
            var command = new NpgsqlCommand(sql, Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 2;
            command.ExecuteNonQuery();
        }

        [Test]
        public void TestParameterReplace2()
        {
            const string sql = @"select * from data where field_serial = :a+1";
            var command = new NpgsqlCommand(sql, Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = 1;
            command.ExecuteNonQuery();
        }

        [Test]
        public void TestParameterNameInParameterValue()
        {
            const string sql = "insert into data(field_text, field_int4) values ( :a, :b );";
            const string aValue = "test :b";

            var command = new NpgsqlCommand(sql, Conn);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Text));
            command.Parameters[":a"].Value = aValue;
            command.Parameters.Add(new NpgsqlParameter(":b", NpgsqlDbType.Integer));
            command.Parameters[":b"].Value = 1;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);

            var command2 = new NpgsqlCommand("select field_text, field_int4 from data where field_serial = (select max(field_serial) from data)", Conn);
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
            var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", Conn);
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
            var command = new NpgsqlCommand("select case when (foo is null) then false else foo end as bar from (select :a as foo) as x", Conn);
            var p0 = new NpgsqlParameter(":a", true);
            // not setting parameter type
            command.Parameters.Add(p0);
            command.ExecuteScalar();
        }

        private void TestBoolParameter_Internal(bool prepare)
        {
            // Add test for prepared queries with bool parameter.
            // This test was created based on a report from Andrus Moor in the help forum:
            // http://pgfoundry.org/forum/forum.php?thread_id=15672&forum_id=519&group_id=1000140

            var command = new NpgsqlCommand("select :boolValue", Conn);

            command.Parameters.Add(":boolValue", NpgsqlDbType.Boolean);

            if (prepare)
            {
                command.Prepare();
            }

            command.Parameters["boolvalue"].Value = false;

            Assert.IsFalse((bool)command.ExecuteScalar());

            command.Parameters["boolvalue"].Value = true;

            Assert.IsTrue((bool)command.ExecuteScalar());
        }

        [Test]
        public void TestBoolParameter()
        {
            TestBoolParameter_Internal(false);
        }

        [Test]
        public void TestBoolParameterPrepared()
        {
            TestBoolParameter_Internal(true);
        }

        [Test]
        public void TestBoolParameterPrepared_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                TestBoolParameter_Internal(true);
            }
        }

        [Test]
        [Ignore]
        public void TestBoolParameterPrepared2()
        {
            // will throw exception if bool parameter can't be used as boolean expression
            var command = new NpgsqlCommand("select :boolValue", Conn);
            var p0 = new NpgsqlParameter(":boolValue", false);
            // not setting parameter type
            command.Parameters.Add(p0);
            command.Prepare();

            Assert.IsFalse((bool)command.ExecuteScalar());
        }

        [Test]
        public void TestPointSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_point) VALUES ( '(4, 3)' )");
            var command = new NpgsqlCommand("select field_point from data", Conn);
            var p = (NpgsqlPoint) command.ExecuteScalar();
            Assert.AreEqual(4, p.X);
            Assert.AreEqual(3, p.Y);
        }

        [Test]
        public void TestBoxSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_box) VALUES ( '(4, 3), (5, 4)'::box )");
            var command = new NpgsqlCommand("select field_box from data", Conn);
            var box = (NpgsqlBox) command.ExecuteScalar();
            Assert.AreEqual(5, box.UpperRight.X);
            Assert.AreEqual(4, box.UpperRight.Y);
            Assert.AreEqual(4, box.LowerLeft.X);
            Assert.AreEqual(3, box.LowerLeft.Y);
        }

        [Test]
        public void TestLSegSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_lseg) VALUES ( '(4, 3), (5, 4)'::lseg )");
            var command = new NpgsqlCommand("select field_lseg from data", Conn);
            var lseg = (NpgsqlLSeg) command.ExecuteScalar();
            Assert.AreEqual(4, lseg.Start.X);
            Assert.AreEqual(3, lseg.Start.Y);
            Assert.AreEqual(5, lseg.End.X);
            Assert.AreEqual(4, lseg.End.Y);
        }

        [Test]
        public void TestClosedPathSupport()
        {
            ExecuteNonQuery("INSERT INTO data (field_path) VALUES ( '((4, 3), (5, 4))'::path )");
            var command = new NpgsqlCommand("select field_path from data", Conn);
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
            ExecuteNonQuery("INSERT INTO data (field_path) VALUES ( '[(4, 3), (5, 4)]'::path )");
            var command = new NpgsqlCommand("select field_path from data", Conn);
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
            ExecuteNonQuery("INSERT INTO data (field_polygon) VALUES ( '((4, 3), (5, 4))'::polygon )");
            var command = new NpgsqlCommand("select field_polygon from data", Conn);
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
            ExecuteNonQuery("INSERT INTO data (field_circle) VALUES ( '< (4, 3), 5 >'::circle )");
            var command = new NpgsqlCommand("select field_circle from data", Conn);
            var circle = (NpgsqlCircle) command.ExecuteScalar();
            Assert.AreEqual(4, circle.Center.X);
            Assert.AreEqual(3, circle.Center.Y);
            Assert.AreEqual(5, circle.Radius);
        }

        [Test]
        public void TestXmlParameter()
        {
            TestXmlParameter_Internal(false);
        }

        [Test]
        public void TestXmlParameterPrepared()
        {
            TestXmlParameter_Internal(true);
        }

        
        private void TestXmlParameter_Internal(bool prepare)
        {
            using (var command = new NpgsqlCommand("select @PrecisionXML", Conn))
            {
                var sXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <strings type=\"array\"> <string> this is a test with ' single quote </string></strings>";
                var parameter = command.CreateParameter();
                parameter.DbType = DbType.Xml;  // To make it work we need to use DbType.String; and then CAST it in the sSQL: cast(@PrecisionXML as xml)
                parameter.ParameterName = "@PrecisionXML";
                parameter.Value = sXML;
                command.Parameters.Add(parameter);

                if (prepare)
                    command.Prepare();
                command.ExecuteScalar();
            }

        }


        [Test]
        public void SetParameterValueNull()
        {
            var cmd = new NpgsqlCommand("insert into data(field_bytea) values (:val)", Conn);
            var param = cmd.CreateParameter();
            param.ParameterName = "val";
            param.NpgsqlDbType = NpgsqlDbType.Bytea;
            param.Value = DBNull.Value;
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();

            cmd = new NpgsqlCommand("select field_bytea from data where field_serial = (select max(field_serial) from data)", Conn);
            var result = cmd.ExecuteScalar();
            Assert.AreEqual(DBNull.Value, result);
        }

        [Test]
        public void TestCharParameterLength()
        {
            const string sql = "insert into data(field_char5) values ( :a );";
            const string aValue = "atest";
            var command = new NpgsqlCommand(sql, Conn);
            command.Parameters.Add(new NpgsqlParameter(":a", NpgsqlDbType.Char));
            command.Parameters[":a"].Value = aValue;
            command.Parameters[":a"].Size = 5;
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(rowsAdded, 1);

            var command2 = new NpgsqlCommand("select field_char5 from data where field_serial = (select max(field_serial) from data)", Conn);
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
            ExecuteNonQuery(@"INSERT INTO data (field_int4, field_time) VALUES (2, '10:03:45.345')");
            var command = new NpgsqlCommand("select to_char(field_time, 'HH24:MI') from data where field_int4 = :a;", Conn);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Integer);
            p.Value = 2;
            command.Parameters.Add(p);
            var d = (String) command.ExecuteScalar();
            Assert.AreEqual("10:03", d);
        }

        [Test]
        public void MultipleRefCursorSupport()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testmultcurfunc() RETURNS SETOF refcursor AS 'DECLARE ref1 refcursor; ref2 refcursor; BEGIN OPEN ref1 FOR SELECT 1; RETURN NEXT ref1; OPEN ref2 FOR SELECT 2; RETURN next ref2; RETURN; END;' LANGUAGE 'plpgsql';");
            using (Conn.BeginTransaction())
            {
                var command = new NpgsqlCommand("testmultcurfunc", Conn);
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
        }

        [Test]
        public void ProcedureNameWithSchemaSupport()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnrecord() returns record as 'select 4 ,5' language 'sql'");
            var command = new NpgsqlCommand("public.testreturnrecord", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnrecord() returns record as 'select 4 ,5' language 'sql'");
            var command = new NpgsqlCommand("testreturnrecord", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnsetofrecord() returns setof record as 'values (8,9), (6,7)' language 'sql'");
            var command = new NpgsqlCommand("testreturnsetofrecord", Conn);
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testreturnrecordresultset(x int4, y int4) returns table (a int4, b int4) as
                              $BODY$
                              begin
                              return query
                              select 1, 2;
                              end;
                              $BODY$
                              language 'plpgsql'");
            var command = new NpgsqlCommand("testreturnrecordresultset", Conn);
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
            using (var c = new NpgsqlConnection(ConnectionString))
            {
                c.Open();
                var t = c.BeginTransaction(IsolationLevel.ReadUncommitted);
                Assert.IsNotNull(t);

                var command = new NpgsqlCommand(sql, Conn);
                command.ExecuteReader().Close();
            }
        }

        [Test]
        public void RepeatableReadTransactionSupport()
        {
            const string sql = "select 1 as test";
            using (var c = new NpgsqlConnection(ConnectionString))
            {
                c.Open();
                var t = c.BeginTransaction(IsolationLevel.RepeatableRead);
                Assert.IsNotNull(t);

                var command = new NpgsqlCommand(sql, Conn);
                command.ExecuteReader().Close();
                c.Close();
            }
        }

        [Test]
        public void SetTransactionToSerializable()
        {
            const string sql = "show transaction_isolation;";
            using (var c = new NpgsqlConnection(ConnectionString))
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
            const string sql = "insert into data(field_char5) values ( :a.parameter );";
            const string aValue = "atest";
            var command = new NpgsqlCommand(sql, Conn);
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

            var command2 = new NpgsqlCommand("select field_char5 from data where field_serial = (select max(field_serial) from data)", Conn);
            var a = (String) command2.ExecuteScalar();
            Assert.AreEqual(aValue, a);
        }

        [Test]
        public void LastInsertedOidSupport()
        {
            var insertCommand = new NpgsqlCommand("insert into data(field_text) values ('a');", Conn);
            // Insert this dummy row, just to enable us to see what was the last oid in order we can assert it later.
            insertCommand.ExecuteNonQuery();

            var selectCommand = new NpgsqlCommand("select max(oid) from data;", Conn);
            var previousOid = (Int32) selectCommand.ExecuteScalar();

            insertCommand.ExecuteNonQuery();

            Assert.AreEqual(previousOid + 1, insertCommand.LastInsertedOID);
        }

        /*[Test]
        public void SetServerVersionToNull()
        {
            ServerVersion o = Conn.ServerVersion;
            if(o == null)
              return;
        }*/

        [Test]
        public void VerifyFunctionNameWithDeriveParameters()
        {
            try
            {
                var invalidCommandName = new NpgsqlCommand("invalidfunctionname", Conn);
                NpgsqlCommandBuilder.DeriveParameters(invalidCommandName);
            }
            catch (InvalidOperationException e)
            {
                var expected = string.Format(L10N.InvalidFunctionName, "invalidfunctionname");
                Assert.AreEqual(expected, e.Message);
            }
        }

        [Test]
        public void DoubleSingleQuotesValueSupport()
        {
            var command = new NpgsqlCommand("insert into data(field_text) values (:a)", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", NpgsqlDbType.Text));
            command.Parameters[0].Value = "''";
            var rowsAdded = command.ExecuteNonQuery();
            Assert.AreEqual(1, rowsAdded);

            command.CommandText = "select * from data where field_text = :a";
            using (var dr = command.ExecuteReader())
            {
                Assert.IsTrue(dr.Read());
            }
        }

        [Test]
        public void ReturnInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_timestamp) values ('infinity'::timestamp);", Conn);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from data where field_serial = (select max(field_serial) from data);", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        [Test]
        public void ReturnMinusInfinityDateTimeSupportNpgsqlDbType()
        {
            var command = new NpgsqlCommand("insert into data(field_timestamp) values ('-infinity'::timestamp);", Conn);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_timestamp from data where field_serial = (select max(field_serial) from data);", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void MinusInfinityDateTimeSupport()
        {
            var command = Conn.CreateCommand();
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MinValue));
            command.CommandText = "select 1 where current_date=:p0";
            var result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void PlusInfinityDateTimeSupport()
        {
            var command = Conn.CreateCommand();
            command.Parameters.Add(new NpgsqlParameter("p0", DateTime.MaxValue));
            command.CommandText = "select 1 where current_date=:p0";
            var result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void InetTypeSupport()
        {
            var command = new NpgsqlCommand("INSERT INTO data(field_inet) VALUES (:a);", Conn);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Inet);
            p.Value = new NpgsqlInet("127.0.0.1");
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("SELECT field_inet FROM data WHERE field_serial = (SELECT(field_serial) FROM data)", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual((IPAddress) new NpgsqlInet("127.0.0.1"), (IPAddress) result);
        }

        [Test]
        public void IPAddressTypeSupport()
        {
            var command = new NpgsqlCommand("insert into data(field_inet) values (:a);", Conn);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Inet);
            p.Value = IPAddress.Parse("127.0.0.1");
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_inet from data where field_serial = (select max(field_serial) from data);", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(IPAddress.Parse("127.0.0.1"), result);
        }

        [Test]
        public void BitTypeSupportWithPrepare()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = true;
                command.Parameters.Add(p);
                command.Prepare();
                command.ExecuteNonQuery();
            }

            Assert.IsTrue((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data)"));
        }

        [Test]
        public void BitTypeSupport()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = true;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }

            Assert.IsTrue((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data)"));
        }

        [Test]
        public void BitTypeSupport2()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = 3;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }

            Assert.IsTrue((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data);"));
        }

        [Test]
        public void BitTypeSupport3()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = 6;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }

            Assert.IsFalse((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data)"));
        }

        //[Test]
        public void FunctionReceiveCharParameter()
        {
            var command = new NpgsqlCommand("d/;", Conn);
            var p = new NpgsqlParameter("a", NpgsqlDbType.Inet);
            p.Value = IPAddress.Parse("127.0.0.1");
            command.Parameters.Add(p);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("select field_inet from data where field_serial = (select max(field_serial) from data);", Conn);
            var result = command.ExecuteScalar();
            Assert.AreEqual(new NpgsqlInet("127.0.0.1"), result);
        }

        [Test]
        public void FunctionCaseSensitiveName()
        {
            CreateCaseSensitiveFunction();
            var command = new NpgsqlCommand("\"FunctionCaseSensitive\"", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            var result = command.ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FunctionCaseSensitiveNameWithSchema()
        {
            CreateCaseSensitiveFunction();
            var command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", Conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Integer));
            command.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.Text));
            var result = command.ExecuteScalar();
            Assert.AreEqual(0, result);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParameters()
        {
            var command = new NpgsqlCommand("\"FunctionCaseSensitive\"", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        [Test]
        public void FunctionCaseSensitiveNameDeriveParametersWithSchema()
        {
            CreateCaseSensitiveFunction();
            var command = new NpgsqlCommand("\"public\".\"FunctionCaseSensitive\"", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }

        void CreateCaseSensitiveFunction()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION ""FunctionCaseSensitive""(int4, text) returns int4 as
                              $BODY$
                              begin
                                return 0;
                              end
                              $BODY$
                              language 'plpgsql';");
        }

        [Test]
        public void CaseSensitiveParameterNames()
        {
            var command = new NpgsqlCommand("select :p1", Conn);
            command.Parameters.Add(new NpgsqlParameter("P1", NpgsqlDbType.Integer)).Value = 5;
            var result = command.ExecuteScalar();
            Assert.AreEqual(5, result);
        }

        [Test]
        public void FunctionTestTimestamptzParameterSupport()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('Y')");
            using (Conn.BeginTransaction())
            {
                ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testtimestamptzparameter(timestamptz) returns refcursor as
                                  $BODY$
                                  declare ref refcursor;
                                  begin
                                          open ref for select * from data;
                                          return ref;
                                  end
                                  $BODY$
                                  language 'plpgsql' volatile called on null input security invoker;");
                var command = new NpgsqlCommand("testtimestamptzparameter", Conn);
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
        }

        [Test]
        public void LessThanParamNoWhitespaceBetween()
        {
            OperatorParamNoWhitespaceBetween("<", false);
        }

        [Test]
        public void LessThanParamNoWhitespaceBetweenWithPrepare()
        {
            OperatorParamNoWhitespaceBetween("<", true);
        }

        [Test]
        public void GreaterThanParamNoWhitespaceBetween()
        {
            OperatorParamNoWhitespaceBetween(">", false);
        }

        [Test]
        public void GreaterThanParamNoWhitespaceBetweenWithPrepare()
        {
            OperatorParamNoWhitespaceBetween(">", true);
        }

        [Test]
        public void NotEqualThanParamNoWhitespaceBetween()
        {
            OperatorParamNoWhitespaceBetween("<>", false);
        }

        [Test]
        public void NotEqualThanParamNoWhitespaceBetweenWithPrepare()
        {
            OperatorParamNoWhitespaceBetween("<>", true);
        }

        private void OperatorParamNoWhitespaceBetween(string op, bool prepare)
        {
            var command = new NpgsqlCommand(string.Format("select 1{0}:param1", op), Conn);
            command.Parameters.AddWithValue(":param1", 1);

            if (prepare)
            {
                command.Prepare();
            }

            command.ExecuteScalar();
        }

        [Test]
        public void CharParameterValueSupport()
        {
            const String query = @"create temp table test ( tc char(1) );
                                   insert into test values(' ');
                                   select * from test where tc=:charparam";
            var command = new NpgsqlCommand(query, Conn);
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
            using (var conn = new NpgsqlConnection(ConnectionString + ";CommandTimeout=180;pooling=false"))
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

            using (NpgsqlCommand cmd = new NpgsqlCommand("select a, max(b) from (select :param as a, 1 as b) x group by a", Conn))
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
        [Ignore]
        public void ParameterExplicitType2()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_date=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.Value = "2008-1-1";
                //sqlParam.DbType = DbType.Object;
                command.Parameters.Add(sqlParam);
                command.ExecuteScalar();
            }
        }

        [Test]
        public void ParameterExplicitType2DbTypeObject()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_date=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.Value = "2008-1-1";
                sqlParam.DbType = DbType.Object;
                command.Parameters.Add(sqlParam);
                command.ExecuteScalar();
            }
        }

        [Test]
        public void ParameterExplicitType2DbTypeObjectTypeFirst()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_date=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.DbType = DbType.Object;
                sqlParam.Value = "2008-1-1";
                command.Parameters.Add(sqlParam);
                command.ExecuteScalar();
            }
        }

        [Test]
        public void ParameterExplicitType2DbTypeObjectWithPrepare()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_date=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.Value = "2008-1-1";
                sqlParam.DbType = DbType.Object;
                command.Parameters.Add(sqlParam);
                command.Prepare();
                command.ExecuteScalar();
            }
        }

        [Test]
        public void ParameterExplicitType2DbTypeObjectWithPrepareTypeFirst()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_date=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.DbType = DbType.Object;
                sqlParam.Value = "2008-1-1";
                command.Parameters.Add(sqlParam);
                command.Prepare();
                command.ExecuteScalar();
            }
        }

        [Test]
        public void ParameterExplicitType2DbTypeObjectWithPrepare2()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_date=:param or field_date=:param2", Conn))
            {
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
        }

        [Test]
        public void ParameterExplicitType2DbTypeObjectInt()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_int4=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.DbType = DbType.Object;
                sqlParam.Value = 1;
                command.Parameters.Add(sqlParam);
                command.ExecuteScalar();
            }
        }

        [Test]
        public void ParameterExplicitType2DbTypeObjectIntTypeFirst()
        {
            using (var command = new NpgsqlCommand(@"SELECT * FROM data WHERE field_int4=:param", Conn))
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = "param";
                sqlParam.DbType = DbType.Object;
                sqlParam.Value = 1;
                command.Parameters.Add(sqlParam);
                command.ExecuteScalar();
            }
        }

        [Test]
        public void Int32WithoutQuotesPolygon()
        {
            var a = new NpgsqlCommand("select 'polygon ((:a :b))' ", Conn);
            a.Parameters.Add(new NpgsqlParameter("a", 1));
            a.Parameters.Add(new NpgsqlParameter("b", 1));
            a.ExecuteScalar();
        }

        [Test]
        public void Int32WithoutQuotesPolygon2()
        {
            var a = new NpgsqlCommand("select 'polygon ((:a :b))' ", Conn);
            a.Parameters.Add(new NpgsqlParameter("a", 1)).DbType = DbType.Int32;
            a.Parameters.Add(new NpgsqlParameter("b", 1)).DbType = DbType.Int32;
            a.ExecuteScalar();
        }

        private void MoneyHandlingInternal(bool prepare)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                decimal inVal = 12345.12m;
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Money);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                if (prepare)
                {
                    cmd.Prepare();
                }

                var retVal = (decimal) cmd.ExecuteScalar();
                Assert.AreEqual(inVal, retVal);
            }
        }

        [Test]
        public void MoneyHandling()
        {
            MoneyHandlingInternal(false);
        }

        [Test]
        public void MoneyHandlingPrepared()
        {
            MoneyHandlingInternal(true);
        }

        private void TimeStampHandlingInternal(bool prepare)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                DateTime inVal = DateTime.Parse("02/28/2000 02:20:20 PM", CultureInfo.InvariantCulture);
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Timestamp);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                if (prepare)
                {
                    cmd.Prepare();
                }

                var retVal = (DateTime) cmd.ExecuteScalar();
                Assert.AreEqual(inVal, retVal);
            }
        }

        [Test]
        public void TimeStampHandling()
        {
            TimeStampHandlingInternal(false);
        }

        [Test]
        public void TimeStampHandlingPrepared()
        {
            TimeStampHandlingInternal(true);
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
            var command = new NpgsqlCommand(createTable, Conn);
            command.ExecuteNonQuery();

            NpgsqlParameter uuidDbParam = new NpgsqlParameter(":param1", NpgsqlDbType.Uuid);
            uuidDbParam.Value = Guid.NewGuid();

            command = new NpgsqlCommand(@"INSERT INTO person (person_uuid) VALUES (:param1);", Conn);
            command.Parameters.Add(uuidDbParam);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("SELECT person_uuid::uuid FROM person LIMIT 1", Conn);
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

            var command = new NpgsqlCommand(createFunction, Conn);
            command.ExecuteNonQuery();

            command = new NpgsqlCommand("more_params", Conn);
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
            const String theSavePoint = "theSavePoint";

            using (var transaction = Conn.BeginTransaction())
            {
                transaction.Save(theSavePoint);

                ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('savepointtest')");
                var result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(1, result);

                transaction.Rollback(theSavePoint);

                result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(0, result);
            }
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestSavePointWithSemicolon()
        {
            const String theSavePoint = "theSavePoint;";

            using (var transaction = Conn.BeginTransaction())
            {
                transaction.Save(theSavePoint);

                ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('savepointtest')");
                var result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(1, result);

                transaction.Rollback(theSavePoint);

                result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(0, result);
            }
        }

        [Test]
        public void TestPreparedStatementParameterCastIsNotAdded()
        {
            // Test by Waldemar Bergstreiser

            var cmd = new NpgsqlCommand("select field_int4 from data where :p0 is null or field_int4 = :p0 ", Conn);

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
            var cmd = new NpgsqlCommand("sele", Conn);
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
            $$ LANGUAGE plpgsql;", Conn).ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand("NullTest", Conn))
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
            $$ LANGUAGE plpgsql;", Conn).ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand("NullTest", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new Double[] {};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                var retVal = (Double[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
            }
        }

        private void DecimalArrayHandlingInternal(bool prepare)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] { 1d, 1000d, 1000000d };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Numeric | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                if (prepare)
                {
                    cmd.Prepare();
                }

                var retVal = (decimal[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void DecimalArrayHandling()
        {
            DecimalArrayHandlingInternal(false);
        }

        [Test]
        public void DecimalArrayHandlingPrepared()
        {
            DecimalArrayHandlingInternal(true);
        }

        [Test]
        public void TextArrayHandling()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] {"Array element", "Array element with a '", "Array element with a \"", "Array element with a ,", "Array element with a \\"};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Text | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                var retVal = (string[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        private void TextArrayHandlingPreparedInternal()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] {"Array element", "Array element with a '", "Array element with a \"", "Array element with a ,", "Array element with a \\"};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Text | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (string[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void TextArrayHandlingPrepared()
        {
            TextArrayHandlingPreparedInternal();
        }

        [Test]
        public void TextArrayHandlingPrepared_SuppressBinary()
        {
            using (this.SuppressBackendBinary())
            {
                TextArrayHandlingPreparedInternal();
            }
        }

        private void IntArrayHandlingPreparedInternal()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] {1, 2, 3, 0xFE, 0xFD, 0xFC};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Integer | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (int[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void IntArrayHandlingPrepared()
        {
            IntArrayHandlingPreparedInternal();
        }

        [Test]
        public void IntArrayHandlingPrepared_SuppressBinary()
        {
            using (this.SuppressBackendBinary())
            {
                IntArrayHandlingPreparedInternal();
            }
        }

        private void DoubleArrayHandlingPreparedInternal()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] {12345.12345d, 98765.98765d};
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
        public void DoubleArrayHandlingPrepared()
        {
            DoubleArrayHandlingPreparedInternal();
        }

        [Test]
        public void DoubleArrayHandlingPrepared_SuppressBinary()
        {
            using (this.SuppressBackendBinary())
            {
                DoubleArrayHandlingPreparedInternal();
            }
        }

        [Test]
        public void DoubleArrayHandlingZeroItemPrepared()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
        public void DoubleArrayHandlingValues([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] { double.NaN, 12345.12345d, double.PositiveInfinity, double.NegativeInfinity };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                if (prepareCommand)
                    cmd.Prepare();

                var retVal = (Double[])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void SingleArrayHandlingValues([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] { float.NaN, 12345.12345f, float.PositiveInfinity, float.NegativeInfinity };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Real | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                if (prepareCommand)
                    cmd.Prepare();

                var retVal = (float[])cmd.ExecuteScalar();
                Assert.That(retVal, Is.EqualTo(inVal).Within(100).Ulps);
            }
        }


        [Test]
        public void ByteaArrayHandling()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var bytes = new byte[] {1,2,3,4,5,34,39,48,49,50,51,52,92,127,128,255,254,253,252,251};
                var inVal = new[] {bytes, bytes};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                var retVal = (byte[][])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        // Type coersion in the native to backend converters does not work so
        // well for arrays.  The rules for type coersion of array elements
        // does not work the same as for non-arrays.  This test demonstrates
        // the problem.
        private void DateTimeArrayHandlingInternal(bool prepare)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var inVal = new[] { DateTime.Now, DateTime.Now.AddDays(7) };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Timestamp | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                if (prepare)
                {
                    cmd.Prepare();
                }

                var retVal = (DateTime[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        [Ignore]
        public void DateTimeArrayHandling()
        {
            DateTimeArrayHandlingInternal(false);
        }

        [Test]
        [Ignore]
        public void DateTimeArrayHandlingPrepared()
        {
            DateTimeArrayHandlingInternal(true);
        }

        private void ByteaArrayHandlingPreparedInternal()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                var bytes = new byte[] {1,2,3,4,5,34,39,48,49,50,51,52,92,127,128,255,254,253,252,251};
                var inVal = new[] {bytes, bytes};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (byte[][]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void ByteaArrayHandlingPrepared()
        {
            ByteaArrayHandlingPreparedInternal();
        }

        [Test]
        public void ByteaArrayHandlingPrepared_SuppressBinary()
        {
            using (this.SuppressBackendBinary())
            {
                ByteaArrayHandlingPreparedInternal();
            }
        }

        private void MoneyArrayHandlingInternal(bool prepare)
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
            {
                decimal[] inVal = new[] {12345.12m, 98765.98m};
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Money | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);

                if (prepare)
                {
                    cmd.Prepare();
                }

                var retVal = (decimal[]) cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void MoneyArrayHandling()
        {
            MoneyArrayHandlingInternal(false);
        }

        [Test]
        public void MoneyArrayHandlingPrepared()
        {
            MoneyArrayHandlingInternal(true);
        }

        [Test]
        public void Bug1010521NpgsqlIntervalShouldBeQuoted()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            var command = new NpgsqlCommand("select :a", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = Int16.MinValue;
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int16.MinValue, result);
        }

        [Test]
        public void HandleInt32MinValueParameter()
        {
            var command = new NpgsqlCommand("select :a", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = Int32.MinValue;
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int32.MinValue, result);
        }

        [Test]
        public void HandleInt64MinValueParameter()
        {
            var command = new NpgsqlCommand("select :a", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int64));
            command.Parameters[0].Value = Int64.MinValue;
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int64.MinValue, result);
        }

        [Test]
        public void HandleInt16MinValueParameterPrepared()
        {
            var command = new NpgsqlCommand("select :a", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int16));
            command.Parameters[0].Value = Int16.MinValue;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int16.MinValue, result);
        }

        [Test]
        public void HandleInt32MinValueParameterPrepared()
        {
            var command = new NpgsqlCommand("select :a", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters[0].Value = Int32.MinValue;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int32.MinValue, result);
        }

        [Test]
        public void HandleInt64MinValueParameterPrepared()
        {
            var command = new NpgsqlCommand("select :a", Conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int64));
            command.Parameters[0].Value = Int64.MinValue;
            command.Prepare();
            var result = command.ExecuteScalar();
            Assert.AreEqual(Int64.MinValue, result);
        }

        [Test]
        public void Bug1010557BackslashGetDoubled()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            using (var cmd = new NpgsqlCommand("select :p1", Conn))
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
            var cmd = Conn.CreateCommand();
            int? i = null;
            cmd.Parameters.Add(new NpgsqlParameter("p0", i));
            cmd.CommandText = "select :p0 is null or :p0=0 ";
            cmd.ExecuteScalar();
        }

        [Test]
        public void PreparedStatementWithParametersWithSize()
        {
            using (var cmd = new NpgsqlCommand("select :p0, :p1;", Conn))
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
            cmd.Connection = Conn;
            Assert.AreEqual(Int32.MaxValue, cmd.CommandTimeout);
        }

        [Test]
        public void SelectInfinityValueDateDataType()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_date) VALUES ('-infinity'::date)");
            using (var cmd = new NpgsqlCommand(@"SELECT field_date FROM data", Conn))
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
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testoutparameter2(x int, y int, out sum int, out product int) as 'select $1 + $2, $1 * $2' language 'sql';");
            var command = new NpgsqlCommand("testoutparameter2", Conn);
            command.CommandType = CommandType.StoredProcedure;
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(":x", command.Parameters[0].ParameterName);
            Assert.AreEqual(":y", command.Parameters[1].ParameterName);
        }

        [Test]
        public void NegativeMoneySupport()
        {
            var command = new NpgsqlCommand("select '-10.5'::money", Conn);
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
            var command = new NpgsqlCommand("select :moneyvalue", Conn);
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
            var command = new NpgsqlCommand("select field_bytea from data where field_bytea = :bytesData", Conn);
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

            var command = new NpgsqlCommand("select '192.168.10.10'::inet;", Conn);
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
                cmd.Connection = Conn;

                var paramBLOB = new NpgsqlParameter();
                paramBLOB.ParameterName = "BLOB";
                cmd.Parameters.Add(paramBLOB);
                cmd.Parameters[0].Value = DBNull.Value;
                Assert.AreEqual(DbType.Object, cmd.Parameters[0].DbType);
                Assert.AreEqual(NpgsqlDbType.Unknown, cmd.Parameters[0].NpgsqlDbType);

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

            var command = new NpgsqlCommand("select '2010-01-17 15:45'::timestamp;", Conn);
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
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            try//the next command will fail on earlier postgres versions, but that is not a bug in itself.
            {
                new NpgsqlCommand("set standard_conforming_strings=off", conn).ExecuteNonQuery();
            }
            catch{}

            using (conn = new NpgsqlConnection(ConnectionString)) {
                conn.Open();
                try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
                {
                    new NpgsqlCommand("set standard_conforming_strings=off", conn).ExecuteNonQuery();
                }
                catch
                {
                }

                using (var command = new NpgsqlCommand("SELECT :dummy, pg_sleep(3)", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("dummy", NpgsqlDbType.Text));
                    command.Parameters[0].Value = "foo";
                    command.CommandTimeout = 1;
                    try
                    {
                        command.ExecuteNonQuery();
                        Assert.Fail("3s command survived a 1s timeout");
                    }
                    catch (NpgsqlException)
                    {
                        // We cannot currently identify that the exception was a timeout
                        // in a locale-independent fashion, so just assume so.
                    }
                }

                using (var command = new NpgsqlCommand("SELECT :dummy, pg_sleep(3)", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("dummy", NpgsqlDbType.Text));
                    command.Parameters[0].Value = "foo";
                    command.CommandTimeout = 4;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (NpgsqlException)
                    {
                        // We cannot currently identify that the exception was a timeout
                        // in a locale-independent fashion, so just assume so.
                        throw new Exception("3s command did NOT survive a 4s timeout");
                    }
                }
            }
        }

        [Test]
        public void TimeoutFirstFunctionCall()
        {
            // Function calls entail internal queries; verify that they do not
            // sabotage the requested timeout.

            using (var conn = new NpgsqlConnection(ConnectionString)) {
                conn.Open();

                var command = new NpgsqlCommand("pg_sleep", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new NpgsqlParameter());
                command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Double;
                command.Parameters[0].Value = 3d;
                command.CommandTimeout = 1;
                try
                {
                    command.ExecuteNonQuery();
                    Assert.Fail("3s function call survived a 1s timeout");
                }
                catch (NpgsqlException)
                {
                    // We cannot currently identify that the exception was a timeout
                    // in a locale-independent fashion, so just assume so.
                }
            }

            using (var conn = new NpgsqlConnection(ConnectionString)) {
                conn.Open();

                var command = new NpgsqlCommand("pg_sleep", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new NpgsqlParameter());
                command.Parameters[0].NpgsqlDbType = NpgsqlDbType.Double;
                command.Parameters[0].Value = 3d;
                command.CommandTimeout = 4;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (NpgsqlException)
                {
                    // We cannot currently identify that the exception was a timeout
                    // in a locale-independent fashion, so just assume so.
                    throw new Exception("3s command did NOT survive a 4s timeout");
                }
            }
        }

        [Test]
        public void Bug1010788UpdateRowSource()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("select * from data", conn);
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
            var command = new NpgsqlCommand("funcb", Conn);
            NpgsqlCommandBuilder.DeriveParameters(command);
        }

        [Test]
        public void DataTypeTests()
        {
            // Test all types according to this table:
            // http://www.postgresql.org/docs/9.1/static/datatype.html

            // bigint
            var cmd = new NpgsqlCommand("select 1::bigint", Conn);
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
            cmd.CommandText = "select 'true'::boolean";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Boolean), result.GetType());

            // boolean
            cmd.CommandText = "select 1::boolean";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Boolean), result.GetType());

            // box
            cmd.CommandText = "select '((7,4),(8,3))'::box";
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (NpgsqlBox), result.GetType());

            // bytea
            cmd.CommandText = string.Format(@"SELECT {0}'\{1}xDEADBEEF'::bytea;", Conn.UseConformantStrings ? "" : "E", Conn.UseConformantStrings ? "" : @"\");
            result = cmd.ExecuteScalar();
            Assert.AreEqual(typeof (Byte[]), result.GetType());

            // bytea
            cmd.CommandText = string.Format(@"SELECT {0}'\{1}001\{1}002\{1}003\{1}377\{1}376\{1}375'::bytea;", ! Conn.UseConformantStrings ? "E" : "", Conn.UseConformantStrings ? "" : @"\");
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

        [Test]
        // Target NpgsqlCommand.AppendCommandReplacingParameterValues()'s handling of operator @@.
        public void Operator_At_At_RewriteTest()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT to_tsvector('fat cats ate rats') @@ to_tsquery('cat & rat')", Conn);

            Assert.IsTrue((bool)cmd.ExecuteScalar());
        }

        [Test]
        // Target NpgsqlCommand.AppendCommandReplacingParameterValues()'s handling of operator @>.
        public void Operator_At_GT_RewriteTest()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT 'cat'::tsquery @> 'cat & rat'::tsquery", Conn);

            Assert.IsFalse((bool)cmd.ExecuteScalar());
        }

        [Test]
        // Target NpgsqlCommand.AppendCommandReplacingParameterValues()'s handling of operator <@.
        public void Operator_LT_At_RewriteTest()
        {
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT 'cat'::tsquery <@ 'cat & rat'::tsquery", Conn);

            Assert.IsTrue((bool)cmd.ExecuteScalar());
        }

        [Test]
        public void Bug184RollbackFailsOnAbortedTransaction()
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            csb.CommandTimeout = 100000;

            using (NpgsqlConnection connTimeoutChanged = new NpgsqlConnection(csb.ToString()))
            {
                connTimeoutChanged.Open();
                using (var t = connTimeoutChanged.BeginTransaction())
                {
                    try
                    {
                        var command = new NpgsqlCommand("select count(*) from dta", connTimeoutChanged);
                        command.Transaction = t;
                        Object result = command.ExecuteScalar();


                    }
                    catch (Exception)
                    {

                        t.Rollback();
                    }

                }
            }
        }

        [Test, Description("If a custom command timeout is set, a failed transaction could not be rollbacked to a previous savepoint")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/363")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/184")]
        public void FailedTransactionCantRollbackToSavepointWithCustomTimeout()
        {
            var transaction = Conn.BeginTransaction();
            transaction.Save("TestSavePoint");

            using (var command = new NpgsqlCommand("SELECT unknown_thing", Conn))
            {
                command.CommandTimeout = 1;
                try
                {
                    command.ExecuteScalar();
                }
                catch (NpgsqlException)
                {
                    transaction.Rollback("TestSavePoint");
                }
            }
        }

        [Test]
        public void TestCommentInQuery01()
        {
            using (var command = new NpgsqlCommand("-- 1\n-- 2; abc\n-- 3;", Conn))
            {
                Assert.AreEqual(null, command.ExecuteScalar());
            }
        }

        [Test]
        public void TestCommentInQuery02()
        {
            using (var command = new NpgsqlCommand("select -- lc;lc\r\n1", Conn))
            {
                Assert.AreEqual(1, command.ExecuteScalar());
            }
        }

        [Test]
        public void TestCommentInQuery03()
        {
            using (var command = new NpgsqlCommand("select -- lc;lc /* lc;lc */\r\n1", Conn))
            {
                Assert.AreEqual(1, command.ExecuteScalar());
            }
        }

        [Test]
        public void TestIEnumerableAsArray()
        {
            using (var command = new NpgsqlCommand("SELECT :array", Conn))
            {
                var expected = new[] { 1, 2, 3, 4 };
                command.Parameters.AddWithValue("array", expected.Select(x => x));
                var res = command.ExecuteScalar() as int[];

                Assert.NotNull(res);
                CollectionAssert.AreEqual(expected, res);
            }
        }

        [Test]
        [MinPgVersion(9, 2, 0, "json data type not yet introduced")]
        public void InsertJsonValueDataType()
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_json) VALUES (:param)", Conn))
            {
                cmd.Parameters.AddWithValue("param", @"{ ""Key"" : ""Value"" }");
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Json;
                Assert.That(cmd.ExecuteNonQuery(), Is.EqualTo(1));
            }
        }

        [Test]
        [MinPgVersion(9, 4, 0, "jsonb data type not yet introduced")]
        public void InsertJsonbValueDataType()
        {
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_jsonb) VALUES (:param)", Conn))
            {
                cmd.Parameters.AddWithValue("param", @"{ ""Key"" : ""Value"" }");
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Jsonb;
                Assert.That(cmd.ExecuteNonQuery(), Is.EqualTo(1));
            }
        }

        [Test]
        [MinPgVersion(9, 1, 0, "hstore data type not yet introduced")]
        public void InsertHstoreValueDataType()
        {
            CreateSchema("hstore");
            ExecuteNonQuery(@"CREATE EXTENSION IF NOT EXISTS hstore WITH SCHEMA hstore");
            ExecuteNonQuery(@"ALTER TABLE data DROP COLUMN IF EXISTS field_hstore");
            try
            {
                ExecuteNonQuery(@"ALTER TABLE data ADD COLUMN field_hstore hstore.HSTORE");
            }
            catch (NpgsqlException e)
            {
                if (e.Code == "42704")
                    TestUtil.Inconclusive("HSTORE does not seem to be installed at the backend");
            }

            ExecuteNonQuery(@"SET search_path = public, hstore");
            using (var cmd = new NpgsqlCommand("INSERT INTO data (field_hstore) VALUES (:param)", Conn))
            {
                cmd.Parameters.AddWithValue("param", @"""a"" => 3, ""b"" => 4");
                cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Hstore;
                Assert.That(cmd.ExecuteNonQuery(), Is.EqualTo(1));
            }
        }

        [Test]
        public void Bugs_240_and_296()
        {
            // Query from bug #240 (modified):
            // Original: @"INSERT INTO TestTable (StringColumn, ByteaColumn) VALUES ('b''la', @SomeValue)"
            Excecute_Bugs_240_and_296_query(@"SELECT 'b''la', @p");
            //              Query processing breaks here ^

            // Query from bug #296 (line breaks removed):
            Excecute_Bugs_240_and_296_query(@"SELECT 1 WHERE ''= 'type(''m.response'')#''O''%' AND :p");
            //                              Query processing breaks here ^

            // Simplified query used to find the root cause of the bug:
            Excecute_Bugs_240_and_296_query(@"SELECT '''a' || :p");
            //             Query processing breaks here ^
        }

        private void Excecute_Bugs_240_and_296_query(string query)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("p", DBNull.Value);

                // syntax error at or near ":"
                cmd.ExecuteReader().Dispose();
            }
        }

        [Test]
        public void ParameterSubstitutionLexerTest()
        {
            using (var r = PSLT(@"SELECT :str, :int, :null"))
            {
                Assert.AreEqual("string", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
                Assert.IsTrue(r.IsDBNull(2));
            }
            using (var r = PSLT(@"SELECT e'ab\'c:str', :int"))
            {
                Assert.AreEqual("ab'c:str", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
            }
            using (var r = PSLT(@"SELECT E'a\'b'
-- a comment here :str)'
'c\'d:str', :int, E''
'\':str', :int"))
            {
                Assert.AreEqual("a'bc'd:str", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
                Assert.AreEqual("':str", r.GetString(2));
                Assert.AreEqual(123, r.GetInt32(3));
            }
            using (var r = PSLT(@"SELECT 'abc'::text, :text, 246/:int, 122<@int, (ARRAY[1,2,3,4])[1:@int-121]::text, (ARRAY[1,2,3,4])[1: :int-121]::text, (ARRAY[1,2,3,4])[1:two]::text FROM (SELECT 2 AS two) AS a"))
            {
                Assert.AreEqual("abc", r.GetString(0));
                Assert.AreEqual("tt", r.GetString(1));
                Assert.AreEqual(2, r.GetInt32(2));
                Assert.IsTrue(r.GetBoolean(3));
                Assert.AreEqual("{1,2}", r.GetString(4));
                Assert.AreEqual("{1,2}", r.GetString(5));
                Assert.AreEqual("{1,2}", r.GetString(6));
            }
            using (var r = PSLT("SELECT/*/* -- nested comment :int /*/* *//*/ **/*/*/*/:str"))
            {
                Assert.AreEqual("string", r.GetString(0));
            }
            using (var r = PSLT("SELECT--comment\r:str"))
            {
                Assert.AreEqual("string", r.GetString(0));
            }
            using (var r = PSLT("SELECT $\u00ffabc0$literal string :str :int$\u00ffabc0 $\u00ffabc0$, :int, $$:str$$"))
            {
                Assert.AreEqual("literal string :str :int$\u00ffabc0 ", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
                Assert.AreEqual(":str", r.GetString(2));
            }
            if (!Conn.UseConformantStrings)
            {
                using (var r = PSLT(@"SELECT 'abc\':str''a:str', :int"))
                {
                    Assert.AreEqual("abc':str'a:str", r.GetString(0));
                    Assert.AreEqual(123, r.GetInt32(1));
                }
            }
            else
            {
                using (var r = PSLT(@"SELECT 'abc'':str''a:str', :int"))
                {
                    Assert.AreEqual("abc':str'a:str", r.GetString(0));
                    Assert.AreEqual(123, r.GetInt32(1));
                }
            }

            // Don't touch output parameters
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT (ARRAY[1,2,3])[1:abc]::text AS abc FROM (SELECT 2 AS abc) AS a";
                var param = new NpgsqlParameter { Direction = ParameterDirection.Output, DbType = DbType.String, ParameterName = "abc" };
                cmd.Parameters.Add(param);
                using (var r = cmd.ExecuteReader())
                {
                    r.Read();
                    Assert.AreEqual("{1,2}", param.Value);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(Npgsql.NpgsqlException), ExpectedMessage="ERROR: 42P01: relation \":str\" does not exist")]
        public void ParameterSubstitutionLexerTestDoubleQuoted()
        {
            using (var r = PSLT("SELECT 1 FROM \":str\""))
            {
            }
        }

        private NpgsqlDataReader PSLT(string query)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("str", "string");
                cmd.Parameters.AddWithValue("int", 123);
                cmd.Parameters.AddWithValue("text", "tt");
                cmd.Parameters.AddWithValue("null", DBNull.Value);

                // syntax error at or near ":"
                var rdr = cmd.ExecuteReader();
                Assert.IsTrue(rdr.Read());
                return rdr;
            }
        }

        [Test]
        public void TableDirect()
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "(select 1) as a; (select 1) as b;";
                cmd.CommandType = CommandType.TableDirect;
                using (var rdr = cmd.ExecuteReader())
                {
                    do
                    {
                        rdr.Read();
                        Assert.AreEqual(rdr.GetInt32(0), 1);
                    } while (rdr.NextResult());
                }
            }
        }

        [Test]
        public void InputAndOutputParameters([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select :a + 2 as b, :c - 1 as c";
                var b = new NpgsqlParameter() { ParameterName = "b", Direction = ParameterDirection.Output };
                cmd.Parameters.Add(b);
                cmd.Parameters.Add(new NpgsqlParameter("a", 3));
                var c = new NpgsqlParameter() { ParameterName = "c", Direction = ParameterDirection.InputOutput, Value = 4 };
                cmd.Parameters.Add(c);
                if (prepareCommand)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(5, b.Value);
                    Assert.AreEqual(3, c.Value);
                }
            }
        }

        [Test]
        public void BitString([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select :bs1 as output, :bs2, :bs3, :bs4, :bs5, array [1::bit, 0::bit], array [bit '10', bit '01'], :ba1, :ba2, :ba3";
                var output = new NpgsqlParameter() { ParameterName = "output", Direction = ParameterDirection.Output };
                cmd.Parameters.Add(output);
                cmd.Parameters.Add(new NpgsqlParameter("bs1", NpgsqlDbType.Bit) { Value = new BitString("1011") });
                cmd.Parameters.Add(new NpgsqlParameter("bs2", NpgsqlDbType.Bit, 1) { Value = true });
                cmd.Parameters.Add(new NpgsqlParameter("bs3", NpgsqlDbType.Bit, 1) { Value = false });
                cmd.Parameters.Add(new NpgsqlParameter("bs4", NpgsqlDbType.Bit, 2) { Value = new BitString("01") });
                cmd.Parameters.Add(new NpgsqlParameter("bs5", NpgsqlDbType.Varbit) { Value = new BitString("01") });
                cmd.Parameters.Add(new NpgsqlParameter("ba1", NpgsqlDbType.Varbit | NpgsqlDbType.Array) { Value = new BitString[] { new BitString("10"), new BitString("01") } });
                cmd.Parameters.Add(new NpgsqlParameter("ba2", NpgsqlDbType.Bit | NpgsqlDbType.Array, 1) { Value = new bool[] { true, false } });
                cmd.Parameters.Add(new NpgsqlParameter("ba3", NpgsqlDbType.Bit | NpgsqlDbType.Array, 1) { Value = new BitString[] { new BitString("1"), new BitString("0") } });
                if (prepareCommand)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.IsTrue(new BitString("1011") == (BitString)output.Value);
                    Assert.IsTrue(new BitString("1011") == (BitString)reader.GetValue(0));
                    Assert.AreEqual(true, reader.GetValue(1));
                    Assert.AreEqual(false, reader.GetValue(2));
                    Assert.IsTrue(new BitString("01") == (BitString)reader.GetValue(3));
                    Assert.IsTrue(new BitString("01") == (BitString)reader.GetValue(4));
                    Assert.AreEqual(true, ((bool[])reader.GetValue(5))[0]);
                    Assert.AreEqual(false, ((bool[])reader.GetValue(5))[1]);
                    for (int i = 6; i <= 7; i++)
                    {
                        Assert.AreEqual(new BitString("10"), ((BitString[])reader.GetValue(i))[0]);
                        Assert.AreEqual(new BitString("01"), ((BitString[])reader.GetValue(i))[1]);
                    }
                    for (int i = 8; i <= 9; i++)
                    {
                        Assert.AreEqual(true, ((bool[])reader.GetValue(i))[0]);
                        Assert.AreEqual(false, ((bool[])reader.GetValue(i))[1]);
                    }
                }
            }
        }

        [Test]
        public void SingleChar([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = Conn.CreateCommand())
            {
                var testArr = new byte[] { prepareCommand ? (byte)200 : (byte)'}', prepareCommand ? (byte)0 : (byte)'"', 3 };
                var testArr2 = new char[] { prepareCommand ? (char)200 : '}', prepareCommand ? (char)0 : '"', (char)3 };

                cmd.CommandText = "Select 'a'::\"char\", (-3)::\"char\", :p1, :p2, :p3, :p4, :p5, :p6, :p7, :p8";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.SingleChar) { Value = 'b' });
                cmd.Parameters.Add(new NpgsqlParameter("p2", NpgsqlDbType.SingleChar) { Value = 66 });
                cmd.Parameters.Add(new NpgsqlParameter("p3", NpgsqlDbType.SingleChar) { Value = "" });
                cmd.Parameters.Add(new NpgsqlParameter("p4", NpgsqlDbType.SingleChar) { Value = "\0" });
                cmd.Parameters.Add(new NpgsqlParameter("p5", NpgsqlDbType.SingleChar) { Value = "a" });
                cmd.Parameters.Add(new NpgsqlParameter("p6", NpgsqlDbType.SingleChar) { Value = (byte)231 });
                cmd.Parameters.Add(new NpgsqlParameter("p7", NpgsqlDbType.SingleChar | NpgsqlDbType.Array) { Value = testArr });
                cmd.Parameters.Add(new NpgsqlParameter("p8", NpgsqlDbType.SingleChar | NpgsqlDbType.Array) { Value = testArr2 });
                if (prepareCommand)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var expected = new char[] { 'a', (char)(256-3), 'b', (char)66, '\0', '\0', 'a', (char)231 };
                    for (int i = 0; i < expected.Length; i++)
                    {
                        Assert.AreEqual(expected[i], reader.GetChar(i));
                    }
                    var arr = (char[])reader.GetValue(8);
                    var arr2 = (char[])reader.GetValue(9);
                    Assert.AreEqual(testArr.Length, arr.Length);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        Assert.AreEqual(testArr[i], arr[i]);
                        Assert.AreEqual(testArr2[i], arr2[i]);
                    }
                }
            }
        }

        [Test]
        public void Unknown([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select :p1::timestamp, :p2::timestamp, :p3::int4";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Unknown) { Value = "2008-1-1" });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p2", Value = null });
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p3", Value = "3" });
                if (prepareCommand)
                {
                    cmd.Prepare();

                    Assert.AreEqual(NpgsqlDbType.Timestamp, cmd.Parameters[1].NpgsqlDbType); // Should be inferred by context
                    Assert.AreEqual(NpgsqlDbType.Text, cmd.Parameters[2].NpgsqlDbType); // Is inferred from the parametert value and not context
                }
                cmd.Parameters[1].Value = new DateTime(2008, 1, 1);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(new DateTime(2008, 1, 1), reader.GetValue(0));
                    Assert.AreEqual(new DateTime(2008, 1, 1), reader.GetValue(1));
                }
            }
        }

        [Test]
        public void TestEmptyIEnumerableAsArray()
        {
            using (var command = new NpgsqlCommand("SELECT :array", Conn))
            {
                var expected = new[] { 1, 2, 3, 4 };
                command.Parameters.AddWithValue("array", expected.Where(x => false));
                var res = command.ExecuteScalar() as int[];

                Assert.NotNull(res);
                Assert.AreEqual(0, res.Length);
            }
        }

        [Test, Description("Basic cancellation scenario")]
        [Timeout(6000)]
        public void Cancel()
        {
            using (var cmd = new NpgsqlCommand("SELECT pg_sleep(5)", Conn))
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(300);
                    cmd.Cancel();
                });
                Assert.That(() => cmd.ExecuteNonQuery(),
                    Throws.TypeOf<NpgsqlException>()
                    .With.Property("Code").EqualTo("57014")
                );
            }
        }

        [Test, Description("Check that cancel only affects the command on which its was invoked")]
        [Timeout(3000)]
        public void CancelCrossCommand()
        {
            using (var cmd1 = new NpgsqlCommand("SELECT pg_sleep(2)", Conn))
            using (var cmd2 = new NpgsqlCommand("SELECT 1", Conn))
            {
                var cancelTask = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(300);
                    cmd2.Cancel();
                });
                Assert.That(() => cmd1.ExecuteNonQuery(), Throws.Nothing);
                cancelTask.Wait();
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/393")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/299")]
        public void DisposePreparedAfterCommandClose()
        {
            using (var c = new NpgsqlConnection(ConnectionString))
            {
                using (var cmd = c.CreateCommand())
                {
                    c.Open();
                    cmd.CommandText = "select 1";
                    cmd.Prepare();
                    c.Close();
                }
            }
        }
    }
}
