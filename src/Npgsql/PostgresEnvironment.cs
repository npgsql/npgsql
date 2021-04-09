using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Npgsql.Util;

namespace Npgsql
{
    static class PostgresEnvironment
    {
        internal static string? User => Environment.GetEnvironmentVariable("PGUSER");

        internal static string? Password => Environment.GetEnvironmentVariable("PGPASSWORD");

        internal static string? PassFile => Environment.GetEnvironmentVariable("PGPASSFILE");

        internal static string? PassFileDefault
            => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   ? Path.Combine(GetHomePostgresDir(), "pgpass.conf")
                   : Path.Combine(GetHomeDir(), ".pgpass")) is var path &&
               File.Exists(path)
                ? path
                : null;

        internal static string? SslCert => Environment.GetEnvironmentVariable("PGSSLCERT");

        internal static string? SslCertDefault
            => Path.Combine(GetHomePostgresDir(), "postgresql.crt") is var path && File.Exists(path) ? path : null;

        internal static string? SslKey => Environment.GetEnvironmentVariable("PGSSLKEY");

        internal static string? SslKeyDefault
            => Path.Combine(GetHomePostgresDir(), "postgresql.key") is var path && File.Exists(path) ? path : null;

        internal static string? SslCertRoot => Environment.GetEnvironmentVariable("PGSSLROOTCERT");

        internal static string? SslCertRootDefault
            => Path.Combine(GetHomePostgresDir(), "root.crt") is var path && File.Exists(path) ? path : null;

        internal static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        internal static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        internal static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

        internal static string? TargetSessionAttributes => Environment.GetEnvironmentVariable("PGTARGETSESSIONATTRS");

        static string GetHomeDir()
        {
            var envVar = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "APPDATA" : "HOME";
            return Environment.GetEnvironmentVariable(envVar) is string homedir
                ? homedir
                : throw new InvalidOperationException($"Environment variable {envVar} not defined");
        }

        static string GetHomePostgresDir()
            => Path.Combine(GetHomeDir(), RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "postgresql" : ".postgresql");
    }
}
