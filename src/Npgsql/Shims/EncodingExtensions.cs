// ReSharper disable RedundantUsingDirective
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper restore RedundantUsingDirective

// ReSharper disable once CheckNamespace
namespace System.Text;

static class EncodingExtensions
{
#if NETSTANDARD2_0

    /// <summary>
    /// Returns a reference to the 0th element of the ReadOnlySpan. If the ReadOnlySpan is empty, returns a reference to fake non-null pointer. Such a reference
    /// can be used for pinning but must never be dereferenced. This is useful for interop with methods that do not accept null pointers for zero-sized buffers.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe ref readonly T GetNonNullPinnableReference<T>(ReadOnlySpan<T> span)
        => ref span.Length != 0 ? ref span.GetPinnableReference() : ref Unsafe.AsRef<T>((void*)1);

    /// <summary>
    /// Returns a reference to the 0th element of the ReadOnlySpan. If the ReadOnlySpan is empty, returns a reference to fake non-null pointer. Such a reference
    /// can be used for pinning but must never be dereferenced. This is useful for interop with methods that do not accept null pointers for zero-sized buffers.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe ref T GetNonNullPinnableReference<T>(Span<T> span)
        => ref span.Length != 0 ? ref span.GetPinnableReference() : ref Unsafe.AsRef<T>((void*)1);

    public static unsafe int GetByteCount(this Encoding encoding, ReadOnlySpan<char> chars)
    {
        fixed (char* charsPtr = &GetNonNullPinnableReference(chars))
        {
            return encoding.GetByteCount(charsPtr, chars.Length);
        }
    }

    public static unsafe int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        fixed (char* charsPtr = &GetNonNullPinnableReference(chars))
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        {
            return encoding.GetBytes(charsPtr, chars.Length, bytesPtr, bytes.Length);
        }
    }

    public static unsafe int GetCharCount(this Encoding encoding, ReadOnlySpan<byte> bytes)
    {
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        {
            return encoding.GetCharCount(bytesPtr, bytes.Length);
        }
    }

    public static unsafe int GetCharCount(this Decoder encoding, ReadOnlySpan<byte> bytes, bool flush)
    {
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        {
            return encoding.GetCharCount(bytesPtr, bytes.Length, flush);
        }
    }

    public static unsafe int GetChars(this Decoder encoding, ReadOnlySpan<byte> bytes, Span<char> chars, bool flush)
    {
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        fixed (char* charsPtr = &GetNonNullPinnableReference(chars))
        {
            return encoding.GetChars(bytesPtr, bytes.Length, charsPtr, chars.Length, flush);
        }
    }

    public static unsafe int GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars)
    {
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        fixed (char* charsPtr = &GetNonNullPinnableReference(chars))
        {
            return encoding.GetChars(bytesPtr, bytes.Length, charsPtr, chars.Length);
        }
    }

    public static unsafe void Convert(this Encoder encoder, ReadOnlySpan<char> chars, Span<byte> bytes, bool flush, out int charsUsed, out int bytesUsed, out bool completed)
    {
        fixed (char* charsPtr = &GetNonNullPinnableReference(chars))
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        {
            encoder.Convert(charsPtr, chars.Length, bytesPtr, bytes.Length, flush, out charsUsed, out bytesUsed, out completed);
        }
    }

    public static unsafe void Convert(this Decoder encoder, ReadOnlySpan<byte> bytes, Span<char> chars, bool flush, out int bytesUsed, out int charsUsed, out bool completed)
    {
        fixed (byte* bytesPtr = &GetNonNullPinnableReference(bytes))
        fixed (char* charsPtr = &GetNonNullPinnableReference(chars))
        {
            encoder.Convert(bytesPtr, bytes.Length, charsPtr, chars.Length, flush, out bytesUsed, out charsUsed, out completed);
        }
    }
#endif

#if NETSTANDARD
    /// <summary>
    /// Decodes the specified <see cref="ReadOnlySequence{Byte}"/> to <see langword="char"/>s using the specified <see cref="Encoding"/>
    /// and outputs the result to <paramref name="chars"/>.
    /// </summary>
    /// <param name="encoding">The <see cref="Encoding"/> which represents how the data in <paramref name="bytes"/> is encoded.</param>
    /// <param name="bytes">The <see cref="ReadOnlySequence{Byte}"/> to decode to characters.</param>
    /// <param name="chars">The destination buffer to which the decoded characters will be written.</param>
    /// <returns>The number of chars written to <paramref name="chars"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="chars"/> is not large enough to contain the encoded form of <paramref name="bytes"/>.</exception>
    /// <exception cref="DecoderFallbackException">Thrown if <paramref name="bytes"/> contains data that cannot be decoded and <paramref name="encoding"/> is configured
    /// to throw an exception when such data is seen.</exception>
    public static int GetChars(this Encoding encoding, in ReadOnlySequence<byte> bytes, Span<char> chars)
    {
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        if (bytes.IsSingleSegment)
        {
            // If the incoming sequence is single-segment, one-shot this.

            return encoding.GetChars(bytes.First.Span, chars);
        }
        else
        {
            // If the incoming sequence is multi-segment, create a stateful Decoder
            // and use it as the workhorse. On the final iteration we'll pass flush=true.

            var remainingBytes = bytes;
            var originalCharsLength = chars.Length;
            var decoder = encoding.GetDecoder();
            bool isFinalSegment;

            do
            {
                var firstSpan = remainingBytes.First.Span;
                var next = remainingBytes.GetPosition(firstSpan.Length);
                isFinalSegment = remainingBytes.IsSingleSegment;

                var charsWrittenJustNow = decoder.GetChars(firstSpan, chars, flush: isFinalSegment);
                chars = chars.Slice(charsWrittenJustNow);
                remainingBytes = remainingBytes.Slice(next);
            } while (!isFinalSegment);

            return originalCharsLength - chars.Length; // total number of chars we wrote
        }
    }

    public static string GetString(this Encoding encoding, in ReadOnlySequence<byte> bytes)
    {
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        // If the incoming sequence is single-segment, one-shot this.
        if (bytes.IsSingleSegment)
        {
#if NETSTANDARD2_1
            return encoding.GetString(bytes.First.Span);
#else
            var rented = false;
            byte[] arr;
            var offset = 0;
            var memory = bytes.First;
            if (MemoryMarshal.TryGetArray(memory, out var segment))
            {
                arr = segment.Array!;
                offset = segment.Offset;
            }
            else
            {
                rented = true;
                arr = ArrayPool<byte>.Shared.Rent(memory.Length);
                bytes.First.Span.CopyTo(arr);
            }
            var ret = encoding.GetString(arr, offset, memory.Length);
            if (rented)
                ArrayPool<byte>.Shared.Return(arr);
            return ret;
#endif
        }

        // If the incoming sequence is multi-segment, create a stateful Decoder
        // and use it as the workhorse. On the final iteration we'll pass flush=true.

        var decoder = encoding.GetDecoder();

        // Maintain a list of all the segments we'll need to concat together.
        // These will be released back to the pool at the end of the method.

        var listOfSegments = new List<(char[], int)>();
        var totalCharCount = 0;

        var remainingBytes = bytes;
        bool isFinalSegment;

        do
        {
            var firstSpan = remainingBytes.First.Span;
            var next = remainingBytes.GetPosition(firstSpan.Length);
            isFinalSegment = remainingBytes.IsSingleSegment;

            var charCountThisIteration = decoder.GetCharCount(firstSpan, flush: isFinalSegment); // could throw ArgumentException if overflow would occur
            var rentedArray = ArrayPool<char>.Shared.Rent(charCountThisIteration);
            var actualCharsWrittenThisIteration = decoder.GetChars(firstSpan, rentedArray, flush: isFinalSegment);
            listOfSegments.Add((rentedArray, actualCharsWrittenThisIteration));

            totalCharCount += actualCharsWrittenThisIteration;
            if (totalCharCount < 0)
            {
                // If we overflowed, call string.Create, passing int.MaxValue.
                // This will end up throwing the expected OutOfMemoryException
                // since strings are limited to under int.MaxValue elements in length.

                totalCharCount = int.MaxValue;
                break;
            }

            remainingBytes = remainingBytes.Slice(next);
        } while (!isFinalSegment);

        // Now build up the string to return, then release all of our scratch buffers
        // back to the shared pool.
        var chars = ArrayPool<char>.Shared.Rent(totalCharCount);
        var span = chars.AsSpan();
        foreach (var (array, length) in listOfSegments)
        {
            array.AsSpan(0, length).CopyTo(span);
            ArrayPool<char>.Shared.Return(array);
            span = span.Slice(length);
        }

        var str = new string(chars);
        ArrayPool<char>.Shared.Return(chars);
        return str;
    }
#endif
}
