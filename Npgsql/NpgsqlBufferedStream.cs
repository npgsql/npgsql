using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlBufferedStream : NpgsqlStream
    {
        private bool ownStream;
        private int minMultiByteStringChunkedEncode;

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

            minMultiByteStringChunkedEncode = maxBytesPerChar == 1 ? 1 : 24;
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

            if (count == 0)
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

        public override string ReadString(ReadStringoptions options, int byteCount = -1)
        {
            bool readLength = ((options & ReadStringoptions.ReadLength) == ReadStringoptions.ReadLength);
            bool nullTerminated = ((options & ReadStringoptions.NullTerminated) == ReadStringoptions.NullTerminated);

            if (readLength && byteCount >= 0)
            {
                throw new ArgumentException("byteCount");
            }

            if (! readLength && ! nullTerminated && byteCount < 0)
            {
                throw new ArgumentException("byteCount");
            }

            string result = string.Empty;

            if (readLength)
            {
                byteCount = ReadInt32();

                if (byteCount < sizeof(Int32) - (nullTerminated ? 1 : 0))
                {
                    throw new IOException("Embedded length identifier out of range");
                }
            }

            if (byteCount >= 0)
            {
                if (readLength)
                {
                    byteCount -= sizeof(Int32);
                }

                if (nullTerminated)
                {
                    byteCount -= 1;
                }

                if (byteCount > 0)
                {
                    if (byteCount <= BufferSize)
                    {
                        PopulateReadBuffer(byteCount);

                        result = textEncoding.GetString(readBuffer, readBufferPosition, byteCount);
                        readBufferPosition += byteCount;
                    }
                    else
                    {
                        using (NpgsqlMemoryStream stream = new NpgsqlMemoryStream(false, textEncoding))
                        {
                            CopyTo(stream, 0, byteCount);

                            result = stream.ReadString(ReadStringoptions.None, byteCount);
                        }
                    }
                }

                if (nullTerminated)
                {
                    ReadByte();
                }
            }
            else if (nullTerminated)
            {
                result = ReadNullTerminatedString();
            }

            return result;
        }

        private string ReadNullTerminatedString()
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
                                result = stream.ReadString(ReadStringoptions.None, byteCount);
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

        public override void Skip(int byteCount)
        {
            int skipped = 0;

            while (skipped < byteCount)
            {
                PopulateReadBuffer(1);

                int count = Math.Min(byteCount - skipped, readBufferCapacity - readBufferPosition);

                readBufferPosition += count;
                skipped += count;
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

        public override NpgsqlStream WriteString(string text, int charOffset, int charCount, WriteStringoptions options = WriteStringoptions.None)
        {
            bool prependLength = ((options & WriteStringoptions.PrependLength) == WriteStringoptions.PrependLength);
            bool nullTerminate = ((options & WriteStringoptions.NullTerminate) == WriteStringoptions.NullTerminate);
            int bytesPerChar = ((options & WriteStringoptions.ASCII) == WriteStringoptions.ASCII) ? 1 : maxBytesPerChar;
            int textByteCount;
            int totalByteCount;
            bool byteCountExact = false;

            if (bytesPerChar == 1)
            {
                textByteCount = charCount;
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0);
                byteCountExact = true;
            }
            else
            {
                textByteCount = bytesPerChar * charCount;
                totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0);

                if (textByteCount > BufferSize)
                {
                    if (charCount < text.Length)
                    {
                        text = text.Substring(charOffset, charCount);
                        charOffset = 0;
                        charCount = text.Length;
                    }

                    textByteCount = textEncoding.GetByteCount(text);
                    totalByteCount = textByteCount + (prependLength ? sizeof(Int32) : 0);
                    byteCountExact = true;
                }
            }

            if (totalByteCount > 0)
            {
                int count = 0;

                if (byteCountExact)
                {
                    if (prependLength)
                    {
                        WriteInt32(sizeof(Int32) + textByteCount + (nullTerminate ? 1 : 0));
                    }

                    if (textByteCount <= BufferSize)
                    {
                        EnsureWriteBufferSpace(textByteCount);

                        textEncoding.GetBytes(text, charOffset, charCount, writeBuffer, writeBufferPosition);

                        writeBufferPosition += textByteCount;
                    }
                    else
                    {
                        for (int totalCharsWritten = 0 ; totalCharsWritten < charCount ; )
                        {
                            int charWriteCount;

                            if (BufferSize - writeBufferPosition >= minMultiByteStringChunkedEncode)
                            {
                                charWriteCount = Math.Min(charCount - totalCharsWritten, (BufferSize - writeBufferPosition) / bytesPerChar);
                            }
                            else
                            {
                                charWriteCount = Math.Min(charCount - totalCharsWritten, BufferSize / bytesPerChar);
                            }

                            EnsureWriteBufferSpace(charWriteCount);

                            count = textEncoding.GetBytes(text, charOffset + totalCharsWritten, charWriteCount, writeBuffer, writeBufferPosition);

                            writeBufferPosition += count;
                            totalCharsWritten += charWriteCount;

                            if (totalCharsWritten < charCount && bytesPerChar > 1)
                            {
                                while (char.IsHighSurrogate(text[charOffset + totalCharsWritten - 1]))
                                {
                                    totalCharsWritten -= 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int writePosition;

                    EnsureWriteBufferSpace(totalByteCount);

                    writePosition = writeBufferPosition;

                    if (prependLength)
                    {
                        writePosition += sizeof(Int32);
                    }

                    try
                    {
                        count = textEncoding.GetBytes(text, charOffset, charCount, writeBuffer, writePosition);

                        if (count > textByteCount)
                        {
                            throw new ArgumentException("options");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("text", e);
                    }

                    if (prependLength)
                    {
                        WriteInt32(sizeof(Int32) + count + (nullTerminate ? 1 : 0));
                    }

                    writeBufferPosition += count;
                }
            }

            if (nullTerminate)
            {
                WriteByte(0);
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

        public override void CopyTo(Stream destination)
        {
            throw new InvalidOperationException();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            throw new InvalidOperationException();
        }

        public override void CopyTo(Stream destination, int bufferSize, int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                PopulateReadBuffer(1);

                int readCount = Math.Min(count - totalBytesRead, readBufferCapacity - readBufferPosition);

                destination.Write(readBuffer, readBufferPosition, readCount);
                readBufferPosition += readCount;
                totalBytesRead += readCount;
            }
        }
    }
}
