using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on PostgreSQL arrays
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/arrays.html
    /// </remarks>
    class ArrayTests : TestBase
    {
        public ArrayTests(string backendVersion) : base(backendVersion) {}

        [Test, Description("Roundtrips a simple, one-dimensional array of ints")]
        public void Ints([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[] { 1, 5, 9 };
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer);
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            p1.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            // Via NpgsqlDbType
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));

            // Via inference
            Assert.That(reader.GetFieldType(1), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetFieldValue<int[]>(1), Is.EqualTo(expected));

            cmd.Dispose();
        }

        [Test, Description("Roundtrips a simple, one-dimensional array of strings, including a null")]
        public void StringsWithNull([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[] { "value1", null, "value2" };
            var cmd = new NpgsqlCommand("SELECT @p", Conn);
            var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Text) { Value = expected };
            cmd.Parameters.Add(p);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<string[]>(0), Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("Roundtrips a zero-dimensional array of ints, should return empty one-dimensional")]
        public void ZeroDimensional([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new int[0];
            var cmd = new NpgsqlCommand("SELECT @p", Conn);
            var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = expected };
            cmd.Parameters.Add(p);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("Roundtrips a two-dimensional array of ints")]
        public void TwoDimensionalInts([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[,] { { 1, 2, 3 }, { 7, 8, 9 } };
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Integer);
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            p1.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("Reads a one-dimensional array dates, both as DateTime and as the provider-specific NpgsqlDate")]
        public void ReadProviderSpecificType([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expectedRegular = new[] { new DateTime(2014, 1, 4),   new DateTime(2014, 1, 8) };
            var expectedPsv     = new[] { new NpgsqlDate(2014, 1, 4), new NpgsqlDate(2014, 1, 8) };
            var cmd = new NpgsqlCommand(@"SELECT '{ ""2014-01-04"", ""2014-01-08"" }'::DATE[]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expectedRegular));
            Assert.That(reader.GetFieldValue<DateTime[]>(0), Is.EqualTo(expectedRegular));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expectedPsv));
            Assert.That(reader.GetFieldValue<NpgsqlDate[]>(0), Is.EqualTo(expectedPsv));
            cmd.Dispose();
        }

        [Test, Description("Reads an one-dimensional array with lower bound != 0")]
        public void ReadNonZeroLowerBounded([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var cmd = new NpgsqlCommand("SELECT '[2:3]={ 8, 9 }'::INT[]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(new[] { 8, 9 }));
            reader.Close();
            cmd.Dispose();

            cmd = new NpgsqlCommand("SELECT '[2:3][2:3]={ {8,9}, {1,2} }'::INT[][]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetFieldValue<int[,]>(0), Is.EqualTo(new[,] { {8,9}, {1,2} }));
            reader.Close();
            cmd.Dispose();
        }

        [Test, Description("Roundtrips a one-dimensional array of bytea values")]
        public void Byteas([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[] { new byte[] { 1, 2 }, new byte[] { 3, 4, }};
            var cmd = new NpgsqlCommand("SELECT @p1, @p2", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Array | NpgsqlDbType.Bytea);
            var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            p1.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<byte[][]>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));
            cmd.Dispose();
        }

        // Older tests

        [Test]
        public void TestMultiDimensionalArray()
        {
            var command = new NpgsqlCommand("select :i", Conn);
            command.Parameters.AddWithValue(":i", (new decimal[,] { { 0, 1, 2 }, { 3, 4, 5 } }));
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(2, (dr[0] as Array).Rank);
                var da = (decimal[,])dr[0];
                Assert.AreEqual(da.GetUpperBound(0), 1);
                Assert.AreEqual(da.GetUpperBound(1), 2);
                decimal cmp = 0m;
                foreach (decimal el in da)
                    Assert.AreEqual(el, cmp++);
            }
        }

        [Test]
        public void TestBug1010488ArrayParameterWithNullValue()
        {
            // Test by Christ Akkermans
            new NpgsqlCommand(@"CREATE OR REPLACE FUNCTION NullTest (input INT4[]) RETURNS VOID
            AS $$
            DECLARE
            BEGIN
            END
            $$ LANGUAGE plpgsql;", Conn).ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand("NullTest", Conn))
            {
                var parameter = new NpgsqlParameter("", NpgsqlDbType.Integer | NpgsqlDbType.Array);
                parameter.Value = new object[] { 5, 5, DBNull.Value };
                cmd.Parameters.Add(parameter);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void TestBug1010675ArrayParameterWithNullValue()
        {
            new NpgsqlCommand(@"CREATE OR REPLACE FUNCTION NullTest (input INT4[]) RETURNS VOID
            AS $$
            DECLARE
            BEGIN
            END
            $$ LANGUAGE plpgsql;", Conn).ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand("NullTest", Conn))
            {
                NpgsqlParameter parameter = new NpgsqlParameter("", NpgsqlDbType.Integer | NpgsqlDbType.Array);
                parameter.Value = new object[] { 5, 5, null };
                cmd.Parameters.Add(parameter);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void TestEmptyIEnumerableAsArray()
        {
            using (var command = new NpgsqlCommand("SELECT :array", Conn))
            {
                var expected = new[] { 1, 2, 3, 4 };
                command.Parameters.AddWithValue("array", expected.Where(x => false));
                var res = command.ExecuteScalar() as int[];

                Assert.NotNull(res);
                Assert.AreEqual(0, res.Length);
            }
        }

        [Test]
        public void TestIEnumerableAsArray()
        {
            using (var command = new NpgsqlCommand("SELECT :array", Conn))
            {
                var expected = new[] { 1, 2, 3, 4 };
                command.Parameters.AddWithValue("array", expected.Select(x => x));
                var res = command.ExecuteScalar() as int[];

                Assert.NotNull(res);
                CollectionAssert.AreEqual(expected, res);
            }
        }
    }
}
