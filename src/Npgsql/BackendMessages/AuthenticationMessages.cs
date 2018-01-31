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

        internal static AuthenticationMD5PasswordMessage Load(ReadBuffer buf)
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
    class AuthenticationSASLMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType => State;

        internal AuthenticationRequestType State { get; private set; }
        internal string ProposedScheme { get; private set; }
        internal string ServerFirst { get; private set; }
        internal string ServerProof { get; private set; }

        internal static AuthenticationSASLMessage Load(ReadBuffer buf, int len, AuthenticationRequestType state)
        {
            var payload = buf.ReadString(len);
            return new AuthenticationSASLMessage(state, payload);
        }

        AuthenticationSASLMessage(AuthenticationRequestType state, string value)
        {
            State = state;
            if (State == AuthenticationRequestType.AuthenticationSASLInit)
                // Resolve -2, strip last two spaces 
                ProposedScheme = value.Substring(0, value.Length - 2);
            else if (State == AuthenticationRequestType.AuthenticationSCRAMFirst)
                ServerFirst = value;
            else if (State == AuthenticationRequestType.AuthenticationSCRAMFinal)
                ServerProof = value;
            else
                throw new NpgsqlException("Invalid SASL State");
        }

        public enum STATE
        {
            INIT = 0,
            CONTINUE = 1,
            FINAL = 2,
            INVALID = -1
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

        internal static AuthenticationGSSContinueMessage Load(ReadBuffer buf, int len)
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
        AuthenticationSASLInit = 10,
        AuthenticationSCRAMFirst = 11,
        AuthenticationSCRAMFinal = 12
    }
}
