using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Util;
using static Npgsql.Util.Statics;

namespace Npgsql
{
    partial class NpgsqlConnector
    {
        async Task Authenticate(string username, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
        {
            Log.Trace("Authenticating...", Id);

            timeout.CheckAndApply(this);
            var msg = Expect<AuthenticationRequestMessage>(await ReadMessage(async), this);
            switch (msg.AuthRequestType)
            {
            case AuthenticationRequestType.AuthenticationOk:
                return;

            case AuthenticationRequestType.AuthenticationCleartextPassword:
                await AuthenticateCleartext(username, async, cancellationToken);
                return;

            case AuthenticationRequestType.AuthenticationMD5Password:
                await AuthenticateMD5(username, ((AuthenticationMD5PasswordMessage)msg).Salt, async, cancellationToken);
                return;

            case AuthenticationRequestType.AuthenticationSASL:
                await AuthenticateSASL(((AuthenticationSASLMessage)msg).Mechanisms, username, async, cancellationToken);
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

        async Task AuthenticateCleartext(string username, bool async, CancellationToken cancellationToken = default)
        {
            var passwd = GetPassword(username);
            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in cleartext)");

            var encoded = new byte[Encoding.UTF8.GetByteCount(passwd) + 1];
            Encoding.UTF8.GetBytes(passwd, 0, passwd.Length, encoded, 0);

            await WritePassword(encoded, async, cancellationToken);
            await Flush(async, cancellationToken);
            Expect<AuthenticationRequestMessage>(await ReadMessage(async), this);
        }

        async Task AuthenticateSASL(List<string> mechanisms, string username, bool async, CancellationToken cancellationToken = default)
        {
            // At the time of writing PostgreSQL only supports SCRAM-SHA-256 and SCRAM-SHA-256-PLUS
            var supportsSha256 = mechanisms.Contains("SCRAM-SHA-256");
            var supportsSha256Plus = mechanisms.Contains("SCRAM-SHA-256-PLUS");
            if (!supportsSha256 && !supportsSha256Plus)
                throw new NpgsqlException("No supported SASL mechanism found (only SCRAM-SHA-256 and SCRAM-SHA-256-PLUS are supported for now). " +
                                          "Mechanisms received from server: " + string.Join(", ", mechanisms));

            var mechanism = string.Empty;
            var cbindFlag = string.Empty;
            var cbind = string.Empty;
            var successfulBind = false;

            if (supportsSha256Plus)
            {
                var sslStream = (SslStream)_stream;
                if (sslStream.RemoteCertificate is null)
                {
                    Log.Warn("Remote certificate null, falling back to SCRAM-SHA-256");
                }
                else
                {
                    using var remoteCertificate = new X509Certificate2(sslStream.RemoteCertificate);
                    // Checking for hashing algorithms
                    HashAlgorithm? hashAlgorithm = null;
                    var algorithmName = remoteCertificate.SignatureAlgorithm.FriendlyName;
                    if (algorithmName is null)
                    {
                        Log.Warn("Signature algorithm was null, falling back to SCRAM-SHA-256");
                    }
                    else if (algorithmName.StartsWith("sha1", StringComparison.OrdinalIgnoreCase) ||
                             algorithmName.StartsWith("md5", StringComparison.OrdinalIgnoreCase) ||
                             algorithmName.StartsWith("sha256", StringComparison.OrdinalIgnoreCase))
                    {
                        hashAlgorithm = SHA256.Create();
                    }
                    else if (algorithmName.StartsWith("sha384", StringComparison.OrdinalIgnoreCase))
                    {
                        hashAlgorithm = SHA384.Create();
                    }
                    else if (algorithmName.StartsWith("sha512", StringComparison.OrdinalIgnoreCase))
                    {
                        hashAlgorithm = SHA512.Create();
                    }
                    else
                    {
                        Log.Warn($"Support for signature algorithm {algorithmName} is not yet implemented, falling back to SCRAM-SHA-256");
                    }

                    if (hashAlgorithm != null)
                    {
                        using var _ = hashAlgorithm;

                        // RFC 5929
                        mechanism = "SCRAM-SHA-256-PLUS";
                        // PostgreSQL only supports tls-server-end-point binding
                        cbindFlag = "p=tls-server-end-point";
                        // SCRAM-SHA-256-PLUS depends on using ssl stream, so it's fine
                        var cbindFlagBytes = Encoding.UTF8.GetBytes($"{cbindFlag},,");

                        var certificateHash = hashAlgorithm.ComputeHash(remoteCertificate.GetRawCertData());
                        var cbindBytes = cbindFlagBytes.Concat(certificateHash).ToArray();
                        cbind = Convert.ToBase64String(cbindBytes);
                        successfulBind = true;
                        IsScramPlus = true;
                    }
                }
            }

            if (!successfulBind && supportsSha256)
            {
                mechanism = "SCRAM-SHA-256";
                // We can get here if PostgreSQL supports only SCRAM-SHA-256 or there was an error while binding to SCRAM-SHA-256-PLUS
                // So, we set 'n' (client does not support binding) if there was an error while binding
                // or 'y' (client supports but server doesn't) in other case
                cbindFlag = supportsSha256Plus ? "n" : "y";
                cbind = supportsSha256Plus ? "biws" : "eSws";
                successfulBind = true;
                IsScram = true;
            }

            if (!successfulBind)
            {
                // We can get here if PostgreSQL supports only SCRAM-SHA-256-PLUS but there was an error while binding to it
                throw new NpgsqlException("Unable to bind to SCRAM-SHA-256-PLUS, check logs for more information");
            }

            var passwd = GetPassword(username) ??
                         throw new NpgsqlException($"No password has been provided but the backend requires one (in SASL/{mechanism})");

            // Assumption: the write buffer is big enough to contain all our outgoing messages
            var clientNonce = GetNonce();

            await WriteSASLInitialResponse(mechanism, PGUtil.UTF8Encoding.GetBytes($"{cbindFlag},,n=*,r={clientNonce}"), async, cancellationToken);
            await Flush(async, cancellationToken);

            var saslContinueMsg = Expect<AuthenticationSASLContinueMessage>(await ReadMessage(async), this);
            if (saslContinueMsg.AuthRequestType != AuthenticationRequestType.AuthenticationSASLContinue)
                throw new NpgsqlException("[SASL] AuthenticationSASLContinue message expected");
            var firstServerMsg = AuthenticationSCRAMServerFirstMessage.Load(saslContinueMsg.Payload);
            if (!firstServerMsg.Nonce.StartsWith(clientNonce))
                throw new NpgsqlException("[SCRAM] Malformed SCRAMServerFirst message: server nonce doesn't start with client nonce");

            var saltBytes = Convert.FromBase64String(firstServerMsg.Salt);
            var saltedPassword = Hi(passwd.Normalize(NormalizationForm.FormKC), saltBytes, firstServerMsg.Iteration);

            var clientKey = HMAC(saltedPassword, "Client Key");
            byte[] storedKey;
            using (var sha256 = SHA256.Create())
                storedKey = sha256.ComputeHash(clientKey);

            var clientFirstMessageBare = $"n=*,r={clientNonce}";
            var serverFirstMessage = $"r={firstServerMsg.Nonce},s={firstServerMsg.Salt},i={firstServerMsg.Iteration}";
            var clientFinalMessageWithoutProof = $"c={cbind},r={firstServerMsg.Nonce}";

            var authMessage = $"{clientFirstMessageBare},{serverFirstMessage},{clientFinalMessageWithoutProof}";

            var clientSignature = HMAC(storedKey, authMessage);
            var clientProofBytes = Xor(clientKey, clientSignature);
            var clientProof = Convert.ToBase64String(clientProofBytes);

            var serverKey = HMAC(saltedPassword, "Server Key");
            var serverSignature = HMAC(serverKey, authMessage);

            var messageStr = $"{clientFinalMessageWithoutProof},p={clientProof}";

            await WriteSASLResponse(Encoding.UTF8.GetBytes(messageStr), async, cancellationToken);
            await Flush(async, cancellationToken);

            var saslFinalServerMsg = Expect<AuthenticationSASLFinalMessage>(await ReadMessage(async), this);
            if (saslFinalServerMsg.AuthRequestType != AuthenticationRequestType.AuthenticationSASLFinal)
                throw new NpgsqlException("[SASL] AuthenticationSASLFinal message expected");

            var scramFinalServerMsg = AuthenticationSCRAMServerFinalMessage.Load(saslFinalServerMsg.Payload);
            if (scramFinalServerMsg.ServerSignature != Convert.ToBase64String(serverSignature))
                throw new NpgsqlException("[SCRAM] Unable to verify server signature");

            var okMsg = Expect<AuthenticationRequestMessage>(await ReadMessage(async), this);
            if (okMsg.AuthRequestType != AuthenticationRequestType.AuthenticationOk)
                throw new NpgsqlException("[SASL] Expected AuthenticationOK message");

            static string GetNonce()
            {
                using var rncProvider = RandomNumberGenerator.Create();
                var nonceBytes = new byte[18];

                rncProvider.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }

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
                    Xor(hi, u2);
                    u1 = u2;
                }

                return hi;
            }

            static byte[] Xor(byte[] buffer1, byte[] buffer2)
            {
                for (var i = 0; i < buffer1.Length; i++)
                    buffer1[i] ^= buffer2[i];
                return buffer1;
            }

            static byte[] HMAC(byte[] data, string key)
            {
                using var hmacsha256 = new HMACSHA256(data);
                return hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        async Task AuthenticateMD5(string username, byte[] salt, bool async, CancellationToken cancellationToken = default)
        {
            var passwd = GetPassword(username);
            if (passwd == null)
                throw new NpgsqlException("No password has been provided but the backend requires one (in MD5)");

            byte[] result;
            using (var md5 = MD5.Create())
            {
                // First phase
                var passwordBytes = PGUtil.UTF8Encoding.GetBytes(passwd);
                var usernameBytes = PGUtil.UTF8Encoding.GetBytes(username);
                var cryptBuf = new byte[passwordBytes.Length + usernameBytes.Length];
                passwordBytes.CopyTo(cryptBuf, 0);
                usernameBytes.CopyTo(cryptBuf, passwordBytes.Length);

                var sb = new StringBuilder();
                var hashResult = md5.ComputeHash(cryptBuf);
                foreach (var b in hashResult)
                    sb.Append(b.ToString("x2"));

                var prehash = sb.ToString();

                var prehashbytes = PGUtil.UTF8Encoding.GetBytes(prehash);
                cryptBuf = new byte[prehashbytes.Length + 4];

                Array.Copy(salt, 0, cryptBuf, prehashbytes.Length, 4);

                // 2.
                prehashbytes.CopyTo(cryptBuf, 0);

                sb = new StringBuilder("md5");
                hashResult = md5.ComputeHash(cryptBuf);
                foreach (var b in hashResult)
                    sb.Append(b.ToString("x2"));

                var resultString = sb.ToString();
                result = new byte[Encoding.UTF8.GetByteCount(resultString) + 1];
                Encoding.UTF8.GetBytes(resultString, 0, resultString.Length, result, 0);
                result[result.Length - 1] = 0;
            }

            await WritePassword(result, async, cancellationToken);
            await Flush(async, cancellationToken);
            Expect<AuthenticationRequestMessage>(await ReadMessage(async), this);
        }

        async Task AuthenticateGSS(bool async)
        {
            if (!IntegratedSecurity)
                throw new NpgsqlException("GSS/SSPI authentication but IntegratedSecurity not enabled");

            using var negotiateStream = new NegotiateStream(new GSSPasswordMessageStream(this), true);
            try
            {
                var targetName = $"{KerberosServiceName}/{Host}";
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
        class GSSPasswordMessageStream : Stream
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
                    var response = Expect<AuthenticationRequestMessage>(await _connector.ReadMessage(async), _connector);
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

        class AuthenticationCompleteException : Exception { }

        string? GetPassword(string username)
        {
            var password = Settings.Password;
            if (password != null)
                return password;

            if (ProvidePasswordCallback is { } passwordCallback)
                try
                {
                    Log.Trace($"Taking password from {nameof(ProvidePasswordCallback)} delegate");
                    password = passwordCallback(Host, Port, Settings.Database!, username);
                }
                catch (Exception e)
                {
                    throw new NpgsqlException($"Obtaining password using {nameof(NpgsqlConnection)}.{nameof(ProvidePasswordCallback)} delegate failed", e);
                }

            if (password is null)
                password = PostgresEnvironment.Password;

            if (password != null)
                return password;

            var passFile = Settings.Passfile ?? PostgresEnvironment.PassFile ?? PostgresEnvironment.PassFileDefault;
            if (passFile != null)
            {
                var matchingEntry = new PgPassFile(passFile!)
                    .GetFirstMatchingEntry(Host, Port, Settings.Database!, username);
                if (matchingEntry != null)
                {
                    Log.Trace("Taking password from pgpass file");
                    password = matchingEntry.Password;
                }
            }

            return password;
        }
    }
}
