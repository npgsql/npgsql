using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication.Logical;
using Npgsql.Replication.Logical.Protocol;
using NUnit.Framework;

namespace Npgsql.Tests.Replication
{
    public class PgOutputReplicationTests : SafeReplicationTestBase<NpgsqlLogicalReplicationConnection>
    {
        [Test]
        public Task CreateReplicationSlot()
            => SafeReplicationTest(
                async (slotName, _) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await using var rc = await OpenReplicationConnectionAsync();
                    var options = await rc.CreateReplicationSlot(slotName);

                    using var cmd =
                        new NpgsqlCommand($"SELECT * FROM pg_replication_slots WHERE slot_name = '{options.SlotName}'",
                            c);
                    await using var reader = await cmd.ExecuteReaderAsync();

                    Assert.That(reader.Read, Is.True);
                    Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("slot_type")), Is.EqualTo("logical"));
                    Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("plugin")), Is.EqualTo("pgoutput"));
                    Assert.That(reader.Read, Is.False);
                });

        [Test(Description = "Tests whether INSERT commands get replicated as Logical Replication Protocol Messages")]
        public Task Insert()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} (name) VALUES ('val1'), ('val2')");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('d'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Insert first value
                    var insertMsg = await NextMessage<InsertMessage>(messages);
                    Assert.That(insertMsg.NewRow.Length, Is.EqualTo(2));
                    Assert.That(insertMsg.NewRow[0].Value, Is.EqualTo("1"));
                    Assert.That(insertMsg.NewRow[1].Value, Is.EqualTo("val1"));

                    // Insert second value
                    insertMsg = await NextMessage<InsertMessage>(messages);
                    Assert.That(insertMsg.NewRow.Length, Is.EqualTo(2));
                    Assert.That(insertMsg.NewRow[0].Value, Is.EqualTo("2"));
                    Assert.That(insertMsg.NewRow[1].Value, Is.EqualTo("val2"));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether UPDATE commands get replicated as Logical Replication Protocol Messages for tables using the default replica identity")]
        public Task UpdateForDefaultReplicaIdentity()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"UPDATE {tableName} SET name='val1' WHERE name='val'");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('d'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Update
                    var updateMsg = await NextMessage<UpdateMessage>(messages);
                    Assert.That(updateMsg.NewRow.Length, Is.EqualTo(2));
                    Assert.That(updateMsg.NewRow[0].Value, Is.EqualTo("1"));
                    Assert.That(updateMsg.NewRow[1].Value, Is.EqualTo("val1"));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether UPDATE commands get replicated as Logical Replication Protocol Messages for tables using an index as replica identity")]
        public  Task UpdateForIndexReplicaIdentity()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    var indexName = $"i_{tableName.Substring(2)}";
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
CREATE UNIQUE INDEX {indexName} ON {tableName} (name);
ALTER TABLE {tableName} REPLICA IDENTITY USING INDEX {indexName};
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"UPDATE {tableName} SET name='val1' WHERE name='val'");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('i'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Update
                    var updateMsg = await NextMessage<IndexUpdateMessage>(messages);
                    Assert.That(updateMsg.KeyRow!.Length, Is.EqualTo(2));
                    Assert.That(updateMsg.KeyRow![0].Value, Is.Null);
                    Assert.That(updateMsg.KeyRow![1].Value, Is.EqualTo("val"));
                    Assert.That(updateMsg.NewRow.Length, Is.EqualTo(2));
                    Assert.That(updateMsg.NewRow[0].Value, Is.EqualTo("1"));
                    Assert.That(updateMsg.NewRow[1].Value, Is.EqualTo("val1"));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether UPDATE commands get replicated as Logical Replication Protocol Messages for tables using full replica identity")]
        public  Task UpdateForFullReplicaIdentity()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
ALTER TABLE {tableName} REPLICA IDENTITY FULL;
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"UPDATE {tableName} SET name='val1' WHERE name='val'");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('f'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Update
                    var updateMsg = await NextMessage<FullUpdateMessage>(messages);
                    Assert.That(updateMsg.OldRow!.Length, Is.EqualTo(2));
                    Assert.That(updateMsg.OldRow![0].Value, Is.EqualTo("1"));
                    Assert.That(updateMsg.OldRow![1].Value, Is.EqualTo("val"));
                    Assert.That(updateMsg.NewRow.Length, Is.EqualTo(2));
                    Assert.That(updateMsg.NewRow[0].Value, Is.EqualTo("1"));
                    Assert.That(updateMsg.NewRow[1].Value, Is.EqualTo("val1"));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether DELETE commands get replicated as Logical Replication Protocol Messages for tables using the default replica identity")]
        public  Task DeleteForDefaultReplicaIdentity()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"DELETE FROM {tableName} WHERE name='val2'");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('d'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Delete
                    var deleteMsg = await NextMessage<KeyDeleteMessage>(messages);
                    Assert.That(deleteMsg.KeyRow!.Length, Is.EqualTo(2));
                    Assert.That(deleteMsg.KeyRow[0].Value, Is.EqualTo("2"));
                    Assert.That(deleteMsg.KeyRow[1].Value, Is.Null);

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether DELETE commands get replicated as Logical Replication Protocol Messages for tables using an index as replica identity")]
        public Task DeleteForIndexReplicaIdentity()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    var indexName = $"i_{tableName.Substring(2)}";
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
CREATE UNIQUE INDEX {indexName} ON {tableName} (name);
ALTER TABLE {tableName} REPLICA IDENTITY USING INDEX {indexName};
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"DELETE FROM {tableName} WHERE name='val2'");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('i'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Delete
                    var deleteMsg = await NextMessage<KeyDeleteMessage>(messages);
                    Assert.That(deleteMsg.KeyRow!.Length, Is.EqualTo(2));
                    Assert.That(deleteMsg.KeyRow[0].Value, Is.Null);
                    Assert.That(deleteMsg.KeyRow[1].Value, Is.EqualTo("val2"));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether DELETE commands get replicated as Logical Replication Protocol Messages for tables using full replica identity")]
        public Task DeleteForFullReplicaIdentity()
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
ALTER TABLE {tableName} REPLICA IDENTITY FULL;
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    await c.ExecuteNonQueryAsync($"DELETE FROM {tableName} WHERE name='val2'");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('f'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Delete
                    var deleteMsg = await NextMessage<FullDeleteMessage>(messages);
                    Assert.That(deleteMsg.OldRow!.Length, Is.EqualTo(2));
                    Assert.That(deleteMsg.OldRow[0].Value, Is.EqualTo("2"));
                    Assert.That(deleteMsg.OldRow[1].Value, Is.EqualTo("val2"));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                });

        [Test(Description = "Tests whether TRUNCATE commands get replicated as Logical Replication Protocol Messages on PostgreSQL 11 and above")]
        [TestCase(TruncateOptions.None)]
        [TestCase(TruncateOptions.Cascade)]
        [TestCase(TruncateOptions.RestartIdentity)]
        [TestCase(TruncateOptions.Cascade | TruncateOptions.RestartIdentity)]
        public Task Truncate(TruncateOptions truncateOptionFlags)
            => SafeReplicationTest(
                async (slotName, tableName, publicationName) =>
                {
                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
INSERT INTO {tableName} (name) VALUES ('val'), ('val2');
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                    var rc = await OpenReplicationConnectionAsync();
                    var slot = await rc.CreateReplicationSlot(slotName);
                    StringBuilder sb = new StringBuilder("TRUNCATE TABLE ").Append(tableName);
                    if (truncateOptionFlags.HasFlag(TruncateOptions.RestartIdentity))
                        sb.Append(" RESTART IDENTITY");
                    if (truncateOptionFlags.HasFlag(TruncateOptions.Cascade))
                        sb.Append(" CASCADE");
                    await c.ExecuteNonQueryAsync(sb.ToString());

                    using var streamingCts = new CancellationTokenSource();
                    var messages = rc.StartReplication(slot, new NpgsqlPgOutputPluginOptions(publicationName), streamingCts.Token)
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    _ = await NextMessage<BeginMessage>(messages);

                    // Relation
                    var relMsg = await NextMessage<RelationMessage>(messages);
                    Assert.That(relMsg.RelationReplicaIdentitySetting, Is.EqualTo('d'));
                    Assert.That(relMsg.Namespace, Is.EqualTo("public"));
                    Assert.That(relMsg.RelationName, Is.EqualTo(tableName));
                    Assert.That(relMsg.Columns.Length, Is.EqualTo(2));
                    Assert.That(relMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                    Assert.That(relMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                    // Truncate
                    var truncateMsg = await NextMessage<TruncateMessage>(messages);
                    Assert.That(truncateMsg.Options, Is.EqualTo(truncateOptionFlags));
                    Assert.That(truncateMsg.RelationIds.Length, Is.EqualTo(1));

                    // Commit Transaction
                    _ = await NextMessage<CommitMessage>(messages);

                    streamingCts.Cancel();
                    Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                        .With.InnerException.InstanceOf<PostgresException>()
                        .And.InnerException.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                }, nameof(Truncate) + truncateOptionFlags.ToString("D"));


        static async ValueTask<TExpected> NextMessage<TExpected>(
            IAsyncEnumerator<LogicalReplicationProtocolMessage> enumerator)
            where TExpected : LogicalReplicationProtocolMessage
        {
            Assert.True(await enumerator.MoveNextAsync());
            Assert.That(enumerator.Current, Is.TypeOf<TExpected>());
            return (TExpected)enumerator.Current!;
        }

        protected override string Postfix => "pgoutput_l";

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var c = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(c, "10.0", "The Logical Replication Protocol (via pgoutput plugin) was introduced in PostgreSQL 10");
        }
    }
}
