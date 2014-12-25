using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NpgsqlTests
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

        [Test, Description("Reads a simple, one-dimensional array of ints")]
        public void ReadInts([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[] { 1, 5, 9 };
            var cmd = new NpgsqlCommand("SELECT '{ 1, 5, 9 }'::INTEGER[]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("Reads a simple, one-dimensional array of strings")]
        public void ReadStringsWithNull([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[] { "value1", null, "value2" };
            var cmd = new NpgsqlCommand(@"SELECT '{ ""value1"", NULL, ""value2"" }'::TEXT[]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<string[]>(0), Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("Reads a zero-dimensional array of ints, should return empty one-dimensional")]
        public void ReadZeroDimensional([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new int[0];
            var cmd = new NpgsqlCommand("SELECT '{}'::INTEGER[]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader();
            reader.Read();
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<int[]>(0), Is.EqualTo(expected));
            cmd.Dispose();
        }

        [Test, Description("Reads a two-dimensional array of ints")]
        public void ReadTwoDimensionalInts([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var expected = new[,] { { 1, 2, 3 }, { 7, 8, 9 } };
            var cmd = new NpgsqlCommand("SELECT '{ { 1, 2, 3 }, { 7, 8, 9 } }'::INTEGER[][]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
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

        [Test, Description("Check the field types are returned correctly for array types, and that retrieving them doesn't consume the column")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.Default,          TestName = "UnpreparedNonSequential")]
        [TestCase(PrepareOrNot.NotPrepared, CommandBehavior.SequentialAccess, TestName = "UnpreparedSequential"   )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.Default,          TestName = "PreparedNonSequential"  )]
        [TestCase(PrepareOrNot.Prepared,    CommandBehavior.SequentialAccess, TestName = "PreparedSequential"     )]
        public void FieldType(PrepareOrNot prepare, CommandBehavior behavior)
        {
            var cmd = new NpgsqlCommand(@"SELECT '{ ""2014-01-04"", ""2014-01-08"" }'::DATE[]", Conn);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            var reader = cmd.ExecuteReader(behavior);
            reader.Read();
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
            Assert.That(reader.GetProviderSpecificFieldType(0), Is.EqualTo(typeof(Array)));
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
        public void TestArrayOfBytea1()
        {
            var command = new NpgsqlCommand("select get_byte(:i[1], 2)", Conn);
            command.Parameters.AddWithValue(":i", new byte[][] { new byte[] { 0, 1, 2 }, new byte[] { 3, 4, 5 } });
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr[0], 2);
            }
        }

        [Test]
        public void TestArrayOfBytea2()
        {
            var command = new NpgsqlCommand("select get_byte(:i[1], 2)", Conn);
            command.Parameters.AddWithValue(":i", new byte[][] { new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 } });
            using (var dr = command.ExecuteReader())
            {
                dr.Read();
                Assert.AreEqual(dr[0], 3);
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
