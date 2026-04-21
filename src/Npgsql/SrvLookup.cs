using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;

namespace Npgsql;

/// <summary>
/// Resolves DNS SRV records for PostgreSQL service discovery.
/// Looks up <c>_postgresql._tcp.&lt;srvHost&gt;</c> and returns host/port pairs
/// sorted by priority ascending then weight descending per RFC 2782.
/// </summary>
static class SrvLookup
{
    const string ServicePrefix = "_postgresql._tcp.";

    /// <summary>
    /// Resolves SRV records synchronously and returns them as a comma-separated
    /// <c>host:port,host:port,...</c> string suitable for <see cref="NpgsqlConnectionStringBuilder.Host"/>.
    /// </summary>
    /// <param name="srvHost">The cluster domain, e.g. <c>cluster.example.com</c>.</param>
    /// <param name="lookupClient">
    /// Optional custom <see cref="ILookupClient"/>. When <see langword="null"/>, the system
    /// default resolver is used. Pass a custom client in tests to avoid live DNS queries.
    /// </param>
    internal static string ResolveToHostString(string srvHost, ILookupClient? lookupClient = null)
    {
        var dnsName = ServicePrefix + srvHost;
        var client = lookupClient ?? new LookupClient();
        var result = client.Query(dnsName, QueryType.SRV);

        if (result.HasError)
            throw new NpgsqlException($"SRV lookup failed for {dnsName}: {result.ErrorMessage}");

        return BuildHostString(result.Answers.OfType<SrvRecord>(), dnsName);
    }

    /// <summary>
    /// Resolves SRV records asynchronously and returns them as a comma-separated
    /// <c>host:port,host:port,...</c> string.
    /// </summary>
    internal static async Task<string> ResolveToHostStringAsync(
        string srvHost,
        ILookupClient? lookupClient = null,
        CancellationToken cancellationToken = default)
    {
        var dnsName = ServicePrefix + srvHost;
        var client = lookupClient ?? new LookupClient();
        var result = await client.QueryAsync(dnsName, QueryType.SRV, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (result.HasError)
            throw new NpgsqlException($"SRV lookup failed for {dnsName}: {result.ErrorMessage}");

        return BuildHostString(result.Answers.OfType<SrvRecord>(), dnsName);
    }

    static string BuildHostString(IEnumerable<SrvRecord> records, string dnsName)
    {
        var hostString = SortAndBuild(records);
        if (hostString is null)
            throw new NpgsqlException($"No SRV records found for {dnsName}");
        return hostString;
    }

    /// <summary>
    /// Sorts <paramref name="records"/> by priority ascending then weight descending (RFC 2782)
    /// and returns a comma-separated <c>host:port</c> string, or <see langword="null"/> when
    /// the sequence is empty.
    /// </summary>
    /// <remarks>
    /// Package-internal so unit tests can exercise sorting without a live DNS server.
    /// </remarks>
    internal static string? SortAndBuild(IEnumerable<SrvRecord> records)
    {
        var sorted = new List<SrvRecord>(records);
        if (sorted.Count == 0)
            return null;

        sorted.Sort((a, b) =>
        {
            var cmp = a.Priority.CompareTo(b.Priority);
            return cmp != 0 ? cmp : b.Weight.CompareTo(a.Weight);
        });

        var sb = new StringBuilder();
        for (var i = 0; i < sorted.Count; i++)
        {
            if (i > 0) sb.Append(',');
            // DnsClient returns FQDNs with trailing dot — strip it for Npgsql
            var host = sorted[i].Target.Value.TrimEnd('.');
            sb.Append(host).Append(':').Append(sorted[i].Port);
        }
        return sb.ToString();
    }
}
