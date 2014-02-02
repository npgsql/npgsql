/*-------------------------------------------------------------------------

  Fastpath.cs
      This class is a port of the class Fastpath.java implemented by
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using Npgsql;

namespace NpgsqlTypes
{
    /// <summary>
    /// This class implements the Fastpath api.
    /// </summary>
    public class Fastpath
    {
        /// <summary>
        /// This maps the functions names to their id's (possible unique just
        /// to a connection).
        /// </summary>
        protected Dictionary<string, int> func = new Dictionary<string, int>();

        /// <summary>
        /// Our connection.
        /// </summary>
        protected NpgsqlConnection conn;

        /// <summary>
        /// The network stream.
        /// </summary>
        protected Stream stream;

        /// <summary>
        /// Initialises the fastpath system.
        /// </summary>
        /// <param name="conn">BaseConnection to attach to.</param>
        /// <param name="stream">The network stream to the backend.</param>
        public Fastpath(NpgsqlConnection conn, Stream stream)
        {
            this.conn = conn;
            this.stream = stream;
        }

        /// <summary>
        /// Initialises the fastpath system.
        /// </summary>
        /// <param name="conn">BaseConnection to attach to.</param>
        public Fastpath(NpgsqlConnection conn)
        {
            this.conn = conn;
            // check if the connection is closed ?
            this.stream = conn.Connector.Stream;
        }

        /// <summary>
        /// Send a function call to the PostgreSQL backend.
        /// </summary>
        /// <param name="fnid">Function id.</param>
        /// <param name="resulttype">True if the result is an integer, false for other results.</param>
        /// <param name="args">FastpathArguments to pass to fastpath.</param>
        /// <returns>null if no data, Integer if an integer result, or byte[] otherwise.</returns>
        public Object FastpathCall(Int32 fnid, Boolean resulttype, FastpathArg[] args)
        {
            try
            {
                return FastpathV3(fnid, resulttype, args);
            }
            catch (IOException)
            {
                conn.ClearPool();
                throw new NpgsqlException("The Connection is broken.");
            }
        }

        private Object FastpathV3(Int32 fnid, Boolean resulttype, FastpathArg[] args)
        {
            // give  thread safety
            lock (stream)
            {
                // send the function call

                {
                    Int32 l_msgLen = 0;
                    l_msgLen += 16;
                    for (Int32 i = 0; i < args.Length; i++)
                    {
                        l_msgLen += args[i].SendSize();
                    }

                    stream
                        .WriteBytes((Byte)ASCIIBytes.F)
                        .WriteInt32(l_msgLen)
                        .WriteInt32(fnid)
                        .WriteInt16(1)
                        .WriteInt16(1)
                        .WriteInt16((short)args.Length);

                    for (Int32 i = 0; i < args.Length; i++)
                    {
                        args[i].Send(stream);
                    }

                    stream.WriteInt16(1);

                    // This is needed, otherwise data can be lost
                    stream.Flush();
                }

                // Now handle the result

                // Now loop, reading the results
                Object result = null; // our result
                Exception error = null;
                Int32 c;
                Boolean l_endQuery = false;

                while (!l_endQuery)
                {
                    c = (Char) stream.ReadByte();

                    switch (c)
                    {
                        case 'A': // Asynchronous Notify
                            Int32 msglen = PGUtil.ReadInt32(stream);
                            Int32 pid = PGUtil.ReadInt32(stream);
                            String msg = PGUtil.ReadString(stream);
                            PGUtil.ReadString(stream);
                            String param = PGUtil.ReadString(stream);

                            break;
                            //------------------------------
                            // Error message returned
                        case 'E':
                            NpgsqlError e = new NpgsqlError(stream);
                            throw new NpgsqlException(e.ToString());

                            //------------------------------
                            // Notice from backend
                        case 'N':
                            Int32 l_nlen = PGUtil.ReadInt32(stream);

                            conn.Connector.FireNotice(new NpgsqlError(stream));

                            break;

                        case 'V':
                            Int32 l_msgLen = PGUtil.ReadInt32(stream);
                            Int32 l_valueLen = PGUtil.ReadInt32(stream);

                            if (l_valueLen == -1)
                            {
                                //null value
                            }
                            else if (l_valueLen == 0)
                            {
                                result = new Byte[0];
                            }
                            else
                            {
                                // Return an Integer if
                                if (resulttype)
                                {
                                    result = PGUtil.ReadInt32(stream);
                                }
                                else
                                {
                                    Byte[] buf = new Byte[l_valueLen];

                                    Int32 bytes_from_stream = 0;
                                    Int32 total_bytes_read = 0;
                                    Int32 size = l_valueLen;
                                    do
                                    {
                                        bytes_from_stream = stream.Read(buf, total_bytes_read, size);
                                        total_bytes_read += bytes_from_stream;
                                        size -= bytes_from_stream;
                                    }
                                    while (size > 0);

                                    result = buf;
                                }
                            }
                            break;

                        case 'Z':
                            //TODO: use size better
                            if (PGUtil.ReadInt32(stream) != 5)
                            {
                                throw new NpgsqlException("Received Z");
                            }
                            //TODO: handle transaction status
                            Char l_tStatus = (Char) stream.ReadByte();
                            l_endQuery = true;
                            break;

                        default:
                            throw new NpgsqlException(string.Format("postgresql.fp.protocol received {0}", c));
                    }
                }

                if (error != null)
                {
                    throw error;
                }

                return result;
            }
        }

        /// <summary>
        /// Send a function call to the PostgreSQL backend by name.
        /// Note: the mapping for the procedure name to function id needs to exist,
        /// usually to an earlier call to addfunction().
        /// This is the prefered method to call, as function id's can/may change
        /// between versions of the backend.
        /// For an example of how this works, refer to NpgsqlTypes.LargeObject
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="resulttype">True if the result is an integer, false for other results.</param>
        /// <param name="args">FastpathArguments to pass to fastpath.</param>
        /// <returns>null if no data, Integer if an integer result, or byte[] otherwise.</returns>
        public Object FastpathCall(String name, Boolean resulttype, FastpathArg[] args)
        {
            return FastpathCall(GetID(name), resulttype, args);
        }

        /// <summary>
        /// This convenience method assumes that the return value is an Integer.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="args">Function arguments.</param>
        /// <returns>Integer result.</returns>
        public Int32 GetInteger(String name, FastpathArg[] args)
        {
            Int32 i = (Int32) FastpathCall(name, true, args);

            return i;
        }

        /// <summary>
        /// This convenience method assumes that the return value is an Integer.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="args">Function arguments.</param>
        /// <returns>Array containing result</returns>
        public Byte[] GetData(String name, FastpathArg[] args)
        {
            return (Byte[]) FastpathCall(name, false, args);
        }

        /// <summary>
        /// This adds a function to our lookup table.
        /// User code should use the addFunctions method, which is based upon a
        /// query, rather than hard coding the oid. The oid for a function is not
        /// guaranteed to remain static, even on different servers of the same
        /// version.
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <param name="fnid">Function id.</param>
        public void AddFunction(String name, Int32 fnid)
        {
            func.Add(name, fnid);
        }

        /// <summary>
        /// This takes a ResultSet containing two columns. Column 1 contains the
        /// function name, Column 2 the oid.
        /// It reads the entire ResultSet, loading the values into the function
        /// table.
        /// REMEMBER to close() the resultset after calling this!!
        /// Implementation note about function name lookups:
        /// PostgreSQL stores the function id's and their corresponding names in
        /// the pg_proc table. To speed things up locally, instead of querying each
        /// function from that table when required, a Dictionary is used. Also, only
        /// the function's required are entered into this table, keeping connection
        /// times as fast as possible.
        /// The org.postgresql.largeobject.LargeObject class performs a query upon it's startup,
        /// and passes the returned ResultSet to the addFunctions() method here.
        /// Once this has been done, the LargeObject api refers to the functions by
        /// name.
        /// Dont think that manually converting them to the oid's will work. Ok,
        /// they will for now, but they can change during development (there was some
        /// discussion about this for V7.0), so this is implemented to prevent any
        /// unwarranted headaches in the future.
        /// </summary>
        /// <param name="rs">ResultSet</param>
        public void AddFunctions(IDataReader rs)
        {
            while (rs.Read())
            {
                String key = (String) rs[0];
                if (!func.ContainsKey(key))
                {
                    func.Add(key, Int32.Parse(rs[1].ToString()));
                }
            }
        }

        /// <summary>
        /// This returns the function id associated by its name
        /// If addFunction() or addFunctions() have not been called for this name,
        /// then an NpgsqlException is thrown.
        /// </summary>
        /// <param name="name">Function name to lookup.</param>
        /// <returns>Function ID for fastpath call.</returns>
        public Int32 GetID(String name)
        {
            return func[name];
        }
    }
}
