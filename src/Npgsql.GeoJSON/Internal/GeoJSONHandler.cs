using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using GeoJSON.Net;
using GeoJSON.Net.CoordinateReferenceSystem;
using GeoJSON.Net.Geometry;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.GeoJSON.Internal;

sealed partial class GeoJsonHandler : NpgsqlTypeHandler<GeoJSONObject>,
    INpgsqlTypeHandler<Point>, INpgsqlTypeHandler<MultiPoint>,
    INpgsqlTypeHandler<Polygon>, INpgsqlTypeHandler<MultiPolygon>,
    INpgsqlTypeHandler<LineString>, INpgsqlTypeHandler<MultiLineString>,
    INpgsqlTypeHandler<GeometryCollection>,
    INpgsqlTypeHandler<IGeoJSONObject>,
    INpgsqlTypeHandler<IGeometryObject>
{
    readonly GeoJSONOptions _options;
    readonly CrsMap _crsMap;
    readonly ConcurrentDictionary<int, NamedCRS> _cachedCrs = new();

    internal GeoJsonHandler(PostgresType postgresType, GeoJSONOptions options, CrsMap crsMap)
        : base(postgresType)
    {
        _options = options;
        _crsMap = crsMap;
    }

    GeoJSONOptions CrsType => _options & (GeoJSONOptions.ShortCRS | GeoJSONOptions.LongCRS);

    bool BoundingBox => (_options & GeoJSONOptions.BoundingBox) != 0;

    static bool HasSrid(EwkbGeometryType type)
        => (type & EwkbGeometryType.HasSrid) != 0;

    static bool HasZ(EwkbGeometryType type)
        => (type & EwkbGeometryType.HasZ) != 0;

    static bool HasM(EwkbGeometryType type)
        => (type & EwkbGeometryType.HasM) != 0;

    static bool HasZ(IPosition coordinates)
        => coordinates.Altitude.HasValue;

    const int SizeOfLength = sizeof(int);
    const int SizeOfHeader = sizeof(byte) + sizeof(EwkbGeometryType);
    const int SizeOfHeaderWithLength = SizeOfHeader + SizeOfLength;
    const int SizeOfPoint2D = 2 * sizeof(double);
    const int SizeOfPoint3D = 3 * sizeof(double);

    static int SizeOfPoint(bool hasZ)
        => hasZ ? SizeOfPoint3D : SizeOfPoint2D;

    static int SizeOfPoint(EwkbGeometryType type)
    {
        var size = SizeOfPoint2D;
        if (HasZ(type))
            size += sizeof(double);
        if (HasM(type))
            size += sizeof(double);
        return size;
    }

    #region Throw

    static Exception UnknownPostGisType()
        => throw new InvalidOperationException("Invalid PostGIS type");

    static Exception AllOrNoneCoordiantesMustHaveZ(NpgsqlParameter? parameter, string typeName)
        => parameter is null
            ? new ArgumentException($"The Z coordinate must be specified for all or none elements of {typeName}")
            : new ArgumentException($"The Z coordinate must be specified for all or none elements of {typeName} in the {parameter.ParameterName} parameter", parameter.ParameterName);

    #endregion

    #region Read

    public override ValueTask<GeoJSONObject> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        => ReadGeometry(buf, async);

    async ValueTask<Point> INpgsqlTypeHandler<Point>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (Point)await ReadGeometry(buf, async);

    async ValueTask<LineString> INpgsqlTypeHandler<LineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (LineString)await ReadGeometry(buf, async);

    async ValueTask<Polygon> INpgsqlTypeHandler<Polygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (Polygon)await ReadGeometry(buf, async);

    async ValueTask<MultiPoint> INpgsqlTypeHandler<MultiPoint>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (MultiPoint)await ReadGeometry(buf, async);

    async ValueTask<MultiLineString> INpgsqlTypeHandler<MultiLineString>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (MultiLineString)await ReadGeometry(buf, async);

    async ValueTask<MultiPolygon> INpgsqlTypeHandler<MultiPolygon>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (MultiPolygon)await ReadGeometry(buf, async);

    async ValueTask<GeometryCollection> INpgsqlTypeHandler<GeometryCollection>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (GeometryCollection)await ReadGeometry(buf, async);

    async ValueTask<IGeoJSONObject> INpgsqlTypeHandler<IGeoJSONObject>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => await ReadGeometry(buf, async);

    async ValueTask<IGeometryObject> INpgsqlTypeHandler<IGeometryObject>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => (IGeometryObject)await ReadGeometry(buf, async);

    async ValueTask<GeoJSONObject> ReadGeometry(NpgsqlReadBuffer buf, bool async)
    {
        var boundingBox = BoundingBox ? new BoundingBoxBuilder() : null;
        var geometry = await ReadGeometryCore(buf, async, boundingBox);

        geometry.BoundingBoxes = boundingBox?.Build();
        return geometry;
    }

    async ValueTask<GeoJSONObject> ReadGeometryCore(NpgsqlReadBuffer buf, bool async, BoundingBoxBuilder? boundingBox)
    {
        await buf.Ensure(SizeOfHeader, async);
        var littleEndian = buf.ReadByte() > 0;
        var type = (EwkbGeometryType)buf.ReadUInt32(littleEndian);

        GeoJSONObject geometry;
        NamedCRS? crs = null;

        if (HasSrid(type))
        {
            await buf.Ensure(4, async);
            crs = GetCrs(buf.ReadInt32(littleEndian));
        }

        switch (type & EwkbGeometryType.BaseType)
        {
        case EwkbGeometryType.Point:
        {
            await buf.Ensure(SizeOfPoint(type), async);
            var position = ReadPosition(buf, type, littleEndian);
            boundingBox?.Accumulate(position);
            geometry = new Point(position);
            break;
        }

        case EwkbGeometryType.LineString:
        {
            await buf.Ensure(SizeOfLength, async);
            var coordinates = new Position[buf.ReadInt32(littleEndian)];
            for (var i = 0; i < coordinates.Length; ++i)
            {
                await buf.Ensure(SizeOfPoint(type), async);
                var position = ReadPosition(buf, type, littleEndian);
                boundingBox?.Accumulate(position);
                coordinates[i] = position;
            }
            geometry = new LineString(coordinates);
            break;
        }

        case EwkbGeometryType.Polygon:
        {
            await buf.Ensure(SizeOfLength, async);
            var lines = new LineString[buf.ReadInt32(littleEndian)];
            for (var i = 0; i < lines.Length; ++i)
            {
                await buf.Ensure(SizeOfLength, async);
                var coordinates = new Position[buf.ReadInt32(littleEndian)];
                for (var j = 0; j < coordinates.Length; ++j)
                {
                    await buf.Ensure(SizeOfPoint(type), async);
                    var position = ReadPosition(buf, type, littleEndian);
                    boundingBox?.Accumulate(position);
                    coordinates[j] = position;
                }
                lines[i] = new LineString(coordinates);
            }
            geometry = new Polygon(lines);
            break;
        }

        case EwkbGeometryType.MultiPoint:
        {
            await buf.Ensure(SizeOfLength, async);
            var points = new Point[buf.ReadInt32(littleEndian)];
            for (var i = 0; i < points.Length; ++i)
            {
                await buf.Ensure(SizeOfHeader + SizeOfPoint(type), async);
                await buf.Skip(SizeOfHeader, async);
                var position = ReadPosition(buf, type, littleEndian);
                boundingBox?.Accumulate(position);
                points[i] = new Point(position);
            }
            geometry = new MultiPoint(points);
            break;
        }

        case EwkbGeometryType.MultiLineString:
        {
            await buf.Ensure(SizeOfLength, async);
            var lines = new LineString[buf.ReadInt32(littleEndian)];
            for (var i = 0; i < lines.Length; ++i)
            {
                await buf.Ensure(SizeOfHeaderWithLength, async);
                await buf.Skip(SizeOfHeader, async);
                var coordinates = new Position[buf.ReadInt32(littleEndian)];
                for (var j = 0; j < coordinates.Length; ++j)
                {
                    await buf.Ensure(SizeOfPoint(type), async);
                    var position = ReadPosition(buf, type, littleEndian);
                    boundingBox?.Accumulate(position);
                    coordinates[j] = position;
                }
                lines[i] = new LineString(coordinates);
            }
            geometry = new MultiLineString(lines);
            break;
        }

        case EwkbGeometryType.MultiPolygon:
        {
            await buf.Ensure(SizeOfLength, async);
            var polygons = new Polygon[buf.ReadInt32(littleEndian)];
            for (var i = 0; i < polygons.Length; ++i)
            {
                await buf.Ensure(SizeOfHeaderWithLength, async);
                await buf.Skip(SizeOfHeader, async);
                var lines = new LineString[buf.ReadInt32(littleEndian)];
                for (var j = 0; j < lines.Length; ++j)
                {
                    await buf.Ensure(SizeOfLength, async);
                    var coordinates = new Position[buf.ReadInt32(littleEndian)];
                    for (var k = 0; k < coordinates.Length; ++k)
                    {
                        await buf.Ensure(SizeOfPoint(type), async);
                        var position = ReadPosition(buf, type, littleEndian);
                        boundingBox?.Accumulate(position);
                        coordinates[k] = position;
                    }
                    lines[j] = new LineString(coordinates);
                }
                polygons[i] = new Polygon(lines);
            }
            geometry = new MultiPolygon(polygons);
            break;
        }

        case EwkbGeometryType.GeometryCollection:
        {
            await buf.Ensure(SizeOfLength, async);
            var elements = new IGeometryObject[buf.ReadInt32(littleEndian)];
            for (var i = 0; i < elements.Length; ++i)
                elements[i] = (IGeometryObject)await ReadGeometryCore(buf, async, boundingBox);
            geometry = new GeometryCollection(elements);
            break;
        }

        default:
            throw UnknownPostGisType();
        }

        geometry.CRS = crs;
        return geometry;
    }

    static Position ReadPosition(NpgsqlReadBuffer buf, EwkbGeometryType type, bool littleEndian)
    {
        var position = new Position(
            longitude: buf.ReadDouble(littleEndian),
            latitude: buf.ReadDouble(littleEndian),
            altitude: HasZ(type) ? buf.ReadDouble() : (double?)null);
        if (HasM(type)) buf.ReadDouble(littleEndian);
        return position;
    }

    #endregion

    #region Write

    public override int ValidateAndGetLength(GeoJSONObject value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => value.Type switch
        {
            GeoJSONObjectType.Point              => ValidateAndGetLength((Point)value, ref lengthCache, parameter),
            GeoJSONObjectType.LineString         => ValidateAndGetLength((LineString)value, ref lengthCache, parameter),
            GeoJSONObjectType.Polygon            => ValidateAndGetLength((Polygon)value, ref lengthCache, parameter),
            GeoJSONObjectType.MultiPoint         => ValidateAndGetLength((MultiPoint)value, ref lengthCache, parameter),
            GeoJSONObjectType.MultiLineString    => ValidateAndGetLength((MultiLineString)value, ref lengthCache, parameter),
            GeoJSONObjectType.MultiPolygon       => ValidateAndGetLength((MultiPolygon)value, ref lengthCache, parameter),
            GeoJSONObjectType.GeometryCollection => ValidateAndGetLength((GeometryCollection)value, ref lengthCache, parameter),
            _                                    => throw UnknownPostGisType()
        };

    public int ValidateAndGetLength(Point value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var length = SizeOfHeader + SizeOfPoint(HasZ(value.Coordinates));
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        return length;
    }

    public int ValidateAndGetLength(LineString value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var coordinates = value.Coordinates;
        if (NotValid(coordinates, out var hasZ))
            throw AllOrNoneCoordiantesMustHaveZ(parameter, nameof(LineString));

        var length = SizeOfHeaderWithLength + coordinates.Count * SizeOfPoint(hasZ);
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        return length;
    }

    public int ValidateAndGetLength(Polygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var lines = value.Coordinates;
        var length = SizeOfHeaderWithLength + SizeOfLength * lines.Count;
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        var hasZ = false;
        for (var i = 0; i < lines.Count; ++i)
        {
            var coordinates = lines[i].Coordinates;
            if (NotValid(coordinates, out var lineHasZ))
                throw AllOrNoneCoordiantesMustHaveZ(parameter, nameof(Polygon));

            if (hasZ != lineHasZ)
            {
                if (i == 0) hasZ = lineHasZ;
                else throw AllOrNoneCoordiantesMustHaveZ(parameter, nameof(LineString));
            }

            length += coordinates.Count * SizeOfPoint(hasZ);
        }

        return length;
    }

    static bool NotValid(ReadOnlyCollection<IPosition> coordinates, out bool hasZ)
    {
        if (coordinates.Count == 0)
            hasZ = false;
        else
        {
            hasZ = HasZ(coordinates[0]);
            for (var i = 1; i < coordinates.Count; ++i)
                if (HasZ(coordinates[i]) != hasZ) return true;
        }
        return false;
    }

    public int ValidateAndGetLength(MultiPoint value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var length = SizeOfHeaderWithLength;
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        var coordinates = value.Coordinates;
        for (var i = 0; i < coordinates.Count; ++i)
            length += ValidateAndGetLength(coordinates[i], ref lengthCache, parameter);

        return length;
    }

    public int ValidateAndGetLength(MultiLineString value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var length = SizeOfHeaderWithLength;
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        var coordinates = value.Coordinates;
        for (var i = 0; i < coordinates.Count; ++i)
            length += ValidateAndGetLength(coordinates[i], ref lengthCache, parameter);

        return length;
    }

    public int ValidateAndGetLength(MultiPolygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var length = SizeOfHeaderWithLength;
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        var coordinates = value.Coordinates;
        for (var i = 0; i < coordinates.Count; ++i)
            length += ValidateAndGetLength(coordinates[i], ref lengthCache, parameter);

        return length;
    }

    public int ValidateAndGetLength(GeometryCollection value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        var length = SizeOfHeaderWithLength;
        if (GetSrid(value.CRS) != 0)
            length += sizeof(int);

        var geometries = value.Geometries;
        for (var i = 0; i < geometries.Count; ++i)
            length += ValidateAndGetLength((GeoJSONObject)geometries[i], ref lengthCache, parameter);

        return length;
    }

    int INpgsqlTypeHandler<IGeoJSONObject>.ValidateAndGetLength(IGeoJSONObject value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength((GeoJSONObject)value, ref lengthCache, parameter);

    int INpgsqlTypeHandler<IGeometryObject>.ValidateAndGetLength(IGeometryObject value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => ValidateAndGetLength((GeoJSONObject)value, ref lengthCache, parameter);

    public override Task Write(GeoJSONObject value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value.Type switch
        {
            GeoJSONObjectType.Point              => Write((Point)value, buf, lengthCache, parameter, async, cancellationToken),
            GeoJSONObjectType.LineString         => Write((LineString)value, buf, lengthCache, parameter, async, cancellationToken),
            GeoJSONObjectType.Polygon            => Write((Polygon)value, buf, lengthCache, parameter, async, cancellationToken),
            GeoJSONObjectType.MultiPoint         => Write((MultiPoint)value, buf, lengthCache, parameter, async, cancellationToken),
            GeoJSONObjectType.MultiLineString    => Write((MultiLineString)value, buf, lengthCache, parameter, async, cancellationToken),
            GeoJSONObjectType.MultiPolygon       => Write((MultiPolygon)value, buf, lengthCache, parameter, async, cancellationToken),
            GeoJSONObjectType.GeometryCollection => Write((GeometryCollection)value, buf, lengthCache, parameter, async, cancellationToken),
            _                                    => throw UnknownPostGisType()
        };

    public async Task Write(Point value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.Point;
        var size = SizeOfHeader;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);

        if (srid != 0)
            buf.WriteInt32(srid);

        await WritePosition(value.Coordinates, buf, async, cancellationToken);
    }

    public async Task Write(LineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.LineString;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        var coordinates = value.Coordinates;

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);
        buf.WriteInt32(coordinates.Count);

        if (srid != 0)
            buf.WriteInt32(srid);

        for (var i = 0; i < coordinates.Count; ++i)
            await WritePosition(coordinates[i], buf, async, cancellationToken);
    }

    public async Task Write(Polygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.Polygon;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        var lines = value.Coordinates;

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);
        buf.WriteInt32(lines.Count);

        if (srid != 0)
            buf.WriteInt32(srid);

        for (var i = 0; i < lines.Count; ++i)
        {
            if (buf.WriteSpaceLeft < SizeOfLength)
                await buf.Flush(async, cancellationToken);
            var coordinates = lines[i].Coordinates;
            buf.WriteInt32(coordinates.Count);
            for (var j = 0; j < coordinates.Count; ++j)
                await WritePosition(coordinates[j], buf, async, cancellationToken);
        }
    }

    public async Task Write(MultiPoint value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.MultiPoint;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        var coordinates = value.Coordinates;

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);
        buf.WriteInt32(coordinates.Count);

        if (srid != 0)
            buf.WriteInt32(srid);

        for (var i = 0; i < coordinates.Count; ++i)
            await Write(coordinates[i], buf, lengthCache, parameter, async, cancellationToken);
    }

    public async Task Write(MultiLineString value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.MultiLineString;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        var coordinates = value.Coordinates;

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);
        buf.WriteInt32(coordinates.Count);

        if (srid != 0)
            buf.WriteInt32(srid);

        for (var i = 0; i < coordinates.Count; ++i)
            await Write(coordinates[i], buf, lengthCache, parameter, async, cancellationToken);
    }

    public async Task Write(MultiPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.MultiPolygon;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        var coordinates = value.Coordinates;

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);
        buf.WriteInt32(coordinates.Count);

        if (srid != 0)
            buf.WriteInt32(srid);
        for (var i = 0; i < coordinates.Count; ++i)
            await Write(coordinates[i], buf, lengthCache, parameter, async, cancellationToken);
    }

    public async Task Write(GeometryCollection value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var type = EwkbGeometryType.GeometryCollection;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (buf.WriteSpaceLeft < size)
            await buf.Flush(async, cancellationToken);

        var geometries = value.Geometries;

        buf.WriteByte(0); // Most significant byte first
        buf.WriteInt32((int)type);
        buf.WriteInt32(geometries.Count);

        if (srid != 0)
            buf.WriteInt32(srid);

        for (var i = 0; i < geometries.Count; ++i)
            await Write((GeoJSONObject) geometries[i], buf, lengthCache, parameter, async, cancellationToken);
    }

    Task INpgsqlTypeHandler<IGeoJSONObject>.Write(IGeoJSONObject value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
        => Write((GeoJSONObject)value, buf, lengthCache, parameter, async, cancellationToken);

    Task INpgsqlTypeHandler<IGeometryObject>.Write(IGeometryObject value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
        => Write((GeoJSONObject)value, buf, lengthCache, parameter, async, cancellationToken);

    static async Task WritePosition(IPosition coordinate, NpgsqlWriteBuffer buf, bool async, CancellationToken cancellationToken = default)
    {
        var altitude = coordinate.Altitude;
        if (buf.WriteSpaceLeft < SizeOfPoint(altitude.HasValue))
            await buf.Flush(async, cancellationToken);
        buf.WriteDouble(coordinate.Longitude);
        buf.WriteDouble(coordinate.Latitude);
        if (altitude.HasValue)
            buf.WriteDouble(altitude.Value);
    }

    #endregion

    #region Crs

    NamedCRS? GetCrs(int srid)
    {
        var crsType = CrsType;
        if (crsType == GeoJSONOptions.None)
            return null;

#if NETSTANDARD2_0
        return _cachedCrs.GetOrAdd(srid, srid =>
        {
            var authority = _crsMap.GetAuthority(srid);

            return authority is null
                ? throw new InvalidOperationException($"SRID {srid} unknown in spatial_ref_sys table")
                : new NamedCRS(crsType == GeoJSONOptions.LongCRS
                    ? "urn:ogc:def:crs:" + authority + "::" + srid
                    : authority + ":" + srid);
        });
#else
        return _cachedCrs.GetOrAdd(srid, static (srid, me) =>
        {
            var authority = me._crsMap.GetAuthority(srid);

            return authority is null
                ? throw new InvalidOperationException($"SRID {srid} unknown in spatial_ref_sys table")
                : new NamedCRS(me.CrsType == GeoJSONOptions.LongCRS
                    ? "urn:ogc:def:crs:" + authority + "::" + srid
                    : authority + ":" + srid);
        }, this);
#endif
    }

    static int GetSrid(ICRSObject crs)
    {
        if (crs == null || crs is UnspecifiedCRS)
            return 0;

        var namedCrs = crs as NamedCRS;
        if (namedCrs == null)
            throw new NotSupportedException("The LinkedCRS class isn't supported");

        if (namedCrs.Properties.TryGetValue("name", out var value) && value != null)
        {
            var name = value.ToString()!;
            if (string.Equals(name, "urn:ogc:def:crs:OGC::CRS84", StringComparison.Ordinal))
                return 4326;

            var index = name.LastIndexOf(':');
            if (index != -1 && int.TryParse(name.Substring(index + 1), out var srid))
                return srid;

            throw new FormatException("The specified CRS isn't properly named");
        }

        return 0;
    }

    #endregion
}

/// <summary>
/// Represents the identifier of the Well Known Binary representation of a geographical feature specified by the OGC.
/// http://portal.opengeospatial.org/files/?artifact_id=13227 Chapter 6.3.2.7
/// </summary>
[Flags]
enum EwkbGeometryType : uint
{
    // Types
    Point = 1,
    LineString = 2,
    Polygon = 3,
    MultiPoint = 4,
    MultiLineString = 5,
    MultiPolygon = 6,
    GeometryCollection = 7,

    // Masks
    BaseType = Point | LineString | Polygon | MultiPoint | MultiLineString | MultiPolygon | GeometryCollection,

    // Flags
    HasSrid = 0x20000000,
    HasM = 0x40000000,
    HasZ = 0x80000000
}
