using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension allowing adding range and multirange mappings to an Npgsql type mapper.
/// </summary>
public static class NpgsqlRangeExtensions
{
    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>range</c> and <c>multirange</c> types.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    public static INpgsqlTypeMapper UseRange(this INpgsqlTypeMapper mapper)
    {
        mapper.AddTypeResolverFactory(new RangeTypeHandlerResolverFactory());
        return mapper;
    }
}
