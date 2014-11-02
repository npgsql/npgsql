using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlBufferedStream : NpgsqlStream
    {
        private bool _ownStream;
        private int _minMultiByteStringChunkedEncode;

        public NpgsqlBufferedStream(Stream stream, int bufferSize, bool performNetworkByteOrderSwap, Encoding textEncoding, bool ownStream)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            if (bufferSize < 8)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this.Stream = stream;
            this.BufferSize = bufferSize;
            this._ownStream = ownStream;

            _minMultiByteStringChunkedEncode = _maxBytesPerChar == 1 ? 1 : 24;
        }

        public Stream Stream
        {
            get;
            private set;
        }

        public int BufferSize
        {
            get;
            private set;
        }

        public int BufferedBytes
        {
            get { return _readBufferCapacity - _readBufferPosition; }
        }

        public override long Length
        {
            get { throw new InvalidOperationException(); }
        }

        protected override bool PopulateReadBuffer(int minimumByteCount)
        {
            if (_readBufferCapacity - _readBufferPosition >= minimumByteCount)
            {
                return true;
            }

            if (_readBuffer == null)
            {
                _readBuffer = new byte[BufferSize];
            }

            if (_readBufferPosition > 0)
            {
                if (_readBufferPosition == _readBufferCapacity)
                {
                    _readBufferCapacity = 0;
                    _readBufferPosition = 0;
                }
                else if (BufferSize - _readBufferPosition < minimumByteCount)
                {
                    Array.Copy(_readBuffer, _readBufferPosition, _readBuffer, 0, _readBufferCapacity - _readBufferPosition);
                    _readBufferCapacity = _readBufferCapacity - _readBufferPosition;
                    _readBufferPosition = 0;
                }
            }

            int bytesRead;

            while (
                _readBufferCapacity - _readBufferPosition < minimumByteCount &&
                (bytesRead = Stream.Read(_readBuffer, _readBufferCapacity, BufferSize - _readBufferCapacity)) > 0
            )
            {
                _readBufferCapacity += bytesRead;
            }

            return true;
        }

        private void FlushWriteBuffer()
        {
            Stream.Write(_writeBuffer, 0, _writeBufferPosition);
            _writeBufferPosition = 0;
        }

        protected override void EnsureWriteBufferSpace(int count)
        {
            if (count > BufferSize)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (_writeBuffer == null)
            {
                _writeBuffer = new byte[BufferSize];
            }
            else if (_writeBufferPosition + count > BufferSize)
            {
                FlushWriteBuffer();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (count == 0)
            {
                return 0;
            }

            if (count < BufferSize || _readBufferCapacity - _readBufferPosition > 0)
            {
                int bytesRead;

                PopulateReadBuffer(1);

                bytesRead = Math.Min(_readBufferCapacity - _readBufferPosition, count);

                if (bytesRead > 0)
                {
                    Array.Copy(_readBuffer, _readBufferPosition, buffer, offset, bytesRead);
                    _readBufferPosition += bytesRead;
                }

                return bytesRead;
            }
            else
            {
                return Stream.Read(buffer, offset, Math.Min(count, BufferSize));
            }
        }

        public override string ReadString(ReadStringoptions options, int byteCount = -1)
        {
            bool readLength = ((options & ReadStringoptions.ReadLength) == ReadStringoptions.ReadLength);
            bool nullTerminated = ((options & ReadStringoptions.NullTerminated) == ReadStringoptions.NullTerminated);

            if (readLength && byteCount >= 0)
            {
                throw new ArgumentException("byteCount");
            }

            if (! readLength && ! nullTerminated && byteCount < 0)
            {
                throw new ArgumentException("byteCount");
            }

            string result = string.Empty;

            if (readLength)
            {
                byteCount = ReadInt32();

                if (byteCount < sizeof(Int32) + (nullTerminated ? 1 : 0))
                {
                    throw new IOException("Embedded length identifier out of range");
                }
            }

            if (byteCount >= 0)
            {
                if (readLength)
                {
                    byteCount -= sizeof(Int32);
                }

                if (nullTerminated)
                {
                    byteCount -= 1;
                }

                if (byteCount > 0)
                {
                    if (byteCount <= BufferSize)
                    {
                        if (!PopulateReadBuffer(byteCount))
                        {
                            throw new EndOfStreamException();
                        }

                        result = _textEncoding.GetString(_readBuffer, _readBufferPosition, byteCount);
                        _readBufferPosition += byteCount;
                    }
                    else
                    {
                        using (NpgsqlMemoryStream stream = new NpgsqlMemoryStream(false, _textEncoding))
                        {
                            CopyTo(stream, 0, byteCount);

                            result = stream.ReadString(ReadStringoptions.None, byteCount);
                        }
                    }
                }

                if (nullTerminated)
                {
                    ReadByte();
                }
            }
            else if (nullTerminated)
            {
                result = ReadNullTerminatedString();
            }

            return result;
        }

        private string ReadNullTerminatedString()
        {
            NpgsqlMemoryStream stream = null;
            int byteCount = 0;
            string result = null;

            try
            {
                do
                {
                    PopulateReadBuffer(1);

                    int i = _readBufferPosition;

                    for ( ; i < _readBufferCapacity ; i++)
                    {
                        if (_readBuffer[i] != 0)
                        {
                            byteCount++;
                        }
                        else
                        {
                            int count = i - _readBufferPosition;

                            if (stream != null)
                            {
                                stream.Write(_readBuffer, _readBufferPosition, count);
                                result = stream.ReadString(ReadStringoptions.None, byteCount);
                            }
                            else
                            {
                                 result = _textEncoding.GetString(_readBuffer, _readBufferPosition, count);
                            }

                            _readBufferPosition += (count + 1);

                            break;
                        }
                    }

                    if (result == null)
                    {
                        if (stream == null)
                        {
                            stream = new NpgsqlMemoryStream(false, _textEncoding);
                        }

                        int count = i - _readBufferPosition;

                        stream.Write(_readBuffer, _readBufferPosition, count);
                        _readBufferPosition += count;
                    }
                } while (result == null);

                return result;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        public override void Skip(int byteCount)
        {
            int skipped = 0;

            while (skipped < byteCount)
            {
                int count;

                PopulateReadBuffer(1);

                count = Math.Min(byteCount - skipped, _readBufferCapacity - _readBufferPosition);

                _readBufferPosition += count;
                skipped += count;
            }
        }

        public override void Write(byte[] callersBuffer, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (count == 0)
            {
                return;
            }

            int bytesWritten = 0;

            if (count >= BufferSize)
            {
                if (_writeBufferPosition > 0)
                {
                    FlushWriteBuffer();
                }

                while (count >= BufferSize)
                {
                    Stream.Write(callersBuffer, offset + bytesWritten, BufferSize);
                    bytesWritten += BufferSize;
                    count -= BufferSize;
                }
            }

            while (count > 0)
            {
                int bytesWrite;

                if (_writeBufferPosition == BufferSize)
                {
                    bytesWrite = count;
                }
                else
                {
                    bytesWrite = Math.Min(count, BufferSize - _writeBufferPosition);
                }

                EnsureWriteBufferSpace(bytesWrite);

                Array.Copy(callersBuffer, offset + bytesWritten, _writeBuffer, _writeBufferPosition, bytesWrite);
                _writeBufferPosition += bytesWrite;
                count -= bytesWrite;
            }
        }

        public override NpgsqlStream WriteString(string text, int charOffset, int charCount, WriteStringoptions options = WriteStringoptions.None)
        {
            bool prependLength = ((options & WriteStringoptions.PrependLength) == WriteStringoptions.PrependLength);
            bool nullTerminate = ((options & WriteStringoptions.NullTerminate) == WriteStringoptions.NullTerminate);
            int bytesPerChar = ((options & WriteStringoptions.ASCII) == WriteStringoptions.ASCII) ? 1 : _maxBytesPerChar;
            int textByteCount;
            int totalByteCount;
            bool byteCountExact = false;

            if (bytesPerChar == 1)
            {
                textByteCount = charCount;
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0);
                byteCountExact = true;
            }
            else
            {
                textByteCount = bytesPerChar * charCount;
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0);

                if (textByteCount > BufferSize)
                {
                    if (charCount < text.Length)
                    {
                        text = text.Substring(charOffset, charCount);
                        charOffset = 0;
                        charCount = text.Length;
                    }

                    textByteCount = _textEncoding.GetByteCount(text);
                    totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0);
                    byteCountExact = true;
                }
            }

            if (totalByteCount > 0)
            {
                int count = 0;

                if (byteCountExact)
                {
                    if (prependLength)
                    {
                        WriteInt32(sizeof(Int32) + textByteCount + (nullTerminate ? 1 : 0));
                    }

                    if (textByteCount <= BufferSize)
                    {
                        EnsureWriteBufferSpace(textByteCount);

                        _textEncoding.GetBytes(text, charOffset, charCount, _writeBuffer, _writeBufferPosition);

                        _writeBufferPosition += textByteCount;
                    }
                    else
                    {
                        for (int totalCharsWritten = 0 ; totalCharsWritten < charCount ; )
                        {
                            int charWriteCount;

                            if (BufferSize - _writeBufferPosition >= _minMultiByteStringChunkedEncode)
                            {
                                charWriteCount = Math.Min(charCount - totalCharsWritten, (BufferSize - _writeBufferPosition) / bytesPerChar);
                            }
                            else
                            {
                                charWriteCount = Math.Min(charCount - totalCharsWritten, BufferSize / bytesPerChar);
                            }

                            EnsureWriteBufferSpace(charWriteCount);

                            count = _textEncoding.GetBytes(text, charOffset + totalCharsWritten, charWriteCount, _writeBuffer, _writeBufferPosition);

                            _writeBufferPosition += count;
                            totalCharsWritten += charWriteCount;

                            if (totalCharsWritten < charCount && bytesPerChar > 1)
                            {
                                if (char.IsHighSurrogate(text[charOffset + totalCharsWritten - 1]))
                                {
                                    totalCharsWritten -= 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int writePosition;

                    EnsureWriteBufferSpace(totalByteCount);

                    writePosition = _writeBufferPosition;

                    if (prependLength)
                    {
                        writePosition += sizeof(Int32);
                    }

                    count = _textEncoding.GetBytes(text, charOffset, charCount, _writeBuffer, writePosition);

                    if (count > textByteCount)
                    {
                        throw new ArgumentException("text");
                    }

                    if (prependLength)
                    {
                        WriteInt32(sizeof(Int32) + count + (nullTerminate ? 1 : 0));
                    }

                    _writeBufferPosition += count;
                }
            }

            if (nullTerminate)
            {
                WriteByte(0);
            }

            return this;
        }

        public override void Flush()
        {
            if (_writeBufferPosition > 0)
            {
                FlushWriteBuffer();
                Stream.Flush();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }
            }
        }

        public override void CopyTo(Stream destination)
        {
            throw new InvalidOperationException();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            throw new InvalidOperationException();
        }

        public override void CopyTo(Stream destination, int bufferSize, int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int readCount;

                PopulateReadBuffer(1);

                readCount = Math.Min(count - totalBytesRead, _readBufferCapacity - _readBufferPosition);

                destination.Write(_readBuffer, _readBufferPosition, readCount);
                _readBufferPosition += readCount;
                totalBytesRead += readCount;
            }
        }
    }
}
