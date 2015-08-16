#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
    class CompositeTests : TestBase
    {
        public CompositeTests(string backendVersion) : base(backendVersion) {}

        #region Test Types

        class Composite1
        {
            public int x { get; set; }
            [PgName("some_text")]
            public string SomeText { get; set; }
        }

        class Composite2
        {
            public int a { get; set; }
            [PgName("contained")]
            public Composite1 Contained { get; set; }
        }

        struct CompositeStruct
        {
            public int x { get; set; }
            [PgName("some_text")]
            public string SomeText { get; set; }
        }

        #endregion

        [Test]
        public void LateMapping()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.composite1 AS (x int, some_text text)");
            Conn.ReloadTypes();
            Conn.MapComposite<Composite1>("composite1");

            var expected = new Composite1 { x = 8, SomeText = "foo" };
            using (var cmd = new NpgsqlCommand("SELECT @p1::composite1, @p2::composite1", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Composite) { Value = expected, SpecificType = typeof(Composite1)});
                cmd.Parameters.AddWithValue("p2", expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        var actual = reader.GetFieldValue<Composite1>(i);
                        Assert.That(actual.x, Is.EqualTo(8));
                        Assert.That(actual.SomeText, Is.EqualTo("foo"));
                    }
                }
            }
        }

        [Test]
        public void GlobalMapping()
        {
            NpgsqlConnection.MapCompositeGlobally<Composite1>();
            ExecuteNonQuery("CREATE TYPE pg_temp.composite1 AS (x int, some_text text)");
            Conn.ReloadTypes();

            var expected = new Composite1 { x = 8, SomeText = "foo" };
            using (var cmd = new NpgsqlCommand("SELECT @p::composite1", Conn))
            {
                cmd.Parameters.AddWithValue("p", expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var actual = reader.GetFieldValue<Composite1>(0);
                    Assert.That(actual.x, Is.EqualTo(8));
                    Assert.That(actual.SomeText, Is.EqualTo("foo"));
                }
            }
        }

        [Test, Description("Tests a composite within another composite")]
        public void Recursive()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.composite1 AS (x int, some_text text)");
            ExecuteNonQuery("CREATE TYPE pg_temp.composite2 AS (a int, contained composite1)");
            Conn.ReloadTypes();
            // Registration in inverse dependency order should work
            Conn.MapComposite<Composite2>("composite2");
            Conn.MapComposite<Composite1>("composite1");

            var expected = new Composite2 {
                a = 4,
                Contained = new Composite1 { x = 8, SomeText = "foo" }
            };

            using (var cmd = new NpgsqlCommand("SELECT @p::composite2", Conn))
            {
                cmd.Parameters.AddWithValue("p", expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var actual = reader.GetFieldValue<Composite2>(0);
                    Assert.That(actual.a, Is.EqualTo(4));
                    Assert.That(actual.Contained.x, Is.EqualTo(8));
                    Assert.That(actual.Contained.SomeText, Is.EqualTo("foo"));
                }
            }
        }

        [Test]
        public void Struct()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.composite_struct AS (x int, some_text text)");
            Conn.ReloadTypes();
            Conn.MapComposite<CompositeStruct>("composite_struct");

            var expected = new CompositeStruct { x = 8, SomeText = "foo" };
            using (var cmd = new NpgsqlCommand("SELECT @p::composite_struct", Conn))
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

        [Test]
        public void Array()
        {
            ExecuteNonQuery("CREATE TYPE pg_temp.composite1 AS (x int, some_text text)");
            Conn.ReloadTypes();
            Conn.MapComposite<Composite1>("composite1");

            var expected = new[] {
                new Composite1 { x = 8, SomeText = "foo" },
                new Composite1 { x = 9, SomeText = "bar" }
            };

            using (var cmd = new NpgsqlCommand("SELECT @p1::composite1[], @p2::composite1[]", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Composite) { Value = expected, SpecificType = typeof(Composite1) });
                cmd.Parameters.AddWithValue("p2", expected);  // Infer
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        var actual = reader.GetFieldValue<Composite1[]>(i);
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
