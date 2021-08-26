using System;
using System.Data;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandling
{
    public abstract class TypeHandlerResolverFactory
    {
        public abstract TypeHandlerResolver Create(NpgsqlConnector connector);

        public abstract string? GetDataTypeNameByClrType(Type clrType);
        public virtual string? GetDataTypeNameByValueDependentValue(object value) => null;
        public abstract TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName);
    }

    static class TypeHandlerResolverFactoryExtensions
    {
        internal static TypeMappingInfo? GetMappingByClrType(this TypeHandlerResolverFactory factory, Type clrType)
            => factory.GetDataTypeNameByClrType(clrType) is { } dataTypeName ? factory.GetMappingByDataTypeName(dataTypeName) : null;

        internal static TypeMappingInfo? GetMappingByValueDependentValue(this TypeHandlerResolverFactory factory, object value)
            => factory.GetDataTypeNameByValueDependentValue(value) is { } dataTypeName ? factory.GetMappingByDataTypeName(dataTypeName) : null;
    }
}

public class TypeMappingInfo
{
    public TypeMappingInfo(NpgsqlDbType? npgsqlDbType, DbType dbType, string? dataTypeName, Type clrType)
        => (NpgsqlDbType, DbType, DataTypeName, ClrTypes) = (npgsqlDbType, dbType, dataTypeName, new[] { clrType });

    public TypeMappingInfo(NpgsqlDbType? npgsqlDbType, DbType dbType, string? dataTypeName, params Type[] clrTypes)
        => (NpgsqlDbType, DbType, DataTypeName, ClrTypes) = (npgsqlDbType, dbType, dataTypeName, clrTypes);

    public NpgsqlDbType? NpgsqlDbType { get; }
    public DbType DbType { get; }
    public string? DataTypeName { get; }
    public Type[] ClrTypes { get; }
}
