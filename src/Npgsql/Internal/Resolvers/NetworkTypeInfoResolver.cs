using System;
using System.Net;
using System.Net.NetworkInformation;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping.Resolvers;

class NetworkTypeInfoResolver : IPgTypeInfoResolver
{
    public static NetworkTypeInfoResolver Instance { get; } = new();

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
            static (options, mapping, _) => mapping.CreateInfo(options, new MacaddrConverter()), isDefault: true);

        // inet
        mappings.AddType<IPAddress>(DataTypeNames.Inet,
            static (options, mapping, _) => mapping.CreateInfo(options, new IPAddressConverter()), isDefault: true);
        mappings.AddStructType<NpgsqlInet>(DataTypeNames.Inet,
            static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlInetConverter()));

        // cidr
        mappings.AddStructType<NpgsqlCidr>(DataTypeNames.Cidr,
            static (options, mapping, _) => mapping.CreateInfo(options, new NpgsqlCidrConverter()), isDefault: true);
    }

    protected static void AddArrayInfos(TypeInfoMappingCollection mappings)
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
