using System.Data;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class StoredProcedureTests : TestBase
{
    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task With_input_parameters(bool withPositional, bool withNamed)
    {
        var table = await CreateTempTable(SharedDataSource, "foo int, bar int");
        var sproc = await GetTempProcedureName(SharedDataSource);

        await SharedDataSource.ExecuteNonQueryAsync(@$"
CREATE PROCEDURE {sproc}(a int, b int)
LANGUAGE SQL
AS $$
    INSERT INTO {table} VALUES (a, b);
$$");

        await using (var command = SharedDataSource.CreateCommand(sproc))
        {
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(withPositional
                ? new() { Value = 8 }
                : new() { ParameterName = "a", Value = 8 });

            command.Parameters.Add(withNamed
                ? new() { ParameterName = "b", Value = 9 }
                : new() { Value = 9 });

            await command.ExecuteNonQueryAsync();
        }

        await using (var command = SharedDataSource.CreateCommand($"SELECT * FROM {table}"))
        await using (var reader = await command.ExecuteReaderAsync())
        {
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo(8));
            Assert.That(reader[1], Is.EqualTo(9));
        }
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task With_output_parameters(bool withPositional, bool withNamed)
    {
        MinimumPgVersion(SharedDataSource, "14.0", "Stored procedure OUT parameters are only support starting with version 14");

        var sproc = await GetTempProcedureName(SharedDataSource);

        await SharedDataSource.ExecuteNonQueryAsync(@$"
CREATE PROCEDURE {sproc}(a int, OUT out1 int, OUT out2 int, b int)
LANGUAGE plpgsql
AS $$
BEGIN
    out1 = a;
    out2 = b;
END$$");

        await using var command = SharedDataSource.CreateCommand(sproc);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new() { Value = 8 });

        command.Parameters.Add(withPositional
            ? new() { Direction = ParameterDirection.Output }
            : new() { ParameterName = "out1", Direction = ParameterDirection.Output });

        command.Parameters.Add(withNamed
            ? new() { ParameterName = "out2", Direction = ParameterDirection.Output }
            : new() { Direction = ParameterDirection.Output });

        command.Parameters.Add(new() { ParameterName = "b", Value = 9 });

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader[0], Is.EqualTo(8));
        Assert.That(reader[1], Is.EqualTo(9));
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task With_input_output_parameters(bool withPositional, bool withNamed)
    {
        var sproc = await GetTempProcedureName(SharedDataSource);

        await SharedDataSource.ExecuteNonQueryAsync(@$"
CREATE PROCEDURE {sproc}(a int, INOUT inout1 int, INOUT inout2 int, b int)
LANGUAGE plpgsql
AS $$
BEGIN
    inout1 = inout1 + a;
    inout2 = inout2 + b;
END$$");

        await using var command = SharedDataSource.CreateCommand(sproc);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new() { Value = 8 });

        command.Parameters.Add(withPositional
            ? new() { Value = 1, Direction = ParameterDirection.InputOutput }
            : new() { ParameterName = "inout1", Value = 1, Direction = ParameterDirection.InputOutput });

        command.Parameters.Add(withNamed
            ? new() { ParameterName = "inout2", Value = 2, Direction = ParameterDirection.InputOutput }
            : new() { Value = 2, Direction = ParameterDirection.InputOutput });

        command.Parameters.Add(new() { ParameterName = "b", Value = 9 });

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        Assert.That(reader[0], Is.EqualTo(9));
        Assert.That(reader[1], Is.EqualTo(11));
    }

    #region DeriveParameters

    [Test, Description("Tests function parameter derivation with IN, OUT and INOUT parameters")]
    public async Task DeriveParameters_procedure_various()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Stored procedure OUT parameters are only support starting with version 14");
        var sproc = await GetTempProcedureName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE PROCEDURE {sproc}(IN param1 INT, OUT param2 text, INOUT param3 INT) AS $$
BEGIN
    param2 = 'sometext';
    param3 = param1 + param3;
END;
$$ LANGUAGE plpgsql");

        await using var command = new NpgsqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(command);
        Assert.That(command.Parameters, Has.Count.EqualTo(3));
        Assert.That(command.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(command.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(command.Parameters[0].PostgresType, Is.TypeOf<PostgresBaseType>());
        Assert.That(command.Parameters[0].DataTypeName, Is.EqualTo("integer"));
        Assert.That(command.Parameters[0].ParameterName, Is.EqualTo("param1"));
        Assert.That(command.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(command.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text));
        Assert.That(command.Parameters[1].PostgresType, Is.TypeOf<PostgresBaseType>());
        Assert.That(command.Parameters[1].DataTypeName, Is.EqualTo("text"));
        Assert.That(command.Parameters[1].ParameterName, Is.EqualTo("param2"));
        Assert.That(command.Parameters[2].Direction, Is.EqualTo(ParameterDirection.InputOutput));
        Assert.That(command.Parameters[2].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(command.Parameters[2].PostgresType, Is.TypeOf<PostgresBaseType>());
        Assert.That(command.Parameters[2].DataTypeName, Is.EqualTo("integer"));
        Assert.That(command.Parameters[2].ParameterName, Is.EqualTo("param3"));
        command.Parameters[0].Value = 5;
        command.Parameters[2].Value = 4;
        await command.ExecuteNonQueryAsync();
        Assert.That(command.Parameters[0].Value, Is.EqualTo(5));
        Assert.That(command.Parameters[1].Value, Is.EqualTo("sometext"));
        Assert.That(command.Parameters[2].Value, Is.EqualTo(9));
    }

    [Test, Description("Tests function parameter derivation with IN-only parameters")]
    public async Task DeriveParameters_procedure_in_only()
    {
        await using var conn = await OpenConnectionAsync();
        var sproc = await GetTempProcedureName(conn);

        await conn.ExecuteNonQueryAsync($@"CREATE PROCEDURE {sproc}(IN param1 INT, IN param2 INT) AS '' LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
        cmd.Parameters[0].Value = 5;
        cmd.Parameters[1].Value = 4;
        Assert.DoesNotThrowAsync(() => cmd.ExecuteNonQueryAsync());
    }

    [Test, Description("Tests function parameter derivation with no parameters")]
    public async Task DeriveParameters_procedure_no_params()
    {
        await using var conn = await OpenConnectionAsync();
        var sproc = await GetTempProcedureName(conn);

        await conn.ExecuteNonQueryAsync($@"CREATE PROCEDURE {sproc}() AS '' LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Is.Empty);
    }

    [Test]
    public async Task DeriveParameters_procedure_with_case_sensitive_name()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync(@"CREATE OR REPLACE PROCEDURE ""ProcedureCaseSensitive""(int4, text) AS '' LANGUAGE sql");

        try
        {
            await using var command = new NpgsqlCommand(@"""ProcedureCaseSensitive""", conn) { CommandType = CommandType.StoredProcedure };
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync(@"DROP PROCEDURE ""ProcedureCaseSensitive""");
        }
    }

    [Test, Description("Tests function parameter derivation for quoted functions with double quotes in the name works")]
    public async Task DeriveParameters_quote_characters_in_function_name()
    {
        await using var conn = await OpenConnectionAsync();
        var sproc = @"""""""ProcedureQuote""""CharactersInName""""""";
        await conn.ExecuteNonQueryAsync($"CREATE OR REPLACE PROCEDURE {sproc}(int4, text) AS 'SELECT 0' LANGUAGE sql");

        try
        {
            await using var command = new NpgsqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync("DROP PROCEDURE " + sproc);
        }
    }

    [Test, Description("Tests function parameter derivation for quoted functions with dots in the name works")]
    public async Task DeriveParameters_dot_character_in_function_name()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync(
            @"CREATE OR REPLACE PROCEDURE ""My.Dotted.Procedure""(int4, text) AS 'SELECT 0' LANGUAGE sql");

        try
        {
            await using var command = new NpgsqlCommand(@"""My.Dotted.Procedure""", conn) { CommandType = CommandType.StoredProcedure };
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync(@"DROP PROCEDURE ""My.Dotted.Procedure""");
        }
    }

    [Test]
    public async Task DeriveParameters_parameter_name_from_function()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "14.0", "Stored procedure OUT parameters are only support starting with version 14");
        var sproc = await GetTempProcedureName(conn);

        await conn.ExecuteNonQueryAsync(
            $"CREATE PROCEDURE {sproc}(x int, y int, out sum int, out product int) AS 'SELECT $1 + $2, $1 * $2' LANGUAGE sql");
        await using var command = new NpgsqlCommand(sproc, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(command);
        Assert.AreEqual("x", command.Parameters[0].ParameterName);
        Assert.AreEqual("y", command.Parameters[1].ParameterName);
    }

    [Test]
    public async Task DeriveParameters_non_existing_procedure()
    {
        await using var conn = await OpenConnectionAsync();
        var invalidCommandName = new NpgsqlCommand("invalidprocedurename", conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(invalidCommandName),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedFunction));
    }

    [Test, Description("Tests if the right function according to search_path is used in function parameter derivation")]
    public async Task DeriveParameters_procedure_correct_schema_resolution()
    {
        await using var conn = await OpenConnectionAsync();
        var schema1 = await CreateTempSchema(conn);
        var schema2 = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE PROCEDURE {schema1}.redundantsproc() AS 'SELECT 1' LANGUAGE sql;
CREATE PROCEDURE {schema2}.redundantsproc(IN param1 INT, IN param2 INT) AS 'SELECT param1 + param2' LANGUAGE sql;
SET search_path TO {schema2};");
        await using var command = new NpgsqlCommand("redundantsproc", conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(command);
        Assert.That(command.Parameters, Has.Count.EqualTo(2));
        Assert.That(command.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(command.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
    }

    [Test, Description("Tests if function parameter derivation throws an exception if the specified function is not in the search_path")]
    public async Task DeriveParameters_throws_for_existing_procedure_that_is_not_in_search_path()
    {
        await using var conn = await OpenConnectionAsync();
        var schema = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE PROCEDURE {schema}.schema1sproc() AS 'SELECT 1' LANGUAGE sql;
RESET search_path;");
        await using var command = new NpgsqlCommand("schema1sproc", conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(command),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedFunction));
    }

    [Test, Description("Tests if an exception is thrown if multiple functions with the specified name are in the search_path")]
    public async Task DeriveParameters_throws_for_multiple_procedures_name_hits_in_search_path()
    {
        await using var conn = await OpenConnectionAsync();
        var schema1 = await CreateTempSchema(conn);
        var schema2 = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync(
            $@"
CREATE PROCEDURE {schema1}.redundantsproc() AS 'SELECT 1' LANGUAGE sql;
CREATE PROCEDURE {schema1}.redundantsproc(IN param1 INT, IN param2 INT) AS 'SELECT param1 + param2' LANGUAGE sql;
SET search_path TO {schema1}, {schema2};");
        var command = new NpgsqlCommand("redundantsproc", conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(command),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.AmbiguousFunction));
    }

    #endregion DeriveParameters

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "11.0", "Stored procedures were introduced in PostgreSQL 11");
    }
}
