using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using Newtonsoft.Json;
using Npgsql.Json.NET.Internal;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension allowing adding the Json.NET plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlJsonNetExtensions
{
    // Note: defined for binary compatibility and NpgsqlConnection.GlobalTypeMapper.
    /// <summary>
    /// Sets up JSON.NET mappings for the PostgreSQL json and jsonb types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    /// <param name="settings">Optional settings to customize JSON serialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static INpgsqlTypeMapper UseJsonNet(
        this INpgsqlTypeMapper mapper,
        JsonSerializerSettings? settings = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        // Reverse order
        mapper.AddTypeInfoResolverFactory(new JsonNetPocoTypeInfoResolverFactory(jsonbClrTypes, jsonClrTypes, settings));
        mapper.AddTypeInfoResolverFactory(new JsonNetTypeInfoResolverFactory(settings));
        return mapper;
    }

    /// <summary>
    /// Sets up JSON.NET mappings for the PostgreSQL json and jsonb types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    /// <param name="settings">Optional settings to customize JSON serialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static TMapper UseJsonNet<TMapper>(
        this TMapper mapper,
        JsonSerializerSettings? settings = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
        where TMapper : INpgsqlTypeMapper
    {
        // Reverse order
        mapper.AddTypeInfoResolverFactory(new JsonNetPocoTypeInfoResolverFactory(jsonbClrTypes, jsonClrTypes, settings));
        mapper.AddTypeInfoResolverFactory(new JsonNetTypeInfoResolverFactory(settings));
        return mapper;
    }
}
