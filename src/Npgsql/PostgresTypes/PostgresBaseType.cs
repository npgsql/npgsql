﻿namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL base data type, which is a simple scalar value.
    /// </summary>
    public class PostgresBaseType : PostgresType
    {
        /// <inheritdoc/>
        protected internal PostgresBaseType(string ns, string internalName, uint oid)
            : base(ns, TranslateInternalName(internalName), internalName, oid)
        {}

        /// <inheritdoc/>
        internal override string GetPartialNameWithFacets(int typeModifier)
        {
            var facets = GetFacets(typeModifier);
            if (facets == PostgresFacets.None)
                return Name;

            switch (Name)
            {
            // Special case for timestamptz and timetz where the facet is embedded in the middle
            case "timestamp with time zone":
                return $"timestamp{facets} with time zone";
            case "time with time zone":
                return $"time{facets} with time zone";
            default:
                return $"{Name}{facets}";
            }
        }

        internal override PostgresFacets GetFacets(int typeModifier)
        {
            if (typeModifier == -1)
                return PostgresFacets.None;

            switch (Name)
            {
            case "character":
                return new PostgresFacets(typeModifier - 4, null, null);
            case "character varying":
                return new PostgresFacets(typeModifier - 4, null, null);  // Max length
            case "numeric":
            case "decimal":
                // See http://stackoverflow.com/questions/3350148/where-are-numeric-precision-and-scale-for-a-field-found-in-the-pg-catalog-tables
                var precision = ((typeModifier - 4) >> 16) & 65535;
                var scale = (typeModifier - 4) & 65535;
                return new PostgresFacets(null, precision, scale == 0 ? (int?)null : scale);
            case "timestamp":
            case "time":
            case "interval":
                precision = typeModifier & 0xFFFF;
                return new PostgresFacets(null, precision, null);
            case "timestamp with time zone":
                precision = typeModifier & 0xFFFF;
                return new PostgresFacets(null, precision, null);
            case "time with time zone":
                precision = typeModifier & 0xFFFF;
                return new PostgresFacets(null, precision, null);
            case "bit":
            case "bit varying":
                return new PostgresFacets(typeModifier, null, null);
            default:
                return PostgresFacets.None;
            }
        }

        // The type names returned by PostgreSQL are internal names (int4 instead of
        // integer). We perform translation to the user-facing standard names.
        // https://www.postgresql.org/docs/current/static/datatype.html#DATATYPE-TABLE
        static string TranslateInternalName(string internalName)
        {
            switch (internalName)
            {
            case "bool":
                return "boolean";
            case "bpchar":
                return "character";
            case "decimal":
                return "numeric";
            case "float4":
                return "real";
            case "float8":
                return "double precision";
            case "int2":
                return "smallint";
            case "int4":
                return "integer";
            case "int8":
                return "bigint";
            case "timetz":
                return "time with time zone";
            case "timestamptz":
                return "timestamp with time zone";
            case "varbit":
                return "bit varying";
            case "varchar":
                return "character varying";
            default:
                return internalName;
            }
        }
    }
}
