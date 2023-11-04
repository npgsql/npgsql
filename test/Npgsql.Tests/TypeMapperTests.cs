using Npgsql.Internal;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class TypeMapperTests : TestBase
{
    [Test]
    public async Task ReloadTypes_across_connections_in_data_source()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        // Note that we don't actually create the type in the database at this point; we want to exercise the type being created later,
        // via the data source.

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection1 = await dataSource.OpenConnectionAsync();
        await using var connection2 = await dataSource.OpenConnectionAsync();

        await connection1.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await connection1.ReloadTypesAsync();

        // The data source type mapper has been replaced and connection1 should have the new mapper, but connection2 should retain the older
        // type mapper - where there's no mapping - as long as it's still open
        Assert.ThrowsAsync<InvalidCastException>(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
        Assert.DoesNotThrowAsync(async () => await connection1.ExecuteScalarAsync($"SELECT 'happy'::{type}"));

        // Close connection2 and reopen to make sure it picks up the new type and mapping from the data source
        var connId = connection2.ProcessID;
        await connection2.CloseAsync();
        await connection2.OpenAsync();
        Assert.That(connection2.ProcessID, Is.EqualTo(connId), "Didn't get the same connector back");

        Assert.DoesNotThrowAsync(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
    }

    [Test]
    [NonParallelizable] // Depends on citext which could be dropped concurrently
    public async Task String_to_citext()
    {
        await using var adminConnection = await OpenConnectionAsync();
        await EnsureExtensionAsync(adminConnection, "citext");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.AddTypeInfoResolverFactory(new CitextToStringTypeHandlerResolverFactory());
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand("SELECT @p = 'hello'::citext", connection);
        command.Parameters.AddWithValue("p", "HeLLo");
        Assert.That(command.ExecuteScalar(), Is.True);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4582")]
    [NonParallelizable] // Drops extension
    public async Task Type_in_non_default_schema()
    {
        await using var conn = await OpenConnectionAsync();

        var schemaName = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync(@$"
DROP EXTENSION IF EXISTS citext;
CREATE EXTENSION citext SCHEMA ""{schemaName}""");

        try
        {
            await conn.ReloadTypesAsync();

            var tableName = await CreateTempTable(conn, $"created_by {schemaName}.citext NOT NULL");

            const string expected = "SomeValue";
            await conn.ExecuteNonQueryAsync($"INSERT INTO \"{tableName}\" VALUES('{expected}')");

            var value = (string?)await conn.ExecuteScalarAsync($"SELECT created_by FROM \"{tableName}\" LIMIT 1");
            Assert.That(value, Is.EqualTo(expected));
        }
        finally
        {
            await conn.ExecuteNonQueryAsync(@"DROP EXTENSION citext CASCADE");
        }
    }

    #region Support

    class CitextToStringTypeHandlerResolverFactory : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new Resolver();
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        sealed class Resolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            {
                if (type == typeof(string) || dataTypeName?.UnqualifiedName == "citext")
                    if (options.DatabaseInfo.TryGetPostgresTypeByName("citext", out var pgType))
                        return new(options, new StringTextConverter(options.TextEncoding), options.ToCanonicalTypeId(pgType));

                return null;
            }
        }

    }

    enum Mood { Sad, Ok, Happy }

    #endregion Support
}
