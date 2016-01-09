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
using System.Net.Security;
using System.Text.RegularExpressions;
using NpgsqlTypes;

namespace Npgsql.Tests
{
    public class ConnectionTests : TestBase
    {
        public ConnectionTests(string backendVersion) : base(backendVersion) { }

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
                    Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception.TypeOf<IOException>());

                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
                Assert.That(eventBroken, Is.True);
            }
        }

        #region Connection Errors

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConnectionRefused(bool pooled)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Port = 44444, Pooling = pooled };
            using (var conn = new NpgsqlConnection(csb)) {
                Assert.That(() => conn.Open(), Throws.Exception
                    .TypeOf<SocketException>()
                    .With.Property("SocketErrorCode").EqualTo(SocketError.ConnectionRefused)
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
                    .With.Property("SocketErrorCode").EqualTo(SocketError.ConnectionRefused)
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        [Ignore("Fails in a non-determinstic manner and only on the build server... investigate...")]
        public void InvalidUserId()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Username = "unknown", Pooling = false };
            using (var conn = new NpgsqlConnection(csb))
            {
                Assert.That(conn.Open, Throws.Exception
                    .TypeOf<NpgsqlException>()
                    .With.Property("Code").EqualTo("28P01")
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test, Description("Connects with a bad password to ensure the proper error is thrown")]
        public void AuthenticationFailure()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Password = "bad", Pooling = false };
            using (var conn = new NpgsqlConnection(csb))
            {
                Assert.That(() => conn.Open(), Throws.Exception
                    .TypeOf<NpgsqlException>()
                    .With.Property("Code").EqualTo("28P01")
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Closed));
            }
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
                    var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        Database = dbName,
                        Pooling = false
                    };

                    using (var conn2 = new NpgsqlConnection(csb))
                    {
                        Assert.That(() => conn2.Open(),
                            Throws.Exception.TypeOf<NpgsqlException>()
                            .With.Property("Code").EqualTo("3D000") // database doesn't exist
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
        public void NoUsername()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Username = null };
            using (var conn = new NpgsqlConnection(csb))
                Assert.That(() => conn.Open(), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        [Timeout(10000)]
        public void ConnectTimeout()
        {
            var unknownIp = Environment.GetEnvironmentVariable("NPGSQL_UNKNOWN_IP");
            if (unknownIp == null)
                TestUtil.IgnoreExceptOnBuildServer("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) {
                Host = unknownIp,
                Pooling = false,
                Timeout = 2
            };
            using (var conn = new NpgsqlConnection(csb))
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
                TestUtil.IgnoreExceptOnBuildServer("NPGSQL_UNKNOWN_IP isn't defined and is required for connection timeout tests");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = unknownIp,
                Pooling = false,
                Timeout = 2
            };
            using (var conn = new NpgsqlConnection(csb))
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
                TestUtil.IgnoreExceptOnBuildServer("NPGSQL_UNKNOWN_IP isn't defined and is required for connection cancellation tests");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Host = unknownIp,
                Pooling = false,
                Timeout = 30
            };
            using (var conn = new NpgsqlConnection(csb))
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(1000);
                Assert.That(async () => await conn.OpenAsync(cts.Token), Throws.Exception.TypeOf<TaskCanceledException>());
                Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        #endregion

        #region Notification

        [Test, Description("Simple synchronous LISTEN/NOTIFY scenario")]
        public void NotificationSync()
        {
            using (var conn = OpenConnection())
            {
                var receivedNotification = false;
                conn.ExecuteNonQuery("LISTEN notifytest");
                conn.Notification += (o, e) => receivedNotification = true;
                conn.ExecuteNonQuery("NOTIFY notifytest");
                Assert.IsTrue(receivedNotification);
            }
        }

        [Test, Description("An asynchronous LISTEN/NOTIFY scenario")]
        [Timeout(10000)]
        public void NotificationAsync()
        {
            using (var notifyingConn = OpenConnection())
            {
                var mre = new ManualResetEvent(false);
                using (var listeningConn = new NpgsqlConnection(ConnectionString + ";ContinuousProcessing=true"))
                {
                    listeningConn.Open();
                    listeningConn.ExecuteNonQuery("LISTEN notifytest2");
                    listeningConn.Notification += (o, e) => mre.Set();

                    // Send notify via the other connection
                    notifyingConn.ExecuteNonQuery("NOTIFY notifytest2");
                    mre.WaitOne();

                    // And again
                    mre.Reset();
                    notifyingConn.ExecuteNonQuery("NOTIFY notifytest2");
                    mre.WaitOne();
                }
            }
        }

        [Test, Description("A notification arriving while we have an open Reader")]
        public void NotificationDuringReader()
        {
            using (var notifyingConn = OpenConnection())
            {
                var receivedNotification = false;
                using (var listeningConn = new NpgsqlConnection(ConnectionString + ";ContinuousProcessing=true"))
                {
                    listeningConn.Open();
                    listeningConn.ExecuteNonQuery("LISTEN notifytest2");
                    listeningConn.Notification += (o, e) => receivedNotification = true;

                    using (var cmd = new NpgsqlCommand("SELECT 1", listeningConn))
                    using (cmd.ExecuteReader())
                    {
                        // Send notify via the other connection
                        notifyingConn.ExecuteNonQuery("NOTIFY notifytest2");
                        Thread.Sleep(500);
                    }
                }
                Assert.That(receivedNotification, Is.True);
            }
        }

        [Test, Description("Receive an asynchronous notification when a message has already been prepended")]
        [Timeout(10000)]
        public void NotificationAsyncWithPrepend()
        {
            using (var notifyingConn = OpenConnection())
            {
                var mre = new ManualResetEvent(false);
                using (var listeningConn = new NpgsqlConnection(ConnectionString + ";ContinuousProcessing=true"))
                {
                    listeningConn.Open();
                    listeningConn.ExecuteNonQuery("LISTEN notifytest2");
                    listeningConn.BeginTransaction();

                    // Send notify via the other connection
                    listeningConn.Notification += (o, e) => mre.Set();
                    notifyingConn.ExecuteNonQuery("NOTIFY notifytest2");
                    mre.WaitOne();
                }
            }
        }

        [Test, Description("Generates a notification that arrives after reader data that is already being read")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/252")]
        public void NotificationAfterData()
        {
            var receivedNotification = false;
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "LISTEN notifytest1";
                cmd.ExecuteNonQuery();
                conn.Notification += (o, e) => receivedNotification = true;

                cmd.CommandText = "SELECT generate_series(1,10000)";
                using (var reader = cmd.ExecuteReader())
                {

                    //After "notify notifytest1", a notification message will be sent to client,
                    //And so the notification message will stick with the last response message of "select generate_series(1,10000)" in Npgsql's tcp receiving buffer.
                    using (var conn2 = new NpgsqlConnection(ConnectionString))
                    {
                        conn2.Open();
                        using (var command = conn2.CreateCommand())
                        {
                            command.CommandText = "NOTIFY notifytest1";
                            command.ExecuteNonQuery();
                        }
                    }

                    // Allow some time for the notification to get delivered
                    Thread.Sleep(2000);

                    Assert.IsTrue(reader.Read());
                    Assert.AreEqual(1, reader.GetValue(0));
                }

                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                Assert.IsTrue(receivedNotification);
            }
        }

        #endregion

        #region Keepalive

        [Test, Description("Makes sure that if keepalive is enabled, broken connections are detected")]
        [Timeout(10000)]
        public void Keepalive()
        {
            using (var conn1 = OpenConnection())
            {
                var mre = new ManualResetEvent(false);
                using (var conn2 = new NpgsqlConnection(ConnectionString + ";KeepAlive=1;ContinuousProcessing=true"))
                {
                    conn2.Open();

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
        }

        #endregion

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/903")]
        public void DataSource()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
                Assert.That(conn.DataSource, Is.EqualTo($"tcp://{conn.Host}:{conn.Port}"));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/703")]
        public void NoDatabaseDefaultsToUsername()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Database = null };
            using (var conn = OpenConnection(csb))
                Assert.That(conn.ExecuteScalar("SELECT current_database()"), Is.EqualTo(csb.Username));
        }

        [Test, Description("Breaks a connector while it's in the pool, with a keepalive and without")]
        [TestCase(false, TestName = "WithoutKeepAlive")]
        [TestCase(false, TestName = "WithKeepAlive")]
        public void BreakConnectorInPool(bool keepAlive)
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { MaxPoolSize = 1 };
            if (keepAlive)
                csb.KeepAlive = 1;

            using (var conn = new NpgsqlConnection(csb))
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
                Assert.That(conn.ProcessID, Is.EqualTo(connectorId));
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
                if (keepAlive)
                    Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                else
                    Assert.That(() => conn.ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<IOException>());
            }
        }

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
        public void ChangeDatabaseTestConnectionCache()
        {
            using (var conn1 = new NpgsqlConnection(ConnectionString))
            using (var conn2 = new NpgsqlConnection(ConnectionString))
            {
                //    connection 1 change database
                conn1.Open();
                conn1.ChangeDatabase("template1");
                var command = new NpgsqlCommand("select current_database()", conn1);
                var db1 = (String)command.ExecuteScalar();
                Assert.AreEqual("template1", db1);

                //    connection 2 's database should not changed, so should different from conn1
                conn2.Open();
                command = new NpgsqlCommand("select current_database()", conn2);
                var db2 = (String)command.ExecuteScalar();
                Assert.AreNotEqual(db1, db2);
            }
        }

        [Test]
        public void NestedTransaction()
        {
            using (var conn = OpenConnection())
            {
                conn.BeginTransaction();
                Assert.That(() => conn.BeginTransaction(), Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void BeginTransactionBeforeOpen()
        {
            using (var conn = new NpgsqlConnection())
                Assert.That(() => conn.BeginTransaction(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void SequencialTransaction()
        {
            using (var conn = OpenConnection())
            {
                conn.BeginTransaction().Rollback();
                conn.BeginTransaction();
            }
        }

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
        public void GetSchemaForeignKeys()
        {
            using (var conn = OpenConnection())
            {
                var dt = conn.GetSchema("ForeignKeys");
                Assert.IsNotNull(dt);
            }
        }

        [Test]
        public void GetSchemaParameterMarkerFormats()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data; CREATE TABLE data (int INTEGER);");
                conn.ExecuteNonQuery("INSERT INTO data (int) VALUES (4)");
                var dt = conn.GetSchema("DataSourceInformation");
                var parameterMarkerFormat = (string) dt.Rows[0]["ParameterMarkerFormat"];

                using (var conn2 = new NpgsqlConnection(ConnectionString))
                {
                    conn2.Open();
                    using (var command = conn2.CreateCommand())
                    {
                        const String parameterName = "@p_int";
                        command.CommandText = "SELECT * FROM data WHERE int=" +
                                              String.Format(parameterMarkerFormat, parameterName);
                        command.Parameters.Add(new NpgsqlParameter(parameterName, 4));
                        using (var reader = command.ExecuteReader())
                        {
                            Assert.IsTrue(reader.Read());
                            // This is OK, when no exceptions are occurred.
                        }
                    }
                }
            }
        }

        [Test]
        public void GetConnectionState()
        {
            // Test created to PR #164

            NpgsqlConnection c = new NpgsqlConnection();
            c.Dispose();
            Assert.AreEqual(ConnectionState.Closed, c.State);
        }

        [Test]
        public void ChangeApplicationNameWithConnectionStringBuilder()
        {
            // Test for issue #165 on github.
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();
            builder.ApplicationName = "test";
        }

        [Test, Description("Makes sure notices are probably received and emitted as events")]
        public void Notice()
        {
            using (var conn = OpenConnection())
            {
                // Make sure messages are in English
                conn.ExecuteNonQuery(@"SET lc_messages='English_United States.1252'");
                conn.ExecuteNonQuery(@"
                        CREATE OR REPLACE FUNCTION pg_temp.emit_notice() RETURNS VOID AS
                        'BEGIN RAISE NOTICE ''testnotice''; END;'
                        LANGUAGE 'plpgsql';
                ");

                NpgsqlNotice notice = null;
                NoticeEventHandler action = (sender, args) => notice = args.Notice;
                conn.Notice += action;
                try
                {
                    conn.ExecuteNonQuery("SELECT pg_temp.emit_notice()::TEXT"); // See docs for CreateSleepCommand
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
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            using (cmd.ExecuteReader())
                Assert.That(() => conn.ExecuteScalar("SELECT 1"), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void NoContinuousProcessingWithSslStream()
        {
            using (var conn = new NpgsqlConnection(ConnectionString + ";UseSslStream=true;ContinuousProcessing=true"))
                Assert.That(() => conn.Open(), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/783")]
        public void PersistSecurityInfoIsOn()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) { PersistSecurityInfo = true };
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
        public void NoPasswordWithoutPersistSecurityInfo()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
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
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };
            using (var conn = new NpgsqlConnection(connString))
            {
                ProvideClientCertificatesCallback callback1 = certificates => { };
                conn.ProvideClientCertificatesCallback = callback1;
                RemoteCertificateValidationCallback callback2 = (sender, certificate, chain, errors) => true;
                conn.UserCertificateValidationCallback = callback2;

                conn.Open();
                using (var conn2 = (NpgsqlConnection) ((ICloneable) conn).Clone())
                {
                    Assert.That(conn2.ConnectionString, Is.EqualTo(conn.ConnectionString));
                    Assert.That(conn2.ProvideClientCertificatesCallback, Is.SameAs(callback1));
                    Assert.That(conn2.UserCertificateValidationCallback, Is.SameAs(callback2));
                    conn2.Open();
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/824")]
        public void ReloadTypes()
        {
            using (var conn1 = OpenConnection())
            using (var conn2 = OpenConnection())
            {
                Assert.That(conn2.ExecuteScalar("SELECT EXISTS (SELECT * FROM pg_type WHERE typname='reload_types_enum')"), Is.False);
                conn2.ExecuteNonQuery("CREATE TYPE pg_temp.reload_types_enum AS ENUM ('First', 'Second')");
                conn1.ReloadTypes();
                conn1.MapEnum<ReloadTypesEnum>("reload_types_enum");
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
        public void ExceptionDuringClose()
        {
            using (var conn = OpenConnection(new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false }))
            {
                var connectorId = conn.ProcessID;

                using (var conn2 = OpenConnection())
                    conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({connectorId})");

                conn.Close();
            }
        }

        #region GetSchema

        [Test]
        public void GetSchema()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema();
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more metadatacollections returned. No connectionstring is required.");
            }
        }

        [Test]
        public void GetSchemaWithDbMetaDataCollectionNames()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more metadatacollections returned.");
                foreach (DataRow row in metaDataCollections.Rows)
                {
                    var collectionName = (string)row["CollectionName"];
                    //checking this collection
                    if (collectionName != System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections)
                    {
                        var collection = conn.GetSchema(collectionName);
                        Assert.IsNotNull(collection, "Each of the advertised metadata collections should work");
                    }
                }
            }
        }

        [Test]
        public void GetSchemaWithRestrictions()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more Restrictions returned.");
            }
        }

        [Test]
        public void GetSchemaWithReservedWords()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more ReservedWords returned.");
            }
        }

        #endregion
    }
}
