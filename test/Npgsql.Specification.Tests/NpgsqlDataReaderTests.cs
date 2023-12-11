using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlDataReaderTests(NpgsqlSelectValueFixture fixture) : DataReaderTestBase<NpgsqlSelectValueFixture>(fixture);