using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
#if NETSTANDARD1_3
using System.Data.Common;
#endif

#pragma warning disable 1591

namespace Npgsql.Schema
{
    /// <summary>
    /// Provides schema information about a column.
    /// (e.g. SELECT 8);
    /// </summary>
    /// <remarks>
    /// Note that this can correspond to a field returned in a query which isn't an actual table column
    ///
    /// See https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
    /// for information on the meaning of the different fields.
    /// </remarks>
    public class NpgsqlDbColumn : DbColumn
    {
        internal NpgsqlDbColumn()
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
            get { return base.AllowDBNull; }
            internal set { base.AllowDBNull = value; }
        }

        public new string BaseCatalogName
        {
            get { return base.BaseCatalogName; }
            internal set { base.BaseCatalogName = value; }
        }

        public new string BaseColumnName
        {
            get { return base.BaseColumnName; }
            internal set { base.BaseColumnName = value; }
        }

        public new string BaseSchemaName
        {
            get { return base.BaseSchemaName; }
            internal set { base.BaseSchemaName = value; }
        }

        public new string BaseServerName
        {
            get { return base.BaseServerName; }
            internal set { base.BaseServerName = value; }
        }

        public new string BaseTableName
        {
            get { return base.BaseTableName; }
            internal set { base.BaseTableName = value; }
        }

        public new string ColumnName
        {
            get { return base.ColumnName; }
            internal set { base.ColumnName = value; }
        }

        public new int? ColumnOrdinal
        {
            get { return base.ColumnOrdinal; }
            internal set { base.ColumnOrdinal = value; }
        }

        public new int? ColumnSize
        {
            get { return base.ColumnSize; }
            internal set { base.ColumnSize = value; }
        }

        public new bool? IsAutoIncrement
        {
            get { return base.IsAutoIncrement; }
            internal set { base.IsAutoIncrement = value; }
        }

        public new bool? IsKey
        {
            get { return base.IsKey; }
            internal set { base.IsKey = value; }
        }

        public new bool? IsLong
        {
            get { return base.IsLong; }
            internal set { base.IsLong = value; }
        }

        public new bool? IsReadOnly
        {
            get { return base.IsReadOnly; }
            internal set { base.IsReadOnly = value; }
        }

        public new bool? IsUnique
        {
            get { return base.IsUnique; }
            internal set { base.IsUnique = value; }
        }

        public new int? NumericPrecision
        {
            get { return base.NumericPrecision; }
            internal set { base.NumericPrecision = value; }
        }

        public new int? NumericScale
        {
            get { return base.NumericScale; }
            internal set { base.NumericScale = value; }
        }

        public new string UdtAssemblyQualifiedName
        {
            get { return base.UdtAssemblyQualifiedName; }
            internal set { base.UdtAssemblyQualifiedName = value; }
        }

        public new Type DataType
        {
            get { return base.DataType; }
            internal set { base.DataType = value; }
        }

        public new string DataTypeName
        {
            get { return base.DataTypeName; }
            internal set { base.DataTypeName = value; }
        }

        #endregion Standard fields

        #region Npgsql-specific fields

        public uint TypeOID { get; internal set; }
        public uint TableOID { get; internal set; }
        public short? ColumnAttributeNumber { get; internal set; }
        public string DefaultValue { get; internal set; }

        public override object this[string property]
        {
            get
            {
                switch (property)
                {
                case nameof(TypeOID):
                    return TypeOID;
                case nameof(TableOID):
                    return TableOID;
                case nameof(ColumnAttributeNumber):
                    return ColumnAttributeNumber;
                case nameof(DefaultValue):
                    return DefaultValue;
                }

                return base[property];
            }
        }

        #endregion Npgsql-specific fields
    }
}
