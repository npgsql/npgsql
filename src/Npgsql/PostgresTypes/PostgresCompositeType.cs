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
        internal List<Field> Fields { get; }

        /// <summary>
        /// Constructs a representation of a PostgreSQL array data type.
        /// </summary>
#pragma warning disable CA2222 // Do not decrease inherited member visibility
        internal PostgresCompositeType(string ns, string name, uint oid, List<Field> fields)
#pragma warning restore CA2222 // Do not decrease inherited member visibility
            : base(ns, name, oid)
        {
            Fields = fields;
        }

        internal struct Field
        {
            internal string PgName;
            internal uint TypeOID;

            public override string ToString() => $"{PgName} => {TypeOID}";
        }
    }
}
