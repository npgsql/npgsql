using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Tests;

public class RoleTests : MultiplexingTestBase
{
    private const string Role1 = "npgsql_tests_role1";
    private const string Role2 = "npgsql_tests_role2";

    [Test]
    [TestCase(true, TestName = "Role")]
    [TestCase(false, TestName = "NoRole")]
    public async Task Role(bool useRole)
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Role = useRole ? Role1 : null,
            MaxPoolSize = 1,
            MinPoolSize = 1,
        };
        _ = builder.Username ?? throw new NullReferenceException($"Username provided by {nameof(MultiplexingTestBase)} is null.");

        var initialUser = useRole ? Role1 : builder.Username;

        // Initial user is as expected
        using var conn1 = await OpenConnectionAsync(builder);
        await AssertCurrentUserIs(conn1, initialUser);

        // User at the time a connection ends
        //  * doesn't influence next connection in non-multiplexing mode
        //  * does influence next connection in multiplexing mode
        await conn1.ExecuteNonQueryAsync($"SET ROLE {Role2}");
        await AssertCurrentUserIs(conn1, Role2);
        await conn1.CloseAsync();

        using var conn2 = await OpenConnectionAsync(builder);

        if (!IsMultiplexing)
            await AssertCurrentUserIs(conn2, initialUser);
        else
            await AssertCurrentUserIs(conn2, Role2);

        await conn2.CloseAsync();
    }

    public async Task AssertCurrentUserIs(NpgsqlConnection conn, string expectedUser)
    {
        var currentUser = await conn.ExecuteScalarAsync("SELECT current_user");
        var currentUserAsString = currentUser as string;
        Assert.That(currentUserAsString, Is.Not.Null);
        Assert.That(currentUser, Is.EqualTo(expectedUser));
    }

    public RoleTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) { }
}
