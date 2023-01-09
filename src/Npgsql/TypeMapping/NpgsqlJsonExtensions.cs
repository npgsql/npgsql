using System;
using System.Text.Json;
using Npgsql.TypeMapping;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension allowing adding the System.Text.Json plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlJsonExtensions
{
    /// <summary>
    /// Sets up System.Text.Json mappings for the PostgreSQL <c>json</c> and <c>jsonb</c> types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    /// <param name="serializerOptions">Options to customize JSON serialization and deserialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    public static INpgsqlTypeMapper UseSystemTextJson(
        this INpgsqlTypeMapper mapper,
        JsonSerializerOptions? serializerOptions = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        mapper.AddTypeResolverFactory(new JsonTypeHandlerResolverFactory(jsonbClrTypes, jsonClrTypes, serializerOptions));
        return mapper;
    }
}
