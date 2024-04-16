using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class LoggingTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public async Task Command_ExecuteScalar_single_statement_without_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn);

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
    public async Task Command_ExecuteScalar_single_statement_with_positional_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1, $2", conn);
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
    public async Task Command_ExecuteScalar_single_statement__Should_unwrap_array_and_truncate_and_write_nulls()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1, $2, $3, $4, $5, $6", conn);
        cmd.Parameters.Add(new NpgsqlParameter<int> { TypedValue = 1024 });
        cmd.Parameters.Add(new NpgsqlParameter<int[]> { TypedValue = [1, 2, 3], NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Integer });
        cmd.Parameters.Add(new NpgsqlParameter<int[]> { TypedValue = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12], NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Integer });
        cmd.Parameters.Add(new NpgsqlParameter<int?[]> { TypedValue = [1, null], NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Integer });
        cmd.Parameters.Add(new NpgsqlParameter<int?> { TypedValue = null });
        cmd.Parameters.Add(new() { NpgsqlDbType = NpgsqlDbType.Integer, Value = DBNull.Value });

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Command execution completed")
            .And.Contains("SELECT $1, $2, $3, $4, $5, $6")
            .And.Contains("Parameters: [1024, [1, 2, 3], [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, ...], [1, NULL], NULL, NULL]"));
        AssertLoggingStateContains(executingCommandEvent, "CommandText", "SELECT $1, $2, $3, $4, $5, $6");
        AssertLoggingStateContains(executingCommandEvent, "Parameters", new object[] { 1024, "[1, 2, 3]", "[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, ...]", "[1, NULL]", "NULL", "NULL" });

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Command_ExecuteScalar_single_statement_with_named_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
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
    public async Task Command_ExecuteScalar_single_statement_with_parameter_logging_off()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider, sensitiveDataLoggingEnabled: false);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1, $2", conn);
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

    [Test]
    public async Task Command_ExecuteScalar_multiple_statement_without_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn);

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Batch execution completed").And.Contains("[(SELECT 1, System.Object[]), (SELECT 2, System.Object[])]"));
        var batchCommands = (IList<(string CommandText, object[] Parameters)>)AssertLoggingStateContains(executingCommandEvent, "BatchCommands");
        Assert.That(batchCommands.Count, Is.EqualTo(2));
        Assert.That(batchCommands[0].CommandText, Is.EqualTo("SELECT 1"));
        Assert.That(batchCommands[0].Parameters, Is.Empty);
        Assert.That(batchCommands[1].CommandText, Is.EqualTo("SELECT 2"));
        Assert.That(batchCommands[1].Parameters, Is.Empty);
        AssertLoggingStateDoesNotContain(executingCommandEvent, "Parameters");

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Command_ExecuteScalar_multiple_statement_with_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1; SELECT @p2", conn);
        cmd.Parameters.Add(new() { ParameterName = "p1", Value = 8 });
        cmd.Parameters.Add(new() { ParameterName = "p2", Value = 9 });

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Batch execution completed").And.Contains("[(SELECT $1, System.Object[]), (SELECT $1, System.Object[])]"));
        var batchCommands = (IList<(string CommandText, object[] Parameters)>)AssertLoggingStateContains(executingCommandEvent, "BatchCommands");
        Assert.That(batchCommands.Count, Is.EqualTo(2));
        Assert.That(batchCommands[0].CommandText, Is.EqualTo("SELECT $1"));
        Assert.That(batchCommands[0].Parameters[0], Is.EqualTo(8));
        Assert.That(batchCommands[1].CommandText, Is.EqualTo("SELECT $1"));
        Assert.That(batchCommands[1].Parameters[0], Is.EqualTo(9));
        AssertLoggingStateDoesNotContain(executingCommandEvent, "Parameters");

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Command_ExecuteScalar_multiple_statement_with_parameter_logging_off()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider, sensitiveDataLoggingEnabled: false);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1; SELECT @p2", conn);
        cmd.Parameters.Add(new() { ParameterName = "p1", Value = 8 });
        cmd.Parameters.Add(new() { ParameterName = "p2", Value = 9 });

        using (listLoggerProvider.Record())
        {
            await cmd.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Batch execution completed").And.Contains("[SELECT $1, SELECT $1]"));
        var batchCommands = (IList<string>)AssertLoggingStateContains(executingCommandEvent, "BatchCommands");
        Assert.That(batchCommands.Count, Is.EqualTo(2));
        Assert.That(batchCommands[0], Is.EqualTo("SELECT $1"));
        Assert.That(batchCommands[1], Is.EqualTo("SELECT $1"));
        AssertLoggingStateDoesNotContain(executingCommandEvent, "Parameters");

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);
    }

    [Test]
    public async Task Batch_ExecuteScalar_single_statement_without_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlBatch(conn)
        {
            BatchCommands = { new("SELECT 1") }
        };

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
    public async Task Batch_ExecuteScalar_multiple_statements_with_parameters()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new("SELECT $1") { Parameters = { new() { Value = 8 } } },
                new("SELECT $1, 9") { Parameters = { new() { Value = 9 } } }
            }
        };

        using (listLoggerProvider.Record())
        {
            await batch.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);

        // Note: the message formatter of Microsoft.Extensions.Logging doesn't seem to handle arrays inside tuples, so we get the
        // following ugliness (https://github.com/dotnet/runtime/issues/63165). Serilog handles this fine.
        Assert.That(executingCommandEvent.Message, Does.Contain("Batch execution completed").And.Contains("[(SELECT $1, System.Object[]), (SELECT $1, 9, System.Object[])]"));
        AssertLoggingStateDoesNotContain(executingCommandEvent, "CommandText");
        AssertLoggingStateDoesNotContain(executingCommandEvent, "Parameters");

        if (!IsMultiplexing)
            AssertLoggingStateContains(executingCommandEvent, "ConnectorId", conn.ProcessID);

        var batchCommands = (IList<(string CommandText, object[] Parameters)>)AssertLoggingStateContains(executingCommandEvent, "BatchCommands");
        Assert.That(batchCommands.Count, Is.EqualTo(2));
        Assert.That(batchCommands[0].CommandText, Is.EqualTo("SELECT $1"));
        Assert.That(batchCommands[0].Parameters[0], Is.EqualTo(8));
        Assert.That(batchCommands[1].CommandText, Is.EqualTo("SELECT $1, 9"));
        Assert.That(batchCommands[1].Parameters[0], Is.EqualTo(9));
    }

    [Test]
    public async Task Batch_ExecuteScalar_single_statement_with_parameter_logging_off()
    {
        await using var dataSource = CreateLoggingDataSource(out var listLoggerProvider, sensitiveDataLoggingEnabled: false);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var batch = new NpgsqlBatch(conn)
        {
            BatchCommands =
            {
                new("SELECT $1") { Parameters = { new() { Value = 8 } } },
                new("SELECT $1, 9") { Parameters = { new() { Value = 9 } } }
            }
        };

        using (listLoggerProvider.Record())
        {
            await batch.ExecuteScalarAsync();
        }

        var executingCommandEvent = listLoggerProvider.Log.Single(l => l.Id == NpgsqlEventId.CommandExecutionCompleted);
        Assert.That(executingCommandEvent.Message, Does.Contain("Batch execution completed").And.Contains("[SELECT $1, SELECT $1, 9]"));
        var batchCommands = (IList<string>)AssertLoggingStateContains(executingCommandEvent, "BatchCommands");
        Assert.That(batchCommands.Count, Is.EqualTo(2));
        Assert.That(batchCommands[0], Is.EqualTo("SELECT $1"));
        Assert.That(batchCommands[1], Is.EqualTo("SELECT $1, 9"));
    }
}
