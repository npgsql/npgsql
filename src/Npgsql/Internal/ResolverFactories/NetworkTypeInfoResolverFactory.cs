using System;
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
            // There are certain IPAddress values like Loopback or Any that return a *private* derived type (see https://github.com/dotnet/runtime/issues/27870).
            // However we still need to be able to resolve some typed converter for those values.
            // We do so by returning a boxing info when we deal with a derived type, as a result we don't need an exact typed converter.
            // For arrays users can't actually reference the private type so we'll only see some version of ArrayType<IPAddress>.
            // For reads we'll only see the public type so we never surface an InvalidCastException trying to cast IPAddress to ReadOnlyIPAddress.
            // Finally we add a custom predicate to be able to match any type which values are assignable to IPAddress.
            mappings.AddType<IPAddress>(DataTypeNames.Inet,
                static (options, mapping, _) => new PgTypeInfo(options, new IPAddressConverter(),
                    new DataTypeName(mapping.DataTypeName), unboxedType: mapping.Type == typeof(IPAddress) ? null : mapping.Type),
                mapping => mapping with
                {
                    MatchRequirement = MatchRequirement.Single,
                    TypeMatchPredicate = type => type is null || typeof(IPAddress).IsAssignableFrom(type)
                });
            mappings.AddStructType<NpgsqlInet>(DataTypeNames.Inet,
                static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlInetConverter()));

            // cidr
            mappings.AddStructType<IPNetwork>(DataTypeNames.Cidr,
                static (options, mapping, _) => mapping.CreateInfo(options, new IPNetworkConverter()), isDefault: true);

#pragma warning disable CS0618 // NpgsqlCidr is obsolete
            mappings.AddStructType<NpgsqlCidr>(DataTypeNames.Cidr,
                static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlCidrConverter()));
#pragma warning restore CS0618

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
            mappings.AddStructArrayType<IPNetwork>(DataTypeNames.Cidr);
#pragma warning disable CS0618 // NpgsqlCidr is obsolete
            mappings.AddStructArrayType<NpgsqlCidr>(DataTypeNames.Cidr);
#pragma warning restore CS0618

            return mappings;
        }
    }
}
