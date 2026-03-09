using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Npgsql.Util;

// Many array cannot be pooled until https://github.com/dotnet/runtime/issues/125325 is addressed.
struct IterationIndices
{
    long _indicesSum;

    public long IndicesSum => _indicesSum;

    public int Rank { get; private init; }
    public int One => (int)_indicesSum;
    public int[]? Many { get; private init; }
    public int Last => Many is null ? (int)_indicesSum : Many[^1];

    // Also accept the count for the most common case where we have a single dimension array to avoid the bounds check.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAdvance(int lastCount, ReadOnlySpan<int> counts)
    {
        Debug.Assert(counts.IsEmpty || lastCount == counts[^1]);

        ref var lastIndex = ref Many is null ? ref GetIntRefFromLong(ref _indicesSum) : ref Many![^1];

        if (lastIndex < lastCount - 1)
        {
            lastIndex++;
            return true;
        }

        return Many is not null && IncrementOrCarry(counts);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    bool IncrementOrCarry(ReadOnlySpan<int> counts)
    {
        Debug.Assert(counts.Length > 1);
        Debug.Assert(Rank > 1);

        // Find the first dimension from the end that isn't at or past its length, increment it and bring all previous dimensions to zero.
        for (var dim = Rank - 1; dim >= 0; dim--)
        {
            if (this[dim] >= counts[dim] - 1)
                continue;

            Many.AsSpan().Slice(dim + 1).Clear();
            this[dim]++;
            _indicesSum++;
            return true;
        }

        // We're done if we can't find any dimension that isn't at its length.
        return false;
    }

    public ref int this[int index]
    {
        [UnscopedRef]
        get
        {
            switch (Rank)
            {
            case 0:
                ThrowHelper.ThrowIndexOutOfRangeException("Cannot index into a 0-dimensional array.");
                return ref Unsafe.NullRef<int>();
            case 1:
                Debug.Assert(index is 0);
                Debug.Assert(Many is null);
                return ref GetIntRefFromLong(ref  _indicesSum);
            default:
                return ref Many![index];
            }
        }
    }

    public void Reset()
    {
        if (Many is null)
        {
            _indicesSum = 0;
            return;
        }

        Array.Clear(Many);
    }

    public static IterationIndices Create(int dimensions)
    {
        switch (dimensions)
        {
        case 0:
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(dimensions), "Cannot create a 0-dimensional array.");
            return default;
        case 1:
            return new() { Rank = dimensions };
        default:
            return new()
            {
                Rank = dimensions,
                Many = new int[dimensions],
            };
        }
    }

    static ref int GetIntRefFromLong(ref long value)
        => ref BitConverter.IsLittleEndian
            ? ref Unsafe.As<long, int>(ref value)
            : ref Unsafe.Add(ref Unsafe.As<long, int>(ref value), 1); // Take high 32 bits.
}
