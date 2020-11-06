using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;

namespace Npgsql.Tests.Replication
{
    public abstract class SafeReplicationTestBase<TConnection> : TestBase
        where TConnection : ReplicationConnection, new()
    {
        protected abstract string Postfix { get; }

        int _maxIdentifierLength;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await using var conn = await OpenConnectionAsync();
            _maxIdentifierLength = int.Parse((string)(await conn.ExecuteScalarAsync("SHOW max_identifier_length"))!);
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
                TestUtil.IgnoreExceptOnBuildServer(
                    $"max_wal_senders is too low ({maxWalSenders}) and could lead to transient failures. Skipping replication tests");
        }

        private protected async Task<TConnection> OpenReplicationConnectionAsync(NpgsqlConnectionStringBuilder? csb = null, CancellationToken cancellationToken = default)
        {
            var c = new TConnection { ConnectionString = csb?.ToString() ?? ConnectionString };
            await c.OpenAsync(cancellationToken);
            return c;
        }

        private protected Task SafeReplicationTest(Func<string, string, Task> testAction, [CallerMemberName] string memberName = "")
            => SafeReplicationTestCore((slotName, tableName, publicationName) => testAction(slotName, tableName), memberName);

        private protected Task SafeReplicationTest(Func<string, string, string, Task> testAction, [CallerMemberName] string memberName = "")
            => SafeReplicationTestCore(testAction, memberName);

        async Task SafeReplicationTestCore(Func<string, string, string, Task> testAction, string memberName)
        {
            // if the supplied name is too long we create on from a guid.
            var baseName = $"{memberName}_{Postfix}";
            var name = (baseName.Length > _maxIdentifierLength - 4 ? Guid.NewGuid().ToString("N") : baseName).ToLowerInvariant();
            var slotName = $"s_{name}".ToLowerInvariant();
            var tableName = $"t_{name}".ToLowerInvariant();
            var publicationName = $"p_{name}".ToLowerInvariant();
            try
            {
                await testAction(slotName, tableName, publicationName);
            }
            finally
            {
                await using var c = await OpenConnectionAsync();
                try
                {
                    await DropSlot();
                }
                catch (PostgresException e) when (e.SqlState == "55006" && e.Message.Contains(slotName))
                {
                    // The slot is still in use. Probably because we didn't terminate
                    // the streaming replication properly.
                    // The following is ugly, but let's try to clean up after us if we can.
                    var pid = Regex.Match(e.MessageText, "PID (?<pid>\\d+)", RegexOptions.IgnoreCase).Groups["pid"];
                    if (pid.Success)
                    {
                        await c.ExecuteNonQueryAsync($"SELECT pg_terminate_backend ({pid.Value})");
                    }
                    // Old backends don't report the PID
                    for (var i = 0; (bool)(await c.ExecuteScalarAsync("SELECT EXISTS(SELECT * FROM pg_stat_replication)"))! && i < 30; i++)
                        await Task.Delay(TimeSpan.FromSeconds(1));

                    await DropSlot();
                }

                await c.ExecuteNonQueryAsync($"DROP PUBLICATION IF EXISTS {publicationName}");
                await c.ExecuteNonQueryAsync($"DROP TABLE IF EXISTS {tableName}");

                async Task DropSlot()
                {
                    try
                    {
                        await c.ExecuteNonQueryAsync($"SELECT pg_drop_replication_slot('{slotName}')");
                    }
                    catch (PostgresException ex) when (ex.SqlState == "42704" && ex.Message.Contains(slotName))
                    {
                        // Temporary slots might already have been deleted
                        // We don't care as long as it's gone and we don't have to clean it up
                    }
                }
            }
        }

        private protected static CancellationTokenSource GetCancelledCancellationTokenSource()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            return cts;
        }
    }
}
