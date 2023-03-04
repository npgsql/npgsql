using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension allowing adding record mapping to an Npgsql type mapper.
/// </summary>
public static class NpgsqlRecordExtensions
{
    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type.
    /// </summary>
    /// <param name="mapper">The type mapper to set up.</param>
    public static INpgsqlTypeMapper UseRecord(this INpgsqlTypeMapper mapper)
    {
        mapper.AddTypeResolverFactory(new RecordTypeHandlerResolverFactory());
        return mapper;
    }
}
