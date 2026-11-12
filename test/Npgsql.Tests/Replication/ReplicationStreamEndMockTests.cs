using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Replication.Internal;
using Npgsql.Replication.TestDecoding;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.Support.ReplicationMock;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Tests how replication streams end, and what the client reports while streaming, against a mocked walsender.
/// </summary>
public class ReplicationStreamEndMockTests : TestBase
{
    static readonly NpgsqlLogSequenceNumber StartLsn = new(0x1000000);
    static readonly NpgsqlLogSequenceNumber SwitchLsn = new(0x1000003);

    [Test, Description("At the end of a historic timeline the server ends copy mode, then sends the next timeline")]
    public async Task Physical_stream_ends_at_the_end_of_the_timeline([Values] bool keepaliveAfterServerCopyDone)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token, timeline: 1).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000 TIMELINE 1").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, SwitchLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // We don't read the data, which mustn't confuse what follows
        moveNext = enumerator.MoveNextAsync().AsTask();
        server.WriteCopyDone();
        if (keepaliveAfterServerCopyDone)
            server.WriteKeepalive(SwitchLsn, replyRequested: true); // Must neither be answered nor break anything
        await server.FlushAsync();

        // A status update after our CopyDone would be a protocol violation, so CopyDone must come next
        await server.ExpectCopyDone().WaitAsync(ReplicationMock.Timeout);
        await WriteNextTimeline(server, nextTimeline: 2, SwitchLsn)
            .WriteCommandComplete("START_STREAMING")
            .WriteCommandComplete("START_REPLICATION")
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(await moveNext, Is.False);
        await enumerator.DisposeAsync();
        await AssertConnectionIsUsable(rc, server);
    }

    [Test, Description("The server doesn't enter copy mode when asked to start at the end of a historic timeline")]
    public async Task START_REPLICATION_at_the_end_of_a_historic_timeline()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(SwitchLsn, cts.Token, timeline: 1).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000003 TIMELINE 1").WaitAsync(ReplicationMock.Timeout);
        await WriteNextTimeline(server, nextTimeline: 2, SwitchLsn)
            .WriteCommandComplete("START_STREAMING")
            .WriteCommandComplete("START_REPLICATION")
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(await moveNext, Is.False);
        await enumerator.DisposeAsync();
        await AssertConnectionIsUsable(rc, server);
    }

    [Test, Description("A server error that ends the stream (e.g. after pg_cancel_backend) surfaces, also after a keepalive")]
    public async Task Stream_ended_by_a_server_error()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        // So that a wait for data that never comes times out within the test's timeout
        rc.WalReceiverTimeout = TimeSpan.FromSeconds(5);
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);
        var walEnd = new NpgsqlLogSequenceNumber(0x1000000 + 300);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, walEnd, new byte[300])
            .FlushAsync();
        Assert.That(await moveNext, Is.True);
        await enumerator.Current.Data.ReadExactlyAsync(new byte[300], cts.Token);

        // Everything the server sent has been read, so the keepalive arrives into an empty buffer, which makes positions from before
        // it stale. Then the server ends the command with an error that we didn't request.
        moveNext = enumerator.MoveNextAsync().AsTask();
        await server
            .WriteKeepalive(walEnd)
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(async () => await moveNext, Throws.Exception.TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.QueryCanceled));
        await enumerator.DisposeAsync();
        await AssertConnectionIsUsable(rc, server);
    }

    [Test, Description("A walsender that exits after everything has been replicated (e.g. at shutdown) sends 'COPY 0'")]
    public async Task Stream_ended_by_the_server()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteCommandComplete("COPY 0")
            .FlushAsync();
        server.Close();

        Assert.That(async () => await moveNext,
            Throws.Exception.TypeOf<NpgsqlException>().With.Message.Contains("ended the replication stream"));
        Assert.That(rc.Connector.IsBroken, Is.True);
        await enumerator.DisposeAsync();
    }

    [Test, Description("Logical replication skips unknown CopyData message types; both skip extra keepalive fields newer servers may add")]
    public async Task Unknown_message_content_is_skipped([Values] bool physical)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        ReplicationConnection rc;
        PgServerMock server;
        if (physical)
            (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        else
            (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = physical
            ? ((PhysicalReplicationConnection)rc).StartReplication(StartLsn, cts.Token).GetAsyncEnumerator()
            : ((LogicalReplicationConnection)rc).StartLogicalReplication(new TestDecodingReplicationSlot("s"), cts.Token, StartLsn)
                .GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        // A message type the client doesn't know
        var unknownMessage = new byte[1 + 8 + 8 + 8 + 8];
        unknownMessage[0] = (byte)'?';
        // A keepalive ('k': WAL end, clock and reply flag) with an additional 4-byte field
        var keepalive = new byte[1 + 8 + 8 + 1 + 4];
        keepalive[0] = (byte)'k';
        BinaryPrimitives.WriteUInt64BigEndian(keepalive.AsSpan(1), (ulong)StartLsn);
        server.WriteCopyBothResponse();
        if (!physical)
            server.WriteCopyData(unknownMessage);
        await server
            .WriteCopyData(keepalive)
            .WriteXLogData(StartLsn, SwitchLsn, "abc"u8)
            .FlushAsync();

        Assert.That(await moveNext, Is.True);
        Assert.That(enumerator.Current.WalStart, Is.EqualTo(StartLsn));
        Assert.That(enumerator.Current.Data.Length, Is.EqualTo(3));
        await StopWithCancellation(enumerator, postmaster, server, cts);
    }

    [Test, Description("Physical replication must not lose WAL, so an unknown CopyData message type breaks the connection")]
    public async Task Unknown_message_type_in_physical_replication()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteCopyData(new byte[] { (byte)'?', 0, 0, 0, 0 })
            .FlushAsync();

        Assert.That(async () => await moveNext, Throws.Exception.TypeOf<NpgsqlException>());
        Assert.That(rc.Connector.IsBroken, Is.True);
        await enumerator.DisposeAsync();
    }

    [Test, Description("A physical standby reports the end of the WAL it received, not XLogData's WAL end, which is the server's")]
    public async Task Physical_received_position()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, walEnd: new NpgsqlLogSequenceNumber(0x2000000), "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // A keepalive's WAL end is only the end of the WAL the server sent, so it doesn't move the reported position either
        moveNext = enumerator.MoveNextAsync().AsTask();
        await server.WriteKeepalive(new NpgsqlLogSequenceNumber(0x3000000), replyRequested: true).FlushAsync();
        var statusUpdate = await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        Assert.That(statusUpdate.WriteLsn, Is.EqualTo(new NpgsqlLogSequenceNumber(0x1000003)));
        Assert.That(rc.LastReceivedLsn, Is.EqualTo(new NpgsqlLogSequenceNumber(0x1000003)));

        await StopWithCancellation(enumerator, postmaster, server, cts, moveNext);
    }

    [Test, Description("A logical replication client reports the WAL end, like PostgreSQL's apply worker")]
    public async Task Logical_received_position()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartLogicalReplication(new TestDecodingReplicationSlot("s"), cts.Token, StartLsn).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, walEnd: new NpgsqlLogSequenceNumber(0x2000000), "BEGIN 1"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        moveNext = enumerator.MoveNextAsync().AsTask();
        await server.WriteKeepalive(new NpgsqlLogSequenceNumber(0x3000000), replyRequested: true).FlushAsync();
        var statusUpdate = await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        Assert.That(statusUpdate.WriteLsn, Is.EqualTo(new NpgsqlLogSequenceNumber(0x3000000)));

        await StopWithCancellation(enumerator, postmaster, server, cts, moveNext);
    }

    [Test, Description("What was received on a previous stream doesn't count for the next one, which may start lower")]
    public async Task Received_position_starts_over_with_each_stream()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        // A first stream up to 0/2000003, which ends at a timeline switch
        var enumerator = rc.StartReplication(new NpgsqlLogSequenceNumber(0x2000000), cts.Token, timeline: 1).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(new NpgsqlLogSequenceNumber(0x2000000), new NpgsqlLogSequenceNumber(0x2000003), "abc"u8)
            .WriteCopyDone()
            .FlushAsync();
        Assert.That(await moveNext, Is.True);
        moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectCopyDone().WaitAsync(ReplicationMock.Timeout);
        await WriteNextTimeline(server, nextTimeline: 2, new NpgsqlLogSequenceNumber(0x1000000))
            .WriteCommandComplete("START_STREAMING")
            .WriteCommandComplete("START_REPLICATION")
            .WriteReadyForQuery()
            .FlushAsync();
        Assert.That(await moveNext, Is.False);
        await enumerator.DisposeAsync();
        Assert.That(rc.LastReceivedLsn, Is.EqualTo(new NpgsqlLogSequenceNumber(0x2000003)));

        // The next stream starts lower, on the new timeline
        enumerator = rc.StartReplication(StartLsn, cts.Token, timeline: 2).GetAsyncEnumerator();
        moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000 TIMELINE 2").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteKeepalive(StartLsn, replyRequested: true)
            .FlushAsync();
        var statusUpdate = await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        Assert.That(statusUpdate.WriteLsn, Is.EqualTo(StartLsn));

        await StopWithCancellation(enumerator, postmaster, server, cts, moveNext);
    }

    static PgServerMock WriteNextTimeline(PgServerMock server, int nextTimeline, NpgsqlLogSequenceNumber nextTimelineStart)
        => server
            .WriteRowDescription(Column("next_tli", Int8Oid), Column("next_tli_startpos", TextOid))
            .WriteDataRow(Text(nextTimeline.ToString()), Text(nextTimelineStart.ToString()));

    static async Task AssertConnectionIsUsable(ReplicationConnection rc, PgServerMock server)
    {
        var identifyTask = rc.IdentifySystem();
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server.WriteIdentifySystemResponse("7000000000000000001", 2, SwitchLsn, dbName: null).FlushAsync();
        Assert.That((await identifyTask).Timeline, Is.EqualTo(2u));
    }

    /// <summary>
    /// Stops replication the way the server reacts to a cancellation.
    /// </summary>
    static async Task StopWithCancellation<T>(
        IAsyncEnumerator<T> enumerator,
        PgPostmasterMock postmaster,
        PgServerMock server,
        CancellationTokenSource cts,
        Task<bool>? pendingMoveNext = null)
    {
        var moveNext = pendingMoveNext ?? enumerator.MoveNextAsync().AsTask();
        cts.Cancel();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();
        Assert.That(async () => await moveNext, Throws.InstanceOf<OperationCanceledException>());
        await enumerator.DisposeAsync();
    }
}
