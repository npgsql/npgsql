using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal enum ReadStringoptions
    {
        None = 0,
        ReadLength = 1,
        NullTerminated = 2,
    }

    internal enum WriteStringoptions
    {
        None = 0,
        PrependLength = 1,
        NullTerminate = 2,
        ASCII = 4
    }

    internal abstract class NpgsqlStream : Stream
    {
        protected byte[] readBuffer;
        protected int readBufferCapacity;
        protected int readBufferPosition;
        protected byte[] writeBuffer;
        protected int writeBufferPosition;
        protected bool performNetworkByteOrderSwap;
        protected Encoding textEncoding;

        public NpgsqlStream(bool performNetworkByteOrderSwap, Encoding textEncoding)
        {
            this.performNetworkByteOrderSwap = performNetworkByteOrderSwap;
            this.textEncoding = textEncoding;
        }

        protected abstract bool PopulateReadBuffer(int count);

        protected abstract void EnsureWriteBufferSpace(int count);

        protected virtual void FinalizeWrite() {}

        public virtual string ReadString(int byteCount)
        {
            return ReadString(ReadStringoptions.None, byteCount);
        }

        public abstract string ReadString(ReadStringoptions options, int byteCount = -1);

        public abstract void Skip(int byteCount);

        public virtual NpgsqlStream WriteString(string text, WriteStringoptions options = WriteStringoptions.None)
        {
            return WriteString(text, 0, text.Length, options);
        }

        public virtual NpgsqlStream WriteASCIIString(string text)
        {
            return WriteString(text, 0, text.Length, WriteStringoptions.ASCII);
        }

        public abstract NpgsqlStream WriteString(string text, int charOffset, int charCount, WriteStringoptions options = WriteStringoptions.None);

        public new abstract void CopyTo(Stream destination);
        public new abstract void CopyTo(Stream destination, int bufferSize);
        public abstract void CopyTo(Stream destination, int bufferSize, int count);

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

        public virtual void ReadExact(byte[] buffer, int offset, int count)
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

            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead;

                bytesRead = Read(buffer, totalBytesRead, count - totalBytesRead);
                totalBytesRead += bytesRead;
            }
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

        protected virtual Int32 ReadInt32NoAdvance()
        {
            if (performNetworkByteOrderSwap && BitConverter.IsLittleEndian)
            {
                return (Int32)(
                    (readBuffer[readBufferPosition + 0] << 24) |
                    (readBuffer[readBufferPosition + 1] << 16) |
                    (readBuffer[readBufferPosition + 2] << 08) |
                    (readBuffer[readBufferPosition + 3] << 00)
                );
            }
            else
            {
                return (Int32)(
                    (readBuffer[readBufferPosition + 0] << 00) |
                    (readBuffer[readBufferPosition + 1] << 08) |
                    (readBuffer[readBufferPosition + 2] << 16) |
                    (readBuffer[readBufferPosition + 3] << 24)
                );
            }
        }

        public virtual Int32 ReadInt32()
        {
            if (! PopulateReadBuffer(sizeof(Int32)))
            {
                throw new EndOfStreamException();
            }

            Int32 result;

            result = ReadInt32NoAdvance();

            readBufferPosition += sizeof(Int32);

            return result;
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
                    ((Int64)readBuffer[readBufferPosition++] << 56) |
                    ((Int64)readBuffer[readBufferPosition++] << 48) |
                    ((Int64)readBuffer[readBufferPosition++] << 40) |
                    ((Int64)readBuffer[readBufferPosition++] << 32) |
                    ((Int64)readBuffer[readBufferPosition++] << 24) |
                    ((Int64)readBuffer[readBufferPosition++] << 16) |
                    ((Int64)readBuffer[readBufferPosition++] << 08) |
                    ((Int64)readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (Int64)(
                    ((Int64)readBuffer[readBufferPosition++] << 00) |
                    ((Int64)readBuffer[readBufferPosition++] << 08) |
                    ((Int64)readBuffer[readBufferPosition++] << 16) |
                    ((Int64)readBuffer[readBufferPosition++] << 24) |
                    ((Int64)readBuffer[readBufferPosition++] << 32) |
                    ((Int64)readBuffer[readBufferPosition++] << 40) |
                    ((Int64)readBuffer[readBufferPosition++] << 48) |
                    ((Int64)readBuffer[readBufferPosition++] << 56)
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
                    ((UInt64)readBuffer[readBufferPosition++] << 56) |
                    ((UInt64)readBuffer[readBufferPosition++] << 48) |
                    ((UInt64)readBuffer[readBufferPosition++] << 40) |
                    ((UInt64)readBuffer[readBufferPosition++] << 32) |
                    ((UInt64)readBuffer[readBufferPosition++] << 24) |
                    ((UInt64)readBuffer[readBufferPosition++] << 16) |
                    ((UInt64)readBuffer[readBufferPosition++] << 08) |
                    ((UInt64)readBuffer[readBufferPosition++] << 00)
                );
            }
            else
            {
                return (UInt64)(
                    ((UInt64)readBuffer[readBufferPosition++] << 00) |
                    ((UInt64)readBuffer[readBufferPosition++] << 08) |
                    ((UInt64)readBuffer[readBufferPosition++] << 16) |
                    ((UInt64)readBuffer[readBufferPosition++] << 24) |
                    ((UInt64)readBuffer[readBufferPosition++] << 32) |
                    ((UInt64)readBuffer[readBufferPosition++] << 40) |
                    ((UInt64)readBuffer[readBufferPosition++] << 48) |
                    ((UInt64)readBuffer[readBufferPosition++] << 56)
                );
            }
        }

        public virtual NpgsqlStream Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);

            return this;
        }

        public override void WriteByte(byte b)
        {
            EnsureWriteBufferSpace(sizeof(byte));

            writeBuffer[writeBufferPosition++] = b;

            FinalizeWrite();
        }

        public virtual NpgsqlStream WriteInt16(Int16 i)
        {
            EnsureWriteBufferSpace(sizeof(Int16));

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

            return this;
        }

        public virtual NpgsqlStream WriteUInt16(UInt16 i)
        {
            EnsureWriteBufferSpace(sizeof(UInt16));

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

            return this;
        }

        public virtual NpgsqlStream WriteInt32(Int32 i)
        {
            EnsureWriteBufferSpace(sizeof(Int32));

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

            return this;
        }

        public virtual NpgsqlStream WriteUInt32(UInt32 i)
        {
            EnsureWriteBufferSpace(sizeof(UInt32));

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

            return this;
        }

        public virtual NpgsqlStream WriteInt64(Int64 i)
        {
            EnsureWriteBufferSpace(sizeof(Int64));

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

            return this;
        }

        public virtual NpgsqlStream WriteUInt64(UInt64 i)
        {
            EnsureWriteBufferSpace(sizeof(UInt64));

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

            return this;
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
