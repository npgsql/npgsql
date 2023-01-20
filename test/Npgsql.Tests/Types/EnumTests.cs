using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

public class EnumTests : MultiplexingTestBase
{
    enum Mood { Sad, Ok, Happy }
    enum AnotherEnum { Value1, Value2 }

    [Test]
    public async Task Data_source_mapping()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, Mood.Happy, "happy", type, npgsqlDbType: null);
    }

    [Test]
    public async Task Dual_enums()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type1 = await GetTempTypeName(adminConnection);
        var type2 = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {type1} AS ENUM ('sad', 'ok', 'happy');
CREATE TYPE {type2} AS ENUM ('label1', 'label2', 'label3')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type1);
        dataSourceBuilder.MapEnum<TestEnum>(type2);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, new[] { Mood.Ok, Mood.Sad }, "{ok,sad}", type1 + "[]", npgsqlDbType: null);
    }

    [Test]
    public async Task Array()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, new[] { Mood.Ok, Mood.Happy }, "{ok,happy}", type + "[]", npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
    public async Task Name_translation_default_snake_case()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var enumName1 = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {enumName1} AS ENUM ('simple', 'two_words', 'some_database_name')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<NameTranslationEnum>(enumName1);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, NameTranslationEnum.Simple, "simple", enumName1, npgsqlDbType: null);
        await AssertType(dataSource, NameTranslationEnum.TwoWords, "two_words", enumName1, npgsqlDbType: null);
        await AssertType(dataSource, NameTranslationEnum.SomeClrName, "some_database_name", enumName1, npgsqlDbType: null);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
    public async Task Name_translation_null()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('Simple', 'TwoWords', 'some_database_name')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<NameTranslationEnum>(type, nameTranslator: new NpgsqlNullNameTranslator());
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, NameTranslationEnum.Simple, "Simple", type, npgsqlDbType: null);
        await AssertType(dataSource, NameTranslationEnum.TwoWords, "TwoWords", type, npgsqlDbType: null);
        await AssertType(dataSource, NameTranslationEnum.SomeClrName, "some_database_name", type, npgsqlDbType: null);
    }

    [Test]
    public async Task Unmapped_enum_as_clr_enum()
    {
        await using var connection = await OpenConnectionAsync();
        var type1 = await GetTempTypeName(connection);
        var type2 = await GetTempTypeName(connection);
        await connection.ExecuteNonQueryAsync(@$"
CREATE TYPE {type1} AS ENUM ('sad', 'ok', 'happy');
CREATE TYPE {type2} AS ENUM ('value1', 'value2');");
        await connection.ReloadTypesAsync();

        await AssertType(connection, Mood.Happy, "happy", type1, npgsqlDbType: null, isDefault: false);
        await AssertType(connection, AnotherEnum.Value2, "value2", type2, npgsqlDbType: null, isDefault: false);
    }

    [Test]
    public async Task Unmapped_enum_as_string()
    {
        await using var connection = await OpenConnectionAsync();
        var type = await GetTempTypeName(connection);
        await connection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await connection.ReloadTypesAsync();

        await AssertType(connection, "happy", "happy", type, npgsqlDbType: null, isDefaultForWriting: false);
    }

    enum NameTranslationEnum
    {
        Simple,
        TwoWords,
        [PgName("some_database_name")]
        SomeClrName
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/632")]
    public async Task Same_name_in_different_schemas()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var schema1 = await CreateTempSchema(adminConnection);
        var schema2 = await CreateTempSchema(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {schema1}.my_enum AS ENUM ('one');
CREATE TYPE {schema2}.my_enum AS ENUM ('alpha');");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Enum1>($"{schema1}.my_enum");
        dataSourceBuilder.MapEnum<Enum2>($"{schema2}.my_enum");
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, Enum1.One, "one", $"{schema1}.my_enum", npgsqlDbType: null);
        await AssertType(dataSource, Enum2.Alpha, "alpha", $"{schema2}.my_enum", npgsqlDbType: null);
    }

    enum Enum1 { One }
    enum Enum2 { Alpha }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1779")]
    public async Task GetPostgresType()
    {
        using var _ = CreateTempPool(ConnectionString, out var connectionString);
        using var conn = await OpenConnectionAsync(connectionString);
        var type = await GetTempTypeName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        conn.ReloadTypes();

        using var cmd = new NpgsqlCommandOrig($"SELECT 'ok'::{type}", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        var enumType = (PostgresEnumType)reader.GetPostgresType(0);
        Assert.That(enumType.Name, Is.EqualTo(type));
        Assert.That(enumType.Labels, Is.EqualTo(new List<string> { "sad", "ok", "happy" }));
    }

    enum TestEnum
    {
        label1,
        label2,
        [PgName("label3")]
        Label3
    }

    public EnumTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
