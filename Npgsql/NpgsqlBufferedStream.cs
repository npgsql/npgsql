using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            Debug.Assert(other.Size - other.FilledBytes >= BytesLeft);
            Array.Copy(_buf, Position, other._buf, other.FilledBytes, BytesLeft);
            other.FilledBytes += BytesLeft;
        }

        internal void Clear()
        {
            Position = 0;
            FilledBytes = 0;
        }

        /// <summary>
        /// Seeks within the current in-memory data. Does not read any data from the underlying.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        internal void Seek(int offset, SeekOrigin origin)
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

        /// <summary>
        /// Note that unlike the primitive readers, this reader can read any length, looping internally
        /// and reading directly from Underlying.
        /// </summary>
        internal void ReadBytes(byte[] output, int outputOffset, int len)
        {
            if (len <= BytesLeft)
            {
                Array.Copy(_buf, Position, output, outputOffset, len);
                Position += len;
                return;
            }

            Array.Copy(_buf, Position, output, outputOffset, BytesLeft);
            var offset = outputOffset + BytesLeft;
            len -= BytesLeft;
            Clear();
            while (len > 0)
            {
                var read = Underlying.Read(output, offset, len);
                if (read == 0) { throw new EndOfStreamException(); }
                len -= read;
                offset += read;
            }
        }

        #endregion

        internal void Skip(long len)
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
                Ensure((int)len);
            }

            Position += (int)len;
        }

        internal Stream GetStream(int len)
        {
            // All requested bytes are already in memory, return a simple MemoryStream over them
            if (len <= BytesLeft) {
                return new MemoryStream(_buf, Position, len, false, false);
            }

            var ms = new MemoryStream(_buf, Position, BytesLeft, false, false);
            Position = FilledBytes;
            return new MultiStream(ms, this, len);   // TODO: Recycle?
        }

        /// <summary>
        /// Encapsulates a block of data already in memory and pending data still not read,
        /// exposing them as a single, concatenated stream. Used when reading sequentially.
        /// 
        /// When the MultiStream is closed, the buffered stream will be read up to the length given.
        /// This consumes the column's data and leaves us at the beginning of the next column.
        /// </summary>
        internal class MultiStream : Stream
        {
            int _pos, _len;
            readonly MemoryStream _s1;
            readonly NpgsqlBufferedStream _buf;

            /// <summary>
            /// Constructs the MultiStream over the given streams
            /// </summary>
            /// <param name="memoryStream"></param>
            /// <param name="buf"></param>
            /// <param name="len">the total length of the combined streams</param>
            internal MultiStream(MemoryStream memoryStream, NpgsqlBufferedStream buf, int len)
            {
                _s1 = memoryStream;
                _buf = buf;
                _len = len;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var readFromMemory = 0;
                if (_pos > _s1.Length)
                {
                    readFromMemory = Math.Min(count, (int)_s1.Length - _pos);
                    _s1.Read(buffer, offset, readFromMemory);
                    _pos += readFromMemory;
                    offset += readFromMemory;
                    count -= readFromMemory;
                }

                if (count == 0) {
                    return readFromMemory;
                }

                var readFromSocket = _buf.Underlying.Read(buffer, offset, count);
                _pos += readFromSocket;
                return readFromMemory + readFromSocket;
            }

            public async override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                var readFromMemory = 0;
                if (_pos > _s1.Length)
                {
                    readFromMemory = Math.Min(count, (int)_s1.Length - _pos);
                    _s1.Read(buffer, offset, readFromMemory);
                    _pos += readFromMemory;
                    offset += readFromMemory;
                    count -= readFromMemory;
                }

                if (count == 0)
                {
                    return readFromMemory;
                }

                var readFromSocket = await _buf.Underlying.ReadAsync(buffer, offset, count, cancellationToken);
                _pos += readFromSocket;
                return readFromMemory + readFromSocket;
            }

            public override void Close()
            {
                var needToRead = Math.Min(_len - _pos, _len - _s1.Length);
                _buf.Skip(needToRead);
            }

            public override long Position
            {
                get { return _pos; }
                set { throw new NotSupportedException(); }
            }

            public override long Length
            {
                get { return _len; }
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            #region Unsupported

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
