using System;
using System.IO;

namespace Npgsql
{
    internal abstract class NpgsqlStream : Stream
    {
        protected byte[] readBuffer;
        protected int readBufferCapacity;
        protected int readBufferPosition;
        protected byte[] writeBuffer;
        protected int writeBufferPosition;
        protected bool performNetworkByteOrderSwap;

        protected abstract bool PopulateReadBuffer(int count);

        protected abstract void EnsureWriteBufferSpace(int count);
        protected abstract void EnsureWriteBufferSpace01();
        protected abstract void EnsureWriteBufferSpace02();
        protected abstract void EnsureWriteBufferSpace04();
        protected abstract void EnsureWriteBufferSpace08();

        protected virtual void FinalizeWrite() {}

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new InvalidOperationException(); }
        }

        public override long Position
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        public override void Flush() {}

        public virtual int Read(byte[] buffer)
        {
            return Read(buffer, 0, buffer.Length);
        }

        public override int ReadByte()
        {
            if (! PopulateReadBuffer(sizeof(byte)))
            {
                return -1;
            }

            return readBuffer[readBufferPosition++];
        }

        public virtual Int16 ReadInt16()
        {
            if (! PopulateReadBuffer(sizeof(Int16)))
            {
                throw new EndOfStreamException();
            }

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (Int16)(
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (Int16)(
                    (readBuffer[readBufferPosition++] << 00) |
                    (readBuffer[readBufferPosition++] << 08)
                );
            }
        }

        public virtual UInt16 ReadUInt16()
        {
            if (! PopulateReadBuffer(sizeof(UInt16)))
            {
                throw new EndOfStreamException();
            }

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (UInt16)(
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (UInt16)(
                    (readBuffer[readBufferPosition++] << 00) |
                    (readBuffer[readBufferPosition++] << 08)
                );
            }
        }

        public virtual Int32 ReadInt32()
        {
            if (! PopulateReadBuffer(sizeof(Int32)))
            {
                throw new EndOfStreamException();
            }

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (Int32)(
                    (readBuffer[readBufferPosition++] << 24) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (Int32)(
                    (readBuffer[readBufferPosition++] << 00) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 24)
                );
            }
        }

        public virtual UInt32 ReadUInt32()
        {
            if (! PopulateReadBuffer(sizeof(UInt32)))
            {
                throw new EndOfStreamException();
            }

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (UInt32)(
                    (readBuffer[readBufferPosition++] << 24) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (UInt32)(
                    (readBuffer[readBufferPosition++] << 00) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 24)
                );
            }
        }

        public virtual Int64 ReadInt64()
        {
            if (! PopulateReadBuffer(sizeof(Int64)))
            {
                throw new EndOfStreamException();
            }

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (Int64)(
                    (readBuffer[readBufferPosition++] << 56) |
                    (readBuffer[readBufferPosition++] << 48) |
                    (readBuffer[readBufferPosition++] << 40) |
                    (readBuffer[readBufferPosition++] << 32) |
                    (readBuffer[readBufferPosition++] << 24) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (Int64)(
                    (readBuffer[readBufferPosition++] << 00) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 24) |
                    (readBuffer[readBufferPosition++] << 32) |
                    (readBuffer[readBufferPosition++] << 40) |
                    (readBuffer[readBufferPosition++] << 48) |
                    (readBuffer[readBufferPosition++] << 56)
                );
            }
        }

        public virtual UInt64 ReadUInt64()
        {
            if (! PopulateReadBuffer(sizeof(UInt64)))
            {
                throw new EndOfStreamException();
            }

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (UInt64)(
                    (readBuffer[readBufferPosition++] << 56) |
                    (readBuffer[readBufferPosition++] << 48) |
                    (readBuffer[readBufferPosition++] << 40) |
                    (readBuffer[readBufferPosition++] << 32) |
                    (readBuffer[readBufferPosition++] << 24) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (UInt64)(
                    (readBuffer[readBufferPosition++] << 00) |
                    (readBuffer[readBufferPosition++] << 08) |
                    (readBuffer[readBufferPosition++] << 16) |
                    (readBuffer[readBufferPosition++] << 24) |
                    (readBuffer[readBufferPosition++] << 32) |
                    (readBuffer[readBufferPosition++] << 40) |
                    (readBuffer[readBufferPosition++] << 48) |
                    (readBuffer[readBufferPosition++] << 56)
                );
            }
        }

        public virtual void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public virtual void Write(byte b)
        {
            EnsureWriteBufferSpace01();

            writeBuffer[writeBufferPosition++] = b;

            FinalizeWrite();
        }

        public virtual void Write(Int16 i)
        {
            EnsureWriteBufferSpace02();

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
            }
            else
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
            }

            FinalizeWrite();
        }

        public virtual void Write(UInt16 i)
        {
            EnsureWriteBufferSpace02();

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
            }
            else
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
            }

            FinalizeWrite();
        }

        public virtual void Write(Int32 i)
        {
            EnsureWriteBufferSpace04();

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
            }
            else
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
            }

            FinalizeWrite();
        }

        public virtual void Write(UInt32 i)
        {
            EnsureWriteBufferSpace04();

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
            }
            else
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
            }

            FinalizeWrite();
        }

        public virtual void Write(Int64 i)
        {
            EnsureWriteBufferSpace08();

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 56) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 48) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 40) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 32) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
            }
            else
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 32) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 40) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 48) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 56) & 0xFF);
            }

            FinalizeWrite();
        }

        public virtual void Write(UInt64 i)
        {
            EnsureWriteBufferSpace08();

            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 56) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 48) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 40) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 32) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
            }
            else
            {
                writeBuffer[writeBufferPosition++] = (byte)((i >> 00) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 08) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 16) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 24) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 32) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 40) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 48) & 0xFF);
                writeBuffer[writeBufferPosition++] = (byte)((i >> 56) & 0xFF);
            }

            FinalizeWrite();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }
    }
}
