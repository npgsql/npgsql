using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Replication;
using Npgsql.Replication.Internal;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Replication.TestDecoding;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.Support.PgOutputMessageBuilder;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Tests stopping replication streams early (break, exceptions, cancellation) against a mocked walsender.
/// </summary>
public class ReplicationStopMockTests : TestBase
{
    static readonly NpgsqlLogSequenceNumber StartLsn = new(0x1000000);

    [Test, Description("Stopping a physical stream early ends copy mode, so that the connection can be used again")]
    public async Task Physical_stream_stopped_early([Values] bool readSomeData)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var consumer = Task.Run(async () =>
        {
            await foreach (var message in rc.StartReplication(StartLsn, cts.Token))
            {
                if (readSomeData)
                    await message.Data.ReadExactlyAsync(new byte[100], cts.Token);
                break;
            }
        });

        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        // More data than fits into the read buffer, so that the rest isn't buffered yet when the consumer stops
        server.WriteCopyBothResponse();
        await server.WriteXLogDataWithFlush(StartLsn, new NpgsqlLogSequenceNumber(0x1000000 + 20000), new byte[20000]);
        await server.FlushAsync();

        // A final status update, then copy mode ends. The consumer stopped while it held the message, so the message doesn't
        // count as received.
        var statusUpdate = await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        Assert.That(statusUpdate.WriteLsn, Is.EqualTo(StartLsn));
        await server.ExpectCopyDone().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteKeepalive(StartLsn) // Still on its way when the server got our CopyDone
            .WriteCopyDone()
            .WriteCommandComplete("START_STREAMING")
            .WriteCommandComplete("START_REPLICATION")
            .WriteReadyForQuery()
            .FlushAsync();
        await consumer.WaitAsync(ReplicationMock.Timeout);

        await AssertConnectionIsUsable(rc, server);
    }

    [Test, Description("Stopping a logical stream early cancels the command, so that the connection can be used again")]
    public async Task Logical_stream_stopped_early()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var consumer = Task.Run(async () =>
        {
            await foreach (var _ in rc.StartLogicalReplication(new TestDecodingReplicationSlot("s"), cts.Token, StartLsn))
                break;
        });

        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "BEGIN 1"u8)
            .FlushAsync();

        // A final status update, then a cancellation, since a logical walsender can't stream again after CopyDone
        await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteXLogData(StartLsn, StartLsn, "COMMIT 1"u8) // Still on its way when the server got the cancellation
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();
        await consumer.WaitAsync(ReplicationMock.Timeout);

        await AssertConnectionIsUsable(rc, server);
    }

    [Test, Description("The exception that made the consumer stop surfaces, even if stopping the stream fails")]
    public async Task Consumer_exception_is_not_replaced()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var consumer = Task.Run(async () =>
        {
            await foreach (var _ in rc.StartReplication(StartLsn, cts.Token))
                throw new ConsumerException();
        });

        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();

        // The server goes away instead of ending copy mode
        await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        server.Close();

        Assert.That(async () => await consumer.WaitAsync(ReplicationMock.Timeout), Throws.TypeOf<ConsumerException>());
        Assert.That(rc.Connector.IsBroken, Is.True);
    }

    [Test, Description("After cancellation, messages that are already on their way aren't handed out")]
    public async Task No_messages_after_cancellation()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var timeoutCts = new CancellationTokenSource(ReplicationMock.Timeout);
        using var streamingCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token);

        var enumerator = rc.StartReplication(StartLsn, streamingCts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        streamingCts.Cancel();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteXLogData(new NpgsqlLogSequenceNumber(0x1000003), new NpgsqlLogSequenceNumber(0x1000003), "def"u8)
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(async () => await enumerator.MoveNextAsync(), Throws.InstanceOf<OperationCanceledException>());
        await enumerator.DisposeAsync();
        await AssertConnectionIsUsable(rc, server);
    }

    [Test, Description("Starting a second stream while one is in progress fails without disturbing the first one")]
    public async Task Second_stream_while_one_is_in_progress()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var first = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = first.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        var second = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        Assert.That(async () => await second.MoveNextAsync(), Throws.TypeOf<NpgsqlOperationInProgressException>());
        await second.DisposeAsync();

        // Disposing the connection still stops the first stream properly, with a cancellation
        var dispose = rc.DisposeAsync().AsTask();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();
        await server.ExpectTerminate().WaitAsync(ReplicationMock.Timeout);
        await dispose.WaitAsync(ReplicationMock.Timeout);
    }

    [Test, Description("A message bigger than the read buffer (e.g. a notice) makes the connector switch buffers mid-stream")]
    public async Task Oversize_message_during_streaming()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var timeoutCts = new CancellationTokenSource(ReplicationMock.Timeout);
        using var streamingCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token);

        var enumerator = rc.StartReplication(StartLsn, streamingCts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        server.WriteCopyBothResponse();
        await server.WriteBuffer.WriteBytesRaw(OversizeNotice(), async: true);
        await server
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();

        Assert.That(await moveNext, Is.True);
        var data = new byte[3];
        await enumerator.Current.Data.ReadExactlyAsync(data, timeoutCts.Token);
        Assert.That(data, Is.EqualTo("abc"u8.ToArray()));

        moveNext = enumerator.MoveNextAsync().AsTask();
        streamingCts.Cancel();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();
        Assert.That(async () => await moveNext, Throws.InstanceOf<OperationCanceledException>());
        await enumerator.DisposeAsync();
    }

    [Test, Description("pgoutput reads from the bigger buffer the connector switches to after an oversize message mid-stream")]
    public async Task Oversize_message_during_pgoutput_streaming()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var timeoutCts = new CancellationTokenSource(ReplicationMock.Timeout);
        using var streamingCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token);
        var commitLsn = new NpgsqlLogSequenceNumber(0x1000100);

        var enumerator = rc.StartReplication(
                new PgOutputReplicationSlot(new ReplicationSlotOptions("s", StartLsn)),
                new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V1),
                streamingCts.Token)
            .GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        server.WriteCopyBothResponse();
        await server.WriteXLogDataWithFlush(StartLsn, StartLsn,
            Relation(1, "public", "t", ("id", ReplicationMock.Int4Oid), ("name", ReplicationMock.TextOid)));
        await server.WriteBuffer.WriteBytesRaw(OversizeNotice(), async: true);
        await server.WriteXLogDataWithFlush(StartLsn, StartLsn, Insert(1, "1", "abc"));
        // An update with the old row ('O') before the new one ('N')
        var fullUpdate = new PgOutputMessageBuilder('U').UInt32(1)
            .Byte((byte)'O').Int16(2).Byte((byte)'t').Int32(1).Bytes("1"u8).Byte((byte)'t').Int32(3).Bytes("abc"u8)
            .Byte((byte)'N').Int16(2).Byte((byte)'t').Int32(1).Bytes("1"u8).Byte((byte)'t').Int32(3).Bytes("def"u8)
            .ToArray();
        await server.WriteXLogDataWithFlush(StartLsn, StartLsn, fullUpdate);
        // The same update again, which the consumer doesn't read
        await server.WriteXLogDataWithFlush(StartLsn, StartLsn, fullUpdate);
        // A value bigger than the buffer the connector switched to, so that a read from the old buffer gets bytes from its middle. Without
        // data left on the socket, that read would hang: cancellation only reaches the current buffer, and the mock has no read timeout
        await server.WriteXLogDataWithFlush(StartLsn, StartLsn, Insert(1, "2", new string('x', 40000)));
        await server.WriteXLogDataWithFlush(StartLsn, StartLsn, Commit(commitLsn, commitLsn, DateTime.UtcNow));
        await server.FlushAsync();

        Assert.That(await moveNext, Is.True);
        Assert.That(enumerator.Current, Is.InstanceOf<RelationMessage>());

        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        Assert.That(await ReadValues(((InsertMessage)enumerator.Current).NewRow), Is.EqualTo(new[] { "1", "abc" }));

        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        var update = (FullUpdateMessage)enumerator.Current;
        Assert.That(await ReadValues(update.OldRow), Is.EqualTo(new[] { "1", "abc" }));
        Assert.That(await ReadValues(update.NewRow), Is.EqualTo(new[] { "1", "def" }));

        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        Assert.That(enumerator.Current, Is.InstanceOf<FullUpdateMessage>());
        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        Assert.That(enumerator.Current, Is.InstanceOf<InsertMessage>());
        Assert.That(await enumerator.MoveNextAsync(), Is.True);
        Assert.That(((CommitMessage)enumerator.Current).CommitLsn, Is.EqualTo(commitLsn));

        moveNext = enumerator.MoveNextAsync().AsTask();
        streamingCts.Cancel();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();
        Assert.That(async () => await moveNext, Throws.InstanceOf<OperationCanceledException>());
        await enumerator.DisposeAsync();

        static async Task<List<string>> ReadValues(ReplicationTuple row)
        {
            var values = new List<string>();
            await foreach (var value in row)
                values.Add(await value.Get<string>());
            return values;
        }
    }

    // A NoticeResponse with a message longer than the client's read buffer
    static byte[] OversizeNotice()
    {
        var stream = new MemoryStream();
        stream.WriteByte((byte)BackendMessageCode.NoticeResponse);
        stream.Write(new byte[4]); // Length, filled in below
        WriteField('S', "WARNING");
        WriteField('C', PostgresErrorCodes.Warning);
        WriteField('M', new string('x', 20000));
        stream.WriteByte(0);
        var bytes = stream.ToArray();
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(1), bytes.Length - 1);
        return bytes;

        void WriteField(char code, string value)
        {
            stream.WriteByte((byte)code);
            stream.Write(Encoding.UTF8.GetBytes(value));
            stream.WriteByte(0);
        }
    }

    static async Task AssertConnectionIsUsable(ReplicationConnection rc, PgServerMock server)
    {
        var identifyTask = rc.IdentifySystem();
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server.WriteIdentifySystemResponse("7000000000000000001", 1, StartLsn, dbName: null).FlushAsync();
        Assert.That((await identifyTask.WaitAsync(ReplicationMock.Timeout)).Timeline, Is.EqualTo(1u));
    }

    sealed class ConsumerException : Exception;
}
