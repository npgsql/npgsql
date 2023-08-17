using System;
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
    public static INpgsqlTypeMapper UseJsonNet(
        this INpgsqlTypeMapper mapper,
        JsonSerializerSettings? settings = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        // TODO opt-in of arrays.
        // Reverse order
        mapper.AddTypeInfoResolver(new JsonNetPocoArrayTypeInfoResolver(jsonbClrTypes, jsonClrTypes, settings));
        mapper.AddTypeInfoResolver(new JsonNetArrayTypeInfoResolver(settings));
        mapper.AddTypeInfoResolver(new JsonNetPocoTypeInfoResolver(jsonbClrTypes, jsonClrTypes, settings));
        mapper.AddTypeInfoResolver(new JsonNetTypeInfoResolver(settings));
        return mapper;
    }
}
