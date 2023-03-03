using NUnit.Framework;

namespace Npgsql.NodaTime.Tests;

// Note that we register NodaTime globally, rather than using the more standard data source mapping.
// We can do this since NUnit runs each test assembly in a different process, so we get isolation and don't interfere with other,
// non-NodaTime tests. This also allows us to test global type inference, which only works with global mappings.
[SetUpFixture]
public class NodaTimeSetupFixture
{
#pragma warning disable CS0618 // GlobalTypeMapper is obsolete
    [OneTimeSetUp]
    public void OneTimeSetUp() => NpgsqlConnection.GlobalTypeMapper.UseNodaTime();

    [OneTimeTearDown]
    public void OneTimeTearDown() => NpgsqlConnection.GlobalTypeMapper.Reset();
#pragma warning restore CS0618 // GlobalTypeMapper is obsolete
}
