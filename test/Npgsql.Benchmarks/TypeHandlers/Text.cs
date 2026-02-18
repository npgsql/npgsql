using BenchmarkDotNet.Attributes;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using System.Collections.Generic;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Text() : TypeHandlerBenchmarks<string>(new StringTextConverter(NpgsqlWriteBuffer.UTF8Encoding))
{
    protected override IEnumerable<string> ValuesOverride()
    {
        for (var i = 1; i <= 10000; i *= 10)
            yield return new string('x', i);
    }
}
