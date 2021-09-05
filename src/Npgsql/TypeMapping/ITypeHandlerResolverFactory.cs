using System;
using System.Data;
using Npgsql.Internal;
using NpgsqlTypes;

#pragma warning disable 1591
#pragma warning disable RS0016

namespace Npgsql.TypeMapping
{
    public interface ITypeHandlerResolverFactory
    {
        ITypeHandlerResolver Create(NpgsqlConnector connector);

        string? GetDataTypeNameByClrType(Type type);
        TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName);
    }

    static class TypeHandlerResolverFactoryExtensions
    {
        internal static TypeMappingInfo? GetMappingByClrType(this ITypeHandlerResolverFactory factory, Type clrType)
            => factory.GetDataTypeNameByClrType(clrType) is { } dataTypeName ? factory.GetMappingByDataTypeName(dataTypeName) : null;
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
