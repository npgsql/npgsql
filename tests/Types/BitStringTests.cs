using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests.Types
{
    /// <summary>
    /// Tests on the PostgreSQL BitString type
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-bit.html
    /// </remarks>
    public class BitStringTests : TestBase
    {
        [Test]
        public void Roundtrip(
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

            var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", Conn);
            var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Varbit);
            var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Bit);
            var p3 = new NpgsqlParameter("p3", NpgsqlDbType.Varbit) { Value = bits };
            var p4 = new NpgsqlParameter { ParameterName = "p4", Value = expected };
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            p1.Value = p2.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                Assert.That(reader.GetFieldValue<BitArray>(i), Is.EqualTo(expected));
                Assert.That(reader.GetValue(i),                Is.EqualTo(expected));
                Assert.That(() => reader.GetFieldValue<bool>(i), Throws.Exception.TypeOf<InvalidCastException>());
            }

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void Long()
        {
            var bitLen = (Conn.BufferSize + 10) * 8;
            var chars = new char[bitLen];
            for (var i = 0; i < bitLen; i++)
                chars[i] = i % 2 == 0 ? '0' : '1';
            Roundtrip(new string(chars));
        }

        [Test, Description("Roundtrips a single bit")]
        public void SingleBit()
        {
            const bool expected = true;
            var cmd = new NpgsqlCommand("SELECT @p::BIT(1), B'01'::BIT(2)", Conn);
            var p = new NpgsqlParameter("p", NpgsqlDbType.Bit);
            // Type inference? But bool is mapped to PG bool
            cmd.Parameters.Add(p);
            p.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader.GetBoolean(0), Is.EqualTo(true));
            Assert.That(reader.GetValue(0), Is.EqualTo(true));
            Assert.That(reader.GetFieldValue<bool>(0), Is.EqualTo(true));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof (bool)));

            // BIT(N) shouldn't be accessible as bool
            Assert.That(() => reader.GetBoolean(1), Throws.Exception.TypeOf<InvalidCastException>());

            reader.Dispose();
            cmd.Dispose();            
        }

        [Test]
        public void Array()
        {
            var expected = new[] { new BitArray(new[] { true, false, true }), new BitArray(new[] { false }) };
            var cmd = new NpgsqlCommand("SELECT @p", Conn);
            var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Varbit) { Value = expected };
            cmd.Parameters.Add(p);
            p.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<BitArray[]>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void SingleBitArray()
        {
            var expected = new[] { true, false };
            var cmd = new NpgsqlCommand("SELECT @p::BIT(1)[]", Conn);
            var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Bit) { Value = expected };
            cmd.Parameters.Add(p);
            p.Value = expected;
            var reader = cmd.ExecuteReader();
            reader.Read();

            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldValue<bool[]>(0), Is.EqualTo(expected));
            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));

            reader.Dispose();
            cmd.Dispose();
        }

        [Test]
        public void Validation()
        {
            var cmd = new NpgsqlCommand("SELECT @p1::BIT VARYING", Conn);
            var p = new NpgsqlParameter("p1", NpgsqlDbType.Bit);
            cmd.Parameters.Add(p);
            cmd.Prepare();
            p.Value = "001q0";
            Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<FormatException>());

            // Make sure the connection state is OK
            Assert.That(ExecuteScalar("SELECT 8"), Is.EqualTo(8));
        }

        // Older tests from here

        [Test]
        public void BitTypeSupport()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = true;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }

            Assert.IsTrue((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data)"));
        }

        [Test]
        public void BitTypeSupport2()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = 3;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }

            Assert.IsTrue((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data);"));
        }

        [Test]
        public void BitTypeSupport3()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = 6;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }

            Assert.IsFalse((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data)"));
        }

        [Test]
        public void BitTypeSupportWithPrepare()
        {
            using (var command = new NpgsqlCommand("INSERT INTO data(field_bit) VALUES (:a);", Conn))
            {
                var p = new NpgsqlParameter("a", NpgsqlDbType.Bit);
                p.Value = true;
                command.Parameters.Add(p);
                command.Prepare();
                command.ExecuteNonQuery();
            }

            Assert.IsTrue((bool)ExecuteScalar("SELECT field_bit FROM data WHERE field_serial = (SELECT MAX(field_serial) FROM data)"));
        }

        // TODO: Bring this test back
#if FIX
        [Test]
        public void BitString([Values(true, false)] bool prepareCommand)
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
                using (var reader = cmd.ExecuteReader())
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
        public BitStringTests(string backendVersion) : base(backendVersion) {}
    }
}
