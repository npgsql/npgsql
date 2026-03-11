using BenchmarkDotNet.Attributes;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

namespace Npgsql.Benchmarks;

[MemoryDiagnoser]
public class ResolveHandler
{
    PgSerializerOptions _serializerOptions = null!;

    [Params(0, 1, 2)]
    public int NumPlugins { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder();
        if (NumPlugins > 0)
            dataSourceBuilder.UseNodaTime();
        if (NumPlugins > 1)
            dataSourceBuilder.UseNetTopologySuite();

        // Alternatively we must build a data source and get it bootstrapped against a real database.
        (_, var config) = dataSourceBuilder.PrepareConfiguration();
        _serializerOptions = new PgSerializerOptions(PostgresMinimalDatabaseInfo.DefaultTypeCatalog, config.ResolverChain);
    }

    [Benchmark]
    public PgTypeInfo? ResolveDefault()
        => _serializerOptions.GetTypeInfoInternal(null, new Oid(23)); // int4

    [Benchmark]
    public PgTypeInfo? ResolveType()
        => _serializerOptions.GetTypeInfoInternal(typeof(int), null);

    [Benchmark]
    public PgTypeInfo? ResolveBoth()
        => _serializerOptions.GetTypeInfoInternal(typeof(int), new Oid(23)); // int4
}
