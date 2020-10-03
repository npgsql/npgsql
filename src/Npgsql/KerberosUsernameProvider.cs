﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Npgsql.Logging;

namespace Npgsql
{
    /// <summary>
    /// Launches MIT Kerberos klist and parses out the default principal from it.
    /// Caches the result.
    /// </summary>
    class KerberosUsernameProvider
    {
        static bool _performedDetection;
        static string? _principalWithRealm;
        static string? _principalWithoutRealm;
        static readonly Regex _clientRegex = new Regex(".*Client:(.*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(KerberosUsernameProvider));

        internal static string? GetUsername(bool includeRealm)
        {
            if (!_performedDetection)
            {
                DetectUsername();
                _performedDetection = true;
            }
            return includeRealm ? _principalWithRealm : _principalWithoutRealm;
        }

        static void DetectUsername()
        {
            var klistPath = FindKListInPath();
            if (klistPath == null)
            {
                Log.Debug("klist not found in PATH, skipping Kerberos username detection");
                return;
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
                Log.Debug($"klist process could not be started");
                return;
            }

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Log.Debug($"klist exited with code {process.ExitCode}: {process.StandardError.ReadToEnd()}");
                return;
            }

            var fullPrincipal = GetFullPrincipalFromFirstTicket(process.StandardOutput.ReadToEnd());

            if (fullPrincipal == string.Empty)
            {
                Log.Debug("Unexpected output from klist, aborting Kerberos username detection");
                return;
            }

            _principalWithRealm = fullPrincipal;
            _principalWithoutRealm = fullPrincipal.Split('@').FirstOrDefault();
        }

        static string? FindKListInPath() => Environment.GetEnvironmentVariable("PATH")
            ?.Split(Path.PathSeparator)
            .SelectMany(p => Directory.GetFiles(p, "klist*"))
            .FirstOrDefault();

        static string GetFullPrincipalFromFirstTicket(string klistOutput)
        {
            var firstMatch = _clientRegex.Match(klistOutput);

            return firstMatch.Success
                ? firstMatch.Groups[1].Value.Trim().Replace(" ", "")
                : string.Empty;
        }
    }
}
