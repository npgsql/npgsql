using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Tests replication connections against a mocked walsender. This covers server behavior that is hard or impossible to
/// trigger reliably on a real server, like timeline switches, shutdowns or protocol edge cases.
/// </summary>
public class ReplicationMockTests : TestBase
{
    [Test]
    public async Task Logical_startup_and_IDENTIFY_SYSTEM()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);

        Assert.That(server.StartupParameters["replication"], Is.EqualTo("database"));

        var identifyTask = rc.IdentifySystem(cts.Token);
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteIdentifySystemResponse("7000000000000000001", 1, new NpgsqlLogSequenceNumber(0x1000000), "npgsql_tests")
            .FlushAsync();

        var info = await identifyTask;
        Assert.That(info.SystemId, Is.EqualTo("7000000000000000001"));
        Assert.That(info.Timeline, Is.EqualTo(1u));
        Assert.That(info.XLogPos, Is.EqualTo(new NpgsqlLogSequenceNumber(0x1000000)));
        Assert.That(info.DbName, Is.EqualTo("npgsql_tests"));
    }

    [Test]
    public async Task Physical_streaming_and_cancellation()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var timeoutCts = new CancellationTokenSource(ReplicationMock.Timeout);
        using var streamingCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token);

        Assert.That(server.StartupParameters["replication"], Is.EqualTo("true"));

        var startLsn = new NpgsqlLogSequenceNumber(0x1000000);
        var dataEndLsn = new NpgsqlLogSequenceNumber(0x1000003);
        var enumerator = rc.StartReplication(startLsn, streamingCts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        try
        {
            await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000").WaitAsync(ReplicationMock.Timeout);
            await server
                .WriteCopyBothResponse()
                .WriteXLogData(startLsn, dataEndLsn, "abc"u8)
                .FlushAsync();

            Assert.That(await moveNext, Is.True);
            var message = enumerator.Current;
            Assert.That(message.WalStart, Is.EqualTo(startLsn));
            Assert.That(message.WalEnd, Is.EqualTo(dataEndLsn));
            var data = new byte[3];
            await message.Data.ReadExactlyAsync(data, timeoutCts.Token);
            Assert.That(data, Is.EqualTo("abc"u8.ToArray()));

            // Notices don't disturb the stream, and a keepalive requesting a reply gets an immediate status update
            moveNext = enumerator.MoveNextAsync().AsTask();
            await server
                .WriteNoticeResponse(PostgresErrorCodes.Warning)
                .WriteKeepalive(dataEndLsn, replyRequested: true)
                .FlushAsync();
            var statusUpdate = await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
            Assert.That(statusUpdate.WriteLsn, Is.EqualTo(dataEndLsn));
            Assert.That(statusUpdate.ReplyRequested, Is.False);

            // Cancelling the token cancels START_REPLICATION with a PostgreSQL cancel request
            streamingCts.Cancel();
            await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
            await server
                .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
                .WriteReadyForQuery()
                .FlushAsync();
            Assert.That(async () => await moveNext, Throws.InstanceOf<OperationCanceledException>());
        }
        finally
        {
            if (!moveNext.IsCompleted)
            {
                // A pending MoveNextAsync would make DisposeAsync throw and hide the actual failure, so end it first
                server.Close();
                try
                {
                    await moveNext;
                }
                catch
                {
                    // The test has already failed
                }
            }

            await enumerator.DisposeAsync();
        }

        // The connection can be used for further commands
        var identifyTask = rc.IdentifySystem(timeoutCts.Token);
        await server.ExpectSimpleQuery("IDENTIFY_SYSTEM").WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteIdentifySystemResponse("7000000000000000001", 1, dataEndLsn, dbName: null)
            .FlushAsync();
        var info = await identifyTask;
        Assert.That(info.XLogPos, Is.EqualTo(dataEndLsn));
        Assert.That(info.DbName, Is.Null);
    }

    [Test, Description("The mock writes WAL data larger than its write buffer, also when the buffer has no room left for the header")]
    public async Task Physical_streaming_of_data_larger_than_the_buffers()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var timeoutCts = new CancellationTokenSource(ReplicationMock.Timeout);
        using var streamingCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token);

        var startLsn = new NpgsqlLogSequenceNumber(0x1000000);
        var enumerator = rc.StartReplication(startLsn, streamingCts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        try
        {
            await server.ExpectSimpleQuery("START_REPLICATION PHYSICAL 0/1000000").WaitAsync(ReplicationMock.Timeout);
            await server.WriteCopyBothResponse().FlushAsync();

            // The first message leaves less room in the write buffer than the second one's header needs (message code,
            // length, 'w', WAL start and end, server clock), and the second one's data is larger than the client's and the
            // mock's buffers
            const int xLogDataHeaderLength = 1 + 4 + 1 + 8 + 8 + 8;
            var first = Pattern(server.WriteBuffer.WriteSpaceLeft - xLogDataHeaderLength - xLogDataHeaderLength / 2, 1);
            var second = Pattern(3 * server.WriteBuffer.Size, 2);
            var secondLsn = startLsn + first.Length;
            server.WriteXLogData(startLsn, secondLsn, first);
            Assert.That(server.WriteBuffer.WriteSpaceLeft, Is.LessThan(xLogDataHeaderLength));
            await server.WriteXLogDataWithFlush(secondLsn, secondLsn + second.Length, second);
            await server.FlushAsync();

            Assert.That(await moveNext, Is.True);
            var data = new byte[first.Length];
            await enumerator.Current.Data.ReadExactlyAsync(data, timeoutCts.Token);
            Assert.That(data, Is.EqualTo(first));

            Assert.That(await enumerator.MoveNextAsync(), Is.True);
            Assert.That(enumerator.Current.WalStart, Is.EqualTo(secondLsn));
            data = new byte[second.Length];
            await enumerator.Current.Data.ReadExactlyAsync(data, timeoutCts.Token);
            Assert.That(data, Is.EqualTo(second));

            moveNext = enumerator.MoveNextAsync().AsTask();
            streamingCts.Cancel();
            await postmaster.WaitForCancellationRequest().AsTask().WaitAsync(ReplicationMock.Timeout);
            await server
                .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
                .WriteReadyForQuery()
                .FlushAsync();
            Assert.That(async () => await moveNext, Throws.InstanceOf<OperationCanceledException>());
        }
        finally
        {
            if (!moveNext.IsCompleted)
            {
                // A pending MoveNextAsync would make DisposeAsync throw and hide the actual failure, so end it first
                server.Close();
                try
                {
                    await moveNext;
                }
                catch
                {
                    // The test has already failed
                }
            }

            await enumerator.DisposeAsync();
        }

        static byte[] Pattern(int length, int seed)
        {
            var bytes = new byte[length];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)(i * 7 + seed);
            return bytes;
        }
    }
}
