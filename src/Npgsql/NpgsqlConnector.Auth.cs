﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using Npgsql.Tls;
namespace Npgsql
{
    partial class NpgsqlConnector
    {
        async Task Authenticate(string username, NpgsqlTimeout timeout, bool async)
        {
            Log.Trace("Authenticating...", Id);

            var msg = await ReadExpecting<AuthenticationRequestMessage>(async);
            timeout.Check();
            switch (msg.AuthRequestType)
            {
            case AuthenticationRequestType.AuthenticationOk:
                return;

            case AuthenticationRequestType.AuthenticationCleartextPassword:
                await AuthenticateCleartext(async);
                return;

            case AuthenticationRequestType.AuthenticationMD5Password:
                await AuthenticateMD5(username, ((AuthenticationMD5PasswordMessage)msg).Salt, async);
                return;

            case AuthenticationRequestType.AuthenticationSASLInit:
                await AuthenticateSASL(username, ((AuthenticationSASLMessage)msg).ProposedScheme, async);
                return;

            case AuthenticationRequestType.AuthenticationGSS:
            case AuthenticationRequestType.AuthenticationSSPI:
                await AuthenticateGSS(async);
                return;

            case AuthenticationRequestType.AuthenticationGSSContinue:
                throw new NpgsqlException("Can't start auth cycle with AuthenticationGSSContinue");

            default:
                throw new NotSupportedException($"Authentication method not supported (Received: {msg.AuthRequestType})");
            }
        }

        async Task AuthenticateCleartext(bool async)
        {
            var passwd = Settings.Password;

            // No password was provided. Attempt to pull the password from the pgpass file.
            if (passwd == null)
            {
                var matchingEntry = PgPassFile.Load(Settings.Passfile)?.GetFirstMatchingEntry(Settings.Host, Settings.Port, Settings.Database, Settings.Username);
                if (matchingEntry != null)
                {
                    Log.Trace("Taking password from pgpass file");
                    passwd = matchingEntry.Password;
                }
            }

            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in cleartext)");

            await PasswordMessage
                .CreateClearText(passwd)
                .Write(WriteBuffer, async);
            await WriteBuffer.Flush(async);
            await ReadExpecting<AuthenticationRequestMessage>(async);
        }

        async Task AuthenticateSASL(string username, string scheme, bool async)
        {
            // Postgresql >= 10.0 support only SCRAM-SHA-256
            if (scheme == "SCRAM-SHA-256")
            {
                await AuthenticateSCRAM(username, scheme, async);
            }
            else
                throw new NpgsqlException("SASL authentication scheme ["+scheme+"] not supported");
        }

        async Task AuthenticateSCRAM(string username, string scheme, bool async)
        {
            var scram = new SCRAM(scheme, username);
            await SCRAMMessage
                 .CreateClientFirstMessage(scram)
                 .Write(WriteBuffer, async);
            await WriteBuffer.Flush(async);

            var msg = await ReadExpecting<AuthenticationRequestMessage>(async);


            // Expecting SCRAMServerFirst message 
            if (((AuthenticationSASLMessage)msg).AuthRequestType == AuthenticationRequestType.AuthenticationSCRAMFirst)
            {
                scram.parseServerFirstMessage(((AuthenticationSASLMessage)msg).ServerFirst);
                await AuthenticateSASLContinue(scram, async);
            }
            else
                throw new NpgsqlException("[SCRAM] SCRAMServerFirst message expected");
        }

        async Task AuthenticateSASLContinue(SCRAM scram, bool async)
        {
            var passwd = Settings.Password;
            if (passwd == null)
            {
                // No password was provided. Attempt to pull the password from the pgpass file.
                var matchingEntry = PgPassFile.Load(Settings.Passfile)?.GetFirstMatchingEntry(Settings.Host, Settings.Port, Settings.Database, Settings.Username);
                if (matchingEntry != null)
                {
                    Log.Trace("Taking password from pgpass file");
                    passwd = matchingEntry.Password;
                }
            }

            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in SCRAM-SHA-256)");

            scram.Password = passwd;

            await SCRAMMessage
                .CreateClientFinalMessage(scram)
                .Write(WriteBuffer, async);
            await WriteBuffer.Flush(async);

            var msg = await ReadExpecting<AuthenticationRequestMessage>(async);
            // Expecting SCRAMServerFinal message
            if (((AuthenticationSASLMessage)msg).AuthRequestType == AuthenticationRequestType.AuthenticationSCRAMFinal)
            { 
                AuthenticateSASLFinal(scram, ((AuthenticationSASLMessage)msg).ServerProof);
            }
            else
                throw new NpgsqlException("[SCRAM] SCRAMServerFinal message expected");

            await ReadExpecting<AuthenticationRequestMessage>(async);
        }
        void AuthenticateSASLFinal(SCRAM scram, string serverFinalMsg)
        {
            scram.verifyServer(serverFinalMsg);
        }

        async Task AuthenticateMD5(string username, byte[] salt, bool async)
        {
            var passwd = Settings.Password;

            // No password was provided. Attempt to pull the password from the pgpass file.
            if (passwd == null)
            {
                var matchingEntry = PgPassFile.Load(Settings.Passfile)?.GetFirstMatchingEntry(Settings.Host, Settings.Port, Settings.Database, Settings.Username);
                if (matchingEntry != null)
                {
                    Log.Trace("Taking password from pgpass file");
                    passwd = matchingEntry.Password;
                }
            }

            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in MD5)");

            await PasswordMessage
                .CreateMD5(passwd, username, salt)
                .Write(WriteBuffer, async);
            await WriteBuffer.Flush(async);
            await ReadExpecting<AuthenticationRequestMessage>(async);
        }

#pragma warning disable CA1801 // Review unused parameters
        async Task AuthenticateGSS(bool async)
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
                    if (async)
                        negotiateStream.AuthenticateAsClient(CredentialCache.DefaultNetworkCredentials, targetName);
                    else
                        await negotiateStream.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, targetName);
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
#pragma warning restore CA1801 // Review unused parameters

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
                    .Write(_connector.WriteBuffer, false);
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

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();

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
    }
}
