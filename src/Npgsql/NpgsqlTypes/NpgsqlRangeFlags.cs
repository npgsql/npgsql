using System;

namespace NpgsqlTypes
{
    [Flags]
    enum NpgsqlRangeFlags : byte
    {
        None = 0,
        Empty = 1,
        LowerBoundInclusive = 2,
        UpperBoundInclusive = 4,
        LowerBoundInfinite = 8,
        UpperBoundInfinite = 16,
    }
}
