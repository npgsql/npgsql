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
    internal interface IServerMessage {}

    internal class ReadyForQueryMsg : IServerMessage
    {
        internal static readonly ReadyForQueryMsg Instance = new ReadyForQueryMsg();
    }

    internal class CopyInResponseMsg : IServerMessage
    {
        internal static readonly CopyInResponseMsg Instance = new CopyInResponseMsg();
    }

    internal class CopyOutResponseMsg : IServerMessage
    {
        internal static readonly CopyOutResponseMsg Instance = new CopyOutResponseMsg();
    }
    internal class CopyDataMsg : IServerMessage
    {
        internal static readonly CopyDataMsg Instance = new CopyDataMsg();
    }

    /// <summary>
    /// Represents a completed response message.
    /// </summary>
    internal class CompletedResponse : IServerMessage
    {
        private readonly int? _rowsAffected;
        private readonly long? _lastInsertedOID;

        public CompletedResponse(Stream stream)
        {
            string[] tokens = stream.ReadString().Split();
            if (tokens.Length > 1)
            {
                int rowsAffected;
                if (int.TryParse(tokens[tokens.Length - 1], out rowsAffected))
                    _rowsAffected = rowsAffected;
                else
                    _rowsAffected = null;

            }
            _lastInsertedOID = (tokens.Length > 2 && tokens[0].Trim().ToUpperInvariant() == "INSERT")
                                   ? long.Parse(tokens[1])
                                   : (long?)null;
        }

        public long? LastInsertedOID
        {
            get { return _lastInsertedOID; }
        }

        public int? RowsAffected
        {
            get { return _rowsAffected; }
        }
    }

    internal class ParameterDescriptionResponse : IServerMessage
    {
        private readonly int[] _typeoids;

        public ParameterDescriptionResponse(int[] typeoids)
        {
            _typeoids = typeoids;
        }

        public int[] TypeOIDs
        {
            get { return _typeoids; }
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

    /// <summary>
    /// Marker interface which identifies a class which may take possession of a stream for the duration of
    /// it's lifetime (possibly temporarily giving that possession to another class for part of that time.
    ///
    /// It inherits from IDisposable, since any such class must make sure it leaves the stream in a valid state.
    ///
    /// The most important such class is that compiler-generated from ProcessBackendResponsesEnum. Of course
    /// we can't make that inherit from this interface, alas.
    /// </summary>
    internal interface IStreamOwner : IServerMessage, IDisposable
    {
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

    internal enum AuthenticationRequestType
    {
        AuthenticationOk = 0,
        AuthenticationKerberosV4 = 1,
        AuthenticationKerberosV5 = 2,
        AuthenticationClearTextPassword = 3,
        AuthenticationCryptPassword = 4,
        AuthenticationMD5Password = 5,
        AuthenticationSCMCredential = 6,
        AuthenticationGSS = 7,
        AuthenticationGSSContinue = 8,
        AuthenticationSSPI = 9
    }
    
    #pragma warning restore 1591
}
