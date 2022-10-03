using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using TruncateOptions = Npgsql.Replication.PgOutput.Messages.TruncateMessage.TruncateOptions;
using ReplicaIdentitySetting = Npgsql.Replication.PgOutput.Messages.RelationMessage.ReplicaIdentitySetting;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Replication;

[TestFixture(ProtocolVersion.V1, ReplicationDataMode.DefaultReplicationDataMode, TransactionMode.DefaultTransactionMode)]
[TestFixture(ProtocolVersion.V1, ReplicationDataMode.BinaryReplicationDataMode, TransactionMode.DefaultTransactionMode)]
[TestFixture(ProtocolVersion.V2, ReplicationDataMode.DefaultReplicationDataMode, TransactionMode.StreamingTransactionMode)]
[TestFixture(ProtocolVersion.V3, ReplicationDataMode.DefaultReplicationDataMode, TransactionMode.DefaultTransactionMode)]
[TestFixture(ProtocolVersion.V3, ReplicationDataMode.DefaultReplicationDataMode, TransactionMode.StreamingTransactionMode)]
// We currently don't execute all possible combinations of settings for efficiency reasons because they don't
// interact in the current implementation.
// Feel free to uncomment some or all of the following lines if the implementation changed or you suspect a
// problem with some combination.
// [TestFixture(ProtocolVersion.V1, ReplicationDataMode.TextReplicationDataMode, TransactionMode.NonStreamingTransactionMode)]
// [TestFixture(ProtocolVersion.V2, ReplicationDataMode.DefaultReplicationDataMode, TransactionMode.DefaultTransactionMode)]
// [TestFixture(ProtocolVersion.V2, ReplicationDataMode.TextReplicationDataMode, TransactionMode.NonStreamingTransactionMode)]
// [TestFixture(ProtocolVersion.V2, ReplicationDataMode.BinaryReplicationDataMode, TransactionMode.DefaultTransactionMode)]
// [TestFixture(ProtocolVersion.V2, ReplicationDataMode.BinaryReplicationDataMode, TransactionMode.StreamingTransactionMode)]
// [TestFixture(ProtocolVersion.V3, ReplicationDataMode.TextReplicationDataMode, TransactionMode.NonStreamingTransactionMode)]
// [TestFixture(ProtocolVersion.V3, ReplicationDataMode.BinaryReplicationDataMode, TransactionMode.DefaultTransactionMode)]
// [TestFixture(ProtocolVersion.V3, ReplicationDataMode.BinaryReplicationDataMode, TransactionMode.StreamingTransactionMode)]
[Platform(Exclude = "MacOsX", Reason = "Replication tests are flaky in CI on Mac")]
[NonParallelizable] // These tests aren't designed to be parallelizable
public class PgOutputReplicationTests : SafeReplicationTestBase<LogicalReplicationConnection>
{
    readonly ulong _protocolVersion;
    readonly bool? _binary;
    readonly bool? _streaming;

    bool IsBinary => _binary ?? false;
    bool IsStreaming => _streaming ?? false;
    ulong Version => _protocolVersion;

    public PgOutputReplicationTests(ProtocolVersion protocolVersion, ReplicationDataMode dataMode, TransactionMode transactionMode)
    {
        _protocolVersion = (ulong)protocolVersion;
        _binary = dataMode == ReplicationDataMode.BinaryReplicationDataMode
            ? true
            : dataMode == ReplicationDataMode.TextReplicationDataMode
                ? false
                : null;
        _streaming = transactionMode == TransactionMode.StreamingTransactionMode
            ? true
            : transactionMode == TransactionMode.NonStreamingTransactionMode
                ? false
                : null;
    }

    [Test]
    public Task CreatePgOutputReplicationSlot()
    {
        // There's nothing special here for binary data or when streaming so only execute once
        if (IsBinary || IsStreaming)
            return Task.CompletedTask;

        return SafeReplicationTest(
            async (slotName, _) =>
            {

                await using var c = await OpenConnectionAsync();
                await using var rc = await OpenReplicationConnectionAsync();
                var options = await rc.CreatePgOutputReplicationSlot(slotName);

                using var cmd =
                    new NpgsqlCommand($"SELECT * FROM pg_replication_slots WHERE slot_name = '{options.Name}'",
                        c);
                await using var reader = await cmd.ExecuteReaderAsync();

                Assert.That(reader.Read, Is.True);
                Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("slot_type")), Is.EqualTo("logical"));
                Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("plugin")), Is.EqualTo("pgoutput"));
                Assert.That(reader.Read, Is.False);
            });
    }

    [Test(Description = "Tests whether INSERT commands get replicated as Logical Replication Protocol Messages")]
    public Task Insert()
        => SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NULL);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"INSERT INTO {tableName} VALUES (1, 'val1'), (2, NULL), (3, 'ignored');
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(4, 15000) s(i);");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.Default));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Insert first value
                var insertMsg = await NextMessage<InsertMessage>(messages);
                Assert.That(insertMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(insertMsg.Relation, Is.SameAs(relationMsg));
                var columnEnumerator = insertMsg.NewRow.GetAsyncEnumerator();
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await columnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("1"));

                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(columnEnumerator.Current.IsDBNull, Is.False);
                Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("val1"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.False);

                // Insert second value
                insertMsg = await NextMessage<InsertMessage>(messages);
                Assert.That(insertMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(insertMsg.Relation, Is.SameAs(relationMsg));
                columnEnumerator = insertMsg.NewRow.GetAsyncEnumerator();
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await columnEnumerator.Current.Get<int>(), Is.EqualTo(2));
                else
                    Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("2"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(columnEnumerator.Current.IsDBNull, Is.True);
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.False);

                // Insert third value
                insertMsg = await NextMessage<InsertMessage>(messages);
                Assert.That(insertMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(insertMsg.Relation, Is.SameAs(relationMsg));
                await foreach (var tuple in insertMsg.NewRow) // Don't consume the value to trigger eventual bugs
                    Assert.That(tuple.Kind, IsBinary ? Is.EqualTo(TupleDataKind.BinaryValue) : Is.EqualTo(TupleDataKind.TextValue));

                // Remaining inserts
                for (var insertCount = 0; insertCount < 14997; insertCount++)
                {
                    await NextMessage<InsertMessage>(messages);
                }

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });

    [Test(Description = "Tests whether UPDATE commands get replicated as Logical Replication Protocol Messages for tables using the default replica identity")]
    public Task Update_for_default_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"UPDATE {tableName} SET name='val1_updated' WHERE id = 1;
                                                    UPDATE {tableName} SET name = md5(name) WHERE id > 1");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.Default));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Update
                var updateMsg = await NextMessage<DefaultUpdateMessage>(messages);
                Assert.That(updateMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(updateMsg.Relation, Is.SameAs(relationMsg));
                var columnEnumerator = updateMsg.NewRow.GetAsyncEnumerator();
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await columnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("1"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(columnEnumerator.Current.IsDBNull, Is.False);
                Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("val1_updated"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.False);

                // Remaining updates
                for (var updateCount = 0; updateCount < 14999; updateCount++)
                    await NextMessage<DefaultUpdateMessage>(messages);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });

    [Test(Description = "Tests whether UPDATE commands get replicated as Logical Replication Protocol Messages for tables using an index as replica identity")]
    public  Task Update_for_index_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                var indexName = $"i_{tableName.Substring(2)}";
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    CREATE UNIQUE INDEX {indexName} ON {tableName} (name);
                                                    ALTER TABLE {tableName} REPLICA IDENTITY USING INDEX {indexName};
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"UPDATE {tableName} SET name='val1_updated' WHERE id = 1;
                                                    UPDATE {tableName} SET name = md5(name) WHERE id > 1");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.IndexWithIndIsReplIdent));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Update
                var updateMsg = await NextMessage<IndexUpdateMessage>(messages);
                Assert.That(updateMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(updateMsg.Relation, Is.SameAs(relationMsg));

                var oldRowColumnEnumerator = updateMsg.Key.GetAsyncEnumerator();
                Assert.That(await oldRowColumnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(oldRowColumnEnumerator.Current.IsDBNull, Is.True);
                Assert.That(await oldRowColumnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(await oldRowColumnEnumerator.Current.Get<string>(), Is.EqualTo("val1"));
                Assert.That(await oldRowColumnEnumerator.MoveNextAsync(), Is.False);

                var newRowColumnEnumerator = updateMsg.NewRow.GetAsyncEnumerator();
                Assert.That(await newRowColumnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await newRowColumnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await newRowColumnEnumerator.Current.Get<string>(), Is.EqualTo("1"));
                Assert.That(await newRowColumnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(await newRowColumnEnumerator.Current.Get<string>(), Is.EqualTo("val1_updated"));
                Assert.That(await newRowColumnEnumerator.MoveNextAsync(), Is.False);

                // Remaining updates
                for (var updateCount = 0; updateCount < 14999; updateCount++)
                    await NextMessage<IndexUpdateMessage>(messages);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });

    [Test(Description = "Tests whether UPDATE commands get replicated as Logical Replication Protocol Messages for tables using full replica identity")]
    public  Task Update_for_full_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    ALTER TABLE {tableName} REPLICA IDENTITY FULL;
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"UPDATE {tableName} SET name='val1_updated' WHERE id = 1;
                                                    UPDATE {tableName} SET name = md5(name) WHERE id > 1");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.AllColumns));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Update
                var updateMsg = await NextMessage<FullUpdateMessage>(messages);
                Assert.That(updateMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(updateMsg.Relation, Is.SameAs(relationMsg));

                var oldRowColumnEnumerator = updateMsg.OldRow.GetAsyncEnumerator();
                Assert.That(await oldRowColumnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await oldRowColumnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await oldRowColumnEnumerator.Current.Get<string>(), Is.EqualTo("1"));
                Assert.That(await oldRowColumnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(await oldRowColumnEnumerator.Current.Get<string>(), Is.EqualTo("val1"));
                Assert.That(await oldRowColumnEnumerator.MoveNextAsync(), Is.False);

                var newRowColumnEnumerator = updateMsg.NewRow.GetAsyncEnumerator();
                Assert.That(await newRowColumnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(await newRowColumnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(await newRowColumnEnumerator.Current.Get<string>(), Is.EqualTo("val1_updated"));
                Assert.That(await newRowColumnEnumerator.MoveNextAsync(), Is.False);

                // Remaining updates
                for (var updateCount = 0; updateCount < 14999; updateCount++)
                    await NextMessage<FullUpdateMessage>(messages);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                Assert.That(async () => await messages.MoveNextAsync(), Throws.Exception.AssignableTo<OperationCanceledException>()
                    .With.InnerException.InstanceOf<PostgresException>()
                    .And.InnerException.Property(nameof(PostgresException.SqlState))
                    .EqualTo(PostgresErrorCodes.QueryCanceled));
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });

    [Test(Description = "Tests whether DELETE commands get replicated as Logical Replication Protocol Messages for tables using the default replica identity")]
    public Task Delete_for_default_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"DELETE FROM {tableName} WHERE id = 1;
                                                    DELETE FROM {tableName} WHERE id > 1");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.Default));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Delete
                var deleteMsg = await NextMessage<KeyDeleteMessage>(messages);
                Assert.That(deleteMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(deleteMsg.Relation, Is.SameAs(relationMsg));
                var columnEnumerator = deleteMsg.Key.GetAsyncEnumerator();
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await columnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("1"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(columnEnumerator.Current.IsDBNull, Is.True);
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.False);

                // Remaining deletes
                for (var deleteCount = 0; deleteCount < 14999; deleteCount++)
                    await NextMessage<KeyDeleteMessage>(messages);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });

    [Test(Description = "Tests whether DELETE commands get replicated as Logical Replication Protocol Messages for tables using an index as replica identity")]
    public Task Delete_for_index_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                var indexName = $"i_{tableName.Substring(2)}";
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    CREATE UNIQUE INDEX {indexName} ON {tableName} (name);
                                                    ALTER TABLE {tableName} REPLICA IDENTITY USING INDEX {indexName};
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"DELETE FROM {tableName} WHERE id = 1;
                                                    DELETE FROM {tableName} WHERE id > 1");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.IndexWithIndIsReplIdent));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Delete
                var deleteMsg = await NextMessage<KeyDeleteMessage>(messages);
                Assert.That(deleteMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(deleteMsg.Relation, Is.SameAs(relationMsg));
                var columnEnumerator = deleteMsg.Key.GetAsyncEnumerator();
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(columnEnumerator.Current.IsDBNull, Is.True);
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("val1"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.False);

                // Remaining deletes
                for (var deleteCount = 0; deleteCount < 14999; deleteCount++)
                    await NextMessage<KeyDeleteMessage>(messages);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });

    [Test(Description = "Tests whether DELETE commands get replicated as Logical Replication Protocol Messages for tables using full replica identity")]
    public Task Delete_for_full_replica_identity()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    ALTER TABLE {tableName} REPLICA IDENTITY FULL;
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"DELETE FROM {tableName} WHERE id = 1;
                                                    DELETE FROM {tableName} WHERE id > 1");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMsg.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.AllColumns));
                Assert.That(relationMsg.Namespace, Is.EqualTo("public"));
                Assert.That(relationMsg.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("name"));

                // Delete
                var deleteMsg = await NextMessage<FullDeleteMessage>(messages);
                Assert.That(deleteMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(deleteMsg.Relation, Is.SameAs(relationMsg));
                var columnEnumerator = deleteMsg.OldRow.GetAsyncEnumerator();
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                if (IsBinary)
                    Assert.That(await columnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("1"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.True);
                Assert.That(columnEnumerator.Current.IsDBNull, Is.False);
                Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("val1"));
                Assert.That(await columnEnumerator.MoveNextAsync(), Is.False);

                // Remaining deletes
                for (var deleteCount = 0; deleteCount < 14999; deleteCount++)
                    await NextMessage<FullDeleteMessage>(messages);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
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
                TestUtil.MinimumPgVersion(c, "11.0", "Replication of TRUNCATE commands was introduced in PostgreSQL 11");
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
                                                    INSERT INTO {tableName} (name) VALUES ('val1');
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);
                var sb = new StringBuilder("TRUNCATE TABLE ").Append(tableName);
                if (truncateOptionFlags.HasFlag(TruncateOptions.RestartIdentity))
                    sb.Append(" RESTART IDENTITY");
                if (truncateOptionFlags.HasFlag(TruncateOptions.Cascade))
                    sb.Append(" CASCADE");
                sb.Append($"; INSERT INTO {tableName} (name) SELECT 'val' || i::text FROM generate_series(1, 15000) s(i);");

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(sb.ToString());
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // Relation
                var relationMessage = await NextMessage<RelationMessage>(messages);
                Assert.That(relationMessage.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(relationMessage.ReplicaIdentity, Is.EqualTo(ReplicaIdentitySetting.Default));
                Assert.That(relationMessage.Namespace, Is.EqualTo("public"));
                Assert.That(relationMessage.RelationName, Is.EqualTo(tableName));
                Assert.That(relationMessage.Columns.Count, Is.EqualTo(2));
                Assert.That(relationMessage.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMessage.Columns[1].ColumnName, Is.EqualTo("name"));

                // Truncate
                var truncateMsg = await NextMessage<TruncateMessage>(messages);
                Assert.That(truncateMsg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                Assert.That(truncateMsg.Options, Is.EqualTo(truncateOptionFlags));
                Assert.That(truncateMsg.Relations.Single(), Is.SameAs(relationMessage));

                // Remaining inserts
                // Since the inserts run in the same transaction as the truncate, we'll
                // get a RelationMessage after every StreamStartMessage
                for (var insertCount = 0; insertCount < 15000; insertCount++)
                    await NextMessage<InsertMessage>(messages, expectRelationMessage: true);

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            }, nameof(Truncate) + truncateOptionFlags.ToString("D"));

    [Test(Description = "Tests whether disposing while replicating will get us stuck forever.")]
    public Task Dispose_while_replicating()
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableName} (id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY, name TEXT NOT NULL);
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};
");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);
                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} (name) VALUES ('value 1'), ('value 2');");

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                await NextMessage<BeginMessage>(messages);
            }, nameof(Dispose_while_replicating));

    [TestCase(true)]
    [TestCase(false)]
    [Test(Description = "Tests whether logical decoding messages get replicated as Logical Replication Protocol Messages on PostgreSQL 14 and above")]
    public Task LogicalDecodingMessage(bool writeMessages)
        => SafeReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                const string prefix = "My test Prefix";
                const string transactionalMessage = "A transactional message";
                const string nonTransactionalMessage = "A non-transactional message";
                await using var c = await OpenConnectionAsync();
                TestUtil.MinimumPgVersion(c, "14.0", "Replication of logical decoding messages was introduced in PostgreSQL 14");
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NOT NULL);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"SELECT pg_logical_emit_message(true, '{prefix}', '{transactionalMessage}');
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);", tran);
                await tran.CommitAsync();

                await using var tran2 = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"SELECT pg_logical_emit_message(false, '{prefix}', '{nonTransactionalMessage}');
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(15001, 15010) s(i);
                                                    SELECT pg_logical_emit_message(true, '{prefix}', '{transactionalMessage}');
                                                    INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(15011, 30000) s(i);
                                                    SELECT pg_logical_emit_message(false, '{prefix}', '{nonTransactionalMessage}');
                                                    ", tran2);
                await tran2.RollbackAsync();
                await c.ExecuteNonQueryAsync(@$"SELECT pg_switch_wal();");

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot,
                        GetOptions(publicationName, writeMessages), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction 1
                var transactionXid = await AssertTransactionStart(messages);

                // LogicalDecodingMessage
                if (writeMessages)
                {
                    var msg = await NextMessage<LogicalDecodingMessage>(messages);
                    Assert.That(msg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                    Assert.That(msg.Flags, Is.EqualTo(1));
                    Assert.That(msg.Prefix, Is.EqualTo(prefix));
                    Assert.That(msg.Data.Length, Is.EqualTo(transactionalMessage.Length));
                    var buffer = new MemoryStream();
                    await msg.Data.CopyToAsync(buffer, CancellationToken.None);
                    Assert.That(rc.Encoding.GetString(buffer.ToArray()), Is.EqualTo(transactionalMessage));
                }

                // Relation
                await NextMessage<RelationMessage>(messages);

                // Inserts
                for (var insertCount = 0; insertCount < 15000; insertCount++)
                    await NextMessage<InsertMessage>(messages);

                // Commit Transaction 1
                await AssertTransactionCommit(messages);

                // LogicalDecodingMessage 1 (non-transactional)
                if (writeMessages)
                {
                    var msg = await NextMessage<LogicalDecodingMessage>(messages);
                    Assert.That(msg.TransactionXid, Is.Null);
                    Assert.That(msg.Flags, Is.EqualTo(0));
                    Assert.That(msg.Prefix, Is.EqualTo(prefix));
                    Assert.That(msg.Data.Length, Is.EqualTo(nonTransactionalMessage.Length));
                    var buffer = new MemoryStream();
                    await msg.Data.CopyToAsync(buffer, CancellationToken.None);
                    Assert.That(rc.Encoding.GetString(buffer.ToArray()), Is.EqualTo(nonTransactionalMessage));
                }

                if (IsStreaming)
                {
                    // Begin Transaction 2
                    transactionXid = await AssertTransactionStart(messages);

                    // Relation
                    await NextMessage<RelationMessage>(messages);

                    // Inserts
                    for (var insertCount = 0; insertCount < 10; insertCount++)
                        await NextMessage<InsertMessage>(messages);

                    // LogicalDecodingMessage 2 (transactional)
                    if (writeMessages)
                    {
                        var msg = await NextMessage<LogicalDecodingMessage>(messages);
                        Assert.That(msg.TransactionXid, IsStreaming ? Is.EqualTo(transactionXid) : Is.Null);
                        Assert.That(msg.Flags, Is.EqualTo(1));
                        Assert.That(msg.Prefix, Is.EqualTo(prefix));
                        Assert.That(msg.Data.Length, Is.EqualTo(transactionalMessage.Length));
                        var buffer = new MemoryStream();
                        await msg.Data.CopyToAsync(buffer, CancellationToken.None);
                        Assert.That(rc.Encoding.GetString(buffer.ToArray()), Is.EqualTo(transactionalMessage));
                    }

                    // Further inserts
                    // We don't try to predict how many insert messages we get here
                    // since the streaming transaction will most likely abort before
                    // we reach the expected number
                    while (await messages.MoveNextAsync() && messages.Current is InsertMessage
                           || messages.Current is StreamStopMessage
                           && await messages.MoveNextAsync()
                           && messages.Current is StreamStartMessage
                           && await messages.MoveNextAsync()
                           && messages.Current is InsertMessage)
                    {
                        // Ignore
                    }
                }
                else if (writeMessages)
                    await messages.MoveNextAsync();

                // LogicalDecodingMessage 3 (non-transactional)
                if (writeMessages)
                {
                    var msg = (LogicalDecodingMessage)messages.Current;
                    Assert.That(msg.TransactionXid, Is.Null);
                    Assert.That(msg.Flags, Is.EqualTo(0));
                    Assert.That(msg.Prefix, Is.EqualTo(prefix));
                    Assert.That(msg.Data.Length, Is.EqualTo(nonTransactionalMessage.Length));
                    var buffer = new MemoryStream();
                    await msg.Data.CopyToAsync(buffer, CancellationToken.None);
                    Assert.That(rc.Encoding.GetString(buffer.ToArray()), Is.EqualTo(nonTransactionalMessage));
                    if (IsStreaming)
                        await messages.MoveNextAsync();
                }

                // Rollback Transaction 2
                if (IsStreaming)
                    Assert.That(messages.Current, Is.TypeOf<StreamAbortMessage>());

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            }, $"{GetObjectName(nameof(LogicalDecodingMessage))}_m_{BoolToChar(writeMessages)}");

    [Test]
    public Task Stream()
    {
        // We don't test transaction streaming here because there's nothing special in that case
        if (IsStreaming)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (bytes bytea);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                var bytes = new byte[16384];
                for (var i = 0; i < 10; i++)
                    bytes[i] = (byte)i;

                using (var command = new NpgsqlCommand($"INSERT INTO {tableName} VALUES ($1)", c))
                {
                    command.Parameters.Add(new() { Value = bytes });
                    await command.ExecuteNonQueryAsync();
                }

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                await AssertTransactionStart(messages);
                await NextMessage<RelationMessage>(messages);
                var insertMsg = await NextMessage<InsertMessage>(messages);
                var columnEnumerator = insertMsg.NewRow.GetAsyncEnumerator();
                await columnEnumerator.MoveNextAsync();

                var stream = columnEnumerator.Current.GetStream();
                Assert.That(() => columnEnumerator.Current.GetStream(), Throws.Exception.TypeOf<InvalidOperationException>());
                Assert.That(() => columnEnumerator.Current.Get(), Throws.Exception.TypeOf<InvalidOperationException>());
                Assert.That(() => columnEnumerator.Current.Get<byte[]>(), Throws.Exception.TypeOf<InvalidOperationException>());

                if (IsBinary)
                {
                    var someBytes = new byte[10];
                    Assert.That(await stream.ReadAsync(someBytes, 0, 10), Is.EqualTo(10));
                    Assert.That(someBytes, Is.EquivalentTo(bytes[..10]));
                }
                else
                {
                    // We assume bytea hex format here
                    var hexString = "\\x" + BitConverter.ToString(bytes[..10]).Replace("-", string.Empty);
                    var expected = Encoding.ASCII.GetBytes(hexString);
                    var someBytes = new byte[expected.Length];
                    Assert.That(await stream.ReadAsync(someBytes, 0, someBytes.Length), Is.EqualTo(someBytes.Length));
                    Assert.That(someBytes, Is.EquivalentTo(expected));
                }

                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });
    }

    [Test]
    public Task TextReader()
    {
        // We don't test transaction streaming here because there's nothing special in that case
        if (IsStreaming)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NULL);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                var expectedText = "val1";
                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES (1, '{expectedText}')");

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                await AssertTransactionStart(messages);
                await NextMessage<RelationMessage>(messages);
                var insertMsg = await NextMessage<InsertMessage>(messages);
                var columnEnumerator = insertMsg.NewRow.GetAsyncEnumerator();
                await columnEnumerator.MoveNextAsync(); // We are not interested in the id field
                await columnEnumerator.MoveNextAsync();
                using var reader = columnEnumerator.Current.GetTextReader();
                Assert.That(await reader.ReadToEndAsync(), Is.EqualTo(expectedText));

                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });
    }

    [Test]
    public Task ValueMetadata()
    {
        // We don't test transaction streaming here because there's nothing special in that case
        if (IsStreaming)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (id INT PRIMARY KEY, name TEXT NULL);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES (1, 'val1')");

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                await AssertTransactionStart(messages);
                await NextMessage<RelationMessage>(messages);
                var insertMsg = await NextMessage<InsertMessage>(messages);
                var columnEnumerator = insertMsg.NewRow.GetAsyncEnumerator();
                await columnEnumerator.MoveNextAsync();

                Assert.That(columnEnumerator.Current.GetFieldType(), Is.SameAs(IsBinary ? typeof(int) : typeof(string)));
                Assert.That(columnEnumerator.Current.GetPostgresType().Name, Is.EqualTo("integer"));
                Assert.That(columnEnumerator.Current.GetDataTypeName(), Is.EqualTo("integer"));
                Assert.That(columnEnumerator.Current.IsUnchangedToastedValue, Is.False);

                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });
    }

    [Test]
    public Task Null()
    {
        // We don't test transaction streaming here because there's nothing special in that case
        if (IsStreaming)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (int1 INT, int2 INT);
                                                    CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES (1, 1), (NULL, NULL)");

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                await AssertTransactionStart(messages);
                await NextMessage<RelationMessage>(messages);

                // non-null
                var columnEnumerator = (await NextMessage<InsertMessage>(messages)).NewRow.GetAsyncEnumerator();
                await columnEnumerator.MoveNextAsync();
                Assert.That(columnEnumerator.Current.IsDBNull, Is.False);
                Assert.That(columnEnumerator.Current.IsUnchangedToastedValue, Is.False);
                if (IsBinary)
                    Assert.That(await columnEnumerator.Current.Get<int>(), Is.EqualTo(1));
                else
                    Assert.That(await columnEnumerator.Current.Get<string>(), Is.EqualTo("1"));
                await columnEnumerator.MoveNextAsync();
                Assert.That(await columnEnumerator.Current.Get(), Is.EqualTo(IsBinary ? 1 : "1"));

                // null
                columnEnumerator = (await NextMessage<InsertMessage>(messages)).NewRow.GetAsyncEnumerator();
                await columnEnumerator.MoveNextAsync();
                Assert.That(columnEnumerator.Current.IsDBNull, Is.True);
                Assert.That(columnEnumerator.Current.IsUnchangedToastedValue, Is.False);
                if (IsBinary)
                    Assert.That(() => columnEnumerator.Current.Get<int>(), Throws.Exception.TypeOf<InvalidCastException>());
                else
                    Assert.That(() => columnEnumerator.Current.Get<string>(), Throws.Exception.TypeOf<InvalidCastException>());
                await columnEnumerator.MoveNextAsync();
                Assert.That(await columnEnumerator.Current.Get(), Is.SameAs(DBNull.Value));

                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            });
    }

    [NpgsqlTypes.PgName("descriptor")]
    public class Descriptor
    {
        [NpgsqlTypes.PgName("id")]
        public long Id { get; set; }

        [NpgsqlTypes.PgName("name")]
        public string Name { get; set; } = string.Empty;
    }

#pragma warning disable CS0618 // GlobalTypeMapper is obsolete
    [Test, NonParallelizable]
    public Task CompositeType()
    {
        // We don't test transaction streaming here because there's nothing special in that case
        if (IsStreaming)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                await using var adminConnection = await OpenConnectionAsync();
                await adminConnection.ExecuteNonQueryAsync(@$"
DROP TYPE IF EXISTS descriptor CASCADE;
CREATE TYPE descriptor AS (id bigint, name text);
CREATE TABLE {tableName} (descriptor_field descriptor);
CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");

                NpgsqlConnection.GlobalTypeMapper.MapComposite<Descriptor>("descriptor");

                try
                {

                    // Use a one-time connection string to make sure we get a new data source without cached mappings.
                    // In regular tests we'd use a data source, but replication doesn't work with data sources (yet).
                    // In addition, clear the DatabaseInfo cache.
                    using var _ = CreateTempPool(ConnectionString, out var connString);
                    var rc = await OpenReplicationConnectionAsync(connString);
                    var slot = await rc.CreatePgOutputReplicationSlot(slotName);
                    var expected = new Descriptor { Id = 1248, Name = "My Descriptor" };
                    var stringValue = $"({expected.Id},\"{expected.Name}\")";

                    await using var c = await OpenConnectionAsync();
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} VALUES ('{stringValue}')");

                    using var streamingCts = new CancellationTokenSource();
                    var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                        .GetAsyncEnumerator();

                    await AssertTransactionStart(messages);
                    await NextMessage<TypeMessage>(messages);
                    await NextMessage<RelationMessage>(messages);

                    // non-null
                    var columnEnumerator = (await NextMessage<InsertMessage>(messages)).NewRow.GetAsyncEnumerator();
                    await columnEnumerator.MoveNextAsync();
                    Assert.That(columnEnumerator.Current.IsDBNull, Is.False);
                    Assert.That(columnEnumerator.Current.IsUnchangedToastedValue, Is.False);
                    if (IsBinary)
                    {
                        var result = await columnEnumerator.Current.Get<Descriptor>();
                        Assert.That(result.Id, Is.EqualTo(expected.Id));
                        Assert.That(result.Name, Is.EqualTo(expected.Name));
                    }
                    else
                        Assert.That(await columnEnumerator.Current.Get(), Is.EqualTo(stringValue));

                    await columnEnumerator.MoveNextAsync();

                    await AssertTransactionCommit(messages);

                    streamingCts.Cancel();
                    await AssertReplicationCancellation(messages);
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                }
                finally
                {
                    await adminConnection.ExecuteNonQueryAsync("DROP TYPE IF EXISTS descriptor CASCADE;");

                    NpgsqlConnection.GlobalTypeMapper.Reset();
                }
            });
    }
#pragma warning restore CS0618 // GlobalTypeMapper is obsolete

    [Test]
    public Task TwoPhase([Values]bool commit)
    {
        // Streaming of prepared transaction is only supported for
        // logical streaming replication protocol >= 3
        if (_protocolVersion < 3UL)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableName, publicationName) =>
            {
                var gid = Guid.NewGuid().ToString();
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"CREATE TABLE {tableName} (a int primary key, b varchar);
                                                CREATE PUBLICATION {publicationName} FOR TABLE {tableName};");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName, twoPhase: true);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"INSERT INTO {tableName} SELECT i, 'val' || i::text FROM generate_series(1, 15000) s(i);
	                                            PREPARE TRANSACTION '{gid}';");
                try
                {
                    using var streamingCts = new CancellationTokenSource();
                    var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                        .GetAsyncEnumerator();

                    // Begin Transaction
                    var transactionXid = await AssertTransactionStart(messages);

                    // Relation
                    await NextMessage<RelationMessage>(messages);

                    // Remaining inserts
                    for (var insertCount = 0; insertCount < 15000; insertCount++)
                    {
                        await NextMessage<InsertMessage>(messages);
                    }

                    var prepareMessageBase = await AssertPrepare(messages);
                    Assert.That(prepareMessageBase.TransactionXid, Is.EqualTo(transactionXid));
                    Assert.That(prepareMessageBase.TransactionGid, Is.EqualTo(gid));

                    if (commit)
                    {
                        await c.ExecuteNonQueryAsync(@$"COMMIT PREPARED '{gid}';");

                        var commitPreparedMessage = await NextMessage<CommitPreparedMessage>(messages);
                        Assert.That(commitPreparedMessage.TransactionXid, Is.EqualTo(transactionXid));
                        Assert.That(commitPreparedMessage.TransactionGid, Is.EqualTo(gid));
                    }
                    else
                    {
                        await c.ExecuteNonQueryAsync(@$"ROLLBACK PREPARED '{gid}';");

                        var rollbackPreparedMessage = await NextMessage<RollbackPreparedMessage>(messages);
                        Assert.That(rollbackPreparedMessage.TransactionXid, Is.EqualTo(transactionXid));
                        Assert.That(rollbackPreparedMessage.TransactionGid, Is.EqualTo(gid));
                    }

                    streamingCts.Cancel();
                    await AssertReplicationCancellation(messages);
                    await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
                }
                finally
                {
                    try
                    {
                        await using var cx = await OpenConnectionAsync();
                        await cx.ExecuteNonQueryAsync(@$"ROLLBACK PREPARED '{gid}';");
                    }
                    catch
                    {
                        // Give up
                    }
                }
            }, $"{GetObjectName(nameof(TwoPhase))}_{(commit ? "commit" : "rollback")}");
    }


    [Test(Description = "Tests whether columns of internally cached RelationMessage instances are accidentally overwritten.")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4633")]
    public Task Bug4633()
    {
        // We don't need all the various test cases here since the bug gets triggered in any case
        if (IsStreaming || IsBinary || Version > 1)
            return Task.CompletedTask;

        return SafePgOutputReplicationTest(
            async (slotName, tableNames, publicationName) =>
            {
                await using var c = await OpenConnectionAsync();
                await c.ExecuteNonQueryAsync(@$"
CREATE TABLE {tableNames[0]}
(
    id uuid NOT NULL,
    text text NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_{tableNames[0]} PRIMARY KEY (id)
);
CREATE TABLE {tableNames[1]}
(
    id uuid NOT NULL,
    message_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT pk_{tableNames[1]} PRIMARY KEY (id),
    CONSTRAINT fk_{tableNames[1]}_message_id FOREIGN KEY (message_id) REFERENCES {tableNames[0]} (id)
);
CREATE PUBLICATION {publicationName} FOR TABLE {tableNames[0]}, {tableNames[1]} WITH (PUBLISH = 'insert');");
                await using var rc = await OpenReplicationConnectionAsync();
                var slot = await rc.CreatePgOutputReplicationSlot(slotName);

                await using var tran = await c.BeginTransactionAsync();
                await c.ExecuteNonQueryAsync(@$"
INSERT INTO {tableNames[0]} VALUES ('B6CB5293-F65E-4F48-A74B-06D5355DAA74', 'random', now());
INSERT INTO {tableNames[1]} VALUES ('55870BEC-C42E-4AB0-83BA-225BB7777B37', 'B6CB5293-F65E-4F48-A74B-06D5355DAA74', now());
INSERT INTO {tableNames[0]} VALUES ('5F89F5FE-6F4F-465F-BB87-716B1413F88D', 'another random', now());");
                await tran.CommitAsync();

                using var streamingCts = new CancellationTokenSource();
                var messages = SkipEmptyTransactions(rc.StartReplication(slot, GetOptions(publicationName), streamingCts.Token))
                    .GetAsyncEnumerator();

                // Begin Transaction
                var transactionXid = await AssertTransactionStart(messages);

                // First Relation
                var relationMsg = await NextMessage<RelationMessage>(messages);
                var relation1Name = relationMsg.RelationName;
                var relation1Id = relationMsg.RelationId;
                Assert.That(relation1Name, Is.EqualTo(tableNames[0]));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(3));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("text"));
                Assert.That(relationMsg.Columns[2].ColumnName, Is.EqualTo("created_at"));

                // Insert first value
                var insertMsg = await NextMessage<InsertMessage>(messages);
                Assert.That(insertMsg.Relation.RelationName, Is.EqualTo(relation1Name));
                Assert.That(insertMsg.Relation.RelationId, Is.EqualTo(relation1Id));
                Assert.That(insertMsg.Relation.Columns.Count, Is.EqualTo(3));
                Assert.That(insertMsg.Relation.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(insertMsg.Relation.Columns[1].ColumnName, Is.EqualTo("text"));
                Assert.That(insertMsg.Relation.Columns[2].ColumnName, Is.EqualTo("created_at"));

                // Second Relation
                relationMsg = await NextMessage<RelationMessage>(messages);
                var relation2Name = relationMsg.RelationName;
                var relation2Id = relationMsg.RelationId;
                Assert.That(relation2Name, Is.EqualTo(tableNames[1]));
                Assert.That(relationMsg.Columns.Count, Is.EqualTo(3));
                Assert.That(relationMsg.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(relationMsg.Columns[1].ColumnName, Is.EqualTo("message_id"));
                Assert.That(relationMsg.Columns[2].ColumnName, Is.EqualTo("created_at"));

                // Insert second value
                insertMsg = await NextMessage<InsertMessage>(messages);
                Assert.That(insertMsg.Relation.RelationName, Is.EqualTo(relation2Name));
                Assert.That(insertMsg.Relation.RelationId, Is.EqualTo(relation2Id));
                Assert.That(insertMsg.Relation.Columns.Count, Is.EqualTo(3));
                Assert.That(insertMsg.Relation.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(insertMsg.Relation.Columns[1].ColumnName, Is.EqualTo("message_id"));
                Assert.That(insertMsg.Relation.Columns[2].ColumnName, Is.EqualTo("created_at"));

                // Insert third value
                insertMsg = await NextMessage<InsertMessage>(messages);
                Assert.That(insertMsg.Relation.RelationName, Is.EqualTo(relation1Name));
                Assert.That(insertMsg.Relation.RelationId, Is.EqualTo(relation1Id));
                Assert.That(insertMsg.Relation.Columns.Count, Is.EqualTo(3));
                Assert.That(insertMsg.Relation.Columns[0].ColumnName, Is.EqualTo("id"));
                Assert.That(insertMsg.Relation.Columns[1].ColumnName, Is.EqualTo("text"));
                Assert.That(insertMsg.Relation.Columns[2].ColumnName, Is.EqualTo("created_at"));

                // Commit Transaction
                await AssertTransactionCommit(messages);

                streamingCts.Cancel();
                await AssertReplicationCancellation(messages);
                await rc.DropReplicationSlot(slotName, cancellationToken: CancellationToken.None);
            }, 2);
    }

    #region Non-Test stuff (helper methods, initialization, enums, ...)

    async Task<uint?> AssertTransactionStart(IAsyncEnumerator<PgOutputReplicationMessage> messages)
    {
        Assert.True(await messages.MoveNextAsync());

        switch (messages.Current)
        {
        case StreamStartMessage streamStartMessage:
            Assert.That(IsStreaming);
            return streamStartMessage.TransactionXid;
        case BeginMessage beginMessage:
            Assert.That(!IsStreaming);
            return beginMessage.TransactionXid;
        case BeginPrepareMessage beginPrepareMessage:
            Assert.That(!IsStreaming);
            return beginPrepareMessage.TransactionXid;
        default:
            Assert.Fail("Expected transaction start message but got: " + messages.Current);
            throw new Exception();
        }
    }

    async Task AssertTransactionCommit(IAsyncEnumerator<PgOutputReplicationMessage> messages)
    {
        Assert.True(await messages.MoveNextAsync());

        switch (messages.Current)
        {
        case StreamStopMessage:
            Assert.That(IsStreaming);
            Assert.True(await messages.MoveNextAsync());
            Assert.That(messages.Current, Is.TypeOf<StreamCommitMessage>());
            return;
        case CommitMessage:
            return;
        default:
            Assert.Fail("Expected transaction end message but got: " + messages.Current);
            throw new Exception();
        }
    }

    async Task<PrepareMessageBase> AssertPrepare(IAsyncEnumerator<PgOutputReplicationMessage> enumerator)
    {
        Assert.True(await enumerator.MoveNextAsync());
        if (IsStreaming && enumerator.Current is StreamStopMessage)
        {
            Assert.True(await enumerator.MoveNextAsync());
            Assert.That(enumerator.Current, Is.TypeOf<StreamPrepareMessage>());
            return (PrepareMessageBase)enumerator.Current!;
        }

        Assert.That(enumerator.Current, Is.TypeOf<PrepareMessage>());
        return (PrepareMessageBase)enumerator.Current!;
    }

    async ValueTask<TExpected> NextMessage<TExpected>(IAsyncEnumerator<PgOutputReplicationMessage> enumerator, bool expectRelationMessage = false)
        where TExpected : PgOutputReplicationMessage
    {
        Assert.True(await enumerator.MoveNextAsync());
        if (IsStreaming && enumerator.Current is StreamStopMessage)
        {
            Assert.True(await enumerator.MoveNextAsync());
            Assert.That(enumerator.Current, Is.TypeOf<StreamStartMessage>());
            Assert.True(await enumerator.MoveNextAsync());
            if (expectRelationMessage)
            {
                Assert.That(enumerator.Current, Is.TypeOf<RelationMessage>());
                Assert.True(await enumerator.MoveNextAsync());
            }
        }

        Assert.That(enumerator.Current, Is.TypeOf<TExpected>());
        return (TExpected)enumerator.Current!;
    }

    /// <summary>
    /// Unfortunately, empty transactions may get randomly created by PG because of auto-vacuuming; these cause test failures as we
    /// assert for specific expected message types. This filters them out.
    /// </summary>
    async IAsyncEnumerable<PgOutputReplicationMessage> SkipEmptyTransactions(IAsyncEnumerable<PgOutputReplicationMessage> messages)
    {
        var enumerator = messages.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync())
        {
            if (enumerator.Current is BeginMessage)
            {
                var current = enumerator.Current;
                if (!await enumerator.MoveNextAsync())
                {
                    yield return current;
                    yield break;
                }

                var next = enumerator.Current;
                if (next is CommitMessage)
                    continue;

                yield return current;
                yield return next;
                continue;
            }

            yield return enumerator.Current;
        }
    }

    PgOutputReplicationOptions GetOptions(string publicationName, bool? messages = null)
        => new(publicationName, _protocolVersion, _binary, _streaming, messages);

    Task SafePgOutputReplicationTest(Func<string, string, string, Task> testAction, [CallerMemberName] string memberName = "")
        => SafeReplicationTest(testAction, GetObjectName(memberName));

    Task SafePgOutputReplicationTest(Func<string, string[], string, Task> testAction, int tableCount, [CallerMemberName] string memberName = "")
        => SafeReplicationTest(testAction, tableCount, GetObjectName(memberName));

    string GetObjectName(string memberName)
    {
        var sb = new StringBuilder(memberName)
            .Append("_v").Append(_protocolVersion);
        if (_binary.HasValue)
            sb.Append("_b_").Append(BoolToChar(_binary.Value));
        if (_streaming.HasValue)
            sb.Append("_s_").Append(BoolToChar(_streaming.Value));
        return sb.ToString();
    }

    static char BoolToChar(bool value)
        => value ? 't' : 'f';


    protected override string Postfix => "pgoutput_l";

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var c = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(c, "10.0", "The Logical Replication Protocol (via pgoutput plugin) was introduced in PostgreSQL 10");
        if (_protocolVersion > 2)
            TestUtil.MinimumPgVersion(c, "15.0", "Logical Streaming Replication Protocol version 3 was introduced in PostgreSQL 15");
        if (_protocolVersion > 1)
            TestUtil.MinimumPgVersion(c, "14.0", "Logical Streaming Replication Protocol version 2 was introduced in PostgreSQL 14");
        if (IsBinary)
            TestUtil.MinimumPgVersion(c, "14.0", "Sending replication values in binary representation was introduced in PostgreSQL 14");
        if (IsStreaming)
        {
            TestUtil.MinimumPgVersion(c, "14.0", "Streaming of in-progress transactions was introduced in PostgreSQL 14");
            var logicalDecodingWorkMem = (string)(await c.ExecuteScalarAsync("SHOW logical_decoding_work_mem"))!;
            if (logicalDecodingWorkMem != "64kB")
            {
                TestUtil.IgnoreExceptOnBuildServer(
                    $"logical_decoding_work_mem is set to '{logicalDecodingWorkMem}', but must be set to '64kB' in order for the " +
                    "streaming replication tests to work correctly. Skipping replication tests");
            }
        }
    }

    public enum ProtocolVersion : ulong
    {
        V1 = 1UL,
        V2 = 2UL,
        V3 = 3UL,
    }
    public enum ReplicationDataMode
    {
        DefaultReplicationDataMode,
        TextReplicationDataMode,
        BinaryReplicationDataMode,
    }
    public enum TransactionMode
    {
        DefaultTransactionMode,
        NonStreamingTransactionMode,
        StreamingTransactionMode,
    }

    #endregion Non-Test stuff (helper methods, initialization, ennums, ...)
}
