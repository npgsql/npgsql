#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

#if NET451

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Tests;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class SchemaTests : TestBase
    {
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

        [Test]
        public void GetSchema()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema();
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more metadatacollections returned. No connectionstring is required.");
            }
        }

        [Test]
        public void GetSchemaWithDbMetaDataCollectionNames()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more metadatacollections returned.");
                foreach (DataRow row in metaDataCollections.Rows)
                {
                    var collectionName = (string)row["CollectionName"];
                    //checking this collection
                    if (collectionName != System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections)
                    {
                        var collection = conn.GetSchema(collectionName);
                        Assert.IsNotNull(collection, "Each of the advertised metadata collections should work");
                    }
                }
            }
        }

        [Test]
        public void GetSchemaWithRestrictions()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more Restrictions returned.");
            }
        }

        [Test]
        public void GetSchemaWithReservedWords()
        {
            using (var conn = OpenConnection())
            {
                DataTable metaDataCollections = conn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                Assert.IsTrue(metaDataCollections.Rows.Count > 0, "There should be one or more ReservedWords returned.");
            }
        }

        [Test]
        public void GetSchemaForeignKeys()
        {
            using (var conn = OpenConnection())
            {
                var dt = conn.GetSchema("ForeignKeys");
                Assert.IsNotNull(dt);
            }
        }

        [Test]
        public void GetSchemaParameterMarkerFormats()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data; CREATE TABLE data (int INTEGER);");
                conn.ExecuteNonQuery("INSERT INTO data (int) VALUES (4)");
                var dt = conn.GetSchema("DataSourceInformation");
                var parameterMarkerFormat = (string)dt.Rows[0]["ParameterMarkerFormat"];

                using (var conn2 = new NpgsqlConnection(ConnectionString))
                {
                    conn2.Open();
                    using (var command = conn2.CreateCommand())
                    {
                        const String parameterName = "@p_int";
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
        }
    }
}

#endif