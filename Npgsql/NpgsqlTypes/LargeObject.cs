/*-------------------------------------------------------------------------

  LargeObject.cs
      This class is a port of the class LargeObject.java implemented by
      PostgreSQL Global Development Group

  Copyright (c) 2004, Emiliano Necciari
  Original Code: Copyright (c) 2003, PostgreSQL Global Development Group

  Note: (Francisco Figueiredo Jr.)
      Changed case of method names to conform to .Net names standard.
      Also changed type names to their true names. i.e. int -> Int32

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
-------------------------------------------------------------------------
*/

using System;

namespace NpgsqlTypes
{
    /// <summary>
    /// Large Object.
    /// </summary>
    public class LargeObject
    {
        /// <summary>
        /// Indicates a seek from the begining of a file.
        /// </summary>
        public const Int32 SEEK_SET = 0;

        /// <summary>
        /// Indicates a seek from the current position.
        /// </summary>
        public const Int32 SEEK_CUR = 1;

        /// <summary>
        /// Indicates a seek from the end of a file.
        /// </summary>
        public const Int32 SEEK_END = 2;

        private readonly Fastpath fp; // Fastpath API to use
        private readonly Int32 oid; // OID of this object
        private readonly Int32 fd; // the descriptor of the open large object

        private Boolean closed = false; // true when we are closed

        /// <summary>
        /// This opens a large object.
        /// If the object does not exist, then an NpgsqlException is thrown.
        /// </summary>
        /// <param name="fp">FastPath API for the connection to use.</param>
        /// <param name="oid">OID of the Large Object to open.</param>
        /// <param name="mode">Mode of opening the large object</param>
        public LargeObject(Fastpath fp, Int32 oid, Int32 mode)
        {
            this.fp = fp;
            this.oid = oid;

            FastpathArg[] args = new FastpathArg[2];
            args[0] = new FastpathArg(oid);
            args[1] = new FastpathArg(mode);
            this.fd = fp.GetInteger("lo_open", args);
        }

        /// <summary>
        /// OID getter.
        /// </summary>
        /// <returns>The OID of this LargeObject.</returns>
        public Int32 GetOID()
        {
            return oid;
        }

        /// <summary>
        /// OID.
        /// </summary>
        public Int32 OID
        {
            get { return oid; }
        }

        /// <summary>
        /// This method closes the object. You must not call methods in this
        /// object after this is called.
        /// </summary>
        public void Close()
        {
            if (!closed)
            {
                // finally close
                FastpathArg[] args = new FastpathArg[1];
                args[0] = new FastpathArg(fd);
                fp.FastpathCall("lo_close", false, args); // true here as we dont care!!
                closed = true;
            }
        }

        /// <summary>
        /// Reads some data from the object, and return as a byte[] array.
        /// </summary>
        /// <param name="len">Number of bytes to read.</param>
        /// <returns>Array containing data read.</returns>
        public Byte[] Read(Int32 len)
        {
            // This is the original method, where the entire block (len bytes)
            // is retrieved in one go.
            FastpathArg[] args = new FastpathArg[2];
            args[0] = new FastpathArg(fd);
            args[1] = new FastpathArg(len);
            return fp.GetData("loread", args);

            // This version allows us to break this down Int32o 4k blocks
            //if (len<=4048) {
            //// handle as before, return the whole block in one go
            //FastpathArg args[] = new FastpathArg[2];
            //args[0] = new FastpathArg(fd);
            //args[1] = new FastpathArg(len);
            //return fp.getData("loread",args);
            //} else {
            //// return in 4k blocks
            //byte[] buf=new byte[len];
            //int off=0;
            //while (len>0) {
            //int bs=4048;
            //len-=bs;
            //if (len<0) {
            //bs+=len;
            //len=0;
            //}
            //read(buf,off,bs);
            //off+=bs;
            //}
            //return buf;
            //}
        }

        /// <summary>
        /// Reads some data from the object into an existing array.
        /// </summary>
        /// <param name="buf">Destination array.</param>
        /// <param name="off">Offset within array.</param>
        /// <param name="len">Maximum number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public Int32 Read(Byte[] buf, Int32 off, Int32 len)
        {
            Byte[] b = Read(len);
            if (b.Length < len)
            {
                len = b.Length;
            }
            Array.Copy(b, 0, buf, off, len);
            return len;
        }

        /// <summary>
        /// Writes an array to the object.
        /// </summary>
        /// <param name="buf">Array to write.</param>
        public void Write(Byte[] buf)
        {
            FastpathArg[] args = new FastpathArg[2];
            args[0] = new FastpathArg(fd);
            args[1] = new FastpathArg(buf);
            fp.FastpathCall("lowrite", false, args);
        }

        /// <summary>
        /// Writes some data from an array to the object.
        /// </summary>
        /// <param name="buf">Destination array.</param>
        /// <param name="off">Offset within array.</param>
        /// <param name="len">Number of bytes to write.</param>
        public void Write(Byte[] buf, Int32 off, Int32 len)
        {
            Byte[] data = new Byte[len];

            Array.Copy(buf, off, data, 0, len);
            Write(data);
        }

        /// <summary>
        /// Sets the current position within the object.
        /// This is similar to the fseek() call in the standard C library. It
        /// allows you to have random access to the large object.
        /// </summary>
        /// <param name="pos">Position within object.</param>
        /// <param name="refi">Either SEEK_SET, SEEK_CUR or SEEK_END.</param>
        public void Seek(Int32 pos, Int32 refi)
        {
            FastpathArg[] args = new FastpathArg[3];
            args[0] = new FastpathArg(fd);
            args[1] = new FastpathArg(pos);
            args[2] = new FastpathArg(refi);
            fp.FastpathCall("lo_lseek", false, args);
        }

        /// <summary>
        /// Sets the current position within the object.
        /// This is similar to the fseek() call in the standard C library. It
        /// allows you to have random access to the large object.
        /// </summary>
        /// <param name="pos">Position within object from begining.</param>
        public void Seek(Int32 pos)
        {
            Seek(pos, SEEK_SET);
        }

        /// <summary>
        /// Report the current position within the object.
        /// </summary>
        /// <returns>The current position within the object.</returns>
        public Int32 Tell()
        {
            FastpathArg[] args = new FastpathArg[1];
            args[0] = new FastpathArg(fd);
            return fp.GetInteger("lo_tell", args);
        }

        /// <summary>
        /// This method is inefficient, as the only way to find out the size of
        /// the object is to seek to the end, record the current position, then
        /// return to the original position.
        /// A better method will be found in the future.
        /// </summary>
        /// <returns>The size of the large object.</returns>
        public Int32 Size()
        {
            Int32 cp = Tell();
            Seek(0, SEEK_END);
            Int32 sz = Tell();
            Seek(cp, SEEK_SET);
            return sz;
        }
    }
}
