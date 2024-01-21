using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable AccessToDisposedClosure

namespace Npgsql.PluginTests;

/// <summary>
/// Tests for the Npgsql.Json.NET mapping plugin
/// </summary>
[TestFixture(NpgsqlDbType.Jsonb)]
// [TestFixture(NpgsqlDbType.Json)]
public class JsonNetDynamicTests : TestBase
{
    [Test]
    public async Task Roundtrip_dynamic_array()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.EnableDynamicJson().UseJsonNet();
        await using var dataSource = dataSourceBuilder.Build();


        object jObject = new { name = "value1", amount = 1, unit = "ml" };
        object array = new[] { jObject, new { name = "value2", amount = 2, unit = "g" } };

        var connection = await dataSource.OpenConnectionAsync();
        var tableName = await TestUtil.CreateTempTable(connection
            , @$"key SERIAL PRIMARY KEY
              , ingredient_object {_npgsqlDbType}, ingredients_array {_npgsqlDbType}
              , integer_array {_npgsqlDbType}, string_array {_npgsqlDbType}");
        await using var write = new NpgsqlCommand { Connection = connection };
        write.CommandText = @$"
INSERT INTO {tableName}
(ingredient_object, ingredients_array, integer_array, string_array)
VALUES (@p1, @p2, @p3, @p4)";
        write.Parameters.Add(new("p1", jObject) { NpgsqlDbType = _npgsqlDbType });
        write.Parameters.Add(new("p2", array) { NpgsqlDbType = _npgsqlDbType });
        write.Parameters.Add(new("p3", new[] { 1, 2 }) { NpgsqlDbType = _npgsqlDbType });
        write.Parameters.Add(new("p4", new[] { "test", "test" }) { NpgsqlDbType = _npgsqlDbType });
        await write.ExecuteNonQueryAsync();

        await using var read = new NpgsqlCommand { Connection = connection };
        read.CommandText = $"select * from {tableName}";
        var reader = await read.ExecuteReaderAsync();
        await reader.ReadAsync();
        Dictionary<int, Type> expected = new() {
            { 0, typeof(int) }
            , { 1, typeof(JObject) }
            , { 2, typeof(JArray) }
            , { 3, typeof(JArray) }
            , { 4, typeof(JArray) }
        };
        foreach (var kvp in expected)
        {
            var value = reader[kvp.Key];
            Assert.NotNull(value);
            Assert.AreNotEqual(value, DBNull.Value);
            Assert.That(value, Is.TypeOf(kvp.Value));
        }
    }

    readonly NpgsqlDbType _npgsqlDbType;
    readonly string _pgTypeName;

    public JsonNetDynamicTests(NpgsqlDbType npgsqlDbType)
    {
        _npgsqlDbType = npgsqlDbType;
        _pgTypeName = npgsqlDbType.ToString().ToLower();
    }

    public override string ConnectionString => base.ConnectionString + ";Include Error Detail=True;";

    [OneTimeSetUp]
    public void SetUp()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseJsonNet();
    }
}
