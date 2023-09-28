using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.Properties;

namespace Npgsql.Internal;

class IntegratedSecurityHandler
{
    public virtual bool IsSupported => false;

    public virtual ValueTask<string?> GetUsername(bool async, bool includeRealm, ILogger connectionLogger, CancellationToken cancellationToken)
    {
        connectionLogger.LogDebug(string.Format(NpgsqlStrings.IntegratedSecurityDisabled, nameof(NpgsqlSlimDataSourceBuilder.EnableIntegratedSecurity)));
        return new();
    }

    public virtual ValueTask NegotiateAuthentication(bool async, NpgsqlConnector connector)
        => throw new NotSupportedException(string.Format(NpgsqlStrings.IntegratedSecurityDisabled, nameof(NpgsqlSlimDataSourceBuilder.EnableIntegratedSecurity)));
}

sealed class RealIntegratedSecurityHandler : IntegratedSecurityHandler
{
    public override bool IsSupported => true;

    public override ValueTask<string?> GetUsername(bool async, bool includeRealm, ILogger connectionLogger, CancellationToken cancellationToken)
        => KerberosUsernameProvider.GetUsername(async, includeRealm, connectionLogger, cancellationToken);

    public override ValueTask NegotiateAuthentication(bool async, NpgsqlConnector connector)
        => new(connector.AuthenticateGSS(async));
}
