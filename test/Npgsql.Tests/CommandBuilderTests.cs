using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

class CommandBuilderTests : TestBase
{
    // See function parameter derivation tests in FunctionTests, and stored procedure derivation tests in StoredProcedureTests

    [Test, Description("Tests parameter derivation for parameterized queries (CommandType.Text)")]
    public async Task DeriveParameters_text_one_parameter_with_same_type()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "id int, val text");

        var cmd = new NpgsqlCommand(
            $@"INSERT INTO {table} VALUES(:x, 'some value');
                    UPDATE {table} SET val = 'changed value' WHERE id = :x;
                    SELECT val FROM {table} WHERE id = :x;",
            conn);
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
        Assert.That(cmd.Parameters[0].Direction, Is.EqualTo(ParameterDirection.Input));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("x"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        cmd.Parameters[0].Value = 42;
        var retVal = await cmd.ExecuteScalarAsync();
        Assert.That(retVal, Is.EqualTo("changed value"));
    }

    [Test, Description("Tests parameter derivation for parameterized queries (CommandType.Text) where different types would be inferred for placeholders with the same name.")]
    public async Task DeriveParameters_text_one_parameter_with_different_types()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "id int, val text");

        var cmd = new NpgsqlCommand(
            $@"INSERT INTO {table} VALUES(:x, 'some value');
                    UPDATE {table} SET val = 'changed value' WHERE id = :x::double precision;
                    SELECT val FROM {table} WHERE id = :x::numeric;",
            conn);
        var ex = Assert.Throws<NpgsqlException>(() => NpgsqlCommandBuilder.DeriveParameters(cmd))!;
        Assert.That(ex.Message, Is.EqualTo("The backend parser inferred different types for parameters with the same name. Please try explicit casting within your SQL statement or batch or use different placeholder names."));
    }

    [Test, Description("Tests parameter derivation for parameterized queries (CommandType.Text) with multiple parameters")]
    public async Task DeriveParameters_multiple_parameters()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "id int, val text");

        var cmd = new NpgsqlCommand(
            $@"INSERT INTO {table} VALUES(:x, 'some value');
                    UPDATE {table} SET val = 'changed value' WHERE id = @y::double precision;
                    SELECT val FROM {table} WHERE id = :z::numeric;",
            conn);
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(3));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("x"));
        Assert.That(cmd.Parameters[1].ParameterName, Is.EqualTo("y"));
        Assert.That(cmd.Parameters[2].ParameterName, Is.EqualTo("z"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Double));
        Assert.That(cmd.Parameters[2].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Numeric));

        cmd.Parameters[0].Value = 42;
        cmd.Parameters[1].Value = 42d;
        cmd.Parameters[2].Value = 42;
        var retVal = await cmd.ExecuteScalarAsync();
        Assert.That(retVal, Is.EqualTo("changed value"));
    }

    [Test, Description("Tests parameter derivation a parameterized query (CommandType.Text) that is already prepared.")]
    public async Task DeriveParameters_text_prepared_statement()
    {
        const string query = "SELECT @p::integer";
        const int answer = 42;
        using var _ = CreateTempPool(ConnectionString, out var connString);
        using var conn = await OpenConnectionAsync(connString);
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@p", NpgsqlDbType.Integer, answer);
        cmd.Prepare();
        Assert.That(conn.Connector!.PreparedStatementManager.NumPrepared, Is.EqualTo(1));

        var ex = Assert.Throws<NpgsqlException>(() =>
        {
            // Derive parameters for the already prepared statement
            NpgsqlCommandBuilder.DeriveParameters(cmd);

        })!;

        Assert.That(ex.Message, Is.EqualTo("Deriving parameters isn't supported for commands that are already prepared."));

        // We leave the command intact when throwing so it should still be useable
        Assert.That(cmd.Parameters.Count, Is.EqualTo(1));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("@p"));
        Assert.That(conn.Connector.PreparedStatementManager.NumPrepared, Is.EqualTo(1));
        cmd.Parameters["@p"].Value = answer;
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(answer));

        conn.UnprepareAll();
    }

    [Test, Description("Tests parameter derivation for array parameters in parameterized queries (CommandType.Text)")]
    public async Task DeriveParameters_text_array()
    {
        using var conn = await OpenConnectionAsync();
        var cmd = new NpgsqlCommand("SELECT :a::integer[]", conn);
        var val = new[] { 7, 42 };

        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("a"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer | NpgsqlDbType.Array));
        cmd.Parameters[0].Value = val;
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult | CommandBehavior.SingleRow);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(val));
    }

    [Test, Description("Tests parameter derivation for domain parameters in parameterized queries (CommandType.Text)")]
    public async Task DeriveParameters_text_domain()
    {
        using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "11.0", "Arrays of domains and domains over arrays were introduced in PostgreSQL 11");
        var domainType = await GetTempTypeName(conn);
        var domainArrayType = await GetTempTypeName(conn);
        await conn.ExecuteNonQueryAsync($@"
CREATE DOMAIN {domainType} AS integer CHECK (VALUE > 0);
CREATE DOMAIN {domainArrayType} AS int[] CHECK(array_length(VALUE, 1) = 2);");
        conn.ReloadTypes();

        var cmd = new NpgsqlCommand($"SELECT :a::{domainType}, :b::{domainType}[], :c::{domainArrayType}", conn);
        var val = 23;
        var arrayVal = new[] { 7, 42 };

        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(3));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("a"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(cmd.Parameters[0].DataTypeName, Does.EndWith(domainType));
        Assert.That(cmd.Parameters[1].ParameterName, Is.EqualTo("b"));
        Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer | NpgsqlDbType.Array));
        Assert.That(cmd.Parameters[1].DataTypeName, Does.EndWith(domainType + "[]"));
        Assert.That(cmd.Parameters[2].ParameterName, Is.EqualTo("c"));
        Assert.That(cmd.Parameters[2].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer | NpgsqlDbType.Array));
        Assert.That(cmd.Parameters[2].DataTypeName, Does.EndWith(domainArrayType));
        cmd.Parameters[0].Value = val;
        cmd.Parameters[1].Value = arrayVal;
        cmd.Parameters[2].Value = arrayVal;
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetFieldValue<int>(0), Is.EqualTo(val));
        Assert.That(reader.GetFieldValue<int[]>(1), Is.EqualTo(arrayVal));
        Assert.That(reader.GetFieldValue<int[]>(2), Is.EqualTo(arrayVal));
    }

    [Test, Description("Tests parameter derivation for unmapped enum parameters in parameterized queries (CommandType.Text)")]
    public async Task DeriveParameters_text_unmapped_enum()
    {
        using var conn = await OpenConnectionAsync();
        var type = await GetTempTypeName(conn);
        await conn.ExecuteNonQueryAsync($@"CREATE TYPE {type} AS ENUM ('Apple', 'Cherry', 'Plum')");
        conn.ReloadTypes();

        var cmd = new NpgsqlCommand($"SELECT :x::{type}", conn);
        const string val1 = "Apple";
        var val2 = new string[] { "Cherry", "Plum" };

        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(1));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("x"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
        Assert.That(cmd.Parameters[0].PostgresType, Is.InstanceOf<PostgresEnumType>());
        Assert.That(cmd.Parameters[0].PostgresType!.Name, Is.EqualTo(type));
        Assert.That(cmd.Parameters[0].DataTypeName, Does.EndWith(type));
        cmd.Parameters[0].Value = val1;
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult | CommandBehavior.SingleRow);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetString(0), Is.EqualTo(val1));
    }

    enum Fruit { Apple, Cherry, Plum }

    [Test, Description("Tests parameter derivation for mapped enum parameters in parameterized queries (CommandType.Text)")]
    public async Task DeriveParameters_text_mapped_enum()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($@"CREATE TYPE {type} AS ENUM ('apple', 'cherry', 'plum')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Fruit>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var cmd = new NpgsqlCommand($"SELECT :x::{type}, :y::{type}[]", connection);
        const Fruit val1 = Fruit.Apple;
        var val2 = new[] { Fruit.Cherry, Fruit.Plum };

        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("x"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
        Assert.That(cmd.Parameters[0].PostgresType, Is.InstanceOf<PostgresEnumType>());
        Assert.That(cmd.Parameters[0].DataTypeName, Does.EndWith(type));
        Assert.That(cmd.Parameters[1].ParameterName, Is.EqualTo("y"));
        Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
        Assert.That(cmd.Parameters[1].PostgresType, Is.InstanceOf<PostgresArrayType>());
        Assert.That(cmd.Parameters[1].DataTypeName, Does.EndWith(type + "[]"));
        cmd.Parameters[0].Value = val1;
        cmd.Parameters[1].Value = val2;
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult | CommandBehavior.SingleRow);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetFieldValue<Fruit>(0), Is.EqualTo(val1));
        Assert.That(reader.GetFieldValue<Fruit[]>(1), Is.EqualTo(val2));
    }

    class SomeComposite
    {
        public int X { get; set; }

        [PgName("some_text")]
        public string SomeText { get; set; } = "";
    }

    [Test]
    public async Task DeriveParameters_text_mapped_composite()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (x int, some_text text)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var expected1 = new SomeComposite { X = 8, SomeText = "foo" };
        var expected2 = new[] { expected1, new SomeComposite {X = 9, SomeText = "bar"} };

        await using var cmd = new NpgsqlCommand($"SELECT @p1::{type}, @p2::{type}[]", connection);
        NpgsqlCommandBuilder.DeriveParameters(cmd);
        Assert.That(cmd.Parameters, Has.Count.EqualTo(2));
        Assert.That(cmd.Parameters[0].ParameterName, Is.EqualTo("p1"));
        Assert.That(cmd.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
        Assert.That(cmd.Parameters[0].PostgresType, Is.InstanceOf<PostgresCompositeType>());
        Assert.That(cmd.Parameters[0].DataTypeName, Does.EndWith(type));
        var p1Fields = ((PostgresCompositeType)cmd.Parameters[0].PostgresType!).Fields;
        Assert.That(p1Fields[0].Name, Is.EqualTo("x"));
        Assert.That(p1Fields[1].Name, Is.EqualTo("some_text"));

        Assert.That(cmd.Parameters[1].ParameterName, Is.EqualTo("p2"));
        Assert.That(cmd.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown));
        Assert.That(cmd.Parameters[1].PostgresType, Is.InstanceOf<PostgresArrayType>());
        Assert.That(cmd.Parameters[1].DataTypeName, Does.EndWith(type + "[]"));
        var p2Element = ((PostgresArrayType)cmd.Parameters[1].PostgresType!).Element;
        Assert.That(p2Element, Is.InstanceOf<PostgresCompositeType>());
        Assert.That(p2Element.Name, Is.EqualTo(type));
        var p2Fields = ((PostgresCompositeType)p2Element).Fields;
        Assert.That(p2Fields[0].Name, Is.EqualTo("x"));
        Assert.That(p2Fields[1].Name, Is.EqualTo("some_text"));

        cmd.Parameters[0].Value = expected1;
        cmd.Parameters[1].Value = expected2;
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleResult | CommandBehavior.SingleRow);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetFieldValue<SomeComposite>(0).SomeText, Is.EqualTo(expected1.SomeText));
        Assert.That(reader.GetFieldValue<SomeComposite>(0).X, Is.EqualTo(expected1.X));
        for (var i = 0; i < 2; i++)
        {
            Assert.That(reader.GetFieldValue<SomeComposite[]>(1)[i].SomeText, Is.EqualTo(expected2[i].SomeText));
            Assert.That(reader.GetFieldValue<SomeComposite[]>(1)[i].X, Is.EqualTo(expected2[i].X));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1591")]
    public async Task Get_update_command_infers_parameters_with_NpgsqDbType()
    {
        using var conn = await OpenConnectionAsync();
        var table = await GetTempTableName(conn);
        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (
    Cod varchar(5) NOT NULL,
    Descr varchar(40),
    Data date,
    DataOra timestamp,
    Intero smallInt NOT NULL,
    Decimale money,
    Singolo float,
    Booleano bit,
    Nota varchar(255),
    BigIntArr bigint[],
    VarCharArr character varying(20)[],
    PRIMARY KEY (Cod)
);
INSERT INTO {table} VALUES('key1', 'description', '2018-07-03', '2018-07-03 07:02:00', 123, 123.4, 1234.5, B'1', 'note')");

        var daDataAdapter =
            new NpgsqlDataAdapter(
                $"SELECT Cod, Descr, Data, DataOra, Intero, Decimale, Singolo, Booleano, Nota, BigIntArr, VarCharArr FROM {table}", conn);

        var cbCommandBuilder = new NpgsqlCommandBuilder(daDataAdapter);
        var dtTable = new DataTable();

        daDataAdapter.InsertCommand = cbCommandBuilder.GetInsertCommand();
        daDataAdapter.UpdateCommand = cbCommandBuilder.GetUpdateCommand();
        daDataAdapter.DeleteCommand = cbCommandBuilder.GetDeleteCommand();

        Assert.That(daDataAdapter.UpdateCommand.Parameters[0].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[1].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[2].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Date));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[3].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[4].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Smallint));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[5].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Money));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[6].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Double));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[7].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bit));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[8].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[9].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Bigint));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[10].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Varchar));

        Assert.That(daDataAdapter.UpdateCommand.Parameters[11].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[13].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[15].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Date));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[17].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[18].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Smallint));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[20].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Money));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[22].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Double));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[24].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bit));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[26].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[28].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Bigint));
        Assert.That(daDataAdapter.UpdateCommand.Parameters[30].NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Varchar));

        daDataAdapter.Fill(dtTable);

        var row = dtTable.Rows[0];

        Assert.That(row[0], Is.EqualTo("key1"));
        Assert.That(row[1], Is.EqualTo("description"));
        Assert.That(row[2], Is.EqualTo(new DateTime(2018, 7, 3)));
        Assert.That(row[3], Is.EqualTo(new DateTime(2018, 7, 3, 7, 2, 0)));
        Assert.That(row[4], Is.EqualTo(123));
        Assert.That(row[5], Is.EqualTo(123.4));
        Assert.That(row[6], Is.EqualTo(1234.5));
        Assert.That(row[7], Is.EqualTo(true));
        Assert.That(row[8], Is.EqualTo("note"));

        dtTable.Rows[0]["Singolo"] = 1.1D;

        Assert.That(daDataAdapter.Update(dtTable), Is.EqualTo(1));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2560")]
    public async Task Get_update_command_with_column_aliases()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "Cod varchar(5) PRIMARY KEY, Descr varchar(40), Data date");
        using var cmd = new NpgsqlCommand($"SELECT Cod as CodAlias, Descr as DescrAlias, Data as DataAlias FROM {table}", conn);
        using var daDataAdapter = new NpgsqlDataAdapter(cmd);
        using var cbCommandBuilder = new NpgsqlCommandBuilder(daDataAdapter);

        daDataAdapter.UpdateCommand = cbCommandBuilder.GetUpdateCommand();
        Assert.True(daDataAdapter.UpdateCommand.CommandText.Contains("SET \"cod\" = @p1, \"descr\" = @p2, \"data\" = @p3 WHERE ((\"cod\" = @p4) AND ((@p5 = 1 AND \"descr\" IS NULL) OR (\"descr\" = @p6)) AND ((@p7 = 1 AND \"data\" IS NULL) OR (\"data\" = @p8)))"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2846")]
    public async Task Get_update_command_with_array_column_type()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "Cod varchar(5) PRIMARY KEY, Vettore character varying(20)[]");
        using var daDataAdapter = new NpgsqlDataAdapter($"SELECT cod, vettore FROM {table} ORDER By cod", conn);
        using var cbCommandBuilder = new NpgsqlCommandBuilder(daDataAdapter);
        var dtTable = new DataTable();

        cbCommandBuilder.SetAllValues = true;

        daDataAdapter.UpdateCommand = cbCommandBuilder.GetUpdateCommand();

        daDataAdapter.Fill(dtTable);
        dtTable.Rows.Add();
        dtTable.Rows[0]["cod"] = '0';
        dtTable.Rows[0]["vettore"] = new[] { "aaa", "bbb" };

        daDataAdapter.Update(dtTable);
    }
}
