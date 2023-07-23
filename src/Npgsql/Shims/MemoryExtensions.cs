#if !NET7_0_OR_GREATER
namespace System;

static class MemoryExtensions
{
    public static int IndexOfAnyExcept<T>(this ReadOnlySpan<T> span, T value0, T value1) where T : IEquatable<T>
    {
        for (var i = 0; i < span.Length; i++)
        {
            var v = span[i];
            if (!v.Equals(value0) && !v.Equals(value1))
                return i;
        }

        return -1;
    }
}
#endif
