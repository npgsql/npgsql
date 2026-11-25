using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;

namespace Npgsql.Tests.Replication;

public abstract class SafeReplicationTestBase<TConnection> : TestBase
    where TConnection : ReplicationConnection, new()
{
    protected abstract string Postfix { get; }

    int _maxIdentifierLength;
    static Version CurrentServerVersion = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await using var conn = await OpenConnectionAsync();
        CurrentServerVersion = conn.PostgreSqlVersion;
        _maxIdentifierLength = int.Parse((string)(await conn.ExecuteScalarAsync("SHOW max_identifier_length"))!);

        // Creating a logical slot waits for every transaction in the cluster that holds an xid. So if a killed test left a
        // prepared transaction behind, every test that creates a logical slot before the owning test would wait forever.
        // Of the replication tests, only PgOutputReplicationTests prepares transactions, and it names them like its slots (see
        // Cleanup below), so every logical fixture rolls those back first.
        if (typeof(TConnection) == typeof(LogicalReplicationConnection))
        {
            foreach (var gid in (string[])(await conn.ExecuteScalarAsync(
                         "SELECT coalesce(array_agg(gid), '{}') FROM pg_prepared_xacts " +
                         "WHERE gid ~ '^s_[a-z0-9_]+_pgoutput_l$' AND database = current_database()"))!)
            {
                await conn.ExecuteNonQueryAsync($"ROLLBACK PREPARED '{gid}'");
            }
        }
    }

    [SetUp]
    public async Task Setup()
    {
        await using var conn = await OpenConnectionAsync();
        var walLevel = (string)(await conn.ExecuteScalarAsync("SHOW wal_level"))!;
        if (walLevel != "logical")
            TestUtil.IgnoreExceptOnBuildServer("wal_level needs to be set to 'logical' in the PostgreSQL conf");

        var maxWalSenders = int.Parse((string)(await conn.ExecuteScalarAsync("SHOW max_wal_senders"))!);
        if (maxWalSenders < 50)
        {
            TestUtil.IgnoreExceptOnBuildServer(
                $"max_wal_senders is too low ({maxWalSenders}) and could lead to transient failures. Skipping replication tests");
        }
    }

    private protected Task<TConnection> OpenReplicationConnectionAsync(
        NpgsqlConnectionStringBuilder csb,
        CancellationToken cancellationToken = default)
        => OpenReplicationConnectionAsync(csb.ToString(), cancellationToken);

    private protected async Task<TConnection> OpenReplicationConnectionAsync(
        string? connectionString = null,
        CancellationToken cancellationToken = default)
    {
        var c = new TConnection { ConnectionString = connectionString ?? ConnectionString };
        await c.Open(cancellationToken);
        return c;
    }

    private protected static async Task AssertReplicationCancellation<T>(IAsyncEnumerator<T> enumerator, bool streamingStarted = true)
    {
        try
        {
            var succeeded = await enumerator.MoveNextAsync();
            Assert.Fail(succeeded
                ? $"Expected replication cancellation but got message: {enumerator.Current}"
                : "Expected replication cancellation but reached enumeration end instead");
        }
        catch (Exception e)
        {
            Assert.That(e, streamingStarted && CurrentServerVersion >= Pg10Version
                ? Is.AssignableTo<OperationCanceledException>()
                    .With.InnerException.InstanceOf<PostgresException>()
                    .And.InnerException.Property(nameof(PostgresException.SqlState))
                    .EqualTo(PostgresErrorCodes.QueryCanceled)
                : Is.AssignableTo<OperationCanceledException>()
                    .With.InnerException.Null);
        }
    }

    private protected Task SafeReplicationTest(Func<string, string, Task> testAction, [CallerMemberName] string memberName = "")
        => SafeReplicationTestCore((slotName, tableNames, publicationName) => testAction(slotName, tableNames[0]), 1, memberName);

    private protected Task SafeReplicationTest(Func<string, string, string, Task> testAction, [CallerMemberName] string memberName = "")
        => SafeReplicationTestCore((slotName, tableNames, publicationName) => testAction(slotName, tableNames[0], publicationName), 1, memberName);

    private protected Task SafeReplicationTest(Func<string, string[], string, Task> testAction, int tableCount, [CallerMemberName] string memberName = "")
        => SafeReplicationTestCore(testAction, tableCount, memberName);

    static readonly Version Pg10Version = new(10, 0);

    async Task SafeReplicationTestCore(Func<string, string[], string, Task> testAction, int tableCount, string memberName)
    {
        // If the supplied name is too long, we derive one from its hash, so that the cleanup of the next run still finds what a
        // killed run left behind
        var baseName = $"{memberName}_{Postfix}";
        var name = (baseName.Length > _maxIdentifierLength - 4
            ? Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(baseName)), 0, 16)
            : baseName).ToLowerInvariant();
        var slotName = $"s_{name}".ToLowerInvariant();
        var tableNames = new string[tableCount];
        for (var i = tableNames.Length - 1; i >= 0; i--)
        {
            tableNames[i] = $"t{(tableCount == 1 ? "" : i.ToString())}_{name}".ToLowerInvariant();
        }
        var publicationName = $"p_{name}".ToLowerInvariant();

        await Cleanup();

        try
        {
            await testAction(slotName, tableNames, publicationName);
        }
        finally
        {
            await Cleanup();
        }

        async Task Cleanup()
        {
            await using var c = await OpenConnectionAsync();

            // Tests that prepare a transaction name it like their slot. If a killed test left it behind, its locks would make
            // dropping the tables wait forever, and new logical slots would wait for it to end.
            if ((long)(await c.ExecuteScalarAsync(
                    $"SELECT count(*) FROM pg_prepared_xacts WHERE gid = '{slotName}' AND database = current_database()"))! > 0)
            {
                await c.ExecuteNonQueryAsync($"ROLLBACK PREPARED '{slotName}'");
            }
            try
            {
                await DropSlot();
            }
            catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.ObjectInUse && e.Message.Contains(slotName))
            {
                // The slot is still in use. Probably because we didn't terminate
                // the streaming replication properly.
                // The following is ugly, but let's try to clean up after us if we can.
                var pid = Regex.Match(e.MessageText, "PID (?<pid>\\d+)", RegexOptions.IgnoreCase).Groups["pid"];
                if (pid.Success)
                {
                    await c.ExecuteNonQueryAsync($"SELECT pg_terminate_backend ({pid.Value})");
                    for (var i = 0; (bool)(await c.ExecuteScalarAsync($"SELECT EXISTS(SELECT * FROM pg_stat_replication where pid = {pid.Value})"))! && i < 20; i++)
                        await Task.Delay(TimeSpan.FromSeconds(1));
                }
                else
                {
                    // Old backends don't report the PID
                    for (var i = 0; (bool)(await c.ExecuteScalarAsync("SELECT EXISTS(SELECT * FROM pg_stat_replication)"))! && i < 20; i++)
                        await Task.Delay(TimeSpan.FromSeconds(1));
                }

                try
                {
                    await DropSlot();
                }
                catch (PostgresException e2) when (e2.SqlState == PostgresErrorCodes.ObjectInUse && e2.Message.Contains(slotName))
                {
                    // We failed to drop the slot, even after 20 seconds. Swallow the exception to avoid failing the test, we'll
                    // likely drop it the next time the test is executed (Cleanup is executed before starting the test as well).

                    return;
                }
            }

            if (c.PostgreSqlVersion >= Pg10Version)
                await c.ExecuteNonQueryAsync($"DROP PUBLICATION IF EXISTS {publicationName}");

            for (var i = tableNames.Length - 1; i >= 0; i--)
                await c.ExecuteNonQueryAsync($"DROP TABLE IF EXISTS {tableNames[i]} CASCADE;");

            async Task DropSlot()
            {
                try
                {
                    await c.ExecuteNonQueryAsync($"SELECT pg_drop_replication_slot('{slotName}')");
                }
                catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UndefinedObject && ex.Message.Contains(slotName))
                {
                    // Temporary slots might already have been deleted
                    // We don't care as long as it's gone and we don't have to clean it up
                }
            }
        }
    }

    private protected static CancellationToken GetCancelledCancellationToken() => new(canceled: true);
}
