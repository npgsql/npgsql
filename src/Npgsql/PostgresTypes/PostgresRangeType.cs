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

using JetBrains.Annotations;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL range data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/rangetypes.html.
    /// </remarks>
    public class PostgresRangeType : PostgresType
    {
        /// <summary>
        /// The PostgreSQL data type of the subtype of this range.
        /// </summary>
        [PublicAPI]
        public PostgresType Subtype { get; }

        /// <summary>
        /// Constructs a representation of a PostgreSQL range data type.
        /// </summary>
        protected internal PostgresRangeType(string ns, string name, uint oid, PostgresType subtypePostgresType)
            : base(ns, name, oid)
        {
            Subtype = subtypePostgresType;
            Subtype.Range = this;
        }
    }
}
