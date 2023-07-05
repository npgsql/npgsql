using System;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

sealed class GeometricTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    public GeometricTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection();
        AddInfos(Mappings);
        // TODO: Opt-in only
        AddArrayInfos(Mappings);
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings)
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
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddStructArrayType<NpgsqlPoint>((string)DataTypeNames.Point);
        mappings.AddStructArrayType<NpgsqlBox>((string)DataTypeNames.Box);
        mappings.AddStructArrayType<NpgsqlPolygon>((string)DataTypeNames.Polygon);
        mappings.AddStructArrayType<NpgsqlLine>((string)DataTypeNames.Line);
        mappings.AddStructArrayType<NpgsqlLSeg>((string)DataTypeNames.LSeg);
        mappings.AddStructArrayType<NpgsqlPath>((string)DataTypeNames.Path);
        mappings.AddStructArrayType<NpgsqlCircle>((string)DataTypeNames.Circle);
    }
}
