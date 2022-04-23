using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public class CubeTests : MultiplexingTestBase
{
    static readonly TestCaseData[] CubeValues =
    {
        new TestCaseData(new NpgsqlCube(new[] { 1.0, 2.0, 3.0 }, new[] { 4.0, 5.0, 6.0 }), "(1, 2, 3),(4, 5, 6)")
            .SetName("Cube_MultiDimensional"),
        new TestCaseData(new NpgsqlCube(new[] { 1.0, 2.0, 3.0 }), "(1, 2, 3)")
            .SetName("Cube_MultiDimensionalPoint"),
        new TestCaseData(new NpgsqlCube(1.0), "(1)")
            .SetName("Cube_SingleDimensionalPoint"),
        new TestCaseData(new NpgsqlCube(1.0, 2.0), "(1),(2)")
            .SetName("Cube_SingleDimensional")
    };

    [Test, TestCaseSource(nameof(CubeValues))]
    public Task Cube(NpgsqlCube cube, string sqlLiteral)
        => AssertType(cube, sqlLiteral, "cube", NpgsqlDbType.Cube);

    [Test]
    public void Cube_Constructor_SingleValue()
    {
        var cube = new NpgsqlCube(1.0);
        Assert.True(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(1));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0 }));
    }

    [Test]
    public void Cube_Constructor_SingleCoord_Point()
    {
        var cube = new NpgsqlCube(1.0, 1.0);
        Assert.True(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(1));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0 }));
    }

    [Test]
    public void Cube_Constructor_SingleCoord_NotPoint()
    {
        var cube = new NpgsqlCube(1.0, 2.0);
        Assert.False(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(1));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 2.0 }));
    }

    [Test]
    public void Cube_Constructor_LowerLeft_UpperRight_NotPoint()
    {
        var cube = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
        Assert.False(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(2));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 3.0, 4.0 }));
    }

    [Test]
    public void Cube_Constructor_LowerLeft_UpperRight_Point()
    {
        var cube = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 1.0, 2.0 });
        Assert.True(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(2));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0, 2.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_Single_Point()
    {
        var existingCube = new NpgsqlCube(new[] { 1.0, 2.0, 3.0 });
        var cube = new NpgsqlCube(existingCube, 4.0);
        Assert.True(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(4));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_Single_NotPoint()
    {
        var existingCube = new NpgsqlCube(new [] { 1.0, 2.0 }, new [] { 3.0, 4.0 });
        var cube = new NpgsqlCube(existingCube, 3.0);
        Assert.False(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(3));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 3.0, 4.0, 3.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_LowerLeft_UpperRight_Point()
    {
        var existingCube = new NpgsqlCube(new[] { 1.0, 2.0, 3.0 });
        var cube = new NpgsqlCube(existingCube, 4.0, 4.0);
        Assert.True(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(4));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_LowerLeft_UpperRight_NotPoint()
    {
        var existingCube = new NpgsqlCube(new [] { 1.0, 2.0 }, new [] { 3.0, 4.0 });
        var cube = new NpgsqlCube(existingCube, 4.0, 5.0);
        Assert.False(cube.Point);
        Assert.That(cube.Dimensions, Is.EqualTo(3));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 4.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 3.0, 4.0, 5.0 }));
    }

    [Test]
    public void Cube_LlCoord()
    {
        var cube = new NpgsqlCube(new [] { 1.0, 2.0 }, new [] { 3.0, 4.0 });
        Assert.That(cube.LlCoord(2), Is.EqualTo(2.0));
    }

    [Test]
    public void Cube_UrCoord()
    {
        var cube = new NpgsqlCube(new [] { 1.0, 2.0 }, new [] { 3.0, 4.0 });
        Assert.That(cube.UrCoord(2), Is.EqualTo(4.0));
    }

    [Test]
    public void Cube_Subset()
    {
        var cube = new NpgsqlCube(new [] { 1.0, 2.0, 3.0 }, new [] { 4.0, 5.0, 6.0 });
        Assert.That(cube.Subset(1, 3, 2, 2), Is.EqualTo(new NpgsqlCube(new [] { 1.0, 3.0, 2.0, 2.0 }, new [] { 4.0, 6.0, 5.0, 5.0 })));
    }

    [Test]
    public void Cube_ToString_NotPoint()
    {
        var cube = new NpgsqlCube(new[] { 1.0, 2.0, 3.0 }, new[] { 4.0, 5.0, 6.0 });
        Assert.That(cube.ToString(), Is.EqualTo("(1, 2, 3),(4, 5, 6)"));
    }

    [Test]
    public void Cube_ToString_Point()
    {
        var cube = new NpgsqlCube(new[] { 1.0, 2.0, 3.0 });
        Assert.That(cube.ToString(), Is.EqualTo("(1, 2, 3)"));
    }

    [OneTimeSetUp]
    public async Task SetUp()
    {
        await using var conn = await OpenConnectionAsync();
        TestUtil.MinimumPgVersion(conn, "13.0");
        await TestUtil.EnsureExtensionAsync(conn, "cube");
    }

    public CubeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) { }
}
