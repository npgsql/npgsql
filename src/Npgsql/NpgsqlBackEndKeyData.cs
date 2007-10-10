// created on 11/6/2002 at 11:53

// Npgsql.NpgsqlBackEndKeyData.cs
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
using System.IO;
using System.Text;
using System.Net;

namespace Npgsql
{
    /// <summary>
    /// This class represents a BackEndKeyData message received
    /// from PostgreSQL
    /// </summary>
    internal sealed class NpgsqlBackEndKeyData
    {
        // Logging related values
        private static readonly String CLASSNAME = "NpgsqlBackEndKeyData";

        private Int32 _processId;
        private Int32 _secretKey;

        private ProtocolVersion _protocolVersion;

        public NpgsqlBackEndKeyData(ProtocolVersion protocolVersion)
        {
            _protocolVersion = protocolVersion;
            _processId = -1;
            _secretKey = -1;
        }


        public void ReadFromStream(Stream inputStream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);

            Byte[] inputBuffer = new Byte[8];

            // Read the BackendKeyData message contents. Two Int32 integers = 8 Bytes.
            // For protocol version 3.0 they are three integers. The first one is just the size of message
            // so, just read it.
            if (_protocolVersion >= ProtocolVersion.Version3)
                inputStream.Read(inputBuffer, 0, 4);

            inputStream.Read(inputBuffer, 0, 8);
            _processId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inputBuffer, 0));
            _secretKey = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inputBuffer, 4));

        }

        public Int32 ProcessID
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "ProcessID");
                return _processId;
            }
        }

        public Int32 SecretKey
        {
            get
            {
                NpgsqlEventLog.LogPropertyGet(LogLevel.Debug, CLASSNAME, "SecretKey");
                return _secretKey;
            }
        }
    }
}
