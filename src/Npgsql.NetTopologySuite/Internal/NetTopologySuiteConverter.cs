using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal;

namespace Npgsql.NetTopologySuite.Internal;

sealed class NetTopologySuiteConverter<T> : PgStreamingConverter<T>
    where T : Geometry
{
    readonly PostGisReader _reader;
    readonly PostGisWriter _writer;

    internal NetTopologySuiteConverter(PostGisReader reader, PostGisWriter writer)
        => (_reader, _writer) = (reader, writer);

    public override T Read(PgReader reader)
        => (T)_reader.Read(reader.GetStream());

    // PostGisReader/PostGisWriter doesn't support async
    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(Read(reader));

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
    {
        var lengthStream = new LengthStream();
        lengthStream.SetLength(0);
        _writer.Write(value, lengthStream);
        return (int)lengthStream.Length;
    }

    public override void Write(PgWriter writer, T value)
        => _writer.Write(value, writer.GetStream(allowMixedIO: true));

    // PostGisReader/PostGisWriter doesn't support async
    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
    {
        Write(writer, value);
        return default;
    }

    sealed class LengthStream : Stream
    {
        long _length;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => _length;

        public override long Position
        {
            get => _length;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => _length = value;

        public override void Write(byte[] buffer, int offset, int count)
            => _length += count;
    }
}
