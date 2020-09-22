﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.TypeHandlers;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL arrays
    /// </summary>
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/arrays.html
    /// </remarks>
    public class ArrayTests : MultiplexingTestBase
    {
        [Test, Description("Resolves an array type handler via the different pathways")]
        public async Task ArrayTypeResolution()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ArrayTypeResolution),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = await OpenConnectionAsync(csb))
            {
                // Resolve type by NpgsqlDbType
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.AddWithValue("p", NpgsqlDbType.Array | NpgsqlDbType.Integer, DBNull.Value);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
                    }
                }

                // Resolve type by ClrType (type inference)
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = new int[0] });
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
                    }
                }

                // Resolve type by OID (read)
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT '{1, 3}'::INTEGER[]", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
                }
            }
        }

        [Test, Description("Roundtrips a simple, one-dimensional array of ints")]
        public async Task Ints()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
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
        }

        [Test, Description("Roundtrips a simple, one-dimensional array of int? values")]
        public async Task NullableInts()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn);

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
        public async Task NullableIntsCannotBeReadAsNonNullable()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT '{1, NULL, 2}'::integer[]", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(() => reader.GetFieldValue<int[]>(0), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(() => reader.GetFieldValue<List<int>>(0), Throws.Exception.TypeOf<InvalidOperationException>());
            Assert.That(() => reader.GetValue(0), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task EmptyArray()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p", conn);

            cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = new int[0] });
            var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetFieldValue<int[]>(0), Is.SameAs(Array.Empty<int>()));
        }

        [Test, Description("Roundtrips an empty multi-dimensional array.")]
        public async Task EmptyMultidimensionalArray()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p", conn);

            var expected = new int[0, 0];
            cmd.Parameters.AddWithValue("p", NpgsqlDbType.Array | NpgsqlDbType.Integer, expected);

            var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(expected));
        }

        [Test, Description("Verifies that an InvalidOperationException is thrown when the returned array has a different number of dimensions from what was requested.")]
        public async Task WrongArrayDimensions()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT ARRAY[[1], [2]]", conn);

            var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            var ex = Assert.Throws<InvalidOperationException>(() => reader.GetFieldValue<int[]>(0));
            Assert.That(ex.Message, Is.EqualTo("Cannot read an array with 1 dimension(s) from an array with 2 dimension(s)"));
        }

        [Test, Description("Verifies that an attempt to read an Array of value types that contains null values as array of a non-nullable type fails.")]
        public async Task ReadNullAsNonNullableArrayFails()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1", conn);

            var expected = new int?[] { 1, 5, null, 9 };
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, expected);

            var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(
                () => reader.GetFieldValue<int[]>(0),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo(ArrayHandler.ReadNonNullableCollectionWithNullsExceptionMessage));
        }


        [Test, Description("Verifies that an attempt to read an Array of value types that contains null values as List of a non-nullable type fails.")]
        public async Task ReadNullAsNonNullableListFails()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1", conn);

            var expected = new int?[] { 1, 5, null, 9 };
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, expected);

            var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(
                () => reader.GetFieldValue<List<int>>(0),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo(ArrayHandler.ReadNonNullableCollectionWithNullsExceptionMessage));
        }

        [Test, Description("Roundtrips a large, one-dimensional array of ints that will be chunked")]
        public async Task LongOneDimensional()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var expected = new int[conn.Settings.WriteBufferSize/4 + 100];
                for (var i = 0; i < expected.Length; i++)
                    expected[i] = i;
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    var p = new NpgsqlParameter {ParameterName = "p", Value = expected};
                    cmd.Parameters.Add(p);
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        reader.Read();
                        Assert.That(reader[0], Is.EqualTo(expected));
                    }
                }
            }
        }

        [Test, Description("Roundtrips a large, two-dimensional array of ints that will be chunked")]
        public async Task LongTwoDimensional()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var len = conn.Settings.WriteBufferSize/2 + 100;
                var expected = new int[2, len];
                for (var i = 0; i < len; i++)
                    expected[0, i] = i;
                for (var i = 0; i < len; i++)
                    expected[1, i] = i;
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    var p = new NpgsqlParameter {ParameterName = "p", Value = expected};
                    cmd.Parameters.Add(p);
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        reader.Read();
                        Assert.That(reader[0], Is.EqualTo(expected));
                    }
                }
            }
        }

        [Test, Description("Roundtrips a long, one-dimensional array of strings, including a null")]
        public async Task StringsWithNull()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var largeString = new StringBuilder();
                largeString.Append('a', conn.Settings.WriteBufferSize);
                var expected = new[] {"value1", null, largeString.ToString(), "val3"};
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Text) {Value = expected};
                    cmd.Parameters.Add(p);
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldValue<string[]>(0), Is.EqualTo(expected));
                    }
                }
            }
        }

        [Test, Description("Roundtrips a zero-dimensional array of ints, should return empty one-dimensional")]
        public async Task ZeroDimensional()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var expected = new int[0];
                var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = expected };
                cmd.Parameters.Add(p);
                var reader = await cmd.ExecuteReaderAsync();
                reader.Read();
                Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
                Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(expected));
                cmd.Dispose();
            }
        }

        [Test, Description("Roundtrips a two-dimensional array of ints")]
        public async Task TwoDimensionalInts()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
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
        }

        [Test, Description("Reads a one-dimensional array dates, both as DateTime and as the provider-specific NpgsqlDate")]
        public async Task ReadProviderSpecificType()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand(@"SELECT '{ ""2014-01-04"", ""2014-01-08"" }'::DATE[]", conn))
            {
                var expectedRegular = new[] { new DateTime(2014, 1, 4), new DateTime(2014, 1, 8) };
                var expectedPsv = new[] { new NpgsqlDate(2014, 1, 4), new NpgsqlDate(2014, 1, 8) };
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetValue(0), Is.EqualTo(expectedRegular));
                    Assert.That(reader.GetFieldValue<DateTime[]>(0), Is.EqualTo(expectedRegular));
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedPsv));
                    Assert.That(reader.GetFieldValue<NpgsqlDate[]>(0), Is.EqualTo(expectedPsv));
                }
            }
        }

        [Test, Description("Reads an one-dimensional array with lower bound != 0")]
        public async Task ReadNonZeroLowerBounded()
        {
            using (var conn = await OpenConnectionAsync())
            {
                using (var cmd = new NpgsqlCommand("SELECT '[2:3]={ 8, 9 }'::INT[]", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(new[] {8, 9}));
                }

                using (var cmd = new NpgsqlCommand("SELECT '[2:3][2:3]={ {8,9}, {1,2} }'::INT[][]", conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(new[,] {{8, 9}, {1, 2}}));
                }
            }
        }

        [Test, Description("Roundtrips a one-dimensional array of bytea values")]
        public async Task Byteas()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var expected = new[] { new byte[] { 1, 2 }, new byte[] { 3, 4, } };
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Bytea);
                var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                p1.Value = expected;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<byte[][]>(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
                    Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));
                }
            }
        }


        [Test, Description("Roundtrips a non-generic IList as an array")]
        // ReSharper disable once InconsistentNaming
        public async Task IListNonGeneric()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var expected = new ArrayList(new[] { 1, 2, 3 });
                var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = expected };
                cmd.Parameters.Add(p);
                Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(expected.ToArray()));
            }
        }

        [Test, Description("Roundtrips a generic List as an array")]
        // ReSharper disable once InconsistentNaming
        public async Task IListGeneric()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var expected = new[] { 1, 2, 3 }.ToList();
                var p1 = new NpgsqlParameter { ParameterName = "p1", Value = expected };
                var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<List<int>>(1), Is.EqualTo(expected));
                }
            }
        }

        [Test, Description("Tests for failure when reading a generic IList from a multidimensional array")]
        // ReSharper disable once InconsistentNaming
        public async Task IListGenericFailsForMultidimensionalArray()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1", conn))
            {
                var expected = new[,] { { 1, 2 }, { 3, 4 } };
                var p1 = new NpgsqlParameter { ParameterName = "p1", Value = expected };
                cmd.Parameters.Add(p1);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                    var exception = Assert.Throws<NotSupportedException>(() =>
                    {
                        reader.GetFieldValue<List<int>>(0);
                    });
                    Assert.That(exception.Message, Is.EqualTo("Can't read multidimensional array as List<Int32>"));
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/844")]
        public async Task IEnumerableThrowsFriendlyException()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1", conn))
            {
                cmd.Parameters.AddWithValue("p1", Enumerable.Range(1, 3));
                Assert.That(async () => await cmd.ExecuteScalarAsync(), Throws.Exception.TypeOf<NotSupportedException>().With.Message.Contains("use .ToList()/.ToArray() instead"));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/960")]
        public async Task MixedElementTypes()
        {
            var mixedList = new ArrayList { 1, "yo" };
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1", conn))
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, mixedList);
                Assert.That(async () => await cmd.ExecuteNonQueryAsync(), Throws.Exception
                    .TypeOf<Exception>()
                    .With.Message.Contains("mix"));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/960")]
        public async Task JaggedArraysNotSupported()
        {
            var jagged = new int[2][];
            jagged[0] = new[] { 8 };
            jagged[1] = new[] { 8, 10 };
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1", conn))
            {
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer, jagged);
                Assert.That(async () => await cmd.ExecuteNonQueryAsync(), Throws.Exception
                    .TypeOf<Exception>()
                    .With.Message.Contains("jagged"));
            }
        }

        [Test, Description("Checks that IList<T>s are properly serialized as arrays of their underlying types")]
        public async Task ListTypeResolution()
        {
            using (var conn = await OpenConnectionAsync(ConnectionString))
            {
                await AssertIListRoundtrips(conn, new[] { 1, 2, 3 });
                await AssertIListRoundtrips(conn, new IntList { 1, 2, 3 });
                await AssertIListRoundtrips(conn, new MisleadingIntList<string>() { 1, 2, 3 });
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1546")]
        public void GenericListGetNpgsqlDbType()
        {
            var p = new NpgsqlParameter
            {
                ParameterName = "p1",
                Value = new List<int> { 1, 2, 3 }
            };
            Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Integer));
        }

        [Test, Description("Roundtrips one-dimensional and two-dimensional arrays of a PostgreSQL domain.")]
        public async Task ArrayOfDomain()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            using (var conn = await OpenConnectionAsync())
            {
                TestUtil.MinimumPgVersion(conn, "11.0", "Arrays of domains were introduced in PostgreSQL 11");
                conn.ExecuteNonQuery("CREATE DOMAIN pg_temp.posint AS integer CHECK (VALUE > 0);");
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT @p1::posint[], @p2::posint[][]", conn))
                {
                    var oneDim = new[] { 1, 3, 5, 9 };
                    var twoDim = new[,] { { 1, 3 }, { 5, 9 } };
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Integer | NpgsqlDbType.Array, oneDim);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer | NpgsqlDbType.Array, twoDim);
                    using var reader = cmd.ExecuteReader();
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
            }
        }

        [Test, Description("Roundtrips a PostgreSQL domain over a one-dimensional and a two-dimensional array.")]
        public async Task DomainOfArray()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing, ReloadTypes");

            using (var conn = await OpenConnectionAsync())
            {
                TestUtil.MinimumPgVersion(conn, "11.0", "Domains over arrays were introduced in PostgreSQL 11");
                conn.ExecuteNonQuery("CREATE DOMAIN pg_temp.int_array_1d  AS int[] CHECK(array_length(VALUE, 1) = 4);" +
                                     "CREATE DOMAIN pg_temp.int_array_2d  AS int[][] CHECK(array_length(VALUE, 2) = 2);");
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT @p1::int_array_1d, @p2::int_array_2d", conn))
                {
                    var oneDim = new[] { 1, 3, 5, 9 };
                    var twoDim = new[,] { { 1, 3 }, { 5, 9 } };
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Integer | NpgsqlDbType.Array, oneDim);
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer | NpgsqlDbType.Array, twoDim);
                    using var reader = cmd.ExecuteReader();
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
            }
        }

        async Task AssertIListRoundtrips<TElement>(NpgsqlConnection conn, IEnumerable<TElement> value)
        {
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = value });

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("integer[]"));
                    Assert.That(reader[0], Is.EqualTo(value.ToArray()));
                }
            }
        }

        class IntList : List<int> { }
        class MisleadingIntList<T> : List<int> { }

        public ArrayTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
