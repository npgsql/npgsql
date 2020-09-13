using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Replication.Logical;
using Npgsql.Replication.Physical;
using NUnit.Framework;

namespace Npgsql.Tests.Replication
{
    public class SafeReplicationTestBase<TConnection> : TestBase
        where TConnection : NpgsqlReplicationConnection, new()
    {
        readonly string _postfix = new TConnection() switch {
            NpgsqlLogicalReplicationConnection _ => "_l",
            NpgsqlPhysicalReplicationConnection _ => "_p",
            _ => throw new ArgumentOutOfRangeException($"{typeof(TConnection)} is not expected.")
        };
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
        }

        private protected async Task<TConnection> OpenReplicationConnectionAsync(NpgsqlConnectionStringBuilder? csb = null, CancellationToken cancellationToken = default)
        {
            var c = new TConnection { ConnectionString = csb?.ToString() ?? ConnectionString };
            await c.OpenAsync(cancellationToken);
            return c;
        }

        private protected Task SafeReplicationTest(string baseName, Func<string, string, Task> testAction) =>
            SafeReplicationTest(baseName, (slotName, tableName, publicationName) => testAction(slotName, tableName));

        private protected async Task SafeReplicationTest(string baseName, Func<string, string, string, Task> testAction)
        {
            // if the supplied name is too long we create on from a guid.
            var name = (baseName.Length > _maxIdentifierLength - 4 ? Guid.NewGuid().ToString("N") : baseName).ToLowerInvariant();
            var slotName = $"s_{name}{_postfix}".ToLowerInvariant();
            var tableName = $"t_{name}{_postfix}".ToLowerInvariant();
            var publicationName = $"p_{name}{_postfix}".ToLowerInvariant();
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
                        // We don't care as log as it's gone and we don't have to clean it up
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

        private protected static async Task<T> DequeueMessage<T>(ConcurrentQueue<T> queue, TimeSpan? delay = null, TimeSpan? timeout = null)
        {
            var effectiveDelay = delay ?? TimeSpan.FromMilliseconds(10);
            var effectiveTimeout = timeout ?? TimeSpan.FromSeconds(5);
            Debug.Assert(effectiveDelay < effectiveTimeout);
            var iterations = effectiveTimeout.Ticks / effectiveDelay.Ticks;
            for (var i = 0; i < iterations; i++)
            {
                if (!queue.IsEmpty && queue.TryDequeue(out var value))
                    return value;

                await Task.Delay(effectiveDelay, CancellationToken.None);
            }

            throw new TimeoutException("A timeout occurred while trying to dequeue a message.");
        }
    }
}
