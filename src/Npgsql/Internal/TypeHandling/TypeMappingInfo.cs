using System;
using System.Data;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandling;

public class TypeMappingInfo
{
    public TypeMappingInfo(NpgsqlDbType? npgsqlDbType, string? dataTypeName, Type clrType)
        => (NpgsqlDbType, DataTypeName, ClrTypes) = (npgsqlDbType, dataTypeName, new[] { clrType });

    public TypeMappingInfo(NpgsqlDbType? npgsqlDbType, string? dataTypeName, params Type[] clrTypes)
        => (NpgsqlDbType, DataTypeName, ClrTypes) = (npgsqlDbType, dataTypeName, clrTypes);

    public NpgsqlDbType? NpgsqlDbType { get; }
    // Note that we can't cache the result due to nullable's assignment not being thread safe
    public DbType DbType
        => NpgsqlDbType is null ? DbType.Object : GlobalTypeMapper.NpgsqlDbTypeToDbType(NpgsqlDbType.Value);
    public string? DataTypeName { get; }
    public Type[] ClrTypes { get; }
}
