using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using Npgsql.Tests;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.PluginTests;

public class NetTopologySuiteTests : TestBase
{
    static readonly TestCaseData[] TestCases =
    [
        new TestCaseData(Ordinates.None, new Point(1d, 2500d), "st_makepoint(1,2500)")
            .SetName("Point"),

        new TestCaseData(Ordinates.None, new MultiPoint([new Point(new Coordinate(1d, 1d))]), "st_multi(st_makepoint(1, 1))")
            .SetName("MultiPoint"),

        new TestCaseData(
                Ordinates.None,
                new LineString([new Coordinate(1d, 1d), new Coordinate(1d, 2500d)]),
                "st_makeline(st_makepoint(1,1),st_makepoint(1,2500))")
            .SetName("LineString"),

        new TestCaseData(
                Ordinates.None,
                new MultiLineString([
                    new LineString([
                        new Coordinate(1d, 1d),
                        new Coordinate(1d, 2500d)
                    ])
                ]),
                "st_multi(st_makeline(st_makepoint(1,1),st_makepoint(1,2500)))")
            .SetName("MultiLineString"),

        new TestCaseData(
                Ordinates.None,
                new Polygon(
                    new LinearRing([
                        new Coordinate(1d, 1d),
                        new Coordinate(2d, 2d),
                        new Coordinate(3d, 3d),
                        new Coordinate(1d, 1d)
                    ])
                ),
                "st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)]))")
            .SetName("Polygon"),

        new TestCaseData(
                Ordinates.None,
                new MultiPolygon([
                    new Polygon(
                        new LinearRing([
                            new Coordinate(1d, 1d),
                            new Coordinate(2d, 2d),
                            new Coordinate(3d, 3d),
                            new Coordinate(1d, 1d)
                        ])
                    )
                ]),
                "st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])))")
            .SetName("MultiPolygon"),

        new TestCaseData(Ordinates.None, GeometryCollection.Empty, "st_geomfromtext('GEOMETRYCOLLECTION EMPTY')")
            .SetName("EmptyCollection"),

        new TestCaseData(
                Ordinates.None,
                new GeometryCollection([
                    new Point(new Coordinate(1d, 1d)),
                    new MultiPolygon([
                        new Polygon(
                            new LinearRing([
                                new Coordinate(1d, 1d),
                                new Coordinate(2d, 2d),
                                new Coordinate(3d, 3d),
                                new Coordinate(1d, 1d)
                            ])
                        )
                    ])
                ]),
                "st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)]))))")
            .SetName("Collection"),

        new TestCaseData(
                Ordinates.None,
                new GeometryCollection([
                    new Point(new Coordinate(1d, 1d)),
                    new GeometryCollection([
                        new Point(new Coordinate(1d, 1d)),
                        new MultiPolygon([
                            new Polygon(
                                new LinearRing([
                                    new Coordinate(1d, 1d),
                                    new Coordinate(2d, 2d),
                                    new Coordinate(3d, 3d),
                                    new Coordinate(1d, 1d)
                                ])
                            )
                        ])
                    ])
                ]),
                "st_collect(st_makepoint(1,1),st_collect(st_makepoint(1,1),st_multi(st_makepolygon(st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])))))")
            .SetName("CollectionNested"),

        new TestCaseData(Ordinates.XYZ, new Point(1d, 2d, 3d), "st_makepoint(1,2,3)")
            .SetName("PointXYZ"),

        new TestCaseData(
                Ordinates.XYZM,
                new Point(
                    new DotSpatialAffineCoordinateSequence([1d, 2d], [3d], [4d]),
                    GeometryFactory.Default),
                "st_makepoint(1,2,3,4)")
            .SetName("PointXYZM"),

        new TestCaseData(
                Ordinates.None,
                new LinearRing([
                    new Coordinate(1d, 1d),
                        new Coordinate(2d, 2d),
                        new Coordinate(3d, 3d),
                        new Coordinate(1d, 1d)
                ]),
                "st_makeline(ARRAY[st_makepoint(1,1),st_makepoint(2,2),st_makepoint(3,3),st_makepoint(1,1)])")
            .SetName("LinearRing")
    ];

    [Test, TestCaseSource(nameof(TestCases))]
    public async Task Read(Ordinates ordinates, Geometry geometry, string sqlRepresentation)
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT {sqlRepresentation}";
        Assert.That(Equals(cmd.ExecuteScalar(), geometry));
    }

    [Test, TestCaseSource(nameof(TestCases))]
    public async Task Write(Ordinates ordinates, Geometry geometry, string sqlRepresentation)
    {
        await using var conn = await OpenConnectionAsync(handleOrdinates: ordinates);
        await using var cmd = conn.CreateCommand();
        cmd.Parameters.AddWithValue("p1", geometry);
        cmd.CommandText = $"SELECT st_asewkb(@p1) = st_asewkb({sqlRepresentation})";
        Assert.That(cmd.ExecuteScalar(), Is.True);
    }

    [Test]
    public async Task Array()
    {
        var point = new Point(new Coordinate(1d, 1d));

        await AssertType(
            DataSource,
            new Geometry[] { point },
            '{' + GetSqlLiteral(point) + '}',
            "geometry[]",
            NpgsqlDbType.Geometry | NpgsqlDbType.Array,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public async Task Read_as_concrete_type()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT st_makepoint(1,1)", conn);
        await using var reader = cmd.ExecuteReader();
        reader.Read();
        Assert.That(reader.GetFieldValue<Point>(0), Is.EqualTo(new Point(new Coordinate(1d, 1d))));
        Assert.That(() => reader.GetFieldValue<Polygon>(0), Throws.Exception.TypeOf<InvalidCastException>());
    }

    [Test]
    public async Task Roundtrip_geometry_geography()
    {
        var point = new Point(new Coordinate(1d, 1d));
        await using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync("CREATE TEMP TABLE data (geom GEOMETRY, geog GEOGRAPHY)");
        await using (var cmd = new NpgsqlCommand("INSERT INTO data (geom, geog) VALUES (@p, @p)", conn))
        {
            cmd.Parameters.AddWithValue("@p", point);
            cmd.ExecuteNonQuery();
        }

        await using (var cmd = new NpgsqlCommand("SELECT geom, geog FROM data", conn))
        await using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            Assert.That(reader[0], Is.EqualTo(point));
            Assert.That(reader[1], Is.EqualTo(point));
        }
    }

    [Test, Explicit]
    public async Task Concurrency_test()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var table = await CreateTempTable(
            adminConnection,
            "point GEOMETRY, linestring GEOMETRY, polygon GEOMETRY, " +
            "multipoint GEOMETRY, multilinestring GEOMETRY, multipolygon GEOMETRY, " +
            "collection GEOMETRY");
        await adminConnection.ExecuteNonQueryAsync($"INSERT INTO {table} DEFAULT VALUES");

        var point = new Point(new Coordinate(1d, 1d));
        var lineString = new LineString([new Coordinate(1d, 1d), new Coordinate(1d, 2500d)]);
        var polygon = new Polygon(
            new LinearRing([
                new Coordinate(1d, 1d),
                new Coordinate(2d, 2d),
                new Coordinate(3d, 3d),
                new Coordinate(1d, 1d)
            ])
        );
        var multiPoint = new MultiPoint([new Point(new Coordinate(1d, 1d))]);
        var multiLineString = new MultiLineString([
            new LineString([
                new Coordinate(1d, 1d),
                new Coordinate(1d, 2500d)
            ])
        ]);
        var multiPolygon = new MultiPolygon([
            new Polygon(
                new LinearRing([
                    new Coordinate(1d, 1d),
                    new Coordinate(2d, 2d),
                    new Coordinate(3d, 3d),
                    new Coordinate(1d, 1d)
                ])
            )
        ]);
        var collection = new GeometryCollection([
            new Point(new Coordinate(1d, 1d)),
            new MultiPolygon([
                new Polygon(
                    new LinearRing([
                        new Coordinate(1d, 1d),
                        new Coordinate(2d, 2d),
                        new Coordinate(3d, 3d),
                        new Coordinate(1d, 1d)
                    ])
                )
            ])
        ]);

        await Task.WhenAll(Enumerable.Range(0, 30).Select(i => Task.Run(async () =>
        {
            for (var i = 0; i < 1000; i++)
            {
                await using var connection = OpenConnection();

                await using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText =
                        $"UPDATE {table} SET point=$1, linestring=$2, polygon=$3, multipoint=$4, multilinestring=$5, multipolygon=$6, collection=$7";
                    cmd.Parameters.Add(new() { Value = point });
                    cmd.Parameters.Add(new() { Value = lineString });
                    cmd.Parameters.Add(new() { Value = polygon });
                    cmd.Parameters.Add(new() { Value = multiPoint });
                    cmd.Parameters.Add(new() { Value = multiLineString });
                    cmd.Parameters.Add(new() { Value = multiPolygon });
                    cmd.Parameters.Add(new() { Value = collection });
                    await cmd.ExecuteNonQueryAsync();
                }

                await using (var cmd = new NpgsqlCommand($"SELECT * FROM {table}", connection))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    await reader.ReadAsync();
                    Assert.That(reader.GetFieldValue<Geometry>(0), Is.EqualTo(point));
                    Assert.That(reader.GetFieldValue<Geometry>(1), Is.EqualTo(lineString));
                    Assert.That(reader.GetFieldValue<Geometry>(2), Is.EqualTo(polygon));
                    Assert.That(reader.GetFieldValue<Geometry>(3), Is.EqualTo(multiPoint));
                    Assert.That(reader.GetFieldValue<Geometry>(4), Is.EqualTo(multiLineString));
                    Assert.That(reader.GetFieldValue<Geometry>(5), Is.EqualTo(multiPolygon));
                    Assert.That(reader.GetFieldValue<Geometry>(6), Is.EqualTo(collection));
                }
            }
        })));
    }

    protected ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null, Ordinates handleOrdinates = Ordinates.None)
    {
        if (handleOrdinates == Ordinates.None)
            handleOrdinates = Ordinates.XY;

        var dataSource = NtsDataSources.GetOrAdd(handleOrdinates, o =>
        {
            var dataSourceBuilder = CreateDataSourceBuilder();
            dataSourceBuilder.UseNetTopologySuite(
                new DotSpatialAffineCoordinateSequenceFactory(handleOrdinates),
                handleOrdinates: o);
            return dataSourceBuilder.Build();
        });

        if (handleOrdinates == Ordinates.XY)
            _xyDataSource ??= dataSource;

        return dataSource.OpenConnectionAsync();
    }

    static string GetSqlLiteral(Geometry geometry)
        => string.Join("", geometry.ToBinary().Select(b => $"{b:X2}"));

    [OneTimeSetUp]
    public async Task SetUp()
    {
        var connection = await OpenConnectionAsync(handleOrdinates: Ordinates.XY);
        await EnsurePostgis(connection);
    }

    [OneTimeTearDown]
    public async Task Teardown()
        => await Task.WhenAll(NtsDataSources.Values.Select(async ds => await ds.DisposeAsync()));

    protected override NpgsqlDataSource DataSource => _xyDataSource ?? throw new InvalidOperationException();
    NpgsqlDataSource? _xyDataSource;

    ConcurrentDictionary<Ordinates, NpgsqlDataSource> NtsDataSources = new();
}
