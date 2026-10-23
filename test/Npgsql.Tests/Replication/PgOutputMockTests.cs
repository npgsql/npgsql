using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.Support.PgOutputMessageBuilder;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Tests the pgoutput decoding against a mocked walsender, for messages that are hard to get from a real server.
/// </summary>
public class PgOutputMockTests : TestBase
{
    static readonly PgOutputReplicationSlot Slot = new(new ReplicationSlotOptions("s", new NpgsqlLogSequenceNumber(0x1000000)));
    static readonly NpgsqlLogSequenceNumber Lsn1 = new(0x1000100);
    static readonly NpgsqlLogSequenceNumber Lsn2 = new(0x1000200);
    static readonly DateTime Timestamp1 = new(2026, 10, 24, 12, 0, 0, DateTimeKind.Utc);
    static readonly DateTime Timestamp2 = new(2026, 10, 24, 12, 0, 1, DateTimeKind.Utc);

    [Test, Description("Stream aborts without parallel streaming have no abort LSN or timestamp, whatever the protocol version. " +
                       "PostgreSQL 18.0-18.6 also sends them with streaming = off (bug #19616).")]
    public async Task Stream_abort_without_parallel_streaming(
        [Values(PgOutputProtocolVersion.V1, PgOutputProtocolVersion.V2, PgOutputProtocolVersion.V4)] PgOutputProtocolVersion version)
    {
        var options = new PgOutputReplicationOptions(
            ["pub"], version, streamingMode: version == PgOutputProtocolVersion.V1 ? null : PgOutputStreamingMode.On);

        await RunReplication(
            options,
            [new PgOutputMessageBuilder('A').UInt32(42).UInt32(43).ToArray(), Begin(Lsn1, Timestamp1, xid: 44)],
            async messages =>
            {
                var abort = await Next<StreamAbortMessage>(messages);
                Assert.That(abort, Is.Not.InstanceOf<ParallelStreamAbortMessage>());
                Assert.That(abort.TransactionXid, Is.EqualTo(42));
                Assert.That(abort.SubtransactionXid, Is.EqualTo(43));

                // The following message is parsed correctly, so the abort message didn't consume too much
                var begin = await Next<BeginMessage>(messages);
                Assert.That(begin.TransactionFinalLsn, Is.EqualTo(Lsn1));
                Assert.That(begin.TransactionXid, Is.EqualTo(44));
            });
    }

    [Test]
    public async Task Stream_abort_with_parallel_streaming()
        => await RunReplication(
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V4, streamingMode: PgOutputStreamingMode.Parallel),
            [
                new PgOutputMessageBuilder('A').UInt32(42).UInt32(43).Lsn(Lsn1).Timestamp(Timestamp1).ToArray(),
                Begin(Lsn2, Timestamp2, xid: 44)
            ],
            async messages =>
            {
                var abort = await Next<ParallelStreamAbortMessage>(messages);
                Assert.That(abort.TransactionXid, Is.EqualTo(42));
                Assert.That(abort.SubtransactionXid, Is.EqualTo(43));
                Assert.That(abort.AbortLsn, Is.EqualTo(Lsn1));
                Assert.That(abort.AbortTimestamp, Is.EqualTo(Timestamp1));

                var begin = await Next<BeginMessage>(messages);
                Assert.That(begin.TransactionXid, Is.EqualTo(44));
            });

    [Test, Description("Slots with two-phase decoding enabled (PostgreSQL 15+) send prepared transaction messages with any protocol version")]
    public async Task Two_phase_messages_with_old_protocol_versions(
        [Values(PgOutputProtocolVersion.V1, PgOutputProtocolVersion.V2)] PgOutputProtocolVersion version)
    {
        // Stream prepare messages require streaming, which requires protocol version 2
        var streaming = version >= PgOutputProtocolVersion.V2;

        List<byte[]> pgOutputMessages =
        [
            new PgOutputMessageBuilder('b').Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).UInt32(42).String("gid1").ToArray(),
            new PgOutputMessageBuilder('P').Byte(0).Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).UInt32(42).String("gid1").ToArray(),
            new PgOutputMessageBuilder('K').Byte(0).Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).UInt32(42).String("gid1").ToArray(),
            new PgOutputMessageBuilder('r').Byte(0).Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).Timestamp(Timestamp2).UInt32(43)
                .String("gid2").ToArray()
        ];
        if (streaming)
            pgOutputMessages.Add(new PgOutputMessageBuilder('p').Byte(0).Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).UInt32(44).String("gid3").ToArray());
        pgOutputMessages.Add(Begin(Lsn2, Timestamp2, xid: 45));

        await RunReplication(
            new PgOutputReplicationOptions(["pub"], version, streamingMode: streaming ? PgOutputStreamingMode.On : null),
            [..pgOutputMessages],
            async messages =>
            {
                var beginPrepare = await Next<BeginPrepareMessage>(messages);
                Assert.That(beginPrepare.TransactionXid, Is.EqualTo(42));
                Assert.That(beginPrepare.TransactionGid, Is.EqualTo("gid1"));
                Assert.That((await Next<PrepareMessage>(messages)).TransactionGid, Is.EqualTo("gid1"));
                Assert.That((await Next<CommitPreparedMessage>(messages)).TransactionGid, Is.EqualTo("gid1"));
                var rollbackPrepared = await Next<RollbackPreparedMessage>(messages);
                Assert.That(rollbackPrepared.TransactionXid, Is.EqualTo(43));
                Assert.That(rollbackPrepared.TransactionGid, Is.EqualTo("gid2"));
                if (streaming)
                    Assert.That((await Next<StreamPrepareMessage>(messages)).TransactionGid, Is.EqualTo("gid3"));
                Assert.That((await Next<BeginMessage>(messages)).TransactionXid, Is.EqualTo(45));
            });
    }

    [Test, Description("A GID that arrives in two parts, e.g. in two TCP segments, is read without blocking the thread")]
    public async Task Gid_arriving_in_two_parts([Values('b', 'P', 'K', 'r')] char messageCode, [Values] bool split)
    {
        // The longest GID PostgreSQL allows (GIDSIZE is 200 bytes, including the terminator)
        var gid = new string('g', 199);
        var message = (messageCode switch
        {
            'b' => new PgOutputMessageBuilder('b').Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).UInt32(42),
            'r' => new PgOutputMessageBuilder('r').Byte(0).Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).Timestamp(Timestamp2).UInt32(42),
            _ => new PgOutputMessageBuilder(messageCode).Byte(0).Lsn(Lsn1).Lsn(Lsn2).Timestamp(Timestamp1).UInt32(42)
        }).String(gid).ToArray();
        // Without the split, the whole message arrives at once
        var firstPart = split ? message.Length - 100 : message.Length;

        await RunReplication(
            // Version 3 also gets the prepared transaction messages without the version fixes above
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V3),
            [Begin(Lsn2, Timestamp2, xid: 41)],
            async (messages, connection, server) =>
            {
                await Next<BeginMessage>(messages);
                Assume.That(connection.Connector.ReadBuffer.ReadBytesLeft, Is.EqualTo(PgServerMock.XLogDataHeaderLength + firstPart),
                    "The first part of the message arrived together with the Begin");

                // Decoding the message needs the rest of the GID. Reading it synchronously would block the call until then.
                var callReturned = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                var moveNext = Task.Run(() =>
                {
                    var task = messages.MoveNextAsync().AsTask();
                    callReturned.SetResult();
                    return task;
                });
                var returnedWithoutTheRest = await Task.WhenAny(callReturned.Task, Task.Delay(TimeSpan.FromSeconds(2))) == callReturned.Task;
                if (split)
                {
                    server.WriteBuffer.WriteBytes(message.AsSpan(firstPart));
                    await server.FlushAsync();
                }

                Assert.That(returnedWithoutTheRest, Is.True, "MoveNextAsync blocked the thread until the rest of the GID arrived");
                Assert.That(await moveNext.WaitAsync(ReplicationMock.Timeout), Is.True);
                Assert.That(((PreparedTransactionControlMessage)messages.Current).TransactionGid, Is.EqualTo(gid));
            },
            writeAfterMessages: server =>
            {
                // The message's XLogData header and its first part go out together with the Begin
                server.WriteXLogDataHeader(Lsn2, Lsn2, message.Length, Timestamp1);
                server.WriteBuffer.WriteBytes(message.AsSpan(0, firstPart));
            });
    }

    [Test]
    public async Task Value_access_outside_of_its_column_throws()
        => await RunReplication(
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V1),
            [
                Relation(1, "public", "t", ("id", ReplicationMock.Int4Oid), ("name", ReplicationMock.TextOid)),
                Insert(1, null, "x")
            ],
            async messages =>
            {
                await Next<RelationMessage>(messages);
                var insert = await Next<InsertMessage>(messages);
                var columns = insert.NewRow.GetAsyncEnumerator();
                Assert.That(() => columns.Current, Throws.InvalidOperationException, "Current before MoveNextAsync");

                Assert.That(await columns.MoveNextAsync(), Is.True);
                Assert.That(columns.Current.IsDBNull, Is.True);
                Assert.That(await columns.Current.Get<int?>(), Is.Null, "A NULL can be read as a nullable type in text mode");

                Assert.That(await columns.MoveNextAsync(), Is.True);
                var value = columns.Current;
                Assert.That(await columns.MoveNextAsync(), Is.False);
                Assert.That(async () => await value.Get<string>(), Throws.InvalidOperationException,
                    "The last value is consumed once the columns have been enumerated");
            });

    [Test]
    public async Task Unknown_message_breaks_the_connection()
        => await RunReplication(
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V1),
            [new PgOutputMessageBuilder('?').UInt32(42).ToArray()],
            (messages, connection) =>
            {
                Assert.That(async () => await messages.MoveNextAsync(), Throws.InstanceOf<NotSupportedException>());
                Assert.That(connection.Connector.IsBroken, Is.True);
                return Task.CompletedTask;
            });

    [Test]
    public async Task Change_for_unknown_relation_breaks_the_connection()
        => await RunReplication(
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V1),
            [Insert(1, "x")],
            (messages, connection) =>
            {
                Assert.That(async () => await messages.MoveNextAsync(), Throws.InvalidOperationException);
                Assert.That(connection.Connector.IsBroken, Is.True);
                return Task.CompletedTask;
            });

    [Test]
    public async Task Unknown_tuple_type_breaks_the_connection([Values('I', 'U', 'D')] char messageCode)
        => await RunReplication(
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V1),
            [
                Relation(1, "public", "t", ("id", ReplicationMock.Int4Oid)),
                new PgOutputMessageBuilder(messageCode).UInt32(1).Byte((byte)'?').Int16(1).Byte((byte)'n').ToArray()
            ],
            async (messages, connection) =>
            {
                await Next<RelationMessage>(messages);
                Assert.That(async () => await messages.MoveNextAsync(), Throws.InstanceOf<NotSupportedException>());
                Assert.That(connection.Connector.IsBroken, Is.True);
            });

    [Test]
    public async Task Unknown_column_kind_breaks_the_connection()
        => await RunReplication(
            new PgOutputReplicationOptions(["pub"], PgOutputProtocolVersion.V1),
            [
                Relation(1, "public", "t", ("id", ReplicationMock.Int4Oid)),
                new PgOutputMessageBuilder('I').UInt32(1).Byte((byte)'N').Int16(1).Byte((byte)'?').ToArray()
            ],
            async (messages, connection) =>
            {
                await Next<RelationMessage>(messages);
                var columns = (await Next<InsertMessage>(messages)).NewRow.GetAsyncEnumerator();
                Assert.That(async () => await columns.MoveNextAsync(), Throws.InstanceOf<ArgumentOutOfRangeException>());
                Assert.That(connection.Connector.IsBroken, Is.True);
            });

    [Test]
    public void Options_equality()
    {
        var options = new PgOutputReplicationOptions(new List<string> { "pub1", "pub2" }, PgOutputProtocolVersion.V2, binary: true);
        var equalOptions = new PgOutputReplicationOptions(new List<string> { "pub1", "pub2" }, PgOutputProtocolVersion.V2, binary: true);

        Assert.That(options, Is.EqualTo(equalOptions));
        Assert.That(options.GetHashCode(), Is.EqualTo(equalOptions.GetHashCode()));
        Assert.That(options, Is.Not.EqualTo(new PgOutputReplicationOptions(["pub2", "pub1"], PgOutputProtocolVersion.V2, binary: true)));
        Assert.That(options, Is.Not.EqualTo(new PgOutputReplicationOptions(["pub1", "pub2"], PgOutputProtocolVersion.V2)));
    }

    Task RunReplication(
        PgOutputReplicationOptions options,
        byte[][] pgOutputMessages,
        Func<IAsyncEnumerator<PgOutputReplicationMessage>, Task> test)
        => RunReplication(options, pgOutputMessages, (messages, _, _) => test(messages));

    Task RunReplication(
        PgOutputReplicationOptions options,
        byte[][] pgOutputMessages,
        Func<IAsyncEnumerator<PgOutputReplicationMessage>, LogicalReplicationConnection, Task> test)
        => RunReplication(options, pgOutputMessages, (messages, connection, _) => test(messages, connection));

    /// <summary>
    /// Starts pgoutput replication against the mock, sends the given pgoutput messages and whatever
    /// <paramref name="writeAfterMessages"/> writes, and runs the test on the stream.
    /// </summary>
    async Task RunReplication(
        PgOutputReplicationOptions options,
        byte[][] pgOutputMessages,
        Func<IAsyncEnumerator<PgOutputReplicationMessage>, LogicalReplicationConnection, PgServerMock, Task> test,
        Action<PgServerMock>? writeAfterMessages = null)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString, serverVersion: "16");
        var (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var timeoutCts = new CancellationTokenSource(ReplicationMock.Timeout);
        using var streamingCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token);

        var messages = rc.StartReplication(Slot, options, streamingCts.Token).GetAsyncEnumerator();
        var firstMoveNext = messages.MoveNextAsync().AsTask();
        try
        {
            await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
            server.WriteCopyBothResponse();
            var lsn = 0x1000000UL;
            foreach (var message in pgOutputMessages)
            {
                await server.WriteXLogDataWithFlush(new NpgsqlLogSequenceNumber(lsn), new NpgsqlLogSequenceNumber(lsn), message);
                lsn += 0x100;
            }
            writeAfterMessages?.Invoke(server);
            await server.FlushAsync();

            await test(new FirstMoveNextAwaitingEnumerator(messages, firstMoveNext), rc, server);

            if (!rc.Connector.IsBroken)
            {
                // Stop replication the way the server reacts to a cancellation
                var moveNext = messages.MoveNextAsync().AsTask();
                streamingCts.Cancel();
                await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
                await server
                    .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
                    .WriteReadyForQuery()
                    .FlushAsync();
                Assert.That(async () => await moveNext, Throws.InstanceOf<OperationCanceledException>());
            }
        }
        finally
        {
            // A pending MoveNextAsync would make DisposeAsync throw and hide the actual failure, so end it first
            server.Close();
            try
            {
                await firstMoveNext;
            }
            catch
            {
                // Already reported by the test
            }

            try
            {
                await messages.DisposeAsync();
            }
            catch (NotSupportedException)
            {
                // A MoveNextAsync of the test is still pending; the test has already failed
            }
        }
    }

    static async Task<T> Next<T>(IAsyncEnumerator<PgOutputReplicationMessage> messages) where T : PgOutputReplicationMessage
    {
        Assert.That(await messages.MoveNextAsync(), Is.True);
        Assert.That(messages.Current, Is.InstanceOf<T>());
        return (T)messages.Current;
    }

    /// <summary>
    /// Makes the first MoveNextAsync of the test return the result of the MoveNextAsync that started replication.
    /// </summary>
    sealed class FirstMoveNextAwaitingEnumerator(IAsyncEnumerator<PgOutputReplicationMessage> enumerator, Task<bool> firstMoveNext)
        : IAsyncEnumerator<PgOutputReplicationMessage>
    {
        Task<bool>? _firstMoveNext = firstMoveNext;

        public PgOutputReplicationMessage Current => enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            if (_firstMoveNext is not { } firstMoveNext)
                return enumerator.MoveNextAsync();

            _firstMoveNext = null;
            return new(firstMoveNext);
        }

        public ValueTask DisposeAsync() => enumerator.DisposeAsync();
    }
}
