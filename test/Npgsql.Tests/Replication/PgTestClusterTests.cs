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
        await using var standby = await primary.StartStandby();

        Assert.That(await IsInRecovery(primary), Is.False);
        Assert.That(await IsInRecovery(standby), Is.True);

        await standby.Promote();
        Assert.That(await IsInRecovery(standby), Is.False);

        await primary.Stop();
    }

    static async Task<bool> IsInRecovery(PgTestCluster cluster)
    {
        await using var conn = new NpgsqlConnection(cluster.ConnectionString);
        await conn.OpenAsync();
        return (bool)(await conn.ExecuteScalarAsync("SELECT pg_is_in_recovery()"))!;
    }
}
