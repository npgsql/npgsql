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
    class Postgis3DMTests : TestBase
    {
        public class TestAtt
        {
            public PostgisGeometry<Coordinate3DM> Geom;
            public string SQL;
        }

        static readonly TestAtt[] Tests =
        {
            new TestAtt {
                Geom = new PostgisPointM(1D, 2500D, 42.3D),
                SQL = "st_makepointm(1,2500,42.3)"
            },
            new TestAtt {
                Geom = new PostgisPolygonM(new[] { new[] {
                    new Coordinate3DM(1d,1d,1d),
                    new Coordinate3DM(2d,2d,2d),
                    new Coordinate3DM(3d,3d,3d),
                    new Coordinate3DM(1d,1d,1d)
                }}),
                SQL = "st_makepolygon(st_makeline(ARRAY[st_makepointm(1,1,1),st_makepointm(2,2,2),st_makepointm(3,3,3),st_makepointm(1,1,1)]))"
            },
            new TestAtt {
                Geom = new PostgisMultiPointM(new[] { new Coordinate3DM(1D, 1D, 1D) }),
                SQL = "st_multi(st_makepointm(1,1,1))"
            },
            new TestAtt {
                Geom = new PostgisMultiLineStringM(new[] {
                    new PostgisLineStringM(new[] {
                        new Coordinate3DM(1D, 1D, 1D),
                        new Coordinate3DM(1D, 2500D, 2D)
                    })
                }),
                SQL = "st_multi(st_makeline(st_makepointm(1,1,1),st_makepointm(1,2500,2)))"
            },
            new TestAtt {
                Geom = new PostgisMultiPolygonM(new[] {
                    new PostgisPolygonM(new[] { new[] {
                        new Coordinate3DM(1d,1d,1d),
                        new Coordinate3DM(2d,2d,2d),
                        new Coordinate3DM(3d,3d,3d),
                        new Coordinate3DM(1d,1d,1d)
                    }})
                }),
                SQL = "st_multi(st_makepolygon(st_makeline(ARRAY[st_makepointm(1,1,1),st_makepointm(2,2,2),st_makepointm(3,3,3),st_makepointm(1,1,1)])))"
            },
            new TestAtt {
                Geom = new PostgisGeometryCollectionM(new PostgisGeometry<Coordinate3DM>[] {
                    new PostgisPointM(1,1,1),
                    new PostgisMultiPolygonM(new[] {
                        new PostgisPolygonM(new[] { new[] {
                            new Coordinate3DM(1d,1d,1d),
                            new Coordinate3DM(2d,2d,2d),
                            new Coordinate3DM(3d,3d,3d),
                            new Coordinate3DM(1d,1d,1d)
                        }})
                    })
                }),
                SQL = "st_collect(st_makepointm(1,1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepointm(1,1,1),st_makepointm(2,2,2),st_makepointm(3,3,3),st_makepointm(1,1,1)]))))"
            },
            new TestAtt {
                Geom = new PostgisGeometryCollectionM(new PostgisGeometry<Coordinate3DM>[] {
                    new PostgisPointM(1,1,1),
                    new PostgisGeometryCollectionM(new PostgisGeometry<Coordinate3DM>[] {
                        new PostgisPointM(1,1,1),
                        new PostgisMultiPolygonM(new[] {
                            new PostgisPolygonM(new[] { new[] {
                                new Coordinate3DM(1d,1d,1d),
                                new Coordinate3DM(2d,2d,2d),
                                new Coordinate3DM(3d,3d,3d),
                                new Coordinate3DM(1d,1d,1d)
                            }})
                        })
                    })
                }),
                SQL = "st_collect(st_makepointm(1,1,1),st_collect(st_makepointm(1,1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepointm(1,1,1),st_makepointm(2,2,2),st_makepointm(3,3,3),st_makepointm(1,1,1)])))))"
            }
        };

        [Test, TestCaseSource(nameof(Tests))]
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
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Geometry, a.Geom);
                a.Geom.SRID = 0;
                cmd.CommandText = "Select st_asewkb(:p1) = st_asewkb(" + a.SQL + ")";
                bool areEqual;
                try
                {
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
                cmd.CommandText = "Select st_asewkb(:p1) = st_asewkb(st_setsrid(" + a.SQL + ",3942))";
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
                Assert.IsTrue((p as PostgisGeometry<Coordinate3DM>).SRID == 3942);
            }
        }

        [Test]
        public void PostgisTestArrayRead()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Select ARRAY(select st_makepointm(1,1,1))";
                var p = cmd.ExecuteScalar() as PostgisGeometry[];
                var p2 = new PostgisPointM(1d, 1d, 1d);
                Assert.IsTrue(p?[0] is PostgisPointM && p2 == (PostgisPointM)p[0]);
            }
        }

        [Test]
        public void PostgisTestArrayWrite()
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                var p = new PostgisPointM[1] { new PostgisPointM(1d, 1d, 1d) };
                cmd.Parameters.AddWithValue(":p1", NpgsqlDbType.Array | NpgsqlDbType.Geometry, p);
                cmd.CommandText = "SELECT :p1 = array(select st_makepointm(1,1,1))";
                Assert.IsTrue((bool)cmd.ExecuteScalar());
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1022")]
        public void MultiPolygonWithMultiplePolygons()
        {
            var geom2 = new PostgisMultiPolygonM(new[]
            {
                new PostgisPolygonM(new[] {
                    new[]
                    {
                        new Coordinate3DM(40, 40, 0),
                        new Coordinate3DM(20, 45, 0),
                        new Coordinate3DM(45, 30, 0),
                        new Coordinate3DM(40, 40, 0)
                    }
                }),
                new PostgisPolygonM(new[] {
                    new[]
                    {
                        new Coordinate3DM(20, 35, 0),
                        new Coordinate3DM(10, 30, 0),
                        new Coordinate3DM(10, 10, 0),
                        new Coordinate3DM(30, 5, 0),
                        new Coordinate3DM(45, 20, 0),
                        new Coordinate3DM(20, 35, 0)
                    }
                })
            })
            { SRID = 4326 };
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
                var point = new PostgisPointM(8, 8, 0);

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

                    var length = (int)reader.GetBytes(0, 0, null, 0, 0);
                    var buffer = new byte[length];
                    reader.GetBytes(0, 0, buffer, 0, length);

                    Assert.That(buffer, Is.EqualTo(bytes));
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
                    {
                        for (var i = 0; i < 1000; i++)
                            writer.WriteRow(a.Geom);
                        writer.Commit();
                    }
                }
                catch (Exception e)
                {
                    Assert.Fail($"Copy from stdin failed with {e} at geometry {a.Geom}.");
                }

                try
                {
                    using (var rdr = c.BeginBinaryExport($"COPY testcopybin (g) TO STDOUT (FORMAT BINARY) "))
                    {
                        for (var i = 0; i < 1000; i++)
                        {
                            rdr.StartRow();
                            Assert.IsTrue(a.Geom.Equals(rdr.Read<PostgisGeometry>()));
                        }
                    }
                }
                catch (Exception e)
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
                    {
                        for (var i = 0; i < 1000; i++)
                            writer.WriteRow(new[] { t });
                        writer.Commit();
                    }
                }
                catch (Exception e)
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
                catch (Exception e)
                {
                    Assert.Fail($"Copy to stdout failed with {e} at geometry {a.Geom}.");
                }
            }
        }

        [Test]
        public void TestPolygonEnumeration()
        {
            var a = new Coordinate3DM[2][] {
                new Coordinate3DM[4] { new Coordinate3DM(0D, 0D, 0D), new Coordinate3DM(0D, 1D, 1D),
                                      new Coordinate3DM(1D, 1D, 1D), new Coordinate3DM(0D, 0D, 2D) },
                new Coordinate3DM[5] { new Coordinate3DM(0D, 0D, 0D), new Coordinate3DM(0D, 2D, 8D),
                                      new Coordinate3DM(2D, 2D,2D),new Coordinate3DM(2D, 0D, 3D),
                                     new Coordinate3DM(0D, 0D, 6D) } };
            Assert.That(a.SequenceEqual(new PostgisPolygonM(a)));
        }

        [Test]
        public void ReadAsConcreteType()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_makepointm(1, 1, 1)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<PostgisPointM>(0), Is.EqualTo(new PostgisPointM(1, 1, 1)));
                Assert.That(() => reader.GetFieldValue<PostgisPolygonM>(0), Throws.Exception.TypeOf<InvalidCastException>());
            }
        }

        [Test]
        public void Bug1381()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add("p", NpgsqlTypes.NpgsqlDbType.Geometry).Value = new PostgisMultiPolygonM(new[]
                {
                    new PostgisPolygonM(new[]
                        {
                            new[]
                            {
                                new Coordinate3DM(-0.555701, 46.42473701, 18.215),
                                new Coordinate3DM(-0.549486, 46.42707801, 18.215),
                                new Coordinate3DM(-0.549843, 46.42749901, 18.215),
                                new Coordinate3DM(-0.555524, 46.42533901, 18.215),
                                new Coordinate3DM(-0.555701, 46.42473701, 18.215)
                            }
                        })
                        // This is the problem:
                        { SRID = 4326 }
                })
                { SRID = 4326 };

                cmd.ExecuteNonQuery();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1557")]
        public void SubGeometriesWithSRID()
        {
            var point = new PostgisPointM(1, 1, 1)
            {
                SRID = 4326
            };

            var lineString = new PostgisLineStringM(new[] { new Coordinate3DM(2, 2, 2), new Coordinate3DM(3, 3, 3) })
            {
                SRID = 4326
            };

            var polygon = new PostgisPolygonM(new[] { new[] { new Coordinate3DM(4, 4, 4), new Coordinate3DM(5, 5, 5), new Coordinate3DM(6, 6, 6), new Coordinate3DM(4, 4, 4) } })
            {
                SRID = 4326
            };

            var collection = new PostgisGeometryCollectionM(new PostgisGeometry<Coordinate3DM>[] { point, lineString, polygon })
            {
                SRID = 4326
            };

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT :p", conn))
            {
                cmd.Parameters.AddWithValue("p", collection);
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
