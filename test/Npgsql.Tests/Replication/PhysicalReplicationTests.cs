using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;

namespace Npgsql.Tests.Replication
{
    [Explicit("Flakiness")]
    public class PhysicalReplicationTests : SafeReplicationTestBase<PhysicalReplicationConnection>
    {
        [Test]
        public Task CreateReplicationSlot()
            => SafeReplicationTest(
                async (slotName, _) =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);

                    await using var c = await OpenConnectionAsync();
                    using var cmd =
                        new NpgsqlCommand($"SELECT * FROM pg_replication_slots WHERE slot_name = '{slot.Name}'",
                            c);
                    await using var reader = await cmd.ExecuteReaderAsync();

                    Assert.That(reader.Read, Is.True);
                    Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("slot_type")), Is.EqualTo("physical"));
                    Assert.That(reader.Read, Is.False);
                    await rc.DropReplicationSlot(slotName);
                });

        [Test]
        public Task Replication_with_slot()
            => SafeReplicationTest(
                async (slotName, tableName) =>
                {
                    // var messages = new ConcurrentQueue<(NpgsqlLogSequenceNumber WalStart, NpgsqlLogSequenceNumber WalEnd, byte[] data)>();
                    var rc = await OpenReplicationConnectionAsync();
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
            var rc = await OpenReplicationConnectionAsync();
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
}
