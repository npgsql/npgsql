using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

class RecordTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    protected TypeInfoMappingCollection Mappings => _mappings ??= AddInfos(new());

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddType<object[]>(DataTypeNames.Record, static (options, mapping, _) =>
                mapping.CreateInfo(options, new RecordConverter<object[]>(options), supportsWriting: false),
            MatchRequirement.DataTypeName);

        return mappings;
    }

    protected static TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddArrayType<object[]>(DataTypeNames.Record);

        return mappings;
    }

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type != typeof(object) && dataTypeName == DataTypeNames.Record)
        {
            throw new NotSupportedException(
                string.Format(
                    NpgsqlStrings.RecordsNotEnabled,
                    nameof(INpgsqlTypeMapperExtensions.EnableRecordsAsTuples),
                    typeof(TBuilder).Name,
                    nameof(NpgsqlSlimDataSourceBuilder.EnableRecords)));
        }
    }
}

sealed class RecordArrayTypeInfoResolver : RecordTypeInfoResolver, IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings));

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}

[RequiresUnreferencedCode("Tupled record resolver may perform reflection on trimmed tuple types.")]
[RequiresDynamicCode("Tupled records need to construct a generic converter for a statically unknown (value)tuple type.")]
class TupledRecordTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    protected TypeInfoMappingCollection Mappings => _mappings ??= AddInfos(new());

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    // Stand-in type, type match predicate does the actual work.
    static TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddType<Tuple<object>>(DataTypeNames.Record, Factory,
            mapping => mapping with
            {
                MatchRequirement = MatchRequirement.DataTypeName,
                TypeMatchPredicate = type => type is { IsConstructedGenericType: true, FullName: not null }
                                             && type.FullName.StartsWith("System.Tuple", StringComparison.Ordinal)
            });

        mappings.AddStructType<ValueTuple<object>>(DataTypeNames.Record, Factory,
                mapping => mapping with
                {
                    MatchRequirement = MatchRequirement.DataTypeName,
                    TypeMatchPredicate = type => type is { IsConstructedGenericType: true, FullName: not null }
                                                 && type.FullName.StartsWith("System.ValueTuple", StringComparison.Ordinal)
                });

        return mappings;
    }

    protected static TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddArrayType<Tuple<object>>(DataTypeNames.Record, suppressObjectMapping: true);
        mappings.AddStructArrayType<ValueTuple<object>>(DataTypeNames.Record, suppressObjectMapping: true);

        return mappings;
    }

    static readonly TypeInfoFactory Factory = static (options, mapping, _) =>
        {
            var constructors = mapping.Type.GetConstructors();
            ConstructorInfo? constructor = null;
            if (constructors.Length is 1)
                constructor = constructors[0];
            else
            {
                var args = mapping.Type.GenericTypeArguments.Length;
                foreach (var ctor in constructors)
                    if (ctor.GetParameters().Length == args)
                    {
                        constructor = ctor;
                        break;
                    }
            }

            if (constructor is null)
                throw new InvalidOperationException($"Couldn't find a suitable constructor for record type: {mapping.Type.FullName}");

            var factory = typeof(TupledRecordTypeInfoResolver).GetMethod(nameof(CreateFactory), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(mapping.Type)
                .Invoke(null, new object[] { constructor, constructor.GetParameters().Length });

            var converterType = typeof(RecordConverter<>).MakeGenericType(mapping.Type);
            var converter = (PgConverter)Activator.CreateInstance(converterType, options, factory)!;
            return mapping.CreateInfo(options, converter, supportsWriting: false);
        };

    static Func<object[], T> CreateFactory<T>(ConstructorInfo constructor, int constructorParameters) => array =>
    {
        if (array.Length != constructorParameters)
            throw new InvalidCastException($"Cannot read record type with {array.Length} fields as {typeof(T)}");
        return (T)constructor.Invoke(array);
    };
}

[RequiresUnreferencedCode("Tupled record resolver may perform reflection on trimmed tuple types.")]
[RequiresDynamicCode("Tupled records need to construct a generic converter for a statically unknown (value)tuple type.")]
sealed class TupledRecordArrayTypeInfoResolver : TupledRecordTypeInfoResolver, IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings));

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
