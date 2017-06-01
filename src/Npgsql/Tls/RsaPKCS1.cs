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
using System.Numerics;
using System.Security.Cryptography;

namespace Npgsql.Tls
{
    internal static class RsaPKCS1
    {
        public static bool VerifyRsaPKCS1(RSA key, byte[] signature, byte[] hash, bool allowNoPadding)
        {
            var parameters = key.ExportParameters(false);

            var e = Utils.BigIntegerFromBigEndian(parameters.Exponent, 0, parameters.Exponent.Length);
            var mod = Utils.BigIntegerFromBigEndian(parameters.Modulus, 0, parameters.Modulus.Length);
            var m = Utils.BigIntegerFromBigEndian(signature, 0, signature.Length);
            var decryptedArr = Utils.BigEndianFromBigInteger(BigInteger.ModPow(m, e, mod));

            /*
            PKCS padding used in TLS 1.0/TLS 1.1:
            00 01 [k-3-hashlen 0xff bytes] 00 (hash)
            OR, for only TLS 1.0, there may be no padding (or equivalently, 00 00 [k-3-hashlen 00 bytes] 00 (hash))
            where k is the keylen
            */

            if (allowNoPadding && decryptedArr.Length <= hash.Length)
            {
                var zeros = hash.Length - decryptedArr.Length;
                for (var i = 0; i < zeros; i++)
                {
                    if (hash[i] != 0)
                        return false;
                }
                return Utils.ArraysEqual(decryptedArr, 0, hash, zeros, hash.Length - zeros);
            }

            if (decryptedArr.Length != parameters.Modulus.Length - 1)
                return false;

            if (decryptedArr[0] != 1)
                return false;

            for (var i = 1; i < decryptedArr.Length - hash.Length - 1; i++)
            {
                if (decryptedArr[i] != 0xff)
                    return false;
            }
            if (decryptedArr[decryptedArr.Length - hash.Length - 1] != 0)
                return false;

            return Utils.ArraysEqual(decryptedArr, decryptedArr.Length - hash.Length, hash, 0, hash.Length);
        }
        public static byte[] SignRsaPKCS1(RSA key, byte[] hash)
        {
            // NOTE: The X509Certificate2 must be initialized with the X509KeyStorageFlags.Exportable flag
            var parameters = key.ExportParameters(true);

            var dp = Utils.BigIntegerFromBigEndian(parameters.DP, 0, parameters.DP.Length);
            var dq = Utils.BigIntegerFromBigEndian(parameters.DQ, 0, parameters.DQ.Length);
            var qinv = Utils.BigIntegerFromBigEndian(parameters.InverseQ, 0, parameters.InverseQ.Length);
            var p = Utils.BigIntegerFromBigEndian(parameters.P, 0, parameters.P.Length);
            var q = Utils.BigIntegerFromBigEndian(parameters.Q, 0, parameters.Q.Length);

            var data = new byte[parameters.D.Length - 1];
            data[0] = 1;
            for (var i = 1; i < data.Length - hash.Length - 1; i++)
            {
                data[i] = 0xff;
            }
            data[data.Length - hash.Length - 1] = 0;
            Buffer.BlockCopy(hash, 0, data, data.Length - hash.Length, hash.Length);

            var m = Utils.BigIntegerFromBigEndian(data, 0, data.Length);

            var m1 = BigInteger.ModPow(m, dp, p);
            var m2 = BigInteger.ModPow(m, dq, q);
            var h = qinv * (m1 - m2) % p;
            if (h.Sign == -1)
                h += p;
            var signature = Utils.BigEndianFromBigInteger(m2 + h * q);

            Utils.ClearArray(parameters.D);
            Utils.ClearArray(parameters.DP);
            Utils.ClearArray(parameters.DQ);
            Utils.ClearArray(parameters.InverseQ);
            Utils.ClearArray(parameters.P);
            Utils.ClearArray(parameters.Q);

            return signature;
        }
    }
}
