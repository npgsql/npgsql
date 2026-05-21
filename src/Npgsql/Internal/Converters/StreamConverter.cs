using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

#pragma warning disable CS9113 // supportsTextFormat is no longer consulted by the converter — format registration is handled externally. Parameter retained for caller-source stability pending registration cleanup.
sealed class StreamConverter(bool supportsTextFormat) : PgStreamingConverter<Stream>
#pragma warning restore CS9113
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => new() { BufferRequirements = BufferRequirements.Streaming };

    public override Stream Read(PgReader reader)
        => reader.GetStream();

    public override ValueTask<Stream> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(reader.GetStream());

    protected override Size BindValue(in BindContext context, Stream value, ref object? writeState)
    {
        if (value.CanSeek)
            return checked((int)(value.Length - value.Position));

        var memoryStream = new MemoryStream();
        writeState = memoryStream;
        value.CopyTo(memoryStream);
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
