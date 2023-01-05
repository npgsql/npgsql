using NUnit.Framework;
using System.Data;
using System.Threading.Tasks;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class AsyncTests : TestBase
{
    [Test]
    public async Task NonQuery()
    {
        await using var conn = await OpenConnectionAsync();
        var tableName = await CreateTempTable(conn, "int INTEGER");
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"INSERT INTO {tableName} (int) VALUES (4)";
        await cmd.ExecuteNonQueryAsync();
        Assert.That(await conn.ExecuteScalarAsync($"SELECT int FROM {tableName}"), Is.EqualTo(4));
    }

    [Test]
    public async Task Scalar()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Reader()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.That(reader[0], Is.EqualTo(1));
    }

    [Test]
    public async Task Columnar()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT NULL, 2, 'Some Text'", conn);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        await reader.ReadAsync();
        Assert.That(await reader.IsDBNullAsync(0), Is.True);
        Assert.That(await reader.GetFieldValueAsync<string>(2), Is.EqualTo("Some Text"));
    }
}
