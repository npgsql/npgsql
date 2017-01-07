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

using NUnit.Framework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace Npgsql.Tests.Types
{
    [Parallelizable(ParallelScope.None)]
    class CompositeTests : TestBase
    {
        #region Test Types

        class SomeComposite
        {
            public int x { get; set; }
            [PgName("some_text")]
            public string SomeText { get; set; }
        }

        class SomeCompositeContainer
        {
            public int a { get; set; }
            [PgName("contained")]
            public SomeComposite Contained { get; set; }
        }

        struct CompositeStruct
        {
            public int x { get; set; }
            [PgName("some_text")]
            public string SomeText { get; set; }
        }

        #endregion

        [Test, Description("Resolves an enum type handler via the different pathways, with global mapping")]
        public void CompositeTypeResolutionWithGlobalMapping()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(CompositeTypeResolutionWithGlobalMapping),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite1 AS (x int, some_text text)");
                NpgsqlConnection.MapCompositeGlobally<SomeComposite>("composite1");
                try
                {
                    conn.ReloadTypes();

                    // Resolve type by NpgsqlDbType
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Composite) { SpecificType = typeof(SomeComposite), Value = DBNull.Value });
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite1"));
                            Assert.That(reader.IsDBNull(0), Is.True);
                        }
                    }

                    // Resolve type by ClrType (type inference)
                    conn.ReloadTypes();
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = new SomeComposite { x = 8, SomeText = "foo" }});
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite1"));
                        }
                    }

                    // Resolve type by OID (read)
                    conn.ReloadTypes();
                    using (var cmd = new NpgsqlCommand("SELECT ROW(1, 'foo')::COMPOSITE1", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite1"));
                    }
                }
                finally
                {
                    NpgsqlConnection.UnmapCompositeGlobally<SomeComposite>("composite1");
                }
            }
        }

        [Test, Description("Resolves a composite type handler via the different pathways, with late mapping")]
        public void CompositeTypeResolutionWithLateMapping()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(CompositeTypeResolutionWithLateMapping),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite2 AS (x int, some_text text)");
                // Resolve type by NpgsqlDbType
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>("composite2");
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Composite) { SpecificType = typeof(SomeComposite), Value = DBNull.Value });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite2"));
                        Assert.That(reader.IsDBNull(0), Is.True);
                    }
                }

                // Resolve type by ClrType (type inference)
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>("composite2");
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = new SomeComposite { x = 8, SomeText = "foo" } });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite2"));
                    }
                }

                // Resolve type by OID (read)
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>("composite2");
                using (var cmd = new NpgsqlCommand("SELECT ROW(1, 'foo')::COMPOSITE2", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite2"));
                }
            }
        }

        [Test, Parallelizable(ParallelScope.None)]
        public void LateMapping()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite3 AS (x int, some_text text)");
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>("composite3");

                var expected = new SomeComposite {x = 8, SomeText = "foo"};
                using (var cmd = new NpgsqlCommand("SELECT @p1::composite3, @p2::composite3", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Composite) {
                        Value = expected,
                        SpecificType = typeof(SomeComposite)
                    });
                    cmd.Parameters.AddWithValue("p2", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            var actual = reader.GetFieldValue<SomeComposite>(i);
                            Assert.That(actual.x, Is.EqualTo(8));
                            Assert.That(actual.SomeText, Is.EqualTo("foo"));
                        }
                    }
                }
            }
        }

        [Test]
        public void GlobalMapping()
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS composite4");
                    conn.ExecuteNonQuery("CREATE TYPE composite4 AS (x int, some_text text)");
                    conn.ReloadTypes();
                }
                NpgsqlConnection.MapCompositeGlobally<SomeComposite>("composite4");
                using (var conn = OpenConnection())
                {
                    var expected = new SomeComposite { x = 8, SomeText = "foo" };
                    using (var cmd = new NpgsqlCommand($"SELECT @p::composite4", conn))
                    {
                        cmd.Parameters.AddWithValue("p", expected);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            var actual = reader.GetFieldValue<SomeComposite>(0);
                            Assert.That(actual.x, Is.EqualTo(8));
                            Assert.That(actual.SomeText, Is.EqualTo("foo"));
                        }
                    }
                }
            }
            finally
            {
                NpgsqlConnection.UnmapCompositeGlobally<SomeComposite>("composite4");
                using (var conn = OpenConnection())
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS composite4");
            }
        }

        [Test, Description("Tests a composite within another composite")]
        public void Recursive()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite_contained AS (x int, some_text text)");
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite_container AS (a int, contained composite_contained)");
                conn.ReloadTypes();
                // Registration in inverse dependency order should work
                conn.MapComposite<SomeCompositeContainer>("composite_container");
                conn.MapComposite<SomeComposite>("composite_contained");

                var expected = new SomeCompositeContainer {
                    a = 4,
                    Contained = new SomeComposite {x = 8, SomeText = "foo"}
                };

                using (var cmd = new NpgsqlCommand("SELECT @p::composite_container", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<SomeCompositeContainer>(0);
                        Assert.That(actual.a, Is.EqualTo(4));
                        Assert.That(actual.Contained.x, Is.EqualTo(8));
                        Assert.That(actual.Contained.SomeText, Is.EqualTo("foo"));
                    }
                }
            }
        }

        [Test]
        public void Struct()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite_struct AS (x int, some_text text)");
                conn.ReloadTypes();
                conn.MapComposite<CompositeStruct>("composite_struct");

                var expected = new CompositeStruct {x = 8, SomeText = "foo"};
                using (var cmd = new NpgsqlCommand("SELECT @p::composite_struct", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<CompositeStruct>(0);
                        Assert.That(actual.x, Is.EqualTo(8));
                        Assert.That(actual.SomeText, Is.EqualTo("foo"));
                    }
                }
            }
        }

        [Test]
        public void Array()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite5 AS (x int, some_text text)");
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>("composite5");

                var expected = new[] {
                    new SomeComposite {x = 8, SomeText = "foo"},
                    new SomeComposite {x = 9, SomeText = "bar"}
                };

                using (var cmd = new NpgsqlCommand("SELECT @p1::composite5[], @p2::composite5[]", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Composite) {
                        Value = expected,
                        SpecificType = typeof(SomeComposite)
                    });
                    cmd.Parameters.AddWithValue("p2", expected); // Infer
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            var actual = reader.GetFieldValue<SomeComposite[]>(i);
                            Assert.That(actual[0].x, Is.EqualTo(expected[0].x));
                            Assert.That(actual[0].SomeText, Is.EqualTo(expected[0].SomeText));
                            Assert.That(actual[1].x, Is.EqualTo(expected[1].x));
                            Assert.That(actual[1].SomeText, Is.EqualTo(expected[1].SomeText));
                        }
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
        public void NameTranslation()
        {
            var expected = new NameTranslationComposite { Simple = 2, TwoWords = 3, SomeClrName = 4 };
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.name_translation_composite AS (simple int, two_words int, some_database_name int)");
                conn.ReloadTypes();
                conn.MapComposite<NameTranslationComposite>();

                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    var actual = (NameTranslationComposite)cmd.ExecuteScalar();
                    Assert.That(actual.Simple, Is.EqualTo(expected.Simple));
                    Assert.That(actual.TwoWords, Is.EqualTo(expected.TwoWords));
                    Assert.That(actual.SomeClrName, Is.EqualTo(expected.SomeClrName));
                }
            }
        }

        class NameTranslationComposite
        {
            public int Simple { get; set; }
            public int TwoWords { get; set; }
            [PgName("some_database_name")]
            public int SomeClrName { get; set; }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/856")]
        public void Domain()
        {
            var setupSql = @"SET search_path=pg_temp;

CREATE DOMAIN us_postal_code AS TEXT
CHECK
(
    VALUE ~ '^\d{5}$'
    OR VALUE ~ '^\d{5}-\d{4}$'
);

CREATE TYPE address AS
(
    street TEXT,
    postal_code us_postal_code
)";
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(setupSql);
                conn.ReloadTypes();
                conn.MapComposite<Address>();

                var expected = new Address { PostalCode = "12345", Street = "Main St."};
                using (var cmd = new NpgsqlCommand(@"SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value=expected });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<Address>(0);
                        Assert.That(actual.Street, Is.EqualTo(expected.Street));
                        Assert.That(actual.PostalCode, Is.EqualTo(expected.PostalCode));
                    }
                }
            }
        }

        public class Address
        {
            public string Street { get; set; }
            public string PostalCode { get; set; }
        }

        class TableAsCompositeType
        {
            public int Foo { get; set; }
        }
        
        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/990")]
        public void TableAsComposite()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE table_as_composite (foo int); INSERT INTO table_as_composite (foo) VALUES (8)");
                conn.ReloadTypes();
                conn.MapComposite<TableAsCompositeType>("table_as_composite");
                var value = (TableAsCompositeType)conn.ExecuteScalar(@"SELECT t.*::table_as_composite FROM table_as_composite AS t");
                Assert.That(value.Foo, Is.EqualTo(8));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1267")]
        public void TableAsCompositeWithDeleteColumns()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"
                    CREATE TEMP TABLE table_as_composite (foo int, bar int);
                    ALTER TABLE table_as_composite DROP COLUMN bar;
                    INSERT INTO table_as_composite (foo) VALUES (8)");
                conn.ReloadTypes();
                conn.MapComposite<TableAsCompositeType>("table_as_composite");
                var value = (TableAsCompositeType)conn.ExecuteScalar(@"SELECT t.*::table_as_composite FROM table_as_composite AS t");
                Assert.That(value.Foo, Is.EqualTo(8));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1125")]
        public void NullableProperty()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.nullable_property_type AS (foo INT)");
                conn.ReloadTypes();
                conn.MapComposite<NullablePropertyType>();

                var expected1 = new NullablePropertyType { Foo = 8 };
                var expected2 = new NullablePropertyType { Foo = null };
                using (var cmd = new NpgsqlCommand(@"SELECT @p1, @p2", conn))
                {
                    cmd.Parameters.AddWithValue("p1", expected1);
                    cmd.Parameters.AddWithValue("p2", expected2);

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldValue<NullablePropertyType>(0).Foo, Is.EqualTo(8));
                        Assert.That(reader.GetFieldValue<NullablePropertyType>(1).Foo, Is.Null);
                    }
                }
            }
        }

        class NullablePropertyType
        {
            public int? Foo { get; set; }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1168")]
        public void WithSchema()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(WithSchema),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS composite_schema CASCADE; CREATE SCHEMA composite_schema");
                try
                {
                    conn.ExecuteNonQuery("CREATE TYPE composite_schema.composite AS (foo int)");
                    conn.ReloadTypes();
                    conn.MapComposite<Composite1>("composite_schema.composite");
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.AddWithValue("p", new Composite1 { Foo = 8 });
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite_schema.composite"));
                            Assert.That(reader.GetFieldValue<Composite1>(0).Foo, Is.EqualTo(8));
                        }
                    }
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS composite_schema CASCADE");
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1168")]
        public void InDifferentSchemas()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(WithSchema),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS composite_schema1 CASCADE; CREATE SCHEMA composite_schema1");
                conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS composite_schema2 CASCADE; CREATE SCHEMA composite_schema2");
                try
                {
                    conn.ExecuteNonQuery("CREATE TYPE composite_schema1.composite AS (foo int)");
                    conn.ExecuteNonQuery("CREATE TYPE composite_schema2.composite AS (bar int)");
                    conn.ReloadTypes();
                    // Attempting to map without a fully-qualified name should fail
                    Assert.That(() => conn.MapComposite<Composite1>("composite"),
                        Throws.Exception.TypeOf<NpgsqlException>()
                    );
                    conn.MapComposite<Composite1>("composite_schema1.composite");
                    conn.MapComposite<Composite2>("composite_schema2.composite");
                    using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                    {
                        cmd.Parameters.AddWithValue("p1", new Composite1 { Foo = 8 });
                        cmd.Parameters.AddWithValue("p2", new Composite2 { Bar = 9 });
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite_schema1.composite"));
                            Assert.That(reader.GetFieldValue<Composite1>(0).Foo, Is.EqualTo(8));
                            Assert.That(reader.GetDataTypeName(1), Is.EqualTo("composite_schema2.composite"));
                            Assert.That(reader.GetFieldValue<Composite2>(1).Bar, Is.EqualTo(9));
                        }
                    }
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS composite_schema1 CASCADE");
                        conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS composite_schema2 CASCADE");
                    }
                }
            }
        }

        class Composite1 { public int Foo { get; set; } }
        class Composite2 { public int Bar { get; set; } }
    }
}
