using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Scaffolding;
using Microsoft.Data.Entity.Scaffolding.Metadata;
using Xunit;
using EntityFramework7.Npgsql.FunctionalTests;
using Microsoft.Extensions.Logging;

namespace EntityFramework7.Npgsql.Design.FunctionalTests
{
    public class NpgsqlDatabaseModelFactoryTest : IClassFixture<NpgsqlDatabaseModelFixture>
    {
        [Fact]
        public void It_reads_tables()
        {
            var sql = @"
CREATE TABLE public.everest (id int);
CREATE TABLE public.denali (id int);";
            var dbModel = CreateModel(sql, new TableSelectionSet(new List<string> { "everest", "denali" }));

            Assert.Collection(dbModel.Tables.OrderBy(t => t.Name),
                d =>
                {
                    Assert.Equal("public", d.SchemaName);
                    Assert.Equal("denali", d.Name);
                },
                e =>
                {
                    Assert.Equal("public", e.SchemaName);
                    Assert.Equal("everest", e.Name);
                });
        }

        [Fact]
        public void It_reads_foreign_keys()
        {
            _fixture.ExecuteNonQuery("CREATE SCHEMA db2");
            var sql = "CREATE TABLE public.ranges (id INT PRIMARY KEY);" +
                      "CREATE TABLE db2.mountains (range_id INT NOT NULL, FOREIGN KEY (range_id) REFERENCES ranges(id) ON DELETE CASCADE)";
            var dbModel = CreateModel(sql, new TableSelectionSet(new List<string> { "ranges", "mountains" }));

            var fk = Assert.Single(dbModel.Tables.Single(t => t.ForeignKeys.Count > 0).ForeignKeys);

            Assert.Equal("db2", fk.Table.SchemaName);
            Assert.Equal("mountains", fk.Table.Name);
            Assert.Equal("public", fk.PrincipalTable.SchemaName);
            Assert.Equal("ranges", fk.PrincipalTable.Name);
            Assert.Equal("range_id", fk.Columns.Single().Name);
            Assert.Equal("id", fk.PrincipalColumns.Single().Name);
            Assert.Equal(ReferentialAction.Cascade, fk.OnDelete);
        }

        [Fact]
        public void It_reads_composite_foreign_keys()
        {
            _fixture.ExecuteNonQuery("CREATE SCHEMA db3");
            var sql = "CREATE TABLE public.ranges1 (id INT, alt_id INT, PRIMARY KEY(id, alt_id));" +
                      "CREATE TABLE db3.mountains1 (range_id INT NOT NULL, range_alt_id INT NOT NULL, FOREIGN KEY (range_id, range_alt_id) REFERENCES ranges1(id, alt_id) ON DELETE NO ACTION)";
            var dbModel = CreateModel(sql, new TableSelectionSet(new List<string> { "ranges1", "mountains1" }));

            var fk = Assert.Single(dbModel.Tables.Single(t => t.ForeignKeys.Count > 0).ForeignKeys);

            Assert.Equal("db3", fk.Table.SchemaName);
            Assert.Equal("mountains1", fk.Table.Name);
            Assert.Equal("public", fk.PrincipalTable.SchemaName);
            Assert.Equal("ranges1", fk.PrincipalTable.Name);
            Assert.Equal(new[] { "range_id", "range_alt_id" }, fk.Columns.Select(c => c.Name).ToArray());
            Assert.Equal(new[] { "id", "alt_id" }, fk.PrincipalColumns.Select(c => c.Name).ToArray());
            Assert.Equal(ReferentialAction.NoAction, fk.OnDelete);
        }

        [Fact]
        public void It_reads_indexes()
        {
            var sql = @"CREATE TABLE place (id int PRIMARY KEY, name int UNIQUE, location int);" +
                      @"CREATE INDEX ""IX_name_location"" ON place (name, location)";
            var dbModel = CreateModel(sql, new TableSelectionSet(new List<string> { "place" }));

            var indexes = dbModel.Tables.Single().Indexes;

            Assert.All(indexes, c =>
            {
                Assert.Equal("public", c.Table.SchemaName);
                Assert.Equal("place", c.Table.Name);
            });

            Assert.Collection(indexes,
                unique =>
                {
                    Assert.True(unique.IsUnique);
                    Assert.Equal("name", unique.Columns.Single().Name);
                },
                composite =>
                {
                    Assert.Equal("IX_name_location", composite.Name);
                    Assert.False(composite.IsUnique);
                    Assert.Equal(new List<string> { "name", "location" }, composite.Columns.Select(c => c.Name).ToList());
                });
        }

        [Fact]
        public void It_reads_columns()
        {
            var sql = @"
CREATE TABLE public.mountains_columns (
    id int,
    name varchar(100) NOT NULL,
    decimal decimal(5,2) DEFAULT 0.0,
    decimal2 numeric,
    created timestamp DEFAULT now(),
    PRIMARY KEY (name, id)
);";
            var dbModel = CreateModel(sql, new TableSelectionSet(new List<string> { "mountains_columns" }));

            var columns = dbModel.Tables.Single().Columns.OrderBy(c => c.Ordinal);

            Assert.All(columns, c =>
            {
                Assert.Equal("public", c.Table.SchemaName);
                Assert.Equal("mountains_columns", c.Table.Name);
            });

            Assert.Collection(columns,
                id =>
                {
                    Assert.Equal("id", id.Name);
                    Assert.Equal("int4", id.DataType);
                    Assert.Equal(2, id.PrimaryKeyOrdinal);
                    Assert.False(id.IsNullable);
                    Assert.Equal(0, id.Ordinal);
                    Assert.Null(id.DefaultValue);
                },
                name =>
                {
                    Assert.Equal("name", name.Name);
                    Assert.Equal("varchar", name.DataType);
                    Assert.Equal(1, name.PrimaryKeyOrdinal);
                    Assert.False(name.IsNullable);
                    Assert.Equal(1, name.Ordinal);
                    Assert.Null(name.DefaultValue);
                    Assert.Equal(100, name.MaxLength);
                },
                dec =>
                {
                    Assert.Equal("decimal", dec.Name);
                    Assert.Equal("numeric", dec.DataType);
                    Assert.Null(dec.PrimaryKeyOrdinal);
                    Assert.True(dec.IsNullable);
                    Assert.Equal(2, dec.Ordinal);
                    Assert.Equal("0.0", dec.DefaultValue);
                    Assert.Equal(5, dec.Precision);
                    Assert.Equal(2, dec.Scale);
                    Assert.Null(dec.MaxLength);
                },
                dec =>
                {
                    Assert.Equal("decimal2", dec.Name);
                    Assert.Equal("numeric", dec.DataType);
                    Assert.Null(dec.PrimaryKeyOrdinal);
                    Assert.True(dec.IsNullable);
                    Assert.Equal(3, dec.Ordinal);
                    Assert.Null(dec.DefaultValue);
                    Assert.Null(dec.Precision);
                    Assert.Null(dec.Scale);
                    Assert.Null(dec.MaxLength);
                },
                created =>
                {
                    Assert.Equal("created", created.Name);
                    Assert.Equal("now()", created.DefaultValue);
                });
        }

        [Theory]
        [InlineData("varchar(341)", 341)]
        [InlineData("char(89)", 89)]
        [InlineData("varchar", null)]
        [InlineData("text", null)]
        public void It_reads_max_length(string type, int? length)
        {
            var sql = "DROP TABLE IF EXISTS strings;" +
                     $"CREATE TABLE public.strings (char_column {type});";
            var db = CreateModel(sql, new TableSelectionSet(new List<string> { "strings" }));

            Assert.Equal(length, db.Tables.Single().Columns.Single().MaxLength);
        }

        [Fact]
        public void It_reads_pk()
        {
            var sql = "CREATE TABLE pks (id int PRIMARY KEY, non_id int)";
            var dbModel = CreateModel(sql, new TableSelectionSet(new List<string> { "pks" }));

            var columns = dbModel.Tables.Single().Columns.OrderBy(c => c.Ordinal);
            Assert.Collection(columns,
                id =>
                {
                    Assert.Equal("id", id.Name);
                    Assert.Equal(1, id.PrimaryKeyOrdinal);
                },
                nonId =>
                {
                    Assert.Equal("non_id", nonId.Name);
                    Assert.Null(nonId.PrimaryKeyOrdinal);
                });
        }

        [Fact]
        public void It_filters_tables()
        {
            var sql = @"CREATE TABLE public.k2 (id int, a varchar, UNIQUE (a));" +
                      @"CREATE TABLE public.kilimanjaro (id int, b varchar, UNIQUE (b), FOREIGN KEY (b) REFERENCES k2 (a));";

            var selectionSet = new TableSelectionSet(new List<string> { "k2" });

            var dbModel = CreateModel(sql, selectionSet);
            var table = Assert.Single(dbModel.Tables);
            Assert.Equal("k2", table.Name);
            Assert.Equal(2, table.Columns.Count);
            Assert.Equal(1, table.Indexes.Count);
            Assert.Empty(table.ForeignKeys);
        }

        [Fact]
        public void It_reads_sequences()
        {
            var sql = @"CREATE SEQUENCE ""DefaultValues_ascending_read"";
 
CREATE SEQUENCE ""DefaultValues_descending_read"" INCREMENT BY -1;

CREATE SEQUENCE ""CustomSequence_read""
    START WITH 1 
    INCREMENT BY 2 
    MAXVALUE 8 
    MINVALUE -3 
    CYCLE;";

            var dbModel = CreateModel(sql);
            Assert.Collection(dbModel.Sequences.Where(s => s.Name.EndsWith("_read")).OrderBy(s => s.Name),
                c =>
                    {
                        Assert.Equal(c.Name, "CustomSequence_read");
                        Assert.Equal(c.SchemaName, "public");
                        Assert.Equal(c.DataType, "bigint");
                        Assert.Equal(1, c.Start);
                        Assert.Equal(2, c.IncrementBy);
                        Assert.Equal(8, c.Max);
                        Assert.Equal(-3, c.Min);
                        Assert.True(c.IsCyclic);
                    },
                da =>
                    {
                        Assert.Equal(da.Name, "DefaultValues_ascending_read");
                        Assert.Equal(da.SchemaName, "public");
                        Assert.Equal(da.DataType, "bigint");
                        Assert.Equal(1, da.IncrementBy);
                        Assert.False(da.IsCyclic);
                        Assert.Null(da.Max);
                        Assert.Null(da.Min);
                        Assert.Null(da.Start);
                    },
                dd =>
                {
                    Assert.Equal(dd.Name, "DefaultValues_descending_read");
                    Assert.Equal(dd.SchemaName, "public");
                    Assert.Equal(dd.DataType, "bigint");
                    Assert.Equal(-1, dd.IncrementBy);
                    Assert.False(dd.IsCyclic);
                    Assert.Null(dd.Max);
                    Assert.Null(dd.Min);
                    Assert.Null(dd.Start);
                });
        }

        // Sequences which belong to a serial column should not get reverse engineered
        [Fact]
        public void Serial_sequences()
        {
            Assert.Empty(CreateModel(@"CREATE TABLE ""Foo"" (""FooId"" serial primary key);").Sequences);
        }

        private readonly NpgsqlDatabaseModelFixture _fixture;

        public DatabaseModel CreateModel(string createSql, TableSelectionSet selection = null)
            => _fixture.CreateModel(createSql, selection);

        public NpgsqlDatabaseModelFactoryTest(NpgsqlDatabaseModelFixture fixture)
        {
            _fixture = fixture;
        }
    }

    public class NpgsqlDatabaseModelFixture : IDisposable
    {
        private readonly NpgsqlTestStore _testStore;

        public NpgsqlDatabaseModelFixture()
        {
            _testStore = NpgsqlTestStore.CreateScratch();
        }

        public DatabaseModel CreateModel(string createSql, TableSelectionSet selection = null)
        {
            _testStore.ExecuteNonQuery(createSql);

            var reader = new NpgsqlDatabaseModelFactory(new LoggerFactory());

            return reader.Create(_testStore.Connection.ConnectionString, selection ?? TableSelectionSet.All);
        }

        public void ExecuteNonQuery(string sql) => _testStore.ExecuteNonQuery(sql);

        public void Dispose()
        {
            _testStore.Dispose();
        }
    }
}
