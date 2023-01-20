using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlCommandOrigTests : CommandTestBase<NpgsqlDbFactoryFixture>
{
    public NpgsqlCommandOrigTests(NpgsqlDbFactoryFixture fixture)
        : base(fixture)
    {
    }

    // PostgreSQL only supports a single transaction on a given connection at a given time. As a result,
    // Npgsql completely ignores DbCommand.Transaction.
    public override void ExecuteReader_throws_when_transaction_required() {}
    public override void ExecuteReader_throws_when_transaction_mismatched() {}
}