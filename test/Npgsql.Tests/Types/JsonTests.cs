using System;
using System.Data;
using System.IO;
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
    public Task As_MemoryStream()
        => AssertTypeWrite(() => new MemoryStream("""{"K": "V"}"""u8.ToArray()), """{"K": "V"}""", PostgresType, NpgsqlDbType, isDefault: false);

    [Test]
    public async Task As_JsonDocument()
        => await AssertType(
            JsonDocument.Parse("""{"K": "V"}"""),
            IsJsonb ? """{"K": "V"}""" : """{"K":"V"}""",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            comparer: (x, y) => x.RootElement.GetProperty("K").GetString() == y.RootElement.GetProperty("K").GetString());

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/5540")]
    public async Task As_JsonDocument_with_null_root()
        => await AssertType(
            JsonDocument.Parse("null"),
            "null",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            comparer: (x, y) => x.RootElement.ValueKind == y.RootElement.ValueKind,
            skipArrayCheck: true);

    [Test]
    public async Task As_JsonElement_with_null_root()
        => await AssertType(
            JsonDocument.Parse("null").RootElement,
            "null",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            comparer: (x, y) => x.ValueKind == y.ValueKind,
            skipArrayCheck: true);

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

    [Test]
    public Task Roundtrip_string()
        => AssertType(
            @"{""p"": 1}",
            @"{""p"": 1}",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Roundtrip_char_array()
        => AssertType(
            @"{""p"": 1}".ToCharArray(),
            @"{""p"": 1}",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Roundtrip_byte_array()
        => AssertType(
            Encoding.ASCII.GetBytes(@"{""p"": 1}"),
            @"{""p"": 1}",
            PostgresType,
            NpgsqlDbType,
            isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: false);

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

    public JsonTests(MultiplexingMode multiplexingMode, NpgsqlDbType npgsqlDbType)
        : base(multiplexingMode)
    {
        if (npgsqlDbType == NpgsqlDbType.Jsonb)
            using (var conn = OpenConnection())
                TestUtil.MinimumPgVersion(conn, "9.4.0", "JSONB data type not yet introduced");

        NpgsqlDbType = npgsqlDbType;
    }

    bool IsJsonb => NpgsqlDbType == NpgsqlDbType.Jsonb;
    string PostgresType => IsJsonb ? "jsonb" : "json";
    readonly NpgsqlDbType NpgsqlDbType;
}
