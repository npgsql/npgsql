using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlTextWriter
    {
        NpgsqlBuffer _outBuf;
        List<byte[]> _buffers;
        List<int> _writePositions;
        byte[] _currentBuffer;
        int _writePosition;
        int _startPosition;

        byte[] _doubleQuoteReplace;
        byte[] _singleQuoteReplace;
        byte[] _backslashReplace;

        string _doubleQuoteReplaceString;
        string _singleQuoteReplaceString;
        string _backslashReplaceString;

        bool _hasWrittenToOutputStream;

        bool HasReplacers { get { return _doubleQuoteReplace != null || _singleQuoteReplace != null || _backslashReplace != null; } }

        int WriteSpaceLeft { get { return _currentBuffer.Length - _writePosition;  } }

        internal NpgsqlTextWriter(NpgsqlBuffer npgsqlBuffer = null)
        {
            _outBuf = npgsqlBuffer;
            if (npgsqlBuffer != null)
            {
                Contract.Assert(npgsqlBuffer.WriteSpaceLeft >= 1000, "Sholud have write space in NpgsqlBuffer");
                _writePosition = npgsqlBuffer.WritePosition;
                _startPosition = _writePosition;
                _currentBuffer = npgsqlBuffer._buf;
            }
            else
            {
                _buffers = new List<byte[]>();
                _writePositions = new List<int>();
                _currentBuffer = new byte[32];
                _buffers.Add(_currentBuffer);
            }
        }

        public EscapeState PushEscapeDoubleQuoteWithDoubleQuote()
        {
            var state = GetCurrentEscapeState();
            if (_backslashReplaceString == null)
            {
                _singleQuoteReplaceString = "'";
                _doubleQuoteReplaceString = "\"\"";
                _backslashReplaceString = @"\\";
                _singleQuoteReplace = ASCIIByteArrays.SingleQuote;
            }
            else
            {
                _doubleQuoteReplaceString += _doubleQuoteReplaceString;
                _backslashReplaceString += _backslashReplaceString;
            }
            _doubleQuoteReplace = Encoding.UTF8.GetBytes(_doubleQuoteReplaceString);
            _backslashReplace = Encoding.UTF8.GetBytes(_backslashReplaceString);
            return state;
        }

        public EscapeState PushEscapeSingleQuoteWithSingleQuote()
        {
            var state = GetCurrentEscapeState();
            if (_backslashReplaceString == null)
            {
                _singleQuoteReplaceString = @"''";
                _doubleQuoteReplaceString = "\"";
                _backslashReplaceString = @"\";
                _doubleQuoteReplace = ASCIIByteArrays.DoubleQuote;
                _backslashReplace = ASCIIByteArrays.BackSlash;
            }
            else
            {
                _singleQuoteReplaceString += _singleQuoteReplaceString;
                _backslashReplaceString += _backslashReplaceString;
            }
            _singleQuoteReplace = Encoding.UTF8.GetBytes(_singleQuoteReplaceString);
            return state;
        }

        public EscapeState PushEscapeDoubleQuoteWithBackspace()
        {
            var state = GetCurrentEscapeState();
            if (_backslashReplaceString == null)
            {
                _singleQuoteReplaceString = "'";
                _doubleQuoteReplaceString = "\\\"";
                _backslashReplaceString = @"\\";
                _singleQuoteReplace = ASCIIByteArrays.SingleQuote;
            }
            else
            {
                _doubleQuoteReplaceString = _backslashReplaceString + _doubleQuoteReplaceString;
                _backslashReplaceString += _backslashReplaceString;
            }
            _doubleQuoteReplace = Encoding.UTF8.GetBytes(_doubleQuoteReplaceString);
            _backslashReplace = Encoding.UTF8.GetBytes(_backslashReplaceString);
            return state;
        }

        public EscapeState PushEscapeSingleQuoteWithBackspace()
        {
            var state = GetCurrentEscapeState();
            if (_backslashReplaceString == null)
            {
                _singleQuoteReplaceString = @"\'";
                _doubleQuoteReplaceString = "\"";
                _backslashReplaceString = @"\\";
                _doubleQuoteReplace = ASCIIByteArrays.DoubleQuote;
            }
            else
            {
                _singleQuoteReplaceString = _backslashReplaceString + _singleQuoteReplaceString;
                _backslashReplaceString += _backslashReplaceString;
            }
            _singleQuoteReplace = Encoding.UTF8.GetBytes(_singleQuoteReplaceString);
            _backslashReplace = Encoding.UTF8.GetBytes(_backslashReplaceString);
            return state;
        }

        void AllocateNewBuffer()
        {
            if (_buffers == null)
            {
                _buffers = new List<byte[]>();
                _writePositions = new List<int>();
            }
            _currentBuffer = new byte[Math.Min(_currentBuffer.Length * 2, 8192)];
            _buffers.Add(_currentBuffer);
            _writePositions.Add(_writePosition);
            _writePosition = 0;
        }

        public void WriteSingleChar(char c)
        {
            if (c == '\'' && _singleQuoteReplace != null)
            {
                WriteRawByteArray(_singleQuoteReplace);
            }
            else if (c == '"' && _doubleQuoteReplace != null)
            {
                WriteRawByteArray(_doubleQuoteReplace);
            }
            else if (c == '\\' && _backslashReplace != null)
            {
                WriteRawByteArray(_backslashReplace);
            }
            else if (c < 0x80)
            {
                if (WriteSpaceLeft == 0)
                    AllocateNewBuffer();
                _currentBuffer[_writePosition++] = (byte)c;
            }
            else
            {
                throw new ArgumentException("Char out of range to encode as a single byte: " + (int)c);
            }
        }

        public void WriteRawByteArray(byte[] array)
        {
            if (WriteSpaceLeft < array.Length)
                AllocateNewBuffer();
            Buffer.BlockCopy(array, 0, _currentBuffer, _writePosition, array.Length);
            _writePosition += array.Length;
        }

        public void WriteRawByteArray(byte[] array, int offset, int len)
        {
            if (WriteSpaceLeft < len)
                AllocateNewBuffer();
            Buffer.BlockCopy(array, offset, _currentBuffer, _writePosition, len);
            _writePosition += len;
        }

        public void WriteString(string s)
        {
            if (!HasReplacers)
            {
                WriteRawString(s);
            }
            else
            {
                if (_backslashReplaceString != null)
                    s = s.Replace("\\", _backslashReplaceString);
                if (_doubleQuoteReplaceString != null)
                    s = s.Replace("\"", _doubleQuoteReplaceString);
                if (_singleQuoteReplaceString != null)
                    s = s.Replace("'", _singleQuoteReplaceString);
                WriteRawString(s);
            }
        }

        public void WriteString(string s, int charOffset, int charLen)
        {
            if (!HasReplacers)
            {
                WriteRawString(s, charOffset, charLen);
            }
            else
            {
                s = s.Substring(charOffset, charLen);
                if (_backslashReplaceString != null)
                    s = s.Replace("\\", _backslashReplaceString);
                if (_doubleQuoteReplaceString != null)
                    s = s.Replace("\"", _doubleQuoteReplaceString);
                if (_singleQuoteReplaceString != null)
                    s = s.Replace("'", _singleQuoteReplaceString);
                WriteRawString(s);
            }
        }

        void WriteRawString(string s)
        {
            WriteRawString(s, 0, s.Length);
        }

        void WriteRawString(string s, int charPos, int charLen)
        {

            for (; ; )
            {
                if (charLen * 3 <= WriteSpaceLeft)
                {
                    _writePosition += Encoding.UTF8.GetBytes(s, charPos, charLen, _currentBuffer, _writePosition);
                    return;
                }

                int numCharsCanBeWritten = WriteSpaceLeft / 3;
                if (numCharsCanBeWritten >= 20) // Don't do this if the buffer is almost full
                {
                    char lastChar = s[charPos + numCharsCanBeWritten - 1];
                    if (lastChar >= 0xD800 && lastChar <= 0xDBFF)
                    {
                        --numCharsCanBeWritten; // Don't use high/lead surrogate pair as last char in block
                    }
                    int wrote = Encoding.UTF8.GetBytes(s, charPos, numCharsCanBeWritten, _currentBuffer, _writePosition);
                    _writePosition += wrote;
                    charPos += numCharsCanBeWritten;
                    charLen -= numCharsCanBeWritten;
                }
                else
                {
                    AllocateNewBuffer();
                }
            }
        }

        public int GetTotalByteLength()
        {
            int len = _writePosition;
            if (_writePositions != null)
            {
                for (var i = 0; i < _writePositions.Count; i++)
                {
                    len += _writePositions[i];
                }
            }
            len -= _startPosition;
            return len;
        }

        internal byte[] GetTruncatedCopy()
        {
            Contract.Assert(_outBuf != null, "NpgsqlBuffer missing");
            int len = GetTotalByteLength();
            if (len <= 1000)
            {
                var arr = new byte[len];
                Buffer.BlockCopy(_currentBuffer, _startPosition, arr, 0, len);
                return arr;
            }
            else
            {
                var arr = new byte[1003];
                Buffer.BlockCopy(_outBuf._buf, _startPosition, arr, 0, 1000);
                arr[1000] = arr[1001] = arr[1002] = (byte)'.';
                return arr;
            }
        }

        // TODO: [GenerateAsync]
        internal void WriteToOutputBuffer()
        {
            Contract.Assert(_outBuf != null, "NpgsqlBuffer missing");
            Contract.Assert(!_hasWrittenToOutputStream, "Has already written once to output stream");

            _outBuf.WritePosition = _writePositions != null ? _writePositions[0] : _writePosition;

            if (_buffers == null)
                // No buffered data
                return;

            for (var i = 0; i < _buffers.Count - 1; i++)
            {
                _outBuf.Write(_buffers[i], 0, _writePositions[i + 1]);
            }

            _outBuf.Write(_buffers[_buffers.Count - 1], 0, _writePosition);
        }

        // TODO: [GenerateAsync]
        internal void WriteToOutputBuffer(NpgsqlBuffer buffer)
        {
            Contract.Assert(_outBuf == null, "This version of WriteToOutputBuffer requires that no NpgsqlBuffer has been set upon initialization");

            for (var i = 0; i < _buffers.Count - 1; i++)
            {
                buffer.Write(_buffers[i], 0, _writePositions[i]);
            }

            buffer.Write(_buffers[_buffers.Count - 1], 0, _writePosition);
        }

        public EscapeState GetCurrentEscapeState()
        {
            return new EscapeState(_doubleQuoteReplace, _singleQuoteReplace, _backslashReplace, _doubleQuoteReplaceString, _singleQuoteReplaceString, _backslashReplaceString);
        }

        public void ResetEscapeState(EscapeState s)
        {
            _doubleQuoteReplace = s.DoubleQuote;
            _singleQuoteReplace = s.SingleQuote;
            _backslashReplace = s.Backslash;
            _doubleQuoteReplaceString = s.DoubleQuoteString;
            _singleQuoteReplaceString = s.SingleQuoteString;
            _backslashReplaceString = s.BackslashString;
        }

        public struct EscapeState
        {
            public byte[] DoubleQuote;
            public byte[] SingleQuote;
            public byte[] Backslash;

            public string DoubleQuoteString;
            public string SingleQuoteString;
            public string BackslashString;

            public EscapeState(byte[] dq, byte[] sq, byte[] b, string dqs, string sqs, string bs)
            {
                DoubleQuote = dq;
                SingleQuote = sq;
                Backslash = b;
                DoubleQuoteString = dqs;
                SingleQuoteString = sqs;
                BackslashString = bs;
            }
        }
    }
}
