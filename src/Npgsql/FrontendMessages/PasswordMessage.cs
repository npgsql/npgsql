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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;

namespace Npgsql.FrontendMessages
{
    class PasswordMessage : FrontendMessage
    {
        internal byte[] Payload { get; private set; }
        internal int PayloadOffset { get; private set; }
        internal int PayloadLength { get; private set; }

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
            foreach (var b in hashResult)
                sb.Append(b.ToString("x2"));

            var prehash = sb.ToString();

            var prehashbytes = PGUtil.UTF8Encoding.GetBytes(prehash);
            cryptBuf = new byte[prehashbytes.Length + 4];

            Array.Copy(serverSalt, 0, cryptBuf, prehashbytes.Length, 4);

            // 2.
            prehashbytes.CopyTo(cryptBuf, 0);

            sb = new StringBuilder("md5");
            hashResult = md5.ComputeHash(cryptBuf);
            foreach (var b in hashResult)
                sb.Append(b.ToString("x2"));

            var resultString = sb.ToString();
            var result = new byte[Encoding.UTF8.GetByteCount(resultString) + 1];
            Encoding.UTF8.GetBytes(resultString, 0, resultString.Length, result, 0);
            result[result.Length - 1] = 0;

            return new PasswordMessage(result);
        }

        internal PasswordMessage() {}

        PasswordMessage(byte[] payload)
        {
            Payload = payload;
            PayloadOffset = 0;
            PayloadLength = payload.Length;
        }

        internal PasswordMessage Populate(byte[] payload, int offset, int count)
        {
            Payload = payload;
            PayloadOffset = offset;
            PayloadLength = count;
            return this;
        }

        internal override async Task Write(NpgsqlWriteBuffer buf, bool async)
        {
            if (buf.WriteSpaceLeft < 1 + 5)
                await buf.Flush(async);
            buf.WriteByte(Code);
            buf.WriteInt32(4 + PayloadLength);

            if (PayloadLength <= buf.WriteSpaceLeft)
            {
                // The entire array fits in our buffer, copy it into the buffer as usual.
                buf.WriteBytes(Payload, PayloadOffset, Payload.Length);
                return;
            }

            await buf.Flush(async);
            buf.DirectWrite(Payload, PayloadOffset, PayloadLength);
        }

        public override string ToString() =>  "[Password]";
    }

    #region SASL

    // TODO: Refactor above password messages into different classes to harmonize, clean up
    class SASLInitialResponseMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'p';
        readonly string _mechanism;
        [CanBeNull]
        readonly byte[] _initialResponse;

        internal SASLInitialResponseMessage(string mechanism, byte[] initialResponse)
        {
            _mechanism = mechanism;
            _initialResponse = initialResponse;
        }

        internal override int Length =>
            1 + 4 +
            PGUtil.UTF8Encoding.GetByteCount(_mechanism) + 1 +
            4 + (_initialResponse?.Length ?? 0);

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);

            buf.WriteString(_mechanism);
            buf.WriteByte(0);   // null terminator
            if (_initialResponse == null)
                buf.WriteInt32(-1);
            else
            {
                buf.WriteInt32(_initialResponse.Length);
                buf.WriteBytes(_initialResponse);
            }
        }
    }

    class SCRAMClientFinalMessage : SimpleFrontendMessage
    {
        const byte Code = (byte)'p';

        readonly string _messageStr;

        const string ClientKey = "Client Key";
        const string ServerKey = "Server Key";

        internal byte[] ServerSignature { get; }

        internal SCRAMClientFinalMessage(string password, string serverNonce, string salt, int serverIteration, string clientNonce)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var saltedPassword = Hi(password.Normalize(NormalizationForm.FormKC), saltBytes, serverIteration);

            var clientKey = HMAC(saltedPassword, ClientKey);
            var storedKey = SHA256.Create().ComputeHash(clientKey);

            var clientFirstMessageBare = "n=*,r=" + clientNonce;
            var serverFirstMessage = $"r={serverNonce},s={salt},i={serverIteration}";
            var clientFinalMessageWithoutProof = "c=biws,r=" + serverNonce;

            var authMessage = $"{clientFirstMessageBare},{serverFirstMessage},{clientFinalMessageWithoutProof}";

            var clientSignature = HMAC(storedKey, authMessage);
            var clientProofBytes = XOR(clientKey, clientSignature);
            var clientProof = Convert.ToBase64String(clientProofBytes);

            var serverKey = HMAC(saltedPassword, ServerKey);
            var serverSignatureBytes = HMAC(serverKey, authMessage);
            ServerSignature = serverSignatureBytes;

            _messageStr = $"{clientFinalMessageWithoutProof},p={clientProof}";
        }

        internal override int Length => 1 + 4 + PGUtil.UTF8Encoding.GetByteCount(_messageStr);

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            buf.WriteString(_messageStr);
        }

        static byte[] Hi(string str, byte[] salt, int count)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(str)))
            {
                var salt1 = new byte[salt.Length + 4];
                byte[] hi, u1;

                Buffer.BlockCopy(salt, 0, salt1, 0, salt.Length);
                salt1[salt1.Length - 1] = (byte)1;

                hi = u1 = hmac.ComputeHash(salt1);

                for (var i = 1; i < count; i++)
                {
                    var u2 = hmac.ComputeHash(u1);
                    XOR(hi, u2);
                    u1 = u2;
                }

                return hi;
            }
        }

        static byte[] XOR(byte[] buffer1, byte[] buffer2)
        {
            for (var i = 0; i < buffer1.Length; i++)
                buffer1[i] ^= buffer2[i];
            return buffer1;
        }

        static byte[] HMAC(byte[] data, string key)
           => new HMACSHA256(data).ComputeHash(Encoding.UTF8.GetBytes(key));
    }

    #endregion SASL
}
