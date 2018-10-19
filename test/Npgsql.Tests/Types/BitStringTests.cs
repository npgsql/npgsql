#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on the PostgreSQL BitString type
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-bit.html
    /// </remarks>
    public class BitStringTests : TestBase
    {
        [Test]
        public void RoundtripBitArray(
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

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Varbit);
                var p2 = new NpgsqlParameter("p2", NpgsqlDbType.Bit);
                var p3 = new NpgsqlParameter("p3", NpgsqlDbType.Varbit) {Value = bits};
                var p4 = new NpgsqlParameter {ParameterName = "p4", Value = expected};
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                p1.Value = p2.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldValue<BitArray>(i), Is.EqualTo(expected));
                        Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                        Assert.That(() => reader.GetFieldValue<bool>(i), Throws.Exception.TypeOf<InvalidCastException>());
                    }
                }
            }
        }

        [Test]
        public void Long()
        {
            using (var conn = OpenConnection())
            {
                var bitLen = (conn.Settings.WriteBufferSize + 10) * 8;
                var chars = new char[bitLen];
                for (var i = 0; i < bitLen; i++)
                    chars[i] = i % 2 == 0 ? '0' : '1';
                RoundtripBitArray(new string(chars));
            }
        }

        [Test]
        public void RoundtripBitVector32([Values(15, 0)] int bits)
        {
            var expected = new BitVector32(bits);

            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", expected);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetFieldValue<BitVector32>(0), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void BitVector32TooLong()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand($"SELECT B'{new string('0', 34)}'", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(() => reader.GetFieldValue<BitVector32>(0), Throws.Exception.TypeOf<InvalidCastException>());
                }
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, Description("Roundtrips a single bit")]
        public void SingleBit()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::BIT(1), B'01'::BIT(2)", conn))
            {
                const bool expected = true;
                var p = new NpgsqlParameter("p", NpgsqlDbType.Bit);
                // Type inference? But bool is mapped to PG bool
                cmd.Parameters.Add(p);
                p.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    Assert.That(reader.GetBoolean(0), Is.EqualTo(true));
                    Assert.That(reader.GetValue(0), Is.EqualTo(true));
                    Assert.That(reader.GetFieldValue<bool>(0), Is.EqualTo(true));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(bool)));
                }
            }
        }

        [Test, Description("BIT(N) shouldn't be accessible as bool")]
        public void BitstringAsSingleBit()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("SELECT B'01'::BIT(2)", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(() => reader.GetBoolean(0), Throws.Exception.TypeOf<InvalidCastException>());

                }
                // Connection should still be OK
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test]
        public void Array()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var expected = new[] { new BitArray(new[] { true, false, true }), new BitArray(new[] { false }) };
                var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Varbit) { Value = expected };
                cmd.Parameters.Add(p);
                p.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<BitArray[]>(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
                }
            }
        }

        [Test]
        public void SingleBitArray()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::BIT(1)[]", conn))
            {
                var expected = new[] { true, false };
                var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Bit) {Value = expected};
                cmd.Parameters.Add(p);
                p.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    var x = reader.GetValue(0);
                    Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<bool[]>(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
                }
            }
        }

        [Test]
        public void Validation()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1::BIT VARYING", conn))
            {
                var p = new NpgsqlParameter("p1", NpgsqlDbType.Bit);
                cmd.Parameters.Add(p);
                cmd.Prepare();
                p.Value = "001q0";
                Assert.That(() => cmd.ExecuteReader(), Throws.Exception.TypeOf<FormatException>());

                // Make sure the connection state is OK
                Assert.That(conn.ExecuteScalar("SELECT 8"), Is.EqualTo(8));
            }
        }

        // Older tests from here

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
    }
}
