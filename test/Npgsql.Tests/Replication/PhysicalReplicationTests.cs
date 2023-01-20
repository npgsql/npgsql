using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using NpgsqlTypes;

namespace Npgsql.Tests.Replication;

[Explicit("Flakiness")]
public class PhysicalReplicationTests : SafeReplicationTestBase<PhysicalReplicationConnection>
{
    [Test]
    public Task CreateReplicationSlot([Values]bool temporary, [Values]bool reserveWal)
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                if (reserveWal)
                    TestUtil.MinimumPgVersion(c, "10.0", "The RESERVE_WAL syntax was introduced in PostgreSQL 10");
                if (temporary)
                    TestUtil.MinimumPgVersion(c, "10.0", "Temporary replication slots were introduced in PostgreSQL 10");

                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateReplicationSlot(slotName, temporary, reserveWal);

                using var cmd =
                    new NpgsqlCommandOrig($"SELECT * FROM pg_replication_slots WHERE slot_name = '{slot.Name}'",
                        c);
                await using var reader = await cmd.ExecuteReaderAsync();

                Assert.That(reader.Read, Is.True);
                Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("slot_type")), Is.EqualTo("physical"));
                Assert.That(reader.Read, Is.False);
                await rc.DropReplicationSlot(slotName);
            }, nameof(CreateReplicationSlot) + (temporary ? "_t" : "") + (reserveWal ? "_r" : ""));

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, false)]
    public Task ReadReplicationSlot(bool createSlot, bool reserveWal)
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(c, "15.0", "The READ_REPLICATION_SLOT command was introduced in PostgreSQL 15");
                if (createSlot)
                    await c.ExecuteNonQueryAsync($"SELECT pg_create_physical_replication_slot('{slotName}', {reserveWal}, false)");
                using var cmd =
                    new NpgsqlCommandOrig($@"SELECT slot_name, substring(pg_walfile_name(restart_lsn), 1, 8)::bigint AS timeline_id, restart_lsn
                                            FROM pg_replication_slots
                                            WHERE slot_name = '{slotName}'", c);
                await using var reader = await cmd.ExecuteReaderAsync();
                Assert.That(reader.Read, Is.EqualTo(createSlot));
                var expectedSlotName = createSlot ? reader.GetFieldValue<string>(reader.GetOrdinal("slot_name")) : null;
                var expectedTli = createSlot ? unchecked((ulong?)reader.GetFieldValue<long?>(reader.GetOrdinal("timeline_id"))) : null;
                var expectedRestartLsn = createSlot ? reader.GetFieldValue<NpgsqlLogSequenceNumber?>(reader.GetOrdinal("restart_lsn")) : null;
                Assert.That(reader.Read, Is.False);
                await using var rc = await OpenReplicationConnectionAsync();

                var slot = await rc.ReadReplicationSlot(slotName);

                Assert.That(slot?.Name, Is.EqualTo(expectedSlotName));
                Assert.That(slot?.RestartTimeline, Is.EqualTo(expectedTli));
                Assert.That(slot?.RestartLsn, Is.EqualTo(expectedRestartLsn));

            }, $"{nameof(ReadReplicationSlot)}_{reserveWal}");

    [Test]
    public Task Replication_with_slot()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                // var messages = new ConcurrentQueue<(NpgsqlLogSequenceNumber WalStart, NpgsqlLogSequenceNumber WalEnd, byte[] data)>();
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateReplicationSlot(slotName);
                var info = await rc.IdentifySystem();

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, info.XLogPos, streamingCts.Token).GetAsyncEnumerator();

                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync($"CREATE TABLE {tableName} (value text)");

                for (var i = 1; i <= 10; i++)
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES ('Value {i}')");

                // We can't assert a lot in physical replication.
                // Since we're replicating in the scope of the whole cluster,
                // other transactions possibly from system processes can
                // interfere here, inserting additional messages, but more
                // likely we'll get everything in one big chunk.
                Assert.True(await messages.MoveNextAsync());
                var message = messages.Current;
                Assert.That(message.WalStart, Is.EqualTo(info.XLogPos));
                Assert.That(message.WalEnd, Is.GreaterThan(message.WalStart));
                Assert.That(message.Data.Length, Is.GreaterThan(0));

                streamingCts.Cancel();
                var exception = Assert.ThrowsAsync(Is.AssignableTo<OperationCanceledException>(), async () => await messages.MoveNextAsync());
                if (c.PostgreSqlVersion < Version.Parse("9.4"))
                {
                    Assert.That(exception, Has.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                }
            });

    [Test]
    public async Task Replication_without_slot()
    {
        await using var rc = await OpenReplicationConnectionAsync();
        var info = await rc.IdentifySystem();

        using var streamingCts = new CancellationTokenSource();
        var messages = rc.StartReplication(info.XLogPos, streamingCts.Token).GetAsyncEnumerator();

        var tableName = "t_physicalreplicationwithoutslot_p";
        await using var c = await OpenConnectionAsync();
        await c.ExecuteNonQueryAsync($"CREATE TABLE IF NOT EXISTS {tableName} (value text)");
        try
        {
            for (var i = 1; i <= 10; i++)
                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES ('Value {i}')");

            // We can't assert a lot in physical replication.
            // Since we're replicating in the scope of the whole cluster,
            // other transactions possibly from system processes can
            // interfere here, inserting additional messages, but more
            // likely we'll get everything in one big chunk.
            Assert.True(await messages.MoveNextAsync());
            var message = messages.Current;
            Assert.That(message.WalStart, Is.EqualTo(info.XLogPos));
            Assert.That(message.WalEnd, Is.GreaterThan(message.WalStart));
            Assert.That(message.Data.Length, Is.GreaterThan(0));

            streamingCts.Cancel();
            var exception = Assert.ThrowsAsync(Is.AssignableTo<OperationCanceledException>(), async () => await messages.MoveNextAsync());
            if (c.PostgreSqlVersion < Version.Parse("9.4"))
            {
                Assert.That(exception, Has.InnerException.InstanceOf<PostgresException>()
                    .And.InnerException.Property(nameof(PostgresException.SqlState))
                    .EqualTo(PostgresErrorCodes.QueryCanceled));
            }
        }
        finally
        {
            await c.ExecuteNonQueryAsync($"DROP TABLE {tableName}");
        }
    }

    protected override string Postfix => "physical_p";
}
