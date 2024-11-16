using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Npgsql.Util;

static class Statics
{
    internal static readonly bool EnableAssertions;
#if DEBUG
    internal static bool LegacyTimestampBehavior;
    internal static bool DisableDateTimeInfinityConversions;
#else
    internal static readonly bool LegacyTimestampBehavior;
    internal static readonly bool DisableDateTimeInfinityConversions;
#endif

    static Statics()
    {
        EnableAssertions = AppContext.TryGetSwitch("Npgsql.EnableAssertions", out var enabled) && enabled;
        LegacyTimestampBehavior = AppContext.TryGetSwitch("Npgsql.EnableLegacyTimestampBehavior", out enabled) && enabled;
        DisableDateTimeInfinityConversions = AppContext.TryGetSwitch("Npgsql.DisableDateTimeInfinityConversions", out enabled) && enabled;
    }

    /// Returns the escaped SQL representation of a string literal.
    /// <param name="literal">The identifier to be escaped.</param>
    internal static string EscapeLiteral(string literal)
    {
        // There is no support for escape sequences in quoted values for PostgreSQL, so replacing ' is enough.
        // (to be able to use escaped characters an alternative syntax exists, it requires E to appear directly before the opening quote)
        return literal.Replace("'", "''");
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

    [Conditional("DEBUG")]
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
