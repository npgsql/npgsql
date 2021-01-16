using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using Npgsql.Replication.TestDecoding;
using NpgsqlTypes;

namespace Npgsql.Tests.Replication
{
    [TestFixture(typeof(LogicalReplicationConnection))]
    [TestFixture(typeof(PhysicalReplicationConnection))]
    public class CommonReplicationTests<TConnection> : SafeReplicationTestBase<TConnection>
        where TConnection : ReplicationConnection, new()
    {
        #region Open

        [Test]
        public async Task Open()
        {
            await using var rc = await OpenReplicationConnectionAsync();
        }

        [Test]
        public void OpenCancelled()
            => Assert.That(async () =>
            {
                using var cts = GetCancelledCancellationTokenSource();
                await using var rc = await OpenReplicationConnectionAsync(cancellationToken: cts.Token);
            }, Throws.Exception.AssignableTo<OperationCanceledException>());

        [Test]
        public void OpenDisposed()
            => Assert.That(async () =>
            {
                var rc = await OpenReplicationConnectionAsync();
                await rc.DisposeAsync();
                await rc.Open();
            }, Throws.InstanceOf<ObjectDisposedException>()
                .With.Property(nameof(ObjectDisposedException.ObjectName))
                .EqualTo(typeof(TConnection).Name));

        #endregion Open

        #region IdentifySystem

        [Test]
        public async Task IdentifySystem()
        {
            await using var rc = await OpenReplicationConnectionAsync();
            var info = await rc.IdentifySystem();
            Assert.That(info.Timeline, Is.GreaterThan(0));
        }

        [Test]
        public void IdentifySystemCancelled()
            => Assert.That(async () =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                using var cts = GetCancelledCancellationTokenSource();
                await rc.IdentifySystem(cts.Token);
            }, Throws.Exception.AssignableTo<OperationCanceledException>());

        [Test]
        public void IdentifySystemDisposed()
            => Assert.That(async () =>
            {
                var rc = await OpenReplicationConnectionAsync();
                await rc.DisposeAsync();
                await rc.IdentifySystem();
            }, Throws.InstanceOf<ObjectDisposedException>()
                .With.Property(nameof(ObjectDisposedException.ObjectName))
                .EqualTo(typeof(TConnection).Name));

        #endregion IdentifySystem

        #region Show

        [Test]
        public async Task Show()
        {
            await using var c = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(c, "10.0", "The SHOW command was added to the Streaming Replication Protocol in PostgreSQL 10");

            await using var rc = await OpenReplicationConnectionAsync();
            Assert.That(await rc.Show("integer_datetimes"), Is.EqualTo("on"));
        }

        [Test]
        public async Task ShowNullArgument()
        {
            await using var c = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(c, "10.0", "The SHOW command was added to the Streaming Replication Protocol in PostgreSQL 10");

            Assert.That(async () =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                await rc.Show(null!);
            }, Throws.ArgumentNullException
                .With.Property("ParamName")
                .EqualTo("parameterName"));
        }

        [Test]
        public async Task ShowCancelled()
        {
            await using var c = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(c, "10.0", "The SHOW command was added to the Streaming Replication Protocol in PostgreSQL 10");

            Assert.That(async () =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                using var cts = GetCancelledCancellationTokenSource();
                await rc.Show("integer_datetimes", cts.Token);
            }, Throws.Exception.AssignableTo<OperationCanceledException>());
        }

        [Test]
        public async Task ShowDisposed()
        {
            await using var c = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(c, "10.0", "The SHOW command was added to the Streaming Replication Protocol in PostgreSQL 10");

            Assert.That(async () =>
            {
                var rc = await OpenReplicationConnectionAsync();
                await rc.DisposeAsync();
                await rc.Show("integer_datetimes");
            }, Throws.InstanceOf<ObjectDisposedException>()
                .With.Property(nameof(ObjectDisposedException.ObjectName))
                .EqualTo(typeof(TConnection).Name));
        }

        #endregion Show

        #region TimelineHistory

        [Test, Explicit("After initdb a PostgreSQL cluster only has one timeline and no timeline history so this command fails. " +
                        "You need to explicitly create multiple timelines (e. g. via PITR or by promoting a standby) for this test to work.")]
        public async Task TimelineHistory()
        {
            await using var rc = await OpenReplicationConnectionAsync();
            var systemInfo = await rc.IdentifySystem();
            var info = await rc.TimelineHistory(systemInfo.Timeline);
            Assert.That(info.FileName, Is.Not.Null);
            Assert.That(info.Content.Length, Is.GreaterThan(0));
        }

        [Test]
        public void TimelineHistoryCancelled()
            => Assert.That(async () =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                var systemInfo = await rc.IdentifySystem();
                using var cts = GetCancelledCancellationTokenSource();
                await rc.TimelineHistory(systemInfo.Timeline, cts.Token);
            }, Throws.Exception.AssignableTo<OperationCanceledException>());

        [Test]
        public void TimelineHistoryNonExisting()
            => Assert.That(async () =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                await rc.TimelineHistory(uint.MaxValue);
            }, Throws
                .InstanceOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedFile)
                .Or
                .InstanceOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.CharacterNotInRepertoire));

        [Test]
        public void TimelineHistoryDisposed()
            => Assert.That(async () =>
            {
                var rc = await OpenReplicationConnectionAsync();
                var systemInfo = await rc.IdentifySystem();
                await rc.DisposeAsync();
                await rc.TimelineHistory(systemInfo.Timeline);
            }, Throws.InstanceOf<ObjectDisposedException>()
                .With.Property(nameof(ObjectDisposedException.ObjectName))
                .EqualTo(typeof(TConnection).Name));

        #endregion TimelineHistory

        #region DropReplicationSlot

        [Test]
        public void DropReplicationSlotNullSlot()
            => Assert.That(async () =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                await rc.DropReplicationSlot(null!);
            }, Throws.ArgumentNullException
                .With.Property("ParamName")
                .EqualTo("slotName"));

        [Test]
        public Task DropReplicationSlotCancelled()
            => SafeReplicationTest(
                async (slotName, _) =>
                {
                    await CreateReplicationSlot(slotName);
                    await using var rc = await OpenReplicationConnectionAsync();
                    using var cts = GetCancelledCancellationTokenSource();
                    Assert.That(async () => await rc.DropReplicationSlot(slotName, cancellationToken: cts.Token), Throws.Exception.AssignableTo<OperationCanceledException>());
                });

        [Test]
        public Task DropReplicationSlotDisposed()
            => SafeReplicationTest(
                async (slotName, _) =>
                {
                    await CreateReplicationSlot(slotName);
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.DisposeAsync();
                    Assert.That(async () => await rc.DropReplicationSlot(slotName), Throws.InstanceOf<ObjectDisposedException>()
                        .With.Property(nameof(ObjectDisposedException.ObjectName))
                        .EqualTo(typeof(TConnection).Name));
                });

        #endregion

        [Test(Description = "Tests whether our automated feedback thread prevents the backend from disconnecting due to wal_sender_timeout")]
        public Task ReplicationSurvivesPausesLongerThanWalSenderTimeout()
            => SafeReplicationTest(
                async (slotName, tableName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    TestUtil.MinimumPgVersion(c, "10.0", "The SHOW command, which is required to run this test was added to the Streaming Replication Protocol in PostgreSQL 10");
                    var messages = new ConcurrentQueue<ReplicationMessage>();
                    await c.ExecuteNonQueryAsync($"CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);");
                    await using var rc = await OpenReplicationConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        ApplicationName = slotName
                    });
                    var walSenderTimeout = ParseTimespan(await rc.Show("wal_sender_timeout"));
                    var info = await rc.IdentifySystem();
                    if (walSenderTimeout > TimeSpan.FromSeconds(3) && !TestUtil.IsOnBuildServer)
                        Assert.Ignore($"wal_sender_timeout is set to {walSenderTimeout}, skipping");
                    Console.WriteLine($"The server wal_sender_timeout is configured to {walSenderTimeout}");
                    var walReceiverStatusInterval = TimeSpan.FromTicks(walSenderTimeout.Ticks / 2L);
                    Console.WriteLine($"Setting {nameof(ReplicationConnection)}.{nameof(ReplicationConnection.WalReceiverStatusInterval)} to {walReceiverStatusInterval}");
                    rc.WalReceiverStatusInterval = walReceiverStatusInterval;
                    await CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"INSERT INTO \"{tableName}\" (name) VALUES ('val1')");
                    using var streamingCts = new CancellationTokenSource();

                    var replicationEnumerator = StartReplication(rc, slotName, info.XLogPos, streamingCts.Token).GetAsyncEnumerator(streamingCts.Token);

                    var delay = TimeSpan.FromTicks((long)(walSenderTimeout.Ticks * 1.1));
                    Console.WriteLine($"Going to sleep for {delay}");
                    await Task.Delay(delay, CancellationToken.None);

                    Assert.That(await replicationEnumerator.MoveNextAsync(), Is.True);
                    var message = replicationEnumerator.Current;
                    Assert.That(message.WalStart, Is.GreaterThanOrEqualTo(info.XLogPos));
                    Assert.That(message.WalEnd, Is.GreaterThanOrEqualTo(message.WalStart));

                    streamingCts.Cancel();

                    Assert.That(async () => { while (await replicationEnumerator.MoveNextAsync()){} }, Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));

                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether synchronous replication works the way it should.")]
        [Explicit("Test is flaky (on Windows)")]
        public Task SynchronousReplication()
            => SafeReplicationTest(
                async (slotName, tableName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    //TestUtil.MinimumPgVersion(c, "9.4", "Logical Replication was introduced in PostgreSQL 9.4");
                    //
                    TestUtil.MinimumPgVersion(c, "12.0", "Setting wal_sender_timeout at runtime was introduced in in PostgreSQL 12");

                    var synchronousCommit = (string)(await c.ExecuteScalarAsync("SHOW synchronous_commit"))!;
                    if (synchronousCommit != "local")
                        TestUtil.IgnoreExceptOnBuildServer("Ignoring because synchronous_commit isn't 'local'");
                    var synchronousStandbyNames = (string)(await c.ExecuteScalarAsync("SHOW synchronous_standby_names"))!;
                    if (synchronousStandbyNames != "npgsql_test_sync_standby")
                        TestUtil.IgnoreExceptOnBuildServer("Ignoring because synchronous_standby_names isn't 'npgsql_test_sync_standby'");

                    await c.ExecuteNonQueryAsync(@$"
    CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
    ");

                    await using var rc = await OpenReplicationConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString)
                    {
                        // This must be one of the configured synchronous_standby_names from postgresql.conf
                        ApplicationName = "npgsql_test_sync_standby",
                        // We need wal_sender_timeout to be at least twice checkpoint_timeout to avoid getting feedback requests
                        // from the backend in physical replication which makes this test fail, so we disable it for this test.
                        Options = "-c wal_sender_timeout=0"
                    });
                    var info = await rc.IdentifySystem();

                    // Set WalReceiverStatusInterval to infinite so that the automated feedback doesn't interfere with
                    // our manual feedback
                    rc.WalReceiverStatusInterval = Timeout.InfiniteTimeSpan;

                    await CreateReplicationSlot(slotName);
                    using var streamingCts = new CancellationTokenSource();
                    var messages = ParseMessages(
                            StartReplication(rc, slotName, info.XLogPos, streamingCts.Token))
                        .GetAsyncEnumerator();

                    var value1String = Guid.NewGuid().ToString("B");
                    // We need to start a separate thread here as the insert command wil not complete until
                    // the transaction successfully completes (which we block here from the standby side) and by that
                    // will occupy the connection it is bound to.
                    var insertTask = Task.Run(async () =>
                    {
                        await using var insertConn = await OpenConnectionAsync(new NpgsqlConnectionStringBuilder(ConnectionString)
                        {
                            Options = "-c synchronous_commit=on"
                        });
                        await insertConn.ExecuteNonQueryAsync($"INSERT INTO {tableName} (name) VALUES ('{value1String}')");
                    });

                    var commitLsn = await GetCommitLsn(value1String);

                    var result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.Null); // Not committed yet because we didn't report fsync yet

                    // Report last received LSN
                    await rc.SendStatusUpdate(CancellationToken.None);

                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.Null); // Not committed yet because we still didn't report fsync yet

                    // Report last applied LSN
                    rc.LastAppliedLsn = commitLsn;
                    await rc.SendStatusUpdate(CancellationToken.None);

                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.Null); // Not committed yet because we still didn't report fsync yet

                    // Report last flushed LSN
                    rc.LastFlushedLsn = commitLsn;
                    await rc.SendStatusUpdate(CancellationToken.None);

                    await insertTask;
                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.EqualTo(value1String)); // Now it's committed because we reported fsync

                    var value2String = Guid.NewGuid().ToString("B");
                    insertTask = Task.Run(async () =>
                    {
                        await using var insertConn = OpenConnection(new NpgsqlConnectionStringBuilder(ConnectionString)
                        {
                            Options = "-c synchronous_commit=remote_apply"
                        });
                        await insertConn.ExecuteNonQueryAsync($"INSERT INTO {tableName} (name) VALUES ('{value2String}')");
                    });

                    commitLsn = await GetCommitLsn(value2String);

                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.EqualTo(value1String)); // Not committed yet because we didn't report apply yet

                    // Report last received LSN
                    await rc.SendStatusUpdate(CancellationToken.None);

                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.EqualTo(value1String)); // Not committed yet because we still didn't report apply yet

                    // Report last applied LSN
                    rc.LastAppliedLsn = commitLsn;
                    await rc.SendStatusUpdate(CancellationToken.None);

                    await insertTask;
                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.EqualTo(value2String)); // Now it's committed because we reported apply

                    var value3String = Guid.NewGuid().ToString("B");
                    insertTask = Task.Run(async () =>
                    {
                        await using var insertConn = OpenConnection(new NpgsqlConnectionStringBuilder(ConnectionString)
                        {
                            Options = "-c synchronous_commit=remote_write"
                        });
                        await insertConn.ExecuteNonQueryAsync($"INSERT INTO {tableName} (name) VALUES ('{value3String}')");
                    });

                    await GetCommitLsn(value3String);

                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.EqualTo(value2String)); // Not committed yet because we didn't report receive yet

                    // Report last received LSN
                    await rc.SendStatusUpdate(CancellationToken.None);

                    await insertTask;
                    result = await c.ExecuteScalarAsync($"SELECT name FROM {tableName} ORDER BY id DESC LIMIT 1;");
                    Assert.That(result, Is.EqualTo(value3String)); // Now it's committed because we reported receive

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);

                    static async IAsyncEnumerable<(NpgsqlLogSequenceNumber Lsn, string? MessageData)> ParseMessages(
                        IAsyncEnumerable<ReplicationMessage> messages)
                    {
                        await foreach (var msg in messages)
                        {
                            if (typeof(TConnection) == typeof(PhysicalReplicationConnection))
                            {
                                var buffer = new MemoryStream();
                                ((XLogDataMessage)msg).Data.CopyTo(buffer);
                                // Hack: This is really gruesome but we really have no idea how many
                                // messages we get in physical replication
                                var messageString = Encoding.ASCII.GetString(buffer.ToArray());
                                yield return (msg.WalEnd, messageString);
                            }
                            else
                            {
                                yield return (msg.WalEnd, null);
                            }
                        }
                    }

                    async Task<NpgsqlLogSequenceNumber> GetCommitLsn(string valueString)
                    {
                        if (typeof(TConnection) == typeof(PhysicalReplicationConnection))
                            while (await messages.MoveNextAsync())
                                if (messages.Current.MessageData!.Contains(valueString))
                                    return messages.Current.Lsn;

                        // NpgsqlLogicalReplicationConnection
                        // Begin Transaction, Insert, Commit Transaction
                        for (var i = 0; i < 3; i++)
                            Assert.True(await messages.MoveNextAsync());
                        return messages.Current.Lsn;

                    }
                });

        #region BaseBackup

        // ToDo: Implement BaseBackup and create tests for it

        #endregion

        async Task CreateReplicationSlot(string slotName)
        {
            await using var c = await OpenConnectionAsync();
            await c.ExecuteNonQueryAsync(typeof(TConnection) == typeof(PhysicalReplicationConnection)
                ? $"SELECT pg_create_physical_replication_slot('{slotName}')"
                : $"SELECT pg_create_logical_replication_slot ('{slotName}', 'test_decoding')");
        }

        async IAsyncEnumerable<ReplicationMessage> StartReplication(TConnection connection, string slotName,
            NpgsqlLogSequenceNumber xLogPos, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (typeof(TConnection) == typeof(PhysicalReplicationConnection))
            {
                var slot = new PhysicalReplicationSlot(slotName);
                var rc = (PhysicalReplicationConnection)(ReplicationConnection)connection;
                await foreach (var msg in rc.StartReplication(slot, xLogPos, cancellationToken))
                {
                    yield return msg;
                }
            }
            else if (typeof(TConnection) == typeof(LogicalReplicationConnection))
            {
                var slot = new TestDecodingReplicationSlot(slotName);
                var rc = (LogicalReplicationConnection)(ReplicationConnection)connection;
                await foreach (var msg in rc.StartReplication(slot, cancellationToken, walLocation: xLogPos))
                {
                    yield return msg;
                }
            }
        }

        static TimeSpan ParseTimespan(string str)
        {
            var span = str.AsSpan();
            var pos = 0;
            var number = 0;
            while (pos < span.Length)
            {
                var c = span[pos];
                if (!char.IsDigit(c))
                    break;
                number = number * 10 + (c - 0x30);
                pos++;
            }

            if (number == 0)
                return Timeout.InfiniteTimeSpan;
            if ("ms".AsSpan().Equals(span.Slice(pos), StringComparison.Ordinal))
                return TimeSpan.FromMilliseconds(number);
            if ("s".AsSpan().Equals(span.Slice(pos), StringComparison.Ordinal))
                return TimeSpan.FromSeconds(number);
            if ("min".AsSpan().Equals(span.Slice(pos), StringComparison.Ordinal))
                return TimeSpan.FromMinutes(number);
            if ("h".AsSpan().Equals(span.Slice(pos), StringComparison.Ordinal))
                return TimeSpan.FromHours(number);
            if ("d".AsSpan().Equals(span.Slice(pos), StringComparison.Ordinal))
                return TimeSpan.FromDays(number);

            throw new ArgumentException($"Can not parse timestamp '{span.ToString()}'");
        }

        protected override string Postfix =>
            "common_" +
            new TConnection() switch
            {
                LogicalReplicationConnection _ => "_l",
                PhysicalReplicationConnection _ => "_p",
                _ => throw new ArgumentOutOfRangeException($"{typeof(TConnection)} is not expected.")
            };
    }
}
