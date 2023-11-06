using Npgsql.Internal.Postgres;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represents a PostgreSQL multirange data type.
/// </summary>
/// <remarks>
/// <p>See https://www.postgresql.org/docs/current/static/rangetypes.html.</p>
/// <p>Multirange types were introduced in PostgreSQL 14.</p>
/// </remarks>
public class PostgresMultirangeType : PostgresType
{
    /// <summary>
    /// The PostgreSQL data type of the range of this multirange.
    /// </summary>
    public PostgresRangeType Subrange { get; }

    /// <summary>
    /// Constructs a representation of a PostgreSQL multirange data type.
    /// </summary>
    protected internal PostgresMultirangeType(string ns, string name, uint oid, PostgresRangeType rangePostgresType)
        : base(ns, name, oid)
    {
        Subrange = rangePostgresType;
        Subrange.Multirange = this;
    }

    /// <summary>
    /// Constructs a representation of a PostgreSQL multirange data type.
    /// </summary>
    internal PostgresMultirangeType(DataTypeName dataTypeName, Oid oid, PostgresRangeType rangePostgresType)
        : base(dataTypeName, oid)
    {
        Subrange = rangePostgresType;
        Subrange.Multirange = this;
    }
}
