#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

        internal NpgsqlLengthCache()
        {
            Lengths = new List<int>();
        }

        internal NpgsqlLengthCache(int capacity)
        {
            Lengths = new List<int>(capacity);
        }

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
