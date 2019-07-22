using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests
{
    public sealed class NpgsqlConnectionTests : ConnectionTestBase<NpgsqlDbFactoryFixture>
    {
        public NpgsqlConnectionTests(NpgsqlDbFactoryFixture fixture)
            : base(fixture) {}

        // TODO: investigate
        [Fact(Skip = "TODO: investigate")]
        public override void Commit_transaction_clears_Connection() {}

        // TODO: investigate
        [Fact(Skip = "TODO: investigate")]
        public override void Rollback_transaction_clears_Connection() {}
    }
}
