using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests
{
    public sealed class NpgsqlCommandTests : CommandTestBase<NpgsqlDbFactoryFixture>
    {
        public NpgsqlCommandTests(NpgsqlDbFactoryFixture fixture)
            : base(fixture) {}

        // TODO: investigate
        [Fact(Skip = "TODO: investigate")]
        public override void Connection_throws_when_set_when_open_reader() {}

        // PostgreSQL only supports a single transaction on a given connection at a given time. As a result,
        // Npgsql completely ignores DbCommand.Transaction.
        [Fact]
        public override void ExecuteReader_throws_when_transaction_required() {}

        [Fact]
        public override void ExecuteReader_throws_when_transaction_mismatched() {}
    }
}
