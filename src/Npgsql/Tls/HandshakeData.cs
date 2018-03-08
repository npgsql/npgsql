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
using System.Security.Cryptography.X509Certificates;

namespace Npgsql.Tls
{
    class HandshakeData
    {
        internal List<X509Certificate2> CertList;
        internal X509Chain CertChain;

        // For Finished verify-data
        internal Hasher HandshakeHash1;
        internal Hasher HandshakeHash2;
        internal Hasher HandshakeHash1_384;
        internal Hasher HandshakeHash2_384;
        internal Hasher HandshakeHash1_MD5SHA1;
        internal Hasher HandshakeHash2_MD5SHA1;

        internal Hasher CertificateVerifyHash_SHA1;
        internal Hasher CertificateVerifyHash_MD5;

        // Diffie hellman ephemeral
        internal byte[] P, G, Ys;

        // ECDHE
        internal EllipticCurve EcCurve;
        internal EllipticCurve.BigInt EcX;
        internal EllipticCurve.BigInt EcY;

        // Certificate request
        internal List<ClientCertificateType> CertificateTypes;
        internal List<Tuple<TlsHashAlgorithm, SignatureAlgorithm>> SupportedSignatureAlgorithms;
        internal List<string> CertificateAuthorities;

        // Selected client certificate
        internal X509Chain SelectedClientCertificate;
    }
}
