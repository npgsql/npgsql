using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;
using Npgsql.Util;

namespace Npgsql.Internal;

interface IEncryptionHandler
{
    bool SupportEncryption { get; }

    Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt);

    Task AuthenticateSASL(NpgsqlConnector connector, List<string> mechanisms, string username, bool async,
        CancellationToken cancellationToken);
}

sealed class EncryptionHandler : IEncryptionHandler
{
    public bool SupportEncryption => true;

    public Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt)
        => connector.NegotiateEncryption(sslMode, timeout, async, isFirstAttempt);

    public Task AuthenticateSASL(NpgsqlConnector connector, List<string> mechanisms, string username, bool async,
        CancellationToken cancellationToken)
        => connector.AuthenticateSASL(mechanisms, username, async, cancellationToken);
}

sealed class EmptyEncryptionHandler : IEncryptionHandler
{
    public bool SupportEncryption => false;

    public Task NegotiateEncryption(NpgsqlConnector connector, SslMode sslMode, NpgsqlTimeout timeout, bool async, bool isFirstAttempt)
        => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);

    public Task AuthenticateSASL(NpgsqlConnector connector, List<string> mechanisms, string username, bool async,
        CancellationToken cancellationToken)
        => throw new InvalidOperationException(NpgsqlStrings.EncryptionDisabled);
}
