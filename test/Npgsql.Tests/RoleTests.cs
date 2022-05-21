using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Tests;

public class RoleTests : MultiplexingTestBase
{
    private const string TestRole = "npgsql_role";

    [Test]
    [TestCase(true, TestName = "Role")]
    [TestCase(false, TestName = "NoRole")]
    public async Task Role(bool useRole)
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Role = useRole ? TestRole : null,
            MaxPoolSize = 1,
        };

        var doTest = async () =>
        {
            using var conn = await OpenConnectionAsync(builder);
            var currentUser = await conn.ExecuteScalarAsync("SELECT current_user");
            var currentUserAsString = currentUser as string;

            Assert.That(currentUserAsString, Is.Not.Null);
            if (useRole)
                Assert.That(currentUser, Is.EqualTo(TestRole));
            else
                Assert.That(currentUser, Is.EqualTo(builder.Username));

            await conn.CloseAsync();
        };

        await doTest();
        await doTest();
    }

    [OneTimeSetUp]
    public async Task Setup()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
        using var conn = await OpenConnectionAsync(builder);
        var result = await conn.ExecuteNonQueryAsync($"CREATE ROLE {TestRole}");
        await conn.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
        using var conn = await OpenConnectionAsync(builder);
        var result = await conn.ExecuteNonQueryAsync($"DROP ROLE {TestRole}");
        await conn.CloseAsync();
    }

    public RoleTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) { }
}
