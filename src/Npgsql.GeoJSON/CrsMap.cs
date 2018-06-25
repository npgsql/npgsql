using System;

namespace Npgsql.GeoJSON
{
    /// <summary>
    /// An entry which maps the authority to the inclusive range of SRID.
    /// </summary>
    readonly struct CrsMapEntry
    {
        internal readonly int MinSrid;
        internal readonly int MaxSrid;
        internal readonly string Authority;

        internal CrsMapEntry(int minSrid, int maxSrid, string authority)
        {
            MinSrid = minSrid;
            MaxSrid = maxSrid;
            Authority = authority != null
                ? string.IsInterned(authority) ?? authority
                : null;
        }
    }

    ref struct CrsMapBuilder
    {
        CrsMapEntry[] _overrides;
        int _overridenIndex;
        int _wellKnownIndex;

        internal void Add(CrsMapEntry entry)
        {
            var wellKnown = CrsMap.WellKnown[_wellKnownIndex];
            if (wellKnown.MinSrid == entry.MinSrid &&
                wellKnown.MaxSrid == entry.MaxSrid &&
                string.Equals(wellKnown.Authority, entry.Authority, StringComparison.Ordinal))
            {
                _wellKnownIndex++;
                return;
            }

            if (wellKnown.MinSrid < entry.MinSrid)
            {
                do
                    _wellKnownIndex++;
                while (CrsMap.WellKnown.Length < _wellKnownIndex &&
                       CrsMap.WellKnown[_wellKnownIndex].MaxSrid < entry.MaxSrid);
                AddCore(new CrsMapEntry(wellKnown.MinSrid, Math.Min(wellKnown.MaxSrid, entry.MinSrid - 1), null));
            }

            AddCore(entry);
        }

        void AddCore(CrsMapEntry entry)
        {
            var index = _overridenIndex + 1;
            if (_overrides == null)
                _overrides = new CrsMapEntry[4];
            else
            if (_overrides.Length == index)
                Array.Resize(ref _overrides, _overrides.Length << 1);

            _overrides[_overridenIndex] = entry;
            _overridenIndex = index;
        }

        internal CrsMap Build()
        {
            if (_overrides != null && _overrides.Length < _overridenIndex)
                Array.Resize(ref _overrides, _overridenIndex);

            return new CrsMap(_overrides);
        }
    }

    readonly partial struct CrsMap
    {
        readonly CrsMapEntry[] _overriden;

        internal CrsMap(CrsMapEntry[] overriden)
            => _overriden = overriden;

        internal string GetAuthority(int srid)
            => GetAuthority(_overriden, srid) ?? GetAuthority(WellKnown, srid);

        static string GetAuthority(CrsMapEntry[] entries, int srid)
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
}
