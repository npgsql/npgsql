using System;
using System.Diagnostics;

namespace Npgsql.Internal;

public enum SizeKind : byte
{
    Unknown = 0,
    Exact,
    UpperBound
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct Size : IEquatable<Size>
{
    readonly int _value;

    Size(int value, SizeKind kind)
    {
        _value = value;
        Kind = kind;
    }

    public bool IsDefault => Kind == default && _value == default;

    public int Value
        => Kind is SizeKind.Unknown ? throw new InvalidOperationException() : _value;

    public SizeKind Kind { get; }

    public static Size Create(int byteCount) => new(byteCount, SizeKind.Exact);
    public static Size CreateUpperBound(int byteCount) => new(byteCount, SizeKind.UpperBound);
    public static Size Unknown => new(-1, SizeKind.Unknown);
    public static Size Zero => new(0, SizeKind.Exact);

    public Size Combine(Size result)
    {
        if (Kind is SizeKind.Unknown || result.Kind is SizeKind.Unknown)
            return this;

        if (Kind is SizeKind.UpperBound || result.Kind is SizeKind.UpperBound)
            return CreateUpperBound(_value + result._value);

        return Create(_value + result._value);
    }

    public static implicit operator Size(int value) => Create(value);

    string DebuggerDisplay
        => Kind switch
        {
            SizeKind.Exact or SizeKind.UpperBound => $"{_value} ({Kind})",
            SizeKind.Unknown => "Unknown",
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Equals(Size other) => _value == other._value && Kind == other.Kind;
    public override bool Equals(object? obj) => obj is Size other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_value, (int)Kind);
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    public static bool operator !=(Size left, Size right) => !left.Equals(right);
}
