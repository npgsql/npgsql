using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.Support.ReplicationMock;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Tests replication commands other than START_REPLICATION against a mocked walsender.
/// </summary>
public class ReplicationCommandMockTests : TestBase
{
    [Test, Description("The history file content is raw bytes, whether the server declares it as bytea (< 14) or text (14+)")]
    public async Task TIMELINE_HISTORY([Values] bool declaredAsText)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        // Not valid UTF-8 and containing a backslash, both of which must survive
        byte[] content = [.."1\t0/3000000\tno recovery target \\ specified\n"u8, 0xFF];
        var historyTask = rc.TimelineHistory(2, cts.Token);
        await server.ExpectSimpleQuery("TIMELINE_HISTORY 2").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteRowDescription(Column("filename", TextOid), Column("content", declaredAsText ? TextOid : ByteaOid))
            .WriteDataRow(Text("00000002.history"), content)
            .WriteCommandComplete("TIMELINE_HISTORY")
            .WriteReadyForQuery()
            .FlushAsync();

        var history = await historyTask;
        Assert.That(history.FileName, Is.EqualTo("00000002.history"));
        Assert.That(history.Content, Is.EqualTo(content));
    }

    [Test, Description("SHOW ALL returns many rows, which must not break the connection")]
    public async Task Show_all()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var showTask = rc.Show("all", cts.Token);
        await server.ExpectSimpleQuery("SHOW all").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteRowDescription(Column("name", TextOid), Column("setting", TextOid), Column("description", TextOid))
            .WriteDataRow(Text("allow_alter_system"), Text("on"), Text("Allows running the ALTER SYSTEM command."))
            .WriteDataRow(Text("application_name"), Text("Npgsql"), Text("Sets the application name."))
            .WriteCommandComplete("SHOW")
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(async () => await showTask, Throws.InstanceOf<NotSupportedException>());

        // The connection can still be used
        var identifyTask = rc.IdentifySystem(cts.Token);
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server.WriteIdentifySystemResponse("7000000000000000001", 1, new(0x1000000), dbName: null).FlushAsync();
        Assert.That((await identifyTask).Timeline, Is.EqualTo(1u));
    }

    [Test, Description("The CommandTimeout property applies to replication commands, not just the connection string's")]
    public async Task CommandTimeout_is_honored(
        [Values] bool setBeforeOpen,
        [Values("IDENTIFY_SYSTEM", "DROP_REPLICATION_SLOT", "START_REPLICATION")] string command)
    {
        // The test connection string has Command Timeout=0, i.e. no timeout
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var rc = new PhysicalReplicationConnection(postmaster.ConnectionString);
        await using var _ = rc;
        if (setBeforeOpen)
            rc.CommandTimeout = TimeSpan.FromSeconds(1);
        await rc.Open();
        var server = await postmaster.WaitForServerConnection().AsTask().WaitAsync(ReplicationMock.Timeout);
        if (!setBeforeOpen)
            rc.CommandTimeout = TimeSpan.FromSeconds(1);

        // The mock never answers
        Task commandTask = command switch
        {
            "IDENTIFY_SYSTEM" => rc.IdentifySystem(),
            "DROP_REPLICATION_SLOT" => rc.DropReplicationSlot("s"),
            _ => rc.StartReplication(new NpgsqlLogSequenceNumber(0x1000000), CancellationToken.None).GetAsyncEnumerator().MoveNextAsync().AsTask()
        };
        try
        {
            Assert.That(async () => await commandTask.WaitAsync(ReplicationMock.Timeout),
                Throws.Exception.TypeOf<NpgsqlException>().With.InnerException.TypeOf<TimeoutException>());
        }
        finally
        {
            // Without a working timeout the command would wait forever, and so would disposing the connection
            if (!commandTask.IsCompleted)
                server.Close();
        }
    }

    [Test, Description("A timeout too large for the socket (e.g. TimeSpan.MaxValue) means no timeout")]
    public async Task CommandTimeout_too_large_for_a_socket()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        rc.CommandTimeout = TimeSpan.MaxValue;

        var identifyTask = rc.IdentifySystem();
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server.WriteIdentifySystemResponse("7000000000000000001", 1, new(0x1000000), dbName: null).FlushAsync();
        Assert.That((await identifyTask.WaitAsync(ReplicationMock.Timeout)).Timeline, Is.EqualTo(1u));
    }

    [Test]
    public async Task StartReplication_with_null_slot()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, _) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var __ = rc;

        Assert.That(() => rc.StartReplication((PhysicalReplicationSlot)null!, CancellationToken.None),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("slot"));
    }
}
