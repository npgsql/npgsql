using Npgsql.Tests;
using NUnit.Framework;

[SetUpFixture]
public class AssemblySetUp
{
    [OneTimeSetUp]
    public void Setup()
        => TestUtil.EnsureTestDatabase(GetType().Assembly);
}
