﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Internal.TypeHandlers;
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
public class ArrayTests : MultiplexingTestBase
{
    [Test, Description("Resolves an array type handler via the different pathways")]
    public async Task Array_resolution()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing, ReloadTypes");

        await using var dataSource = CreateDataSource(csb => csb.Pooling = false);
        await using var conn = await dataSource.OpenConnectionAsync();

        // Resolve type by NpgsqlDbType
        await using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.AddWithValue("p", NpgsqlDbType.Array | NpgsqlDbType.Integer, DBNull.Value);
            await using var reader = await cmd.ExecuteReaderAsync();

            reader.Read();
            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
        }

        // Resolve type by ClrType (type inference)
        await conn.ReloadTypesAsync();
        await using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = Array.Empty<int>() });
            await using var reader = await cmd.ExecuteReaderAsync();

            reader.Read();
            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
        }

        // Resolve type by DataTypeName
        await conn.ReloadTypesAsync();
        await using (var cmd = new NpgsqlCommand("SELECT @p", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter { ParameterName="p", DataTypeName = "integer[]", Value = DBNull.Value });
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
            }
        }

        // Resolve type by OID (read)
        await conn.ReloadTypesAsync();
        await using (var cmd = new NpgsqlCommand("SELECT '{1, 3}'::INTEGER[]", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(new[] { 1, 3 }));
        }
    }

    [Test]
    public async Task Bind_int_then_array_of_int()
    {
        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        await using var cmd = new NpgsqlCommand("SELECT 1", conn);
        _ = await cmd.ExecuteScalarAsync();

        cmd.CommandText = "SELECT ARRAY[1,2]";
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(new[] { 1, 2 }));
    }

    [Test, Description("Roundtrips a simple, one-dimensional array of ints")]
    public async Task Ints()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);

        var expected = new[] { 1, 5, 9 };
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer);
        var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
        var p3 = new NpgsqlParameter<int[]>("p3", expected);
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        cmd.Parameters.Add(p3);
        p1.Value = expected;
        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        for (var i = 0; i < cmd.Parameters.Count; i++)
        {
            Assert.That(reader.GetValue(i), Is.EqualTo(expected));
            Assert.That(reader.GetValue(i), Is.TypeOf<int[]>());
            Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<int[]>(i), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(Array)));
        }
    }

    [Test, Description("Roundtrips a simple, one-dimensional array of int? values")]
    public async Task Nullable_ints()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);

        var expected = new int?[] { 1, 5, null, 9 };
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer);
        var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
        var p3 = new NpgsqlParameter<int?[]>("p3", expected);
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        cmd.Parameters.Add(p3);
        p1.Value = expected;
        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        for (var i = 0; i < cmd.Parameters.Count; i++)
        {
            Assert.That(reader.GetFieldValue<int?[]>(i), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<List<int?>>(i), Is.EqualTo(expected.ToList()));
            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetProviderSpecificFieldType(i), Is.EqualTo(typeof(Array)));
        }
    }

    [Test, Description("Checks that PG arrays containing nulls can't be read as CLR arrays of non-nullable value types.")]
    public async Task Nullable_ints_cannot_be_read_as_non_nullable()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT '{1, NULL, 2}'::integer[]", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        Assert.That(() => reader.GetFieldValue<int[]>(0), Throws.Exception.TypeOf<InvalidOperationException>());
        Assert.That(() => reader.GetFieldValue<List<int>>(0), Throws.Exception.TypeOf<InvalidOperationException>());
        Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidOperationException>());
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
            Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(() => reader.GetValue(1), Throws.Exception.TypeOf<InvalidOperationException>());
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

    [Test]
    public async Task Empty_array()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);

        cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = Array.Empty<int>() });
        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        Assert.That(reader.GetFieldValue<int[]>(0), Is.SameAs(Array.Empty<int>()));
        Assert.That(reader.GetFieldValue<int?[]>(0), Is.SameAs(Array.Empty<int?>()));
    }

    [Test, Description("Roundtrips an empty multi-dimensional array.")]
    public async Task Empty_multidimensional_array()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);

        var expected = new int[0, 0];
        cmd.Parameters.AddWithValue("p", NpgsqlDbType.Array | NpgsqlDbType.Integer, expected);

        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(expected));
    }

    [Test, Description("Verifies that an InvalidOperationException is thrown when the returned array has a different number of dimensions from what was requested.")]
    public async Task Wrong_array_dimensions_throws()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT ARRAY[[1], [2]]", conn);

        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();

        var ex = Assert.Throws<InvalidOperationException>(() => reader.GetFieldValue<int[]>(0))!;
        Assert.That(ex.Message, Is.EqualTo("Cannot read an array with 1 dimension(s) from an array with 2 dimension(s)"));
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
            Throws.Exception.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo(ArrayHandlerOps.ReadNonNullableCollectionWithNullsExceptionMessage));
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
            Throws.Exception.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo(ArrayHandlerOps.ReadNonNullableCollectionWithNullsExceptionMessage));
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

    [Test, Description("Roundtrips a zero-dimensional array of ints, should return empty one-dimensional")]
    public async Task Zero_dimensional()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var expected = Array.Empty<int>();
        var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = expected };
        cmd.Parameters.Add(p);
        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(expected));
        cmd.Dispose();
    }

    [Test, Description("Roundtrips a two-dimensional array of ints")]
    public async Task Two_dimensional_ints()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
        var expected = new[,] { { 1, 2, 3 }, { 7, 8, 9 } };
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer);
        var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        p1.Value = expected;
        var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(expected));
    }

    [Test, Description("Reads an one-dimensional array with lower bound != 0")]
    public async Task Read_non_zero_lower_bounded()
    {
        await using var conn = await OpenConnectionAsync();
        await using (var cmd = new NpgsqlCommand("SELECT '[2:3]={ 8, 9 }'::INT[]", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(new[] {8, 9}));
        }

        await using (var cmd = new NpgsqlCommand("SELECT '[2:3][2:3]={ {8,9}, {1,2} }'::INT[][]", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            reader.Read();
            Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(new[,] {{8, 9}, {1, 2}}));
        }
    }

    [Test, Description("Roundtrips a one-dimensional array of bytea values")]
    public async Task Array_of_byte_arrays()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
        var expected = new[] { new byte[] { 1, 2 }, new byte[] { 3, 4, } };
        var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Bytea);
        var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        p1.Value = expected;
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldValue<byte[][]>(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
        Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));
    }


    [Test, Description("Roundtrips a non-generic IList as an array")]
    // ReSharper disable once InconsistentNaming
    public async Task IList_non_generic()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var expected = new ArrayList(new[] { 1, 2, 3 });
        var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = expected };
        cmd.Parameters.Add(p);
        Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected.ToArray()));
    }

    [Test, Description("Roundtrips a generic List as an array")]
    // ReSharper disable once InconsistentNaming
    public async Task IList_generic()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn);
        var expected = new[] { 1, 2, 3 }.ToList();
        var p1 = new NpgsqlParameter { ParameterName = "p1", Value = expected };
        var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldValue<List<int>>(1), Is.EqualTo(expected));
    }

    [Test, Description("Tests for failure when reading a generic IList from a multidimensional array")]
    // ReSharper disable once InconsistentNaming
    public async Task IList_generic_fails_for_multidimensional_array()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        var expected = new[,] { { 1, 2 }, { 3, 4 } };
        var p1 = new NpgsqlParameter { ParameterName = "p1", Value = expected };
        cmd.Parameters.Add(p1);
        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        var exception = Assert.Throws<NotSupportedException>(() =>
        {
            reader.GetFieldValue<List<int>>(0);
        })!;
        Assert.That(exception.Message, Is.EqualTo("Can't read multidimensional array as List<Int32>"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/844")]
    public async Task IEnumerable_throws_friendly_exception()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        cmd.Parameters.AddWithValue("p1", Enumerable.Range(1, 3));
        Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<NotSupportedException>().With.Message.Contains("array or List"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/960")]
    public async Task Mixed_element_types()
    {
        var mixedList = new ArrayList { 1, "yo" };
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, mixedList);
        Assert.That(async () => await cmd.ExecuteNonQueryAsync(), Throws.Exception
            .TypeOf<Exception>()
            .With.Message.Contains("mix"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/960")]
    public async Task Jagged_arrays_not_supported()
    {
        var jagged = new int[2][];
        jagged[0] = new[] { 8 };
        jagged[1] = new[] { 8, 10 };
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p1", conn);
        cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, jagged);
        Assert.That(async () => await cmd.ExecuteNonQueryAsync(), Throws.Exception
            .TypeOf<Exception>()
            .With.Message.Contains("jagged"));
    }

    [Test, Description("Checks that IList<T>s are properly serialized as arrays of their underlying types")]
    public async Task List_type_resolution()
    {
        await using var conn = await OpenConnectionAsync();
        await AssertIListRoundtrips(conn, new[] { 1, 2, 3 });
        await AssertIListRoundtrips(conn, new IntList { 1, 2, 3 });
        await AssertIListRoundtrips(conn, new MisleadingIntList<string>() { 1, 2, 3 });
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1546")]
    public void Generic_List_get_NpgsqlDbType()
    {
        var p = new NpgsqlParameter
        {
            ParameterName = "p1",
            Value = new List<int> { 1, 2, 3 }
        };
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Integer));
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

    async Task AssertIListRoundtrips<TElement>(NpgsqlConnection conn, IEnumerable<TElement> value)
    {
        await using var cmd = new NpgsqlCommand("SELECT @p", conn);
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = value });

        await using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
        Assert.That(reader[0], Is.EqualTo(value.ToArray()));
    }

    class IntList : List<int> { }
    // ReSharper disable UnusedTypeParameter
    class MisleadingIntList<T> : List<int> { }
    // ReSharper restore UnusedTypeParameter

    public ArrayTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
