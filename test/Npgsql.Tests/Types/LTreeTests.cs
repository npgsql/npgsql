using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

[NonParallelizable]
public class LTreeTests : MultiplexingTestBase
{
    [Test]
    public Task LQuery()
        => AssertType("Top.Science.*", "Top.Science.*", "lquery", NpgsqlDbType.LQuery, isDefaultForWriting: false);

    [Test]
    public Task LTree()
        => AssertType("Top.Science.Astronomy", "Top.Science.Astronomy", "ltree", NpgsqlDbType.LTree, isDefaultForWriting: false);

    [Test]
    public Task LTxtQuery()
        => AssertType("Science & Astronomy", "Science & Astronomy", "ltxtquery", NpgsqlDbType.LTxtQuery, isDefaultForWriting: false);

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "13.0");
        await TestUtil.EnsureExtensionAsync(conn, "ltree");
    }

    public LTreeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
