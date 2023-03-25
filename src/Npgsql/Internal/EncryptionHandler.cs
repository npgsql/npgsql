using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Npgsql.Properties;
using Npgsql.Util;

namespace Npgsql.Internal;

class EncryptionHandler
{
    public virtual bool SupportEncryption => false;

    public virtual Func<X509Certificate2?>? RootCertificateCallback
    {
        get => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
        set => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
    }

    public virtual Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt)
        => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);

    public virtual void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
        ref bool successfulBind)
        => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
}

sealed class RealEncryptionHandler : EncryptionHandler
{
    public override bool SupportEncryption => true;

    public override Func<X509Certificate2?>? RootCertificateCallback { get; set; }

    public override Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt)
        => connector.NegotiateEncryption(sslMode, timeout, async, isFirstAttempt);

    public override void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
            ref bool successfulBind)
        => connector.AuthenticateSASLSha256Plus(ref mechanism, ref cbindFlag, ref cbind, ref successfulBind);
}
