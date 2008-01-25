#if ENTITIES
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.Xml;

namespace Npgsql
{
    internal class NpgsqlProviderManifest : DbXmlEnabledProviderManifest
	{
        public NpgsqlProviderManifest()
            : base(CreateXmlReaderForResource("Npgsql.NpgsqlProviderManifest.Manifest.xml"))
        {
        }

        protected override XmlReader GetDbInformation(string informationType)
        {
            XmlReader xmlReader = null;
            if (informationType == ConceptualSchemaDefinition)
            {
            }
            else if (informationType == StoreSchemaDefinition)
            {
            }
            else if (informationType == StoreSchemaMapping)
            {
            }
            return xmlReader;
        }

        private const string MaxLengthFacet = "MaxLength";
        private const string ScaleFacet = "Scale";
        private const string PrecisionFacet = "Precision";

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
                case "boolean":
                case "smallint":
                case "integer":
                case "bigint":
                case "real":
                    return TypeUsage.CreateDefaultTypeUsage(primitiveType);
                case "number":
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
                case "character":
                    if (storeType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, true, (int)facet.Value);
                    else
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, true);
                case "character varying":
                    if (storeType.Facets.TryGetValue(MaxLengthFacet, false, out facet) &&
                        !facet.IsUnbounded && facet.Value != null)
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, false, (int)facet.Value);
                    else
                        return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, false);
                case "text":
                    return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, false);
                case "timestamp without time zone":
                    // TODO: make sure the arguments are correct here
                    return TypeUsage.CreateDateTimeTypeUsage(primitiveType, true, DateTimeKind.Unspecified);
                case "bytea":
                    return TypeUsage.CreateBinaryTypeUsage(primitiveType, false);
                    //TypeUsage.CreateBinaryTypeUsage
                    //TypeUsage.CreateDateTimeTypeUsage
                    //TypeUsage.CreateDecimalTypeUsage
                    //TypeUsage.CreateStringTypeUsage
            }
            throw new NotSupportedException();
        }

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            throw new NotImplementedException();
        }

        public override string Token
        {
            get { throw new NotImplementedException(); }
        }

        private static XmlReader CreateXmlReaderForResource(string resourceName)
        {
            return XmlReader.Create(System.Reflection.Assembly.GetAssembly(typeof(NpgsqlProviderManifest)).GetManifestResourceStream(resourceName));
        }

        //private XmlReader GetDbInformation(string informationType, DbConnection connection)
        //{
        //    if (connection == null)
        //        throw new ArgumentNullException("connection");

        //    XmlReader xmlReader = null;
        //    if (informationType == DbProviderServices.ProviderManifest)
        //    {
        //        xmlReader = CreateXmlReaderForResource("Npgsql.NpgsqlServices.Manifest.xml");
        //    }
        //    else if (informationType == DbProviderServices.StoreSchemaDefinition)
        //    {
        //    }
        //    else if (informationType == DbProviderServices.StoreSchemaMapping)
        //    {
        //    }
        //    else if (informationType == DbProviderServices.ConceptualSchemaDefinition)
        //    {
        //    }

        //    return xmlReader;
        //}
    }
}
#endif