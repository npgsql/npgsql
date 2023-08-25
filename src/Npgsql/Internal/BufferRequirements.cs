using System;

namespace Npgsql.Internal;

public readonly struct BufferRequirements : IEquatable<BufferRequirements>
{
    readonly Size _readRequirement;
    readonly Size _writeRequirement;

    BufferRequirements(Size readRequirement, Size writeRequirement)
    {
        _readRequirement = readRequirement;
        _writeRequirement = writeRequirement;
    }

    public Size Read => _readRequirement.IsDefault ? ThrowDefaultException() : _readRequirement;
    public Size Write => _writeRequirement.IsDefault ? ThrowDefaultException() : _writeRequirement;

    public bool IsFixedSize => Write.IsFixedSizeRequirement() && _readRequirement == _writeRequirement;

    /// Streaming
    public static BufferRequirements None => new(Size.Zero, Size.Zero);
    /// Entire value should be buffered
    public static BufferRequirements Value => new(Size.Unknown, Size.Unknown);
    /// Fixed size value should be buffered
    public static BufferRequirements CreateFixedSize(int byteCount) => new(byteCount, byteCount);
    /// Custom requirements
    public static BufferRequirements Create(Size requirement) => new(requirement, requirement);
    public static BufferRequirements Create(Size readRequirement, Size writeRequirement) => new(readRequirement, writeRequirement);

    public BufferRequirements Combine(BufferRequirements other)
    {
        return new BufferRequirements(
            CombineRequirements(_readRequirement, other._readRequirement),
            CombineRequirements(_writeRequirement, other._writeRequirement)
        );

        // The oddity, we shouldn't add sizes to zero, which is supposed to mean streaming.
        static Size CombineRequirements(Size left, Size right)
            => left == Size.Zero || right == Size.Zero ? Size.Zero : left.Combine(right);
    }

    public BufferRequirements Combine(int byteCount)
        => Combine(CreateFixedSize(byteCount));

    static Size ThrowDefaultException() => throw new InvalidOperationException($"This operation cannot be performed on a default instance of {nameof(BufferRequirements)}.");

    public bool Equals(BufferRequirements other) => _readRequirement.Equals(other._readRequirement) && _writeRequirement.Equals(other._writeRequirement);
    public override bool Equals(object? obj) => obj is BufferRequirements other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_readRequirement, _writeRequirement);
    public static bool operator ==(BufferRequirements left, BufferRequirements right) => left.Equals(right);
    public static bool operator !=(BufferRequirements left, BufferRequirements right) => !left.Equals(right);
}

static class BufferRequirementsSizeExtensions
{
    public static bool IsFixedSizeRequirement(this Size requirement)
        => requirement is { Kind: SizeKind.Exact, Value : > 0 };

    public static bool IsUpperBoundRequirement(this Size requirement)
        => requirement.Kind is SizeKind.UpperBound;

    public static bool IsStreamingRequirement(this Size requirement)
        => requirement is { Kind: SizeKind.Exact, Value : 0 };

    public static bool IsValueRequirement(this Size requirement)
        => requirement == Size.Unknown;
}
