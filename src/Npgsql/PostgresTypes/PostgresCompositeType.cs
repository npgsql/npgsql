using System.Collections.Generic;
using Npgsql.Internal.Postgres;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represents a PostgreSQL composite data type, which can hold multiple fields of varying types in a single column.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/rowtypes.html.
/// </remarks>
public class PostgresCompositeType : PostgresType
{
    /// <summary>
    /// Holds the name and types for all fields.
    /// </summary>
    public IReadOnlyList<Field> Fields => MutableFields;

    internal List<Field> MutableFields { get; } = [];

    /// <summary>
    /// Constructs a representation of a PostgreSQL array data type.
    /// </summary>
    internal PostgresCompositeType(string ns, string name, uint oid)
        : base(ns, name, oid) {}

    /// <summary>
    /// Constructs a representation of a PostgreSQL domain data type.
    /// </summary>
    internal PostgresCompositeType(DataTypeName dataTypeName, Oid oid)
        : base(dataTypeName, oid) {}

    /// <summary>
    /// Represents a field in a PostgreSQL composite data type.
    /// </summary>
    public class Field
    {
        internal Field(string name, PostgresType type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// The name of the composite field.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The type of the composite field.
        /// </summary>
        public PostgresType Type { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Name} => {Type}";
    }
}
