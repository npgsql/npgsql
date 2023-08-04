using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

sealed class NetworkTypeInfoResolver : IPgTypeInfoResolver
{
    TypeInfoMappingCollection Mappings { get; }

    public NetworkTypeInfoResolver()
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
        // macaddr
        mappings.AddType<PhysicalAddress>(DataTypeNames.MacAddr,
            static (options, mapping, _) => mapping.CreateInfo(options, new MacaddrConverter()), isDefault: true);
        mappings.AddType<PhysicalAddress>(DataTypeNames.MacAddr8,
            static (options, mapping, _) => mapping.CreateInfo(options, new MacaddrConverter()),
            mapping => mapping with { MatchRequirement = MatchRequirement.DataTypeName });

        // inet
        // This is one of the rare mappings that force us to use reflection for a lack of any alternative.
        // There are certain IPAddress values like Loopback or Any that return a *private* derived type (see https://github.com/dotnet/runtime/issues/27870).
        // However we still need to be able to resolve an exactly typed converter for those values.
        // We do so by wrapping our converter in a casting converter constructed over the derived type.
        // Finally we add a custom predicate to be able to match any type which values are assignable to IPAddress.
        mappings.AddType<IPAddress>(DataTypeNames.Inet,
            [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "MakeGenericType is safe because the target will only ever be a reference type.")]
            static (options, resolvedMapping, _) =>
            {
                var derivedType = resolvedMapping.Type != typeof(IPAddress);
                PgConverter converter = new IPAddressConverter();
                if (derivedType)
                    // There is not much more we can do, the deriving type IPAdress+ReadOnlyIPAdress isn't public.
                    converter = (PgConverter)Activator.CreateInstance(typeof(CastingConverter<>).MakeGenericType(resolvedMapping.Type), converter)!;

                return resolvedMapping.CreateInfo(options, converter, supportsWriting: !derivedType);
            }, mapping => mapping with { MatchRequirement = MatchRequirement.Single, TypeMatchPredicate = type => typeof(IPAddress).IsAssignableFrom(type) });
        mappings.AddStructType<NpgsqlInet>(DataTypeNames.Inet,
            static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlInetConverter()));

        // cidr
        mappings.AddStructType<NpgsqlCidr>(DataTypeNames.Cidr,
            static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlCidrConverter()), isDefault: true);
    }

    static void AddArrayInfos(TypeInfoMappingCollection mappings)
    {
        // macaddr
        mappings.AddArrayType<PhysicalAddress>((string)DataTypeNames.MacAddr);
        mappings.AddArrayType<PhysicalAddress>((string)DataTypeNames.MacAddr8);

        // inet
        mappings.AddArrayType<IPAddress>((string)DataTypeNames.Inet);
        mappings.AddStructArrayType<NpgsqlInet>((string)DataTypeNames.Inet);

        // cidr
        mappings.AddStructArrayType<NpgsqlCidr>((string)DataTypeNames.Cidr);
    }
}
