using System;

namespace Npgsql.Internal.Postgres;

public readonly struct Oid(uint value) : IEquatable<Oid>
{
    public static explicit operator uint(Oid oid) => oid.Value;
    public static implicit operator Oid(uint oid) => new(oid);
    public uint Value { get; init; } = value;

    public override string ToString() => Value.ToString();
    public bool Equals(Oid other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Oid other && Equals(other);
    public override int GetHashCode() => (int)Value;
    public static bool operator ==(Oid left, Oid right) => left.Equals(right);
    public static bool operator !=(Oid left, Oid right) => !left.Equals(right);
}
