using System;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeMapping;

public abstract class TypeMappingResolver
{
    public abstract string? GetDataTypeNameByClrType(Type clrType);
    public virtual string? GetDataTypeNameByValueDependentValue(object value) => null;
    public abstract TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName);

    /// <summary>
    /// Gets type mapping information for a given PostgreSQL type.
    /// Invoked in scenarios when mapping information is required, rather than a type handler for reading or writing.
    /// </summary>
    public abstract TypeMappingInfo? GetMappingByPostgresType(TypeMapper typeMapper, PostgresType type);

    internal TypeMappingInfo? GetMappingByValueDependentValue(object value)
        => GetDataTypeNameByValueDependentValue(value) is { } dataTypeName ? GetMappingByDataTypeName(dataTypeName) : null;

    internal TypeMappingInfo? GetMappingByClrType(Type clrType)
        => GetDataTypeNameByClrType(clrType) is { } dataTypeName ? GetMappingByDataTypeName(dataTypeName) : null;
}