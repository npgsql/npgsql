using System;
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

    [Test]
    public async Task Write_Record_is_not_supported()
    {
        var exception = await AssertTypeUnsupportedWrite<object[], NotSupportedException>(new object[] { 1, "foo" }, "record");
        Assert.That(exception.Message, Is.EqualTo(NpgsqlStrings.WriteRecordNotSupported));

        exception = await AssertTypeUnsupportedWrite<(int, string), NotSupportedException>((1, "foo"), "record");
        Assert.That(exception.Message, Is.EqualTo(NpgsqlStrings.WriteRecordNotSupported));

        exception = await AssertTypeUnsupportedWrite<Tuple<int, string>, NotSupportedException>(Tuple.Create(1, "foo"), "record");
        Assert.That(exception.Message, Is.EqualTo(NpgsqlStrings.WriteRecordNotSupported));
    }

    [Test]
    public async Task Records_supported_only_with_EnableRecords([Values] bool withMappings)
    {
        Func<IResolveConstraint> assertExpr = () => withMappings
            ? Throws.Nothing
            : Throws.Exception
                .TypeOf<NotSupportedException>()
                .With.Property("Message")
                .EqualTo(string.Format(NpgsqlStrings.RecordsNotEnabled, "EnableRecords", "NpgsqlSlimDataSourceBuilder"));

        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        if (withMappings)
            dataSourceBuilder.EnableRecords();
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        // RecordHandler doesn't support writing, so we only check for reading
        cmd.CommandText = "SELECT ('one'::text, 2)";
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(() => reader.GetValue(0), assertExpr());
        Assert.That(() => reader.GetFieldValue<object[]>(0), assertExpr());
    }

    public RecordTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
