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
        public int BlockLen => 16;
        public HMAC ReadMac { get; set; }
        public HMAC WriteMac { get; set; }
        public ICryptoTransform ReadAesECB { get; set; }
        public ICryptoTransform WriteAesECB { get; set; }
        public ulong[] ReadGCMTable { get; set; }
        public ulong[] WriteGCMTable { get; set; }
        public int MacLen => CipherSuite.MACLen / 8;
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
        public int IvLen => CipherSuite == null ? 0 : CipherSuite.AesMode == AesMode.GCM ? 8 : TlsVersion != TlsVersion.TLSv1_0 ? 16 : 0;

        public PRFAlgorithm PRFAlgorithm => TlsVersion == TlsVersion.TLSv1_2 ? CipherSuite.PRFAlgorithm : PRFAlgorithm.TLSPrfMD5SHA1;

        public bool IsAuthenticated => ReadAes != null;

        public int WriteStartPos
        {
            get
            {
                if (TlsVersion != TlsVersion.TLSv1_0)
                {
                    return 5 + IvLen;
                }
                else
                {
                    // To avoid the BEAST attack, we add an empty application data record
                    return 5 + (MacLen + BlockLen) / BlockLen * BlockLen + 5;
                }
            }
        }

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
#endif
