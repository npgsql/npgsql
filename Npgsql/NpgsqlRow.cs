// created on 4/3/2003 at 19:45

// Npgsql.NpgsqlBinaryRow.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Localization;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// This is the abstract base class for NpgsqlAsciiRow and NpgsqlBinaryRow.
    /// </summary>
    internal abstract class NpgsqlRow : IStreamOwner
    {
        public abstract object Get(int index);
        public abstract Task<object> GetAsync(int index);
        public abstract int NumFields { get; }
        public abstract bool IsDBNull(int index);
        public abstract Task<bool> IsDBNullAsync(int index);
        public abstract void Dispose();
        public abstract long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);
        public abstract long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);
    }

    internal sealed class CachingRow : NpgsqlRow
    {
        private readonly List<object> _data = new List<object>();
        private readonly ForwardsOnlyRow _inner;

        public CachingRow(ForwardsOnlyRow fo)
        {
            _inner = fo;
        }

        public override object Get(int index)
        {
            if ((index < 0) || (index >= NumFields))
            {
                throw new IndexOutOfRangeException("this[] index value");
            }
            while (_data.Count <= index)
            {
                _data.Add(_inner.Get(_data.Count));
            }
            return _data[index];
        }

        /// <summary>
        /// Async implementation of <see cref="Get"/>.
        ///
        /// Note that since the CachingRow has already read all the columns into memory, no I/O
        /// operation is needed and therefore this method simply calls <see cref="Get"/>
        /// </summary>
        public override Task<object> GetAsync(int index)
        {
            return PGUtil.TaskFromResult(Get(index));
        }

        public override int NumFields
        {
            get { return _inner.NumFields; }
        }

        public override bool IsDBNull(int index)
        {
            return Get(index) == DBNull.Value;
        }

        /// <summary>
        /// Async implementation of <see cref="IsDBNull"/>.
        ///
        /// Note that since the CachingRow has already read all the columns into memory, no I/O
        /// operation is needed and therefore this method simply calls <see cref="IsDBNull"/>
        /// </summary>
        public override Task<bool> IsDBNullAsync(int index)
        {
            return PGUtil.TaskFromResult(IsDBNull(index));
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            byte[] source = (byte[]) Get(i);
            if (buffer == null)
            {
                return source.Length - fieldOffset;
            }
            long finalLength = Math.Max(0, Math.Min(length, source.Length - fieldOffset));
            Array.Copy(source, fieldOffset, buffer, bufferoffset, finalLength);
            return finalLength;
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            string source = (string) Get(i);
            if (buffer == null)
            {
                return source.Length - fieldoffset;
            }
            long finalLength = Math.Max(0, Math.Min(length, source.Length - fieldoffset));
            Array.Copy(source.ToCharArray(), fieldoffset, buffer, bufferoffset, finalLength);
            return finalLength;
        }

        public override void Dispose()
        {
            _inner.Dispose();
        }
    }

    internal sealed partial class ForwardsOnlyRow : NpgsqlRow
    {
        /// <summary>
        /// The index of the current field in the stream, i.e. the one that hasn't
        /// been read yet
        /// </summary>
        private int _i;
        private readonly RowReader _reader;

        public ForwardsOnlyRow(RowReader reader)
        {
            _reader = reader;
        }

        [GenerateAsync]
        private void Seek(int index, bool consume)
        {
            if (index < 0 || index >= NumFields) {
                throw new IndexOutOfRangeException();
            }

            var d = index - _i;

            if (d < 0)
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, index, _i));
            if (d > 0)
            {
                _reader.Skip(d);
                _i += d;                
            }
            if (consume)
                _i++;
        }

        public void SetRowDescription(NpgsqlRowDescription rowDescr)
        {
            _reader.SetRowDescription(rowDescr);
        }

        [GenerateAsync(withOverride: true)]
        public override object Get(int index)
        {
            Seek(index, true);
            return _reader.GetNext();
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (buffer == null)
            {
                throw new NotSupportedException();
            }
            if (!_reader.CanGetByteStream(i))
            {
                throw new InvalidCastException();
            }
            Seek(i, true);
            _reader.SkipBytesTo(fieldOffset);
            return _reader.Read(buffer, bufferoffset, length);
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            if (buffer == null)
            {
                throw new NotSupportedException();
            }
            if (!_reader.CanGetCharStream(i))
            {
                throw new InvalidCastException();
            }
            Seek(i, true);
            _reader.SkipCharsTo(fieldoffset);
            return _reader.Read(buffer, bufferoffset, length);
        }

        public override int NumFields
        {
            get { return _reader.NumFields; }
        }

        [GenerateAsync(withOverride: true)]
        public override bool IsDBNull(int index)
        {
            Seek(index, false);
            return _reader.IsNull;
        }

        public override void Dispose()
        {
            _reader.Dispose();
        }
    }

    /// <summary>
    /// Reads a row, field by field, allowing a DataRow to be built appropriately.
    /// </summary>
    internal abstract partial class RowReader : IStreamOwner
    {
        /// <summary>
        /// Reads part of a field, as needed (for <see cref="System.Data.IDataRecord.GetChars(int, long, char[], int, int)"/>
        /// and <see cref="System.Data.IDataRecord.GetBytes(int, long, byte[], int, int)"/>
        /// </summary>
        protected abstract class Streamer : IStreamOwner
        {
            protected readonly NpgsqlStream _stream;
            protected int _remainingBytes;
            private int _alreadyRead = 0;

            protected Streamer(NpgsqlStream stream, int remainingBytes)
            {
                _stream = stream;
                _remainingBytes = remainingBytes;
            }

            public int AlreadyRead
            {
                get { return _alreadyRead; }
                protected set { _alreadyRead = value; }
            }

            public void Dispose()
            {
                _stream.EatStreamBytes(_remainingBytes);
            }
        }

        /// <summary>
        /// Adds further functionality to stream that is dependant upon the type of data read.
        /// </summary>
        protected abstract class Streamer<T> : Streamer
        {
            protected Streamer(NpgsqlStream stream, int remainingBytes)
                : base(stream, remainingBytes)
            {
            }

            public abstract int DoRead(T[] output, int outputIdx, int length);
            public abstract int DoSkip(int length);

            public int Read(T[] output, int outputIdx, int length)
            {
                int ret = DoRead(output, outputIdx, length);
                AlreadyRead += ret;
                return ret;
            }

            private void Skip(int length)
            {
                AlreadyRead += DoSkip(length);
            }

            public void SkipTo(long position)
            {
                if (position < AlreadyRead)
                {
                    throw new InvalidOperationException();
                }
                Skip((int) position - AlreadyRead);
            }
        }

        /// <summary>
        /// Completes the implementation of Streamer for char data.
        /// </summary>
        protected sealed class CharStreamer : Streamer<char>
        {
            public CharStreamer(NpgsqlStream stream, int remainingBytes)
                : base(stream, remainingBytes)
            {
            }

            public override int DoRead(char[] output, int outputIdx, int length)
            {
                return _stream.ReadChars(output, length, ref _remainingBytes, outputIdx);
            }

            public override int DoSkip(int length)
            {
                return _stream.SkipChars(length, ref _remainingBytes);
            }
        }

        /// <summary>
        /// Completes the implementation of Streamer for byte data.
        /// </summary>
        protected sealed class ByteStreamer : Streamer<byte>
        {
            public ByteStreamer(NpgsqlStream stream, int remainingBytes)
                : base(stream, remainingBytes)
            {
            }

            public override int DoRead(byte[] output, int outputIdx, int length)
            {
                return _stream.ReadEscapedBytes(output, length, ref _remainingBytes, outputIdx);
            }

            public override int DoSkip(int length)
            {
                return _stream.SkipEscapedBytes(length, ref _remainingBytes);
            }
        }

        protected static readonly Encoding UTF8Encoding = Encoding.UTF8;
        protected NpgsqlRowDescription _rowDesc;
        private readonly NpgsqlStream _stream;
        private Streamer _streamer;
        protected int _currentField = -1;

        public RowReader(NpgsqlStream stream)
        {
            _stream = stream;
        }

        public virtual void SetRowDescription(NpgsqlRowDescription rowDesc)
        {
            _rowDesc = rowDesc;
        }

        protected Streamer CurrentStreamer
        {
            get { return _streamer; }
            set
            {
                if (_streamer != null)
                {
                    _streamer.Dispose();
                }
                _streamer = value;
            }
        }

        public bool CurrentlyStreaming
        {
            get { return _streamer != null; }
        }

        public bool CanGetByteStream(int index)
        {
//TODO: Add support for byte[] being read as a stream of bytes.
            return _rowDesc[index].TypeInfo.NpgsqlDbType == NpgsqlDbType.Bytea;
        }

        public bool CanGetCharStream(int index)
        {
//TODO: Add support for arrays of string types?
            return _rowDesc[index].TypeInfo.Type.Equals(typeof (string));
        }

        protected Streamer<byte> CurrentByteStreamer
        {
            get
            {
                if (CurrentStreamer == null)
                {
                    if (!CanGetByteStream(_currentField + 1))
                    {
                        throw new InvalidCastException();
                    }
                    ++_currentField;
                    return (CurrentStreamer = new ByteStreamer(Stream, GetNextFieldCount())) as ByteStreamer;
                }
                else if (!(CurrentStreamer is Streamer<byte>))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return CurrentStreamer as ByteStreamer;
                }
            }
        }

        protected Streamer<char> CurrentCharStreamer
        {
            get
            {
                if (CurrentStreamer == null)
                {
                    if (!CanGetCharStream(_currentField + 1))
                    {
                        throw new InvalidCastException();
                    }
                    ++_currentField;
                    return (CurrentStreamer = new CharStreamer(Stream, GetNextFieldCount())) as CharStreamer;
                }
                else if (!(CurrentStreamer is Streamer<char>))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return CurrentStreamer as CharStreamer;
                }
            }
        }

        protected NpgsqlStream Stream
        {
            get { return _stream; }
        }

        protected NpgsqlRowDescription.FieldData FieldData
        {
            get { return _rowDesc[_currentField]; }
        }

        public int NumFields
        {
            get { return _rowDesc.NumFields; }
        }

        protected int CurrentField
        {
            get { return _currentField; }
        }

        protected abstract object ReadNext();
        protected abstract Task<object> ReadNextAsync();

        [GenerateAsync]
        public object GetNext()
        {
            if (++_currentField == _rowDesc.NumFields)
            {
                throw new IndexOutOfRangeException();
            }
            return ReadNext();
        }

        public abstract bool IsNull { get; }
        protected abstract void SkipOne();
        protected abstract Task SkipOneAsync();

        [GenerateAsync]
        public void Skip(int count)
        {
            if (count > 0)
            {
                if (_currentField + count >= _rowDesc.NumFields)
                {
                    throw new IndexOutOfRangeException();
                }
                while (count-- > 0)
                {
                    ++_currentField;
                    SkipOne();
                }
            }
        }

        protected abstract int GetNextFieldCount();

        public int Read(byte[] output, int outputIdx, int length)
        {
            return CurrentByteStreamer.Read(output, outputIdx, length);
        }

        public void SkipBytesTo(long position)
        {
            CurrentByteStreamer.SkipTo(position);
        }

        public int Read(char[] output, int outputIdx, int length)
        {
            return CurrentCharStreamer.Read(output, outputIdx, length);
        }

        public void SkipCharsTo(long position)
        {
            CurrentCharStreamer.SkipTo(position);
        }

        public virtual void Dispose()
        {}
    }
}
