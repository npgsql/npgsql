using DotNet.Testcontainers.Configurations;
using Npgsql;
using Npgsql.Tests;
using NUnit.Framework;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

[SetUpFixture]
public class AssemblySetUp
{
    static PostgreSqlContainer? _container;
    [OneTimeSetUp]
    public async Task Setup()
    {
        var connString = TestUtil.ConnectionString;
        using var conn = new NpgsqlConnection(connString);
        try
        {
            conn.Open();
        }
        catch (NpgsqlException e) when (e.InnerException is SocketException)
        {
            _container ??= await SetupContainerAsync();
            await _container.StartAsync();
            conn.Open();
            return;
        }
        catch (PostgresException e)
        {
            if (e.SqlState == PostgresErrorCodes.InvalidPassword && connString == TestUtil.DefaultConnectionString)
                throw new Exception("Please create a user npgsql_tests as follows: CREATE USER npgsql_tests PASSWORD 'npgsql_tests' SUPERUSER");

            if (e.SqlState == PostgresErrorCodes.InvalidCatalogName)
            {
                var builder = new NpgsqlConnectionStringBuilder(connString)
                {
                    Pooling = false,
                    Multiplexing = false,
                    Database = "postgres"
                };

                using var adminConn = new NpgsqlConnection(builder.ConnectionString);
                adminConn.Open();
                adminConn.ExecuteNonQuery("CREATE DATABASE " + conn.Database);
                adminConn.Close();
                Thread.Sleep(1000);

                conn.Open();
                return;
            }

            throw;
        }
    }

    //[OneTimeTearDown]
    //public async Task Teardown()
    //{
    //    if (_container != null)
    //    {
    //        await _container.DisposeAsync();
    //    }
    //}

    static async Task<PostgreSqlContainer> SetupContainerAsync()
    {
        var repoRoot = GetRepoRoot();
        var initScriptPath = Path.Combine(repoRoot, "test", "containers", "postgres", "init-db.sh");
        var certsPath = Path.Combine(repoRoot, ".build");

        if (!File.Exists(initScriptPath))
            throw new InvalidOperationException($"Init script not found: {initScriptPath}");
        if (!Directory.Exists(certsPath))
            throw new InvalidOperationException($"Certs directory not found: {certsPath}");

        var image = Environment.GetEnvironmentVariable("NPGSQL_TEST_IMAGE") ?? "postgres:18";

        var builder = new PostgreSqlBuilder(image)
            .WithDatabase("npgsql_tests")
            .WithUsername("npgsql_tests")
            .WithPassword("npgsql_tests")
            .WithPortBinding(5432, false)
            .WithBindMount(certsPath, "/certs", AccessMode.ReadOnly)
            .WithBindMount(initScriptPath, "/docker-entrypoint-initdb.d/01-init-db.sh", AccessMode.ReadOnly);

        if (!OperatingSystem.IsWindows())
            builder = builder.WithBindMount("/tmp", "/tmp");

        return builder.Build();
    }

    static string GetRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Npgsql.slnx")) || Directory.Exists(Path.Combine(dir.FullName, ".git")))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate repo root for testcontainers assets.");
    }
}
