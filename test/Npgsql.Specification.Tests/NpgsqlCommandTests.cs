using AdoNet.Specification.Tests;
using Xunit;
using Xunit.Sdk;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlCommandTests(NpgsqlDbFactoryFixture fixture) : CommandTestBase<NpgsqlDbFactoryFixture>(fixture)
{
    public override void ExecuteReader_throws_when_transaction_required()
    {
        // PostgreSQL only supports a single transaction on a given connection at a given time. As a result,
        // Npgsql completely ignores DbCommand.Transaction.
        var ex = Assert.Throws<ThrowsException>(() => base.ExecuteReader_throws_when_transaction_required());
        Assert.Contains("No exception was thrown", ex.Message);
    }

    public override void ExecuteReader_throws_when_transaction_mismatched()
    {
        // PostgreSQL only supports a single transaction on a given connection at a given time. As a result,
        // Npgsql completely ignores DbCommand.Transaction.
        var ex = Assert.Throws<ThrowsException>(() => base.ExecuteReader_throws_when_transaction_mismatched());
        Assert.Contains("No exception was thrown", ex.Message);
    }

    // Skipped tests mark places where Npgsql currently diverges from AdoNet.Specification.Tests expectations.
    // Some divergences may be by design; others may indicate compatibility gaps worth investigating.

    [Fact(Skip = "NpgsqlCommand.ExecuteReader() throws NpgsqlOperationInProgressException instead of InvalidOperationException when another reader is already open")]
    public override void ExecuteReader_throws_when_reader_open() {}

    [Fact(Skip = "NpgsqlCommand.Execute() throws InvalidCastException instead of NotSupportedException for unknown ParameterValue types")]
    public override void Execute_throws_for_unknown_ParameterValue_type() {}
}
