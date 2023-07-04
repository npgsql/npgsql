using System;
using System.Diagnostics;

namespace Npgsql.Internal;

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
            _ => throw new UnreachableException()
        };

    public static short ToFormatCode(this DataFormat dataFormat)
        => dataFormat switch
        {
            DataFormat.Text => 0,
            DataFormat.Binary => 1,
            var code => throw new ArgumentOutOfRangeException("", code, "Unknown postgres format code, please file a bug,")
        };
}
