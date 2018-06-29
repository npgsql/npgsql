using System;
using System.Collections;
using System.Data;
using NpgsqlTypes;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Npgsql.VSIX
{
    internal class NpgsqlDataObjectConceptMapper : IVsDataMappedObjectConverter
    {
        #region NpgsqlColumnDataTypes
        /// <summary>
        /// Npgsql column data type
        /// </summary>
        private static class NpgsqlColumnDataTypes
        {
            public const string OIDVECTOR = "OIDVECTOR";
            public const string UNKNOWN = "UNKNOWN";
            public const string REFCURSOR = "REFCURSOR";
            public const string CHAR = "CHAR";
            public const string BPCHAR = "BPCHAR";
            public const string NBPCHAR = "NBPCHAR";
            public const string VARCHAR = "VARCHAR";
            public const string NVARCHAR = "NVARCHAR";
            public const string TEXT = "TEXT";
            public const string NAME = "NAME";
            public const string BYTEA = "BYTEA";
            public const string BIT = "BIT";
            public const string BOOL = "BOOL";
            public const string INT2 = "INT2";
            public const string INT4 = "INT4";
            public const string INT8 = "INT8";
            public const string OID = "OID";
            public const string FLOAT4 = "FLOAT4";
            public const string FLOAT8 = "FLOAT8";
            public const string NUMERIC = "NUMERIC";
            public const string INET = "INET";
            public const string MACADDR = "MACADDR";
            public const string MONEY = "MONEY";
            public const string POINT = "POINT";
            public const string LSEG = "LSEG";
            public const string PATH = "PATH";
            public const string BOX = "BOX";
            public const string CIRCLE = "CIRCLE";
            public const string POLYGON = "POLYGON";
            public const string UUID = "UUID";
            public const string XML = "XML";
            public const string INTERVAL = "INTERVAL";
            public const string DATE = "DATE";
            public const string TIME = "TIME";
            public const string TIMETZ = "TIMETZ";
            public const string TIMESTAMP = "TIMESTAMP";
            public const string TIMESTAMPTZ = "TIMESTAMPTZ";
        }
        #endregion

        #region Private Fields
        private const string PROVIDER_DATATYPE = "ProviderDataType";
        private const string PROVIDER_DBTYPE = "ProviderDbType";
        private const string FRAMEWORK_DATATYPE = "FrameworkDataType";
        private const string CLASS_NAME = "NpgsqlDataObjectConceptMapper";
        private const string METHOD_NAME = "ConvertToMappedMember";
        #endregion

        #region GetTypes
        /// <summary>
        /// Gets the db type of column from data type of column.
        /// </summary>
        /// <param name="nativeType">column data type.</param>
        /// <returns></returns>
        protected virtual DbType GetDbTypeFromNativeType(string nativeType)
        {
            var strType = nativeType.Trim().ToUpper();
            DbType ret;

            switch (strType)
            {
                case NpgsqlColumnDataTypes.OIDVECTOR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.UNKNOWN:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.REFCURSOR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.CHAR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.BPCHAR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.NBPCHAR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.VARCHAR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.NVARCHAR:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.TEXT:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.NAME:
                    ret = DbType.String;
                    break;
                case NpgsqlColumnDataTypes.BYTEA:
                    ret = DbType.Binary;
                    break;
                case NpgsqlColumnDataTypes.BOOL:
                    ret = DbType.Boolean;
                    break;
                case NpgsqlColumnDataTypes.INT2:
                    ret = DbType.Int16;
                    break;
                case NpgsqlColumnDataTypes.INT4:
                    ret = DbType.Int32;
                    break;
                case NpgsqlColumnDataTypes.INT8:
                    ret = DbType.Int64;
                    break;
                case NpgsqlColumnDataTypes.OID:
                    ret = DbType.Int64;
                    break;
                case NpgsqlColumnDataTypes.FLOAT4:
                    ret = DbType.Single;
                    break;
                case NpgsqlColumnDataTypes.FLOAT8:
                    ret = DbType.Double;
                    break;
                case NpgsqlColumnDataTypes.NUMERIC:
                    ret = DbType.Decimal;
                    break;
                case NpgsqlColumnDataTypes.MONEY:
                    ret = DbType.Currency;
                    break;
                case NpgsqlColumnDataTypes.UUID:
                    ret = DbType.Guid;
                    break;
                case NpgsqlColumnDataTypes.XML:
                    ret = DbType.Xml;
                    break;
                case NpgsqlColumnDataTypes.DATE:
                    ret = DbType.Date;
                    break;
                case NpgsqlColumnDataTypes.TIMETZ:
                    ret = DbType.Time;
                    break;
                case NpgsqlColumnDataTypes.TIME:
                    ret = DbType.Time;
                    break;
                case NpgsqlColumnDataTypes.TIMESTAMPTZ:
                    ret = DbType.DateTime;
                    break;
                case NpgsqlColumnDataTypes.TIMESTAMP:
                    ret = DbType.DateTime;
                    break;
                default:
                    ret = DbType.Object;
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Gets the Framework type of column from data type of column.
        /// </summary>
        /// <param name="nativeType">column data type.</param>
        /// <returns></returns>
        protected virtual Type GetFrameworkTypeFromNativeType(string nativeType)
        {
            var strType = nativeType.Trim().ToUpper();
            Type ret;

            switch (strType)
            {
                case NpgsqlColumnDataTypes.OIDVECTOR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.UNKNOWN:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.REFCURSOR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.CHAR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.BPCHAR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.NBPCHAR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.VARCHAR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.NVARCHAR:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.TEXT:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.NAME:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.BYTEA:
                    ret = typeof(byte[]);
                    break;
                case NpgsqlColumnDataTypes.BIT:
                    ret = typeof(BitArray);
                    break;
                case NpgsqlColumnDataTypes.BOOL:
                    ret = typeof(bool);
                    break;
                case NpgsqlColumnDataTypes.INT2:
                    ret = typeof(short);
                    break;
                case NpgsqlColumnDataTypes.INT4:
                    ret = typeof(int);
                    break;
                case NpgsqlColumnDataTypes.INT8:
                    ret = typeof(long);
                    break;
                case NpgsqlColumnDataTypes.OID:
                    ret = typeof(long);
                    break;
                case NpgsqlColumnDataTypes.FLOAT4:
                    ret = typeof(float);
                    break;
                case NpgsqlColumnDataTypes.FLOAT8:
                    ret = typeof(double);
                    break;
                case NpgsqlColumnDataTypes.NUMERIC:
                    ret = typeof(decimal);
                    break;
                case NpgsqlColumnDataTypes.INET:
                    ret = typeof(System.Net.IPAddress);
                    break;
                case NpgsqlColumnDataTypes.MACADDR:
                    ret = typeof(System.Net.NetworkInformation.PhysicalAddress);
                    break;
                case NpgsqlColumnDataTypes.MONEY:
                    ret = typeof(decimal);
                    break;
                case NpgsqlColumnDataTypes.UUID:
                    ret = typeof(Guid);
                    break;
                case NpgsqlColumnDataTypes.XML:
                    ret = typeof(string);
                    break;
                case NpgsqlColumnDataTypes.INTERVAL:
                    ret = typeof(TimeSpan);
                    break;
                case NpgsqlColumnDataTypes.DATE:
                    ret = typeof(DateTime);
                    break;
                case NpgsqlColumnDataTypes.TIME:
                    ret = typeof(DateTime);
                    break;
                case NpgsqlColumnDataTypes.TIMETZ:
                    ret = typeof(DateTimeOffset);
                    break;
                case NpgsqlColumnDataTypes.TIMESTAMP:
                    ret = typeof(DateTime);
                    break;
                case NpgsqlColumnDataTypes.TIMESTAMPTZ:
                    ret = typeof(DateTime);
                    break;
                default:
                    ret = typeof(object);
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Gets the provider type of column from data type of column.
        /// </summary>
        /// <param name="nativeType">column data type.</param>
        /// <returns></returns>
        protected virtual NpgsqlDbType GetProviderTypeFromNativeType(string nativeType)
        {
            var strType = nativeType.Trim().ToUpper();
            NpgsqlDbType ret;

            switch (strType)
            {
                case NpgsqlColumnDataTypes.OIDVECTOR:
                    ret = NpgsqlDbType.Text;
                    break;
                case NpgsqlColumnDataTypes.UNKNOWN:
                    ret = NpgsqlDbType.Text;
                    break;
                case NpgsqlColumnDataTypes.REFCURSOR:
                    ret = NpgsqlDbType.Refcursor;
                    break;
                case NpgsqlColumnDataTypes.CHAR:
                    ret = NpgsqlDbType.Char;
                    break;
                case NpgsqlColumnDataTypes.BPCHAR:
                    ret = NpgsqlDbType.Char;
                    break;
                case NpgsqlColumnDataTypes.NBPCHAR:
                    ret = NpgsqlDbType.NChar;
                    break;
                case NpgsqlColumnDataTypes.VARCHAR:
                    ret = NpgsqlDbType.Varchar;
                    break;
                case NpgsqlColumnDataTypes.NVARCHAR:
                    ret = NpgsqlDbType.NVarchar;
                    break;
                case NpgsqlColumnDataTypes.TEXT:
                    ret = NpgsqlDbType.Text;
                    break;
                case NpgsqlColumnDataTypes.NAME:
                    ret = NpgsqlDbType.Name;
                    break;
                case NpgsqlColumnDataTypes.BYTEA:
                    ret = NpgsqlDbType.Bytea;
                    break;
                case NpgsqlColumnDataTypes.BIT:
                    ret = NpgsqlDbType.Bit;
                    break;
                case NpgsqlColumnDataTypes.BOOL:
                    ret = NpgsqlDbType.Boolean;
                    break;
                case NpgsqlColumnDataTypes.INT2:
                    ret = NpgsqlDbType.Smallint;
                    break;
                case NpgsqlColumnDataTypes.INT4:
                    ret = NpgsqlDbType.Integer;
                    break;
                case NpgsqlColumnDataTypes.INT8:
                    ret = NpgsqlDbType.Bigint;
                    break;
                case NpgsqlColumnDataTypes.FLOAT4:
                    ret = NpgsqlDbType.Real;
                    break;
                case NpgsqlColumnDataTypes.FLOAT8:
                    ret = NpgsqlDbType.Double;
                    break;
                case NpgsqlColumnDataTypes.NUMERIC:
                    ret = NpgsqlDbType.Numeric;
                    break;
                case NpgsqlColumnDataTypes.INET:
                    ret = NpgsqlDbType.Inet;
                    break;
                case NpgsqlColumnDataTypes.MACADDR:
                    ret = NpgsqlDbType.MacAddr;
                    break;
                case NpgsqlColumnDataTypes.MONEY:
                    ret = NpgsqlDbType.Money;
                    break;
                case NpgsqlColumnDataTypes.POINT:
                    ret = NpgsqlDbType.Point;
                    break;
                case NpgsqlColumnDataTypes.LSEG:
                    ret = NpgsqlDbType.LSeg;
                    break;
                case NpgsqlColumnDataTypes.PATH:
                    ret = NpgsqlDbType.Path;
                    break;
                case NpgsqlColumnDataTypes.BOX:
                    ret = NpgsqlDbType.Box;
                    break;
                case NpgsqlColumnDataTypes.CIRCLE:
                    ret = NpgsqlDbType.Circle;
                    break;
                case NpgsqlColumnDataTypes.POLYGON:
                    ret = NpgsqlDbType.Polygon;
                    break;
                case NpgsqlColumnDataTypes.UUID:
                    ret = NpgsqlDbType.Uuid;
                    break;
                case NpgsqlColumnDataTypes.XML:
                    ret = NpgsqlDbType.Xml;
                    break;
                case NpgsqlColumnDataTypes.INTERVAL:
                    ret = NpgsqlDbType.Interval;
                    break;
                case NpgsqlColumnDataTypes.DATE:
                    ret = NpgsqlDbType.Date;
                    break;
                case NpgsqlColumnDataTypes.TIMETZ:
                    ret = NpgsqlDbType.TimeTZ;
                    break;
                case NpgsqlColumnDataTypes.TIME:
                    ret = NpgsqlDbType.Time;
                    break;
                case NpgsqlColumnDataTypes.TIMESTAMPTZ:
                    ret = NpgsqlDbType.TimestampTZ;
                    break;
                case NpgsqlColumnDataTypes.TIMESTAMP:
                    ret = NpgsqlDbType.Timestamp;
                    break;
                default:
                    ret = NpgsqlDbType.Text;
                    break;
            }
            if (strType[0] == '_' || (strType[strType.Length - 2] == ']' && strType[strType.Length - 1] == '['))
            {
                ret = NpgsqlDbType.Array;
            }

            return ret;
        }
        #endregion

        #region Override Methods
        public object ConvertToUnderlyingRestriction(string mappedTypeName, int substitutionValueIndex, object[] mappedRestrictions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Maps one or more data-source-specific values to a specified generic concept.
        /// </summary>
        /// <param name="typeName">The name of the data object type requesting this mapping.</param>
        /// <param name="conceptName">The name of the generic concept (see Concepts Reference Overview).</param>
        /// <param name="values">One or more data source specific values to map into the concept specified by conceptName.</param>
        /// <returns>
        /// Returns the value of the generic concept.
        /// </returns>
        public object ConvertToMappedMember(string typeName, string mappedMemberName, object[] underlyingValues)
        {
            try
            {
                if (typeName == null)
                {
                    throw new NpgsqlException($"A system error occurred. Class = {CLASS_NAME}, \nMethod = {METHOD_NAME}, Location = 1, Data = {typeName}."); 
                }
                if (mappedMemberName == null)
                {
                    throw new NpgsqlException($"A system error occurred. Class = {CLASS_NAME}, \nMethod = {METHOD_NAME}, Location = 2, Data = {mappedMemberName}.");
                }
                if ((underlyingValues == null) || !(underlyingValues[0] is string))
                {
                    throw new NpgsqlException($"A system error occurred. Class = {CLASS_NAME}, \nMethod = {METHOD_NAME}, Location = 3, Data = {underlyingValues}.");
                }
                if (mappedMemberName.Equals(PROVIDER_DATATYPE, StringComparison.OrdinalIgnoreCase))
                {
                    return Convert.ToInt32(GetProviderTypeFromNativeType(underlyingValues[0] as string));
                }
                if (mappedMemberName.Equals(PROVIDER_DBTYPE, StringComparison.OrdinalIgnoreCase))
                {
                    return GetDbTypeFromNativeType(underlyingValues[0] as string);
                }
                if (!mappedMemberName.Equals(FRAMEWORK_DATATYPE, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NpgsqlException($"A system error occurred. Class = {CLASS_NAME}, \nMethod = {METHOD_NAME}, Location = 4, Data = {mappedMemberName}.");
                }
                return GetFrameworkTypeFromNativeType(underlyingValues[0] as string).ToString();

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    #endregion
    }
}
