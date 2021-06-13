using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Npgsql
{
    static class PostgresEnvironment
    {
        public static string? User => Environment.GetEnvironmentVariable("PGUSER");

        public static string? Password => Environment.GetEnvironmentVariable("PGPASSWORD");

        public static string? PassFile => Environment.GetEnvironmentVariable("PGPASSFILE");

        public static string? PassFileDefault
            => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? GetHomePostgresDir() : GetHomeDir()) is string homedir &&
               Path.Combine(homedir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "pgpass.conf" : ".pgpass") is var path &&
               File.Exists(path)
                ? path
                : null;

        public static string? SslCert => Environment.GetEnvironmentVariable("PGSSLCERT");

        public static string? SslCertDefault
            => GetHomePostgresDir() is string homedir && Path.Combine(homedir, "postgresql.crt") is var path && File.Exists(path)
                ? path
                : null;

        public static string? SslKey => Environment.GetEnvironmentVariable("PGSSLKEY");

        public static string? SslCertRoot => Environment.GetEnvironmentVariable("PGSSLROOTCERT");

        public static string? SslCertRootDefault
            => GetHomePostgresDir() is string homedir && Path.Combine(homedir, "root.crt") is var path && File.Exists(path)
                ? path
                : null;

        public static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        public static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        public static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

        static string? GetHomeDir()
            => Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "APPDATA" : "HOME");

        static string? GetHomePostgresDir()
            => GetHomeDir() is string homedir
                ? Path.Combine(homedir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "postgresql" : ".postgresql")
                : null;
    }
}
