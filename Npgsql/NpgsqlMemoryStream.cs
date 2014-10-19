using System;
using System.IO;

namespace Npgsql
{
    internal class NpgsqlMemoryStream : NpgsqlStream
    {
        public NpgsqlMemoryStream(bool performNetworkByteOrderSwap)
        {
            this.performNetworkByteOrderSwap = performNetworkByteOrderSwap;
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

        protected override void EnsureWriteBufferSpace01()
        {
            EnsureWriteBufferSpace(1);
        }

        protected override void EnsureWriteBufferSpace02()
        {
            EnsureWriteBufferSpace(2);
        }

        protected override void EnsureWriteBufferSpace04()
        {
            EnsureWriteBufferSpace(4);
        }

        protected override void EnsureWriteBufferSpace08()
        {
            EnsureWriteBufferSpace(8);
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
