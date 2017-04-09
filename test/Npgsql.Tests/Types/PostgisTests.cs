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

using System;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    class PostgisTests : TestBase
    {
        public class TestAtt
        {
            public PostgisGeometry Geom;
            public string SQL;
        }

        static readonly TestAtt[] Tests =
        {
            new TestAtt { Geom = new PostgisPoint(1D, 2500D), SQL = "st_makepoint(1,2500)" },
            new TestAtt {
                Geom = new PostgisLineString(new[] { new Coordinate2D(1D, 1D), new Coordinate2D(1D, 2500D) }),
                SQL = "st_makeline(st_makepoint(1,1),st_makepoint(1,2500))"
            },
            new TestAtt {
                Geom = new PostgisPolygon(new[] { new[] {
                    new Coordinate2D(1d,1d),
                    new Coordinate2D(2d,2d),
                    new Coordinate2D(3d,3d),
                    new Coordinate2D(1d,1d)
                }}),
                SQL = "st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)]))"
            },
            new TestAtt {
                Geom = new PostgisMultiPoint(new[] { new Coordinate2D(1D, 1D) }),
                SQL = "st_multi(st_makepoint(1,1))"
            },
            new TestAtt {
                Geom = new PostgisMultiLineString(new[] {
                    new PostgisLineString(new[] {
                        new Coordinate2D(1D, 1D),
                        new Coordinate2D(1D, 2500D)
                    })
                }),
                SQL = "st_multi(st_makeline(st_makepoint(1,1),st_makepoint(1,2500)))"
            },
            new TestAtt {
                Geom = new PostgisMultiPolygon(new[] {
                    new PostgisPolygon(new[] { new[] {
                        new Coordinate2D(1d,1d),
                        new Coordinate2D(2d,2d),
                        new Coordinate2D(3d,3d),
                        new Coordinate2D(1d,1d)
                    }})
                }),
                SQL = "st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])))"
            },
            new TestAtt {
                Geom = new PostgisGeometryCollection(new PostgisGeometry[] {
                    new PostgisPoint(1,1),
                    new PostgisMultiPolygon(new[] {
                        new PostgisPolygon(new[] { new[] {
                            new Coordinate2D(1d,1d),
                            new Coordinate2D(2d,2d),
                            new Coordinate2D(3d,3d),
                            new Coordinate2D(1d,1d)
                        }})
                    })
                }),
                SQL = "st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)]))))"
            },
            new TestAtt {
                Geom = new PostgisGeometryCollection(new PostgisGeometry[] {
                    new PostgisPoint(1,1),
                    new PostgisGeometryCollection(new PostgisGeometry[] {
                        new PostgisPoint(1,1),
                        new PostgisMultiPolygon(new[] {
                            new PostgisPolygon(new[] { new[] {
                                new Coordinate2D(1d,1d),
                                new Coordinate2D(2d,2d),
                                new Coordinate2D(3d,3d),
                                new Coordinate2D(1d,1d)
                            }})
                        })
                    })
                }),
                SQL = "st_collect(st_makepoint(1,1),st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])))))"
            }
        };

        [Test,TestCaseSource(nameof(Tests))]
        public void PostgisTestRead(TestAtt att)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                var a = att;
                cmd.CommandText = "Select " + a.SQL;
                var p = cmd.ExecuteScalar();
                Assert.IsTrue(p.Equals(a.Geom));
            }
        }

        [Test, TestCaseSource(nameof(Tests))]
        public void PostgisTestWrite(TestAtt a)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Geometry,a.Geom);
                a.Geom.SRID = 0;
                cmd.CommandText = "Select st_asewkb(:p1) = st_asewkb(" + a.SQL + ")";
                bool areEqual;
                try {
                    areEqual = (bool)cmd.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw new Exception("Exception caught on " + a.Geom, e);
                }
                Assert.IsTrue(areEqual, "Error on comparison of " + a.Geom);
            }
        }

        [Test, TestCaseSource(nameof(Tests))]
        public void PostgisTestWriteSrid(TestAtt a)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Geometry, a.Geom);
                a.Geom.SRID = 3942;
                cmd.CommandText = "Select st_asewkb(:p1) = st_asewkb(st_setsrid("+ a.SQL + ",3942))";
                var p = (bool)cmd.ExecuteScalar();
                Assert.IsTrue(p);
            }
        }

        [Test, TestCaseSource(nameof(Tests))]
        public void PostgisTestReadSrid(TestAtt a)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Select st_setsrid(" + a.SQL + ",3942)";
                var p = cmd.ExecuteScalar();
                Assert.IsTrue(p.Equals(a.Geom));
                Assert.IsTrue((p as PostgisGeometry).SRID == 3942);
            }
        }

        [Test]
        public void PostgisTestArrayRead()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Select ARRAY(select st_makepoint(1,1))";
                var p = cmd.ExecuteScalar() as PostgisGeometry[];
                var p2 = new PostgisPoint(1d, 1d);
                Assert.IsTrue(p?[0] is PostgisPoint && p2 == (PostgisPoint)p[0]);
            }
        }

        [Test]
        public void PostgisTestArrayWrite()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                var p = new PostgisPoint[1] { new PostgisPoint(1d, 1d) };
                cmd.Parameters.AddWithValue(":p1", NpgsqlDbType.Array | NpgsqlDbType.Geometry, p);
                cmd.CommandText = "SELECT :p1 = array(select st_makepoint(1,1))";
                Assert.IsTrue((bool)cmd.ExecuteScalar());
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1022")]
        public void MultiPolygonWithMultiplePolygons()
        {
            var geom2 = new PostgisMultiPolygon(new[]
            {
                new PostgisPolygon(new[] {
                    new[]
                    {
                        new Coordinate2D(40, 40),
                        new Coordinate2D(20, 45),
                        new Coordinate2D(45, 30),
                        new Coordinate2D(40, 40)
                    }
                }),
                new PostgisPolygon(new[] {
                    new[]
                    {
                        new Coordinate2D(20, 35),
                        new Coordinate2D(10, 30),
                        new Coordinate2D(10, 10),
                        new Coordinate2D(30, 5),
                        new Coordinate2D(45, 20),
                        new Coordinate2D(20, 35)
                    }
                })
            }) { SRID = 4326 };
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
            {
                command.Parameters.AddWithValue("p1", geom2);
                command.CommandText = "Select :p1";
                command.ExecuteScalar();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1121")]
        public void AsBinaryWkb()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (foo GEOMETRY)");
                var point = new PostgisPoint(8, 8);

                using (var cmd = new NpgsqlCommand("INSERT INTO data (foo) VALUES (@p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Geometry, point);
                    cmd.ExecuteNonQuery();
                }

                byte[] bytes;
                using (var cmd = new NpgsqlCommand("SELECT foo FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    bytes = reader.GetFieldValue<byte[]>(0);
                }

                conn.ExecuteNonQuery("TRUNCATE data");

                using (var cmd = new NpgsqlCommand("INSERT INTO data (foo) VALUES (@p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Geometry, bytes);
                    cmd.ExecuteNonQuery();
                }

                Assert.That(conn.ExecuteScalar("SELECT foo FROM data"), Is.EqualTo(point));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, TestCaseSource(nameof(Tests)), IssueLink("https://github.com/npgsql/npgsql/issues/1260")]
        public void CopyBinary(TestAtt a)
        {
            using (var c = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("CREATE TEMPORARY TABLE testcopybin (g geometry)", c))
                    cmd.ExecuteNonQuery();

                try
                {
                    using (var writer = c.BeginBinaryImport($"COPY testcopybin (g) FROM STDIN (FORMAT BINARY)"))
                        for (var i = 0; i < 1000; i++)
                            writer.WriteRow(a.Geom);
                }
                catch(Exception e)
                {
                    Assert.Fail($"Copy from stdin failed with {e} at geometry {a.Geom}.");
                }

                try
                {
                    using (var rdr = c.BeginBinaryExport($"COPY testcopybin (g) TO STDOUT (FORMAT BINARY) "))
                    {
                        for (var i =0; i < 1000; i++)
                        {
                            rdr.StartRow();
                            Assert.IsTrue(a.Geom.Equals(rdr.Read<PostgisGeometry>()));
                        }
                    }
                }
                catch(Exception e)
                {
                    Assert.Fail($"Copy from stdout failed with {e} at geometry {a.Geom}.");
                }
            }
        }

        [Test, TestCaseSource(nameof(Tests)), IssueLink("https://github.com/npgsql/npgsql/issues/1260")]
        public void CopyBinaryArray(TestAtt a)
        {
            using (var c = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("CREATE TEMPORARY TABLE testcopybinarray (g geometry[3])", c))
                    cmd.ExecuteNonQuery();

                var t = new PostgisGeometry[3] { a.Geom, a.Geom, a.Geom };
                try
                {
                    using (var writer = c.BeginBinaryImport("COPY testcopybinarray (g) FROM STDIN (FORMAT BINARY)"))
                        for (var i = 0; i < 1000; i++)
                            writer.WriteRow(new[] {t});
                }
                catch(Exception e)
                {
                    Assert.Fail($"Copy from stdin failed with {e} at geometry {a.Geom}.");
                }

                try
                {
                    using (var rdr = c.BeginBinaryExport("COPY testcopybinarray (g) TO STDOUT (FORMAT BINARY)"))
                        for (var i = 0; i < 1000; i++)
                        {
                            rdr.StartRow();
                            Assert.IsTrue(t.SequenceEqual(rdr.Read<PostgisGeometry[]>()));
                        }
                }
                catch(Exception e)
                {
                    Assert.Fail($"Copy to stdout failed with {e} at geometry {a.Geom}.");
                }
            }
        }

        [Test]
        public void TestPolygonEnumeration()
        {
            var a = new Coordinate2D[2][] {
                new Coordinate2D[4] { new Coordinate2D(0D, 0D), new Coordinate2D(0D, 1D),
                                      new Coordinate2D(1D, 1D), new Coordinate2D(0D, 0D) },
                new Coordinate2D[5] { new Coordinate2D(0D, 0D), new Coordinate2D(0D, 2D),
                                      new Coordinate2D(2D, 2D),new Coordinate2D(2D, 0D),
                                     new Coordinate2D(0D, 0D) } };
            Assert.That(a.SequenceEqual(new PostgisPolygon(a)));
        }

        [Test]
        public void ReadAsConcreteType()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_makepoint(1, 1)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<PostgisPoint>(0), Is.EqualTo(new PostgisPoint(1, 1)));
                Assert.That(() => reader.GetFieldValue<PostgisPolygon>(0), Throws.Exception.TypeOf<InvalidCastException>());
            }
        }

        [Test]
        public void Bug1381()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add("p", NpgsqlTypes.NpgsqlDbType.Geometry).Value = new PostgisMultiPolygon(new[]
                {
                    new PostgisPolygon(new[]
                        {
                            new[]
                            {
                                new Coordinate2D(-0.555701, 46.42473701),
                                new Coordinate2D(-0.549486, 46.42707801),
                                new Coordinate2D(-0.549843, 46.42749901),
                                new Coordinate2D(-0.555524, 46.42533901),
                                new Coordinate2D(-0.555701, 46.42473701)
                            }
                        })
                        // This is the problem:
                        { SRID = 4326 }
                }) { SRID = 4326 };

                cmd.ExecuteNonQuery();
            }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT postgis_version()", conn))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (PostgresException)
                {
                    cmd.CommandText = "SELECT version()";
                    var versionString = (string)cmd.ExecuteScalar();
                    Debug.Assert(versionString != null);
                    var m = Regex.Match(versionString, @"^PostgreSQL ([0-9.]+(\w*)?)");
                    if (!m.Success)
                        throw new Exception("Couldn't parse PostgreSQL version string: " + versionString);
                    var version = m.Groups[1].Value;
                    var prerelease = m.Groups[2].Value;
                    if (!string.IsNullOrWhiteSpace(prerelease))
                        Assert.Ignore($"PostGIS not installed, ignoring because we're on a prerelease version of PostgreSQL ({version})");
                    TestUtil.IgnoreExceptOnBuildServer("PostGIS extension not installed.");
                }
            }
        }
    }
}
