using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql;

/// <summary>
/// Resolves DNS SRV records for PostgreSQL service discovery without external dependencies.
/// Looks up <c>_postgresql._tcp.&lt;srvHost&gt;</c> and returns host:port pairs
/// sorted by priority ascending then weight descending per RFC 2782.
/// </summary>
static class SrvLookup
{
    const string ServicePrefix = "_postgresql._tcp.";
    const int DnsPort = 53;
    const int TypeSRV = 33;
    const int ClassIN = 1;
    const int TimeoutMs = 5000;

    /// <summary>Internal SRV record representation used in unit tests.</summary>
    internal readonly record struct SrvRecord(ushort Priority, ushort Weight, ushort Port, string Target);

    internal static string ResolveToHostString(string srvHost)
    {
        var qname = ServicePrefix + srvHost;
        var records = QuerySrv(qname);
        return SortAndBuild(records) ?? throw new NpgsqlException($"No SRV records found for {qname}");
    }

    internal static async Task<string> ResolveToHostStringAsync(
        string srvHost,
        CancellationToken cancellationToken = default)
    {
        var qname = ServicePrefix + srvHost;
        var records = await QuerySrvAsync(qname, cancellationToken).ConfigureAwait(false);
        return SortAndBuild(records) ?? throw new NpgsqlException($"No SRV records found for {qname}");
    }

    /// <summary>
    /// Sorts <paramref name="records"/> by priority ascending then weight descending (RFC 2782)
    /// and returns a comma-separated <c>host:port</c> string, or <see langword="null"/> when empty.
    /// </summary>
    /// <remarks>Internal so unit tests can exercise sorting without a live DNS server.</remarks>
    internal static string? SortAndBuild(IEnumerable<SrvRecord> records)
    {
        var list = new List<SrvRecord>(records);
        if (list.Count == 0)
            return null;

        list.Sort((a, b) =>
        {
            var cmp = a.Priority.CompareTo(b.Priority);
            return cmp != 0 ? cmp : b.Weight.CompareTo(a.Weight);
        });

        var sb = new StringBuilder();
        for (var i = 0; i < list.Count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(list[i].Target).Append(':').Append(list[i].Port);
        }
        return sb.ToString();
    }

    static List<SrvRecord> QuerySrv(string qname)
    {
        var query = BuildDnsQuery(qname);
        Exception? last = null;
        foreach (var server in GetSystemDnsServers())
        {
            try
            {
                using var udp = new UdpClient(server.AddressFamily);
                udp.Client.SendTimeout = TimeoutMs;
                udp.Client.ReceiveTimeout = TimeoutMs;
                udp.Connect(server, DnsPort);
                udp.Send(query, query.Length);
                var remote = new IPEndPoint(IPAddress.Any, 0);
                return ParseDnsResponse(qname, udp.Receive(ref remote));
            }
            catch (NpgsqlException) { throw; }
            catch (Exception ex) { last = ex; }
        }
        throw new NpgsqlException($"DNS SRV lookup failed for {qname}", last);
    }

    static async Task<List<SrvRecord>> QuerySrvAsync(string qname, CancellationToken ct)
    {
        var query = BuildDnsQuery(qname);
        Exception? last = null;
        foreach (var server in GetSystemDnsServers())
        {
            try
            {
                using var udp = new UdpClient(server.AddressFamily);
                udp.Connect(server, DnsPort);
                await udp.SendAsync(new ReadOnlyMemory<byte>(query), ct).ConfigureAwait(false);
                var result = await udp.ReceiveAsync(ct).ConfigureAwait(false);
                return ParseDnsResponse(qname, result.Buffer);
            }
            catch (NpgsqlException) { throw; }
            catch (Exception ex) { last = ex; }
        }
        throw new NpgsqlException($"DNS SRV lookup failed for {qname}", last);
    }

    static IPAddress[] GetSystemDnsServers()
    {
        var servers = new List<IPAddress>();
        foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (iface.OperationalStatus != OperationalStatus.Up)
                continue;
            foreach (var addr in iface.GetIPProperties().DnsAddresses)
                if (!servers.Contains(addr))
                    servers.Add(addr);
        }
        return servers.Count > 0 ? [.. servers] : [IPAddress.Loopback];
    }

    // Encode a DNS query packet for qname/SRV/IN.
    static byte[] BuildDnsQuery(string qname)
    {
        var buf = new List<byte>(64);
        var id = (ushort)Random.Shared.Next(65536);
        buf.Add((byte)(id >> 8)); buf.Add((byte)id);
        buf.Add(0x01); buf.Add(0x00); // flags: QR=0 RD=1
        buf.Add(0x00); buf.Add(0x01); // QDCOUNT=1
        buf.Add(0x00); buf.Add(0x00); // ANCOUNT=0
        buf.Add(0x00); buf.Add(0x00); // NSCOUNT=0
        buf.Add(0x00); buf.Add(0x00); // ARCOUNT=0
        foreach (var label in qname.Split('.'))
        {
            buf.Add((byte)label.Length);
            foreach (var c in label) buf.Add((byte)c);
        }
        buf.Add(0x00);               // root label
        buf.Add(0x00); buf.Add(TypeSRV);  // QTYPE=SRV
        buf.Add(0x00); buf.Add(ClassIN);  // QCLASS=IN
        return [.. buf];
    }

    // Parse a DNS response and extract SRV records from the answer section.
    static List<SrvRecord> ParseDnsResponse(string qname, byte[] buf)
    {
        if (buf.Length < 12)
            throw new NpgsqlException($"DNS response too short for {qname}");

        var rcode = buf[3] & 0x0F;
        if (rcode != 0)
            throw new NpgsqlException($"DNS SRV lookup failed for {qname}: RCODE={rcode}");

        var qdcount = (buf[4] << 8) | buf[5];
        var ancount = (buf[6] << 8) | buf[7];
        var pos = 12;

        for (var i = 0; i < qdcount; i++)
        {
            pos = SkipName(buf, pos);
            pos += 4; // QTYPE + QCLASS
        }

        var records = new List<SrvRecord>(ancount);
        for (var i = 0; i < ancount; i++)
        {
            pos = SkipName(buf, pos);
            if (pos + 10 > buf.Length) break;

            var rtype = (buf[pos] << 8) | buf[pos + 1];
            var rdlen = (buf[pos + 8] << 8) | buf[pos + 9];
            pos += 10;
            if (pos + rdlen > buf.Length) break;

            if (rtype == TypeSRV && rdlen >= 7)
            {
                var priority = (ushort)((buf[pos] << 8) | buf[pos + 1]);
                var weight   = (ushort)((buf[pos + 2] << 8) | buf[pos + 3]);
                var port     = (ushort)((buf[pos + 4] << 8) | buf[pos + 5]);
                var target   = ReadName(buf, pos + 6).TrimEnd('.');
                records.Add(new SrvRecord(priority, weight, port, target));
            }
            pos += rdlen;
        }
        return records;
    }

    // Skip a (possibly compressed) DNS name; returns position after it.
    static int SkipName(byte[] buf, int pos)
    {
        while (pos < buf.Length)
        {
            var len = buf[pos];
            if (len == 0) return pos + 1;
            if ((len & 0xC0) == 0xC0) return pos + 2;
            pos += len + 1;
        }
        return pos;
    }

    // Decode a (possibly compressed) DNS name starting at pos.
    static string ReadName(byte[] buf, int pos)
    {
        var sb = new StringBuilder();
        var jumped = false;
        while (pos < buf.Length)
        {
            var len = buf[pos];
            if (len == 0) break;
            if ((len & 0xC0) == 0xC0)
            {
                pos = ((len & 0x3F) << 8) | buf[pos + 1];
                jumped = true;
                continue;
            }
            if (sb.Length > 0) sb.Append('.');
            sb.Append(Encoding.ASCII.GetString(buf, pos + 1, len));
            pos += len + 1;
            if (jumped) continue;
        }
        return sb.ToString();
    }
}
