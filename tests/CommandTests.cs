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
using System.Collections;
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
using System.Text;
using NUnit.Framework.Constraints;

namespace NpgsqlTests
{
    public class CommandTests : TestBase
    {
        public CommandTests(string backendVersion) : base(backendVersion) { }

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
        public void ExecuteScalar()
        {
            using (var command = new NpgsqlCommand("SELECT field_text FROM data", Conn))
            {
                Assert.That(command.ExecuteScalar(), Is.Null);

                ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES (NULL)");
                Assert.That(command.ExecuteScalar(), Is.EqualTo(DBNull.Value));

                ExecuteNonQuery(@"TRUNCATE data");
                for (var i = 0; i < 2; i++)
                    ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('X')");
                Assert.That(command.ExecuteScalar(), Is.EqualTo("X"));
            }
        }

        [Test, Description("Makes sure a command is unusable after it is disposed")]
        public void Dispose()
        {
            var cmd = new NpgsqlCommand("SELECT 1", Conn);
            cmd.Dispose();
            Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<ObjectDisposedException>());
            Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception.TypeOf<ObjectDisposedException>());
            Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<ObjectDisposedException>());
            Assert.That(() => cmd.Prepare(), Throws.Exception.TypeOf<ObjectDisposedException>());
        }

        [Test, Description("Disposing a command with an open reader does not close the reader. This is the SqlClient behavior.")]
        public void DisposeCommandDoesNotCloseReader()
        {
            var cmd = new NpgsqlCommand("SELECT 1, 2", Conn);
            cmd.ExecuteReader();
            cmd.Dispose();
            cmd = new NpgsqlCommand("SELECT 3", Conn);
            Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        #region Cursors

        [Test]
        public void CursorStatement()
        {
            using (var t = Conn.BeginTransaction())
            {
                for (var x = 0; x < 5; x++)
                    ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");

                Int32 i = 0;
                var command = new NpgsqlCommand("DECLARE TE CURSOR FOR SELECT * FROM DATA", Conn);
                command.ExecuteNonQuery();
                command.CommandText = "FETCH FORWARD 3 IN TE";
                var dr = command.ExecuteReader();

                while (dr.Read())
                    i++;
                Assert.AreEqual(3, i);
                dr.Close();

                i = 0;
                command.CommandText = "FETCH BACKWARD 1 IN TE";
                var dr2 = command.ExecuteReader();
                while (dr2.Read())
                    i++;
                Assert.AreEqual(1, i);
                dr2.Close();

                command.CommandText = "close te;";
                command.ExecuteNonQuery();
            }
        }

        [Test]
        public void MultipleRefCursorSupport()
        {
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testmultcurfunc() RETURNS SETOF refcursor AS 'DECLARE ref1 refcursor; ref2 refcursor; BEGIN OPEN ref1 FOR SELECT 1; RETURN NEXT ref1; OPEN ref2 FOR SELECT 2; RETURN next ref2; RETURN; END;' LANGUAGE 'plpgsql';");
            using (Conn.BeginTransaction()) {
                var command = new NpgsqlCommand("testmultcurfunc", Conn);
                command.CommandType = CommandType.StoredProcedure;
                using (var dr = command.ExecuteReader()) {
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

        #endregion

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

        [Test, Description("Makes sure that calling Prepare() twice on a command deallocates the first prepared statement")]
        public void DoublePrepare()
        {
            var cmd = new NpgsqlCommand("INSERT INTO data (field_text) VALUES (:p0)", Conn);
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

            cmd.Dispose();
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
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

        #region Multiquery

        /// <summary>
        /// Tests various configurations of queries and non-queries within a multiquery
        /// </summary>
        [Test]
        [TestCase(new[] { true         }, TestName = "SingleQuery"   )]
        [TestCase(new[] { false        }, TestName = "SingleNonQuery")]
        [TestCase(new[] { true, true   }, TestName = "TwoQueries"    )]
        [TestCase(new[] { false, false }, TestName = "TwoNonQueries" )]
        [TestCase(new[] { false, true  }, TestName = "NonQueryQuery" )]
        [TestCase(new[] { true, false  }, TestName = "QueryNonQuery" )]
        public void Multiqueries(bool[] queries)
        {
            var sb = new StringBuilder();
            foreach (var query in queries)
                sb.Append(query ? "SELECT 1;" : "UPDATE data SET field_text='yo' WHERE 1=0;");
            var sql = sb.ToString();
            foreach (var prepare in new[] { false, true })
            {
                var cmd = new NpgsqlCommand(sql, Conn);
                if (prepare)
                    cmd.Prepare();
                var reader = cmd.ExecuteReader();
                var numResultSets = queries.Count(q => q);
                for (var i = 0; i < numResultSets; i++)
                {
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader[0], Is.EqualTo(1));
                    Assert.That(reader.NextResult(), Is.EqualTo(i != numResultSets - 1));
                }
                reader.Close();
                cmd.Dispose();
            }
        }

        [Test]
        public void MultipleQueriesWithParameters([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT @p1; SELECT @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Text);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            if (prepare == PrepareOrNot.Prepared) {
                cmd.Prepare();
            }
            p1.Value = 8;
            p2.Value = "foo";
            var reader = cmd.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.GetInt32(0), Is.EqualTo(8));
            Assert.That(reader.NextResult(), Is.True);
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.GetString(0), Is.EqualTo("foo"));
            Assert.That(reader.NextResult(), Is.False);
            reader.Close();
            cmd.Dispose();
        }

        [Test]
        public void MultipleQueriesSingleRow([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", Conn);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.GetInt32(0), Is.EqualTo(1));
            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.NextResult(), Is.False);
            reader.Close();
            cmd.Dispose();
        }

        #endregion

        #region Query parsing

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

            var command2 = new NpgsqlCommand("select field_char5 from data", Conn);
            var a = (String) command2.ExecuteScalar();
            Assert.AreEqual(aValue, a);
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

            if (prepare) {
                command.Prepare();
            }

            command.ExecuteScalar();
        }

        [Test]
        public void Bug1010557BackslashGetDoubled()
        {
            using (var cmd = new NpgsqlCommand("select :p1", Conn)) {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Text);
                parameter.Value = "test\\str";
                cmd.Parameters.Add(parameter);
                var result = cmd.ExecuteScalar();
                Assert.AreEqual("test\\str", result);
            }
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
        public void ParseStringWithSpecialChars()
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "SELECT 'b''la'";
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo("b'la"));
                cmd.CommandText = "SELECT 'type(''m.response'')#''O''%'";
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo("type('m.response')#'O'%"));
            }
        }

        [Test]
        public void ParameterSubstitutionLexerTest()
        {
            using (var r = PSLT(@"SELECT :str, :int, :null")) {
                Assert.AreEqual("string", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
                Assert.IsTrue(r.IsDBNull(2));
            }
            using (var r = PSLT(@"SELECT e'ab\'c:str', :int")) {
                Assert.AreEqual("ab'c:str", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
            }
            using (var r = PSLT(@"SELECT E'a\'b'
-- a comment here :str)'
'c\'d:str', :int, E''
'\':str', :int")) {
                Assert.AreEqual("a'bc'd:str", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
                Assert.AreEqual("':str", r.GetString(2));
                Assert.AreEqual(123, r.GetInt32(3));
            }
            using (var r = PSLT(@"SELECT 'abc'::text, :text, 246/:int, 122<@int, (ARRAY[1,2,3,4])[1:@int-121]::text, (ARRAY[1,2,3,4])[1: :int-121]::text, (ARRAY[1,2,3,4])[1:two]::text FROM (SELECT 2 AS two) AS a")) {
                Assert.AreEqual("abc", r.GetString(0));
                Assert.AreEqual("tt", r.GetString(1));
                Assert.AreEqual(2, r.GetInt32(2));
                Assert.IsTrue(r.GetBoolean(3));
                Assert.AreEqual("{1,2}", r.GetString(4));
                Assert.AreEqual("{1,2}", r.GetString(5));
                Assert.AreEqual("{1,2}", r.GetString(6));
            }
            using (var r = PSLT("SELECT/*/* -- nested comment :int /*/* *//*/ **/*/*/*/:str")) {
                Assert.AreEqual("string", r.GetString(0));
            }
            using (var r = PSLT("SELECT--comment\r:str")) {
                Assert.AreEqual("string", r.GetString(0));
            }
            using (var r = PSLT("SELECT $\u00ffabc0$literal string :str :int$\u00ffabc0 $\u00ffabc0$, :int, $$:str$$")) {
                Assert.AreEqual("literal string :str :int$\u00ffabc0 ", r.GetString(0));
                Assert.AreEqual(123, r.GetInt32(1));
                Assert.AreEqual(":str", r.GetString(2));
            }
            if (!Conn.UseConformantStrings) {
                using (var r = PSLT(@"SELECT 'abc\':str''a:str', :int")) {
                    Assert.AreEqual("abc':str'a:str", r.GetString(0));
                    Assert.AreEqual(123, r.GetInt32(1));
                }
            } else {
                using (var r = PSLT(@"SELECT 'abc'':str''a:str', :int")) {
                    Assert.AreEqual("abc':str'a:str", r.GetString(0));
                    Assert.AreEqual(123, r.GetInt32(1));
                }
            }

            // Don't touch output parameters
            using (var cmd = Conn.CreateCommand()) {
                cmd.CommandText = @"SELECT (ARRAY[1,2,3])[1:abc]::text AS abc FROM (SELECT 2 AS abc) AS a";
                var param = new NpgsqlParameter { Direction = ParameterDirection.Output, DbType = DbType.String, ParameterName = "abc" };
                cmd.Parameters.Add(param);
                using (var r = cmd.ExecuteReader()) {
                    r.Read();
                    Assert.AreEqual("{1,2}", param.Value);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(Npgsql.NpgsqlException), ExpectedMessage = "ERROR: 42P01: relation \":str\" does not exist")]
        public void ParameterSubstitutionLexerTestDoubleQuoted()
        {
            using (var r = PSLT("SELECT 1 FROM \":str\"")) {
            }
        }

        #endregion

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
        public void CaseSensitiveParameterNames()
        {
            var command = new NpgsqlCommand("select :p1", Conn);
            command.Parameters.Add(new NpgsqlParameter("P1", NpgsqlDbType.Integer)).Value = 5;
            var result = command.ExecuteScalar();
            Assert.AreEqual(5, result);
        }

        [Test]
        public void ConnectionStringCommandTimeout()
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";CommandTimeout=180;pooling=false"))
            {
                var command = new NpgsqlCommand("\"Foo\"", conn);
                conn.Open();
                Assert.AreEqual(180, command.CommandTimeout);
            }
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
        [ExpectedException(typeof (NpgsqlException))]
        public void TestErrorInPreparedStatementCausesReleaseConnectionToThrowException()
        {
            // This is caused by having an error with the prepared statement and later, Npgsql is trying to release the plan as it was successful created.
            var cmd = new NpgsqlCommand("sele", Conn);
            cmd.Prepare();
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

        private NpgsqlDataReader PSLT(string query)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("str", "string");
                cmd.Parameters.AddWithValue("int", 123);
                cmd.Parameters.AddWithValue("text", "tt");
                cmd.Parameters.AddWithValue("null", NpgsqlDbType.Text, DBNull.Value);

                // syntax error at or near ":"
                var rdr = cmd.ExecuteReader();
                Assert.IsTrue(rdr.Read());
                return rdr;
            }
        }

        [Test]
        public void TableDirect()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('foo')");
            var cmd = new NpgsqlCommand("data", Conn) { CommandType = CommandType.TableDirect };
            var rdr = cmd.ExecuteReader();
            Assert.That(rdr.Read(), Is.True);
            Assert.That(rdr["field_text"], Is.EqualTo("foo"));
            rdr.Close();
            cmd.Dispose();
        }

        [Test]
        public void InputAndOutputParameters()
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select :a + 2 as b, :c - 1 as c";
                var b = new NpgsqlParameter() { ParameterName = "b", Direction = ParameterDirection.Output };
                cmd.Parameters.Add(b);
                cmd.Parameters.Add(new NpgsqlParameter("a", 3));
                var c = new NpgsqlParameter() { ParameterName = "c", Direction = ParameterDirection.InputOutput, Value = 4 };
                cmd.Parameters.Add(c);
                using (cmd.ExecuteReader())
                {
                    Assert.AreEqual(5, b.Value);
                    Assert.AreEqual(3, c.Value);
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

        #region Cancel

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

        #endregion

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/503")]
        public void InvalidUTF8()
        {
            const string badString = "SELECT 'abc\uD801\uD802d'";
            Assert.That(() => ExecuteScalar(badString), Throws.Exception.TypeOf<EncoderFallbackException>());
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

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void PreparedAcrossCloseOpen()
        {
            using (var c = new NpgsqlConnection(ConnectionString))
            {
                using (var cmd = c.CreateCommand())
                {
                    c.Open();
                    cmd.CommandText = "SELECT 1";
                    cmd.Prepare();
                    Assert.That(cmd.IsPrepared, Is.True);
                    c.Close();
                    c.Open();
                    Assert.That(cmd.IsPrepared, Is.False);
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1)); // Execute unprepared
                    cmd.Prepare();
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void UseAcrossConnectionChange([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var c = new NpgsqlConnection(ConnectionString))
            {
                using (var cmd = c.CreateCommand())
                {
                    c.Open();
                    cmd.CommandText = "SELECT 1";
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    cmd.Connection = Conn;
                    Assert.That(cmd.IsPrepared, Is.False);
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void DefaultCommandTimeout()
        {
            if (Conn.Connector.DefaultCommandTimeout >= 100 && Conn.Connector.DefaultCommandTimeout < 105)
                TestUtil.Inconclusive("Bad default command timeout");
            using (var c1 = new NpgsqlConnection(ConnectionString + ";CommandTimeout=100"))
            {
                using (var cmd = c1.CreateCommand())
                {
                    Assert.That(cmd.CommandTimeout, Is.EqualTo(100));
                    using (var c2 = new NpgsqlConnection(ConnectionString + ";CommandTimeout=101"))
                    {
                        cmd.Connection = c2;
                        Assert.That(cmd.CommandTimeout, Is.EqualTo(101));
                    }
                    cmd.CommandTimeout = 102;
                    using (var c2 = new NpgsqlConnection(ConnectionString + ";CommandTimeout=101"))
                    {
                        cmd.Connection = c2;
                        Assert.That(cmd.CommandTimeout, Is.EqualTo(102));
                    }
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/416")]
        public void PreparedDisposeWithOpenReader()
        {
            var cmd1 = new NpgsqlCommand("SELECT 1", Conn);
            var cmd2 = new NpgsqlCommand("SELECT 1", Conn);
            cmd1.Prepare();
            cmd2.Prepare();
            var reader = cmd2.ExecuteReader();
            reader.Read();
            cmd1.Dispose();
            cmd2.Dispose();
            reader.Close();
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/466")]
        public void TimeoutResetOnRollback()
        {
            var conn = new NpgsqlConnection(ConnectionString + ";CommandTimeout=0");
            conn.Open();
            ExecuteScalar("SELECT 1", conn);  // Set timeout to 0
            var tx = conn.BeginTransaction(); // Set timeout to 20
            ExecuteScalar("SELECT 1", conn);  // Set timeout to 0
            tx.Rollback();                    // Rollback, backend has timeout 0 but Npgsql thinks it's still 20
            Assert.That(ExecuteScalar("SHOW statement_timeout", conn), Is.EqualTo("0"));
            conn.Close();
        }
    }
}
