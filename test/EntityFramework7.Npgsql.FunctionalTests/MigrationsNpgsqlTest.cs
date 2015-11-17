using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Operations;
using Npgsql;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class MigrationsNpgsqlTest : MigrationsTestBase<MigrationsNpgsqlFixture>
    {
        public MigrationsNpgsqlTest(MigrationsNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Can_generate_up_scripts()
        {
            base.Can_generate_up_scripts();

            Assert.Equal(
                @"CREATE TABLE ""__EFMigrationsHistory"" (
    ""MigrationId"" text NOT NULL,
    ""ProductVersion"" text NOT NULL,
    CONSTRAINT ""PK_HistoryRow"" PRIMARY KEY (""MigrationId"")
);

CREATE TABLE ""Table1"" (
    ""Id"" int4 NOT NULL,
    CONSTRAINT ""PK_Table1"" PRIMARY KEY (""Id"")
);

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000001_Migration1', '7.0.0-test');

ALTER TABLE ""Table1"" RENAME TO ""Table2"";

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('00000000000002_Migration2', '7.0.0-test');

",
                Sql);
        }

        public override void Can_generate_idempotent_up_scripts()
        {
            Assert.Throws<NotSupportedException>(() => base.Can_generate_idempotent_up_scripts());
        }

        public override void Can_generate_down_scripts()
        {
            base.Can_generate_down_scripts();

            Assert.Equal(
                @"ALTER TABLE ""Table2"" RENAME TO ""Table1"";

DELETE FROM ""__EFMigrationsHistory""
WHERE ""MigrationId"" = '00000000000002_Migration2';

DROP TABLE ""Table1"";

DELETE FROM ""__EFMigrationsHistory""
WHERE ""MigrationId"" = '00000000000001_Migration1';

",
                Sql);
        }

        public override void Can_generate_idempotent_down_scripts()
        {
            Assert.Throws<NotSupportedException>(() => base.Can_generate_idempotent_down_scripts());
        }

        protected override void AssertFirstMigration(DbConnection connection)
        {
            var sql = GetDatabaseSchemaAsync(connection);
            Assert.Equal(
                @"
CreatedTable
    Id int4 NOT NULL
    ColumnWithDefaultToDrop int4 NULL DEFAULT 0
    ColumnWithDefaultToAlter int4 NULL DEFAULT 1
",
                sql);
        }

        protected override void BuildSecondMigration(MigrationBuilder migrationBuilder)
        {
            base.BuildSecondMigration(migrationBuilder);

            for (int i = migrationBuilder.Operations.Count - 1; i >= 0; i--)
            {
                var operation = migrationBuilder.Operations[i];
                if (operation is AlterColumnOperation
                    || operation is DropColumnOperation)
                {
                    migrationBuilder.Operations.RemoveAt(i);
                }
            }
        }

        protected override void AssertSecondMigration(DbConnection connection)
        {
            var sql = GetDatabaseSchemaAsync(connection);
            Assert.Equal(
                @"
CreatedTable
    Id int4 NOT NULL
    ColumnWithDefaultToDrop int4 NULL DEFAULT 0
    ColumnWithDefaultToAlter int4 NULL DEFAULT 1
",
                sql);
        }

        private string GetDatabaseSchemaAsync(DbConnection connection)
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
            command.Parameters.Add(new NpgsqlParameter() { ParameterName = "db", Value = dbName });

            using (var reader = command.ExecuteReader())
            {
                var first = true;
                string lastTable = null;
                while (reader.Read())
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

                    if (!reader.IsDBNull(4))
                    {
                        builder
                            .Append(" DEFAULT ")
                            .Append(reader[4]);
                    }

                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        protected new string Sql => base.Sql;
    }
}
