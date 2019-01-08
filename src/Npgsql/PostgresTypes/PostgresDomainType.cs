using JetBrains.Annotations;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL domain type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/sql-createdomain.html.
    ///
    /// When PostgreSQL returns a RowDescription for a domain type, the type OID is the base type's
    /// (so fetching a domain type over text returns a RowDescription for text).
    /// However, when a composite type is returned, the type OID there is that of the domain,
    /// so we provide "clean" support for domain types.
    /// </remarks>
    public class PostgresDomainType : PostgresType
    {
        /// <summary>
        /// The PostgreSQL data type of the base type, i.e. the type this domain is based on.
        /// </summary>
        [PublicAPI]
        public PostgresType BaseType { get; }

        /// <summary>
        /// Constructs a representation of a PostgreSQL domain data type.
        /// </summary>
        protected internal PostgresDomainType(string ns, string name, uint oid, PostgresType baseType)
            : base(ns, name, oid)
        {
            BaseType = baseType;
        }

        internal override PostgresFacets GetFacets(int typeModifier)
            => BaseType.GetFacets(typeModifier);
    }
}
