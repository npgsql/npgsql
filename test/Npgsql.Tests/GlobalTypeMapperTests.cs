using System;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

#pragma warning disable CS0618 // GlobalTypeMapper is obsolete

[NonParallelizable]
public class GlobalTypeMapperTests : TestBase
{
    [Test]
    public async Task MapEnum()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>(type);

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

        // But they do affect new data sources
        await using var dataSource2 = CreateDataSource();
        await AssertType(dataSource2, "happy", "happy", type, npgsqlDbType: null, isDefault: false);
    }

    [Test]
    public async Task MapEnum_NonGeneric()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        NpgsqlConnection.GlobalTypeMapper.MapEnum(typeof(Mood), type);

        try
        {
            await using var dataSource1 = CreateDataSource();

            await using (var connection = await dataSource1.OpenConnectionAsync())
            {
                await connection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
                await connection.ReloadTypesAsync();

                await AssertType(connection, Mood.Happy, "happy", type, npgsqlDbType: null);
            }

            NpgsqlConnection.GlobalTypeMapper.UnmapEnum(typeof(Mood), type);

            // Global mapping changes have no effect on already-built data sources
            await AssertType(dataSource1, Mood.Happy, "happy", type, npgsqlDbType: null);

            // But they do affect new data sources
            await using var dataSource2 = CreateDataSource();
            Assert.ThrowsAsync<InvalidCastException>(() => AssertType(dataSource2, Mood.Happy, "happy", type, npgsqlDbType: null));
        }
        finally
        {
            NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>(type);
        }
    }

    [Test]
    public async Task Reset()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>(type);

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

        // But they do affect new data sources
        await using var dataSource2 = CreateDataSource();
        await AssertType(dataSource2, "happy", "happy", type, npgsqlDbType: null, isDefault: false);
    }

    [Test]
    public void Reset_and_add_resolver()
    {
        NpgsqlConnection.GlobalTypeMapper.Reset();
        NpgsqlConnection.GlobalTypeMapper.AddTypeInfoResolverFactory(new DummyResolverFactory());
    }

    [TearDown]
    public void Teardown()
        => NpgsqlConnection.GlobalTypeMapper.Reset();

    enum Mood { Sad, Ok, Happy }

    class DummyResolverFactory : PgTypeInfoResolverFactory
    {
        public override IPgTypeInfoResolver CreateResolver() => new DummyResolver();
        public override IPgTypeInfoResolver? CreateArrayResolver() => null;

        class DummyResolver : IPgTypeInfoResolver
        {
            public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options) => null;
        }
    }
}
