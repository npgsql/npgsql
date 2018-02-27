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
using System.Security.Cryptography;

namespace Npgsql.Tls
{
    // http://csrc.nist.gov/publications/nistpubs/800-38D/SP-800-38D.pdf
    // http://csrc.nist.gov/groups/ST/toolkit/BCM/documents/proposedmodes/gcm/gcm-spec.pdf
    // This implementation uses "simple tables" with 8192 bytes per key
    class GaloisCounterMode
    {
        // Slow multiplication
        static void Mul(ulong h1, ulong h2, ulong y1, ulong y2, ulong x1, ulong x2, ref ulong ynext1, ref ulong ynext2)
        {
            var left1 = y1 ^ x1;
            var left2 = y2 ^ x2;
            ulong z1 = 0;
            ulong z2 = 0;
            for (var i = 63; i >= 0; i--)
            {
                if (((left1 >> i) & 1) != 0)
                {
                    z1 ^= h1;
                    z2 ^= h2;
                }
                var lsb = (h2 & 1) != 0;
                h2 = (h2 >> 1) | (h1 << 63);
                h1 >>= 1;
                if (lsb)
                    h1 ^= 225UL << 56;
            }
            for (var i = 63; i >= 0; i--)
            {
                if (((left2 >> i) & 1) != 0)
                {
                    z1 ^= h1;
                    z2 ^= h2;
                }
                var lsb = (h2 & 1) != 0;
                h2 = (h2 >> 1) | (h1 << 63);
                h1 >>= 1;
                if (lsb)
                    h1 ^= 225UL << 56;
            }
            ynext1 = z1;
            ynext2 = z2;
        }

        // Faster multiplication
        static void MulWithTable(ulong[] tbl, ulong y1, ulong y2, ulong x1, ulong x2, ref ulong ynext1, ref ulong ynext2)
        {
            var left1 = y1 ^ x1;
            var left2 = y2 ^ x2;
            ulong z1 = 0;
            ulong z2 = 0;
            for (int i = 60, j = 31 * 16; i >= 0; i -= 4, j -= 16)
            {
                z1 ^= tbl[j | (int)(left1 >> i) & 0xf];
                z2 ^= tbl[32 * 16 | j | (int)(left1 >> i) & 0xf];
            }
            for (int i = 60, j = 15 * 16; i >= 0; i -= 4, j -= 16)
            {
                z1 ^= tbl[j | (int)(left2 >> i) & 0xf];
                z2 ^= tbl[32 * 16 | j | (int)(left2 >> i) & 0xf];
            }
            ynext1 = z1;
            ynext2 = z2;
        }

        // Fastest multiplication
        static void MulWithTable2(ulong[] tbl, ulong y1, ulong y2, byte[] x, int offset, ref ulong ynext1, ref ulong ynext2)
        {
            ulong z1 = 0;
            ulong z2 = 0;

            // These loops are unrolled below
            /*for (var i = 0; i < 8; i++)
            {
                z1 ^= tbl[(31-i*2) * 16 | (((int)(y1 >> (60 - i * 8)) ^ (x[offset + i] >> 4)) & 0xf)];
                z2 ^= tbl[(63-i*2) * 16 | (((int)(y1 >> (60 - i * 8)) ^ (x[offset + i] >> 4)) & 0xf)];
                z1 ^= tbl[(30-i*2) * 16 | (((int)(y1 >> (56 - i * 8)) ^ x[offset + i]) & 0xf)];
                z2 ^= tbl[(62-i*2) * 16 | (((int)(y1 >> (56 - i * 8)) ^ x[offset + i]) & 0xf)];
            }
            for (var i = 0; i < 8; i++)
            {
                z1 ^= tbl[(15 - i * 2) * 16 | (((int)(y2 >> (60 - i * 8)) ^ (x[offset + 8+i] >> 4)) & 0xf)];
                z2 ^= tbl[(47 - i * 2) * 16 | (((int)(y2 >> (60 - i * 8)) ^ (x[offset + 8+i] >> 4)) & 0xf)];
                z1 ^= tbl[(14 - i * 2) * 16 | (((int)(y2 >> (56 - i * 8)) ^ x[offset + 8+i]) & 0xf)];
                z2 ^= tbl[(46 - i * 2) * 16 | (((int)(y2 >> (56 - i * 8)) ^ x[offset + 8+i]) & 0xf)];
            }*/

            int b;

            b = x[offset + 0];
            z1 ^= tbl[31 * 16 | (((int)(y1 >> 60) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[63 * 16 | (((int)(y1 >> 60) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[30 * 16 | (((int)(y1 >> 56) ^ b) & 0xf)];
            z2 ^= tbl[62 * 16 | (((int)(y1 >> 56) ^ b) & 0xf)];
            b = x[offset + 1];
            z1 ^= tbl[29 * 16 | (((int)(y1 >> 52) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[61 * 16 | (((int)(y1 >> 52) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[28 * 16 | (((int)(y1 >> 48) ^ b) & 0xf)];
            z2 ^= tbl[60 * 16 | (((int)(y1 >> 48) ^ b) & 0xf)];
            b = x[offset + 2];
            z1 ^= tbl[27 * 16 | (((int)(y1 >> 44) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[59 * 16 | (((int)(y1 >> 44) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[26 * 16 | (((int)(y1 >> 40) ^ b) & 0xf)];
            z2 ^= tbl[58 * 16 | (((int)(y1 >> 40) ^ b) & 0xf)];
            b = x[offset + 3];
            z1 ^= tbl[25 * 16 | (((int)(y1 >> 36) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[57 * 16 | (((int)(y1 >> 36) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[24 * 16 | (((int)(y1 >> 32) ^ b) & 0xf)];
            z2 ^= tbl[56 * 16 | (((int)(y1 >> 32) ^ b) & 0xf)];
            b = x[offset + 4];
            z1 ^= tbl[23 * 16 | (((int)(y1 >> 28) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[55 * 16 | (((int)(y1 >> 28) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[22 * 16 | (((int)(y1 >> 24) ^ b) & 0xf)];
            z2 ^= tbl[54 * 16 | (((int)(y1 >> 24) ^ b) & 0xf)];
            b = x[offset + 5];
            z1 ^= tbl[21 * 16 | (((int)(y1 >> 20) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[53 * 16 | (((int)(y1 >> 20) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[20 * 16 | (((int)(y1 >> 16) ^ b) & 0xf)];
            z2 ^= tbl[52 * 16 | (((int)(y1 >> 16) ^ b) & 0xf)];
            b = x[offset + 6];
            z1 ^= tbl[19 * 16 | (((int)(y1 >> 12) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[51 * 16 | (((int)(y1 >> 12) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[18 * 16 | (((int)(y1 >> 8) ^ b) & 0xf)];
            z2 ^= tbl[50 * 16 | (((int)(y1 >> 8) ^ b) & 0xf)];
            b = x[offset + 7];
            z1 ^= tbl[17 * 16 | (((int)(y1 >> 4) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[49 * 16 | (((int)(y1 >> 4) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[16 * 16 | (((int)(y1 >> 0) ^ b) & 0xf)];
            z2 ^= tbl[48 * 16 | (((int)(y1 >> 0) ^ b) & 0xf)];
            b = x[offset + 8];
            z1 ^= tbl[15 * 16 | (((int)(y2 >> 60) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[47 * 16 | (((int)(y2 >> 60) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[14 * 16 | (((int)(y2 >> 56) ^ b) & 0xf)];
            z2 ^= tbl[46 * 16 | (((int)(y2 >> 56) ^ b) & 0xf)];
            b = x[offset + 9];
            z1 ^= tbl[13 * 16 | (((int)(y2 >> 52) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[45 * 16 | (((int)(y2 >> 52) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[12 * 16 | (((int)(y2 >> 48) ^ b) & 0xf)];
            z2 ^= tbl[44 * 16 | (((int)(y2 >> 48) ^ b) & 0xf)];
            b = x[offset + 10];
            z1 ^= tbl[11 * 16 | (((int)(y2 >> 44) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[43 * 16 | (((int)(y2 >> 44) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[10 * 16 | (((int)(y2 >> 40) ^ b) & 0xf)];
            z2 ^= tbl[42 * 16 | (((int)(y2 >> 40) ^ b) & 0xf)];
            b = x[offset + 11];
            z1 ^= tbl[9 * 16 | (((int)(y2 >> 36) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[41 * 16 | (((int)(y2 >> 36) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[8 * 16 | (((int)(y2 >> 32) ^ b) & 0xf)];
            z2 ^= tbl[40 * 16 | (((int)(y2 >> 32) ^ b) & 0xf)];
            b = x[offset + 12];
            z1 ^= tbl[7 * 16 | (((int)(y2 >> 28) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[39 * 16 | (((int)(y2 >> 28) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[6 * 16 | (((int)(y2 >> 24) ^ b) & 0xf)];
            z2 ^= tbl[38 * 16 | (((int)(y2 >> 24) ^ b) & 0xf)];
            b = x[offset + 13];
            z1 ^= tbl[5 * 16 | (((int)(y2 >> 20) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[37 * 16 | (((int)(y2 >> 20) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[4 * 16 | (((int)(y2 >> 16) ^ b) & 0xf)];
            z2 ^= tbl[36 * 16 | (((int)(y2 >> 16) ^ b) & 0xf)];
            b = x[offset + 14];
            z1 ^= tbl[3 * 16 | (((int)(y2 >> 12) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[35 * 16 | (((int)(y2 >> 12) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[2 * 16 | (((int)(y2 >> 8) ^ b) & 0xf)];
            z2 ^= tbl[34 * 16 | (((int)(y2 >> 8) ^ b) & 0xf)];
            b = x[offset + 15];
            z1 ^= tbl[1 * 16 | (((int)(y2 >> 4) ^ (b >> 4)) & 0xf)];
            z2 ^= tbl[33 * 16 | (((int)(y2 >> 4) ^ (b >> 4)) & 0xf)];
            z1 ^= tbl[0 * 16 | (((int)(y2 >> 0) ^ b) & 0xf)];
            z2 ^= tbl[32 * 16 | (((int)(y2 >> 0) ^ b) & 0xf)];

            ynext1 = z1;
            ynext2 = z2;
        }

        // Constructs H table
        static ulong[] Construct(ulong h1, ulong h2)
        {
            var tbl = new ulong[32 * 16 * 2 + 2];
            for (var i = 31; i >= 0; i--)
            {
                for (var j = 0; j < 16; j++)
                {
                    ulong y1 = 0, y2 = 0;
                    if (i >= 16) y1 = (ulong)j << ((i - 16) * 4);
                    else y2 = (ulong)j << (i * 4);

                    ulong out1 = 0, out2 = 0;

                    Mul(h1, h2, y1, y2, 0, 0, ref out1, ref out2);

                    tbl[i * 16 + j] = out1;
                    tbl[32 * 16 + i * 16 + j] = out2;
                }
            }
            // Also fill in the original h1 and h2 values
            tbl[tbl.Length - 2] = h1;
            tbl[tbl.Length - 1] = h2;
            return tbl;
        }

        static void Encrypt(byte[] data, int offset, int len, ICryptoTransform key, byte[] iv, int startCounter, byte[] temp512)
        {
            var buf = temp512;
            var block = startCounter;

            const int numBlocks = 32;

            for (var pos = 0; pos < len; pos += 16 * numBlocks)
            {
                var blocks = numBlocks;
                if (pos + 16 * numBlocks > len)
                    blocks = (len - pos + 15) / 16;

                for (var i = 0; i < blocks; i++)
                {
                    Buffer.BlockCopy(iv, 0, buf, i * 16, 14);
                    buf[i * 16 + 14] = (byte)(block >> 8);
                    buf[i * 16 + 15] = (byte)block;
                    ++block;
                }

                key.TransformBlock(buf, 0, blocks * 16, buf, 0);
                var end = Math.Min(pos + numBlocks * 16, len);
                for (int i = pos, j = 0; i < end; i++, j++)
                {
                    data[offset + i] ^= buf[j];
                }
            }
        }

        static void CalcHash(ICryptoTransform key, byte[] iv, byte[] data, int offset, int len, ulong seqNum, ulong header, ulong[] h, byte[] temp512)
        {
            ulong s1 = 0, s2 = 0;

            // Additional data
            // Don't use table-based multiplication for the first block to avoid potential timing-attack vulnerabilities as described in
            // http://udspace.udel.edu/bitstream/handle/19716/9765/Bonan_Huang_thesis.pdf (Cache-collision timing attacks against AES-GCM)
            Mul(h[h.Length - 2], h[h.Length - 1], s1, s2, seqNum, header, ref s1, ref s2);

            // Ciphertext
            var end = offset + len / 16 * 16;
            for (var pos = offset; pos < end; )
            {
                MulWithTable2(h, s1, s2, data, pos, ref s1, ref s2);
                pos += 16;
            }
            if (end != offset + len)
            {
                ulong x1 = 0;
                ulong x2 = 0;
                for (int pos = end, b = 15; pos < offset + len; pos++, b--)
                {
                    if (b >= 8)
                        x1 |= (ulong)data[pos] << (b - 8) * 8;
                    else
                        x2 |= (ulong)data[pos] << b * 8;
                }
                MulWithTable(h, s1, s2, x1, x2, ref s1, ref s2);
            }

            // Length of additional data and ciphertext data
            MulWithTable(h, s1, s2, 13 * 8, (ulong)(len * 8), ref s1, ref s2);

            // Put the hash at the end of the data stream and encrypt it
            Utils.WriteUInt64(data, offset + len, s1);
            Utils.WriteUInt64(data, offset + len + 8, s2);
            Encrypt(data, offset + len, 16, key, iv, 1, temp512);
        }

        // Constructs the second part of the "additional data" for TLS 1.2, which is the packet header
        static ulong GetHeader(byte contentType, int length)
        {
            return ((ulong)contentType << 56) | ((ulong)0x0303 << 40) | ((ulong)length << 24);
        }

        // key = aes key
        // iv = 128 bit, 96 msb is salt + explicit nonce
        // h1 = aes(key, 0)[0..63]
        // h2 = aes(key, 0)[64..127]
        public static void GCMAE(ICryptoTransform key, byte[] iv, byte[] data, int offset, int len, ulong seqNum, byte contentType, ulong[] h, byte[] temp512)
        {
            Encrypt(data, offset, len, key, iv, 2, temp512);
            CalcHash(key, iv, data, offset, len, seqNum, GetHeader(contentType, len), h, temp512);
        }

        public static bool GCMAD(ICryptoTransform key, byte[] iv, byte[] data, int offset, int len, ulong seqNum, byte contentType, ulong[] h, byte[] temp512)
        {
            var tOffset = offset + len;
            var s1 = Utils.ReadUInt64(data, ref tOffset);
            var s2 = Utils.ReadUInt64(data, ref tOffset);
            CalcHash(key, iv, data, offset, len, seqNum, GetHeader(contentType, len), h, temp512);
            tOffset = offset + len;
            var s1p = Utils.ReadUInt64(data, ref tOffset);
            var s2p = Utils.ReadUInt64(data, ref tOffset);
            if (s1 != s1p || s2 != s2p)
                return false;
            Encrypt(data, offset, len, key, iv, 2, temp512);
            return true;
        }

        public static ulong[] GetH(ICryptoTransform key)
        {
            var bytes = new byte[16];
            key.TransformBlock(bytes, 0, 16, bytes, 0);
            var offset = 0;
            var h1 = Utils.ReadUInt64(bytes, ref offset);
            var h2 = Utils.ReadUInt64(bytes, ref offset);
            Utils.ClearArray(bytes);
            return Construct(h1, h2);
        }
    }
}
