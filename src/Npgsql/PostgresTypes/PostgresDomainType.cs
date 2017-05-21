#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            TypeHandler baseTypeHandler;
            if (!registry.TryGetByOID(BaseType.OID, out baseTypeHandler))
            {
                // Base type hasn't been set up yet, do it now
                baseTypeHandler = BaseType.Activate(registry);
            }

            // Make the domain type OID point to the base type's type handler, the wire encoding
            // is the same
            registry.ByOID[OID] = baseTypeHandler;

            return baseTypeHandler;
        }
    }
}
