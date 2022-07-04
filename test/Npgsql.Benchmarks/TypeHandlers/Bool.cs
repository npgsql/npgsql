using BenchmarkDotNet.Attributes;
using Npgsql.Internal.TypeHandlers;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Bool : TypeHandlerBenchmarks<bool>
{
    public Bool() : base(new BoolHandler(GetPostgresType("boolean"))) { }
}
