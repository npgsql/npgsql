using System;
using System.Runtime.CompilerServices;

namespace Npgsql.Netstandard20;

static class ReadOnlySpanOfCharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt(this ReadOnlySpan<char> span)
        => int.Parse(span
#if NETSTANDARD2_0
                    .ToString()
#endif
        );
}