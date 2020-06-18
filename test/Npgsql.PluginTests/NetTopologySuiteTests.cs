using System;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using Npgsql.Tests;
using NUnit.Framework;

namespace Npgsql.PluginTests
{
    public class NetTopologySuiteTests : TestBase
    {
        public struct TestData
        {
            public Ordinates Ordinates;
            public Geometry Geometry;
            public string CommandText;
        }

        public static IEnumerable TestCases {
            get
            {
                // Two dimensional data
                yield return new TestCaseData(Ordinates.None, new Point(1d, 2500d), "st_makepoint(1,2500)");

                yield return new TestCaseData(
                    Ordinates.None,
                    new LineString(new[] { new Coordinate(1d, 1d), new Coordinate(1d, 2500d) }),
                    "st_makeline(st_makepoint(1,1),st_makepoint(1,2500))"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    new Polygon(
                        new LinearRing(new[]
                        {
                            new Coordinate(1d, 1d),
                            new Coordinate(2d, 2d),
                            new Coordinate(3d, 3d),
                            new Coordinate(1d, 1d)
                        })
                    ),
                    "st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)]))"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    new MultiPoint(new[] { new Point(new Coordinate(1d, 1d)) }),
                    "st_multi(st_makepoint(1, 1))"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    new MultiLineString(new[]
                    {
                        new LineString(new[]
                        {
                            new Coordinate(1d, 1d),
                            new Coordinate(1d, 2500d)
                        })
                    }),
                    "st_multi(st_makeline(st_makepoint(1,1),st_makepoint(1,2500)))"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    new MultiPolygon(new[]
                    {
                        new Polygon(
                            new LinearRing(new[]
                            {
                                new Coordinate(1d, 1d),
                                new Coordinate(2d, 2d),
                                new Coordinate(3d, 3d),
                                new Coordinate(1d, 1d)
                            })
                        )
                    }),
                    "st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])))"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    GeometryCollection.Empty,
                    "st_geomfromtext('GEOMETRYCOLLECTION EMPTY')"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    new GeometryCollection(new Geometry[]
                    {
                        new Point(new Coordinate(1d, 1d)),
                        new MultiPolygon(new[]
                        {
                            new Polygon(
                                new LinearRing(new[]
                                {
                                    new Coordinate(1d, 1d),
                                    new Coordinate(2d, 2d),
                                    new Coordinate(3d, 3d),
                                    new Coordinate(1d, 1d)
                                })
                            )
                        })
                    }),
                    "st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)]))))"
                );

                yield return new TestCaseData(
                    Ordinates.None,
                    new GeometryCollection(new Geometry[]
                    {
                        new Point(new Coordinate(1d, 1d)),
                        new GeometryCollection(new Geometry[]
                        {
                            new Point(new Coordinate(1d, 1d)),
                            new MultiPolygon(new[]
                            {
                                new Polygon(
                                    new LinearRing(new[]
                                    {
                                        new Coordinate(1d, 1d),
                                        new Coordinate(2d, 2d),
                                        new Coordinate(3d, 3d),
                                        new Coordinate(1d, 1d)
                                    })
                                )
                            })
                        })
                    }),
                    "st_collect(st_makepoint(1,1),st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])))))"
                );

                yield return new TestCaseData(Ordinates.XYZ, new Point(1d, 2d, 3d), "st_makepoint(1,2,3)");

                yield return new TestCaseData(
                    Ordinates.XYZM,
                    new Point(
                        new DotSpatialAffineCoordinateSequence(new[] { 1d, 2d }, new[] { 3d }, new[] { 4d }),
                        GeometryFactory.Default),
                    "st_makepoint(1,2,3,4)"
                );
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void TestRead(Ordinates ordinates, Geometry geometry, string sqlRepresentation)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"SELECT {sqlRepresentation}";
                Assert.That(Equals(cmd.ExecuteScalar(), geometry));
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void TestWrite(Ordinates ordinates, Geometry geometry, string sqlRepresentation)
        {
            using (var conn = OpenConnection(handleOrdinates: ordinates))
            using (var cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("p1", geometry);
                cmd.CommandText = $"SELECT st_asewkb(@p1) = st_asewkb({sqlRepresentation})";
                Assert.That(cmd.ExecuteScalar(), Is.True);
            }
        }

        [Test]
        public void TestArrayRead()
        {
            using (var conn = OpenConnection(handleOrdinates: Ordinates.XY))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT ARRAY(SELECT st_makepoint(1,1))";
                var result = cmd.ExecuteScalar();
                Assert.That(result, Is.InstanceOf<Geometry[]>());
                Assert.That(result, Is.EquivalentTo(new[] { new Point(new Coordinate(1d, 1d)) }));
            }
        }

        [Test]
        public void TestArrayWrite()
        {
            using (var conn = OpenConnection(handleOrdinates: Ordinates.XY))
            using (var cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@p1", new[] { new Point(new Coordinate(1d, 1d)) });
                cmd.CommandText = "SELECT @p1 = array(select st_makepoint(1,1))";
                Assert.That(cmd.ExecuteScalar(), Is.True);
            }
        }

        [Test]
        public void ReadAsConcreteType()
        {
            using (var conn = OpenConnection(handleOrdinates: Ordinates.XY))
            using (var cmd = new NpgsqlCommand("SELECT st_makepoint(1,1)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.That(reader.GetFieldValue<Point>(0), Is.EqualTo(new Point(new Coordinate(1d, 1d))));
                Assert.That(() => reader.GetFieldValue<Polygon>(0), Throws.Exception.TypeOf<InvalidCastException>());
            }
        }

        [Test]
        public void RoundtripGeometryGeography()
        {
            var point = new Point(new Coordinate(1d, 1d));
            using (var conn = OpenConnection(handleOrdinates: Ordinates.XY))
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (geom GEOMETRY, geog GEOGRAPHY)");
                using (var cmd = new NpgsqlCommand("INSERT INTO data (geom, geog) VALUES (@p, @p)", conn))
                {
                    cmd.Parameters.AddWithValue("@p", point);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("SELECT geom, geog FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader[0], Is.EqualTo(point));
                    Assert.That(reader[1], Is.EqualTo(point));
                }
            }
        }

        protected override NpgsqlConnection OpenConnection(string? connectionString = null)
            => OpenConnection(connectionString);

        protected NpgsqlConnection OpenConnection(string? connectionString = null, Ordinates handleOrdinates = Ordinates.None)
        {
            if (handleOrdinates == Ordinates.None)
                handleOrdinates = Ordinates.XY;

            var conn = base.OpenConnection(connectionString);
            conn.TypeMapper.UseNetTopologySuite(
                new DotSpatialAffineCoordinateSequenceFactory(handleOrdinates),
                handleOrdinates: handleOrdinates);
            return conn;
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using var conn = await base.OpenConnectionAsync();
            await TestUtil.EnsurePostgis(conn);
        }
    }
}
