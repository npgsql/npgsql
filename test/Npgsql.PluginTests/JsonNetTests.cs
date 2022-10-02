using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable AccessToModifiedClosure
// ReSharper disable AccessToDisposedClosure

namespace Npgsql.PluginTests;

/// <summary>
/// Tests for the Npgsql.Json.NET mapping plugin
/// </summary>
[NonParallelizable]
[TestFixture(NpgsqlDbType.Jsonb)]
[TestFixture(NpgsqlDbType.Json)]
public class JsonNetTests : TestBase
{
    [Test]
    public Task Roundtrip_object()
        => AssertType(
            JsonDataSource,
            new Foo { Bar = 8 },
            IsJsonb ? @"{""Bar"": 8}" : @"{""Bar"":8}",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3085")]
    public Task Roundtrip_string()
        => AssertType(
            JsonDataSource,
            @"{""p"": 1}",
            @"{""p"": 1}",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3085")]
    public Task Roundtrip_char_array()
        => AssertType(
            JsonDataSource,
            @"{""p"": 1}".ToCharArray(),
            @"{""p"": 1}",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3085")]
    public Task Roundtrip_byte_array()
        => AssertType(
            JsonDataSource,
            Encoding.ASCII.GetBytes(@"{""p"": 1}"),
            @"{""p"": 1}",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Roundtrip_JObject()
        => AssertType(
            JsonDataSource,
            new JObject { ["Bar"] = 8 },
            IsJsonb ? @"{""Bar"": 8}" : @"{""Bar"":8}",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Roundtrip_JArray()
        => AssertType(
            JsonDataSource,
            new JArray(new[] { 1, 2, 3 }),
            IsJsonb ? "[1, 2, 3]" : "[1,2,3]",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public async Task Deserialize_failure()
    {
        await using var conn = await JsonDataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($@"SELECT '[1, 2, 3]'::{_pgTypeName}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        // Attempt to deserialize JSON array into object
        Assert.That(() => reader.GetFieldValue<Foo>(0), Throws.TypeOf<JsonSerializationException>());
        // State should still be OK to continue
        var actual = reader.GetFieldValue<JArray>(0);
        Assert.That((int)actual[0], Is.EqualTo(1));
    }

    [Test]
    public async Task Clr_type_mapping()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        if (IsJsonb)
            dataSourceBuilder.UseJsonNet(jsonbClrTypes: new[] { typeof(Foo) });
        else
            dataSourceBuilder.UseJsonNet(jsonClrTypes: new[] { typeof(Foo) });
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(
            dataSource,
            new Foo { Bar = 8 },
            IsJsonb ? @"{""Bar"": 8}" : @"{""Bar"":8}",
            _pgTypeName,
            _npgsqlDbType,
            isDefaultForReading: false,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public async Task Roundtrip_clr_array()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        if (IsJsonb)
            dataSourceBuilder.UseJsonNet(jsonbClrTypes: new[] { typeof(int[]) });
        else
            dataSourceBuilder.UseJsonNet(jsonClrTypes: new[] { typeof(int[]) });
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(
            dataSource,
            new[] { 1, 2, 3 },
            IsJsonb ? "[1, 2, 3]" : "[1,2,3]",
            _pgTypeName,
            _npgsqlDbType,
            isDefaultForReading: false,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    class DateWrapper
    {
        public DateTime Date;
        public override bool Equals(object? obj) => (obj as DateWrapper)?.Date == Date;
        public override int GetHashCode() => Date.GetHashCode();
    }

    [Test]
    public async Task Custom_serializer_settings()
    {
        var settings = new JsonSerializerSettings { DateFormatString = @"T\he d\t\h o\f MMMM, yyyy" };

        var dataSourceBuilder = CreateDataSourceBuilder();
        if (IsJsonb)
            dataSourceBuilder.UseJsonNet(jsonbClrTypes: new[] { typeof(DateWrapper) }, settings: settings);
        else
            dataSourceBuilder.UseJsonNet(jsonClrTypes: new[] { typeof(DateWrapper) }, settings: settings);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(
            dataSource,
            new DateWrapper { Date = new DateTime(2018, 04, 20) },
            IsJsonb ? "{\"Date\": \"The 20th of April, 2018\"}" : "{\"Date\":\"The 20th of April, 2018\"}",
            _pgTypeName,
            _npgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);
    }
    [Test]
    public async Task Bug3464()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseJsonNet(new[] { typeof(Bug3464Class) });
        await using var dataSource = dataSourceBuilder.Build();

        var expected = new Bug3464Class { SomeString = new string('5', 8174) };
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(@"SELECT @p1, @p2", conn);

        cmd.Parameters.AddWithValue("p1", expected).NpgsqlDbType = _npgsqlDbType;
        cmd.Parameters.AddWithValue("p2", expected).NpgsqlDbType = _npgsqlDbType;

        await using var reader = cmd.ExecuteReader();
    }

    public class Bug3464Class
    {
        public string? SomeString { get; set; }
    }

    readonly NpgsqlDbType _npgsqlDbType;
    readonly string _pgTypeName;

    class Foo
    {
        public int Bar { get; set; }
        public override bool Equals(object? obj) => (obj as Foo)?.Bar == Bar;
        public override int GetHashCode() => Bar.GetHashCode();
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseJsonNet();
        JsonDataSource = dataSourceBuilder.Build();
    }

    [OneTimeTearDown]
    public async Task Teardown()
        => await JsonDataSource.DisposeAsync();

    public JsonNetTests(NpgsqlDbType npgsqlDbType)
    {
        _npgsqlDbType = npgsqlDbType;
        _pgTypeName = npgsqlDbType.ToString().ToLower();
    }

    bool IsJsonb => _npgsqlDbType == NpgsqlDbType.Jsonb;

    NpgsqlDataSource JsonDataSource = default!;
}
