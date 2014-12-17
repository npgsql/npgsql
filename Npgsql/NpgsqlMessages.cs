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
using System.IO;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Marker interface which identifies a class which represents part of
    /// a response from the server.
    /// </summary>
    internal abstract class ServerMessage
    {
        internal abstract BackEndMessageCode Code { get; }
    }

    /// <summary>
    /// For classes representing messages sent from the client to the server.
    /// </summary>
    internal interface IClientMessage
    {
        void WriteToStream(Stream outputStream);
        Task WriteToStreamAsync(Stream outputStream);
    }

    #pragma warning disable 1591

    internal enum FrontEndMessageCode : byte
    {
        StartupPacket = (byte) ' ',
        Termination = (byte) 'X',
        CopyFail = (byte) 'f',
        CopyData = (byte) 'd',
        CopyDone = (byte) 'c',
        Flush = (byte) 'H',
        Query = (byte) 'Q',
        Parse = (byte) 'P',
        Bind = (byte) 'B',
        Execute = (byte) 'E',
        Describe = (byte) 'D',
        Close = (byte) 'C',
        Sync = (byte) 'S',
        PasswordMessage = (byte) 'p',
        FunctionCall = (byte)'F',
    }

    internal enum BackEndMessageCode
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

    #pragma warning restore 1591
}
