using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Buffers.Binary;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class CommandTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    static uint Int4Oid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Int4).Value;
    static uint TextOid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Text).Value;

    #region Legacy batching

    [Test]
    [TestCase(new[] { true }, TestName = "SingleQuery")]
    [TestCase(new[] { false }, TestName = "SingleNonQuery")]
    [TestCase(new[] { true, true }, TestName = "TwoQueries")]
    [TestCase(new[] { false, false }, TestName = "TwoNonQueries")]
    [TestCase(new[] { false, true }, TestName = "NonQueryQuery")]
    [TestCase(new[] { true, false }, TestName = "QueryNonQuery")]
    public async Task Multiple_statements(bool[] queries)
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        var sb = new StringBuilder();
        foreach (var query in queries)
            sb.Append(query ? "SELECT 1;" : $"UPDATE {table} SET name='yo' WHERE 1=0;");
        var sql = sb.ToString();
        foreach (var prepare in new[] { false, true })
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (prepare && !IsMultiplexing)
                await cmd.PrepareAsync();
            await using var reader = await cmd.ExecuteReaderAsync();
            var numResultSets = queries.Count(q => q);
            for (var i = 0; i < numResultSets; i++)
            {
                Assert.That(await reader.ReadAsync(), Is.True);
                Assert.That(reader[0], Is.EqualTo(1));
                Assert.That(await reader.NextResultAsync(), Is.EqualTo(i != numResultSets - 1));
            }
        }
    }

    [Test]
    public async Task Multiple_statements_with_parameters([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT @p1; SELECT @p2";
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
        var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Text);
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        p1.Value = 8;
        p2.Value = "foo";
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(8));
        Assert.That(await reader.NextResultAsync(), Is.True);
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader.GetString(0), Is.EqualTo("foo"));
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test]
    public async Task SingleRow_legacy_batching([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        Assert.That(reader.Read(), Is.False);
        Assert.That(reader.NextResult(), Is.False);
    }

    [Test, Description("Makes sure a later command can depend on an earlier one")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/641")]
    public async Task Multiple_statements_with_dependencies()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "a INT");

        await conn.ExecuteNonQueryAsync($"ALTER TABLE {table} ADD COLUMN b INT; INSERT INTO {table} (b) VALUES (8)");
        Assert.That(await conn.ExecuteScalarAsync($"SELECT b FROM {table}"), Is.EqualTo(8));
    }

    [Test, Description("Forces async write mode when the first statement in a multi-statement command is big")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/641")]
    public async Task Multiple_statements_large_first_command()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand($"SELECT repeat('X', {conn.Settings.WriteBufferSize}); SELECT @p", conn);
        var expected1 = new string('X', conn.Settings.WriteBufferSize);
        var expected2 = new string('Y', conn.Settings.WriteBufferSize);
        cmd.Parameters.AddWithValue("p", expected2);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetString(0), Is.EqualTo(expected1));
        reader.NextResult();
        reader.Read();
        Assert.That(reader.GetString(0), Is.EqualTo(expected2));
    }

    [Test]
    [NonParallelizable] // Disables sql rewriting
    public async Task Legacy_batching_is_not_supported_when_EnableSqlParsing_is_disabled()
    {
        using var _ = DisableSqlRewriting();

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn);
        Assert.That(async () => await cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.SyntaxError));
    }

    [Test]
    [NonParallelizable] // Disables sql rewriting
    public async Task Positional_parameters_are_supported_when_EnableSqlParsing_is_disabled()
    {
        using var _ = DisableSqlRewriting();

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(8));
    }

    [Test]
    [NonParallelizable] // Disables sql rewriting
    public async Task Named_parameters_are_not_supported_when_EnableSqlParsing_is_disabled()
    {
        using var _ = DisableSqlRewriting();

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter("p", 8));
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    #endregion

    #region Timeout

    [Test, Description("Checks that CommandTimeout gets enforced as a socket timeout")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/327")]
    public async Task Timeout()
    {
        if (IsMultiplexing)
            return; // Multiplexing, Timeout

        await using var dataSource = CreateDataSource(csb => csb.CommandTimeout = 1);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = CreateSleepCommand(conn, 10);
        Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
            .TypeOf<NpgsqlException>()
            .With.InnerException.TypeOf<TimeoutException>()
        );
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
    }

    [Test, Description("Times out an async operation, testing that cancellation occurs successfully")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/607")]
    public async Task Timeout_async_soft()
    {
        if (IsMultiplexing)
            return; // Multiplexing, Timeout

        await using var dataSource = CreateDataSource(csb => csb.CommandTimeout = 1);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = CreateSleepCommand(conn, 10);
        Assert.That(async () => await cmd.ExecuteNonQueryAsync(),
            Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<TimeoutException>());
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
    }

    [Test, Description("Times out an async operation, with unsuccessful cancellation (socket break)")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/607")]
    public async Task Timeout_async_hard()
    {
        if (IsMultiplexing)
            return; // Multiplexing, Timeout

        var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { CommandTimeout = 1 };
        await using var postmasterMock = PgPostmasterMock.Start(builder.ConnectionString);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        Assert.That(async () => await conn.ExecuteScalarAsync("SELECT 1"),
            Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<TimeoutException>());

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
        Assert.That((await postmasterMock.WaitForCancellationRequest()).ProcessId,
            Is.EqualTo(processId));
    }

    [Test]
    public async Task Timeout_from_connection_string()
    {
        Assert.That(NpgsqlConnector.MinimumInternalCommandTimeout, Is.Not.EqualTo(NpgsqlCommand.DefaultTimeout));
        var timeout = NpgsqlConnector.MinimumInternalCommandTimeout;
        await using var dataSource = CreateDataSource(csb => csb.CommandTimeout = timeout);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var command = new NpgsqlCommand("SELECT 1", conn);
        Assert.That(command.CommandTimeout, Is.EqualTo(timeout));
        command.CommandTimeout = 10;
        await command.ExecuteScalarAsync();
        Assert.That(command.CommandTimeout, Is.EqualTo(10));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
    public async Task Timeout_switch_connection()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
        if (csb.CommandTimeout is >= 100 and < 105)
            IgnoreExceptOnBuildServer("Bad default command timeout");

        await using var dataSource1 = CreateDataSource(ConnectionString + ";CommandTimeout=100");
        await using var c1 = dataSource1.CreateConnection();
        await using var cmd = c1.CreateCommand();
        Assert.That(cmd.CommandTimeout, Is.EqualTo(100));
        await using var dataSource2 = CreateDataSource(ConnectionString + ";CommandTimeout=101");
        await using (var c2 = dataSource2.CreateConnection())
        {
            cmd.Connection = c2;
            Assert.That(cmd.CommandTimeout, Is.EqualTo(101));
        }
        cmd.CommandTimeout = 102;
        await using (var c2 = dataSource2.CreateConnection())
        {
            cmd.Connection = c2;
            Assert.That(cmd.CommandTimeout, Is.EqualTo(102));
        }
    }

    [Test]
    public async Task Prepare_timeout_hard([Values] SyncOrAsync async)
    {
        if (IsMultiplexing)
            return; // Multiplexing, Timeout

        var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { CommandTimeout = 1 };
        await using var postmasterMock = PgPostmasterMock.Start(builder.ConnectionString);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        var cmd = new NpgsqlCommand("SELECT 1", conn);
        Assert.That(async () =>
            {
                if (async == SyncOrAsync.Sync)
                    cmd.Prepare();
                else
                    await cmd.PrepareAsync();
            },
            Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<TimeoutException>());

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
        Assert.That((await postmasterMock.WaitForCancellationRequest()).ProcessId,
            Is.EqualTo(processId));
    }

    #endregion

    #region Cancel

    [Test, Description("Basic cancellation scenario")]
    public async Task Cancel()
    {
        if (IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        await using var cmd = CreateSleepCommand(conn, 5);

        var queryTask = Task.Run(() => cmd.ExecuteNonQuery());
        // We have to be sure the command's state is InProgress, otherwise the cancellation request will never be sent
        cmd.WaitUntilCommandIsInProgress();
        cmd.Cancel();
        Assert.That(async () => await queryTask, Throws
            .TypeOf<OperationCanceledException>()
            .With.InnerException.TypeOf<PostgresException>()
            .With.InnerException.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.QueryCanceled)
        );
    }

    [Test]
    public async Task Cancel_async_immediately()
    {
        if (IsMultiplexing)
            return; // Multiplexing, cancellation

        await using var conn = await OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1";

        var t = cmd.ExecuteScalarAsync(new(canceled: true));
        Assert.That(t.IsCompleted, Is.True); // checks, if a query has completed synchronously
        Assert.That(t.Status, Is.EqualTo(TaskStatus.Canceled));
        Assert.ThrowsAsync<OperationCanceledException>(async () => await t);

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test, Description("Cancels an async query with the cancellation token, with successful PG cancellation")]
    public async Task Cancel_async_soft()
    {
        if (IsMultiplexing)
            return; // Multiplexing, cancellation

        await using var conn = await OpenConnectionAsync();
        await using var cmd = CreateSleepCommand(conn);
        using var cancellationSource = new CancellationTokenSource();
        var t = cmd.ExecuteNonQueryAsync(cancellationSource.Token);
        cancellationSource.Cancel();

        var exception = Assert.ThrowsAsync<OperationCanceledException>(async () => await t)!;
        Assert.That(exception.InnerException,
            Is.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.QueryCanceled));
        Assert.That(exception.CancellationToken, Is.EqualTo(cancellationSource.Token));

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
        Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test, Description("Cancels an async query with the cancellation token and prepended query, with successful PG cancellation")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/5191")]
    public async Task Cancel_async_soft_with_prepended_query()
    {
        if (IsMultiplexing)
            return; // Multiplexing, cancellation

        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        var server = await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        await using var tx = await conn.BeginTransactionAsync();
        await using var cmd = CreateSleepCommand(conn);
        using var cancellationSource = new CancellationTokenSource();
        var t = cmd.ExecuteNonQueryAsync(cancellationSource.Token);

        await server.ExpectSimpleQuery("BEGIN TRANSACTION ISOLATION LEVEL READ COMMITTED");
        cancellationSource.Cancel();
        await server
            .WriteCommandComplete()
            .WriteReadyForQuery(TransactionStatus.InTransactionBlock)
            .FlushAsync();

         Assert.That((await postmasterMock.WaitForCancellationRequest()).ProcessId,
            Is.EqualTo(processId));

         await server
             .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
             .WriteReadyForQuery()
             .FlushAsync();

        var exception = Assert.ThrowsAsync<OperationCanceledException>(async () => await t)!;
        Assert.That(exception.InnerException,
            Is.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.QueryCanceled));
        Assert.That(exception.CancellationToken, Is.EqualTo(cancellationSource.Token));

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
    }

    [Test, Description("Cancels an async query with the cancellation token, with unsuccessful PG cancellation (socket break)")]
    public async Task Cancel_async_hard()
    {
        if (IsMultiplexing)
            return; // Multiplexing, cancellation

        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        using var cancellationSource = new CancellationTokenSource();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        var t = cmd.ExecuteScalarAsync(cancellationSource.Token);
        cancellationSource.Cancel();

        var exception = Assert.ThrowsAsync<OperationCanceledException>(async () => await t)!;
        Assert.That(exception.InnerException, Is.TypeOf<TimeoutException>());
        Assert.That(exception.CancellationToken, Is.EqualTo(cancellationSource.Token));

        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
        Assert.That((await postmasterMock.WaitForCancellationRequest()).ProcessId,
            Is.EqualTo(processId));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3466")]
    [Ignore("https://github.com/npgsql/npgsql/issues/4668")]
    public async Task Bug3466([Values(false, true)] bool isBroken)
    {
        if (IsMultiplexing)
            return; // Multiplexing, cancellation

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = false
        };
        await using var postmasterMock = PgPostmasterMock.Start(csb.ToString(), completeCancellationImmediately: false);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        var serverMock = await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        using var cancellationSource = new CancellationTokenSource();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn)
        {
            CommandTimeout = 4
        };
        var t = Task.Run(() => cmd.ExecuteScalar());
        // We have to be sure the command's state is InProgress, otherwise the cancellation request will never be sent
        cmd.WaitUntilCommandIsInProgress();
        // Perform cancellation, which will block on the server side
        var cancelTask = Task.Run(() => cmd.Cancel());
        // Note what we have to wait for the cancellation request, otherwise the connection might be closed concurrently
        // and the cancellation request is never send
        var cancellationRequest = await postmasterMock.WaitForCancellationRequest();

        if (isBroken)
        {
            Assert.ThrowsAsync<OperationCanceledException>(async () => await t);
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Broken));
        }
        else
        {
            await serverMock
                .WriteParseComplete()
                .WriteBindComplete()
                .WriteRowDescription(new FieldDescription(Int4Oid))
                .WriteDataRow(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(1)))
                .WriteCommandComplete()
                .WriteReadyForQuery()
                .FlushAsync();
            Assert.DoesNotThrowAsync(async () => await t);
            Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
            await conn.CloseAsync();
        }

        // Release the cancellation at the server side, and make sure it completes without an exception
        cancellationRequest.Complete();
        Assert.DoesNotThrowAsync(async () => await cancelTask);
    }

    [Test, Description("Check that cancel only affects the command on which its was invoked")]
    [Explicit("Timing-sensitive")]
    public async Task Cancel_cross_command()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd1 = CreateSleepCommand(conn, 2);
        await using var cmd2 = new NpgsqlCommand("SELECT 1", conn);
        var cancelTask = Task.Factory.StartNew(() =>
        {
            Thread.Sleep(300);
            cmd2.Cancel();
        });
        Assert.That(() => cmd1.ExecuteNonQueryAsync(), Throws.Nothing);
        cancelTask.Wait();
    }

    #endregion

    #region Cursors

    [Test]
    public async Task Cursor_statement()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        using var t = conn.BeginTransaction();

        for (var x = 0; x < 5; x++)
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')");

        var i = 0;
        var command = new NpgsqlCommand($"DECLARE TE CURSOR FOR SELECT * FROM {table}", conn);
        command.ExecuteNonQuery();
        command.CommandText = "FETCH FORWARD 3 IN TE";
        var dr = command.ExecuteReader();

        while (dr.Read())
            i++;
        Assert.AreEqual(3, i);
        dr.Close();

        i = 0;
        command.CommandText = "FETCH BACKWARD 1 IN TE";
        var dr2 = command.ExecuteReader();
        while (dr2.Read())
            i++;
        Assert.AreEqual(1, i);
        dr2.Close();

        command.CommandText = "close te;";
        command.ExecuteNonQuery();
    }

    [Test]
    public async Task Cursor_move_RecordsAffected()
    {
        using var connection = await OpenConnectionAsync();
        using var transaction = connection.BeginTransaction();
        var command = new NpgsqlCommand("DECLARE curs CURSOR FOR SELECT * FROM (VALUES (1), (2), (3)) as t", connection);
        command.ExecuteNonQuery();
        command.CommandText = "MOVE FORWARD ALL IN curs";
        var count = command.ExecuteNonQuery();
        Assert.AreEqual(3, count);
    }

    #endregion

    #region CommandBehavior.CloseConnection

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/693")]
    public async Task CloseConnection()
    {
        using var conn = await OpenConnectionAsync();
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
            while (reader.Read()) {}
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1194")]
    public async Task CloseConnection_with_open_reader_with_CloseConnection()
    {
        using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommand("SELECT 1", conn);
        var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        var wasClosed = false;
        reader.ReaderClosed += (sender, args) => { wasClosed = true; };
        conn.Close();
        Assert.That(wasClosed, Is.True);
    }

    [Test]
    public async Task CloseConnection_with_exception()
    {
        using var conn = await OpenConnectionAsync();
        using (var cmd = new NpgsqlCommand("SE", conn))
            Assert.That(() => cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection), Throws.Exception.TypeOf<PostgresException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    #endregion

    [Test]
    public async Task SingleRow([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1, 2 UNION SELECT 3, 4", conn);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();

        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        Assert.That(() => reader.GetInt32(0), Throws.Exception.TypeOf<InvalidOperationException>());
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        Assert.That(reader.Read(), Is.False);
    }

    [Test]
    public async Task CommandText_not_set()
    {
        await using var conn = await OpenConnectionAsync();
        await using (var cmd = new NpgsqlCommand())
        {
            cmd.Connection = conn;
            Assert.That(cmd.ExecuteNonQueryAsync, Throws.Exception.TypeOf<InvalidOperationException>());
            cmd.CommandText = null;
            Assert.That(cmd.ExecuteNonQueryAsync, Throws.Exception.TypeOf<InvalidOperationException>());
            cmd.CommandText = "";
        }

        await using (var cmd = conn.CreateCommand())
            Assert.That(cmd.ExecuteNonQueryAsync, Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task ExecuteScalar()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        await using var command = new NpgsqlCommand($"SELECT name FROM {table}", conn);
        Assert.That(command.ExecuteScalarAsync, Is.Null);

        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES (NULL)");
        Assert.That(command.ExecuteScalarAsync, Is.EqualTo(DBNull.Value));

        await conn.ExecuteNonQueryAsync($"TRUNCATE {table}");
        for (var i = 0; i < 2; i++)
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')");
        Assert.That(command.ExecuteScalarAsync, Is.EqualTo("X"));
    }

    [Test]
    public async Task ExecuteNonQuery()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand { Connection = conn };
        var table = await CreateTempTable(conn, "name TEXT");

        cmd.CommandText = $"INSERT INTO {table} (name) VALUES ('John')";
        Assert.That(cmd.ExecuteNonQueryAsync, Is.EqualTo(1));

        cmd.CommandText = $"INSERT INTO {table} (name) VALUES ('John'); INSERT INTO {table} (name) VALUES ('John')";
        Assert.That(cmd.ExecuteNonQueryAsync, Is.EqualTo(2));

        cmd.CommandText = $"INSERT INTO {table} (name) VALUES ('{new string('x', conn.Settings.WriteBufferSize)}')";
        Assert.That(cmd.ExecuteNonQueryAsync, Is.EqualTo(1));
    }

    [Test, Description("Makes sure a command is unusable after it is disposed")]
    public async Task Dispose()
    {
        await using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Dispose();
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(() => cmd.PrepareAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    [Test, Description("Disposing a command with an open reader does not close the reader. This is the SqlClient behavior.")]
    public async Task Command_Dispose_does_not_close_reader()
    {
        await using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommand("SELECT 1, 2", conn);
        await cmd.ExecuteReaderAsync();
        cmd.Dispose();
        cmd = new NpgsqlCommand("SELECT 3", conn);
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
    }

    [Test]
    public async Task Non_standards_conforming_strings()
    {
        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        if (IsMultiplexing)
        {
            Assert.That(async () => await conn.ExecuteNonQueryAsync("set standard_conforming_strings=off"),
                Throws.Exception.TypeOf<NotSupportedException>());
        }
        else
        {
            await conn.ExecuteNonQueryAsync("set standard_conforming_strings=off");
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
            await conn.ExecuteNonQueryAsync("set standard_conforming_strings=on");
        }
    }

    [Test]
    public async Task Parameter_and_operator_unclear()
    {
        await using var conn = await OpenConnectionAsync();
        //Without parenthesis the meaning of [, . and potentially other characters is
        //a syntax error. See comment in NpgsqlCommand.GetClearCommandText() on "usually-redundant parenthesis".
        await using var command = new NpgsqlCommand("select :arr[2]", conn);
        command.Parameters.AddWithValue(":arr", new int[] {5, 4, 3, 2, 1});
        await using var rdr = await command.ExecuteReaderAsync();
        rdr.Read();
        Assert.AreEqual(rdr.GetInt32(0), 4);
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4171")]
    public async Task Cached_command_clears_parameters_placeholder_type()
    {
        await using var conn = await OpenConnectionAsync();

        await using (var cmd1 = conn.CreateCommand())
        {
            cmd1.CommandText = "SELECT @p1";
            cmd1.Parameters.AddWithValue("@p1", 8);
            await using var reader1 = await cmd1.ExecuteReaderAsync();
            reader1.Read();
            Assert.That(reader1[0], Is.EqualTo(8));
        }

        await using (var cmd2 = conn.CreateCommand())
        {
            cmd2.CommandText = "SELECT $1";
            cmd2.Parameters.AddWithValue(8);
            await using var reader2 = await cmd2.ExecuteReaderAsync();
            reader2.Read();
            Assert.That(reader2[0], Is.EqualTo(8));
        }
    }

    [Test]
    [TestCase(CommandBehavior.Default)]
    [TestCase(CommandBehavior.SequentialAccess)]
    public async Task Statement_mapped_output_parameters(CommandBehavior behavior)
    {
        await using var conn = await OpenConnectionAsync();
        var command = new NpgsqlCommand("select 3, 4 as param1, 5 as param2, 6;", conn);

        var p = new NpgsqlParameter("param2", NpgsqlDbType.Integer);
        p.Direction = ParameterDirection.Output;
        p.Value = -1;
        command.Parameters.Add(p);

        p = new NpgsqlParameter("param1", NpgsqlDbType.Integer);
        p.Direction = ParameterDirection.Output;
        p.Value = -1;
        command.Parameters.Add(p);

        p = new NpgsqlParameter("p", NpgsqlDbType.Integer);
        p.Direction = ParameterDirection.Output;
        p.Value = -1;
        command.Parameters.Add(p);

        await using var reader = await command.ExecuteReaderAsync(behavior);

        Assert.AreEqual(4, command.Parameters["param1"].Value);
        Assert.AreEqual(5, command.Parameters["param2"].Value);

        reader.Read();

        Assert.AreEqual(3, reader.GetInt32(0));
        Assert.AreEqual(4, reader.GetInt32(1));
        Assert.AreEqual(5, reader.GetInt32(2));
        Assert.AreEqual(6, reader.GetInt32(3));
    }

    [Test]
    public async Task Bug1006158_output_parameters()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Stored procedure OUT parameters are only support starting with version 14");
        var sproc = await GetTempProcedureName(conn);

        var createFunction = $@"
CREATE PROCEDURE {sproc}(OUT a integer, OUT b boolean) AS $$
BEGIN
    a := 3;
    b := true;
END
$$ LANGUAGE plpgsql;";

        var command = new NpgsqlCommand(createFunction, conn);
        await command.ExecuteNonQueryAsync();

        command = new NpgsqlCommand(sproc, conn);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
        command.Parameters[0].Direction = ParameterDirection.Output;
        command.Parameters.Add(new NpgsqlParameter("b", DbType.Boolean));
        command.Parameters[1].Direction = ParameterDirection.Output;

        _ = await command.ExecuteScalarAsync();

        Assert.AreEqual(3, command.Parameters[0].Value);
        Assert.AreEqual(true, command.Parameters[1].Value);
    }

    [Test]
    public async Task Bug1010788_UpdateRowSource()
    {
        if (IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "id SERIAL PRIMARY KEY, name TEXT");

        var command = new NpgsqlCommand($"SELECT * FROM {table}", conn);
        Assert.AreEqual(UpdateRowSource.Both, command.UpdatedRowSource);

        var cmdBuilder = new NpgsqlCommandBuilder();
        var da = new NpgsqlDataAdapter(command);
        cmdBuilder.DataAdapter = da;
        Assert.IsNotNull(da.SelectCommand);
        Assert.IsNotNull(cmdBuilder.DataAdapter);

        var updateCommand = cmdBuilder.GetUpdateCommand();
        Assert.AreEqual(UpdateRowSource.None, updateCommand.UpdatedRowSource);
    }

    [Test]
    public async Task TableDirect()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('foo')");
        using var cmd = new NpgsqlCommand(table, conn) { CommandType = CommandType.TableDirect };
        using var rdr = await cmd.ExecuteReaderAsync();
        Assert.That(rdr.Read(), Is.True);
        Assert.That(rdr["name"], Is.EqualTo("foo"));
    }


    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/503")]
    public async Task Invalid_UTF8()
    {
        const string badString = "SELECT 'abc\uD801\uD802d'";
        await using var dataSource = CreateDataSource();
        using var conn = await dataSource.OpenConnectionAsync();
        Assert.That(() => conn.ExecuteScalarAsync(badString), Throws.Exception.TypeOf<EncoderFallbackException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
    public async Task Use_across_connection_change([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn1 = await OpenConnectionAsync();
        using var conn2 = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT 1", conn1);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        cmd.Connection = conn2;
        Assert.That(cmd.IsPrepared, Is.False);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    // The asserts we're testing are debug only.
    [Test]
    public async Task Use_after_reload_types_invalidates_cached_infos()
    {
        if (IsMultiplexing)
            return;

        using var conn1 = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT 1", conn1);
        cmd.Prepare();
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            await reader.ReadAsync();
            Assert.DoesNotThrow(() => reader.GetInt32(0));
        }

        await conn1.ReloadTypesAsync();

        using (var reader = await cmd.ExecuteReaderAsync())
        {
            await reader.ReadAsync();
            Assert.DoesNotThrow(() => reader.GetInt32(0));
        }
    }

    [Test]
    public async Task Parameter_overflow_message_length_throws()
    {
        // Create a separate dataSource because of Multiplexing (otherwise we can break unrelated queries)
        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @a, @b, @c, @d, @e, @f, @g, @h", conn);

        var largeParam = new string('A', 1 << 29);
        cmd.Parameters.AddWithValue("a", largeParam);
        cmd.Parameters.AddWithValue("b", largeParam);
        cmd.Parameters.AddWithValue("c", largeParam);
        cmd.Parameters.AddWithValue("d", largeParam);
        cmd.Parameters.AddWithValue("e", largeParam);
        cmd.Parameters.AddWithValue("f", largeParam);
        cmd.Parameters.AddWithValue("g", largeParam);
        cmd.Parameters.AddWithValue("h", largeParam);

        Assert.ThrowsAsync<OverflowException>(() => cmd.ExecuteReaderAsync());
    }

    [Test]
    public async Task Composite_overflow_message_length_throws()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync(
            $"CREATE TYPE {type} AS (a text, b text, c text, d text, e text, f text, g text, h text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<BigComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var largeString = new string('A', 1 << 29);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT @a";
        cmd.Parameters.AddWithValue("a", new BigComposite
        {
            A = largeString,
            B = largeString,
            C = largeString,
            D = largeString,
            E = largeString,
            F = largeString,
            G = largeString,
            H = largeString
        });

        Assert.ThrowsAsync<OverflowException>(async () => await cmd.ExecuteNonQueryAsync());
    }

    record BigComposite
    {
        public string A { get; set; } = null!;
        public string B { get; set; } = null!;
        public string C { get; set; } = null!;
        public string D { get; set; } = null!;
        public string E { get; set; } = null!;
        public string F { get; set; } = null!;
        public string G { get; set; } = null!;
        public string H { get; set; } = null!;
    }

    [Test]
    public async Task Array_overflow_message_length_throws()
    {
        // Create a separate dataSource because of Multiplexing (otherwise we can break unrelated queries)
        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        var largeString = new string('A', 1 << 29);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT @a";
        var array = new[]
        {
            largeString,
            largeString,
            largeString,
            largeString,
            largeString,
            largeString,
            largeString,
            largeString
        };
        cmd.Parameters.AddWithValue("a", array);

        Assert.ThrowsAsync<OverflowException>(async () => await cmd.ExecuteNonQueryAsync());
    }

    [Test]
    public async Task Range_overflow_message_length_throws()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        var rangeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync(
            $"CREATE TYPE {type} AS (a text, b text, c text, d text, e text, f text, g text, h text);CREATE TYPE {rangeType} AS RANGE(subtype={type})");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<BigComposite>(type);
        dataSourceBuilder.EnableUnmappedTypes();
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var largeString = new string('A', (1 << 28) + 2000000);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT @a";
        var composite = new BigComposite
        {
            A = largeString,
            B = largeString,
            C = largeString,
            D = largeString
        };
        var range = new NpgsqlRange<BigComposite>(composite, composite);
        cmd.Parameters.Add(new NpgsqlParameter
        {
            Value = range,
            ParameterName = "a",
            DataTypeName = rangeType
        });

        Assert.ThrowsAsync<OverflowException>(async () => await cmd.ExecuteNonQueryAsync());
    }

    [Test]
    public async Task Multirange_overflow_message_length_throws()
    {
        await using var adminConnection = await OpenConnectionAsync();
        MinimumPgVersion(adminConnection, "14.0", "Multirange types were introduced in PostgreSQL 14");
        var type = await GetTempTypeName(adminConnection);
        var rangeType = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync(
            $"CREATE TYPE {type} AS (a text, b text, c text, d text, e text, f text, g text, h text);CREATE TYPE {rangeType} AS RANGE(subtype={type})");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<BigComposite>(type);
        dataSourceBuilder.EnableUnmappedTypes();
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var largeString = new string('A', (1 << 28) + 2000000);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT @a";
        var composite = new BigComposite
        {
            A = largeString
        };
        var range = new NpgsqlRange<BigComposite>(composite, composite);
        var multirange = new[]
        {
            range,
            range,
            range,
            range
        };
        cmd.Parameters.Add(new NpgsqlParameter
        {
            Value = multirange,
            ParameterName = "a",
            DataTypeName = rangeType + "_multirange"
        });

        Assert.ThrowsAsync<OverflowException>(async () => await cmd.ExecuteNonQueryAsync());
    }

    [Test, Description("CreateCommand before connection open")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/565")]
    public async Task Create_command_before_connection_open()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        var cmd = new NpgsqlCommand("SELECT 1", conn);
        conn.Open();
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public void Connection_not_set_throws()
    {
        var cmd = new NpgsqlCommand("SELECT 1");
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Connection_not_open_throws()
    {
        using var conn = CreateConnection();
        var cmd = new NpgsqlCommand("SELECT 1", conn);
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task ExecuteNonQuery_Throws_PostgresException([Values] bool async)
    {
        if (!async && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();

        var table1 = await CreateTempTable(conn, "id integer PRIMARY key, t varchar(40)");
        var table2 = await CreateTempTable(conn, $"id SERIAL primary key, {table1}_id integer references {table1}(id) INITIALLY DEFERRED");

        var sql = $"insert into {table2} ({table1}_id) values (1) returning id";

        var ex = async
            ? Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteNonQueryAsync(sql))
            : Assert.Throws<PostgresException>(() => conn.ExecuteNonQuery(sql));
        Assert.That(ex!.SqlState, Is.EqualTo(PostgresErrorCodes.ForeignKeyViolation));
    }

    [Test]
    public async Task ExecuteScalar_Throws_PostgresException([Values] bool async)
    {
        if (!async && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();

        var table1 = await CreateTempTable(conn, "id integer PRIMARY key, t varchar(40)");
        var table2 = await CreateTempTable(conn, $"id SERIAL primary key, {table1}_id integer references {table1}(id) INITIALLY DEFERRED");

        var sql = $"insert into {table2} ({table1}_id) values (1) returning id";

        var ex = async
            ? Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteScalarAsync(sql))
            : Assert.Throws<PostgresException>(() => conn.ExecuteScalar(sql));
        Assert.That(ex!.SqlState, Is.EqualTo(PostgresErrorCodes.ForeignKeyViolation));
    }

    [Test]
    public async Task ExecuteReader_Throws_PostgresException([Values] bool async)
    {
        if (!async && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();

        var table1 = await CreateTempTable(conn, "id integer PRIMARY key, t varchar(40)");
        var table2 = await CreateTempTable(conn, $"id SERIAL primary key, {table1}_id integer references {table1}(id) INITIALLY DEFERRED");

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"insert into {table2} ({table1}_id) values (1) returning id";

        await using var reader = async
            ? await cmd.ExecuteReaderAsync()
            : cmd.ExecuteReader();

        Assert.IsTrue(async ? await reader.ReadAsync() : reader.Read());
        var value = reader.GetInt32(0);
        Assert.That(value, Is.EqualTo(1));
        Assert.IsFalse(async ? await reader.ReadAsync() : reader.Read());
        var ex = async
            ? Assert.ThrowsAsync<PostgresException>(async () => await reader.NextResultAsync())
            : Assert.Throws<PostgresException>(() => reader.NextResult());
        Assert.That(ex!.SqlState, Is.EqualTo(PostgresErrorCodes.ForeignKeyViolation));
    }

    [Test]
    public void Command_is_recycled([Values] bool allResultTypesAreUnknown)
    {
        using var conn = OpenConnection();
        var cmd1 = conn.CreateCommand();
        cmd1.CommandText = "SELECT @p1";
        if (allResultTypesAreUnknown)
            cmd1.AllResultTypesAreUnknown = true;
        else
            cmd1.UnknownResultTypeList = [true];
        var tx = conn.BeginTransaction();
        cmd1.Transaction = tx;
        cmd1.Parameters.AddWithValue("p1", 8);
        _ = cmd1.ExecuteScalar();
        cmd1.Dispose();

        var cmd2 = conn.CreateCommand();
        Assert.That(cmd2, Is.SameAs(cmd1));
        Assert.That(cmd2.CommandText, Is.Empty);
        Assert.That(cmd2.CommandType, Is.EqualTo(CommandType.Text));
        Assert.That(cmd2.Transaction, Is.Null);
        Assert.That(cmd2.Parameters, Is.Empty);
        Assert.That(cmd2.AllResultTypesAreUnknown, Is.False);
        Assert.That(cmd2.UnknownResultTypeList, Is.Null);
        // TODO: Leaving this for now, since it'll be replaced by the new batching API
        // Assert.That(cmd2.Statements, Is.Empty);
    }

    [Test]
    public void Command_recycled_resets_CommandType()
    {
        using var conn = CreateConnection();
        var cmd1 = conn.CreateCommand();
        cmd1.CommandType = CommandType.StoredProcedure;
        cmd1.Dispose();

        var cmd2 = conn.CreateCommand();
        Assert.That(cmd2.CommandType, Is.EqualTo(CommandType.Text));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/831")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/2795")]
    public async Task Many_parameters([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "some_column INT");
        using var cmd = new NpgsqlCommand { Connection = conn };
        var sb = new StringBuilder($"INSERT INTO {table} (some_column) VALUES ");
        for (var i = 0; i < ushort.MaxValue; i++)
        {
            var paramName = "p" + i;
            cmd.Parameters.Add(new NpgsqlParameter(paramName, 8));
            if (i > 0)
                sb.Append(", ");
            sb.Append($"(@{paramName})");
        }

        cmd.CommandText = sb.ToString();

        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();

        await cmd.ExecuteNonQueryAsync();
    }

    [Test, Description("Bypasses PostgreSQL's uint16 limitation on the number of parameters")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/831")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/858")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/2703")]
    public async Task Too_many_parameters_throws([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand { Connection = conn };
        var sb = new StringBuilder("SOME RANDOM SQL ");
        for (var i = 0; i < ushort.MaxValue + 1; i++)
        {
            var paramName = "p" + i;
            cmd.Parameters.Add(new NpgsqlParameter(paramName, 8));
            if (i > 0)
                sb.Append(", ");
            sb.Append('@');
            sb.Append(paramName);
        }

        cmd.CommandText = sb.ToString();

        if (prepare == PrepareOrNot.Prepared)
        {
            Assert.That(() => cmd.Prepare(), Throws.Exception
                .InstanceOf<NpgsqlException>()
                .With.Message.EqualTo("A statement cannot have more than 65535 parameters"));
        }
        else
        {
            Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception
                .InstanceOf<NpgsqlException>()
                .With.Message.EqualTo("A statement cannot have more than 65535 parameters"));
        }
    }

    [Test, Description("An individual statement cannot have more than 65535 parameters, but a command can (across multiple statements).")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/1199")]
    public async Task Many_parameters_across_statements()
    {
        // Create a command with 1000 statements which have 70 params each
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand { Connection = conn };
        var paramIndex = 0;
        var sb = new StringBuilder();
        for (var statementIndex = 0; statementIndex < 1000; statementIndex++)
        {
            if (statementIndex > 0)
                sb.Append("; ");
            sb.Append("SELECT ");
            var startIndex = paramIndex;
            var endIndex = paramIndex + 70;
            for (; paramIndex < endIndex; paramIndex++)
            {
                var paramName = "p" + paramIndex;
                cmd.Parameters.Add(new NpgsqlParameter(paramName, 8));
                if (paramIndex > startIndex)
                    sb.Append(", ");
                sb.Append('@');
                sb.Append(paramName);
            }
        }

        cmd.CommandText = sb.ToString();
        await cmd.ExecuteNonQueryAsync();
    }

    [Test, Description("Makes sure that Npgsql doesn't attempt to send all data before the user can start reading. That would cause a deadlock.")]
    public async Task Batched_big_statements_do_not_deadlock()
    {
        // We're going to send a large multistatement query that would exhaust both the client's and server's
        // send and receive buffers (assume 64k per buffer).
        var data = new string('x', 1024);
        using var conn = await OpenConnectionAsync();
        var sb = new StringBuilder();
        for (var i = 0; i < 500; i++)
            sb.Append("SELECT @p;");
        using var cmd = new NpgsqlCommand(sb.ToString(), conn);
        cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, data);
        using var reader = await cmd.ExecuteReaderAsync();
        for (var i = 0; i < 500; i++)
        {
            reader.Read();
            Assert.That(reader.GetString(0), Is.EqualTo(data));
            reader.NextResult();
        }
    }

    [Test]
    public void Batched_small_then_big_statements_do_not_deadlock_in_sync_io()
    {
        if (IsMultiplexing)
            return; // Multiplexing, sync I/O

        // This makes sure we switch to async writing for batches, starting from the 2nd statement at the latest.
        // Otherwise, a small first first statement followed by a huge big one could cause us to deadlock, as we're stuck
        // synchronously sending the 2nd statement while PG is stuck sending the results of the 1st.
        using var conn = OpenConnection();
        var data = new string('x', 5_000_000);
        using var cmd = new NpgsqlCommand("SELECT generate_series(1, 500000); SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, data);
        cmd.ExecuteNonQuery();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1429")]
    public async Task Same_command_different_param_values()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", 8);
        await cmd.ExecuteNonQueryAsync();

        cmd.Parameters[0].Value = 9;
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(9));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1429")]
    public async Task Same_command_different_param_instances()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", 8);
        await cmd.ExecuteNonQueryAsync();

        cmd.Parameters.RemoveAt(0);
        cmd.Parameters.AddWithValue("p", 9);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(9));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3509"), Ignore("Flaky")]
    public async Task Bug3509()
    {
        if (IsMultiplexing)
            return;

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            KeepAlive = 1,
        };
        await using var postmasterMock = PgPostmasterMock.Start(csb.ToString());
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        var serverMock = await postmasterMock.WaitForServerConnection();
        // Wait for a keepalive to arrive at the server, reply with an error
        await serverMock.WaitForData();
        var queryTask = Task.Run(async () => await conn.ExecuteNonQueryAsync("SELECT 1"));
        // TODO: kind of flaky - think of the way to rewrite
        // giving a queryTask some time to get stuck on a lock
        await Task.Delay(300);
        await serverMock
            .WriteErrorResponse("42")
            .WriteReadyForQuery()
            .FlushAsync();

        await serverMock
            .WriteScalarResponseAndFlush(1);

        var ex = Assert.ThrowsAsync<NpgsqlException>(async () => await queryTask)!;
        Assert.That(ex.InnerException, Is.TypeOf<NpgsqlException>()
            .With.InnerException.TypeOf<PostgresException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4134")]
    public async Task Cached_command_double_dispose()
    {
        await using var conn = await OpenConnectionAsync();

        var cmd1 = conn.CreateCommand();
        cmd1.Dispose();
        cmd1.Dispose();

        var cmd2 = conn.CreateCommand();
        Assert.That(cmd2, Is.SameAs(cmd1));

        cmd2.CommandText = "SELECT 1";
        Assert.That(await cmd2.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4330")]
    public async Task Prepare_with_positional_placeholders_after_named()
    {
        if (IsMultiplexing)
            return; // Explicit preparation

        await using var conn = await OpenConnectionAsync();

        await using var command = new NpgsqlCommand("SELECT @p", conn);
        command.Parameters.AddWithValue("p", 10);
        await command.ExecuteNonQueryAsync();

        command.Parameters.Clear();

        command.CommandText = "SELECT $1";
        command.Parameters.Add(new() { NpgsqlDbType = NpgsqlDbType.Integer });
        Assert.DoesNotThrowAsync(() => command.PrepareAsync());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4621")]
    [Description("Most of 08* errors are coming whenever there was an error while connecting to a remote server from a cluster, so the connection to the cluster is still OK")]
    public async Task Postgres_connection_errors_not_break_connection()
    {
        if (IsMultiplexing)
            return;

        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT 1";
        var queryTask = cmd.ExecuteNonQueryAsync();

        var server = await postmasterMock.WaitForServerConnection();
        await server
            .WriteErrorResponse(PostgresErrorCodes.SqlClientUnableToEstablishSqlConnection)
            .WriteReadyForQuery()
            .FlushAsync();

        var ex = Assert.ThrowsAsync<PostgresException>(async () => await queryTask)!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.SqlClientUnableToEstablishSqlConnection));
        Assert.That(conn.FullState, Is.EqualTo(ConnectionState.Open));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4804")]
    [Description("Concurrent write and read failure can lead to deadlocks while cleaning up the connector.")]
    public async Task Concurrent_read_write_failure_deadlock()
    {
        if (IsMultiplexing)
            return;

        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmasterMock.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();

        await using var cmd = conn.CreateCommand();
        // Attempt to send a big enough query to fill buffers
        // That way the write side should be stuck, waiting for the server to empty buffers
        cmd.CommandText = new string('a', 8_000_000);
        var queryTask = cmd.ExecuteNonQueryAsync();

        var server = await postmasterMock.WaitForServerConnection();
        server.Close();

        Assert.ThrowsAsync<NpgsqlException>(async () => await queryTask);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4906")]
    [Description("Make sure we don't cancel a prepended query (and do not deadlock in case of a failure)")]
    [Explicit("Flaky due to #5033")]
    public async Task Not_cancel_prepended_query([Values] bool failPrependedQuery)
    {
        if (IsMultiplexing)
            return;

        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        var csb = new NpgsqlConnectionStringBuilder(postmasterMock.ConnectionString)
        {
            NoResetOnClose = false
        };
        await using var dataSource = CreateDataSource(csb.ConnectionString);
        await using var conn = await dataSource.OpenConnectionAsync();
        // reopen connection to append prepended query
        await conn.CloseAsync();
        await conn.OpenAsync();

        using var cts = new CancellationTokenSource();
        var queryTask = conn.ExecuteNonQueryAsync("SELECT 1", cancellationToken: cts.Token);

        var server = await postmasterMock.WaitForServerConnection();
        await server.ExpectSimpleQuery("DISCARD ALL");
        await server.ExpectExtendedQuery();

        var cancelTask = Task.Run(cts.Cancel);
        var cancellationRequestTask = postmasterMock.WaitForCancellationRequest().AsTask();
        // Give 1 second to make sure we didn't send cancellation request
        await Task.Delay(1000);
        Assert.IsFalse(cancelTask.IsCompleted);
        Assert.IsFalse(cancellationRequestTask.IsCompleted);

        if (failPrependedQuery)
        {
            await server
                .WriteErrorResponse(PostgresErrorCodes.SyntaxError)
                .WriteReadyForQuery()
                .FlushAsync();

            await cancelTask;
            await cancellationRequestTask;

            Assert.ThrowsAsync<PostgresException>(async () => await queryTask);
            Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
            return;
        }

        await server
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .FlushAsync();

        await cancelTask;
        await cancellationRequestTask;

        await server
            .WriteErrorResponse(PostgresErrorCodes.QueryCanceled)
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.ThrowsAsync<OperationCanceledException>(async () => await queryTask);

        queryTask = conn.ExecuteNonQueryAsync("SELECT 1");
        await server.ExpectExtendedQuery();
        await server
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteNoData()
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .FlushAsync();
        await queryTask;
    }

    [Test]
    public async Task Cancel_while_reading_from_long_running_query()
    {
        if (IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
SELECT *, CASE WHEN "t"."i" = 50000 THEN pg_sleep(100) ELSE NULL END
FROM
(
    SELECT generate_series(1, 1000000) AS "i"
) AS "t"
""";

        using (var cts = new CancellationTokenSource())
        await using (var reader = await cmd.ExecuteReaderAsync(cts.Token))
        {
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                var i = 0;
                while (await reader.ReadAsync(cts.Token))
                {
                    i++;
                    if (i == 10)
                        cts.Cancel();
                }
            });
        }

        cmd.CommandText = "SELECT 42";
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(42));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/5218")]
    [Description("Make sure we do not lose unread messages after resetting oversize buffer")]
    public async Task Oversize_buffer_lost_messages()
    {
        if (IsMultiplexing)
            return;

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            NoResetOnClose = true
        };
        await using var mock = PgPostmasterMock.Start(csb.ConnectionString);
        await using var dataSource = CreateDataSource(mock.ConnectionString);
        await using var connection = await dataSource.OpenConnectionAsync();
        var connector = connection.Connector!;

        var server = await mock.WaitForServerConnection();
        await server
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteRowDescription(new FieldDescription(TextOid))
            .WriteDataRowWithFlush(Encoding.ASCII.GetBytes(new string('a', connection.Settings.ReadBufferSize * 2)));
        // Just to make sure we have enough space
        await server.FlushAsync();
        await server
            .WriteDataRow(Encoding.ASCII.GetBytes("abc"))
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .WriteParameterStatus("SomeKey", "SomeValue")
            .FlushAsync();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT 1";
        await using (await cmd.ExecuteReaderAsync()) { }

        await connection.CloseAsync();
        await connection.OpenAsync();

        Assert.AreSame(connector, connection.Connector);
        // We'll get new value after the next query reads ParameterStatus from the buffer
        Assert.That(connection.PostgresParameters, Does.Not.ContainKey("SomeKey").WithValue("SomeValue"));

        await server
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteRowDescription(new FieldDescription(TextOid))
            .WriteDataRow(Encoding.ASCII.GetBytes("abc"))
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .FlushAsync();

        await cmd.ExecuteNonQueryAsync();

        Assert.That(connection.PostgresParameters, Contains.Key("SomeKey").WithValue("SomeValue"));
    }
}
