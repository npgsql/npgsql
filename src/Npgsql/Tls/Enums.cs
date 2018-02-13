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

#pragma warning disable CA1707

// ReSharper disable InconsistentNaming

namespace Npgsql.Tls
{
    enum TlsVersion : ushort
    {
        TLSv1_0 = 0x0301,
        TLSv1_1 = 0x0302,
        TLSv1_2 = 0x0303
    }

    enum AlertLevel : byte
    {
        Warning = 1,
        Fatal = 2
    }

    enum AlertDescription : byte
    {
        CloseNotify = 0,
        UnexpectedMessage = 10,
        BadRecordMac = 20,
        RecordOverflow = 22,
        HandshakeFailure = 40,
        BadCertificate = 42,
        CertificateRevoked = 44,
        CertificateExpired = 45,
        CertificateUnknown = 46,
        IllegalParameter = 47,
        DecodeError = 50,
        DecryptError = 51,
        ProtocolVersion = 70
    }

    enum ContentType : byte
    {
        ChangeCipherSpec = 20,
        Alert = 21,
        Handshake = 22,
        ApplicationData = 23
    }

    enum HandshakeType : byte
    {
        HelloRequest = 0,
        ClientHello = 1,
        ServerHello = 2,
        Certificate = 11,
        ServerKeyExchange = 12,
        CertificateRequest = 13,
        ServerHelloDone = 14,
        CertificateVerify = 15,
        ClientKeyExchange = 16,
        Finished = 20
    }

    enum CipherSuite : ushort
    {
        TLS_RSA_WITH_AES_128_CBC_SHA = 0x002F,
        TLS_DHE_DSS_WITH_AES_128_CBC_SHA = 0x0032,
        TLS_DHE_RSA_WITH_AES_128_CBC_SHA = 0x0033,
        TLS_RSA_WITH_AES_256_CBC_SHA = 0x0035,
        TLS_DHE_DSS_WITH_AES_256_CBC_SHA = 0x0038,
        TLS_DHE_RSA_WITH_AES_256_CBC_SHA = 0x0039,
        TLS_DHE_DSS_WITH_AES_128_CBC_SHA256 = 0x0040,
        TLS_RSA_WITH_AES_128_CBC_SHA256 = 0x003C,
        TLS_RSA_WITH_AES_256_CBC_SHA256 = 0x003D,
        TLS_DHE_RSA_WITH_AES_128_CBC_SHA256 = 0x0067,
        TLS_DHE_DSS_WITH_AES_256_CBC_SHA256 = 0x006A,
        TLS_DHE_RSA_WITH_AES_256_CBC_SHA256 = 0x006B,

        // RFC 5288
        TLS_RSA_WITH_AES_128_GCM_SHA256 = 0x009C,
        TLS_RSA_WITH_AES_256_GCM_SHA384 = 0x009D,
        TLS_DHE_RSA_WITH_AES_128_GCM_SHA256 = 0x009E,
        TLS_DHE_RSA_WITH_AES_256_GCM_SHA384 = 0x009F,
        TLS_DHE_DSS_WITH_AES_128_GCM_SHA256 = 0x00A2,
        TLS_DHE_DSS_WITH_AES_256_GCM_SHA384 = 0x00A3,

        // RFC 4492
        TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA = 0xC004,
        TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA = 0xC005,
        TLS_ECDH_RSA_WITH_AES_128_CBC_SHA = 0xC00E,
        TLS_ECDH_RSA_WITH_AES_256_CBC_SHA = 0xC00F,
        TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA = 0xC013,
        TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA = 0xC014,

        // RFC 5289 CBC
        TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256 = 0xC023,
        TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384 = 0xC024,
        TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256 = 0xC025,
        TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384 = 0xC026,
        TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256 = 0xC027,
        TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384 = 0xC028,
        TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256 = 0xC029,
        TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384 = 0xC02A,

        // RFC 5289 GCM
        TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256 = 0xC02B,
        TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384 = 0xC02C,
        TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256 = 0xC02D,
        TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384 = 0xC02E,
        TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256 = 0xC02F,
        TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384 = 0xC030,
        TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256 = 0xC031,
        TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384 = 0xC032
    }

    enum ExtensionType : ushort
    {
        ServerName = 0x0000,
        SupportedEllipticCurves = 0x000a,
        SupportedPointFormats = 0x000b,
        SignatureAlgorithms = 0x000d,
        RenegotiationInfo = 0xff01
    }

    enum TlsHashAlgorithm : byte
    {
        MD5 = 1,

        SHA1 = 2,
        SHA256 = 4,
        SHA384 = 5,
        SHA512 = 6,

        MD5SHA1 = 255 // Not defined by any spec
    }

    enum SignatureAlgorithm : byte
    {
        RSA = 1,
        DSA = 2,
        ECDSA = 3
    }

    enum NamedCurve : ushort
    {
        secp256r1 = 23,
        secp384r1 = 24,
        secp521r1 = 25
    }

    enum ClientCertificateType : byte
    {
        RSASign = 1,
        DSSSign = 2,
        RSAFixedDH = 3,
        DSSFixedDH = 4,
        ECDSASign = 64,
        RSAFixedECDH = 65,
        ECDSAFixedECDH = 66
    }

    enum KeyExchange : byte
    {
        NULL,
        RSA,
        DHE_RSA,
        DHE_DSS,
        ECDHE_RSA,
        ECDHE_ECDSA,
        ECDH_RSA,
        ECDH_ECDSA
    }

    enum PRFAlgorithm : byte
    {
        TLSPrfSHA256,
        TLSPrfSHA384,
        TLSPrfMD5SHA1
    }

    enum AesMode : byte
    {
        CBC,
        GCM
    }
}
