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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Npgsql.Tls
{
    class SCRAM
    {
        internal string Scheme { get; private set; }
        internal string UserName { get; private set; }
        internal string Password { get; set; }
        internal string ClientSignature { get; set; }
        internal string ServerSignature { get; set; }
        internal string ClientNonce { get; private set; }
        internal string ServerNonce { get; private set; }
        internal string ServerSalt { get; private set; }
        internal int ServerIteration { get; private set; }

        internal bool ServerFirstReceived { get; private set; }

        internal SCRAM(string schemeName, string userName) {
            Scheme = schemeName;
            UserName = userName;
            // TODO: Create random nonce
            ClientNonce = "fyko+d2lbbFgONRv9qkxdawL";
        }

        internal string GetClientFirstMessage()
        {
            // Postgresql does not expect username in SCRAMFirst message, User name already provided in authentication request
            var userName = "*";
            var msg = "n,,n="+userName+",r=" + ClientNonce;
            return msg;
        }

        internal byte[] HMAC(byte[] data, string key)
        {
            var hmac = new HMACSHA256(data);
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        internal byte[] getSaltedPassword(string salt, int count)
        {
            var saltBytes = Convert.FromBase64String(salt);

            var saltedPassword = Hi(Password.Normalize(NormalizationForm.FormKC), saltBytes, count);
            return saltedPassword;
        }

        internal void parseServerFirstMessage(string payload)
        {
            var delimiter = ',';
            var parts = payload.Split(delimiter);
            if (parts.Length != 3 )
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFirst message");

            // Server Nonce expected
            var r = parts[0];
            if (!parts[0].StartsWith("r="))
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFirst message");
            ServerNonce = parts[0].Substring(2);

            // Salt expected
            var s = parts[1];
            if (!parts[1].StartsWith("s="))
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFirst message");
            ServerSalt = parts[1].Substring(2);

            // Iteration count expected
            var i = parts[2];
            if (!parts[2].StartsWith("i="))
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFirst message");
            ServerIteration = int.Parse(parts[2].Substring(2));

            checkServerNonce();

            ServerFirstReceived = true;
        }

        internal void checkServerNonce()
        {
            if ( !ServerNonce.StartsWith(ClientNonce))
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFirst message");
        }

        internal string CreateClientFinalMessage()
        {
            if (!ServerFirstReceived)
                throw new InvalidOperationException("[SCRAM] SCRAMServerFirst not received");

            var saltedPassword = getSaltedPassword(ServerSalt, ServerIteration);

            var clientKey = HMAC(saltedPassword, "Client Key");
            var storedKey = H(clientKey);
            
            var client_first_message_bare = "n=*,r=" + ClientNonce;

            var server_first_message = "r=" + ServerNonce + ",s=" + ServerSalt + ",i=" + ServerIteration.ToString();

            var client_final_message_without_proof = "c=biws,r=" + ServerNonce;

            var AuthMessage = client_first_message_bare + ","
                                + server_first_message + "," 
                                + client_final_message_without_proof;

            var clientSignature = HMAC(storedKey, AuthMessage);
            var clientProofBytes = XOR(clientKey, clientSignature);
            var serverKey = HMAC(saltedPassword, "Server Key");
            var serverSignature = HMAC(serverKey, AuthMessage);

            var clientProof = Convert.ToBase64String(clientProofBytes);
            
            var msg = client_final_message_without_proof + ",p=" + clientProof;
            return msg;

        }

        internal void verifyServer(string payload)
        {
            if (!payload.StartsWith("v="))
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFinal message");
            var v = payload.Substring(2);

        }
        public static byte[] XOR(byte[] buffer1, byte[] buffer2)
        {
            for (var i = 0; i < buffer1.Length; i++)
                buffer1[i] ^= buffer2[i];
            return buffer1;
        }

        public static byte[] H(byte[] data)
        {
            var sha256 = SHA256.Create();
            return sha256.ComputeHash(data);
        }

        public static byte[] Hi(string str, byte[] salt, int count)
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
        
    }
}
