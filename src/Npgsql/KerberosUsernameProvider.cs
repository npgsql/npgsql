#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
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
        static string _principalWithRealm;
        static string _principalWithoutRealm;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        [CanBeNull]
        internal static string GetUsername(bool includeRealm)
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
            var klistPath = FindInPath("klist");
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
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Log.Debug($"klist exited with code {process.ExitCode}: {process.StandardError.ReadToEnd()}");
                return;
            }

            var line = "";
            for (var i = 0; i < 2; i++)
                if ((line = process.StandardOutput.ReadLine()) == null)
                {
                    Log.Debug("Unexpected output from klist, aborting Kerberos username detection");
                    return;
                }

            var components = line.Split(':');
            if (components.Length != 2)
            {
                Log.Debug("Unexpected output from klist, aborting Kerberos username detection");
                return;
            }

            var principalWithRealm = components[1].Trim();
            components = principalWithRealm.Split('@');
            if (components.Length != 2)
            {
                Log.Debug($"Badly-formed default principal {principalWithRealm} from klist, aborting Kerberos username detection");
                return;
            }

            _principalWithRealm = principalWithRealm;
            _principalWithoutRealm = components[0];
        }

        static string FindInPath(string name) => Environment.GetEnvironmentVariable("PATH")
            .Split(Path.PathSeparator)
            .Select(p => Path.Combine(p, name))
            .FirstOrDefault(File.Exists);
    }
}