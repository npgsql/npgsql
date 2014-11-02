using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlMemoryStream : NpgsqlStream
    {
        private bool _fixedCapacity;

        public NpgsqlMemoryStream(bool performNetworkByteOrderSwap, Encoding textEncoding)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            _fixedCapacity = false;
        }

        public NpgsqlMemoryStream(bool performNetworkByteOrderSwap, Encoding textEncoding, int capacity)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            _fixedCapacity = true;

            _readBuffer = new byte[capacity];
            _writeBuffer = _readBuffer;
        }

        public NpgsqlMemoryStream(bool performNetworkByteOrderSwap, Encoding textEncoding, byte[] buffer, int fixedOffset, int fixedLength, int writeOffset)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (fixedOffset < 0 || fixedOffset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("fixedOffset");
            }

            if (fixedLength > buffer.Length - fixedOffset)
            {
                throw new ArgumentOutOfRangeException("fixedLength");
            }

            if (writeOffset < fixedOffset || writeOffset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("writeOffset");
            }

            _fixedCapacity = true;
            _readBuffer = buffer;

            _writeBuffer = _readBuffer;

            _readBufferCapacity = writeOffset;
            _readBufferPosition = fixedOffset;

            _writeBufferPosition = writeOffset;
        }

        public override long Length
        {
            get { return _readBufferCapacity; }
        }

        protected override bool PopulateReadBuffer(int minimumByteCount)
        {
            return (_readBufferCapacity - _readBufferPosition >= minimumByteCount);
        }

        protected override void EnsureWriteBufferSpace(int count)
        {
            int target = _writeBufferPosition + count;

            if ((_writeBuffer == null ? 0 : _writeBuffer.Length) < target)
            {
                if (_fixedCapacity)
                {
                    throw new EndOfStreamException();
                }

                for (int i = 16 ; i < Int32.MaxValue ; i *= 2)
                {
                    if (i >= target)
                    {
                        _readBuffer = new byte[i];

                        if (_writeBuffer != null)
                        {
                            Array.Copy(_writeBuffer, 0, _readBuffer, 0, _writeBufferPosition);
                        }

                        _writeBuffer = _readBuffer;

                        break;
                    }
                }
            }
        }

        protected override void FinalizeWrite()
        {
            if (_writeBufferPosition > _readBufferCapacity)
            {
                _readBufferCapacity = _writeBufferPosition;
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

            int bytesRead = Math.Min(_readBufferCapacity - _readBufferPosition, count);

            if (bytesRead > 0)
            {
                Array.Copy(_readBuffer, _readBufferPosition, buffer, offset, bytesRead);
                _readBufferPosition += bytesRead;
            }

            return bytesRead;
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

            int readPosition = _readBufferPosition;

            if (readLength)
            {
                if (_readBufferCapacity - _readBufferPosition < sizeof(Int32))
                {
                    throw new EndOfStreamException();
                }

                byteCount = ReadInt32NoAdvance();
                readPosition += sizeof(Int32);

                if (byteCount < sizeof(Int32) - (nullTerminated ? 1 : 0))
                {
                    throw new IOException("Embedded length identifier out of range");
                }

                if (_readBufferCapacity - readPosition < byteCount)
                {
                    throw new EndOfStreamException();
                }
            }
            else if (nullTerminated)
            {
                byteCount = 0;

                for (int i = readPosition ; i < _readBufferCapacity ; i++)
                {
                    byteCount++;

                    if (_readBuffer[i] == 0)
                    {
                        break;
                    }
                }
            }

            string result = string.Empty;

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
                result = _textEncoding.GetString(_readBuffer, readPosition, byteCount);
                _readBufferPosition += byteCount;
            }

            if (readLength)
            {
                _readBufferPosition += sizeof(Int32);
            }

            if (nullTerminated)
            {
                _readBufferPosition++;
            }

            return result;
        }

        public override void Skip(int byteCount)
        {
            if (! PopulateReadBuffer(byteCount))
            {
                throw new EndOfStreamException();
            }

            _readBufferPosition += byteCount;
        }

        public override void Write(byte[] buffer, int offset, int count)
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

            EnsureWriteBufferSpace(count);

            Array.Copy(buffer, offset, _writeBuffer, _writeBufferPosition, count);
            _writeBufferPosition += count;

            FinalizeWrite();
        }

        public override NpgsqlStream WriteString(string text, int charOffset, int charCount, WriteStringoptions options = WriteStringoptions.None)
        {
            bool prependLength = ((options & WriteStringoptions.PrependLength) == WriteStringoptions.PrependLength);
            int textByteCount;
            int totalByteCount;
            bool nullTerminate = ((options & WriteStringoptions.NullTerminate) == WriteStringoptions.NullTerminate);

            if ((options & WriteStringoptions.ASCII) == WriteStringoptions.ASCII)
            {
                textByteCount = charCount;
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0) + (nullTerminate ? 1 : 0);
           }
            else
            {
                textByteCount = _textEncoding.GetMaxByteCount(charCount);
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0) + (nullTerminate ? 1 : 0);

                if (_writeBuffer == null || totalByteCount > _writeBuffer.Length - _writeBufferPosition)
                {
                    if (charCount < text.Length)
                    {
                        text = text.Substring(charOffset, charCount);
                        charOffset = 0;
                        charCount = text.Length;
                    }

                    textByteCount = _textEncoding.GetByteCount(text);
                    totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0) + (nullTerminate ? 1 : 0);
                }
            }

            if (totalByteCount == 0)
            {
                return this;
            }

            EnsureWriteBufferSpace(totalByteCount);

            int count;
            int writePosition;

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
                WriteInt32(count + sizeof(Int32) + (nullTerminate ? 1 : 0));
            }

            _writeBufferPosition += count;

            if (nullTerminate)
            {
                _writeBuffer[_writeBufferPosition++] = 0;
            }

            FinalizeWrite();

            return this;
        }

        public override void CopyTo(Stream destination)
        {
            CopyTo(destination, 8192, -1);
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            CopyTo(destination, bufferSize, -1);
        }

        public override void CopyTo(Stream destination, int bufferSize, int count)
        {
            if (count >= 0 && count > _readBufferCapacity - _readBufferPosition)
            {
                throw new EndOfStreamException();
            }
            else
            {
                count = _readBufferCapacity - _readBufferPosition;
            }

            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int readCount = Math.Min(count - totalBytesRead, bufferSize);

                destination.Write(_readBuffer, _readBufferPosition, readCount);
                _readBufferPosition += readCount;
                totalBytesRead += readCount;
            }
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[_readBufferCapacity];

            Array.Copy(_readBuffer, 0, result, 0, _readBufferCapacity);

            return result;
        }
    }
}
