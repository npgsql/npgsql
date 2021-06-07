using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Npgsql.Netstandard20
{
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
}
