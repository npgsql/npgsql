// Npgsql.NpgsqlMessageTypes.cs
//
// Author:
// 	Dave Joyner <d4ljoyn@yahoo.com>
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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


using System;

namespace Npgsql
{
    /// <summary>
    /// Class NpgsqlMessageTypes_Ver_2.
    /// Defines PG frontend/backend protocol message types and parameters used in protocol version 2.
    /// </summary>
    internal sealed class NpgsqlMessageTypes_Ver_2
    {
        private NpgsqlMessageTypes_Ver_2()
        {}

        public const Char StartupPacket = ' ';
        public const Char Terminate = 'X';

        public const Char AsciiRow = 'D';
        public const Char BinaryRow = 'B';

        public const Char AuthenticationRequest = 'R';
        // specific Authentication request types
        public const Int32 AuthenticationOk = 0;
        public const Int32 AuthenticationKerberosV4 = 1;
        public const Int32 AuthenticationKerberosV5 = 2;
        public const Int32 AuthenticationClearTextPassword = 3;
        public const Int32 AuthenticationCryptPassword = 4;
        public const Int32 AuthenticationMD5Password = 5;
        public const Int32 AuthenticationSCMCredential = 6;

        public const Char BackendKeyData = 'K';
        public const Char CancelRequest = 'F';
        public const Char CompletedResponse = 'C';
        public const Char CopyDataRows = ' ';
        public const Char CopyInResponse = 'G';
        public const Char CopyOutResponse = 'H';
        public const Char CursorResponse = 'P';
        public const Char EmptyQueryResponse = 'I';
        public const Char ErrorResponse = 'E';
        public const Char FunctionCall = 'F';

        public const Char FunctionResultResponse = 'V';
        // specific function result responses
        public const Char FunctionResultNonEmptyResponse = 'G';
        public const Char FunctionResultVoidResponse = '0';

        public const Char NoticeResponse = 'N';
        public const Char NotificationResponse = 'A';
        public const Char PasswordPacket = ' ';
        public const Char Query = 'Q';
        public const Char ReadyForQuery = 'Z';
        public const Char RowDescription = 'T';
        public const Char SSLRequest = ' ';
    }


    /// <summary>
    /// Class NpgsqlMessageTypes_Ver_3.
    /// Defines PG frontend/backend protocol message types and parameters used in protocol version 3.
    /// </summary>
    internal sealed class NpgsqlMessageTypes_Ver_3
    {
        private NpgsqlMessageTypes_Ver_3()
        {}

        public const Char StartupPacket = ' ';
        public const Char Termination = 'X';

        public const Char DataRow = 'D';

        public const Char AuthenticationRequest = 'R';
        // specific Authentication request types
        public const Int32 AuthenticationOk = 0;
        public const Int32 AuthenticationKerberosV4 = 1;
        public const Int32 AuthenticationKerberosV5 = 2;
        public const Int32 AuthenticationClearTextPassword = 3;
        public const Int32 AuthenticationCryptPassword = 4;
        public const Int32 AuthenticationMD5Password = 5;
        public const Int32 AuthenticationSCMCredential = 6;

        public const Char BackendKeyData = 'K';
        public const Char CancelRequest = 'F';
        public const Char CompletedResponse = 'C';
        public const Char CopyDataRows = ' ';
        public const Char CopyData = 'd';
        public const Char CopyDone = 'c';
        public const Char CopyFail = 'f';
        public const Char CopyInResponse = 'G';
        public const Char CopyOutResponse = 'H';
        public const Char EmptyQueryResponse = 'I';
        public const Char ErrorResponse = 'E';
        public const Char FunctionCall = 'F';
        public const Char FunctionCallResponse = 'V';

        public const Char NoticeResponse = 'N';
        public const Char NotificationResponse = 'A';
        public const Char ParameterStatus = 'S';
        public const Char PasswordPacket = ' ';
        public const Char Query = 'Q';
        public const Char ReadyForQuery = 'Z';
        public const Char RowDescription = 'T';
        public const Char SSLRequest = ' ';

        // extended query frontend messages
        public const Char Parse = 'P';
        public const Char Bind = 'B';
        public const Char Execute = 'E';
        public const Char Describe = 'D';
        public const Char Close = 'C';
        public const Char Flush = 'H';
        public const Char Sync = 'S';

        // extended query backend messages
        public const Char ParseComplete = '1';
        public const Char BindComplete = '2';
        public const Char PortalSuspended = 's';
        public const Char ParameterDescription = 't';
        public const Char NoData = 'n';
        public const Char CloseComplete = '3';

    }
}
