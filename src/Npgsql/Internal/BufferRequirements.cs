using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public readonly struct BufferRequirements : IEquatable<BufferRequirements>
{
    readonly Size _read;
    readonly Size _write;

    BufferRequirements(Size read, Size write)
    {
        _read = read;
        _write = write;
    }

    public Size Read => _read;
    public Size Write => _write;

    /// Streaming read and write; converter handles its own chunking
    public static BufferRequirements Streaming => new(Size.Unknown, Size.Unknown);
    /// <inheritdoc cref="Streaming"/>
    [Obsolete("Use BufferRequirements.Streaming instead.")]
    public static BufferRequirements None => new(Size.Unknown, Size.Unknown);
    /// Entire value should be buffered
    public static BufferRequirements Value => new(Size.CreateUpperBound(int.MaxValue), Size.CreateUpperBound(int.MaxValue));
    /// Fixed size value should be buffered
    public static BufferRequirements CreateFixedSize(int byteCount) => new(byteCount, byteCount);
    /// Custom requirements
    public static BufferRequirements Create(Size value) => new(value, value);
    public static BufferRequirements Create(Size read, Size write) => new(read, write);

    public BufferRequirements Combine(Size read, Size write)
        => new(_read.Combine(read), _write.Combine(write));

    public BufferRequirements Combine(BufferRequirements other)
        => Combine(other._read, other._write);

    public BufferRequirements Combine(int byteCount)
        => Combine(CreateFixedSize(byteCount));

    public bool Equals(BufferRequirements other) => _read.Equals(other._read) && _write.Equals(other._write);
    public override bool Equals(object? obj) => obj is BufferRequirements other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_read, _write);
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
