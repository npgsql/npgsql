using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public enum DataFormat : byte
{
    Binary,
    Text
}

static class DataFormatUtils
{
    public static DataFormat Create(short formatCode)
        => formatCode switch
        {
            0 => DataFormat.Text,
            1 => DataFormat.Binary,
            _ => throw new ArgumentOutOfRangeException(nameof(formatCode), formatCode, "Unknown postgres format code, please file a bug,")
        };

    public static short ToFormatCode(this DataFormat dataFormat)
        => dataFormat switch
        {
            DataFormat.Text => 0,
            DataFormat.Binary => 1,
            _ => throw new UnreachableException()
        };
}
