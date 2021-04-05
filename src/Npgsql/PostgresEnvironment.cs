using System;
using System.Collections.Generic;
using System.IO;
using Npgsql.Util;

namespace Npgsql
{
    static class PostgresEnvironment
    {
        internal static string? User => Environment.GetEnvironmentVariable("PGUSER");

        internal static string? Password => Environment.GetEnvironmentVariable("PGPASSWORD");

        internal static string? PassFile => Environment.GetEnvironmentVariable("PGPASSFILE");

        internal static string? PassFileDefault => GetDefaultFilePath(PGUtil.IsWindows ? "pgpass.conf" : ".pgpass");

        internal static string? SslCert => Environment.GetEnvironmentVariable("PGSSLCERT");

        internal static string? SslCertDefault => GetDefaultFilePath("postgresql.crt");

        internal static string? SslCertRoot => Environment.GetEnvironmentVariable("PGSSLROOTCERT");

        internal static string? SslCertRootDefault => GetDefaultFilePath("root.crt");

        internal static string? SslKey => Environment.GetEnvironmentVariable("PGSSLKEY");

        internal static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        internal static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        internal static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

        internal static string? TargetSessionAttributes => Environment.GetEnvironmentVariable("PGTARGETSESSIONATTRS");

        static string? GetDefaultFilePath(string fileName) =>
            Environment.GetEnvironmentVariable(PGUtil.IsWindows ? "APPDATA" : "HOME") is string appData &&
            Path.Combine(appData, "postgresql", fileName) is string filePath &&
            File.Exists(filePath)
                ? filePath
                : null;
    }
}
