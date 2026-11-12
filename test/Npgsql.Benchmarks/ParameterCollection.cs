using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks;

[MemoryDiagnoser]
public class ParameterCollection
{
    private NpgsqlParameter[] manyParameters = [];

    [IterationSetup]
    public void IterationSetup()
    {
        List<NpgsqlParameter> parameters = [];

        for (var i = 0; i < 2000; i++)
        {
            parameters.Add(new NpgsqlParameter());
        }

        manyParameters = parameters.ToArray();
    }

    [Benchmark]
    public void AddRange()
    {
        NpgsqlParameterCollection parameterCollection = [];
        parameterCollection.AddRange(manyParameters);
    }
}
