using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represents a PostgreSQL enum data type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-enum.html.
/// </remarks>
public class PostgresEnumType : PostgresType
{
    /// <summary>
    /// The enum's fields.
    /// </summary>
    public IReadOnlyList<string> Labels => MutableLabels;

    internal List<string> MutableLabels { get; } = [];

    /// <summary>
    /// Constructs a representation of a PostgreSQL enum data type.
    /// </summary>
    protected internal PostgresEnumType(string ns, string name, uint oid)
        : base(ns, name, oid) {}

    /// <summary>
    /// Constructs a representation of a PostgreSQL enum data type.
    /// </summary>
    internal PostgresEnumType(DataTypeName dataTypeName, Oid oid)
        : base(dataTypeName, oid) {}

}
