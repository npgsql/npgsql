using Npgsql;
using Npgsql.Tests;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Images;
using Npgsql.Tests.Support;
using Testcontainers.PostgreSql;

[SetUpFixture]
public class AssemblySetUp
{
    PostgreSqlContainer? _postgreSqlContainer;

    protected virtual DockerImage ContainerImage
    {
        get
        {
            // Form native arm64 support, use imresamu/postgis, see https://github.com/postgis/docker-postgis/issues/216#issuecomment-2936824962
            // For example, imresamu/postgis:18-3.6.1-alpine3.23
            var imageName = Environment.GetEnvironmentVariable("NPGSQL_TEST_DOCKER_IMAGE");
            if (imageName != null)
            {
                return new DockerImage(imageName);
            }

            // Explicit linux/amd64 platform since arm64 is not yet supported, see https://github.com/postgis/docker-postgis/issues/216
            return new DockerImage("postgis/postgis:18-3.6", new Platform("linux/amd64"));
        }
    }

    protected virtual string? ContainerName => typeof(AssemblySetUp).Assembly.GetName().Name;

    [OneTimeSetUp]
    public async Task Setup()
    {
        try
        {
            CheckConnection();
        }
        catch when (Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") == null)
        {
            // Connection to the default connection string failed, use Docker to run PostgreSQL
            _postgreSqlContainer = new PostgreSqlBuilder(ContainerImage).WithName(ContainerName).Build();
            await _postgreSqlContainer.StartAsync();
            await _postgreSqlContainer.EnableSslAsync();

            TestUtil.ConnectionString = _postgreSqlContainer.GetConnectionString();
            CheckConnection();
        }
    }

    static void CheckConnection()
    {
        var connString = TestUtil.ConnectionString;
        using var conn = new NpgsqlConnection(connString);
        try
        {
            conn.Open();
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

    [OneTimeTearDown]
    public async Task TearDown()
    {
        if (_postgreSqlContainer != null)
        {
            await _postgreSqlContainer.DisposeAsync();
        }
    }
}
