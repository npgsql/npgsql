using System;
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
        => AssertType(cube, sqlLiteral, "cube", NpgsqlDbType.Cube, isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public void Cube_Constructor_SingleValue()
    {
        var cube = new NpgsqlCube(1.0);
        Assert.That(cube.Point, Is.True);
        Assert.That(cube.Dimensions, Is.EqualTo(1));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0 }));
    }

    [Test]
    public void Cube_Constructor_SingleCoord_Point()
    {
        var cube = new NpgsqlCube(1.0, 1.0);
        Assert.That(cube.Point, Is.True);
        Assert.That(cube.Dimensions, Is.EqualTo(1));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0 }));
    }

    [Test]
    public void Cube_Constructor_SingleCoord_NotPoint()
    {
        var cube = new NpgsqlCube(1.0, 2.0);
        Assert.That(cube.Point, Is.False);
        Assert.That(cube.Dimensions, Is.EqualTo(1));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 2.0 }));
    }

    [Test]
    public void Cube_Constructor_LowerLeft_UpperRight_NotPoint()
    {
        var cube = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
        Assert.That(cube.Point, Is.False);
        Assert.That(cube.Dimensions, Is.EqualTo(2));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 3.0, 4.0 }));
    }

    [Test]
    public void Cube_Constructor_LowerLeft_UpperRight_Point()
    {
        var cube = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 1.0, 2.0 });
        Assert.That(cube.Point, Is.True);
        Assert.That(cube.Dimensions, Is.EqualTo(2));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0, 2.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_Single_Point()
    {
        var existingCube = new NpgsqlCube(new[] { 1.0, 2.0, 3.0 });
        var cube = new NpgsqlCube(existingCube, 4.0);
        Assert.That(cube.Point, Is.True);
        Assert.That(cube.Dimensions, Is.EqualTo(4));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_Single_NotPoint()
    {
        var existingCube = new NpgsqlCube(new [] { 1.0, 2.0 }, new [] { 3.0, 4.0 });
        var cube = new NpgsqlCube(existingCube, 3.0);
        Assert.That(cube.Point, Is.False);
        Assert.That(cube.Dimensions, Is.EqualTo(3));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 3.0, 4.0, 3.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_LowerLeft_UpperRight_Point()
    {
        var existingCube = new NpgsqlCube(new[] { 1.0, 2.0, 3.0 });
        var cube = new NpgsqlCube(existingCube, 4.0, 4.0);
        Assert.That(cube.Point, Is.True);
        Assert.That(cube.Dimensions, Is.EqualTo(4));
        Assert.That(cube.LowerLeft, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
        Assert.That(cube.UpperRight, Is.EquivalentTo(new [] { 1.0, 2.0, 3.0, 4.0 }));
    }

    [Test]
    public void Cube_Constructor_AddDimension_LowerLeft_UpperRight_NotPoint()
    {
        var existingCube = new NpgsqlCube(new [] { 1.0, 2.0 }, new [] { 3.0, 4.0 });
        var cube = new NpgsqlCube(existingCube, 4.0, 5.0);
        Assert.That(cube.Point, Is.False);
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

    [Test]
    public async Task Cube_Array()
    {
        var data = new[]
        {
            new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 }),
            new NpgsqlCube(new[] { 5.0, 6.0 }),
            new NpgsqlCube(1.0, 2.0)
        };

        await AssertType(
            data,
            @"{""(1, 2),(3, 4)"",""(5, 6)"",""(1),(2)""}",
            "cube[]",
            NpgsqlDbType.Cube | NpgsqlDbType.Array,
            isNpgsqlDbTypeInferredFromClrType: false);
    }

    [Test]
    public void Cube_DimensionMismatch_ThrowsFormatException()
    {
        var ex = Assert.Throws<FormatException>(() => new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0 }));
        Assert.That(ex!.Message, Does.Contain("Different point dimensions"));
    }

    [Test]
    public void Cube_ExceedsMaxDimensions_ThrowsFormatException()
    {
        var lowerLeft = new double[101];
        var upperRight = new double[101];

        var ex = Assert.Throws<FormatException>(() => new NpgsqlCube(lowerLeft, upperRight));
        Assert.That(ex!.Message, Does.Contain("exceeds 100 dimensions"));
    }

    [Test]
    public Task Cube_NegativeValues()
        => AssertType(
            new NpgsqlCube(new[] { -1.0, -2.0, -3.0 }, new[] { -4.0, -5.0, -6.0 }),
            "(-1, -2, -3),(-4, -5, -6)",
            "cube",
            NpgsqlDbType.Cube,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public void Cube_Equality_HashCode()
    {
        var cube1 = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
        var cube2 = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 });
        var cube3 = new NpgsqlCube(new[] { 1.0, 2.0 }, new[] { 3.0, 5.0 });

        // Test equality
        Assert.That(cube1, Is.EqualTo(cube2));
        Assert.That(cube1 == cube2, Is.True);
        Assert.That(cube1 != cube3, Is.True);
        Assert.That(cube1.Equals(cube2), Is.True);
        Assert.That(cube1.Equals(cube3), Is.False);

        // Test hash code consistency
        Assert.That(cube1.GetHashCode(), Is.EqualTo(cube2.GetHashCode()));
        Assert.That(cube1.GetHashCode(), Is.Not.EqualTo(cube3.GetHashCode()));
    }

    [Test]
    public Task Cube_ZeroValues()
        => AssertType(
            new NpgsqlCube(0.0, 0.0),
            "(0)",
            "cube",
            NpgsqlDbType.Cube,
            isNpgsqlDbTypeInferredFromClrType: false);

    [Test]
    public Task Cube_MaxDimensions()
    {
        var lowerLeft = new double[100];
        var upperRight = new double[100];
        for (var i = 0; i < 100; i++)
        {
            lowerLeft[i] = i;
            upperRight[i] = i + 100;
        }

        var expectedLower = string.Join(", ", lowerLeft);
        var expectedUpper = string.Join(", ", upperRight);
        var expected = $"({expectedLower}),({expectedUpper})";

        return AssertType(
            new NpgsqlCube(lowerLeft, upperRight),
            expected,
            "cube",
            NpgsqlDbType.Cube,
            isNpgsqlDbTypeInferredFromClrType: false);
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
