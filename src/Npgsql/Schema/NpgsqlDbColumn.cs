using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlDbColumn" /> class.
        /// </summary>
        public NpgsqlDbColumn()
        {
            PostgresType = UnknownBackendType.Instance;

            // Not supported in PostgreSQL
            IsExpression = false;
            IsAliased = false;
            IsHidden = false;
            IsIdentity = false;
        }

        internal NpgsqlDbColumn Clone() =>
            Unsafe.As<NpgsqlDbColumn>(MemberwiseClone());

        #region Standard fields
        // ReSharper disable once InconsistentNaming
        /// <inheritdoc cref="DbColumn.AllowDBNull" />
        public new bool? AllowDBNull
        {
            get => base.AllowDBNull;
            protected internal set => base.AllowDBNull = value;
        }

        /// <inheritdoc cref="DbColumn.BaseCatalogName" />
        public new string BaseCatalogName
        {
            get => base.BaseCatalogName!;
            protected internal set => base.BaseCatalogName = value;
        }

        /// <inheritdoc cref="DbColumn.BaseColumnName" />
        public new string? BaseColumnName
        {
            get => base.BaseColumnName;
            protected internal set => base.BaseColumnName = value;
        }

        /// <inheritdoc cref="DbColumn.BaseSchemaName" />
        public new string? BaseSchemaName
        {
            get => base.BaseSchemaName;
            protected internal set => base.BaseSchemaName = value;
        }

        /// <inheritdoc cref="DbColumn.BaseServerName" />
        public new string BaseServerName
        {
            get => base.BaseServerName!;
            protected internal set => base.BaseServerName = value;
        }

        /// <inheritdoc cref="DbColumn.BaseTableName" />
        public new string? BaseTableName
        {
            get => base.BaseTableName;
            protected internal set => base.BaseTableName = value;
        }

        /// <inheritdoc cref="DbColumn.ColumnName" />
        public new string ColumnName
        {
            get => base.ColumnName;
            protected internal set => base.ColumnName = value;
        }

        /// <inheritdoc cref="DbColumn.ColumnOrdinal" />
        public new int? ColumnOrdinal
        {
            get => base.ColumnOrdinal;
            protected internal set => base.ColumnOrdinal = value;
        }

        /// <inheritdoc cref="DbColumn.ColumnSize" />
        public new int? ColumnSize
        {
            get => base.ColumnSize;
            protected internal set => base.ColumnSize = value;
        }

        /// <inheritdoc cref="DbColumn.IsAliased" />
        public new bool? IsAliased
        {
            get => base.IsAliased;
            protected internal set => base.IsAliased = value;
        }

        /// <inheritdoc cref="DbColumn.IsAutoIncrement" />
        public new bool? IsAutoIncrement
        {
            get => base.IsAutoIncrement;
            protected internal set => base.IsAutoIncrement = value;
        }

        /// <inheritdoc cref="DbColumn.IsKey" />
        public new bool? IsKey
        {
            get => base.IsKey;
            protected internal set => base.IsKey = value;
        }

        /// <inheritdoc cref="DbColumn.IsLong" />
        public new bool? IsLong
        {
            get => base.IsLong;
            protected internal set => base.IsLong = value;
        }

        /// <inheritdoc cref="DbColumn.IsReadOnly" />
        public new bool? IsReadOnly
        {
            get => base.IsReadOnly;
            protected internal set => base.IsReadOnly = value;
        }

        /// <inheritdoc cref="DbColumn.IsUnique" />
        public new bool? IsUnique
        {
            get => base.IsUnique;
            protected internal set => base.IsUnique = value;
        }

        /// <inheritdoc cref="DbColumn.NumericPrecision" />
        public new int? NumericPrecision
        {
            get => base.NumericPrecision;
            protected internal set => base.NumericPrecision = value;
        }

        /// <inheritdoc cref="DbColumn.NumericScale" />
        public new int? NumericScale
        {
            get => base.NumericScale;
            protected internal set => base.NumericScale = value;
        }

        /// <inheritdoc cref="DbColumn.UdtAssemblyQualifiedName" />
        public new string? UdtAssemblyQualifiedName
        {
            get => base.UdtAssemblyQualifiedName;
            protected internal set => base.UdtAssemblyQualifiedName = value;
        }

        /// <inheritdoc cref="DbColumn.DataType" />
        public new Type? DataType
        {
            get => base.DataType;
            protected internal set => base.DataType = value;
        }

        /// <inheritdoc cref="DbColumn.DataTypeName" />
        public new string DataTypeName
        {
            get => base.DataTypeName!;
            protected internal set => base.DataTypeName = value;
        }

        #endregion Standard fields

        #region Npgsql-specific fields

        /// <summary>
        /// The <see cref="PostgresType" /> describing the type of this column.
        /// </summary>
        public PostgresType PostgresType { get; internal set; }

        /// <summary>
        /// The OID of the type of this column in the PostgreSQL pg_type catalog table.
        /// </summary>
        public uint TypeOID { get; internal set; }

        /// <summary>
        /// The OID of the PostgreSQL table of this column.
        /// </summary>
        public uint TableOID { get; internal set; }

        /// <summary>
        /// The column's position within its table. Note that this is different from <see cref="ColumnOrdinal" />,
        /// which is the column's position within the resultset.
        /// </summary>
        public short? ColumnAttributeNumber { get; internal set; }

        /// <summary>
        /// The default SQL expression for this column.
        /// </summary>
        public string? DefaultValue { get; internal set; }

        /// <summary>
        /// The <see cref="NpgsqlDbType" /> value for this column's type.
        /// </summary>
        public NpgsqlDbType? NpgsqlDbType { get; internal set; }

        /// <inheritdoc />
        public override object? this[string propertyName]
            => propertyName switch
            {
                nameof(PostgresType)          => PostgresType,
                nameof(TypeOID)               => TypeOID,
                nameof(TableOID)              => TableOID,
                nameof(ColumnAttributeNumber) => ColumnAttributeNumber,
                nameof(DefaultValue)          => DefaultValue,
                nameof(NpgsqlDbType)          => NpgsqlDbType,
                _                             => base[propertyName]
            };

        #endregion Npgsql-specific fields
    }
}
