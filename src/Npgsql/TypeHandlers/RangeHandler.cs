#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

        #region State

        ReadBuffer _readBuf;
        TElement _lowerBound;
        RangeFlags _flags;
        State _state;
        FieldDescription _fieldDescription;
        int _elementLen;
        bool _preparedRead;

        void CleanupState()
        {
            _readBuf = null;
            _lowerBound = default(TElement);
            _fieldDescription = null;
            _state = State.Done;
        }

        #endregion

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            _readBuf = buf;
            _state = State.Flags;
            _lowerBound = default(TElement);
            _elementLen = -1;
        }

        public override bool Read(out NpgsqlRange<TElement> result)
        {
            switch (_state) {
            case State.Flags:
                if (_readBuf.ReadBytesLeft < 1)
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                _flags = (RangeFlags)_readBuf.ReadByte();

                if ((_flags & RangeFlags.Empty) != 0) {
                    result = NpgsqlRange<TElement>.Empty;
                    CleanupState();
                    return true;
                }

                if ((_flags & RangeFlags.LowerBoundInfinite) != 0)
                    goto case State.UpperBound;

                _state = State.LowerBound;
                goto case State.LowerBound;

            case State.LowerBound:
                _state = State.LowerBound;
                if (!ReadSingleElement(out _lowerBound))
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                goto case State.UpperBound;

            case State.UpperBound:
                _state = State.UpperBound;
                if ((_flags & RangeFlags.UpperBoundInfinite) != 0) {
                    result = new NpgsqlRange<TElement>(_lowerBound, default(TElement), _flags);
                    CleanupState();
                    return true;
                }

                TElement upperBound;
                if (!ReadSingleElement(out upperBound))
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                result = new NpgsqlRange<TElement>(_lowerBound, upperBound, _flags);
                CleanupState();
                return true;

            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        bool ReadSingleElement([CanBeNull] out TElement element)
        {
            try {
                if (_elementLen == -1) {
                    if (_readBuf.ReadBytesLeft < 4) {
                        element = default(TElement);
                        return false;
                    }
                    _elementLen = _readBuf.ReadInt32();
                    Debug.Assert(_elementLen != -1);
                }

                var asSimpleReader = ElementHandler as ISimpleTypeHandler<TElement>;
                if (asSimpleReader != null) {
                    if (_readBuf.ReadBytesLeft < _elementLen) {
                        element = default(TElement);
                        return false;
                    }
                    element = asSimpleReader.Read(_readBuf, _elementLen, _fieldDescription);
                    _elementLen = -1;
                    return true;
                }

                var asChunkingReader = ElementHandler as IChunkingTypeHandler<TElement>;
                if (asChunkingReader != null) {
                    if (!_preparedRead)
                    {
                        asChunkingReader.PrepareRead(_readBuf, _elementLen, _fieldDescription);
                        _preparedRead = true;
                    }
                    if (!asChunkingReader.Read(out element))
                        return false;
                    _elementLen = -1;
                    _preparedRead = false;
                    return true;
                }

                throw new InvalidOperationException("Internal Npgsql bug, please report.");
            } catch (SafeReadException e) {
                // TODO: Implement safe reading. For now, translate the safe exception to an unsafe one
                // to break the connector.
                throw e.InnerException;
            }
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

        enum State
        {
            Flags,
            LowerBound,
            UpperBound,
            Done
        }
    }
}
