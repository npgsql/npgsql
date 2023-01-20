using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using Npgsql.Replication.TestDecoding;

namespace Npgsql.Tests.Replication;

/// <summary>
/// These tests are meant to run on PostgreSQL versions back to 9.4 where the
/// implementation of logical replication was still somewhat incomplete.
/// Please don't change them without confirming that they still work on those old versions.
/// </summary>
[Platform(Exclude = "MacOsX", Reason = "Replication tests are flaky in CI on Mac")]
[NonParallelizable] // These tests aren't designed to be parallelizable
public class TestDecodingReplicationTests : SafeReplicationTestBase<LogicalReplicationConnection>
{
    [Test]
    public Task CreateTestDecodingReplicationSlot()
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var rc = await OpenReplicationConnectionAsync();
                var options = await rc.CreateTestDecodingReplicationSlot(slotName);

                await using var c = await OpenConnectionAsync();
                using var cmd =
                    new NpgsqlCommandOrig($"SELECT * FROM pg_replication_slots WHERE slot_name = '{options.Name}'",
                        c);
                await using var reader = await cmd.ExecuteReaderAsync();

                Assert.That(reader.Read, Is.True);
                Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("slot_type")), Is.EqualTo("logical"));
                Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("plugin")), Is.EqualTo("test_decoding"));
                Assert.That(reader.Read, Is.False);
            });

    [Test(Description = "Tests whether INSERT commands get replicated via test_decoding plugin")]
    public Task Insert()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync($"CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL)");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} (name) VALUES ('val1'), ('val2')");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Insert first value
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: INSERT: id[integer]:1 name[text]:'val1'"));

                // Insert second value
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: INSERT: id[integer]:2 name[text]:'val2'"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether UPDATE commands get replicated via test_decoding plugin for tables using the default replica identity")]
    public Task Update_for_default_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync($@"CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
INSERT INTO {tableName} (name) VALUES ('val'), ('val2')");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"UPDATE {tableName} SET name='val1' WHERE name='val'");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Update
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: UPDATE: id[integer]:1 name[text]:'val1'"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether UPDATE commands get replicated via test_decoding plugin for tables using an index as replica identity")]
    public Task Update_for_index_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                var indexName = $"i_{tableName.Substring(2)}";
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
CREATE UNIQUE INDEX {indexName} ON {tableName} (name);
ALTER TABLE {tableName} REPLICA IDENTITY USING INDEX {indexName};
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"UPDATE {tableName} SET name='val1' WHERE name='val'");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Update
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: UPDATE: old-key: name[text]:'val' new-tuple: id[integer]:1 name[text]:'val1'"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether UPDATE commands get replicated via test_decoding plugin for tables using full replica identity")]
    public Task Update_for_full_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
ALTER TABLE {tableName} REPLICA IDENTITY FULL;
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"UPDATE {tableName} SET name='val1' WHERE name='val'");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Update
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: UPDATE: old-key: id[integer]:1 name[text]:'val' new-tuple: id[integer]:1 name[text]:'val1'"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether DELETE commands get replicated via test_decoding plugin for tables using the default replica identity")]
    public Task Delete_for_default_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"DELETE FROM {tableName} WHERE name='val2'");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Delete
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: DELETE: id[integer]:2"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether DELETE commands get replicated via test_decoding plugin for tables using an index as replica identity")]
    public Task Delete_for_index_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                var indexName = $"i_{tableName.Substring(2)}";
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
CREATE UNIQUE INDEX {indexName} ON {tableName} (name);
ALTER TABLE {tableName} REPLICA IDENTITY USING INDEX {indexName};
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"DELETE FROM {tableName} WHERE name='val2'");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Delete
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: DELETE: name[text]:'val2'"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether DELETE commands get replicated via test_decoding plugin for tables using full replica identity")]
    public Task Delete_for_full_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
ALTER TABLE {tableName} REPLICA IDENTITY FULL;
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"DELETE FROM {tableName} WHERE name='val2'");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Delete
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: DELETE: id[integer]:2 name[text]:'val2'"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    [Test(Description = "Tests whether TRUNCATE commands get replicated via test_decoding plugin")]
    public Task Truncate()
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(c, "11.0", "Replication of TRUNCATE commands was introduced in PostgreSQL 11");
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id serial PRIMARY KEY, name TEXT NOT NULL);
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreateTestDecodingReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"TRUNCATE TABLE {tableName} RESTART IDENTITY CASCADE");

                using var streamingCts = new CancellationTokenSource();
                var messages = rc.StartReplication(slot, streamingCts.Token, new TestDecodingOptions(skipEmptyXacts: true)).GetAsyncEnumerator();

                // Begin Transaction
                var message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("BEGIN "));

                // Truncate
                message = await NextMessage(messages);
                Assert.That(message.Data,
                    Is.EqualTo($"table public.{tableName}: TRUNCATE: restart_seqs cascade"));

                // Commit Transaction
                message = await NextMessage(messages);
                Assert.That(message.Data, Does.StartWith("COMMIT "));

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
            });

    static async ValueTask<TestDecodingData> NextMessage(IAsyncEnumerator<TestDecodingData> enumerator)
    {
        Assert.True(await enumerator.MoveNextAsync());
        return enumerator.Current!;
    }

    protected override string Postfix => "test_encoding_l";

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var c = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(c, "9.4", "Logical Replication was introduced in PostgreSQL 9.4");
    }
}
