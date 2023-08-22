using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Npgsql.Util;

static class Statics
{
#if DEBUG
    internal static bool LegacyTimestampBehavior;
    internal static bool DisableDateTimeInfinityConversions;
#else
    internal static readonly bool LegacyTimestampBehavior;
    internal static readonly bool DisableDateTimeInfinityConversions;
#endif

    static Statics()
    {
        LegacyTimestampBehavior = AppContext.TryGetSwitch("Npgsql.EnableLegacyTimestampBehavior", out var enabled) && enabled;
        DisableDateTimeInfinityConversions = AppContext.TryGetSwitch("Npgsql.DisableDateTimeInfinityConversions", out enabled) && enabled;
    }

    internal static T Expect<T>(IBackendMessage msg, NpgsqlConnector connector)
    {
        if (msg.GetType() != typeof(T))
            ThrowIfMsgWrongType<T>(msg, connector);

        return (T)msg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ExpectAny<T>(IBackendMessage msg, NpgsqlConnector connector)
    {
        if (msg is T t)
            return t;

        ThrowIfMsgWrongType<T>(msg, connector);
        return default;
    }

    [DoesNotReturn]
    static void ThrowIfMsgWrongType<T>(IBackendMessage msg, NpgsqlConnector connector)
        => throw connector.Break(
            new NpgsqlException($"Received backend message {msg.Code} while expecting {typeof(T).Name}. Please file a bug."));

    internal static void ValidateBackendMessageCode(BackendMessageCode code)
    {
        switch (code)
        {
        case BackendMessageCode.AuthenticationRequest:
        case BackendMessageCode.BackendKeyData:
        case BackendMessageCode.BindComplete:
        case BackendMessageCode.CloseComplete:
        case BackendMessageCode.CommandComplete:
        case BackendMessageCode.CopyData:
        case BackendMessageCode.CopyDone:
        case BackendMessageCode.CopyBothResponse:
        case BackendMessageCode.CopyInResponse:
        case BackendMessageCode.CopyOutResponse:
        case BackendMessageCode.DataRow:
        case BackendMessageCode.EmptyQueryResponse:
        case BackendMessageCode.ErrorResponse:
        case BackendMessageCode.FunctionCall:
        case BackendMessageCode.FunctionCallResponse:
        case BackendMessageCode.NoData:
        case BackendMessageCode.NoticeResponse:
        case BackendMessageCode.NotificationResponse:
        case BackendMessageCode.ParameterDescription:
        case BackendMessageCode.ParameterStatus:
        case BackendMessageCode.ParseComplete:
        case BackendMessageCode.PasswordPacket:
        case BackendMessageCode.PortalSuspended:
        case BackendMessageCode.ReadyForQuery:
        case BackendMessageCode.RowDescription:
            return;
        default:
            ThrowUnknownMessageCode(code);
            return;
        }

        static void ThrowUnknownMessageCode(BackendMessageCode code)
            => ThrowHelper.ThrowNpgsqlException($"Unknown message code: {code}");
    }
}

static class EnumerableExtensions
{
    internal static string Join(this IEnumerable<string> values, string separator)
        => string.Join(separator, values);
}

static class ExceptionExtensions
{
    internal static Exception UnwrapAggregate(this Exception exception)
        => exception is AggregateException agg ? agg.InnerException! : exception;
}

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

        // Note that we set UserTimeout as well, otherwise the read timeout will get overwritten in ReadMessage
        // Note also that we must set the read buffer's timeout directly (above), since the SSL handshake
        // reads data directly from the buffer, without going through ReadMessage.
        connector.UserTimeout = (int) Math.Ceiling(timeLeft.TotalMilliseconds);
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
