using JetBrains.Annotations;

namespace Npgsql.PostgresTypes
{
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
        [PublicAPI]
        public PostgresType Subtype { get; }

        /// <summary>
        /// Constructs a representation of a PostgreSQL range data type.
        /// </summary>
        protected internal PostgresRangeType(string ns, string name, uint oid, PostgresType subtypePostgresType)
            : base(ns, name, oid)
        {
            Subtype = subtypePostgresType;
            Subtype.Range = this;
        }
    }
}
