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

using GeoJSON.Net;
using GeoJSON.Net.Converters;
using GeoJSON.Net.CoordinateReferenceSystem;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Npgsql.GeoJSON;
using Npgsql.Tests;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Npgsql.PluginTests
{
    public class GeoJSONTests : TestBase
    {
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

        protected override NpgsqlConnection OpenConnection(string connectionString = null)
            => OpenConnection(connectionString, GeoJSONOptions.None);

        protected NpgsqlConnection OpenConnection(string connectionString = null, GeoJSONOptions option = GeoJSONOptions.None)
        {
            var conn = base.OpenConnection(connectionString);
            conn.TypeMapper.UseGeoJson(option);
            return conn;
        }

        public struct TestData
        {
            public GeoJSONObject Geometry;
            public string CommandText;
        }

        public static readonly TestData[] Tests =
        {
            new TestData {
                Geometry = new Point(
                    new Position(longitude: 1d, latitude: 2d))
                { BoundingBoxes = new[] { 1d, 2d, 1d, 2d } },
                CommandText = "st_makepoint(1,2)"
            },
            new TestData {
                Geometry = new LineString(new[] {
                    new Position(longitude: 1d, latitude: 1d),
                    new Position(longitude: 1d, latitude: 2d)
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 2d } },
                CommandText = "st_makeline(st_makepoint(1,1), st_makepoint(1,2))"
            },
            new TestData {
                Geometry = new Polygon(new[] {
                    new LineString(new[] {
                        new Position(longitude: 1d, latitude: 1d),
                        new Position(longitude: 2d, latitude: 2d),
                        new Position(longitude: 3d, latitude: 3d),
                        new Position(longitude: 1d, latitude: 1d)
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 3d, 3d } },
                CommandText = "st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1), st_makepoint(2,2), st_makepoint(3,3), st_makepoint(1,1)]))"
            },
            new TestData {
                Geometry = new MultiPoint(new[] {
                    new Point(new Position(longitude: 1d, latitude: 1d))
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 1d } },
                CommandText = "st_multi(st_makepoint(1, 1))"
            },
            new TestData {
                Geometry = new MultiLineString(new[] {
                    new LineString(new[] {
                        new Position(longitude: 1d, latitude: 1d),
                        new Position(longitude: 1d, latitude: 2d)
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 2d } },
                CommandText = "st_multi(st_makeline(st_makepoint(1,1), st_makepoint(1,2)))"
            },
            new TestData {
                Geometry = new MultiPolygon(new[] {
                    new Polygon(new[] {
                        new LineString(new[] {
                            new Position(longitude: 1d, latitude: 1d),
                            new Position(longitude: 2d, latitude: 2d),
                            new Position(longitude: 3d, latitude: 3d),
                            new Position(longitude: 1d, latitude: 1d)
                        })
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 3d, 3d } },
                CommandText = "st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1), st_makepoint(2,2), st_makepoint(3,3), st_makepoint(1,1)])))"
            },
            new TestData {
                Geometry = new GeometryCollection(new IGeometryObject[] {
                    new Point(new Position(longitude: 1d, latitude: 1d)),
                    new MultiPolygon(new[] {
                        new Polygon(new[] {
                            new LineString(new[] {
                            new Position(longitude: 1d, latitude: 1d),
                            new Position(longitude: 2d, latitude: 2d),
                            new Position(longitude: 3d, latitude: 3d),
                            new Position(longitude: 1d, latitude: 1d)
                            })
                        })
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 3d, 3d } },
                CommandText = "st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1), st_makepoint(2,2), st_makepoint(3,3), st_makepoint(1,1)]))))"
            },
        };

        [Test, TestCaseSource(nameof(Tests))]
        public void Read(TestData data)
        {
            using (var conn = OpenConnection(option: GeoJSONOptions.BoundingBox))
            using (var cmd = new NpgsqlCommand($"SELECT {data.CommandText}, st_asgeojson({data.CommandText},options:=1)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                Assert.That(reader.Read());
                Assert.That(reader.GetFieldValue<GeoJSONObject>(0), Is.EqualTo(data.Geometry));
                Assert.That(reader.GetFieldValue<GeoJSONObject>(0), Is.EqualTo(JsonConvert.DeserializeObject<IGeometryObject>(reader.GetFieldValue<string>(1), new GeometryConverter())));
            }
        }

        [Test, TestCaseSource(nameof(Tests))]
        public void Write(TestData data)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($"SELECT st_asewkb(@p) = st_asewkb({data.CommandText})", conn))
            {
                cmd.Parameters.AddWithValue("p", data.Geometry);
                Assert.That(cmd.ExecuteScalar(), Is.True);
            }
        }

        [Test]
        public void IgnoreM()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_makepointm(1,1,1)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                Assert.That(reader.Read());
                Assert.That(reader.GetFieldValue<Point>(0), Is.EqualTo(new Point(new Position(1d, 1d))));
            }
        }

        public static readonly TestData[] NotAllZSpecifiedTests =
        {
            new TestData {
                Geometry = new LineString(new[] {
                    new Position(1d, 1d, 0d),
                    new Position(2d, 2d)
                })
            },
            new TestData {
                Geometry =  new LineString(new[] {
                    new Position(1d, 1d, 0d),
                    new Position(2d, 2d),
                    new Position(3d, 3d),
                    new Position(4d, 4d)
                })
            }
        };

        [Test, TestCaseSource(nameof(NotAllZSpecifiedTests))]
        public void NotAllZSpecified(TestData data)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", data.Geometry);
                Assert.That(() => cmd.ExecuteScalar(), Throws.ArgumentException);
            }
        }

        [Test]
        public void ReadUnknownCRS()
        {
            using (var conn = OpenConnection(option: GeoJSONOptions.ShortCRS))
            using (var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 1)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                Assert.That(reader.Read());
                Assert.That(() => reader.GetValue(0), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void ReadUnspecifiedCRS()
        {
            using (var conn = OpenConnection(option: GeoJSONOptions.ShortCRS))
            using (var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 0)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                Assert.That(reader.Read());
                Assert.That(reader.GetFieldValue<Point>(0).CRS, Is.Null);
            }
        }

        [Test]
        public void ReadShortCRS()
        {
            using (var conn = OpenConnection(option: GeoJSONOptions.ShortCRS))
            using (var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 4326)", conn))
            {
                var point = (Point)cmd.ExecuteScalar();
                var crs = point.CRS as NamedCRS;

                Assert.That(crs, Is.Not.Null);
                Assert.That(crs.Properties["name"], Is.EqualTo("EPSG:4326"));
            }
        }

        [Test]
        public void ReadLongCRS()
        {
            using (var conn = OpenConnection(option: GeoJSONOptions.LongCRS))
            using (var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 4326)", conn))
            {
                var point = (Point)cmd.ExecuteScalar();
                var crs = point.CRS as NamedCRS;

                Assert.That(crs, Is.Not.Null);
                Assert.That(crs.Properties["name"], Is.EqualTo("urn:ogc:def:crs:EPSG::4326"));
            }
        }

        [Test]
        public void WriteIllFormedCRS()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("ill:formed") });
                Assert.That(() => cmd.ExecuteScalar(), Throws.TypeOf<FormatException>());
            }
        }

        [Test]
        public void WriteLinkedCRS()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new LinkedCRS("href") });
                Assert.That(() => cmd.ExecuteScalar(), Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void WriteUnspecifiedCRS()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new UnspecifiedCRS() });
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(0));
            }
        }

        [Test]
        public void WriteShortCRS()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("EPSG:4326") });
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(4326));
            }
        }

        [Test]
        public void WriteLongCRS()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("urn:ogc:def:crs:EPSG::4326") });
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(4326));
            }
        }

        [Test]
        public void WriteCRS84()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn))
            {
                cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("urn:ogc:def:crs:OGC::CRS84") });
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(4326));
            }
        }

        [Test]
        public void RoundtripGeometryGeography()
        {
            var point = new Point(new Position(0d, 0d));
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (geom GEOMETRY, geog GEOGRAPHY)");
                using (var cmd = new NpgsqlCommand("INSERT INTO data (geom, geog) VALUES (@p, @p)", conn))
                {
                    cmd.Parameters.AddWithValue("p", point);
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
    }
}
