using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on the PostgreSQL BitString type
    /// </summary>
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/datatype-bit.html
    /// </remarks>
    public class BitStringTests : MultiplexingTestBase
    {
        [Test]
        public async Task Roundtrip_BitArray(
            [Values(
                "1011011000101111010110101101011011",  // 34 bits
                "10110110",
                ""
            )]
            string bits
        )
        {
            var expected = new BitArray(bits.Length);
            for (var i = 0; i < bits.Length; i++)
                expected[i] = bits[i] == '1';

            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Varbit);
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Bit);
            var p3 = new NpgsqlParameter("p3", NpgsqlDbType.Varbit) {Value = bits};
            var p4 = new NpgsqlParameter {ParameterName = "p4", Value = expected};
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            p1.Value = p2.Value = expected;
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldValue<BitArray>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                Assert.That(() => reader.GetFieldValue<bool>(i), Throws.Exception.TypeOf<InvalidCastException>());
            }
        }

        [Test]
        public async Task Long()
        {
            using var conn = await OpenConnectionAsync();
            var bitLen = (conn.Settings.WriteBufferSize + 10) * 8;
            var chars = new char[bitLen];
            for (var i = 0; i < bitLen; i++)
                chars[i] = i % 2 == 0 ? '0' : '1';
            await Roundtrip_BitArray(new string(chars));
        }

        [Test]
        public async Task Roundtrip_BitVector32([Values(15, 0)] int bits)
        {
            var expected = new BitVector32(bits);

            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            cmd.Parameters.AddWithValue("p", expected);
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<BitVector32>(0), Is.EqualTo(expected));
        }

        [Test]
        public async Task BitVector32_too_long()
        {
            using var conn = await OpenConnectionAsync();
            using (var cmd = new NpgsqlCommand($"SELECT B'{new string('0', 34)}'", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                Assert.That(() => reader.GetFieldValue<BitVector32>(0), Throws.Exception.TypeOf<InvalidCastException>());
            }
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }

        [Test, Description("Roundtrips a single bit")]
        public async Task Single_bit()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p::BIT(1), B'01'::BIT(2)", conn);
            const bool expected = true;
            var p = new NpgsqlParameter("p", NpgsqlDbType.Bit);
            // Type inference? But bool is mapped to PG bool
            cmd.Parameters.Add(p);
            p.Value = expected;
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetBoolean(0), Is.EqualTo(true));
            Assert.That(reader.GetValue(0), Is.EqualTo(true));
            Assert.That(reader.GetFieldValue<bool>(0), Is.EqualTo(true));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(bool)));
        }

        [Test, Description("BIT(N) shouldn't be accessible as bool")]
        public async Task Bitstring_with_multiple_bits_as_bool_throws()
        {
            using var conn = await OpenConnectionAsync();
            using (var cmd = new NpgsqlCommand("SELECT B'01'::BIT(2)", conn))
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            {
                reader.Read();
                Assert.That(() => reader.GetBoolean(0), Throws.Exception.TypeOf<InvalidCastException>());

            }
            // Connection should still be OK
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }

        [Test]
        public async Task Array()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p", conn);
            var expected = new[] { new BitArray(new[] { true, false, true }), new BitArray(new[] { false }) };
            var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Varbit) { Value = expected };
            cmd.Parameters.Add(p);
            p.Value = expected;
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();

            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<BitArray[]>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
        }

        [Test]
        public async Task Array_of_single_bits()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p::BIT(1)[]", conn);
            var expected = new[] { true, false };
            var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Bit) {Value = expected};
            cmd.Parameters.Add(p);
            p.Value = expected;
            using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            var x = reader.GetValue(0);
            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<bool[]>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
        }

        [Test]
        public async Task Validation()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1::BIT VARYING", conn);
            var p = new NpgsqlParameter("p1", NpgsqlDbType.Bit);
            cmd.Parameters.Add(p);
            p.Value = "001q0";
            Assert.That(async () => await cmd.ExecuteReaderAsync(), Throws.Exception.TypeOf<FormatException>());

            // Make sure the connection state is OK
            Assert.That(await conn.ExecuteScalarAsync("SELECT 8"), Is.EqualTo(8));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2766")]
        [Timeout(3000)]
        public async Task Sequential_read_of_oversized_bit_array()
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT 1::bit(100000)", conn);
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            reader.Read();

            var actual = reader.GetFieldValue<BitArray>(0);
            Assert.That(actual, Has.Length.EqualTo(100000));
        }

        // Older tests from here

        // TODO: Bring this test back
#if FIX
        [Test]
        public async Task Bitstring([Values(true, false)] bool prepareCommand)
        {
            using (var cmd = Conn.CreateCommand())
            {
                cmd.CommandText = "Select :bs1 as output, :bs2, :bs3, :bs4, :bs5, array [1::bit, 0::bit], array [bit '10', bit '01'], :ba1, :ba2, :ba3";
                var output = new NpgsqlParameter() { ParameterName = "output", Direction = ParameterDirection.Output };
                cmd.Parameters.Add(output);
                cmd.Parameters.Add(new NpgsqlParameter("bs1", NpgsqlDbType.Bit) { Value = new BitString("1011") });
                cmd.Parameters.Add(new NpgsqlParameter("bs2", NpgsqlDbType.Bit, 1) { Value = true });
                cmd.Parameters.Add(new NpgsqlParameter("bs3", NpgsqlDbType.Bit, 1) { Value = false });
                cmd.Parameters.Add(new NpgsqlParameter("bs4", NpgsqlDbType.Bit, 2) { Value = new BitString("01") });
                cmd.Parameters.Add(new NpgsqlParameter("bs5", NpgsqlDbType.Varbit) { Value = new BitString("01") });
                cmd.Parameters.Add(new NpgsqlParameter("ba1", NpgsqlDbType.Varbit | NpgsqlDbType.Array) { Value = new BitString[] { new BitString("10"), new BitString("01") } });
                cmd.Parameters.Add(new NpgsqlParameter("ba2", NpgsqlDbType.Bit | NpgsqlDbType.Array, 1) { Value = new bool[] { true, false } });
                cmd.Parameters.Add(new NpgsqlParameter("ba3", NpgsqlDbType.Bit | NpgsqlDbType.Array, 1) { Value = new BitString[] { new BitString("1"), new BitString("0") } });
                if (prepareCommand)
                    cmd.Prepare();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.IsTrue(new BitString("1011") == (BitString)output.Value);
                    Assert.IsTrue(new BitString("1011") == (BitString)reader.GetValue(0));
                    Assert.AreEqual(true, reader.GetValue(1));
                    Assert.AreEqual(false, reader.GetValue(2));
                    Assert.IsTrue(new BitString("01") == (BitString)reader.GetValue(3));
                    Assert.IsTrue(new BitString("01") == (BitString)reader.GetValue(4));
                    Assert.AreEqual(true, ((bool[])reader.GetValue(5))[0]);
                    Assert.AreEqual(false, ((bool[])reader.GetValue(5))[1]);
                    for (int i = 6; i <= 7; i++)
                    {
                        Assert.AreEqual(new BitString("10"), ((BitString[])reader.GetValue(i))[0]);
                        Assert.AreEqual(new BitString("01"), ((BitString[])reader.GetValue(i))[1]);
                    }
                    for (int i = 8; i <= 9; i++)
                    {
                        Assert.AreEqual(true, ((bool[])reader.GetValue(i))[0]);
                        Assert.AreEqual(false, ((bool[])reader.GetValue(i))[1]);
                    }
                }
            }
        }
#endif

                public BitStringTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
