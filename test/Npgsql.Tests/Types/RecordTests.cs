using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.Properties;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types;

public class RecordTests : MultiplexingTestBase
{
    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/724")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/1980")]
    public async Task Read_Record_as_object_array()
    {
        var recordLiteral = "(1,'foo'::text)::record";
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT {recordLiteral}, ARRAY[{recordLiteral}, {recordLiteral}]", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        var record = (object[])reader[0];
        Assert.That(record[0], Is.EqualTo(1));
        Assert.That(record[1], Is.EqualTo("foo"));

        var array = (object[][])reader[1];
        Assert.That(array.Length, Is.EqualTo(2));
        Assert.That(array[0][0], Is.EqualTo(1));
        Assert.That(array[1][0], Is.EqualTo(1));
    }

    [Test]
    public async Task Read_Record_as_ValueTuple()
    {
        var recordLiteral = "(1,'foo'::text)::record";
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT {recordLiteral}, ARRAY[{recordLiteral}, {recordLiteral}]", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        var record = reader.GetFieldValue<(int, string)>(0);
        Assert.That(record.Item1, Is.EqualTo(1));
        Assert.That(record.Item2, Is.EqualTo("foo"));

        var array = (object[][])reader[1];
        Assert.That(array.Length, Is.EqualTo(2));
        Assert.That(array[0][0], Is.EqualTo(1));
        Assert.That(array[1][0], Is.EqualTo(1));
    }

    [Test]
    public async Task Read_Record_as_Tuple()
    {
        var recordLiteral = "(1,'foo'::text)::record";
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT {recordLiteral}, ARRAY[{recordLiteral}, {recordLiteral}]", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        var record = reader.GetFieldValue<Tuple<int, string>>(0);
        Assert.That(record.Item1, Is.EqualTo(1));
        Assert.That(record.Item2, Is.EqualTo("foo"));

        var array = (object[][])reader[1];
        Assert.That(array.Length, Is.EqualTo(2));
        Assert.That(array[0][0], Is.EqualTo(1));
        Assert.That(array[1][0], Is.EqualTo(1));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1238")]
    public async Task Record_with_non_int_field()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT ('one'::TEXT, 2)", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        var record = reader.GetFieldValue<object[]>(0);
        Assert.That(record[0], Is.EqualTo("one"));
        Assert.That(record[1], Is.EqualTo(2));
    }

    [Test]
    public async Task Records_not_supported_by_default_on_NpgsqlSlimSourceBuilder()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        // RecordHandler doesn't support writing, so we only check for reading
        cmd.CommandText = "SELECT ('one'::text, 2)";
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        var errorMessage = string.Format(
            NpgsqlStrings.RecordsNotEnabled,
            nameof(NpgsqlSlimDataSourceBuilder.EnableRecords),
            nameof(NpgsqlSlimDataSourceBuilder));

        var exception = Assert.Throws<InvalidCastException>(() => reader.GetValue(0))!;
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.AreEqual(errorMessage, exception.InnerException!.Message);

        exception = Assert.Throws<InvalidCastException>(() => reader.GetFieldValue<object[]>(0))!;
        Assert.IsInstanceOf<NotSupportedException>(exception.InnerException);
        Assert.AreEqual(errorMessage, exception.InnerException!.Message);
    }

    [Test]
    public async Task NpgsqlSlimSourceBuilder_EnableRecords()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableRecords();
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        // RecordHandler doesn't support writing, so we only check for reading
        cmd.CommandText = "SELECT ('one'::text, 2)";
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(() => reader.GetValue(0), Throws.Nothing);
        Assert.That(() => reader.GetFieldValue<object[]>(0), Throws.Nothing);
    }

    public RecordTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
