using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class StreamConverter(bool supportsTextFormat) : PgStreamingConverter<Stream>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return supportsTextFormat
            ? format is DataFormat.Text or DataFormat.Binary
            : format is DataFormat.Binary;
    }

    public override Stream Read(PgReader reader)
        => reader.GetStream();

    public override ValueTask<Stream> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(reader.GetStream());

    public override Size GetSize(SizeContext context, Stream value, ref object? writeState)
    {
        if (value.CanSeek)
            return checked((int)(value.Length - value.Position));

        var memoryStream = new MemoryStream();
        value.CopyTo(memoryStream);
        writeState = memoryStream;
        return checked((int)memoryStream.Length);
    }

    public override void Write(PgWriter writer, Stream value)
    {
        if (writer.Current.WriteState is not null)
        {
            if (!((MemoryStream)writer.Current.WriteState!).TryGetBuffer(out var writeStateSegment))
                throw new InvalidOperationException();

            writer.WriteBytes(writeStateSegment.AsSpan());
            return;
        }

        // Non-derived MemoryStream fast path
        if (value is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var segment))
            writer.WriteBytes(segment.AsSpan((int)value.Position));
        else
            value.CopyTo(writer.GetStream());
    }

    public override ValueTask WriteAsync(PgWriter writer, Stream value, CancellationToken cancellationToken = default)
    {
        if (writer.Current.WriteState is not null)
        {
            if (!((MemoryStream)writer.Current.WriteState!).TryGetBuffer(out var writeStateSegment))
                throw new InvalidOperationException();

            return writer.WriteBytesAsync(writeStateSegment.AsMemory(), cancellationToken);
        }

        // Non-derived MemoryStream fast path
        if (value is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var segment))
        {
            return writer.WriteBytesAsync(segment.AsMemory((int)value.Position), cancellationToken);
        }
        else
        {
            return new ValueTask(value.CopyToAsync(writer.GetStream(), cancellationToken));
        }
    }
}
