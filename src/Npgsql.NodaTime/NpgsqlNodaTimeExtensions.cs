﻿using Npgsql.NodaTime.Internal;
using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension adding the NodaTime plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlNodaTimeExtensions
{
    /// <summary>
    /// Sets up NodaTime mappings for the PostgreSQL date/time types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    public static INpgsqlTypeMapper UseNodaTime(this INpgsqlTypeMapper mapper)
    {
        // TODO opt-in of arrays.
        // Reverse order
        mapper.AddTypeInfoResolver(new NodaTimeArrayTypeInfoResolver());
        mapper.AddTypeInfoResolver(new NodaTimeTypeInfoResolver());
        return mapper;
    }
}
