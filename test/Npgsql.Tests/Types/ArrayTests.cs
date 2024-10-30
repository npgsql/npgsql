using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Internal.Converters;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

/// <summary>
/// Tests on PostgreSQL arrays
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/arrays.html
/// </remarks>
public class ArrayTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    static readonly TestCaseData[] ArrayTestCases =
    [
        new TestCaseData(new[] { 1, 2, 3 }, "{1,2,3}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array)
            .SetName("Integer_array"),
        new TestCaseData(Array.Empty<int>(), "{}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array)
            .SetName("Empty_array"),
        new TestCaseData(new[,] { { 1, 2, 3 }, { 7, 8, 9 } }, "{{1,2,3},{7,8,9}}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array)
            .SetName("Two_dimensional_array"),
        new TestCaseData(new[] { [1, 2], new byte[] { 3, 4 } }, """{"\\x0102","\\x0304"}""", "bytea[]", NpgsqlDbType.Bytea | NpgsqlDbType.Array)
            .SetName("Bytea_array")
    ];

    [Test, TestCaseSource(nameof(ArrayTestCases))]
    public Task Arrays<T>(T array, string sqlLiteral, string pgTypeName, NpgsqlDbType? npgsqlDbType)
        => AssertType(array, sqlLiteral, pgTypeName, npgsqlDbType);

    [Test]
    public async Task NullableInts()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ArrayNullabilityMode = ArrayNullabilityMode.Always
        };
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionStringBuilder.ToString());
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, new int?[] { 1, 2, null, 3 }, "{1,2,NULL,3}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array);
    }

    [Test, Description("Checks that PG arrays containing nulls can't be read as CLR arrays of non-nullable value types (the default).")]
    public async Task Nullable_ints_cannot_be_read_as_non_nullable()
        => await AssertTypeUnsupportedRead<InvalidOperationException>("{1,NULL,2}", "int[]");

    [Test]
    public async Task Throws_too_many_dimensions()
    {
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Parameters.AddWithValue("p", new int[1, 1, 1, 1, 1, 1, 1, 1, 1]); // 9 dimensions
        Assert.That(
            () => cmd.ExecuteScalarAsync(),
            Throws.Exception.TypeOf<ArgumentException>().With.Message.EqualTo("Postgres arrays can have at most 8 dimensions. (Parameter 'values')"));
    }

    [Test, Description("Checks that PG arrays containing nulls are returned as set via ValueTypeArrayMode.")]
    [TestCase(ArrayNullabilityMode.Always)]
    [TestCase(ArrayNullabilityMode.Never)]
    [TestCase(ArrayNullabilityMode.PerInstance)]
    public async Task Value_type_array_nullabilities(ArrayNullabilityMode mode)
    {
        await using var dataSource = CreateDataSource(csb => csb.ArrayNullabilityMode = mode);
        await using var conn = await dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(
"""
SELECT onedim, twodim FROM (VALUES
('{1, 2, 3, 4}'::int[],'{{1, 2},{3, 4}}'::int[][]),
('{5, NULL, 6, 7}'::int[],'{{5, NULL},{6, 7}}'::int[][])) AS x(onedim,twodim)
""", conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        switch (mode)
        {
        case ArrayNullabilityMode.Never:
            reader.Read();
            var value = reader.GetValue(0);
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int[])));
            Assert.That(value, Is.EqualTo(new []{1, 2, 3, 4}));
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetValue(1).GetType(), Is.EqualTo(typeof(int[,])));
            Assert.That(reader.GetValue(1), Is.EqualTo(new [,]{{1, 2}, {3, 4}}));
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidCastException>());
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(() => reader.GetValue(1), Throws.Exception.TypeOf<InvalidCastException>());
            break;
        case ArrayNullabilityMode.Always:
            reader.Read();
            value = reader.GetValue(0);
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int?[])));
            Assert.That(value, Is.EqualTo(new int?[]{1, 2, 3, 4}));
            value = reader.GetValue(1);
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int?[,])));
            Assert.That(value, Is.EqualTo(new int?[,]{{1, 2}, {3, 4}}));
            reader.Read();
            value = reader.GetValue(0);
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int?[])));
            Assert.That(value, Is.EqualTo(new int?[]{5, null, 6, 7}));
            value = reader.GetValue(1);
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int?[,])));
            Assert.That(value, Is.EqualTo(new int?[,]{{5, null},{6, 7}}));
            break;
        case ArrayNullabilityMode.PerInstance:
            reader.Read();
            value = reader.GetValue(0);
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int[])));
            Assert.That(value, Is.EqualTo(new []{1, 2, 3, 4}));
            value = reader.GetValue(1);
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int[,])));
            Assert.That(value, Is.EqualTo(new [,]{{1, 2}, {3, 4}}));
            reader.Read();
            value = reader.GetValue(0);
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int?[])));
            Assert.That(value, Is.EqualTo(new int?[]{5, null, 6, 7}));
            value = reader.GetValue(1);
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(value.GetType(), Is.EqualTo(typeof(int?[,])));
            Assert.That(value, Is.EqualTo(new int?[,]{{5, null},{6, 7}}));
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    // Note that PG normalizes empty multidimensional arrays to single-dimensional, e.g. ARRAY[[], []]::integer[] returns {}.
    [Test]
    public async Task Write_empty_multidimensional_array()
        => await AssertTypeWrite(new int[0, 0], "{}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array);

    [Test]
    public async Task Generic_List()
        => await AssertType(
            new List<int> { 1, 2, 3 }, "{1,2,3}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array, isDefaultForReading: false);

    [Test]
    public async Task Write_IList_implementation()
        => await AssertTypeWrite(
            ImmutableArray.Create(1, 2, 3), "{1,2,3}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array);

    [Test]
    public void Read_IList_implementation_throws()
        => Assert.ThrowsAsync<InvalidCastException>(() =>
            AssertTypeRead("{1,2,3}", "integer[]", ImmutableArray.Create(1, 2, 3), isDefault: false));

    [Test]
    public async Task Generic_IList()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);

        var expected = ImmutableArray.Create(1,2,3);
        cmd.Parameters.Add(new NpgsqlParameter<IList<int>>("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer) { TypedValue = expected });

        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.AreEqual(expected, reader.GetFieldValue<int[]>(0));
    }

    [Test, Description("Verifies that an InvalidOperationException is thrown when the returned array has a different number of dimensions from what was requested.")]
    public async Task Wrong_array_dimensions_throws()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT ARRAY[[1], [2]]", conn);

        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        var ex = Assert.Throws<InvalidCastException>(() => reader.GetFieldValue<int[]>(0))!;
        Assert.That(ex.Message, Does.StartWith("Cannot read an array value with 2 dimensions into a collection type with 1 dimension"));
    }

    [Test, Description("Verifies that an attempt to read an Array of value types that contains null values as array of a non-nullable type fails.")]
    public async Task Read_null_as_non_nullable_array_throws()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);

        var expected = new int?[] { 1, 5, null, 9 };
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, expected);

        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        Assert.That(
            () => reader.GetFieldValue<int[]>(0),
            Throws.Exception.TypeOf<InvalidCastException>()
                .With.Message.EqualTo(PgArrayConverter.ReadNonNullableCollectionWithNullsExceptionMessage));
    }


    [Test, Description("Verifies that an attempt to read an Array of value types that contains null values as List of a non-nullable type fails.")]
    public async Task Read_null_as_non_nullable_list_throws()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);

        var expected = new int?[] { 1, 5, null, 9 };
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, expected);

        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        Assert.That(
            () => reader.GetFieldValue<List<int>>(0),
            Throws.Exception.TypeOf<InvalidCastException>()
                .With.Message.EqualTo(PgArrayConverter.ReadNonNullableCollectionWithNullsExceptionMessage));
    }

    [Test, Description("Roundtrips a large, one-dimensional array of ints that will be chunked")]
    public async Task Long_one_dimensional()
    {
        await using var conn = await OpenConnectionAsync();

        var expected = new int[conn.Settings.WriteBufferSize/4 + 100];
        for (var i = 0; i < expected.Length; i++)
            expected[i] = i;

        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var p = new NpgsqlParameter {ParameterName = "p", Value = expected};
        cmd.Parameters.Add(p);

        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        reader.Read();
        Assert.That(reader[0], Is.EqualTo(expected));
    }

    [Test, Description("Roundtrips a large, two-dimensional array of ints that will be chunked")]
    public async Task Long_two_dimensional()
    {
        await using var conn = await OpenConnectionAsync();
        var len = conn.Settings.WriteBufferSize/2 + 100;
        var expected = new int[2, len];
        for (var i = 0; i < len; i++)
            expected[0, i] = i;
        for (var i = 0; i < len; i++)
            expected[1, i] = i;
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var p = new NpgsqlParameter {ParameterName = "p", Value = expected};
        cmd.Parameters.Add(p);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        reader.Read();
        Assert.That(reader[0], Is.EqualTo(expected));
    }

    [Test, Description("Reads an one-dimensional array with lower bound != 0")]
    public Task Read_non_zero_lower_bounded()
        => AssertTypeRead("[2:3]={ 8, 9 }", "integer[]", new[] { 8, 9 });

    [Test, Description("Reads an one-dimensional array with lower bound != 0")]
    public Task Read_non_zero_lower_bounded_multidimensional()
        => AssertTypeRead("[2:3][2:3]={ {8,9}, {1,2} }", "integer[]", new[,] { { 8, 9 }, { 1, 2 }});

    [Test, Description("Roundtrips a long, one-dimensional array of strings, including a null")]
    public async Task Strings_with_null()
    {
        await using var conn = await OpenConnectionAsync();
        var largeString = new StringBuilder();
        largeString.Append('a', conn.Settings.WriteBufferSize);
        var expected = new[] {"value1", null, largeString.ToString(), "val3"};
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Text) {Value = expected};
        cmd.Parameters.Add(p);
        await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        reader.Read();
        Assert.That(reader.GetFieldValue<string[]>(0), Is.EqualTo(expected));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/844")]
    public async Task Writing_IEnumerable_is_not_supported()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        cmd.Parameters.AddWithValue("p1", new EnumerableOnly<int>());
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<InvalidCastException>().With.Property("InnerException").Message.Contains("array or some implementation of IList<T>"));
    }

    class EnumerableOnly<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/960")]
    public async Task Jagged_arrays_not_supported()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, new[] { [8], new[] { 8, 10 } });
        Assert.That(async () => await cmd.ExecuteNonQueryAsync(), Throws.Exception
            .TypeOf<InvalidCastException>()
            .With.Property("InnerException").Message.Contains("jagged"));
    }

    [Test, Description("Roundtrips one-dimensional and two-dimensional arrays of a PostgreSQL domain.")]
    public async Task Array_of_domain()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing, ReloadTypes");

        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "11.0", "Arrays of domains were introduced in PostgreSQL 11");
        await conn.ExecuteNonQueryAsync("CREATE DOMAIN pg_temp.posint AS integer CHECK (VALUE > 0);");
        await conn.ReloadTypesAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1::posint[], @p2::posint[][]", conn);
        var oneDim = new[] { 1, 3, 5, 9 };
        var twoDim = new[,] { { 1, 3 }, { 5, 9 } };
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Integer | NpgsqlDbType.Array, oneDim);
        cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer | NpgsqlDbType.Array, twoDim);
        await using var reader = cmd.ExecuteReader();
        reader.Read();

        Assert.That(reader.GetValue(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
        Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));

        Assert.That(reader.GetValue(1), Is.EqualTo(twoDim));
        Assert.That(reader.GetProviderSpecificValue(1), Is.EqualTo(twoDim));
        Assert.That(reader.GetFieldValue<int[,]>(1), Is.EqualTo(twoDim));
        Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
        Assert.That(reader.GetProviderSpecificFieldType(1), Is.EqualTo(typeof(Array)));
    }

    [Test, Description("Roundtrips a PostgreSQL domain over a one-dimensional and a two-dimensional array.")]
    public async Task Domain_of_array()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing, ReloadTypes");

        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "11.0", "Domains over arrays were introduced in PostgreSQL 11");
        await conn.ExecuteNonQueryAsync(
"""
CREATE DOMAIN pg_temp.int_array_1d  AS int[] CHECK(array_length(VALUE, 1) = 4);
CREATE DOMAIN pg_temp.int_array_2d  AS int[][] CHECK(array_length(VALUE, 2) = 2);
""");
        await conn.ReloadTypesAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1::int_array_1d, @p2::int_array_2d", conn);
        var oneDim = new[] { 1, 3, 5, 9 };
        var twoDim = new[,] { { 1, 3 }, { 5, 9 } };
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Integer | NpgsqlDbType.Array, oneDim);
        cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer | NpgsqlDbType.Array, twoDim);
        await using var reader = cmd.ExecuteReader();
        reader.Read();

        Assert.That(reader.GetValue(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(oneDim));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
        Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));

        Assert.That(reader.GetValue(1), Is.EqualTo(twoDim));
        Assert.That(reader.GetProviderSpecificValue(1), Is.EqualTo(twoDim));
        Assert.That(reader.GetFieldValue<int[,]>(1), Is.EqualTo(twoDim));
        Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
        Assert.That(reader.GetProviderSpecificFieldType(1), Is.EqualTo(typeof(Array)));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3417")]
    public async Task Read_two_empty_arrays()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT '{}'::INT[], '{}'::INT[]", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        Assert.AreSame(reader.GetFieldValue<int[]>(0), reader.GetFieldValue<int[]>(1));
        // Unlike T[], List<T> is mutable so we should not return the same instance
        Assert.AreNotSame(reader.GetFieldValue<List<int>>(0), reader.GetFieldValue<List<int>>(1));
    }

    [Test]
    public async Task Arrays_not_supported_by_default_on_NpgsqlSlimSourceBuilder()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        await using var dataSource = dataSourceBuilder.Build();

        await AssertTypeUnsupportedRead<int[], InvalidCastException>("{1,2,3}", "integer[]", dataSource);
        await AssertTypeUnsupportedWrite<int[], InvalidCastException>([1, 2, 3], "integer[]", dataSource);
    }

    [Test]
    public async Task NpgsqlSlimSourceBuilder_EnableArrays()
    {
        var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(ConnectionString);
        dataSourceBuilder.EnableArrays();
        await using var dataSource = dataSourceBuilder.Build();

        await AssertType(dataSource, new[] { 1, 2, 3 }, "{1,2,3}", "integer[]", NpgsqlDbType.Integer | NpgsqlDbType.Array);
    }
}
