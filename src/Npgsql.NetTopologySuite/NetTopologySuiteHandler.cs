using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NetTopologySuite
{
    class NetTopologySuiteHandler : NpgsqlTypeHandler<Geometry>,
        INpgsqlTypeHandler<Point>,
        INpgsqlTypeHandler<LineString>,
        INpgsqlTypeHandler<Polygon>,
        INpgsqlTypeHandler<MultiPoint>,
        INpgsqlTypeHandler<MultiLineString>,
        INpgsqlTypeHandler<MultiPolygon>,
        INpgsqlTypeHandler<GeometryCollection>
    {
        readonly PostGisReader _reader;
        readonly PostGisWriter _writer;
        readonly LengthStream _lengthStream = new LengthStream();

        internal NetTopologySuiteHandler(PostgresType postgresType, PostGisReader reader, PostGisWriter writer)
            : base(postgresType)
        {
            _reader = reader;
            _writer = writer;
        }

        #region Read

        public override ValueTask<Geometry> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => ReadCore<Geometry>(buf, len);

        ValueTask<Point> INpgsqlTypeHandler<Point>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<Point>(buf, len);

        ValueTask<LineString> INpgsqlTypeHandler<LineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<LineString>(buf, len);

        ValueTask<Polygon> INpgsqlTypeHandler<Polygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<Polygon>(buf, len);

        ValueTask<MultiPoint> INpgsqlTypeHandler<MultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<MultiPoint>(buf, len);

        ValueTask<MultiLineString> INpgsqlTypeHandler<MultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<MultiLineString>(buf, len);

        ValueTask<MultiPolygon> INpgsqlTypeHandler<MultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<MultiPolygon>(buf, len);

        ValueTask<GeometryCollection> INpgsqlTypeHandler<GeometryCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => ReadCore<GeometryCollection>(buf, len);

        ValueTask<T> ReadCore<T>(NpgsqlReadBuffer buf, int len)
            where T : Geometry
            => new ValueTask<T>((T)_reader.Read(buf.GetStream(len, false)));

        #endregion

        #region ValidateAndGetLength

        public override int ValidateAndGetLength(Geometry value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLengthCore(value);

        int INpgsqlTypeHandler<Point>.ValidateAndGetLength(Point value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<LineString>.ValidateAndGetLength(LineString value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<Polygon>.ValidateAndGetLength(Polygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<MultiPoint>.ValidateAndGetLength(MultiPoint value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<MultiLineString>.ValidateAndGetLength(MultiLineString value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<MultiPolygon>.ValidateAndGetLength(MultiPolygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<GeometryCollection>.ValidateAndGetLength(GeometryCollection value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int ValidateAndGetLengthCore(Geometry value)
        {
            _lengthStream.SetLength(0);
            _writer.Write(value, _lengthStream);
            return (int)_lengthStream.Length;
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
            { }

            public override int Read(byte[] buffer, int offset, int count)
                => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin)
                => throw new NotSupportedException();

            public override void SetLength(long value)
                => _length = value;

            public override void Write(byte[] buffer, int offset, int count)
                => _length += count;
        }

        #endregion

        #region Write

        public override Task Write(Geometry value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<Point>.Write(Point value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<LineString>.Write(LineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<Polygon>.Write(Polygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<MultiPoint>.Write(MultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToke)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<MultiLineString>.Write(MultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<MultiPolygon>.Write(MultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteCore(value, buf);

        Task INpgsqlTypeHandler<GeometryCollection>.Write(GeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteCore(value, buf);

        Task WriteCore(Geometry value, NpgsqlWriteBuffer buf)
        {
            _writer.Write(value, buf.GetStream());
            return Task.CompletedTask;
        }

        #endregion
    }
}
