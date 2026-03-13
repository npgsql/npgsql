using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests;

[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
public sealed class NpgsqlDataReaderTests(NpgsqlSelectValueFixture fixture) : DataReaderTestBase<NpgsqlSelectValueFixture>(fixture)
{
    // Skipped tests mark places where Npgsql currently diverges from AdoNet.Specification.Tests expectations.
    // Some divergences may be by design; others may indicate compatibility gaps worth investigating.

    [Fact(Skip = "NpgsqlDataReader.FieldCount throws ObjectDisposedException instead of InvalidOperationException")]
    public override void FieldCount_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.GetBytes() throws ArgumentOutOfRangeException instead of returning 0 when dataOffset is too large")]
    public override void GetBytes_reads_nothing_when_dataOffset_is_too_large() {}

    [Fact(Skip = "NpgsqlDataReader.GetChars() throws EndOfStreamException instead of returning 0 when dataOffset is too large")]
    public override void GetChars_reads_nothing_when_dataOffset_is_too_large() {}

    [Fact(Skip = "NpgsqlDataReader.GetDataTypeName() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void GetDataTypeName_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.GetFieldType() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void GetFieldType_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.GetFieldValueAsync() does not throw an OperationCanceledException when a canceled token is passed")]
    public override Task GetFieldValueAsync_is_canceled() => Task.CompletedTask;

    [Fact(Skip = "NpgsqlDataReader.GetName() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void GetName_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.GetTextReader() throws InvalidCastException when command text is null")]
    public override void GetTextReader_returns_empty_for_null_String() {}

    [Fact(Skip = "NpgsqlDataReader.GetValue() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void GetValue_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.IsDBNull() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void IsDBNull_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.IsDBNullAsync() does not throw OperationCanceledException when a canceled token is passed")]
    public override Task IsDBNullAsync_is_canceled() => Task.CompletedTask;

    [Fact(Skip = "NpgsqlDataReader.NextResult() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void NextResult_throws_when_closed() {}

    [Fact(Skip = "NpgsqlDataReader.Read() throws ObjectDisposedException instead of InvalidOperationException when reader is disposed")]
    public override void Read_throws_when_closed() {}
}
