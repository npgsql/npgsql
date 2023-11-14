using Npgsql.Internal.Postgres;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represents a PostgreSQL range data type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/rangetypes.html.
/// </remarks>
public class PostgresRangeType : PostgresType
{
    /// <summary>
    /// The PostgreSQL data type of the subtype of this range.
    /// </summary>
    public PostgresType Subtype { get; }

    /// <summary>
    /// The PostgreSQL data type of the multirange of this range.
    /// </summary>
    public PostgresMultirangeType? Multirange { get; internal set; }

    /// <summary>
    /// Constructs a representation of a PostgreSQL range data type.
    /// </summary>
    protected internal PostgresRangeType(
        string ns, string name, uint oid, PostgresType subtypePostgresType)
        : base(ns, name, oid)
    {
        Subtype = subtypePostgresType;
        Subtype.Range = this;
    }

    /// <summary>
    /// Constructs a representation of a PostgreSQL range data type.
    /// </summary>
    internal PostgresRangeType(DataTypeName dataTypeName, Oid oid, PostgresType subtypePostgresType)
        : base(dataTypeName, oid)
    {
        Subtype = subtypePostgresType;
        Subtype.Range = this;
    }
}
