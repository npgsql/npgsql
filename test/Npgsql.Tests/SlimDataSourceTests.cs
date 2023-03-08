using System;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests;

class SlimDataSourceTests : TestBase
{
    NpgsqlDataSource _slimWithoutMappingsDataSource = null!;
    NpgsqlDataSource _slimWithMappingsDataSource = null!;

    [Test]
    public async Task Record([Values] bool withMappings)
    {
        const string unsupportedMessage =
            "Records aren't supported; please call EnableRecord on NpgsqlSlimDataSourceBuilder to enable records.";
        Func<IResolveConstraint> assertExpr = () => withMappings
            ? Throws.Nothing
            : Throws.Exception
                .TypeOf<NotSupportedException>()
                .With.Property("Message").EqualTo(unsupportedMessage);

        var dataSource = withMappings ? _slimWithMappingsDataSource : _slimWithoutMappingsDataSource;
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        // RecordHandler doesn't support writing, so we only check for reading
        cmd.CommandText = "SELECT ('one'::text, 2)";
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(() => reader.GetValue(0), assertExpr());
        Assert.That(() => reader.GetFieldValue<object[]>(0), assertExpr());
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        _slimWithoutMappingsDataSource = new NpgsqlSlimDataSourceBuilder(ConnectionString)
            .Build();
        _slimWithMappingsDataSource = new NpgsqlSlimDataSourceBuilder(ConnectionString)
            .EnableRecord()
            .Build();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _slimWithoutMappingsDataSource.Dispose();
        _slimWithMappingsDataSource.Dispose();
    }
}
