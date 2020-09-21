using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

#pragma warning disable 8602 // Warning should be removable after rc2 (https://github.com/dotnet/runtime/pull/42215)

namespace Npgsql.Tests
{
    public class SchemaTests : SyncOrAsyncTestBase
    {
        [Test]
        public async Task MetaDataCollectionNames()
        {
            using var conn = OpenConnection();
            var metaDataCollections = await GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);
            Assert.That(metaDataCollections.Rows, Has.Count.GreaterThan(0));
            foreach (var row in metaDataCollections.Rows.OfType<DataRow>())
            {
                var collectionName = (string)row!["CollectionName"];
                Assert.That(conn.GetSchema(collectionName), Is.Not.Null, $"Collection {collectionName} advertise in MetaDataCollections but is null");
            }
        }

        [Test, Description("Calling GetSchema() without a parameter should be the same as passing MetaDataCollections")]
        public async Task NoParameter()
        {
            using var conn = OpenConnection();
            var dataTable1 = await GetSchema(conn);
            var collections1 = dataTable1.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();
            var dataTable2 = await GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);
            var collections2 = dataTable2.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();
            Assert.That(collections1, Is.EquivalentTo(collections2));
        }

        [Test, Description("Calling GetSchema(collectionName [, restrictions]) case insensive collectionName can be used")]
        public async Task CaseInsensitiveCollectionName()
        {
            using var conn = OpenConnection();
            var dataTable1 = await GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);
            var collections1 = dataTable1.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            var dataTable2 = await GetSchema(conn, "METADATACOLLECTIONS");
            var collections2 = dataTable2.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            var dataTable3 = await GetSchema(conn, "metadatacollections");
            var collections3 = dataTable3.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            var dataTable4 = await GetSchema(conn, "MetaDataCollections");
            var collections4 = dataTable4.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            var dataTable5 = await GetSchema(conn, "METADATACOLLECTIONS", null!);
            var collections5 = dataTable5.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            var dataTable6 = await GetSchema(conn, "metadatacollections", null!);
            var collections6 = dataTable6.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            var dataTable7 = await GetSchema(conn, "MetaDataCollections", null!);
            var collections7 = dataTable7.Rows
                .Cast<DataRow>()
                .Select(r => (string)r["CollectionName"])
                .ToList();

            Assert.That(collections1, Is.EquivalentTo(collections2));
            Assert.That(collections1, Is.EquivalentTo(collections3));
            Assert.That(collections1, Is.EquivalentTo(collections4));
            Assert.That(collections1, Is.EquivalentTo(collections5));
            Assert.That(collections1, Is.EquivalentTo(collections6));
            Assert.That(collections1, Is.EquivalentTo(collections7));
        }

        [Test]
        public async Task DataSourceInformation()
        {
            using var conn = OpenConnection();
            var dataTable = await GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);
            var metadata = dataTable.Rows
                .Cast<DataRow>()
                .Single(r => r["CollectionName"].Equals("DataSourceInformation"));
            Assert.That(metadata["NumberOfRestrictions"], Is.Zero);
            Assert.That(metadata["NumberOfIdentifierParts"], Is.Zero);

            var dataSourceInfo = await GetSchema(conn, DbMetaDataCollectionNames.DataSourceInformation);
            var row = dataSourceInfo.Rows.Cast<DataRow>().Single();

            Assert.That(row["DataSourceProductName"], Is.EqualTo("Npgsql"));

            var pgVersion = conn.PostgreSqlVersion;
            Assert.That(row["DataSourceProductVersion"], Is.EqualTo(pgVersion.ToString()));

            var parsedNormalizedVersion = Version.Parse((string)row["DataSourceProductVersionNormalized"]);
            Assert.That(parsedNormalizedVersion, Is.EqualTo(conn.PostgreSqlVersion));

            Assert.That(Regex.Match("\"some_identifier\"", (string)row["QuotedIdentifierPattern"]).Groups[1].Value,
                Is.EqualTo("some_identifier"));
        }

        [Test]
        public async Task DataTypes()
        {
            using var conn = OpenConnection();
            conn.ExecuteNonQuery("CREATE TYPE pg_temp.test_enum AS ENUM ('a', 'b')");
            conn.ExecuteNonQuery("CREATE TYPE pg_temp.test_composite AS (a INTEGER)");
            conn.ExecuteNonQuery("CREATE DOMAIN pg_temp.us_postal_code AS TEXT");
            conn.ReloadTypes();
            conn.TypeMapper.MapEnum<TestEnum>();
            conn.TypeMapper.MapComposite<TestComposite>();

            var dataTable = await GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);
            var metadata = dataTable.Rows
                .Cast<DataRow>()
                .Single(r => r["CollectionName"].Equals("DataTypes"));
            Assert.That(metadata["NumberOfRestrictions"], Is.Zero);
            Assert.That(metadata["NumberOfIdentifierParts"], Is.Zero);

            var dataTypes = await GetSchema(conn, DbMetaDataCollectionNames.DataTypes);

            var intRow = dataTypes.Rows.Cast<DataRow>().Single(r => r["TypeName"].Equals("integer"));
            Assert.That(intRow["DataType"], Is.EqualTo("System.Int32"));
            Assert.That(intRow["ProviderDbType"], Is.EqualTo((int)NpgsqlDbType.Integer));
            Assert.That(intRow["IsUnsigned"], Is.False);
            Assert.That(intRow["OID"], Is.EqualTo(23));

            var textRow = dataTypes.Rows.Cast<DataRow>().Single(r => r["TypeName"].Equals("text"));
            Assert.That(textRow["DataType"], Is.EqualTo("System.String"));
            Assert.That(textRow["ProviderDbType"], Is.EqualTo((int)NpgsqlDbType.Text));
            Assert.That(textRow["IsUnsigned"], Is.SameAs(DBNull.Value));
            Assert.That(textRow["OID"], Is.EqualTo(25));

            var numericRow = dataTypes.Rows.Cast<DataRow>().Single(r => r["TypeName"].Equals("numeric"));
            Assert.That(numericRow["DataType"], Is.EqualTo("System.Decimal"));
            Assert.That(numericRow["ProviderDbType"], Is.EqualTo((int)NpgsqlDbType.Numeric));
            Assert.That(numericRow["IsUnsigned"], Is.False);
            Assert.That(numericRow["OID"], Is.EqualTo(1700));
            Assert.That(numericRow["CreateFormat"], Is.EqualTo("NUMERIC({0},{1})"));
            Assert.That(numericRow["CreateParameters"], Is.EqualTo("precision, scale"));

            var intArrayRow = dataTypes.Rows.Cast<DataRow>().Single(r => r["TypeName"].Equals("integer[]"));
            Assert.That(intArrayRow["DataType"], Is.EqualTo("System.Int32[]"));
            Assert.That(intArrayRow["ProviderDbType"], Is.EqualTo((int)(NpgsqlDbType.Integer | NpgsqlDbType.Array)));
            Assert.That(intArrayRow["OID"], Is.EqualTo(1007));
            Assert.That(intArrayRow["CreateFormat"], Is.EqualTo("INTEGER[]"));

            var numericArrayRow = dataTypes.Rows.Cast<DataRow>().Single(r => r["TypeName"].Equals("numeric[]"));
            Assert.That(numericArrayRow["CreateFormat"], Is.EqualTo("NUMERIC({0},{1})[]"));
            Assert.That(numericArrayRow["CreateParameters"], Is.EqualTo("precision, scale"));

            var intRangeRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith("int4range"));
            Assert.That(intRangeRow["DataType"], Does.StartWith("NpgsqlTypes.NpgsqlRange`1[[System.Int32"));
            Assert.That(intRangeRow["ProviderDbType"], Is.EqualTo((int)(NpgsqlDbType.Integer | NpgsqlDbType.Range)));
            Assert.That(intRangeRow["OID"], Is.EqualTo(3904));

            var enumRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith(".test_enum"));
            Assert.That(enumRow["DataType"], Is.EqualTo("Npgsql.Tests.SchemaTests+TestEnum"));
            Assert.That(enumRow["ProviderDbType"], Is.SameAs(DBNull.Value));

            var compositeRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith(".test_composite"));
            Assert.That(compositeRow["DataType"], Is.EqualTo("Npgsql.Tests.SchemaTests+TestComposite"));
            Assert.That(compositeRow["ProviderDbType"], Is.SameAs(DBNull.Value));

            var domainRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith(".us_postal_code"));
            Assert.That(domainRow["DataType"], Is.EqualTo("System.String"));
            Assert.That(domainRow["ProviderDbType"], Is.EqualTo((int)NpgsqlDbType.Text));
            Assert.That(domainRow["IsBestMatch"], Is.False);
        }

        enum TestEnum { A, B };

        class TestComposite { public int A { get; set; } }

        [Test]
        public async Task Restrictions()
        {
            using var conn = OpenConnection();
            var restrictions = await GetSchema(conn, DbMetaDataCollectionNames.Restrictions);
            Assert.That(restrictions.Rows, Has.Count.GreaterThan(0));
        }

        [Test]
        public async Task ReservedWords()
        {
            using var conn = OpenConnection();
            var reservedWords = await GetSchema(conn, DbMetaDataCollectionNames.ReservedWords);
            Assert.That(reservedWords.Rows, Has.Count.GreaterThan(0));
        }

        [Test]
        public async Task ForeignKeys()
        {
            using var conn = OpenConnection();
            var dt = await GetSchema(conn, "ForeignKeys");
            Assert.IsNotNull(dt);
        }

        [Test]
        public async Task ParameterMarkerFormats()
        {
            using var conn = OpenConnection();
            var dt = await GetSchema(conn, "DataSourceInformation");
            var parameterMarkerFormat = (string)dt.Rows[0]["ParameterMarkerFormat"];

            conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
            conn.ExecuteNonQuery("INSERT INTO data (int) VALUES (4)");
            using var command = conn.CreateCommand();
            const string parameterName = "@p_int";
            command.CommandText = "SELECT * FROM data WHERE int=" +
                                    string.Format(parameterMarkerFormat, parameterName);
            command.Parameters.Add(new NpgsqlParameter(parameterName, 4));
            using var reader = command.ExecuteReader();
            Assert.IsTrue(reader.Read());
            // This is OK, when no exceptions are occurred.
        }

        [Test]
        public async Task PrecisionAndScale()
        {
            using var conn = OpenConnection();
            conn.ExecuteNonQuery(@"CREATE TEMP TABLE data (explicit_both NUMERIC(10,2), explicit_precision NUMERIC(10), implicit_both NUMERIC, integer INTEGER, text TEXT)");
            var dataTable = await GetSchema(conn, "Columns");
            var rows = dataTable.Rows.Cast<DataRow>().ToList();

            var explicitBoth = rows.Single(r => (string)r["column_name"] == "explicit_both");
            Assert.That(explicitBoth["numeric_precision"], Is.EqualTo(10));
            Assert.That(explicitBoth["numeric_scale"], Is.EqualTo(2));

            var explicitPrecision = rows.Single(r => (string)r["column_name"] == "explicit_precision");
            Assert.That(explicitPrecision["numeric_precision"], Is.EqualTo(10));
            Assert.That(explicitPrecision["numeric_scale"], Is.EqualTo(0)); // Not good

            // Consider exposing actual precision/scale even for implicit
            var implicitBoth = rows.Single(r => (string)r["column_name"] == "implicit_both");
            Assert.That(implicitBoth["numeric_precision"], Is.EqualTo(DBNull.Value));
            Assert.That(implicitBoth["numeric_scale"], Is.EqualTo(DBNull.Value));

            var integer = rows.Single(r => (string)r["column_name"] == "integer");
            Assert.That(integer["numeric_precision"], Is.EqualTo(32));
            Assert.That(integer["numeric_scale"], Is.EqualTo(0));

            var text = rows.Single(r => (string)r["column_name"] == "text");
            Assert.That(text["numeric_precision"], Is.EqualTo(DBNull.Value));
            Assert.That(text["numeric_scale"], Is.EqualTo(DBNull.Value));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1831")]
        public async Task NoSystemTables()
        {
            using (var conn = OpenConnection())
            {
                var dataTable = await GetSchema(conn, "Tables");
                var tables = dataTable.Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["TABLE_NAME"])
                    .ToList();
                Assert.That(tables, Does.Not.Contain("pg_type"));  // schema pg_catalog
                Assert.That(tables, Does.Not.Contain("tables"));   // schema information_schema
            }

            using (var conn = OpenConnection())
            {
                var dataTable = await GetSchema(conn, "Views");
                var views = dataTable.Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["TABLE_NAME"])
                    .ToList();
                Assert.That(views, Does.Not.Contain("pg_user"));  // schema pg_catalog
                Assert.That(views, Does.Not.Contain("views"));    // schema information_schema
            }
        }

        [Test]
        public async Task GetSchemaWithRestrictions()
        {
            // We can't use temporary tables because GetSchema filters out that in WHERE clause.
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
                conn.ExecuteNonQuery("CREATE TABLE data (bar INTEGER)");

                try
                {
                    string[] restrictions = { null!, null!, "data" };
                    var dt = await GetSchema(conn, "Tables", restrictions);
                    foreach (var row in dt.Rows.OfType<DataRow>())
                    {
                        Assert.That(row["table_name"], Is.EqualTo("data"));
                    }
                }
                finally
                {
                    conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
                }
            }

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP VIEW IF EXISTS view");
                conn.ExecuteNonQuery("CREATE VIEW view AS SELECT 8 AS foo");

                try
                {
                    string[] restrictions = { null!, null!, "view" };
                    var dt = await GetSchema(conn, "Views", restrictions);
                    foreach (var row in dt.Rows.OfType<DataRow>())
                    {
                        Assert.That(row["table_name"], Is.EqualTo("view"));
                    }
                }
                finally
                {
                    conn.ExecuteNonQuery("DROP VIEW IF EXISTS view");
                }
            }
        }

        [Test]
        public async Task PrimaryKey()
        {
            using var conn = OpenConnection();
            try
            {
                conn.ExecuteNonQuery(@"
DROP TABLE IF EXISTS data;
CREATE TABLE data (id INT, f1 INT);
ALTER TABLE data ADD PRIMARY KEY (id);");
                string[] restrictions = { null!, null!, "data" };
                var dataTable = await GetSchema(conn, "CONSTRAINTCOLUMNS", restrictions);
                var column = dataTable.Rows.Cast<DataRow>().Single();

                Assert.That(column["table_schema"], Is.EqualTo("public"));
                Assert.That(column["table_name"], Is.EqualTo("data"));
                Assert.That(column["column_name"], Is.EqualTo("id"));
                Assert.That(column["constraint_type"], Is.EqualTo("PRIMARY KEY"));
            }
            finally
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
            }
        }

        [Test]
        public async Task PrimaryKeyComposite()
        {
            using var conn = OpenConnection();
            try
            {
                conn.ExecuteNonQuery(@"
DROP TABLE IF EXISTS data;
CREATE TABLE data (id1 INT, id2 INT, f1 INT);
ALTER TABLE data ADD PRIMARY KEY (id1, id2);");
                string[] restrictions = { null!, null!, "data" };
                var dataTable = await GetSchema(conn, "CONSTRAINTCOLUMNS", restrictions);
                var columns = dataTable.Rows.Cast<DataRow>()
                    .OrderBy(r => r["ordinal_number"]).ToList();

                Assert.That(columns.All(r => r["table_schema"].Equals("public")));
                Assert.That(columns.All(r => r["table_name"].Equals("data")));
                Assert.That(columns.All(r => r["constraint_type"].Equals("PRIMARY KEY")));

                Assert.That(columns[0]["column_name"], Is.EqualTo("id1"));
                Assert.That(columns[1]["column_name"], Is.EqualTo("id2"));
            }
            finally
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
            }
        }

        [Test]
        public async Task UniqueConstraint()
        {
            using var conn = OpenConnection();
            try
            {
                conn.ExecuteNonQuery(@"
DROP TABLE IF EXISTS data;
CREATE TABLE data (f1 INT, f2 INT);
ALTER TABLE data ADD UNIQUE (f1, f2);");
                string[] restrictions = { null!, null!, "data" };
                var dataTable = await GetSchema(conn, "CONSTRAINTCOLUMNS", restrictions);
                var rows = dataTable.Rows.Cast<DataRow>().ToList();
            }
            finally
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
            }
        }

        [Test]
        public async Task UniqueIndexComposite()
        {
            using var conn = OpenConnection();
            try
            {
                conn.ExecuteNonQuery(@"
DROP TABLE IF EXISTS data;
CREATE TABLE data (f1 INT, f2 INT);
CREATE UNIQUE INDEX idx_unique ON data (f1, f2);
");
                var database = conn.ExecuteScalar("SELECT current_database()");

                string[] restrictions = { null!, null!, "data" };
                var dataTable = await GetSchema(conn, "INDEXES", restrictions);
                var index = dataTable.Rows.Cast<DataRow>().Single();

                Assert.That(index["table_schema"], Is.EqualTo("public"));
                Assert.That(index["table_name"], Is.EqualTo("data"));
                Assert.That(index["index_name"], Is.EqualTo("idx_unique"));
                Assert.That(index["type_desc"], Is.EqualTo(""));

                string[] indexColumnRestrictions = { null!, null!, "data" };
                var dataTable2 = await GetSchema(conn, "INDEXCOLUMNS", indexColumnRestrictions);
                var columns = dataTable2.Rows.Cast<DataRow>().ToList();

                Assert.That(columns.All(r => r["constraint_catalog"].Equals(database)));
                Assert.That(columns.All(r => r["constraint_schema"].Equals("public")));
                Assert.That(columns.All(r => r["constraint_name"].Equals("idx_unique")));
                Assert.That(columns.All(r => r["table_catalog"].Equals(database)));
                Assert.That(columns.All(r => r["table_schema"].Equals("public")));
                Assert.That(columns.All(r => r["table_name"].Equals("data")));
                Assert.That(columns.All(r => r["index_name"].Equals("idx_unique")));

                Assert.That(columns[0]["column_name"], Is.EqualTo("f1"));
                Assert.That(columns[1]["column_name"], Is.EqualTo("f2"));
            }
            finally
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1886")]
        public async Task ColumnSchemaDataTypes()
        {
            using var conn = OpenConnection();
            try
            {
                conn.ExecuteNonQuery(@"
DROP TABLE IF EXISTS types_table;
CREATE TABLE types_table
(
  p0 integer NOT NULL,
  achar char,
  char character(3),
  vchar character varying(10),
  text text,
  bytea bytea,
  abit bit(1),
  bit bit(3),
  vbit bit varying(5),
  boolean boolean,
  smallint smallint,
  integer integer,
  bigint bigint,
  real real,
  double double precision,
  numeric numeric,
  money money,
  date date,
  timetz time with time zone,
  timestamptz timestamp with time zone,
  time time without time zone,
  timestamp timestamp without time zone,
  point point,
  box box,
  lseg lseg,
  path path,
  polygon polygon,
  circle circle,
  line line,
  inet inet,
  macaddr macaddr,
  uuid uuid,
  interval interval,
  name name,
  refcursor refcursor,
  numrange numrange,
  oidvector oidvector,
  ""bigint[]"" bigint[],
  cidr cidr,
  maccaddr8 macaddr8,
  jsonb jsonb,
  json json,
  xml xml,
  tsvector tsvector,
  tsquery tsquery,
  tid tid,
  xid xid,
  cid cid,
  CONSTRAINT types_table_pkey PRIMARY KEY(p0)
)
");
                var database = conn.ExecuteScalar("SELECT current_database()");

                string[] restrictions = { "npgsql_tests", "public", "types_table", null! };
                var columnsSchema = await GetSchema(conn, "Columns", restrictions);
                var columns = columnsSchema.Rows.Cast<DataRow>().ToList();

                var dataTypes = await GetSchema(conn, DbMetaDataCollectionNames.DataTypes);

                columns.ForEach(col => Assert.That(dataTypes.Rows.Cast<DataRow>().Any(row => row["TypeName"].Equals(col["data_type"])), Is.True));
            }
            finally
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS types_table");
            }
        }

        public SchemaTests(SyncOrAsync syncOrAsync) : base(syncOrAsync) { }

        private async Task<DataTable> GetSchema(NpgsqlConnection conn)
            => IsAsync ? await conn.GetSchemaAsync() : conn.GetSchema();

        private async Task<DataTable> GetSchema(NpgsqlConnection conn, string collectionName)
            => IsAsync ? await conn.GetSchemaAsync(collectionName) : conn.GetSchema(collectionName);

        private async Task<DataTable> GetSchema(NpgsqlConnection conn, string collectionName, string?[] restrictions)
            => IsAsync ? await conn.GetSchemaAsync(collectionName, restrictions) : conn.GetSchema(collectionName, restrictions);
    }
}
