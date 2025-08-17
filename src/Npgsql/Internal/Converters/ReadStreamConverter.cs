using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

class ReadStreamConverter : PgStreamingConverter<Stream>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return format is DataFormat.Text or DataFormat.Binary;
    }

    public override Stream Read(PgReader reader) => reader.GetStream();
    public override ValueTask<Stream> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => new(reader.GetStream());
    public override Size GetSize(SizeContext context, Stream value, ref object? writeState) => throw new System.NotImplementedException();
    public override void Write(PgWriter writer, Stream value) => throw new System.NotImplementedException();
    public override ValueTask WriteAsync(PgWriter writer, Stream value, CancellationToken cancellationToken = default) => throw new System.NotImplementedException();
}
