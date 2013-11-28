// BufferedWriteStream

// Authors:
// Udo Liess <udo.liess@gmx.net>

// Buffered write operations and direct read operations for network stream.
// After writing Flush() should be called to send the data.

// 2013-11-26 initial version

using System;
using System.IO;

namespace Npgsql
{
    class BufferedWriteStream : Stream
    {
        Stream readStream;
        Stream writeStream;

        public BufferedWriteStream(Stream baseStream, int bufferSize)
        {
            this.readStream = baseStream;
            writeStream = new BufferedStream(baseStream, bufferSize);
        }

        public override bool CanRead
        {
            get { return readStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return writeStream.CanWrite; }
        }

        public override void Flush()
        {
            writeStream.Flush();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return readStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            writeStream.Write(buffer, offset, count);
        }
    }
}
