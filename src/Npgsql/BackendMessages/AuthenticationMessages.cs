﻿using System.Collections.Generic;
using Npgsql.Logging;
using Npgsql.Util;

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
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(AuthenticationSCRAMServerFirstMessage));

        internal string Nonce { get; }
        internal string Salt { get; }
        internal int Iteration { get; }

        internal static AuthenticationSCRAMServerFirstMessage Load(byte[] bytes)
        {
            var data = PGUtil.UTF8Encoding.GetString(bytes);
            string? nonce = null, salt = null;
            var iteration = -1;

            foreach (var part in data.Split(','))
            {
                if (part.StartsWith("r="))
                    nonce = part.Substring(2);
                else if (part.StartsWith("s="))
                    salt = part.Substring(2);
                else if (part.StartsWith("i="))
                    iteration = int.Parse(part.Substring(2));
                else
                    Log.Debug("Unknown part in SCRAM server-first message:" + part);
            }

            if (nonce == null)
                throw new NpgsqlException("Server nonce not received in SCRAM server-first message");
            if (salt == null)
                throw new NpgsqlException("Server salt not received in SCRAM server-first message");
            if (iteration == -1)
                throw new NpgsqlException("Server iterations not received in SCRAM server-first message");

            return new AuthenticationSCRAMServerFirstMessage(nonce, salt, iteration);
        }

        AuthenticationSCRAMServerFirstMessage(string nonce, string salt, int iteration)
        {
            Nonce = nonce;
            Salt = salt;
            Iteration = iteration;
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
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(AuthenticationSCRAMServerFinalMessage));

        internal string ServerSignature { get; }

        internal static AuthenticationSCRAMServerFinalMessage Load(byte[] bytes)
        {
            var data = PGUtil.UTF8Encoding.GetString(bytes);
            string? serverSignature = null;

            foreach (var part in data.Split(','))
            {
                if (part.StartsWith("v="))
                    serverSignature = part.Substring(2);
                else
                    Log.Debug("Unknown part in SCRAM server-first message:" + part);
            }

            if (serverSignature == null)
                throw new NpgsqlException("Server signature not received in SCRAM server-final message");

            return new AuthenticationSCRAMServerFinalMessage(serverSignature);
        }

        internal AuthenticationSCRAMServerFinalMessage(string serverSignature)
            => ServerSignature = serverSignature;
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
