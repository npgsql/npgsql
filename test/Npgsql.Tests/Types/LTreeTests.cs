using System;
using System.Threading.Tasks;
using Npgsql.Properties;
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

    [Test]
    public async Task LTree_not_supported_by_default_on_NpgsqlSlimSourceBuilder()
    {
        var errorMessage = string.Format(
            NpgsqlStrings.LTreeNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableLTree), nameof(NpgsqlSlimDataSourceBuilder));

        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        await using var dataSource = dataSourceBuilder.Build();

        var exception =
            await AssertTypeUnsupportedRead<NpgsqlRange<int>, NotSupportedException>("Top.Science.Astronomy", "ltree", dataSource);
        Assert.That(exception.Message, Is.EqualTo(errorMessage));
        exception = await AssertTypeUnsupportedWrite<string, NotSupportedException>("Top.Science.Astronomy", "ltree", dataSource);
        Assert.That(exception.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task NpgsqlSlimSourceBuilder_EnableRanges()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableRanges();
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType("Top.Science.Astronomy", "Top.Science.Astronomy", "ltree", NpgsqlDbType.LTree, isDefaultForWriting: false);
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "13.0");
        await TestUtil.EnsureExtensionAsync(conn, "ltree");
    }

    public LTreeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
