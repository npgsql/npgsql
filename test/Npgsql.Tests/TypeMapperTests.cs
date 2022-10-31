using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class TypeMapperTests : TestBase
{
#pragma warning disable CS0618 // GlobalTypeMapper is obsolete
    [Test, NonParallelizable]
    public async Task Global_mapping()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>(type);

        try
        {
            await using var dataSource1 = CreateDataSource();

            await using (var connection = await dataSource1.OpenConnectionAsync())
            {
                await connection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
                await connection.ReloadTypesAsync();

                await AssertType(connection, Mood.Happy, "happy", type, npgsqlDbType: null);
            }

            NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>(type);

            // Global mapping changes have no effect on already-built data sources
            await AssertType(dataSource1, Mood.Happy, "happy", type, npgsqlDbType: null);

            // But they do affect on new data sources
            await using var dataSource2 = CreateDataSource();
            Assert.ThrowsAsync<ArgumentException>(() => AssertType(dataSource2, Mood.Happy, "happy", type, npgsqlDbType: null));
        }
        finally
        {
            NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>(type);
        }
    }

    [Test, NonParallelizable]
    public async Task Global_mapping_reset()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>(type);

        try
        {
            await using var dataSource1 = CreateDataSource();

            await using (var connection = await dataSource1.OpenConnectionAsync())
            {
                await connection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
                await connection.ReloadTypesAsync();
            }

            // A global mapping change has no effects on data sources which have already been built
            NpgsqlConnection.GlobalTypeMapper.Reset();

            // Global mapping changes have no effect on already-built data sources
            await AssertType(dataSource1, Mood.Happy, "happy", type, npgsqlDbType: null);

            // But they do affect on new data sources
            await using var dataSource2 = CreateDataSource();
            Assert.ThrowsAsync<ArgumentException>(() => AssertType(dataSource2, Mood.Happy, "happy", type, npgsqlDbType: null));
        }
        finally
        {
            NpgsqlConnection.GlobalTypeMapper.Reset();
        }
    }
#pragma warning restore CS0618 // GlobalTypeMapper is obsolete

    [Test]
    public async Task ReloadTypes_across_connections_in_data_source()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        // Note that we don't actually create the type in the database at this point; we want to exercise the type being created later,
        // via the data source.

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>();
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection1 = await dataSource.OpenConnectionAsync();
        await using var connection2 = await dataSource.OpenConnectionAsync();

        await connection1.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await connection1.ReloadTypesAsync();

        // The data source type mapper has been replaced and connection1 should have the new mapper, but connection2 should retain the older
        // type mapper - where there's no mapping - as long as it's still open
        Assert.DoesNotThrowAsync(async () => await connection1.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
        Assert.ThrowsAsync<NotSupportedException>(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));

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
        dataSourceBuilder.AddTypeResolverFactory(new CitextToStringTypeHandlerResolverFactory());
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

    class CitextToStringTypeHandlerResolverFactory : TypeHandlerResolverFactory
    {
        public override TypeHandlerResolver Create(NpgsqlConnector connector)
            => new CitextToStringTypeHandlerResolver(connector);

        public override TypeMappingInfo GetMappingByDataTypeName(string dataTypeName) => throw new NotSupportedException();
        public override string GetDataTypeNameByClrType(Type clrType) => throw new NotSupportedException();
        public override string GetDataTypeNameByValueDependentValue(object value) => throw new NotSupportedException();

        class CitextToStringTypeHandlerResolver : TypeHandlerResolver
        {
            readonly NpgsqlConnector _connector;
            readonly PostgresType _pgCitextType;

            public CitextToStringTypeHandlerResolver(NpgsqlConnector connector)
            {
                _connector = connector;
                _pgCitextType = connector.DatabaseInfo.GetPostgresTypeByName("citext");
            }

            public override NpgsqlTypeHandler? ResolveByClrType(Type type)
                => type == typeof(string) ? new TextHandler(_pgCitextType, _connector.TextEncoding) : null;
            public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName) => null;

            public override TypeMappingInfo? GetMappingByDataTypeName(string dataTypeName) => throw new NotSupportedException();
        }
    }

    enum Mood { Sad, Ok, Happy }

    #endregion Support
}
