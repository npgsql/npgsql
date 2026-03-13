using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AdoNet.Specification.Tests;
using Xunit;

namespace Npgsql.Specification.Tests;

[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped")]
public sealed class NpgsqlConnectionTests(NpgsqlDbFactoryFixture fixture) : ConnectionTestBase<NpgsqlDbFactoryFixture>(fixture)
{
    // Skipped tests mark places where Npgsql currently diverges from AdoNet.Specification.Tests expectations.
    // Some divergences may be by design; others may indicate compatibility gaps worth investigating.

    [Fact(Skip = "NpgsqlConnection does not support the Disposed event")]
    public override void Dispose_raises_Disposed() {}

    [Fact(Skip = "NpgsqlConnection does not support the Disposed event")]
    public override Task DisposeAsync_raises_Disposed() => Task.CompletedTask;

    [Fact(Skip = "NpgsqlConnection.OpenAsync() does not throw OperationCanceledException when a canceled token is passed")]
    public override Task OpenAsync_is_canceled() => Task.CompletedTask;
}
