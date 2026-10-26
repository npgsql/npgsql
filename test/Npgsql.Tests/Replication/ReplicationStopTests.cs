using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Replication.TestDecoding;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql.Tests.Replication;

[TestFixture(typeof(LogicalReplicationConnection))]
[TestFixture(typeof(PhysicalReplicationConnection))]
[NonParallelizable]
public class ReplicationStopTests<TConnection> : SafeReplicationTestBase<TConnection>
    where TConnection : ReplicationConnection, new()
{
    static bool IsPhysical => typeof(TConnection) == typeof(PhysicalReplicationConnection);

    [Test, Description("Stopping a stream early (here with break) leaves the connection usable, also for streaming again")]
    public Task Stop_early_and_go_on()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                if (!IsPhysical)
                    TestUtil.MinimumPgVersion(c, "10.0", "Before PostgreSQL 10 stopping a logical stream breaks the connection");
                await c.ExecuteNonQueryAsync($"CREATE TABLE {tableName} (value text)");
                if (!IsPhysical)
                    await c.ExecuteNonQueryAsync($"SELECT pg_create_logical_replication_slot('{slotName}', 'test_decoding')");
                await using var rc = await OpenReplicationConnectionAsync();
                var info = await rc.IdentifySystem();
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                for (var round = 0; round < 2; round++)
                {
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES ('value {round}')");
                    await foreach (var _ in StartReplication(rc, slotName, info.XLogPos, cts.Token))
                        break;

                    Assert.That((await rc.IdentifySystem(cts.Token)).SystemId, Is.EqualTo(info.SystemId));
                }
            });

    static IAsyncEnumerable<ReplicationMessage> StartReplication(
        TConnection connection, string slotName, NpgsqlLogSequenceNumber walLocation, CancellationToken cancellationToken)
        => IsPhysical
            ? ((PhysicalReplicationConnection)(ReplicationConnection)connection).StartReplication(walLocation, cancellationToken)
            : ((LogicalReplicationConnection)(ReplicationConnection)connection)
                .StartReplication(new TestDecodingReplicationSlot(slotName), cancellationToken);

    protected override string Postfix => "stop_" + (IsPhysical ? "p" : "l");
}

[NonParallelizable]
public class PgOutputStopTests : SafeReplicationTestBase<LogicalReplicationConnection>
{
    [Test, Description("Stopping in the middle of a change, without reading its row, leaves the connection usable")]
    public Task Stop_in_the_middle_of_a_change()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(c, "10.0", "Publications and pgoutput were introduced in PostgreSQL 10");
                await c.ExecuteNonQueryAsync(
                    $"""
                     CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT);
                     CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
                     """);
                await using var rc = await OpenReplicationConnectionAsync();
                // Without exporting a snapshot: on PostgreSQL 15-17, the first stream after a CREATE_REPLICATION_SLOT that exported
                // one leaks pgoutput's relation cache when it ends with an error (like our cancellation), so the next stream on the
                // connection would lack the Relation messages (fixed in PostgreSQL 18 by 80b727eb9de). PostgreSQL 14 keeps that
                // cache after any error, so there we can't stream again on the same connection.
                var slot = await rc.CreatePgOutputReplicationSlot(slotName, slotSnapshotInitMode: LogicalSlotSnapshotInitMode.NoExport);
                // A row bigger than the read buffer
                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES (1, repeat('x', 20000))");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

                // The slot isn't advanced, so the second round gets the same change again
                var rounds = c.PostgreSqlVersion.IsGreaterOrEqual(15) ? 2 : 1;
                for (var round = 0; round < rounds; round++)
                {
                    var gotInsert = false;
                    await foreach (var message in rc.StartReplication(
                                       slot, new PgOutputReplicationOptions(publicationName, PgOutputProtocolVersion.V1), cts.Token))
                    {
                        if (message is InsertMessage)
                        {
                            gotInsert = true;
                            break;
                        }
                    }

                    Assert.That(gotInsert);
                    await rc.IdentifySystem(cts.Token);
                }
            });

    protected override string Postfix => "pgoutput_stop_l";
}
