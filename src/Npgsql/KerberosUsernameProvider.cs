using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Npgsql;

/// <summary>
/// Launches MIT Kerberos klist and parses out the default principal from it.
/// Caches the result.
/// </summary>
sealed class KerberosUsernameProvider
{
    static bool _performedDetection;
    static string? _principalWithRealm;
    static string? _principalWithoutRealm;

    internal static ValueTask<string?> GetUsername(bool async, bool includeRealm, ILogger connectionLogger, CancellationToken cancellationToken)
    {
        if (_performedDetection)
            return new(includeRealm ? _principalWithRealm : _principalWithoutRealm);

        return GetUsernameInternal();

        async ValueTask<string?> GetUsernameInternal()
        {
            var lines = await PostgresEnvironment.ExecuteCommand("klist", async, logger: connectionLogger, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (lines is null)
            {
                connectionLogger.LogDebug("Skipping Kerberos username detection");
                return null;
            }

            return ParseKListOutput(lines, includeRealm, connectionLogger);
        }
    }

    static string? ParseKListOutput(string[] lines, bool includeRealm, ILogger connectionLogger)
    {
        if (lines.Length < 2) return null;
        var line = lines[1];
        var colonIndex = line.IndexOf(':');
        var colonLastIndex = line.LastIndexOf(':');
        if (colonIndex == -1 || colonIndex != colonLastIndex)
        {
            connectionLogger.LogDebug("Unexpected output from klist, aborting Kerberos username detection");
            return null;
        }
        var secondPart = line.AsSpan(1 + line.IndexOf(':'));

        var principalWithRealm = secondPart.Trim();
        var atIndex = principalWithRealm.IndexOf('@');
        var atLastIndex = principalWithRealm.LastIndexOf('@');
        if (atIndex == -1 || atIndex != atLastIndex)
        {
            connectionLogger.LogDebug(
                $"Badly-formed default principal {principalWithRealm.ToString()} from klist, aborting Kerberos username detection");
            return null;
        }

        _principalWithRealm = principalWithRealm.ToString();
        _principalWithoutRealm = principalWithRealm.Slice(0, atIndex).ToString();
        _performedDetection = true;
        return includeRealm ? _principalWithRealm : _principalWithoutRealm;
    }
}
