#if NET461

using System;

#pragma warning disable 1591
#nullable disable

namespace Npgsql.Schema
{
    /// <summary>
    /// A copy of corefx's DbColumn, used only in .NET Framework where we don't have it.
    /// </summary>
    /// <remarks>
    /// See https://github.com/dotnet/corefx/blob/master/src/System.Data.Common/src/System/Data/Common/DbColumn.cs
    /// </remarks>
    public abstract class DbColumn
    {
        // ReSharper disable once InconsistentNaming
        public bool? AllowDBNull { get; protected set; }
        public string BaseCatalogName { get; protected set; }
        public string BaseColumnName { get; protected set; }
        public string BaseSchemaName { get; protected set; }
        public string BaseServerName { get; protected set; }
        public string BaseTableName { get; protected set; }
        public string ColumnName { get; protected set; }
        public int? ColumnOrdinal { get; protected set; }
        public int? ColumnSize { get; protected set; }
        public bool? IsAliased { get; protected set; }
        public bool? IsAutoIncrement { get; protected set; }
        public bool? IsExpression { get; protected set; }
        public bool? IsHidden { get; protected set; }
        public bool? IsIdentity { get; protected set; }
        public bool? IsKey { get; protected set; }
        public bool? IsLong { get; protected set; }
        public bool? IsReadOnly { get; protected set; }
        public bool? IsUnique { get; protected set; }
        public int? NumericPrecision { get; protected set; }
        public int? NumericScale { get; protected set; }
        public string UdtAssemblyQualifiedName { get; protected set; }
        public Type DataType { get; protected set; }
        public string DataTypeName { get; protected set; }

        public virtual object this[string propertyName]
            => propertyName switch
            {
                nameof(AllowDBNull)              => AllowDBNull,
                nameof(BaseCatalogName)          => BaseCatalogName,
                nameof(BaseColumnName)           => BaseColumnName,
                nameof(BaseSchemaName)           => BaseSchemaName,
                nameof(BaseServerName)           => BaseServerName,
                nameof(BaseTableName)            => BaseTableName,
                nameof(ColumnName)               => ColumnName,
                nameof(ColumnOrdinal)            => ColumnOrdinal,
                nameof(ColumnSize)               => ColumnSize,
                nameof(IsAliased)                => IsAliased,
                nameof(IsAutoIncrement)          => IsAutoIncrement,
                nameof(IsExpression)             => IsExpression,
                nameof(IsHidden)                 => IsHidden,
                nameof(IsIdentity)               => IsIdentity,
                nameof(IsKey)                    => IsKey,
                nameof(IsLong)                   => IsLong,
                nameof(IsReadOnly)               => IsReadOnly,
                nameof(IsUnique)                 => IsUnique,
                nameof(NumericPrecision)         => NumericPrecision,
                nameof(NumericScale)             => NumericScale,
                nameof(UdtAssemblyQualifiedName) => UdtAssemblyQualifiedName,
                nameof(DataType)                 => DataType,
                nameof(DataTypeName)             => DataTypeName,
                _                                => (object)null
            };
    }
}

#endif
