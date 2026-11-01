using System.Threading.Tasks;
using Npgsql.Tests.Support;
using NUnit.Framework;

namespace Npgsql.Tests.Replication;

/// <summary>
/// Smoke test for <see cref="PgTestCluster"/>, which only runs when NPGSQL_TEST_PG_BIN is set.
/// </summary>
public class PgTestClusterTests : TestBase
{
    [Test]
    public async Task Standby_and_promotion()
    {
        await using var primary = await PgTestCluster.StartPrimary();
        // Before the base backup, so that only the rows have to be replayed
        await Execute(primary, "CREATE TABLE replayed (id int)");
        await using var standby = await primary.StartStandby();

        Assert.That(await IsInRecovery(primary), Is.False);
        Assert.That(await IsInRecovery(standby), Is.True);

        await Execute(primary, "INSERT INTO replayed VALUES (1)");
        await WaitForRow(standby, 1);

        await standby.Promote();
        Assert.That(await IsInRecovery(standby), Is.False);

        await primary.Stop();
    }

    [Test, Description("A primary keeps its port when it restarts, so that its standbys reconnect")]
    public async Task Standby_follows_its_primary_across_a_restart()
    {
        await using var primary = await PgTestCluster.StartPrimary();
        await Execute(primary, "CREATE TABLE replayed (id int)");
        await using var standby = await primary.StartStandby();
        var port = primary.Port;

        await primary.Stop();
        await primary.Start();
        Assert.That(primary.Port, Is.EqualTo(port));

        await Execute(primary, "INSERT INTO replayed VALUES (1)");
        await WaitForRow(standby, 1);
    }

    static async Task<bool> IsInRecovery(PgTestCluster cluster)
    {
        await using var conn = new NpgsqlConnection(cluster.ConnectionString);
        await conn.OpenAsync();
        return (bool)(await conn.ExecuteScalarAsync("SELECT pg_is_in_recovery()"))!;
    }

    static async Task Execute(PgTestCluster cluster, string sql)
    {
        await using var conn = new NpgsqlConnection(cluster.ConnectionString);
        await conn.OpenAsync();
        await conn.ExecuteNonQueryAsync(sql);
    }

    // pg_is_in_recovery() is also true for a standby that isn't connected to its primary, so we check that it replays
    static async Task WaitForRow(PgTestCluster standby, int id)
    {
        await using var conn = new NpgsqlConnection(standby.ConnectionString);
        await conn.OpenAsync();
        for (var i = 0; i < 100; i++)
        {
            if ((bool)(await conn.ExecuteScalarAsync($"SELECT EXISTS (SELECT FROM replayed WHERE id = {id})"))!)
                return;
            await Task.Delay(100);
        }

        Assert.Fail($"The standby didn't replay row {id} within 10 seconds");
    }
}
