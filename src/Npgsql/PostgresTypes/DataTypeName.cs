using System;
using System.Diagnostics;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represented the fully-qualified name of a PostgreSQL type.
/// </summary>
[DebuggerDisplay("{DisplayName,nq}")]
public readonly struct DataTypeName : IEquatable<DataTypeName>
{
    /// <summary>
    /// The maximum length of names in an unmodified PostgreSQL installation.
    /// </summary>
    /// <remarks>
    /// We need to respect this to get to valid names when deriving them (for multirange/arrays etc).
    /// This does not include the namespace.
    /// </remarks>
    internal const int NAMEDATALEN = 64 - 1; // Minus null terminator.

    readonly string _value;

    DataTypeName(string fullyQualifiedDataTypeName, bool validated)
    {
        if (!validated)
        {
            var schemaEndIndex = fullyQualifiedDataTypeName.LastIndexOf('.');
            if (schemaEndIndex == -1)
                throw new ArgumentException("Given value does not contain a schema.", nameof(fullyQualifiedDataTypeName));

            var typeNameLength = fullyQualifiedDataTypeName.Length - schemaEndIndex + 1;
            if (typeNameLength > NAMEDATALEN)
            {
                if (fullyQualifiedDataTypeName.EndsWith("[]", StringComparison.Ordinal) &&
                    typeNameLength == NAMEDATALEN + "[]".Length - "_".Length)
                {
                    throw new ArgumentException(
                        $"Name is too long and would be truncated to: {fullyQualifiedDataTypeName.Substring(0, fullyQualifiedDataTypeName.Length - typeNameLength + NAMEDATALEN)}");
                }
            }
        }

        _value = fullyQualifiedDataTypeName;
    }

    public DataTypeName(string fullyQualifiedDataTypeName)
        : this(fullyQualifiedDataTypeName, validated: false) { }

    internal static DataTypeName ValidatedName(string fullyQualifiedDataTypeName)
        => new(fullyQualifiedDataTypeName, validated: true);

    // Includes schema unless it's pg_catalog.
    public string DisplayName =>
        Value.StartsWith("pg_catalog", StringComparison.Ordinal)
            ? ToDisplayName(UnqualifiedNameSpan)
            : Schema + "." + ToDisplayName(UnqualifiedNameSpan);

    public string Schema => Value.Substring(0, _value.IndexOf('.'));
    internal ReadOnlySpan<char> UnqualifiedNameSpan => Value.AsSpan().Slice(_value.IndexOf('.') + 1);
    public string UnqualifiedName => Value.Substring(_value.IndexOf('.') + 1);
    public string Value => ValueOrThrowIfDefault();

    public static explicit operator string(DataTypeName value) => value.Value;

    public bool IsDefault => _value is null;

    string ValueOrThrowIfDefault()
    {
        return _value is null ? Throw() : _value;

        static string Throw() =>
            throw new InvalidOperationException($"This operation cannot be performed on a default instance of {nameof(DataTypeName)}.");
    }

    internal static DataTypeName CreateFullyQualifiedName(string dataTypeName)
        => dataTypeName.IndexOf('.') != -1 ? new(dataTypeName) : new("pg_catalog." + dataTypeName);

    // Static transform as defined by https://www.postgresql.org/docs/current/sql-createtype.html#SQL-CREATETYPE-ARRAY
    // We don't have to deal with [] as we're always starting from a normalized fully qualified name.
    public DataTypeName ToArrayName()
    {
        var unqualifiedNameSpan = UnqualifiedNameSpan;
        if (unqualifiedNameSpan.StartsWith("_".AsSpan(), StringComparison.Ordinal))
            return this;

        var unqualifiedName = unqualifiedNameSpan.ToString();
        if (unqualifiedName.Length + "_".Length > NAMEDATALEN)
            unqualifiedName = unqualifiedName.Substring(0, NAMEDATALEN - "_".Length);

        return new(Schema + "._" + unqualifiedName);
    }

    // Static transform as defined by https://www.postgresql.org/docs/current/sql-createtype.html#SQL-CREATETYPE-RANGE
    // Manual testing on PG confirmed it's only the first occurence of 'range' that gets replaced.
    public DataTypeName ToDefaultMultirangeName()
    {
        var unqualifiedNameSpan = UnqualifiedNameSpan;
        if (UnqualifiedNameSpan.IndexOf("multirange".AsSpan(), StringComparison.Ordinal) != -1)
            return this;

        var unqualifiedName = unqualifiedNameSpan.ToString();
        var rangeIndex = unqualifiedName.IndexOf("range", StringComparison.Ordinal);
        if (rangeIndex != -1)
        {
            var str = unqualifiedName.Substring(0, rangeIndex) + "multirange" + unqualifiedName.Substring(rangeIndex + "range".Length);

            if (unqualifiedName.Length + "multi".Length > NAMEDATALEN)
                return new(str.Substring(0, NAMEDATALEN - "multi".Length));

            return new(Schema + "." + str);
        }

        if (unqualifiedName.Length + "_multirange".Length > NAMEDATALEN)
            return new(Schema + "." + unqualifiedName.Substring(0, NAMEDATALEN - "_multirange".Length) + "_multirange");

        return new(Schema + "." + unqualifiedName + "_multirange");
    }


    // Create a DataTypeName from a broader range of valid names.
    // including SQL aliases like 'timestamp without time zone', trailing facet info etc.
    public static DataTypeName FromDisplayName(string displayDataTypeName)
    {
        var displayNameSpan = displayDataTypeName.AsSpan().Trim();

        // If we have a schema we're done, Postgres doesn't do display name conversions on fully qualified names.
        // There is one exception and that's array syntax, which is always resolvable in both ways, while we want the canonical name.
        if (displayNameSpan.IndexOf('.') != -1 && !displayNameSpan.EndsWith("[]"))
            return new(displayDataTypeName);

        // First we strip the schema to get the type name.
        var schemaEndIndex = displayNameSpan.IndexOf('.');
        if (schemaEndIndex != -1)
            displayNameSpan = displayNameSpan.Slice(schemaEndIndex + 1);

        // Then we strip either of the two valid array representations to get the base type name (with or without facets).
        var isArray = false;
        if (displayNameSpan.StartsWith("_".AsSpan()))
        {
            isArray = true;
            displayNameSpan = displayNameSpan.Slice(1);
        }
        else if (displayNameSpan.EndsWith("[]".AsSpan()))
        {
            isArray = true;
            displayNameSpan = displayNameSpan.Slice(0, displayNameSpan.Length - 2);
        }

        // Finally we strip the facet info.
        var parenIndex = displayNameSpan.IndexOf('(');
        if (parenIndex > -1)
            displayNameSpan = displayNameSpan.Slice(0, parenIndex);

        // Map any aliases to the internal type name.
        var mapped = displayNameSpan.ToString() switch
        {
            "boolean" => "bool",
            "character" => "bpchar",
            "decimal" => "numeric",
            "real" => "float4",
            "double precision" => "float8",
            "smallint" => "int2",
            "integer" => "int4",
            "bigint" => "int8",
            "time without time zone" => "time",
            "timestamp without time zone" => "timestamp",
            "time with time zone" => "timetz",
            "timestamp with time zone" => "timestamptz",
            "bit varying" => "varbit",
            "character varying" => "varchar",
            var value => value
        };

        // And concat with pg_catalog to get a fully qualified name for our constructor.
        return new((schemaEndIndex is -1 ? "pg_catalog" : displayDataTypeName.Substring(0, schemaEndIndex)) + "." + (isArray ? "_" : "") + mapped);
    }

    // The type names stored in a DataTypeName are usually the actual typname from the pg_type column.
    // There are some canonical aliases defined in the SQL standard which we take into account.
    // Additionally array types have a '_' prefix while for readability their element type should be postfixed with '[]'.
    // See the table for all the aliases https://www.postgresql.org/docs/current/static/datatype.html#DATATYPE-TABLE
    // Alternatively some of the source lives at https://github.com/postgres/postgres/blob/c8e1ba736b2b9e8c98d37a5b77c4ed31baf94147/src/backend/utils/adt/format_type.c#L186
    static string ToDisplayName(ReadOnlySpan<char> unqualifiedName)
    {
        var prefixedArrayType = unqualifiedName.IndexOf('_') == 0;
        var postfixedArrayType = unqualifiedName.EndsWith("[]".AsSpan(), StringComparison.Ordinal);
        string baseTypeName;
        if (prefixedArrayType)
            baseTypeName = unqualifiedName.Slice(1).ToString();
        else if (postfixedArrayType)
            baseTypeName = unqualifiedName.Slice(0, unqualifiedName.Length - 2).ToString();
        else
            baseTypeName = unqualifiedName.ToString();

        var mappedBaseType = baseTypeName switch
        {
            "bool" => "boolean",
            "bpchar" => "character",
            "decimal" => "numeric",
            "float4" => "real",
            "float8" => "double precision",
            "int2" => "smallint",
            "int4" => "integer",
            "int8" => "bigint",
            "time" => "time without time zone",
            "timestamp" => "timestamp without time zone",
            "timetz" => "time with time zone",
            "timestamptz" => "timestamp with time zone",
            "varbit" => "bit varying",
            "varchar" => "character varying",
            _ => baseTypeName
        };

        if (prefixedArrayType || postfixedArrayType)
            return mappedBaseType + "[]";

        return mappedBaseType;
    }

    internal static bool IsFullyQualified(ReadOnlySpan<char> dataTypeName) => dataTypeName.Contains(".".AsSpan(), StringComparison.Ordinal);

    public override string ToString() => Value;
    public bool Equals(DataTypeName other) => !IsDefault && !other.IsDefault && _value == other._value;
    public override bool Equals(object? obj) => obj is DataTypeName other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public static bool operator ==(DataTypeName left, DataTypeName right) => left.Equals(right);
    public static bool operator !=(DataTypeName left, DataTypeName right) => !left.Equals(right);
}


