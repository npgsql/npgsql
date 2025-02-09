using System;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.Properties;

namespace Npgsql.Internal.ResolverFactories;

sealed class LTreeTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver();
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

    public static void ThrowIfUnsupported<TBuilder>(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (dataTypeName is { UnqualifiedNameSpan: "ltree" or "_ltree" or "lquery" or "_lquery" or "ltxtquery" or "_ltxtquery" })
            throw new NotSupportedException(
                string.Format(NpgsqlStrings.LTreeNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableLTree),
                    typeof(TBuilder).Name));
    }

    class Resolver : IPgTypeInfoResolver
    {
        const byte LTreeVersion = 1;
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            mappings.AddType<string>("ltree",
                static (options, mapping, _) => mapping.CreateInfo(options,
                    new VersionPrefixedTextConverter<string>(LTreeVersion, new StringTextConverter(options.TextEncoding))),
                MatchRequirement.DataTypeName);
            mappings.AddType<string>("lquery",
                static (options, mapping, _) => mapping.CreateInfo(options,
                    new VersionPrefixedTextConverter<string>(LTreeVersion, new StringTextConverter(options.TextEncoding))),
                MatchRequirement.DataTypeName);
            mappings.AddType<string>("ltxtquery",
                static (options, mapping, _) => mapping.CreateInfo(options,
                    new VersionPrefixedTextConverter<string>(LTreeVersion, new StringTextConverter(options.TextEncoding))),
                MatchRequirement.DataTypeName);

            return mappings;
        }
    }

    sealed class ArrayResolver : Resolver, IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            mappings.AddArrayType<string>("ltree");
            mappings.AddArrayType<string>("lquery");
            mappings.AddArrayType<string>("ltxtquery");

            return mappings;
        }
    }
}
