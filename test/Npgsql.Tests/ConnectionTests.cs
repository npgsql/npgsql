#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Resources;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using NpgsqlTypes;

namespace Npgsql.Tests
{
    public class ConnectionTests : TestBase
    {
        [Test, Description("Makes sure the connection goes through the proper state lifecycle")]
        //[Timeout(5000)]
        public void BasicLifecycle()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                bool eventOpen = false, eventClosed = false, eventBroken = false;
                conn.StateChange += (s, e) =>
                {
                    if (e.OriginalState == ConnectionState.Closed && e.CurrentState == ConnectionState.Open)
                        eventOpen = true;
                    if (e.OriginalState == ConnectionState.Open && e.CurrentState == ConnectionState.Closed)
                        eventClosed = true;
                };

                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));

                // TODO: Connecting state?

                conn.Open();
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.Connector.State, Is.EqualTo(ConnectorState.Ready));
                Assert.That(eventOpen, Is.True);

                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open | ConnectionState.Fetching));
                    Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
                    Assert.That(conn.Connector.State, Is.EqualTo(ConnectorState.Fetching));
                }

                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.Connector.State, Is.EqualTo(ConnectorState.Ready));

                using (var cmd = CreateSleepCommand(conn, 1))
                {
                    var exitFlag = false;
                    var pollingTask = Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            if (exitFlag) {
                                Assert.Fail("Connection did not reach the Executing state");
                            }
                            if (conn.Connector.State == ConnectorState.Executing)
                            {
                                Assert.That(conn.FullState & ConnectionState.Executing, Is.Not.EqualTo(0));
                                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
                                return;
                            }
                        }
                    });
                    cmd.ExecuteNonQuery();
                    exitFlag = true;
                    pollingTask.Wait();
                }

                conn.Close();
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
                Assert.That(eventClosed, Is.True);

                conn.Open();
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                Assert.That(conn.Connector.State, Is.EqualTo(ConnectorState.Ready));

                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");

                conn.StateChange += (sender, args) =>
                {
                    if (args.CurrentState == ConnectionState.Closed)
                        eventBroken = true;
                };

                // Allow some time for the pg_terminate to kill our connection
                using (var cmd = CreateSleepCommand(conn, 10))
                    Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
                        .TypeOf<NpgsqlException>()
                        .With.InnerException.InstanceOf<IOException>()
                    );

                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
                Assert.That(eventBroken, Is.True);
            }
        }

        #region Connection Errors

#if IGNORE
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConnectionRefused(bool pooled)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Port = 44444, Pooling = pooled };
            using (var conn = new NpgsqlConnection(csb)) {
                Assert.That(() => conn.Open(), Throws.Exception
                    .TypeOf<SocketException>()
#if !NETCOREAPP1_0
// CoreCLR currently has an issue which causes the wrong SocketErrorCode to be set on Linux:
// https://github.com/dotnet/corefx/issues/8464

                    .With.Property(nameof(SocketException.SocketErrorCode)).EqualTo(SocketError.ConnectionRefused)
#endif
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConnectionRefusedAsync(bool pooled)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Port = 44444, Pooling = pooled };
            using (var conn = new NpgsqlConnection(csb))
            {
                Assert.That(async () => await conn.OpenAsync(), Throws.Exception
                    .TypeOf<SocketException>()
                    .With.Property(nameof(SocketException.SocketErrorCode)).EqualTo(SocketError.ConnectionRefused)
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }
#endif

        [Test]
        [Ignore("Fails in a non-determinstic manner and only on the build server... investigate...")]
        public void InvalidUserId()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Username = "unknown", Pooling = false
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                Assert.That(conn.Open, Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("28P01")
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test, Description("Connects with a bad password to ensure the proper error is thrown")]
        public void AuthenticationFailure()
        {
            if (Environment.GetEnvironmentVariable("TRAVIS") != null)
                Assert.Ignore("Test mysteriously fails on Travis only");
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Password = "bad",
                Pooling = false
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                Assert.That(() => conn.Open(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("28P01")
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        public void BadDatabase()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Database = "does_not_exist"
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
                Assert.That(() => conn.Open(),
                    Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("3D000")
                );
        }

        [Test, Description("Tests that mandatory connection string parameters are indeed mandatory")]
        public void MandatoryConnectionStringParams()
        {
            Assert.That(() => new NpgsqlConnection("User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests").Open(), Throws.Exception.TypeOf<ArgumentException>());
        }


        [Test, Description("Reuses the same connection instance for a failed connection, then a successful one")]
        public void FailConnectThenSucceed()
        {
            var dbName = TestUtil.GetUniqueIdentifier(nameof(FailConnectThenSucceed));
            using (var conn1 = OpenConnection())
            {
                conn1.ExecuteNonQuery($"DROP DATABASE IF EXISTS \"{dbName}\"");
                try
                {
                    var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = dbName,
                        Pooling = false
                    }.ToString();

                    using (var conn2 = new NpgsqlConnection(connString))
                    {
                        Assert.That(() => conn2.Open(),
                            Throws.Exception.TypeOf<PostgresException>()
                            .With.Property(nameof(PostgresException.SqlState)).EqualTo("3D000") // database doesn't exist
                        );
                        Assert.That(conn2.FullState, Is.EqualTo(ConnectionState.Closed));

                        conn1.ExecuteNonQuery($"CREATE DATABASE \"{dbName}\" TEMPLATE template0");

                        conn2.Open();
                        conn2.Close();
                    }
                }
                finally
                {
                    //conn1.ExecuteNonQuery($"DROP DATABASE IF EXISTS \"{dbName}\"");
                }
            }
        }

        [Test]
        [Timeout(10000)]
        public void ConnectTimeout()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) {
                Host = unknownIp,
                Pooling = false,
                Timeout = 2
            };
            using (var conn = new NpgsqlConnection(csb.ToString()))
            {
                var sw = Stopwatch.StartNew();
                Assert.That(() => conn.Open(), Throws.Exception.TypeOf<TimeoutException>());
                Assert.That(sw.Elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo((csb.Timeout * 1000) - 100),
                    $"Timeout was supposed to happen after {csb.Timeout} seconds, but fired after {sw.Elapsed.TotalSeconds}");
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        [Timeout(10000)]
        public void ConnectTimeoutAsync()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = unknownIp,
                Pooling = false,
                Timeout = 2
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                Assert.That(async () => await conn.OpenAsync(), Throws.Exception.TypeOf<TimeoutException>());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        [Timeout(10000)]
        public void ConnectTimeoutCancel()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                Assert.Ignore("NPGSQL_UNKNOWN_IP isn't defined and is required for connection cancellation tests");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = unknownIp,
                Pooling = false,
                Timeout = 30
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(1000);
                Assert.That(async () => await conn.OpenAsync(cts.Token), Throws.Exception.TypeOf<TaskCanceledException>());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        #endregion

        #region Keepalive

        [Test, Description("Makes sure that if keepalive is enabled, broken connections are detected")]
        [Timeout(10000)]
        public void Keepalive()
        {
            var csbWithKeepAlive = new NpgsqlConnectionStringBuilder(ConnectionString) { KeepAlive = 1 };
            var mre = new ManualResetEvent(false);
            using (var conn1 = OpenConnection())
            using (var conn2 = OpenConnection(csbWithKeepAlive))
            {
                conn2.StateChange += (sender, args) =>
                {
                    if (args.CurrentState == ConnectionState.Closed)
                        mre.Set();
                };

                // Use another connection to kill our keepalive connection
                conn1.ExecuteNonQuery($"SELECT pg_terminate_backend({conn2.ProcessID})");
                mre.WaitOne();
                Assert.That(conn2.State, Is.EqualTo(ConnectionState.Closed));
                Assert.That(conn2.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        #endregion

        #region Client Encoding

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
        public void ClientEncodingIsUTF8ByDefault()
        {
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar("SHOW client_encoding"), Is.EqualTo("UTF8"));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
        [Parallelizable(ParallelScope.None)]
        public void ClientEncodingEnvVar()
        {
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar("SHOW client_encoding"), Is.Not.EqualTo("SQL_ASCII"));
            var prevEnvVar = Environment.GetEnvironmentVariable("PGCLIENTENCODING");
            Environment.SetEnvironmentVariable("PGCLIENTENCODING", "SQL_ASCII");
            // Note that the pool is unaware of the environment variable, so if a connection is
            // returned from the pool it may contain the wrong client_encoding
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ClientEncodingEnvVar),
                Pooling = false
            };
            try
            {
                using (var conn = OpenConnection(connString))
                    Assert.That(conn.ExecuteScalar("SHOW client_encoding"), Is.EqualTo("SQL_ASCII"));
            }
            finally
            {
                Environment.SetEnvironmentVariable("PGCLIENTENCODING", prevEnvVar);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1065")]
        public void ClientEncodingConnectionParam()
        {
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar("SHOW client_encoding"), Is.Not.EqualTo("SQL_ASCII"));
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ClientEncoding = "SQL_ASCII",
                Pooling = false
            };
            using (var conn = OpenConnection(connString))
                Assert.That(conn.ExecuteScalar("SHOW client_encoding"), Is.EqualTo("SQL_ASCII"));
        }

        #endregion

        [Test, WindowsIgnore]
        public void UnixDomainSocket()
        {
            var port = new NpgsqlConnectionStringBuilder(ConnectionString).Port;
            var candidateDirectories = new[] { "/var/run/postgresql", "/tmp" };
            var dir = candidateDirectories.FirstOrDefault(d => File.Exists(Path.Combine(d, $".s.PGSQL.{port}")));
            if (dir == null)
            {
                TestUtil.IgnoreExceptOnBuildServer("No PostgreSQL unix domain socket was found");
                return;
            }

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host=dir,
                Username=null  // Let Npgsql detect the username
            };
            using (var conn = OpenConnection(csb))
            {
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/903")]
        public void DataSource()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
                Assert.That(conn.DataSource, Is.EqualTo($"tcp://{conn.Host}:{conn.Port}"));
        }

        [Test]
        public void SetConnectionString()
        {
            using (var conn = new NpgsqlConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();
                Assert.That(() => conn.ConnectionString = "", Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/703")]
        public void NoDatabaseDefaultsToUsername()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Database = null };
            using (var conn = OpenConnection(csb))
                Assert.That(conn.ExecuteScalar("SELECT current_database()"), Is.EqualTo(csb.Username));
        }

        [Test, Description("Breaks a connector while it's in the pool, with a keepalive and without")]
        [TestCase(false, TestName = "BreakConnectorInPoolWithoutKeepAlive")]
        [TestCase(true, TestName = "BreakConnectorInPoolWithKeepAlive")]
        public void BreakConnectorInPool(bool keepAlive)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { MaxPoolSize = 1 };
            if (keepAlive)
                csb.KeepAlive = 1;
            using (var conn = new NpgsqlConnection(csb.ToString()))
            {
                conn.Open();
                var connectorId = conn.ProcessID;
                conn.Close();

                // Use another connection to kill the connector currently in the pool
                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({connectorId})");

                // Allow some time for the terminate to occur
                Thread.Sleep(2000);

                conn.Open();
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                if (keepAlive)
                {
                    Assert.That(conn.ProcessID, Is.Not.EqualTo(connectorId));
                    Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                }
                else
                {
                    Assert.That(conn.ProcessID, Is.EqualTo(connectorId));
                    Assert.That(() => conn.ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<NpgsqlException>());
                }
            }
        }

        #region ChangeDatabase

        [Test]
        public void ChangeDatabase()
        {
            using (var conn = OpenConnection())
            {
                conn.ChangeDatabase("template1");
                using (var cmd = new NpgsqlCommand("select current_database()", conn))
                    Assert.That(cmd.ExecuteScalar(), Is.EqualTo("template1"));
            }
        }

        [Test]
        public void ChangeDatabaseDoesNotAffectOtherConnections()
        {
            using (var conn1 = new NpgsqlConnection(ConnectionString))
            using (var conn2 = new NpgsqlConnection(ConnectionString))
            {
                // Connection 1 changes database
                conn1.Open();
                conn1.ChangeDatabase("template1");
                Assert.That(conn1.ExecuteScalar("SELECT current_database()"), Is.EqualTo("template1"));

                // Connection 2's database should not changed
                conn2.Open();
                Assert.That(conn2.ExecuteScalar("SELECT current_database()"), Is.Not.EqualTo(conn1.Database));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1331")]
        public void ChangeDatabaseConnectionNotOpen()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
                Assert.That(() => conn.ChangeDatabase("template1"), Throws.Exception
                    .TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Connection is not open"));
        }

        #endregion

        [Test, Description("Tests closing a connector while a reader is open")]
        [TestCase(true)]
        [TestCase(false)]
        [Timeout(10000)]
        public void CloseDuringRead(bool pooled)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            if (pooled)
                csb.MaxPoolSize = 1;
            else
                csb.Pooling = false;

            var conn = OpenConnection(csb);
            var connectorId = conn.ProcessID;
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                conn.Close();
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
                Assert.That(reader.IsClosed);
            }

            conn.Open();
            if (pooled)   // Make sure we can reuse the pooled connector
                Assert.That(conn.ProcessID, Is.EqualTo(connectorId));
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
            Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
        }

        [Test]
        public void SearchPath()
        {
            using (var conn = OpenConnection(new NpgsqlConnectionStringBuilder(ConnectionString) { SearchPath = "foo" }))
                Assert.That(conn.ExecuteScalar("SHOW search_path"), Contains.Substring("foo"));
        }

        [Test]
        public void ConnectorNotInitializedException1000581()
        {
            var command = new NpgsqlCommand();
            command.CommandText = @"SELECT 123";

            for (var i = 0; i < 2; i++)
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.Transaction = connection.BeginTransaction();
                    command.ExecuteScalar();
                    command.Transaction.Commit();
                }
            }
        }

        [Test]
        [Ignore("")]
        public void NpgsqlErrorRepro1()
        {
            throw new NotImplementedException();
#if WHAT_TO_DO_WITH_THIS
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var largeObjectMgr = new LargeObjectManager(connection);
                    try
                    {
                        var largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                        transaction.Commit();
                    }
                    catch
                    {
                        // ignore the LO failure
                    }
                } // *1* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read()); // *2* this fails if the initial connection is used
                    }
                }
            } // *3* sometimes it throws "System.NotSupportedException: This stream does not support seek operations"
#endif
        }

        [Test]
        public void Bug1011001()
        {
            //[#1011001] Bug in NpgsqlConnectionStringBuilder affects on cache and connection pool

            var csb1 = new NpgsqlConnectionStringBuilder(@"Server=server;Port=5432;User Id=user;Password=passwor;Database=database;");
            var cs1 = csb1.ToString();
            var csb2 = new NpgsqlConnectionStringBuilder(cs1);
            var cs2 = csb2.ToString();
            Assert.IsTrue(cs1 == cs2);
        }

        [Test]
        public void NpgsqlErrorRepro2()
        {
#if WHAT_TO_DO_WITH_THIS
            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();
            var largeObjectMgr = new LargeObjectManager(connection);
            try
            {
                var largeObject = largeObjectMgr.Open(-1, LargeObjectManager.READWRITE);
                transaction.Commit();
            }
            catch
            {
                // ignore the LO failure
                try
                {
                    transaction.Dispose();
                }
                catch
                {
                    // ignore dispose failure
                }
                try
                {
                    connection.Dispose();
                }
                catch
                {
                    // ignore dispose failure
                }
            }

            using (connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pg_database";
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        // *1* this fails if the connection for the pool happens to be the bad one from above
                        Assert.IsTrue(!String.IsNullOrEmpty((string)reader["datname"]));
                    }
                }
            }
#endif
        }

        [Test]
        public void GetConnectionState()
        {
            // Test created to PR #164

            var c = new NpgsqlConnection();
            c.Dispose();
            Assert.AreEqual(ConnectionState.Closed, c.State);
        }

        [Test]
        public void ChangeApplicationNameWithConnectionStringBuilder()
        {
            // Test for issue #165 on github.
            var builder = new NpgsqlConnectionStringBuilder();
            builder.ApplicationName = "test";
        }

        [Test, Description("Makes sure notices are probably received and emitted as events")]
        public void Notice()
        {
            if (Environment.GetEnvironmentVariable("TRAVIS") != null)
                Assert.Ignore("Test mysteriously fails on Travis only");
            using (var conn = OpenConnection())
            {
                // Make sure messages are in English
                conn.ExecuteNonQuery(@"SET lc_messages='en_US.UTF8'");
                conn.ExecuteNonQuery(@"
                        CREATE OR REPLACE FUNCTION pg_temp.emit_notice() RETURNS VOID AS
                        'BEGIN RAISE NOTICE ''testnotice''; END;'
                        LANGUAGE 'plpgsql';
                ");

                var mre = new ManualResetEvent(false);
                PostgresNotice notice = null;
                NoticeEventHandler action = (sender, args) =>
                {
                    notice = args.Notice;
                    mre.Set();
                };
                conn.Notice += action;
                try
                {
                    conn.ExecuteNonQuery("SELECT pg_temp.emit_notice()::TEXT"); // See docs for CreateSleepCommand
                    mre.WaitOne(5000);
                    Assert.That(notice, Is.Not.Null, "No notice was emitted");
                    Assert.That(notice.MessageText, Is.EqualTo("testnotice"));
                    Assert.That(notice.Severity, Is.EqualTo("NOTICE"));
                }
                finally
                {
                    conn.Notice -= action;
                }
            }
        }

        [Test, Description("Makes sure that concurrent use of the connection throws an exception")]
        public void ConcurrentUse()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                using (cmd.ExecuteReader())
                    Assert.That(() => conn.ExecuteScalar("SELECT 2"),
                        Throws.Exception.TypeOf<NpgsqlOperationInProgressException>()
                            .With.Property(nameof(NpgsqlOperationInProgressException.CommandInProgress)).SameAs(cmd));

                conn.ExecuteNonQuery("CREATE TEMP TABLE foo (bar INT)");
                using (conn.BeginBinaryImport("COPY foo (bar) FROM STDIN BINARY"))
                {
                    Assert.That(() => conn.ExecuteScalar("SELECT 2"),
                        Throws.Exception.TypeOf<NpgsqlOperationInProgressException>()
                            .With.Message.Contains("Copy"));
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void PersistSecurityInfoIsOn([Values(true, false)] bool pooling)
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                PersistSecurityInfo = true,
                Pooling = pooling
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                var passwd = new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password;
                Assert.That(passwd, Is.Not.Null);
                conn.Open();
                Assert.That(new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password, Is.EqualTo(passwd));
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void NoPasswordWithoutPersistSecurityInfo([Values(true, false)] bool pooling)
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = pooling
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
                Assert.That(csb.PersistSecurityInfo, Is.False);
                Assert.That(csb.Password, Is.Not.Null);
                conn.Open();
                Assert.That(new NpgsqlConnectionStringBuilder(conn.ConnectionString).Password, Is.Null);
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/743")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void Clone()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                ProvideClientCertificatesCallback callback1 = certificates => { };
                conn.ProvideClientCertificatesCallback = callback1;
                RemoteCertificateValidationCallback callback2 = (sender, certificate, chain, errors) => true;
                conn.UserCertificateValidationCallback = callback2;

                conn.Open();
#if NET451
                using (var conn2 = (NpgsqlConnection)((ICloneable)conn).Clone())
#else
                using (var conn2 = conn.Clone())
#endif
                {
                    Assert.That(conn2.ConnectionString, Is.EqualTo(conn.ConnectionString));
                    Assert.That(conn2.ProvideClientCertificatesCallback, Is.SameAs(callback1));
                    Assert.That(conn2.UserCertificateValidationCallback, Is.SameAs(callback2));
                    conn2.Open();
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/824")]
        [Explicit("Failing for some inexplicable reason on the build server on Linux only")]
        public void ReloadTypes()
        {
            using (var conn = OpenConnection())
            {
                Assert.That(conn.ExecuteScalar("SELECT EXISTS (SELECT * FROM pg_type WHERE typname='reload_types_enum')"), Is.False);
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.reload_types_enum AS ENUM ('First', 'Second')");
                Assert.That(() => conn.MapEnum<ReloadTypesEnum>(), Throws.Exception.TypeOf<NpgsqlException>());
                conn.ReloadTypes();
                conn.MapEnum<ReloadTypesEnum>();
            }
        }
        enum ReloadTypesEnum { First, Second };

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/736")]
        public void ManyOpenClose()
        {
            // The connector's _sentRfqPrependedMessages is a byte, too many open/closes made it overflow
            for (var i = 0; i < 255; i++)
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                }
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/736")]
        public void ManyOpenCloseWithTransaction()
        {
            // The connector's _sentRfqPrependedMessages is a byte, too many open/closes made it overflow
            for (var i = 0; i < 255; i++)
            {
                using (var conn = OpenConnection())
                    conn.BeginTransaction();
            }
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/927")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/736")]
        [Ignore("Fails when running the entire test suite but not on its own...")]
        public void RollbackOnClose()
        {
            // Npgsql 3.0.0 to 3.0.4 prepended a rollback for the next time the connector is used, as an optimization.
            // This caused some issues (#927) and was removed.

            // Clear connections in pool as we're going to need to reopen the same connection
            var dummyConn = new NpgsqlConnection(ConnectionString);
            NpgsqlConnection.ClearPool(dummyConn);

            int processId;
            using (var conn = OpenConnection())
            {
                processId = conn.Connector.BackendProcessId;
                conn.BeginTransaction();
                conn.ExecuteNonQuery("SELECT 1");
                Assert.That(conn.Connector.TransactionStatus, Is.EqualTo(TransactionStatus.InTransactionBlock));
            }
            using (var conn = OpenConnection())
            {
                Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(processId));
                Assert.That(conn.Connector.TransactionStatus, Is.EqualTo(TransactionStatus.Idle));
            }
        }

        [Test, Description("Tests an exception happening when sending the Terminate message while closing a ready connector")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/777")]
        [Ignore("Flaky")]
        public void ExceptionDuringClose()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };
            using (var conn = OpenConnection(csb))
            {
                var connectorId = conn.ProcessID;

                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({connectorId})");

                conn.Close();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1180")]
        [Ignore("Flaky")]
        public void PoolByPassword()
        {
            NpgsqlConnection goodConn = null;
            try
            {
                var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
                {
                    ApplicationName = nameof(PoolByPassword)
                };
                using (goodConn = new NpgsqlConnection(csb.ToString()))
                    goodConn.Open();
                csb.Password = "badpasswd";
                using (var conn = new NpgsqlConnection(csb.ToString()))
                    Assert.That(conn.Open, Throws.Exception.TypeOf<PostgresException>());
            }
            finally
            {
                if (goodConn != null)
                    NpgsqlConnection.ClearPool(goodConn);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1158")]
        public void TableNamedRecord()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TABLE record ()");
                try
                {
                    conn.ReloadTypes();
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM record"), Is.Zero);
                }
                finally
                {
                    conn.ExecuteNonQuery("DROP TABLE record");
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/392")]
        [LinuxIgnore]
        public void NonUTF8Encoding()
        {
            using (var adminConn = OpenConnection())
            {
                // Create the database with server encoding sql-ascii
                adminConn.ExecuteNonQuery("DROP DATABASE IF EXISTS sqlascii");
                adminConn.ExecuteNonQuery("CREATE DATABASE sqlascii ENCODING 'sql_ascii' TEMPLATE template0");
                try
                {
                    // Insert some win1252 data
                    var goodCsb = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = "sqlascii",
                        Encoding = "windows-1252",
                        ClientEncoding = "sql-ascii",
                        Pooling = false
                    };
                    using (var conn = OpenConnection(goodCsb))
                    {
                        conn.ExecuteNonQuery("CREATE TABLE foo (bar TEXT)");
                        conn.ExecuteNonQuery("INSERT INTO foo (bar) VALUES ('éàç')");
                        Assert.That(conn.ExecuteScalar("SELECT * FROM foo"), Is.EqualTo("éàç"));
                    }

                    // A normal connection with the default UTF8 encoding and client_encoding should fail
                    var badCsb = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = "sqlascii",
                        Pooling = false
                    };
                    using (var conn = OpenConnection(badCsb))
                    {
                        Assert.That(() => conn.ExecuteScalar("SELECT * FROM foo"),
                            Throws.Exception.TypeOf<PostgresException>()
                                .With.Property(nameof(PostgresException.SqlState)).EqualTo("22021")
                                .Or.TypeOf<DecoderFallbackException>()
                        );
                    }
                }
                finally
                {
                    adminConn.ExecuteNonQuery("DROP DATABASE IF EXISTS sqlascii");
                }
            }
        }

        [Test]
        public void OversizeBuffer()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(OversizeBuffer)
            };
            using (var conn = OpenConnection(csb))
            {
                Assert.That(conn.Connector.ReadBuffer.Size, Is.EqualTo(csb.ReadBufferSize));

                // Read a big row, we should now be using an oversize buffer
                var bigString1 = new string('x', csb.ReadBufferSize + 10);
                using (var cmd = new NpgsqlCommand($"SELECT '{bigString1}'", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetString(0), Is.EqualTo(bigString1));
                }
                var size1 = conn.Connector.ReadBuffer.Size;
                Assert.That(conn.Connector.ReadBuffer.Size, Is.GreaterThan(csb.ReadBufferSize));

                // Even bigger oversize buffer
                var bigString2 = new string('x', csb.ReadBufferSize + 20);
                using (var cmd = new NpgsqlCommand($"SELECT '{bigString2}'", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetString(0), Is.EqualTo(bigString2));
                }
                Assert.That(conn.Connector.ReadBuffer.Size, Is.GreaterThan(size1));

                var processId = conn.ProcessID;
                conn.Close();
                conn.Open();
                Assert.That(conn.ProcessID, Is.EqualTo(processId));
                Assert.That(conn.Connector.ReadBuffer.Size, Is.EqualTo(csb.ReadBufferSize));

            }
        }

        #region pgpass

        [Test]
        public void UsePgPassFile()
        {
            var file = SetupTestData();

            try
            {
                var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
                {
                    Pooling = false,
                    IntegratedSecurity = false,
                    Password = null
                };
                using (OpenConnection(builder)) {}
            }
            finally
            {
                RestorePriorConfiguration(file, _pgpassEnvVarValue);
            }
        }

        string _pgpassEnvVarValue;

        public string SetupTestData()
        {
            _pgpassEnvVarValue = Environment.GetEnvironmentVariable("PGPASSFILE");

            // set up pgpass file with connection credentials
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            var content = $"*:*:*:{builder.Username}:{builder.Password}";
            var pgpassFile = Path.GetTempFileName();
            File.WriteAllText(pgpassFile, content);
            Environment.SetEnvironmentVariable("PGPASSFILE", pgpassFile);
            return pgpassFile;
        }

        public void RestorePriorConfiguration(string fileName, string environmentVariableValue)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            Environment.SetEnvironmentVariable("PGPASSFILE", _pgpassEnvVarValue);
        }

        #endregion
    }
}
