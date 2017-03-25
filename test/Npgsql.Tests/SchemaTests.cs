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
using System.Data.Common;
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
    }
}

#endif