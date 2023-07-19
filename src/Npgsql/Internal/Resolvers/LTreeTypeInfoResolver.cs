using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

sealed class LTreeTypeInfoResolver : IPgTypeInfoResolver
{
    const byte LTreeVersion = 1;
    TypeInfoMappingCollection Mappings { get; }

    public LTreeTypeInfoResolver()
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
        mappings.AddType<string>("ltree",
            static (options, mapping, _) => mapping.CreateInfo(options, new LTreeConverter(LTreeVersion, options.TextEncoding)),
            isDefault: true);
        mappings.AddType<string>("lquery",
            static (options, mapping, _) => mapping.CreateInfo(options, new LTreeConverter(LTreeVersion, options.TextEncoding)),
            isDefault: true);
        mappings.AddType<string>("ltxtquery",
            static (options, mapping, _) => mapping.CreateInfo(options, new LTreeConverter(LTreeVersion, options.TextEncoding)),
            isDefault: true);
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        mappings.AddArrayType<string>("ltree");
        mappings.AddArrayType<string>("lquery");
        mappings.AddArrayType<string>("ltxtquery");
    }

    public static void CheckUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type != typeof(object) && dataTypeName is { UnqualifiedName: "ltree" or "lquery" or "ltxtquery" })
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.LTreeNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableLTree),
                    typeof(TBuilder).Name));
    }
}
