using Npgsql.Internal;
using Npgsql.Internal.Converters;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Text() : TypeHandlerBenchmarks<string>(new StringTextConverter(NpgsqlWriteBuffer.UTF8Encoding))
{
    protected override IEnumerable<string> ValuesOverride()
    {
        for (var i = Encoding.UTF8.GetByteCount("x"); i <= NpgsqlWriteBuffer.DefaultSize; i *= 4)
            yield return new string('x', i);
    }
}
