using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Text;
using Npgsql.Internal.Converters;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Text() : TypeHandlerBenchmarks<string>(new StringTextConverter(Encoding.UTF8))
{
    protected override IEnumerable<string> ValuesOverride()
    {
        for (var i = 1; i <= 10000; i *= 10)
            yield return new string('x', i);
    }
}
