using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Npgsql.Properties;
using Npgsql.Util;

namespace Npgsql.Internal;

interface IEncryptionHandler
{
    bool SupportEncryption { get; }

    Func<X509Certificate2?>? RootCertificateCallback { get; set; }

    Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt);

    void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
        ref bool successfulBind);
}

sealed class EncryptionHandler : IEncryptionHandler
{
    public bool SupportEncryption => true;

    public Func<X509Certificate2?>? RootCertificateCallback { get; set; }

    public Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt)
        => connector.NegotiateEncryption(sslMode, timeout, async, isFirstAttempt);

    public void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
            ref bool successfulBind)
        => connector.AuthenticateSASLSha256Plus(ref mechanism, ref cbindFlag, ref cbind, ref successfulBind);
}

sealed class EmptyEncryptionHandler : IEncryptionHandler
{
    public bool SupportEncryption => false;

    public Func<X509Certificate2?>? RootCertificateCallback
    {
        get => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
        set => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
    }

    public Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt)
        => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);

    public void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
        ref bool successfulBind)
        => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
}
