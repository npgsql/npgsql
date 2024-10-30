using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests;

public sealed class NpgsqlConnectionTests(NpgsqlDbFactoryFixture fixture) : ConnectionTestBase<NpgsqlDbFactoryFixture>(fixture);