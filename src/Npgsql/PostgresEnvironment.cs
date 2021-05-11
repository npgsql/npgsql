using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Npgsql.Util;

namespace Npgsql
{
    static class PostgresEnvironment
    {
        public static string? User => Environment.GetEnvironmentVariable("PGUSER");

        public static string? Password => Environment.GetEnvironmentVariable("PGPASSWORD");

        public static string? PassFile => Environment.GetEnvironmentVariable("PGPASSFILE");

        public static string? PassFileDefault
            => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   ? Path.Combine(GetHomePostgresDir(), "pgpass.conf")
                   : Path.Combine(GetHomeDir(), ".pgpass")) is var path &&
               File.Exists(path)
                ? path
                : null;

        public static string? SslCert => Environment.GetEnvironmentVariable("PGSSLCERT");

        public static string? SslCertDefault
            => Path.Combine(GetHomePostgresDir(), "postgresql.crt") is var path && File.Exists(path) ? path : null;

        public static string? SslKey => Environment.GetEnvironmentVariable("PGSSLKEY");

        public static string? SslCertRoot => Environment.GetEnvironmentVariable("PGSSLROOTCERT");

        public static string? SslCertRootDefault
            => Path.Combine(GetHomePostgresDir(), "root.crt") is var path && File.Exists(path) ? path : null;

        public static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        public static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        public static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

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
