using System;
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;
using Npgsql.Replication.Internal;
using NpgsqlTypes;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Tests for common logical replication functionality.
/// </summary>
/// <remarks>
/// While these tests might seem superfluous since we perform similar tests
/// for the individual logical replication tests, they are in fact not, because
/// the methods they test are extension points for plugin developers.
/// </remarks>
[Platform(Exclude = "MacOsX", Reason = "Replication tests are flaky in CI on Mac")]
[NonParallelizable]
public class CommonLogicalReplicationTests : SafeReplicationTestBase<LogicalReplicationConnection>
{
    // We use the test_decoding logical decoding plugin for the common
    // logical replication tests because it has existed since the
    // beginning of logical decoding and by that has the best backwards
    // compatibility.
    const string OutputPlugin = "test_decoding";

    [Test]
    public Task CreateLogicalReplicationSlot([Values]bool temporary, [Values]bool twoPhase)
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                if (twoPhase)
                    TestUtil.MinimumPgVersion(c, "15.0", "Replication slots with two phase commit support were introduced in PostgreSQL 15");
                if (temporary)
                    TestUtil.MinimumPgVersion(c, "10.0", "Temporary replication slots were introduced in PostgreSQL 10");

                await using var rc = await OpenReplicationConnectionAsync();
                var options = await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, temporary, twoPhase: twoPhase);

                using var cmd =
                    new NpgsqlCommandOrig($"SELECT * FROM pg_replication_slots WHERE slot_name = '{options.SlotName}'",
                        c);
                await using var reader = await cmd.ExecuteReaderAsync();

                Assert.That(reader.Read, Is.True);
                Assert.That(reader.GetFieldValue<string>(reader.GetOrdinal("slot_type")), Is.EqualTo("logical"));
                if (c.PostgreSqlVersion >= Version.Parse("15.0"))
                    Assert.That(reader.GetFieldValue<bool>(reader.GetOrdinal("two_phase")), Is.EqualTo(twoPhase));
                if (c.PostgreSqlVersion >= Version.Parse("10.0"))
                    Assert.That(reader.GetFieldValue<bool>(reader.GetOrdinal("temporary")), Is.EqualTo(temporary));
                Assert.That(reader.GetFieldValue<bool>(reader.GetOrdinal("active")), Is.EqualTo(temporary));
                if (c.PostgreSqlVersion >= Version.Parse("9.6"))
                    Assert.That(reader.GetFieldValue<NpgsqlLogSequenceNumber>(reader.GetOrdinal("confirmed_flush_lsn")),
                        Is.EqualTo(options.ConsistentPoint));
                Assert.That(reader.Read, Is.False);
            }, nameof(CreateLogicalReplicationSlot) + (temporary ? "_tmp" : "") + (twoPhase ? "_tp" : ""));

    [Test]
    public Task CreateLogicalReplicationSlot_NoExport([Values]bool temporary, [Values]bool twoPhase)
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                if (temporary)
                    TestUtil.MinimumPgVersion(c, "10.0", "Temporary replication slots were introduced in PostgreSQL 10");
                if (twoPhase)
                    TestUtil.MinimumPgVersion(c, "15.0", "Replication slots with two phase commit support were introduced in PostgreSQL 15");

                TestUtil.MinimumPgVersion(c, "10.0", "The *_SNAPSHOT syntax was introduced in PostgreSQL 10");
                await using var rc = await OpenReplicationConnectionAsync();
                var options = await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, temporary, LogicalSlotSnapshotInitMode.NoExport, twoPhase);
                Assert.That(options.SnapshotName, Is.Null);
            }, nameof(CreateLogicalReplicationSlot_NoExport) + (temporary ? "_tmp" : "") + (twoPhase ? "_tp" : ""));

    [Test(Description = "Tests whether we throw a helpful exception about the unsupported *_SNAPSHOT syntax on old servers.")]
    [TestCase(LogicalSlotSnapshotInitMode.Export)]
    [TestCase(LogicalSlotSnapshotInitMode.NoExport)]
    [TestCase(LogicalSlotSnapshotInitMode.Use)]
    public Task CreateLogicalReplicationSlot_with_SnapshotInitMode_on_old_postgres_throws(LogicalSlotSnapshotInitMode mode)
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                TestUtil.MaximumPgVersionExclusive(c, "10.0", "The *_SNAPSHOT syntax was introduced in PostgreSQL 10");
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, slotSnapshotInitMode: mode);
                }, Throws.InstanceOf<NotSupportedException>()
                    .With.Message.StartsWith("The EXPORT_SNAPSHOT, USE_SNAPSHOT and NOEXPORT_SNAPSHOT syntax was introduced in PostgreSQL")
                    .And.InnerException.TypeOf<PostgresException>()
                    .And.InnerException.Property("SqlState").EqualTo(PostgresErrorCodes.SyntaxError));
            });

    [Test(Description = "Tests whether we throw a helpful exception about unsupported temporary replication slots on old servers.")]
    public Task CreateLogicalReplicationSlot_with_isTemporary_set_to_true_on_old_postgres_throws()
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                TestUtil.MaximumPgVersionExclusive(c, "10.0", "Temporary replication slots were introduced in PostgreSQL 10");
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, isTemporary: true);
                }, Throws.InstanceOf<NotSupportedException>()
                    .With.Message.StartsWith("Temporary replication slots were introduced in PostgreSQL")
                    .And.InnerException.TypeOf<PostgresException>()
                    .And.InnerException.Property("SqlState").EqualTo(PostgresErrorCodes.SyntaxError));
            });

    [Test(Description = "Tests whether we throw a helpful exception about the unsupported TWO_PHASE syntax on old servers.")]
    public Task CreateLogicalReplicationSlot_with_twoPhase_set_to_true_on_old_postgres_throws()
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                TestUtil.MaximumPgVersionExclusive(c, "15.0",
                    "Logical replication support for prepared transactions was  introduced in PostgreSQL 15");
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, twoPhase: true);
                }, Throws.InstanceOf<NotSupportedException>()
                    .With.Message.StartsWith("Logical replication support for prepared transactions was introduced in PostgreSQL")
                    .And.InnerException.TypeOf<PostgresException>()
                    .And.InnerException.Property("SqlState").EqualTo(PostgresErrorCodes.SyntaxError));
            });

    [Test(Description = "We can use the exported snapshot to query the database in the very moment the replication slot was created.")]
    public Task CreateLogicalReplicationSlot_Export([Values]bool temporary, [Values]bool twoPhase, [Values]bool implicitInitMode)
        => SafeReplicationTest(
            async (slotName, tableName) =>
            {
                await using var c = await OpenConnectionAsync();
                if (temporary)
                    TestUtil.MinimumPgVersion(c, "10.0", "Temporary replication slots were introduced in PostgreSQL 10");
                if (twoPhase)
                    TestUtil.MinimumPgVersion(c, "15.0", "Replication slots with two phase commit support were introduced in PostgreSQL 15");
                if (!implicitInitMode)
                    TestUtil.MinimumPgVersion(c, "10.0", "The *_SNAPSHOT syntax was introduced in PostgreSQL 10");
                await using (var transaction = c.BeginTransaction())
                {
                    await c.ExecuteNonQueryAsync($"CREATE TABLE {tableName} (value text)");
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} (value) VALUES('Before snapshot')");
                    transaction.Commit();
                }
                await using var rc = await OpenReplicationConnectionAsync();
                var options = await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, temporary, implicitInitMode ? null : LogicalSlotSnapshotInitMode.Export, twoPhase);
                await using (var transaction = c.BeginTransaction())
                {
                    await c.ExecuteNonQueryAsync($"INSERT INTO {tableName} (value) VALUES('After snapshot')");
                    transaction.Commit();
                }
                await using (var transaction = c.BeginTransaction(IsolationLevel.RepeatableRead))
                {
                    await c.ExecuteScalarAsync($"SET TRANSACTION SNAPSHOT '{options.SnapshotName}';", transaction);
                    using var cmd = new NpgsqlCommandOrig($"SELECT value FROM {tableName}", c, transaction);
                    await using var reader = await cmd.ExecuteReaderAsync();
                    Assert.That(reader.Read, Is.True);
                    Assert.That(reader.GetFieldValue<string>(0), Is.EqualTo("Before snapshot"));
                    Assert.That(reader.Read, Is.False);
                }
            }, nameof(CreateLogicalReplicationSlot_Export) + (temporary ? "_tmp" : "") + (twoPhase ? "_tp" : "") + (implicitInitMode ? "_i" : ""));

    [Test(Description = "Since we currently don't provide an API to start a transaction on a logical replication connection, " +
                        "USE_SNAPSHOT currently doesn't work and always leads to an exception. On the other hand, starting" +
                        "a transaction would only be useful if we'd also provide an API to issue commands.")]
    public Task CreateLogicalReplicationSlot_Use([Values]bool temporary, [Values]bool twoPhase)
        => SafeReplicationTest(
            async (slotName, _) =>
            {
                await using var c = await OpenConnectionAsync();
                if (temporary)
                    TestUtil.MinimumPgVersion(c, "10.0", "Temporary replication slots were introduced in PostgreSQL 10");
                if (twoPhase)
                    TestUtil.MinimumPgVersion(c, "15.0", "Replication slots with two phase commit support were introduced in PostgreSQL 15");

                TestUtil.MinimumPgVersion(c, "10.0", "The *_SNAPSHOT syntax was introduced in PostgreSQL 10");
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, temporary, LogicalSlotSnapshotInitMode.Use, twoPhase);
                }, Throws.InstanceOf<PostgresException>()
                    .With.Property("SqlState")
                    .EqualTo("XX000")
                    .And.Message.Contains(
                        c.PostgreSqlVersion.Major < 15
                            ? "USE_SNAPSHOT"
                            : "(SNAPSHOT 'use')"
                        ));
            }, nameof(CreateLogicalReplicationSlot_Use) + (temporary ? "_tmp" : "") + (twoPhase ? "_tp" : ""));

    [Test]
    public void CreateLogicalReplicationSlot_with_null_slot_throws()
        => Assert.That(async () =>
        {
            await using var rc = await OpenReplicationConnectionAsync();
            await rc.CreateLogicalReplicationSlot(null!, OutputPlugin);
        }, Throws.ArgumentNullException
            .With.Property("ParamName")
            .EqualTo("slotName"));

    [Test]
    public Task CreateLogicalReplicationSlot_with_null_output_plugin_throws()
        => SafeReplicationTest(
            (slotName, _) =>
            {
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, null!);
                }, Throws.ArgumentNullException
                    .With.Property("ParamName")
                    .EqualTo("outputPlugin"));
                return Task.CompletedTask;
            });

    [Test]
    public Task CreateLogicalReplicationSlot_with_cancelled_token()
        => SafeReplicationTest(
            (slotName, _) =>
            {
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    var token = GetCancelledCancellationToken();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, cancellationToken: token);
                }, Throws.Exception.AssignableTo<OperationCanceledException>());
                return Task.CompletedTask;
            });

    [Test]
    public Task CreateLogicalReplicationSlot_with_invalid_SnapshotInitMode_throws()
        => SafeReplicationTest(
            (slotName, _) =>
            {
                Assert.That(async () =>
                {
                    await using var rc = await OpenReplicationConnectionAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin, slotSnapshotInitMode: (LogicalSlotSnapshotInitMode)42);
                }, Throws.InstanceOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName")
                    .EqualTo("slotSnapshotInitMode")
                    .And.Property("ActualValue")
                    .EqualTo((LogicalSlotSnapshotInitMode)42));
                return Task.CompletedTask;
            });

    [Test]
    public Task CreateLogicalReplicationSlot_with_disposed_connection_throws()
        => SafeReplicationTest(
            (slotName, _) =>
            {
                Assert.That(async () =>
                {
                    var rc = await OpenReplicationConnectionAsync();
                    await rc.DisposeAsync();
                    await rc.CreateLogicalReplicationSlot(slotName, OutputPlugin);
                }, Throws.InstanceOf<ObjectDisposedException>()
                    .With.Property(nameof(ObjectDisposedException.ObjectName))
                    .EqualTo(nameof(LogicalReplicationConnection)));
                return Task.CompletedTask;
            });

    protected override string Postfix => "commonl_l";
}
