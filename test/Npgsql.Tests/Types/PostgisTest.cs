using System;
using System.Collections.Generic;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    class PostgisTest : TestBase
    {
        public class TestAtt
        {
            public IGeometry Geom;
            public string SQL;
        }

        readonly static TestAtt[] Tests =
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
                Geom = new PostgisGeometryCollection(new IGeometry[] {
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
                Geom = new PostgisGeometryCollection(new IGeometry[] {
                    new PostgisPoint(1,1),
                    new PostgisGeometryCollection(new IGeometry[] {
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

        [Test,TestCaseSource("Tests")]
        public void PostgisTestRead(TestAtt att)
        {
            using (var cmd = Conn.CreateCommand())
            {
                var a = att;
                cmd.CommandText = "Select " + a.SQL;
                var p = cmd.ExecuteScalar();
                Assert.IsTrue(p.Equals(a.Geom));
            }
        }

        [Test, TestCaseSource("Tests")]
        public void PostgisTestWrite(TestAtt a)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Geometry,a.Geom);
                a.Geom.SRID = 0;
                cmd.CommandText = "Select st_asewkb(:p1) = st_asewkb(" + a.SQL + ")";
                try
                {
                    Assert.IsTrue((bool)cmd.ExecuteScalar(),"Error on comparison of " + a.Geom);
                }
                catch (Exception)
                {
                    Assert.Fail("Exception caught on " + a.Geom);
                }

            }
        }

        [Test, TestCaseSource("Tests")]
        public void PostgisTestWriteSrid(TestAtt a)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Geometry, a.Geom);
                a.Geom.SRID = 3942;
                cmd.CommandText = "Select st_asewkb(:p1) = st_asewkb(st_setsrid("+ a.SQL + ",3942))";
                var p = (bool)cmd.ExecuteScalar();
                Assert.IsTrue(p);
            }
        }

        [Test, TestCaseSource("Tests")]
        public void PostgisTestReadSrid(TestAtt a)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select st_setsrid(" + a.SQL + ",3942)";
                var p = cmd.ExecuteScalar();
                Assert.IsTrue(p.Equals(a.Geom));
                Assert.IsTrue((p as IGeometry).SRID == 3942);
            }
        }

        [Test]
        public void PostgisTestArrayRead()
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select ARRAY(select st_makepoint(1,1))";
                var p = cmd.ExecuteScalar() as IGeometry[];
                var p2 = new PostgisPoint(1d, 1d);
                Assert.IsTrue(p != null && p[0] is PostgisPoint && p2 == (PostgisPoint)p[0]);
            }
        }

        [Test]
        public void PostgisTestArrayWrite()
        {
            using (var cmd = Conn.CreateCommand())
            {
                var p = new PostgisPoint[1] { new PostgisPoint(1d, 1d) };
                cmd.Parameters.AddWithValue(":p1", NpgsqlDbType.Array | NpgsqlDbType.Geometry, p);
                cmd.CommandText = "SELECT :p1 = array(select st_makepoint(1,1))";
                Assert.IsTrue((bool)cmd.ExecuteScalar());
            }
        }

        protected override void SetUp()
        {
            base.SetUp();
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "SELECT postgis_version();";
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (NpgsqlException)
                {
                    TestUtil.IgnoreExceptOnBuildServer(("Skipping tests : postgis extension not found."));
                }
            }
        }

        public PostgisTest(string backendVersion)
            : base(backendVersion){ }
    }
}
