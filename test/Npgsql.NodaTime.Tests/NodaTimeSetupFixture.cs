using NUnit.Framework;

namespace Npgsql.NodaTime.Tests;

[SetUpFixture]
public class NodaTimeSetupFixture
{
    [OneTimeSetUp]
    public void OneTimeSetUp() => NpgsqlConnection.GlobalTypeMapper.UseNodaTime();

    [OneTimeTearDown]
    public void OneTimeTearDown() => NpgsqlConnection.GlobalTypeMapper.Reset();
}