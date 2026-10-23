using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Replication.Internal;
using Npgsql.Replication.TestDecoding;

namespace Npgsql.Tests.Replication;

[TestFixture(typeof(LogicalReplicationConnection))]
[TestFixture(typeof(PhysicalReplicationConnection))]
[NonParallelizable]
public class ReplicationSlotNameTests<TConnection> : SafeReplicationTestBase<TConnection>
    where TConnection : ReplicationConnection, new()
{
    static bool IsPhysical => typeof(TConnection) == typeof(PhysicalReplicationConnection);

    [Test, Description("Valid slot names may start with a digit, which only works when Npgsql quotes them")]
    public Task Slot_name_starting_with_a_digit()
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                var digitSlotName = "1" + slotName[1..];
                await using var c = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(c, "10.0",
                    "Before PostgreSQL 10 the stream's cancellation breaks the connection, and DROP_REPLICATION_SLOT has no WAIT");
                await DropSlotIfExists(c, digitSlotName);
                try
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    var info = await rc.IdentifySystem();
                    if (IsPhysical)
                    {
                        var prc = (PhysicalReplicationConnection)(ReplicationConnection)rc;
                        var slot = await prc.CreateReplicationSlot(digitSlotName);
                        Assert.That(slot.Name, Is.EqualTo(digitSlotName));
                        if (c.PostgreSqlVersion.Major >= 15)
                            Assert.That((await prc.ReadReplicationSlot(digitSlotName))?.Name, Is.EqualTo(digitSlotName));
                    }
                    else
                    {
                        var lrc = (LogicalReplicationConnection)(ReplicationConnection)rc;
                        var slot = await lrc.CreateTestDecodingReplicationSlot(digitSlotName);
                        Assert.That(slot.Name, Is.EqualTo(digitSlotName));
                    }

                    using var streamingCts = new CancellationTokenSource();
                    var enumerator = StartReplication(rc, digitSlotName, info.XLogPos, streamingCts.Token);
                    var moveNext = enumerator.MoveNextAsync().AsTask();
                    await WaitForActiveSlot(c, digitSlotName, moveNext);

                    streamingCts.Cancel();
                    Assert.That(async () =>
                    {
                        while (await moveNext)
                            moveNext = enumerator.MoveNextAsync().AsTask();
                    }, Throws.Exception.AssignableTo<OperationCanceledException>());
                    await enumerator.DisposeAsync();

                    await rc.DropReplicationSlot(digitSlotName, wait: true);
                    Assert.That(
                        await c.ExecuteScalarAsync($"SELECT EXISTS (SELECT * FROM pg_replication_slots WHERE slot_name = '{digitSlotName}')"),
                        Is.False);
                }
                finally
                {
                    await DropSlotIfExists(c, digitSlotName);
                }
            });

    static IAsyncEnumerator<ReplicationMessage> StartReplication(
        TConnection connection, string slotName, NpgsqlTypes.NpgsqlLogSequenceNumber walLocation, CancellationToken cancellationToken)
        => IsPhysical
            ? ((PhysicalReplicationConnection)(ReplicationConnection)connection)
                .StartReplication(new PhysicalReplicationSlot(slotName), walLocation, cancellationToken)
                .GetAsyncEnumerator(cancellationToken)
            : ((LogicalReplicationConnection)(ReplicationConnection)connection)
                .StartReplication(new TestDecodingReplicationSlot(slotName), cancellationToken, walLocation: walLocation)
                .GetAsyncEnumerator(cancellationToken);

    static async Task WaitForActiveSlot(NpgsqlConnection c, string slotName, Task<bool> moveNext)
    {
        for (var i = 0; i < 100; i++)
        {
            // Surface the error if START_REPLICATION failed
            if (moveNext.IsFaulted)
                await moveNext;
            if ((bool?)await c.ExecuteScalarAsync($"SELECT active FROM pg_replication_slots WHERE slot_name = '{slotName}'") == true)
                return;
            await Task.Delay(100);
        }

        Assert.Fail($"Replication slot {slotName} didn't become active");
    }

    static async Task DropSlotIfExists(NpgsqlConnection c, string slotName)
    {
        // A failed test run may have left behind a walsender that still holds the slot. pg_terminate_backend's timeout argument
        // needs PostgreSQL 14, so we wait for the slot to become inactive ourselves.
        await c.ExecuteNonQueryAsync(
            $"SELECT pg_terminate_backend(active_pid) FROM pg_replication_slots WHERE slot_name = '{slotName}' AND active_pid IS NOT NULL");
        for (var i = 0; i < 50 && (bool)(await c.ExecuteScalarAsync(
                 $"SELECT EXISTS (SELECT * FROM pg_replication_slots WHERE slot_name = '{slotName}' AND active)"))!; i++)
        {
            await Task.Delay(100);
        }
        await c.ExecuteNonQueryAsync($"SELECT pg_drop_replication_slot(slot_name) FROM pg_replication_slots WHERE slot_name = '{slotName}'");
    }

    protected override string Postfix => "slot_names_" + (IsPhysical ? "p" : "l");
}

[NonParallelizable]
public class LogicalReplicationOptionTests : SafeReplicationTestBase<LogicalReplicationConnection>
{
    [Test]
    public async Task Null_option_names_are_rejected()
    {
        await using var rc = await OpenReplicationConnectionAsync();
        var enumerator = rc.StartLogicalReplication(
            new TestDecodingReplicationSlot("unused"), CancellationToken.None, options: [new(null!, "x")]).GetAsyncEnumerator();
        Assert.That(async () => await enumerator.MoveNextAsync(),
            Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("options"));
        await enumerator.DisposeAsync();
    }

    [Test, Description("Publication names are sent as case-sensitive quoted identifiers, so they may contain quotes and commas")]
    public Task Publication_names_with_special_characters()
        => SafeReplicationTest(
            async (slotName, tableNames, _) =>
            {
                string[] publicationNames = ["Npgsql \"Quoted\", Pub", "npgsql_it's_pub"];
                await using var c = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(c, "10.0", "Publications and pgoutput were introduced in PostgreSQL 10");
                await DropPublications(c, publicationNames);
                try
                {
                    await c.ExecuteNonQueryAsync(
                        $"""
                         CREATE TABLE {tableNames[0]} (id INT PRIMARY KEY);
                         CREATE TABLE {tableNames[1]} (id INT PRIMARY KEY);
                         CREATE PUBLICATION {QuoteIdentifier(publicationNames[0])} FOR TABLE {tableNames[0]};
                         CREATE PUBLICATION {QuoteIdentifier(publicationNames[1])} FOR TABLE {tableNames[1]};
                         """);
                    await using var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreatePgOutputReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableNames[0]} VALUES (1)");
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableNames[1]} VALUES (2)");

                    using var streamingCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var options = new PgOutputReplicationOptions(publicationNames, PgOutputProtocolVersion.V1);
                    var enumerator = rc.StartReplication(slot, options, streamingCts.Token).GetAsyncEnumerator();
                    var relationNames = new List<string>();

                    // Both publications deliver their table's insert
                    while (relationNames.Count < 2 && await enumerator.MoveNextAsync())
                    {
                        if (enumerator.Current is RelationMessage relation)
                            relationNames.Add(relation.RelationName);
                    }
                    Assert.That(relationNames, Is.EqualTo(tableNames));

                    streamingCts.Cancel();
                    Assert.That(async () =>
                    {
                        while (await enumerator.MoveNextAsync())
                        {
                        }
                    }, Throws.Exception.AssignableTo<OperationCanceledException>());
                    await enumerator.DisposeAsync();
                }
                finally
                {
                    await DropPublications(c, publicationNames);
                }
            }, tableCount: 2);

    static async Task DropPublications(NpgsqlConnection c, string[] publicationNames)
    {
        foreach (var publicationName in publicationNames)
            await c.ExecuteNonQueryAsync($"DROP PUBLICATION IF EXISTS {QuoteIdentifier(publicationName)}");
    }

    static string QuoteIdentifier(string identifier) => "\"" + identifier.Replace("\"", "\"\"") + "\"";

    protected override string Postfix => "options_l";
}
