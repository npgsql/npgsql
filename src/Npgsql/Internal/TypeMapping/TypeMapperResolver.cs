using System;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeMapping;

public abstract class TypeMapperResolver
{
    public abstract string? GetDataTypeNameByClrType(Type clrType);
    public virtual string? GetDataTypeNameByValueDependentValue(object value) => null;
    public abstract TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName);

    /// <summary>
    /// Gets type mapping information for a given PostgreSQL type.
    /// Invoked in scenarios when mapping information is required, rather than a type handler for reading or writing.
    /// </summary>
    public abstract TypeMappingInfo? GetMappingByPostgresType(TypeMapper typeMapper, PostgresType type);
}

static class TypeMapperResolverExtensions
{
    internal static TypeMappingInfo? GetMappingByClrType(this TypeMapperResolver typeMapper, Type clrType)
        => typeMapper.GetDataTypeNameByClrType(clrType) is { } dataTypeName ? typeMapper.GetMappingByDataTypeName(dataTypeName) : null;

    internal static TypeMappingInfo? GetMappingByValueDependentValue(this TypeMapperResolver typeMapper, object value)
        => typeMapper.GetDataTypeNameByValueDependentValue(value) is { } dataTypeName ? typeMapper.GetMappingByDataTypeName(dataTypeName) : null;
}