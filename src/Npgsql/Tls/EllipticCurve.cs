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

#pragma warning disable CA2225 // Operator overloads have named alternates

using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

/*
 * Based almost completely on https://www.nsa.gov/ia/_files/nist-routines.pdf
 */

namespace Npgsql.Tls
{
    class EllipticCurve
    {
        internal static readonly EllipticCurve P256 = new EllipticCurve(
            new BigInt {
                _bits = new uint[] { 0xffffffff, 0xffffffff, 0xffffffff, 0, 0, 0, 1, 0xffffffff }
            },
            new BigInteger(new byte[] {0x4b, 0x60, 0xd2, 0x27, 0x3e, 0x3c, 0xce, 0x3b, 0xf6, 0xb0, 0x53, 0xcc, 0xb0, 0x06, 0x1d, 0x65,
               0xbc, 0x86, 0x98, 0x76, 0x55, 0xbd, 0xeb, 0xb3, 0xe7, 0x93, 0x3a, 0xaa, 0xd8, 0x35, 0xc6, 0x5a }),
            new BigInteger(new byte[] {0x51, 0x25, 0x63, 0xfc, 0xc2, 0xca, 0xb9, 0xf3, 0x84, 0x9e, 0x17, 0xa7, 0xad, 0xfa, 0xe6, 0xbc,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0 }),
            new BigInt {
                _bits = new uint[] { 0xd898c296, 0xf4a13945, 0x2deb33a0, 0x77037d81, 0x63a440f2, 0xf8bce6e5, 0xe12c4247, 0x6b17d1f2 }
            },
            new BigInt {
                _bits = new uint[] { 0x37bf51f5, 0xcbb64068, 0x6b315ece, 0x2bce3357, 0x7c0f9e16, 0x8ee7eb4a, 0xfe1a7f9b, 0x4fe342e2 }
            },
            v => { v.ModP256(); return v; },
            256,
            new byte[] { 0x06, 0x08, 0x2a, 0x86, 0x48, 0xce, 0x3d, 0x03, 0x01, 0x07 }
        );

        internal static readonly EllipticCurve P384 = new EllipticCurve(
            new BigInt {
                _bits = new uint[] {
                    0xffffffff, 0, 0, 0xffffffff, 0xfffffffe, 0xffffffff,
                    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff
                }
            },
            new BigInteger(new byte[] {0xef, 0x2a, 0xec, 0xd3, 0xed, 0xc8, 0x85, 0x2a, 0x9d, 0xd1, 0x2e, 0x8a,
                0x8d, 0x39, 0x56, 0xc6, 0x5a, 0x87, 0x13, 0x50, 0x8f, 0x08, 0x14, 0x03,
                0x12, 0x41, 0x81, 0xfe, 0x6e, 0x9c, 0x1d, 0x18, 0x19, 0x2d, 0xf8, 0xe3,
                0x6b, 0x05, 0x8e, 0x98, 0xe4, 0xe7, 0x3e, 0xe2, 0xa7, 0x2f, 0x31, 0xb3}),
            new BigInteger(new byte[] { 0x73, 0x29, 0xc5, 0xcc, 0x6a, 0x19, 0xec, 0xec, 0x7a, 0xa7, 0xb0, 0x48, 0xb2, 0x0d, 0x1a, 0x58, 0xdf, 0x2d, 0x37, 0xf4, 0x81, 0x4d, 0x63, 0xc7,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0 }),
            new BigInt {
                _bits = new uint[] {
                    0x72760ab7, 0x3a545e38, 0xbf55296c, 0x5502f25d, 0x82542a38, 0x59f741e0,
                    0x8ba79b98, 0x6e1d3b62, 0xf320ad74, 0x8eb1c71e, 0xbe8b0537, 0xaa87ca22
                }
            },
            new BigInt {
                _bits = new uint[] {
                    0x90ea0e5f, 0x7a431d7c, 0x1d7e819d, 0x0a60b1ce, 0xb5f0b8c0, 0xe9da3113,
                    0x289a147c, 0xf8f41dbd, 0x9292dc29, 0x5d9e98bf, 0x96262c6f, 0x3617de4a
                }
            },
            v => { v.ModP384(); return v; },
            384,
            new byte[] { 0x06, 0x05, 0x2B, 0x81, 0x04, 0x00, 0x22 }
        );

        internal static readonly EllipticCurve P521 = new EllipticCurve(
            new BigInt {
                _bits = new uint[] {
                    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff,
                    0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0xffffffff, 0x000001ff
                }
            },
            new BigInteger(new byte[] {
                0x00, 0x3f, 0x50, 0x6b, 0xd4, 0x1f, 0x45, 0xef, 0xf1, 0x34, 0x2c, 0x3d, 0x88, 0xdf, 0x73, 0x35, 0x07, 0xbf, 0xb1, 0x3b,
                0xbd, 0xc0, 0x52, 0x16, 0x7b, 0x93, 0x7e, 0xec, 0x51, 0x39, 0x19, 0x56, 0xe1, 0x09, 0xf1, 0x8e, 0x91, 0x89, 0xb4, 0xb8, 0xf3, 0x15, 0xb3, 0x99,
                0x5b, 0x72, 0xda, 0xa2, 0xee, 0x40, 0x85, 0xb6, 0xa0, 0x21, 0x9a, 0x92, 0x1f, 0x9a, 0x1c, 0x8e, 0x61, 0xb9, 0x3e, 0x95, 0x51 }),
            new BigInteger(new byte[] { 0x09, 0x64, 0x38, 0x91, 0x1e, 0xb7, 0x6f, 0xbb, 0xae, 0x47, 0x9c, 0x89, 0xb8, 0xc9, 0xb5,
                0x3b, 0xd0, 0xa5, 0x09, 0xf7, 0x48, 0x01, 0xcc, 0x7f, 0x6b, 0x96, 0x2f, 0xbf, 0x83, 0x87, 0x86, 0x51, 0xfa, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
                0xff, 0x01
            }),
            new BigInt {
                _bits = new uint[] {
                    0xc2e5bd66, 0xf97e7e31, 0x856a429b, 0x3348b3c1, 0xa2ffa8de, 0xfe1dc127, 0xefe75928, 0xa14b5e77,
                    0x6b4d3dba, 0xf828af60, 0x053fb521, 0x9c648139, 0x2395b442, 0x9e3ecb66, 0x0404e9cd, 0x858e06b7, 0x000000c6
                }
            },
            new BigInt {
                _bits = new uint[] {
                    0x9fd16650, 0x88be9476, 0xa272c240, 0x353c7086, 0x3fad0761, 0xc550b901, 0x5ef42640, 0x97ee7299,
                    0x273e662c, 0x17afbd17, 0x579b4468, 0x98f54449, 0x2c7d1bd9, 0x5c8a5fb4, 0x9a3bc004, 0x39296a78, 0x00000118
                },
            },
            v => { v.ModP521(); return v; },
            521,
            new byte[] { 0x06, 0x05, 0x2B, 0x81, 0x04, 0x00, 0x23 }
        );

        internal readonly BigInt p;
        BigInteger b;
        BigInteger q;
        BigInt negP;
        internal readonly BigInt xg;
        internal readonly BigInt yg;
        Func<BigInt, BigInt> modp;
        internal readonly int curveLen;
        internal int curveByteLen => (curveLen + 7) >> 3;
        internal readonly byte[] asnName;

        EllipticCurve(BigInt p, BigInteger b, BigInteger q, BigInt xg, BigInt yg, Func<BigInt, BigInt> modp, int curveLen, byte[] asnName)
        {
            this.p = p;
            this.b = b;
            this.q = q;
            this.xg = xg;
            this.yg = yg;
            this.modp = modp;
            this.curveLen = curveLen;
            this.asnName = asnName;

            negP = p.Clone();
            BigInt.TwosComplement(negP._bits, negP._bits);
        }

        internal class BigInt
        {
            internal uint[] _bits;

            internal int Length => _bits.Length;

            internal BigInt(uint[] bits)
            {
                _bits = (uint[])bits.Clone();
            }
            internal BigInt(byte[] bigEndian, int offset, int byteLen)
            {
                var chunks = (byteLen + 3) / 4;
                _bits = new uint[chunks];
                for (int i = 0, shift = 0; i < byteLen; i++)
                {
                    _bits[i >> 2] |= (uint)bigEndian[offset + (byteLen - 1 - i)] << shift;
                    shift = (shift + 8) & 31;
                }
            }
            internal BigInt(BigInteger bi, int size)
            {
                var littleEndian = bi.ToByteArray();
                _bits = new uint[size];
                for (var i = 0; i < littleEndian.Length; i++)
                {
                    if (littleEndian[i] != 0)
                        _bits[i >> 2] |= (uint)littleEndian[i] << ((i & 3) << 3);
                }

            }
            internal BigInt(uint i, int size)
            {
                _bits = new uint[size];
                _bits[0] = i;
            }
            internal BigInt() { }

            internal void Clear()
            {
                for (var i = 0; i < _bits.Length; i++)
                    _bits[i] = 0;
            }

            internal static BigInt Create(int size)
            {
                return new BigInt { _bits = new uint[size] };
            }
            internal BigInt Clone()
            {
                return new BigInt { _bits = (uint[])_bits.Clone() };
            }

            void Truncate(int size)
            {
                var bits = new uint[size];
                for (var i = 0; i < size; i++)
                    bits[i] = _bits[i];
                Clear();
                _bits = bits;
            }

            internal byte[] ExportToBigEndian(int byteLen)
            {
                var output = new byte[byteLen];
                ExportToBigEndian(output, 0, byteLen);
                return output;
            }
            internal int ExportToBigEndian(byte[] buf, int offset, int byteLen)
            {
                for (int i = 0, shift = 0; i < byteLen; i++)
                {
                    var pos = offset + (byteLen - 1 - i);
                    buf[pos] = (byte)((_bits[i >> 2] >> shift) & 0xff);
                    shift = (shift + 8) & 31;
                }
                return byteLen;
            }
            internal BigInteger ExportToBigInteger()
            {
                var bytes = new byte[_bits.Length * 4 + 1];
                for (int i = 0, j = 0; i < _bits.Length; i++, j += 4)
                {
                    bytes[j] = (byte)_bits[i];
                    bytes[j + 1] = (byte)(_bits[i] >> 8);
                    bytes[j + 2] = (byte)(_bits[i] >> 16);
                    bytes[j + 3] = (byte)(_bits[i] >> 24);
                }
                return new BigInteger(bytes);
            }

            internal bool IsZero()
            {
                for (var i = 0; i < Length; i++)
                    if (_bits[i] != 0)
                        return false;
                return true;
            }
            internal bool IsOne()
            {
                if (_bits[0] != 1)
                    return false;
                for (var i = 1; i < Length; i++)
                    if (_bits[i] != 0)
                        return false;
                return true;
            }

            internal bool IsEven()
            {
                return (_bits[0] & 1) == 0;
            }

            internal int BitCount()
            {
                for (var i = Length - 1; i >= 0; i--)
                {
                    if (_bits[i] != 0)
                    {
                        for (var j = 31; j >= 0; j--)
                        {
                            if ((_bits[i] & (1U << j)) != 0)
                            {
                                return i * 32 + j + 1;
                            }
                        }
                    }
                }
                return 0;
            }
            internal bool BitAt(int pos)
            {
                if (pos >= Length << 5 || pos < 0)
                    return false;
                return (_bits[pos >> 5] & (1 << (pos & 31))) != 0;
            }

            internal uint this[int i]
            {
                get => _bits[i];
                set => _bits[i] = value;
            }

            public static bool operator >=(BigInt a, BigInt o)
            {
                Debug.Assert(a.Length == o.Length + 1 || a.Length == o.Length);

                if (a.Length == o.Length + 1 && a._bits[a.Length - 1] != 0)
                    return true;

                for (var i = o.Length - 1; i >= 0; i--) {
                    if (a._bits[i] > o._bits[i])
                        return true;
                    if (a._bits[i] < o._bits[i])
                        return false;
                }
                return true;
            }

            public static bool operator <=(BigInt a, BigInt o) => o >= a;

            // r can be the same array as a or o
            // a.Length <= o.Length (if a.Length < o.Length, the rest is ignored)
            // r.Length >= a.Length
            internal static uint AddRaw(uint[] a, uint[] o, uint[] r)
            {
                uint carry = 0;
                for (var i = 0; i < a.Length; i++)
                {
                    var add = (ulong)a[i] + o[i] + carry;
                    r[i] = (uint)add;
                    carry = (uint)(add >> 32);
                }
                return carry;
            }

            // output can be input
            internal static void TwosComplement(uint[] input, uint[] output)
            {
                var carry = true;
                for (var i = 0; i < input.Length; i++)
                {
                    output[i] = ~input[i];
                    if (carry)
                    {
                        if (output[i] == uint.MaxValue)
                        {
                            output[i] = 0;
                        }
                        else
                        {
                            output[i] += 1;
                            carry = false;
                        }
                    }
                }
            }

            public static BigInt operator -(BigInt i) {
                var res = i.Clone();
                BigInt.TwosComplement(res._bits, res._bits);
                return res;
            }

            internal BigInt Add(BigInt o)
            {
                var r = Create(_bits.Length + 1);
                var carry = AddRaw(_bits, o._bits, r._bits);
                r._bits[_bits.Length] = carry;

                return r;
            }

            public static BigInt operator +(BigInt a, BigInt o) => a.Add(o);

            internal BigInt AddMod(BigInt o, BigInt m, BigInt negM)
            {
                var r = this + o;
                if (r >= m)
                {
                    var r2 = negM + r;
                    r.Clear();
                    r = r2;
                }
                r.Truncate(Length);
                return r;
            }

            internal BigInt SubMod(BigInt o, BigInt m, BigInt negM)
            {
                var negO = -o;
                AddRaw(negO._bits, m._bits, negO._bits);
                var res = AddMod(negO, m, negM);
                negO.Clear();
                return res;
            }

            internal BigInt Mul(BigInt o)
            {
                var ret = Create(Length * 2);
                for (var i = 0; i < Length; i++)
                {
                    uint carry = 0;
                    for (var j = 0; j < Length; j++)
                    {
                        var mul = (ulong)_bits[i] * o._bits[j] + ret._bits[i + j] + carry;
                        carry = (uint)(mul >> 32);
                        ret._bits[i + j] = (uint)mul;
                    }
                    ret._bits[Length + i] = carry;
                }
                return ret;
            }

            public static BigInt operator *(BigInt a, BigInt o) => a.Mul(o);

            internal BigInt Mul3()
            {
                var tmp = Add(this);
                tmp._bits[tmp.Length - 1] += BigInt.AddRaw(_bits, tmp._bits, tmp._bits);
                return tmp;
            }
            internal BigInt Mul3Mod(BigInt m, BigInt negM)
            {
                var tmp1 = AddMod(this, m, negM);
                var tmp2 = AddMod(tmp1, m, negM);
                tmp1.Clear();
                return tmp2;
            }
            internal BigInt Mul4Mod(BigInt m, BigInt negM)
            {
                var tmp1 = AddMod(this, m, negM);
                var tmp2 = tmp1.AddMod(tmp1, m, negM);
                tmp1.Clear();
                return tmp2;
            }
            internal BigInt Mul8Mod(BigInt m, BigInt negM)
            {
                var tmp1 = Mul4Mod(m, negM);
                var tmp2 = tmp1.AddMod(tmp1, m, negM);
                tmp1.Clear();
                return tmp2;
            }
            internal void ModP256()
            {
                var p = EllipticCurve.P256.p;
                var negP = EllipticCurve.P256.negP;

                var a = _bits;
                var t = new BigInt { _bits = new uint[] { a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7] } };
                var s1 = new BigInt { _bits = new uint[] { 0, 0, 0, a[11], a[12], a[13], a[14], a[15] } };
                var s2 = new BigInt { _bits = new uint[] { 0, 0, 0, a[12], a[13], a[14], a[15], 0 } };
                var s3 = new BigInt { _bits = new uint[] { a[8], a[9], a[10], 0, 0, 0, a[14], a[15] } };
                var s4 = new BigInt { _bits = new uint[] { a[9], a[10], a[11], a[13], a[14], a[15], a[13], a[8] } };
                var d1 = new BigInt { _bits = new uint[] { a[11], a[12], a[13], 0, 0, 0, a[8], a[10] } };
                var d2 = new BigInt { _bits = new uint[] { a[12], a[13], a[14], a[15], 0, 0, a[9], a[11] } };
                var d3 = new BigInt { _bits = new uint[] { a[13], a[14], a[15], a[8], a[9], a[10], 0, a[12] } };
                var d4 = new BigInt { _bits = new uint[] { a[14], a[15], 0, a[9], a[10], a[11], 0, a[13] } };

                var extraAddD1 = d1 >= p;
                BigInt.TwosComplement(d1._bits, d1._bits);
                BigInt.AddRaw(d1._bits, p._bits, d1._bits);
                if (extraAddD1)
                    BigInt.AddRaw(d1._bits, p._bits, d1._bits);

                var extraAddD2 = d2 >= p;
                BigInt.TwosComplement(d2._bits, d2._bits);
                BigInt.AddRaw(d2._bits, p._bits, d2._bits);
                if (extraAddD2)
                    BigInt.AddRaw(d2._bits, p._bits, d2._bits);

                BigInt.TwosComplement(d3._bits, d3._bits);
                BigInt.AddRaw(d3._bits, p._bits, d3._bits);
                BigInt.TwosComplement(d4._bits, d4._bits);
                BigInt.AddRaw(d4._bits, p._bits, d4._bits);

                var res = BigInt.Create(8);
                var toAdd = new BigInt[] { t, s1, s1, s2, s2, s3, s4, d1, d2, d3, d4 };
                foreach (var num in toAdd)
                {
                    var carry = BigInt.AddRaw(num._bits, res._bits, res._bits) == 1;
                    if (carry || res >= p)
                    {
                        BigInt.AddRaw(res._bits, negP._bits, res._bits);
                    }
                }
                foreach (var num in toAdd)
                {
                    num.Clear();
                }
                Clear();
                _bits = res._bits;
            }

            internal void ModP521()
            {
                var a = _bits;
                var t = BigInt.Create(17);
                var s = BigInt.Create(17);
                for (var i = 0; i < 16; i++)
                {
                    t[i] = a[i];
                }
                t[16] = a[16] & 0x1ff;
                for (var i = 0; i < 16; i++)
                {
                    s[i] = (a[16 + i] >> 9) | (a[17 + i] << 23);
                }
                s[16] = a[32] >> 9;
                var res = t.AddMod(s, EllipticCurve.P521.p, EllipticCurve.P521.negP);
                t.Clear();
                s.Clear();
                Clear();
                _bits = res._bits;
            }

            internal void ModP384()
            {
                var p = EllipticCurve.P384.p;
                var negP = EllipticCurve.P384.negP;

                var a = _bits;
                var t = new BigInt { _bits = new uint[] { a[0], a[1], a[2], a[3], a[4], a[5], a[6], a[7], a[8], a[9], a[10], a[11] } };
                var s1 = new BigInt { _bits = new uint[] { 0, 0, 0, 0, a[21], a[22], a[23], 0, 0, 0, 0, 0 } };
                var s2 = new BigInt { _bits = new uint[] { a[12], a[13], a[14], a[15], a[16], a[17], a[18], a[19], a[20], a[21], a[22], a[23] } };
                var s3 = new BigInt { _bits = new uint[] { a[21], a[22], a[23], a[12], a[13], a[14], a[15], a[16], a[17], a[18], a[19], a[20] } };
                var s4 = new BigInt { _bits = new uint[] { 0, a[23], 0, a[20], a[12], a[13], a[14], a[15], a[16], a[17], a[18], a[19] } };
                var s5 = new BigInt { _bits = new uint[] { 0, 0, 0, 0, a[20], a[21], a[22], a[23], 0, 0, 0, 0 } };
                var s6 = new BigInt { _bits = new uint[] { a[20], 0, 0, a[21], a[22], a[23], 0, 0, 0, 0, 0, 0 } };
                var d1 = new BigInt { _bits = new uint[] { a[23], a[12], a[13], a[14], a[15], a[16], a[17], a[18], a[19], a[20], a[21], a[22] } };
                var d2 = new BigInt { _bits = new uint[] { 0, a[20], a[21], a[22], a[23], 0, 0, 0, 0, 0, 0, 0 } };
                var d3 = new BigInt { _bits = new uint[] { 0, 0, 0, a[23], a[23], 0, 0, 0, 0, 0, 0, 0 } };

                BigInt.TwosComplement(d1._bits, d1._bits);
                BigInt.AddRaw(d1._bits, p._bits, d1._bits);

                BigInt.TwosComplement(d2._bits, d2._bits);
                BigInt.AddRaw(d2._bits, s2._bits, s2._bits);

                BigInt.TwosComplement(d3._bits, d3._bits);
                BigInt.AddRaw(d3._bits, s2._bits, s2._bits);

                var res = s1;
                BigInt.AddRaw(res._bits, res._bits, res._bits);
                BigInt.AddRaw(res._bits, s5._bits, res._bits);
                BigInt.AddRaw(res._bits, s6._bits, res._bits);

                var toAdd = new BigInt[] { t, s2, s3, s4, d1 };
                foreach (var num in toAdd)
                {
                    var carry = BigInt.AddRaw(num._bits, res._bits, res._bits) == 1;
                    if (carry || res >= p)
                    {
                        BigInt.AddRaw(res._bits, negP._bits, res._bits);
                    }
                }
                s5.Clear();
                s6.Clear();
                d2.Clear();
                d3.Clear();
                foreach (var num in toAdd)
                {
                    num.Clear();
                }
                Clear();
                _bits = res._bits;
            }

            internal void Div2Trunc()
            {
                for (var i = 0; i < Length - 1; i++)
                {
                    _bits[i] = (_bits[i] >> 1) | (_bits[i + 1] << 31);
                }
                _bits[Length - 1] >>= 1;
            }
            internal void Div2(BigInt p)
            {
                if ((_bits[0] & 1) == 0)
                {
                    Div2Trunc();
                }
                else
                {
                    var carry = AddRaw(_bits, p._bits, _bits);
                    Div2Trunc();
                    _bits[Length - 1] |= carry << 31;
                }
            }
            internal BigInt ModInv(BigInt p, BigInt negP)
            {
                var u = Clone();
                var v = p.Clone();
                var x1 = new BigInt(1, p.Length);
                var x2 = new BigInt(0, p.Length);
                while (!u.IsOne() && !v.IsOne())
                {
                    while (u.IsEven())
                    {
                        u.Div2Trunc();
                        x1.Div2(p);
                    }
                    while (v.IsEven())
                    {
                        v.Div2Trunc();
                        x2.Div2(p);
                    }
                    if (u >= v)
                    {
                        var uTmp = u;
                        u = u.SubMod(v, p, negP);
                        uTmp.Clear();
                        var x1Tmp = x1;
                        x1 = x1.SubMod(x2, p, negP);
                        x1Tmp.Clear();
                    }
                    else
                    {
                        var vTmp = v;
                        v = v.SubMod(u, p, negP);
                        vTmp.Clear();
                        var x2Tmp = x2;
                        x2 = x2.SubMod(x1, p, negP);
                        x2Tmp.Clear();
                    }
                }
                v.Clear();
                if (u.IsOne())
                {
                    u.Clear();
                    x2.Clear();
                    return x1;
                }
                else
                {
                    u.Clear();
                    x1.Clear();
                    return x2;
                }
            }
        }

        internal class Projective
        {
            internal BigInt x, y, z;
            internal void Clear()
            {
                x.Clear();
                y.Clear();
                z.Clear();
            }
            internal Projective Clone()
            {
                return new Projective() { x = x.Clone(), y = y.Clone(), z = z.Clone() };
            }
        }

        internal class Affine
        {
            internal BigInt x, y;
            internal void Clear()
            {
                x.Clear();
                y.Clear();
            }
        }

        internal Projective EcDouble(Projective s)
        {
            var size = s.x.Length;

            var t1 = s.x;
            var t2 = s.y;
            var t3 = s.z;
            if (t3.IsZero())
            {
                return new Projective() { x = new BigInt(1, size), y = new BigInt(1, size), z = new BigInt(0, size) };
            }
            var t4 = modp(t3 * t3);
            var t5 = t1.SubMod(t4, p, negP);
            var t4_2 = t1.AddMod(t4, p, negP);
            var t5_2 = modp(t4_2 * t5);
            var t4_3 = t5_2.Mul3Mod(p, negP);
            var t3_2 = modp(t3 * t2);
            var t3_3 = t3_2.AddMod(t3_2, p, negP);
            var t2_2 = modp(t2 * t2);
            var t5_3 = modp(t1 * t2_2);
            var t5_4 = t5_3.Mul4Mod(p, negP);
            var t1_2 = modp(t4_3 * t4_3);
            var t1_3tmp = t1_2.SubMod(t5_4, p, negP);
            var t1_3 = t1_3tmp.SubMod(t5_4, p, negP);
            var t2_3 = modp(t2_2 * t2_2);
            var t2_4 = t2_3.Mul8Mod(p, negP);
            var t5_5 = t5_4.SubMod(t1_3, p, negP);
            var t5_6 = modp(t4_3 * t5_5);
            var t2_5 = t5_6.SubMod(t2_4, p, negP);
            t4.Clear();
            t5.Clear();
            t4_2.Clear();
            t5_2.Clear();
            t4_3.Clear();
            t3_2.Clear();
            t2_2.Clear();
            t5_3.Clear();
            t5_4.Clear();
            t1_2.Clear();
            t1_3tmp.Clear();
            t2_3.Clear();
            t2_4.Clear();
            t5_5.Clear();
            t5_6.Clear();
            return new Projective() { x = t1_3, y = t2_5, z = t3_3 };
        }

        BigInt Replace(BigInt old, BigInt new_)
        {
            old.Clear();
            return new_;
        }
        void Assign(ref BigInt variable, BigInt new_)
        {
            variable = Replace(variable, new_);
        }

        internal Projective EcAdd(Projective s, Projective t)
        {
            var size = s.x.Length;

            var t1 = s.x.Clone();
            var t2 = s.y.Clone();
            var t3 = s.z.Clone();
            var t4 = t.x;
            var t5 = t.y;
            var t6 = t.z;
            if (!t.z.IsOne())
            {
                var t7_ = modp(t6 * t6);
                Assign(ref t1, modp(t1 * t7_));
                Assign(ref t7_, modp(t6 * t7_));
                Assign(ref t2, modp(t2 * t7_));
                t7_.Clear();
            }
            var t7 = modp(t3 * t3);
            t4 = modp(t4 * t7);
            Assign(ref t7, modp(t3 * t7));
            t5 = modp(t5 * t7);
            Assign(ref t4, t1.SubMod(t4, p, negP));
            Assign(ref t5, t2.SubMod(t5, p, negP));
            if (t4.IsZero())
            {
                t1.Clear();
                t2.Clear();
                t3.Clear();
                t4.Clear();
                t7.Clear();
                if (t5.IsZero())
                {
                    return new Projective() { x = new BigInt(0, size), y = new BigInt(0, size), z = new BigInt(0, size) };
                }
                else
                {
                    t5.Clear();
                    return new Projective() { x = new BigInt(1, size), y = new BigInt(1, size), z = new BigInt(0, size) };
                }
            }
            Assign(ref t1, t1.AddMod(t1, p, negP));
            Assign(ref t1, t1.SubMod(t4, p, negP));
            Assign(ref t2, t2.AddMod(t2, p, negP));
            Assign(ref t2, t2.SubMod(t5, p, negP));
            if (!t.z.IsOne())
            {
                Assign(ref t3, modp(t3 * t6));
            }
            Assign(ref t3, modp(t3 * t4));
            Assign(ref t7, modp(t4 * t4));
            Assign(ref t4, modp(t4 * t7));
            Assign(ref t7, modp(t1 * t7));
            Assign(ref t1, modp(t5 * t5));
            Assign(ref t1, t1.SubMod(t7, p, negP));
            Assign(ref t7, t7.SubMod(t1, p, negP));
            Assign(ref t7, t7.SubMod(t1, p, negP));
            Assign(ref t5, modp(t5 * t7));
            Assign(ref t4, modp(t2 * t4));
            Assign(ref t2, t5.SubMod(t4, p, negP));
            t2.Div2(p);
            t4.Clear();
            t5.Clear();
            t7.Clear();
            return new Projective() { x = t1, y = t2, z = t3 };
        }

        internal Projective EcFullAdd(Projective s, Projective t)
        {
            if (s.z.IsZero())
            {
                return t.Clone();
            }
            if (t.z.IsZero())
            {
                return s.Clone();
            }
            var r = EcAdd(s, t);
            if (r.x.IsZero() && r.y.IsZero() && r.z.IsZero())
            {
                return EcDouble(s);
            }
            return r;
        }

        internal Projective EcFullSub(Projective s, Projective t)
        {
            var u = new Projective() { x = t.x.Clone(), y = p.SubMod(t.y, p, negP), z = t.z.Clone() };
            var res = EcFullAdd(s, u);
            u.Clear();
            return res;
        }

        internal Affine EcAffinify(Projective s)
        {
            Debug.Assert(!s.z.IsZero());

            var lambda = s.z.ModInv(p, negP);
            var lambda2 = modp(lambda * lambda);
            var lambda3 = modp(lambda2 * lambda);

            var res = new Affine() { x = modp(lambda2 * s.x), y = modp(lambda3 * s.y) };
            lambda.Clear();
            lambda2.Clear();
            lambda3.Clear();
            return res;
        }

        internal Projective EcProjectify(Affine s)
        {
            return new Projective() { x = s.x, y = s.y, z = new BigInt(1, s.x.Length) };
        }

        internal Projective EcMult(BigInt d, Projective s)
        {
            var size = s.x.Length;

            if (d.IsZero() || s.z.IsZero())
            {
                return new Projective() { x = new BigInt(1, size), y = new BigInt(1, size), z = new BigInt(0, size) };
            }
            if (d.IsOne())
            {
                return s.Clone();
            }
            if (!s.z.IsOne())
            {
                var affine = EcAffinify(s);
                s = EcProjectify(affine);
                affine.Clear();
            }
            else
            {
                s = s.Clone();
            }
            var r = s.Clone();
            var h = d.Mul3();

            for (var i = h.BitCount() - 2; i >= 1; i--)
            {
                var rTmp = r;
                r = EcDouble(r);
                rTmp.Clear();

                if (h.BitAt(i) && !d.BitAt(i))
                {
                    var u = EcFullAdd(r, s);
                    r.Clear();
                    r = u;
                }
                else if (!h.BitAt(i) && d.BitAt(i))
                {
                    var u = EcFullSub(r, s);
                    r.Clear();
                    r = u;
                }
            }
            h.Clear();

            return r;
        }

        internal Projective EcTwinMult(BigInt d0, Projective S, BigInt d1, Projective T)
        {
            var d = new BigInt[2] { d0, d1 };

            var SpT = EcFullAdd(S, T);
            var SmT = EcFullSub(S, T);

            var m0 = d0.BitCount();
            var m1 = d1.BitCount();
            var m = Math.Max(m0, m1);

            var c = new int[2][];
            c[0] = new int[6] { 0, 0, d0.BitAt(m - 1) ? 1 : 0, d0.BitAt(m - 2) ? 1 : 0, d0.BitAt(m - 3) ? 1 : 0, d0.BitAt(m - 4) ? 1 : 0 };
            c[1] = new int[6] { 0, 0, d1.BitAt(m - 1) ? 1 : 0, d1.BitAt(m - 2) ? 1 : 0, d1.BitAt(m - 3) ? 1 : 0, d1.BitAt(m - 4) ? 1 : 0 };

            var R = new Projective() { x = new BigInt(1, S.x.Length), y = new BigInt(1, S.x.Length), z = new BigInt(0, S.x.Length) };

            int[] h = new int[2], u = new int[2];

            for (var k = m; k >= 0; k--)
            {
                for (var i = 0; i <= 1; i++)
                {
                    h[i] = (c[i][1] << 4) + (c[i][2] << 3) + (c[i][3] << 2) + (c[i][4] << 1) + c[i][5];
                    if (c[i][0] == 1)
                        h[i] = 31 - h[i];
                }
                for (var i = 0; i <= 1; i++)
                {
                    var t = h[1 - i];
                    int f;
                    if (18 <= t && t < 22)
                        f = 9;
                    else if (14 <= t && t < 18)
                        f = 10;
                    else if (22 <= t && t < 24)
                        f = 11;
                    else if (4 <= t && t < 12)
                        f = 14;
                    else
                        f = 12;
                    if (h[i] < f)
                        u[i] = 0;
                    else
                        u[i] = (c[i][0] & 1) != 0 ? -1 : 1;
                }
                for (var i = 0; i < 2; i++)
                {
                    c[i][0] = ((u[i] != 0) ^ (c[i][1] != 0)) ? 1 : 0;
                    c[i][1] = c[i][2];
                    c[i][2] = c[i][3];
                    c[i][3] = c[i][4];
                    c[i][4] = c[i][5];
                    c[i][5] = d[i].BitAt(k - 5) ? 1 : 0;
                }
                    R = EcDouble(R);
                if (u[0] == -1)
                {
                    if (u[1] == -1)
                        R = EcFullSub(R, SpT);
                    else if (u[1] == 0)
                        R = EcFullSub(R, S);
                    else
                        R = EcFullSub(R, SmT);
                }
                else if (u[0] == 0)
                {
                    if (u[1] == -1)
                        R = EcFullSub(R, T);
                    else if (u[1] == 1)
                        R = EcFullAdd(R, T);
                }
                else if (u[0] == 1)
                {
                    if (u[1] == -1)
                        R = EcFullAdd(R, SmT);
                    else if (u[1] == 0)
                        R = EcFullAdd(R, S);
                    else
                        R = EcFullAdd(R, SpT);
                }
            }

            return R;
        }

        internal BigInt GenPriv(RandomNumberGenerator rng)
        {
            EllipticCurve.BigInt priv = null;
            do
            {
                if (priv != null)
                    priv.Clear();
                var bytes = new byte[curveByteLen];
                rng.GetBytes(bytes);
                var byteMask = (1 << (curveLen & 7)) - 1;
                if (byteMask != 0)
                    bytes[0] &= (byte)byteMask;
                priv = new EllipticCurve.BigInt(bytes, 0, bytes.Length);
                Utils.ClearArray(bytes);
            } while (priv >= p || priv.IsZero());
            return priv;
        }

        internal static EllipticCurve GetCurveFromParameters(byte[] pkParameters)
        {
            if (pkParameters.SequenceEqual(EllipticCurve.P256.asnName))
            {
                return EllipticCurve.P256;
            }
            else if (pkParameters.SequenceEqual(EllipticCurve.P384.asnName))
            {
                return EllipticCurve.P384;
            }
            else if (pkParameters.SequenceEqual(EllipticCurve.P521.asnName))
            {
                return EllipticCurve.P521;
            }
            else
            {
                return null;
            }
        }

        internal void Ecdh(BigInt publicX, BigInt publicY, RandomNumberGenerator rng, out byte[] preMasterSecret, out Affine publicPoint)
        {
            var limbLen = (curveByteLen + 3) >> 2;

            var priv = GenPriv(rng);

            var serverPublic = new EllipticCurve.Projective() { x = publicX, y = publicY, z = new EllipticCurve.BigInt(1, limbLen) };
            var commonSecretProj = EcMult(priv, serverPublic);
            var commonSecretAff = EcAffinify(commonSecretProj);
            serverPublic.Clear();
            commonSecretProj.Clear();

            var g = new EllipticCurve.Projective() { x = xg, y = yg, z = new EllipticCurve.BigInt(1, limbLen) };
            var pubProj = EcMult(priv, g);
            var pubAff = EcAffinify(pubProj);
            priv.Clear();
            pubProj.Clear();

            preMasterSecret = commonSecretAff.x.ExportToBigEndian(curveByteLen);
            commonSecretAff.Clear();

            publicPoint = pubAff;
        }

        internal static bool? VerifySignature(byte[] pkParameters, byte[] pkKey, byte[] hash, byte[] signature)
        {
            var curve = GetCurveFromParameters(pkParameters);
            if (curve == null)
                return null;

            // We must decode the signature from DER format to raw format (to coordinates on the curve)
            var offset = 1; // Skip tag 0x30
            Utils.GetASNLength(signature, ref offset); // P521 requires 2 bytes to specify the length
            offset += 1; // 0x02
            var len1 = Utils.GetASNLength(signature, ref offset);

            var rBytes = new byte[len1];
            for (var i = 0; i < len1; i++)
                rBytes[i] = signature[offset + len1 - 1 - i];

            offset += len1;
            offset += 1; // 0x02
            var len2 = Utils.GetASNLength(signature, ref offset);

            var sBytes = new byte[len2];
            for (var i = 0; i < len2; i++)
                sBytes[i] = signature[offset + len2 - 1 - i];

            var r = new BigInteger(rBytes);
            var s = new BigInteger(sBytes);

            // Verify that r, s are in [1, q - 1] and Qa lies on the curve

            if (r == 0 || s == 0)
                return false;
            if (r >= curve.q || s >= curve.q)
                return false;

            BigInt Qax = new BigInt(pkKey, 1, curve.curveByteLen), Qay = new BigInt(pkKey, 1 + curve.curveByteLen, curve.curveByteLen);
            BigInteger QaxBi = Qax.ExportToBigInteger(), QayBi = Qay.ExportToBigInteger();

            var pBi = curve.p.ExportToBigInteger();
            if (QaxBi >= pBi || QayBi >= pBi)
                return false;
            if ((QaxBi * QaxBi * QaxBi - 3 * QaxBi + curve.b + pBi * 3) % pBi != QayBi * QayBi % pBi)
                return false;

            // Data points are ok, now start verify the signature

            var hashLen = hash.Length;
            byte[] hash2 = null;
            if (hash[0] >= 128)
            {
                hash2 = new byte[hash.Length + 1];
                hash2[hash.Length] = 0;
                for (var i = 0; i < hash.Length; i++)
                    hash2[i] = hash[hash.Length - 1 - i];
            }
            else
            {
                Array.Reverse(hash);
                hash2 = hash;
            }
            var z = new BigInteger(hash2);
            if (hashLen * 8 > curve.curveLen) // Should be compared to order length, but p length is the same for NIST curves
            {
                var excessBits = hashLen * 8 - curve.curveLen;
                z >>= excessBits;
            }
            var w = BigInteger.ModPow(s, curve.q - 2, curve.q);
            var u1 = z * w % curve.q;
            var u2 = r * w % curve.q;

            var mul = curve.EcTwinMult(new BigInt(u1, (curve.curveByteLen + 3) / 4), new Projective() { x = curve.xg, y = curve.yg, z = new BigInt(1, (curve.curveByteLen + 3) / 4) },
                new BigInt(u2, (curve.curveByteLen + 3) / 4), new Projective() { x = Qax, y = Qay, z = new BigInt(1, (curve.curveByteLen + 3) / 4) });
            var res = curve.EcAffinify(mul).x.ExportToBigInteger();

            if (res >= curve.q)
                res -= curve.q;

            var ok = res == r;

            return ok;
        }

#if OPTIMIZED_CRYPTOGRAPHY && !NETSTANDARD1_3
        internal static bool? VerifySignatureCng(byte[] pkParameters, byte[] pkKey, byte[] hash, byte[] signature)
        {
            EllipticCurve curve = null;
            var ecsngHeader = new byte[8] { (byte)'E', (byte)'C', (byte)'S', 0, 0, 0, 0, 0 };

            curve = GetCurveFromParameters(pkParameters);
            if (curve == null)
                return null;

            if (curve == EllipticCurve.P256)
                ecsngHeader[3] = (byte)'1';
            else if (curve == EllipticCurve.P384)
                ecsngHeader[3] = (byte)'3';
            else // P512
                ecsngHeader[3] = (byte)'5';

            ecsngHeader[4] = (byte)curve.curveByteLen;

            var pubKeyBlob = new byte[8 + curve.curveByteLen * 2];
            Buffer.BlockCopy(ecsngHeader, 0, pubKeyBlob, 0, 8);
            Buffer.BlockCopy(pkKey, 1, pubKeyBlob, 8, curve.curveByteLen * 2);
            var ecdsa = new ECDsaCng(CngKey.Import(pubKeyBlob, CngKeyBlobFormat.EccPublicBlob));

            // We must decode the signature from DER format to raw format (to coordinates on the curve)
            var decodedSignature = Utils.DecodeDERSignature(signature, 0, signature.Length, curve.curveByteLen);

            bool ok = ecdsa.VerifyHash(hash, decodedSignature);

            return ok;
        }
#endif
    }
}
