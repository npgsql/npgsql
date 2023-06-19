using System;

namespace Npgsql.Internal;

public enum SizeKind : byte
{
    Unknown = 0,
    Exact,
    UpperBound
}

public readonly record struct Size
{
    readonly int _byteCount;

    Size(int byteCount, SizeKind kind)
    {
        _byteCount = byteCount;
        Kind = kind;
    }

    public int Value
    {
        get
        {
            if (Kind is SizeKind.Unknown)
                throw new InvalidOperationException();

            return _byteCount;
        }
    }
    public SizeKind Kind { get; }

    public static Size Create(int byteCount) => new(byteCount, SizeKind.Exact);
    public static Size CreateUpperBound(int byteCount) => new(byteCount, SizeKind.UpperBound);
    public static Size Unknown => new(default, SizeKind.Unknown);
    public static Size Zero => new(0, SizeKind.Exact);

    public Size Combine(Size result)
    {
        if (Kind is SizeKind.Unknown || result.Kind is SizeKind.Unknown)
            return this;

        return Create(_byteCount + result._byteCount);
    }

    public static implicit operator Size(int value) => Create(value);
}
