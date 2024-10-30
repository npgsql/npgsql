using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

/// <summary>
/// Tests on PostgreSQL geometric types
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-geometric.html
/// </remarks>
class GeometricTypeTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public Task Point()
        => AssertType(new NpgsqlPoint(1.2, 3.4), "(1.2,3.4)", "point", NpgsqlDbType.Point);

    [Test]
    public Task Line()
        => AssertType(new NpgsqlLine(1, 2, 3), "{1,2,3}", "line", NpgsqlDbType.Line);

    [Test]
    public Task LineSegment()
        => AssertType(new NpgsqlLSeg(1, 2, 3, 4), "[(1,2),(3,4)]", "lseg", NpgsqlDbType.LSeg);

    [Test]
    public async Task Box()
    {
        await AssertType(
            new NpgsqlBox(top: 3, right: 4, bottom: 1, left: 2),
            "(4,3),(2,1)",
            "box",
            NpgsqlDbType.Box,
            skipArrayCheck: true); // Uses semicolon instead of comma as separator

        await AssertType(
            new NpgsqlBox(top: -10, right: 0, bottom: -20, left: -10),
            "(0,-10),(-10,-20)",
            "box",
            NpgsqlDbType.Box,
            skipArrayCheck: true); // Uses semicolon instead of comma as separator

        await AssertType(
            new NpgsqlBox(top: 1, right: 2, bottom: 3, left: 4),
            "(4,3),(2,1)",
            "box",
            NpgsqlDbType.Box,
            skipArrayCheck: true); // Uses semicolon instead of comma as separator

        var swapped = new NpgsqlBox(top: -20, right: -10, bottom: -10, left: 0);

        await AssertType(
            swapped,
            "(0,-10),(-10,-20)",
            "box",
            NpgsqlDbType.Box,
            skipArrayCheck: true); // Uses semicolon instead of comma as separator

        await AssertType(
            swapped with { UpperRight = new NpgsqlPoint(-20,-10) },
            "(-10,-10),(-20,-20)",
            "box",
            NpgsqlDbType.Box,
            skipArrayCheck: true); // Uses semicolon instead of comma as separator

        await AssertType(
            swapped with { LowerLeft = new NpgsqlPoint(10, 10) },
            "(10,10),(0,-10)",
            "box",
            NpgsqlDbType.Box,
            skipArrayCheck: true); // Uses semicolon instead of comma as separator
    }

    [Test]
    public async Task Box_array()
    {
        var data = new[]
        {
            new NpgsqlBox(top: 3, right: 4, bottom: 1, left: 2),
            new NpgsqlBox(top: 5, right: 6, bottom: 3, left: 4),
            new NpgsqlBox(top: -10, right: 0, bottom: -20, left: -10)
        };

        await AssertType(
            data,
            "{(4,3),(2,1);(6,5),(4,3);(0,-10),(-10,-20)}",
            "box[]",
            NpgsqlDbType.Box | NpgsqlDbType.Array
            );

        var swappedData = new[]
        {
            new NpgsqlBox(top: 1, right: 2, bottom: 3, left: 4),
            new NpgsqlBox(top: 3, right: 4, bottom: 5, left: 6),
            new NpgsqlBox(top: -20, right: -10, bottom: -10, left: 0)
        };

        await AssertType(
            swappedData,
            "{(4,3),(2,1);(6,5),(4,3);(0,-10),(-10,-20)}",
            "box[]",
            NpgsqlDbType.Box | NpgsqlDbType.Array
            );
    }

    [Test]
    public Task Path_closed()
        => AssertType(
            new NpgsqlPath([new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4)], false),
            "((1,2),(3,4))",
            "path",
            NpgsqlDbType.Path);

    [Test]
    public Task Path_open()
        => AssertType(
            new NpgsqlPath([new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4)], true),
            "[(1,2),(3,4)]",
            "path",
            NpgsqlDbType.Path);

    [Test]
    public Task Polygon()
        => AssertType(
            new NpgsqlPolygon(new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4)),
            "((1,2),(3,4))",
            "polygon",
            NpgsqlDbType.Polygon);

    [Test]
    public Task Circle()
        => AssertType(
            new NpgsqlCircle(1, 2, 0.5),
            "<(1,2),0.5>",
            "circle",
            NpgsqlDbType.Circle);
}
