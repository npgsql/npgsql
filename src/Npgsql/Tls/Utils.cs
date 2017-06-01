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
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Npgsql.Tls
{
    static class Utils
    {
        public static readonly Dictionary<string, string> HashNameToOID = new Dictionary<string, string>() {
            {"SHA1", "1.3.14.3.2.26"},
            {"SHA256", "2.16.840.1.101.3.4.2.1"},
            {"SHA384", "2.16.840.1.101.3.4.2.2"},
            {"SHA512", "2.16.840.1.101.3.4.2.3"}
        };

        public static int GetHashLen(TlsHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case TlsHashAlgorithm.SHA1:
                    return 160;
                case TlsHashAlgorithm.SHA256:
                    return 256;
                case TlsHashAlgorithm.SHA384:
                    return 384;
                case TlsHashAlgorithm.SHA512:
                    return 512;
                default:
                    throw new NotSupportedException();
            }
        }

        public static void ClearArray(Array array)
        {
            Array.Clear(array, 0, array.Length);
        }

        public static bool ArraysEqual(byte[] arr1, int offset1, byte[] arr2, int offset2, int len)
        {
            var end = offset1 + len;
            for (int i1 = offset1, i2 = offset2; i1 < end; i1++, i2++)
            {
                if (arr1[i1] != arr2[i2])
                    return false;
            }
            return true;
        }

        public static int WriteUInt64(byte[] buf, int offset, ulong v)
        {
            buf[offset] = (byte)(v >> 56);
            buf[offset + 1] = (byte)(v >> 48);
            buf[offset + 2] = (byte)(v >> 40);
            buf[offset + 3] = (byte)(v >> 32);
            buf[offset + 4] = (byte)(v >> 24);
            buf[offset + 5] = (byte)(v >> 16);
            buf[offset + 6] = (byte)(v >> 8);
            buf[offset + 7] = (byte)v;

            return 8;
        }

        public static int WriteUInt32(byte[] buf, int offset, uint v)
        {
            buf[offset] = (byte)(v >> 24);
            buf[offset + 1] = (byte)(v >> 16);
            buf[offset + 2] = (byte)(v >> 8);
            buf[offset + 3] = (byte)v;

            return 4;
        }

        public static int WriteUInt24(byte[] buf, int offset, int v)
        {
            buf[offset] = (byte)(v >> 16);
            buf[offset + 1] = (byte)(v >> 8);
            buf[offset + 2] = (byte)v;

            return 3;
        }

        public static int WriteUInt16(byte[] buf, int offset, ushort v)
        {
            buf[offset] = (byte)(v >> 8);
            buf[offset + 1] = (byte)v;

            return 2;
        }

        public static ushort ReadUInt16(byte[] buf, ref int offset)
        {
            var res = (ushort)((buf[offset] << 8) | buf[offset + 1]);
            offset += 2;
            return res;
        }

        public static int ReadUInt24(byte[] buf, ref int offset)
        {
            var res = (buf[offset] << 16) | (buf[offset + 1] << 8) | buf[offset + 2];
            offset += 3;
            return res;
        }

        public static uint ReadUInt32(byte[] buf, ref int offset)
        {
            var res = (buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3];
            offset += 4;
            return (uint)res;
        }

        public static ulong ReadUInt64(byte[] buf, ref int offset)
        {
            var res = ((ulong)buf[offset] << 56) | ((ulong)buf[offset + 1] << 48) | ((ulong)buf[offset + 2] << 40) | ((ulong)buf[offset + 3] << 32) |
                ((ulong)buf[offset + 4] << 24) | ((ulong)buf[offset + 5] << 16) | ((ulong)buf[offset + 6] << 8) | (ulong)buf[offset + 7];
            offset += 8;
            return res;
        }

        /// <summary>
        /// hmac should be initialized with the secret key
        /// </summary>
        /// <param name="hmac"></param>
        /// <param name="label"></param>
        /// <param name="seed"></param>
        /// <param name="bytesNeeded"></param>
        /// <returns></returns>
        public static byte[] PRF(HMAC hmac, string label, byte[] seed, int bytesNeeded)
        {
            var blockSize = hmac.HashSize / 8;
            var rounds = (bytesNeeded + (blockSize - 1)) / blockSize;

            var labelLen = Encoding.ASCII.GetByteCount(label);
            var a = new byte[labelLen + seed.Length];
            Encoding.ASCII.GetBytes(label, 0, label.Length, a, 0);
            Buffer.BlockCopy(seed, 0, a, labelLen, seed.Length);

            var ret = new byte[rounds * blockSize];
            var input = new byte[blockSize + a.Length];
            Buffer.BlockCopy(a, 0, input, blockSize, a.Length);

            for (var i = 0; i < rounds; i++)
            {
                var aNew = hmac.ComputeHash(a);
                ClearArray(a);
                a = aNew;
                Buffer.BlockCopy(a, 0, input, 0, blockSize);
                var temp = hmac.ComputeHash(input);
                Buffer.BlockCopy(temp, 0, ret, i * blockSize, blockSize);
                ClearArray(temp);
            }
            ClearArray(a);
            ClearArray(input);
            if (bytesNeeded == ret.Length)
                return ret;
            var retTruncated = new byte[bytesNeeded];
            Buffer.BlockCopy(ret, 0, retTruncated, 0, bytesNeeded);
            ClearArray(ret);
            return retTruncated;
        }

        public static byte[] PRF(PRFAlgorithm prfAlgorithm, byte[] key, string label, byte[] seed, int bytesNeeded)
        {
            switch (prfAlgorithm)
            {
                case PRFAlgorithm.TLSPrfSHA256:
                    using (var hmac = new HMACSHA256(key))
                        return PRF(hmac, label, seed, bytesNeeded);
                case PRFAlgorithm.TLSPrfSHA384:
                    using (var hmac = new HMACSHA384(key))
                        return PRF(hmac, label, seed, bytesNeeded);
                case PRFAlgorithm.TLSPrfMD5SHA1:
                    var halfKeyLen = (key.Length + 1) / 2;
                    var key1 = new byte[halfKeyLen];
                    var key2 = new byte[halfKeyLen];
                    Buffer.BlockCopy(key, 0, key1, 0, halfKeyLen);
                    Buffer.BlockCopy(key, key.Length - halfKeyLen, key2, 0, halfKeyLen);
                    using (var hmac1 = new HMACMD5(key1))
                    {
                        using (var hmac2 = new HMACSHA1(key2))
                        {
                            var prf1 = PRF(hmac1, label, seed, bytesNeeded);
                            var prf2 = PRF(hmac2, label, seed, bytesNeeded);
                            for (var i = 0; i < bytesNeeded; i++)
                            {
                                prf1[i] ^= prf2[i];
                            }
                            ClearArray(key1);
                            ClearArray(key2);
                            ClearArray(prf2);
                            return prf1;
                        }
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        public static int GetASNLength(byte[] buf, ref int offset)
        {
            if ((buf[offset] & 0x80) == 0)
                return buf[offset++];
            var lenLen = buf[offset++] & ~0x80;
            if (lenLen > 3)
                throw new NotSupportedException("ASN sequences longer than 2^24 bytes not supported.");
            var len = 0;
            for (var i = 0; i < lenLen; i++)
            {
                len <<= 8;
                len += buf[offset++];
            }
            return len;
        }

        public static bool HostnameInCertificate(X509Certificate2 certificate, string hostname)
        {
            var ext = certificate.Extensions["2.5.29.17"];
            if (ext != null)
            {
                var bytes = ext.RawData;
                if (bytes[0] == 0x30) // General names tag
                {
                    var offset = 1;
                    var len = GetASNLength(bytes, ref offset);
                    var end = offset + len;
                    while (offset < end)
                    {
                        var tag = bytes[offset++];
                        var itemLen = GetASNLength(bytes, ref offset);
                        switch (tag)
                        {
                            case 0x82: // dNSName
                                var name = Encoding.ASCII.GetString(bytes, offset, itemLen);
                                if (MatchHostname(name, hostname))
                                    return true;
                                break;
                            case 0x87: // iPAddress
                                var ipBytes = new byte[itemLen];
                                Buffer.BlockCopy(bytes, offset, ipBytes, 0, itemLen);
                                var ip = new System.Net.IPAddress(ipBytes);
                                if (System.Net.IPAddress.TryParse(hostname, out var hostIp) && ip.Equals(hostIp))
                                    return true;
                                break;
                            default: // Other types are not checked according to rfc2818, so skip
                                break;
                        }
                        offset += itemLen;
                    }
                }
            }
            var other = certificate.GetNameInfo(X509NameType.DnsName, false);
            if (!string.IsNullOrEmpty(other))
                return MatchHostname(other, hostname);
            return false;
        }

        public static bool MatchHostname(string altname, string hostname)
        {
            altname = altname.ToLower();
            hostname = hostname.ToLower();
            if (altname == hostname)
                return true;

            if (altname == "")
                return false;

            var dotIndex = altname.IndexOf('.');
            string firstPart;
            var rest = "";
            if (dotIndex != -1)
            {
                firstPart = altname.Substring(0, dotIndex);
                rest = altname.Substring(dotIndex);
            }
            else
            {
                firstPart = altname;
            }

            if (firstPart.Length > 0 && firstPart.All(c => c == '*'))
                return Regex.IsMatch(hostname, "^[^.]+" + Regex.Escape(rest) + "$");

            if (firstPart.StartsWith("xn--") || hostname.StartsWith("xn--"))
                return false;

            return Regex.IsMatch(hostname, "^" + Regex.Escape(firstPart).Replace(@"\*", "[^.]*") + Regex.Escape(rest) + "$");
        }

        public static byte[] DecodeDERSignature(byte[] signature, int offset, int len, int integerLength)
        {
            var decodedSignature = new byte[integerLength * 2];
            offset += 1; // Skip tag 0x30 (SEQUENCE)
            Utils.GetASNLength(signature, ref offset);
            offset += 1; // 0x02 (INTEGER)

            var len1 = Utils.GetASNLength(signature, ref offset);
            if (integerLength == len1 - 1)
            {
                // Remove sign byte
                offset++;
                len1--;
            }
            // NOTE: MSB zeros are not present in the ASN-encoding, so we must add them if the length is shorter than expected
            Buffer.BlockCopy(signature, offset, decodedSignature, integerLength - len1, len1);
            offset += len1;
            offset += 1; // 0x02 (INTEGER)
            var len2 = Utils.GetASNLength(signature, ref offset);
            if (integerLength == len2 - 1)
            {
                // Remove sign byte
                offset++;
                len2--;
            }
            Buffer.BlockCopy(signature, offset, decodedSignature, integerLength * 2 - len2, len2);

            return decodedSignature;
        }

        public static BigInteger BigIntegerFromBigEndian(byte[] arr, int offset, int len)
        {
            var littleEndian = new byte[len + 1];
            for (var i = 0; i < len; i++)
            {
                littleEndian[len - 1 - i] = arr[offset + i];
            }
            return new BigInteger(littleEndian);
        }

        public static byte[] BigEndianFromBigInteger(BigInteger bi)
        {
            var littleEndian = bi.ToByteArray();
            var bigEndian = new byte[littleEndian[littleEndian.Length - 1] == 0 ? littleEndian.Length - 1 : littleEndian.Length];
            for (var i = 0; i < bigEndian.Length; i++)
            {
                bigEndian[i] = littleEndian[bigEndian.Length - 1 - i];
            }
            return bigEndian;
        }

#if NET45 || NET451
        public static void TransformBlock(this HashAlgorithm hashAlg, byte[] buf, int offset, int len)
        {
            hashAlg.TransformBlock(buf, offset, len, null, 0);
        }

        public static void AppendData(this HashAlgorithm hash, byte[] data, int offset, int len)
        {
            hash.TransformBlock(data, offset, len);
        }

        public static byte[] GetHashAndReset(this HashAlgorithm hash)
        {
            hash.TransformFinalBlock(Hasher.EmptyByteArray, 0, 0);
            byte[] data = hash.Hash;
            hash.Initialize();
            return data;
        }
#endif

        public static byte[] EncryptPkcsPadding(X509Certificate2 cert, byte[] rgb)
        {
#if NET45 || NET451
            return ((RSACryptoServiceProvider)cert.PublicKey.Key).Encrypt(rgb, false);
#else
            using (var rsa = cert.GetRSAPublicKey())
            {
                return rsa.Encrypt(rgb, RSAEncryptionPadding.Pkcs1);
            }
#endif
        }
    }
}
