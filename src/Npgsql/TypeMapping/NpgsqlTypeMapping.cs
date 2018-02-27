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
using System.Data;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    /// <summary>
    /// Builds instances of <see cref="NpgsqlTypeMapping"/> for addition into <see cref="INpgsqlTypeMapper"/>.
    /// </summary>
    public class NpgsqlTypeMappingBuilder
    {
        /// <summary>
        /// The name of the PostgreSQL type name, as it appears in the pg_type catalog.
        /// </summary>
        /// <remarks>
        /// This can a a partial name (without the schema), or a fully-qualified name
        /// (schema.typename) - the latter can be used if you have two types with the same
        /// name in different schemas.
        /// </remarks>
        public string PgTypeName { get; set; }

        /// <summary>
        /// The <see cref="NpgsqlDbType"/> that corresponds to this type. Setting an
        /// <see cref="NpgsqlParameter"/>'s <see cref="NpgsqlParameter.NpgsqlDbType"/> property
        /// to this value will make Npgsql write its value to PostgreSQL with this mapping.
        /// </summary>
        public NpgsqlDbType? NpgsqlDbType { get; set; }

        /// <summary>
        /// A set of <see cref="DbType"/>s that correspond to this type. Setting an
        /// <see cref="NpgsqlParameter"/>'s <see cref="NpgsqlParameter.DbType"/> property
        /// to one of these values will make Npgsql write its value to PostgreSQL with this mapping.
        /// </summary>
        public DbType[] DbTypes { get; set; }

        /// <summary>
        /// A set of CLR types that correspond to this type. Setting an
        /// <see cref="NpgsqlParameter"/>'s <see cref="NpgsqlParameter.Value"/> property
        /// to one of these types will make Npgsql write its value to PostgreSQL with this mapping.
        /// </summary>
        public Type[] ClrTypes { get; set; }

        /// <summary>
        /// Determines what is returned from <see cref="NpgsqlParameter.DbType"/> when this mapping
        /// is used.
        /// </summary>
        public DbType? InferredDbType { get; set; }

        /// <summary>
        /// A factory for a type handler that will be used to read and write values for PostgreSQL type.
        /// </summary>
        public NpgsqlTypeHandlerFactory TypeHandlerFactory { get; set; }

        /// <summary>
        /// Builds an <see cref="NpgsqlTypeMapping"/> that can be added to an <see cref="INpgsqlTypeMapper"/>.
        /// </summary>
        /// <returns></returns>
        public NpgsqlTypeMapping Build()
        {
            if (string.IsNullOrWhiteSpace(PgTypeName))
                throw new ArgumentException($"{PgTypeName} must contain the name of a PostgreSQL data type");
            if (TypeHandlerFactory == null)
                throw new ArgumentException($"{TypeHandlerFactory} must refer to a type handler factory");
            return new NpgsqlTypeMapping(PgTypeName, NpgsqlDbType, DbTypes, ClrTypes, InferredDbType, TypeHandlerFactory);
        }
    }

    /// <summary>
    /// Represents a type mapping for a PostgreSQL data type, which can be added to a type mapper,
    /// managing when that data type will be read and written and how.
    /// </summary>
    /// <seealso cref="NpgsqlConnection.GlobalTypeMapper"/>
    /// <seealso cref="NpgsqlConnection.TypeMapper"/>
    public sealed class NpgsqlTypeMapping
    {
        internal NpgsqlTypeMapping(
            string pgTypeName,
            NpgsqlDbType? npgsqlDbType, DbType[] dbTypes, Type[] clrTypes, DbType? inferredDbType,
            NpgsqlTypeHandlerFactory typeHandlerFactory)
        {
            PgTypeName = pgTypeName;
            NpgsqlDbType = npgsqlDbType;
            DbTypes = dbTypes ?? EmptyDbTypes;
            ClrTypes = clrTypes ?? EmptyClrTypes;
            InferredDbType = inferredDbType;
            TypeHandlerFactory = typeHandlerFactory;
        }

        /// <summary>
        /// The name of the PostgreSQL type name, as it appears in the pg_type catalog.
        /// </summary>
        /// <remarks>
        /// This can a a partial name (without the schema), or a fully-qualified name
        /// (schema.typename) - the latter can be used if you have two types with the same
        /// name in different schemas.
        /// </remarks>
        public string PgTypeName { get; }

        /// <summary>
        /// The <see cref="NpgsqlDbType"/> that corresponds to this type. Setting an
        /// <see cref="NpgsqlParameter"/>'s <see cref="NpgsqlParameter.NpgsqlDbType"/> property
        /// to this value will make Npgsql write its value to PostgreSQL with this mapping.
        /// </summary>
        public NpgsqlDbType? NpgsqlDbType { get; }

        /// <summary>
        /// A set of <see cref="DbType"/>s that correspond to this type. Setting an
        /// <see cref="NpgsqlParameter"/>'s <see cref="NpgsqlParameter.DbType"/> property
        /// to one of these values will make Npgsql write its value to PostgreSQL with this mapping.
        /// </summary>
        public DbType[] DbTypes { get; }

        /// <summary>
        /// A set of CLR types that correspond to this type. Setting an
        /// <see cref="NpgsqlParameter"/>'s <see cref="NpgsqlParameter.Value"/> property
        /// to one of these types will make Npgsql write its value to PostgreSQL with this mapping.
        /// </summary>
        public Type[] ClrTypes { get; }

        /// <summary>
        /// Determines what is returned from <see cref="NpgsqlParameter.DbType"/> when this mapping
        /// is used.
        /// </summary>
        public DbType? InferredDbType { get; }

        /// <summary>
        /// A factory for a type handler that will be used to read and write values for PostgreSQL type.
        /// </summary>
        public NpgsqlTypeHandlerFactory TypeHandlerFactory { get; }

        /// <summary>
        /// The default CLR type that handlers produced by this factory will read and write.
        /// Used by the EF Core provider (and possibly others in the future).
        /// </summary>
        internal Type DefaultClrType => TypeHandlerFactory.DefaultValueType;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString() => $"{PgTypeName} => {TypeHandlerFactory.GetType().Name}";

        static readonly DbType[] EmptyDbTypes = new DbType[0];
        static readonly Type[] EmptyClrTypes = new Type[0];
    }
}
