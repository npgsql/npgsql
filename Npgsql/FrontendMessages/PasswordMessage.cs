using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;

namespace Npgsql.FrontendMessages
{
    class PasswordMessage : SimpleFrontendMessage
    {
        internal byte[] Password { get; set; }

        const byte Code = (byte)'p';

        internal static PasswordMessage CreateClearText(string password)
        {
            var encoded = new byte[Encoding.UTF8.GetByteCount(password) + 1];
            Encoding.UTF8.GetBytes(password, 0, password.Length, encoded, 0);
            encoded[encoded.Length - 1] = 0;
            return new PasswordMessage(encoded);
        }

        /// <summary>
        /// Creates an MD5 password message.
        /// This is the password, hashed with the username as salt, and hashed again with the backend-provided
        /// salt.
        /// </summary>
        internal static PasswordMessage CreateMD5(string password, string username, byte[] serverSalt)
        {
            var md5 = MD5.Create();

            // First phase
            var passwordBytes = PGUtil.UTF8Encoding.GetBytes(password);
            var usernameBytes = PGUtil.UTF8Encoding.GetBytes(username);
            var cryptBuf = new byte[passwordBytes.Length + usernameBytes.Length];
            passwordBytes.CopyTo(cryptBuf, 0);
            usernameBytes.CopyTo(cryptBuf, passwordBytes.Length);

            var sb = new StringBuilder();
            var hashResult = md5.ComputeHash(cryptBuf);
            foreach (var b in hashResult) {
                sb.Append(b.ToString("x2"));
            }

            var prehash = sb.ToString();

            var prehashbytes = PGUtil.UTF8Encoding.GetBytes(prehash);
            cryptBuf = new byte[prehashbytes.Length + 4];

            Array.Copy(serverSalt, 0, cryptBuf, prehashbytes.Length, 4);

            // 2.
            prehashbytes.CopyTo(cryptBuf, 0);

            sb = new StringBuilder("md5");
            hashResult = md5.ComputeHash(cryptBuf);
            foreach (var b in hashResult) {
                sb.Append(b.ToString("x2"));
            }

            var resultString = sb.ToString();
            var result = new byte[Encoding.UTF8.GetByteCount(resultString) + 1];
            Encoding.UTF8.GetBytes(resultString, 0, resultString.Length, result, 0);
            result[result.Length - 1] = 0;

            return new PasswordMessage(result);
        }

        internal PasswordMessage(byte[] password)
        {
            Password = password;
        }

        internal override int Length { get { return 4 + Password.Length; } }

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length);
            buf.WriteBytes(Password);
        }

        public override string ToString() { return "[Password]"; }
    }
}
