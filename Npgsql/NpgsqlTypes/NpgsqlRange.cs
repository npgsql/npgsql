using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NpgsqlTypes
{
    public struct NpgsqlRange<T>
    {
        public static NpgsqlRange<T> Empty = new NpgsqlRange<T>();

        public T LowerBound { get; private set; }
        public T UpperBound { get; private set; }

        public bool LowerBoundIsInclusive { get; private set; }
        public bool UpperBoundIsInclusive { get; private set; }

        public bool LowerBoundIsMinusInfinity { get; private set; }
        public bool UpperBoundIsInfinity { get; private set; }

        public bool IsEmpty { get { return !_isNonEmpty; } }

        internal bool _isNonEmpty;

        public NpgsqlRange(T LowerBound, bool LowerBoundIsInclusive, bool LowerBoundIsMinusInfinity,
                           T UpperBound, bool UpperBoundIsInclusive, bool UpperBoundIsInfinity) : this() {
            this. LowerBound = LowerBound;
            this.UpperBound = UpperBound;

            this.LowerBoundIsInclusive = LowerBoundIsInclusive;
            this.UpperBoundIsInclusive = UpperBoundIsInclusive;

            this.LowerBoundIsMinusInfinity = LowerBoundIsMinusInfinity;
            this.UpperBoundIsInfinity = UpperBoundIsInfinity;

            this._isNonEmpty = true;
        }
    }
}
