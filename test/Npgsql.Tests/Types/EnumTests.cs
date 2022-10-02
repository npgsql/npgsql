using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Util.Statics;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

public class EnumTests : MultiplexingTestBase
{
    enum Mood { Sad, Ok, Happy }

    [PgName("explicitly_named_mood")]
    enum MoodUnmapped { Sad, Ok, Happy };

    [Test]
    public async Task Unmapped_enum()
    {
        await using var connection = await OpenConnectionAsync();
        await using var _ = await GetTempTypeName(connection, out var type);
        await connection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await connection.ReloadTypesAsync();

        await AssertType(connection, Mood.Happy, "happy", type, npgsqlDbType: null, isDefault: false);
    }

    [Test]
    public async Task Data_source_mapping()
    {
        await using var adminConnection = await OpenConnectionAsync();
        await using var _ = await GetTempTypeName(adminConnection, out var type);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await adminConnection.ReloadTypesAsync();

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, Mood.Happy, "happy", type, npgsqlDbType: null);
    }

    [Test]
    public async Task Dual_enums()
    {
        await using var adminConnection = await OpenConnectionAsync();
        await using var _ = await GetTempTypeName(adminConnection, out var type1);
        await using var __ = await GetTempTypeName(adminConnection, out var type2);
        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {type1} AS ENUM ('sad', 'ok', 'happy');
CREATE TYPE {type2} AS ENUM ('label1', 'label2', 'label3')");
        await adminConnection.ReloadTypesAsync();

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
        await using var _ = await GetTempTypeName(adminConnection, out var type);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        await adminConnection.ReloadTypesAsync();

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, new[] { Mood.Ok, Mood.Happy }, "{ok,happy}", type + "[]", npgsqlDbType: null);
    }

    [Test]
    public async Task Read_unmapped_enum_as_string()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        await using var _ = await GetTempTypeName(conn, out var type);

        await conn.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('Sad', 'Ok', 'Happy')");
        conn.ReloadTypes();
        using var cmd = new NpgsqlCommand($"SELECT 'Sad'::{type}, ARRAY['Ok', 'Happy']::{type}[]", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader[0], Is.EqualTo("Sad"));
        Assert.That(reader.GetDataTypeName(0), Is.EqualTo($"public.{type}"));
        Assert.That(reader[1], Is.EqualTo(new[] { "Ok", "Happy" }));
    }

    [Test, Description("Test that a c# string can be written to a backend enum when DbType is unknown")]
    public async Task Write_string_to_backend_enum()
    {
        await using var conn = await OpenConnectionAsync();
        await using var _ = await GetTempTypeName(conn, out var type);
        await using var __ = await GetTempTableName(conn, out var table);
        await conn.ExecuteNonQueryAsync($@"
CREATE TYPE {type} AS ENUM ('Banana', 'Apple', 'Orange');
CREATE TABLE {table} (id SERIAL, value1 {type}, value2 {type});");
        await conn.ReloadTypesAsync();
        const string expected = "Banana";
        using var cmd = new NpgsqlCommand($"INSERT INTO {table} (id, value1, value2) VALUES (default, @p1, @p2);", conn);
        cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Unknown, expected);
        var p2 = new NpgsqlParameter("p1", NpgsqlDbType.Unknown) {Value = expected};
        cmd.Parameters.Add(p2);
        await cmd.ExecuteNonQueryAsync();
    }

    [Test, NonParallelizable]
    public async Task Write_unmapped_enum()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync(@"
DROP TYPE IF EXISTS explicitly_named_mood;
CREATE TYPE explicitly_named_mood AS ENUM ('sad', 'ok', 'happy')");

        await conn.ReloadTypesAsync();

        await using var cmd = new NpgsqlCommand($"SELECT @p::text", conn)
        {
            Parameters = { new("p", MoodUnmapped.Happy) }
        };

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.AreEqual("happy", reader.GetFieldValue<string>(0));
    }

    [Test, Description("Tests that a a C# enum an be written to an enum backend when passed as dbUnknown")]
    public async Task Write_enum_as_NpgsqlDbType_Unknown()
    {
        await using var conn = await OpenConnectionAsync();
        await using var _ = await GetTempTypeName(conn, out var type);
        await using var __ = await GetTempTableName(conn, out var table);
        await conn.ExecuteNonQueryAsync($@"
CREATE TYPE {type} AS ENUM ('Sad', 'Ok', 'Happy');
CREATE TABLE {table} (value1 {type})");
        await conn.ReloadTypesAsync();
        var expected = Mood.Happy;
        using var cmd = new NpgsqlCommand($"INSERT INTO {table} (value1) VALUES (@p1);", conn);
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Unknown, expected);
        await cmd.ExecuteNonQueryAsync();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
    public async Task Name_translation_default_snake_case()
    {
        await using var adminConnection = await OpenConnectionAsync();
        await using var _ = await GetTempTypeName(adminConnection, out var enumName1);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {enumName1} AS ENUM ('simple', 'two_words', 'some_database_name')");
        await adminConnection.ReloadTypesAsync();

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
        await using var _ = await GetTempTypeName(adminConnection, out var type);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('Simple', 'TwoWords', 'some_database_name')");
        await adminConnection.ReloadTypesAsync();

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<NameTranslationEnum>(type, nameTranslator: new NpgsqlNullNameTranslator());
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, NameTranslationEnum.Simple, "Simple", type, npgsqlDbType: null);
        await AssertType(dataSource, NameTranslationEnum.TwoWords, "TwoWords", type, npgsqlDbType: null);
        await AssertType(dataSource, NameTranslationEnum.SomeClrName, "some_database_name", type, npgsqlDbType: null);
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
        await using var _ = await CreateTempSchema(adminConnection, out var schema1);
        await using var __ = await CreateTempSchema(adminConnection, out var schema2);
        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {schema1}.my_enum AS ENUM ('one');
CREATE TYPE {schema2}.my_enum AS ENUM ('alpha');");
        await adminConnection.ReloadTypesAsync();

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
        await using var __ = await GetTempTypeName(conn, out var type);
        await conn.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");
        conn.ReloadTypes();

        using var cmd = new NpgsqlCommand($"SELECT 'ok'::{type}", conn);
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
