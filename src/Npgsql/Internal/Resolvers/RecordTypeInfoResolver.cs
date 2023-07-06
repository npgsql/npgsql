using System;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

sealed class RecordTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    public RecordTypeInfoResolver()
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
        mappings.AddType<object[]>(DataTypeNames.Record, static (options, mapping, _) => mapping.CreateInfo(options, new ObjectArrayRecordConverter(options), supportsWriting: false),
            mappings => mappings with { MatchRequirement = MatchRequirement.DataTypeName });

        // TODO: ValueTuple
        // TODO: Tuple
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // mappings.AddArrayType<object[]>((string)DataTypeNames.Record);
    }

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type != typeof(object) && dataTypeName == DataTypeNames.Record)
        {
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.RecordsNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRecords), typeof(TBuilder).Name));
        }
    }
}
