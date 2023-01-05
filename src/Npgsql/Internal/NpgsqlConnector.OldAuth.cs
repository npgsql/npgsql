using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using static Npgsql.Util.Statics;

namespace Npgsql.Internal;


partial class NpgsqlConnector
{
#if !NET6_0_OR_GREATER
    static byte[] Hi(string str, byte[] salt, int count)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(str));
        var salt1 = new byte[salt.Length + 4];
        byte[] hi, u1;

        Buffer.BlockCopy(salt, 0, salt1, 0, salt.Length);
        salt1[salt1.Length - 1] = 1;

        hi = u1 = hmac.ComputeHash(salt1);

        for (var i = 1; i < count; i++)
        {
            var u2 = hmac.ComputeHash(u1);
            NpgsqlConnector.Xor(hi, u2);
            u1 = u2;
        }

        return hi;
    }
#endif

#if !NET7_0_OR_GREATER
    async Task AuthenticateGSS(bool async)
    {
        var targetName = $"{KerberosServiceName}/{Host}";

        using var negotiateStream = new NegotiateStream(new GSSPasswordMessageStream(this), true);
        try
        {
            if (async)
                await negotiateStream.AuthenticateAsClientAsync(CredentialCache.DefaultNetworkCredentials, targetName);
            else
                negotiateStream.AuthenticateAsClient(CredentialCache.DefaultNetworkCredentials, targetName);
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
    sealed class GSSPasswordMessageStream : Stream
    {
        readonly NpgsqlConnector _connector;
        int _leftToWrite;
        int _leftToRead, _readPos;
        byte[]? _readBuf;

        internal GSSPasswordMessageStream(NpgsqlConnector connector)
            => _connector = connector;

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
            => Write(buffer, offset, count, true, cancellationToken);

        public override void Write(byte[] buffer, int offset, int count)
            => Write(buffer, offset, count, false).GetAwaiter().GetResult();

        async Task Write(byte[] buffer, int offset, int count, bool async, CancellationToken cancellationToken = default)
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
            await _connector.WritePassword(buffer, offset, count, async, cancellationToken);
            await _connector.Flush(async, cancellationToken);
            _leftToWrite -= count;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
            => Read(buffer, offset, count, true, cancellationToken);

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer, offset, count, false).GetAwaiter().GetResult();

        async Task<int> Read(byte[] buffer, int offset, int count, bool async, CancellationToken cancellationToken = default)
        {
            if (_leftToRead == 0)
            {
                var response = ExpectAny<AuthenticationRequestMessage>(await _connector.ReadMessage(async), _connector);
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
            Array.Copy(_readBuf!, _readPos, buffer, offset, count);
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

    sealed class AuthenticationCompleteException : Exception { }
#endif
}
