using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a single PostgreSQL data type, as discovered from pg_type.
    /// </summary>
    /// <remarks>
    /// Note that this simply a data structure describing a type and not a handler, and is shared between connectors
    /// having the same connection string.
    /// </remarks>
    public abstract class PostgresType
    {
        /// <summary>
        /// Constructs a representation of a PostgreSQL data type.
        /// </summary>
        /// <param name="ns">The data type's namespace (or schema).</param>
        /// <param name="name">The data type's name.</param>
        /// <param name="oid">The data type's OID.</param>
        protected PostgresType(string ns, string name, uint oid)
        {
            Namespace = ns;
            Name = name;
            FullName = Namespace + '.' + Name;
            DisplayName = Namespace == "pg_catalog" || Namespace == ""
                ? Name
                : FullName;
            OID = oid;
        }

        /// <summary>
        /// The data type's namespace (or schema).
        /// </summary>
        public string Namespace { get; }
        /// <summary>
        /// The data type's name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The data type's OID - a unique id identifying the data type in a given database (in pg_type).
        /// </summary>
        public uint OID { get; }
        public string FullName { get; }
        public string DisplayName { get; }
        [CanBeNull]
        internal PostgresArrayType Array;
        [CanBeNull]
        internal PostgresRangeType Range;

        internal NpgsqlDbType? NpgsqlDbType { get; protected set; }

        /// <summary>
        /// For base types, contains the handler type.
        /// If null, this backend type isn't supported by Npgsql.
        /// </summary>
        [CanBeNull]
        internal Type HandlerType;

        internal virtual void AddTo(TypeHandlerRegistry.AvailablePostgresTypes types)
        {
            types.ByOID[OID] = this;
            if (NpgsqlDbType != null)
                types.ByNpgsqlDbType[NpgsqlDbType.Value] = this;
        }

        internal abstract TypeHandler Activate(TypeHandlerRegistry registry);

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() => DisplayName;
    }
}
