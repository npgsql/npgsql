using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;
using Npgsql.Util;

namespace Npgsql.Internal;

class TransportSecurityHandler
{
    public virtual bool SupportEncryption => false;

    public virtual Func<X509Certificate2?>? RootCertificateCallback
    {
        get => throw new NotSupportedException(string.Format(NpgsqlStrings.TransportSecurityDisabled, nameof(NpgsqlSlimDataSourceBuilder.EnableTransportSecurity)));
        set => throw new NotSupportedException(string.Format(NpgsqlStrings.TransportSecurityDisabled, nameof(NpgsqlSlimDataSourceBuilder.EnableTransportSecurity)));
    }

    public virtual Task NegotiateEncryption(bool async, NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        => throw new NotSupportedException(string.Format(NpgsqlStrings.TransportSecurityDisabled, nameof(NpgsqlSlimDataSourceBuilder.EnableTransportSecurity)));

    public virtual void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
        ref bool successfulBind)
        => throw new NotSupportedException(string.Format(NpgsqlStrings.TransportSecurityDisabled, nameof(NpgsqlSlimDataSourceBuilder.EnableTransportSecurity)));
}

sealed class RealTransportSecurityHandler : TransportSecurityHandler
{
    public override bool SupportEncryption => true;

    public override Func<X509Certificate2?>? RootCertificateCallback { get; set; }

    public override Task NegotiateEncryption(bool async, NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        => connector.NegotiateEncryption(sslMode, timeout, async, cancellationToken);

    public override void AuthenticateSASLSha256Plus(NpgsqlConnector connector, ref string mechanism, ref string cbindFlag, ref string cbind,
            ref bool successfulBind)
        => connector.AuthenticateSASLSha256Plus(ref mechanism, ref cbindFlag, ref cbind, ref successfulBind);
}
