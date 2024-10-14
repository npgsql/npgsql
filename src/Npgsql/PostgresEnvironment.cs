using System;
using System.IO;
using System.Runtime.InteropServices;

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

    internal static string? SslNegotiation => Environment.GetEnvironmentVariable("PGSSLNEGOTIATION");

    static string? GetHomeDir()
        => Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "APPDATA" : "HOME");

    static string? GetHomePostgresDir()
        => GetHomeDir() is string homedir
            ? Path.Combine(homedir, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "postgresql" : ".postgresql")
            : null;
}
