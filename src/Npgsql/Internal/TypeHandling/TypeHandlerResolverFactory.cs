using System;

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
