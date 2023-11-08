namespace Npgsql.PostgresTypes;

enum PostgresTypeKind
{
    /// A base type.
    Base,
    /// An enum carrying its variants.
    Enum,
    /// A pseudo type like anyarray.
    Pseudo,
    // An array carrying its element type.
    Array,
    // A range carrying its element type.
    Range,
    // A multi-range carrying its element type.
    Multirange,
    // A domain carrying its underlying type.
    Domain,
    // A composite carrying its constituent fields.
    Composite
}
