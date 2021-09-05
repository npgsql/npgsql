using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.TypeHandling;
using NpgsqlTypes;

#pragma warning disable 1591
#pragma warning disable RS0016

namespace Npgsql.TypeMapping
{
    public interface ITypeHandlerResolver
    {
        NpgsqlTypeHandler? ResolveByOID(uint oid);

        NpgsqlTypeHandler? ResolveByDataTypeName(string typeName);

        // TODO: Add generic GetByClrType with a default implementation that delegates to the non-generic version.
        // This way the built-in resolver can specialize for some types.
        NpgsqlTypeHandler? ResolveByClrType(Type type);

        string? GetDataTypeNameByOID(uint oid);

        TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName);
    }
}
