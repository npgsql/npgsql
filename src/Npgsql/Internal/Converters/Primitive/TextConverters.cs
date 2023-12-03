using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

abstract class StringBasedTextConverter<T> : PgStreamingConverter<T>
{
    readonly Encoding _encoding;
    protected StringBasedTextConverter(Encoding encoding) => _encoding = encoding;

    public override T Read(PgReader reader)
        => Read(async: false, reader, _encoding).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, _encoding, cancellationToken);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => TextConverter.GetSize(ref context, ConvertTo(value), _encoding);

    public override void Write(PgWriter writer, T value)
        => writer.WriteChars(ConvertTo(value).Span, _encoding);

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => writer.WriteCharsAsync(ConvertTo(value), _encoding, cancellationToken);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return format is DataFormat.Binary or DataFormat.Text;
    }

    protected abstract ReadOnlyMemory<char> ConvertTo(T value);
    protected abstract T ConvertFrom(string value);

    ValueTask<T> Read(bool async, PgReader reader, Encoding encoding, CancellationToken cancellationToken = default)
    {
        return async
            ? ReadAsync(reader, encoding, cancellationToken)
            : new(ConvertFrom(encoding.GetString(reader.ReadBytes(reader.CurrentRemaining))));

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<T> ReadAsync(PgReader reader, Encoding encoding, CancellationToken cancellationToken)
            => ConvertFrom(encoding.GetString(await reader.ReadBytesAsync(reader.CurrentRemaining, cancellationToken).ConfigureAwait(false)));
    }
}

sealed class ReadOnlyMemoryTextConverter : StringBasedTextConverter<ReadOnlyMemory<char>>
{
    public ReadOnlyMemoryTextConverter(Encoding encoding) : base(encoding) { }
    protected override ReadOnlyMemory<char> ConvertTo(ReadOnlyMemory<char> value) => value;
    protected override ReadOnlyMemory<char> ConvertFrom(string value) => value.AsMemory();
}

sealed class StringTextConverter : StringBasedTextConverter<string>
{
    public StringTextConverter(Encoding encoding) : base(encoding) { }
    protected override ReadOnlyMemory<char> ConvertTo(string value) => value.AsMemory();
    protected override string ConvertFrom(string value) => value;
}

abstract class ArrayBasedTextConverter<T> : PgStreamingConverter<T>
{
    readonly Encoding _encoding;
    protected ArrayBasedTextConverter(Encoding encoding) => _encoding = encoding;

    public override T Read(PgReader reader)
        => Read(async: false, reader, _encoding).GetAwaiter().GetResult();
    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, _encoding);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => TextConverter.GetSize(ref context, ConvertTo(value), _encoding);

    public override void Write(PgWriter writer, T value)
        => writer.WriteChars(ConvertTo(value).AsSpan(), _encoding);

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => writer.WriteCharsAsync(ConvertTo(value), _encoding, cancellationToken);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return format is DataFormat.Binary or DataFormat.Text;
    }

    protected abstract ArraySegment<char> ConvertTo(T value);
    protected abstract T ConvertFrom(ArraySegment<char> value);

    ValueTask<T> Read(bool async, PgReader reader, Encoding encoding)
    {
        return async ? ReadAsync(reader, encoding) : new(ConvertFrom(GetSegment(reader.ReadBytes(reader.CurrentRemaining), encoding)));

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<T> ReadAsync(PgReader reader, Encoding encoding)
            => ConvertFrom(GetSegment(await reader.ReadBytesAsync(reader.CurrentRemaining).ConfigureAwait(false), encoding));

        static ArraySegment<char> GetSegment(ReadOnlySequence<byte> bytes, Encoding encoding)
        {
            var array = TextConverter.GetChars(encoding, bytes);
            return new(array, 0, array.Length);
        }
    }
}

sealed class CharArraySegmentTextConverter : ArrayBasedTextConverter<ArraySegment<char>>
{
    public CharArraySegmentTextConverter(Encoding encoding) : base(encoding) { }
    protected override ArraySegment<char> ConvertTo(ArraySegment<char> value) => value;
    protected override ArraySegment<char> ConvertFrom(ArraySegment<char> value) => value;
}

sealed class CharArrayTextConverter : ArrayBasedTextConverter<char[]>
{
    public CharArrayTextConverter(Encoding encoding) : base(encoding) { }
    protected override ArraySegment<char> ConvertTo(char[] value) => new(value, 0, value.Length);
    protected override char[] ConvertFrom(ArraySegment<char> value)
    {
        if (value.Array?.Length == value.Count)
            return value.Array!;

        var array = new char[value.Count];
        Array.Copy(value.Array!, value.Offset, array, 0, value.Count);
        return array;
    }
}

sealed class CharTextConverter : PgBufferedConverter<char>
{
    readonly Encoding _encoding;
    readonly Size _oneCharMaxByteCount;

    public CharTextConverter(Encoding encoding)
    {
        _encoding = encoding;
        _oneCharMaxByteCount = Size.CreateUpperBound(encoding.GetMaxByteCount(1));
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Create(_oneCharMaxByteCount);
        return format is DataFormat.Binary or DataFormat.Text;
    }

    protected override char ReadCore(PgReader reader)
    {
        var byteSeq = reader.ReadBytes(Math.Min(_oneCharMaxByteCount.Value, reader.CurrentRemaining));
        Debug.Assert(byteSeq.IsSingleSegment);
        var bytes = byteSeq.FirstSpan;

        var chars = _encoding.GetCharCount(bytes);
        if (chars < 1)
            throw new NpgsqlException("Could not read char - string was empty");

        Span<char> destination = stackalloc char[chars];
        _encoding.GetChars(bytes, destination);
        return destination[0];
    }

    public override Size GetSize(SizeContext context, char value, ref object? writeState)
    {
        Span<char> spanValue = stackalloc char[] { value };
        return _encoding.GetByteCount(spanValue);
    }

    protected override void WriteCore(PgWriter writer, char value)
    {
        Span<char> spanValue = stackalloc char[] { value };
        writer.WriteChars(spanValue, _encoding);
    }
}

sealed class TextReaderTextConverter : PgStreamingConverter<TextReader>
{
    readonly Encoding _encoding;
    public TextReaderTextConverter(Encoding encoding) => _encoding = encoding;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return format is DataFormat.Binary or DataFormat.Text;
    }

    public override TextReader Read(PgReader reader)
        => reader.GetTextReader(_encoding);

    public override ValueTask<TextReader> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => reader.GetTextReaderAsync(_encoding, cancellationToken);

    public override Size GetSize(SizeContext context, TextReader value, ref object? writeState) => throw new NotImplementedException();
    public override void Write(PgWriter writer, TextReader value) => throw new NotImplementedException();
    public override ValueTask WriteAsync(PgWriter writer, TextReader value, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}


readonly struct GetChars
{
    public int Read { get; }
    public GetChars(int read) => Read = read;
}

sealed class GetCharsTextConverter : PgStreamingConverter<GetChars>, IResumableRead
{
    readonly Encoding _encoding;
    public GetCharsTextConverter(Encoding encoding) => _encoding = encoding;

    public override GetChars Read(PgReader reader)
        => reader.IsCharsRead
            ? ResumableRead(reader)
            : throw new NotSupportedException();

    public override ValueTask<GetChars> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public override Size GetSize(SizeContext context, GetChars value, ref object? writeState) => throw new NotSupportedException();
    public override void Write(PgWriter writer, GetChars value) => throw new NotSupportedException();
    public override ValueTask WriteAsync(PgWriter writer, GetChars value, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    GetChars ResumableRead(PgReader reader)
    {
        reader.GetCharsReadInfo(_encoding, out var charsRead, out var textReader, out var charsOffset, out var buffer);
        if (charsOffset < charsRead || (buffer is null && charsRead > 0))
        {
            // With variable length encodings, moving backwards based on bytes means we have to start over.
            reader.ResetCharsRead(out charsRead);
        }

        // First seek towards the charsOffset.
        // If buffer is null read the entire thing and report the length, see sql client remarks.
        // https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader.getchars
        int read;
        if (buffer is null)
        {
            read = ConsumeChars(textReader, null);
        }
        else
        {
            var consumed = ConsumeChars(textReader, charsOffset - charsRead);
            Debug.Assert(consumed == charsOffset - charsRead);
            read = textReader.ReadBlock(buffer.GetValueOrDefault().Array!, buffer.GetValueOrDefault().Offset, buffer.GetValueOrDefault().Count);
        }

        return new(read);

        static int ConsumeChars(TextReader reader, int? count)
        {
            if (count is 0)
                return 0;

            const int maxStackAlloc = 512;
            Span<char> tempCharBuf = stackalloc char[maxStackAlloc];
            var totalRead = 0;
            var fin = false;
            while (!fin)
            {
                var toRead = count is null ? maxStackAlloc : Math.Min(maxStackAlloc, count.Value - totalRead);
                var read = reader.ReadBlock(tempCharBuf.Slice(0, toRead));
                totalRead += read;
                if (read is 0 && count >= totalRead)
                    throw new EndOfStreamException();

                fin = count is null ? read is 0 : totalRead >= count;
            }
            return totalRead;
        }
    }

    bool IResumableRead.Supported => true;
}

// Moved out for code size/sharing.
static class TextConverter
{
    public static Size GetSize(ref SizeContext context, ReadOnlyMemory<char> value, Encoding encoding)
        => encoding.GetByteCount(value.Span);

    // Adapted version of GetString(ROSeq) removing the intermediate string allocation to make a contiguous char array.
    public static char[] GetChars(Encoding encoding, ReadOnlySequence<byte> bytes)
    {
        if (bytes.IsSingleSegment)
        {
            // If the incoming sequence is single-segment, one-shot this.
            var firstSpan = bytes.First.Span;
            var chars = new char[encoding.GetCharCount(firstSpan)];
            encoding.GetChars(bytes.First.Span, chars);
            return chars;
        }
        else
        {
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
                    throw new OutOfMemoryException();

                remainingBytes = remainingBytes.Slice(next);
            } while (!isFinalSegment);

            // Now build up the string to return, then release all of our scratch buffers
            // back to the shared pool.
            var chars = new char[totalCharCount];
            var span = chars.AsSpan();
            foreach (var (array, length) in listOfSegments)
            {
                array.AsSpan(0, length).CopyTo(span);
                ArrayPool<char>.Shared.Return(array);
                span = span.Slice(length);
            }

            return chars;
        }
    }
}
