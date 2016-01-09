#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Sockets;
using NpgsqlTypes;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Text;
using Npgsql.Logging;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests
{
    public class CommandTests : TestBase
    {
        #region Multiquery

        /// <summary>
        /// Tests various configurations of queries and non-queries within a multiquery
        /// </summary>
        [Test]
        [TestCase(new[] { true }, TestName = "SingleQuery")]
        [TestCase(new[] { false }, TestName = "SingleNonQuery")]
        [TestCase(new[] { true, true }, TestName = "TwoQueries")]
        [TestCase(new[] { false, false }, TestName = "TwoNonQueries")]
        [TestCase(new[] { false, true }, TestName = "NonQueryQuery")]
        [TestCase(new[] { true, false }, TestName = "QueryNonQuery")]
        public void Multiqueries(bool[] queries)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var sb = new StringBuilder();
                foreach (var query in queries)
                    sb.Append(query ? "SELECT 1;" : "UPDATE data SET name='yo' WHERE 1=0;");
                var sql = sb.ToString();
                foreach (var prepare in new[] {false, true})
                {
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        if (prepare)
                            cmd.Prepare();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var numResultSets = queries.Count(q => q);
                            for (var i = 0; i < numResultSets; i++)
                            {
                                Assert.That(reader.Read(), Is.True);
                                Assert.That(reader[0], Is.EqualTo(1));
                                Assert.That(reader.NextResult(), Is.EqualTo(i != numResultSets - 1));
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void MultipleQueriesWithParameters([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT @p1; SELECT @p2", conn))
                {
                    var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
                    var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Text);
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    p1.Value = 8;
                    p2.Value = "foo";
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.That(reader.Read(), Is.True);
                        Assert.That(reader.GetInt32(0), Is.EqualTo(8));
                        Assert.That(reader.NextResult(), Is.True);
                        Assert.That(reader.Read(), Is.True);
                        Assert.That(reader.GetString(0), Is.EqualTo("foo"));
                        Assert.That(reader.NextResult(), Is.False);
                    }
                }
            }
        }

        [Test]
        public void MultipleQueriesSingleRow([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
                {
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        Assert.That(reader.Read(), Is.True);
                        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                        Assert.That(reader.Read(), Is.False);
                        Assert.That(reader.NextResult(), Is.False);
                    }
                }
            }
        }

        #endregion

        #region Timeout

        [Test]
        public void TimeoutFromConnectionString()
        {
            Assert.That(NpgsqlConnector.MinimumInternalCommandTimeout, Is.Not.EqualTo(NpgsqlCommand.DefaultTimeout));
            var timeout = NpgsqlConnector.MinimumInternalCommandTimeout;
            int connId;
            using (var conn = new NpgsqlConnection(ConnectionString + ";CommandTimeout=" + timeout))
            {
                var command = new NpgsqlCommand("SELECT 1", conn);
                conn.Open();
                Assert.That(command.CommandTimeout, Is.EqualTo(timeout));
                command.CommandTimeout = 10;
                command.ExecuteScalar();
                Assert.That(command.CommandTimeout, Is.EqualTo(10));
                connId = conn.ProcessID;
            }
            using (var conn = new NpgsqlConnection(ConnectionString + ";CommandTimeout=" + timeout))
            {
                conn.Open();
                var command = CreateSleepCommand(conn, timeout + 2);
                Assert.That(conn.ProcessID, Is.EqualTo(connId), "Got a different connection...");
                Assert.That(command.CommandTimeout, Is.EqualTo(timeout));
                Assert.That(() => command.ExecuteNonQuery(),
                    Throws.TypeOf<NpgsqlException>()
                    .With.Property("Code").EqualTo("57014")
                );
                Assert.That(conn.ExecuteScalar("SHOW statement_timeout"), Is.EqualTo(timeout + "s"));
                conn.Close();
                NpgsqlConnection.ClearPool(conn);
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
            } catch { }

            using (conn = new NpgsqlConnection(ConnectionString)) {
                conn.Open();
                try //the next command will fail on earlier postgres versions, but that is not a bug in itself.
                {
                    new NpgsqlCommand("set standard_conforming_strings=off", conn).ExecuteNonQuery();
                } catch {
                }

                using (var command = CreateSleepCommand(conn, 3)) {
                    command.CommandTimeout = 1;
                    try {
                        command.ExecuteNonQuery();
                        Assert.Fail("3s command survived a 1s timeout");
                    } catch (NpgsqlException) {
                        // We cannot currently identify that the exception was a timeout
                        // in a locale-independent fashion, so just assume so.
                    }
                }

                using (var command = CreateSleepCommand(conn, 3)) {
                    command.CommandTimeout = 4;
                    try {
                        command.ExecuteNonQuery();
                    } catch (NpgsqlException) {
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

            using (var conn = OpenConnection())
            {
                var command = CreateSleepCommand(conn, 3);
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

            using (var conn = OpenConnection())
            {
                var command = CreateSleepCommand(conn, 3);
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

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void TimeoutSwitchConnection()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.CommandTimeout >= 100 && conn.CommandTimeout < 105)
                    TestUtil.IgnoreExceptOnBuildServer("Bad default command timeout");
            }

            using (var c1 = OpenConnection(ConnectionString + ";CommandTimeout=100"))
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

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/466")]
        public void TimeoutResetOnRollback()
        {
            using (var conn = OpenConnection(ConnectionString + ";CommandTimeout=10"))
            {
                var tx = conn.BeginTransaction();
                conn.ExecuteScalar("SELECT 1"); // Set timeout to 10
                tx.Rollback(); // Rollback, Npgsql should reset its timeout to unknown
                using (var cmd = new NpgsqlCommand("SHOW statement_timeout", conn))
                {
                    cmd.CommandTimeout = 20;
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo("20s"));
                }
            }
        }

        [Test, Description("Test disabling backend timeouts altogether")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/351")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/350")]
        public void TimeoutDisable()
        {
            using (var conn = OpenConnection(ConnectionString + ";BackendTimeouts=false"))
            {
                Assert.That(conn.ExecuteScalar("SHOW statement_timeout"), Is.EqualTo("0"));
                using (var tx = conn.BeginTransaction())
                {
                    // If backend timeouts has been properly disabled, statement_timeout should
                    // still be the PostgreSQL default, which is 0
                    Assert.That(conn.ExecuteScalar("SHOW statement_timeout", tx), Is.EqualTo("0"));
                }
            }
        }

        [Test, Description("Checks that the client socket timeout works as a last resort even when there's no backend timeout")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/327")]
        [Timeout(10000)]
        public void TimeoutFrontend()
        {
            using (var conn = OpenConnection(ConnectionString + ";CommandTimeout=1;BackendTimeouts=false"))
            {
                using (var cmd = CreateSleepCommand(conn, 10))
                {
                    Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception.TypeOf<IOException>());
                    Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
                }
            }
        }

        [Test, Description("Makes sure the frontend socket timeout is set to 0 (infinite) for async notification reads")]
        public void TimeoutFrontendWithAsyncNotification()
        {
            using (var conn = OpenConnection(ConnectionString + ";CommandTimeout=1;BackendTimeouts=false"))
            {
                conn.ExecuteNonQuery("SELECT 1");
                // Socket timeout is now 1 second
            }
        }

        #endregion

        #region Cancel

        [Test, Description("Basic cancellation scenario")]
        [Timeout(6000)]
        public void Cancel()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = CreateSleepCommand(conn, 5))
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(300);
                        cmd.Cancel();
                    });
                    Assert.That(() => cmd.ExecuteNonQuery(), Throws.TypeOf<NpgsqlException>().With.Property("Code").EqualTo("57014"));
                }
            }
        }

        [Test, Description("Check that cancel only affects the command on which its was invoked")]
        [Timeout(3000)]
        public void CancelCrossCommand()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd1 = CreateSleepCommand(conn, 2))
                using (var cmd2 = new NpgsqlCommand("SELECT 1", conn))
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
        }

        #endregion

        #region Cursors

        [Test]
        public void CursorStatement()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                using (var t = conn.BeginTransaction())
                {
                    for (var x = 0; x < 5; x++)
                        conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('X')");

                    Int32 i = 0;
                    var command = new NpgsqlCommand("DECLARE TE CURSOR FOR SELECT * FROM DATA", conn);
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
        }

        #endregion

        #region CommandBehavior.CloseConnection

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/693")]
        public void CloseConnection()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    while (reader.Read()) {}
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        public void CloseConnectionWithException()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SE", conn))
                    Assert.That(() => cmd.ExecuteReader(CommandBehavior.CloseConnection), Throws.Exception.TypeOf<NpgsqlException>());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        #endregion

        [Test, Description("Makes sure writing an unset parameter isn't allowed")]
        public void ParameterUnset()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("@p", NpgsqlDbType.Integer));
                    Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidCastException>());
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
        public void SameParamMultipleTimes()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT @p1, @p1", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", 8);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader[0], Is.EqualTo(8));
                        Assert.That(reader[1], Is.EqualTo(8));
                    }
                }
            }
        }

        [Test]
        public void EmptyQuery()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("");
                conn.ExecuteNonQuery(";");
            }
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
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                using (var command = new NpgsqlCommand("SELECT name FROM data", conn))
                {
                    Assert.That(command.ExecuteScalar(), Is.Null);

                    conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES (NULL)");
                    Assert.That(command.ExecuteScalar(), Is.EqualTo(DBNull.Value));

                    conn.ExecuteNonQuery(@"TRUNCATE data");
                    for (var i = 0; i < 2; i++)
                        conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')");
                    Assert.That(command.ExecuteScalar(), Is.EqualTo("X"));
                }
            }
        }

        [Test, Description("Makes sure a command is unusable after it is disposed")]
        public void Dispose()
        {
            using (var conn = OpenConnection())
            {
                var cmd = new NpgsqlCommand("SELECT 1", conn);
                cmd.Dispose();
                Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<ObjectDisposedException>());
                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception.TypeOf<ObjectDisposedException>());
                Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<ObjectDisposedException>());
                Assert.That(() => cmd.Prepare(), Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test, Description("Disposing a command with an open reader does not close the reader. This is the SqlClient behavior.")]
        public void DisposeCommandDoesNotCloseReader()
        {
            using (var conn = OpenConnection())
            {
                var cmd = new NpgsqlCommand("SELECT 1, 2", conn);
                cmd.ExecuteReader();
                cmd.Dispose();
                cmd = new NpgsqlCommand("SELECT 3", conn);
                Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test, Description("Basic prepared system scenario. Checks proper backend deallocation of the statement.")]
        public void Prepare()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));

                using (var cmd = new NpgsqlCommand("INSERT INTO data (name) VALUES (:p0);", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
                    cmd.Prepare();
                    cmd.Parameters["p0"].Value = "test";
                    using (var dr = cmd.ExecuteReader())
                    {
                        Assert.IsNotNull(dr);
                        dr.Close();
                        Assert.That(dr.RecordsAffected, Is.EqualTo(1));
                    }
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(1));
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data WHERE name = 'test'"), Is.EqualTo(1));
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
            }
        }

        [Test]
        public void PreparedStatementWithParameters()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER, long BIGINT)");
                using (var command = new NpgsqlCommand("select * from data where int = :a and long = :b;", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
                    command.Parameters.Add(new NpgsqlParameter("b", DbType.Int64));
                    Assert.AreEqual(2, command.Parameters.Count);
                    Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

                    command.Prepare();
                    command.Parameters[0].Value = 3;
                    command.Parameters[1].Value = 5;
                    using (var dr = command.ExecuteReader())
                    {
                        Assert.IsNotNull(dr);
                    }
                }
            }
        }

        [Test, Description("Makes sure that calling Prepare() twice on a command deallocates the first prepared statement")]
        public void DoublePrepare()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT, int INTEGER)");
                using (var cmd = new NpgsqlCommand("INSERT INTO data (name) VALUES (:p0)", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Text));
                    cmd.Parameters["p0"].Value = "test";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO data (int) VALUES (:p0)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new NpgsqlParameter("p0", NpgsqlDbType.Integer));
                    cmd.Parameters["p0"].Value = 8;
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0), "Prepared statements are being leaked");
            }
        }

        [Test]
        public void StringEscapeSyntax()
        {
            using (var conn = OpenConnection())
            {

                //the next command will fail on earlier postgres versions, but that is not a bug in itself.
                try
                {
                    conn.ExecuteNonQuery("set standard_conforming_strings=off;set escape_string_warning=off");
                }
                catch
                {
                }
                string cmdTxt = "select :par";
                var command = new NpgsqlCommand(cmdTxt, conn);
                var arrCommand = new NpgsqlCommand(cmdTxt, conn);
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
                    conn.ExecuteNonQuery("set standard_conforming_strings=on;set escape_string_warning=on");
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
        }

        [Test]
        public void ParameterAndOperatorUnclear()
        {
            using (var conn = OpenConnection())
            {
                //Without parenthesis the meaning of [, . and potentially other characters is
                //a syntax error. See comment in NpgsqlCommand.GetClearCommandText() on "usually-redundant parenthesis".
                using (var command = new NpgsqlCommand("select :arr[2]", conn))
                {
                    command.Parameters.AddWithValue(":arr", new int[] {5, 4, 3, 2, 1});
                    using (var rdr = command.ExecuteReader())
                    {
                        rdr.Read();
                        Assert.AreEqual(rdr.GetInt32(0), 4);
                    }
                }
            }
        }

        [Test]
        public void StatementMappedOutputParameters()
        {
            using (var conn = OpenConnection())
            {
                var command = new NpgsqlCommand("select 3, 4 as param1, 5 as param2, 6;", conn);

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
        }

        [Test]
        public void CaseSensitiveParameterNames()
        {
            using (var conn = OpenConnection())
            {
                using (var command = new NpgsqlCommand("select :p1", conn))
                {
                    command.Parameters.Add(new NpgsqlParameter("P1", NpgsqlDbType.Integer)).Value = 5;
                    var result = command.ExecuteScalar();
                    Assert.AreEqual(5, result);
                }
            }
        }

        [Test]
        public void TestBug1006158OutputParameters()
        {
            using (var conn = OpenConnection())
            {
                const string createFunction =
                    @"CREATE OR REPLACE FUNCTION pg_temp.more_params(OUT a integer, OUT b boolean) AS
            $BODY$DECLARE
                BEGIN
                    a := 3;
                    b := true;
                END;$BODY$
              LANGUAGE 'plpgsql' VOLATILE;";

                var command = new NpgsqlCommand(createFunction, conn);
                command.ExecuteNonQuery();

                command = new NpgsqlCommand("pg_temp.more_params", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
                command.Parameters[0].Direction = ParameterDirection.Output;
                command.Parameters.Add(new NpgsqlParameter("b", DbType.Boolean));
                command.Parameters[1].Direction = ParameterDirection.Output;

                var result = command.ExecuteScalar();

                Assert.AreEqual(3, command.Parameters[0].Value);
                Assert.AreEqual(true, command.Parameters[1].Value);
            }
        }

        [Test]
        public void TestErrorInPreparedStatementCausesReleaseConnectionToThrowException()
        {
            using (var conn = OpenConnection())
            {
                // This is caused by having an error with the prepared statement and later, Npgsql is trying to release the plan as it was successful created.
                var cmd = new NpgsqlCommand("sele", conn);
                Assert.That(() => cmd.Prepare(), Throws.Exception.TypeOf<NpgsqlException>());
            }
        }

        [Test]
        public void Bug1010788UpdateRowSource()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY, name TEXT)");
                var command = new NpgsqlCommand("SELECT * FROM data", conn);
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
        public void TableDirect()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('foo')");
                using (var cmd = new NpgsqlCommand("data", conn) { CommandType = CommandType.TableDirect })
                using (var rdr = cmd.ExecuteReader())
                {
                    Assert.That(rdr.Read(), Is.True);
                    Assert.That(rdr["name"], Is.EqualTo("foo"));
                }
            }
        }

        [Test]
        public void InputAndOutputParameters()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "Select :a + 2 as b, :c - 1 as c";
                var b = new NpgsqlParameter { ParameterName = "b", Direction = ParameterDirection.Output };
                cmd.Parameters.Add(b);
                cmd.Parameters.Add(new NpgsqlParameter("a", 3));
                var c = new NpgsqlParameter { ParameterName = "c", Direction = ParameterDirection.InputOutput, Value = 4 };
                cmd.Parameters.Add(c);
                using (cmd.ExecuteReader())
                {
                    Assert.AreEqual(5, b.Value);
                    Assert.AreEqual(3, c.Value);
                }
            }
        }

        [Test]
        public void SendUnknown([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::TIMESTAMP", conn))
            {
                cmd.CommandText = "SELECT @p::TIMESTAMP";
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Unknown) { Value = "2008-1-1" });
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetValue(0), Is.EqualTo(new DateTime(2008, 1, 1)));
                }
            }
        }

        [Test, Description("Checks that prepares requires all params to have explicitly set types (NpgsqlDbType or DbType)")]
        public void PrepareRequiresParamTypesSet()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var p = new NpgsqlParameter("p", 8);
                cmd.Parameters.Add(p);
                Assert.That(() => cmd.Prepare(), Throws.InvalidOperationException);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/503")]
        public void InvalidUTF8()
        {
            const string badString = "SELECT 'abc\uD801\uD802d'";
            using (var conn = OpenConnection())
            {
                Assert.That(() => conn.ExecuteScalar(badString), Throws.Exception.TypeOf<EncoderFallbackException>());
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/393")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/299")]
        public void DisposePreparedAfterCommandClose()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select 1";
                cmd.Prepare();
                conn.Close();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void PreparedAcrossCloseOpen()
        {
            using (var conn1 = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
            {
                cmd.Prepare();
                Assert.That(cmd.IsPrepared, Is.True);
                conn1.Close();
                conn1.Open();
                Assert.That(cmd.IsPrepared, Is.False);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1)); // Execute unprepared
                cmd.Prepare();
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
        public void UseAcrossConnectionChange([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var conn1 = OpenConnection())
            using (var conn2 = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
            {
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                cmd.Connection = conn2;
                Assert.That(cmd.IsPrepared, Is.False);
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
            }
        }

        [Test, Description("CreateCommand before connection open")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/565")]
        public void CreateCommandBeforeConnectionOpen()
        {
            using (var conn = new NpgsqlConnection(ConnectionString)) {
                var cmd = new NpgsqlCommand("SELECT 1", conn);
                conn.Open();
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
            }
        }

        [Test]
        public void BadConnection()
        {
            var cmd = new NpgsqlCommand("SELECT 1");
            Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidOperationException>());

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                cmd = new NpgsqlCommand("SELECT 1", conn);
                Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/416")]
        public void PreparedDisposeWithOpenReader()
        {
            using (var conn = OpenConnection())
            {
                var cmd1 = new NpgsqlCommand("SELECT 1", conn);
                var cmd2 = new NpgsqlCommand("SELECT 1", conn);
                cmd1.Prepare();
                cmd2.Prepare();
                var reader = cmd2.ExecuteReader();
                reader.Read();
                cmd1.Dispose();
                cmd2.Dispose();
                reader.Close();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements"), Is.EqualTo(0));
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/400")]
        public void ExceptionThrownFromExecuteQuery([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"
                     CREATE OR REPLACE FUNCTION pg_temp.emit_exception() RETURNS VOID AS
                        'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
                     LANGUAGE 'plpgsql';
                ");

                using (var cmd = new NpgsqlCommand("SELECT pg_temp.emit_exception()", conn))
                {
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<NpgsqlException>());
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/831")]
        [Timeout(10000)]
        public void ManyParameters()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                for (var i = 0; i < conn.BufferSize; i++)
                    cmd.Parameters.Add(new NpgsqlParameter("p" + i, 8));
                cmd.ExecuteNonQuery();
            }
        }

        [Test, Description("Bypasses PostgreSQL's int16 limitation on the number of parameters")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/831")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/858")]
        [Timeout(10000)]
        public void TooManyParameters()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand { Connection = conn })
            {
                var sb = new StringBuilder("SELECT ");
                for (var i = 0; i < 65536; i++)
                {
                    var paramName = "p" + i;
                    cmd.Parameters.Add(new NpgsqlParameter(paramName, 8));
                    if (i > 0)
                        sb.Append(", ");
                    sb.Append('@');
                    sb.Append(paramName);
                }
                cmd.CommandText = sb.ToString();
                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
                    .InstanceOf<Exception>()
                    .With.Message.EqualTo("A command cannot have more than 65535 parameters")
                );
            }
        }

        public CommandTests(string backendVersion) : base(backendVersion) { }
    }
}
