using Npgsql.PostgresTypes;

namespace Npgsql.Internal.Descriptors;

/// Base field type shared between tables and composites.
public readonly record struct Field(string Name, PgTypeId PgTypeId, int TypeModifier)
{
}
