namespace Npgsql.PostgresTypes;

enum PostgresTypeKind
{
    /// A base type.
    Base,
    /// An enum carying its variants.
    Enum,
    /// A pseudo type like anyarray.
    Pseudo,
    // An array carying its element type.
    Array,
    // A range carying its element type.
    Range,
    // A multi-range carying its element type.
    Multirange,
    // A domain carying its underlying type.
    Domain,
    // A composite carying its constituent fields.
    Composite
}
