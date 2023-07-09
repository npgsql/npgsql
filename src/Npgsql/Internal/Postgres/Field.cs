namespace Npgsql.Internal.Postgres;

/// Base field type shared between tables and composites.
public readonly struct Field
{
    public Field(string name, PgTypeId pgTypeId, int typeModifier)
    {
        Name = name;
        PgTypeId = pgTypeId;
        TypeModifier = typeModifier;
    }

    public string Name { get; init; }
    public PgTypeId PgTypeId { get; init; }
    public int TypeModifier { get; init; }
}
