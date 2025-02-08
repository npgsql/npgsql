using System;
using System.Text;

namespace Npgsql.Internal;

/// <summary>
/// A special instance of <see cref="UTF8Encoding"/>. This type allows for devirtualization
/// when the type is known.
/// </summary>
internal sealed class UTF8EncodingNoBom : UTF8Encoding
{
    public UTF8EncodingNoBom() : base(false) { }
    public UTF8EncodingNoBom(bool throwOnInvalidBytes) : base(false, throwOnInvalidBytes) { }

    public override ReadOnlySpan<byte> Preamble => default;

    public override int GetMaxByteCount(int charCount)
    {
        // This is a specialization of UTF8Encoding.GetMaxByteCount
        // with the assumption that the default replacement fallback
        // emits 3 fallback bytes ([ EF BF BD ] = '\uFFFD') per
        // malformed input char in the worst case.
        const int MaxUtf8BytesPerChar = 3;

        if ((uint)charCount > (int.MaxValue / MaxUtf8BytesPerChar) - 1)
            ThrowHelper.ThrowArgumentOutOfRangeException();

        return (charCount * MaxUtf8BytesPerChar) + MaxUtf8BytesPerChar;
    }
}
