using System;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests;

public class CommandParameterTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    [TestCase(CommandBehavior.Default)]
    [TestCase(CommandBehavior.SequentialAccess)]
    public async Task Input_and_output_parameters(CommandBehavior behavior)
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @c-1 AS c, @a+2 AS b", conn);
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
        using var cmd = new NpgsqlCommand("SELECT @p::TIMESTAMP", conn);
        cmd.CommandText = "SELECT @p::TIMESTAMP";
        cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Unknown) { Value = "2008-1-1" });
        if (prepare == PrepareOrNot.Prepared)
            cmd.Prepare();
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(new DateTime(2008, 1, 1)));
    }

    [Test]
    public async Task Positional_parameter()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(8));
    }

    [Test]
    public async Task Positional_parameters_are_not_supported_with_legacy_batching()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1; SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.SyntaxError));
    }

    [Test]
    public async Task Unreferenced_named_parameter_works()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Parameters.AddWithValue("not_used", 8);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Unreferenced_positional_parameter_works()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Mixing_positional_and_named_parameters_is_not_supported()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1, @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter { Value = 8 });
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = 9 });
        Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception.TypeOf<NotSupportedException>());
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
        await using var cmd = new NpgsqlCommand("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter { Value = 8, Direction = ParameterDirection.InputOutput });
        Assert.That(() => cmd.ExecuteNonQueryAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test]
    public void Parameters_get_name()
    {
        var command = new NpgsqlCommand();

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
        using var cmd = new NpgsqlCommand("SELECT @p1, @p1", conn);
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
        using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
        cmd.Parameters.Add(new NpgsqlParameter<int>("p1", 8));
        cmd.Parameters.Add(new NpgsqlParameter<short>("p2", 8) { NpgsqlDbType = NpgsqlDbType.Integer });
        cmd.Parameters.Add(new NpgsqlParameter<string>("p3", "hello"));
        cmd.Parameters.Add(new NpgsqlParameter<char[]>("p4", ['f', 'o', 'o']));
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetInt32(0), Is.EqualTo(8));
        Assert.That(reader.GetInt32(1), Is.EqualTo(8));
        Assert.That(reader.GetString(2), Is.EqualTo("hello"));
        Assert.That(reader.GetString(3), Is.EqualTo("foo"));
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task Parameter_must_be_set(bool genericParam)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1::TEXT", conn);
        cmd.Parameters.Add(
            genericParam
                ? new NpgsqlParameter<object?>("p1", null)
                : new NpgsqlParameter("p1", null)
        );

        Assert.That(async () => await cmd.ExecuteReaderAsync(),
            Throws.Exception
                .TypeOf<InvalidOperationException>()
                .With.Message.EqualTo("Parameter 'p1' must have either its NpgsqlDbType or its DataTypeName or its Value set."));
    }

    [Test]
    public async Task Object_generic_param_does_runtime_lookup()
    {
        await AssertTypeWrite<object>(1, "1", "integer", NpgsqlDbType.Integer, DbType.Int32, DbType.Int32, isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: true, skipArrayCheck: true);
        await AssertTypeWrite<object>(new[] {1, 1}, "{1,1}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array, isDefault: false,
            isNpgsqlDbTypeInferredFromClrType: true, skipArrayCheck: true);
    }

    [Test]
    public async Task Object_generic_parameter_works()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT $1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<object> { NpgsqlDbType = NpgsqlDbType.Integer, Value = 8 });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(8));
    }
}
