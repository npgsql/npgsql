#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using Npgsql.Logging;

namespace Npgsql.BackendMessages
{
    abstract class AuthenticationRequestMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.AuthenticationRequest;
        internal abstract AuthenticationRequestType AuthRequestType { get; }
    }

    class AuthenticationOkMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationOk;

        internal static readonly AuthenticationOkMessage Instance = new AuthenticationOkMessage();
        AuthenticationOkMessage() { }
    }

    class AuthenticationKerberosV5Message : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationKerberosV5;

        internal static readonly AuthenticationKerberosV5Message Instance = new AuthenticationKerberosV5Message();
        AuthenticationKerberosV5Message() { }
    }

    class AuthenticationCleartextPasswordMessage  : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationCleartextPassword;

        internal static readonly AuthenticationCleartextPasswordMessage Instance = new AuthenticationCleartextPasswordMessage();
        AuthenticationCleartextPasswordMessage() { }
    }

    class AuthenticationMD5PasswordMessage  : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationMD5Password;

        internal byte[] Salt { get; private set; }

        internal static AuthenticationMD5PasswordMessage Load(NpgsqlReadBuffer buf)
        {
            var salt = new byte[4];
            buf.ReadBytes(salt, 0, 4);
            return new AuthenticationMD5PasswordMessage(salt);
        }

        AuthenticationMD5PasswordMessage(byte[] salt)
        {
            Salt = salt;
        }
    }

    class AuthenticationSCMCredentialMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationSCMCredential;

        internal static readonly AuthenticationSCMCredentialMessage Instance = new AuthenticationSCMCredentialMessage();
        AuthenticationSCMCredentialMessage() { }
    }

    class AuthenticationGSSMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationGSS;

        internal static readonly AuthenticationGSSMessage Instance = new AuthenticationGSSMessage();
        AuthenticationGSSMessage() { }
    }

    class AuthenticationGSSContinueMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationGSSContinue;

        internal byte[] AuthenticationData { get; private set; }

        internal static AuthenticationGSSContinueMessage Load(NpgsqlReadBuffer buf, int len)
        {
            len -= 4;   // The AuthRequestType code
            var authenticationData = new byte[len];
            buf.ReadBytes(authenticationData, 0, len);
            return new AuthenticationGSSContinueMessage(authenticationData);
        }

        AuthenticationGSSContinueMessage(byte[] authenticationData)
        {
            AuthenticationData = authenticationData;
        }
    }

    class AuthenticationSSPIMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationSSPI;

        internal static readonly AuthenticationSSPIMessage Instance = new AuthenticationSSPIMessage();
        AuthenticationSSPIMessage() { }
    }

    #region SASL

    class AuthenticationSASLMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationSASL;
        internal List<string> Mechanisms { get; } = new List<string>();

        internal AuthenticationSASLMessage(NpgsqlReadBuffer buf)
        {
            while (buf.Buffer[buf.ReadPosition] != 0)
                Mechanisms.Add(buf.ReadNullTerminatedString());
            buf.ReadByte();
            if (Mechanisms.Count == 0)
                throw new NpgsqlException("Received AuthenticationSASL message with 0 mechanisms!");
        }
    }

    class AuthenticationSASLContinueMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationSASLContinue;
        internal byte[] Payload { get; }

        internal AuthenticationSASLContinueMessage(NpgsqlReadBuffer buf, int len)
        {
            Payload = new byte[len];
            buf.ReadBytes(Payload, 0, len);
        }
    }

    class AuthenticationSCRAMServerFirstMessage
    {
        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal string Nonce { get; }
        internal string Salt { get; }
        internal int Iteration { get; } = -1;

        internal AuthenticationSCRAMServerFirstMessage(byte[] bytes)
        {
            var data = PGUtil.UTF8Encoding.GetString(bytes);

            foreach (var part in data.Split(','))
            {
                if (part.StartsWith("r="))
                    Nonce = part.Substring(2);
                else if (part.StartsWith("s="))
                    Salt = part.Substring(2);
                else if (part.StartsWith("i="))
                    Iteration = int.Parse(part.Substring(2));
                else
                    Log.Debug("Unknown part in SCRAM server-first message:" + part);
            }

            if (Nonce == null)
                throw new NpgsqlException("Server nonce not received in SCRAM server-first message");
            if (Salt == null)
                throw new NpgsqlException("Server salt not received in SCRAM server-first message");
            if (Iteration == -1)
                throw new NpgsqlException("Server iterations not received in SCRAM server-first message");
        }
    }

    class AuthenticationSASLFinalMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => AuthenticationRequestType.AuthenticationSASLFinal;
        internal byte[] Payload { get; }

        internal AuthenticationSASLFinalMessage(NpgsqlReadBuffer buf, int len)
        {
            Payload = new byte[len];
            buf.ReadBytes(Payload, 0, len);
        }
    }

    class AuthenticationSCRAMServerFinalMessage
    {
        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal string ServerSignature { get; }

        internal AuthenticationSCRAMServerFinalMessage(byte[] bytes)
        {
            var data = PGUtil.UTF8Encoding.GetString(bytes);

            foreach (var part in data.Split(','))
            {
                if (part.StartsWith("v="))
                    ServerSignature = part.Substring(2);
                else
                    Log.Debug("Unknown part in SCRAM server-first message:" + part);
            }

            if (ServerSignature == null)
                throw new NpgsqlException("Server signature not received in SCRAM server-final message");
        }
    }

    #endregion SASL

    // TODO: Remove Authentication prefix from everything
    enum AuthenticationRequestType
    {
        AuthenticationOk = 0,
        AuthenticationKerberosV4 = 1,
        AuthenticationKerberosV5 = 2,
        AuthenticationCleartextPassword = 3,
        AuthenticationCryptPassword = 4,
        AuthenticationMD5Password = 5,
        AuthenticationSCMCredential = 6,
        AuthenticationGSS = 7,
        AuthenticationGSSContinue = 8,
        AuthenticationSSPI = 9,
        AuthenticationSASL = 10,
        AuthenticationSASLContinue = 11,
        AuthenticationSASLFinal = 12
    }
}
