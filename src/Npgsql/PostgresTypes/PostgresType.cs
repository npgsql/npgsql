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
using System.Text;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL data type, such as int4 or text, as discovered from pg_type.
    /// This class is abstract, see derived classes for concrete types of PostgreSQL types.
    /// </summary>
    /// <remarks>
    /// Instances of this class are shared between connections to the same databases.
    /// For more info about what this class and its subclasses represent, see
    /// https://www.postgresql.org/docs/current/static/catalog-pg-type.html.
    /// </remarks>
    public abstract class PostgresType
    {
        #region Constructors

        /// <summary>
        /// Constructs a representation of a PostgreSQL data type.
        /// </summary>
        /// <param name="ns">The data type's namespace (or schema).</param>
        /// <param name="name">The data type's name.</param>
        /// <param name="oid">The data type's OID.</param>
        protected PostgresType(string ns, string name, uint oid)
            : this(ns, name, name, oid) {}

        /// <summary>
        /// Constructs a representation of a PostgreSQL data type.
        /// </summary>
        /// <param name="ns">The data type's namespace (or schema).</param>
        /// <param name="name">The data type's name.</param>
        /// <param name="internalName">The data type's internal name (e.g. _int4 for integer[]).</param>
        /// <param name="oid">The data type's OID.</param>
        protected PostgresType(string ns, string name, string internalName, uint oid)
        {
            Namespace = ns;
            Name = name;
            FullName = Namespace + '.' + Name;
            InternalName = internalName;
            OID = oid;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The data type's OID - a unique id identifying the data type in a given database (in pg_type).
        /// </summary>
        [PublicAPI]
        public uint OID { get; }

        /// <summary>
        /// The data type's namespace (or schema).
        /// </summary>
        [PublicAPI]
        public string Namespace { get; }

        /// <summary>
        /// The data type's name.
        /// </summary>
        /// <remarks>
        /// Note that this is the standard, user-displayable type name (e.g. integer[]) rather than the internal
        /// PostgreSQL name as it is in pg_type (_int4). See <see cref="InternalName"/> for the latter.
        /// </remarks>
        [PublicAPI]
        public string Name { get; }

        /// <summary>
        /// The full name of the backend type, including its namespace.
        /// </summary>
        [PublicAPI]
        public string FullName { get; }

        /// <summary>
        /// A display name for this backend type, including the namespace unless it is pg_catalog (the namespace
        /// for all built-in types).
        /// </summary>
        [PublicAPI]
        public string DisplayName => Namespace == "pg_catalog" ? Name : FullName;

        /// <summary>
        /// The data type's internal PostgreSQL name (e.g. integer[] not _int4).
        /// See <see cref="Name"/> for a more user-friendly name.
        /// </summary>
        [PublicAPI]
        public string InternalName { get; }

        /// <summary>
        /// If a PostgreSQL array type exists for this type, it will be referenced here.
        /// Otherwise null.
        /// </summary>
        [PublicAPI, CanBeNull]
        public PostgresArrayType Array { get; internal set; }

        /// <summary>
        /// If a PostgreSQL range type exists for this type, it will be referenced here.
        /// Otherwise null.
        /// </summary>
        [PublicAPI, CanBeNull]
        public PostgresRangeType Range { get; internal set; }

        #endregion

        internal virtual string GetPartialNameWithFacets(int typeModifier) => Name;

        /// <summary>
        /// Generates the type name including any facts (size, precision, scale), given the PostgreSQL type modifier.
        /// </summary>
        internal string GetDisplayNameWithFacets(int typeModifier)
            => Namespace == "pg_catalog"
                ? GetPartialNameWithFacets(typeModifier)
                : Namespace + '.' + GetPartialNameWithFacets(typeModifier);

        internal virtual PostgresFacets GetFacets(int typeModifier) => PostgresFacets.None;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() => DisplayName;
    }
}
