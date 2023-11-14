using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

sealed class NetworkTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new Resolver();
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver();

    class Resolver : IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        static TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            // macaddr
            mappings.AddType<PhysicalAddress>(DataTypeNames.MacAddr,
                static (options, mapping, _) => mapping.CreateInfo(options, new MacaddrConverter(macaddr8: false)), isDefault: true);
            mappings.AddType<PhysicalAddress>(DataTypeNames.MacAddr8,
                static (options, mapping, _) => mapping.CreateInfo(options, new MacaddrConverter(macaddr8: true)),
                mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

            // inet
            // This is one of the rare mappings that force us to use reflection for a lack of any alternative.
            // There are certain IPAddress values like Loopback or Any that return a *private* derived type (see https://github.com/dotnet/runtime/issues/27870).
            // However we still need to be able to resolve an exactly typed converter for those values.
            // We do so by wrapping our converter in a casting converter constructed over the derived type.
            // Finally we add a custom predicate to be able to match any type which values are assignable to IPAddress.
            mappings.AddType<IPAddress>(DataTypeNames.Inet,
                CreateInfo,
                mapping => mapping with
                {
                    MatchRequirement = MatchRequirement.Single,
                    TypeMatchPredicate = type => type is null || typeof(IPAddress).IsAssignableFrom(type)
                });
            mappings.AddStructType<NpgsqlInet>(DataTypeNames.Inet,
                static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlInetConverter()));

            // cidr
            mappings.AddStructType<NpgsqlCidr>(DataTypeNames.Cidr,
                static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlCidrConverter()), isDefault: true);

            // Code is split out to a local method as suppression attributes on lambdas aren't properly handled by the ILLink analyzer yet.
            [UnconditionalSuppressMessage("AotAnalysis", "IL3050",
                Justification = "MakeGenericType is safe because the target will only ever be a reference type.")]
            static PgTypeInfo CreateInfo(PgSerializerOptions options, TypeInfoMapping resolvedMapping, bool _)
            {
                var derivedType = resolvedMapping.Type != typeof(IPAddress);
                PgConverter converter = new IPAddressConverter();
                if (derivedType)
                    // There is not much more we can do, the deriving type IPAddress+ReadOnlyIPAddress isn't public.
                    converter = (PgConverter)Activator.CreateInstance(typeof(CastingConverter<>).MakeGenericType(resolvedMapping.Type),
                        converter)!;

                return resolvedMapping.CreateInfo(options, converter);
            }

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
            // macaddr
            mappings.AddArrayType<PhysicalAddress>(DataTypeNames.MacAddr);
            mappings.AddArrayType<PhysicalAddress>(DataTypeNames.MacAddr8);

            // inet
            mappings.AddArrayType<IPAddress>(DataTypeNames.Inet);
            mappings.AddStructArrayType<NpgsqlInet>(DataTypeNames.Inet);

            // cidr
            mappings.AddStructArrayType<NpgsqlCidr>(DataTypeNames.Cidr);

            return mappings;
        }
    }
}
