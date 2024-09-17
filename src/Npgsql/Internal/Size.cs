using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public enum SizeKind
{
    Unknown = 0,
    Exact,
    UpperBound
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct Size : IEquatable<Size>
{
    readonly int _value;
    readonly SizeKind _kind;

    Size(SizeKind kind, int value)
    {
        _value = value;
        _kind = kind;
    }

    public int Value
    {
        get
        {
            if (_kind is SizeKind.Unknown)
                ThrowHelper.ThrowInvalidOperationException("Cannot get value from default or Unknown kind");
            return _value;
        }
    }

    internal int GetValueOrDefault() => _value;

    public SizeKind Kind => _kind;

    public static Size Create(int byteCount) => new(SizeKind.Exact, byteCount);
    public static Size CreateUpperBound(int byteCount) => new(SizeKind.UpperBound, byteCount);
    public static Size Unknown { get; } = new(SizeKind.Unknown, 0);
    public static Size Zero { get; } = new(SizeKind.Exact, 0);

    public bool TryCombine(Size other, out Size result)
    {
        if (_kind is SizeKind.Unknown || other._kind is SizeKind.Unknown)
        {
            result = Unknown;
            return true;
        }

        var sum = unchecked(_value + other._value);
        if ((_value >= 0 && sum < other._value) || (_value < 0 && sum > other._value))
        {
            result = default;
            return false;
        }

        if (_kind is SizeKind.UpperBound || other._kind is SizeKind.UpperBound)
        {
            result = CreateUpperBound(sum);
            return true;
        }

        result = Create(sum);
        return true;
    }

    public Size Combine(Size other)
    {
        if (_kind is SizeKind.Unknown || other._kind is SizeKind.Unknown)
            return Unknown;

        if (_kind is SizeKind.UpperBound || other._kind is SizeKind.UpperBound)
            return CreateUpperBound(checked(_value + other._value));

        return Create(checked(_value + other._value));
    }

    public static implicit operator Size(int value) => Create(value);

    string DebuggerDisplay => ToString();

    public bool Equals(Size other) => _value == other._value && _kind == other.Kind;
    public override bool Equals(object? obj) => obj is Size other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_value, (int)_kind);
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    public static bool operator !=(Size left, Size right) => !left.Equals(right);

    public override string ToString() => _kind switch
    {
        SizeKind.Exact or SizeKind.UpperBound => $"{_value} ({_kind.ToString()})",
        SizeKind.Unknown => nameof(SizeKind.Unknown),
        _ => throw new ArgumentOutOfRangeException()
    };
}
