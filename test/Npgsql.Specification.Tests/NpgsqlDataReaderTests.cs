using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlDataReaderOrigTests : DataReaderTestBase<NpgsqlSelectValueFixture>
{
    public NpgsqlDataReaderOrigTests(NpgsqlSelectValueFixture fixture)
        : base(fixture) {}
}