using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class SchemaTests : SyncOrAsyncTestBase
{
    [Test]
    public async Task MetaDataCollections()
    {
        await using var conn = await OpenConnectionAsync();

        var metaDataCollections = await GetSchema(conn, DbMetaDataCollectionNames.MetaDataCollections);
        Assert.That(metaDataCollections.Rows, Has.Count.GreaterThan(0));

        foreach (var row in metaDataCollections.Rows.OfType<DataRow>())
        {
            var collectionName = (string)row!["CollectionName"];
            Assert.That(await GetSchema(conn, collectionName), Is.Not.Null, $"Collection {collectionName} advertise in MetaDataCollections but is null");
        }
    }

    [Test, Description("Calling GetSchema() without a parameter should be the same as passing MetaDataCollections")]
    public async Task No_parameter()
    {
        await using var conn = await OpenConnectionAsync();

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
    public async Task Case_insensitive_collection_name()
    {
        await using var conn = await OpenConnectionAsync();

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
        await using var conn = await OpenConnectionAsync();
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
        await using var adminConnection = await OpenConnectionAsync();
        var enumType = await GetTempTypeName(adminConnection);
        var compositeType = await GetTempTypeName(adminConnection);
        var domainType = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {enumType} AS ENUM ('a', 'b');
CREATE TYPE {compositeType} AS (a INTEGER);
CREATE DOMAIN {domainType} AS TEXT");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<TestEnum>(enumType);
        dataSourceBuilder.MapComposite<TestComposite>(compositeType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var dataTable = await GetSchema(connection, DbMetaDataCollectionNames.MetaDataCollections);
        var metadata = dataTable.Rows
            .Cast<DataRow>()
            .Single(r => r["CollectionName"].Equals("DataTypes"));
        Assert.That(metadata["NumberOfRestrictions"], Is.Zero);
        Assert.That(metadata["NumberOfIdentifierParts"], Is.Zero);

        var dataTypes = await GetSchema(connection, DbMetaDataCollectionNames.DataTypes);

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

        var enumRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith("." + enumType));
        Assert.That(enumRow["DataType"], Is.EqualTo("Npgsql.Tests.SchemaTests+TestEnum"));
        Assert.That(enumRow["ProviderDbType"], Is.SameAs(DBNull.Value));

        var compositeRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith("." + compositeType));
        Assert.That(compositeRow["DataType"], Is.EqualTo("Npgsql.Tests.SchemaTests+TestComposite"));
        Assert.That(compositeRow["ProviderDbType"], Is.SameAs(DBNull.Value));

        var domainRow = dataTypes.Rows.Cast<DataRow>().Single(r => ((string)r["TypeName"]).EndsWith("." + domainType));
        Assert.That(domainRow["DataType"], Is.EqualTo("System.String"));
        Assert.That(domainRow["ProviderDbType"], Is.EqualTo((int)NpgsqlDbType.Text));
        Assert.That(domainRow["IsBestMatch"], Is.False);
    }

    enum TestEnum { A, B };

    class TestComposite { public int A { get; set; } }

    [Test]
    public async Task Restrictions()
    {
        await using var conn = await OpenConnectionAsync();
        var restrictions = await GetSchema(conn, DbMetaDataCollectionNames.Restrictions);
        Assert.That(restrictions.Rows, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task ReservedWords()
    {
        await using var conn = await OpenConnectionAsync();
        var reservedWords = await GetSchema(conn, DbMetaDataCollectionNames.ReservedWords);
        Assert.That(reservedWords.Rows, Has.Count.GreaterThan(0));
    }

    [Test]
    public async Task ForeignKeys()
    {
        await using var conn = await OpenConnectionAsync();
        var dt = await GetSchema(conn, "ForeignKeys");
        Assert.IsNotNull(dt);
    }

    [Test]
    public async Task ParameterMarkerFormat()
    {
        await using var conn = await OpenConnectionAsync();

        var table = await CreateTempTable(conn, "int INTEGER");
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (int) VALUES (4)");

        var dt = await GetSchema(conn, "DataSourceInformation");
        var parameterMarkerFormat = (string)dt.Rows[0]["ParameterMarkerFormat"];

        await using var command = conn.CreateCommand();
        const string parameterName = "@p_int";
        command.CommandText = $"SELECT * FROM {table} WHERE int=" + string.Format(parameterMarkerFormat, parameterName);
        command.Parameters.Add(new NpgsqlParameter(parameterName, 4));
        await using var reader = await command.ExecuteReaderAsync();
        Assert.IsTrue(reader.Read());
    }

    [Test]
    public async Task Precision_and_scale()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(
            conn, "explicit_both NUMERIC(10,2), explicit_precision NUMERIC(10), implicit_both NUMERIC, integer INTEGER, text TEXT");

        var dataTable = await GetSchema(conn, "Columns", new[] { null, null, table });
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
    public async Task No_system_tables()
    {
        await using var conn = await OpenConnectionAsync();

        var dataTable = await GetSchema(conn, "Tables");
        var tables = dataTable.Rows
            .Cast<DataRow>()
            .Select(r => (string)r["TABLE_NAME"])
            .ToList();
        Assert.That(tables, Does.Not.Contain("pg_type"));  // schema pg_catalog
        Assert.That(tables, Does.Not.Contain("tables"));   // schema information_schema

        dataTable = await GetSchema(conn, "Views");
        var views = dataTable.Rows
            .Cast<DataRow>()
            .Select(r => (string)r["TABLE_NAME"])
            .ToList();
        Assert.That(views, Does.Not.Contain("pg_user"));  // schema pg_catalog
        Assert.That(views, Does.Not.Contain("views"));    // schema information_schema
    }

    [Test]
    public async Task GetSchema_tables_with_restrictions()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "bar INTEGER");

        var dt = await GetSchema(conn, "Tables",  new[] { null, null, table });
        foreach (var row in dt.Rows.OfType<DataRow>())
            Assert.That(row["table_name"], Is.EqualTo(table));
    }

    [Test]
    public async Task GetSchema_views_with_restrictions()
    {
        await using var conn = await OpenConnectionAsync();
        var view = await GetTempViewName(conn);

        await conn.ExecuteNonQueryAsync($"CREATE VIEW {view} AS SELECT 8 AS foo");

        var dt = await GetSchema(conn, "Views", new[] { null, null, view });
        foreach (var row in dt.Rows.OfType<DataRow>())
            Assert.That(row["table_name"], Is.EqualTo(view));
    }

    [Test]
    public async Task Primary_key()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "id INT PRIMARY KEY, f1 INT");

        var dataTable = await GetSchema(conn, "CONSTRAINTCOLUMNS", new[] { null, null, table });
        var column = dataTable.Rows.Cast<DataRow>().Single();

        Assert.That(column["table_schema"], Is.EqualTo("public"));
        Assert.That(column["table_name"], Is.EqualTo(table));
        Assert.That(column["column_name"], Is.EqualTo("id"));
        Assert.That(column["constraint_type"], Is.EqualTo("PRIMARY KEY"));
    }

    [Test]
    public async Task Primary_key_composite()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "id1 INT, id2 INT, f1 INT, PRIMARY KEY (id1, id2)");

        var dataTable = await GetSchema(conn, "CONSTRAINTCOLUMNS", new[] { null, null, table });
        var columns = dataTable.Rows.Cast<DataRow>().OrderBy(r => r["ordinal_number"]).ToList();

        Assert.That(columns.All(r => r["table_schema"].Equals("public")));
        Assert.That(columns.All(r => r["table_name"].Equals(table)));
        Assert.That(columns.All(r => r["constraint_type"].Equals("PRIMARY KEY")));

        Assert.That(columns[0]["column_name"], Is.EqualTo("id1"));
        Assert.That(columns[1]["column_name"], Is.EqualTo("id2"));
    }

    [Test]
    public async Task Unique_constraint()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "f1 INT, f2 INT, UNIQUE (f1, f2)");

        var database = await conn.ExecuteScalarAsync("SELECT current_database()");

        var dataTable = await GetSchema(conn, "CONSTRAINTCOLUMNS", new[] { null, null, table });
        var columns = dataTable.Rows.Cast<DataRow>().ToList();

        Assert.That(columns.All(r => r["constraint_catalog"].Equals(database)));
        Assert.That(columns.All(r => r["constraint_schema"].Equals("public")));
        Assert.That(columns.All(r => r["constraint_name"] is not null));
        Assert.That(columns.All(r => r["table_catalog"].Equals(database)));
        Assert.That(columns.All(r => r["table_schema"].Equals("public")));
        Assert.That(columns.All(r => r["table_name"].Equals(table)));
        Assert.That(columns.All(r => r["constraint_type"].Equals("UNIQUE KEY")));

        Assert.That(columns[0]["column_name"], Is.EqualTo("f1"));
        Assert.That(columns[0]["ordinal_number"], Is.EqualTo(1));

        Assert.That(columns[1]["column_name"], Is.EqualTo("f2"));
        Assert.That(columns[1]["ordinal_number"], Is.EqualTo(2));
    }

    [Test]
    public async Task Unique_index_composite()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await GetTempTableName(conn);
        var constraint = table + "_uq";
        await conn.ExecuteNonQueryAsync(@$"
CREATE TABLE {table} (
    f1 INT,
    f2 INT,
    CONSTRAINT {constraint} UNIQUE (f1, f2)
)");

        var database = await conn.ExecuteScalarAsync("SELECT current_database()");

        var dataTable = await GetSchema(conn, "INDEXES", new[] { null, null, table });
        var index = dataTable.Rows.Cast<DataRow>().Single();

        Assert.That(index["table_schema"], Is.EqualTo("public"));
        Assert.That(index["table_name"], Is.EqualTo(table));
        Assert.That(index["index_name"], Is.EqualTo(constraint));
        Assert.That(index["type_desc"], Is.EqualTo(""));

        string[] indexColumnRestrictions = { null!, null!, table };
        var dataTable2 = await GetSchema(conn, "INDEXCOLUMNS", indexColumnRestrictions);
        var columns = dataTable2.Rows.Cast<DataRow>().ToList();

        Assert.That(columns.All(r => r["constraint_catalog"].Equals(database)));
        Assert.That(columns.All(r => r["constraint_schema"].Equals("public")));
        Assert.That(columns.All(r => r["constraint_name"].Equals(constraint)));
        Assert.That(columns.All(r => r["table_catalog"].Equals(database)));
        Assert.That(columns.All(r => r["table_schema"].Equals("public")));
        Assert.That(columns.All(r => r["table_name"].Equals(table)));

        Assert.That(columns[0]["column_name"], Is.EqualTo("f1"));
        Assert.That(columns[1]["column_name"], Is.EqualTo("f2"));

        string[] indexColumnRestrictions3 = { (string) database! , "public", table, constraint, "f1" };
        var dataTable3 = await GetSchema(conn, "INDEXCOLUMNS", indexColumnRestrictions3);
        var columns3 = dataTable3.Rows.Cast<DataRow>().ToList();
        Assert.That(columns3.Count, Is.EqualTo(1));
        Assert.That(columns3.Single()["column_name"], Is.EqualTo("f1"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1886")]
    public async Task Column_schema_data_types()
    {
        await using var conn = await OpenConnectionAsync();

        var columnDefinition = @"
p0 integer PRIMARY KEY NOT NULL,
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
cid cid";
        var table = await CreateTempTable(conn, columnDefinition);

        var columnsSchema = await GetSchema(conn, "Columns", new[] { null, null, table });
        var columns = columnsSchema.Rows.Cast<DataRow>().ToList();

        var dataTypes = await GetSchema(conn, DbMetaDataCollectionNames.DataTypes);

        var nonMatching = columns.FirstOrDefault(col => !dataTypes.Rows.Cast<DataRow>().Any(row => row["TypeName"].Equals(col["data_type"])));
        if (nonMatching is not null)
            Assert.Fail($"Could not find matching data type for column {nonMatching["column_name"]} with type {nonMatching["data_type"]}");
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4392")]
    public async Task Enum_in_public_schema()
    {
        await using var conn = await OpenConnectionAsync();
        var enumName = await GetTempTypeName(conn);
        var table = await GetTempTableName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE TYPE {enumName} AS ENUM ('red', 'yellow', 'blue');
CREATE TABLE {table} (color {enumName});");

        var dataTable = await GetSchema(conn, "Columns", new[] { null, null, table });
        var row = dataTable.Rows.Cast<DataRow>().Single();
        Assert.That(row["data_type"], Is.EqualTo(enumName));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4392")]
    public async Task Enum_in_non_public_schema()
    {
        await using var conn = await OpenConnectionAsync();
        const string enumName = "my_enum";
        var schema = await CreateTempSchema(conn);
        var table = await GetTempTableName(conn);

        await conn.ExecuteNonQueryAsync($@"
CREATE TYPE {schema}.{enumName} AS ENUM ('red', 'yellow', 'blue');
CREATE TABLE {table} (color {schema}.{enumName});");

        var dataTable = await GetSchema(conn, "Columns", new[] { null, null, table });
        var row = dataTable.Rows.Cast<DataRow>().Single();
        Assert.That(row["data_type"], Is.EqualTo($"{schema}.{enumName}"));
    }

    public SchemaTests(SyncOrAsync syncOrAsync) : base(syncOrAsync) { }

    // ReSharper disable MethodHasAsyncOverload
    async Task<DataTable> GetSchema(NpgsqlConnection conn)
        => IsAsync ? await conn.GetSchemaAsync() : conn.GetSchema();

    async Task<DataTable> GetSchema(NpgsqlConnection conn, string collectionName)
        => IsAsync ? await conn.GetSchemaAsync(collectionName) : conn.GetSchema(collectionName);

    async Task<DataTable> GetSchema(NpgsqlConnection conn, string collectionName, string?[] restrictions)
        => IsAsync ? await conn.GetSchemaAsync(collectionName, restrictions) : conn.GetSchema(collectionName, restrictions);
    // ReSharper restore MethodHasAsyncOverload
}
