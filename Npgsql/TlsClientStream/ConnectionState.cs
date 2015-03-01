using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TlsClientStream
{
    internal class ConnectionState : IDisposable
    {
        public TlsVersion TlsVersion { get; set; }
        public CipherSuiteInfo CipherSuite { get; set; }
        public AesCryptoServiceProvider ReadAes { get; set; }
        public AesCryptoServiceProvider WriteAes { get; set; }
        public int BlockLen { get { return 16; } }
        public HMAC ReadMac { get; set; }
        public HMAC WriteMac { get; set; }
        public ICryptoTransform ReadAesECB { get; set; }
        public ICryptoTransform WriteAesECB { get; set; }
        public ulong[] ReadGCMTable { get; set; }
        public ulong[] WriteGCMTable { get; set; }
        public int MacLen { get { return CipherSuite.MACLen / 8; } }
        public byte[] MasterSecret { get; set; }
        public byte[] ClientRandom { get; set; }
        public byte[] ServerRandom { get; set; }
        public ulong ReadSeqNum { get; set; }
        public ulong WriteSeqNum { get; set; }
        public byte[] ReadIv { get; set; }
        public byte[] WriteIv { get; set; }
        public bool SecureRenegotiation { get; set; }
        public byte[] ClientVerifyData { get; set; }
        public byte[] ServerVerifyData { get; set; }
        public int IvLen { get { return CipherSuite == null ? 0 : CipherSuite.AesMode == AesMode.GCM ? 8 : TlsVersion != TlsVersion.TLSv1_0 ? 16 : 0; } }

        public PRFAlgorithm PRFAlgorithm { get { return TlsVersion == TlsVersion.TLSv1_2 ? CipherSuite.PRFAlgorithm : PRFAlgorithm.TLSPrfMD5SHA1; } }

        public bool IsAuthenticated { get { return ReadAes != null; } }

        public void Dispose()
        {
            if (ReadAesECB != null)
                ReadAesECB.Dispose();
            if (WriteAesECB != null)
                WriteAesECB.Dispose();
            if (ReadAes != null)
                ReadAes.Clear();
            if (WriteAes != null)
                WriteAes.Clear();
            if (ReadMac != null)
                ReadMac.Clear();
            if (WriteMac != null)
                WriteMac.Clear();
            if (ReadIv != null)
                Utils.ClearArray(ReadIv);
            if (WriteIv != null)
                Utils.ClearArray(WriteIv);
            if (ReadGCMTable != null)
                Utils.ClearArray(ReadGCMTable);
            if (WriteGCMTable != null)
                Utils.ClearArray(WriteGCMTable);
            if (MasterSecret != null)
                Utils.ClearArray(MasterSecret);
            if (ClientRandom != null)
                Utils.ClearArray(ClientRandom);
            if (ServerRandom != null)
                Utils.ClearArray(ServerRandom);
            if (ClientVerifyData != null)
                Utils.ClearArray(ClientVerifyData);
            if (ServerVerifyData != null)
                Utils.ClearArray(ServerVerifyData);
        }
    }
}
