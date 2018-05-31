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
using JetBrains.Annotations;
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
        /// Holds the name and types for all fields.
        /// </summary>
        public IReadOnlyList<Field> Fields => MutableFields;

        internal List<Field> MutableFields { get; } = new List<Field>();

        /// <summary>
        /// Constructs a representation of a PostgreSQL array data type.
        /// </summary>
#pragma warning disable CA2222 // Do not decrease inherited member visibility
        internal PostgresCompositeType(string ns, string name, uint oid)
            : base(ns, name, oid) {}
#pragma warning restore CA2222 // Do not decrease inherited member visibility

        /// <summary>
        /// Represents a field in a PostgreSQL composite data type.
        /// </summary>
        public class Field
        {
            internal Field(string name, PostgresType type)
            {
                Name = name;
                Type = type;
            }

            /// <summary>
            /// The name of the composite field.
            /// </summary>
            public string Name { get; }
            /// <summary>
            /// The type of the composite field.
            /// </summary>
            public PostgresType Type { get; }

            /// <inheritdoc />
            public override string ToString() => $"{Name} => {Type}";
        }
    }
}
