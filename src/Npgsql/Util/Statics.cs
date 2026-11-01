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
        // Doubling the single quotes is enough where backslashes are no escapes: in SQL with standard_conforming_strings on and
        // in replication commands. QuoteLiteral also covers SQL with it off.
        return literal.Replace("'", "''");
    }

    /// <summary>
    /// Quotes a string literal for the SQL parser, or for the replication command parser.
    /// </summary>
    /// <remarks>
    /// In SQL, a backslash in a string literal starts an escape sequence when standard_conforming_strings is off. So a value
    /// with backslashes becomes an escape string (<c>E'...'</c>) with its backslashes doubled, which reads the same whatever
    /// the setting, as libpq's PQescapeLiteral does; like libpq, we prepend a space so that the E can't join a preceding
    /// identifier. The replication command parser has neither escape strings nor backslash escapes (repl_scanner.l), so
    /// there only the single quotes are doubled.
    /// </remarks>
    internal static string QuoteLiteral(string value, bool replicationParser)
        => !replicationParser && value.Contains('\\')
            ? " E'" + EscapeLiteral(value.Replace("\\", "\\\\")) + "'"
            : "'" + EscapeLiteral(value) + "'";

    /// <summary>
    /// Appends a string literal quoted like <see cref="QuoteLiteral"/>, without building the quoted string first.
    /// </summary>
    internal static StringBuilder AppendQuotedLiteral(this StringBuilder builder, string value, bool replicationParser)
        => !replicationParser && value.Contains('\\')
            ? builder.Append(" E'").Append(EscapeLiteral(value.Replace("\\", "\\\\"))).Append('\'')
            : builder.Append('\'').Append(EscapeLiteral(value)).Append('\'');

    /// <summary>
    /// Quotes an identifier, e.g. a savepoint name, doubling the double quotes in it.
    /// </summary>
    /// <remarks>
    /// The replication command parser only understands doubled double quotes since PostgreSQL 14.24, 15.19, 16.15, 17.11 and
    /// 18.6, but valid slot and plugin names can't contain double quotes anyway.
    /// </remarks>
    internal static string QuoteIdentifier(string identifier)
        => "\"" + identifier.Replace("\"", "\"\"") + "\"";

    /// <summary>
    /// Appends an identifier quoted like <see cref="QuoteIdentifier"/>, without building the quoted string first.
    /// </summary>
    internal static StringBuilder AppendQuotedIdentifier(this StringBuilder builder, string identifier)
        => builder.Append('"').Append(identifier.Replace("\"", "\"\"")).Append('"');

    /// <summary>
    /// Quotes a name the way the server reads it unquoted: without surrounding whitespace and with A-Z lowercased (valid slot
    /// names are ASCII-only). A name that the caller already quoted stays as it is.
    /// </summary>
    /// <remarks>
    /// Replication commands send slot and output plugin names with it, the way PostgreSQL's own clients do since commit
    /// a75bd485b5e (libpqwalreceiver.c, streamutil.c): names Npgsql used to send unquoted keep their meaning, no name can
    /// change the command (e.g. <c>x TEMPORARY</c>), and valid names that would lex as numbers (like <c>1abc</c>) work.
    /// </remarks>
    internal static string QuoteName(string name)
    {
        name = name.Trim(ScannerWhitespace);

        // Names that callers quoted themselves (the only way to use slot names starting with a digit as long as Npgsql
        // didn't quote them) keep working, since the server took them as they are.
        if (name.Length >= 2 && name[0] == '"' && name[^1] == '"' && name.AsSpan(1, name.Length - 2).IndexOf('"') < 0)
            return name;

        return QuoteIdentifier(FoldName(name));

        static string FoldName(string name)
        {
            if (name.AsSpan().IndexOfAnyInRange('A', 'Z') < 0)
                return name;

            return string.Create(name.Length, name, static (span, name) =>
            {
                for (var i = 0; i < span.Length; i++)
                {
                    var c = name[i];
                    span[i] = c is >= 'A' and <= 'Z' ? (char)(c + ('a' - 'A')) : c;
                }
            });
        }
    }

    // The whitespace the replication command scanner skips between tokens (repl_scanner.l)
    static readonly char[] ScannerWhitespace = [' ', '\t', '\n', '\r', '\f', '\v'];

    /// <summary>
    /// Formats identifiers as the comma-separated list of quoted identifiers that the server's <c>SplitIdentifierString</c>
    /// parses, e.g. for pgoutput's <c>publication_names</c> option. The identifiers are quoted, so they are case-sensitive.
    /// </summary>
    internal static string QuoteIdentifierList(IEnumerable<string> identifiers)
    {
        var builder = new StringBuilder();
        foreach (var identifier in identifiers)
        {
            if (builder.Length > 0)
                builder.Append(',');
            builder.AppendQuotedIdentifier(identifier);
        }

        return builder.ToString();
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
