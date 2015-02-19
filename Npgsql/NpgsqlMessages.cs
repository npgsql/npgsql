// Npgsql.NpgsqlMessageTypes.cs
//
// Author:
//     Dave Joyner <d4ljoyn@yahoo.com>
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
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

// Keep the xml comment warning quiet for this file.

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Base class for all classes which represent a message sent by the PostgreSQL backend.
    /// </summary>
    internal abstract class BackendMessage
    {
        internal abstract BackendMessageCode Code { get; }
    }

    /// <summary>
    /// Base class for all classes which represent a message sent to the PostgreSQL backend.
    /// </summary>
    internal abstract class FrontendMessage
    {
        /// <summary>
        /// Called to prepare a message before writing to the buffer. Maybe through validation exceptions.
        /// </summary>
        internal virtual void Prepare() { }
    }

    /// <summary>
    /// Represents a simple frontend message which is typically small and fits well within
    /// the write buffer. The message is first queries for the number of bytes it requires,
    /// and then writes itself out.
    /// </summary>
    internal abstract class SimpleFrontendMessage : FrontendMessage
    {
        /// <summary>
        /// Returns the number of bytes needed to write this message. Can only be called after
        /// <see cref="FrontendMessage.Prepare"/> has been called.
        /// </summary>
        internal abstract int Length { get; }

        /// <summary>
        /// Writes the message contents into the buffer. 
        /// </summary>
        internal abstract void Write(NpgsqlBuffer buf);
    }

    /// <summary>
    /// Represents an arbitrary-length message capable of flushing the buffer internally as it's
    /// writing itself out.
    /// </summary>
    internal abstract class ChunkingFrontendMessage : FrontendMessage
    {
        /// <param name="buf">the buffer into which to write the message.</param>
        /// <param name="directBuf">
        /// an option buffer that, if returned, will be written to the server directly, bypassing our
        /// NpgsqlBuffer. This is an optimization hack for bytea.
        /// </param>
        /// <returns>
        /// Whether there was enough space in the buffer to contain the entire message.
        /// If false, the buffer should be flushed and write should be called again.
        /// </returns>
        internal virtual bool Write(NpgsqlBuffer buf)
        {
            throw new NotImplementedException("Write()");
        }

        internal virtual bool Write(NpgsqlBuffer buf, ref DirectBuffer directBuf)
        {
            return Write(buf);
        }
    }

    /// <summary>
    /// For classes representing messages sent from the client to the server.
    /// </summary>
    internal interface IClientMessage
    {
        void WriteToStream(Stream outputStream);
        Task WriteToStreamAsync(Stream outputStream);
    }

    internal enum BackendMessageCode
    {
        IO_ERROR = -1, // Connection broken. Mono returns -1 instead of throwing an exception as ms.net does.

        CopyData = 'd',
        CopyDone = 'c',
        DataRow = 'D',

        BackendKeyData = 'K',
        CancelRequest = 'F',
        CompletedResponse = 'C',
        CopyDataRows = ' ',
        CopyInResponse = 'G',
        CopyOutResponse = 'H',
        EmptyQueryResponse = 'I',
        ErrorResponse = 'E',
        FunctionCall = 'F',
        FunctionCallResponse = 'V',

        AuthenticationRequest = 'R',

        NoticeResponse = 'N',
        NotificationResponse = 'A',
        ParameterStatus = 'S',
        PasswordPacket = ' ',
        ReadyForQuery = 'Z',
        RowDescription = 'T',
        SSLRequest = ' ',

        // extended query backend messages
        ParseComplete = '1',
        BindComplete = '2',
        PortalSuspended = 's',
        ParameterDescription = 't',
        NoData = 'n',
        CloseComplete = '3'
    }

    enum StatementOrPortal : byte
    {
        Statement = (byte)'S',
        Portal = (byte)'P'
    }
}
