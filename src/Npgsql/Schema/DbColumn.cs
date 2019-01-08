﻿#if NET452

using System;

#pragma warning disable 1591

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
        {
            get
            {
                switch (propertyName)
                {
                case nameof(AllowDBNull):
                    return AllowDBNull;
                case nameof(BaseCatalogName):
                    return BaseCatalogName;
                case nameof(BaseColumnName):
                    return BaseColumnName;
                case nameof(BaseSchemaName):
                    return BaseSchemaName;
                case nameof(BaseServerName):
                    return BaseServerName;
                case nameof(BaseTableName):
                    return BaseTableName;
                case nameof(ColumnName):
                    return ColumnName;
                case nameof(ColumnOrdinal):
                    return ColumnOrdinal;
                case nameof(ColumnSize):
                    return ColumnSize;
                case nameof(IsAliased):
                    return IsAliased;
                case nameof(IsAutoIncrement):
                    return IsAutoIncrement;
                case nameof(IsExpression):
                    return IsExpression;
                case nameof(IsHidden):
                    return IsHidden;
                case nameof(IsIdentity):
                    return IsIdentity;
                case nameof(IsKey):
                    return IsKey;
                case nameof(IsLong):
                    return IsLong;
                case nameof(IsReadOnly):
                    return IsReadOnly;
                case nameof(IsUnique):
                    return IsUnique;
                case nameof(NumericPrecision):
                    return NumericPrecision;
                case nameof(NumericScale):
                    return NumericScale;
                case nameof(UdtAssemblyQualifiedName):
                    return UdtAssemblyQualifiedName;
                case nameof(DataType):
                    return DataType;
                case nameof(DataTypeName):
                    return DataTypeName;
                default:
                    return null;
                }
            }
        }
    }
}

#endif
