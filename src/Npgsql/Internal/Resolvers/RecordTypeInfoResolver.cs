using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

class RecordTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();
    public RecordTypeInfoResolver() => AddInfos(Mappings);

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static void AddInfos(TypeInfoMappingCollection mappings)
        => mappings.AddType<object[]>(DataTypeNames.Record, static (options, mapping, _) =>
                mapping.CreateInfo(options, new ObjectArrayRecordConverter<object[]>(options), supportsWriting: false),
            MatchRequirement.DataTypeName);

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
        => mappings.AddArrayType<object[]>((string)DataTypeNames.Record);

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type != typeof(object) && dataTypeName == DataTypeNames.Record)
        {
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RecordsNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRecords), typeof(TBuilder).Name));
        }
    }
}

sealed class RecordArrayTypeInfoResolver : RecordTypeInfoResolver, IPgTypeInfoResolver
{
    public RecordArrayTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings.Items);
        AddArrayInfos(Mappings);
    }

    new TypeInfoMappingCollection Mappings { get; }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
