﻿// BufferedNetworkStream.cs

// Authors:
// Udo Liess <udo.liess@gmx.net>

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
using System.IO;

namespace Npgsql
{
    /// <summary>
    /// This stream buffers reads and writes in separate BufferedStream instances.
    /// Additionally it reads one byte in background and thus can signal if there are data available for reading.
    /// After writing Flush() should be called to send the data.
    /// </summary>
    class BufferedNetworkStream : Stream
    {
        Stream readStream;
        Stream writeStream;
        IAsyncResult readResult;
        byte[] readBuffer = new byte[1];

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseStream"></param>
        /// <param name="bufferSize"></param>
        public BufferedNetworkStream(Stream baseStream, int bufferSize)
        {
            readStream = new BufferedStream(baseStream, bufferSize);
            writeStream = new BufferedStream(baseStream, bufferSize);
            BackgroundReading();
        }

        /// <summary>
        /// Constructor.
        /// The buffer size is set to ushort.MaxValue.
        /// </summary>
        /// <param name="baseStream"></param>
        public BufferedNetworkStream(Stream baseStream)
            : this(baseStream, ushort.MaxValue) { }

        void BackgroundReading()
        {
            readResult = readStream.BeginRead(readBuffer, 0, 1, null, null);
        }

        /// <summary>
        /// </summary>
        public override bool CanRead
        {
            get { return readStream.CanRead; }
        }

        /// <summary>
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// </summary>
        public override bool CanWrite
        {
            get { return writeStream.CanWrite; }
        }

        /// <summary>
        /// Flush() should be called after writing in order to send buffered data.
        /// </summary>
        public override void Flush()
        {
            writeStream.Flush();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
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

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count < 1)
                return 0;
            var result = readStream.EndRead(readResult);
            if (result > 0)
            {
                buffer[offset] = readBuffer[0];
                if (count > result)
                    result += readStream.Read(buffer, offset + 1, count - 1);
            }
            BackgroundReading();
            return result;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Written data are buffered. To be sure they are sent, call Flush() after writing.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            writeStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Indicates if data is available for reading.
        /// </summary>
        public bool Available
        {
            get
            {
                return readResult.IsCompleted;
            }
        }

        static readonly TimeSpan waitLimit = TimeSpan.FromMilliseconds(int.MaxValue); // maximum for WaitHandle.WaitOne()

        /// <summary>
        /// Waits for data available for reading.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>True is data is available for reading.</returns>
        public bool WaitAvailable(TimeSpan timeout)
        {
            do
            {
                var waitNow = timeout > waitLimit ? waitLimit : timeout;
                if (readResult.AsyncWaitHandle.WaitOne(waitNow))
                    return Available;
                timeout -= waitNow;
            } while (timeout > TimeSpan.Zero);
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && readBuffer != null)
            {
                try { Flush(); }
                catch { }
                try { readStream.Close(); }
                catch { }
                try { writeStream.Close(); }
                catch { }
                var s = readStream;
                var r = readResult;
                var a = new Action(() => { try { s.EndRead(r); } catch { } });
                try { a.BeginInvoke(ar => a.EndInvoke(ar), null); }
                catch { }
                readStream = null;
                writeStream = null;
                readResult = null;
                readBuffer = null;
                base.Dispose(disposing);
            }
        }
    }
}
