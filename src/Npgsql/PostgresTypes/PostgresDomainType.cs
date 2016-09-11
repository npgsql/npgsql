using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL domain type.
    /// </summary>
    /// <remarks>
    /// When PostgreSQL returns a RowDescription for a domain type, the type OID is the base type's
    /// (so fetching a domain type over text returns a RowDescription for text).
    /// However, when a composite type is returned, the type OID there is that of the domain,
    /// so we provide "clean" support for domain types.
    /// </remarks>
    class PostgresDomainType : PostgresType
    {
        readonly PostgresType _basePostgresType;

        public PostgresDomainType(string ns, string name, uint oid, PostgresType basePostgresType)
            : base(ns, name, oid)
        {
            _basePostgresType = basePostgresType;
        }

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            TypeHandler baseTypeHandler;
            if (!registry.TryGetByOID(_basePostgresType.OID, out baseTypeHandler))
            {
                // Base type hasn't been set up yet, do it now
                baseTypeHandler = _basePostgresType.Activate(registry);
            }

            // Make the domain type OID point to the base type's type handler, the wire encoding
            // is the same
            registry.ByOID[OID] = baseTypeHandler;

            return baseTypeHandler;
        }
    }
}
