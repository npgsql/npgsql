using System;
using Npgsql.Internal.Postgres;

namespace Npgsql.PostgresTypes;

/// <summary>
/// Represents a PostgreSQL data type, such as int4 or text, as discovered from pg_type.
/// This class is abstract, see derived classes for concrete types of PostgreSQL types.
/// </summary>
/// <remarks>
/// Instances of this class are shared between connections to the same databases.
/// For more info about what this class and its subclasses represent, see
/// https://www.postgresql.org/docs/current/static/catalog-pg-type.html.
/// </remarks>
public abstract class PostgresType
{
    #region Constructors

    /// <summary>
    /// Constructs a representation of a PostgreSQL data type.
    /// </summary>
    /// <param name="ns">The data type's namespace (or schema).</param>
    /// <param name="name">The data type's name.</param>
    /// <param name="oid">The data type's OID.</param>
    private protected PostgresType(string ns, string name, uint oid)
    {
        DataTypeName = DataTypeName.FromDisplayName(name, ns);
        OID = oid;
        FullName = Namespace + "." + Name;
    }

    /// <summary>
    /// Constructs a representation of a PostgreSQL data type.
    /// </summary>
    /// <param name="dataTypeName">The data type's fully qualified name.</param>
    /// <param name="oid">The data type's OID.</param>
    private protected PostgresType(DataTypeName dataTypeName, Oid oid)
    {
        DataTypeName = dataTypeName;
        OID = oid.Value;
        FullName = Namespace + "." + Name;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The data type's OID - a unique id identifying the data type in a given database (in pg_type).
    /// </summary>
    public uint OID { get; }

    /// <summary>
    /// The data type's namespace (or schema).
    /// </summary>
    public string Namespace => DataTypeName.Schema;

    /// <summary>
    /// The data type's name.
    /// </summary>
    /// <remarks>
    /// Note that this is the standard, user-displayable type name (e.g. integer[]) rather than the internal
    /// PostgreSQL name as it is in pg_type (_int4). See <see cref="InternalName"/> for the latter.
    /// </remarks>
    public string Name => DataTypeName.UnqualifiedDisplayName;

    /// <summary>
    /// The full name of the backend type, including its namespace.
    /// </summary>
    public string FullName { get; }

    internal DataTypeName DataTypeName { get; }

    /// <summary>
    /// A display name for this backend type, including the namespace unless it is pg_catalog (the namespace
    /// for all built-in types).
    /// </summary>
    public string DisplayName => DataTypeName.DisplayName;

    /// <summary>
    /// The data type's internal PostgreSQL name (e.g. <c>_int4</c> not <c>integer[]</c>).
    /// See <see cref="Name"/> for a more user-friendly name.
    /// </summary>
    public string InternalName => DataTypeName.UnqualifiedName;

    /// <summary>
    /// If a PostgreSQL array type exists for this type, it will be referenced here.
    /// Otherwise null.
    /// </summary>
    public PostgresArrayType? Array { get; internal set; }

    /// <summary>
    /// If a PostgreSQL range type exists for this type, it will be referenced here.
    /// Otherwise null.
    /// </summary>
    public PostgresRangeType? Range { get; internal set; }

    #endregion

    internal virtual string GetPartialNameWithFacets(int typeModifier) => Name;

    /// <summary>
    /// Generates the type name including any facts (size, precision, scale), given the PostgreSQL type modifier.
    /// </summary>
    internal string GetDisplayNameWithFacets(int typeModifier)
        => Namespace == "pg_catalog"
            ? GetPartialNameWithFacets(typeModifier)
            : Namespace + '.' + GetPartialNameWithFacets(typeModifier);

    internal virtual PostgresFacets GetFacets(int typeModifier) => PostgresFacets.None;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => DisplayName;

    PostgresType? _representationalType;

    /// Canonizes (nested) domain types to underlying types, does not handle composites.
    internal PostgresType GetRepresentationalType()
    {
        return _representationalType ??= Core(this) ?? throw new InvalidOperationException("Couldn't map type to representational type");

        static PostgresType? Core(PostgresType? postgresType)
            => (postgresType as PostgresDomainType)?.BaseType ?? postgresType switch
            {
                PostgresArrayType { Element: PostgresDomainType domain } => Core(domain.BaseType)?.Array,
                PostgresMultirangeType { Subrange.Subtype: PostgresDomainType domain } => domain.BaseType.Range?.Multirange,
                PostgresRangeType { Subtype: PostgresDomainType domain } => domain.Range,
                var type => type
            };
    }
}
