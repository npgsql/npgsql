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

using NUnit.Framework;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Tests.Types
{
    [NonParallelizable]
    class CompositeTests : TestBase
    {
        #region Test Types

        class SomeComposite
        {
            public int X { get; set; }
            public string SomeText { get; set; }
        }

        class SomeCompositeContainer
        {
            public int A { get; set; }
            public SomeComposite Contained { get; set; }
        }

        struct CompositeStruct
        {
            public int X { get; set; }
            public string SomeText { get; set; }
        }

        #endregion

        [Test]
        public void UnmappedComposite()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(UnmappedComposite),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.unmapped_comp AS (x int, some_text text)");
                conn.ReloadTypes();
                var tempSchema = conn.ExecuteScalar("SELECT nspname FROM pg_namespace WHERE oid = pg_my_temp_schema()");

                var composite = new SomeComposite { X = 8, SomeText = "foo" };

                var expando = (dynamic)new ExpandoObject();
                expando.X = 8;
                expando.SomeText = "foo";

                var compositeArray = new[]
                {
                    new SomeComposite { X = 9, SomeText = "bar" },
                    new SomeComposite { X = 10, SomeText = "baz" }
                };

                var expandoArray = new dynamic[2];
                expandoArray[0] = new ExpandoObject();
                expandoArray[0].x = 9;
                expandoArray[0].some_text = "bar";
                expandoArray[1] = new ExpandoObject();
                expandoArray[1].x = 10;
                expandoArray[1].some_text = "baz";

                using (var cmd = new NpgsqlCommand("SELECT @scalar1, @scalar2, @scalar3, @scalar4, @array1, @array2", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "scalar1",
                        Value = composite,
                        DataTypeName = $"{tempSchema}.unmapped_comp"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "scalar2",
                        Value = expando,
                        DataTypeName = $"{tempSchema}.unmapped_comp"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter<SomeComposite>
                    {
                        ParameterName = "scalar3",
                        TypedValue = composite,
                        DataTypeName = $"{tempSchema}.unmapped_comp"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter<dynamic>
                    {
                        ParameterName = "scalar4",
                        TypedValue = expando,
                        DataTypeName = $"{tempSchema}.unmapped_comp"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "array1",
                        Value = compositeArray,
                        DataTypeName = $"{tempSchema}.unmapped_comp[]"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "array2",
                        Value = expandoArray,
                        DataTypeName = $"{tempSchema}.unmapped_comp[]"
                    });

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        for (var i = 0; i < 4; i++)
                        {
                            Assert.That(reader.GetDataTypeName(i),
                                Does.StartWith("pg_temp") & Does.EndWith(".unmapped_comp"));

                            var asTyped = reader.GetFieldValue<SomeComposite>(i);
                            Assert.That(asTyped.X, Is.EqualTo(8));
                            Assert.That(asTyped.SomeText, Is.EqualTo("foo"));

                            // TODO: Need name translation...
                            var asDynamic = (dynamic)reader.GetValue(i);
                            Assert.That(asDynamic.x, Is.EqualTo(8));
                            Assert.That(asDynamic.some_text, Is.EqualTo("foo"));
                        }

                        for (var i = 4; i < 6; i++)
                        {
                            Assert.That(reader.GetDataTypeName(i),
                                Does.StartWith("pg_temp") & Does.EndWith(".unmapped_comp[]"));

                            // TODO: The following doesn't work because of limitations in ArrayHandler.
                            // You currently have to map the composite in order to read an array.
                            /*
                            var asTyped = reader.GetFieldValue<SomeComposite[]>(i);
                            Assert.That(asTyped[0].X, Is.EqualTo(9));
                            Assert.That(asTyped[0].SomeText, Is.EqualTo("bar"));
                            Assert.That(asTyped[1].X, Is.EqualTo(10));
                            Assert.That(asTyped[1].SomeText, Is.EqualTo("baz"));

                            // TODO: Need name translation...
                            var asDynamic = (dynamic[])reader.GetValue(i);
                            Assert.That(asDynamic[0].x, Is.EqualTo(9));
                            Assert.That(asDynamic[0].some_text, Is.EqualTo("bar"));
                            Assert.That(asDynamic[1].x, Is.EqualTo(10));
                            Assert.That(asDynamic[1].some_text, Is.EqualTo("baz"));
                            */
                        }
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1779")]
        public void CompositePostgresType()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PostgresType),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.comp1 AS (x int, some_text text)");
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.comp2 AS (comp comp1, comps comp1[])");
                conn.ReloadTypes();

                using (var cmd = new NpgsqlCommand("SELECT ROW(ROW(8, 'foo')::comp1, ARRAY[ROW(9, 'bar')::comp1, ROW(10, 'baz')::comp1])::comp2", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var comp2Type = (PostgresCompositeType)reader.GetPostgresType(0);
                        Assert.That(comp2Type.Name, Is.EqualTo("comp2"));
                        Assert.That(comp2Type.FullName, Does.StartWith("pg_temp_") & Does.EndWith(".comp2"));
                        Assert.That(comp2Type.Fields, Has.Count.EqualTo(2));
                        var field1 = comp2Type.Fields[0];
                        var field2 = comp2Type.Fields[1];
                        Assert.That(field1.Name, Is.EqualTo("comp"));
                        Assert.That(field2.Name, Is.EqualTo("comps"));
                        var comp1Type = (PostgresCompositeType)field1.Type;
                        Assert.That(comp1Type.Name, Is.EqualTo("comp1"));
                        var arrType = (PostgresArrayType)field2.Type;
                        Assert.That(arrType.Name, Is.EqualTo("comp1[]"));
                        var elemType = arrType.Element;
                        Assert.That(elemType, Is.SameAs(comp1Type));
                    }
                }
            }
        }

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
                NpgsqlConnection.GlobalTypeMapper.MapComposite<SomeComposite>("composite1");
                try
                {
                    conn.ReloadTypes();

                    // Resolve type by DataTypeName
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter
                        {
                            ParameterName = "p",
                            DataTypeName = "composite1",
                            Value = DBNull.Value
                        });
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
                        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = new SomeComposite { X = 8, SomeText = "foo" }});
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
                    NpgsqlConnection.GlobalTypeMapper.UnmapComposite<SomeComposite>("composite1");
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
                conn.TypeMapper.MapComposite<SomeComposite>("composite2");
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "p",
                        DataTypeName = "composite2",
                        Value = DBNull.Value
                    });

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite2"));
                        Assert.That(reader.IsDBNull(0), Is.True);
                    }
                }

                // Resolve type by ClrType (type inference)
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<SomeComposite>("composite2");
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = new SomeComposite { X = 8, SomeText = "foo" } });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".composite2"));
                    }
                }

                // Resolve type by OID (read)
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<SomeComposite>("composite2");
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
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(LateMapping),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite3 AS (x int, some_text text)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<SomeComposite>("composite3");

                var expected = new SomeComposite {X = 8, SomeText = "foo"};
                using (var cmd = new NpgsqlCommand("SELECT @p1::composite3, @p2::composite3", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "p1",
                        DataTypeName = "composite3",
                        Value = expected
                    });
                    cmd.Parameters.AddWithValue("p2", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            var actual = reader.GetFieldValue<SomeComposite>(i);
                            Assert.That(actual.X, Is.EqualTo(8));
                            Assert.That(actual.SomeText, Is.EqualTo("foo"));
                        }
                    }
                }
            }
        }

        [Test]
        public void GlobalMapping()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(LateMapping),
                Pooling = false
            };
            try
            {
                using (var conn = OpenConnection(csb))
                {
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS composite4");
                    conn.ExecuteNonQuery("CREATE TYPE composite4 AS (x int, some_text text)");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<SomeComposite>("composite4");
                    conn.ReloadTypes();

                    var expected = new SomeComposite { X = 8, SomeText = "foo" };
                    using (var cmd = new NpgsqlCommand($"SELECT @p::composite4", conn))
                    {
                        cmd.Parameters.AddWithValue("p", expected);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            var actual = reader.GetFieldValue<SomeComposite>(0);
                            Assert.That(actual.X, Is.EqualTo(8));
                            Assert.That(actual.SomeText, Is.EqualTo("foo"));
                        }
                    }
                }

                // Unmap
                NpgsqlConnection.GlobalTypeMapper.UnmapComposite<SomeComposite>("composite4");

                using (var conn = OpenConnection(csb))
                {
                    // Composite should have been unmapped and so will return as dynamic
                    Assert.That(conn.ExecuteScalar("SELECT '(8, \"foo\")'::composite4"), Is.TypeOf<ExpandoObject>());
                }
            }
            finally
            {
                using (var conn = OpenConnection(csb))
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS composite4");
            }
        }

        [Test, Description("Tests a composite within another composite")]
        public void Recursive()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(Recursive),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite_contained AS (x int, some_text text)");
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite_container AS (a int, contained composite_contained)");
                conn.ReloadTypes();
                // Registration in inverse dependency order should work
                conn.TypeMapper.MapComposite<SomeCompositeContainer>("composite_container");
                conn.TypeMapper.MapComposite<SomeComposite>("composite_contained");

                var expected = new SomeCompositeContainer {
                    A = 4,
                    Contained = new SomeComposite {X = 8, SomeText = "foo"}
                };

                using (var cmd = new NpgsqlCommand("SELECT @p::composite_container", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<SomeCompositeContainer>(0);
                        Assert.That(actual.A, Is.EqualTo(4));
                        Assert.That(actual.Contained.X, Is.EqualTo(8));
                        Assert.That(actual.Contained.SomeText, Is.EqualTo("foo"));
                    }
                }
            }
        }

        [Test]
        public void Struct()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(Struct),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite_struct AS (x int, some_text text)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<CompositeStruct>("composite_struct");

                var expected = new CompositeStruct {X = 8, SomeText = "foo"};
                using (var cmd = new NpgsqlCommand("SELECT @p::composite_struct", conn))
                {
                    cmd.Parameters.AddWithValue("p", expected);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var actual = reader.GetFieldValue<CompositeStruct>(0);
                        Assert.That(actual.X, Is.EqualTo(8));
                        Assert.That(actual.SomeText, Is.EqualTo("foo"));
                    }
                }
            }
        }

        [Test]
        public void Array()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(Array),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite5 AS (x int, some_text text)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<SomeComposite>("composite5");

                var expected = new[] {
                    new SomeComposite {X = 8, SomeText = "foo"},
                    new SomeComposite {X = 9, SomeText = "bar"}
                };

                using (var cmd = new NpgsqlCommand("SELECT @p1::composite5[], @p2::composite5[]", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "p1",
                        DataTypeName = "composite5[]",
                        Value = expected
                    });
                    cmd.Parameters.AddWithValue("p2", expected); // Infer
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            var actual = reader.GetFieldValue<SomeComposite[]>(i);
                            Assert.That(actual[0].X, Is.EqualTo(expected[0].X));
                            Assert.That(actual[0].SomeText, Is.EqualTo(expected[0].SomeText));
                            Assert.That(actual[1].X, Is.EqualTo(expected[1].X));
                            Assert.That(actual[1].SomeText, Is.EqualTo(expected[1].SomeText));
                        }
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
        public void NameTranslation()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(LateMapping),
                Pooling = false
            };
            var expected = new NameTranslationComposite { Simple = 2, TwoWords = 3, SomeClrName = 4 };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.name_translation_composite AS (simple int, two_words int, some_database_name int)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<NameTranslationComposite>();

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
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(Domain),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery(setupSql);
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<Address>();

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

        #region Table as Composite

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/990")]
        public void TableAsCompositeNotSupportedByDefault()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE table_as_composite (foo int); INSERT INTO table_as_composite (foo) VALUES (8)");
                conn.ReloadTypes();
                Assert.That(() => conn.TypeMapper.MapComposite<TableAsCompositeType>("table_as_composite"), Throws.Exception.TypeOf<ArgumentException>());
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/990")]
        public void TableAsComposite()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                ApplicationName = nameof(TableAsComposite),
                LoadTableComposites = true
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery(
                    "CREATE TEMP TABLE table_as_composite (foo int);" +
                    "INSERT INTO table_as_composite (foo) VALUES (8)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<TableAsCompositeType>("table_as_composite");
                var value = (TableAsCompositeType)conn.ExecuteScalar(@"SELECT t.*::table_as_composite FROM table_as_composite AS t");
                Assert.That(value.Foo, Is.EqualTo(8));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1267")]
        public void TableAsCompositeWithDeleteColumns()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                ApplicationName = nameof(TableAsCompositeWithDeleteColumns),
                LoadTableComposites = true
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery(@"
                    CREATE TEMP TABLE table_as_composite2 (foo int, bar int);
                    ALTER TABLE table_as_composite2 DROP COLUMN bar;
                    INSERT INTO table_as_composite2 (foo) VALUES (8)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<TableAsCompositeType>("table_as_composite2");
                var value = (TableAsCompositeType)conn.ExecuteScalar(@"SELECT t.*::table_as_composite2 FROM table_as_composite2 AS t");
                Assert.That(value.Foo, Is.EqualTo(8));
            }
        }

        #endregion Table as Composite

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1125")]
        public void NullableProperty()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                ApplicationName = nameof(NullableProperty)
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.nullable_property_type AS (foo INT)");
                conn.ReloadTypes();
                conn.TypeMapper.MapComposite<NullablePropertyType>();

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
                    conn.TypeMapper.MapComposite<Composite1>("composite_schema.composite");
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
                ApplicationName = nameof(InDifferentSchemas),  // Prevent backend type caching in TypeHandlerRegistry
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
                    Assert.That(() => conn.TypeMapper.MapComposite<Composite1>("composite"),
                        Throws.Exception.TypeOf<ArgumentException>()
                    );
                    conn.TypeMapper
                        .MapComposite<Composite1>("composite_schema1.composite")
                        .MapComposite<Composite2>("composite_schema2.composite");
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
        class Composite3 { public int Bar { get; set; } }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1612")]
        public void LocalMappingDontLeak()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                ApplicationName = nameof(LocalMappingDontLeak)
            };
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Composite2>("composite");
            try
            {
                using (var conn = OpenConnection(csb))
                {
                    conn.ExecuteNonQuery("CREATE TYPE composite AS (bar int)");
                    conn.ReloadTypes();
                    Assert.That(conn.ExecuteScalar("SELECT '(8)'::composite"), Is.TypeOf<Composite2>());
                    conn.TypeMapper.MapComposite<Composite3>("composite");
                    Assert.That(conn.ExecuteScalar("SELECT '(8)'::composite"), Is.TypeOf<Composite3>());
                }
                using (var conn = OpenConnection(csb))
                    Assert.That(conn.ExecuteScalar("SELECT '(8)'::composite"), Is.TypeOf<Composite2>());
            }
            finally
            {
                NpgsqlConnection.GlobalTypeMapper.UnmapComposite<Composite2>("composite");
                using (var conn = OpenConnection(csb))
                {
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS composite");
                    NpgsqlConnection.ClearPool(conn);
                }
            }
        }
    }
}
