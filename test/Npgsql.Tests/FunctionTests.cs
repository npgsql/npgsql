using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Util.Statics;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

/// <summary>
/// A fixture for tests which interact with functions.
/// All tests should create functions in the pg_temp schema only to ensure there's no interaction between
/// the tests.
/// </summary>
[NonParallelizable] // Manipulates the EnableStoredProcedureCompatMode global flag
public class FunctionTests : TestBase
{
    [Test, Description("Simple function with no parameters, results accessed as a resultset")]
    public async Task Resultset()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE FUNCTION {function}() RETURNS integer AS 'SELECT 8' LANGUAGE sql");
        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(8));
    }

    [Test, Description("Basic function call with an in parameter")]
    public async Task Param_Input()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE FUNCTION {function}(IN param text) RETURNS text AS 'SELECT param' LANGUAGE sql");
        await using var cmd = new NpgsqlCommand(function, conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@param", "hello");
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo("hello"));
    }

    [Test, Description("Basic function call with an out parameter")]
    public async Task Param_Output()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);
        await conn.ExecuteNonQueryAsync(@$"
CREATE FUNCTION {function} (IN param_in text, OUT param_out text) AS $$
BEGIN
    param_out=param_in;
END
$$ LANGUAGE plpgsql");
        await using var cmd = new NpgsqlCommand(function, conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@param_in", "hello");
        var outParam = new NpgsqlParameter("param_out", DbType.String) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(outParam);
        await cmd.ExecuteNonQueryAsync();
        Assert.That(outParam.Value, Is.EqualTo("hello"));
    }

    [Test, Description("Basic function call with an in/out parameter")]
    public async Task Param_InputOutput()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);
        await conn.ExecuteNonQueryAsync($@"
CREATE FUNCTION {function} (INOUT param integer) AS $$
BEGIN
    param=param+1;
END
$$ LANGUAGE plpgsql");
        await using var cmd = new NpgsqlCommand(function, conn);
        cmd.CommandType = CommandType.StoredProcedure;
        var outParam = new NpgsqlParameter("param", DbType.Int32)
        {
            Direction = ParameterDirection.InputOutput,
            Value = 8
        };
        cmd.Parameters.Add(outParam);
        await cmd.ExecuteNonQueryAsync();
        Assert.That(outParam.Value, Is.EqualTo(9));
    }

    [Test]
    public async Task Void()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.1.0", "no binary output function available for type void before 9.1.0");
        var command = new NpgsqlCommand("pg_sleep", conn);
        command.Parameters.AddWithValue(0);
        command.CommandType = CommandType.StoredProcedure;
        await command.ExecuteNonQueryAsync();
    }

    [Test]
    public async Task Named_parameters()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.4.0", "make_timestamp was introduced in 9.4");
        await using var command = new NpgsqlCommand("make_timestamp", conn);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("year", 2015);
        command.Parameters.AddWithValue("month", 8);
        command.Parameters.AddWithValue("mday", 1);
        command.Parameters.AddWithValue("hour", 2);
        command.Parameters.AddWithValue("min", 3);
        command.Parameters.AddWithValue("sec", 4);
        var dt = (DateTime)(await command.ExecuteScalarAsync())!;

        Assert.AreEqual(new DateTime(2015, 8, 1, 2, 3, 4), dt);

        command.Parameters[0].Value = 2014;
        command.Parameters[0].ParameterName = ""; // 2014 will be sent as a positional parameter
        dt = (DateTime)(await command.ExecuteScalarAsync())!;
        Assert.AreEqual(new DateTime(2014, 8, 1, 2, 3, 4), dt);
    }

    [Test]
    public async Task Too_many_output_params()
    {
        await using var conn = await OpenConnectionAsync();
        var command = new NpgsqlCommand("VALUES (4,5), (6,7)", conn);
        command.Parameters.Add(new NpgsqlParameter("a", DbType.Int32)
        {
            Direction = ParameterDirection.Output,
            Value = -1
        });
        command.Parameters.Add(new NpgsqlParameter("b", DbType.Int32)
        {
            Direction = ParameterDirection.Output,
            Value = -1
        });
        command.Parameters.Add(new NpgsqlParameter("c", DbType.Int32)
        {
            Direction = ParameterDirection.Output,
            Value = -1
        });

        await command.ExecuteNonQueryAsync();

        Assert.That(command.Parameters["a"].Value, Is.EqualTo(4));
        Assert.That(command.Parameters["b"].Value, Is.EqualTo(5));
        Assert.That(command.Parameters["c"].Value, Is.EqualTo(-1));
    }

    [Test]
    public async Task CommandBehavior_SchemaOnly_support_function_call()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);

        await conn.ExecuteNonQueryAsync($"CREATE OR REPLACE FUNCTION {function}() RETURNS SETOF integer as 'SELECT 1;' LANGUAGE 'sql';");
        var command = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        await using var dr = await command.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var i = 0;
        while (dr.Read())
            i++;
        Assert.AreEqual(0, i);
    }

    #region DeriveParameters

    [Test, Description("Tests function parameter derivation with IN, OUT and INOUT parameters")]
    public async Task DeriveParameters_function_various()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);

        // This function returns record because of the two Out (InOut & Out) parameters
        await conn.ExecuteNonQueryAsync($@"
CREATE FUNCTION {function}(IN param1 INT, OUT param2 text, INOUT param3 INT) RETURNS record AS $$
BEGIN
    param2 = 'sometext';
    param3 = param1 + param3;
END;
$$ LANGUAGE plpgsql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(3));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(cmd.Parameters[0].PostgresType, Is.TypeOf<PostgresBaseType>());
        Assert.That(cmd.Parameters[0].DataTypeName, Is.EqualTo("integer"));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("param1"));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text));
        Assert.That(cmd.Parameters[1].PostgresType, Is.TypeOf<PostgresBaseType>());
        Assert.That(cmd.Parameters[1].DataTypeName, Is.EqualTo("text"));
        Assert.That(cmd.Parameters[1].ParameterName, Is.EqualTo("param2"));
        Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.InputOutput));
        Assert.That(cmd.Parameters[2].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(cmd.Parameters[2].PostgresType, Is.TypeOf<PostgresBaseType>());
        Assert.That(cmd.Parameters[2].DataTypeName, Is.EqualTo("integer"));
        Assert.That(cmd.Parameters[2].ParameterName, Is.EqualTo("param3"));
        cmd.Parameters[0].Value = 5;
        cmd.Parameters[2].Value = 4;
        await cmd.ExecuteNonQueryAsync();
        Assert.That(cmd.Parameters[0].Value, Is.EqualTo(5));
        Assert.That(cmd.Parameters[1].Value, Is.EqualTo("sometext"));
        Assert.That(cmd.Parameters[2].Value, Is.EqualTo(9));
    }

    [Test, Description("Tests function parameter derivation with IN-only parameters")]
    public async Task DeriveParameters_function_in_only()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);

        // This function returns record because of the two Out (InOut & Out) parameters
        await conn.ExecuteNonQueryAsync(
            $@"CREATE FUNCTION {function}(IN param1 INT, IN param2 INT) RETURNS int AS 'SELECT param1 + param2' LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
        cmd.Parameters[0].Value = 5;
        cmd.Parameters[1].Value = 4;
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(9));
    }

    [Test, Description("Tests function parameter derivation with no parameters")]
    public async Task DeriveParameters_function_no_params()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);

        await conn.ExecuteNonQueryAsync($@"CREATE FUNCTION {function}() RETURNS int AS 'SELECT 4' LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Is.Empty);
    }

    [Test]
    public async Task DeriveParameters_function_with_case_sensitive_name()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync(
            @"CREATE OR REPLACE FUNCTION ""FunctionCaseSensitive""(int4, text) RETURNS int4 AS 'SELECT 0' LANGUAGE sql");

        try
        {
            await using var command = new NpgsqlCommand(@"""FunctionCaseSensitive""", conn) { CommandType = CommandType.StoredProcedure };
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync(@"DROP FUNCTION ""FunctionCaseSensitive""");
        }
    }

    [Test, Description("Tests function parameter derivation for quoted functions with double quotes in the name works")]
    public async Task DeriveParameters_quote_characters_in_function_name()
    {
        await using var conn = await OpenConnectionAsync();
        var function = @"""""""FunctionQuote""""CharactersInName""""""";
        await conn.ExecuteNonQueryAsync($"CREATE OR REPLACE FUNCTION {function}(int4, text) RETURNS int4 AS 'SELECT 0' LANGUAGE sql");

        try
        {
            await using var command = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync("DROP FUNCTION " + function);
        }
    }

    [Test, Description("Tests function parameter derivation for quoted functions with dots in the name works")]
    public async Task DeriveParameters_dot_character_in_function_name()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync(
            @"CREATE OR REPLACE FUNCTION ""My.Dotted.Function""(int4, text) RETURNS int4 AS 'SELECT 0' LANGUAGE sql");

        try
        {
            await using var command = new NpgsqlCommand(@"""My.Dotted.Function""", conn) { CommandType = CommandType.StoredProcedure };
            NpgsqlCommandBuilder.DeriveParameters(command);
            Assert.AreEqual(NpgsqlDbType.Integer, command.Parameters[0].NpgsqlDbType);
            Assert.AreEqual(NpgsqlDbType.Text, command.Parameters[1].NpgsqlDbType);
        }
        finally
        {
            await conn.ExecuteNonQueryAsync(@"DROP FUNCTION ""My.Dotted.Function""");
        }
    }

    [Test]
    public async Task DeriveParameters_parameter_name_from_function()
    {
        await using var conn = await OpenConnectionAsync();
        var function = await GetTempFunctionName(conn);

        await conn.ExecuteNonQueryAsync(
            $"CREATE FUNCTION {function}(x int, y int, out sum int, out product int) AS 'SELECT $1 + $2, $1 * $2' LANGUAGE sql");
        await using var command = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(command);
        Assert.AreEqual("x", command.Parameters[0].ParameterName);
        Assert.AreEqual("y", command.Parameters[1].ParameterName);
    }

    [Test]
    public async Task DeriveParameters_non_existing_function()
    {
        await using var conn = await OpenConnectionAsync();
        var invalidCommandName = new NpgsqlCommand("invalidfunctionname", conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(invalidCommandName),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedFunction));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1212")]
    public async Task DeriveParameters_function_with_table_parameters()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.2.0");
        var function = await GetTempFunctionName(conn);

        // This function returns record because of the two Out (InOut & Out) parameters
        await conn.ExecuteNonQueryAsync(
            $"CREATE FUNCTION {function}(IN in1 INT) RETURNS TABLE(t1 INT, t2 INT) AS 'SELECT in1, in1+1' LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(3));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
        cmd.Parameters[0].Value = 5;
        await cmd.ExecuteNonQueryAsync();
        Assert.That(cmd.Parameters[1].Value, Is.EqualTo(5));
        Assert.That(cmd.Parameters[2].Value, Is.EqualTo(6));
    }

    [Test, Description("Tests if the right function according to search_path is used in function parameter derivation")]
    public async Task DeriveParameters_function_correct_schema_resolution()
    {
        await using var conn = await OpenConnectionAsync();
        var schema1 = await CreateTempSchema(conn);
        var schema2 = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE FUNCTION {schema1}.redundantfunc() RETURNS int AS 'SELECT 1' LANGUAGE sql;
CREATE FUNCTION {schema2}.redundantfunc(IN param1 INT, IN param2 INT) RETURNS int AS 'SELECT param1 + param2' LANGUAGE sql;
SET search_path TO {schema2};");
        await using var command = new NpgsqlCommand("redundantfunc", conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(command);
        Assert.That(command.Parameters, Has.Count.EqualTo(2));
        Assert.That(command.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(command.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Input));
        command.Parameters[0].Value = 5;
        command.Parameters[1].Value = 4;
        Assert.That(await command.ExecuteScalarAsync(), Is.EqualTo(9));
    }

    [Test, Description("Tests if function parameter derivation throws an exception if the specified function is not in the search_path")]
    public async Task DeriveParameters_throws_for_existing_function_that_is_not_in_search_path()
    {
        await using var conn = await OpenConnectionAsync();
        var schema = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE FUNCTION {schema}.schema1func() RETURNS int AS 'SELECT 1' LANGUAGE sql;
RESET search_path;");
        await using var command = new NpgsqlCommand("schema1func", conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(command),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedFunction));
    }

    [Test, Description("Tests if an exception is thrown if multiple functions with the specified name are in the search_path")]
    public async Task DeriveParameters_throws_for_multiple_function_name_hits_in_search_path()
    {
        await using var conn = await OpenConnectionAsync();
        var schema1 = await CreateTempSchema(conn);
        var schema2 = await CreateTempSchema(conn);

        await conn.ExecuteNonQueryAsync(
            $@"
CREATE FUNCTION {schema1}.redundantfunc() RETURNS int AS 'SELECT 1' LANGUAGE sql;
CREATE FUNCTION {schema1}.redundantfunc(IN param1 INT, IN param2 INT) RETURNS int AS 'SELECT param1 + param2' LANGUAGE sql;
SET search_path TO {schema1}, {schema2};");
        var command = new NpgsqlCommand("redundantfunc", conn) { CommandType = CommandType.StoredProcedure };
        Assert.That(() => NpgsqlCommandBuilder.DeriveParameters(command),
            Throws.Exception.TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.AmbiguousFunction));
    }

    #region Set returning functions

    [Test, Description("Tests parameter derivation for a function that returns SETOF sometype")]
    public async Task DeriveParameters_function_returning_setof_type()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.2.0");

        var table = await GetTempTableName(conn);
        var function = await GetTempFunctionName(conn);

        // This function returns record because of the two Out (InOut & Out) parameters
        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (fooid int, foosubid int, fooname text);
INSERT INTO {table} VALUES (1, 1, 'Joe'), (1, 2, 'Ed'), (2, 1, 'Mary');
CREATE FUNCTION {function}(int) RETURNS SETOF {table} AS $$
    SELECT * FROM {table} WHERE {table}.fooid = $1 ORDER BY {table}.foosubid;
$$ LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(4));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
        cmd.Parameters[0].Value = 1;
        await cmd.ExecuteNonQueryAsync();
        Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
    }

    [Test, Description("Tests parameter derivation for a function that returns TABLE")]
    public async Task DeriveParameters_function_returning_table()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.2.0");

        var table = await GetTempTableName(conn);
        var function = await GetTempFunctionName(conn);

        // This function returns record because of the two Out (InOut & Out) parameters
        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (fooid int, foosubid int, fooname text);
INSERT INTO {table} VALUES (1, 1, 'Joe'), (1, 2, 'Ed'), (2, 1, 'Mary');
CREATE FUNCTION {function}(int) RETURNS TABLE(fooid int, foosubid int, fooname text) AS $$
    SELECT * FROM {table} WHERE {table}.fooid = $1 ORDER BY {table}.foosubid;
$$ LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(4));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
        cmd.Parameters[0].Value = 1;
        await cmd.ExecuteNonQueryAsync();
        Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
    }

    [Test, Description("Tests parameter derivation for a function that returns SETOF record")]
    public async Task DeriveParameters_function_returning_setof_record()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.2.0");

        var table = await GetTempTableName(conn);
        var function = await GetTempFunctionName(conn);

        // This function returns record because of the two Out (InOut & Out) parameters
        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (fooid int, foosubid int, fooname text);
INSERT INTO {table} VALUES (1, 1, 'Joe'), (1, 2, 'Ed'), (2, 1, 'Mary');
CREATE FUNCTION {function}(int, OUT fooid int, OUT foosubid int, OUT fooname text) RETURNS SETOF record AS $$
    SELECT * FROM {table} WHERE {table}.fooid = $1 ORDER BY {table}.foosubid;
$$ LANGUAGE sql");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(4));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[2].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[3].Direction, Is.EqualTo(ParameterDirection.Output));
        cmd.Parameters[0].Value = 1;
        await cmd.ExecuteNonQueryAsync();
        Assert.That(cmd.Parameters[0].Value, Is.EqualTo(1));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2022")]
    public async Task DeriveParameters_function_returning_setof_type_with_dropped_column()
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "9.2.0");

        var table = await GetTempTableName(conn);
        var function = await GetTempFunctionName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (id serial PRIMARY KEY, t1 text, t2 text);
CREATE FUNCTION {function}() RETURNS SETOF {table} AS 'SELECT * FROM {table}' LANGUAGE sql;
ALTER TABLE {table} DROP t2;");

        await using var cmd = new NpgsqlCommand(function, conn) { CommandType = CommandType.StoredProcedure };
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(cmd.Parameters[1].Direction, Is.EqualTo(ParameterDirection.Output));
        Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text));
    }

    #endregion

    #endregion DeriveParameters

#if DEBUG
    [OneTimeSetUp]
    public void OneTimeSetup() => NpgsqlCommand.EnableStoredProcedureCompatMode = true;

    [OneTimeTearDown]
    public void OneTimeTeardown() => NpgsqlCommand.EnableStoredProcedureCompatMode = false;
#else
    [OneTimeSetUp]
    public void OneTimeSetup()
        => Assert.Ignore("Cannot test function invocation via CommandType.StoredProcedure since that depends on the global EnableStoredProcedureCompatMode compatibility flag");
#endif
}
