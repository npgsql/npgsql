using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

    internal static ValueTask<string?> GetUsernameAsync(bool includeRealm, ILogger connectionLogger, bool async, CancellationToken cancellationToken)
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

#pragma warning disable CS1998
        async ValueTask<string?> GetUsernameAsyncInternal()
#pragma warning restore CS1998
        {
#if NET5_0_OR_GREATER
            if (async)
                await process.WaitForExitAsync(cancellationToken);
            else
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                process.WaitForExit();
#else
            // ReSharper disable once MethodHasAsyncOverload
            process.WaitForExit();
#endif

            if (process.ExitCode != 0)
            {
                connectionLogger.LogDebug($"klist exited with code {process.ExitCode}: {process.StandardError.ReadToEnd()}");
                return null;
            }

            var line = default(string);
            for (var i = 0; i < 2; i++)
                // ReSharper disable once MethodHasAsyncOverload
#if NET7_0_OR_GREATER
                if ((line = async ? await process.StandardOutput.ReadLineAsync(cancellationToken) : process.StandardOutput.ReadLine()) == null)
#elif NET5_0_OR_GREATER
                if ((line = async ? await process.StandardOutput.ReadLineAsync() : process.StandardOutput.ReadLine()) == null)
#else
                if ((line = process.StandardOutput.ReadLine()) == null)
#endif
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

    static string? FindInPath(string name) => Environment.GetEnvironmentVariable("PATH")
        ?.Split(Path.PathSeparator)
        .Select(p => Path.Combine(p, name))
        .FirstOrDefault(File.Exists);
}
