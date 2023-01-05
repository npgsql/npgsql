using BenchmarkDotNet.Attributes;
using Npgsql.Internal.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Benchmarks;

[MemoryDiagnoser]
public class ResolveHandler
{
    NpgsqlDataSource? _dataSource;
    TypeMapper _typeMapper = null!;

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
        _dataSource = dataSourceBuilder.Build();
        _typeMapper = _dataSource.TypeMapper;
    }

    [GlobalCleanup]
    public void Cleanup() => _dataSource?.Dispose();

    [Benchmark]
    public NpgsqlTypeHandler ResolveOID()
        => _typeMapper.ResolveByOID(23); // int4

    [Benchmark]
    public NpgsqlTypeHandler ResolveNpgsqlDbType()
        => _typeMapper.ResolveByNpgsqlDbType(NpgsqlDbType.Integer);

    [Benchmark]
    public NpgsqlTypeHandler ResolveDataTypeName()
        => _typeMapper.ResolveByDataTypeName("integer");

    [Benchmark]
    public NpgsqlTypeHandler ResolveClrTypeNonGeneric()
        => _typeMapper.ResolveByValue((object)8);

    [Benchmark]
    public NpgsqlTypeHandler ResolveClrTypeGeneric()
        => _typeMapper.ResolveByValue(8);
}
