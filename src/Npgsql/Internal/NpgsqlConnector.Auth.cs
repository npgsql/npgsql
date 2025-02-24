using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.BackendMessages;
using Npgsql.Util;
using static Npgsql.Util.Statics;

namespace Npgsql.Internal;

partial class NpgsqlConnector
{
    async Task Authenticate(string username, NpgsqlTimeout timeout, bool async, CancellationToken cancellationToken)
    {
        while (true)
        {
            timeout.CheckAndApply(this);
            var msg = ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async).ConfigureAwait(false), this);
            switch (msg.AuthRequestType)
            {
            case AuthenticationRequestType.Ok:
                return;

            case AuthenticationRequestType.CleartextPassword:
                await AuthenticateCleartext(username, async, cancellationToken).ConfigureAwait(false);
                break;

            case AuthenticationRequestType.MD5Password:
                await AuthenticateMD5(username, ((AuthenticationMD5PasswordMessage)msg).Salt, async, cancellationToken).ConfigureAwait(false);
                break;

            case AuthenticationRequestType.SASL:
                await AuthenticateSASL(((AuthenticationSASLMessage)msg).Mechanisms, username, async,
                    cancellationToken).ConfigureAwait(false);
                break;

            case AuthenticationRequestType.GSS:
            case AuthenticationRequestType.SSPI:
                await DataSource.IntegratedSecurityHandler.NegotiateAuthentication(async, this).ConfigureAwait(false);
                return;

            case AuthenticationRequestType.GSSContinue:
                throw new NpgsqlException("Can't start auth cycle with AuthenticationGSSContinue");

            default:
                throw new NotSupportedException($"Authentication method not supported (Received: {msg.AuthRequestType})");
            }
        }
    }

    async Task AuthenticateCleartext(string username, bool async, CancellationToken cancellationToken = default)
    {
        var passwd = await GetPassword(username, async, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(passwd))
            throw new NpgsqlException("No password has been provided but the backend requires one (in cleartext)");

        var encoded = new byte[Encoding.UTF8.GetByteCount(passwd) + 1];
        Encoding.UTF8.GetBytes(passwd, 0, passwd.Length, encoded, 0);

        await WritePassword(encoded, async, cancellationToken).ConfigureAwait(false);
        await Flush(async, cancellationToken).ConfigureAwait(false);
    }

    async Task AuthenticateSASL(List<string> mechanisms, string username, bool async, CancellationToken cancellationToken)
    {
        // At the time of writing PostgreSQL only supports SCRAM-SHA-256 and SCRAM-SHA-256-PLUS
        var serverSupportsSha256 = mechanisms.Contains("SCRAM-SHA-256");
        var allowSha256 = serverSupportsSha256 && Settings.ChannelBinding != ChannelBinding.Require;
        var serverSupportsSha256Plus = mechanisms.Contains("SCRAM-SHA-256-PLUS");
        var allowSha256Plus = serverSupportsSha256Plus && Settings.ChannelBinding != ChannelBinding.Disable;
        if (!allowSha256 && !allowSha256Plus)
        {
            if (serverSupportsSha256 && Settings.ChannelBinding == ChannelBinding.Require)
                throw new NpgsqlException($"Couldn't connect because {nameof(ChannelBinding)} is set to {nameof(ChannelBinding.Require)} " +
                                          "but the server doesn't support SCRAM-SHA-256-PLUS");
            if (serverSupportsSha256Plus && Settings.ChannelBinding == ChannelBinding.Disable)
                throw new NpgsqlException($"Couldn't connect because {nameof(ChannelBinding)} is set to {nameof(ChannelBinding.Disable)} " +
                                          "but the server doesn't support SCRAM-SHA-256");

            throw new NpgsqlException("No supported SASL mechanism found (only SCRAM-SHA-256 and SCRAM-SHA-256-PLUS are supported for now). " +
                                      "Mechanisms received from server: " + string.Join(", ", mechanisms));
        }

        var mechanism = string.Empty;
        var cbindFlag = string.Empty;
        var cbind = string.Empty;
        var successfulBind = false;

        if (allowSha256Plus)
            DataSource.TransportSecurityHandler.AuthenticateSASLSha256Plus(this, ref mechanism, ref cbindFlag, ref cbind, ref successfulBind);

        if (!successfulBind && allowSha256)
        {
            mechanism = "SCRAM-SHA-256";
            // We can get here if PostgreSQL supports only SCRAM-SHA-256 or there was an error while binding to SCRAM-SHA-256-PLUS
            // Or the user specifically requested to not use bindings
            // So, we set 'n' (client does not support binding) if there was an error while binding
            // or 'y' (client supports but server doesn't) in other case
            cbindFlag = serverSupportsSha256Plus ? "n" : "y";
            cbind = serverSupportsSha256Plus ? "biws" : "eSws";
            successfulBind = true;
            IsScram = true;
        }

        if (!successfulBind)
        {
            // We can get here if PostgreSQL supports only SCRAM-SHA-256-PLUS but there was an error while binding to it
            throw new NpgsqlException("Unable to bind to SCRAM-SHA-256-PLUS, check logs for more information");
        }

        var passwd = await GetPassword(username, async, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(passwd))
            throw new NpgsqlException($"No password has been provided but the backend requires one (in SASL/{mechanism})");

        // Assumption: the write buffer is big enough to contain all our outgoing messages
        var clientNonce = GetNonce();

        await WriteSASLInitialResponse(mechanism, NpgsqlWriteBuffer.UTF8Encoding.GetBytes($"{cbindFlag},,n=*,r={clientNonce}"), async, cancellationToken).ConfigureAwait(false);
        await Flush(async, cancellationToken).ConfigureAwait(false);

        var saslContinueMsg = Expect<AuthenticationSASLContinueMessage>(await ReadMessage(async).ConfigureAwait(false), this);
        if (saslContinueMsg.AuthRequestType != AuthenticationRequestType.SASLContinue)
            throw new NpgsqlException("[SASL] AuthenticationSASLContinue message expected");
        var firstServerMsg = AuthenticationSCRAMServerFirstMessage.Load(saslContinueMsg.Payload, ConnectionLogger);
        if (!firstServerMsg.Nonce.StartsWith(clientNonce, StringComparison.Ordinal))
            throw new NpgsqlException("[SCRAM] Malformed SCRAMServerFirst message: server nonce doesn't start with client nonce");

        var saltBytes = Convert.FromBase64String(firstServerMsg.Salt);
        var saltedPassword = Hi(passwd.Normalize(NormalizationForm.FormKC), saltBytes, firstServerMsg.Iteration);

        var clientKey = HMAC(saltedPassword, "Client Key");
        var storedKey = SHA256.HashData(clientKey);
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

        await WriteSASLResponse(Encoding.UTF8.GetBytes(messageStr), async, cancellationToken).ConfigureAwait(false);
        await Flush(async, cancellationToken).ConfigureAwait(false);

        var saslFinalServerMsg = Expect<AuthenticationSASLFinalMessage>(await ReadMessage(async).ConfigureAwait(false), this);
        if (saslFinalServerMsg.AuthRequestType != AuthenticationRequestType.SASLFinal)
            throw new NpgsqlException("[SASL] AuthenticationSASLFinal message expected");

        var scramFinalServerMsg = AuthenticationSCRAMServerFinalMessage.Load(saslFinalServerMsg.Payload, ConnectionLogger);
        if (scramFinalServerMsg.ServerSignature != Convert.ToBase64String(serverSignature))
            throw new NpgsqlException("[SCRAM] Unable to verify server signature");


        static string GetNonce()
        {
            using var rncProvider = RandomNumberGenerator.Create();
            var nonceBytes = new byte[18];

            rncProvider.GetBytes(nonceBytes);
            return Convert.ToBase64String(nonceBytes);
        }
    }

    internal void AuthenticateSASLSha256Plus(ref string mechanism, ref string cbindFlag, ref string cbind,
        ref bool successfulBind)
    {
        // The check below is copied from libpq (with commentary)
        // https://github.com/postgres/postgres/blob/98640f960eb9ed80cf90de3ef5d2e829b785b3eb/src/interfaces/libpq/fe-auth.c#L507-L517

        // The server offered SCRAM-SHA-256-PLUS, but the connection
        // is not SSL-encrypted. That's not sane. Perhaps SSL was
        // stripped by a proxy? There's no point in continuing,
        // because the server will reject the connection anyway if we
        // try authenticate without channel binding even though both
        // the client and server supported it. The SCRAM exchange
        // checks for that, to prevent downgrade attacks.
        if (!IsSecure)
            throw new NpgsqlException("Server offered SCRAM-SHA-256-PLUS authentication over a non-SSL connection");

        var sslStream = (SslStream)_stream;
        if (sslStream.RemoteCertificate is null)
        {
            ConnectionLogger.LogWarning("Remote certificate null, falling back to SCRAM-SHA-256");
            return;
        }

        // While SslStream.RemoteCertificate is X509Certificate2, it actually returns X509Certificate2
        // But to be on the safe side we'll just create a new instance of it
        using var remoteCertificate = new X509Certificate2(sslStream.RemoteCertificate);
        // Checking for hashing algorithms
        HashAlgorithm? hashAlgorithm = null;
        var algorithmName = remoteCertificate.SignatureAlgorithm.FriendlyName;
        if (algorithmName is null)
        {
            ConnectionLogger.LogWarning("Signature algorithm was null, falling back to SCRAM-SHA-256");
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
            ConnectionLogger.LogWarning(
                $"Support for signature algorithm {algorithmName} is not yet implemented, falling back to SCRAM-SHA-256");
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
            var cbindBytes = new byte[cbindFlagBytes.Length + certificateHash.Length];
            cbindFlagBytes.CopyTo(cbindBytes, 0);
            certificateHash.CopyTo(cbindBytes, cbindFlagBytes.Length);
            cbind = Convert.ToBase64String(cbindBytes);
            successfulBind = true;
            IsScramPlus = true;
        }
    }

    static byte[] Hi(string str, byte[] salt, int count)
        => Rfc2898DeriveBytes.Pbkdf2(str, salt, count, HashAlgorithmName.SHA256, 256 / 8);

    static byte[] Xor(byte[] buffer1, byte[] buffer2)
    {
        for (var i = 0; i < buffer1.Length; i++)
            buffer1[i] ^= buffer2[i];
        return buffer1;
    }

    static byte[] HMAC(byte[] key, string data) => HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(data));

    async Task AuthenticateMD5(string username, byte[] salt, bool async, CancellationToken cancellationToken = default)
    {
        var passwd = await GetPassword(username, async, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(passwd))
            throw new NpgsqlException("No password has been provided but the backend requires one (in MD5)");

        byte[] result;
        {
            // First phase
            var passwordBytes = NpgsqlWriteBuffer.UTF8Encoding.GetBytes(passwd);
            var usernameBytes = NpgsqlWriteBuffer.UTF8Encoding.GetBytes(username);
            var cryptBuf = new byte[passwordBytes.Length + usernameBytes.Length];
            passwordBytes.CopyTo(cryptBuf, 0);
            usernameBytes.CopyTo(cryptBuf, passwordBytes.Length);

            var sb = new StringBuilder();
            var hashResult = MD5.HashData(cryptBuf);
            foreach (var b in hashResult)
                sb.Append(b.ToString("x2"));

            var prehash = sb.ToString();

            var prehashbytes = NpgsqlWriteBuffer.UTF8Encoding.GetBytes(prehash);
            cryptBuf = new byte[prehashbytes.Length + 4];

            Array.Copy(salt, 0, cryptBuf, prehashbytes.Length, 4);

            // 2.
            prehashbytes.CopyTo(cryptBuf, 0);

            sb = new StringBuilder("md5");
            hashResult = MD5.HashData(cryptBuf);
            foreach (var b in hashResult)
                sb.Append(b.ToString("x2"));

            var resultString = sb.ToString();
            result = new byte[Encoding.UTF8.GetByteCount(resultString) + 1];
            Encoding.UTF8.GetBytes(resultString, 0, resultString.Length, result, 0);
            result[^1] = 0;
        }

        await WritePassword(result, async, cancellationToken).ConfigureAwait(false);
        await Flush(async, cancellationToken).ConfigureAwait(false);
    }

    internal async Task AuthenticateGSS(bool async)
    {
        var targetName = $"{KerberosServiceName}/{Host}";

        var clientOptions = new NegotiateAuthenticationClientOptions { TargetName = targetName };
        NegotiateOptionsCallback?.Invoke(clientOptions);

        using var authContext = new NegotiateAuthentication(clientOptions);
        var data = authContext.GetOutgoingBlob(ReadOnlySpan<byte>.Empty, out var statusCode)!;
        Debug.Assert(statusCode == NegotiateAuthenticationStatusCode.ContinueNeeded);
        await WritePassword(data, 0, data.Length, async, UserCancellationToken).ConfigureAwait(false);
        await Flush(async, UserCancellationToken).ConfigureAwait(false);
        while (true)
        {
            var response = ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async).ConfigureAwait(false), this);
            if (response.AuthRequestType == AuthenticationRequestType.Ok)
                break;
            if (response is not AuthenticationGSSContinueMessage gssMsg)
                throw new NpgsqlException($"Received unexpected authentication request message {response.AuthRequestType}");
            data = authContext.GetOutgoingBlob(gssMsg.AuthenticationData.AsSpan(), out statusCode)!;
            if (statusCode is not NegotiateAuthenticationStatusCode.Completed and not NegotiateAuthenticationStatusCode.ContinueNeeded)
                throw new NpgsqlException($"Error while authenticating GSS/SSPI: {statusCode}");
            // We might get NegotiateAuthenticationStatusCode.Completed but the data will not be null
            // This can happen if it's the first cycle, in which case we have to send that data to complete handshake (#4888)
            if (data is null)
                continue;
            await WritePassword(data, 0, data.Length, async, UserCancellationToken).ConfigureAwait(false);
            await Flush(async, UserCancellationToken).ConfigureAwait(false);
        }
    }

    async ValueTask<string?> GetPassword(string username, bool async, CancellationToken cancellationToken = default)
    {
        var password = await DataSource.GetPassword(async, cancellationToken).ConfigureAwait(false);

        if (password is not null)
            return password;

        if (ProvidePasswordCallback is { } passwordCallback)
        {
            try
            {
                ConnectionLogger.LogTrace($"Taking password from {nameof(ProvidePasswordCallback)} delegate");
                password = passwordCallback(Host, Port, Settings.Database!, username);
            }
            catch (Exception e)
            {
                throw new NpgsqlException($"Obtaining password using {nameof(NpgsqlConnection)}.{nameof(ProvidePasswordCallback)} delegate failed", e);
            }
        }

        password ??= PostgresEnvironment.Password;

        if (password != null)
            return password;

        var passFile = Settings.Passfile ?? PostgresEnvironment.PassFile ?? PostgresEnvironment.PassFileDefault;
        if (passFile != null)
        {
            var matchingEntry = new PgPassFile(passFile!)
                .GetFirstMatchingEntry(Host, Port, Settings.Database!, username);
            if (matchingEntry != null)
            {
                ConnectionLogger.LogTrace("Taking password from pgpass file");
                password = matchingEntry.Password;
            }
        }

        return password;
    }
}
