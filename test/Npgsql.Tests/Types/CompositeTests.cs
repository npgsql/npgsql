#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;

namespace Npgsql.Tests.Types
{
    [Parallelizable(ParallelScope.None)]
    class CompositeTests : TestBase
    {
        public CompositeTests(string backendVersion) : base(backendVersion) {}

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
                            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite1"));
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
                            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite1"));
                        }
                    }

                    // Resolve type by OID (read)
                    conn.ReloadTypes();
                    using (var cmd = new NpgsqlCommand("SELECT ROW(1, 'foo')::COMPOSITE1", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite1"));
                    }
                }
                finally
                {
                    NpgsqlConnection.UnmapCompositeGlobally("composite1");
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
                    cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Enum) { SpecificType = typeof(SomeComposite), Value = DBNull.Value });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite2"));
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
                        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite2"));
                    }
                }

                // Resolve type by OID (read)
                conn.ReloadTypes();
                conn.MapComposite<SomeComposite>("composite2");
                using (var cmd = new NpgsqlCommand("SELECT ROW(1, 'foo')::COMPOSITE2", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("composite2"));
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
                        SpecificType = typeof (SomeComposite)
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
            using (var conn = OpenConnection())
            {
                NpgsqlConnection.MapCompositeGlobally<SomeComposite>("composite4");
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.composite4 AS (x int, some_text text)");
                conn.ReloadTypes();
                var myTempSchema = conn.ExecuteScalar("SELECT nspname FROM pg_namespace WHERE oid = pg_my_temp_schema()");
                var expected = new SomeComposite {x = 8, SomeText = "foo"};
                using (var cmd = new NpgsqlCommand($"SELECT @p::{myTempSchema}.composite4", conn))
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
                        SpecificType = typeof (SomeComposite)
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
    }
}
