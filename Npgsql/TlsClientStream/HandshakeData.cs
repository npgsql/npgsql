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

        public HashAlgorithm CertificateVerifyHash_SHA1;

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
