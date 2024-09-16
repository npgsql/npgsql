using System;

namespace Npgsql.GeoJSON.Internal;

struct CrsMapBuilder
{
    CrsMapEntry[] _overrides;
    int _overriddenIndex;
    int _wellKnownIndex;

    internal void Add(in CrsMapEntry entry)
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

    void AddCore(in CrsMapEntry entry)
    {
        var index = _overriddenIndex + 1;
        if (_overrides == null)
            _overrides = new CrsMapEntry[4];
        else
        if (_overrides.Length == index)
            Array.Resize(ref _overrides, _overrides.Length << 1);

        _overrides[_overriddenIndex] = entry;
        _overriddenIndex = index;
    }

    internal CrsMap Build()
    {
        if (_overrides != null && _overrides.Length < _overriddenIndex)
            Array.Resize(ref _overrides, _overriddenIndex);

        return new CrsMap(_overrides);
    }
}
