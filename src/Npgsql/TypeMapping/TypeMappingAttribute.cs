using System;
using System.Data;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [MeansImplicitUse]
    class TypeMappingAttribute : Attribute
    {
        /// <summary>
        /// Maps an Npgsql type handler to a PostgreSQL type.
        /// </summary>
        /// <param name="pgName">A PostgreSQL type name as it appears in the pg_type table.</param>
        /// <param name="npgsqlDbType">
        /// A member of <see cref="NpgsqlDbType"/> which represents this PostgreSQL type.
        /// An <see cref="NpgsqlParameter"/> with <see cref="NpgsqlParameter.NpgsqlDbType"/> set to
        /// this value will be sent with the type handler mapped by this attribute.
        /// </param>
        /// <param name="dbTypes">
        /// All members of <see cref="DbType"/> which represent this PostgreSQL type.
        /// An <see cref="NpgsqlParameter"/> with <see cref="NpgsqlParameter.DbType"/> set to
        /// one of these values will be sent with the type handler mapped by this attribute.
        /// </param>
        /// <param name="clrTypes">
        /// Any .NET type which corresponds to this PostgreSQL type.
        /// An <see cref="NpgsqlParameter"/> with <see cref="NpgsqlParameter.Value"/> set to
        /// one of these values will be sent with the type handler mapped by this attribute.
        /// </param>
        /// <param name="inferredDbType">
        /// The "primary" <see cref="DbType"/> which best corresponds to this PostgreSQL type.
        /// When <see cref="NpgsqlParameter.NpgsqlDbType"/> or <see cref="NpgsqlParameter.Value"/>
        /// set, <see cref="NpgsqlParameter.DbType"/> will be set to this value.
        /// </param>
        internal TypeMappingAttribute(string pgName, NpgsqlDbType? npgsqlDbType, [CanBeNull] DbType[] dbTypes, [CanBeNull] Type[] clrTypes, DbType? inferredDbType)
        {
            if (string.IsNullOrWhiteSpace(pgName))
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            PgName = pgName;
            NpgsqlDbType = npgsqlDbType;
            DbTypes = dbTypes ?? new DbType[0];
            ClrTypes = clrTypes ?? new Type[0];
            InferredDbType = inferredDbType;
        }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes, [CanBeNull] Type[] clrTypes, DbType inferredDbType)
            : this(pgName, (NpgsqlDbType?)npgsqlDbType, dbTypes, clrTypes, inferredDbType)
        { }

        //internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes=null, Type type=null)
        //    : this(pgName, npgsqlDbType, dbTypes, type == null ? null : new[] { type }) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new Type[0], null)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType inferredDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new Type[0], inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes, Type clrType, DbType inferredDbType)
            : this(pgName, npgsqlDbType, dbTypes, new[] { clrType }, inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes)
            : this(pgName, npgsqlDbType, dbTypes, new Type[0], null)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type[] clrTypes)
            : this(pgName, npgsqlDbType, new[] { dbType }, clrTypes, dbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type clrType = null)
            : this(pgName, npgsqlDbType, new[] { dbType }, clrType == null ? null : new[] { clrType }, dbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type[] clrTypes, DbType inferredDbType)
            : this(pgName, npgsqlDbType, new DbType[0], clrTypes, inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type[] clrTypes)
            : this(pgName, npgsqlDbType, new DbType[0], clrTypes, null)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type clrType, DbType inferredDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new[] { clrType }, inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type clrType)
            : this(pgName, npgsqlDbType, new DbType[0], new[] { clrType }, null)
        { }

        /// <summary>
        /// Read-only parameter
        /// </summary>
        internal TypeMappingAttribute(string pgName)
            : this(pgName, null, null, null, null)
        { }

        internal string PgName { get; }
        internal NpgsqlDbType? NpgsqlDbType { get; }
        internal DbType[] DbTypes { get; }
        internal Type[] ClrTypes { get; }
        internal DbType? InferredDbType { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0} NpgsqlDbType={1}", PgName, NpgsqlDbType);
            if (DbTypes.Length > 0)
            {
                sb.Append(" DbTypes=");
                sb.Append(string.Join(",", DbTypes.Select(t => t.ToString())));
            }
            if (ClrTypes.Length > 0)
            {
                sb.Append(" ClrTypes=");
                sb.Append(string.Join(",", ClrTypes.Select(t => t.Name)));
            }
            sb.AppendFormat("]");
            return sb.ToString();
        }
    }
}
