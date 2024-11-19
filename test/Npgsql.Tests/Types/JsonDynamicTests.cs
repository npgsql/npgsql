using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Npgsql.Properties;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

[TestFixture(MultiplexingMode.NonMultiplexing, NpgsqlDbType.Json)]
[TestFixture(MultiplexingMode.NonMultiplexing, NpgsqlDbType.Jsonb)]
[TestFixture(MultiplexingMode.Multiplexing, NpgsqlDbType.Json)]
[TestFixture(MultiplexingMode.Multiplexing, NpgsqlDbType.Jsonb)]
public class JsonDynamicTests : MultiplexingTestBase
{
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
    public async Task As_poco_supported_only_with_EnableDynamicJson()
    {
        // This test uses base.DataSource, which doesn't have EnableDynamicJson()

        var errorMessage = string.Format(
            NpgsqlStrings.DynamicJsonNotEnabled,
            nameof(WeatherForecast),
            nameof(NpgsqlSlimDataSourceBuilder.EnableDynamicJson),
            nameof(NpgsqlDataSourceBuilder));

        var exception = await AssertTypeUnsupportedWrite(
                new WeatherForecast
                {
                    Date = new DateTime(2019, 9, 1),
                    Summary = "Partly cloudy",
                    TemperatureC = 10
                },
                PostgresType,
                base.DataSource);

        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));

        exception = await AssertTypeUnsupportedRead<WeatherForecast>(
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10}"""
                : """{"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""",
            PostgresType,
            base.DataSource);

        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.That(exception.InnerException!.Message, Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task Poco_does_not_stomp_GetValue_string()
    {
        var dataSource = CreateDataSourceBuilder()
            .EnableDynamicJson([typeof(WeatherForecast)], [typeof(WeatherForecast)])
            .Build();
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

    [Test]
    public async Task Custom_JsonSerializerOptions()
    {
        await using var dataSource = CreateDataSourceBuilder()
            .ConfigureJsonOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            .EnableDynamicJson()
            .Build();

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

    [Test, Ignore("TODO We should not change the default type for json/jsonb, it makes little sense.")]
    public async Task Poco_default_mapping()
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        if (IsJsonb)
            dataSourceBuilder.EnableDynamicJson(jsonbClrTypes: [typeof(WeatherForecast)]);
        else
            dataSourceBuilder.EnableDynamicJson(jsonClrTypes: [typeof(WeatherForecast)]);
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
            isDefaultForReading: false,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    #region Polymorphic

    [Test]
    public async Task Poco_polymorphic_mapping()
    {
#if !NET9_0_OR_GREATER
        if (IsJsonb)
            return;
#endif
        await using var dataSource = CreateDataSource(builder =>
        {
            var types = new[] {typeof(WeatherForecast)};
            builder
#if NET9_0_OR_GREATER
                .ConfigureJsonOptions(new() { AllowOutOfOrderMetadataProperties = true })
#endif
                .EnableDynamicJson(jsonClrTypes: IsJsonb ? [] : types, jsonbClrTypes: !IsJsonb ? [] : types);
        });

        var value = new ExtendedDerivedWeatherForecast
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        // Note: we assert a specific string representation, though jsonb doesn't guarantee the property ordering; so the assert may break
        // for jsonb if PostgreSQL changes its implementation.
        var sql =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "$type": "extended", "Summary": "Partly cloudy", "TemperatureC": 10, "TemperatureF": 49}"""
                : """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite(dataSource, value, sql, PostgresType, NpgsqlDbType, isNpgsqlDbTypeInferredFromClrType: false);
        await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value, isDefault: false);
    }

    [Test]
    public async Task Poco_polymorphic_mapping_read_parents()
    {
#if !NET9_0_OR_GREATER
        if (IsJsonb)
            return;
#endif
        await using var dataSource = CreateDataSource(builder =>
        {
            var types = new[] {typeof(WeatherForecast)};
            builder
#if NET9_0_OR_GREATER
                .ConfigureJsonOptions(new() { AllowOutOfOrderMetadataProperties = true })
#endif
                .EnableDynamicJson(jsonClrTypes: IsJsonb ? [] : types, jsonbClrTypes: !IsJsonb ? [] : types);
        });

        var value = new ExtendedDerivedWeatherForecast
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        // Note: we assert a specific string representation, though jsonb doesn't guarantee the property ordering; so the assert may break
        // for jsonb if PostgreSQL changes its implementation.
        var sql =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "$type": "extended", "Summary": "Partly cloudy", "TemperatureC": 10, "TemperatureF": 49}"""
                : """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite<WeatherForecast>(dataSource, value, sql, PostgresType, NpgsqlDbType,
            isNpgsqlDbTypeInferredFromClrType: false);

        await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value, isDefault: false);
        await AssertTypeRead(dataSource, sql, PostgresType,
            new DerivedWeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            isDefault: false);
        await AssertTypeRead(dataSource, sql, PostgresType, value, isDefault: false);
    }

    [Test]
    public async Task Poco_exact_polymorphic_mapping()
    {
        await using var dataSource = CreateDataSource(builder =>
        {
            var types = new[] {typeof(ExtendedDerivedWeatherForecast)};
            builder
#if NET9_0_OR_GREATER
                .ConfigureJsonOptions(new() { AllowOutOfOrderMetadataProperties = true })
#endif
                .EnableDynamicJson(jsonClrTypes: IsJsonb ? [] : types, jsonbClrTypes: !IsJsonb ? [] : types);
        });

        var value = new ExtendedDerivedWeatherForecast
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        // Note: we assert a specific string representation, though jsonb doesn't guarantee the property ordering; so the assert may break
        // for jsonb if PostgreSQL changes its implementation.
        var sql =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10, "TemperatureF": 49}"""
                : """{"TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite(dataSource, value, sql, PostgresType, NpgsqlDbType, isNpgsqlDbTypeInferredFromClrType: false);
        await AssertTypeRead(dataSource, sql, PostgresType, value, isDefault: false);
    }

    [Test]
    public async Task Poco_unspecified_polymorphic_mapping()
    {
#if !NET9_0_OR_GREATER
        if (IsJsonb)
            return;
#endif

        await using var dataSource = CreateDataSource(builder =>
        {
            builder
#if NET9_0_OR_GREATER
                .ConfigureJsonOptions(new() { AllowOutOfOrderMetadataProperties = true })
#endif
                .EnableDynamicJson();
        });

        var value = new ExtendedDerivedWeatherForecast
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        // Note: we assert a specific string representation, though jsonb doesn't guarantee the property ordering; so the assert may break
        // for jsonb if PostgreSQL changes its implementation.
        var sql =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "$type": "extended", "Summary": "Partly cloudy", "TemperatureC": 10, "TemperatureF": 49}"""
                : """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite(dataSource, value, sql, PostgresType, NpgsqlDbType, isDefault: false);

        // Reading as DerivedWeatherForecast should not cause us to get an instance of ExtendedDerivedWeatherForecast (as it doesn't define JsonDerivedType)
        await AssertTypeRead(dataSource, sql, PostgresType,
            new DerivedWeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            isDefault: false);
        await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value, isDefault: false);
    }

    [Test]
    public async Task Poco_polymorphic_mapping_without_AllowOutOfOrderMetadataProperties()
    {
        await using var dataSource = CreateDataSource(builder =>
        {
            var types = new[] {typeof(WeatherForecast)};
            builder
#if NET9_0_OR_GREATER
                .ConfigureJsonOptions(new() { AllowOutOfOrderMetadataProperties = false })
#endif
                .EnableDynamicJson(jsonClrTypes: IsJsonb ? [] : types, jsonbClrTypes: !IsJsonb ? [] : types);
        });

        var value = new ExtendedDerivedWeatherForecast
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        // Note: we assert a specific string representation, though jsonb doesn't guarantee the property ordering; so the assert may break
        // for jsonb if PostgreSQL changes its implementation.
        var sql =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10, "TemperatureF": 49}"""
                : """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite(dataSource, value, sql, PostgresType, NpgsqlDbType, isNpgsqlDbTypeInferredFromClrType: false);

        // As we have disabled polymorphism for jsonb when AllowOutOfOrderMetadataProperties = false we should be able to read it as equalt to a WeatherForecast instance.
        if (IsJsonb)
            await AssertTypeRead(dataSource, sql, PostgresType,
                new WeatherForecast
                {
                    Date = new DateTime(2019, 9, 1),
                    Summary = "Partly cloudy",
                    TemperatureC = 10
                },
                isDefault: false);

        // Reading as DerivedWeatherForecast should not cause us to get an instance of ExtendedDerivedWeatherForecast (as it doesn't define JsonDerivedType)
        await AssertTypeRead(dataSource, sql, PostgresType,
            new DerivedWeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            isDefault: false);

        // We won't get the original value back for jsonb as we can't support polymorphism without also enforcing AllowOutOfOrderMetadataProperties is true.
        // If we output $type, jsonb won't have that at the start and STJ will throw due to it appearing later in the object. So it's disabled entirely.
        if (!IsJsonb)
            await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value, isDefault: false);
    }

    [Test]
    public async Task Poco_unspecified_polymorphic_mapping_without_AllowOutOfOrderMetadataProperties()
    {
        await using var dataSource = CreateDataSource(builder =>
        {
            builder
#if NET9_0_OR_GREATER
                .ConfigureJsonOptions(new() { AllowOutOfOrderMetadataProperties = false })
#endif
                .EnableDynamicJson();
        });

        var value = new ExtendedDerivedWeatherForecast
        {
            Date = new DateTime(2019, 9, 1),
            Summary = "Partly cloudy",
            TemperatureC = 10
        };

        // Note: we assert a specific string representation, though jsonb doesn't guarantee the property ordering; so the assert may break
        // for jsonb if PostgreSQL changes its implementation.
        var sql =
            IsJsonb
                ? """{"Date": "2019-09-01T00:00:00", "Summary": "Partly cloudy", "TemperatureC": 10, "TemperatureF": 49}"""
                : """{"$type":"extended","TemperatureF":49,"Date":"2019-09-01T00:00:00","TemperatureC":10,"Summary":"Partly cloudy"}""";

        await AssertTypeWrite(dataSource, value, sql, PostgresType, NpgsqlDbType, isDefault: false);

        // As we have disabled polymorphism for jsonb when AllowOutOfOrderMetadataProperties = false we should be able to read it as equalt to a WeatherForecast instance.
        if (IsJsonb)
            await AssertTypeRead(dataSource, sql, PostgresType,
                new WeatherForecast
                {
                    Date = new DateTime(2019, 9, 1),
                    Summary = "Partly cloudy",
                    TemperatureC = 10
                },
                isDefault: false);

        // Reading as DerivedWeatherForecast should not cause us to get an instance of ExtendedDerivedWeatherForecast (as it doesn't define JsonDerivedType)
        await AssertTypeRead(dataSource, sql, PostgresType,
            new DerivedWeatherForecast
            {
                Date = new DateTime(2019, 9, 1),
                Summary = "Partly cloudy",
                TemperatureC = 10
            },
            isDefault: false);

        // We won't get the original value back for jsonb as we can't support polymorphism without also enforcing AllowOutOfOrderMetadataProperties is true.
        // If we output $type, jsonb won't have that at the start and STJ will throw due to it appearing later in the object. So it's disabled entirely.
        if (!IsJsonb)
            await AssertTypeRead<WeatherForecast>(dataSource, sql, PostgresType, value, isDefault: false);
    }

    // ReSharper disable UnusedAutoPropertyAccessor.Local
    // ReSharper disable UnusedMember.Local
    [JsonDerivedType(typeof(ExtendedDerivedWeatherForecast), typeDiscriminator: "extended")]
    record WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; } = "";
    }

    record DerivedWeatherForecast : WeatherForecast;

    record ExtendedDerivedWeatherForecast : DerivedWeatherForecast
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
    // ReSharper restore UnusedMember.Local
    // ReSharper restore UnusedAutoPropertyAccessor.Local

    #endregion Polymorphic

    public JsonDynamicTests(MultiplexingMode multiplexingMode, NpgsqlDbType npgsqlDbType)
        : base(multiplexingMode)
    {
        DataSource = CreateDataSource(b => b.EnableDynamicJson());

        if (npgsqlDbType == NpgsqlDbType.Jsonb)
            using (var conn = OpenConnection())
                TestUtil.MinimumPgVersion(conn, "9.4.0", "JSONB data type not yet introduced");

        NpgsqlDbType = npgsqlDbType;
    }

    protected override NpgsqlDataSource DataSource { get; }

    bool IsJsonb => NpgsqlDbType == NpgsqlDbType.Jsonb;
    string PostgresType => IsJsonb ? "jsonb" : "json";
    readonly NpgsqlDbType NpgsqlDbType;
}
