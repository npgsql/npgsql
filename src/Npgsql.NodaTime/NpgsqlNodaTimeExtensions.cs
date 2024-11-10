using Npgsql.NodaTime.Internal;
using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension adding the NodaTime plugin to an Npgsql type mapper.
/// </summary>
public static class NpgsqlNodaTimeExtensions
{
    // Note: defined for binary compatibility and NpgsqlConnection.GlobalTypeMapper.
    /// <summary>
    /// Sets up NodaTime mappings for the PostgreSQL date/time types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    public static INpgsqlTypeMapper UseNodaTime(this INpgsqlTypeMapper mapper)
    {
        mapper.AddTypeInfoResolverFactory(new NodaTimeTypeInfoResolverFactory());
        return mapper;
    }

    /// <summary>
    /// Sets up NodaTime mappings for the PostgreSQL date/time types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up (global or connection-specific)</param>
    public static TMapper UseNodaTime<TMapper>(this TMapper mapper) where TMapper : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolverFactory(new NodaTimeTypeInfoResolverFactory());
        return mapper;
    }
}
