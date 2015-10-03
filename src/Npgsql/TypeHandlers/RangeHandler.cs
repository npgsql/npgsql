#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpgsqlTypes;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

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
    internal class RangeHandler<TElement> : ChunkingTypeHandler<NpgsqlRange<TElement>>
    {
        /// <summary>
        /// The type handler for the element that this range type holds
        /// </summary>
        public TypeHandler ElementHandler { get; private set; }

        public RangeHandler(TypeHandler<TElement> elementHandler, string name)
        {
            ElementHandler = elementHandler;
            PgName = name;
        }

        #region State

        NpgsqlBuffer _buf;
        LengthCache _lengthCache;
        NpgsqlRange<TElement> _value;
        State _state;
        FieldDescription _fieldDescription;
        int _elementLen;
        bool _wroteElementLen;
        bool _preparedRead;

        void CleanupState()
        {
            _buf = null;
            _value = default(NpgsqlRange<TElement>);
            _fieldDescription = null;
            _state = State.Done;
        }

        #endregion

        #region Read

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            _buf = buf;
            _state = State.Flags;
            _elementLen = -1;
        }

        public override bool Read(out NpgsqlRange<TElement> result)
        {
            switch (_state) {
            case State.Flags:
                if (_buf.ReadBytesLeft < 1)
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                var flags = (RangeFlags)_buf.ReadByte();

                _value = new NpgsqlRange<TElement>(flags);
                if (_value.IsEmpty) {
                    result = _value;
                    CleanupState();
                    return true;
                }

                if (_value.LowerBoundInfinite) {
                    goto case State.UpperBound;
                }

                _state = State.LowerBound;
                goto case State.LowerBound;

            case State.LowerBound:
                _state = State.LowerBound;
                TElement lowerBound;
                if (!ReadSingleElement(out lowerBound))
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                _value.LowerBound = lowerBound;
                goto case State.UpperBound;

            case State.UpperBound:
                _state = State.UpperBound;
                if (_value.UpperBoundInfinite) {
                    result = _value;
                    CleanupState();
                    return true;
                }

                TElement upperBound;
                if (!ReadSingleElement(out upperBound))
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                _value.UpperBound = upperBound;
                result = _value;
                CleanupState();
                return true;

            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        bool ReadSingleElement(out TElement element)
        {
            try {
                if (_elementLen == -1) {
                    if (_buf.ReadBytesLeft < 4) {
                        element = default(TElement);
                        return false;
                    }
                    _elementLen = _buf.ReadInt32();
                    Contract.Assume(_elementLen != -1);
                }

                var asSimpleReader = ElementHandler as ISimpleTypeHandler<TElement>;
                if (asSimpleReader != null) {
                    if (_buf.ReadBytesLeft < _elementLen) {
                        element = default(TElement);
                        return false;
                    }
                    element = asSimpleReader.Read(_buf, _elementLen, _fieldDescription);
                    _elementLen = -1;
                    return true;
                }

                var asChunkingReader = ElementHandler as IChunkingTypeHandler<TElement>;
                if (asChunkingReader != null) {
                    if (!_preparedRead)
                    {
                        asChunkingReader.PrepareRead(_buf, _elementLen, _fieldDescription);
                        _preparedRead = true;
                    }
                    if (!asChunkingReader.Read(out element)) {
                        return false;
                    }
                    _elementLen = -1;
                    _preparedRead = false;
                    return true;
                }

                throw PGUtil.ThrowIfReached();
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

            var lengthCachePos = lengthCache != null ? lengthCache.Position : 0;
            if (!range.IsEmpty)
            {
                var asChunkingWriter = ElementHandler as IChunkingTypeHandler;
                if (!range.LowerBoundInfinite) {
                    totalLen += 4 + (asChunkingWriter != null
                        ? asChunkingWriter.ValidateAndGetLength(range.LowerBound, ref lengthCache, null)
                        : ((ISimpleTypeHandler)ElementHandler).ValidateAndGetLength(range.LowerBound, null));
                }

                if (!range.UpperBoundInfinite) {
                    totalLen += 4 + (asChunkingWriter != null
                        ? asChunkingWriter.ValidateAndGetLength(range.UpperBound, ref lengthCache, null)
                        : ((ISimpleTypeHandler)ElementHandler).ValidateAndGetLength(range.UpperBound, null));
                }
            }

            // If we're traversing an already-populated length cache, rewind to first element slot so that
            // the elements' handlers can access their length cache values
            if (lengthCache != null && lengthCache.IsPopulated) {
                lengthCache.Position = lengthCachePos;
            }

            return totalLen;
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter)
        {
            _buf = buf;
            _lengthCache = lengthCache;
            _value = (NpgsqlRange<TElement>)value;
            _state = State.Flags;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            switch (_state)
            {
            case State.Flags:
                if (_buf.WriteSpaceLeft < 1) { return false; }
                _buf.WriteByte((byte)_value.Flags);
                if (_value.IsEmpty)
                {
                    CleanupState();
                    return true;
                }
                goto case State.LowerBound;

            case State.LowerBound:
                _state = State.LowerBound;
                if (_value.LowerBoundInfinite) {
                    goto case State.UpperBound;
                }

                if (!WriteSingleElement(_value.LowerBound, ref directBuf)) { return false; }
                goto case State.UpperBound;

            case State.UpperBound:
                _state = State.UpperBound;
                if (_value.UpperBoundInfinite)
                {
                    CleanupState();
                    return true;
                }
                if (!WriteSingleElement(_value.UpperBound, ref directBuf)) { return false; }
                CleanupState();
                return true;

            default:
                throw PGUtil.ThrowIfReached();
            }
        }

        // TODO: Duplicated from ArrayHandler... Refactor...
        bool WriteSingleElement([CanBeNull] object element, ref DirectBuffer directBuf)
        {
            if (element == null || element is DBNull)
            {
                if (_buf.WriteSpaceLeft < 4)
                {
                    return false;
                }
                _buf.WriteInt32(-1);
                return true;
            }

            var asSimpleWriter = ElementHandler as ISimpleTypeHandler;
            if (asSimpleWriter != null)
            {
                var elementLen = asSimpleWriter.ValidateAndGetLength(element, null);
                if (_buf.WriteSpaceLeft < 4 + elementLen) { return false; }
                _buf.WriteInt32(elementLen);
                asSimpleWriter.Write(element, _buf, null);
                return true;
            }

            var asChunkedWriter = ElementHandler as IChunkingTypeHandler;
            if (asChunkedWriter != null)
            {
                if (!_wroteElementLen)
                {
                    if (_buf.WriteSpaceLeft < 4) { return false; }
                    _buf.WriteInt32(asChunkedWriter.ValidateAndGetLength(element, ref _lengthCache, null));
                    asChunkedWriter.PrepareWrite(element, _buf, _lengthCache, null);
                    _wroteElementLen = true;
                }
                if (!asChunkedWriter.Write(ref directBuf))
                {
                    return false;
                }
                _wroteElementLen = false;
                return true;
            }

            throw PGUtil.ThrowIfReached();
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
