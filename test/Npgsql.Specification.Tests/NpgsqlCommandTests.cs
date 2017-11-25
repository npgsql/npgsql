using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests
{
    public sealed class NpgsqlCommandTests : CommandTestBase<NpgsqlDbFactoryFixture>
    {
        public NpgsqlCommandTests(NpgsqlDbFactoryFixture fixture)
            : base(fixture)
        {
        }
    }
}
