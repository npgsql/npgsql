using BenchmarkDotNet.Attributes;
using Npgsql.Internal.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Benchmarks
{
    [MemoryDiagnoser]
    public class ResolveHandler
    {
        NpgsqlConnection _conn = null!;
        ConnectorTypeMapper _typeMapper = null!;

        [Params(0, 1, 2)]
        public int NumPlugins { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            _typeMapper = (ConnectorTypeMapper)_conn.TypeMapper;

            if (NumPlugins > 0)
                _typeMapper.UseNodaTime();
            if (NumPlugins > 1)
                _typeMapper.UseNetTopologySuite();
        }

        [GlobalCleanup]
        public void Cleanup() => _conn.Dispose();

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
}
