using System;
using System.IO;
using System.Threading.Tasks;

namespace Npgsql
{
    sealed class PreparedTextReader : TextReader
    {
        readonly string _str;
        readonly Stream _stream;

        int _position;

        public PreparedTextReader(string str, Stream stream)
        {
            _str = str;
            _stream = stream;
        }

        public override int Peek()
        {
            var value = -1;
            if (_position < _str.Length)
                value = _str[_position];

            return value;
        }

        public override int Read()
        {
            var value = -1;
            if (_position < _str.Length)
            {
                value = _str[_position];
                _position++;
            }

            return value;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count));
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            var toRead = Math.Min(count, _str.Length - _position);
            if (toRead == 0)
                return 0;

            _str.CopyTo(_position, buffer, index, toRead);
            _position += toRead;
            return toRead;
        }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
            => Task.FromResult(Read(buffer, index, count));

        public override string ReadToEnd()
        {
            if (_position == _str.Length)
                return string.Empty;

            var str = _str.Substring(_position);
            _position = _str.Length;
            return str;
        }

        public override Task<string> ReadToEndAsync() => Task.FromResult(ReadToEnd());

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            if (disposing)
                _stream.Dispose();
        }
    }
}
