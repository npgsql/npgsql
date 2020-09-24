using System;
using System.Collections.Generic;
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

        public static string? SslCertRoot => Environment.GetEnvironmentVariable("PGSSLROOTCERT");

        public static string? SslCertRootDefault => GetDefaultFilePath("root.crt");

        public static string? SslKey => Environment.GetEnvironmentVariable("PGSSLKEY");

        public static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        public static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        public static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

        public static Dictionary<string, string> ParsedOptions => ParseOptions(Options);

        static Dictionary<string, string> ParseOptions(string? options)
        {
            var parsedOptions = new Dictionary<string, string>();

            if (options != null)
            {
                var pos = 0;
                while (pos < options.Length)
                {
                    var key = NpgsqlConnectionStringBuilder.ParseKey(options, ref pos);
                    var value = NpgsqlConnectionStringBuilder.ParseValue(options, ref pos);
                    parsedOptions[key] = value;
                }
            }

            return parsedOptions;
        }

        static string? GetDefaultFilePath(string fileName) =>
            Environment.GetEnvironmentVariable(PGUtil.IsWindows ? "APPDATA" : "HOME") is string appData
                ? Path.Combine(appData, "postgresql", fileName)
                : null;
    }
}
