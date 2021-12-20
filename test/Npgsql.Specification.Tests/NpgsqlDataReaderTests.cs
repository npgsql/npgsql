using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlDataReaderTests : DataReaderTestBase<NpgsqlSelectValueFixture>
{
    public NpgsqlDataReaderTests(NpgsqlSelectValueFixture fixture)
        : base(fixture) {}
}