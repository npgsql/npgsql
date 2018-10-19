using System;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

#if NETSTANDARD2_0
using System.Data.Common;
#endif

#pragma warning disable 1591

namespace Npgsql.Schema
{
    /// <summary>
    /// Provides schema information about a column.
    /// </summary>
    /// <remarks>
    /// Note that this can correspond to a field returned in a query which isn't an actual table column
    ///
    /// See https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
    /// for information on the meaning of the different fields.
    /// </remarks>
    public class NpgsqlDbColumn : DbColumn
    {
        public NpgsqlDbColumn()
        {
            // Not supported in PostgreSQL
            IsExpression = false;
            IsAliased = false;
            IsHidden = false;
            IsIdentity = false;
        }

        #region Standard fields

        // ReSharper disable once InconsistentNaming
        public new bool? AllowDBNull
        {
            get => base.AllowDBNull;
            protected internal set => base.AllowDBNull = value;
        }

        public new string BaseCatalogName
        {
            get => base.BaseCatalogName;
            protected internal set => base.BaseCatalogName = value;
        }

        public new string BaseColumnName
        {
            get => base.BaseColumnName;
            protected internal set => base.BaseColumnName = value;
        }

        public new string BaseSchemaName
        {
            get => base.BaseSchemaName;
            protected internal set => base.BaseSchemaName = value;
        }

        public new string BaseServerName
        {
            get => base.BaseServerName;
            protected internal set => base.BaseServerName = value;
        }

        public new string BaseTableName
        {
            get => base.BaseTableName;
            protected internal set => base.BaseTableName = value;
        }

        public new string ColumnName
        {
            get => base.ColumnName;
            protected internal set => base.ColumnName = value;
        }

        public new int? ColumnOrdinal
        {
            get => base.ColumnOrdinal;
            protected internal set => base.ColumnOrdinal = value;
        }

        public new int? ColumnSize
        {
            get => base.ColumnSize;
            protected internal set => base.ColumnSize = value;
        }

        public new bool? IsAutoIncrement
        {
            get => base.IsAutoIncrement;
            protected internal set => base.IsAutoIncrement = value;
        }

        public new bool? IsKey
        {
            get => base.IsKey;
            protected internal set => base.IsKey = value;
        }

        public new bool? IsLong
        {
            get => base.IsLong;
            protected internal set => base.IsLong = value;
        }

        public new bool? IsReadOnly
        {
            get => base.IsReadOnly;
            protected internal set => base.IsReadOnly = value;
        }

        public new bool? IsUnique
        {
            get => base.IsUnique;
            protected internal set => base.IsUnique = value;
        }

        public new int? NumericPrecision
        {
            get => base.NumericPrecision;
            protected internal set => base.NumericPrecision = value;
        }

        public new int? NumericScale
        {
            get => base.NumericScale;
            protected internal set => base.NumericScale = value;
        }

        public new string UdtAssemblyQualifiedName
        {
            get => base.UdtAssemblyQualifiedName;
            protected internal set => base.UdtAssemblyQualifiedName = value;
        }

        public new Type DataType
        {
            get => base.DataType;
            protected internal set => base.DataType = value;
        }

        public new string DataTypeName
        {
            get => base.DataTypeName;
            protected internal set => base.DataTypeName = value;
        }

        #endregion Standard fields

        #region Npgsql-specific fields

        [PublicAPI]
        public PostgresType PostgresType { get; internal set; }
        [PublicAPI]
        public uint TypeOID { get; internal set; }
        [PublicAPI]
        public uint TableOID { get; internal set; }
        [PublicAPI]
        public short? ColumnAttributeNumber { get; internal set; }
        [PublicAPI]
        public string DefaultValue { get; internal set; }
        [PublicAPI]
        public NpgsqlDbType? NpgsqlDbType { get; internal set; }

        [CanBeNull]
        public override object this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                case nameof(PostgresType):
                    return PostgresType;
                case nameof(TypeOID):
                    return TypeOID;
                case nameof(TableOID):
                    return TableOID;
                case nameof(ColumnAttributeNumber):
                    return ColumnAttributeNumber;
                case nameof(DefaultValue):
                    return DefaultValue;
                case nameof(NpgsqlDbType):
                    return NpgsqlDbType;
                }

                return base[propertyName];
            }
        }

        #endregion Npgsql-specific fields
    }
}
