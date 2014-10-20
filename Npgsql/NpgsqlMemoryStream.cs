using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlMemoryStream : NpgsqlStream
    {
        bool fixedCapacity;

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

        public override string ReadString(int byteCount)
        {
            if (readBufferCapacity - readBufferPosition < byteCount)
            {
                throw new EndOfStreamException();
            }

            string result;

            result = textEncoding.GetString(readBuffer, readBufferPosition, byteCount);
            readBufferPosition += byteCount;

            return result;
        }

        public override string ReadString()
        {
            for (int i = readBufferPosition ; i < readBufferCapacity ; i++)
            {
                if (readBuffer[i] == 0)
                {
                    int length;
                    string result;

                    length = i - readBufferPosition;
                    result = textEncoding.GetString(readBuffer, readBufferPosition, length);
                    readBufferPosition += (length + 1);

                    return result;
                }
            }

            throw new EndOfStreamException();
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

        public override NpgsqlStream WriteString(string text, int byteCount = -1, bool nullTerminate = false)
        {
            int count = 0;

            if (byteCount < 0)
            {
                byteCount = textEncoding.GetMaxByteCount(text.Length);

                if (writeBuffer == null || byteCount > writeBuffer.Length - writeBufferPosition)
                {
                    byteCount = textEncoding.GetByteCount(text);
                }
            }

            if (byteCount == 0 && ! nullTerminate)
            {
                return this;
            }

            EnsureWriteBufferSpace(byteCount + (nullTerminate ? 1 : 0));

            if (byteCount > 0)
            {
                try
                {
                    count = Encoding.UTF8.GetBytes(text, 0, text.Length, writeBuffer, writeBufferPosition);

                    if (count > byteCount)
                    {
                        throw new ArgumentOutOfRangeException("byteCount");
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException("text", e);
                }

                writeBufferPosition += count;
            }

            if (nullTerminate)
            {
                writeBuffer[writeBufferPosition++] = 0;
            }

            FinalizeWrite();

            return this;
        }

        public new void CopyTo(Stream destination)
        {
            CopyTo(destination, 8192);
        }

        public new void CopyTo(Stream destination, int bufferSize)
        {
            while (readBufferCapacity - readBufferPosition > 0)
            {
                int count = Math.Min(readBufferCapacity - readBufferPosition, bufferSize);

                destination.Write(readBuffer, readBufferPosition, count);
                readBufferPosition += count;
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
