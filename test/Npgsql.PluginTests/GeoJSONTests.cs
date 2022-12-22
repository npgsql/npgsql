using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeoJSON.Net;
using GeoJSON.Net.Converters;
using GeoJSON.Net.CoordinateReferenceSystem;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Npgsql.GeoJSON.Internal;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.PluginTests;

public class GeoJSONTests : TestBase
{
    public struct TestData
    {
        public GeoJSONObject Geometry;
        public string CommandText;
    }

    public static readonly TestData[] Tests =
    {
        new()
        {
            Geometry = new Point(
                    new Position(longitude: 1d, latitude: 2d))
                { BoundingBoxes = new[] { 1d, 2d, 1d, 2d } },
            CommandText = "st_makepoint(1,2)"
        },
        new()
        {
            Geometry = new LineString(new[]
                {
                    new Position(longitude: 1d, latitude: 1d),
                    new Position(longitude: 1d, latitude: 2d)
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 2d } },
            CommandText = "st_makeline(st_makepoint(1,1), st_makepoint(1,2))"
        },
        new()
        {
            Geometry = new Polygon(new[]
                {
                    new LineString(new[]
                    {
                        new Position(longitude: 1d, latitude: 1d),
                        new Position(longitude: 2d, latitude: 2d),
                        new Position(longitude: 3d, latitude: 3d),
                        new Position(longitude: 1d, latitude: 1d)
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 3d, 3d } },
            CommandText = "st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1), st_makepoint(2,2), st_makepoint(3,3), st_makepoint(1,1)]))"
        },
        new()
        {
            Geometry = new MultiPoint(new[]
                {
                    new Point(new Position(longitude: 1d, latitude: 1d))
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 1d } },
            CommandText = "st_multi(st_makepoint(1, 1))"
        },
        new()
        {
            Geometry = new MultiLineString(new[]
                {
                    new LineString(new[]
                    {
                        new Position(longitude: 1d, latitude: 1d),
                        new Position(longitude: 1d, latitude: 2d)
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 2d } },
            CommandText = "st_multi(st_makeline(st_makepoint(1,1), st_makepoint(1,2)))"
        },
        new()
        {
            Geometry = new MultiPolygon(new[]
                {
                    new Polygon(new[]
                    {
                        new LineString(new[]
                        {
                            new Position(longitude: 1d, latitude: 1d),
                            new Position(longitude: 2d, latitude: 2d),
                            new Position(longitude: 3d, latitude: 3d),
                            new Position(longitude: 1d, latitude: 1d)
                        })
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 3d, 3d } },
            CommandText =
                "st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1), st_makepoint(2,2), st_makepoint(3,3), st_makepoint(1,1)])))"
        },
        new()
        {
            Geometry = new GeometryCollection(new IGeometryObject[]
                {
                    new Point(new Position(longitude: 1d, latitude: 1d)),
                    new MultiPolygon(new[]
                    {
                        new Polygon(new[]
                        {
                            new LineString(new[]
                            {
                                new Position(longitude: 1d, latitude: 1d),
                                new Position(longitude: 2d, latitude: 2d),
                                new Position(longitude: 3d, latitude: 3d),
                                new Position(longitude: 1d, latitude: 1d)
                            })
                        })
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 3d, 3d } },
            CommandText =
                "st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1), st_makepoint(2,2), st_makepoint(3,3), st_makepoint(1,1)]))))"
        },
    };

    [Test, TestCaseSource(nameof(Tests))]
    public void Read(TestData data)
    {
        using var conn = OpenConnection(option: GeoJSONOptions.BoundingBox);
        using var cmd = new NpgsqlCommand($"SELECT {data.CommandText}, st_asgeojson({data.CommandText},options:=1)", conn);
        using var reader = cmd.ExecuteReader();
        Assert.That(reader.Read());
        Assert.That(reader.GetFieldValue<GeoJSONObject>(0), Is.EqualTo(data.Geometry));
        Assert.That(reader.GetFieldValue<GeoJSONObject>(0),
            Is.EqualTo(JsonConvert.DeserializeObject<IGeometryObject>(reader.GetFieldValue<string>(1), new GeometryConverter())));
    }

    [Test, TestCaseSource(nameof(Tests))]
    public void Write(TestData data)
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand($"SELECT st_asewkb(@p) = st_asewkb({data.CommandText})", conn);
        cmd.Parameters.AddWithValue("p", data.Geometry);
        Assert.That(cmd.ExecuteScalar(), Is.True);
    }

    [Test]
    public void IgnoreM()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_makepointm(1,1,1)", conn);
        using var reader = cmd.ExecuteReader();
        Assert.That(reader.Read());
        Assert.That(reader.GetFieldValue<Point>(0), Is.EqualTo(new Point(new Position(1d, 1d))));
    }

    public static readonly TestData[] NotAllZSpecifiedTests =
    {
        new()
        {
            Geometry = new LineString(new[]
            {
                new Position(1d, 1d, 0d),
                new Position(2d, 2d)
            })
        },
        new()
        {
            Geometry = new LineString(new[]
            {
                new Position(1d, 1d, 0d),
                new Position(2d, 2d),
                new Position(3d, 3d),
                new Position(4d, 4d)
            })
        }
    };

    [Test, TestCaseSource(nameof(NotAllZSpecifiedTests))]
    public void Not_all_Z_specified(TestData data)
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", data.Geometry);
        Assert.That(() => cmd.ExecuteScalar(), Throws.ArgumentException);
    }

    [Test]
    public void Read_unknown_CRS()
    {
        using var conn = OpenConnection(option: GeoJSONOptions.ShortCRS);
        using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 1)", conn);
        using var reader = cmd.ExecuteReader();
        Assert.That(reader.Read());
        Assert.That(() => reader.GetValue(0), Throws.InvalidOperationException);
    }

    [Test]
    public void Read_unspecified_CRS()
    {
        using var conn = OpenConnection(option: GeoJSONOptions.ShortCRS);
        using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 0)", conn);
        using var reader = cmd.ExecuteReader();
        Assert.That(reader.Read());
        Assert.That(reader.GetFieldValue<Point>(0).CRS, Is.Null);
    }

    [Test]
    public void Read_short_CRS()
    {
        using var conn = OpenConnection(option: GeoJSONOptions.ShortCRS);
        using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 4326)", conn);
        var point = (Point)cmd.ExecuteScalar()!;
        var crs = point.CRS as NamedCRS;

        Assert.That(crs, Is.Not.Null);
        Assert.That(crs!.Properties["name"], Is.EqualTo("EPSG:4326"));
    }

    [Test]
    public void Read_long_CRS()
    {
        using var conn = OpenConnection(option: GeoJSONOptions.LongCRS);
        using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 4326)", conn);
        var point = (Point)cmd.ExecuteScalar()!;
        var crs = point.CRS as NamedCRS;

        Assert.That(crs, Is.Not.Null);
        Assert.That(crs!.Properties["name"], Is.EqualTo("urn:ogc:def:crs:EPSG::4326"));
    }

    [Test]
    public void Write_ill_formed_CRS()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("ill:formed") });
        Assert.That(() => cmd.ExecuteScalar(), Throws.TypeOf<FormatException>());
    }

    [Test]
    public void Write_linked_CRS()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new LinkedCRS("href") });
        Assert.That(() => cmd.ExecuteScalar(), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void Write_unspecified_CRS()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new UnspecifiedCRS() });
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(0));
    }

    [Test]
    public void Write_short_CRS()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("EPSG:4326") });
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(4326));
    }

    [Test]
    public void Write_long_CRS()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("urn:ogc:def:crs:EPSG::4326") });
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(4326));
    }

    [Test]
    public void Write_CRS84()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("urn:ogc:def:crs:OGC::CRS84") });
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(4326));
    }

    [Test]
    public void Roundtrip_geometry_geography()
    {
        var point = new Point(new Position(0d, 0d));
        using var conn = OpenConnection();
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

    [Test, TestCaseSource(nameof(Tests))]
    public void Import_geometry(TestData data)
    {
        using var conn = OpenConnection(option: GeoJSONOptions.BoundingBox);
        conn.ExecuteNonQuery("CREATE TEMP TABLE data (field geometry)");

        using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
        {
            writer.StartRow();
            writer.Write(data.Geometry, NpgsqlDbType.Geometry);

            var rowsWritten = writer.Complete();
            Assert.That(rowsWritten, Is.EqualTo(1));
        }

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT field FROM data";
        using var reader = cmd.ExecuteReader();
        Assert.IsTrue(reader.Read());
        var actual = reader.GetValue(0);
        Assert.That(actual, Is.EqualTo(data.Geometry));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4827")]
    public void Import_big_geometry()
    {
        using var conn = OpenConnection();
        conn.ExecuteNonQuery("CREATE TEMP TABLE data (id text, field geometry)");

        var geometry = new MultiLineString(new[]
        {
            new LineString(
                Enumerable.Range(1, 507)
                    .Select(i => new Position(longitude: i, latitude: i))
                    .Append(new Position(longitude: 1d, latitude: 1d))),
            new LineString(new[]
            {
                new Position(longitude: 1d, latitude: 1d),
                new Position(longitude: 1d, latitude: 2d),
                new Position(longitude: 1d, latitude: 3d),
                new Position(longitude: 1d, latitude: 1d),
            })
        });

        using (var writer = conn.BeginBinaryImport("COPY data (id, field) FROM STDIN BINARY"))
        {
            writer.StartRow();
            writer.Write("a", NpgsqlDbType.Text);
            writer.Write(geometry, NpgsqlDbType.Geometry);

            var rowsWritten = writer.Complete();
            Assert.That(rowsWritten, Is.EqualTo(1));
        }

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT field FROM data";
        using var reader = cmd.ExecuteReader();
        Assert.IsTrue(reader.Read());
        var actual = reader.GetValue(0);
        Assert.That(actual, Is.EqualTo(geometry));
    }

    [Test, TestCaseSource(nameof(Tests))]
    public void Export_geometry(TestData data)
    {
        using var conn = OpenConnection(option: GeoJSONOptions.BoundingBox);
        conn.ExecuteNonQuery("CREATE TEMP TABLE data (field geometry)");

        using (var writer = conn.BeginBinaryImport("COPY data (field) FROM STDIN BINARY"))
        {
            writer.StartRow();
            writer.Write(data.Geometry, NpgsqlDbType.Geometry);

            var rowsWritten = writer.Complete();
            Assert.That(rowsWritten, Is.EqualTo(1));
        }

        using (var reader = conn.BeginBinaryExport("COPY data (field) TO STDOUT BINARY"))
        {
            reader.StartRow();
            var field = reader.Read<GeoJSONObject>(NpgsqlDbType.Geometry);
            Assert.That(field, Is.EqualTo(data.Geometry));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4830")]
    public void Export_big_geometry()
    {
        using var conn = OpenConnection();
        conn.ExecuteNonQuery("CREATE TEMP TABLE data (id text, field geometry)");

        var geometry = new Polygon(new[] {
            new LineString(
                Enumerable.Range(1, 507)
                    .Select(i => new Position(longitude: i, latitude: i))
                    .Append(new Position(longitude: 1d, latitude: 1d))),
            new LineString(new[] {
                new Position(longitude: 1d, latitude: 1d),
                new Position(longitude: 1d, latitude: 2d),
                new Position(longitude: 1d, latitude: 3d),
                new Position(longitude: 1d, latitude: 1d),
            })
        });

        using (var writer = conn.BeginBinaryImport("COPY data (id, field) FROM STDIN BINARY"))
        {
            writer.StartRow();
            writer.Write("aaaa", NpgsqlDbType.Text);
            writer.Write(geometry, NpgsqlDbType.Geometry);

            var rowsWritten = writer.Complete();
            Assert.That(rowsWritten, Is.EqualTo(1));
        }

        using (var reader = conn.BeginBinaryExport("COPY data (id, field) TO STDOUT BINARY"))
        {
            reader.StartRow();
            var id = reader.Read<string>();
            var field = reader.Read<GeoJSONObject>(NpgsqlDbType.Geometry);
            Assert.That(id, Is.EqualTo("aaaa"));
            Assert.That(field, Is.EqualTo(geometry));
        }
    }

    protected override NpgsqlConnection OpenConnection(string? connectionString = null)
        => OpenConnection(connectionString, GeoJSONOptions.None);

    protected NpgsqlConnection OpenConnection(string? connectionString = null, GeoJSONOptions option = GeoJSONOptions.None)
    {
        var conn = base.OpenConnection(connectionString);
        conn.TypeMapper.UseGeoJson(option);
        return conn;
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var conn = await base.OpenConnectionAsync();
        await TestUtil.EnsurePostgis(conn);
    }
}
