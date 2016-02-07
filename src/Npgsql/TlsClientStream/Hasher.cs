using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TlsClientStream
{
    internal abstract class Hasher : IDisposable
    {
        internal static byte[] EmptyByteArray = new byte[0];

        public abstract void Initialize();
        public abstract void Update(byte[] arr, int offset, int len);
        public abstract byte[] Final();
        public abstract void Dispose();

#if !(NET45 || NET451 || NET452 || DNX451)
        public static HashAlgorithmName GetHashAlgorithmName(TLSHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case TLSHashAlgorithm.SHA1: return HashAlgorithmName.SHA1;
                case TLSHashAlgorithm.SHA256: return HashAlgorithmName.SHA256;
                case TLSHashAlgorithm.SHA384: return HashAlgorithmName.SHA384;
                case TLSHashAlgorithm.SHA512: return HashAlgorithmName.SHA512;
                default: throw new NotSupportedException();
            }
        }
#endif

        public static Hasher Create(TLSHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
#if NET45 || NET451
                case TLSHashAlgorithm.SHA1: return new HashAlgorithmHasher(new SHA1CryptoServiceProvider());
                case TLSHashAlgorithm.SHA256: return new HashAlgorithmHasher(new SHA256CryptoServiceProvider());
                case TLSHashAlgorithm.SHA384: return new HashAlgorithmHasher(new SHA384CryptoServiceProvider());
                case TLSHashAlgorithm.SHA512: return new HashAlgorithmHasher(new SHA512CryptoServiceProvider());
                case TLSHashAlgorithm.MD5SHA1: return new HashAlgorithmHasher(new MD5SHA1());
                default: throw new NotSupportedException();
#else
                case TLSHashAlgorithm.SHA1: return new IncrementalHashHasher(HashAlgorithmName.SHA1);
                case TLSHashAlgorithm.SHA256: return new IncrementalHashHasher(HashAlgorithmName.SHA256);
                case TLSHashAlgorithm.SHA384: return new IncrementalHashHasher(HashAlgorithmName.SHA384);
                case TLSHashAlgorithm.SHA512: return new IncrementalHashHasher(HashAlgorithmName.SHA512);
                case TLSHashAlgorithm.MD5SHA1: return new MD5SHA1IncrementalHasher();
                default: throw new NotSupportedException();
#endif
            }
        }

#if NET45 || NET451
        private class HashAlgorithmHasher : Hasher
        {
            private HashAlgorithm _hashAlgorithm;

            internal HashAlgorithmHasher(HashAlgorithm hashAlgorithm)
            {
                _hashAlgorithm = hashAlgorithm;
            }

            public override void Dispose()
            {
                _hashAlgorithm.Dispose();
            }

            public override byte[] Final()
            {
                return _hashAlgorithm.TransformFinalBlock(EmptyByteArray, 0, 0);
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
        private class IncrementalHashHasher : Hasher
        {
            private IncrementalHash _incrementalHash;

            internal IncrementalHashHasher(HashAlgorithmName hashAlgorithmName)
            {
                _incrementalHash = IncrementalHash.CreateHash(hashAlgorithmName);
            }

            public override void Dispose()
            {
                _incrementalHash.Dispose();
            }

            public override byte[] Final()
            {
                return _incrementalHash.GetHashAndReset();
            }

            public override void Initialize()
            {
                
            }

            public override void Update(byte[] arr, int offset, int len)
            {
                _incrementalHash.AppendData(arr, offset, len);
            }
        }

        private class MD5SHA1IncrementalHasher : Hasher
        {
            private IncrementalHash _md5;
            private IncrementalHash _sha1;

            internal MD5SHA1IncrementalHasher()
            {
                _md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
                _sha1 = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
            }

            public override void Dispose()
            {
                _md5.Dispose();
                _sha1.Dispose();
            }

            public override byte[] Final()
            {
                byte[] hash = new byte[36];
                var md5Hash = _md5.GetHashAndReset();
                var sha1Hash = _sha1.GetHashAndReset();
                Buffer.BlockCopy(md5Hash, 0, hash, 0, 16);
                Buffer.BlockCopy(sha1Hash, 0, hash, 16, 20);
                Array.Clear(md5Hash, 0, 16);
                Array.Clear(sha1Hash, 0, 20);
                return hash;
            }

            public override void Initialize()
            {
                
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
