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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Npgsql.TypeHandlers;

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
        protected internal PostgresArrayType(string ns, string name, uint oid, PostgresType elementPostgresType)
            : base(ns, name, oid)
        {
            Element = elementPostgresType;
            if (elementPostgresType.NpgsqlDbType.HasValue)
                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | elementPostgresType.NpgsqlDbType;
            Element.Array = this;
        }

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            if (!registry.TryGetByOID(Element.OID, out var elementHandler))
            {
                // Element type hasn't been set up yet, do it now
                elementHandler = Element.Activate(registry);
            }

            var arrayHandler = elementHandler.CreateArrayHandler(this);
            registry.ByOID[OID] = arrayHandler;

            var asEnumHandler = elementHandler as IEnumHandler;
            if (asEnumHandler != null)
            {
                if (registry.ArrayHandlerByType == null)
                    registry.ArrayHandlerByType = new Dictionary<Type, TypeHandler>();
                registry.ArrayHandlerByType[asEnumHandler.EnumType] = arrayHandler;
                return arrayHandler;
            }

            var asCompositeHandler = elementHandler as ICompositeHandler;
            if (asCompositeHandler != null)
            {
                if (registry.ArrayHandlerByType == null)
                    registry.ArrayHandlerByType = new Dictionary<Type, TypeHandler>();
                registry.ArrayHandlerByType[asCompositeHandler.CompositeType] = arrayHandler;
                return arrayHandler;
            }

            if (NpgsqlDbType.HasValue)
                registry.ByNpgsqlDbType[NpgsqlDbType.Value] = arrayHandler;

            // Note that array handlers aren't registered in _byType, because they handle all dimension types and not just one CLR type
            // (e.g. int[], int[,], int[,,]). So the by-type lookup is special, see this[Type type]
            // TODO: register single-dimensional in _byType as a specific optimization? But do PSV as well...

            return arrayHandler;
        }
    }
}
