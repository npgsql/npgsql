using Npgsql.Internal;
using Npgsql.Internal.Converters;
using System.Collections.Generic;
using System.Text;

namespace Npgsql.Benchmarks.TypeHandlers;

public class Text() : TypeHandlerBenchmarks<string>(StringTextConverter.Create(NpgsqlWriteBuffer.UTF8Encoding))
{
    protected override IEnumerable<string> ValuesOverride()
    {
        for (var i = Encoding.UTF8.GetByteCount("x"); i <= NpgsqlWriteBuffer.DefaultSize; i *= 4)
            yield return new string('x', i);
    }
}
