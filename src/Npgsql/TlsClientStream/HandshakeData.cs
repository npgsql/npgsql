#if NET45 || NET452 || DNX452
#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TlsClientStream
{
    internal class HandshakeData
    {
        public List<X509Certificate2> CertList;
        public X509Chain CertChain;

        // For Finished verify-data
        public HashAlgorithm HandshakeHash1;
        public HashAlgorithm HandshakeHash2;
        public HashAlgorithm HandshakeHash1_384;
        public HashAlgorithm HandshakeHash2_384;
        public HashAlgorithm HandshakeHash1_MD5SHA1;
        public HashAlgorithm HandshakeHash2_MD5SHA1;

        public HashAlgorithm CertificateVerifyHash_SHA1;
        public HashAlgorithm CertificateVerifyHash_MD5;

        // Diffie hellman ephemeral
        public byte[] P, G, Ys;

        // ECDHE
        public EllipticCurve EcCurve;
        public EllipticCurve.BigInt EcX;
        public EllipticCurve.BigInt EcY;

        // Certificate request
        public List<ClientCertificateType> CertificateTypes;
        public List<Tuple<TLSHashAlgorithm, SignatureAlgorithm>> SupportedSignatureAlgorithms;
        public List<string> CertificateAuthorities;

        // Selected client certificate
        public X509Chain SelectedClientCertificate;
    }
}
#endif
