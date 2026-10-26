using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Replication;
using Npgsql.Replication.Internal;
using Npgsql.Replication.TestDecoding;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;

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

    [Test, Description("Disposing the connection while the consumer waits for the next message ends that call, also if the server doesn't answer the cancellation")]
    public async Task Dispose_while_MoveNextAsync_is_pending([Values] bool serverAnswersCancellation)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        moveNext = enumerator.MoveNextAsync().AsTask();
        var dispose = rc.DisposeAsync().AsTask();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        if (serverAnswersCancellation)
        {
            await server
                .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
                .WriteReadyForQuery()
                .FlushAsync();
        }

        // Without an answer, the read that waits for one is cancelled after Cancellation Timeout
        Assert.That(async () => await moveNext.WaitAsync(ReplicationMock.Timeout), Throws.InstanceOf<OperationCanceledException>());
        await dispose.WaitAsync(ReplicationMock.Timeout);
        if (serverAnswersCancellation)
            await server.ExpectTerminate().WaitAsync(ReplicationMock.Timeout);
        await enumerator.DisposeAsync();
    }

    [Test, Description("Disposing the connection while the consumer holds a message stops the stream, and the consumer's next MoveNextAsync returns false")]
    public async Task Dispose_while_consumer_holds_a_message([Values] bool serverAnswersCancellation)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        var dispose = rc.DisposeAsync().AsTask();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        if (serverAnswersCancellation)
        {
            await server
                .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
                .WriteReadyForQuery()
                .FlushAsync();
        }

        await dispose.WaitAsync(ReplicationMock.Timeout);
        if (serverAnswersCancellation)
            await server.ExpectTerminate().WaitAsync(ReplicationMock.Timeout);
        Assert.That(await enumerator.MoveNextAsync(), Is.False);
        await enumerator.DisposeAsync();
    }

    [Test, Description("A MoveNextAsync while DisposeAsync stops the stream waits until the connection is disposed and returns false")]
    public async Task MoveNextAsync_while_Dispose_stops_the_stream()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // DisposeAsync returns to us once it waits for the server to end the command
        var dispose = rc.DisposeAsync().AsTask();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        moveNext = enumerator.MoveNextAsync().AsTask();
        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(await moveNext.WaitAsync(ReplicationMock.Timeout), Is.False);
        await dispose.WaitAsync(ReplicationMock.Timeout);
        await server.ExpectTerminate().WaitAsync(ReplicationMock.Timeout);
        await enumerator.DisposeAsync();
    }

    [Test, Description("Breaking out of the loop while DisposeAsync stops the stream waits until the connection is disposed")]
    public async Task Break_while_Dispose_stops_the_stream()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // DisposeAsync returns to us once it waits for the server to end the command. Breaking out of the loop disposes the
        // enumerator.
        var dispose = rc.DisposeAsync().AsTask();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        var consumerDispose = enumerator.DisposeAsync().AsTask();
        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();

        await consumerDispose.WaitAsync(ReplicationMock.Timeout);
        await dispose.WaitAsync(ReplicationMock.Timeout);
        await server.ExpectTerminate().WaitAsync(ReplicationMock.Timeout);
    }

    [Test, Description("A MoveNextAsync that waits for DisposeAsync returns false once that is done, also if DisposeAsync gets called again meanwhile")]
    public async Task Second_Dispose_while_MoveNextAsync_waits_for_the_first()
    {
        // The second DisposeAsync can't take over from the first one and waits out its bound twice, which -1 keeps at a second
        var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString) { CancellationTimeout = -1 }.ConnectionString;
        await using var postmaster = PgPostmasterMock.Start(connectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // The consumer holds a message, so DisposeAsync stops the stream itself, and its read waits until we let it fail
        var stream = new FailingReadStream();
        rc.Connector.ReadBuffer.Underlying = stream;
        var dispose = rc.DisposeAsync().AsTask();
        await stream.ReadStarted.WaitAsync(ReplicationMock.Timeout);
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
        moveNext = enumerator.MoveNextAsync().AsTask();
        var secondDispose = rc.DisposeAsync().AsTask();
        stream.Fail();

        Assert.That(await moveNext.WaitAsync(ReplicationMock.Timeout), Is.False);
        await dispose.WaitAsync(ReplicationMock.Timeout);
        await secondDispose.WaitAsync(ReplicationMock.Timeout);
        await enumerator.DisposeAsync();
    }

    [Test, Description("Disposing the connection while the consumer's stream stops (e.g. after a break) lets that stop end")]
    public async Task Dispose_while_the_stream_stops()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var consumer = Task.Run(async () =>
        {
            await foreach (var _ in rc.StartReplication(StartLsn, cts.Token))
                break;
        });

        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        // The stop sends a final status update and ends copy mode, but the server doesn't answer
        await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        await server.ExpectCopyDone().WaitAsync(ReplicationMock.Timeout);

        await rc.DisposeAsync().AsTask().WaitAsync(ReplicationMock.Timeout);
        await consumer.WaitAsync(ReplicationMock.Timeout);
    }

    [Test, Description("Disposing the connection while the consumer waits for the next message breaks the connection if the server doesn't end the stream in time")]
    public async Task Dispose_while_MoveNextAsync_is_pending_and_server_keeps_streaming()
    {
        var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString) { CancellationTimeout = 1000 }.ConnectionString;
        await using var postmaster = PgPostmasterMock.Start(connectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        moveNext = enumerator.MoveNextAsync().AsTask();
        var dispose = rc.DisposeAsync().AsTask();
        await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);

        // The server keeps the stream going without ending the command, so the read that waits for the end is never cancelled. It
        // sends one long message byte by byte, so that the call still reads it when the connection breaks (a break while the call
        // handles a message can make it fail with another exception).
        server.WriteBuffer.WriteByte((byte)BackendMessageCode.CopyData);
        server.WriteBuffer.WriteInt32(4 + 8000);
        while (!moveNext.IsCompleted && !cts.IsCancellationRequested)
        {
            try
            {
                server.WriteBuffer.WriteByte(0);
                await server.FlushAsync();
            }
            catch
            {
                break; // The client closed the connection (without a connector, the mock's buffer doesn't throw IOException)
            }

            await Task.WhenAny(moveNext, Task.Delay(100));
        }

        Assert.That(async () => await moveNext.WaitAsync(ReplicationMock.Timeout), Throws.InstanceOf<NpgsqlException>());
        await dispose.WaitAsync(ReplicationMock.Timeout);
        await enumerator.DisposeAsync();
    }

    [Test, Description("Disposing a broken connection while the consumer's MoveNextAsync still runs lets that call end with the failure")]
    public async Task Dispose_broken_connection_while_MoveNextAsync_is_pending()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // The next read only fails when we let it, so that the call is still in flight after the connection broke (e.g. because a
        // status update failed)
        var stream = new FailingReadStream();
        rc.Connector.ReadBuffer.Underlying = stream;
        moveNext = enumerator.MoveNextAsync().AsTask();
        await stream.ReadStarted.WaitAsync(ReplicationMock.Timeout);
        rc.Connector.Break(new NpgsqlException("Simulated failure"));

        var dispose = rc.DisposeAsync().AsTask();
        stream.Fail();
        Assert.That(async () => await moveNext.WaitAsync(ReplicationMock.Timeout),
            Throws.InstanceOf<NpgsqlException>().With.InnerException.TypeOf<IOException>());
        await dispose.WaitAsync(ReplicationMock.Timeout);
        await enumerator.DisposeAsync();
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

    [Test, Description("A message the consumer holds when the connection breaks keeps its data, also when the buffer came from ArrayPool")]
    public async Task Held_message_keeps_its_data_after_a_break([Values] bool afterOversizeMessage)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        await using var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        server.WriteCopyBothResponse();
        // A message bigger than the read buffer makes the connector switch to a bigger one, whose array comes from ArrayPool
        if (afterOversizeMessage)
            await server.WriteBuffer.WriteBytesRaw(OversizeNotice(), async: true);
        await server
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);
        var bufferSize = rc.Connector.ReadBuffer.Size;

        // The connection breaks while the consumer holds the message (e.g. because a status update failed), and someone else rents
        // an array of the buffer's size and writes to it. ArrayPool<byte>.Shared hands the array a thread returned last to that
        // thread's next rent of the same size, so we do both on this thread: the rent gets the buffer's array if the break returned it.
        rc.Connector.Break(new Exception("Simulated failure"));
        var array = ArrayPool<byte>.Shared.Rent(bufferSize);
        array.AsSpan().Fill(0xEE);

        var data = new byte[3];
        await enumerator.Current.Data.ReadExactlyAsync(data, cts.Token);
        ArrayPool<byte>.Shared.Return(array);
        Assert.That(data, Is.EqualTo("abc"u8.ToArray()));
    }

    static async Task AssertConnectionIsUsable(ReplicationConnection rc, PgServerMock server)
    {
        var identifyTask = rc.IdentifySystem();
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server.WriteIdentifySystemResponse("7000000000000000001", 1, StartLsn, dbName: null).FlushAsync();
        Assert.That((await identifyTask.WaitAsync(ReplicationMock.Timeout)).Timeline, Is.EqualTo(1u));
    }

    sealed class ConsumerException : Exception;

    /// <summary>
    /// A stream whose reads wait until <see cref="Fail"/> gets called, and then fail.
    /// </summary>
    sealed class FailingReadStream : Stream
    {
        readonly TaskCompletionSource _readStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        readonly TaskCompletionSource _failure = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal Task ReadStarted => _readStarted.Task;

        internal void Fail() => _failure.TrySetResult();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            _readStarted.TrySetResult();
            await _failure.Task.ConfigureAwait(false);
            throw new IOException("Simulated read failure");
        }
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
