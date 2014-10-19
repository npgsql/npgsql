using System;
using System.IO;

namespace Npgsql
{
    internal class NpgsqlBufferedStream : NpgsqlStream
    {
        public bool ownStream;

        public NpgsqlBufferedStream(Stream stream, int bufferSize, bool performNetworkByteOrderSwap, bool ownStream)
        {
            this.Stream = stream;
            this.BufferSize = bufferSize;
            this.performNetworkByteOrderSwap = performNetworkByteOrderSwap;
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
            EnsureWriteBuffer();

            if (writeBufferPosition + count > BufferSize)
            {
                FlushWriteBuffer();
            }
        }

        protected override void EnsureWriteBufferSpace01()
        {
            EnsureWriteBuffer();

            if (writeBufferPosition == BufferSize)
            {
                FlushWriteBuffer();
            }
        }

        protected override void EnsureWriteBufferSpace02()
        {
            EnsureWriteBuffer();

            if (writeBufferPosition + 2 > BufferSize)
            {
                FlushWriteBuffer();
            }
        }

        protected override void EnsureWriteBufferSpace04()
        {
            EnsureWriteBuffer();

            if (writeBufferPosition + 4 > BufferSize)
            {
                FlushWriteBuffer();
            }
        }

        protected override void EnsureWriteBufferSpace08()
        {
            EnsureWriteBuffer();

            if (writeBufferPosition + 8 > BufferSize)
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
                if (writeBufferPosition > 0)
                {
                    FlushWriteBuffer();
                }

                Stream.Write(callersBuffer, offset, count);

                return;
            }

            EnsureWriteBufferSpace(count);

            Array.Copy(callersBuffer, offset, writeBuffer, writeBufferPosition, count);
            writeBufferPosition += count;
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
