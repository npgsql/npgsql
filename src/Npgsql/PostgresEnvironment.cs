using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Npgsql;

static class PostgresEnvironment
{
    internal static string? User => Environment.GetEnvironmentVariable("PGUSER");

    internal static string? Password => Environment.GetEnvironmentVariable("PGPASSWORD");

    internal static string? PassFile => Environment.GetEnvironmentVariable("PGPASSFILE");

    internal static string? PassFileDefault
        => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetHomePostgresDir() : GetHomeDir()) is string homedir &&
           Path.Combine(homedir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "pgpass.conf" : ".pgpass") is var path &&
           File.Exists(path)
            ? path
            : null;

    internal static string? Service => Environment.GetEnvironmentVariable("PGSERVICE");

    static string? UserServiceFilePath
        => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetHomePostgresDir() : GetHomeDir();

    internal static string? UserServiceFile
        => Environment.GetEnvironmentVariable("PGSERVICEFILE")
            ?? (UserServiceFilePath is string path
                ? Path.Combine(path, ".pg_service.conf")
                : null);

    static string? SystemServiceFilePath
        => GetSystemPostgresDir() is string systemDir && Directory.Exists(systemDir)
            ? systemDir :
            GetDefaultSystemPostgresDir() is string defaultSystemDir && Directory.Exists(defaultSystemDir)
             ? defaultSystemDir
             : null;

    internal static string? SystemServiceFile
        => SystemServiceFilePath is string path
            ? Path.Combine(path, "pg_service.conf")
            : null;

    internal static string? SslCert => Environment.GetEnvironmentVariable("PGSSLCERT");

    internal static string? SslCertDefault
        => GetHomePostgresDir() is string homedir && Path.Combine(homedir, "postgresql.crt") is var path && File.Exists(path)
            ? path
            : null;

    internal static string? SslKey => Environment.GetEnvironmentVariable("PGSSLKEY");

    internal static string? SslKeyDefault
        => GetHomePostgresDir() is string homedir && Path.Combine(homedir, "postgresql.key") is var path && File.Exists(path)
            ? path
            : null;

    internal static string? SslCertRoot => Environment.GetEnvironmentVariable("PGSSLROOTCERT");

    internal static string? SslCertRootDefault
        => GetHomePostgresDir() is string homedir && Path.Combine(homedir, "root.crt") is var path && File.Exists(path)
            ? path
            : null;

    internal static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

    internal static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

    internal static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

    internal static string? TargetSessionAttributes => Environment.GetEnvironmentVariable("PGTARGETSESSIONATTRS");

    static string? GetHomeDir()
        => Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "APPDATA" : "HOME");

    static string? GetHomePostgresDir()
        => GetHomeDir() is string homedir
            ? Path.Combine(homedir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "postgresql" : ".postgresql")
            : null;

    static string? GetSystemPostgresDir() => Environment.GetEnvironmentVariable("PGSYSCONFDIR");

    static string? GetDefaultSystemPostgresDir()
    {
        try
        {
            return ExecuteCommand("pg_config", async: false, "--sysconfdir").GetAwaiter().GetResult()?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    internal static ValueTask<string[]?> ExecuteCommand(
        string processName,
        bool async,
        string arguments = "",
        ILogger? logger = default,
        CancellationToken cancellationToken = default)
    {
        var processPath = FindInPath(processName);
        if (processPath == null)
        {
            logger?.LogDebug($"{processName} not found in PATH");
            return new((string[]?)null);
        }
        var processStartInfo = new ProcessStartInfo
        {
            FileName = processPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        var process = Process.Start(processStartInfo);
        if (process is null)
        {
            logger?.LogDebug($"{processName} process could not be started");
            return new((string[]?)null);
        }

        return ExecuteCommandInternal();

#pragma warning disable CS1998
        async ValueTask<string[]?> ExecuteCommandInternal()
#pragma warning restore CS1998
        {
#if NET5_0_OR_GREATER
            if (async)
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            else
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                process.WaitForExit();
#else
            // ReSharper disable once MethodHasAsyncOverload
            process.WaitForExit();
#endif

            if (process.ExitCode != 0)
            {
                logger?.LogDebug($"{processName} exited with code {process.ExitCode}: {process.StandardError.ReadToEnd()}");
                return null;
            }

            var lines = new List<string>();
            string? line;
            // ReSharper disable once MethodHasAsyncOverload
#if NET7_0_OR_GREATER
            while ((line = async
                ? await process.StandardOutput.ReadLineAsync(cancellationToken).ConfigureAwait(false)
                : process.StandardOutput.ReadLine()) is not null)
#elif NET5_0_OR_GREATER
            while ((line = async
                ? await process.StandardOutput.ReadLineAsync().ConfigureAwait(false)
                : process.StandardOutput.ReadLine()) is not null)
#else
            while ((line = process.StandardOutput.ReadLine()) is not null)
#endif
            {
                lines.Add(line);
            }

            return lines.ToArray();
        }
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

    static string ExecutableExtension => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "";
}
