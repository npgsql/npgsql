using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

/// <summary>
/// This tests the new CoreCLR schema/metadata API, which returns ReadOnlyCollection&lt;DbColumn&gt;.
/// Note that this API is also available on .NET Framework.
/// For the old DataTable-based API, see <see cref="ReaderOldSchemaTests"/>.
/// </summary>
public class ReaderNewSchemaTests : SyncOrAsyncTestBase
{
    // ReSharper disable once InconsistentNaming
    [Test]
    public async Task Allow_DBNull()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "nullable INTEGER, non_nullable INTEGER NOT NULL");

        using var cmd = new NpgsqlCommandOrig($"SELECT nullable,non_nullable,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].AllowDBNull, Is.True);
        Assert.That(columns[1].AllowDBNull, Is.False);
        Assert.That(columns[2].AllowDBNull, Is.Null);
    }

    [Test]
    public async Task BaseCatalogName()
    {
        var dbName = new NpgsqlConnectionStringBuilder(ConnectionString).Database;
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].BaseCatalogName, Is.EqualTo(dbName));
        Assert.That(columns[1].BaseCatalogName, Is.EqualTo(dbName));
    }

    [Test]
    public async Task BaseColumnName()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo, foo AS foobar, 8 AS bar, 8, '8'::VARCHAR(10) FROM {table}", conn);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);

        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].BaseColumnName, Is.EqualTo("foo"));
        Assert.That(columns[1].BaseColumnName, Is.EqualTo("foo"));
        Assert.That(columns[2].BaseColumnName, Is.Null);
        Assert.That(columns[3].BaseColumnName, Is.Null);
        Assert.That(columns[4].BaseColumnName, Is.Null);
    }

    [Test]
    public async Task BaseColumnName_with_column_aliases()
    {
        using var conn = OpenConnection();

        conn.ExecuteNonQuery(@"
                CREATE TEMP TABLE data (
                    Cod varchar(5) NOT NULL,
                    Descr varchar(40),
                    Date date,
                    CONSTRAINT PK_test_Cod PRIMARY KEY (Cod)
                );
            ");

        var cmd = new NpgsqlCommandOrig("SELECT Cod as CodAlias, Descr as DescrAlias, Date, NULL AS Generated FROM data", conn);

        using var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var cols = await GetColumnSchema(dr);

        Assert.That(cols[0].BaseColumnName, Is.EqualTo("cod"));
        Assert.That(cols[0].ColumnName, Is.EqualTo("codalias"));
        Assert.That(cols[0].IsAliased, Is.True);

        Assert.That(cols[1].BaseColumnName, Is.EqualTo("descr"));
        Assert.That(cols[1].ColumnName, Is.EqualTo("descralias"));
        Assert.That(cols[1].IsAliased, Is.True);

        Assert.That(cols[2].BaseColumnName, Is.EqualTo("date"));
        Assert.That(cols[2].ColumnName, Is.EqualTo("date"));
        Assert.That(cols[2].IsAliased, Is.False);

        Assert.That(cols[3].BaseColumnName, Is.Null);
        Assert.That(cols[3].ColumnName, Is.EqualTo("generated"));
        Assert.That(cols[3].IsAliased, Is.Null);
    }

    [Test]
    public async Task BaseSchemaName()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].BaseSchemaName, Is.EqualTo("public"));
        Assert.That(columns[1].BaseSchemaName, Is.Null);
    }

    [Test]
    public async Task BaseServerName()
    {
        var host = new NpgsqlConnectionStringBuilder(ConnectionString).Host;
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].BaseServerName, Is.EqualTo(host));
        Assert.That(columns[1].BaseServerName, Is.EqualTo(host));
    }

    [Test]
    public async Task BaseTableName()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].BaseTableName, Does.StartWith("temp_table"));
        Assert.That(columns[1].BaseTableName, Is.Null);
    }

    [Test]
    public async Task ColumnName()
    {
        await using (var conn = await OpenConnectionAsync())
        {
            var table = await CreateTempTable(conn, "foo INTEGER");

            using var cmd = new NpgsqlCommandOrig($"SELECT foo, foo AS foobar, 8 AS bar, 8, '8'::VARCHAR(10) FROM {table}", conn);
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);

            var columns = await GetColumnSchema(reader);
            Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
            Assert.That(columns[1].ColumnName, Is.EqualTo("foobar"));
            Assert.That(columns[2].ColumnName, Is.EqualTo("bar"));
            Assert.That(columns[3].ColumnName, Is.EqualTo("?column?"));
            Assert.That(columns[4].ColumnName, Is.EqualTo("varchar"));
        }

        // See https://github.com/npgsql/npgsql/issues/1676
        using (var conn = await OpenConnectionAsync())
        {
            var table = await CreateTempTable(conn, "col TEXT");

            var behavior = CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
            //var behavior = CommandBehavior.SchemaOnly;
            using var command = new NpgsqlCommandOrig($"SELECT col AS col_alias FROM {table}", conn);
            using var reader = command.ExecuteReader(behavior);
            var columns = await GetColumnSchema(reader);
            Assert.That(columns[0].ColumnName, Is.EqualTo("col_alias"));
        }
    }

    [Test]
    public async Task ColumnOrdinal()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "first INTEGER, second INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT second,first FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].ColumnName, Is.EqualTo("second"));
        Assert.That(columns[0].ColumnOrdinal, Is.EqualTo(0));
        Assert.That(columns[1].ColumnName, Is.EqualTo("first"));
        Assert.That(columns[1].ColumnOrdinal, Is.EqualTo(1));
    }

    [Test]
    public async Task ColumnAttributeNumber()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "first INTEGER, second INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT second,first FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].ColumnName, Is.EqualTo("second"));
        Assert.That(columns[0].ColumnAttributeNumber, Is.EqualTo(2));
        Assert.That(columns[1].ColumnName, Is.EqualTo("first"));
        Assert.That(columns[1].ColumnAttributeNumber, Is.EqualTo(1));
    }

    [Test]
    public async Task ColumnSize()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Column size is never unlimited on Redshift");
        var table = await CreateTempTable(conn, "bounded VARCHAR(30), unbounded VARCHAR");

        using var cmd = new NpgsqlCommandOrig($"SELECT bounded,unbounded,'a'::VARCHAR(10),'b'::VARCHAR FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].ColumnSize, Is.EqualTo(30));
        Assert.That(columns[1].ColumnSize, Is.Null);
        Assert.That(columns[2].ColumnSize, Is.EqualTo(10));
        Assert.That(columns[3].ColumnSize, Is.Null);
    }

    [Test]
    public async Task IsAutoIncrement()
    {
        await using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Serial columns not support on Redshift");

        var table = await CreateTempTable(conn, "serial SERIAL, int INT");

        await using var cmd = new NpgsqlCommandOrig($"SELECT serial, int, 8 FROM {table}", conn);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsAutoIncrement, Is.True, "Serial not identified as autoincrement");
        Assert.That(columns[1].IsAutoIncrement, Is.False, "Regular int column identified as autoincrement");
        Assert.That(columns[2].IsAutoIncrement, Is.Null, "Literal int identified as autoincrement");
    }

    [Test]
    public async Task IsAutoIncrement_identity()
    {
        await using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Identity columns not support on Redshift");
        MinimumPgVersion(conn, "10.0", "IDENTITY introduced in PostgreSQL 10");

        var table =
            await CreateTempTable(conn, "identity1 INT GENERATED BY DEFAULT AS IDENTITY, identity2 INT GENERATED ALWAYS AS IDENTITY");

        await using var cmd = new NpgsqlCommandOrig($"SELECT identity1, identity2 FROM {table}", conn);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsAutoIncrement, Is.True, "PG 10 IDENTITY not identified as autoincrement");
        Assert.That(columns[1].IsAutoIncrement, Is.True, "PG 10 IDENTITY not identified as autoincrement");
    }

    [Test]
    public async Task IsIdentity()
    {
        await using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Identity columns not support on Redshift");
        MinimumPgVersion(conn, "10.0", "IDENTITY introduced in PostgreSQL 10");
        var table = await CreateTempTable(
            conn,
            "identity1 INT GENERATED BY DEFAULT AS IDENTITY, identity2 INT GENERATED ALWAYS AS IDENTITY, serial SERIAL, int INT");

        await using var cmd = new NpgsqlCommandOrig($"SELECT identity1, identity2, serial, int, 8 FROM {table}", conn);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsIdentity, Is.True, "PG 10 IDENTITY not identified as identity");
        Assert.That(columns[1].IsIdentity, Is.True, "PG 10 IDENTITY not identified as identity");
        Assert.That(columns[2].IsIdentity, Is.False, "Serial identified as identity");
        Assert.That(columns[3].IsIdentity, Is.False, "Regular int column identified as identity");
        Assert.That(columns[4].IsIdentity, Is.False, "Literal int identified as identity");
    }

    [Test]
    public async Task IsKey()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Key not supported in reader schema on Redshift");
        var table = await CreateTempTable(conn, "id INT PRIMARY KEY, non_id INT, uniq INT UNIQUE");

        using var cmd = new NpgsqlCommandOrig($"SELECT id,non_id,uniq,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsKey, Is.True);
        Assert.That(columns[1].IsKey, Is.False);

        // Note: according to the old API docs any unique column is considered key.
        // https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
        // But in the new API we have a separate IsUnique so IsKey should be false
        Assert.That(columns[2].IsKey, Is.False);

        Assert.That(columns[3].IsKey, Is.Null);
    }

    [Test]
    public async Task IsKey_composite()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Key not supported in reader schema on Redshift");
        var table = await CreateTempTable(conn, "id1 INT, id2 INT, PRIMARY KEY (id1, id2)");

        using var cmd = new NpgsqlCommandOrig($"SELECT id1,id2 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsKey, Is.True);
        Assert.That(columns[1].IsKey, Is.True);
    }

    [Test]
    public async Task IsLong()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "bytea not supported on Redshift");
        var table = await CreateTempTable(conn, "long BYTEA, non_long INT");

        using var cmd = new NpgsqlCommandOrig($"SELECT long, non_long, 8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsLong, Is.True);
        Assert.That(columns[1].IsLong, Is.False);
        Assert.That(columns[2].IsLong, Is.False);
    }

    [Test]
    public async Task IsReadOnly_on_view()
    {
        using var conn = await OpenConnectionAsync();
        var view = await GetTempViewName(conn);
        var table = await GetTempTableName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE VIEW {view} AS SELECT 8 AS foo;
CREATE TABLE {table} (bar INTEGER)");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,bar FROM {view},{table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsReadOnly, Is.True);
        Assert.That(columns[1].IsReadOnly, Is.False);
    }

    [Test]
    public async Task IsReadOnly_on_non_column()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("SELECT 8", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns.Single().IsReadOnly, Is.True);
    }

    [Test]
    public async Task IsUnique()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Unique not supported in reader schema on Redshift");
        var table = await GetTempTableName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (id INT PRIMARY KEY, non_id INT, uniq INT UNIQUE, non_id_second INT, non_id_third INT);
CREATE UNIQUE INDEX idx_{table} ON {table} (non_id_second, non_id_third)");

        using var cmd = new NpgsqlCommandOrig($"SELECT id,non_id,uniq,8,non_id_second,non_id_third FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsUnique, Is.True);
        Assert.That(columns[1].IsUnique, Is.False);
        Assert.That(columns[2].IsUnique, Is.True);
        Assert.That(columns[3].IsUnique, Is.Null);
        Assert.That(columns[4].IsUnique, Is.False);
        Assert.That(columns[5].IsUnique, Is.False);
    }

    [Test]
    public async Task NumericPrecision()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Precision is never unlimited on Redshift");
        var table = await CreateTempTable(conn, "a NUMERIC(8), b NUMERIC, c INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT a,b,c,8.3::NUMERIC(8) FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].NumericPrecision, Is.EqualTo(8));
        Assert.That(columns[1].NumericPrecision, Is.Null);
        Assert.That(columns[2].NumericPrecision, Is.Null);
        Assert.That(columns[3].NumericPrecision, Is.EqualTo(8));
    }

    [Test]
    public async Task NumericScale()
    {
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Scale is never unlimited on Redshift");
        var table = await CreateTempTable(conn, "a NUMERIC(8,5), b NUMERIC, c INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT a,b,c,8.3::NUMERIC(8,5) FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].NumericScale, Is.EqualTo(5));
        Assert.That(columns[1].NumericScale, Is.Null);
        Assert.That(columns[2].NumericScale, Is.Null);
        Assert.That(columns[3].NumericScale, Is.EqualTo(5));
    }

    [Test]
    public async Task DataType()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8::INTEGER FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].DataType, Is.SameAs(typeof(int)));
        Assert.That(columns[1].DataType, Is.SameAs(typeof(int)));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1305")]
    public async Task DataType_unknown_type()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo::INTEGER FROM {table}", conn);
        cmd.AllResultTypesAreUnknown = true;
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].DataType, Is.SameAs(typeof(int)));
    }

    [Test]
    public async Task DataType_with_composite()
    {
        await using var adminConnection = await OpenConnectionAsync();
        IgnoreOnRedshift(adminConnection, "Composite types not support on Redshift");
        var type = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS (foo int)");
        var tableName = await CreateTempTable(adminConnection, $"comp {type}");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommandOrig($"SELECT comp,'(4)'::{type} FROM {tableName}", connection);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].DataType, Is.SameAs(typeof(SomeComposite)));
        Assert.That(columns[0].UdtAssemblyQualifiedName, Is.EqualTo(typeof(SomeComposite).AssemblyQualifiedName));
        Assert.That(columns[1].DataType, Is.SameAs(typeof(SomeComposite)));
        Assert.That(columns[1].UdtAssemblyQualifiedName, Is.EqualTo(typeof(SomeComposite).AssemblyQualifiedName));
    }

    [Test]
    public async Task UdtAssemblyQualifiedName()
    {
        // Also see DataTypeWithComposite
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].UdtAssemblyQualifiedName, Is.Null);
        Assert.That(columns[1].UdtAssemblyQualifiedName, Is.Null);
    }

    [Test]
    public async Task PostgresType()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        var intType = columns[0].PostgresType;
        Assert.That(columns[1].PostgresType, Is.SameAs(intType));
        Assert.That(intType.Name, Is.EqualTo("integer"));
        Assert.That(intType.InternalName, Is.EqualTo("int4"));
    }

    [Test]
    public async Task ColumnSchema_with_and_without_KeyInfo()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo, foo AS foobar, 8 AS bar, 8, '8'::VARCHAR(10) FROM {table}", conn);
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            var columns = await GetColumnSchema(reader);

            Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
            Assert.That(columns[0].BaseColumnName, Is.Null);
            Assert.That(columns[0].BaseTableName, Is.Null);
            Assert.That(columns[0].BaseSchemaName, Is.Null);
            Assert.That(columns[0].IsAliased, Is.Null);
            Assert.That(columns[0].IsKey, Is.Null);
            Assert.That(columns[0].IsUnique, Is.Null);
            Assert.That(columns[1].ColumnName, Is.EqualTo("foobar"));
            Assert.That(columns[1].BaseColumnName, Is.Null);
            Assert.That(columns[1].BaseTableName, Is.Null);
            Assert.That(columns[1].BaseSchemaName, Is.Null);
            Assert.That(columns[1].IsAliased, Is.Null);
            Assert.That(columns[1].IsKey, Is.Null);
            Assert.That(columns[1].IsUnique, Is.Null);
            Assert.That(columns[2].ColumnName, Is.EqualTo("bar"));
            Assert.That(columns[2].BaseColumnName, Is.Null);
            Assert.That(columns[2].BaseTableName, Is.Null);
            Assert.That(columns[2].BaseSchemaName, Is.Null);
            Assert.That(columns[2].IsAliased, Is.Null);
            Assert.That(columns[2].IsKey, Is.Null);
            Assert.That(columns[2].IsUnique, Is.Null);
            Assert.That(columns[3].ColumnName, Is.EqualTo("?column?"));
            Assert.That(columns[3].BaseColumnName, Is.Null);
            Assert.That(columns[3].BaseTableName, Is.Null);
            Assert.That(columns[3].BaseSchemaName, Is.Null);
            Assert.That(columns[3].IsAliased, Is.Null);
            Assert.That(columns[3].IsKey, Is.Null);
            Assert.That(columns[3].IsUnique, Is.Null);
            Assert.That(columns[4].ColumnName, Is.EqualTo("varchar"));
            Assert.That(columns[4].BaseColumnName, Is.Null);
            Assert.That(columns[4].BaseTableName, Is.Null);
            Assert.That(columns[4].BaseSchemaName, Is.Null);
            Assert.That(columns[4].IsAliased, Is.Null);
            Assert.That(columns[4].IsKey, Is.Null);
            Assert.That(columns[4].IsUnique, Is.Null);

        }

        await using (var readerInfo = await cmd.ExecuteReaderAsync(CommandBehavior.KeyInfo))
        {
            var columnsInfo = await GetColumnSchema(readerInfo);

            Assert.That(columnsInfo[0].ColumnName, Is.EqualTo("foo"));
            Assert.That(columnsInfo[0].BaseColumnName, Is.EqualTo("foo"));
            Assert.That(columnsInfo[0].BaseSchemaName, Is.EqualTo("public"));
            Assert.That(columnsInfo[0].IsAliased, Is.EqualTo(false));
            Assert.That(columnsInfo[0].IsKey, Is.EqualTo(false));
            Assert.That(columnsInfo[0].IsUnique, Is.EqualTo(false));
            Assert.That(columnsInfo[1].ColumnName, Is.EqualTo("foobar"));
            Assert.That(columnsInfo[1].BaseColumnName, Is.EqualTo("foo"));
            Assert.That(columnsInfo[1].BaseSchemaName, Is.EqualTo("public"));
            Assert.That(columnsInfo[1].IsAliased, Is.EqualTo(true));
            Assert.That(columnsInfo[1].IsKey, Is.EqualTo(false));
            Assert.That(columnsInfo[1].IsUnique, Is.EqualTo(false));
            Assert.That(columnsInfo[2].ColumnName, Is.EqualTo("bar"));
            Assert.That(columnsInfo[2].BaseColumnName, Is.Null);
            Assert.That(columnsInfo[2].BaseSchemaName, Is.Null);
            Assert.That(columnsInfo[2].IsAliased, Is.Null);
            Assert.That(columnsInfo[2].IsKey, Is.Null);
            Assert.That(columnsInfo[2].IsUnique, Is.Null);
            Assert.That(columnsInfo[3].ColumnName, Is.EqualTo("?column?"));
            Assert.That(columnsInfo[3].BaseColumnName, Is.Null);
            Assert.That(columnsInfo[3].BaseSchemaName, Is.Null);
            Assert.That(columnsInfo[3].IsAliased, Is.Null);
            Assert.That(columnsInfo[3].IsKey, Is.Null);
            Assert.That(columnsInfo[3].IsUnique, Is.Null);
            Assert.That(columnsInfo[4].ColumnName, Is.EqualTo("varchar"));
            Assert.That(columnsInfo[4].BaseColumnName, Is.Null);
            Assert.That(columnsInfo[4].BaseSchemaName, Is.Null);
            Assert.That(columnsInfo[4].IsAliased, Is.Null);
            Assert.That(columnsInfo[4].IsKey, Is.Null);
            Assert.That(columnsInfo[4].IsUnique, Is.Null);
        }
    }

    /// <seealso cref="ReaderTests.GetDataTypeName"/>
    [Test]
    [TestCase("integer")]
    [TestCase("real")]
    [TestCase("integer[]")]
    [TestCase("character varying(10)", 10)]
    [TestCase("character varying")]
    [TestCase("character varying(10)[]", 10)]
    [TestCase("character(10)", 10)]
    [TestCase("character", 1)]
    [TestCase("character(1)", 1)]
    [TestCase("numeric(1000, 2)", null, 1000, 2)]
    [TestCase("numeric(1000)", null, 1000, null)]
    [TestCase("numeric")]
    [TestCase("timestamp without time zone")]
    [TestCase("timestamp(2) without time zone", null, 2)]
    [TestCase("timestamp(2) with time zone", null, 2)]
    [TestCase("time without time zone")]
    [TestCase("time(2) without time zone", null, 2)]
    [TestCase("time(2) with time zone", null, 2)]
    [TestCase("interval")]
    [TestCase("interval(2)", null, 2)]
    [TestCase("bit", 1)]
    [TestCase("bit(3)", 3)]
    [TestCase("bit varying")]
    [TestCase("bit varying(3)", 3)]
    public async Task DataTypeName(string typeName, int? size = null, int? precision = null, int? scale = null)
    {
        var openingParen = typeName.IndexOf('(');
        var typeNameWithoutFacets = openingParen == -1
            ? typeName
            : typeName.Substring(0, openingParen) + typeName.Substring(typeName.IndexOf(')') + 1);

        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, $"foo {typeName}");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,NULL::{typeName} FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        var tableColumn = columns[0];
        var nonTableColumn = columns[1];
        Assert.That(tableColumn.DataTypeName, Is.EqualTo(typeNameWithoutFacets));
        Assert.That(tableColumn.ColumnSize, Is.EqualTo(size));
        Assert.That(tableColumn.NumericPrecision, Is.EqualTo(precision));
        Assert.That(tableColumn.NumericScale, Is.EqualTo(scale));
        Assert.That(nonTableColumn.DataTypeName, Is.EqualTo(typeNameWithoutFacets));
        Assert.That(nonTableColumn.ColumnSize, Is.EqualTo(size));
        Assert.That(nonTableColumn.NumericPrecision, Is.EqualTo(precision));
        Assert.That(nonTableColumn.NumericScale, Is.EqualTo(scale));
    }

    [Test]
    public async Task DefaultValue()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "with_default INTEGER DEFAULT(8), without_default INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT with_default,without_default,8 FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].DefaultValue, Is.EqualTo("8"));
        Assert.That(columns[1].DefaultValue, Is.Null);
        Assert.That(columns[2].DefaultValue, Is.Null);
    }

    [Test]
    public async Task Same_column_name()
    {
        using var conn = await OpenConnectionAsync();
        var table1 = await GetTempTableName(conn);
        var table2 = await GetTempTableName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table1} (foo INTEGER);
CREATE TABLE {table2} (foo INTEGER)");

        using var cmd = new NpgsqlCommandOrig($"SELECT {table1}.foo,{table2}.foo FROM {table1},{table2}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
        Assert.That(columns[0].BaseTableName, Does.StartWith("temp_table"));
        Assert.That(columns[1].ColumnName, Is.EqualTo("foo"));
        Assert.That(columns[1].BaseTableName, Does.StartWith("temp_table"));
        Assert.That(columns[0].BaseTableName, Is.Not.EqualTo(columns[1].BaseTableName));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1553")]
    public async Task Domain_type()
    {
        // if (IsMultiplexing)
        //     Assert.Ignore("Multiplexing: ReloadTypes");
        using var conn = await OpenConnectionAsync();
        IgnoreOnRedshift(conn, "Domain types not support on Redshift");

        const string domainTypeName = "my_domain";
        var schema = await CreateTempSchema(conn);
        var tableName = await GetTempTableName(conn);
        await conn.ExecuteNonQueryAsync($"CREATE DOMAIN {schema}.{domainTypeName} AS varchar(2)");
        conn.ReloadTypes();
        await conn.ExecuteNonQueryAsync($"CREATE TABLE {tableName} (domain {schema}.{domainTypeName})");
        using var cmd = new NpgsqlCommandOrig($"SELECT domain FROM {tableName}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
        var columns = await GetColumnSchema(reader);
        var domainSchema = columns.Single(c => c.ColumnName == "domain");
        Assert.That(domainSchema.ColumnSize, Is.EqualTo(2));
        var pgType = domainSchema.PostgresType;
        Assert.That(pgType, Is.InstanceOf<PostgresDomainType>());
        Assert.That(((PostgresDomainType)pgType).BaseType.Name, Is.EqualTo("character varying"));
    }

    [Test]
    public async Task NpgsqlDbType()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo,8::INTEGER FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].NpgsqlDbType, Is.EqualTo(NpgsqlTypes.NpgsqlDbType.Integer));
        Assert.That(columns[1].NpgsqlDbType, Is.EqualTo(NpgsqlTypes.NpgsqlDbType.Integer));
    }

    [Test]
    [NonParallelizable]
    public async Task NpgsqlDbType_extension()
    {
        using var conn = await OpenConnectionAsync();
        await EnsureExtensionAsync(conn, "hstore", "9.1");

        using var cmd = new NpgsqlCommandOrig("SELECT NULL::HSTORE", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        var columns = await GetColumnSchema(reader);
        // The full datatype name for PostGIS is public.geometry (unlike int4 which is in pg_catalog).
        Assert.That(columns[0].NpgsqlDbType, Is.EqualTo(NpgsqlTypes.NpgsqlDbType.Hstore));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1950")]
    public async Task No_resultset()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommandOrig("COMMIT", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        await GetColumnSchema(reader);
    }

    [Test]
    public async Task IsAliased()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo, foo AS bar, NULL AS foobar FROM {table}", conn);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);

        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].IsAliased, Is.False);
        Assert.That(columns[1].IsAliased, Is.True);
        Assert.That(columns[2].IsAliased, Is.Null);
    }

    [Test] // #4672
    public async Task With_parameter_without_value()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT foo FROM {table} WHERE foo > @p", conn)
        {
            Parameters = { new() { ParameterName = "p", NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer } }
        };
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);

        var columns = await GetColumnSchema(reader);
        Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
    }

    #region Not supported

    [Test]
    public async Task IsExpression()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT * FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        Assert.That(reader.GetColumnSchema().Single().IsExpression, Is.False);
    }

    [Test]
    public async Task IsHidden()
    {
        using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "foo INTEGER");

        using var cmd = new NpgsqlCommandOrig($"SELECT * FROM {table}", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);
        Assert.That(reader.GetColumnSchema().Single().IsHidden, Is.False);
    }

    #endregion

    class SomeComposite
    {
        public int Foo { get; set; }
    }

    public ReaderNewSchemaTests(SyncOrAsync syncOrAsync) : base(syncOrAsync) { }

    async Task<ReadOnlyCollection<Schema.NpgsqlDbColumn>> GetColumnSchema(NpgsqlDataReaderOrig reader)
        => IsAsync ? await reader.GetColumnSchemaAsync() : reader.GetColumnSchema();
}
