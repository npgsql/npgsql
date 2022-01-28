using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests;

public class NotificationTests : MultiplexingTestBase
{
    [Test, Description("Simple LISTEN/NOTIFY scenario")]
    public async Task Notification()
    {
        if (IsMultiplexing)
            return;

        using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        var receivedNotification = false;
        await conn.ExecuteNonQueryAsync("LISTEN notifytest");
        conn.Notification += (o, e) => receivedNotification = true;
        await conn.ExecuteNonQueryAsync("NOTIFY notifytest");
        Assert.IsTrue(receivedNotification);
    }

    //[Test, Description("Generates a notification that arrives after reader data that is already being read")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/252")]
    public async Task Notification_after_data()
    {
        var receivedNotification = false;
        using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "LISTEN notifytest1";
        await cmd.ExecuteNonQueryAsync();
        conn.Notification += (o, e) => receivedNotification = true;

        cmd.CommandText = "SELECT generate_series(1,10000)";
        using (var reader = cmd.ExecuteReader())
        {
            //After "notify notifytest1", a notification message will be sent to client,
            //And so the notification message will stick with the last response message of "select generate_series(1,10000)" in Npgsql's tcp receiving buffer.
            using (var conn2 = new NpgsqlConnection(ConnectionString))
            {
                await conn2.OpenAsync();
                using (var command = conn2.CreateCommand())
                {
                    command.CommandText = "NOTIFY notifytest1";
                    await command.ExecuteNonQueryAsync();
                }
            }

            // Allow some time for the notification to get delivered
            Thread.Sleep(2000);

            Assert.IsTrue(await reader.ReadAsync());
            Assert.AreEqual(1, reader.GetValue(0));
        }

        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        Assert.IsTrue(receivedNotification);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1024")]
    [Timeout(10000)]
    public void Wait()
    {
        if (IsMultiplexing)
            return;

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        using var notifyingConn = new NpgsqlConnection(ConnectionString);
        notifyingConn.Open();

        var receivedNotification = false;
        conn.ExecuteNonQuery("LISTEN notifytest");
        notifyingConn.ExecuteNonQuery("NOTIFY notifytest");
        conn.Notification += (o, e) => receivedNotification = true;
        Assert.That(conn.Wait(0), Is.EqualTo(true));
        Assert.IsTrue(receivedNotification);
        Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1024")]
    //[Timeout(10000)]
    public void Wait_with_timeout()
    {
        if (IsMultiplexing)
            return;

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        Assert.That(conn.Wait(100), Is.EqualTo(false));
        Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public void Wait_with_prepended_message()
    {
        if (IsMultiplexing)
            return;


        // A DISCARD ALL is now prepended in the connection's write buffer
        using var c = new NpgsqlConnection(ConnectionString);
        c.Open();

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        Assert.That(conn.Wait(100), Is.EqualTo(false));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1024")]
    [Timeout(10000)]
    public async Task WaitAsync()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        using var notifyingConn = new NpgsqlConnection(ConnectionString);
        await notifyingConn.OpenAsync();

        var receivedNotification = false;
        await conn.ExecuteNonQueryAsync("LISTEN notifytest");
        conn.Notification += (o, e) => receivedNotification = true;
        await notifyingConn.ExecuteNonQueryAsync("NOTIFY notifytest");
        await conn.WaitAsync(0);
        Assert.IsTrue(receivedNotification);
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task WaitAsync_with_timeout()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        var notification = await conn.WaitAsync(100);

        Assert.That(notification, Is.EqualTo(false));
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task WaitAsync_does_not_allow_concurrent_actions()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        using var cts = new CancellationTokenSource();
        var notificationTask = conn.WaitAsync(cts.Token);
        Assert.ThrowsAsync<NpgsqlOperationInProgressException>(async () =>
        {
            try
            {
                await conn.ExecuteScalarAsync("SELECT 1");
            }
            finally
            {
                cts.Cancel();
            }
        });

        // A safeguard against closing an active connection
        try
        {
            await notificationTask;
        }
        catch (OperationCanceledException)
        {

        }
    }

    [Test]
    public async Task Wait_with_keepalive()
    {
        if (IsMultiplexing)
            return;

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            KeepAlive = 1,
            Pooling = false
        };

        using var conn = new NpgsqlConnection(csb.ToString());
        conn.Open();

        using var notifyingConn = new NpgsqlConnection(csb.ToString());
        notifyingConn.Open();

        conn.ExecuteNonQuery("LISTEN notifytest");
        var notificationTask = Task.Delay(2000).ContinueWith(t => notifyingConn.ExecuteNonQuery("NOTIFY notifytest"));
        conn.Wait();
        Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
        // A safeguard against closing an active connection
        await notificationTask;
        //Assert.That(TestLoggerSink.Records, Has.Some.With.Property("EventId").EqualTo(new EventId(NpgsqlEventId.Keepalive)));
    }

    [Test]
    public async Task WaitAsync_with_keepalive()
    {
        if (IsMultiplexing)
            return;

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            KeepAlive = 1,
        };
        using var conn = new NpgsqlConnection(csb.ToString());
        await conn.OpenAsync();

        using var notifyingConn = new NpgsqlConnection(csb.ToString());
        await notifyingConn.OpenAsync();

        await conn.ExecuteNonQueryAsync("LISTEN notifytest");
        var notificationTask = Task.Delay(2000).ContinueWith(t => notifyingConn.ExecuteNonQueryAsync("NOTIFY notifytest")).Unwrap();
        await conn.WaitAsync();
        //Assert.That(TestLoggerSink.Records, Has.Some.With.Property("EventId").EqualTo(new EventId(NpgsqlEventId.Keepalive)));
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        // A safeguard against closing an active connection
        await notificationTask;
    }

    // [Test]
    public async Task WaitAsync_cancellation()
    {
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            Assert.That(async () => await conn.WaitAsync(new CancellationToken(true)),
                Throws.Exception.TypeOf<OperationCanceledException>());
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            await conn.ExecuteNonQueryAsync("LISTEN notifytest");
            var cts = new CancellationTokenSource(1000);
            Assert.That(async () => await conn.WaitAsync(cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }
    }

    [Test]
    public void Wait_breaks_connection()
    {
        if (IsMultiplexing)
            return;

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        Task.Delay(1000).ContinueWith(t =>
        {
            using var conn2 = new NpgsqlConnection(ConnectionString);
            conn2.Open();
            conn2.ExecuteNonQuery($"SELECT pg_terminate_backend({conn.ProcessID})");
        });

        Assert.That(() => conn.Wait(), Throws.Exception.TypeOf<PostgresException>());
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
    }

    [Test]
    public async Task WaitAsync_breaks_connection()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        var waitTask = conn.WaitAsync(); // We have a bound connector now.

        await Task.Delay(1000);

        using var conn2 = new NpgsqlConnection(ConnectionString);
        await conn2.OpenAsync();
        await conn2.ExecuteNonQueryAsync($"SELECT pg_terminate_backend({conn.ProcessID})");

        Assert.That(async () => await waitTask, Throws.Exception.TypeOf<PostgresException>());
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
    }

    public NotificationTests(MultiplexingMode multiplexingMode) : base(multiplexingMode)
    {
    }
}
