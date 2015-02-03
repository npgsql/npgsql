using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal abstract class AuthenticationRequestMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.AuthenticationRequest; } }
        internal abstract AuthenticationRequestType AuthRequestType { get; }
    }

    internal class AuthenticationOkMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType {
            get { return AuthenticationRequestType.AuthenticationOk; }
        }

        internal static readonly AuthenticationOkMessage Instance = new AuthenticationOkMessage();
        AuthenticationOkMessage() { }
    }

    internal class AuthenticationKerberosV5Message : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationKerberosV5; }
        }

        internal static readonly AuthenticationKerberosV5Message Instance = new AuthenticationKerberosV5Message();
        AuthenticationKerberosV5Message() { }
    }

    internal class AuthenticationCleartextPasswordMessage  : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationCleartextPassword; }
        }

        internal static readonly AuthenticationCleartextPasswordMessage Instance = new AuthenticationCleartextPasswordMessage();
        AuthenticationCleartextPasswordMessage() { }
    }

    internal class AuthenticationMD5PasswordMessage  : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationMD5Password; }
        }

        internal byte[] Salt { get; private set; }

        internal static AuthenticationMD5PasswordMessage Load(NpgsqlBuffer buf)
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

    internal class AuthenticationSCMCredentialMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationSCMCredential; }
        }

        internal static readonly AuthenticationSCMCredentialMessage Instance = new AuthenticationSCMCredentialMessage();
        AuthenticationSCMCredentialMessage() { }
    }

    internal class AuthenticationGSSMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationGSS; }
        }

        internal static readonly AuthenticationGSSMessage Instance = new AuthenticationGSSMessage();
        AuthenticationGSSMessage() { }
    }

    internal class AuthenticationGSSContinueMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationGSSContinue; }
        }

        internal byte[] AuthenticationData { get; private set; }

        internal static AuthenticationGSSContinueMessage Load(NpgsqlBuffer buf, int len)
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

    internal class AuthenticationSSPIMessage : AuthenticationRequestMessage
    {
        internal override AuthenticationRequestType AuthRequestType
        {
            get { return AuthenticationRequestType.AuthenticationSSPI; }
        }

        internal static readonly AuthenticationSSPIMessage Instance = new AuthenticationSSPIMessage();
        AuthenticationSSPIMessage() { }
    }

    internal enum AuthenticationRequestType
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
        AuthenticationSSPI = 9
    }
}
