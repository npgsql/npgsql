using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Resolvers;

[RequiresUnreferencedCode("Tupled record resolver may perform reflection on trimmed tuple types.")]
[RequiresDynamicCode("Tupled records need to construct a generic converter for a statically unknown (value)tuple type.")]
class TupledRecordTypeInfoResolver : IPgTypeInfoResolver
{
    protected TypeInfoMappingCollection Mappings { get; } = new();
    public TupledRecordTypeInfoResolver() => AddInfos(Mappings);

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    // Stand-in type, type match predicate does the actual work.
    static void AddInfos(TypeInfoMappingCollection mappings)
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
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddArrayType<Tuple<object>>((string)DataTypeNames.Record);
        mappings.AddStructArrayType<ValueTuple<object>>((string)DataTypeNames.Record);
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

            var converterType = typeof(ObjectArrayRecordConverter<>).MakeGenericType(mapping.Type);
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
    public TupledRecordArrayTypeInfoResolver()
    {
        Mappings = new TypeInfoMappingCollection(base.Mappings.Items);
        AddArrayInfos(Mappings);
    }

    new TypeInfoMappingCollection Mappings { get; }

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
