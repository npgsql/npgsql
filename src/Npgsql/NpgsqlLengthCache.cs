using System.Collections.Generic;
using System.Diagnostics;
using Npgsql.TypeHandling;

namespace Npgsql
{
    /// <summary>
    /// An array of cached lengths for the parameters sending process.
    ///
    /// When sending parameters, lengths need to be calculated more than once (once for Bind, once for
    /// an array, once for the string within that array). This cache optimized that. Lengths are added
    /// to the cache, and then retrieved at the same order.
    /// </summary>
    public sealed class NpgsqlLengthCache
    {
        internal bool IsPopulated;
        internal int Position;
        internal List<int> Lengths;

        internal NpgsqlLengthCache() => Lengths = new List<int>();

        internal NpgsqlLengthCache(int capacity) => Lengths = new List<int>(capacity);

        /// <summary>
        /// Stores a length value in the cache, to be fetched later via <see cref="Get"/>.
        /// Called at the <see cref="NpgsqlTypeHandler.ValidateAndGetLength{TAny}"/> phase.
        /// </summary>
        /// <returns>The length parameter.</returns>
        public int Set(int len)
        {
            Debug.Assert(!IsPopulated);
            Lengths.Add(len);
            Position++;
            return len;
        }

        /// <summary>
        /// Retrieves a length value previously stored in the cache via <see cref="Set(int)"/>.
        /// Called at the writing phase, after validation has already occurred and the length cache is populated.
        /// </summary>
        /// <returns></returns>
        public int Get()
        {
            Debug.Assert(IsPopulated);
            return Lengths[Position++];
        }

        internal int GetLast()
        {
            Debug.Assert(IsPopulated);
            return Lengths[Position-1];
        }

        internal void Rewind()
        {
            Position = 0;
            IsPopulated = true;
        }

        internal void Clear()
        {
            Lengths.Clear();
            Position = 0;
            IsPopulated = false;
        }
    }
}
