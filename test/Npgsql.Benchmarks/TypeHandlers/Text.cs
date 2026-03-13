using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Text;
using Npgsql.Internal;
using Npgsql.Internal.Converters;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Text() : TypeHandlerBenchmarks<string>(new StringTextConverter(Encoding.UTF8))
{
    protected override IEnumerable<string> ValuesOverride()
    {
        for (var i = Encoding.UTF8.GetByteCount("x"); i <= NpgsqlWriteBuffer.DefaultSize; i *= 4)
            yield return new string('x', i);
    }
}
