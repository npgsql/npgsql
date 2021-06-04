using Npgsql;
using Npgsql.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[SetUpFixture]
public class AssemblySetUp
{
    [OneTimeSetUp]
    public void Setup()
    {
        var connString = TestUtil.ConnectionString;
        using var conn = new NpgsqlConnection(connString);
        try
        {
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
}
