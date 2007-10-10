// created on 4/3/2003 at 19:45

// Npgsql.NpgsqlBinaryRow.cs
//
// Author:
//	Francisco Jr. (fxjrlists@yahoo.com.br)
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
using System.Collections;
using System.IO;
using System.Text;
using System.Net;

using NpgsqlTypes;

namespace Npgsql
{

    /// <summary>
    /// This class represents the BinaryRow message sent from  the PostgreSQL
    /// server.  This is unused as of protocol version 3.
    /// </summary>
    internal sealed class NpgsqlBinaryRow : NpgsqlRow
    {
        // Logging related values
        private static readonly String CLASSNAME = "NpgsqlBinaryRow";

        public NpgsqlBinaryRow(NpgsqlRowDescription rowDesc)
                : base(rowDesc, ProtocolVersion.Version2)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);
        }

        public override void ReadFromStream(Stream inputStream, Encoding encoding)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "ReadFromStream");

            //Byte[] input_buffer = new Byte[READ_BUFFER_SIZE];
            Byte[]       input_buffer = null;
            Byte[]       null_map_array = new Byte[(row_desc.NumFields + 7)/8];

            null_map_array = new Byte[(row_desc.NumFields + 7)/8];
            Array.Clear(null_map_array, 0, null_map_array.Length);

            // Read the null fields bitmap.
            inputStream.Read(null_map_array, 0, null_map_array.Length );

            // Get the data.
            for (Int16 field_count = 0; field_count < row_desc.NumFields; field_count++)
            {

                // Check if this field is null
                if (IsBackendNull(null_map_array, field_count))
                {
                    data.Add(DBNull.Value);
                    continue;
                }

                // Read the first data of the first row.

                PGUtil.CheckedStreamRead(inputStream, input_buffer, 0, 4);

                Int32 field_value_size = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(input_buffer, 0));

                Int32 bytes_left = field_value_size; //Size of data is the value read.

                input_buffer = new Byte[bytes_left];

                // Now, read just the field value.
                PGUtil.CheckedStreamRead(inputStream, input_buffer, 0, bytes_left);

                // Add them to the BinaryRow data.
                data.Add(input_buffer);
            }
        }

        // Using the given null field map (provided by the backend),
        // determine if the given field index is mapped null by the backend.
        // We only need to do this for version 2 protocol.
        private static Boolean IsBackendNull(Byte[] null_map_array, Int32 index)
        {
            // Get the byte that holds the bit index position.
            Byte test_byte = null_map_array[index/8];

            // Now, check if index bit is set.
            // To do this, get its position in the byte, shift to
            // MSB and test it with the byte 10000000.
            return (((test_byte << (index%8)) & 0x80) == 0);
        }
    }

}
