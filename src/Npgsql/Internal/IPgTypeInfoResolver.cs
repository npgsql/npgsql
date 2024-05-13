using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

/// <summary>
/// An Npgsql resolver for type info. Used by Npgsql to read and write values to PostgreSQL.
/// </summary>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public interface IPgTypeInfoResolver
{
    /// <summary>
    /// Resolve a type info for a given type and data type name, at least one value will be non-null.
    /// </summary>
    /// <param name="type">The clr type being requested.</param>
    /// <param name="dataTypeName">The postgres type being requested.</param>
    /// <param name="options">Used for configuration state and Npgsql type info or PostgreSQL type catalog lookups.</param>
    /// <returns>A result, or null if there was no match.</returns>
    PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options);
}
