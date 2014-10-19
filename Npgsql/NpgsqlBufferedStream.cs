using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlBufferedStream : NpgsqlStream
    {
        public bool ownStream;

        public NpgsqlBufferedStream(Stream stream, int bufferSize, bool performNetworkByteOrderSwap, Encoding textEncoding, bool ownStream)
        : base(performNetworkByteOrderSwap, textEncoding)
        {
            if (bufferSize < 8)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this.Stream = stream;
            this.BufferSize = bufferSize;
            this.ownStream = ownStream;
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
            get { return readBufferCapacity - readBufferPosition; }
        }

        public override long Length
        {
            get { throw new InvalidOperationException(); }
        }

        protected override bool PopulateReadBuffer(int count)
        {
            if (readBufferPosition + count <= readBufferCapacity)
            {
                return true;
            }

            if (readBuffer == null)
            {
                readBuffer = new byte[BufferSize];
            }

            if (readBufferPosition < readBufferCapacity)
            {
                Array.Copy(readBuffer, readBufferPosition, readBuffer, 0, readBufferCapacity - readBufferPosition);
                readBufferCapacity = readBufferCapacity - readBufferPosition;
            }
            else
            {
                readBufferCapacity = 0;
            }

            readBufferPosition = 0;

            readBufferCapacity += Stream.Read(readBuffer, readBufferCapacity, BufferSize - readBufferCapacity);

            return (readBufferPosition + count <= readBufferCapacity);
        }

        private void EnsureWriteBuffer()
        {
            if (writeBuffer == null)
            {
                writeBuffer = new byte[BufferSize];
            }
        }

        private void FlushWriteBuffer()
        {
            Stream.Write(writeBuffer, 0, writeBufferPosition);
            writeBufferPosition = 0;
        }

        protected override void EnsureWriteBufferSpace(int count)
        {
            if (count > BufferSize)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            EnsureWriteBuffer();

            if (writeBufferPosition + count > BufferSize)
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

            if (count <= 0)
            {
                return 0;
            }

            if (count < BufferSize || readBufferPosition > 0)
            {
                if (! PopulateReadBuffer(1))
                {
                    return 0;
                }

                int bytesRead = Math.Min(readBufferCapacity - readBufferPosition, count);

                Array.Copy(readBuffer, readBufferPosition, buffer, offset, bytesRead);
                readBufferPosition += bytesRead;

                return bytesRead;
            }
            else
            {
                return Stream.Read(buffer, offset, count);
            }
        }

        public override string ReadString(int byteCount)
        {
            string result;

            if (byteCount <= readBufferCapacity - readBufferPosition)
            {
                result = textEncoding.GetString(readBuffer, readBufferPosition, byteCount);
                readBufferPosition += byteCount;

                return result;
            }
            using (NpgsqlMemoryStream stream = new NpgsqlMemoryStream(false, textEncoding, byteCount))
            {
                while (stream.Length < byteCount)
                {
                    PopulateReadBuffer(1);

                    int count = Math.Min(byteCount - (int)stream.Length, readBufferCapacity - readBufferPosition);

                    stream.Write(readBuffer, readBufferPosition, count);
                    readBufferPosition += count;
                }

                return stream.ReadString(byteCount);
            }
        }

        public override string ReadString()
        {
            NpgsqlMemoryStream stream = null;
            int byteCount = 0;
            string result = null;

            try
            {
                do
                {
                    PopulateReadBuffer(1);

                    int i = readBufferPosition;

                    for ( ; i < readBufferCapacity ; i++)
                    {
                        if (readBuffer[i] != 0)
                        {
                            byteCount++;
                        }
                        else
                        {
                            int count = i - readBufferPosition;

                            if (stream != null)
                            {
                                stream.Write(readBuffer, readBufferPosition, count);
                                result = stream.ReadString(byteCount);
                            }
                            else
                            {
                                 result = textEncoding.GetString(readBuffer, readBufferPosition, count);
                            }

                            readBufferPosition += (count + 1);

                            break;
                        }
                    }

                    if (result == null)
                    {
                        if (stream == null)
                        {
                            stream = new NpgsqlMemoryStream(false, textEncoding);
                        }

                        int count = i - readBufferPosition;

                        stream.Write(readBuffer, readBufferPosition, count);
                        readBufferPosition += count;
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

            if (count > BufferSize)
            {
                EnsureWriteBufferSpace(BufferSize);

                Stream.Write(callersBuffer, offset, count);
            }
            else
            {
                EnsureWriteBufferSpace(count);

                Array.Copy(callersBuffer, offset, writeBuffer, writeBufferPosition, count);
                writeBufferPosition += count;
            }
        }

        public override NpgsqlStream WriteString(string text, int byteCount = -1, bool nullTerminate = false)
        {
            int count = 0;

            if (byteCount < 0)
            {
                byteCount = textEncoding.GetMaxByteCount(text.Length);

                if (byteCount > BufferSize)
                {
                    byteCount = textEncoding.GetByteCount(text);
                }
            }

            if (byteCount == 0 && ! nullTerminate)
            {
                return this;
            }

            if (byteCount > 0)
            {
                if (byteCount <= BufferSize)
                {
                    EnsureWriteBufferSpace(byteCount);

                    try
                    {
                        count = Encoding.UTF8.GetBytes(text, 0, text.Length, writeBuffer, writeBufferPosition);

                        if (count > byteCount)
                        {
                            throw new ArgumentOutOfRangeException("byteCount");
                        }

                        writeBufferPosition += count;
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("text", e);
                    }
                }
                else
                {
                    using (NpgsqlMemoryStream stream = new NpgsqlMemoryStream(false, textEncoding))
                    {
                        stream.WriteString(text, byteCount, false);
                        stream.CopyTo(this, BufferSize);
                    }
                }
            }

            if (nullTerminate)
            {
                EnsureWriteBufferSpace(1);
                writeBuffer[writeBufferPosition++] = 0;
            }

            return this;
        }

        public override void Flush()
        {
            if (writeBufferPosition > 0)
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

        public new void CopyTo(Stream destination)
        {
            CopyTo(destination, 8192);
        }

        public new void CopyTo(Stream destination, int bufferSize)
        {
            throw new InvalidOperationException();
        }
    }
}
