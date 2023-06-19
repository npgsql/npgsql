namespace Npgsql.PostgresTypes;

// Oid is a uint, easy to check in psql SELECT (2147483648 + 1)::oid succeeds while it fails for ::int4.
public readonly record struct Oid(uint Value)
{
    public static explicit operator uint(Oid oid) => oid.Value;
    public static implicit operator Oid(uint oid) => new(oid);
}
