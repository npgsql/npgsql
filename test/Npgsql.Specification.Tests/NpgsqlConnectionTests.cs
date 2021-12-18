using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlConnectionTests : ConnectionTestBase<NpgsqlDbFactoryFixture>
{
    public NpgsqlConnectionTests(NpgsqlDbFactoryFixture fixture)
        : base(fixture)
    {
    }
}