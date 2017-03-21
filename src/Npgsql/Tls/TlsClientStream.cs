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

#undef CHECK_ARGUMENTS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Npgsql.Tls
{
    class TlsClientStream : Stream
    {
        const TlsVersion HighestTlsVersionSupported = TlsVersion.TLSv1_2;

        const int MaxEncryptedRecordLen = (1 << 14) /* data */ + 16 + 64 + 256 /* iv + mac + padding (accept long CBC mode padding) */;

        // Buffer data
        byte[] _readBuf = new byte[5 /* header */ + MaxEncryptedRecordLen];
        byte[] _writeBuf = new byte[5 /* header */ + 8192 /* Data */ + 16 /* iv */ + 64 /* mac */ + 16 /* padding */];
        int _readStart;
        int _readEnd;
        int _readPacketLen;

        SemaphoreSlim _writeLock = new SemaphoreSlim(1);

        readonly Stream _baseStream;

        // Connection states
        // Read connection state is for the purpose when we have sent ChangeCipherSpec but the server hasn't yet
        ConnectionState _connState;
        ConnectionState _readConnState;
        [CanBeNull]
        ConnectionState _pendingConnState;

        readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        // Info about the current message in the read buffer
        ContentType _contentType;
        int _plaintextLen;
        int _plaintextStart;

        // Temp buffers to hold sequence number
        readonly byte[] _tempBuf8_1 = new byte[8];
        readonly byte[] _tempBuf8_2 = new byte[8];
        // Temp buffer for GCM
        byte[] _temp512_1;
        byte[] _temp512_2;

        // Holds buffered handshake messages that will be dequeued as soon as a proper final message has been received (ServerHelloDone or Finished)
        readonly HandshakeMessagesBuffer _handshakeMessagesBuffer = new HandshakeMessagesBuffer();
        HandshakeData _handshakeData;

        // User parameters
        bool _noRenegotiationExtensionSupportIsFatal = false;
        string _hostName;
        X509CertificateCollection _clientCertificates;
        System.Net.Security.RemoteCertificateValidationCallback _remoteCertificationValidationCallback;
        bool _checkCertificateRevocation;

        bool _waitingForChangeCipherSpec;
        bool _waitingForFinished;

        // Used in Read and Write methods to keep track of position
        int _writePos;
        int _decryptedReadPos;
        int _decryptedReadEnd;

        // When we write in the middle of a handshake, we must block until the handshake is completed before
        // we can actually write the data, due to a bug in OpenSSL. If we at the same time receive data from
        // the server, we must buffer it so it can be delivered to the application later.
        // Note that this is quite uncommon, since one normally drains the read buffer before writing.
        const int MaxBufferedReadData = 10 * (1 << 20); // 10 MB
        Queue<byte[]> _bufferedReadData;
        int _posBufferedReadData;
        int _lenBufferedReadData;

        // Stream state
        bool _eof;
        bool _closed;

        /// <summary>
        /// Creates a new TlsClientStream with the given underlying stream.
        /// The handshake must be manually initiated with the method PerformInitialHandshake.
        /// </summary>
        /// <param name="baseStream">Base stream</param>
        public TlsClientStream(Stream baseStream)
        {
            _connState = new ConnectionState() { TlsVersion = TlsVersion.TLSv1_0 };
            _readConnState = _connState;
            _baseStream = baseStream;
        }

        #region Record layer

        /// <summary>
        /// Makes sure there is at least one full record available at _readStart.
        /// Also sets _packetLen (does not include packet header of 5 bytes).
        /// </summary>
        /// <returns>True on success, false on End Of Stream.</returns>
        async Task<bool> ReadRecord(bool async)
        {
            int packetLength = -1;
            while (true)
            {
                if (packetLength == -1 && _readEnd - _readStart >= 5)
                {
                    // We have at least a header in our buffer, so extract the length
                    packetLength = (_readBuf[_readStart + 3] << 8) | _readBuf[_readStart + 4];
                    if (packetLength > MaxEncryptedRecordLen)
                    {
                        SendAlertFatal(AlertDescription.RecordOverflow);
                    }
                }
                if (packetLength != -1 && 5 + packetLength <= _readEnd - _readStart)
                {
                    // The whole record fits in the buffer. We are done.
                    _readPacketLen = packetLength;
                    return true;
                }
                if (_readEnd - _readStart > 0 && _readStart > 0)
                {
                    // We only have a partial record in the buffer,
                    // move that to the beginning to be able to read as much as possible from the network.
                    Buffer.BlockCopy(_readBuf, _readStart, _readBuf, 0, _readEnd - _readStart);
                    _readEnd -= _readStart;
                    _readStart = 0;
                }
                if (packetLength == -1 || _readEnd < 5 + packetLength)
                {
                    if (_readStart == _readEnd)
                    {
                        // The read buffer is empty, so start reading at the start of the buffer
                        _readStart = 0;
                        _readEnd = 0;
                    }
                    int read = async
                        ? await _baseStream.ReadAsync(_readBuf, _readEnd, _readBuf.Length - _readEnd)
                        : _baseStream.Read(_readBuf, _readEnd, _readBuf.Length - _readEnd);
                    if (read == 0)
                    {
                        return false;
                    }
                    _readEnd += read;
                }
            }
        }

        // ReadRecord should be called first.
        // Sets _contentType, _plaintextStart and _plaintextLength, and increments _readStart
        void Decrypt()
        {
            _contentType = (ContentType)_readBuf[_readStart];
            if (_readConnState.CipherSuite == null)
            {
                _plaintextStart = _readStart + 5;
                _plaintextLen = _readPacketLen;
            }
            else if (_readConnState.CipherSuite.AesMode == AesMode.CBC)
            {
                var minPlaintextBytes = _readConnState.MacLen + 1;
                var minEncryptedBlocks = (minPlaintextBytes + _readConnState.BlockLen - 1) / _readConnState.BlockLen;
                var minEncryptedBytes = minEncryptedBlocks * _readConnState.BlockLen;
                if (_readPacketLen < _readConnState.IvLen + minEncryptedBytes || (_readPacketLen - _readConnState.IvLen) % _readConnState.BlockLen != 0)
                    SendAlertFatal(AlertDescription.BadRecordMac);
                Buffer.BlockCopy(_readBuf, _readStart + 5, _readConnState.ReadIv, 0, _readConnState.IvLen);

                _readConnState.ReadAes.IV = _readConnState.ReadIv;
                int cipherStartPos = _readStart + 5 + _readConnState.IvLen;
                int cipherLen = _readPacketLen - _readConnState.IvLen;

                if (_readConnState.TlsVersion == TlsVersion.TLSv1_0)
                {
                    // Save the last ciphertext block to become the IV for the next record
                    Buffer.BlockCopy(_readBuf, cipherStartPos + cipherLen - _readConnState.BlockLen, _readConnState.ReadIv, 0, _readConnState.BlockLen);
                }

                using (var decryptor = _readConnState.ReadAes.CreateDecryptor())
                {
                    decryptor.TransformBlock(_readBuf, cipherStartPos, cipherLen, _readBuf, cipherStartPos);
                }
                int paddingLen = _readBuf[cipherStartPos + cipherLen - 1];
                bool paddingFail = false;
                if (paddingLen > cipherLen - 1 - _readConnState.MacLen)
                {
                    // We have found illegal padding. Instead of just send fatal alert directly,
                    // still do the mac computation and let it fail to deal with timing attacks.
                    paddingLen = 0;
                    paddingFail = true;
                }
                int plaintextLen = cipherLen - 1 - paddingLen - _readConnState.MacLen;

                // We don't need the IV anymore in the buffer, so overwrite it with seq_num + header to calculate MAC
                /*Buffer.BlockCopy(_readBuf, _readStart, _readBuf, cipherStartPos - 5, 3);
                Utils.WriteUInt16(_readBuf, cipherStartPos - 2, (ushort)plaintextLen);
                Utils.WriteUInt64(_readBuf, cipherStartPos - 5 - 8, _readConnState.ReadSeqNum);*/

                // We should use the plaintext len, not the encrypted len for the MAC
                //_readConnState.ReadMac.Initialize();
                Utils.WriteUInt64(_tempBuf8_1, 0, _readConnState.ReadSeqNum);
                _readConnState.ReadMac.AppendData(_tempBuf8_1, 0, 8);
                Utils.WriteUInt16(_readBuf, _readStart + 3, (ushort)plaintextLen);
                _readConnState.ReadMac.AppendData(_readBuf, _readStart, 5);
                _readConnState.ReadMac.AppendData(_readBuf, cipherStartPos, plaintextLen);
                var hmac = _readConnState.ReadMac.GetHashAndReset();

                if (!Utils.ArraysEqual(hmac, 0, _readBuf, cipherStartPos + plaintextLen, hmac.Length))
                    SendAlertFatal(AlertDescription.BadRecordMac);

                // Verify that the padding bytes contain the correct value (paddingLen)
                for (int i = 0; i < paddingLen; i++)
                    if (_readBuf[cipherStartPos + cipherLen - 2 - i] != paddingLen)
                        SendAlertFatal(AlertDescription.BadRecordMac);

                // Very unlikely MAC didn't catch this
                if (paddingFail)
                    SendAlertFatal(AlertDescription.BadRecordMac);

                _plaintextStart = cipherStartPos;
                _plaintextLen = plaintextLen;
            }
            else if (_readConnState.CipherSuite.AesMode == AesMode.GCM)
            {
                Buffer.BlockCopy(_readBuf, _readStart + 5, _readConnState.ReadIv, 4, _readConnState.IvLen);
                var cipherStartPos = _readStart + 5 + _readConnState.IvLen;
                var plaintextLen = _readPacketLen - 16 - _readConnState.IvLen;
                if (plaintextLen < 0)
                    SendAlertFatal(AlertDescription.BadRecordMac);
                var ok = GaloisCounterMode.GCMAD(_readConnState.ReadAesECB, _readConnState.ReadIv, _readBuf, cipherStartPos, plaintextLen, _readConnState.ReadSeqNum, (byte)_contentType, _readConnState.ReadGCMTable, _temp512_1);
                if (!ok)
                    SendAlertFatal(AlertDescription.BadRecordMac);
                _plaintextStart = cipherStartPos;
                _plaintextLen = plaintextLen;
            }
            _readStart += 5 + _readPacketLen;
            _readConnState.ReadSeqNum++;
        }

        // startPos: at content type, len: plaintext length without header
        // updates seq num
        /// <summary>
        /// Encrypts a record.
        /// A header should be at startPos containing TLS record type and version.
        /// At startPos + 5 + ivLen the plaintext should start.
        /// </summary>
        /// <param name="startPos">Should point to the beginning of the record (content type)</param>
        /// <param name="len">Plaintext length (without header)</param>
        /// <returns>The byte position after the last byte in this encrypted record</returns>
        int Encrypt(int startPos, int len)
        {
            if (_connState.CipherSuite != null && _connState.CipherSuite.AesMode == AesMode.CBC)
            {
                // Update length first with plaintext length
                Utils.WriteUInt16(_writeBuf, startPos + 3, (ushort)len);

                Utils.WriteUInt64(_tempBuf8_2, 0, _connState.WriteSeqNum++);

                //_connState.WriteMac.Initialize();
                _connState.WriteMac.AppendData(_tempBuf8_2, 0, 8);
                _connState.WriteMac.AppendData(_writeBuf, startPos, 5);
                _connState.WriteMac.AppendData(_writeBuf, startPos + 5 + _connState.IvLen, len);
                var mac = _connState.WriteMac.GetHashAndReset();

                Buffer.BlockCopy(mac, 0, _writeBuf, startPos + 5 + _connState.IvLen + len, mac.Length);
                Utils.ClearArray(mac);

                var paddingLen = _connState.BlockLen - (len + _connState.MacLen + 1) % _connState.BlockLen;
                for (var i = 0; i < paddingLen + 1; i++)
                    _writeBuf[startPos + 5 + _connState.IvLen + len + _connState.MacLen + i] = (byte)paddingLen;

                int encryptedLen = len + _connState.MacLen + paddingLen + 1;

                // Update length now with encrypted length
                Utils.WriteUInt16(_writeBuf, startPos + 3, (ushort)(_connState.IvLen + encryptedLen));

                if (_connState.TlsVersion != TlsVersion.TLSv1_0)
                {
                    _rng.GetBytes(_connState.WriteIv);
                    Buffer.BlockCopy(_connState.WriteIv, 0, _writeBuf, startPos + 5, _connState.WriteIv.Length);
                }
                _connState.WriteAes.IV = _connState.WriteIv;
                using (var encryptor = _connState.WriteAes.CreateEncryptor())
                {
                    encryptor.TransformBlock(_writeBuf, startPos + 5 + _connState.IvLen, encryptedLen, _writeBuf, startPos + 5 + _connState.IvLen);
                }
                if (_connState.TlsVersion == TlsVersion.TLSv1_0)
                {
                    // Save last ciphertext block as the next IV
                    Buffer.BlockCopy(_writeBuf, startPos + 5 + encryptedLen - _connState.BlockLen, _connState.WriteIv, 0, _connState.BlockLen);
                }
                return startPos + 5 + _connState.IvLen + encryptedLen;
            }
            else if (_connState.CipherSuite != null && _connState.CipherSuite.AesMode == AesMode.GCM)
            {
                Utils.WriteUInt64(_connState.WriteIv, 4, _connState.WriteSeqNum);
                Utils.WriteUInt64(_writeBuf, startPos + 5, _connState.WriteSeqNum);
                GaloisCounterMode.GCMAE(_connState.WriteAesECB, _connState.WriteIv, _writeBuf, startPos + 5 + _connState.IvLen, len, _connState.WriteSeqNum++, _writeBuf[startPos], _connState.WriteGCMTable, _temp512_2);
                Utils.WriteUInt16(_writeBuf, startPos + 3, (ushort)(_connState.IvLen + len + 16));
                return startPos + 5 + _connState.IvLen + len + 16;
            }
            else // Null cipher
            {
                // Update length
                Utils.WriteUInt16(_writeBuf, startPos + 3, (ushort)len);
                return startPos + 5 + len;
            }
        }

        #endregion

        #region Handshake infrastructure

        void UpdateHandshakeHash(byte[] buf, int offset, int len)
        {
            // .NET hash api does not allow us to clone hash states ...
            _handshakeData.HandshakeHash1?.Update(buf, offset, len);
            _handshakeData.HandshakeHash1_384?.Update(buf, offset, len);
            _handshakeData.HandshakeHash2?.Update(buf, offset, len);
            _handshakeData.HandshakeHash2_384?.Update(buf, offset, len);
            _handshakeData.HandshakeHash1_MD5SHA1?.Update(buf, offset, len);
            _handshakeData.HandshakeHash2_MD5SHA1?.Update(buf, offset, len);
            _handshakeData.CertificateVerifyHash_MD5?.Update(buf, offset, len);
            _handshakeData.CertificateVerifyHash_SHA1?.Update(buf, offset, len);
        }

        async Task GetInitialHandshakeMessages(bool async, bool allowApplicationData = false)
        {
            while (!_handshakeMessagesBuffer.HasServerHelloDone)
            {
                if (!await ReadRecord(async))
                    throw new IOException("Connection EOF in initial handshake");
                Decrypt();

                switch (_contentType)
                {
                    case ContentType.Alert:
                        await HandleAlertMessage(async);
                        break;
                    case ContentType.Handshake:
                        _handshakeMessagesBuffer.AddBytes(_readBuf, _plaintextStart, _plaintextLen, HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IgnoreHelloRequests);
                        if (_handshakeMessagesBuffer.Messages.Count > 5)
                        {
                            // There can never be more than 5 handshake messages in a handshake
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        }
                        break;
                    case ContentType.ApplicationData:
                        EnqueueReadData(allowApplicationData);
                        break;
                    default:
                        SendAlertFatal(AlertDescription.UnexpectedMessage);
                        break;
                }
            }

            var responseLen = TraverseHandshakeMessages();

            if (async)
            {
                await _baseStream.WriteAsync(_writeBuf, 0, responseLen);
                await _baseStream.FlushAsync();
            }
            else
            {
                _baseStream.Write(_writeBuf, 0, responseLen);
                _baseStream.Flush();
            }
            ResetWritePos();
            _waitingForChangeCipherSpec = true;
        }

        int TraverseHandshakeMessages()
        {
            HandshakeType lastType = 0;
            int responseLen = 0;

            for (var i = 0; i < _handshakeMessagesBuffer.Messages.Count; i++)
            {
                int pos = 0;
                var buf = _handshakeMessagesBuffer.Messages[i];
                UpdateHandshakeHash(buf, 0, buf.Length);
                HandshakeType msgType = (HandshakeType)buf[pos++];
                int msgLen = Utils.ReadUInt24(buf, ref pos);

                switch (msgType)
                {
                    case HandshakeType.ServerHello:
                        if (lastType != 0)
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        ParseServerHelloMessage(buf, ref pos, pos + msgLen);
                        break;
                    case HandshakeType.Certificate:
                        if (lastType != HandshakeType.ServerHello)
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        ParseCertificateMessage(buf, ref pos);
                        break;
                    case HandshakeType.ServerKeyExchange:
                        if (lastType != HandshakeType.Certificate)
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        ParseServerKeyExchangeMessage(buf, ref pos);
                        break;
                    case HandshakeType.CertificateRequest:
                        if (lastType != HandshakeType.Certificate && lastType != HandshakeType.ServerKeyExchange)
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        ParseCertificateRequest(buf, ref pos);
                        break;
                    case HandshakeType.ServerHelloDone:
                        if (msgLen != 0)
                            SendAlertFatal(AlertDescription.DecodeError);
                        if ((lastType != HandshakeType.Certificate && lastType != HandshakeType.ServerKeyExchange && lastType != HandshakeType.CertificateRequest)
                            || i != _handshakeMessagesBuffer.Messages.Count - 1)
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        responseLen = GenerateHandshakeResponse();
                        break;
                    default:
                        SendAlertFatal(AlertDescription.UnexpectedMessage);
                        break;
                }
                if (pos != 4 + msgLen)
                    SendAlertFatal(AlertDescription.DecodeError);
                lastType = msgType;
            }
            _handshakeMessagesBuffer.ClearMessages();
            return responseLen;
        }

        // Here we send all client messages in response to server hello
        int GenerateHandshakeResponse()
        {
            var offset = 0;
            var ivLen = _connState.IvLen;
            if (_handshakeData.CertificateTypes != null) // Certificate request has been sent by the server
            {
                SendHandshakeMessage(SendClientCertificate, ref offset, ivLen);
            }
            switch (_pendingConnState.CipherSuite.KeyExchange)
            {
                case KeyExchange.DHE_RSA:
                case KeyExchange.DHE_DSS:
                    SendHandshakeMessage(SendClientKeyExchangeDhe, ref offset, ivLen);
                    break;
                case KeyExchange.ECDHE_RSA:
                case KeyExchange.ECDHE_ECDSA:
                    SendHandshakeMessage(SendClientKeyExchangeEcdhe, ref offset, ivLen);
                    break;
                case KeyExchange.ECDH_ECDSA:
                case KeyExchange.ECDH_RSA:
                    SendHandshakeMessage(SendClientKeyExchangeEcdh, ref offset, ivLen);
                    break;
                case KeyExchange.RSA:
                    SendHandshakeMessage(SendClientKeyExchangeRsa, ref offset, ivLen);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (_handshakeData.CertificateTypes != null && _handshakeData.SelectedClientCertificate != null)
            {
                SendHandshakeMessage(SendCertificateVerify, ref offset, ivLen);
            }

            var cipherSpecStart = offset;
            SendChangeCipherSpec(ref offset, ivLen);
            offset = Encrypt(cipherSpecStart, 1);

            // Key generation from Master Secret
            var mode = _pendingConnState.CipherSuite.AesMode;
            var isCbc = mode == AesMode.CBC;
            var isGcm = mode == AesMode.GCM;

            var concRandom = new byte[_pendingConnState.ServerRandom.Length + _pendingConnState.ClientRandom.Length];
            Buffer.BlockCopy(_pendingConnState.ServerRandom, 0, concRandom, 0, _pendingConnState.ServerRandom.Length);
            Buffer.BlockCopy(_pendingConnState.ClientRandom, 0, concRandom, _pendingConnState.ServerRandom.Length, _pendingConnState.ClientRandom.Length);
            var macLen = isCbc ? _pendingConnState.CipherSuite.MACLen / 8 : 0;
            var aesKeyLen = _pendingConnState.CipherSuite.AesKeyLen / 8;
            var IVLen = isGcm ? 4 : _pendingConnState.TlsVersion != TlsVersion.TLSv1_0 ? 0 : _pendingConnState.BlockLen;
            var keyBlock = Utils.PRF(_pendingConnState.PRFAlgorithm, _pendingConnState.MasterSecret, "key expansion", concRandom, macLen * 2 + aesKeyLen * 2 + IVLen * 2);
            byte[] writeMac = new byte[macLen], readMac = new byte[macLen], writeKey = new byte[aesKeyLen], readKey = new byte[aesKeyLen];
            Buffer.BlockCopy(keyBlock, 0, writeMac, 0, macLen);
            Buffer.BlockCopy(keyBlock, macLen, readMac, 0, macLen);
            Buffer.BlockCopy(keyBlock, macLen * 2, writeKey, 0, aesKeyLen);
            Buffer.BlockCopy(keyBlock, macLen * 2 + aesKeyLen, readKey, 0, aesKeyLen);
            if (isCbc)
            {
                _pendingConnState.WriteMac = _pendingConnState.CipherSuite.CreateHMAC(writeMac);
                _pendingConnState.ReadMac = _pendingConnState.CipherSuite.CreateHMAC(readMac);
            }
            if (IVLen != 0)
            {
                // For GCM we make it bigger to later fill in sequence numbers
                var writeIv = new byte[isGcm ? 16 : IVLen];
                var readIv = new byte[isGcm ? 16 : IVLen];
                Buffer.BlockCopy(keyBlock, macLen * 2 + aesKeyLen * 2, writeIv, 0, IVLen);
                Buffer.BlockCopy(keyBlock, macLen * 2 + aesKeyLen * 2 + IVLen, readIv, 0, IVLen);
                _pendingConnState.WriteIv = writeIv;
                _pendingConnState.ReadIv = readIv;
            }
            else
            {
                _pendingConnState.ReadIv = _pendingConnState.WriteIv = new byte[_pendingConnState.BlockLen];
            }

            _pendingConnState.WriteAes = Aes.Create();
            _pendingConnState.WriteAes.Key = writeKey;
            _pendingConnState.WriteAes.Mode = isCbc ? CipherMode.CBC : CipherMode.ECB;
            _pendingConnState.WriteAes.Padding = PaddingMode.None;

            _pendingConnState.ReadAes = Aes.Create();
            _pendingConnState.ReadAes.Key = readKey;
            _pendingConnState.ReadAes.Mode = isCbc ? CipherMode.CBC : CipherMode.ECB;
            _pendingConnState.ReadAes.Padding = PaddingMode.None;

            // int tmpOffset = macLen * 2 + aesKeyLen * 2;
            if (isGcm)
            {
                _pendingConnState.WriteAesECB = _pendingConnState.WriteAes.CreateEncryptor(writeKey, null);
                _pendingConnState.ReadAesECB = _pendingConnState.ReadAes.CreateEncryptor(readKey, null);
                _pendingConnState.WriteGCMTable = GaloisCounterMode.GetH(_pendingConnState.WriteAesECB);
                _pendingConnState.ReadGCMTable = GaloisCounterMode.GetH(_pendingConnState.ReadAesECB);
                if (_temp512_1 == null)
                {
                    _temp512_1 = new byte[512];
                    _temp512_2 = new byte[512];
                }
            }
            Utils.ClearArray(writeMac);
            Utils.ClearArray(readMac);
            Utils.ClearArray(writeKey);
            Utils.ClearArray(readKey);
            ivLen = _pendingConnState.IvLen;

            _connState = _pendingConnState;
            SendHandshakeMessage(SendFinished, ref offset, ivLen);
            //_handshakeData.HandshakeHash2.Final();

            // _buf is now ready to be written to the base stream, from pos 0 to offset
            return offset;
        }

        delegate HandshakeType SendHandshakeMessageDelegate(ref int offset);

        void SendHandshakeMessage(SendHandshakeMessageDelegate func, ref int offset, int ivLen)
        {
            int start = offset;
            int messageStart = start + 5 + ivLen;

            _writeBuf[offset++] = (byte)ContentType.Handshake;

            // Highest version supported
            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)_connState.TlsVersion);

            // Record length to be filled in later
            offset += 2;

            offset += ivLen;

            int handshakeTypePos = offset;

            // Type and length filled in below
            offset += 4;

            var handshakeType = func(ref offset);
            var messageLen = offset - (handshakeTypePos + 4);
            _writeBuf[handshakeTypePos] = (byte)handshakeType;
            Utils.WriteUInt24(_writeBuf, handshakeTypePos + 1, messageLen);

            UpdateHandshakeHash(_writeBuf, messageStart, offset - messageStart);

            offset = Encrypt(start, offset - messageStart);
        }

        async Task WaitForHandshakeCompleted(bool initialHandshake, bool async)
        {
            for (; ; )
            {
                if (!await ReadRecord(async))
                {
                    _eof = true;
                    throw new IOException("Unexpected connection EOF in handshake");
                }
                Decrypt();
                if (_contentType != ContentType.ChangeCipherSpec)
                {
                    EnqueueReadData(!initialHandshake);
                }
                else
                {
                    ParseChangeCipherSpec();
                    _waitingForChangeCipherSpec = false;
                    break;
                }
            }

            while (_handshakeMessagesBuffer.Messages.Count == 0)
            {
                if (!await ReadRecord(async))
                {
                    _eof = true;
                    throw new IOException("Unexpected connection EOF in handshake");
                }
                Decrypt();
                if (_contentType != ContentType.Handshake)
                {
                    EnqueueReadData(!initialHandshake);
                }
                else
                {
                    _handshakeMessagesBuffer.AddBytes(_readBuf, _plaintextStart, _plaintextLen, HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IgnoreHelloRequestsUntilFinished);
                }
            }

            if ((HandshakeType)_handshakeMessagesBuffer.Messages[0][0] == HandshakeType.Finished)
            {
                ParseFinishedMessage(_handshakeMessagesBuffer.Messages[0]);
                _handshakeMessagesBuffer.RemoveFirst(); // Leave possible hello requests after this position
            }
            else
            {
                SendAlertFatal(AlertDescription.UnexpectedMessage);
            }
        }

        #endregion

        #region Handshake messages

        HandshakeType SendClientHello(ref int offset)
        {
            _pendingConnState = new ConnectionState();
            _handshakeData = new HandshakeData
            {
                HandshakeHash1 = Hasher.Create(TlsHashAlgorithm.SHA256),
                HandshakeHash2 = Hasher.Create(TlsHashAlgorithm.SHA256),
                HandshakeHash1_384 = Hasher.Create(TlsHashAlgorithm.SHA384),
                HandshakeHash2_384 = Hasher.Create(TlsHashAlgorithm.SHA384),
                HandshakeHash1_MD5SHA1 = Hasher.Create(TlsHashAlgorithm.MD5SHA1),
                HandshakeHash2_MD5SHA1 = Hasher.Create(TlsHashAlgorithm.MD5SHA1),
                CertificateVerifyHash_MD5 = Hasher.Create(TlsHashAlgorithm.MD5),
                CertificateVerifyHash_SHA1 = Hasher.Create(TlsHashAlgorithm.SHA1)
            };

            // Highest version supported
            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)HighestTlsVersionSupported);

            // Client random
            var timestamp = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            _pendingConnState.ClientRandom = new byte[32];
            _rng.GetBytes(_pendingConnState.ClientRandom);
            Utils.WriteUInt32(_pendingConnState.ClientRandom, 0, timestamp);

            Buffer.BlockCopy(_pendingConnState.ClientRandom, 0, _writeBuf, offset, 32);
            offset += 32;

            // No session id
            _writeBuf[offset++] = 0;

            // Cipher suites
            var supportedCipherSuites = CipherSuiteInfo.Supported;
            /*
            if (HighestTlsVersionSupported != TlsVersion.TLSv1_2)
                supportedCipherSuites = supportedCipherSuites.Where(cs => cs.IsAllowedBefore1_2).ToArray();
            */
            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)(supportedCipherSuites.Length * sizeof(ushort)));
            foreach (var suite in supportedCipherSuites)
            {
                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)suite.Id);
            }

            // Compression methods
            _writeBuf[offset++] = 1; // Length
            _writeBuf[offset++] = 0; // "null" compression method

            // Extensions length, fill in later
            var extensionLengthOffset = offset;
            offset += 2;

            // Renegotiation extension
            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)ExtensionType.RenegotiationInfo);
            if (_connState.SecureRenegotiation)
            {
                // Extension length
                offset += Utils.WriteUInt16(_writeBuf, offset, 13);

                // Renegotiated connection length
                _writeBuf[offset++] = 12;
                // Renegotiated connection data
                Buffer.BlockCopy(_connState.ClientVerifyData, 0, _writeBuf, offset, 12);
                offset += 12;
            }
            else
            {
                // Extension length
                offset += Utils.WriteUInt16(_writeBuf, offset, 1);
                // Renegotiated connection length
                _writeBuf[offset++] = 0;
            }

            // SNI extension
            if (_hostName != null)
            {
                // TODO: IDN Unicode -> Punycode

                // NOTE: IP addresses should not use SNI extension, per specification.
                if (!System.Net.IPAddress.TryParse(_hostName, out var ip))
                {
                    offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)ExtensionType.ServerName);
                    var byteLen = Encoding.ASCII.GetBytes(_hostName, 0, _hostName.Length, _writeBuf, offset + 7);
                    offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)(5 + byteLen));
                    offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)(3 + byteLen));
                    _writeBuf[offset++] = 0; // host_name
                    offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)byteLen);
                    offset += byteLen;
                }
            }

            if (HighestTlsVersionSupported == TlsVersion.TLSv1_2)
            {
                // Signature algorithms extension. At least IIS 7.5 needs this or it immediately resets the connection.
                // Used to specify what kind of server certificate hash/signature algorithms we can use to verify it.
                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)ExtensionType.SignatureAlgorithms);
                offset += Utils.WriteUInt16(_writeBuf, offset, 20);
                offset += Utils.WriteUInt16(_writeBuf, offset, 18);
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA1;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.ECDSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA256;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.ECDSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA384;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.ECDSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA512;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.ECDSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA1;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.RSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA256;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.RSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA384;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.RSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA512;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.RSA;
                _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA1;
                _writeBuf[offset++] = (byte)SignatureAlgorithm.DSA;
            }

            if (supportedCipherSuites.Any(s => s.KeyExchange == KeyExchange.ECDHE_RSA || s.KeyExchange == KeyExchange.ECDHE_ECDSA))
            {
                // Supported Elliptic Curves Extension

                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)ExtensionType.SupportedEllipticCurves);
                offset += Utils.WriteUInt16(_writeBuf, offset, 8);
                offset += Utils.WriteUInt16(_writeBuf, offset, 6);
                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)NamedCurve.secp256r1);
                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)NamedCurve.secp384r1);
                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)NamedCurve.secp521r1);

                // Supported Point Formats Extension

                offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)ExtensionType.SupportedPointFormats);
                offset += Utils.WriteUInt16(_writeBuf, offset, 2);
                _writeBuf[offset++] = 1; // Length
                _writeBuf[offset++] = 0; // Uncompressed
            }


            Utils.WriteUInt16(_writeBuf, extensionLengthOffset, (ushort)(offset - (extensionLengthOffset + 2)));

            return HandshakeType.ClientHello;
        }

        void ParseServerHelloMessage(byte[] buf, ref int pos, int endPos)
        {
            var renegotiating = _connState.ReadAes != null;

            var version = (TlsVersion)Utils.ReadUInt16(buf, ref pos);
            if (version < TlsVersion.TLSv1_0 || version > TlsVersion.TLSv1_2)
            {
                SendAlertFatal(AlertDescription.ProtocolVersion);
            }
            _connState.TlsVersion = version;
            _pendingConnState.TlsVersion = version;

            _pendingConnState.ServerRandom = new byte[32];
            Buffer.BlockCopy(buf, pos, _pendingConnState.ServerRandom, 0, 32);
            pos += 32;

            // Skip session id
            var sessionIDLength = buf[pos++];
            pos += sessionIDLength;

            var cipherSuite = (CipherSuite)Utils.ReadUInt16(buf, ref pos);
            var compressionMethod = buf[pos++];

            _pendingConnState.CipherSuite = CipherSuiteInfo.Supported.FirstOrDefault(s => s.Id == cipherSuite);
            if (_pendingConnState.CipherSuite == null || !_pendingConnState.CipherSuite.IsAllowedBefore1_2 && version != TlsVersion.TLSv1_2 || compressionMethod != 0)
            {
                SendAlertFatal(AlertDescription.IllegalParameter);
            }

            if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2)
            {
                switch (_pendingConnState.CipherSuite.PRFAlgorithm)
                {
                    case PRFAlgorithm.TLSPrfSHA256:
                        _handshakeData.HandshakeHash1_384.Dispose();
                        _handshakeData.HandshakeHash1_384 = null;
                        _handshakeData.HandshakeHash2_384.Dispose();
                        _handshakeData.HandshakeHash2_384 = null;
                        break;
                    case PRFAlgorithm.TLSPrfSHA384:
                        _handshakeData.HandshakeHash1.Dispose();
                        _handshakeData.HandshakeHash1 = _handshakeData.HandshakeHash1_384;
                        _handshakeData.HandshakeHash1_384 = null;
                        _handshakeData.HandshakeHash2.Dispose();
                        _handshakeData.HandshakeHash2 = _handshakeData.HandshakeHash2_384;
                        _handshakeData.HandshakeHash2_384 = null;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                _handshakeData.HandshakeHash1_MD5SHA1.Dispose();
                _handshakeData.HandshakeHash1_MD5SHA1 = null;
                _handshakeData.HandshakeHash2_MD5SHA1.Dispose();
                _handshakeData.HandshakeHash2_MD5SHA1 = null;
                _handshakeData.CertificateVerifyHash_MD5.Dispose();
                _handshakeData.CertificateVerifyHash_MD5 = null;
            }
            else
            {
                _handshakeData.HandshakeHash1.Dispose();
                _handshakeData.HandshakeHash1 = null;
                _handshakeData.HandshakeHash2.Dispose();
                _handshakeData.HandshakeHash2 = null;
                _handshakeData.HandshakeHash1_384.Dispose();
                _handshakeData.HandshakeHash1_384 = null;
                _handshakeData.HandshakeHash2_384.Dispose();
                _handshakeData.HandshakeHash2_384 = null;
                _handshakeData.HandshakeHash1 = _handshakeData.HandshakeHash1_MD5SHA1;
                _handshakeData.HandshakeHash1_MD5SHA1 = null;
                _handshakeData.HandshakeHash2 = _handshakeData.HandshakeHash2_MD5SHA1;
                _handshakeData.HandshakeHash2_MD5SHA1 = null;
            }

            // If no extensions present, return
            if (pos == endPos)
                return;

            var processedRenegotiationInfo = false;

            var extensionLength = Utils.ReadUInt16(buf, ref pos);
            var extensionsEnd = pos + extensionLength;
            while (pos < extensionsEnd)
            {
                var extensionId = (ExtensionType)Utils.ReadUInt16(buf, ref pos);
                switch (extensionId)
                {
                    case ExtensionType.RenegotiationInfo:
                        if (processedRenegotiationInfo)
                            SendAlertFatal(AlertDescription.HandshakeFailure);
                        processedRenegotiationInfo = true;

                        var lengthFull = Utils.ReadUInt16(buf, ref pos);
                        var length = buf[pos++];
                        if (length + 1 != lengthFull)
                            SendAlertFatal(AlertDescription.HandshakeFailure);

                        if (!renegotiating)
                        {
                            if (length != 0)
                                SendAlertFatal(AlertDescription.HandshakeFailure);
                        }
                        if (renegotiating)
                        {
                            if (!_connState.SecureRenegotiation)
                                SendAlertFatal(AlertDescription.HandshakeFailure);

                            if (length != 24)
                                SendAlertFatal(AlertDescription.HandshakeFailure);

                            for (var j = 0; j < 12; j++)
                            {
                                if (_connState.ClientVerifyData[j] != buf[pos++])
                                    SendAlertFatal(AlertDescription.HandshakeFailure);
                            }
                            for (var j = 0; j < 12; j++)
                            {
                                if (_connState.ServerVerifyData[j] != buf[pos++])
                                    SendAlertFatal(AlertDescription.HandshakeFailure);
                            }
                        }
                        _pendingConnState.SecureRenegotiation = true;
                        break;
                    case ExtensionType.SupportedEllipticCurves:
                        var len = Utils.ReadUInt16(buf, ref pos);
                        // Contains in what formats the server can parse. Ignore it.
                        pos += len;
                        break;
                    case ExtensionType.SupportedPointFormats:
                        var length1 = Utils.ReadUInt16(buf, ref pos);
                        // Contains in what formats the server can parse. Ignore it.
                        pos += length1;
                        break;
                    case ExtensionType.ServerName:
                        var length2 = Utils.ReadUInt16(buf, ref pos);
                        pos += length2;
                        if (length2 != 0)
                            SendAlertFatal(AlertDescription.IllegalParameter);
                        break;
                    default:
                        SendAlertFatal(AlertDescription.IllegalParameter);
                        break;
                }
            }

            if (!processedRenegotiationInfo && (!renegotiating && _noRenegotiationExtensionSupportIsFatal || renegotiating && _connState.SecureRenegotiation))
            {
                SendAlertFatal(AlertDescription.HandshakeFailure);
            }
        }

        void ParseCertificateMessage(byte[] buf, ref int pos)
        {
            _handshakeData.CertList = new List<X509Certificate2>();
            _handshakeData.CertChain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationMode = _checkCertificateRevocation ? X509RevocationMode.Online : X509RevocationMode.NoCheck
                }
            };
            var errors = System.Net.Security.SslPolicyErrors.None;

            var totalLen = Utils.ReadUInt24(buf, ref pos);
            if (totalLen == 0)
                SendAlertFatal(AlertDescription.IllegalParameter);

            int endPos = pos + totalLen;
            while (pos < endPos)
            {
                var certLen = Utils.ReadUInt24(buf, ref pos);
                var certBytes = new byte[certLen];
                Buffer.BlockCopy(buf, pos, certBytes, 0, certLen);
                pos += certLen;

                try
                {
                    var cert = new X509Certificate2(certBytes);
                    if (_handshakeData.CertList.Count != 0)
                        _handshakeData.CertChain.ChainPolicy.ExtraStore.Add(cert);
                    _handshakeData.CertList.Add(cert);
                }
                catch (CryptographicException e)
                {
                    SendAlertFatal(AlertDescription.BadCertificate, e.Message);
                }
            }

            if (_handshakeData.CertList.Count == 0)
            {
                SendAlertFatal(AlertDescription.CertificateUnknown, "No certificate was provided by the server");
            }

            // Validate certificate
            _handshakeData.CertChain.Build(_handshakeData.CertList[0]);
            var hostnameError = false;
            if (!string.IsNullOrEmpty(_hostName))
            {
                hostnameError = !Utils.HostnameInCertificate(_handshakeData.CertList[0], _hostName);
                if (hostnameError)
                    errors |= System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch;
            }
            var hasChainStatus = _handshakeData.CertChain.ChainStatus != null;
            if (hasChainStatus && _handshakeData.CertChain.ChainStatus.Length > 0)
            {
                errors |= System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors;
            }

            bool success = _remoteCertificationValidationCallback != null
                ? _remoteCertificationValidationCallback(this, _handshakeData.CertList[0], _handshakeData.CertChain, errors)
                : errors == System.Net.Security.SslPolicyErrors.None;

            if (!success)
            {
                if (hasChainStatus && _handshakeData.CertChain.ChainStatus.Any(s => (s.Status & X509ChainStatusFlags.NotTimeValid) != 0))
                    SendAlertFatal(AlertDescription.CertificateExpired);
                else if (_handshakeData.CertChain.ChainStatus.Any(s => (s.Status & X509ChainStatusFlags.Revoked) != 0))
                    SendAlertFatal(AlertDescription.CertificateRevoked);
                else
                {
                    var errorMsg = "Server certificate was not accepted.";
                    if ((errors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                        errorMsg += " Chain status: " + string.Join(", ", _handshakeData.CertChain.ChainStatus.Select(s => s.StatusInformation)) + ".";
                    if (hostnameError)
                        errorMsg += " The specified hostname was not present in the certificate.";

                    SendAlertFatal(AlertDescription.CertificateUnknown, errorMsg);
                }
            }
        }

        // Destroys preMasterSecret
        void SetMasterSecret(byte[] preMasterSecret)
        {
            var concRandom = new byte[_pendingConnState.ClientRandom.Length + _pendingConnState.ServerRandom.Length];
            Buffer.BlockCopy(_pendingConnState.ClientRandom, 0, concRandom, 0, _pendingConnState.ClientRandom.Length);
            Buffer.BlockCopy(_pendingConnState.ServerRandom, 0, concRandom, _pendingConnState.ClientRandom.Length, _pendingConnState.ServerRandom.Length);
            _pendingConnState.MasterSecret = Utils.PRF(_pendingConnState.PRFAlgorithm, preMasterSecret, "master secret", concRandom, 48);

            Utils.ClearArray(preMasterSecret);
        }

        HandshakeType SendClientKeyExchangeRsa(ref int offset)
        {
            byte[] preMasterSecret = new byte[48];

            _rng.GetBytes(preMasterSecret);

            // Highest version supported
            Utils.WriteUInt16(preMasterSecret, 0, (ushort)HighestTlsVersionSupported);

            byte[] encryptedPreMasterSecret = Utils.EncryptPkcsPadding(_handshakeData.CertList[0], preMasterSecret);
            /*var rsa = (RSACryptoServiceProvider)_handshakeData.CertList[0].PublicKey.Key;
            var encryptedPreMasterSecret = rsa.Encrypt(preMasterSecret, false);*/
            SetMasterSecret(preMasterSecret);

            // Message content
            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)encryptedPreMasterSecret.Length);
            Buffer.BlockCopy(encryptedPreMasterSecret, 0, _writeBuf, offset, encryptedPreMasterSecret.Length);
            offset += encryptedPreMasterSecret.Length;

            return HandshakeType.ClientKeyExchange;
        }

        void ParseServerKeyExchangeMessage(byte[] buf, ref int pos)
        {
            var secParamStart = pos;

            if (_pendingConnState.CipherSuite.KeyExchange == KeyExchange.DHE_RSA || _pendingConnState.CipherSuite.KeyExchange == KeyExchange.DHE_DSS)
            {
                // DHE

                // We add 1 extra 0-byte to each array to make sure the sign is positive (BigInteger constructor checks most significant bit)
                var Plen = Utils.ReadUInt16(buf, ref pos);
                _handshakeData.P = new byte[Plen + 1];
                for (var i = 0; i < Plen; i++)
                    _handshakeData.P[i] = buf[pos + Plen - 1 - i];
                // Reject prime moduli smaller than 1024 bits to prevent the Logjam attack, see weakdh.org
                if (Plen < 128 || buf[pos] == 0)
                {
                    SendAlertFatal(AlertDescription.IllegalParameter, "Diffie-Hellman prime modulus smaller than 1024 bits offered by the server");
                }
                pos += Plen;

                var Glen = Utils.ReadUInt16(buf, ref pos);
                _handshakeData.G = new byte[Glen + 1];
                for (var i = 0; i < Glen; i++)
                    _handshakeData.G[i] = buf[pos + Glen - 1 - i];
                pos += Glen;

                var Yslen = Utils.ReadUInt16(buf, ref pos);
                _handshakeData.Ys = new byte[Yslen + 1];
                for (var i = 0; i < Yslen; i++)
                    _handshakeData.Ys[i] = buf[pos + Yslen - 1 - i];
                pos += Yslen;

                // We could verify that the parameters are "good", but since we trust the certificate and the parameters are digitally signed,
                // we trust that the server has made a good choice.
            }
            else if (_pendingConnState.CipherSuite.KeyExchange == KeyExchange.ECDHE_RSA || _pendingConnState.CipherSuite.KeyExchange == KeyExchange.ECDHE_ECDSA)
            {
                // ECDHE

                var curveType = buf[pos++];
                if (curveType != 0x03) // 0x03 = Named curve
                {
                    SendAlertFatal(AlertDescription.IllegalParameter);
                }
                var namedcurve = (NamedCurve)Utils.ReadUInt16(buf, ref pos);
                EllipticCurve curve;
                switch (namedcurve)
                {
                    case NamedCurve.secp256r1:
                        curve = EllipticCurve.P256;
                        break;
                    case NamedCurve.secp384r1:
                        curve = EllipticCurve.P384;
                        break;
                    case NamedCurve.secp521r1:
                        curve = EllipticCurve.P521;
                        break;
                    default:
                        SendAlertFatal(AlertDescription.IllegalParameter);
                        curve = null;
                        break;
                }
                pos++;  // opaqueLen. TODO: check len
                if (buf[pos++] != 4) // Uncompressed
                {
                    SendAlertFatal(AlertDescription.IllegalParameter);
                }
                _handshakeData.EcX = new EllipticCurve.BigInt(buf, pos, curve.curveByteLen);
                pos += curve.curveByteLen;
                _handshakeData.EcY = new EllipticCurve.BigInt(buf, pos, curve.curveByteLen);
                pos += curve.curveByteLen;
                _handshakeData.EcCurve = curve;
            }
            else
            {
                SendAlertFatal(AlertDescription.UnexpectedMessage);
            }

            var parametersEnd = pos;

            // Digitally signed client random + server random + parameters
            TlsHashAlgorithm hashAlgorithm;
            SignatureAlgorithm signatureAlgorithm;
            if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2)
            {
                hashAlgorithm = (TlsHashAlgorithm)buf[pos++];
                signatureAlgorithm = (SignatureAlgorithm)buf[pos++];
            }
            else
            {
                signatureAlgorithm = _pendingConnState.CipherSuite.GetSignatureAlgorithm();
                hashAlgorithm = signatureAlgorithm == SignatureAlgorithm.RSA
                    ? TlsHashAlgorithm.MD5SHA1
                    : TlsHashAlgorithm.SHA1;
            }

            var signatureLen = Utils.ReadUInt16(buf, ref pos);
            byte[] signature = new byte[signatureLen];
            Buffer.BlockCopy(buf, pos, signature, 0, signatureLen);
            pos += signatureLen;

            Hasher alg = null;
            switch (hashAlgorithm)
            {
                case TlsHashAlgorithm.SHA1:
                case TlsHashAlgorithm.SHA256:
                case TlsHashAlgorithm.SHA384:
                case TlsHashAlgorithm.SHA512:
                    alg = Hasher.Create(hashAlgorithm); break;
                case TlsHashAlgorithm.MD5SHA1:
                    if (_pendingConnState.TlsVersion != TlsVersion.TLSv1_2)
                    {
                        alg = Hasher.Create(hashAlgorithm);
                        break;
                    }
                    else
                    {
                        goto default;
                    }
                default: SendAlertFatal(AlertDescription.IllegalParameter); break;
            }

            alg.Update(_pendingConnState.ClientRandom, 0, 32);
            alg.Update(_pendingConnState.ServerRandom, 0, 32);
            alg.Update(buf, secParamStart, parametersEnd - secParamStart);
            var hash = alg.Final();
            alg.Dispose();

            if (signatureAlgorithm == SignatureAlgorithm.ECDSA)
            {
                var pkParameters = _handshakeData.CertList[0].GetKeyAlgorithmParameters();
                var pkKey = _handshakeData.CertList[0].GetPublicKey();
                bool? res;

                // .NET Framework has an optimized native implementation here, but it doesn't
                // exist in CoreCLR (netstandard13) or on mono.
                // We include two checks - one compile-time and one runtime - to allow everyone
                // to be happy. The OPTIMIZED_CRYPTOGRAPHY define is only enabled explicitly
                // at the build server
#if OPTIMIZED_CRYPTOGRAPHY && !NETSTANDARD1_3
                if (Type.GetType("Mono.Runtime") != null)
                    res = EllipticCurve.VerifySignature(pkParameters, pkKey, hash, signature);
                else
                    res = EllipticCurve.VerifySignatureCng(pkParameters, pkKey, hash, signature);
#else
                res = EllipticCurve.VerifySignature(pkParameters, pkKey, hash, signature);
#endif

                if (!res.HasValue)
                {
                    SendAlertFatal(AlertDescription.IllegalParameter);
                }
                else if (!res.Value)
                {
                    SendAlertFatal(AlertDescription.DecryptError);
                }
            }
            else
            {
#if NET45 || NET451
                var pubKey = _handshakeData.CertList[0].PublicKey.Key;
                var rsa = pubKey as RSACryptoServiceProvider;
                var dsa = pubKey as DSACryptoServiceProvider;
                if (signatureAlgorithm == SignatureAlgorithm.RSA && rsa != null)
                {
                    bool ok = _pendingConnState.TlsVersion == TlsVersion.TLSv1_2 ?
                        rsa.VerifyHash(hash, Utils.HashNameToOID[hashAlgorithm.ToString()], signature) :
                        RsaPKCS1.VerifyRsaPKCS1(rsa, signature, hash, _pendingConnState.TlsVersion == TlsVersion.TLSv1_0 && _pendingConnState.CipherSuite.KeyExchange == KeyExchange.DHE_RSA);

                    if (!ok)
                    {
                        SendAlertFatal(AlertDescription.DecryptError);
                    }
                }
                else if (signatureAlgorithm == SignatureAlgorithm.DSA && dsa != null)
                {
                    // We must decode from DER to two raw integers
                    // NOTE: DSACryptoServiceProvider can't handle keys larger than 1024 bits, neither can SslStream.
                    var decodedSignature = Utils.DecodeDERSignature(signature, 0, signature.Length, Utils.GetHashLen(hashAlgorithm) >> 3);
                    if (!dsa.VerifyHash(hash, Utils.HashNameToOID[hashAlgorithm.ToString()], decodedSignature))
                    {
                        SendAlertFatal(AlertDescription.DecryptError);
                    }
                }
                else
                {
                    SendAlertFatal(AlertDescription.IllegalParameter);
                }
#else
                bool ok = false;
                if (signatureAlgorithm == SignatureAlgorithm.RSA)
                {
                    using (var rsa = _handshakeData.CertList[0].GetRSAPublicKey())
                    {
                        if (rsa != null)
                        {
                            ok = _pendingConnState.TlsVersion == TlsVersion.TLSv1_2 ?
                                rsa.VerifyHash(hash, signature, Hasher.GetHashAlgorithmName(hashAlgorithm), RSASignaturePadding.Pkcs1) :
                                RsaPKCS1.VerifyRsaPKCS1(rsa, signature, hash, _pendingConnState.TlsVersion == TlsVersion.TLSv1_0 && _pendingConnState.CipherSuite.KeyExchange == KeyExchange.DHE_RSA);
                        }
                    }
                }

                if (!ok)
                {
                    SendAlertFatal(AlertDescription.IllegalParameter);
                }
#endif
            }
        }

        HandshakeType SendClientKeyExchangeDhe(ref int offset)
        {
            if (_handshakeData.P == null)
            {
                // Server Key Exchange with DHE was not received
                SendAlertFatal(AlertDescription.UnexpectedMessage);
            }

            byte[] Xc = new byte[33]; // Use a 256-bit exponent
            _rng.GetBytes(Xc);
            Xc[Xc.Length - 1] = 0; // Set last byte to 0 to force a positive number.
            var gBig = new BigInteger(_handshakeData.G);
            var pBig = new BigInteger(_handshakeData.P);
            var xcBig = new BigInteger(Xc);

            // Note: these calculations are not done in constant-time, but should be a minor problem since we generate a new key for each session
            var Yc = Utils.BigEndianFromBigInteger(BigInteger.ModPow(gBig, xcBig, pBig));
            var Z = Utils.BigEndianFromBigInteger(BigInteger.ModPow(new BigInteger(_handshakeData.Ys), xcBig, pBig));

            SetMasterSecret(Z);

            // Yc
            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)Yc.Length);
            Buffer.BlockCopy(Yc, 0, _writeBuf, offset, Yc.Length);
            offset += Yc.Length;

            return HandshakeType.ClientKeyExchange;
        }

        HandshakeType SendClientKeyExchangeEcdh(ref int offset)
        {
            var pkParameters = _handshakeData.CertList[0].GetKeyAlgorithmParameters();
            var pkKey = _handshakeData.CertList[0].GetPublicKey();

            var curve = EllipticCurve.GetCurveFromParameters(pkParameters);
            if (curve == null)
                SendAlertFatal(AlertDescription.IllegalParameter);

            var Qax = new EllipticCurve.BigInt(pkKey, 1, curve.curveByteLen);
            var Qay = new EllipticCurve.BigInt(pkKey, 1 + curve.curveByteLen, curve.curveByteLen);

            byte[] preMasterSecret;
            curve.Ecdh(Qax, Qay, _rng, out preMasterSecret, out var publicPoint);

            SetMasterSecret(preMasterSecret);
            _writeBuf[offset++] = (byte)(1 + 2 * curve.curveByteLen); // Point length
            _writeBuf[offset++] = 4; // Uncompressed
            offset += publicPoint.x.ExportToBigEndian(_writeBuf, offset, curve.curveByteLen);
            offset += publicPoint.y.ExportToBigEndian(_writeBuf, offset, curve.curveByteLen);
            publicPoint.Clear();

            return HandshakeType.ClientKeyExchange;
        }

        HandshakeType SendClientKeyExchangeEcdhe(ref int offset)
        {
            var ec = _handshakeData.EcCurve;
            if (ec == null)
            {
                // Server Key Exchange with ECDHE was not received
                SendAlertFatal(AlertDescription.UnexpectedMessage);
            }

            ec.Ecdh(_handshakeData.EcX, _handshakeData.EcY, _rng, out var preMasterSecret, out var publicPoint);
            SetMasterSecret(preMasterSecret);

            var byteLen = ec.curveByteLen;

            _writeBuf[offset++] = (byte)(1 + 2 * byteLen); // Point length
            _writeBuf[offset++] = 4; // Uncompressed
            offset += publicPoint.x.ExportToBigEndian(_writeBuf, offset, byteLen);
            offset += publicPoint.y.ExportToBigEndian(_writeBuf, offset, byteLen);
            publicPoint.Clear();

            return HandshakeType.ClientKeyExchange;
        }

        void ParseCertificateRequest(byte[] buf, ref int pos)
        {
            int lenCertificateTypes = buf[pos++];
            var certificateTypes = new List<ClientCertificateType>();
            for (var i = 0; i < lenCertificateTypes; i++)
            {
                certificateTypes.Add((ClientCertificateType)buf[pos++]);
            }

            var supportedSignatureAlgorithms = new List<Tuple<TlsHashAlgorithm, SignatureAlgorithm>>();
            if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2)
            {
                var lenSignatureAlgorithms = Utils.ReadUInt16(buf, ref pos);
                if ((lenSignatureAlgorithms & 1) == 1)
                {
                    SendAlertFatal(AlertDescription.IllegalParameter);
                }
                for (var i = 0; i < lenSignatureAlgorithms; i += 2)
                {
                    var ha = (TlsHashAlgorithm)buf[pos++];
                    var sa = (SignatureAlgorithm)buf[pos++];
                    supportedSignatureAlgorithms.Add(Tuple.Create(ha, sa));
                }
            }

            var lenCertificateAuthorities = Utils.ReadUInt16(buf, ref pos);
            var endCertificateAuthorities = pos + lenCertificateAuthorities;
            var certificateAuthorities = new List<string>();
            while (pos < endCertificateAuthorities)
            {
                var lenCertificateAuthority = Utils.ReadUInt16(buf, ref pos);
                var certificateAuthorityBytes = new byte[lenCertificateAuthority];
                Buffer.BlockCopy(buf, pos, certificateAuthorityBytes, 0, lenCertificateAuthority);
                pos += lenCertificateAuthority;
                var certificateAuthority = new X500DistinguishedName(certificateAuthorityBytes).Name;
                certificateAuthorities.Add(certificateAuthority);
            }

            _handshakeData.CertificateTypes = certificateTypes;
            _handshakeData.SupportedSignatureAlgorithms = supportedSignatureAlgorithms;
            _handshakeData.CertificateAuthorities = certificateAuthorities;
        }

        HandshakeType SendClientCertificate(ref int offset)
        {
            X509Chain selected = null;
            if (_clientCertificates != null)
            {
                foreach (var cert in _clientCertificates)
                {
                    var cert2 = cert as X509Certificate2;
                    if (cert2 == null)
                        cert2 = new X509Certificate2(cert
#if !(NET45 || NET451)
                            .Export(X509ContentType.Cert)
#endif
                            );
                    var chain = new X509Chain();
                    chain.Build(cert2);
                    foreach (var elem in chain.ChainElements)
                    {
                        if (_handshakeData.CertificateAuthorities.Contains(elem.Certificate.Issuer))
                            selected = chain;
                    }
                    // TODO: Verify cert2 against _handshakeData.CertificateTypes
                    if (selected != null)
                        break;
                }
            }

            if (selected == null)
            {
                // Did not find any good certificate, send an empty list...
                offset += Utils.WriteUInt24(_writeBuf, offset, 0);
            }
            else
            {
                // We send the user's certificate and additional parent certificates found in the machine/user certificate store
                var byteEncoded = new byte[selected.ChainElements.Count][];
                for (var i = 0; i < selected.ChainElements.Count; i++)
                {
                    byteEncoded[i] = selected.ChainElements[i].Certificate.Export(X509ContentType.Cert);
                }
                offset += Utils.WriteUInt24(_writeBuf, offset, byteEncoded.Sum(a => 3 + a.Length));

                foreach (var arr in byteEncoded)
                {
                    offset += Utils.WriteUInt24(_writeBuf, offset, arr.Length);
                    Buffer.BlockCopy(arr, 0, _writeBuf, offset, arr.Length);
                    offset += arr.Length;
                }
            }
            _handshakeData.SelectedClientCertificate = selected;

            return HandshakeType.Certificate;
        }

        HandshakeType SendCertificateVerify(ref int offset)
        {
#if NET45 || NET451
            var key = new X509Certificate2(_clientCertificates[0]).PrivateKey;

            var keyDsa = key as DSACryptoServiceProvider;
            var keyRsa = key as RSACryptoServiceProvider;
#else
            var keyRsa = new X509Certificate2(_clientCertificates[0].Export(X509ContentType.Cert)).GetRSAPrivateKey();
#endif

            byte[] signature = null, hash = null;

#if NET45 || NET451
            if (keyDsa != null)
            {
                if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2 && !_handshakeData.SupportedSignatureAlgorithms.Contains(Tuple.Create(TlsHashAlgorithm.SHA1, SignatureAlgorithm.DSA)))
                {
                    SendAlertFatal(AlertDescription.HandshakeFailure, "Server does not support client certificate sha1-dsa signatures");
                }
                hash = _handshakeData.CertificateVerifyHash_SHA1.Final();
                signature = keyDsa.SignHash(hash, Utils.HashNameToOID["SHA1"]);

                // Convert to DER
                var r = new byte[21];
                var s = new byte[21];
                Array.Reverse(signature, 0, 20);
                Array.Reverse(signature, 20, 20);
                Buffer.BlockCopy(signature, 0, r, 0, 20);
                Buffer.BlockCopy(signature, 20, s, 0, 20);
                r = new BigInteger(r).ToByteArray();
                s = new BigInteger(s).ToByteArray();
                Array.Reverse(r);
                Array.Reverse(s);

                signature = new byte[r.Length + s.Length + 6];
                signature[0] = 0x30; // SEQUENCE
                signature[1] = (byte)(signature.Length - 2);
                signature[2] = 0x02; // INTEGER
                signature[3] = (byte)r.Length;
                Buffer.BlockCopy(r, 0, signature, 4, r.Length);
                signature[4 + r.Length] = 0x02; // INTEGER
                signature[4 + r.Length + 1] = (byte)s.Length;
                Buffer.BlockCopy(s, 0, signature, 4 + r.Length + 2, s.Length);

                if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2)
                {
                    _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA1;
                    _writeBuf[offset++] = (byte)SignatureAlgorithm.DSA;
                }
            }
            else
#endif
            if (keyRsa != null)
            {
                if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2 && !_handshakeData.SupportedSignatureAlgorithms.Contains(Tuple.Create(TlsHashAlgorithm.SHA1, SignatureAlgorithm.RSA)))
                {
                    SendAlertFatal(AlertDescription.HandshakeFailure, "Server does not support client certificate sha1-rsa signatures");
                }

                hash = _handshakeData.CertificateVerifyHash_SHA1.Final();
                byte[] md5Hash = null;
                if (_handshakeData.CertificateVerifyHash_MD5 != null)
                {
                    md5Hash = _handshakeData.CertificateVerifyHash_MD5.Final();
                }

                // NOTE: It seems problematic to support other hash algorithms than SHA1 since the PrivateKey included in the certificate
                // often uses an old Crypto Service Provider (Microsoft Base Cryptographic Provider v1.0) instead of Microsoft Enhanced RSA and AES Cryptographic Provider.
                // The following out-commented code might work to change CSP.

                //var csp = new RSACryptoServiceProvider().CspKeyContainerInfo;
                //keyRsa = new RSACryptoServiceProvider(new CspParameters(csp.ProviderType, csp.ProviderName, keyRsa.CspKeyContainerInfo.KeyContainerName));

                // TLS 1.0 and 1.1, export private key and calculate md5-sha1 hash and sign manually
                if (_pendingConnState.TlsVersion != TlsVersion.TLSv1_2)
                {
                    var fullHash = new byte[36];
                    Buffer.BlockCopy(md5Hash, 0, fullHash, 0, 16);
                    Buffer.BlockCopy(hash, 0, fullHash, 16, 20);
                    signature = RsaPKCS1.SignRsaPKCS1(keyRsa, fullHash);

                    // BigIntegers have no Dispose/Clear methods, but they contain sensitive data, so force a garbage collection to remove the data.
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, false);
                }
                else
                {
#if NET45 || NET451
                    signature = keyRsa.SignHash(hash, Utils.HashNameToOID["SHA1"]);
#else
                    signature = keyRsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
#endif

                    if (_pendingConnState.TlsVersion == TlsVersion.TLSv1_2)
                    {
                        _writeBuf[offset++] = (byte)TlsHashAlgorithm.SHA1;
                        _writeBuf[offset++] = (byte)SignatureAlgorithm.RSA;
                    }
                }
            }
            else
            {
                SendAlertFatal(AlertDescription.HandshakeFailure);
            }
            _handshakeData.CertificateVerifyHash_SHA1.Dispose();
            _handshakeData.CertificateVerifyHash_SHA1 = null;
            if (_handshakeData.CertificateVerifyHash_MD5 != null)
            {
                _handshakeData.CertificateVerifyHash_MD5.Dispose();
                _handshakeData.CertificateVerifyHash_MD5 = null;
            }

#if NET45 || NET451
            key.Dispose();
#endif

            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)signature.Length);
            Buffer.BlockCopy(signature, 0, _writeBuf, offset, signature.Length);
            offset += signature.Length;

            return HandshakeType.CertificateVerify;
        }

        void SendChangeCipherSpec(ref int offset, int ivLen)
        {
            _writeBuf[offset++] = (byte)ContentType.ChangeCipherSpec;

            offset += Utils.WriteUInt16(_writeBuf, offset, (ushort)_connState.TlsVersion);

            // Length
            offset += 2;

            offset += ivLen;

            // Content
            _writeBuf[offset++] = 1;
        }

        HandshakeType SendFinished(ref int offset)
        {
            byte[] inputHash = _handshakeData.HandshakeHash1.Final();
            byte[] hash = Utils.PRF(_connState.PRFAlgorithm, _connState.MasterSecret, "client finished", inputHash, 12);
            Buffer.BlockCopy(hash, 0, _writeBuf, offset, 12);
            offset += 12;
            if (_connState.SecureRenegotiation)
                _connState.ClientVerifyData = hash;
            else
                Utils.ClearArray(hash);
            Utils.ClearArray(inputHash);

            _handshakeData.HandshakeHash1.Dispose();
            _handshakeData.HandshakeHash1 = null;

            return HandshakeType.Finished;
        }

        void ParseChangeCipherSpec()
        {
            if (_plaintextLen != 1 || _readBuf[_plaintextStart] != 1)
                SendAlertFatal(AlertDescription.IllegalParameter);
            if (_pendingConnState != _connState)
                SendAlertFatal(AlertDescription.UnexpectedMessage);

            // We don't accept buffered handshake messages to be completed after Change Cipher Spec,
            // since they would not be encrypted correctly
            if (_handshakeMessagesBuffer.HasBufferedData)
                SendAlertFatal(AlertDescription.UnexpectedMessage);

            _readConnState.Dispose();
            _readConnState = _connState;
            _pendingConnState = null;
        }

        void ParseFinishedMessage(byte[] buf)
        {
            byte[] hash = Utils.PRF(_connState.PRFAlgorithm, _connState.MasterSecret, "server finished", _handshakeData.HandshakeHash2.Final(), 12);
            if (buf.Length != 4 + 12 || !hash.SequenceEqual(buf.Skip(4)))
                SendAlertFatal(AlertDescription.DecryptError);
            if (_connState.SecureRenegotiation)
                _connState.ServerVerifyData = hash;

            _handshakeData = null;
        }

        #endregion

        #region Alerts

        void SendAlertFatal(AlertDescription description, string message = null)
        {
            throw new ClientAlertException(description, message);
        }

        async Task WriteAlertFatal(AlertDescription description, bool async)
        {
            if (async)
                await _writeLock.WaitAsync();
            else
                _writeLock.Wait();
            try
            {
                _writeBuf[0] = (byte)ContentType.Alert;
                Utils.WriteUInt16(_writeBuf, 1, (ushort)_connState.TlsVersion);
                _writeBuf[5 + _connState.IvLen] = (byte)AlertLevel.Fatal;
                _writeBuf[5 + _connState.IvLen + 1] = (byte)description;
                int endPos = Encrypt(0, 2);

                if (async)
                {
                    await _baseStream.WriteAsync(_writeBuf, 0, endPos);
                    await _baseStream.FlushAsync();
                }
                else
                {
                    _baseStream.Write(_writeBuf, 0, endPos);
                    _baseStream.Flush();
                }

                _baseStream.Dispose();
                _eof = true;
                _closed = true;
                _connState.Dispose();
                _pendingConnState?.Dispose();
                if (_temp512_1 != null)
                {
                    Utils.ClearArray(_temp512_1);
                    Utils.ClearArray(_temp512_2);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        async Task SendClosureAlert(bool async)
        {
            if (async)
                await _writeLock.WaitAsync();
            else
                _writeLock.Wait();
            try
            {
                _writeBuf[0] = (byte)ContentType.Alert;
                Utils.WriteUInt16(_writeBuf, 1, (ushort)_connState.TlsVersion);
                _writeBuf[5 + _connState.IvLen] = (byte)AlertLevel.Warning;
                _writeBuf[5 + _connState.IvLen + 1] = (byte)AlertDescription.CloseNotify;
                int endPos = Encrypt(0, 2);
                if (async)
                {
                    await _baseStream.WriteAsync(_writeBuf, 0, endPos);
                    await _baseStream.FlushAsync();
                }
                else
                {
                    _baseStream.Write(_writeBuf, 0, endPos);
                    _baseStream.Flush();
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        async Task HandleAlertMessage(bool async)
        {
            if (_plaintextLen != 2)
                SendAlertFatal(AlertDescription.DecodeError);

            var alertLevel = (AlertLevel)_readBuf[_plaintextStart];
            var alertDescription = (AlertDescription)_readBuf[_plaintextStart + 1];

            switch (alertDescription)
            {
                case AlertDescription.CloseNotify:
                    _eof = true;
                    try
                    {
                        await SendClosureAlert(async);
                    }
                    catch (IOException)
                    {
                        // Don't care about this fails (the other end has closed the connection so we couldn't write)
                    }

                    // Now, did the stream end normally (end of stream) or was the connection reset?
                    // We read 0 bytes to find out. If end of stream, it will just return 0, otherwise an exception will be thrown, as we want.
                    // TODO: what to do with _closed? (_eof is true)
                    if (async)
                        await _baseStream.ReadAsync(_readBuf, 0, 0);
                    else
                        _baseStream.Read(_readBuf, 0, 0);

                    _baseStream.Dispose();
                    break;
                default:
                    if (alertLevel == AlertLevel.Fatal)
                    {
                        _eof = true;
                        _baseStream.Dispose();
                        Dispose();
                        throw new IOException("TLS Fatal alert: " + alertDescription);
                    }
                    break;
            }
        }

        #endregion

        void ResetWritePos()
        {
            _writePos = _connState.WriteStartPos;
        }
	
        void CheckNotClosed()
        {
            if (_closed)
            {
                throw new ObjectDisposedException("Stream is closed");
            }
        }

        void EnqueueReadData(bool allowApplicationData)
        {
            if (_contentType == ContentType.ApplicationData && allowApplicationData)
            {
                if (_bufferedReadData == null)
                    _bufferedReadData = new Queue<byte[]>();

                if (_lenBufferedReadData + _plaintextLen <= MaxBufferedReadData)
                {
                    var bytes = new byte[_plaintextLen];
                    Buffer.BlockCopy(_readBuf, _plaintextStart, bytes, 0, _plaintextLen);
                    _bufferedReadData.Enqueue(bytes);
                    return;
                }
            }
            else if (_contentType == ContentType.Alert)
            {
                // TODO: Should this whole method be async?
                HandleAlertMessage(false).GetAwaiter().GetResult();
            }
            SendAlertFatal(AlertDescription.UnexpectedMessage);
        }

        public async Task PerformInitialHandshake(string hostName, X509CertificateCollection clientCertificates, System.Net.Security.RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool checkCertificateRevocation, bool async)
        {
            if (_connState.CipherSuite != null || _pendingConnState != null || _closed)
                throw new InvalidOperationException("Already performed initial handshake");

            _hostName = hostName;
            _clientCertificates = clientCertificates;
            _remoteCertificationValidationCallback = remoteCertificateValidationCallback;
            _checkCertificateRevocation = checkCertificateRevocation;

            try
            {
                int offset = 0;
                SendHandshakeMessage(SendClientHello, ref offset, 0);
                if (async)
                {
                    await _baseStream.WriteAsync(_writeBuf, 0, offset);
                    await _baseStream.FlushAsync();
                }
                else
                {
                    _baseStream.Write(_writeBuf, 0, offset);
                    _baseStream.Flush();
                }
                await GetInitialHandshakeMessages(async);

                var keyExchange = _connState.CipherSuite.KeyExchange;
                if (keyExchange == KeyExchange.RSA || keyExchange == KeyExchange.ECDH_ECDSA || keyExchange == KeyExchange.ECDH_RSA)
                {
                    // Don't use false start for non-forward-secrecy key exchanges; we have to wait for the finished message and verify it

                    await WaitForHandshakeCompleted(true, async);
                }
            }
            catch (ClientAlertException e)
            {
                await WriteAlertFatal(e.Description, async);
                throw new IOException(e.ToString(), e);
            }
        }

        int WriteSpaceLeft => 8192 + _connState.WriteStartPos - _writePos;

        #region Stream overrides

        public override void Write(byte[] buffer, int offset, int len)
            => Write(buffer, offset, len, false).GetAwaiter().GetResult();

        // Note: As this is an internal component, we don't set NoSynchronizationContextScope
        public override Task WriteAsync(byte[] buffer, int offset, int len, CancellationToken cancellationToken)
            => Write(buffer, offset, len, true);

        async Task Write(byte[] buffer, int offset, int len, bool async)
        {
#if CHECK_ARGUMENTS
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (len < 0 || len > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("len");
#endif

            if (async)
                await _writeLock.WaitAsync();
            else
                _writeLock.Wait();
            try
            {
                CheckNotClosed();
                if (_connState.CipherSuite == null)
                {
                    throw new InvalidOperationException("Must perform initial handshake before writing application data");
                }
                
                for (;;)
                {
                    int toWrite = Math.Min(WriteSpaceLeft, len);
                    Buffer.BlockCopy(buffer, offset, _writeBuf, _writePos, toWrite);
                    _writePos += toWrite;
                    offset += toWrite;
                    len -= toWrite;
                    if (len == 0)
                    {
                        return;
                    }
                    await Flush(async);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public override void Flush()
        {
            _writeLock.Wait();
            try
            {
                Flush(false).GetAwaiter().GetResult();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public override async Task FlushAsync(CancellationToken cancellataionToken)
        {
            await _writeLock.WaitAsync();
            try
            {
                await Flush(true);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        async Task Flush(bool async)
        {
            CheckNotClosed();
            if (_writePos > _connState.WriteStartPos)
            {
                _writeBuf[0] = (byte)ContentType.ApplicationData;
                Utils.WriteUInt16(_writeBuf, 1, (ushort)_connState.TlsVersion);

                int offset;
                if (_connState.TlsVersion == TlsVersion.TLSv1_0)
                {
                    // To avoid the BEAST attack, we add an empty application data record
                    offset = Encrypt(0, 0);
                    _writeBuf[offset] = (byte)ContentType.ApplicationData;
                    Utils.WriteUInt16(_writeBuf, offset + 1, (ushort)_connState.TlsVersion);
                }
                else
                {
                    offset = 0;
                }
                int endPos = Encrypt(offset, _writePos - offset - 5 - _connState.IvLen);
                if (async)
                {
                    await _baseStream.WriteAsync(_writeBuf, 0, endPos);
                    await _baseStream.FlushAsync();
                }
                else
                {
                    _baseStream.Write(_writeBuf, 0, endPos);
                    _baseStream.Flush();
                }
                ResetWritePos();
            }
        }

        public override int Read(byte[] buffer, int offset, int len)
            => Read(buffer, offset, len, false, false, false).GetAwaiter().GetResult();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int len, CancellationToken cancellationToken)
            => Read(buffer, offset, len, false, false, true);

        async Task<int> Read(byte[] buffer, int offset, int len, bool onlyProcessHandshake, bool readNewDataIfAvailable, bool async)
        {
#if CHECK_ARGUMENTS
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException("offset");
            if (len < 0 || len > buffer.Length - offset)
                throw new ArgumentOutOfRangeException("len");
#endif
            
            try
            {
                for (; ; )
                {
                    // Handshake messages take priority over application data
                    if (_handshakeMessagesBuffer.Messages.Count > 0)
                    {
                        if (_waitingForFinished)
                        {
                            if (_handshakeMessagesBuffer.Messages[0][0] != (byte)HandshakeType.Finished)
                            {
                                SendAlertFatal(AlertDescription.UnexpectedMessage);
                            }
                            ParseFinishedMessage(_handshakeMessagesBuffer.Messages[0]);
                            _waitingForFinished = false;
                            _handshakeMessagesBuffer.RemoveFirst(); // There may be Hello Requests after Finished
                        }
                        if (_handshakeMessagesBuffer.Messages.Count > 0)
                        {
                            if (_pendingConnState == null)
                            {
                                // Not currently renegotiating, should be a hello request
                                if (_handshakeMessagesBuffer.Messages.Any(m => !HandshakeMessagesBuffer.IsHelloRequest(m)))
                                {
                                    SendAlertFatal(AlertDescription.UnexpectedMessage);
                                }
                                if (async)
                                    await _writeLock.WaitAsync();
                                else
                                    _writeLock.Wait();
                                try
                                {
                                    await Flush(async);
                                    int writeOffset = 0;
                                    SendHandshakeMessage(SendClientHello, ref writeOffset, _connState.IvLen);
                                    if (async)
                                    {
                                        await _baseStream.WriteAsync(_writeBuf, 0, writeOffset);
                                        await _baseStream.FlushAsync();
                                    }
                                    else
                                    {
                                        _baseStream.Write(_writeBuf, 0, writeOffset);
                                        _baseStream.Flush();
                                    }

                                    _handshakeMessagesBuffer.ClearMessages();

                                    // OpenSSL violates TLS 1.2 spec by not allowing interleaved application data and handshake data,
                                    // so we wait for the ServerHello + ... + ServerHelloDone messages and then send the rest of our handshake messages.
                                    await GetInitialHandshakeMessages(async, true);
                                    // Now we do a "false start" regardless of Cipher Suite
                                }
                                finally
                                {
                                    _writeLock.Release();
                                }
                            }
                            else
                            {
                                // Ignore hello request messages when we are renegotiating,
                                // by setting ignoreHelloRequests to false below in AddBytes, if _pendingConnState != null
                                if (_waitingForChangeCipherSpec)
                                {
                                    SendAlertFatal(AlertDescription.UnexpectedMessage);
                                }
                            }
                        }
                    }
                    if (_eof)
                    {
                        return 0;
                    }
                    if (onlyProcessHandshake)
                    {
                        // No data is available in our buffer and we're not waiting for the handshake to complete
                        if (_readStart == _readEnd && _decryptedReadPos == _decryptedReadEnd && !(_pendingConnState != null || _waitingForChangeCipherSpec || _waitingForFinished))
                        {
                            if (!readNewDataIfAvailable || !((NetworkStream)_baseStream).DataAvailable)
                                return 0;
                            // Else there is data available in the NetworkStream and we want to look at it. The record will be read and processed further down.
                        }

                        // There is application data available in our buffer
                        if (_bufferedReadData != null || _decryptedReadPos < _decryptedReadEnd)
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (_bufferedReadData != null)
                        {
                            var buf = _bufferedReadData.Peek();
                            var toRead = Math.Min(buf.Length - _posBufferedReadData, len);
                            Buffer.BlockCopy(buf, _posBufferedReadData, buffer, offset, toRead);
                            _posBufferedReadData += toRead;
                            _lenBufferedReadData -= toRead;
                            if (_posBufferedReadData == buf.Length)
                            {
                                _bufferedReadData.Dequeue();
                                _posBufferedReadData = 0;
                                if (_bufferedReadData.Count == 0)
                                {
                                    _bufferedReadData = null;
                                }
                            }
                            return toRead;
                        }
                        if (_decryptedReadPos < _decryptedReadEnd)
                        {
                            var toRead = Math.Min(_decryptedReadEnd - _decryptedReadPos, len);
                            Buffer.BlockCopy(_readBuf, _decryptedReadPos, buffer, offset, toRead);
                            _decryptedReadPos += toRead;
                            return toRead;
                        }
                    }
                    if (!await ReadRecord(async))
                    {
                        _eof = true;
                        return 0;
                    }
                    Decrypt();

                    if (_contentType == ContentType.ApplicationData)
                    {
                        if (_readConnState.ReadAes == null || _waitingForFinished)
                        {
                            // Bad state, cannot read application data with null cipher, or until finished has been received
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        }
                        _decryptedReadPos = _plaintextStart;
                        _decryptedReadEnd = _decryptedReadPos + _plaintextLen;
                        continue;
                    }
                    else if (_contentType == ContentType.ChangeCipherSpec)
                    {
                        ParseChangeCipherSpec();
                        _waitingForChangeCipherSpec = false;
                        _waitingForFinished = true;
                        continue;
                    }
                    else if (_contentType == ContentType.Handshake)
                    {
                        _handshakeMessagesBuffer.AddBytes(_readBuf, _plaintextStart, _plaintextLen, _pendingConnState != null ?
                            HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IgnoreHelloRequestsUntilFinished :
                            HandshakeMessagesBuffer.IgnoreHelloRequestsSetting.IncludeHelloRequests);
                        if (_handshakeMessagesBuffer.Messages.Count > 2)
                        {
                            // There can never be more than 2 handshake messages (finished + hello request)
                            SendAlertFatal(AlertDescription.UnexpectedMessage);
                        }
                        // The handshake message(s) will be processed in the loop's next iteration
                    }
                    else if (_contentType == ContentType.Alert)
                    {
                        await HandleAlertMessage(async);
                    }
                    else
                    {
                        SendAlertFatal(AlertDescription.UnexpectedMessage);
                    }
                }
            }
            catch (ClientAlertException e)
            {
                await WriteAlertFatal(e.Description, async);
                throw new IOException(e.ToString(), e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            // NOTE: If currently in the middle of a handshake, we send closure alert. That should be ok.
            if (!_closed && disposing)
            {
                try
                {
                    if (!_eof)
                    {
                        // TODO: Ok or not if this throws an exception?

                        SendClosureAlert(false).GetAwaiter().GetResult();
                        _baseStream.Dispose();
                    }
                }
                finally
                {
                    _closed = true;
                    _connState.Dispose();
                    _pendingConnState?.Dispose();
                    if (_temp512_1 != null)
                    {
                        Utils.ClearArray(_temp512_1);
                        Utils.ClearArray(_temp512_2);
                    }
                }
            }
            if (disposing)
            {
                _rng.Dispose();
            }
            base.Dispose(disposing);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }
        public override bool CanRead => !_closed && _connState.IsAuthenticated && _baseStream.CanRead;
        public override bool CanWrite => !_closed && _connState.IsAuthenticated && _baseStream.CanWrite;

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override bool CanSeek => false;

        #endregion

        /// <summary>
        /// This method checks whether there are at least 1 byte that can be read in the buffer.
        /// If not, but there are renegotiation messages in the buffer, these are first processed.
        /// This method should be called between each Read and Write to make sure the buffer is empty before writing.
        /// Only when this method returns false it is safe to call Write.
        /// </summary>
        /// <param name="checkNetworkStream">Whether we should also look in the underlying NetworkStream</param>
        /// <returns>Whether there is available application data</returns>
        public bool HasBufferedReadData(bool checkNetworkStream)
        {
            if (_closed)
                return false;
            if (_lenBufferedReadData > 0)
                return true;
            if (_decryptedReadPos < _decryptedReadEnd)
                return true;
            if (_readStart == _readEnd && !checkNetworkStream)
                return false;

            // Otherwise there may be buffered unprocessed packets. We check if any of them is application data.
            int pos = _readStart;
            while (pos < _readEnd)
            {
                if ((ContentType)_readBuf[pos] == ContentType.ApplicationData)
                    return true;
                if (pos + 5 >= _readEnd)
                    break;
                pos += 3;
                int recordLen = Utils.ReadUInt16(_readBuf, ref pos);
                pos += recordLen;
            }

            // If none of them were application data, they should be handshake messages/change cipher suite.
            // Process potential renegotiation, but stop when application data is received, or the buffer(s) becomes empty.
            return Read(null, 0, 0, true, checkNetworkStream, false).GetAwaiter().GetResult() == 1;
        }
    }
}
