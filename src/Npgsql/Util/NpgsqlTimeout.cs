using System;
using System.Threading;
using Npgsql.Internal;

namespace Npgsql.Util;

/// <summary>
/// Represents a timeout that will expire at some point.
/// </summary>
public readonly struct NpgsqlTimeout
{
    readonly DateTime _expiration;

    internal static readonly NpgsqlTimeout Infinite = new(TimeSpan.Zero);

    internal NpgsqlTimeout(TimeSpan expiration)
        => _expiration = expiration > TimeSpan.Zero
            ? DateTime.UtcNow + expiration
            : expiration == TimeSpan.Zero
                ? DateTime.MaxValue
                : DateTime.MinValue;

    internal void Check()
    {
        if (HasExpired)
            ThrowHelper.ThrowNpgsqlExceptionWithInnerTimeoutException("The operation has timed out");
    }

    internal void CheckAndApply(NpgsqlConnector connector)
    {
        if (!IsSet)
            return;

        var timeLeft = CheckAndGetTimeLeft();
        // Set the remaining timeout on the read and write buffers
        connector.ReadBuffer.Timeout = connector.WriteBuffer.Timeout = timeLeft;
    }

    internal bool IsSet => _expiration != DateTime.MaxValue;

    internal bool HasExpired => DateTime.UtcNow >= _expiration;

    internal TimeSpan CheckAndGetTimeLeft()
    {
        if (!IsSet)
            return Timeout.InfiniteTimeSpan;
        var timeLeft = _expiration - DateTime.UtcNow;
        if (timeLeft <= TimeSpan.Zero)
            Check();
        return timeLeft;
    }
}
