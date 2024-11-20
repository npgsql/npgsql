using System;
using System.Diagnostics;
using System.IO;
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
        var klistPath = FindInPath("klist");
        if (klistPath == null)
        {
            connectionLogger.LogDebug("klist not found in PATH, skipping Kerberos username detection");
            return new((string?)null);
        }
        var processStartInfo = new ProcessStartInfo
        {
            FileName = klistPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        var process = Process.Start(processStartInfo);
        if (process is null)
        {
            connectionLogger.LogDebug("klist process could not be started");
            return new((string?)null);
        }

        return GetUsernameAsyncInternal();

        async ValueTask<string?> GetUsernameAsyncInternal()
        {
            if (async)
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            else
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                process.WaitForExit();

            if (process.ExitCode != 0)
            {
                connectionLogger.LogDebug($"klist exited with code {process.ExitCode}: {process.StandardError.ReadToEnd()}");
                return null;
            }

            var line = default(string);
            for (var i = 0; i < 2; i++)
                // ReSharper disable once MethodHasAsyncOverload
                if ((line = async ? await process.StandardOutput.ReadLineAsync(cancellationToken).ConfigureAwait(false) : process.StandardOutput.ReadLine()) == null)
                {
                    connectionLogger.LogDebug("Unexpected output from klist, aborting Kerberos username detection");
                    return null;
                }

            return ParseKListOutput(line!, includeRealm, connectionLogger);
        }
    }

    static string? ParseKListOutput(string line, bool includeRealm, ILogger connectionLogger)
    {
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

    static string? FindInPath(string name)
    {
        foreach (var p in Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? [])
        {
            var path = Path.Combine(p, name);
            if (File.Exists(path))
                return path;
        }

        return null;
    }
}
