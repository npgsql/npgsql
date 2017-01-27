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
using Npgsql.TypeHandlers;

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
        /// Holds the name and OID for all fields.
        /// Populated on the first activation of the composite.
        /// </summary>
        internal List<RawCompositeField> RawFields { get; }

        /// <summary>
        /// Constructs a representation of a PostgreSQL array data type.
        /// </summary>
#pragma warning disable CA2222 // Do not decrease inherited member visibility
        internal PostgresCompositeType(string ns, string name, uint oid, List<RawCompositeField> rawFields)
#pragma warning restore CA2222 // Do not decrease inherited member visibility
            : base(ns, name, oid)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Composite;
            RawFields = rawFields;
        }

        internal override void AddTo(TypeHandlerRegistry.AvailablePostgresTypes types)
        {
            base.AddTo(types);
            types.ByFullName[FullName] = this;
            types.ByName[Name] = types.ByName.ContainsKey(Name)
                ? null
                : this;
        }

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            // Composites need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
            // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
            throw new Exception($"Composite PostgreSQL type {Name} must be mapped before use");
        }

        internal void Activate(TypeHandlerRegistry registry, ICompositeHandlerFactory factory)
            => Activate(registry, factory.Create(this, RawFields, registry));

        internal void Activate(TypeHandlerRegistry registry, ICompositeHandler compositeHandler)
        {
            var handler = (TypeHandler)compositeHandler;

            registry.ByOID[OID] = handler;
            registry.ByType[compositeHandler.CompositeType] = handler;
            Array?.Activate(registry);
        }
    }
}
