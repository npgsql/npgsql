using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework7.Npgsql.Metadata;
using EntityFramework7.Npgsql.Migrations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Migrations.Operations;
using Npgsql;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class MigrationsNpgsqlTest : MigrationsTestBase<MigrationsNpgsqlFixture>
    {
        NpgsqlMigrationsSqlGenerator SqlGenerator =>
            new NpgsqlMigrationsSqlGenerator(
                new NpgsqlUpdateSqlGenerator(),
                new NpgsqlTypeMapper(),
                new NpgsqlMetadataExtensionProvider());

        private readonly MigrationsNpgsqlFixture _fixture;

        public MigrationsNpgsqlTest(MigrationsNpgsqlFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void MigrationHistory_Table_Is_Created_On_Empty_Database()
        {
            using (var db = _fixture.CreateContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
        }

        protected override async Task AssertFirstMigrationAsync(DbConnection connection)
        {
            var sql = await GetDatabaseSchemaAsync(connection);
            Assert.Equal(
                @"
CreatedTable
    Id int4 NOT NULL
    ColumnWithDefaultToDrop int4 NULL DEFAULT 0
    ColumnWithDefaultToAlter int4 NULL DEFAULT 1
",
                sql);
        }

        protected override async Task AssertSecondMigrationAsync(DbConnection connection)
        {
            var sql = await GetDatabaseSchemaAsync(connection);
            Assert.Equal(
                @"
CreatedTable
    Id int4 NOT NULL
    ColumnWithDefaultToAlter int4 NULL
",
                sql);
        }

        private async Task<string> GetDatabaseSchemaAsync(DbConnection connection)
        {
            var builder = new IndentedStringBuilder();
            var command = connection.CreateCommand();
            command.CommandText = @"
SELECT table_name,
	column_name,
	udt_name,
	is_nullable = 'YES',
	column_default
FROM information_schema.columns
WHERE table_catalog = @db
	AND table_schema = 'public'
ORDER BY table_name, ordinal_position
";

            var dbName = connection.Database;
            var npgConnection = (NpgsqlConnection)connection;
            command.Parameters.Add(new NpgsqlParameter() {ParameterName = "db", Value = dbName});

            using (var reader = await command.ExecuteReaderAsync())
            {
                var first = true;
                string lastTable = null;
                while (await reader.ReadAsync())
                {
                    var currentTable = reader.GetString(0);
                    if (currentTable != lastTable)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            builder.DecrementIndent();
                        }

                        builder
                            .AppendLine()
                            .AppendLine(currentTable)
                            .IncrementIndent();

                        lastTable = currentTable;
                    }

                    builder
                        .Append(reader[1]) // Name
                        .Append(" ")
                        .Append(reader[2]) // Type
                        .Append(" ")
                        .Append(reader.GetBoolean(3) ? "NULL" : "NOT NULL");

                    if (!await reader.IsDBNullAsync(4))
                    {
                        builder
                            .Append(" DEFAULT ")
                            .Append(reader[4]);
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString().ToUnixNewlines();
        }

        [Fact]
        public void EnsureSchemaOperation()
        {
            using (var db = _fixture.CreateContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Run the ensure schema operation twice, once to actually create the schema and another
                // to check everything's OK when it already exists
                for (var i = 0; i < 2; i++)
                {
                    ExecuteMigration(db, new EnsureSchemaOperation {Name = "test_schema"});

                    using (var cmd = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM pg_namespace WHERE nspname='test_schema');"))
                    {
                        db.Database.OpenConnection();
                        cmd.Connection = (NpgsqlConnection) db.Database.GetDbConnection();
                        Assert.True((bool) cmd.ExecuteScalar());
                    }
                }
            }
        }

        void ExecuteMigration(DbContext db, MigrationOperation operation)
        {
            var batch = SqlGenerator.Generate(new[] { operation });
            if (db.Database.GetDbConnection().State != ConnectionState.Open) {
                db.Database.OpenConnection();
            }
            var conn = (NpgsqlConnection)db.Database.GetDbConnection();
            using (var cmd = new NpgsqlCommand("", conn))
            {
                foreach (var sql in batch.Select(b => b.Sql))
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
