/*-------------------------------------------------------------------------

  FastpathArg.cs
      This class is a port of the class FastpathArg.java implemented by
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
using System.IO;
using Npgsql;

namespace NpgsqlTypes
{
    /// <summary>
    /// Fast Path Arg.
    /// </summary>
    public class FastpathArg
    {
        /// <summary>
        /// Type of argument, true=integer, false=byte[].
        /// </summary>
        public Boolean type;

        /// <summary>
        /// Integer value if type=true.
        /// </summary>
        public Int32 value;

        /// <summary>
        /// Byte value if type=false;
        /// </summary>
        public Byte[] bytes;

        /// <summary>
        /// Constructs an argument that consists of an integer value.
        /// </summary>
        /// <param name="value">Int value to set.</param>
        public FastpathArg(Int32 value)
        {
            type = true;
            this.value = value;
        }

        /// <summary>
        /// Constructs an argument that consists of an array of bytes.
        /// </summary>
        /// <param name="bytes">Array to store.</param>
        public FastpathArg(Byte[] bytes)
        {
            type = false;
            this.bytes = bytes;
        }

        /// <summary>
        /// Constructs an argument that consists of part of a byte array.
        /// </summary>
        /// <param name="buf">Source array.</param>
        /// <param name="off">offset within array.</param>
        /// <param name="len">length of data to include.</param>
        public FastpathArg(Byte[] buf, Int32 off, Int32 len)
        {
            type = false;
            bytes = new Byte[len];
            //TODO:
            bytes = buf;
        }

        /// <summary>
        /// Constructs an argument that consists of a String.
        /// </summary>
        /// <param name="s">String to store.</param>
        public FastpathArg(String s)
        {
            //this(s.ToCharArray());
        }

        /// <summary>
        /// This sends this argument down the network stream.
        /// The stream sent consists of the length.int4 then the contents.
        /// Note: This is called from Fastpath, and cannot be called from
        /// client code.
        /// </summary>
        /// <param name="s"></param>
        internal void Send(Stream s)
        {
            if (type)
            {
                // argument is an integer
                PGUtil.WriteInt32(s, 4);
                PGUtil.WriteInt32(s, value); // integer value of argument
            }
            else
            {
                // argument is a byte array
                PGUtil.WriteInt32(s, bytes.Length);
                s.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Report send size.
        /// </summary>
        /// <returns>Send size.</returns>
        public Int32 SendSize()
        {
            if (type)
            {
                return 8;
            }
            else
            {
                return 4 + bytes.Length;
            }
        }
    }
}
