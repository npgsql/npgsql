using System.Diagnostics;
using JetBrains.Annotations;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL array data type, which can hold several multiple values in a single column.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/arrays.html.
    /// </remarks>
    public class PostgresArrayType : PostgresType
    {
        /// <summary>
        /// The PostgreSQL data type of the element contained within this array.
        /// </summary>
        [PublicAPI]
        public PostgresType Element { get; }

        /// <summary>
        /// Constructs a representation of a PostgreSQL array data type.
        /// </summary>
        protected internal PostgresArrayType(string ns, string internalName, uint oid, PostgresType elementPostgresType)
            : base(ns, elementPostgresType.Name + "[]", internalName, oid)
        {
            Debug.Assert(internalName == '_' + elementPostgresType.InternalName);
            Element = elementPostgresType;
            Element.Array = this;
        }

        // PostgreSQL array types have an underscore-prefixed name (_text), but we
        // want to return the public text[] instead
        /// <inheritdoc/>
        internal override string GetPartialNameWithFacets(int typeModifier)
            => Element.GetPartialNameWithFacets(typeModifier) + "[]";

        internal override PostgresFacets GetFacets(int typeModifier)
            => Element.GetFacets(typeModifier);
    }
}
