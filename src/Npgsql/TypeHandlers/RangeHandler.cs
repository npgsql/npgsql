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

using Npgsql.BackendMessages;
using System;
using System.Threading.Tasks;
using NpgsqlTypes;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for PostgreSQL range types
    /// </summary>
    /// <remarks>
    /// Introduced in PostgreSQL 9.2.
    /// http://www.postgresql.org/docs/current/static/rangetypes.html
    /// </remarks>
    /// <typeparam name="TElement">the range subtype</typeparam>
    class RangeHandler<TElement> : NpgsqlTypeHandler<NpgsqlRange<TElement>>
    {
        /// <summary>
        /// The type handler for the element that this range type holds
        /// </summary>
        public NpgsqlTypeHandler ElementHandler { get; }

        public RangeHandler(NpgsqlTypeHandler<TElement> elementHandler)
        {
            ElementHandler = elementHandler;
        }

        internal override NpgsqlTypeHandler CreateRangeHandler(PostgresType backendType)
        {
            throw new Exception("Can't create range handler of range types, this is an Npgsql bug, please report.");
        }

        #region Read

        public override async ValueTask<NpgsqlRange<TElement>> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(1, async);
            var flags = (RangeFlags)buf.ReadByte();
            if ((flags & RangeFlags.Empty) != 0)
                return NpgsqlRange<TElement>.Empty;

            TElement lowerBound = default(TElement), upperBound = default(TElement);
            if ((flags & RangeFlags.LowerBoundInfinite) == 0)
                lowerBound = await ElementHandler.ReadWithLength<TElement>(buf, async);
            if ((flags & RangeFlags.UpperBoundInfinite) == 0)
                upperBound = await ElementHandler.ReadWithLength<TElement>(buf, async);
            return new NpgsqlRange<TElement>(lowerBound, upperBound, flags);
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(NpgsqlRange<TElement> value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            var totalLen = 1;

            var lengthCachePos = lengthCache?.Position ?? 0;
            if (!value.IsEmpty)
            {
                if (!value.LowerBoundInfinite)
                    totalLen += 4 + ElementHandler.ValidateAndGetLength(value.LowerBound, ref lengthCache, null);
                if (!value.UpperBoundInfinite)
                    totalLen += 4 + ElementHandler.ValidateAndGetLength(value.UpperBound, ref lengthCache, null);
            }

            // If we're traversing an already-populated length cache, rewind to first element slot so that
            // the elements' handlers can access their length cache values
            if (lengthCache != null && lengthCache.IsPopulated)
                lengthCache.Position = lengthCachePos;

            return totalLen;
        }

        public override async Task Write(NpgsqlRange<TElement> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte((byte)value.Flags);
            if (value.IsEmpty)
                return;
            if (!value.LowerBoundInfinite)
                await ElementHandler.WriteWithLengthInternal(value.LowerBound, buf, lengthCache, null, async);
            if (!value.UpperBoundInfinite)
                await ElementHandler.WriteWithLengthInternal(value.UpperBound, buf, lengthCache, null, async);
        }

        #endregion
    }
}
