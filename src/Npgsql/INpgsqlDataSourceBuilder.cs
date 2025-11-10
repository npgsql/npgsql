using Npgsql.Internal;
using Npgsql.TypeMapping;

namespace Npgsql;

/// <summary>
/// A data source builder, manages how a data source is configured before being built.
/// </summary>
public interface INpgsqlDataSourceBuilder : INpgsqlTypeMapper
{
    // We don't want external implementations, this allows us to add members to this interface freely.
    internal INpgsqlDataSourceBuilder Instance();

    /// <summary>
    /// Adds a type info resolver factory which can add or modify support for PostgreSQL types.
    /// Typically used by plugins.
    /// </summary>
    /// <param name="factory">The type resolver factory to be added.</param>
    public void AddDbTypeResolverFactory(DbTypeResolverFactory factory);
}
