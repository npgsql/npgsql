using System;
using System.Collections.Generic;
using System.IO;
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
/// Tests the standby status updates (feedback) of replication connections against a mocked walsender.
/// </summary>
public class ReplicationFeedbackMockTests : TestBase
{
    static readonly NpgsqlLogSequenceNumber StartLsn = new(0x1000000);

    [Test, Description("Like in PostgreSQL, zero disables the status interval and the timeout, and so do values too long for a timer")]
    public async Task Disabled_interval_sends_no_status_updates([Values] bool statusInterval, [Values] bool tooLong)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);
        var interval = tooLong ? TimeSpan.MaxValue : TimeSpan.Zero;
        if (statusInterval)
            rc.WalReceiverStatusInterval = interval;
        else
            rc.WalReceiverTimeout = interval;

        var enumerator = rc.StartReplication(StartLsn, cts.Token).GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        // The client replies to the keepalive and receives messages, after both of which it sets its timers again
        await server
            .WriteCopyBothResponse()
            .WriteKeepalive(StartLsn, replyRequested: true)
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);
        Assert.That((await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout)).ReplyRequested, Is.False);

        await Task.Delay(500);
        Assert.That(server.DataAvailable, Is.False, "The client sent a status update");

        // Stopping the stream sends the only other status update
        var dispose = enumerator.DisposeAsync().AsTask();
        await server.ExpectStandbyStatusUpdate().WaitAsync(ReplicationMock.Timeout);
        await server.ExpectCopyDone().WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyDone()
            .WriteCommandComplete("START_STREAMING")
            .WriteCommandComplete("START_REPLICATION")
            .WriteReadyForQuery()
            .FlushAsync();
        await dispose.WaitAsync(ReplicationMock.Timeout);
    }

    [Test, Description("Negative intervals are rejected right away, instead of failing once streaming started")]
    public async Task Negative_intervals_are_rejected()
    {
        await using var rc = new PhysicalReplicationConnection();

        Assert.That(() => rc.WalReceiverStatusInterval = TimeSpan.FromSeconds(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => rc.WalReceiverTimeout = TimeSpan.FromSeconds(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(rc.WalReceiverStatusInterval, Is.EqualTo(TimeSpan.FromSeconds(10)));
        Assert.That(rc.WalReceiverTimeout, Is.EqualTo(TimeSpan.FromSeconds(60)));

        rc.WalReceiverStatusInterval = Timeout.InfiniteTimeSpan;
        rc.WalReceiverTimeout = Timeout.InfiniteTimeSpan;
        Assert.That(rc.WalReceiverStatusInterval, Is.EqualTo(Timeout.InfiniteTimeSpan));
        Assert.That(rc.WalReceiverTimeout, Is.EqualTo(Timeout.InfiniteTimeSpan));
    }

    [Test, Description("A status update that can't be sent fails, instead of being dropped silently")]
    public async Task SendStatusUpdate_reports_write_failures()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);
        await using var enumerator = await StartAndHoldMessage(rc, server, cts.Token);

        // The consumer holds a message, so nothing reads from the connection while the write fails
        rc.Connector.WriteBuffer.Underlying = new FailingStream();
        Assert.That(async () => await rc.SendStatusUpdate(cts.Token),
            Throws.InstanceOf<NpgsqlException>().With.InnerException.TypeOf<IOException>());
        Assert.That(rc.Connector.IsBroken, Is.True);

        // Asking for the next message reports the failure too
        Assert.That(async () => await enumerator.MoveNextAsync(),
            Throws.InstanceOf<NpgsqlException>().With.InnerException.With.InnerException.TypeOf<IOException>());
    }

    [Test, Description("A status update that waited for one that broke the connection fails as not streaming")]
    public async Task SendStatusUpdate_after_a_failed_one()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);
        await using var enumerator = await StartAndHoldMessage(rc, server, cts.Token);

        var stream = new FailingStream(failImmediately: false);
        rc.Connector.WriteBuffer.Underlying = stream;
        var first = rc.SendStatusUpdate(cts.Token);
        var second = rc.SendStatusUpdate(cts.Token); // Waits for the first one
        stream.Fail();

        Assert.That(async () => await first, Throws.InstanceOf<NpgsqlException>().With.InnerException.TypeOf<IOException>());
        Assert.That(async () => await second, Throws.TypeOf<InvalidOperationException>());
    }

    [Test, Description("A reply to a keepalive that can't be sent ends the stream with the failure")]
    public async Task Keepalive_reply_failure_ends_the_stream()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<PhysicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);
        await using var enumerator = await StartAndHoldMessage(rc, server, cts.Token);

        rc.Connector.WriteBuffer.Underlying = new FailingStream();
        await server.WriteKeepalive(StartLsn, replyRequested: true).FlushAsync();

        Assert.That(async () => await enumerator.MoveNextAsync(),
            Throws.InstanceOf<NpgsqlException>().With.InnerException.TypeOf<IOException>());
        Assert.That(rc.Connector.IsBroken, Is.True);
    }

    [Test, Description("A timer's status update that can't be sent while the consumer holds a message ends the stream with the failure")]
    public async Task Timer_status_update_failure_ends_the_stream([Values] bool bypassingStream)
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        var (rc, server) = await postmaster.OpenReplicationConnection<LogicalReplicationConnection>();
        await using var _ = rc;
        using var cts = new CancellationTokenSource(ReplicationMock.Timeout);
        rc.WalReceiverStatusInterval = TimeSpan.FromMilliseconds(10);

        // Plugins like pgoutput bypass the message's stream and read its data from the buffer directly
        await using var enumerator = rc
            .StartLogicalReplication(new TestDecodingReplicationSlot("s"), cts.Token, StartLsn, bypassingStream: bypassingStream)
            .GetAsyncEnumerator();
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);

        // The consumer holds a message, so nothing reads from the connection while the timer's write fails and breaks it
        rc.Connector.WriteBuffer.Underlying = new FailingStream();
        while (!rc.Connector.IsBroken)
            await Task.Delay(10, cts.Token);

        // The write failure that broke the connection is the inner exception
        Assert.That(async () => await enumerator.MoveNextAsync(),
            Throws.InstanceOf<NpgsqlException>().With.InnerException.With.InnerException.TypeOf<IOException>());
    }

    static async Task<IAsyncEnumerator<XLogDataMessage>> StartAndHoldMessage(
        PhysicalReplicationConnection rc, PgServerMock server, CancellationToken cancellationToken)
    {
        var enumerator = rc.StartReplication(StartLsn, cancellationToken).GetAsyncEnumerator(cancellationToken);
        var moveNext = enumerator.MoveNextAsync().AsTask();
        await server.ExpectMessage(FrontendMessageCode.Query).WaitAsync(ReplicationMock.Timeout);
        await server
            .WriteCopyBothResponse()
            .WriteXLogData(StartLsn, StartLsn, "abc"u8)
            .FlushAsync();
        Assert.That(await moveNext, Is.True);
        return enumerator;
    }

    /// <summary>
    /// A stream whose writes fail, either right away or once <see cref="Fail"/> gets called.
    /// </summary>
    sealed class FailingStream : Stream
    {
        readonly TaskCompletionSource _failure = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal FailingStream(bool failImmediately = true)
        {
            if (failImmediately)
                Fail();
        }

        internal void Fail() => _failure.TrySetResult();

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new IOException("Simulated write failure");
        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _failure.Task.ConfigureAwait(false);
            throw new IOException("Simulated write failure");
        }
        public override void Flush() => throw new IOException("Simulated write failure");
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}
