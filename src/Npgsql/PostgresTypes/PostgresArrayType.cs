#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Diagnostics;
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
