using System.Collections.Generic;

namespace Npgsql.PostgresTypes
{
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

        internal List<Field> MutableFields { get; } = new List<Field>();

        /// <summary>
        /// Constructs a representation of a PostgreSQL array data type.
        /// </summary>
#pragma warning disable CA2222 // Do not decrease inherited member visibility
        internal PostgresCompositeType(string ns, string name, uint oid)
            : base(ns, name, oid) {}
#pragma warning restore CA2222 // Do not decrease inherited member visibility

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
}
