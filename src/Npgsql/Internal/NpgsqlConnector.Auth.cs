using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        timeout.CheckAndApply(this);
        var msg = ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async), this);
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
        var passwd = await GetPassword(username, async, cancellationToken);
        if (passwd == null)
            throw new NpgsqlException("No password has been provided but the backend requires one (in cleartext)");

        var encoded = new byte[Encoding.UTF8.GetByteCount(passwd) + 1];
        Encoding.UTF8.GetBytes(passwd, 0, passwd.Length, encoded, 0);

        await WritePassword(encoded, async, cancellationToken);
        await Flush(async, cancellationToken);
        ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async), this);
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
                ConnectionLogger.LogWarning("Remote certificate null, falling back to SCRAM-SHA-256");
            }
            else
            {
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

        var passwd = await GetPassword(username, async, cancellationToken) ??
                     throw new NpgsqlException($"No password has been provided but the backend requires one (in SASL/{mechanism})");

        // Assumption: the write buffer is big enough to contain all our outgoing messages
        var clientNonce = GetNonce();

        await WriteSASLInitialResponse(mechanism, PGUtil.UTF8Encoding.GetBytes($"{cbindFlag},,n=*,r={clientNonce}"), async, cancellationToken);
        await Flush(async, cancellationToken);

        var saslContinueMsg = Expect<AuthenticationSASLContinueMessage>(await ReadMessage(async), this);
        if (saslContinueMsg.AuthRequestType != AuthenticationRequestType.AuthenticationSASLContinue)
            throw new NpgsqlException("[SASL] AuthenticationSASLContinue message expected");
        var firstServerMsg = AuthenticationSCRAMServerFirstMessage.Load(saslContinueMsg.Payload, ConnectionLogger);
        if (!firstServerMsg.Nonce.StartsWith(clientNonce, StringComparison.Ordinal))
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

        var scramFinalServerMsg = AuthenticationSCRAMServerFinalMessage.Load(saslFinalServerMsg.Payload, ConnectionLogger);
        if (scramFinalServerMsg.ServerSignature != Convert.ToBase64String(serverSignature))
            throw new NpgsqlException("[SCRAM] Unable to verify server signature");

        var okMsg = ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async), this);
        if (okMsg.AuthRequestType != AuthenticationRequestType.AuthenticationOk)
            throw new NpgsqlException("[SASL] Expected AuthenticationOK message");


        static string GetNonce()
        {
            using var rncProvider = RandomNumberGenerator.Create();
            var nonceBytes = new byte[18];

            rncProvider.GetBytes(nonceBytes);
            return Convert.ToBase64String(nonceBytes);
        }
    }

#if NET6_0_OR_GREATER
    static byte[] Hi(string str, byte[] salt, int count)
        => Rfc2898DeriveBytes.Pbkdf2(str, salt, count, HashAlgorithmName.SHA256, 256 / 8);
#endif

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

    async Task AuthenticateMD5(string username, byte[] salt, bool async, CancellationToken cancellationToken = default)
    {
        var passwd = await GetPassword(username, async, cancellationToken);
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
        ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async), this);
    }

#if NET7_0_OR_GREATER
    async Task AuthenticateGSS(bool async)
    {
        var targetName = $"{KerberosServiceName}/{Host}";

        using var authContext = new NegotiateAuthentication(new NegotiateAuthenticationClientOptions{ TargetName = targetName});
        var data = authContext.GetOutgoingBlob(ReadOnlySpan<byte>.Empty, out var statusCode)!;
        Debug.Assert(statusCode == NegotiateAuthenticationStatusCode.ContinueNeeded);
        await WritePassword(data, 0, data.Length, async, UserCancellationToken);
        await Flush(async, UserCancellationToken);
        while (true)
        {
            var response = ExpectAny<AuthenticationRequestMessage>(await ReadMessage(async), this);
            if (response.AuthRequestType == AuthenticationRequestType.AuthenticationOk)
                break;
            var gssMsg = response as AuthenticationGSSContinueMessage;
            if (gssMsg == null)
                throw new NpgsqlException($"Received unexpected authentication request message {response.AuthRequestType}");
            data = authContext.GetOutgoingBlob(gssMsg.AuthenticationData.AsSpan(), out statusCode)!;
            if (statusCode == NegotiateAuthenticationStatusCode.Completed)
                continue;
            Debug.Assert(statusCode == NegotiateAuthenticationStatusCode.ContinueNeeded);
            await WritePassword(data, 0, data.Length, async, UserCancellationToken);
            await Flush(async, UserCancellationToken);
        }
    }
#endif

    async ValueTask<string?> GetPassword(string username, bool async, CancellationToken cancellationToken = default)
    {
        var password = await DataSource.GetPassword(async, cancellationToken);

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
