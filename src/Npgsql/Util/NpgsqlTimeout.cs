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

    // A lot of .NET API's don't accept anything less than a millisecond (like Socket.ReceiveTimeout)
    // In addition, it's very unlikely we'll actually succeed in less than 1 millisecond
    // So we might as well just consider as if timeout did trigger
    static readonly TimeSpan MinimalTimeout = TimeSpan.FromMilliseconds(1);

    internal NpgsqlTimeout(TimeSpan expiration)
        => _expiration = expiration > TimeSpan.Zero
            ? DateTime.UtcNow + expiration
            : expiration == TimeSpan.Zero
                ? DateTime.MaxValue
                : DateTime.MinValue;

    static void ThrowTimeoutException() => ThrowHelper.ThrowNpgsqlExceptionWithInnerTimeoutException("The operation has timed out");

    internal void CheckAndApply(NpgsqlConnector connector)
    {
        if (!IsSet)
            return;

        var timeLeft = CheckAndGetTimeLeft();
        // Set the remaining timeout on the read and write buffers
        connector.ReadBuffer.Timeout = connector.WriteBuffer.Timeout = timeLeft;
    }

    internal bool IsSet => _expiration != DateTime.MaxValue;

    internal bool HasExpired => _expiration - DateTime.UtcNow <= MinimalTimeout;

    internal TimeSpan CheckAndGetTimeLeft()
    {
        if (!IsSet)
            return Timeout.InfiniteTimeSpan;
        var timeLeft = _expiration - DateTime.UtcNow;
        if (timeLeft <= MinimalTimeout)
            ThrowTimeoutException();
        return timeLeft;
    }
}
