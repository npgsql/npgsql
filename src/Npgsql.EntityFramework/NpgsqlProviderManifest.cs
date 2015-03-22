using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
#if ENTITIES6
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Metadata.Edm;
#endif
using System.Xml;
using System.Data;

namespace Npgsql
{
    internal class NpgsqlProviderManifest : DbXmlEnabledProviderManifest
    {
        public NpgsqlProviderManifest(string serverVersion)
            : base(CreateXmlReaderForResource("Npgsql.NpgsqlProviderManifest.Manifest.xml"))
        {
        }

        protected override XmlReader GetDbInformation(string informationType)
        {
            XmlReader xmlReader = null;

            if (informationType == StoreSchemaDefinition)
            {
                xmlReader = CreateXmlReaderForResource("Npgsql.NpgsqlSchema.ssdl");
            }
#if NET45
            else if (informationType == StoreSchemaDefinitionVersion3)
            {
                xmlReader = CreateXmlReaderForResource("Npgsql.NpgsqlSchemaV3.ssdl");
            }
#endif
            else if (informationType == StoreSchemaMapping)
            {
                xmlReader = CreateXmlReaderForResource("Npgsql.NpgsqlSchema.msl");
            }

            if (xmlReader == null)
                throw new ArgumentOutOfRangeException("informationType");

            return xmlReader;
        }

        private const string MaxLengthFacet = "MaxLength";
        private const string ScaleFacet = "Scale";
        private const string PrecisionFacet = "Precision";
        private const string FixedLengthFacet = "FixedLength";

        internal static DbType GetDbType(PrimitiveTypeKind _primitiveType)
        {
            switch (_primitiveType)
            {
                case PrimitiveTypeKind.Binary:
                    return DbType.Binary;
                case PrimitiveTypeKind.Boolean:
                    return DbType.Boolean;
                case PrimitiveTypeKind.Byte:
                    return DbType.Byte;
                case PrimitiveTypeKind.SByte:
                    return DbType.SByte;
                case PrimitiveTypeKind.DateTime:
                    return DbType.DateTime;
                case PrimitiveTypeKind.DateTimeOffset:
                    return DbType.DateTimeOffset;
                case PrimitiveTypeKind.Decimal:
                    return DbType.Decimal;
                case PrimitiveTypeKind.Double:
                    return DbType.Double;
                case PrimitiveTypeKind.Int16:
                    return DbType.Int16;
                case PrimitiveTypeKind.Int32:
                    return DbType.Int32;
                case PrimitiveTypeKind.Int64:
                    return DbType.Int64;
                case PrimitiveTypeKind.Single:
                    return DbType.Single;
                case PrimitiveTypeKind.Time:
                    return DbType.Time;
                case PrimitiveTypeKind.Guid:
                    return DbType.Guid;
                case PrimitiveTypeKind.String:
                    return DbType.String;
                default:
                    return DbType.Object;
            }
        }

        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            if (storeType == null)
                throw new ArgumentNullException("storeType");

            string storeTypeName = storeType.EdmType.Name;
            PrimitiveType primitiveType = StoreTypeNameToEdmPrimitiveType[storeTypeName];
            // TODO: come up with way to determin if unicode is used
            bool isUnicode = true;
            Facet facet;

            switch (storeTypeName)
            {
                case "bool":
                case "int2":
                case "int4":
                case "int8":
                case "float4":
                case "float8":
                case "uuid":
                    return TypeUsage.CreateDefaultTypeUsage(primitiveType);
                case "numeric":
                    {
                        byte scale;
                        byte precision;
                        if (storeType.Facets.TryGetValue(ScaleFacet, false, out facet) &&
                            !facet.IsUnbounded && facet.Value != null)
                        {
                            scale = (byte)facet.Value;
                            if (storeType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                                !facet.IsUnbounded && facet.Value != null)
                            {
                                precision = (byte)facet.Value;
                                return TypeUsage.CreateDecimalTypeUsage(primitiveType, precision, scale);
                            }
                        }
                        return TypeUsage.CreateDecimalTypeUsage(primitiveType);
                    }
                case "bpchar":
                    if (storeType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, true, (int)facet.Value);
                    else
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, true);
                case "varchar":
                    if (storeType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, false, (int)facet.Value);
                    else
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, false);
                case "text":
                case "xml":
                    return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, false);
                case "timestamp":
                    // TODO: make sure the arguments are correct here
                    if (storeType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                    {
                        return TypeUsage.CreateDateTimeTypeUsage(primitiveType, (byte)facet.Value);
                    }
                    else
                    {
                        return TypeUsage.CreateDateTimeTypeUsage(primitiveType, null);
                    }
                case "date":
                    return TypeUsage.CreateDateTimeTypeUsage(primitiveType, 0);
                case "timestamptz":
                    if (storeType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                    {
                        return TypeUsage.CreateDateTimeOffsetTypeUsage(primitiveType, (byte)facet.Value);
                    }
                    else
                    {
                        return TypeUsage.CreateDateTimeOffsetTypeUsage(primitiveType, null);
                    }
                case "time":
                case "interval":
                    if (storeType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                    {
                        return TypeUsage.CreateTimeTypeUsage(primitiveType, (byte)facet.Value);
                    }
                    else
                    {
                        return TypeUsage.CreateTimeTypeUsage(primitiveType, null);
                    }
                case "bytea":
                    {
                        if (storeType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                            !facet.IsUnbounded && facet.Value != null)
                        {
                            return TypeUsage.CreateBinaryTypeUsage(primitiveType, false, (int)facet.Value);
                        }
                        return TypeUsage.CreateBinaryTypeUsage(primitiveType, false);
                    }
                case "rowversion":
                    {
                        return TypeUsage.CreateBinaryTypeUsage(primitiveType, true, 8);
                    }
                    //TypeUsage.CreateBinaryTypeUsage
                    //TypeUsage.CreateDateTimeTypeUsage
                    //TypeUsage.CreateDecimalTypeUsage
                    //TypeUsage.CreateStringTypeUsage
            }
            throw new NotSupportedException("Not supported store type: " + storeTypeName);
        }

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            if (edmType == null)
                throw new ArgumentNullException("edmType");

            PrimitiveType primitiveType = edmType.EdmType as PrimitiveType;
            if (primitiveType == null)
                throw new ArgumentException("Store does not support specified edm type");

            // TODO: come up with way to determin if unicode is used
            bool isUnicode = true;
            Facet facet;

            switch (primitiveType.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Boolean:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["bool"]);
                case PrimitiveTypeKind.Int16:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["int2"]);
                case PrimitiveTypeKind.Int32:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["int4"]);
                case PrimitiveTypeKind.Int64:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["int8"]);
                case PrimitiveTypeKind.Single:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["float4"]);
                case PrimitiveTypeKind.Double:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["float8"]);
                case PrimitiveTypeKind.Decimal:
                    {
                        byte scale;
                        byte precision;
                        if (edmType.Facets.TryGetValue(ScaleFacet, false, out facet) &&
                            !facet.IsUnbounded && facet.Value != null)
                        {
                            scale = (byte)facet.Value;
                            if (edmType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                                !facet.IsUnbounded && facet.Value != null)
                            {
                                precision = (byte)facet.Value;
                                return TypeUsage.CreateDecimalTypeUsage(StoreTypeNameToStorePrimitiveType["numeric"], precision, scale);
                            }
                        }
                        return TypeUsage.CreateDecimalTypeUsage(StoreTypeNameToStorePrimitiveType["numeric"]);
                    }
                case PrimitiveTypeKind.String:
                    {
                        // TODO: could get character, character varying, text
                        if (edmType.Facets.TryGetValue(FixedLengthFacet, false, out facet) &&
                            !facet.IsUnbounded && facet.Value != null && (bool)facet.Value)
                        {
                            PrimitiveType characterPrimitive = StoreTypeNameToStorePrimitiveType["bpchar"];
                            if (edmType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                                !facet.IsUnbounded && facet.Value != null)
                            {
                                return TypeUsage.CreateStringTypeUsage(characterPrimitive, isUnicode, true, (int)facet.Value);
                            }
                            // this may not work well
                            return TypeUsage.CreateStringTypeUsage(characterPrimitive, isUnicode, true);
                        }
                        if (edmType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                            !facet.IsUnbounded && facet.Value != null)
                        {
                            return TypeUsage.CreateStringTypeUsage(StoreTypeNameToStorePrimitiveType["varchar"], isUnicode, false, (int)facet.Value);
                        }
                        // assume text since it is not fixed length and has no max length
                        return TypeUsage.CreateStringTypeUsage(StoreTypeNameToStorePrimitiveType["text"], isUnicode, false);
                    }
                case PrimitiveTypeKind.DateTime:
                    if (edmType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                    {
                        return TypeUsage.CreateDateTimeTypeUsage(StoreTypeNameToStorePrimitiveType["timestamp"], (byte)facet.Value);
                    }
                    else
                    {
                        return TypeUsage.CreateDateTimeTypeUsage(StoreTypeNameToStorePrimitiveType["timestamp"], null);
                    }
                case PrimitiveTypeKind.DateTimeOffset:
                    if (edmType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                    {
                        return TypeUsage.CreateDateTimeOffsetTypeUsage(StoreTypeNameToStorePrimitiveType["timestamptz"], (byte)facet.Value);
                    }
                    else
                    {
                        return TypeUsage.CreateDateTimeOffsetTypeUsage(StoreTypeNameToStorePrimitiveType["timestamptz"], null);
                    }
                case PrimitiveTypeKind.Time:
                    if (edmType.Facets.TryGetValue(PrecisionFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                    {
                        return TypeUsage.CreateTimeTypeUsage(StoreTypeNameToStorePrimitiveType["interval"], (byte)facet.Value);
                    }
                    else
                    {
                        return TypeUsage.CreateTimeTypeUsage(StoreTypeNameToStorePrimitiveType["interval"], null);
                    }
                case PrimitiveTypeKind.Binary:
                    {
                        if (edmType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                            !facet.IsUnbounded && facet.Value != null)
                        {
                            return TypeUsage.CreateBinaryTypeUsage(StoreTypeNameToStorePrimitiveType["bytea"], false, (int)facet.Value);
                        }
                        return TypeUsage.CreateBinaryTypeUsage(StoreTypeNameToStorePrimitiveType["bytea"], false);
                    }
                case PrimitiveTypeKind.Guid:
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["uuid"]);
                case PrimitiveTypeKind.Byte:
                case PrimitiveTypeKind.SByte: 
                    return TypeUsage.CreateDefaultTypeUsage(StoreTypeNameToStorePrimitiveType["int2"]);
            }

            throw new NotSupportedException("Not supported edm type: " + edmType);
        }

        private static XmlReader CreateXmlReaderForResource(string resourceName)
        {
            return XmlReader.Create(System.Reflection.Assembly.GetAssembly(typeof(NpgsqlProviderManifest)).GetManifestResourceStream(resourceName));
        }

        public override bool SupportsEscapingLikeArgument(out char escapeCharacter)
        {
            escapeCharacter = '\\';
            return true;
        }

        public override string EscapeLikeArgument(string argument)
        {
            return argument.Replace("\\","\\\\").Replace("%", "\\%").Replace("_", "\\_");
        }

#if ENTITIES6
        public override bool SupportsInExpression()
        {
            return true;
        }
#endif
    }
}
