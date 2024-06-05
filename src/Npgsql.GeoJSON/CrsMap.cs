
namespace Npgsql.GeoJSON;

/// <summary>
/// A map of entries that map the authority to the inclusive range of SRID.
/// </summary>
public partial class CrsMap
{
    readonly CrsMapEntry[]? _overridden;

    internal CrsMap(CrsMapEntry[]? overridden)
        => _overridden = overridden;

    internal string? GetAuthority(int srid)
        => GetAuthority(_overridden, srid) ?? GetAuthority(WellKnown, srid);

    static string? GetAuthority(CrsMapEntry[]? entries, int srid)
    {
        if (entries == null)
            return null;

        var left = 0;
        var right = entries.Length;
        while (left <= right)
        {
            var middle = left + (right - left) / 2;
            var entry = entries[middle];

            if (srid < entry.MinSrid)
                right = middle - 1;
            else
            if (srid > entry.MaxSrid)
                left = middle + 1;
            else
                return entry.Authority;
        }

        return null;
    }
}

/// <summary>
/// An entry which maps the authority to the inclusive range of SRID.
/// </summary>
readonly struct CrsMapEntry
{
    internal readonly int MinSrid;
    internal readonly int MaxSrid;
    internal readonly string? Authority;

    internal CrsMapEntry(int minSrid, int maxSrid, string? authority)
    {
        MinSrid = minSrid;
        MaxSrid = maxSrid;
        Authority = authority != null
            ? string.IsInterned(authority) ?? authority
            : null;
    }
}
