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
                await conn.WaitAsync();
                Assert.IsTrue(receivedNotification);
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task WaitAsyncCancellation()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("LISTEN notifytest");
                var cts = new CancellationTokenSource(1000);
                Assert.That(async () => await conn.WaitAsync(cts.Token), Throws.Exception.TypeOf<TaskCanceledException>());
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void WaitBreaksConnection()
        {
            using (var conn = OpenConnection())
            {
                new Timer(o =>
                {
                    using (var conn2 = OpenConnection())
                        conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");
                }, null, 500, Timeout.Infinite);

                Assert.That(() => conn.Wait(), Throws.Exception
                        .TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("57P01")
                );
                Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
            }
        }
   }
}