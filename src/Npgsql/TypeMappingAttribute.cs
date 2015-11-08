using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [MeansImplicitUse]
    internal class TypeMappingAttribute : Attribute
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
        /// <param name="types">
        /// Any .NET type which corresponds to this PostgreSQL type.
        /// An <see cref="NpgsqlParameter"/> with <see cref="NpgsqlParameter.Value"/> set to
        /// one of these values will be sent with the type handler mapped by this attribute.
        /// </param>
        /// <param name="inferredDbType">
        /// The "primary" <see cref="DbType"/> which best corresponds to this PostgreSQL type.
        /// When <see cref="NpgsqlParameter.NpgsqlDbType"/> or <see cref="NpgsqlParameter.Value"/>
        /// set, <see cref="NpgsqlParameter.DbType"/> will be set to this value.
        /// </param>
        internal TypeMappingAttribute(string pgName, NpgsqlDbType? npgsqlDbType, [CanBeNull] DbType[] dbTypes, [CanBeNull] Type[] types, DbType? inferredDbType)
        {
            if (String.IsNullOrWhiteSpace(pgName))
                throw new ArgumentException("pgName can't be empty", nameof(pgName));
            Contract.EndContractBlock();

            PgName = pgName;
            NpgsqlDbType = npgsqlDbType;
            DbTypes = dbTypes ?? new DbType[0];
            Types = types ?? new Type[0];
            InferredDbType = inferredDbType;
        }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes, [CanBeNull] Type[] types, DbType inferredDbType)
            : this(pgName, (NpgsqlDbType?)npgsqlDbType, dbTypes, types, inferredDbType)
        { }

        //internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes=null, Type type=null)
        //    : this(pgName, npgsqlDbType, dbTypes, type == null ? null : new[] { type }) {}

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new Type[0], null)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType inferredDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new Type[0], inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes, Type type, DbType inferredDbType)
            : this(pgName, npgsqlDbType, dbTypes, new[] { type }, inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType[] dbTypes)
            : this(pgName, npgsqlDbType, dbTypes, new Type[0], null)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type[] types)
            : this(pgName, npgsqlDbType, new[] { dbType }, types, dbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, DbType dbType, Type type = null)
            : this(pgName, npgsqlDbType, new[] { dbType }, type == null ? null : new[] { type }, dbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type[] types, DbType inferredDbType)
            : this(pgName, npgsqlDbType, new DbType[0], types, inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type[] types)
            : this(pgName, npgsqlDbType, new DbType[0], types, null)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type type, DbType inferredDbType)
            : this(pgName, npgsqlDbType, new DbType[0], new[] { type }, inferredDbType)
        { }

        internal TypeMappingAttribute(string pgName, NpgsqlDbType npgsqlDbType, Type type)
            : this(pgName, npgsqlDbType, new DbType[0], new[] { type }, null)
        { }

        /// <summary>
        /// Read-only parameter
        /// </summary>
        internal TypeMappingAttribute(string pgName)
            : this(pgName, null, null, null, null)
        { }

        internal string PgName { get; private set; }
        internal NpgsqlDbType? NpgsqlDbType { get; private set; }
        internal DbType[] DbTypes { get; private set; }
        internal Type[] Types { get; private set; }
        internal DbType? InferredDbType { get; private set; }

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
                sb.Append(String.Join(",", DbTypes.Select(t => t.ToString())));
            }
            if (Types.Length > 0)
            {
                sb.Append(" Types=");
                sb.Append(String.Join(",", Types.Select(t => t.Name)));
            }
            sb.AppendFormat("]");
            return sb.ToString();
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(!String.IsNullOrWhiteSpace(PgName));
            Contract.Invariant(Types != null);
            Contract.Invariant(DbTypes != null);
        }
    }
}
