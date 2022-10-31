using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GeoJSON.Net;
using GeoJSON.Net.Converters;
using GeoJSON.Net.CoordinateReferenceSystem;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using Npgsql.Tests;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

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
            Geometry = new LineString(new[] {
                    new Position(longitude: 1d, latitude: 1d),
                    new Position(longitude: 1d, latitude: 2d)
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 2d } },
            CommandText = "st_makeline(st_makepoint(1,1), st_makepoint(1,2))"
        },
        new()
        {
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
        new()
        {
            Geometry = new MultiPoint(new[] {
                    new Point(new Position(longitude: 1d, latitude: 1d))
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 1d } },
            CommandText = "st_multi(st_makepoint(1, 1))"
        },
        new()
        {
            Geometry = new MultiLineString(new[] {
                    new LineString(new[] {
                        new Position(longitude: 1d, latitude: 1d),
                        new Position(longitude: 1d, latitude: 2d)
                    })
                })
                { BoundingBoxes = new[] { 1d, 1d, 1d, 2d } },
            CommandText = "st_multi(st_makeline(st_makepoint(1,1), st_makepoint(1,2)))"
        },
        new()
        {
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
        new()
        {
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
    public async Task Read(TestData data)
    {
        await using var conn = await OpenConnectionAsync(GeoJSONOptions.BoundingBox);
        await using var cmd = new NpgsqlCommand($"SELECT {data.CommandText}, st_asgeojson({data.CommandText},options:=1)", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync());
        Assert.That(reader.GetFieldValue<GeoJSONObject>(0), Is.EqualTo(data.Geometry));
        Assert.That(reader.GetFieldValue<GeoJSONObject>(0), Is.EqualTo(JsonConvert.DeserializeObject<IGeometryObject>(reader.GetFieldValue<string>(1), new GeometryConverter())));
    }

    [Test, TestCaseSource(nameof(Tests))]
    public async Task Write(TestData data)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand($"SELECT st_asewkb(@p) = st_asewkb({data.CommandText})", conn);
        cmd.Parameters.AddWithValue("p", data.Geometry);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.True);
    }

    [Test]
    public async Task IgnoreM()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_makepointm(1,1,1)", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync());
        Assert.That(reader.GetFieldValue<Point>(0), Is.EqualTo(new Point(new Position(1d, 1d))));
    }

    public static readonly TestData[] NotAllZSpecifiedTests =
    {
        new()
        {
            Geometry = new LineString(new[] {
                new Position(1d, 1d, 0d),
                new Position(2d, 2d)
            })
        },
        new()
        {
            Geometry =  new LineString(new[] {
                new Position(1d, 1d, 0d),
                new Position(2d, 2d),
                new Position(3d, 3d),
                new Position(4d, 4d)
            })
        }
    };

    [Test, TestCaseSource(nameof(NotAllZSpecifiedTests))]
    public async Task Not_all_Z_specified(TestData data)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.AddWithValue("p", data.Geometry);
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.ArgumentException);
    }

    [Test]
    public async Task Read_unknown_CRS()
    {
        await using var conn = await OpenConnectionAsync(GeoJSONOptions.ShortCRS);
        await using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 1)", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync());
        Assert.That(() => reader.GetValue(0), Throws.InvalidOperationException);
    }

    [Test]
    public async Task Read_unspecified_CRS()
    {
        await using var conn = await OpenConnectionAsync(GeoJSONOptions.ShortCRS);
        await using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 0)", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.That(await reader.ReadAsync());
        Assert.That(reader.GetFieldValue<Point>(0).CRS, Is.Null);
    }

    [Test]
    public async Task Read_short_CRS()
    {
        await using var conn = await OpenConnectionAsync(GeoJSONOptions.ShortCRS);
        await using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 4326)", conn);
        var point = (Point)(await cmd.ExecuteScalarAsync())!;
        var crs = point.CRS as NamedCRS;

        Assert.That(crs, Is.Not.Null);
        Assert.That(crs!.Properties["name"], Is.EqualTo("EPSG:4326"));
    }

    [Test]
    public async Task Read_long_CRS()
    {
        await using var conn = await OpenConnectionAsync(GeoJSONOptions.LongCRS);
        await using var cmd = new NpgsqlCommand("SELECT st_setsrid(st_makepoint(0,0), 4326)", conn);
        var point = (Point)(await cmd.ExecuteScalarAsync())!;
        var crs = point.CRS as NamedCRS;

        Assert.That(crs, Is.Not.Null);
        Assert.That(crs!.Properties["name"], Is.EqualTo("urn:ogc:def:crs:EPSG::4326"));
    }

    [Test]
    public async Task Write_ill_formed_CRS()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("ill:formed") });
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.TypeOf<FormatException>());
    }

    [Test]
    public async Task Write_linked_CRS()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new LinkedCRS("href") });
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task Write_unspecified_CRS()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new UnspecifiedCRS() });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task Write_short_CRS()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("EPSG:4326") });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(4326));
    }

    [Test]
    public async Task Write_long_CRS()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("urn:ogc:def:crs:EPSG::4326") });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(4326));
    }

    [Test]
    public async Task Write_CRS84()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_srid(@p)", conn);
        cmd.Parameters.AddWithValue("p", new Point(new Position(0d, 0d)) { CRS = new NamedCRS("urn:ogc:def:crs:OGC::CRS84") });
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(4326));
    }

    [Test]
    public async Task Roundtrip_geometry_geography()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "geom GEOMETRY, geog GEOGRAPHY");

        var point = new Point(new Position(0d, 0d));
        await using (var cmd = new NpgsqlCommand($"INSERT INTO {table} (geom, geog) VALUES (@p, @p)", conn))
        {
            cmd.Parameters.AddWithValue("p", point);
            await cmd.ExecuteNonQueryAsync();
        }

        await using (var cmd = new NpgsqlCommand($"SELECT geom, geog FROM {table}", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            await reader.ReadAsync();
            Assert.That(reader[0], Is.EqualTo(point));
            Assert.That(reader[1], Is.EqualTo(point));
        }
    }

    ValueTask<NpgsqlConnection> OpenConnectionAsync(GeoJSONOptions options = GeoJSONOptions.None)
        => GetDataSource(options).OpenConnectionAsync();

    NpgsqlDataSource GetDataSource(GeoJSONOptions options = GeoJSONOptions.None)
        => GeoJsonDataSources.GetOrAdd(options, _ =>
        {
            var dataSourceBuilder = CreateDataSourceBuilder();
            dataSourceBuilder.UseGeoJson(options);
            return dataSourceBuilder.Build();
        });

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var conn = await OpenConnectionAsync();
        await EnsurePostgis(conn);
    }

    [OneTimeTearDown]
    public async Task Teardown()
        => await Task.WhenAll(GeoJsonDataSources.Values.Select(async ds => await ds.DisposeAsync()));

    ConcurrentDictionary<GeoJSONOptions, NpgsqlDataSource> GeoJsonDataSources = new();
}
