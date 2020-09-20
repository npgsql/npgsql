using System;

namespace NpgsqlTypes
{
    [Flags]
    enum NpgsqlRangeBoundFlags : byte
    {
        Inclusive = NpgsqlRangeFlags.LowerBoundInclusive,
        Infinity = NpgsqlRangeFlags.LowerBoundInfinite
    }
}
