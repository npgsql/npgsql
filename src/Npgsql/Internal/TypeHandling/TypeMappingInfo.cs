using System;
using System.Data;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandling
{
    public class TypeMappingInfo
    {
        public TypeMappingInfo(NpgsqlDbType? npgsqlDbType, string? dataTypeName, Type clrType)
            => (NpgsqlDbType, DataTypeName, ClrTypes) = (npgsqlDbType, dataTypeName, new[] { clrType });

        public TypeMappingInfo(NpgsqlDbType? npgsqlDbType, string? dataTypeName, params Type[] clrTypes)
            => (NpgsqlDbType, DataTypeName, ClrTypes) = (npgsqlDbType, dataTypeName, clrTypes);

        public NpgsqlDbType? NpgsqlDbType { get; }
        DbType? dbType;
        public DbType DbType
            => dbType ??= NpgsqlDbType is null ? DbType.Object : GlobalTypeMapper.NpgsqlDbTypeToDbType(NpgsqlDbType.Value);
        public string? DataTypeName { get; }
        public Type[] ClrTypes { get; }

        internal void Reset() => dbType = null;
    }
}
