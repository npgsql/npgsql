﻿using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class NotificationTests : TestBase
    {
        [Test, Description("Simple LISTEN/NOTIFY scenario")]
        public void Notification()
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

        //[Test, Description("Generates a notification that arrives after reader data that is already being read")]
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

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1024")]
        [Timeout(10000)]
        public void Wait()
        {
            using (var conn = OpenConnection())
            using (var notifyingConn = OpenConnection())
            {
                var receivedNotification = false;
                conn.ExecuteNonQuery("LISTEN notifytest");
                notifyingConn.ExecuteNonQuery("NOTIFY notifytest");
                conn.Notification += (o, e) => receivedNotification = true;
                Assert.That(conn.Wait(0), Is.EqualTo(true));
                Assert.IsTrue(receivedNotification);
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1024")]
        //[Timeout(10000)]
        public void WaitWithTimeout()
        {
            using (var conn = OpenConnection())
            {
                Assert.That(conn.Wait(100), Is.EqualTo(false));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void WaitWithPrependedMessage()
        {
            using (OpenConnection()) {}  // A DISCARD ALL is now prepended in the connection's write buffer
            using (var conn = OpenConnection())
                Assert.That(conn.Wait(100), Is.EqualTo(false));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1024")]
        [Timeout(10000)]
        public async Task WaitAsync()
        {
            using (var conn = OpenConnection())
            using (var notifyingConn = OpenConnection())
            {
                var receivedNotification = false;
                conn.ExecuteNonQuery("LISTEN notifytest");
                notifyingConn.ExecuteNonQuery("NOTIFY notifytest");
                conn.Notification += (o, e) => receivedNotification = true;
                await conn.WaitAsync(0);
                Assert.IsTrue(receivedNotification);
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void WaitAsyncWithTimeout()
        {
            using var conn = OpenConnection();
            Assert.That(async () => await conn.WaitAsync(100), Is.EqualTo(false));
            Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
        }

        [Test]
        public async Task WaitWithKeepalive()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                KeepAlive = 1,
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            using (var notifyingConn = OpenConnection())
            {
                conn.ExecuteNonQuery("LISTEN notifytest");
                var notificationTask = Task.Delay(2000).ContinueWith(t => notifyingConn.ExecuteNonQuery("NOTIFY notifytest"));
                conn.Wait();
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                // A safeguard against closing an active connection
                await notificationTask;
            }
            //Assert.That(TestLoggerSink.Records, Has.Some.With.Property("EventId").EqualTo(new EventId(NpgsqlEventId.Keepalive)));
        }

        [Test]
        public async Task WaitAsyncWithKeepalive()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                KeepAlive = 1,
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            using (var notifyingConn = OpenConnection())
            {
                conn.ExecuteNonQuery("LISTEN notifytest");
                var notificationTask = Task.Delay(2000).ContinueWith(t => notifyingConn.ExecuteNonQuery("NOTIFY notifytest"));
                await conn.WaitAsync();
                //Assert.That(TestLoggerSink.Records, Has.Some.With.Property("EventId").EqualTo(new EventId(NpgsqlEventId.Keepalive)));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                // A safeguard against closing an active connection
                await notificationTask;
            }
        }

        [Test]
        public void WaitAsyncCancellation()
        {
            using (var conn = OpenConnection())
            {
                Assert.That(async () => await conn.WaitAsync(new CancellationToken(true)),
                    Throws.Exception.TypeOf<OperationCanceledException>());
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("LISTEN notifytest");
                var cts = new CancellationTokenSource(1000);
                Assert.That(async () => await conn.WaitAsync(cts.Token),
                    Throws.Exception.TypeOf<OperationCanceledException>());
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void WaitBreaksConnection()
        {
            using (var conn = OpenConnection())
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    using (var conn2 = OpenConnection())
                        conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");
                });

                Assert.That(() => conn.Wait(), Throws.Exception.TypeOf<PostgresException>());
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }

        [Test]
        public void WaitAsyncBreaksConnection()
        {
            using (var conn = OpenConnection())
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    using (var conn2 = OpenConnection())
                        conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");
                });

                Assert.That(async () => await conn.WaitAsync(), Throws.Exception.TypeOf<PostgresException>());
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }
    }
}
