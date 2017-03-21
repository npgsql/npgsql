#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Text;
using JetBrains.Annotations;

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    public struct NpgsqlRange<T> : IEquatable<NpgsqlRange<T>>
    {
#pragma warning disable CA1000
        public static NpgsqlRange<T> Empty { get; private set; } = new NpgsqlRange<T>(default(T), default(T), RangeFlags.Empty);
#pragma warning restore CA1000

        public T LowerBound { get; }
        public T UpperBound { get; }

        internal RangeFlags Flags { get; set; }

        public bool LowerBoundIsInclusive
        {
            get { return (Flags & RangeFlags.LowerBoundInclusive) != 0; }
            private set
            {
                if (value)
                    Flags |= RangeFlags.LowerBoundInclusive;
                else
                    Flags &= ~RangeFlags.LowerBoundInclusive;
            }
        }

        public bool UpperBoundIsInclusive
        {
            get { return (Flags & RangeFlags.UpperBoundInclusive) != 0; }
            private set
            {
                if (value)
                    Flags |= RangeFlags.UpperBoundInclusive;
                else
                    Flags &= ~RangeFlags.UpperBoundInclusive;
            }
        }

        public bool LowerBoundInfinite
        {
            get { return (Flags & RangeFlags.LowerBoundInfinite) != 0; }
            private set
            {
                if (value)
                    Flags |= RangeFlags.LowerBoundInfinite;
                else
                    Flags &= ~RangeFlags.LowerBoundInfinite;
            }
        }

        public bool UpperBoundInfinite
        {
            get { return (Flags & RangeFlags.UpperBoundInfinite) != 0; }
            private set
            {
                if (value)
                    Flags |= RangeFlags.UpperBoundInfinite;
                else
                    Flags &= ~RangeFlags.UpperBoundInfinite;
            }
        }

        public bool IsEmpty => (Flags & RangeFlags.Empty) != 0;

        public NpgsqlRange(T lowerBound, T upperBound)
            : this(lowerBound, true, false, upperBound, true, false) {}

        public NpgsqlRange(T lowerBound, bool lowerBoundIsInclusive, T upperBound, bool upperBoundIsInclusive)
            : this(lowerBound, lowerBoundIsInclusive, false, upperBound, upperBoundIsInclusive, false) { }

        public NpgsqlRange(T lowerBound, bool lowerBoundIsInclusive, bool lowerBoundInfinite,
                           T upperBound, bool upperBoundIsInclusive, bool upperBoundInfinite) : this()
        {
            if (lowerBoundInfinite && lowerBoundIsInclusive)
                throw new ArgumentException("Infinite bound can't be inclusive", nameof(lowerBoundIsInclusive));
            if (upperBoundInfinite && upperBoundIsInclusive)
                throw new ArgumentException("Infinite bound can't be inclusive", nameof(upperBoundIsInclusive));

            LowerBound = lowerBound;
            UpperBound = upperBound;

            LowerBoundIsInclusive = lowerBoundIsInclusive;
            UpperBoundIsInclusive = upperBoundIsInclusive;

            LowerBoundInfinite = lowerBoundInfinite;
            UpperBoundInfinite = upperBoundInfinite;
        }

        internal NpgsqlRange([CanBeNull] T lowerBound, [CanBeNull] T upperBound, RangeFlags flags) : this()
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            Flags = flags;
        }

        public static bool operator ==(NpgsqlRange<T> x, NpgsqlRange<T> y)
            => x.IsEmpty == y.IsEmpty &&
               (x.LowerBoundInfinite || y.LowerBoundInfinite || x.LowerBound.Equals(y.LowerBound)) &&
               (x.UpperBoundInfinite || y.UpperBoundInfinite || x.UpperBound.Equals(y.UpperBound)) &&
               x.LowerBoundIsInclusive == y.LowerBoundIsInclusive &&
               x.UpperBoundIsInclusive == y.UpperBoundIsInclusive &&
               x.LowerBoundInfinite == y.LowerBoundInfinite &&
               x.UpperBoundInfinite == y.UpperBoundInfinite;

        public static bool operator !=(NpgsqlRange<T> x, NpgsqlRange<T> y) => !(x == y);

        public override bool Equals([CanBeNull] object o) => o is NpgsqlRange<T> && this == (NpgsqlRange<T>)o;

        public bool Equals(NpgsqlRange<T> other) => this == other;

        public override int GetHashCode()
            => IsEmpty ? 0 :
                (LowerBoundInfinite ? 0 : LowerBound.GetHashCode()) +
                (UpperBoundInfinite ? 0 : UpperBound.GetHashCode());

        public override string ToString()
        {
            if (IsEmpty)
                return "empty";
            var sb = new StringBuilder();
            if (LowerBoundInfinite)
                sb.Append('(');
            else
            {
                sb.Append(LowerBoundIsInclusive ? '[' : '(');
                sb.Append(LowerBound);
            }

            sb.Append(',');

            if (UpperBoundInfinite)
                sb.Append(')');
            else {
                sb.Append(UpperBound);
                sb.Append(UpperBoundIsInclusive ? ']' : ')');
            }
            return sb.ToString();
        }
    }

    [Flags]
    enum RangeFlags : byte
    {
        Empty = 1,
        LowerBoundInclusive = 2,
        UpperBoundInclusive = 4,
        LowerBoundInfinite = 8,
        UpperBoundInfinite = 16
    }
}
