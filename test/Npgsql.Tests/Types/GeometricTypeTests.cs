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
class GeometricTypeTests : MultiplexingTestBase
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
    public Task Box()
        => AssertType(new NpgsqlBox(3, 4, 1, 2), "(4,3),(2,1)", "box", NpgsqlDbType.Box);

    [Test]
    public Task Path_closed()
        => AssertType(
            new NpgsqlPath(new[] {new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4)}, false),
            "((1,2),(3,4))",
            "path",
            NpgsqlDbType.Path);

    [Test]
    public Task Path_open()
        => AssertType(
            new NpgsqlPath(new[] { new NpgsqlPoint(1, 2), new NpgsqlPoint(3, 4) }, true),
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

    public GeometricTypeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}