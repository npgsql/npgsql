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

namespace Npgsql.TypeHandlers
{
    internal class RangeHandler<TElement> : TypeHandler<NpgsqlRange<TElement>>,
        IChunkingTypeReader<NpgsqlRange<TElement>>, IChunkingTypeWriter
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

        void CleanupState()
        {
            _buf = null;
            _value = default(NpgsqlRange<TElement>);
            _fieldDescription = null;
            _state = State.Done;
        }

        #endregion

        #region Read

        public void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            _buf = buf;
            _state = State.Start;
            _elementLen = -1;
        }

        public bool Read(out NpgsqlRange<TElement> result)
        {
            switch (_state) {
            case State.Start:
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
                TElement lowerBound;
                if (!ReadSingleElement(out lowerBound))
                {
                    result = default(NpgsqlRange<TElement>);
                    return false;
                }
                _value.LowerBound = lowerBound;
                goto case State.UpperBound;

            case State.UpperBound:
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

                var asSimpleReader = ElementHandler as ISimpleTypeReader<TElement>;
                if (asSimpleReader != null) {
                    if (_buf.ReadBytesLeft < _elementLen) {
                        element = default(TElement);
                        return false;
                    }
                    element = asSimpleReader.Read(_buf, _elementLen, _fieldDescription);
                    _elementLen = -1;
                    return true;
                }

                var asChunkingReader = ElementHandler as IChunkingTypeReader<TElement>;
                if (asChunkingReader != null) {
                    asChunkingReader.PrepareRead(_buf, _elementLen, _fieldDescription);
                    if (!asChunkingReader.Read(out element)) {
                        return false;
                    }
                    _elementLen = -1;
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

        public int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter = null)
        {
            if (!(value is NpgsqlRange<TElement>))
                throw CreateConversionException(value.GetType());

            var range = (NpgsqlRange<TElement>)value;
            var totalLen = 1;

            if (!range.IsEmpty)
            {
                var asChunkingWriter = ElementHandler as IChunkingTypeWriter;
                if (!range.LowerBoundInfinite) {
                    totalLen += 4 + (asChunkingWriter != null
                        ? asChunkingWriter.ValidateAndGetLength(range.LowerBound, ref lengthCache, null)
                        : ((ISimpleTypeWriter)ElementHandler).ValidateAndGetLength(range.LowerBound, null));
                }

                if (!range.UpperBoundInfinite) {
                    totalLen += 4 + (asChunkingWriter != null
                        ? asChunkingWriter.ValidateAndGetLength(range.UpperBound, ref lengthCache, null)
                        : ((ISimpleTypeWriter)ElementHandler).ValidateAndGetLength(range.UpperBound, null));
                }
            }

            return totalLen;
        }

        public void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter)
        {
            _buf = buf;
            _lengthCache = lengthCache;
            _value = (NpgsqlRange<TElement>)value;
            _state = State.Start;
        }

        public bool Write(ref DirectBuffer directBuf)
        {
            var asChunkingWriter = ElementHandler as IChunkingTypeWriter;
            switch (_state)
            {
                case State.Start:
                    if (_buf.WriteSpaceLeft < 1) { return false; }
                    _buf.WriteByte((byte)_value.Flags);

                    if (_value.IsEmpty)
                    {
                        CleanupState();
                        return true;
                    }

                    if (_value.LowerBoundInfinite) {
                        goto case State.BeforeUpperBound;
                    }

                    if (asChunkingWriter != null) {
                        asChunkingWriter.PrepareWrite(_value.LowerBound, _buf, _lengthCache, null);
                    }
                    _state = State.LowerBound;
                    goto case State.LowerBound;

                case State.LowerBound:
                    if (asChunkingWriter == null)
                    {
                        var asSimpleWriter = (ISimpleTypeWriter)ElementHandler;
                        // TODO: Cache length
                        var len = asSimpleWriter.ValidateAndGetLength(_value.LowerBound, null);
                        if (_buf.WriteSpaceLeft < len + 4) { return false; }
                        _buf.WriteInt32(len);
                        asSimpleWriter.Write(_value.LowerBound, _buf, null);
                    }
                    else if (!asChunkingWriter.Write(ref directBuf)) { return false; }
                    goto case State.BeforeUpperBound;

                case State.BeforeUpperBound:
                    if (_value.UpperBoundInfinite) {
                        CleanupState();
                        return true;
                    }
                    if (asChunkingWriter != null) {
                        asChunkingWriter.PrepareWrite(_value.UpperBound, _buf, _lengthCache, null);
                    }
                    _state = State.UpperBound;
                    goto case State.UpperBound;

                case State.UpperBound:
                    if (asChunkingWriter == null)
                    {
                        var asSimpleWriter = (ISimpleTypeWriter)ElementHandler;
                        // TODO: Cache length
                        var len = asSimpleWriter.ValidateAndGetLength(_value.UpperBound, null);
                        if (_buf.WriteSpaceLeft < len + 4) { return false; }
                        _buf.WriteInt32(len);
                        asSimpleWriter.Write(_value.UpperBound, _buf, null);
                    }
                    else if (!asChunkingWriter.Write(ref directBuf)) { return false; }

                    CleanupState();
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        #endregion

        enum State
        {
            Start,
            LowerBound,
            BeforeUpperBound,
            UpperBound,
            Done
        }
    }
}
