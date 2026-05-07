using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public readonly struct BufferRequirements : IEquatable<BufferRequirements>
{
    readonly Size _read;
    readonly Size _write;
    // True when bind can be skipped — the converter has no per-value bind work AND Write is Exact.
    // The invariant is enforced at every construction and combine, so callers can read IsBindOptional directly.
    readonly bool _optionalBind;

    BufferRequirements(Size read, Size write, bool optionalBind)
    {
        _read = read;
        _write = write;
        _optionalBind = optionalBind;
    }

    public Size Read => _read;
    public Size Write => _write;

    /// <summary>
    /// True when <see cref="Write"/> has <see cref="SizeKind.Exact"/>, meaning the converter's bind size
    /// is value-independent — every value serializes to exactly <see cref="Write"/> bytes. Converters that
    /// declare this contract MUST NOT produce writeState in BindValue; if writeState is needed, declare
    /// a non-Exact Kind (UpperBound or Unknown) so the per-value path is selected.
    /// </summary>
    public bool IsBindFixedSize => _write.Kind is SizeKind.Exact;

    /// <summary>
    /// True when bind can be skipped for this format — the converter has no per-value bind work
    /// (sizing, validation, writeState production). Implies <see cref="IsBindFixedSize"/>.
    /// </summary>
    public bool IsBindOptional => _optionalBind;

    /// Streaming read and write; converter handles its own chunking.
    public static BufferRequirements Streaming => new(Size.Unknown, Size.Unknown, optionalBind: false);
    /// <inheritdoc cref="Streaming"/>
    [Obsolete("Use BufferRequirements.Streaming instead.")]
    public static BufferRequirements None => new(Size.Unknown, Size.Unknown, optionalBind: false);
    /// Entire value should be buffered
    public static BufferRequirements Value => new(Size.CreateUpperBound(int.MaxValue), Size.CreateUpperBound(int.MaxValue), optionalBind: false);
    /// Fixed size value should be buffered. <see cref="IsBindOptional"/> is true (the size is fully
    /// determined); converters that still need bind to fire (e.g. validation) use the explicit overload.
    public static BufferRequirements CreateFixedSize(int byteCount) => new(byteCount, byteCount, optionalBind: true);
    /// <summary>Fixed size value with explicit <see cref="IsBindOptional"/>; pass false when the converter
    /// has per-value bind work (e.g. value-shape validation) to fire.</summary>
    public static BufferRequirements CreateFixedSize(int byteCount, bool optionalBind) => new(byteCount, byteCount, optionalBind);
    /// Custom requirements. Defaults <see cref="IsBindOptional"/> to true when <paramref name="value"/> is
    /// <see cref="SizeKind.Exact"/> (no per-value bind work needed when the size is fully known).
    public static BufferRequirements Create(Size value) => new(value, value, optionalBind: value.Kind is SizeKind.Exact);
    /// <inheritdoc cref="Create(Size)"/>
    public static BufferRequirements Create(Size read, Size write) => new(read, write, optionalBind: write.Kind is SizeKind.Exact);
    /// <summary>Custom requirements with explicit <see cref="IsBindOptional"/>; use when the Kind-derived default is wrong (rare).</summary>
    public static BufferRequirements Create(Size read, Size write, bool optionalBind) => new(read, write, optionalBind);

    public BufferRequirements Combine(Size read, Size write)
    {
        var newWrite = _write.Combine(write);
        return new(_read.Combine(read), newWrite, _optionalBind && newWrite.Kind is SizeKind.Exact);
    }

    public BufferRequirements Combine(BufferRequirements other)
    {
        var newWrite = _write.Combine(other._write);
        return new(_read.Combine(other._read), newWrite, _optionalBind && other._optionalBind && newWrite.Kind is SizeKind.Exact);
    }

    public BufferRequirements Combine(int byteCount)
        => Combine(CreateFixedSize(byteCount));

    public bool Equals(BufferRequirements other) => _read.Equals(other._read) && _write.Equals(other._write) && _optionalBind == other._optionalBind;
    public override bool Equals(object? obj) => obj is BufferRequirements other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_read, _write, _optionalBind);
    public static bool operator ==(BufferRequirements left, BufferRequirements right) => left.Equals(right);
    public static bool operator !=(BufferRequirements left, BufferRequirements right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetMinimumBufferByteCount(Size bufferRequirement, int valueSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(valueSize);
        var reqByteCount = bufferRequirement.GetValueOrDefault();
        switch (bufferRequirement.Kind)
        {
        case SizeKind.Exact:
            if (reqByteCount != valueSize)
                ThrowExactMismatch(reqByteCount, valueSize);
            goto default;
        case SizeKind.UpperBound:
            return Math.Min(valueSize, reqByteCount);
        default:
            return reqByteCount;
        }

        static void ThrowExactMismatch(int expected, int actual)
            => throw new ArgumentOutOfRangeException(nameof(bufferRequirement),
                $"Exact buffer requirement size ({expected} bytes) does not match the value size ({actual} bytes).");
    }
}
