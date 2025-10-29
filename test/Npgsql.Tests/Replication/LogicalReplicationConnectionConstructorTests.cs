using System.Threading.Tasks;
using NUnit.Framework;
using Npgsql.Replication;

namespace Npgsql.Tests.Replication;

[Platform(Exclude = "MacOsX", Reason = "Replication tests are flaky in CI on Mac")]
[NonParallelizable]
public class LogicalReplicationConnectionConstructorTests : SafeReplicationTestBase<LogicalReplicationConnection>
{
    [Test]
    public async Task Construct_from_existing_connection()
    {
        var baseConn = new NpgsqlConnection(TestUtil.ConnectionString);
        await using var rc = new LogicalReplicationConnection(baseConn);
        await rc.Open();

        var csb = new NpgsqlConnectionStringBuilder(rc.ConnectionString);
        Assert.That(csb.ReplicationMode, Is.EqualTo(ReplicationMode.Logical));
        Assert.That(csb.Pooling, Is.False);
        Assert.That(csb.Enlist, Is.False);
        Assert.That(csb.Multiplexing, Is.False);
        Assert.That(csb.KeepAlive, Is.EqualTo(0));
    }

    protected override string Postfix => "ctor_l";
}
