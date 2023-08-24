using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

[TestFixture(MultiplexingMode.NonMultiplexing, NpgsqlDbType.Json)]
[TestFixture(MultiplexingMode.NonMultiplexing, NpgsqlDbType.Jsonb)]
[TestFixture(MultiplexingMode.Multiplexing, NpgsqlDbType.Json)]
[TestFixture(MultiplexingMode.Multiplexing, NpgsqlDbType.Jsonb)]
public class JsonTests : MultiplexingTestBase
{
    [Test]
    public async Task As_string()
        => await AssertType("""{"K": "V"}""", """{"K": "V"}""", PostgresType, NpgsqlDbType, isDefaultForWriting: false);

    [Test]
    public async Task As_string_long()
    {
        await using var conn = CreateConnection();

        var value = new StringBuilder()
            .Append(@"{""K"": """)
            .Append('x', conn.Settings.WriteBufferSize)
            .Append(@"""}")
            .ToString();

        await AssertType(value, value, PostgresType, NpgsqlDbType, isDefaultForWriting: false);
    }

    [Test]
    public async Task As_string_with_GetTextReader()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($$"""SELECT '{"K": "V"}'::{{PostgresType}}""", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        using var textReader = await reader.GetTextReaderAsync(0);
        Assert.That(await textReader.ReadToEndAsync(), Is.EqualTo(@"{""K"": ""V""}"));
    }

    [Test]
    public async Task As_char_array()
        => await AssertType("""{"K": "V"}""".ToCharArray(), """{"K": "V"}""", PostgresType, NpgsqlDbType, isDefault: false);

    [Test]
    public async Task As_bytes()
        => await AssertType("""{"K": "V"}"""u8.ToArray(), """{"K": "V"}""", PostgresType, NpgsqlDbType, isDefault: false);

    [Test]
    public async Task Write_as_ReadOnlyMemory_of_byte()
        => await AssertTypeWrite(new ReadOnlyMemory<byte>("""{"K": "V"}"""u8.ToArray()), """{"K": "V"}""", PostgresType, NpgsqlDbType,
            isDefault: false);

    [Test]
    public async Task Write_as_ArraySegment_of_char()
        => await AssertTypeWrite(new ArraySegment<char>("""{"K": "V"}""".ToCharArray()), """{"K": "V"}""", PostgresType, NpgsqlDbType,
            isDefault: false);

    [Test]
    public async Task As_JsonDocument()
        => await AssertType(
            JsonDocument.Parse("""{"K": "V"}"""),
            IsJsonb ? """{"K": "V"}""" : """{"K":"V"}""",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            comparer: (x, y) => x.RootElement.GetProperty("K").GetString() == y.RootElement.GetProperty("K").GetString());

    [Test]
    public async Task As_JsonDocument_supported_only_with_SystemTextJson()
    {
        await using var slimDataSource = new NpgsqlSlimDataSourceBuilder(ConnectionString).Build();

        await AssertTypeUnsupported(
            JsonDocument.Parse("""{"K": "V"}"""),
            """{"K": "V"}""",
            PostgresType,
            slimDataSource);
    }

#if NET6_0_OR_GREATER
    [Test]
    public Task Roundtrip_JsonObject()
        => AssertType(
            new JsonObject { ["Bar"] = 8 },
            IsJsonb ? """{"Bar": 8}""" : """{"Bar":8}""",
            PostgresType,
            NpgsqlDbType,
            // By default we map JsonObject to jsonb
            isDefaultForWriting: IsJsonb,
            isDefaultForReading: false,
            isNpgsqlDbTypeInferredFromClrType: false,
            comparer: (x, y) => x.ToString() == y.ToString());

    [Test]
    public Task Roundtrip_JsonArray()
        => AssertType(
            new JsonArray { 1, 2, 3 },
            IsJsonb ? "[1, 2, 3]" : "[1,2,3]",
            PostgresType,
            NpgsqlDbType,
            // By default we map JsonArray to jsonb
            isDefaultForWriting: IsJsonb,
            isDefaultForReading: false,
            isNpgsqlDbTypeInferredFromClrType: false,
            comparer: (x, y) => x.ToString() == y.ToString());
#endif

    [Test]
    public async Task As_poco()
        => await AssertType(
            new WeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10}"""
                : """{"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""",
            PostgresType,
            NpgsqlDbType,
            isDefault: false);

    [Test]
    public async Task As_poco_long()
    {
        using var conn = CreateConnection();
        var bigString = new string('x', Math.Max(conn.Settings.ReadBufferSize, conn.Settings.WriteBufferSize));

        await AssertType(
            new WeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = bigString,
                TemperatureC = 10
            },
            // Warning: in theory jsonb order and whitespace may change across versions
            IsJsonb
                ? $$"""{"Date": "2019-09-01T00:00:00", "Summary": "{{bigString}}", "TemperatureC": 10}"""
                : $$"""{"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"{{bigString}}"}""",
            PostgresType,
            NpgsqlDbType,
            isDefault: false);
    }

    [Test]
    public async Task As_poco_supported_only_with_SystemTextJson()
    {
        await using var slimDataSource = new NpgsqlSlimDataSourceBuilder(ConnectionString).Build();

        await AssertTypeUnsupported(
            new WeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10}""",
            PostgresType,
            slimDataSource);
    }

    [Test]
    public async Task Poco_does_not_stomp_GetValue_string()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        var dataSource = dataSourceBuilder.UseSystemTextJson(null, new[] {typeof(WeatherForecast)}, new[] {typeof(WeatherForecast)}).Build();
        var sqlLiteral =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10}"""
                : """{"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT '{sqlLiteral}'::{(IsJsonb ? "jsonb" : "json")}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader.GetValue(0), Is.TypeOf<string>());
    }

    [JsonDerivedType(typeof(ExtendedDerivedWeatherForecast), typeDiscriminator: "extended")]
    record WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; } = "";
    }

    record DerivedWeatherForecast : WeatherForecast
    {
    }

    record ExtendedDerivedWeatherForecast : DerivedWeatherForecast
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/2811")]
    [IssueLink("https://github.com/npgsql/efcore.pg/issues/1177")]
    [IssueLink("https://github.com/npgsql/efcore.pg/issues/1082")]
    public async Task Can_read_two_json_documents()
    {
        await using var conn = await OpenConnectionAsync();

        JsonDocument car;
        await using (var cmd = new NpgsqlCommand("""SELECT '{"key" : "foo"}'::jsonb""", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            car = reader.GetFieldValue<JsonDocument>(0);
        }

        await using (var cmd = new NpgsqlCommand("""SELECT '{"key" : "bar"}'::jsonb""", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            reader.GetFieldValue<JsonDocument>(0);
        }

        Assert.That(car.RootElement.GetProperty("key").GetString(), Is.EqualTo("foo"));
    }

#if NET6_0_OR_GREATER
    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4537")]
    public async Task Write_jsonobject_array_without_npgsqldbtype()
    {
        // By default we map JsonObject to jsonb
        if (!IsJsonb)
            return;

        await using var conn = await OpenConnectionAsync();
        var tableName = await TestUtil.CreateTempTable(conn, "key SERIAL PRIMARY KEY, ingredients json[]");

        await using var cmd = new NpgsqlCommand { Connection = conn };

        var jsonObject1 = new JsonObject
        {
            { "name", "value1" },
            { "amount", 1 },
            { "unit", "ml" }
        };

        var jsonObject2 = new JsonObject
        {
            { "name", "value2" },
            { "amount", 2 },
            { "unit", "g" }
        };

        cmd.CommandText = $"INSERT INTO {tableName} (ingredients) VALUES (@p)";
        cmd.Parameters.Add(new("p", new[] { jsonObject1, jsonObject2 }));
        await cmd.ExecuteNonQueryAsync();
    }
#endif

    [Test]
    public async Task Custom_JsonSerializerOptions()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseSystemTextJson(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await using var dataSource = dataSourceBuilder.Build();

        await AssertTypeWrite(
            dataSource,
            new WeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            IsJsonb
                ? """{"date": "2019-09-01T00:00:00", "summary": "Partly cloudy", "temperatureC": 10}"""
                : """{"date":"2019-09-01T00:00:00","temperatureC":10,"summary":"Partly cloudy"}""",
            PostgresType,
            NpgsqlDbType,
            isDefault: false);
    }

    [Test]
    public async Task Poco_default_mapping()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        if (IsJsonb)
            dataSourceBuilder.UseSystemTextJson(jsonbClrTypes: new[] { typeof(WeatherForecast) });
        else
            dataSourceBuilder.UseSystemTextJson(jsonClrTypes: new[] { typeof(WeatherForecast) });
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(
            dataSource,
            new WeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10}"""
                : """{"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""",
            PostgresType,
            NpgsqlDbType,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public async Task Poco_polymorphic_mapping()
    {
        // We don't yet support polymorphic deserialization for jsonb as $type does not come back as the first property.
        // This could be fixed by detecting PolymorphicOptions types, always buffering their values and modifying the text.
        if (IsJsonb)
            return;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseSystemTextJson(jsonClrTypes: new[] { typeof(WeatherForecast) });
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType<WeatherForecast>(
            dataSource,
            new ExtendedDerivedWeatherForecast()
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""",
            PostgresType,
            NpgsqlDbType,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public async Task Poco_polymorphic_mapping_read_parents()
    {
        // We don't yet support polymorphic deserialization for jsonb as $type does not come back as the first property.
        // This could be fixed by detecting PolymorphicOptions types, always buffering their values and modifying the text.
        if (IsJsonb)
            return;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseSystemTextJson(jsonClrTypes: new[] { typeof(WeatherForecast) });
        await using var dataSource = dataSourceBuilder.Build();

        var value = new ExtendedDerivedWeatherForecast()
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        var sql = """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite<WeatherForecast>(
            dataSource,
            value,
            sql,
            PostgresType,
            NpgsqlDbType,
            isNpgsqlDbTypeInferredFromClrType: false);

        // GetValue (so object based, results in resolution based off of WeatherForecast being the default)
        await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value,
            comparer: (_, actual) => actual.GetType() == typeof(ExtendedDerivedWeatherForecast),
            fieldType: typeof(WeatherForecast));
        await AssertTypeRead(dataSource, sql, PostgresType, value, fieldType: typeof(WeatherForecast));

        // GetFieldValue
        await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value,
            comparer: (_, actual) => actual.GetType() == typeof(ExtendedDerivedWeatherForecast),
            isDefault: false);

        await AssertTypeRead<DerivedWeatherForecast>(dataSource, sql, PostgresType, value,
            comparer: (_, actual) => actual.GetType() == typeof(DerivedWeatherForecast), isDefault: false);

        await AssertTypeRead<ExtendedDerivedWeatherForecast>(dataSource, sql, PostgresType, value, isDefault: false);
    }


    [Test]
    public async Task Poco_exact_polymorphic_mapping()
    {
        // We don't yet support polymorphic deserialization for jsonb as $type does not come back as the first property.
        // This could be fixed by detecting PolymorphicOptions types, always buffering their values and modifying the text.
        if (IsJsonb)
            return;

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.UseSystemTextJson(jsonClrTypes: new[] { typeof(ExtendedDerivedWeatherForecast) });
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(
            dataSource,
            new ExtendedDerivedWeatherForecast()
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            """{"TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""",
            PostgresType,
            NpgsqlDbType,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public async Task Poco_unspecified_polymorphic_mapping()
    {
        // We don't yet support polymorphic deserialization for jsonb as $type does not come back as the first property.
        // This could be fixed by detecting PolymorphicOptions types, always buffering their values and modifying the text.
        // In this case we don't have any statically mapped base type to check its PolymorphicOptions on.
        // Detecting whether the type could be polymorphic would require us to duplicate STJ's nearest polymorphic ancestor search.
        if (IsJsonb)
            return;

        var value = new ExtendedDerivedWeatherForecast()
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        var sql = """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertType(
            value,
            sql,
            PostgresType,
            NpgsqlDbType,
            isDefault: false);

        await AssertTypeRead<DerivedWeatherForecast>(DataSource, sql, PostgresType, value,
            comparer: (_, actual) => actual.GetType() == typeof(DerivedWeatherForecast), isDefault: false);

        await AssertTypeRead<WeatherForecast>(DataSource, sql, PostgresType, value,
            comparer: (_, actual) => actual.GetType() == typeof(ExtendedDerivedWeatherForecast), isDefault: false);
    }

    public JsonTests(MultiplexingMode multiplexingMode, NpgsqlDbType npgsqlDbType)
        : base(multiplexingMode)
    {
        using (var conn = OpenConnection())
            TestUtil.MinimumPgVersion(conn, "9.4.0", "JSONB data type not yet introduced");
        NpgsqlDbType = npgsqlDbType;
    }

    bool IsJsonb => NpgsqlDbType == NpgsqlDbType.Jsonb;
    string PostgresType => IsJsonb ? "jsonb" : "json";
    readonly NpgsqlDbType NpgsqlDbType;
}
