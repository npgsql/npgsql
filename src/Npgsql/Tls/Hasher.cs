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
    abstract class Hasher : IDisposable
    {
        internal static byte[] EmptyByteArray = new byte[0];

        public virtual void Initialize() {}
        public abstract void Update(byte[] arr, int offset, int len);
        public abstract byte[] Final();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {}

#if !(NET45 || NET451)
        public static HashAlgorithmName GetHashAlgorithmName(TlsHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case TlsHashAlgorithm.MD5: return HashAlgorithmName.MD5;
                case TlsHashAlgorithm.SHA1: return HashAlgorithmName.SHA1;
                case TlsHashAlgorithm.SHA256: return HashAlgorithmName.SHA256;
                case TlsHashAlgorithm.SHA384: return HashAlgorithmName.SHA384;
                case TlsHashAlgorithm.SHA512: return HashAlgorithmName.SHA512;
                default: throw new NotSupportedException();
            }
        }
#endif

        public static Hasher Create(TlsHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
#if NET45 || NET451
                case TlsHashAlgorithm.MD5: return new HashAlgorithmHasher(new MD5CryptoServiceProvider());
                case TlsHashAlgorithm.SHA1: return new HashAlgorithmHasher(new SHA1CryptoServiceProvider());
                case TlsHashAlgorithm.SHA256: return new HashAlgorithmHasher(new SHA256CryptoServiceProvider());
                case TlsHashAlgorithm.SHA384: return new HashAlgorithmHasher(new SHA384CryptoServiceProvider());
                case TlsHashAlgorithm.SHA512: return new HashAlgorithmHasher(new SHA512CryptoServiceProvider());
                case TlsHashAlgorithm.MD5SHA1: return new HashAlgorithmHasher(new MD5SHA1());
                default: throw new NotSupportedException();
#else
                case TlsHashAlgorithm.MD5: return new IncrementalHashHasher(HashAlgorithmName.MD5);
                case TlsHashAlgorithm.SHA1: return new IncrementalHashHasher(HashAlgorithmName.SHA1);
                case TlsHashAlgorithm.SHA256: return new IncrementalHashHasher(HashAlgorithmName.SHA256);
                case TlsHashAlgorithm.SHA384: return new IncrementalHashHasher(HashAlgorithmName.SHA384);
                case TlsHashAlgorithm.SHA512: return new IncrementalHashHasher(HashAlgorithmName.SHA512);
                case TlsHashAlgorithm.MD5SHA1: return new MD5SHA1IncrementalHasher();
                default: throw new NotSupportedException();
#endif
            }
        }

#if NET45 || NET451
        sealed class HashAlgorithmHasher : Hasher
        {
            readonly HashAlgorithm _hashAlgorithm;

            internal HashAlgorithmHasher(HashAlgorithm hashAlgorithm)
            {
                _hashAlgorithm = hashAlgorithm;
            }

            protected override void Dispose(bool disposing)
                => _hashAlgorithm.Dispose();

            public override byte[] Final()
            {
                _hashAlgorithm.TransformFinalBlock(EmptyByteArray, 0, 0);
                return _hashAlgorithm.Hash;
            }

            public override void Initialize()
            {
                _hashAlgorithm.Initialize();
            }

            public override void Update(byte[] arr, int offset, int len)
            {
                _hashAlgorithm.TransformBlock(arr, offset, len);
            }
        }
#else
        class IncrementalHashHasher : Hasher
        {
            readonly IncrementalHash _incrementalHash;

            internal IncrementalHashHasher(HashAlgorithmName hashAlgorithmName)
            {
                _incrementalHash = IncrementalHash.CreateHash(hashAlgorithmName);
            }

            protected override void Dispose(bool disposing)
                => _incrementalHash.Dispose();

            public override byte[] Final() => _incrementalHash.GetHashAndReset();

            public override void Update(byte[] arr, int offset, int len)
                => _incrementalHash.AppendData(arr, offset, len);
        }

        // ReSharper disable once InconsistentNaming
        class MD5SHA1IncrementalHasher : Hasher
        {
            readonly IncrementalHash _md5;
            readonly IncrementalHash _sha1;

            internal MD5SHA1IncrementalHasher()
            {
                _md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
                _sha1 = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
            }

            protected override void Dispose(bool disposing)
            {
                _md5.Dispose();
                _sha1.Dispose();
            }

            public override byte[] Final()
            {
                var hash = new byte[36];
                var md5Hash = _md5.GetHashAndReset();
                var sha1Hash = _sha1.GetHashAndReset();
                Buffer.BlockCopy(md5Hash, 0, hash, 0, 16);
                Buffer.BlockCopy(sha1Hash, 0, hash, 16, 20);
                Array.Clear(md5Hash, 0, 16);
                Array.Clear(sha1Hash, 0, 20);
                return hash;
            }

            public override void Update(byte[] arr, int offset, int len)
            {
                _md5.AppendData(arr, offset, len);
                _sha1.AppendData(arr, offset, len);
            }
        }
#endif
    }
}
