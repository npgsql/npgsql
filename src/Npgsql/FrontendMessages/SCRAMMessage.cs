#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.Tls;

namespace  Npgsql.FrontendMessages
{
    class SCRAMMessage : SimpleFrontendMessage
    {
        internal byte[] Payload { get; private set; }
        internal int PayloadOffset { get; private set; }
        internal int PayloadLength { get; private set; }

        const byte Code = (byte)'p';

        internal static SCRAMMessage CreateClientFirstMessage(SCRAM scram)
        {
            var schemeBytes = PGUtil.UTF8Encoding.GetBytes(scram.Scheme);

            var clientFirstMessage = scram.GetClientFirstMessage();

            var clientFirstMessageBytes = PGUtil.UTF8Encoding.GetBytes(clientFirstMessage);

            var firstMessageLength = schemeBytes.Length + 1 + 4 + clientFirstMessageBytes.Length;
            
            var encoded = new byte[firstMessageLength+1];
            
            // scheme name
            schemeBytes.CopyTo(encoded, 0);
            // null
            encoded[schemeBytes.Length] = 0;
            // Length
            var lengthBytes = BitConverter.GetBytes(clientFirstMessageBytes.Length);
            var lengthReverseBytes = lengthBytes.Reverse().ToArray();
            lengthReverseBytes.CopyTo(encoded, schemeBytes.Length +1);
            // message
            clientFirstMessageBytes.CopyTo(encoded, schemeBytes.Length + 1 + 4 );
            encoded[encoded.Length - 1] = 0;
            return new SCRAMMessage(encoded);
        }


        internal static SCRAMMessage CreateClientFinalMessage(SCRAM scram)
        {
            var finalMessage = scram.CreateClientFinalMessage();
            var encoded = new byte[finalMessage.Length + 1];
            var finalMessageBytes = PGUtil.UTF8Encoding.GetBytes(finalMessage);
            finalMessageBytes.CopyTo(encoded, 0);
            return new SCRAMMessage(encoded);
        }

        internal SCRAMMessage() {}

        SCRAMMessage(byte[] payload)
        {
            Payload = payload;
            PayloadOffset = 0;
            PayloadLength = payload.Length;
        }

        internal SCRAMMessage Populate(byte[] payload, int offset, int count)
        {
            Payload = payload;
            PayloadOffset = offset;
            PayloadLength = count;
            return this;
        }

        internal override int Length => 1 + 4 + PayloadLength;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length-2);
            buf.WriteBytes(Payload, PayloadOffset, Payload.Length-1);
        }

        public override string ToString() => "[SCRAMMessage]";
    }
}
