using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Tests.Support;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Buffers.Binary;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class CommandTests : MultiplexingTestBase
{
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
        using var cmd = new NpgsqlCommandOrig("SELECT 1; SELECT 2", conn);
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
        using var cmd = new NpgsqlCommandOrig($"SELECT repeat('X', {conn.Settings.WriteBufferSize}); SELECT @p", conn);
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
        using var cmd = new NpgsqlCommandOrig("SELECT 1; SELECT 2", conn);
        Assert.That(async () => await cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.SyntaxError));
    }

    #endregion

    #region Timeout

    [Test, Description("Checks that CommandTimeout gets enforced as a socket timeout")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/327")]
    public async Task Timeout()
    {
        if (IsMultiplexing)
            return; // Multiplexing, Timeout

        // Mono throws a socket exception with WouldBlock instead of TimedOut (see #1330)
        var isMono = Type.GetType("Mono.Runtime") != null;
        using var conn = await OpenConnectionAsync(ConnectionString + ";CommandTimeout=1");
        using var cmd = CreateSleepCommand(conn, 10);
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

        using var conn = await OpenConnectionAsync(builder => builder.CommandTimeout = 1);
        using var cmd = CreateSleepCommand(conn, 10);
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
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);
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
        Assert.That(NpgsqlConnector.MinimumInternalCommandTimeout, Is.Not.EqualTo(NpgsqlCommandOrig.DefaultTimeout));
        var timeout = NpgsqlConnector.MinimumInternalCommandTimeout;
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            CommandTimeout = timeout
        }.ToString();
        using var conn = new NpgsqlConnection(connString);
        var command = new NpgsqlCommandOrig("SELECT 1", conn);
        conn.Open();
        Assert.That(command.CommandTimeout, Is.EqualTo(timeout));
        command.CommandTimeout = 10;
        await command.ExecuteScalarAsync();
        Assert.That(command.CommandTimeout, Is.EqualTo(10));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
    public async Task Timeout_switch_connection()
    {
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            if (conn.CommandTimeout >= 100 && conn.CommandTimeout < 105)
                TestUtil.IgnoreExceptOnBuildServer("Bad default command timeout");
        }

        using (var c1 = await OpenConnectionAsync(ConnectionString + ";CommandTimeout=100"))
        {
            using (var cmd = c1.CreateCommand())
            {
                Assert.That(cmd.CommandTimeout, Is.EqualTo(100));
                using (var c2 = new NpgsqlConnection(ConnectionString + ";CommandTimeout=101"))
                {
                    cmd.Connection = c2;
                    Assert.That(cmd.CommandTimeout, Is.EqualTo(101));
                }
                cmd.CommandTimeout = 102;
                using (var c2 = new NpgsqlConnection(ConnectionString + ";CommandTimeout=101"))
                {
                    cmd.Connection = c2;
                    Assert.That(cmd.CommandTimeout, Is.EqualTo(102));
                }
            }
        }
    }

    [Test]
    public async Task Prepare_timeout_hard([Values] SyncOrAsync async)
    {
        if (IsMultiplexing)
            return; // Multiplexing, Timeout

        var builder = new NpgsqlConnectionStringBuilder(ConnectionString) { CommandTimeout = 1 };
        await using var postmasterMock = PgPostmasterMock.Start(builder.ConnectionString);
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);
        await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
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

        using var conn = await OpenConnectionAsync();
        using var cmd = CreateSleepCommand(conn, 5);

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
        using var cmd = conn.CreateCommand();
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
        using var cmd = CreateSleepCommand(conn);
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

    [Test, Description("Cancels an async query with the cancellation token, with unsuccessful PG cancellation (socket break)")]
    public async Task Cancel_async_hard()
    {
        if (IsMultiplexing)
            return; // Multiplexing, cancellation

        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);
        await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        using var cancellationSource = new CancellationTokenSource();
        using var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
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
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);
        var serverMock = await postmasterMock.WaitForServerConnection();

        var processId = conn.ProcessID;

        using var cancellationSource = new CancellationTokenSource();
        using var cmd = new NpgsqlCommandOrig("SELECT 1", conn)
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
                .WriteRowDescription(new FieldDescription(PostgresTypeOIDs.Int4))
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
        using var conn = await OpenConnectionAsync();
        using var cmd1 = CreateSleepCommand(conn, 2);
        using var cmd2 = new NpgsqlCommandOrig("SELECT 1", conn);
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
        var command = new NpgsqlCommandOrig($"DECLARE TE CURSOR FOR SELECT * FROM {table}", conn);
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
        var command = new NpgsqlCommandOrig("DECLARE curs CURSOR FOR SELECT * FROM (VALUES (1), (2), (3)) as t", connection);
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
        using (var cmd = new NpgsqlCommandOrig("SELECT 1", conn))
        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
            while (reader.Read()) {}
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1194")]
    public async Task CloseConnection_with_open_reader_with_CloseConnection()
    {
        using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
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
        using (var cmd = new NpgsqlCommandOrig("SE", conn))
            Assert.That(() => cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection), Throws.Exception.TypeOf<PostgresException>());
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    #endregion

    [Test]
    public async Task SingleRow([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT 1, 2 UNION SELECT 3, 4", conn);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();

        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
        Assert.That(() => reader.GetInt32(0), Throws.Exception.TypeOf<InvalidOperationException>());
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        Assert.That(reader.Read(), Is.False);
    }

    #region Parameters

    [Test]
    public async Task Positional_parameter()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(8));
    }

    [Test]
    public async Task Positional_parameters_are_not_supported_with_legacy_batching()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1; SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.SyntaxError));
    }

    [Test]
    [NonParallelizable] // Disables sql rewriting
    public async Task Positional_parameters_are_supported_when_EnableSqlParsing_is_disabled()
    {
        using var _ = DisableSqlRewriting();

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(8));
    }

    [Test]
    [NonParallelizable] // Disables sql rewriting
    public async Task Named_parameters_are_not_supported_when_EnableSqlParsing_is_disabled()
    {
        using var _ = DisableSqlRewriting();

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter("p", 8));
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test, Description("Makes sure writing an unset parameter isn't allowed")]
    public async Task Parameter_without_Value()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter("@p", NpgsqlDbType.Integer));
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<InvalidCastException>());
    }

    [Test]
    public async Task Unreferenced_named_parameter_works()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
        cmd.Parameters.AddWithValue("not_used", 8);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Unreferenced_positional_parameter_works()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Mixing_positional_and_named_parameters_is_not_supported()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1, @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter { Value = 8 });
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = 9 });
        Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception.TypeOf<NotSupportedException>());
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
    [IssueLink("https://github.com/npgsql/npgsql/issues/4171")]
    public async Task Reuse_command_with_different_parameter_placeholder_types()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = "SELECT @p1";
        cmd.Parameters.AddWithValue("@p1", 8);
        _ = await cmd.ExecuteScalarAsync();

        cmd.CommandText = "SELECT $1";
        cmd.Parameters[0].ParameterName = null;
        _ = await cmd.ExecuteScalarAsync();
    }

    [Test]
    public async Task Positional_output_parameters_are_not_supported()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { Value = 8, Direction = ParameterDirection.InputOutput });
        Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test]
    public void Parameters_get_name()
    {
        var command = new NpgsqlCommandOrig();

        // Add parameters.
        command.Parameters.Add(new NpgsqlParameter(":Parameter1", DbType.Boolean));
        command.Parameters.Add(new NpgsqlParameter(":Parameter2", DbType.Int32));
        command.Parameters.Add(new NpgsqlParameter(":Parameter3", DbType.DateTime));
        command.Parameters.Add(new NpgsqlParameter("Parameter4", DbType.DateTime));

        var idbPrmtr = command.Parameters["Parameter1"];
        Assert.IsNotNull(idbPrmtr);
        command.Parameters[0].Value = 1;

        // Get by indexers.

        Assert.AreEqual(":Parameter1", command.Parameters["Parameter1"].ParameterName);
        Assert.AreEqual(":Parameter2", command.Parameters["Parameter2"].ParameterName);
        Assert.AreEqual(":Parameter3", command.Parameters["Parameter3"].ParameterName);
        Assert.AreEqual("Parameter4", command.Parameters["Parameter4"].ParameterName); //Should this work?

        Assert.AreEqual(":Parameter1", command.Parameters[0].ParameterName);
        Assert.AreEqual(":Parameter2", command.Parameters[1].ParameterName);
        Assert.AreEqual(":Parameter3", command.Parameters[2].ParameterName);
        Assert.AreEqual("Parameter4", command.Parameters[3].ParameterName);
    }

    [Test]
    public async Task Same_param_multiple_times()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p1, @p1", conn);
        cmd.Parameters.AddWithValue("@p1", 8);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader[0], Is.EqualTo(8));
        Assert.That(reader[1], Is.EqualTo(8));
    }

    [Test]
    public async Task Generic_parameter()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p1, @p2, @p3, @p4", conn);
        cmd.Parameters.Add(new NpgsqlParameter<int>("p1", 8));
        cmd.Parameters.Add(new NpgsqlParameter<short>("p2", 8) { NpgsqlDbType = NpgsqlDbType.Integer });
        cmd.Parameters.Add(new NpgsqlParameter<string>("p3", "hello"));
        cmd.Parameters.Add(new NpgsqlParameter<char[]>("p4", new[] { 'f', 'o', 'o' }));
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetInt32(0), Is.EqualTo(8));
        Assert.That(reader.GetInt32(1), Is.EqualTo(8));
        Assert.That(reader.GetString(2), Is.EqualTo("hello"));
        Assert.That(reader.GetString(3), Is.EqualTo("foo"));
    }

    #endregion Parameters

    [Test]
    public async Task CommandText_not_set()
    {
        using var conn = await OpenConnectionAsync();
        using (var cmd = new NpgsqlCommandOrig())
        {
            cmd.Connection = conn;
            Assert.That(cmd.ExecuteNonQueryAsync, Throws.Exception.TypeOf<InvalidOperationException>());
            cmd.CommandText = null;
            Assert.That(cmd.ExecuteNonQueryAsync, Throws.Exception.TypeOf<InvalidOperationException>());
            cmd.CommandText = "";
        }

        using (var cmd = conn.CreateCommand())
            Assert.That(cmd.ExecuteNonQueryAsync, Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task ExecuteScalar()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        using var command = new NpgsqlCommandOrig($"SELECT name FROM {table}", conn);
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
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig { Connection = conn };
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
        using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
        cmd.Dispose();
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(() => cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(() => cmd.PrepareAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    [Test, Description("Disposing a command with an open reader does not close the reader. This is the SqlClient behavior.")]
    public async Task Command_Dispose_does_not_close_reader()
    {
        using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommandOrig("SELECT 1, 2", conn);
        await cmd.ExecuteReaderAsync();
        cmd.Dispose();
        cmd = new NpgsqlCommandOrig("SELECT 3", conn);
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<NpgsqlOperationInProgressException>());
    }

    [Test]
    public async Task Non_standards_conforming_strings()
    {
        using var _ = CreateTempPool(ConnectionString, out var connString);
        await using var conn = await OpenConnectionAsync(connString);

        if (IsMultiplexing)
        {
            Assert.That(() => conn.ExecuteNonQueryAsync("set standard_conforming_strings=off"),
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
        using var conn = await OpenConnectionAsync();
        //Without parenthesis the meaning of [, . and potentially other characters is
        //a syntax error. See comment in NpgsqlCommandOrig.GetClearCommandText() on "usually-redundant parenthesis".
        using var command = new NpgsqlCommandOrig("select :arr[2]", conn);
        command.Parameters.AddWithValue(":arr", new int[] {5, 4, 3, 2, 1});
        using var rdr = await command.ExecuteReaderAsync();
        rdr.Read();
        Assert.AreEqual(rdr.GetInt32(0), 4);
    }

    [Test]
    [TestCase(CommandBehavior.Default)]
    [TestCase(CommandBehavior.SequentialAccess)]
    public async Task Statement_mapped_output_parameters(CommandBehavior behavior)
    {
        using var conn = await OpenConnectionAsync();
        var command = new NpgsqlCommandOrig("select 3, 4 as param1, 5 as param2, 6;", conn);

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

        using var reader = await command.ExecuteReaderAsync(behavior);

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

        var command = new NpgsqlCommandOrig(createFunction, conn);
        await command.ExecuteNonQueryAsync();

        command = new NpgsqlCommandOrig(sproc, conn);
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

        var command = new NpgsqlCommandOrig($"SELECT * FROM {table}", conn);
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
        using var cmd = new NpgsqlCommandOrig(table, conn) { CommandType = CommandType.TableDirect };
        using var rdr = await cmd.ExecuteReaderAsync();
        Assert.That(rdr.Read(), Is.True);
        Assert.That(rdr["name"], Is.EqualTo("foo"));
    }

    [Test]
    [TestCase(CommandBehavior.Default)]
    [TestCase(CommandBehavior.SequentialAccess)]
    public async Task Input_and_output_parameters(CommandBehavior behavior)
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @c-1 AS c, @a+2 AS b", conn);
        cmd.Parameters.Add(new NpgsqlParameter("a", 3));
        var b = new NpgsqlParameter { ParameterName = "b", Direction = ParameterDirection.Output };
        cmd.Parameters.Add(b);
        var c = new NpgsqlParameter { ParameterName = "c", Direction = ParameterDirection.InputOutput, Value = 4 };
        cmd.Parameters.Add(c);
        using (await cmd.ExecuteReaderAsync(behavior))
        {
            Assert.AreEqual(5, b.Value);
            Assert.AreEqual(3, c.Value);
        }
    }

    [Test]
    public async Task Send_NpgsqlDbType_Unknown([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p::TIMESTAMP", conn);
        cmd.CommandText = "SELECT @p::TIMESTAMP";
        cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Unknown) { Value = "2008-1-1" });
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(new DateTime(2008, 1, 1)));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/503")]
    public async Task Invalid_UTF8()
    {
        const string badString = "SELECT 'abc\uD801\uD802d'";
        using var _ = CreateTempPool(ConnectionString, out var connString);
        using var conn = await OpenConnectionAsync(connString);
        Assert.That(() => conn.ExecuteScalarAsync(badString), Throws.Exception.TypeOf<EncoderFallbackException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
    public async Task Use_across_connection_change([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        using var conn1 = await OpenConnectionAsync();
        using var conn2 = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT 1", conn1);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        cmd.Connection = conn2;
        Assert.That(cmd.IsPrepared, Is.False);
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test, Description("CreateCommand before connection open")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/565")]
    public async Task Create_command_before_connection_open()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
        conn.Open();
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public void Connection_not_set_throws()
    {
        var cmd = new NpgsqlCommandOrig("SELECT 1");
        Assert.That(() => cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Connection_not_open_throws()
    {
        using var conn = CreateConnection();
        var cmd = new NpgsqlCommandOrig("SELECT 1", conn);
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
    public void Command_is_recycled()
    {
        using var conn = OpenConnection();
        var cmd1 = conn.CreateCommand();
        cmd1.CommandText = "SELECT @p1";
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
        using var cmd = new NpgsqlCommandOrig { Connection = conn };
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
        using var cmd = new NpgsqlCommandOrig { Connection = conn };
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
        using var cmd = new NpgsqlCommandOrig { Connection = conn };
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
        using var cmd = new NpgsqlCommandOrig(sb.ToString(), conn);
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
        using var cmd = new NpgsqlCommandOrig("SELECT generate_series(1, 500000); SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, data);
        cmd.ExecuteNonQuery();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1429")]
    public async Task Same_command_different_param_values()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", 8);
        await cmd.ExecuteNonQueryAsync();

        cmd.Parameters[0].Value = 9;
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(9));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1429")]
    public async Task Same_command_different_param_instances()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT @p", conn);
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
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);
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

        await using var command = new NpgsqlCommandOrig("SELECT @p", conn);
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
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);

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
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);

        await using var cmd = conn.CreateCommand();
        // Attempt to send a big enough query to fill buffers
        // That way the write side should be stuck, waiting for the server to empty buffers
        cmd.CommandText = new string('a', 8_000_000);
        var queryTask = cmd.ExecuteNonQueryAsync();

        var server = await postmasterMock.WaitForServerConnection();
        server.Close();

        Assert.ThrowsAsync<NpgsqlException>(async () => await queryTask);
    }

    #region Logging

    [Test]
    public async Task Log_ExecuteScalar_single_statement_without_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT 1", conn);

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Command execution completed").And.Contains("SELECT 1"));
        AssertLoggingStateContains(executingCommandEvent, "CommandText", "SELECT 1");
        AssertLoggingStateDoesNotContain(executingCommandEvent, "Parameters");

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Log_ExecuteScalar_single_statement_with_positional_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1, $2", conn);
        cmd.Parameters.Add(new() { Value = 8 });
        cmd.Parameters.Add(new() { NpgsqlDbType = NpgsqlDbType.Integer, Value = DBNull.Value });

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Command execution completed")
            .And.Contains("SELECT $1, $2")
            .And.Contains("Parameters: [8, NULL]"));
        AssertLoggingStateContains(executingCommandEvent, "CommandText", "SELECT $1, $2");
        AssertLoggingStateContains(executingCommandEvent, "Parameters", new object[] { 8, "NULL" });

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Log_ExecuteScalar_single_statement_with_named_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT @p1, @p2", conn);
        cmd.Parameters.Add(new() { ParameterName = "p1", Value = 8 });
        cmd.Parameters.Add(new() { ParameterName = "p2", NpgsqlDbType = NpgsqlDbType.Integer, Value = DBNull.Value });

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Command execution completed")
            .And.Contains("SELECT $1, $2")
            .And.Contains("Parameters: [8, NULL]"));
        AssertLoggingStateContains(executingCommandEvent, "CommandText", "SELECT $1, $2");
        AssertLoggingStateContains(executingCommandEvent, "Parameters", new object[] { 8, "NULL" });

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Log_ExecuteScalar_single_statement_with_parameter_logging_off()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider, sensitiveDataLoggingEnabled: false);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommandOrig("SELECT $1, $2", conn);
        cmd.Parameters.Add(new() { Value = 8 });
        cmd.Parameters.Add(new() { Value = 9 });

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Command execution completed").And.Contains($"SELECT $1, $2"));
        AssertLoggingStateContains(executingCommandEvent, "CommandText", "SELECT $1, $2");
        AssertLoggingStateDoesNotContain(executingCommandEvent, "Parameters");
    }

    #endregion Logging

    public CommandTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
