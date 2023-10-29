using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

class GeometricTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    protected TypeInfoMappingCollection Mappings => _mappings ??= AddInfos(new());

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddStructType<NpgsqlPoint>(DataTypeNames.Point,
            static (options, mapping, _) => mapping.CreateInfo(options, new PointConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlBox>(DataTypeNames.Box,
            static (options, mapping, _) => mapping.CreateInfo(options, new BoxConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlPolygon>(DataTypeNames.Polygon,
            static (options, mapping, _) => mapping.CreateInfo(options, new PolygonConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlLine>(DataTypeNames.Line,
            static (options, mapping, _) => mapping.CreateInfo(options, new LineConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlLSeg>(DataTypeNames.LSeg,
            static (options, mapping, _) => mapping.CreateInfo(options, new LineSegmentConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlPath>(DataTypeNames.Path,
            static (options, mapping, _) => mapping.CreateInfo(options, new PathConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlCircle>(DataTypeNames.Circle,
            static (options, mapping, _) => mapping.CreateInfo(options, new CircleConverter()), isDefault: true);

        return mappings;
    }

    protected static TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddStructArrayType<NpgsqlPoint>(DataTypeNames.Point);
        mappings.AddStructArrayType<NpgsqlBox>(DataTypeNames.Box);
        mappings.AddStructArrayType<NpgsqlPolygon>(DataTypeNames.Polygon);
        mappings.AddStructArrayType<NpgsqlLine>(DataTypeNames.Line);
        mappings.AddStructArrayType<NpgsqlLSeg>(DataTypeNames.LSeg);
        mappings.AddStructArrayType<NpgsqlPath>(DataTypeNames.Path);
        mappings.AddStructArrayType<NpgsqlCircle>(DataTypeNames.Circle);

        return mappings;
    }
}

sealed class GeometricArrayTypeInfoResolver : GeometricTypeInfoResolver, IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings));

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
