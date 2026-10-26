using System;
using System.Collections.Generic;
using System.Text;

namespace Npgsql.Replication;

/// <summary>
/// Helpers for building the text of replication commands.
/// </summary>
/// <remarks>
/// <para>
/// Names are always sent as quoted identifiers and option values as quoted literals, the way PostgreSQL's own clients do
/// it since commit a75bd485b5e (libpqwalreceiver.c, streamutil.c). This keeps arbitrary input from changing the meaning of
/// a command, e.g. a slot name like <c>x TEMPORARY</c>, and allows valid slot names that would otherwise lex as numbers,
/// like <c>1abc</c>.
/// </para>
/// <para>
/// The replication command scanner (repl_scanner.l) supports neither <c>E''</c> strings nor backslash escapes, so embedded
/// quotes are escaped by doubling them. Doubled double quotes in identifiers are only understood since PostgreSQL 14.24,
/// 15.19, 16.15, 17.11 and 18.6, but valid slot and plugin names can't contain double quotes anyway.
/// </para>
/// </remarks>
static class ReplicationCommandBuilder
{
    /// <summary>
    /// Appends a slot or output plugin name. See <see cref="QuoteName"/>.
    /// </summary>
    internal static StringBuilder AppendName(this StringBuilder builder, string name)
        => builder.Append(QuoteName(name));

    /// <summary>
    /// Quotes a slot or output plugin name the way the server read it when Npgsql sent it unquoted: without surrounding
    /// whitespace and with A-Z lowercased (valid slot names are ASCII-only). This preserves the meaning of existing names.
    /// </summary>
    internal static string QuoteName(string name)
    {
        name = name.Trim(ScannerWhitespace);

        // Names that callers quoted themselves (the only way to use slot names starting with a digit as long as Npgsql
        // didn't quote them) keep working, since the server took them as they are.
        if (name.Length >= 2 && name[0] == '"' && name[^1] == '"' && name.AsSpan(1, name.Length - 2).IndexOf('"') < 0)
            return name;

        return QuoteIdentifier(FoldName(name));
    }

    // The whitespace the replication command scanner skips between tokens (repl_scanner.l)
    static readonly char[] ScannerWhitespace = [' ', '\t', '\n', '\r', '\f', '\v'];

    internal static string QuoteIdentifier(string identifier)
        => "\"" + identifier.Replace("\"", "\"\"") + "\"";

    internal static string QuoteLiteral(string value)
        => "'" + value.Replace("'", "''") + "'";

    /// <summary>
    /// Formats publication names as the comma-separated list of quoted identifiers that pgoutput's
    /// <c>publication_names</c> option expects (it is parsed with <c>SplitIdentifierString</c>).
    /// The names are quoted, so they are case-sensitive. The list itself is sent as a literal option value.
    /// </summary>
    internal static string PublicationNames(IEnumerable<string> publicationNames)
    {
        var builder = new StringBuilder();
        foreach (var publicationName in publicationNames)
        {
            if (builder.Length > 0)
                builder.Append(',');
            builder.Append(QuoteIdentifier(publicationName));
        }

        return builder.ToString();
    }

    internal static string FoldName(string name)
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
