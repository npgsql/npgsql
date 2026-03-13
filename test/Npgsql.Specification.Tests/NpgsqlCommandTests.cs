using System.Diagnostics.CodeAnalysis;
using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests;

[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
public sealed class NpgsqlCommandTests(NpgsqlDbFactoryFixture fixture) : CommandTestBase<NpgsqlDbFactoryFixture>(fixture)
{
    // Skipped tests mark places where Npgsql currently diverges from AdoNet.Specification.Tests expectations.
    // Some divergences may be by design; others may indicate compatibility gaps worth investigating.

    [Fact(Skip = "PostgreSQL only supports a single transaction on a given connection at a given time. As a result, Npgsql completely ignores DbCommand.Transaction")]
    public override void ExecuteReader_throws_when_transaction_required() {}

    [Fact(Skip = "PostgreSQL only supports a single transaction on a given connection at a given time. As a result, Npgsql completely ignores DbCommand.Transaction")]
    public override void ExecuteReader_throws_when_transaction_mismatched() {}

    [Fact(Skip = "NpgsqlCommand.ExecuteReader() throws NpgsqlOperationInProgressException instead of InvalidOperationException when another reader is already open")]
    public override void ExecuteReader_throws_when_reader_open() {}

    [Fact(Skip = "NpgsqlCommand.Execute() throws InvalidCastException instead of NotSupportedException for unknown ParameterValue types")]
    public override void Execute_throws_for_unknown_ParameterValue_type() {}
}
