using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.Postgres;
using Npgsql.Tests.Support;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class PrepareTests: TestBase
{
    static uint Int4Oid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Int4).Value;

    [Test]
    public void Basic()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            AssertNumPreparedStatements(conn, 0);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
            Assert.That(cmd.IsPrepared, Is.False);

            cmd.Prepare();
            AssertNumPreparedStatements(conn, 1);
            Assert.That(cmd.IsPrepared, Is.True);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        }
        AssertNumPreparedStatements(conn, 1);
        conn.UnprepareAll();
    }

    [Test]
    public async Task Async()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            AssertNumPreparedStatements(conn, 0);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
            Assert.That(cmd.IsPrepared, Is.False);

            await cmd.PrepareAsync();
            AssertNumPreparedStatements(conn, 1);
            Assert.That(cmd.IsPrepared, Is.True);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        }
        AssertNumPreparedStatements(conn, 1);
        conn.UnprepareAll();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3443")]
    public void Bug3443()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        AssertNumPreparedStatements(conn, 0);
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        Assert.That(cmd.IsPrepared, Is.False);

        Assert.ThrowsAsync<OperationCanceledException>(() => cmd.PrepareAsync(new(canceled: true)));
        AssertNumPreparedStatements(conn, 0);
        Assert.That(cmd.IsPrepared, Is.False);

        using var cmd2 = new NpgsqlCommand("SELECT 1", conn);
        cmd2.Prepare();
        Assert.That(cmd2.ExecuteScalar(), Is.EqualTo(1));
        AssertNumPreparedStatements(conn, 1);
        Assert.That(cmd2.IsPrepared, Is.True);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4209")]
    public async Task Async_cancel_NullReferenceException()
    {
        for (var i = 0; i < 10; i++)
        {
            using var conn = OpenConnectionAndUnprepare();
            using var cmd = new NpgsqlCommand("SELECT 1", conn);
            using var cts = new CancellationTokenSource();
            using var mre = new ManualResetEventSlim();
            var cancelTask = Task.Run(() =>
            {
                mre.Wait();
                cts.Cancel();
            });
            try
            {
                mre.Set();
                await cmd.PrepareAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // There is a race between us checking the cancellation token and the cancellation itself.
                // If the cancellation happens first, we get OperationCancelledException.
                // In other case, PrepareAsync will not be cancelled and shouldn't throw any exceptions.
            }
            await cancelTask;

            Assert.That(conn.State, Is.EqualTo(ConnectionState.Open));
        }
    }

    [Test]
    public void Unprepare()
        => Unprepare(false).GetAwaiter().GetResult();

    [Test]
    public Task UnprepareAsync()
        => Unprepare(true);

    async Task Unprepare(bool async)
    {
        using var conn = OpenConnectionAndUnprepare();
        AssertNumPreparedStatements(conn, 0);
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        if(async)
            await cmd.PrepareAsync();
        else
            cmd.Prepare();

        AssertNumPreparedStatements(conn, 1);

        if (async)
            await cmd.UnprepareAsync();
        else
            cmd.Unprepare();

        AssertNumPreparedStatements(conn, 0);
        Assert.That(cmd.IsPrepared, Is.False);
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
    }

    [Test]
    public void Named_parameters()
    {
        using var conn = OpenConnectionAndUnprepare();

        for (var i = 0; i < 2; i++)
        {
            using var command = new NpgsqlCommand("SELECT @a, @b", conn);
            command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32));
            command.Parameters.Add(new NpgsqlParameter("b", 8));
            command.Prepare();
            command.Parameters[0].Value = 3;
            command.Parameters[1].Value = 5;

            using (var reader = command.ExecuteReader())
            {
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetInt32(0), Is.EqualTo(3));
                Assert.That(reader.GetInt64(1), Is.EqualTo(5));
            }

            command.Unprepare();
        }
    }

    [Test]
    public void Positional_parameters()
    {
        using var conn = OpenConnectionAndUnprepare();

        for (var i = 0; i < 2; i++)
        {
            using var command = new NpgsqlCommand("SELECT $1, $2", conn);
            command.Parameters.Add(new NpgsqlParameter { DbType = DbType.Int32 });
            command.Parameters.Add(new NpgsqlParameter { Value = 8 });
            command.Prepare();
            command.Parameters[0].Value = 3;
            command.Parameters[1].Value = 5;

            using (var reader = command.ExecuteReader())
            {
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetInt32(0), Is.EqualTo(3));
                Assert.That(reader.GetInt64(1), Is.EqualTo(5));
            }

            command.Unprepare();
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1207")]
    public void Double_prepare_same_sql()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Prepare();
        cmd.Prepare();
        AssertNumPreparedStatements(conn, 1);
        cmd.Unprepare();
        AssertNumPreparedStatements(conn, 0);
    }

    [Test]
    public void Double_prepare_different_sql()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = "SELECT 1";
        cmd.Prepare();
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT 2";
        cmd.Prepare();
        AssertNumPreparedStatements(conn, 2);
        cmd.ExecuteNonQuery();

        conn.UnprepareAll();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/395")]
    public void Across_close_open_same_connector()
    {
        using var dataSource = CreateDataSource();
        using var conn = dataSource.OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Prepare();
        Assert.That(cmd.IsPrepared, Is.True);
        var processId = conn.ProcessID;
        conn.Close();
        conn.Open();
        Assert.That(processId, Is.EqualTo(conn.ProcessID));
        Assert.That(cmd.IsPrepared, Is.True);
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        cmd.Prepare();
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
    }

    [Test]
    public void Across_close_open_different_connector()
    {
        using var dataSource = CreateDataSource();
        using var conn1 = dataSource.CreateConnection();
        using var conn2 = dataSource.CreateConnection();
        using var cmd = new NpgsqlCommand("SELECT 1", conn1);
        conn1.Open();
        cmd.Prepare();
        Assert.That(cmd.IsPrepared, Is.True);
        var processId = conn1.ProcessID;
        conn1.Close();
        conn2.Open();
        conn1.Open();
        Assert.That(conn1.ProcessID, Is.Not.EqualTo(processId));
        Assert.That(cmd.IsPrepared, Is.False);
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));  // Execute unprepared
        cmd.Prepare();
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
    }

    [Test]
    public void Reuse_prepared_statement()
    {
        using var dataSource = CreateDataSource();
        using var conn1 = dataSource.OpenConnection();
        var preparedStatement = Array.Empty<byte>();
        using (var cmd1 = new NpgsqlCommand("SELECT @p", conn1))
        {
            cmd1.Parameters.AddWithValue("p", 8);
            cmd1.Prepare();
            Assert.That(cmd1.IsPrepared, Is.True);
            Assert.That(cmd1.ExecuteScalar(), Is.EqualTo(8));
            preparedStatement = cmd1.InternalBatchCommands[0].PreparedStatement!.Name!;
        }

        using (var cmd2 = new NpgsqlCommand("SELECT @p", conn1))
        {
            cmd2.Parameters.AddWithValue("p", 8);
            cmd2.Prepare();
            Assert.That(cmd2.IsPrepared, Is.True);
            Assert.That(cmd2.InternalBatchCommands[0].PreparedStatement!.Name, Is.EqualTo(preparedStatement));
            Assert.That(cmd2.ExecuteScalar(), Is.EqualTo(8));
        }
    }

    [Test]
    public void Legacy_batching()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
        {
            cmd.Prepare();
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                reader.NextResult();
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(2));
            }
        }

        AssertNumPreparedStatements(conn, 2);

        using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
        {
            cmd.Prepare();
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                reader.NextResult();
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(2));
            }
        }

        AssertNumPreparedStatements(conn, 2);
        conn.UnprepareAll();
    }

    [Test]
    public void Batch()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var batch = new NpgsqlBatch(conn) { BatchCommands = { new("SELECT 1"), new("SELECT 2") } })
        {
            batch.Prepare();
            using (var reader = batch.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                reader.NextResult();
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(2));
            }
        }

        using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
        {
            cmd.Prepare();
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                reader.NextResult();
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(2));
            }
        }

        AssertNumPreparedStatements(conn, 2);

        using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
        {
            cmd.Prepare();
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(1));
                reader.NextResult();
                reader.Read();
                Assert.That(reader.GetInt32(0), Is.EqualTo(2));
            }
        }

        AssertNumPreparedStatements(conn, 2);
        conn.UnprepareAll();
    }

    [Test]
    public void One_command_same_sql_twice()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand("SELECT 1; SELECT 1", conn);
        cmd.Prepare();
        AssertNumPreparedStatements(conn, 1);
        cmd.ExecuteNonQuery();
        cmd.Unprepare();
    }

    [Test]
    public void One_command_same_sql_auto_prepare()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.MaxAutoPrepare = 5;
            csb.AutoPrepareMinUsages = 2;
        });
        using var conn = dataSource.OpenConnection();
        var sql = new StringBuilder();
        for (var i = 0; i < 2 + 1; i++)
            sql.Append("SELECT 1;");
        using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
            cmd.ExecuteNonQuery();
        AssertNumPreparedStatements(conn, 1);
    }

    [Test]
    public void One_command_same_sql_twice_with_params()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand("SELECT @p1; SELECT @p2", conn);
        cmd.Parameters.Add("p1", NpgsqlDbType.Integer);
        cmd.Parameters.Add("p2", NpgsqlDbType.Integer);
        cmd.Prepare();
        AssertNumPreparedStatements(conn, 1);

        cmd.Parameters[0].Value = 8;
        cmd.Parameters[1].Value = 9;
        using (var reader = cmd.ExecuteReader())
        {
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.GetInt32(0), Is.EqualTo(8));
            Assert.That(reader.NextResult(), Is.True);
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader.GetInt32(0), Is.EqualTo(9));
            Assert.That(reader.NextResult(), Is.False);
        }

        cmd.Unprepare();
    }

    [Test]
    public void Unprepare_via_different_command()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd1 = new NpgsqlCommand("SELECT 1; SELECT 2", conn);
        using var cmd2 = new NpgsqlCommand("SELECT 2; SELECT 3", conn);
        cmd1.Prepare();
        cmd2.Prepare();
        // Both commands reference the same prepared statement
        AssertNumPreparedStatements(conn, 3);
        cmd2.Unprepare();
        AssertNumPreparedStatements(conn, 1);
        Assert.That(cmd1.IsPrepared, Is.False);  // Only partially prepared, so no
        cmd1.ExecuteNonQuery();
        cmd1.Unprepare();
        AssertNumPreparedStatements(conn, 0);
        Assert.That(cmd1.IsPrepared, Is.False);
        cmd1.ExecuteNonQuery();

        conn.UnprepareAll();
    }

    [Test, Description("Prepares the same SQL with different parameters (overloading)")]
    public void Overloaded_sql()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.Add("p", NpgsqlDbType.Integer);
            cmd.Prepare();
            Assert.That(cmd.IsPrepared, Is.True);
        }
        using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, "foo");
            cmd.Prepare();
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo("foo"));
            Assert.That(cmd.IsPrepared, Is.False);
        }

        // SQL overloading is a pretty rare/exotic scenario. Handling it properly would involve keying
        // prepared statements not just by SQL but also by the parameter types, which would pointlessly
        // increase allocations. Instead, the second execution simply reruns unprepared
        AssertNumPreparedStatements(conn, 1);
        conn.UnprepareAll();
    }

    [Test]
    public void Many_statements_on_unprepare()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        var sb = new StringBuilder();
        for (var i = 0; i < conn.Settings.WriteBufferSize; i++)
            sb.Append("SELECT 1;");
        cmd.CommandText = sb.ToString();
        cmd.Prepare();
        cmd.Unprepare();
    }

    [Test]
    public void IsPrepared_is_false_after_changing_CommandText()
    {
        using var conn = OpenConnectionAndUnprepare();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Prepare();
        AssertNumPreparedStatements(conn, 1);
        cmd.CommandText = "SELECT 2";
        Assert.That(cmd.IsPrepared, Is.False);
        cmd.ExecuteNonQuery();
        Assert.That(cmd.IsPrepared, Is.False);
        AssertNumPreparedStatements(conn, 1);
        cmd.Unprepare();
    }

    [Test, Description("Basic persistent prepared system scenario. Checks that statement is not deallocated in the backend after command dispose.")]
    public void Persistent_across_commands()
    {
        using var conn = OpenConnectionAndUnprepare();
        AssertNumPreparedStatements(conn, 0);

        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            cmd.Prepare();
            AssertNumPreparedStatements(conn, 1);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        }
        AssertNumPreparedStatements(conn, 1);

        var stmtName = GetPreparedStatements(conn).Single();

        // Rerun the test using the persistent prepared statement
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            cmd.Prepare();
            Assert.That(cmd.IsPrepared, Is.True);
            AssertNumPreparedStatements(conn, 1);
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        }
        AssertNumPreparedStatements(conn, 1);
        Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName));
        conn.UnprepareAll();
    }

    [Test, Description("Basic persistent prepared system scenario. Checks that statement is not deallocated in the backend after connection close.")]
    public void Persistent_across_connections()
    {
        using var dataSource = CreateDataSource();
        using var conn = dataSource.OpenConnection();
        var processId = conn.ProcessID;

        AssertNumPreparedStatements(conn, 0);
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            cmd.Prepare();

        var stmtName = GetPreparedStatements(conn).Single();
        conn.Close();

        conn.Open();
        Assert.That(conn.ProcessID, Is.EqualTo(processId), "Unexpected connection received from the pool");

        AssertNumPreparedStatements(conn, 1, "Prepared statement deallocated");
        Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName), "Prepared statement name changed unexpectedly");

        // Rerun the test using the persistent prepared statement
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            cmd.Prepare();
            Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
        }
        AssertNumPreparedStatements(conn, 1, "Prepared statement deallocated");
        Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName), "Prepared statement name changed unexpectedly");
    }

    [Test, Description("Makes sure that calling Prepare() twice on a command does not deallocate or make a new one after the first prepared statement when command does not change")]
    public void Persistent_double_prepare_command_unchanged()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            var stmtName = GetPreparedStatements(conn).Single();
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            AssertNumPreparedStatements(conn, 1, "Unexpected count of prepared statements");
            Assert.That(GetPreparedStatements(conn).Single(), Is.EqualTo(stmtName), "Persistent prepared statement name changed unexpectedly");
        }
        AssertNumPreparedStatements(conn, 1, "Persistent prepared statement deallocated");
        conn.UnprepareAll();
    }

    [Test]
    public void Persistent_double_prepare_command_changed()
    {
        using var conn = OpenConnectionAndUnprepare();
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT 2";
            AssertNumPreparedStatements(conn, 1);
            cmd.Prepare();
            AssertNumPreparedStatements(conn, 2);
            cmd.ExecuteNonQuery();
        }
        AssertNumPreparedStatements(conn, 2);
        conn.UnprepareAll();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2665")]
    public void Prepared_command_failure()
    {
        using var conn = OpenConnection();

        using (var command = new NpgsqlCommand("INSERT INTO test_table (id) VALUES (1)", conn))
            Assert.Throws<PostgresException>(() => command.Prepare());

        conn.ExecuteNonQuery("CREATE TEMP TABLE test_table (id integer)");

        using (var command = new NpgsqlCommand("INSERT INTO test_table (id) VALUES (1)", conn))
        {
            command.Prepare();
            command.ExecuteNonQuery();
        }
    }

    /*
    [Test]
    public void Unpersist()
    {
        using (var conn = OpenConnectionAndUnprepare())
        {
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                cmd.Prepare(true);

            // Unpersist via a different command
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare(true);
                cmd.Unpersist();
                AssertNumPreparedStatements(conn, 0);
            }

            // Repersist
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare(true);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(1));
                cmd.Unpersist();
                AssertNumPreparedStatements(conn, 0);
            }

            // Unpersist via an unprepared command
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                cmd.Prepare(true);
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                cmd.Unpersist();
            AssertNumPreparedStatements(conn, 0);

            // Unpersist via a prepared but unpersisted command
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                cmd.Prepare(true);
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            {
                cmd.Prepare(false);
                cmd.Unpersist();
            }
            AssertNumPreparedStatements(conn, 0);
        }
    }

    [Test]
    public void Same_sql_different_params()
    {
        using (var conn = OpenConnectionAndUnprepare())
        using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            throw new NotImplementedException("Problem: current setting NpgsqlParameter.Value clears/invalidates...");
            cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Integer));
            cmd.Prepare(true);

            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Text;
            Assert.That(cmd.IsPrepared, Is.False);
            cmd.Prepare(true);
            using (var crapCmd = new NpgsqlCommand("SELECT name,statement,parameter_types::TEXT[] FROM pg_prepared_statements", conn))
            using (var reader = crapCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Statement: {reader.GetString(0)}, {reader.GetString(1)}");
                    foreach (var p in reader.GetFieldValue<string[]>(2))
                    {
                        Console.WriteLine("  Param: " + p);
                    }
                }
            }
            //AssertNumPreparedStatements(conn, 2);
            cmd.Parameters[0].Value = "hello";
            Console.WriteLine(cmd.ExecuteScalar());
        }
    }
    */

    [Test]
    public void Invalid_statement()
    {
        using var conn = OpenConnection();
        var cmd = new NpgsqlCommand("sele", conn);
        Assert.That(() => cmd.Prepare(), Throws.Exception.TypeOf<PostgresException>());
    }

    [Test]
    public void Prepare_multiple_commands_with_parameters()
    {
        using var conn = OpenConnection();
        using var cmd1 = new NpgsqlCommand("SELECT @p1;", conn);
        using var cmd2 = new NpgsqlCommand("SELECT @p1; SELECT @p2;", conn);
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
        var p21 = new NpgsqlParameter("p1", NpgsqlDbType.Text);
        var p22 = new NpgsqlParameter("p2", NpgsqlDbType.Text);
        cmd1.Parameters.Add(p1);
        cmd2.Parameters.Add(p21);
        cmd2.Parameters.Add(p22);
        cmd1.Prepare();
        cmd2.Prepare();
        p1.Value = 8;
        p21.Value = "foo";
        p22.Value = "bar";
        using (var reader1 = cmd1.ExecuteReader())
        {
            Assert.That(reader1.Read(), Is.True);
            Assert.That(reader1.GetInt32(0), Is.EqualTo(8));
        }
        using (var reader2 = cmd2.ExecuteReader())
        {
            Assert.That(reader2.Read(), Is.True);
            Assert.That(reader2.GetString(0), Is.EqualTo("foo"));
            Assert.That(reader2.NextResult(), Is.True);
            Assert.That(reader2.Read(), Is.True);
            Assert.That(reader2.GetString(0), Is.EqualTo("bar"));
        }
    }

    [Test]
    public void Multiplexing_not_supported()
    {
        using var dataSource = CreateDataSource(csb => csb.Multiplexing = true);
        using var conn = dataSource.OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);

        Assert.That(() => cmd.Prepare(), Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => conn.UnprepareAll(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task Explicitly_prepared_statement_invalidation([Values] bool prepareAfterError, [Values] bool unprepareAfterError)
    {
        await using var dataSource = CreateDataSource(csb =>
        {
            csb.MaxAutoPrepare = 10;
            csb.AutoPrepareMinUsages = 2;
        });
        await using var connection = await dataSource.OpenConnectionAsync();
        var table = await CreateTempTable(connection, "foo int");

        await using var command = new NpgsqlCommand($"SELECT * FROM {table}", connection);
        await command.PrepareAsync();

        await connection.ExecuteNonQueryAsync($"ALTER TABLE {table} RENAME COLUMN foo TO bar");

        // Since we've changed the table schema, the next execution of the prepared statement will error with 0A000
        var exception = Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync())!;
        Assert.That(exception.SqlState, Is.EqualTo(PostgresErrorCodes.FeatureNotSupported)); // cached plan must not change result type
        Assert.IsFalse(command.IsPrepared);

        if (unprepareAfterError)
        {
            // Just check that calling unprepare after error doesn't break anything
            await command.UnprepareAsync();
            Assert.IsFalse(command.IsPrepared);
        }

        if (prepareAfterError)
        {
            // If we explicitly prepare after error, we should replace the previous prepared statement with a new one
            await command.PrepareAsync();
            Assert.IsTrue(command.IsPrepared);
        }

        // However, Npgsql should invalidate the prepared statement in this case, so the next execution should work
        Assert.DoesNotThrowAsync(() => command.ExecuteNonQueryAsync());

        if (!prepareAfterError)
        {
            // The command is unprepared, though. It's the user's responsibility to re-prepare if they wish.
            Assert.False(command.IsPrepared);
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4920")]
    public async Task Explicit_prepare_unprepare_many_queries()
    {
        // Set a specific buffer's size to trigger #4920
        await using var dataSource = CreateDataSource(csb => csb.WriteBufferSize = 5002);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();

        cmd.CommandText = string.Join(';', Enumerable.Range(1, 500).Select(x => $"SELECT {x}"));
        await cmd.PrepareAsync();
        await cmd.UnprepareAsync();
    }

    [Test]
    public async Task Explicitly_prepared_batch_sends_prepared_queries()
    {
        await using var postmaster = PgPostmasterMock.Start(ConnectionString);
        await using var dataSource = CreateDataSource(postmaster.ConnectionString);

        await using var conn = await dataSource.OpenConnectionAsync();
        var server = await postmaster.WaitForServerConnection();

        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 2") }
        };

        var prepareTask = batch.PrepareAsync();

        await server.ExpectMessages(
            FrontendMessageCode.Parse, FrontendMessageCode.Describe,
            FrontendMessageCode.Parse, FrontendMessageCode.Describe,
            FrontendMessageCode.Sync);

        await server
            .WriteParseComplete()
            .WriteParameterDescription(new FieldDescription(Int4Oid))
            .WriteRowDescription(new FieldDescription(Int4Oid))
            .WriteParseComplete()
            .WriteParameterDescription(new FieldDescription(Int4Oid))
            .WriteRowDescription(new FieldDescription(Int4Oid))
            .WriteReadyForQuery()
            .FlushAsync();

        await prepareTask;

        for (var i = 0; i < 2; i++)
            await ExecutePreparedBatch(batch, server);

        async Task ExecutePreparedBatch(NpgsqlBatch batch, PgServerMock server)
        {
            var executeBatchTask = batch.ExecuteNonQueryAsync();

            await server.ExpectMessages(
                FrontendMessageCode.Bind, FrontendMessageCode.Execute,
                FrontendMessageCode.Bind, FrontendMessageCode.Execute,
                FrontendMessageCode.Sync);

            await server
                .WriteBindComplete()
                .WriteCommandComplete()
                .WriteBindComplete()
                .WriteCommandComplete()
                .WriteReadyForQuery()
                .FlushAsync();

            await executeBatchTask;
        }
    }

    [Test]
    public async Task Auto_prepared_batch_sends_prepared_queries()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            AutoPrepareMinUsages = 1,
            MaxAutoPrepare = 10
        };
        await using var postmaster = PgPostmasterMock.Start(csb.ConnectionString);
        await using var dataSource = CreateDataSource(postmaster.ConnectionString);

        await using var conn = await dataSource.OpenConnectionAsync();
        var server = await postmaster.WaitForServerConnection();

        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 2") }
        };

        var firstBatchExecuteTask = batch.ExecuteNonQueryAsync();

        await server.ExpectMessages(
            FrontendMessageCode.Parse, FrontendMessageCode.Bind, FrontendMessageCode.Describe, FrontendMessageCode.Execute,
            FrontendMessageCode.Parse, FrontendMessageCode.Bind, FrontendMessageCode.Describe, FrontendMessageCode.Execute,
            FrontendMessageCode.Sync);

        await server
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteRowDescription(new FieldDescription(Int4Oid))
            .WriteCommandComplete()
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteRowDescription(new FieldDescription(Int4Oid))
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .FlushAsync();

        await firstBatchExecuteTask;

        for (var i = 0; i < 2; i++)
            await ExecutePreparedBatch(batch, server);

        async Task ExecutePreparedBatch(NpgsqlBatch batch, PgServerMock server)
        {
            var executeBatchTask = batch.ExecuteNonQueryAsync();

            await server.ExpectMessages(
                FrontendMessageCode.Bind, FrontendMessageCode.Execute,
                FrontendMessageCode.Bind, FrontendMessageCode.Execute,
                FrontendMessageCode.Sync);

            await server
                .WriteBindComplete()
                .WriteCommandComplete()
                .WriteBindComplete()
                .WriteCommandComplete()
                .WriteReadyForQuery()
                .FlushAsync();

            await executeBatchTask;
        }
    }

    NpgsqlConnection OpenConnectionAndUnprepare()
    {
        var conn = OpenConnection();
        conn.UnprepareAll();
        return conn;
    }

    void AssertNumPreparedStatements(NpgsqlConnection conn, int expected)
        => Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements WHERE statement NOT LIKE '%FROM pg_prepared_statements%'"), Is.EqualTo(expected));

    void AssertNumPreparedStatements(NpgsqlConnection conn, int expected, string message)
        => Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM pg_prepared_statements WHERE statement NOT LIKE '%FROM pg_prepared_statements%'"), Is.EqualTo(expected), message);

    List<string> GetPreparedStatements(NpgsqlConnection conn)
    {
        var statements = new List<string>();
        using var cmd = new NpgsqlCommand("SELECT name FROM pg_prepared_statements WHERE statement NOT LIKE '%FROM pg_prepared_statement%'", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            statements.Add(reader.GetString(0));
        return statements;
    }
}
