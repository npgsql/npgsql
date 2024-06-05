using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GeoJSON.Net;
using GeoJSON.Net.CoordinateReferenceSystem;
using GeoJSON.Net.Geometry;
using Npgsql.Internal;

namespace Npgsql.GeoJSON.Internal;

sealed class GeoJSONConverter<T> : PgStreamingConverter<T> where T : IGeoJSONObject
{
    readonly ConcurrentDictionary<int, NamedCRS> _cachedCrs = new();
    readonly GeoJSONOptions _options;
    readonly Func<int, NamedCRS?> _getCrs;

    public GeoJSONConverter(GeoJSONOptions options, CrsMap crsMap)
    {
        _options = options;
        _getCrs = GetCrs(
            crsMap,
            _cachedCrs,
            crsType: _options & (GeoJSONOptions.ShortCRS | GeoJSONOptions.LongCRS)
        );
    }

    bool BoundingBox => (_options & GeoJSONOptions.BoundingBox) != 0;

    public override T Read(PgReader reader)
        => (T)GeoJSONConverter.Read(async: false, reader, BoundingBox ? new BoundingBoxBuilder() : null, _getCrs, CancellationToken.None).GetAwaiter().GetResult();

    public override async ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => (T)await GeoJSONConverter.Read(async: true, reader, BoundingBox ? new BoundingBoxBuilder() : null, _getCrs, cancellationToken).ConfigureAwait(false);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => GeoJSONConverter.GetSize(context, value, ref writeState);

    public override void Write(PgWriter writer, T value)
        => GeoJSONConverter.Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => GeoJSONConverter.Write(async: true, writer, value, CancellationToken.None);

    static Func<int, NamedCRS?> GetCrs(CrsMap crsMap, ConcurrentDictionary<int, NamedCRS> cachedCrs, GeoJSONOptions crsType)
        => srid =>
        {
            if (crsType == GeoJSONOptions.None)
                return null;

            return cachedCrs.GetOrAdd(srid, static (srid, state) =>
            {
                var (crsMap, crsType) = state;
                var authority = crsMap.GetAuthority(srid);

                return authority is null
                    ? throw new InvalidOperationException($"SRID {srid} unknown in spatial_ref_sys table")
                    : new NamedCRS(crsType == GeoJSONOptions.LongCRS
                        ? "urn:ogc:def:crs:" + authority + "::" + srid
                        : authority + ":" + srid);
            }, (crsMap, crsType));
        };
}

static class GeoJSONConverter
{
    public static async ValueTask<IGeoJSONObject> Read(bool async, PgReader reader, BoundingBoxBuilder? boundingBox, Func<int, NamedCRS?> getCrs, CancellationToken cancellationToken)
    {
        var geometry = await Core(async, reader, boundingBox, getCrs, cancellationToken).ConfigureAwait(false);
        geometry.BoundingBoxes = boundingBox?.Build();
        return geometry;

        static async ValueTask<GeoJSONObject> Core(bool async, PgReader reader, BoundingBoxBuilder? boundingbox, Func<int, NamedCRS?> getCrs, CancellationToken cancellationToken)
        {
            if (reader.ShouldBuffer(SizeOfHeader))
                await reader.BufferData(async, SizeOfHeader, cancellationToken).ConfigureAwait(false);

            var littleEndian = reader.ReadByte() > 0;
            var type = (EwkbGeometryType)ReadUInt32(littleEndian);

            GeoJSONObject geometry;
            NamedCRS? crs = null;

            if (HasSrid(type))
            {
                if (reader.ShouldBuffer(sizeof(int)))
                    await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
                crs = getCrs(ReadInt32(littleEndian));
            }

            switch (type & EwkbGeometryType.BaseType)
            {
            case EwkbGeometryType.Point:
            {
                if (SizeOfPoint(type) is var size && reader.ShouldBuffer(size))
                    await reader.BufferData(async, size, cancellationToken).ConfigureAwait(false);

                var position = ReadPosition(reader, type, littleEndian);
                boundingbox?.Accumulate(position);
                geometry = new Point(position);
                break;
            }

            case EwkbGeometryType.LineString:
            {
                if (reader.ShouldBuffer(SizeOfLength))
                    await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                var coordinates = new Position[ReadInt32(littleEndian)];
                for (var i = 0; i < coordinates.Length; ++i)
                {
                    if (SizeOfPoint(type) is var size && reader.ShouldBuffer(size))
                        await reader.BufferData(async, size, cancellationToken).ConfigureAwait(false);

                    var position = ReadPosition(reader, type, littleEndian);
                    boundingbox?.Accumulate(position);
                    coordinates[i] = position;
                }
                geometry = new LineString(coordinates);
                break;
            }

            case EwkbGeometryType.Polygon:
            {
                if (reader.ShouldBuffer(SizeOfLength))
                    await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                var lines = new LineString[ReadInt32(littleEndian)];
                for (var i = 0; i < lines.Length; ++i)
                {
                    if (reader.ShouldBuffer(SizeOfLength))
                        await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                    var coordinates = new Position[ReadInt32(littleEndian)];
                    for (var j = 0; j < coordinates.Length; ++j)
                    {
                        if (SizeOfPoint(type) is var size && reader.ShouldBuffer(size))
                            await reader.BufferData(async, size, cancellationToken).ConfigureAwait(false);

                        var position = ReadPosition(reader, type, littleEndian);
                        boundingbox?.Accumulate(position);
                        coordinates[j] = position;
                    }
                    lines[i] = new LineString(coordinates);
                }
                geometry = new Polygon(lines);
                break;
            }

            case EwkbGeometryType.MultiPoint:
            {
                if (reader.ShouldBuffer(SizeOfLength))
                    await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                var points = new Point[ReadInt32(littleEndian)];
                for (var i = 0; i < points.Length; ++i)
                {
                    if (SizeOfHeader + SizeOfPoint(type) is var size && reader.ShouldBuffer(size))
                        await reader.BufferData(async, size, cancellationToken).ConfigureAwait(false);

                    if (async)
                        await reader.ConsumeAsync(SizeOfHeader, cancellationToken).ConfigureAwait(false);
                    else
                        reader.Consume(SizeOfHeader);

                    var position = ReadPosition(reader, type, littleEndian);
                    boundingbox?.Accumulate(position);
                    points[i] = new Point(position);
                }
                geometry = new MultiPoint(points);
                break;
            }

            case EwkbGeometryType.MultiLineString:
            {
                if (reader.ShouldBuffer(SizeOfLength))
                    await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                var lines = new LineString[ReadInt32(littleEndian)];
                for (var i = 0; i < lines.Length; ++i)
                {
                    if (reader.ShouldBuffer(SizeOfHeaderWithLength))
                        await reader.BufferData(async, SizeOfHeaderWithLength, cancellationToken).ConfigureAwait(false);

                    if (async)
                        await reader.ConsumeAsync(SizeOfHeader, cancellationToken).ConfigureAwait(false);
                    else
                        reader.Consume(SizeOfHeader);

                    var coordinates = new Position[ReadInt32(littleEndian)];
                    for (var j = 0; j < coordinates.Length; ++j)
                    {
                        if (SizeOfPoint(type) is var size && reader.ShouldBuffer(size))
                            await reader.BufferData(async, size, cancellationToken).ConfigureAwait(false);
                        var position = ReadPosition(reader, type, littleEndian);
                        boundingbox?.Accumulate(position);
                        coordinates[j] = position;
                    }
                    lines[i] = new LineString(coordinates);
                }
                geometry = new MultiLineString(lines);
                break;
            }

            case EwkbGeometryType.MultiPolygon:
            {
                if (reader.ShouldBuffer(SizeOfLength))
                    await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                var polygons = new Polygon[ReadInt32(littleEndian)];
                for (var i = 0; i < polygons.Length; ++i)
                {
                    if (reader.ShouldBuffer(SizeOfHeaderWithLength))
                        await reader.BufferData(async, SizeOfHeaderWithLength, cancellationToken).ConfigureAwait(false);

                    if (async)
                        await reader.ConsumeAsync(SizeOfHeader, cancellationToken).ConfigureAwait(false);
                    else
                        reader.Consume(SizeOfHeader);

                    var lines = new LineString[ReadInt32(littleEndian)];
                    for (var j = 0; j < lines.Length; ++j)
                    {
                        if (reader.ShouldBuffer(SizeOfLength))
                            await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);
                        var coordinates = new Position[ReadInt32(littleEndian)];
                        for (var k = 0; k < coordinates.Length; ++k)
                        {
                            if (SizeOfPoint(type) is var size && reader.ShouldBuffer(size))
                                await reader.BufferData(async, size, cancellationToken).ConfigureAwait(false);
                            var position = ReadPosition(reader, type, littleEndian);
                            boundingbox?.Accumulate(position);
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
                if (reader.ShouldBuffer(SizeOfLength))
                    await reader.BufferData(async, SizeOfLength, cancellationToken).ConfigureAwait(false);

                var elements = new IGeometryObject[ReadInt32(littleEndian)];
                for (var i = 0; i < elements.Length; ++i)
                    elements[i] = (IGeometryObject)await Core(async, reader, boundingbox, getCrs, cancellationToken).ConfigureAwait(false);
                geometry = new GeometryCollection(elements);
                break;
            }

            default:
                throw UnknownPostGisType();
            }

            geometry.CRS = crs;
            return geometry;

            int ReadInt32(bool littleEndian)
                => littleEndian ? BinaryPrimitives.ReverseEndianness(reader.ReadInt32()) : reader.ReadInt32();
            uint ReadUInt32(bool littleEndian)
                => littleEndian ? BinaryPrimitives.ReverseEndianness(reader.ReadUInt32()) : reader.ReadUInt32();
        }

        static Position ReadPosition(PgReader reader, EwkbGeometryType type, bool littleEndian)
        {
            var position = new Position(
                longitude: ReadDouble(littleEndian),
                latitude: ReadDouble(littleEndian),
                altitude: HasZ(type) ? reader.ReadDouble() : null);
            if (HasM(type)) ReadDouble(littleEndian);
            return position;

            double ReadDouble(bool littleEndian)
                => littleEndian
                    ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(reader.ReadDouble())))
                    : reader.ReadDouble();
        }
    }

    public static Size GetSize(SizeContext context, IGeoJSONObject value, ref object? writeState)
        => value.Type switch
        {
            GeoJSONObjectType.Point => GetSize((Point)value),
            GeoJSONObjectType.LineString => GetSize((LineString)value),
            GeoJSONObjectType.Polygon => GetSize((Polygon)value),
            GeoJSONObjectType.MultiPoint => GetSize((MultiPoint)value),
            GeoJSONObjectType.MultiLineString => GetSize((MultiLineString)value),
            GeoJSONObjectType.MultiPolygon => GetSize((MultiPolygon)value),
            GeoJSONObjectType.GeometryCollection => GetSize(context, (GeometryCollection)value, ref writeState),
            _ => throw UnknownPostGisType()
        };

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

    static Size GetSize(Point value)
    {
        var length = Size.Create(SizeOfHeader + SizeOfPoint(HasZ(value.Coordinates)));
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        return length;
    }

    static Size GetSize(LineString value)
    {
        var coordinates = value.Coordinates;
        if (NotValid(coordinates, out var hasZ))
            throw AllOrNoneCoordinatesMustHaveZ(nameof(LineString));

        var length = Size.Create(SizeOfHeaderWithLength + coordinates.Count * SizeOfPoint(hasZ));
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        return length;
    }

    static Size GetSize(Polygon value)
    {
        var lines = value.Coordinates;
        var length = Size.Create(SizeOfHeaderWithLength + SizeOfLength * lines.Count);
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        var hasZ = false;
        for (var i = 0; i < lines.Count; ++i)
        {
            var coordinates = lines[i].Coordinates;
            if (NotValid(coordinates, out var lineHasZ))
                throw AllOrNoneCoordinatesMustHaveZ(nameof(Polygon));

            if (hasZ != lineHasZ)
            {
                if (i == 0) hasZ = lineHasZ;
                else throw AllOrNoneCoordinatesMustHaveZ(nameof(LineString));
            }

            length = length.Combine(coordinates.Count * SizeOfPoint(hasZ));
        }

        return length;
    }

    static Size GetSize(MultiPoint value)
    {
        var length = Size.Create(SizeOfHeaderWithLength);
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        var coordinates = value.Coordinates;
        foreach (var t in coordinates)
            length = length.Combine(GetSize(t));

        return length;
    }

    static Size GetSize(MultiLineString value)
    {
        var length = Size.Create(SizeOfHeaderWithLength);
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        var coordinates = value.Coordinates;
        foreach (var t in coordinates)
            length = length.Combine(GetSize(t));

        return length;
    }

    static Size GetSize(MultiPolygon value)
    {
        var length = Size.Create(SizeOfHeaderWithLength);
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        var coordinates = value.Coordinates;
        foreach (var t in coordinates)
            length = length.Combine(GetSize(t));

        return length;
    }

    static Size GetSize(SizeContext context, GeometryCollection value, ref object? writeState)
    {
        var length = Size.Create(SizeOfHeaderWithLength);
        if (GetSrid(value.CRS) != 0)
            length = length.Combine(sizeof(int));

        var geometries = value.Geometries;
        foreach (var t in geometries)
            length = length.Combine(GetSize(context, (IGeoJSONObject)t, ref writeState));

        return length;
    }

    public static ValueTask Write(bool async, PgWriter writer, IGeoJSONObject value, CancellationToken cancellationToken = default)
        => value.Type switch
        {
            GeoJSONObjectType.Point              => Write(async, writer, (Point)value, cancellationToken),
            GeoJSONObjectType.LineString         => Write(async, writer, (LineString)value, cancellationToken),
            GeoJSONObjectType.Polygon            => Write(async, writer, (Polygon)value, cancellationToken),
            GeoJSONObjectType.MultiPoint         => Write(async, writer, (MultiPoint)value, cancellationToken),
            GeoJSONObjectType.MultiLineString    => Write(async, writer, (MultiLineString)value, cancellationToken),
            GeoJSONObjectType.MultiPolygon       => Write(async, writer, (MultiPolygon)value, cancellationToken),
            GeoJSONObjectType.GeometryCollection => Write(async, writer, (GeometryCollection)value, cancellationToken),
            _                                    => throw UnknownPostGisType()
        };

    static async ValueTask Write(bool async, PgWriter writer, Point value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.Point;
        var size = SizeOfHeader;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);

        if (srid != 0)
            writer.WriteInt32(srid);

        await WritePosition(async, writer, value.Coordinates, cancellationToken).ConfigureAwait(false);
    }

    static async ValueTask Write(bool async, PgWriter writer, LineString value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.LineString;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        var coordinates = value.Coordinates;

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);
        writer.WriteInt32(coordinates.Count);

        if (srid != 0)
            writer.WriteInt32(srid);

        foreach (var t in coordinates)
            await WritePosition(async, writer, t, cancellationToken).ConfigureAwait(false);
    }

    static async ValueTask Write(bool async, PgWriter writer, Polygon value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.Polygon;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        var lines = value.Coordinates;

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);
        writer.WriteInt32(lines.Count);

        if (srid != 0)
            writer.WriteInt32(srid);

        foreach (var t in lines)
        {
            if (writer.ShouldFlush(SizeOfLength))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            var coordinates = t.Coordinates;
            writer.WriteInt32(coordinates.Count);
            foreach (var t1 in coordinates)
                await WritePosition(async, writer, t1, cancellationToken).ConfigureAwait(false);
        }
    }

    static async ValueTask Write(bool async, PgWriter writer, MultiPoint value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.MultiPoint;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        var coordinates = value.Coordinates;

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);
        writer.WriteInt32(coordinates.Count);

        if (srid != 0)
            writer.WriteInt32(srid);

        foreach (var t in coordinates)
            await Write(async, writer, t, cancellationToken).ConfigureAwait(false);
    }

    static async ValueTask Write(bool async, PgWriter writer, MultiLineString value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.MultiLineString;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        var coordinates = value.Coordinates;

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);
        writer.WriteInt32(coordinates.Count);

        if (srid != 0)
            writer.WriteInt32(srid);

        foreach (var t in coordinates)
            await Write(async, writer, t, cancellationToken).ConfigureAwait(false);
    }

    static async ValueTask Write(bool async, PgWriter writer, MultiPolygon value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.MultiPolygon;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        var coordinates = value.Coordinates;

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);
        writer.WriteInt32(coordinates.Count);

        if (srid != 0)
            writer.WriteInt32(srid);
        foreach (var t in coordinates)
            await Write(async, writer, t, cancellationToken).ConfigureAwait(false);
    }

    static async ValueTask Write(bool async, PgWriter writer, GeometryCollection value, CancellationToken cancellationToken)
    {
        var type = EwkbGeometryType.GeometryCollection;
        var size = SizeOfHeaderWithLength;
        var srid = GetSrid(value.CRS);
        if (srid != 0)
        {
            size += sizeof(int);
            type |= EwkbGeometryType.HasSrid;
        }

        if (writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        var geometries = value.Geometries;

        writer.WriteByte(0); // Most significant byte first
        writer.WriteInt32((int)type);
        writer.WriteInt32(geometries.Count);

        if (srid != 0)
            writer.WriteInt32(srid);

        foreach (var t in geometries)
            await Write(async, writer, (IGeoJSONObject)t, cancellationToken).ConfigureAwait(false);
    }

    static async ValueTask WritePosition(bool async, PgWriter writer, IPosition coordinate, CancellationToken cancellationToken)
    {
        var altitude = coordinate.Altitude;
        if (SizeOfPoint(altitude.HasValue) is var size && writer.ShouldFlush(size))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteDouble(coordinate.Longitude);
        writer.WriteDouble(coordinate.Latitude);
        if (altitude.HasValue)
            writer.WriteDouble(altitude.Value);
    }

    static ValueTask BufferData(this PgReader reader, bool async, int byteCount, CancellationToken cancellationToken)
    {
        if (async)
            return reader.BufferAsync(byteCount, cancellationToken);

        reader.Buffer(byteCount);
        return new();
    }

    static ValueTask Flush(this PgWriter writer, bool async, CancellationToken cancellationToken)
    {
        if (async)
            return writer.FlushAsync(cancellationToken);

        writer.Flush();
        return new();
    }

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

    static Exception UnknownPostGisType()
        => throw new InvalidOperationException("Invalid PostGIS type");

    static Exception AllOrNoneCoordinatesMustHaveZ(string typeName)
        => new ArgumentException($"The Z coordinate must be specified for all or none elements of {typeName}");

    static int GetSrid(ICRSObject crs)
    {
        if (crs is null or UnspecifiedCRS)
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
