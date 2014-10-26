using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlMemoryStream : NpgsqlStream
    {
        private bool fixedCapacity;

        public NpgsqlMemoryStream(bool performNetworkByteOrderSwap, Encoding textEncoding)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            fixedCapacity = false;
        }

        public NpgsqlMemoryStream(bool performNetworkByteOrderSwap, Encoding textEncoding, int capacity)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            fixedCapacity = true;

            readBuffer = new byte[capacity];
            writeBuffer = readBuffer;
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

            fixedCapacity = true;
            readBuffer = buffer;

            writeBuffer = readBuffer;

            readBufferCapacity = writeOffset;
            readBufferPosition = fixedOffset;

            writeBufferPosition = writeOffset;
        }

        public override long Length
        {
            get { return readBufferCapacity; }
        }

        protected override bool PopulateReadBuffer(int count)
        {
            return (readBufferPosition + count <= readBufferCapacity);
        }

        protected override void EnsureWriteBufferSpace(int count)
        {
            int target = writeBufferPosition + count;

            if ((writeBuffer == null ? 0 : writeBuffer.Length) < target)
            {
                if (fixedCapacity)
                {
                    throw new EndOfStreamException();
                }

                for (int i = 16 ; i < Int32.MaxValue ; i *= 2)
                {
                    if (i >= target)
                    {
                        readBuffer = new byte[i];

                        if (writeBuffer != null)
                        {
                            Array.Copy(writeBuffer, 0, readBuffer, 0, writeBufferPosition);
                        }

                        writeBuffer = readBuffer;

                        break;
                    }
                }
            }
        }

        protected override void FinalizeWrite()
        {
            if (writeBufferPosition > readBufferCapacity)
            {
                readBufferCapacity = writeBufferPosition;
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

            if (! PopulateReadBuffer(1))
            {
                return 0;
            }

            int bytesRead = Math.Min(readBufferCapacity - readBufferPosition, count);

            Array.Copy(readBuffer, readBufferPosition, buffer, offset, bytesRead);
            readBufferPosition += bytesRead;

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

            int readPosition = readBufferPosition;

            if (readLength)
            {
                if (readBufferCapacity - readBufferPosition < sizeof(Int32))
                {
                    throw new EndOfStreamException();
                }

                byteCount = ReadInt32NoAdvance();
                readPosition += sizeof(Int32);

                if (byteCount < sizeof(Int32) - (nullTerminated ? 1 : 0))
                {
                    throw new IOException("Embedded length identifier out of range");
                }

                if (readBufferCapacity - readPosition < byteCount)
                {
                    throw new EndOfStreamException();
                }
            }
            else if (nullTerminated)
            {
                byteCount = 0;

                for (int i = readPosition ; i < readBufferCapacity ; i++)
                {
                    byteCount++;

                    if (readBuffer[i] == 0)
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
                result = textEncoding.GetString(readBuffer, readPosition, byteCount);
                readBufferPosition += byteCount;
            }

            if (readLength)
            {
                readBufferPosition += sizeof(Int32);
            }

            if (nullTerminated)
            {
                readBufferPosition++;
            }

            return result;
        }

        public override void Skip(int byteCount)
        {
            if (! PopulateReadBuffer(byteCount))
           {
                throw new EndOfStreamException();
            }
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

            Array.Copy(buffer, offset, writeBuffer, writeBufferPosition, count);
            writeBufferPosition += count;

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
                textByteCount = textEncoding.GetMaxByteCount(charCount);
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0) + (nullTerminate ? 1 : 0);

                if (writeBuffer == null || totalByteCount > writeBuffer.Length - writeBufferPosition)
                {
                    if (charCount < text.Length)
                    {
                        text = text.Substring(charOffset, charCount);
                        charOffset = 0;
                        charCount = text.Length;
                    }

                    textByteCount = textEncoding.GetByteCount(text);
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

            writePosition = writeBufferPosition;

            if (prependLength)
            {
                writePosition += sizeof(Int32);
            }

            try
            {
                count = Encoding.UTF8.GetBytes(text, charOffset, charCount, writeBuffer, writePosition);

                if (count > textByteCount)
                {
                    throw new ArgumentException("options");
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("text", e);
            }

            if (prependLength)
            {
                WriteInt32(count + sizeof(Int32) + (nullTerminate ? 1 : 0));
            }

            writeBufferPosition += count;

            if (nullTerminate)
            {
                writeBuffer[writeBufferPosition++] = 0;
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
            if (count >= 0 && count > readBufferCapacity - readBufferPosition)
            {
                throw new EndOfStreamException();
            }
            else
            {
                count = readBufferCapacity - readBufferPosition;
            }

            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int readCount = Math.Min(count - totalBytesRead, bufferSize);

                destination.Write(readBuffer, readBufferPosition, readCount);
                readBufferPosition += readCount;
                totalBytesRead += readCount;
            }
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[readBufferCapacity];

            Array.Copy(readBuffer, 0, result, 0, readBufferCapacity);

            return result;
        }
    }
}
