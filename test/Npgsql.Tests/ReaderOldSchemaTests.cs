﻿using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests
{
    /// <summary>
    /// This tests the .NET Framework DbDataReader schema/metadata API, which returns DataTable.
    /// For the new CoreCLR API, see <see cref="ReaderNewSchemaTests"/>.
    /// </summary>
    public class ReaderOldSchemaTests : SyncOrAsyncTestBase
    {
        [Test]
        public async Task PrimaryKeyFieldsMetadataSupport()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await GetTempTableName(conn, out var table);

            await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (
    field_pk1 INT2 NOT NULL,
    field_pk2 INT2 NOT NULL,
    field_serial SERIAL,
    CONSTRAINT data2_pkey PRIMARY KEY (field_pk1, field_pk2)
)");

            using var command = new NpgsqlCommand($"SELECT * FROM {table}", conn);
            using var dr = command.ExecuteReader(CommandBehavior.KeyInfo);
            dr.Read();
            var dataTable = await GetSchemaTable(dr);
#pragma warning disable 8602 // Warning should be removable after rc2 (https://github.com/dotnet/runtime/pull/42215)
            DataRow[] keyColumns =
                dataTable!.Rows.Cast<DataRow>().Where(r => (bool)r["IsKey"]).ToArray()!;
#pragma warning restore 8602
            Assert.That(keyColumns, Has.Length.EqualTo(2));
            Assert.That(keyColumns.Count(c => (string)c["ColumnName"] == "field_pk1"), Is.EqualTo(1));
            Assert.That(keyColumns.Count(c => (string)c["ColumnName"] == "field_pk2"), Is.EqualTo(1));
        }

        [Test]
        public async Task PrimaryKeyFieldMetadataSupport()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await CreateTempTable(conn, "id SERIAL PRIMARY KEY, serial SERIAL", out var table);

            using var command = new NpgsqlCommand($"SELECT * FROM {table}", conn);
            using var dr = command.ExecuteReader(CommandBehavior.KeyInfo);
            dr.Read();
            var metadata = await GetSchemaTable(dr);
#pragma warning disable 8602 // Warning should be removable after rc2 (https://github.com/dotnet/runtime/pull/42215)
            var key = metadata!.Rows.Cast<DataRow>().Single(r => (bool)r["IsKey"])!;
#pragma warning restore 8602
            Assert.That(key["ColumnName"], Is.EqualTo("id"));
        }

        [Test]
        public async Task IsAutoIncrementMetadataSupport()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await CreateTempTable(conn, "id SERIAL PRIMARY KEY", out var table);

            var command = new NpgsqlCommand($"SELECT * FROM {table}", conn);

            using var dr = command.ExecuteReader(CommandBehavior.KeyInfo);
            var metadata = await GetSchemaTable(dr);
#pragma warning disable 8602 // Warning should be removable after rc2 (https://github.com/dotnet/runtime/pull/42215)
            Assert.That(metadata!.Rows.Cast<DataRow>()
                .Where(r => ((string)r["ColumnName"]).Contains("serial"))
                .All(r => (bool)r["IsAutoIncrement"]));
#pragma warning restore 8602
        }

        [Test]
        public async Task IsReadOnlyMetadataSupport()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await GetTempTableName(conn, out var table);
            await using var __ = await GetTempViewName(conn, out var view);

            await conn.ExecuteNonQueryAsync($@"
CREATE TABLE {table} (id SERIAL PRIMARY KEY, int2 SMALLINT);
CREATE OR REPLACE VIEW {view} (id, int2) AS SELECT id, int2 + int2 AS int2 FROM {table}");

            var command = new NpgsqlCommand($"SELECT * FROM {view}", conn);

            using var dr = command.ExecuteReader();
            var metadata = await GetSchemaTable(dr);

            foreach (var r in metadata!.Rows.OfType<DataRow>())
            {
                switch ((string)r["ColumnName"])
                {
                case "field_pk":
                    if (conn.PostgreSqlVersion < new Version("9.4"))
                    {
                        // 9.3 and earlier: IsUpdatable = False
                        Assert.IsTrue((bool)r["IsReadonly"], "field_pk");
                    }
                    else
                    {
                        // 9.4: IsUpdatable = True
                        Assert.IsFalse((bool)r["IsReadonly"], "field_pk");
                    }
                    break;
                case "field_int2":
                    Assert.IsTrue((bool)r["IsReadonly"]);
                    break;
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        [Test]
        public async Task AllowDBNull()
        {
            using var conn = await OpenConnectionAsync();
            await using var _ = await CreateTempTable(conn, "nullable INTEGER, non_nullable INTEGER NOT NULL", out var table);

            using var cmd = new NpgsqlCommand($"SELECT * FROM {table}", conn);
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
            using var metadata = await GetSchemaTable(reader);
            foreach (var row in metadata!.Rows.OfType<DataRow>())
            {
                var isNullable = (bool)row["AllowDBNull"];
                switch ((string)row["ColumnName"])
                {
                case "nullable":
                    Assert.IsTrue(isNullable);
                    continue;
                case "non_nullable":
                    Assert.IsFalse(isNullable);
                    continue;
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1027")]
        public async Task WithoutResult()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT 1", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.NextResult();
            // We're no longer on a result
            var table = await GetSchemaTable(reader);
            Assert.That(table, Is.Null);
        }

        [Test]
        public async Task PrecisionAndScale()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT 1::NUMERIC AS result", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            var schemaTable = await GetSchemaTable(reader);
            foreach (var myField in schemaTable!.Rows.OfType<DataRow>())
            {
                Assert.That(myField["NumericScale"], Is.EqualTo(0));
                Assert.That(myField["NumericPrecision"], Is.EqualTo(0));
            }
        }

        [Test]
        public async Task SchemaOnly([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            // if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            //     return;

            using var conn = await OpenConnectionAsync();
            await using var _ = await CreateTempTable(conn, "name TEXT", out var table);

            var query = $@"
SELECT 1 AS some_column;
UPDATE {table} SET name='yo' WHERE 1=0;
SELECT 1 AS some_other_column, 2";

            using var cmd = new NpgsqlCommand(query, conn);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
            {
                Assert.That(reader.Read(), Is.False);
                var t = await GetSchemaTable(reader);
                Assert.That(t!.Rows[0]["ColumnName"], Is.EqualTo("some_column"));
                Assert.That(reader.NextResult(), Is.True);
                Assert.That(reader.Read(), Is.False);
                t = await GetSchemaTable(reader);
                Assert.That(t!.Rows[0]["ColumnName"], Is.EqualTo("some_other_column"));
                Assert.That(t.Rows[1]["ColumnName"], Is.EqualTo("?column?"));
                Assert.That(reader.NextResult(), Is.False);
            }

            // Close reader in the middle
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly))
                reader.Read();
        }

        [Test]
        public async Task BaseColumnName()
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
            var dt = await GetSchemaTable(dr);

            Assert.That(dt!.Rows[0]["BaseColumnName"].ToString(), Is.EqualTo("cod"));
            Assert.That(dt.Rows[0]["ColumnName"].ToString(), Is.EqualTo("codalias"));
            Assert.That(dt.Rows[1]["BaseColumnName"].ToString(), Is.EqualTo("descr"));
            Assert.That(dt.Rows[1]["ColumnName"].ToString(), Is.EqualTo("descralias"));
            Assert.That(dt.Rows[2]["BaseColumnName"].ToString(), Is.EqualTo("date"));
            Assert.That(dt.Rows[2]["ColumnName"].ToString(), Is.EqualTo("date"));
        }

        public ReaderOldSchemaTests(SyncOrAsync syncOrAsync) : base(syncOrAsync) { }

        private async Task<DataTable?> GetSchemaTable(NpgsqlDataReader dr) => IsAsync ? await dr.GetSchemaTableAsync() : dr.GetSchemaTable();
    }
}
