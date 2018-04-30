#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using GeoAPI.Geometries;
using GeoAPI.IO;
using NetTopologySuite.Geometries;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NetTopologySuite
{
    public class NetTopologySuiteHandlerFactory : NpgsqlTypeHandlerFactory<IGeometry>
    {
        readonly IBinaryGeometryReader _reader;
        readonly IBinaryGeometryWriter _writer;

        internal NetTopologySuiteHandlerFactory(IBinaryGeometryReader reader, IBinaryGeometryWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override NpgsqlTypeHandler<IGeometry> Create(NpgsqlConnection conn)
            => new NetTopologySuiteHandler(_reader, _writer);
    }

    class NetTopologySuiteHandler : NpgsqlTypeHandler<IGeometry>, INpgsqlTypeHandler<Geometry>,
        INpgsqlTypeHandler<IPoint>, INpgsqlTypeHandler<Point>,
        INpgsqlTypeHandler<ILineString>, INpgsqlTypeHandler<LineString>,
        INpgsqlTypeHandler<IPolygon>, INpgsqlTypeHandler<Polygon>,
        INpgsqlTypeHandler<IMultiPoint>, INpgsqlTypeHandler<MultiPoint>,
        INpgsqlTypeHandler<IMultiLineString>, INpgsqlTypeHandler<MultiLineString>,
        INpgsqlTypeHandler<IMultiPolygon>, INpgsqlTypeHandler<MultiPolygon>,
        INpgsqlTypeHandler<IGeometryCollection>, INpgsqlTypeHandler<GeometryCollection>
    {
        readonly IBinaryGeometryReader _reader;
        readonly IBinaryGeometryWriter _writer;
        LengthStream _lengthStream;

        internal NetTopologySuiteHandler(IBinaryGeometryReader reader, IBinaryGeometryWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        #region Read

        public override ValueTask<IGeometry> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => ReadCore<IGeometry>(buf, len);

        ValueTask<Geometry> INpgsqlTypeHandler<Geometry>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<Geometry>(buf, len);

        ValueTask<IPoint> INpgsqlTypeHandler<IPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<IPoint>(buf, len);

        ValueTask<Point> INpgsqlTypeHandler<Point>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<Point>(buf, len);

        ValueTask<ILineString> INpgsqlTypeHandler<ILineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<ILineString>(buf, len);

        ValueTask<LineString> INpgsqlTypeHandler<LineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<LineString>(buf, len);

        ValueTask<IPolygon> INpgsqlTypeHandler<IPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<IPolygon>(buf, len);

        ValueTask<Polygon> INpgsqlTypeHandler<Polygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<Polygon>(buf, len);

        ValueTask<IMultiPoint> INpgsqlTypeHandler<IMultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<IMultiPoint>(buf, len);

        ValueTask<MultiPoint> INpgsqlTypeHandler<MultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<MultiPoint>(buf, len);

        ValueTask<IMultiLineString> INpgsqlTypeHandler<IMultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<IMultiLineString>(buf, len);

        ValueTask<MultiLineString> INpgsqlTypeHandler<MultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<MultiLineString>(buf, len);

        ValueTask<IMultiPolygon> INpgsqlTypeHandler<IMultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<IMultiPolygon>(buf, len);

        ValueTask<MultiPolygon> INpgsqlTypeHandler<MultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<MultiPolygon>(buf, len);

        ValueTask<IGeometryCollection> INpgsqlTypeHandler<IGeometryCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<IGeometryCollection>(buf, len);

        ValueTask<GeometryCollection> INpgsqlTypeHandler<GeometryCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => ReadCore<GeometryCollection>(buf, len);

        ValueTask<T> ReadCore<T>(NpgsqlReadBuffer buf, int len)
            where T : IGeometry
            => new ValueTask<T>((T)_reader.Read(buf.GetStream(len, false)));

        #endregion

        #region ValidateAndGetLength

        public override int ValidateAndGetLength(IGeometry value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLengthCore(value);

        int INpgsqlTypeHandler<Geometry>.ValidateAndGetLength(Geometry value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<IPoint>.ValidateAndGetLength(IPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<Point>.ValidateAndGetLength(Point value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<ILineString>.ValidateAndGetLength(ILineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<LineString>.ValidateAndGetLength(LineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<IPolygon>.ValidateAndGetLength(IPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<Polygon>.ValidateAndGetLength(Polygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<IMultiPoint>.ValidateAndGetLength(IMultiPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<MultiPoint>.ValidateAndGetLength(MultiPoint value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<IMultiLineString>.ValidateAndGetLength(IMultiLineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<MultiLineString>.ValidateAndGetLength(MultiLineString value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<IMultiPolygon>.ValidateAndGetLength(IMultiPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<MultiPolygon>.ValidateAndGetLength(MultiPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<IGeometryCollection>.ValidateAndGetLength(IGeometryCollection value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int INpgsqlTypeHandler<GeometryCollection>.ValidateAndGetLength(GeometryCollection value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int ValidateAndGetLengthCore(IGeometry value)
        {
            if (_lengthStream == null)
                _lengthStream = new LengthStream();
            else
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

        public override Task Write(IGeometry value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<Geometry>.Write(Geometry value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<IPoint>.Write(IPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<Point>.Write(Point value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<ILineString>.Write(ILineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<LineString>.Write(LineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<IPolygon>.Write(IPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<Polygon>.Write(Polygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<IMultiPoint>.Write(IMultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<MultiPoint>.Write(MultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<IMultiLineString>.Write(IMultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<MultiLineString>.Write(MultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<IMultiPolygon>.Write(IMultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<MultiPolygon>.Write(MultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<IGeometryCollection>.Write(IGeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task INpgsqlTypeHandler<GeometryCollection>.Write(GeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteCore(value, buf, lengthCache, parameter, async);

        Task WriteCore(IGeometry value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            _writer.Write(value, buf.GetStream());
#if NET45
            return Task.Delay(0);
#else
            return Task.CompletedTask;
#endif
        }

        #endregion
    }
}
