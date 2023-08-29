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
    readonly SizeKind _kind;

    Size(int value, SizeKind kind)
    {
        _value = value;
        _kind = kind;
    }

    public bool IsDefault => _kind == default && _value == default;

    public int Value
    {
        get
        {
            if (_kind is SizeKind.Unknown)
                ThrowHelper.ThrowInvalidOperationException("Cannot get value from default or Unknown kind");
            return _value;
        }
    }

    public SizeKind Kind => _kind;

    public static Size Create(int byteCount) => new(byteCount, SizeKind.Exact);
    public static Size CreateUpperBound(int byteCount) => new(byteCount, SizeKind.UpperBound);
    public static Size Unknown { get; } = new(-1, SizeKind.Unknown);
    public static Size Zero { get; } = new(0, SizeKind.Exact);

    public Size Combine(Size result)
    {
        if (_kind is SizeKind.Unknown || result._kind is SizeKind.Unknown)
            return this;

        if (_kind is SizeKind.UpperBound || result._kind is SizeKind.UpperBound)
            return CreateUpperBound(_value + result._value);

        return Create(_value + result._value);
    }

    public static implicit operator Size(int value) => Create(value);

    string DebuggerDisplay
        => _kind switch
        {
            SizeKind.Exact or SizeKind.UpperBound => $"{_value} ({_kind})",
            SizeKind.Unknown => "Unknown",
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool Equals(Size other) => _value == other._value && _kind == other.Kind;
    public override bool Equals(object? obj) => obj is Size other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_value, (int)_kind);
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    public static bool operator !=(Size left, Size right) => !left.Equals(right);
}
