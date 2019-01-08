using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class SchemaTests : TestBase
    {
        [Test]
        public void MetaDataCollectionNames()
        {
            using (var conn = OpenConnection())
            {
                var metaDataCollections = conn.GetSchema(DbMetaDataCollectionNames.MetaDataCollections);
                Assert.That(metaDataCollections.Rows, Has.Count.GreaterThan(0));
                foreach (DataRow row in metaDataCollections.Rows)
                {
                    var collectionName = (string)row["CollectionName"];
                    Assert.That(conn.GetSchema(collectionName), Is.Not.Null, $"Collection {collectionName} advertise in MetaDataCollections but is null");
                }
            }
        }

        [Test, Description("Calling GetSchema() without a parameter should be the same as passing MetaDataCollections")]
        public void NoParameter()
        {
            using (var conn = OpenConnection())
            {
                var collections1 = conn.GetSchema().Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();
                var collections2 = conn.GetSchema(DbMetaDataCollectionNames.MetaDataCollections).Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();
                Assert.That(collections1, Is.EquivalentTo(collections2));
            }
        }

        [Test, Description("Calling GetSchema(collectionName [, restrictions]) case insensive collectionName can be used")]
        public void CaseInsensitiveCollectionName()
        {
            using (var conn = OpenConnection())
            {
                var collections1 = conn.GetSchema(DbMetaDataCollectionNames.MetaDataCollections).Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();

                var collections2 = conn.GetSchema("METADATACOLLECTIONS").Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();

                var collections3 = conn.GetSchema("metadatacollections").Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();

                var collections4 = conn.GetSchema("MetaDataCollections").Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();

                var collections5 = conn.GetSchema("METADATACOLLECTIONS", null).Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();

                var collections6 = conn.GetSchema("metadatacollections", null).Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["CollectionName"])
                    .ToList();

                var collections7 = conn.GetSchema("MetaDataCollections", null).Rows
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
        }

        [Test]
        public void DataSourceInformation()
        {
            using (var conn = OpenConnection())
            {
                var metadata = conn.GetSchema(DbMetaDataCollectionNames.MetaDataCollections).Rows
                    .Cast<DataRow>()
                    .Single(r => r["CollectionName"].Equals("DataSourceInformation"));
                Assert.That(metadata["NumberOfRestrictions"], Is.Zero);
                Assert.That(metadata["NumberOfIdentifierParts"], Is.Zero);

                var dataSourceInfo = conn.GetSchema(DbMetaDataCollectionNames.DataSourceInformation);
                var row = dataSourceInfo.Rows.Cast<DataRow>().Single();

                Assert.That(row["DataSourceProductName"], Is.EqualTo("Npgsql"));

                var pgVersion = conn.PostgreSqlVersion;
                Assert.That(row["DataSourceProductVersion"], Is.EqualTo(pgVersion.ToString()));

                var parsedNormalizedVersion = Version.Parse((string)row["DataSourceProductVersionNormalized"]);
                Assert.That(parsedNormalizedVersion, Is.EqualTo(conn.PostgreSqlVersion));

                Assert.That(Regex.Match("\"some_identifier\"", (string)row["QuotedIdentifierPattern"]).Groups[1].Value,
                    Is.EqualTo("some_identifier"));
            }
        }

        [Test]
        public void DataTypes()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.test_enum AS ENUM ('a', 'b')");
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.test_composite AS (a INTEGER)");
                conn.ExecuteNonQuery("CREATE DOMAIN pg_temp.us_postal_code AS TEXT");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<TestEnum>();
                conn.TypeMapper.MapComposite<TestComposite>();

                var metadata = conn.GetSchema(DbMetaDataCollectionNames.MetaDataCollections).Rows
                    .Cast<DataRow>()
                    .Single(r => r["CollectionName"].Equals("DataTypes"));
                Assert.That(metadata["NumberOfRestrictions"], Is.Zero);
                Assert.That(metadata["NumberOfIdentifierParts"], Is.Zero);

                var dataTypes = conn.GetSchema(DbMetaDataCollectionNames.DataTypes);

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
        }

        enum TestEnum { A, B };

        class TestComposite { int A { get; set; } }

        [Test]
        public void Restrictions()
        {
            using (var conn = OpenConnection())
            {
                var restrictions = conn.GetSchema(DbMetaDataCollectionNames.Restrictions);
                Assert.That(restrictions.Rows, Has.Count.GreaterThan(0));
            }
        }

        [Test]
        public void ReservedWords()
        {
            using (var conn = OpenConnection())
            {
                var reservedWords = conn.GetSchema(DbMetaDataCollectionNames.ReservedWords);
                Assert.That(reservedWords.Rows, Has.Count.GreaterThan(0));
            }
        }

        [Test]
        public void ForeignKeys()
        {
            using (var conn = OpenConnection())
            {
                var dt = conn.GetSchema("ForeignKeys");
                Assert.IsNotNull(dt);
            }
        }

        [Test]
        public void ParameterMarkerFormats()
        {
            using (var conn = OpenConnection())
            {
                var dt = conn.GetSchema("DataSourceInformation");
                var parameterMarkerFormat = (string)dt.Rows[0]["ParameterMarkerFormat"];

                conn.ExecuteNonQuery("CREATE TEMP TABLE data (int INTEGER)");
                conn.ExecuteNonQuery("INSERT INTO data (int) VALUES (4)");
                using (var command = conn.CreateCommand())
                {
                    const string parameterName = "@p_int";
                    command.CommandText = "SELECT * FROM data WHERE int=" +
                                            String.Format(parameterMarkerFormat, parameterName);
                    command.Parameters.Add(new NpgsqlParameter(parameterName, 4));
                    using (var reader = command.ExecuteReader())
                    {
                        Assert.IsTrue(reader.Read());
                        // This is OK, when no exceptions are occurred.
                    }
                }
            }
        }

        [Test]
        public void PrecisionAndScale()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE TEMP TABLE data (explicit_both NUMERIC(10,2), explicit_precision NUMERIC(10), implicit_both NUMERIC, integer INTEGER, text TEXT)");
                var rows = conn.GetSchema("Columns").Rows.Cast<DataRow>().ToList();

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
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1831")]
        public void NoSystemTables()
        {
            using (var conn = OpenConnection())
            {
                var tables = conn.GetSchema("Tables").Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["TABLE_NAME"])
                    .ToList();
                Assert.That(tables, Does.Not.Contain("pg_type"));  // schema pg_catalog
                Assert.That(tables, Does.Not.Contain("tables"));   // schema information_schema
            }

            using (var conn = OpenConnection())
            {
                var views = conn.GetSchema("Views").Rows
                    .Cast<DataRow>()
                    .Select(r => (string)r["TABLE_NAME"])
                    .ToList();
                Assert.That(views, Does.Not.Contain("pg_user"));  // schema pg_catalog
                Assert.That(views, Does.Not.Contain("views"));    // schema information_schema
            }
        }

        [Test]
        public void GetSchemaWithRestrictions()
        {
            // We can't use temporary tables because GetSchema filters out that in WHERE clause.
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
                conn.ExecuteNonQuery("CREATE TABLE data (bar INTEGER)");

                try
                {
                    string[] restrictions = { null, null, "data" };
                    var dt = conn.GetSchema("Tables", restrictions);
                    foreach (DataRow row in dt.Rows)
                    {
                        var d = row["table_name"];
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
                    string[] restrictions = { null, null, "view" };
                    var dt = conn.GetSchema("Views", restrictions);
                    foreach (DataRow row in dt.Rows)
                    {
                        var d = row["table_name"];
                        Assert.That(row["table_name"], Is.EqualTo("view"));
                    }
                }
                finally
                {
                    conn.ExecuteNonQuery("DROP VIEW IF EXISTS view");
                }
            }
        }
    }
}
