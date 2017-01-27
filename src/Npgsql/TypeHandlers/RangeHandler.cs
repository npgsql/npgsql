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

using Npgsql.BackendMessages;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NpgsqlTypes;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;

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
    class RangeHandler<TElement> : ChunkingTypeHandler<NpgsqlRange<TElement>>
    {
        /// <summary>
        /// The type handler for the element that this range type holds
        /// </summary>
        public TypeHandler ElementHandler { get; }

        public RangeHandler(PostgresType postgresType, TypeHandler<TElement> elementHandler)
            : base(postgresType)
        {
            ElementHandler = elementHandler;
        }

        internal override TypeHandler CreateRangeHandler(PostgresType backendType)
        {
            throw new Exception("Can't create range handler of range types, this is an Npgsql bug, please report.");
        }

        #region Read

        public override async ValueTask<NpgsqlRange<TElement>> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
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

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            if (!(value is NpgsqlRange<TElement>))
                throw CreateConversionException(value.GetType());

            var range = (NpgsqlRange<TElement>)value;
            var totalLen = 1;

            var lengthCachePos = lengthCache?.Position ?? 0;
            if (!range.IsEmpty)
            {
                if (!range.LowerBoundInfinite)
                    totalLen += 4 + ElementHandler.ValidateAndGetLength(range.LowerBound, ref lengthCache);
                if (!range.UpperBoundInfinite)
                    totalLen += 4 + ElementHandler.ValidateAndGetLength(range.UpperBound, ref lengthCache);
            }

            // If we're traversing an already-populated length cache, rewind to first element slot so that
            // the elements' handlers can access their length cache values
            if (lengthCache != null && lengthCache.IsPopulated)
                lengthCache.Position = lengthCachePos;

            return totalLen;
        }

        protected override async Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            var range = (NpgsqlRange<TElement>)value;
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async, cancellationToken);
            buf.WriteByte((byte)range.Flags);
            if (range.IsEmpty)
                return;
            if (!range.LowerBoundInfinite)
                await ElementHandler.WriteWithLength(range.LowerBound, buf, lengthCache, null, async, cancellationToken);
            if (!range.UpperBoundInfinite)
                await ElementHandler.WriteWithLength(range.UpperBound, buf, lengthCache, null, async, cancellationToken);
        }

        #endregion
    }
}
