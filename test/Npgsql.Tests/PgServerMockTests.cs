using System;
using System.Threading.Tasks;
using Npgsql.Tests.Support;
using NUnit.Framework;

namespace Npgsql.Tests;

/// <summary>
/// Tests how <see cref="PgServerMock"/> frames the messages it reads and writes.
/// </summary>
public class PgServerMockTests : TestBase
{
    static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Test, Description("The mock skips a frontend message whose body is larger than its read buffer, and stays aligned")]
    public async Task Frontend_message_larger_than_the_read_buffer()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmaster.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        var server = await postmaster.WaitForServerConnection();

        // The query text makes the Parse message larger than the mock's read buffer, so the mock never has all of it at once
        var queryTask = conn.ExecuteScalarAsync($"SELECT '{new string('x', 20000)}'");
        try
        {
            await server.ExpectExtendedQuery().WaitAsync(Timeout);
            await server.WriteScalarResponseAndFlush(1);
            Assert.That(await queryTask.WaitAsync(Timeout), Is.EqualTo(1));

            queryTask = conn.ExecuteScalarAsync("SELECT 2");
            await server.ExpectExtendedQuery().WaitAsync(Timeout);
            await server.WriteScalarResponseAndFlush(2);
            Assert.That(await queryTask.WaitAsync(Timeout), Is.EqualTo(2));
        }
        finally
        {
            // A query that still waits for the mock would keep the connection from closing
            if (!queryTask.IsCompleted)
                server.Close();
        }
    }

    [Test, Description("The fields of error and notice messages are UTF-8, as their declared lengths assume")]
    public async Task Notice_with_non_ASCII_text()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmaster.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        var server = await postmaster.WaitForServerConnection();
        PostgresNotice? notice = null;
        conn.Notice += (_, args) => notice = args.Notice;

        var queryTask = conn.ExecuteScalarAsync("SELECT 1");
        await server.ExpectExtendedQuery().WaitAsync(Timeout);
        await server.WriteNoticeResponse(PostgresErrorCodes.Warning, message: "Grüße").WriteScalarResponseAndFlush(1);

        Assert.That(await queryTask.WaitAsync(Timeout), Is.EqualTo(1));
        Assert.That(notice?.MessageText, Is.EqualTo("Grüße"));
    }
}
