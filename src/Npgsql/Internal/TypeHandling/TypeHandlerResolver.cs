using System;
using NpgsqlTypes;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandling;

/// <summary>
/// An Npgsql resolver for type handlers. Typically used by plugins to alter how Npgsql reads and writes values to PostgreSQL.
/// </summary>
public abstract class TypeHandlerResolver
{
    /// <summary>
    /// Resolves a type handler given a PostgreSQL type name, corresponding to the typname column in the PostgreSQL pg_type catalog table.
    /// </summary>
    /// <remarks>See <see href="https://www.postgresql.org/docs/current/catalog-pg-type.html" />.</remarks>
    public abstract NpgsqlTypeHandler? ResolveByDataTypeName(string typeName);

    /// <summary>
    /// Resolves a type handler for a given NpgsqlDbType.
    /// </summary>
    public virtual NpgsqlTypeHandler? ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType) => null;

    /// <summary>
    /// Resolves a type handler given a .NET CLR type.
    /// </summary>
    public abstract NpgsqlTypeHandler? ResolveByClrType(Type type);

    /// <summary>
    /// Resolves a type handler given a PostgreSQL type.
    /// </summary>
    public virtual NpgsqlTypeHandler? ResolveByPostgresType(PostgresType type)
        => ResolveByDataTypeName(type.Name);

    public virtual NpgsqlTypeHandler? ResolveValueDependentValue(object value) => null;

    public virtual NpgsqlTypeHandler? ResolveValueTypeGenerically<T>(T value) => null;
}