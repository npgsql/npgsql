using Npgsql.NodaTime.Internal;
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
    public static T UseNodaTime<T>(this T mapper) where T : INpgsqlTypeMapper
    {
        mapper.AddTypeResolverFactory(new NodaTimeTypeHandlerResolverFactory());
        return mapper;
    }
}
