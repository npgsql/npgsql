#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    /// <summary>
    /// This tests the .NET Framework DbDataReader schema/metadata API, which returns DataTable.
    /// For the new CoreCLR API, see <see cref="ReaderNewSchemaTests"/>.
    /// </summary>
    public class ReaderOldSchemaTests : TestBase
    {
        [Test]
        public void PrimaryKeyFieldsMetadataSupport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS DATA2 CASCADE");
                conn.ExecuteNonQuery(@"CREATE TEMP TABLE DATA2 (
                                field_pk1                      INT2 NOT NULL,
                                field_pk2                      INT2 NOT NULL,
                                field_serial                   SERIAL,
                                CONSTRAINT data2_pkey PRIMARY KEY (field_pk1, field_pk2)
                                ) WITH OIDS");

                using (var command = new NpgsqlCommand("SELECT * FROM DATA2", conn))
                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    dr.Read();
                    var keyColumns =
                        dr.GetSchemaTable().Rows.Cast<DataRow>().Where(r => (bool)r["IsKey"]).ToArray();
                    Assert.That(keyColumns, Has.Length.EqualTo(2));
                    Assert.That(keyColumns.Count(c => (string)c["ColumnName"] == "field_pk1"), Is.EqualTo(1));
                    Assert.That(keyColumns.Count(c => (string)c["ColumnName"] == "field_pk2"), Is.EqualTo(1));
                }
            }
        }

        [Test]
        public void PrimaryKeyFieldMetadataSupport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY, serial SERIAL)");
                using (var command = new NpgsqlCommand("SELECT * FROM data", conn))
                {
                    using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                    {
                        dr.Read();
                        var metadata = dr.GetSchemaTable();
                        var key = metadata.Rows.Cast<DataRow>().Single(r => (bool)r["IsKey"]);
                        Assert.That(key["ColumnName"], Is.EqualTo("id"));
                    }
                }
            }
        }

        [Test]
        public void IsAutoIncrementMetadataSupport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY)");
                var command = new NpgsqlCommand("SELECT * FROM data", conn);

                using (var dr = command.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    var metadata = dr.GetSchemaTable();
                    Assert.That(metadata.Rows.Cast<DataRow>()
                        .Where(r => ((string)r["ColumnName"]).Contains("serial"))
                        .All(r => (bool)r["IsAutoIncrement"]));
                }
            }
        }

        [Test]
        public void IsReadOnlyMetadataSupport()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id SERIAL PRIMARY KEY, int2 SMALLINT)");
                conn.ExecuteNonQuery("CREATE OR REPLACE TEMPORARY VIEW DataView (id, int2) AS SELECT id, int2 + int2 AS int2 FROM data");

                var command = new NpgsqlCommand("SELECT * FROM DataView", conn);

                using (var dr = command.ExecuteReader())
                {
                    var metadata = dr.GetSchemaTable();

                    foreach (DataRow r in metadata.Rows)
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
                        default:
                            break;
                        }
                    }
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        [Test]
        public void AllowDBNull()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (nullable INTEGER, non_nullable INTEGER NOT NULL)");

                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                using (var metadata = reader.GetSchemaTable())
                {
                    foreach (DataRow row in metadata.Rows)
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
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1027")]
        public void GetSchemaTableWithoutResult()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.NextResult();
                // We're no longer on a result
                Assert.That(reader.GetSchemaTable(), Is.Null);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1093")]
        public void WithoutPersistSecurityInfo()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id INT)");
                using (var cmd = new NpgsqlCommand("SELECT id FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo))
                    reader.GetSchemaTable();
            }
        }

        [Test]
        public void PrecisionAndScale()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1::NUMERIC AS result", conn))
            using (var reader = cmd.ExecuteReader())
            {
                var schemaTable = reader.GetSchemaTable();
                foreach (DataRow myField in schemaTable.Rows)
                {
                    Assert.That(myField["NumericScale"], Is.EqualTo(0));
                    Assert.That(myField["NumericPrecision"], Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void SchemaOnly([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                using (var cmd = new NpgsqlCommand(
                    "SELECT 1 AS some_column;" +
                    "UPDATE data SET name='yo' WHERE 1=0;" +
                    "SELECT 1 AS some_other_column",
                    conn))
                {
                    if (prepare == PrepareOrNot.Prepared)
                        cmd.Prepare();
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        Assert.That(reader.Read(), Is.False);
                        var t = reader.GetSchemaTable();
                        Assert.That(t.Rows[0]["ColumnName"], Is.EqualTo("some_column"));
                        Assert.That(reader.NextResult(), Is.True);
                        Assert.That(reader.Read(), Is.False);
                        t = reader.GetSchemaTable();
                        Assert.That(t.Rows[0]["ColumnName"], Is.EqualTo("some_other_column"));
                        Assert.That(reader.NextResult(), Is.False);
                    }

                    // Close reader in the middle
                    using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                        reader.Read();
                }
            }
        }
    }
}
