#if NET45 || NET451
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
    class MD5SHA1 : HashAlgorithm
    {
        MD5 _md5;
        SHA1 _sha1;

        public MD5SHA1()
        {
            _md5 = MD5.Create();
            _sha1 = SHA1.Create();
        }

        //
        // Summary:
        //     Releases the unmanaged resources used by the System.Security.Cryptography.HashAlgorithm
        //     and optionally releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only
        //     unmanaged resources.
        protected override void Dispose(bool disposing)
        {
            _md5.Dispose();
            _sha1.Dispose();
            if (HashValue != null)
                Array.Clear(HashValue, 0, 36);
            HashValue = null;
        }
        //
        // Summary:
        //     When overridden in a derived class, routes data written to the object into
        //     the hash algorithm for computing the hash.
        //
        // Parameters:
        //   array:
        //     The input to compute the hash code for.
        //
        //   ibStart:
        //     The offset into the byte array from which to begin using data.
        //
        //   cbSize:
        //     The number of bytes in the byte array to use as data.
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _md5.TransformBlock(array, ibStart, cbSize);
            _sha1.TransformBlock(array, ibStart, cbSize);
        }
        //
        // Summary:
        //     When overridden in a derived class, finalizes the hash computation after
        //     the last data is processed by the cryptographic stream object.
        //
        // Returns:
        //     The computed hash code.
        protected override byte[] HashFinal()
        {
            byte[] hash = new byte[36];
            _md5.TransformFinalBlock(hash, 0, 0);
            _sha1.TransformFinalBlock(hash, 0, 0);
            var md5Hash = _md5.Hash;
            var sha1Hash = _sha1.Hash;
            Buffer.BlockCopy(md5Hash, 0, hash, 0, 16);
            Buffer.BlockCopy(sha1Hash, 0, hash, 16, 20);
            Array.Clear(md5Hash, 0, 16);
            Array.Clear(sha1Hash, 0, 20);
            HashValue = hash;
            return hash;
        }
        //
        // Summary:
        //     Initializes an implementation of the System.Security.Cryptography.HashAlgorithm
        //     class.
        public override void Initialize()
        {
            _md5.Initialize();
            _sha1.Initialize();
        }
    }
}
#endif
