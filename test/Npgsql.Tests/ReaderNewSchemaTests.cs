using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests
{
    /// <summary>
    /// This tests the new CoreCLR schema/metadata API, which returns ReadOnlyCollection&lt;DbColumn&gt;.
    /// Note that this API is also available on .NET Framework.
    /// For the old DataTable-based API, see <see cref="ReaderOldSchemaTests"/>.
    /// </summary>
    public class ReaderNewSchemaTests : TestBase
    {
        // ReSharper disable once InconsistentNaming
        [Test]
        public async Task AllowDBNull()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "nullable INTEGER, non_nullable INTEGER NOT NULL", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT nullable,non_nullable,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].AllowDBNull, Is.True);
                    Assert.That(columns[1].AllowDBNull, Is.False);
                    Assert.That(columns[2].AllowDBNull, Is.Null);
                }
            }
        }

        [Test]
        public async Task BaseCatalogName()
        {
            var dbName = new NpgsqlConnectionStringBuilder(ConnectionString).Database;
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseCatalogName, Is.EqualTo(dbName));
                    Assert.That(columns[1].BaseCatalogName, Is.EqualTo(dbName));
                }
            }
        }

        [Test]
        public async Task BaseColumnName()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 AS bar,8,'8'::VARCHAR(10) FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[1].BaseColumnName, Is.EqualTo("bar"));
                    Assert.That(columns[2].BaseColumnName, Is.Null);
                    Assert.That(columns[3].BaseColumnName, Is.EqualTo("varchar"));
                }
            }
        }

        [Test]
        public void BaseColumnNameWithColumnAliases()
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

            var cmd = new NpgsqlCommand("SELECT Cod as CodAlias, Descr as DescrAlias, Date FROM data", conn);

            using var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
            var cols = dr.GetColumnSchema();

            Assert.That(cols[0].BaseColumnName, Is.EqualTo("cod"));
            Assert.That(cols[0].ColumnName.ToString(), Is.EqualTo("codalias"));
            Assert.That(cols[1].BaseColumnName.ToString(), Is.EqualTo("descr"));
            Assert.That(cols[1].ColumnName.ToString(), Is.EqualTo("descralias"));
            Assert.That(cols[2].BaseColumnName.ToString(), Is.EqualTo("date"));
            Assert.That(cols[2].ColumnName.ToString(), Is.EqualTo("date"));
        }

        [Test]
        public async Task BaseSchemaName()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseSchemaName, Is.EqualTo("public"));
                    Assert.That(columns[1].BaseSchemaName, Is.Null);
                }
            }
        }

        [Test]
        public async Task BaseServerName()
        {
            var host = new NpgsqlConnectionStringBuilder(ConnectionString).Host;
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseServerName, Is.EqualTo(host));
                    Assert.That(columns[1].BaseServerName, Is.EqualTo(host));
                }
            }
        }

        [Test]
        public async Task BaseTableName()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].BaseTableName, Does.StartWith("temp_table"));
                    Assert.That(columns[1].BaseTableName, Is.Null);
                }
            }
        }

        [Test]
        public async Task ColumnName()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 AS bar,8,'8'::VARCHAR(10) FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("bar"));
                    Assert.That(columns[2].ColumnName, Is.Null);
                    Assert.That(columns[3].ColumnName, Is.EqualTo("varchar"));
                }
            }

            // See https://github.com/npgsql/npgsql/issues/1676
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "col TEXT", out var table);

                var behavior = CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
                //var behavior = CommandBehavior.SchemaOnly;
                using (var command = new NpgsqlCommand($"SELECT col AS col_alias FROM {table}", conn))
                using (var reader = command.ExecuteReader(behavior))
                    Assert.That(reader.GetColumnSchema()[0].ColumnName, Is.EqualTo("col_alias"));
            }
        }

        [Test]
        public async Task ColumnOrdinal()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "first INTEGER, second INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT second,first FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("second"));
                    Assert.That(columns[0].ColumnOrdinal, Is.EqualTo(0));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("first"));
                    Assert.That(columns[1].ColumnOrdinal, Is.EqualTo(1));
                }
            }
        }

        [Test]
        public async Task ColumnAttributeNumber()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "first INTEGER, second INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT second,first FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("second"));
                    Assert.That(columns[0].ColumnAttributeNumber, Is.EqualTo(1));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("first"));
                    Assert.That(columns[1].ColumnAttributeNumber, Is.EqualTo(0));
                }
            }
        }

        [Test]
        public async Task ColumnSize()
        {
            if (IsRedshift)
                Assert.Ignore("Column size is never unlimited on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "bounded VARCHAR(30), unbounded VARCHAR", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT bounded,unbounded,'a'::VARCHAR(10),'b'::VARCHAR FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnSize, Is.EqualTo(30));
                    Assert.That(columns[1].ColumnSize, Is.Null);
                    Assert.That(columns[2].ColumnSize, Is.EqualTo(10));
                    Assert.That(columns[3].ColumnSize, Is.Null);
                }
            }
        }

        [Test]
        public async Task IsAutoIncrement()
        {
            if (IsRedshift)
                Assert.Ignore("Serial columns not support on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "inc SERIAL, non_inc INT", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT inc,non_inc,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsAutoIncrement, Is.True, "Serial not identified as autoincrement");
                    Assert.That(columns[1].IsAutoIncrement, Is.False, "Regular int column identified as autoincrement");
                    Assert.That(columns[2].IsAutoIncrement, Is.Null, "Literal int identified as autoincrement");
                }
            }
        }

        [Test]
        public async Task IsAutoIncrementIdentity()
        {
            using (var conn = await OpenConnectionAsync())
            {
                if (conn.PostgreSqlVersion < new Version(10, 0))
                    Assert.Ignore("IDENTITY introduced in PostgreSQL 10");

                await using var _ = await CreateTempTable(
                    conn,
                    "inc SERIAL, identity INT GENERATED BY DEFAULT AS IDENTITY, non_inc INT",
                    out var table);

                using (var cmd = new NpgsqlCommand($"SELECT inc,identity,non_inc,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsAutoIncrement, Is.True, "Serial not identified as autoincrement");
                    Assert.That(columns[1].IsAutoIncrement, Is.True, "PG 10 IDENTITY not identified as autoincrement");
                    Assert.That(columns[2].IsAutoIncrement, Is.False, "Regular int column identified as autoincrement");
                    Assert.That(columns[3].IsAutoIncrement, Is.Null, "Literal int identified as autoincrement");
                }
            }
        }

        [Test]
        public async Task IsKey()
        {
            if (IsRedshift)
                Assert.Ignore("Key not supported in reader schema on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "id INT PRIMARY KEY, non_id INT, uniq INT UNIQUE", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT id,non_id,uniq,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsKey, Is.True);
                    Assert.That(columns[1].IsKey, Is.False);

                    // Note: according to the old API docs any unique column is considered key.
                    // https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
                    // But in the new API we have a separate IsUnique so IsKey should be false
                    Assert.That(columns[2].IsKey, Is.False);

                    Assert.That(columns[3].IsKey, Is.Null);
                }
            }
        }

        [Test]
        public async Task IsKeyComposite()
        {
            if (IsRedshift)
                Assert.Ignore("Key not supported in reader schema on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "id1 INT, id2 INT, PRIMARY KEY (id1, id2)", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT id1,id2 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsKey, Is.True);
                    Assert.That(columns[1].IsKey, Is.True);
                }
            }
        }

        [Test]
        public async Task IsLong()
        {
            if (IsRedshift)
                Assert.Ignore("bytea not supported on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "long BYTEA, non_long INT", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT long, non_long, 8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsLong, Is.True);
                    Assert.That(columns[1].IsLong, Is.False);
                    Assert.That(columns[2].IsLong, Is.False);
                }
            }
        }

        [Test]
        public async Task IsReadOnlyOnView()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempViewName(conn, out var view);
                await using var __ = await GetTempTableName(conn, out var table);

                await conn.ExecuteNonQueryAsync($@"
CREATE VIEW {view} AS SELECT 8 AS foo;
CREATE TABLE {table} (bar INTEGER)");

                using (var cmd = new NpgsqlCommand($"SELECT foo,bar FROM {view},{table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsReadOnly, Is.True);
                    Assert.That(columns[1].IsReadOnly, Is.False);
                }
            }
        }

        [Test]
        public async Task IsReadOnlyOnNonColumn()
        {
            using (var conn = await OpenConnectionAsync())
            {
                using (var cmd = new NpgsqlCommand("SELECT 8", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsReadOnly, Is.True);
            }
        }

        [Test]
        public async Task IsUnique()
        {
            if (IsRedshift)
                Assert.Ignore("Unique not supported in reader schema on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var __ = await GetTempTableName(conn, out var table);

                await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (id INT PRIMARY KEY, non_id INT, uniq INT UNIQUE, non_id_second INT, non_id_third INT);
CREATE UNIQUE INDEX idx_{table} ON {table} (non_id_second, non_id_third)");

                using (var cmd = new NpgsqlCommand($"SELECT id,non_id,uniq,8,non_id_second,non_id_third FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].IsUnique, Is.True);
                    Assert.That(columns[1].IsUnique, Is.False);
                    Assert.That(columns[2].IsUnique, Is.True);
                    Assert.That(columns[3].IsUnique, Is.Null);
                    Assert.That(columns[4].IsUnique, Is.False);
                    Assert.That(columns[5].IsUnique, Is.False);
                }
            }
        }

        [Test]
        public async Task NumericPrecision()
        {
            if (IsRedshift)
                Assert.Ignore("Precision is never unlimited on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "a NUMERIC(8), b NUMERIC, c INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT a,b,c,8.3::NUMERIC(8) FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].NumericPrecision, Is.EqualTo(8));
                    Assert.That(columns[1].NumericPrecision, Is.Null);
                    Assert.That(columns[2].NumericPrecision, Is.Null);
                    Assert.That(columns[3].NumericPrecision, Is.EqualTo(8));
                }
            }
        }

        [Test]
        public async Task NumericScale()
        {
            if (IsRedshift)
                Assert.Ignore("Scale is never unlimited on Redshift");
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "a NUMERIC(8,5), b NUMERIC, c INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT a,b,c,8.3::NUMERIC(8,5) FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].NumericScale, Is.EqualTo(5));
                    Assert.That(columns[1].NumericScale, Is.Null);
                    Assert.That(columns[2].NumericScale, Is.Null);
                    Assert.That(columns[3].NumericScale, Is.EqualTo(5));
                }
            }
        }

        [Test]
        public async Task DataType()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8::INTEGER FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DataType, Is.SameAs(typeof(int)));
                    Assert.That(columns[1].DataType, Is.SameAs(typeof(int)));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1305")]
        public async Task DataTypeUnknownType()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo::INTEGER FROM {table}", conn))
                {
                    cmd.AllResultTypesAreUnknown = true;
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    {
                        var columns = reader.GetColumnSchema();
                        Assert.That(columns[0].DataType, Is.SameAs(typeof(int)));
                    }
                }
            }
        }

        [Test, NonParallelizable]
        public async Task DataTypeWithComposite()
        {
            if (IsRedshift)
                Assert.Ignore("Composite types not support on Redshift");
            // if (IsMultiplexing)
            //     Assert.Ignore("Multiplexing: ReloadTypes");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(DataTypeWithComposite),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = await OpenConnectionAsync(csb))
            {
                await conn.ExecuteNonQueryAsync("CREATE TYPE pg_temp.some_composite AS (foo int)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<SomeComposite>();
                await conn.ExecuteNonQueryAsync("CREATE TEMP TABLE data (comp pg_temp.some_composite)");

                using (var cmd = new NpgsqlCommand("SELECT comp,'(4)'::some_composite FROM data", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DataType, Is.SameAs(typeof(SomeComposite)));
                    Assert.That(columns[0].UdtAssemblyQualifiedName, Is.EqualTo(typeof(SomeComposite).AssemblyQualifiedName));
                    Assert.That(columns[1].DataType, Is.SameAs(typeof(SomeComposite)));
                    Assert.That(columns[1].UdtAssemblyQualifiedName, Is.EqualTo(typeof(SomeComposite).AssemblyQualifiedName));
                }
            }
        }

        [Test]
        public async Task UdtAssemblyQualifiedName()
        {
            // Also see DataTypeWithComposite
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].UdtAssemblyQualifiedName, Is.Null);
                    Assert.That(columns[1].UdtAssemblyQualifiedName, Is.Null);
                }
            }
        }

        [Test]
        public async Task PostgresType()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    var intType = columns[0].PostgresType;
                    Assert.That(columns[1].PostgresType, Is.SameAs(intType));
                    Assert.That(intType.Name, Is.EqualTo("integer"));
                    Assert.That(intType.InternalName, Is.EqualTo("int4"));
                }
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

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, $"foo {typeName}", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,NULL::{typeName} FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
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
            }
        }

        [Test]
        public async Task DefaultValue()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(
                    conn, "with_default INTEGER DEFAULT(8), without_default INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT with_default,without_default,8 FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].DefaultValue, Is.EqualTo("8"));
                    Assert.That(columns[1].DefaultValue, Is.Null);
                    Assert.That(columns[2].DefaultValue, Is.Null);
                }
            }
        }

        [Test]
        public async Task SameColumnName()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await GetTempTableName(conn, out var table1);
                await using var __ = await GetTempTableName(conn, out var table2);

                await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table1} (foo INTEGER);
CREATE TABLE {table2} (foo INTEGER)");

                using (var cmd = new NpgsqlCommand($"SELECT {table1}.foo,{table2}.foo FROM {table1},{table2}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].ColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[0].BaseTableName, Does.StartWith("temp_table"));
                    Assert.That(columns[1].ColumnName, Is.EqualTo("foo"));
                    Assert.That(columns[1].BaseTableName, Does.StartWith("temp_table"));
                    Assert.That(columns[0].BaseTableName, Is.Not.EqualTo(columns[1].BaseTableName));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1553")]
        public async Task DomainTypes()
        {
            if (IsRedshift)
                Assert.Ignore("Domain types not support on Redshift");
            // if (IsMultiplexing)
            //     Assert.Ignore("Multiplexing: ReloadTypes");
            using (var conn = await OpenConnectionAsync())
            {
                await conn.ExecuteNonQueryAsync("CREATE DOMAIN pg_temp.mydomain AS varchar(2)");
                conn.ReloadTypes();
                await conn.ExecuteNonQueryAsync("CREATE TEMP TABLE data (domain mydomain)");
                using (var cmd = new NpgsqlCommand("SELECT domain FROM data", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var domainSchema = reader.GetColumnSchema().Single(c => c.ColumnName == "domain");
                    Assert.That(domainSchema.ColumnSize, Is.EqualTo(2));
                    var pgType = domainSchema.PostgresType;
                    Assert.That(pgType, Is.InstanceOf<PostgresDomainType>());
                    Assert.That(((PostgresDomainType)pgType).BaseType.Name, Is.EqualTo("character varying"));
                }
            }
        }

        [Test]
        public async Task NpgsqlDbType()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT foo,8::INTEGER FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    Assert.That(columns[0].NpgsqlDbType, Is.EqualTo(NpgsqlTypes.NpgsqlDbType.Integer));
                    Assert.That(columns[1].NpgsqlDbType, Is.EqualTo(NpgsqlTypes.NpgsqlDbType.Integer));
                }
            }
        }

        [Test]
        public async Task NpgsqlDbTypeExtension()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await EnsureExtensionAsync(conn, "hstore", "9.1");

                using (var cmd = new NpgsqlCommand("SELECT NULL::HSTORE", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                {
                    var columns = reader.GetColumnSchema();
                    // The full datatype name for PostGIS is public.geometry (unlike int4 which is in pg_catalog).
                    Assert.That(columns[0].NpgsqlDbType, Is.EqualTo(NpgsqlTypes.NpgsqlDbType.Hstore));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1950")]
        public async Task NoResultset()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("COMMIT", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                reader.GetColumnSchema();
            }
        }

        #region Not supported

        [Test]
        public async Task IsAliased()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT * FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsAliased, Is.False);
            }
        }

        [Test]
        public async Task IsExpression()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT * FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsExpression, Is.False);
            }
        }

        [Test]
        public async Task IsHidden()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT * FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsHidden, Is.False);
            }
        }

        [Test]
        public async Task IsIdentity()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "foo INTEGER", out var table);

                using (var cmd = new NpgsqlCommand($"SELECT * FROM {table}", conn))
                using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                    Assert.That(reader.GetColumnSchema().Single().IsIdentity, Is.False);
            }
        }

        #endregion

        class SomeComposite
        {
            public int Foo { get; set; }
        }
    }
}
