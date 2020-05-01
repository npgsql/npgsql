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

        public static string? ClientEncoding => Environment.GetEnvironmentVariable("PGCLIENTENCODING");

        public static string? TimeZone => Environment.GetEnvironmentVariable("PGTZ");

        public static string? Options => Environment.GetEnvironmentVariable("PGOPTIONS");

        public static Dictionary<string, string> ParsedOptions
        {
            get
            {
                if (_optionsCache == Options)
                    return ParsedOptionsCache;

                _optionsCache = Options;
                ParseOptions();
                return ParsedOptionsCache;
            }
        }

        static readonly Dictionary<string, string> ParsedOptionsCache = new Dictionary<string, string>();

        static string? _optionsCache;

        static void ParseOptions()
        {
            if (_optionsCache == null)
            {
                ParsedOptions.Clear();
            }
            else
            {
                var pos = 0;
                while (pos < _optionsCache.Length)
                {
                    var key = NpgsqlConnectionStringBuilder.ParseKey(_optionsCache, ref pos);
                    var value = NpgsqlConnectionStringBuilder.ParseValue(_optionsCache, ref pos);
                    ParsedOptions[key] = value;
                }
            }
        }

        static string? GetDefaultFilePath(string fileName) =>
            Environment.GetEnvironmentVariable(PGUtil.IsWindows ? "APPDATA" : "HOME") is string appData
                ? Path.Combine(appData, "postgresql", fileName)
                : null;
    }
}
