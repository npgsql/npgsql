using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using Npgsql.Tls;

namespace Npgsql
{
    partial class NpgsqlConnector
    {
        async Task Authenticate(string username, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            Log.Trace("Authenticating...", Id);

            var msg = await ReadExpecting<AuthenticationRequestMessage>(async);
            timeout.Check();
            switch (msg.AuthRequestType)
            {
            case AuthenticationRequestType.AuthenticationOk:
                return;

            case AuthenticationRequestType.AuthenticationCleartextPassword:
                await AuthenticateCleartext(async, cancellationToken);
                return;

            case AuthenticationRequestType.AuthenticationMD5Password:
                await AuthenticateMD5(username, ((AuthenticationMD5PasswordMessage)msg).Salt, async, cancellationToken);
                return;

            case AuthenticationRequestType.AuthenticationSASL:
                await AuthenticateSASL(((AuthenticationSASLMessage)msg).Mechanisms, async, cancellationToken);
                return;

            case AuthenticationRequestType.AuthenticationGSS:
            case AuthenticationRequestType.AuthenticationSSPI:
                await AuthenticateGSS(async, cancellationToken);
                return;

            case AuthenticationRequestType.AuthenticationGSSContinue:
                throw new NpgsqlException("Can't start auth cycle with AuthenticationGSSContinue");

            default:
                throw new NotSupportedException($"Authentication method not supported (Received: {msg.AuthRequestType})");
            }
        }

        async Task AuthenticateCleartext(bool async, CancellationToken cancellationToken)
        {
            var passwd = GetPassword();
            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in cleartext)");

            await PasswordMessage
                .CreateClearText(passwd)
                .Write(WriteBuffer, async, cancellationToken);
            await WriteBuffer.Flush(async);
            await ReadExpecting<AuthenticationRequestMessage>(async);
        }

        async Task AuthenticateSASL(List<string> mechanisms, bool async, CancellationToken cancellationToken)
        {
            // At the time of writing PostgreSQL only supports SCRAM-SHA-256
            if (!mechanisms.Contains("SCRAM-SHA-256"))
                throw new NpgsqlException("No supported SASL mechanism found (only SCRAM-SHA-256 is supported for now). " +
                                          "Mechanisms received from server: " + string.Join(", ", mechanisms));
            var mechanism = "SCRAM-SHA-256";

            var passwd = GetPassword() ??
                         throw new NpgsqlException($"No password has been provided but the backend requires one (in SASL/{mechanism})");

            // Assumption: the write buffer is big enough to contain all our outgoing messages
            var clientNonce = GetNonce();

            await new SASLInitialResponseMessage(mechanism, PGUtil.UTF8Encoding.GetBytes("n,,n=*,r=" + clientNonce))
                .Write(WriteBuffer, async, cancellationToken);
            await WriteBuffer.Flush(async, cancellationToken);

            var saslContinueMsg = await ReadExpecting<AuthenticationSASLContinueMessage>(async);
            if (saslContinueMsg.AuthRequestType != AuthenticationRequestType.AuthenticationSASLContinue)
                throw new NpgsqlException("[SASL] AuthenticationSASLFinal message expected");
            var firstServerMsg = new AuthenticationSCRAMServerFirstMessage(saslContinueMsg.Payload);
            if (!firstServerMsg.Nonce.StartsWith(clientNonce))
                throw new InvalidOperationException("[SCRAM] Malformed SCRAMServerFirst message: server nonce doesn't start with client nonce");

            var scramFinalClientMsg = new SCRAMClientFinalMessage(passwd, firstServerMsg.Nonce, firstServerMsg.Salt, firstServerMsg.Iteration, clientNonce);
            await scramFinalClientMsg.Write(WriteBuffer, async, cancellationToken);
            await WriteBuffer.Flush(async, cancellationToken);

            var saslFinalServerMsg = await ReadExpecting<AuthenticationSASLFinalMessage>(async);
            if (saslFinalServerMsg.AuthRequestType != AuthenticationRequestType.AuthenticationSASLFinal)
                throw new NpgsqlException("[SASL] AuthenticationSASLFinal message expected");
            var scramFinalServerMsg = new AuthenticationSCRAMServerFinalMessage(saslFinalServerMsg.Payload);

            if (scramFinalServerMsg.ServerSignature != Convert.ToBase64String(scramFinalClientMsg.ServerSignature))
                throw new NpgsqlException("[SCRAM] Unable to verify server signature");

            var okMsg = await ReadExpecting<AuthenticationRequestMessage>(async);
            if (okMsg.AuthRequestType != AuthenticationRequestType.AuthenticationOk)
                throw new NpgsqlException("[SASL] Expected AuthenticationOK message");

            string GetNonce()
            {
                var nonceLength = 18;
                var rncProvider = RandomNumberGenerator.Create();
                var nonceBytes = new byte[nonceLength];
                rncProvider.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }
        }

        async Task AuthenticateMD5(string username, byte[] salt, bool async, CancellationToken cancellationToken)
        {
            var passwd = GetPassword();
            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in MD5)");

            await PasswordMessage
                .CreateMD5(passwd, username, salt)
                .Write(WriteBuffer, async, cancellationToken);
            await WriteBuffer.Flush(async);
            await ReadExpecting<AuthenticationRequestMessage>(async);
        }

#pragma warning disable 1998
        async Task AuthenticateGSS(bool async, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            if (!IntegratedSecurity)
                throw new NpgsqlException("SSPI authentication but IntegratedSecurity not enabled");

            using (var negotiateStream = new NegotiateStream(new GSSPasswordMessageStream(this), true))
            {
                try
                {
                    var targetName = $"{KerberosServiceName}/{Host}";
                    // AuthenticateAsClientAsync doesn't exist in .NET 4.5/4.5.1 (only introduced in 4.6)
                    // Conversely, no sync in .NET Standard 1.3 :/
#if NETSTANDARD1_3
                    await negotiateStream.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, targetName);
#else
                    negotiateStream.AuthenticateAsClient(CredentialCache.DefaultNetworkCredentials, targetName);
#endif
                }
                catch (AuthenticationCompleteException)
                {
                    return;
                }
                catch (IOException e) when (e.InnerException is AuthenticationCompleteException)
                {
                    return;
                }
                catch (IOException e) when (e.InnerException is PostgresException)
                {
                    throw e.InnerException;
                }
            }
            throw new NpgsqlException("NegotiateStream.AuthenticateAsClient completed unexpectedly without signaling success");
        }

        /// <summary>
        /// This Stream is placed between NegotiateStream and the socket's NetworkStream (or SSLStream). It intercepts
        /// traffic and performs the following operations:
        /// * Outgoing messages are framed in PostgreSQL's PasswordMessage, and incoming are stripped of it.
        /// * NegotiateStream frames payloads with a 5-byte header, which PostgreSQL doesn't understand. This header is
        /// stripped from outgoing messages and added to incoming ones.
        /// </summary>
        /// <remarks>
        /// See https://referencesource.microsoft.com/#System/net/System/Net/_StreamFramer.cs,16417e735f0e9530,references
        /// </remarks>
        class GSSPasswordMessageStream : Stream
        {
            readonly NpgsqlConnector _connector;
            readonly PasswordMessage _msg;
            int _leftToWrite;
            int _leftToRead, _readPos;
            byte[] _readBuf;

            internal GSSPasswordMessageStream(NpgsqlConnector connector)
            {
                _connector = connector;
                _msg = new PasswordMessage();
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => Write(buffer, offset, count, true);

            public override void Write(byte[] buffer, int offset, int count)
                => Write(buffer, offset, count, false).GetAwaiter().GetResult();

            async Task Write(byte[] buffer, int offset, int count, bool async)
            {
                if (_leftToWrite == 0)
                {
                    // We're writing the frame header, which contains the payload size.
                    _leftToWrite = (buffer[3] << 8) | buffer[4];

                    buffer[0] = 22;
                    if (buffer[1] != 1)
                        throw new NotSupportedException($"Received frame header major v {buffer[1]} (different from 1)");
                    if (buffer[2] != 0)
                        throw new NotSupportedException($"Received frame header minor v {buffer[2]} (different from 0)");

                    // In case of payload data in the same buffer just after the frame header
                    if (count == 5)
                        return;
                    count -= 5;
                    offset += 5;
                }

                if (count > _leftToWrite)
                    throw new NpgsqlException($"NegotiateStream trying to write {count} bytes but according to frame header we only have {_leftToWrite} left!");
                await _msg.Populate(buffer, offset, count)
                    .Write(_connector.WriteBuffer, false, CancellationToken.None);
                await _connector.WriteBuffer.Flush(async);
                _leftToWrite -= count;
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => Read(buffer, offset, count, true);

            public override int Read(byte[] buffer, int offset, int count)
                => Read(buffer, offset, count, false).GetAwaiter().GetResult();

            async Task<int> Read(byte[] buffer, int offset, int count, bool async)
            {
                if (_leftToRead == 0)
                {
                    var response = await _connector.ReadExpecting<AuthenticationRequestMessage>(async);
                    if (response.AuthRequestType == AuthenticationRequestType.AuthenticationOk)
                        throw new AuthenticationCompleteException();
                    var gssMsg = response as AuthenticationGSSContinueMessage;
                    if (gssMsg == null)
                        throw new NpgsqlException($"Received unexpected authentication request message {response.AuthRequestType}");
                    _readBuf = gssMsg.AuthenticationData;
                    _leftToRead = gssMsg.AuthenticationData.Length;
                    _readPos = 0;
                    buffer[0] = 22;
                    buffer[1] = 1;
                    buffer[2] = 0;
                    buffer[3] = (byte)((_leftToRead >> 8) & 0xFF);
                    buffer[4] = (byte)(_leftToRead & 0xFF);
                    return 5;
                }

                if (count > _leftToRead)
                    throw new NpgsqlException($"NegotiateStream trying to read {count} bytes but according to frame header we only have {_leftToRead} left!");
                count = Math.Min(count, _leftToRead);
                Array.Copy(_readBuf, _readPos, buffer, offset, count);
                _leftToRead -= count;
                return count;
            }

            public override void Flush() { }

            public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
            public override void SetLength(long value) { throw new NotSupportedException(); }

            public override bool CanRead => true;
            public override bool CanWrite => true;
            public override bool CanSeek => false;
            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
        }

        class AuthenticationCompleteException : Exception { }

        [CanBeNull]
        string GetPassword()
        {
            var passwd = Settings.Password;
            if (passwd != null)
                return passwd;

            // No password was provided. Attempt to pull the password from the pgpass file.
            var matchingEntry = PgPassFile.LoadDefaultFile()?.GetFirstMatchingEntry(Settings.Host, Settings.Port, Settings.Database, Settings.Username);
            if (matchingEntry != null)
            {
                Log.Trace("Taking password from pgpass file");
                return matchingEntry.Password;
            }

            return null;
        }
    }
}
