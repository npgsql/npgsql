using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlBufferedStream
    {
        internal Stream Underlying { get; private set; }
        internal int Size { get; private set; }
        internal Encoding TextEncoding { get; private set; }

        byte[] _buf;
        internal int Position { get; private set; }
        int FilledBytes;
        internal int BytesLeft { get { return FilledBytes - Position; } }

        byte[] _workspace = new byte[8];
        const int DefaultBufferSize = 8192;

        internal NpgsqlBufferedStream(Stream underlying)
            : this(underlying, DefaultBufferSize, Encoding.UTF8) {}

        internal NpgsqlBufferedStream(Stream underlying, int size, Encoding textEncoding)
        {
            if (size < 8)
            {
                throw new ArgumentOutOfRangeException("size");
            }

            Underlying = underlying;
            Size = size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
        }

        internal void Ensure(int count)
        {
            Debug.Assert(count <= Size);
            if (BytesLeft >= count) { return; }

            if (Position == FilledBytes) {
                Clear();
            } else if (count > Size - Position) {
                Array.Copy(_buf, Position, _buf, 0, BytesLeft);
                FilledBytes = BytesLeft;
                Position = 0;
            }

            while (count > 0)
            {
                var read = Underlying.Read(_buf, FilledBytes, Size - FilledBytes);
                if (read == 0) { throw new EndOfStreamException(); }
                count -= read;
                FilledBytes += read;
            }
        }

        internal void CopyTo(NpgsqlBufferedStream other)
        {
            Debug.Assert(other.Size - other.BytesLeft <= BytesLeft);
            Array.Copy(_buf, Position, other._buf, other.Position += other.BytesLeft, BytesLeft);
        }

        internal void Clear()
        {
            Position = 0;
            FilledBytes = 0;
        }

        internal int Seek(int offset, SeekOrigin origin)
        {
            int absoluteOffset;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    absoluteOffset = offset;
                    break;
                case SeekOrigin.Current:
                    absoluteOffset = Position + offset;
                    break;
                case SeekOrigin.End:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            Debug.Assert(absoluteOffset >= 0 && absoluteOffset <= FilledBytes);

            Position = absoluteOffset;
            return Position;
        }

        internal void Skip(int len)
        {
            if (len > BytesLeft)
            {
                len -= BytesLeft;
                while (len > Size)
                {
                    Clear();
                    Ensure(Size);
                    len -= Size;
                }
                Clear();
                Ensure(len);
            }

            Position += len;
        }

        #region Read

        internal byte ReadByte()
        {
            Debug.Assert(BytesLeft >= sizeof(byte));
            return _buf[Position++];
        }

        internal short ReadInt16()
        {
            Debug.Assert(BytesLeft >= sizeof(short));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_buf, Position));
            Position += 2;
            return result;
        }

        internal int ReadInt32()
        {
            Debug.Assert(BytesLeft >= sizeof(int));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_buf, Position));
            Position += 4;
            return result;
        }

        internal long ReadInt64()
        {
            Debug.Assert(BytesLeft >= sizeof(long));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(_buf, Position));
            Position += 8;
            return result;
        }

        internal float ReadSingle()
        {
            Debug.Assert(BytesLeft >= sizeof(float));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[3] = _buf[Position++];
                _workspace[2] = _buf[Position++];
                _workspace[1] = _buf[Position++];
                _workspace[0] = _buf[Position++];
                return BitConverter.ToSingle(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToSingle(_buf, Position);
                Position += 4;
                return result;
            }
        }

        internal double ReadDouble()
        {
            Debug.Assert(BytesLeft >= sizeof(double));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[7] = _buf[Position++];
                _workspace[6] = _buf[Position++];
                _workspace[5] = _buf[Position++];
                _workspace[4] = _buf[Position++];
                _workspace[3] = _buf[Position++];
                _workspace[2] = _buf[Position++];
                _workspace[1] = _buf[Position++];
                _workspace[0] = _buf[Position++];
                return BitConverter.ToDouble(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToDouble(_buf, Position);
                Position += 8;
                return result;
            }
        }

        internal string ReadString(int len)
        {
            Debug.Assert(BytesLeft >= len);
            var result = TextEncoding.GetString(_buf, Position, len);
            Position += len;
            return result;
        }

        internal string ReadNullTerminatedString()
        {
            int i;
            for (i = Position; _buf[i] != 0; i++) {
                Debug.Assert(i <= Position + BytesLeft);
            }
            Debug.Assert(i > Position);
            var result = TextEncoding.GetString(_buf, Position, i - Position);
            Position = i + 1;
            return result;
        }

        #endregion
    }
}
