using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Physical replication tests that need their own clusters (see <see cref="PgTestCluster"/>), because they promote a
/// standby or shut the server down. They only run when NPGSQL_TEST_PG_BIN is set.
/// </summary>
[NonParallelizable] // Every test starts PostgreSQL servers
public class PhysicalReplicationClusterTests : TestBase
{
    static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

    [Test, Description("A cascading walsender ends the old timeline after its standby gets promoted, and streaming can go on on the new one")]
    public async Task Streaming_from_a_standby_across_its_promotion()
    {
        await using var primary = await PgTestCluster.StartPrimary();
        await using var standby = await primary.StartStandby();
        await using var rc = new PhysicalReplicationConnection(standby.ConnectionString);
        await rc.Open();

        // Streaming on timeline 1 ends when the standby gets promoted
        using (var cts = new CancellationTokenSource(Timeout))
        {
            var info = await rc.IdentifySystem(cts.Token);
            Assert.That(info.Timeline, Is.EqualTo(1u));
            var stream = ReadToEnd(rc.StartReplication(info.XLogPos, cts.Token, info.Timeline));
            await WaitForWalSender(standby);

            await standby.Promote();

            // The stream of the old timeline ends normally
            await stream.WaitAsync(Timeout);
        }

        // The connection can be used again, to find out where to go on
        NpgsqlLogSequenceNumber switchPoint;
        using (var cts = new CancellationTokenSource(Timeout))
        {
            Assert.That((await rc.IdentifySystem(cts.Token)).Timeline, Is.EqualTo(2u));
            var history = await rc.TimelineHistory(2, cts.Token);
            Assert.That(history.FileName, Is.EqualTo("00000002.history"));
            Assert.That(Encoding.ASCII.GetString(history.Content), Does.StartWith("1\t"));
            // Where timeline 2 branched off timeline 1, which may be before the end of the WAL we received on timeline 1
            switchPoint = NpgsqlLogSequenceNumber.Parse(Encoding.ASCII.GetString(history.Content).Split('\t')[1]);
        }

        // Streaming goes on on the new timeline, starting with the WAL the promotion wrote
        using (var cts = new CancellationTokenSource(Timeout))
        {
            var enumerator = rc.StartReplication(switchPoint, cts.Token, timeline: 2).GetAsyncEnumerator(cts.Token);
            Assert.That(await enumerator.MoveNextAsync(), Is.True);
            Assert.That(enumerator.Current.WalStart, Is.EqualTo(switchPoint));
            await enumerator.DisposeAsync();
        }
    }

    [Test, Description("A walsender that shuts down after everything has been replicated ends the stream")]
    public async Task Streaming_ends_when_the_server_shuts_down()
    {
        await using var primary = await PgTestCluster.StartPrimary();
        await using var rc = new PhysicalReplicationConnection(primary.ConnectionString);
        await rc.Open();
        using var cts = new CancellationTokenSource(Timeout);

        var info = await rc.IdentifySystem(cts.Token);
        var stream = ReadToEnd(rc.StartReplication(info.XLogPos, cts.Token));
        await WaitForWalSender(primary);

        // A fast shutdown waits for the walsender, which waits for us to report that we have everything
        await primary.Stop("fast").WaitAsync(Timeout);
        Assert.That(async () => await stream.WaitAsync(Timeout),
            Throws.Exception.TypeOf<NpgsqlException>().With.Message.Contains("ended the replication stream"));
    }

    static async Task ReadToEnd(IAsyncEnumerable<XLogDataMessage> messages)
    {
        await foreach (var _ in messages)
        {
        }
    }

    static async Task WaitForWalSender(PgTestCluster cluster)
    {
        await using var conn = new NpgsqlConnection(cluster.ConnectionString);
        await conn.OpenAsync();
        for (var i = 0; i < 100; i++)
        {
            if ((long)(await conn.ExecuteScalarAsync("SELECT count(*) FROM pg_stat_replication WHERE state = 'streaming'"))! > 0)
                return;
            await Task.Delay(100);
        }

        Assert.Fail("No walsender started streaming");
    }
}
