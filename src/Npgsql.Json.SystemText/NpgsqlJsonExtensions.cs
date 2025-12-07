using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using Npgsql.Json.Internal;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension allowing adding the System.Text.Json plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlJsonExtensions
{
    // Note: defined for binary compatibility and NpgsqlConnection.GlobalTypeMapper.
    /// <summary>
    /// Sets up System.Text.Json mappings for the PostgreSQL json and jsonb types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    /// <param name="options">Optional settings to customize JSON serialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static INpgsqlTypeMapper UseSystemTextJson(
        this INpgsqlTypeMapper mapper,
        JsonSerializerOptions? options = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        // Reverse order
        mapper.AddTypeInfoResolverFactory(new SystemTextJsonPocoTypeInfoResolverFactory(jsonbClrTypes, jsonClrTypes, options));
        mapper.AddTypeInfoResolverFactory(new SystemTextJsonTypeInfoResolverFactory(options));
        return mapper;
    }

    /// <summary>
    /// Sets up System.Text.Json mappings for the PostgreSQL json and jsonb types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    /// <param name="options">Optional settings to customize JSON serialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static TMapper UseSystemTextJson<TMapper>(
        this TMapper mapper,
        JsonSerializerOptions? options = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
        where TMapper : INpgsqlTypeMapper
    {
        // Reverse order
        mapper.AddTypeInfoResolverFactory(new SystemTextJsonPocoTypeInfoResolverFactory(jsonbClrTypes, jsonClrTypes, options));
        mapper.AddTypeInfoResolverFactory(new SystemTextJsonTypeInfoResolverFactory(options));
        return mapper;
    }
}
