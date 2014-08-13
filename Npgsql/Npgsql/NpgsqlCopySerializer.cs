// Npgsql.NpgsqlCopySerializer.cs
//
// Author:
//     Kalle Hallivuori <kato@iki.fi>
//
//    Copyright (C) 2007 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace Npgsql
{
    /// <summary>
    /// Writes given objects into a stream for PostgreSQL COPY in default copy format (not CSV or BINARY).
    /// </summary>
    public class NpgsqlCopySerializer
    {
        /// <summary>
        /// Default delimiter.
        /// </summary>
        public const String DEFAULT_DELIMITER = "\t";

        /// <summary>
        /// Default separator.
        /// </summary>
        public const String DEFAULT_SEPARATOR = "\n";

        /// <summary>
        /// Default null.
        /// </summary>
        public const String DEFAULT_NULL = "\\N";

        /// <summary>
        /// Default escape.
        /// </summary>
        public const String DEFAULT_ESCAPE = "\\";

        /// <summary>
        /// Default quote.
        /// </summary>
        public const String DEFAULT_QUOTE = "\"";

        /// <summary>
        /// Default buffer size.
        /// </summary>
        public const int DEFAULT_BUFFER_SIZE = 8192;

        private static readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;    // PostgreSQL currently only supports SQL notation for decimal point (which is the same as InvariantCulture)

        private readonly NpgsqlConnector _context;
        private Stream _toStream;

        private String _delimiter = DEFAULT_DELIMITER,
                       _escape = DEFAULT_ESCAPE,
                       _separator = DEFAULT_SEPARATOR,
                       _null = DEFAULT_NULL;

        private byte[] _delimiterBytes = null, _escapeBytes = null, _separatorBytes = null, _nullBytes = null;
        private byte[][] _escapeSequenceBytes = null;
        private String[] _stringsToEscape = null;

        private byte[] _sendBuffer = null;
        private int _sendBufferAt = 0, _lastFieldEndAt = 0, _lastRowEndAt = 0, _atField = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="conn"></param>
        public NpgsqlCopySerializer(NpgsqlConnection conn)
        {
            _context = conn.Connector;
        }

        /// <summary>
        /// Report whether the serializer is active.
        /// </summary>
        public bool IsActive
        {
            get { return _toStream != null && _context.Mediator.CopyStream == _toStream && _context.CurrentState is NpgsqlCopyInState; }
        }

        /// <summary>
        /// To Stream.
        /// </summary>
        public Stream ToStream
        {
            get
            {
                if (_toStream == null)
                {
                    _toStream = _context.Mediator.CopyStream;
                }
                return _toStream;
            }
            set
            {
                if (IsActive)
                {
                    throw new NpgsqlException("Do not change stream of an active " + this);
                }
                _toStream = value;
            }
        }

        /// <summary>
        /// Delimiter.
        /// </summary>
        public String Delimiter
        {
            get { return _delimiter; }
            set
            {
                if (IsActive)
                {
                    throw new NpgsqlException("Do not change delimiter of an active " + this);
                }
                _delimiter = value ?? DEFAULT_DELIMITER;
                _delimiterBytes = null;
                _stringsToEscape = null;
                _escapeSequenceBytes = null;
            }
        }

        private byte[] DelimiterBytes
        {
            get
            {
                if (_delimiterBytes == null)
                {
                    _delimiterBytes = BackendEncoding.UTF8Encoding.GetBytes(_delimiter);
                }
                return _delimiterBytes;
            }
        }

        /// <summary>
        /// Separator.
        /// </summary>
        public String Separator
        {
            get { return _separator; }
            set
            {
                if (IsActive)
                {
                    throw new NpgsqlException("Do not change separator of an active " + this);
                }
                _separator = value ?? DEFAULT_SEPARATOR;
                _separatorBytes = null;
                _stringsToEscape = null;
                _escapeSequenceBytes = null;
            }
        }

        private byte[] SeparatorBytes
        {
            get
            {
                if (_separatorBytes == null)
                {
                    _separatorBytes = BackendEncoding.UTF8Encoding.GetBytes(_separator);
                }
                return _separatorBytes;
            }
        }

        /// <summary>
        /// Escape.
        /// </summary>
        public String Escape
        {
            get { return _escape; }
            set
            {
                if (IsActive)
                {
                    throw new NpgsqlException("Do not change escape symbol of an active " + this);
                }
                _escape = value ?? DEFAULT_ESCAPE;
                _escapeBytes = null;
                _stringsToEscape = null;
                _escapeSequenceBytes = null;
            }
        }

        private byte[] EscapeBytes
        {
            get
            {
                if (_escapeBytes == null)
                {
                    _escapeBytes = BackendEncoding.UTF8Encoding.GetBytes(_escape);
                }
                return _escapeBytes;
            }
        }

        /// <summary>
        /// Null.
        /// </summary>
        public String Null
        {
            get { return _null; }
            set
            {
                if (IsActive)
                {
                    throw new NpgsqlException("Do not change null symbol of an active " + this);
                }
                _null = value ?? DEFAULT_NULL;
                _nullBytes = null;
                _stringsToEscape = null;
                _escapeSequenceBytes = null;
            }
        }

        private byte[] NullBytes
        {
            get
            {
                if (_nullBytes == null)
                {
                    _nullBytes = BackendEncoding.UTF8Encoding.GetBytes(_null);
                }
                return _nullBytes;
            }
        }

        /// <summary>
        /// Buffer size.
        /// </summary>
        public Int32 BufferSize
        {
            get { return _sendBuffer != null ? _sendBuffer.Length : DEFAULT_BUFFER_SIZE; }
            set
            {
                byte[] _newBuffer = new byte[value];
                if (_sendBuffer != null)
                {
                    for (int i = 0; i < _sendBufferAt; i++)
                    {
                        _newBuffer[i] = _sendBuffer[i];
                    }
                }
                _sendBuffer = _newBuffer;
            }
        }

        /// <summary>
        /// Flush buffers.
        /// </summary>
        public void Flush()
        {
            if (_sendBufferAt > 0)
            {
                ToStream.Write(_sendBuffer, 0, _sendBufferAt);
                ToStream.Flush();
            }
            _sendBufferAt = 0;
            _lastRowEndAt = 0;
            _lastFieldEndAt = 0;
        }

        /// <summary>
        /// Flush rows.
        /// </summary>
        public void FlushRows()
        {
            if (_lastRowEndAt > 0)
            {
                ToStream.Write(_sendBuffer, 0, _lastRowEndAt);
                ToStream.Flush();
                int len = _sendBufferAt - _lastRowEndAt;
                for (int i = 0; i < len; i++)
                {
                    _sendBuffer[i] = _sendBuffer[_lastRowEndAt + i];
                }
                _lastFieldEndAt -= _lastRowEndAt;
                _sendBufferAt -= _lastRowEndAt;
                _lastRowEndAt = 0;
            }
        }

        /// <summary>
        /// Flush fields.
        /// </summary>
        public void FlushFields()
        {
            if (_lastFieldEndAt > 0)
            {
                ToStream.Write(_sendBuffer, 0, _lastFieldEndAt);
                ToStream.Flush();
                int len = _sendBufferAt - _lastFieldEndAt;
                for (int i = 0; i < len; i++)
                {
                    _sendBuffer[i] = _sendBuffer[_lastFieldEndAt + i];
                }
                _lastRowEndAt -= _lastFieldEndAt;
                _sendBufferAt -= _lastFieldEndAt;
                _lastFieldEndAt = 0;
            }
        }

        /// <summary>
        /// Close the serializer.
        /// </summary>
        public void Close()
        {
            if (_atField > 0)
            {
                EndRow();
            }
            Flush();
            ToStream.Close();
        }

        /// <summary>
        /// Report whether space remains in the buffer.
        /// </summary>
        protected int SpaceInBuffer
        {
            get { return BufferSize - _sendBufferAt; }
        }

        /// <summary>
        /// Strings to escape.
        /// </summary>
        protected String[] StringsToEscape
        {
            get
            {
                if (_stringsToEscape == null)
                {
                    _stringsToEscape = new String[] {Delimiter, Separator, Escape, "\r", "\n"};
                }
                return _stringsToEscape;
            }
        }

        /// <summary>
        /// Escape sequence bytes.
        /// </summary>
        protected byte[][] EscapeSequenceBytes
        {
            get
            {
                if (_escapeSequenceBytes == null)
                {
                    _escapeSequenceBytes = new byte[StringsToEscape.Length][];
                    for (int i = 0; i < StringsToEscape.Length; i++)
                    {
                        _escapeSequenceBytes[i] = EscapeSequenceFor(StringsToEscape[i].ToCharArray(0, 1)[0]);
                    }
                }
                return _escapeSequenceBytes;
            }
        }

        private static readonly byte[] esc_t = new byte[] { (byte)ASCIIBytes.t };
        private static readonly byte[] esc_n = new byte[] { (byte)ASCIIBytes.n };
        private static readonly byte[] esc_r = new byte[] { (byte)ASCIIBytes.r };
        private static readonly byte[] esc_b = new byte[] { (byte)ASCIIBytes.b };
        private static readonly byte[] esc_f = new byte[] { (byte)ASCIIBytes.f };
        private static readonly byte[] esc_v = new byte[] { (byte)ASCIIBytes.v };
 
        /// <summary>
        /// Escape sequence for the given character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected static byte[] EscapeSequenceFor(char c)
        {
            switch (c)
            {
                case '\t' :
                    return  esc_t;

                case '\n' :
                    return esc_n;

                case '\r' :
                    return esc_r;

                case '\b' :
                    return esc_b;

                case '\f' :
                    return esc_f;

                case '\v' :
                    return esc_v;

                default :
                    if (c < 32 || c > 127)
                    {
                        return new byte[] {(byte) ('0' + ((c/64) & 7)), (byte) ('0' + ((c/8) & 7)), (byte) ('0' + (c & 7))};
                    }
                    else
                    {
                        return new byte[] {(byte) c};
                    }

            }
        }

        /// <summary>
        /// Make room for bytes.
        /// </summary>
        /// <param name="len"></param>
        protected void MakeRoomForBytes(int len)
        {
            if (_sendBuffer == null)
            {
                _sendBuffer = new byte[BufferSize];
            }
            if (len >= SpaceInBuffer)
            {
                FlushRows();
                if (len >= SpaceInBuffer)
                {
                    FlushFields();
                    if (len >= SpaceInBuffer)
                    {
                        int increaseBufferSize = len - SpaceInBuffer;

                        // Increase the size of the buffer to allow len size
                        BufferSize += increaseBufferSize;
                    }
                }
            }
        }

        /// <summary>
        /// Add bytes.
        /// </summary>
        /// <param name="bytes"></param>
        protected void AddBytes(byte[] bytes)
        {
            MakeRoomForBytes(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                _sendBuffer[_sendBufferAt++] = bytes[i];
            }
        }

        /// <summary>
        /// End row.
        /// </summary>
        public void EndRow()
        {
            if (_context != null)
            {
                while (_atField < _context.CurrentState.CopyFormat.FieldCount)
                {
                    AddNull();
                }
            }
            if (_context == null || ! _context.CurrentState.CopyFormat.IsBinary)
            {
                AddBytes(SeparatorBytes);
            }
            _lastRowEndAt = _sendBufferAt;
            _atField = 0;
        }

        /// <summary>
        /// Prefix field.
        /// </summary>
        protected void PrefixField()
        {
            if (_atField > 0)
            {
                if (_atField >= _context.CurrentState.CopyFormat.FieldCount)
                {
                    throw new NpgsqlException("Tried to add too many fields to a copy record with " + _atField + " fields");
                }
                AddBytes(DelimiterBytes);
            }
        }

        /// <summary>
        /// Field added.
        /// </summary>
        protected void FieldAdded()
        {
            _lastFieldEndAt = _sendBufferAt;
            _atField++;
        }

        /// <summary>
        /// Add null.
        /// </summary>
        public void AddNull()
        {
            PrefixField();
            AddBytes(NullBytes);
            FieldAdded();
        }

        /// <summary>
        /// Add string.
        /// </summary>
        /// <param name="fieldValue"></param>
        public void AddString(String fieldValue)
        {
            PrefixField();
            int bufferedUpto = 0;
            while (bufferedUpto < fieldValue.Length)
            {
                int escapeAt = fieldValue.Length;
                byte[] escapeSequence = null;

                // choose closest instance of strings to escape in fieldValue
                for (int eachEscapeable = 0; eachEscapeable < StringsToEscape.Length; eachEscapeable++)
                {
                    int i = fieldValue.IndexOf(StringsToEscape[eachEscapeable], bufferedUpto);
                    if (i > -1 && i < escapeAt)
                    {
                        escapeAt = i;
                        escapeSequence = EscapeSequenceBytes[eachEscapeable];
                    }
                }

                // some, possibly all of fieldValue string does not require escaping and can be buffered for output
                if (escapeAt > bufferedUpto)
                {
//                    int encodedLength = BackendEncoding.UTF8Encoding.GetByteCount(fieldValue.ToCharArray(bufferedUpto, escapeAt));
//                    MakeRoomForBytes(encodedLength);
//                    _sendBufferAt += BackendEncoding.UTF8Encoding.GetBytes(fieldValue, bufferedUpto, escapeAt, _sendBuffer, _sendBufferAt);
//                    bufferedUpto = escapeAt;
                    int encodedLength = BackendEncoding.UTF8Encoding.GetByteCount(fieldValue.ToCharArray(bufferedUpto, escapeAt - bufferedUpto));
                    MakeRoomForBytes(encodedLength);
                    _sendBufferAt += BackendEncoding.UTF8Encoding.GetBytes(fieldValue, bufferedUpto, escapeAt - bufferedUpto, _sendBuffer, _sendBufferAt);
                    bufferedUpto = escapeAt;
                }

                // now buffer the escape sequence for output
                if (escapeSequence != null)
                {
                    AddBytes(EscapeBytes);
                    AddBytes(escapeSequence);
                    bufferedUpto++;
                }
            }
            FieldAdded();
        }

        /// <summary>
        /// add Int32.
        /// </summary>
        /// <param name="fieldValue"></param>
        public void AddInt32(Int32 fieldValue)
        {
            AddString(string.Format(_cultureInfo, "{0}", fieldValue));
        }

        /// <summary>
        /// Add Int64.
        /// </summary>
        /// <param name="fieldValue"></param>
        public void AddInt64(Int64 fieldValue)
        {
            AddString(string.Format(_cultureInfo, "{0}", fieldValue));
        }

        /// <summary>
        /// Add number.
        /// </summary>
        /// <param name="fieldValue"></param>
        public void AddNumber(double fieldValue)
        {
            AddString(string.Format(_cultureInfo, "{0}", fieldValue));
        }

        /// <summary>
        /// Add bool
        /// </summary>
        /// <param name="fieldValue"></param>
        public void AddBool(bool fieldValue)
        {
            AddString(fieldValue ? "TRUE" : "FALSE");
        }

        /// <summary>
        /// Add DateTime.
        /// </summary>
        /// <param name="fieldValue"></param>
        public void AddDateTime(DateTime fieldValue)
        {
            AddString(fieldValue.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        }

    }
}
