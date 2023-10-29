using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

class LTreeTypeInfoResolver : IPgTypeInfoResolver
{
    const byte LTreeVersion = 1;
    TypeInfoMappingCollection? _mappings;
    protected TypeInfoMappingCollection Mappings => _mappings ??= AddInfos(new());

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);

    static TypeInfoMappingCollection AddInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddType<string>("ltree",
            static (options, mapping, _) => mapping.CreateInfo(options, new VersionPrefixedTextConverter<string>(LTreeVersion, new StringTextConverter(options.TextEncoding))),
            MatchRequirement.DataTypeName);
        mappings.AddType<string>("lquery",
            static (options, mapping, _) => mapping.CreateInfo(options, new VersionPrefixedTextConverter<string>(LTreeVersion, new StringTextConverter(options.TextEncoding))),
            MatchRequirement.DataTypeName);
        mappings.AddType<string>("ltxtquery",
            static (options, mapping, _) => mapping.CreateInfo(options, new VersionPrefixedTextConverter<string>(LTreeVersion, new StringTextConverter(options.TextEncoding))),
            MatchRequirement.DataTypeName);

        return mappings;
    }

    protected static TypeInfoMappingCollection AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddArrayType<string>("ltree");
        mappings.AddArrayType<string>("lquery");
        mappings.AddArrayType<string>("ltxtquery");

        return mappings;
    }

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type != typeof(object) && dataTypeName is { UnqualifiedName: "ltree" or "lquery" or "ltxtquery" })
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.LTreeNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableLTree),
                    typeof(TBuilder).Name));
    }
}

sealed class LTreeArrayTypeInfoResolver : LTreeTypeInfoResolver, IPgTypeInfoResolver
{
    TypeInfoMappingCollection? _mappings;
    new TypeInfoMappingCollection Mappings => _mappings ??= AddArrayInfos(new(base.Mappings));

    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
        => Mappings.Find(type, dataTypeName, options);
}
