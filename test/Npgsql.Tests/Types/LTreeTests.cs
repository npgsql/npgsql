using System.Data;
using System.Threading.Tasks;
using Npgsql.Properties;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public class LTreeTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public Task LQuery()
        => AssertType("Top.Science.*", "Top.Science.*",
            "lquery", dataTypeInference: DataTypeInferenceKind.WellKnown,
            dbType: DbType.String);

    [Test]
    public Task LTree()
        => AssertType("Top.Science.Astronomy", "Top.Science.Astronomy",
            "ltree", dataTypeInference: DataTypeInferenceKind.WellKnown,
            dbType: DbType.String);

    [Test]
    public Task LTxtQuery()
        => AssertType("Science & Astronomy", "Science & Astronomy",
            "ltxtquery", dataTypeInference: DataTypeInferenceKind.WellKnown,
            dbType: DbType.String);

    [Test]
    public async Task LTree_not_supported_by_default_on_NpgsqlSlimSourceBuilder()
    {
        var errorMessage = string.Format(
            NpgsqlStrings.LTreeNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableLTree), nameof(NpgsqlSlimDataSourceBuilder));

        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        await using var dataSource = dataSourceBuilder.Build();

        var exception =
            await AssertTypeUnsupportedRead<NpgsqlRange<int>>("Top.Science.Astronomy", "ltree", dataSource);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));
        exception = await AssertTypeUnsupportedWrite<string>("Top.Science.Astronomy", "ltree", dataSource);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task NpgsqlSlimSourceBuilder_EnableLTree([Values] bool withArrays)
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableLTree();
        if (withArrays)
            dataSourceBuilder.EnableArrays();
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, "Top.Science.Astronomy", "Top.Science.Astronomy",
            "ltree", dataTypeInference: DataTypeInferenceKind.WellKnown,
            dbType: DbType.String,
            skipArrayCheck: !withArrays);
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "13.0");
        await TestUtil.EnsureExtensionAsync(conn, "ltree");
    }
}
