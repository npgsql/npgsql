using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using Npgsql.Internal.Converters;
using Npgsql.Internal;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Text() : TypeHandlerBenchmarks<string>(StringTextConverter.Create(NpgsqlWriteBuffer.UTF8Encoding))
{
    protected override IEnumerable<string> ValuesOverride()
    {
        for (var i = 1; i <= 1000; i *= 10)
            yield return new string('x', i);
    }
}
