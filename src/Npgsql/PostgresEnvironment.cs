using System;
using System.IO;
using Npgsql.Util;

namespace Npgsql
{
    static class PostgresEnvironment
    {
        public static string? User => Environment.GetEnvironmentVariable("PGUSER");

        public static string? Password => Environment.GetEnvironmentVariable("PGPASSWORD");

        public static string? PassFile => Environment.GetEnvironmentVariable("PGPASSFILE");

        public static string? PassFileDefault => GetDefaultFilePath(PGUtil.IsWindows ? "pgpass.conf" : ".pgpass");

        public static string? SslCert => Environment.GetEnvironmentVariable("PGSSLCERT");

        public static string? SslCertDefault => GetDefaultFilePath("postgresql.crt");

        public static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        public static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        static string? GetDefaultFilePath(string fileName) =>
            Environment.GetEnvironmentVariable(PGUtil.IsWindows ? "APPDATA" : "HOME") is string appData
                ? Path.Combine(appData, "postgresql", fileName)
                : null;
    }
}
