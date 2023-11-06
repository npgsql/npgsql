
using Npgsql.Internal.Postgres;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represents a PostgreSQL base data type, which is a simple scalar value.
/// </summary>
public class PostgresBaseType : PostgresType
{
    /// <summary>
    /// Constructs a representation of a PostgreSQL base data type.
    /// </summary>
    protected internal PostgresBaseType(string ns, string name, uint oid)
        : base(ns, name, oid) {}

    /// <summary>
    /// Constructs a representation of a PostgreSQL base data type.
    /// </summary>
    internal PostgresBaseType(DataTypeName dataTypeName, Oid oid)
        : base(dataTypeName, oid) {}

    /// <inheritdoc/>
    internal override string GetPartialNameWithFacets(int typeModifier)
    {
        var facets = GetFacets(typeModifier);
        if (facets == PostgresFacets.None)
            return Name;

        return Name switch
        {
            // Special case for time, timestamp, timestamptz and timetz where the facet is embedded in the middle
            "timestamp without time zone" => $"timestamp{facets} without time zone",
            "time without time zone"      => $"time{facets} without time zone",
            "timestamp with time zone"    => $"timestamp{facets} with time zone",
            "time with time zone"         => $"time{facets} with time zone",

            // We normalize character(1) to character - they mean the same
            "character" when facets.Size == 1 => "character",

            _                             => $"{Name}{facets}"
        };
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
            // See https://stackoverflow.com/questions/3350148/where-are-numeric-precision-and-scale-for-a-field-found-in-the-pg-catalog-tables
            var precision = ((typeModifier - 4) >> 16) & 65535;
            var scale = (typeModifier - 4) & 65535;
            return new PostgresFacets(null, precision, scale == 0 ? (int?)null : scale);
        case "timestamp without time zone":
        case "time without time zone":
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
}
