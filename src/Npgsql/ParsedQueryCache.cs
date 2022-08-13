using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql;

static class ParsedQueryCache
{
    static readonly ConcurrentDictionary<string, ParsedQuery> Cache = new();

    internal static bool TryGet(string query, [NotNullWhen(true)] out ParsedQuery? parsedQuery)
        => Cache.TryGetValue(query, out parsedQuery);

    internal static void TryAdd(string query, string parsedQuery, List<NpgsqlParameter> parameters)
        => Cache.TryAdd(query, new ParsedQuery(parsedQuery, parameters));
}
