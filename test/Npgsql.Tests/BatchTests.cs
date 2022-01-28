using System;
using System.Data;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

[TestFixture(MultiplexingMode.NonMultiplexing, CommandBehavior.Default)]
[TestFixture(MultiplexingMode.Multiplexing, CommandBehavior.Default)]
[TestFixture(MultiplexingMode.NonMultiplexing, CommandBehavior.SequentialAccess)]
[TestFixture(MultiplexingMode.Multiplexing, CommandBehavior.SequentialAccess)]
public class BatchTests : MultiplexingTestBase
{
    #region Parameters

    [Test]
    public async Task Named_parameters()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new("SELECT @p") { Parameters = { new("p", 8) } },
                new("SELECT @p1, @p2") { Parameters = { new("p1", 9), new("p2", 10) } }
            }
        };

        await using var reader = await batch.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader.FieldCount, Is.EqualTo(1));
        Assert.That(reader[0], Is.EqualTo(8));
        Assert.That(await reader.ReadAsync(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.True);
        Assert.That(reader.FieldCount, Is.EqualTo(2));
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader[0], Is.EqualTo(9));
        Assert.That(reader[1], Is.EqualTo(10));
        Assert.That(await reader.ReadAsync(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test]
    public async Task Positional_parameters()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new("SELECT $1") { Parameters = { new() { Value = 8 } } },
                new("SELECT $1, $2") { Parameters = { new() { Value = 9 }, new() { Value = 10 } } }
            }
        };

        await using var reader = await batch.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader.FieldCount, Is.EqualTo(1));
        Assert.That(reader[0], Is.EqualTo(8));
        Assert.That(await reader.ReadAsync(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.True);
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader.FieldCount, Is.EqualTo(2));
        Assert.That(reader[0], Is.EqualTo(9));
        Assert.That(reader[1], Is.EqualTo(10));
        Assert.That(await reader.ReadAsync(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test]
    public async Task Out_parameters_are_not_allowed()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new("SELECT @p1")
                {
                    Parameters = { new("p", 8) { Direction = ParameterDirection.InputOutput } }
                }
            }
        };

        Assert.That(() => batch.ExecuteReaderAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    #endregion Parameters

    #region NpgsqlBatchCommand

    [Test]
    public async Task RecordsAffected_and_Rows()
    {
        await using var conn = await OpenConnectionAsync();
        await using var _ = await CreateTempTable(conn, "name TEXT", out var table);

        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new($"INSERT INTO {table} (name) VALUES ('a'), ('b')"),
                new($"UPDATE {table} SET name='c' WHERE name='b'"),
                new($"UPDATE {table} SET name='d' WHERE name='doesnt_exist'"),
                new($"SELECT name FROM {table}"),
                new($"DELETE FROM {table}")
            }
        };
        await using var reader = await batch.ExecuteReaderAsync(Behavior);

        // Consume SELECT result set to parse the CommandComplete
        await reader.CloseAsync();

        var command = batch.BatchCommands[0];
        Assert.That(command.RecordsAffected, Is.EqualTo(2));
        Assert.That(command.Rows, Is.EqualTo(2));

        command = batch.BatchCommands[1];
        Assert.That(command.RecordsAffected, Is.EqualTo(1));
        Assert.That(command.Rows, Is.EqualTo(1));

        command = batch.BatchCommands[2];
        Assert.That(command.RecordsAffected, Is.EqualTo(0));
        Assert.That(command.Rows, Is.EqualTo(0));

        command = batch.BatchCommands[3];
        Assert.That(command.RecordsAffected, Is.EqualTo(-1));
        Assert.That(command.Rows, Is.EqualTo(2));

        command = batch.BatchCommands[4];
        Assert.That(command.RecordsAffected, Is.EqualTo(2));
        Assert.That(command.Rows, Is.EqualTo(2));
    }

    [Test]
    public async Task NpgsqlBatchCommand_StatementType()
    {
        await using var conn = await OpenConnectionAsync();
        await using var _ = await CreateTempTable(conn, "name TEXT", out var table);

        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new($"INSERT INTO {table} (name) VALUES ('a'), ('b')"),
                new($"UPDATE {table} SET name='c' WHERE name='b'"),
                new($"UPDATE {table} SET name='d' WHERE name='doesnt_exist'"),
                new("BEGIN"),
                new($"SELECT name FROM {table}"),
                new($"DELETE FROM {table}"),
                new("COMMIT")
            }
        };
        await using var reader = await batch.ExecuteReaderAsync(Behavior);

        // Consume SELECT result set to parse the CommandComplete
        await reader.CloseAsync();

        Assert.That(batch.BatchCommands[0].StatementType, Is.EqualTo(StatementType.Insert));
        Assert.That(batch.BatchCommands[1].StatementType, Is.EqualTo(StatementType.Update));
        Assert.That(batch.BatchCommands[2].StatementType, Is.EqualTo(StatementType.Update));
        Assert.That(batch.BatchCommands[3].StatementType, Is.EqualTo(StatementType.Other));
        Assert.That(batch.BatchCommands[4].StatementType, Is.EqualTo(StatementType.Select));
        Assert.That(batch.BatchCommands[5].StatementType, Is.EqualTo(StatementType.Delete));
        Assert.That(batch.BatchCommands[6].StatementType, Is.EqualTo(StatementType.Other));
    }

    [Test]
    public async Task StatementOID()
    {
        using var conn = await OpenConnectionAsync();

        MaximumPgVersionExclusive(conn, "12.0",
            "Support for 'CREATE TABLE ... WITH OIDS' has been removed in 12.0. See https://www.postgresql.org/docs/12/release-12.html#id-1.11.6.5.4");

        await using var _ = await GetTempTableName(conn, out var table);
        await conn.ExecuteNonQueryAsync($"CREATE TABLE {table} (name TEXT) WITH OIDS");
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new($"INSERT INTO {table} (name) VALUES (@p1)") { Parameters = { new("p1", "foo") } },
                new($"UPDATE {table} SET name='b' WHERE name=@p2") { Parameters = { new("p2", "bar") } }
            }
        };

        await batch.ExecuteNonQueryAsync();

        Assert.That(batch.BatchCommands[0].OID, Is.Not.EqualTo(0));
        Assert.That(batch.BatchCommands[1].OID, Is.EqualTo(0));
    }

    #endregion NpgsqlBatchCommand

    #region Command behaviors

    [Test]
    public async Task SingleResult()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 2") }
        };
        var reader = await batch.ExecuteReaderAsync(CommandBehavior.SingleResult | Behavior);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        Assert.That(reader.NextResult(), Is.False);
    }

    [Test]
    public async Task SingleRow()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 2") }
        };

        await using var reader = await batch.ExecuteReaderAsync(CommandBehavior.SingleRow | Behavior);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        Assert.That(reader.Read(), Is.False);
        Assert.That(reader.NextResult(), Is.False);
    }

    [Test]
    public async Task SchemaOnly_GetFieldType()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 'foo'") }
        };

        await using var reader = await batch.ExecuteReaderAsync(CommandBehavior.SchemaOnly | Behavior);
        Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(int)));
        Assert.That(await reader.NextResultAsync(), Is.True);
        Assert.That(reader.GetFieldType(0), Is.SameAs(typeof(string)));
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test]
    public async Task SchemaOnly_returns_no_data()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 'foo'") }
        };

        await using var reader = await batch.ExecuteReaderAsync(CommandBehavior.SchemaOnly | Behavior);
        Assert.That(reader.Read(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.True);
        Assert.That(reader.Read(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/693")]
    public async Task CloseConnection()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1"), new("SELECT 2") }
        };

        await using (var reader = await batch.ExecuteReaderAsync(CommandBehavior.CloseConnection | Behavior))
            while (reader.Read()) {}
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    #endregion Command behaviors

    #region Miscellaneous

    [Test]
    public async Task Single_batch_command()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 8") }
        };

        await using var reader = await batch.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync(), Is.True);
        Assert.That(reader.FieldCount, Is.EqualTo(1));
        Assert.That(reader[0], Is.EqualTo(8));
        Assert.That(await reader.ReadAsync(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test]
    public async Task Empty_batch()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn);
        await using var reader = await batch.ExecuteReaderAsync();

        Assert.That(await reader.ReadAsync(), Is.False);
        Assert.That(await reader.NextResultAsync(), Is.False);
    }

    [Test]
    public async Task Semicolon_is_not_allowed()
    {
        await using var conn = await OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1; SELECT 2") }
        };

        Assert.That(() => batch.ExecuteReaderAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/967")]
    public async Task NpgsqlException_references_BatchCommand_with_single_command()
    {
        await using var conn = await OpenConnectionAsync();
        await using var _ = GetTempFunctionName(conn, out var function);

        await conn.ExecuteNonQueryAsync($@"
CREATE OR REPLACE FUNCTION {function}() RETURNS VOID AS
   'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
LANGUAGE 'plpgsql'");

        // We use NpgsqlConnection.CreateBatch to test that the batch isn't recycled when referenced in an exception
        var batch = conn.CreateBatch();
        batch.BatchCommands.Add(new($"SELECT {function}()"));

        try
        {
            await batch.ExecuteReaderAsync(Behavior);
            Assert.Fail();
        }
        catch (PostgresException e)
        {
            Assert.That(e.BatchCommand, Is.SameAs(batch.BatchCommands[0]));
        }

        // Make sure the command isn't recycled by the connection when it's disposed - this is important since internal command
        // resources are referenced by the exception above, which is very likely to escape the using statement of the command.
        batch.Dispose();
        var cmd2 = conn.CreateBatch();
        Assert.AreNotSame(cmd2, batch);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/967")]
    public async Task NpgsqlException_references_BatchCommand_with_multiple_commands()
    {
        await using var conn = await OpenConnectionAsync();
        await using var _ = GetTempFunctionName(conn, out var function);

        await conn.ExecuteNonQueryAsync($@"
CREATE OR REPLACE FUNCTION {function}() RETURNS VOID AS
   'BEGIN RAISE EXCEPTION ''testexception'' USING ERRCODE = ''12345''; END;'
LANGUAGE 'plpgsql'");

        // We use NpgsqlConnection.CreateBatch to test that the batch isn't recycled when referenced in an exception
        var batch = conn.CreateBatch();
        batch.BatchCommands.Add(new("SELECT 1"));
        batch.BatchCommands.Add(new($"SELECT {function}()"));

        await using (var reader = await batch.ExecuteReaderAsync(Behavior))
        {
            try
            {
                await reader.NextResultAsync();
                Assert.Fail();
            }
            catch (PostgresException e)
            {
                Assert.That(e.BatchCommand, Is.SameAs(batch.BatchCommands[1]));
            }
        }

        // Make sure the command isn't recycled by the connection when it's disposed - this is important since internal command
        // resources are referenced by the exception above, which is very likely to escape the using statement of the command.
        batch.Dispose();
        var cmd2 = conn.CreateBatch();
        Assert.AreNotSame(cmd2, batch);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4202")]
    public async Task ExecuteScalar_without_parameters()
    {
        await using var conn = await OpenConnectionAsync();
        var batch = new NpgsqlBatch(conn) { BatchCommands = { new("SELECT 1") } };
        Assert.That(await batch.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    #endregion Miscellaneous

    #region Initialization / setup / teardown

    // ReSharper disable InconsistentNaming
    readonly bool IsSequential;
    readonly CommandBehavior Behavior;
    // ReSharper restore InconsistentNaming

    public BatchTests(MultiplexingMode multiplexingMode, CommandBehavior behavior) : base(multiplexingMode)
    {
        Behavior = behavior;
        IsSequential = (Behavior & CommandBehavior.SequentialAccess) != 0;
    }

    #endregion
}