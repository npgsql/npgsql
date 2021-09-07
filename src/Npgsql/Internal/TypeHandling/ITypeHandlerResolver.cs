using System;

namespace Npgsql.Internal.TypeHandling
{
    /// <summary>
    /// An Npgsql resolver for type handlers. Typically used by plugins to alter how Npgsql reads and writes values to PostgreSQL.
    /// </summary>
    public interface ITypeHandlerResolver
    {
        /// <summary>
        /// Resolves a type handler given a PostgreSQL type name, corresponding to the typname column in the PostgreSQL pg_type catalog table.
        /// </summary>
        /// <remarks>See <see href="https://www.postgresql.org/docs/current/catalog-pg-type.html" />.</remarks>
        NpgsqlTypeHandler? ResolveByDataTypeName(string typeName);

        /// <summary>
        /// Resolves a type handler given a .NET CLR type.
        /// </summary>
        NpgsqlTypeHandler? ResolveByClrType(Type type);

        /// <summary>
        /// Gets type mapping information for a given PostgreSQL type.
        /// Invoked in scenarios when mapping information is required, rather than a type handler for reading or writing.
        /// </summary>
        TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName);
    }
}
