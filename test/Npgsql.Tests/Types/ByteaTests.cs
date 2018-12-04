﻿#region License
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on the PostgreSQL bytea type
    /// </summary>
    /// <summary>
    /// http://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </summary>
    class ByteaTests : TestBase
    {
        [Test, Description("Roundtrips a bytea")]
        public void Roundtrip()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                byte[] expected = { 1, 2, 3, 4, 5 };
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Bytea);
                var p2 = new NpgsqlParameter("p2", DbType.Binary);
                var p3 = new NpgsqlParameter { ParameterName = "p3", Value = expected };
                Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
                Assert.That(p3.DbType, Is.EqualTo(DbType.Binary));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(byte[])));
                        Assert.That(reader.GetFieldValue<byte[]>(i), Is.EqualTo(expected));
                        Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                    }
                }
            }
        }

        [Test]
        public void RoundtripLarge()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p::BYTEA", conn))
            {
                var expected = new byte[conn.Settings.WriteBufferSize + 100];
                for (var i = 0; i < expected.Length; i++)
                    expected[i] = 8;
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Bytea) { Value = expected });
                var reader = cmd.ExecuteReader();
                reader.Read();
                Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(byte[])));
                Assert.That(reader.GetFieldValue<byte[]>(0), Is.EqualTo(expected));
            }
        }

        [Test]
        public void Read([Values(CommandBehavior.Default, CommandBehavior.SequentialAccess)] CommandBehavior behavior)
        {
            using (var conn = OpenConnection())
            {
                // TODO: This is too small to actually test any interesting sequential behavior
                byte[] expected = {1, 2, 3, 4, 5};
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (bytes BYTEA)");
                conn.ExecuteNonQuery($@"INSERT INTO data (bytes) VALUES ({TestUtil.EncodeByteaHex(expected)})");

                const string queryText = @"SELECT bytes, 'foo', bytes, bytes, bytes FROM data";
                using (var cmd = new NpgsqlCommand(queryText, conn))
                using (var reader = cmd.ExecuteReader(behavior))
                {
                    reader.Read();

                    var actual = reader.GetFieldValue<byte[]>(0);
                    Assert.That(actual, Is.EqualTo(expected));

                    if (behavior.IsSequential())
                        Assert.That(() => reader[0], Throws.Exception.TypeOf<InvalidOperationException>(), "Seek back sequential");
                    else
                        Assert.That(reader.GetFieldValue<byte[]>(0), Is.EqualTo(expected));

                    Assert.That(reader.GetString(1), Is.EqualTo("foo"));

                    Assert.That(reader[2], Is.EqualTo(expected));
                    Assert.That(reader.GetValue(3), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldValue<byte[]>(4), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void EmptyRoundtrip()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT :val::BYTEA", conn))
            {
                var expected = new byte[0];
                cmd.Parameters.Add("val", NpgsqlDbType.Bytea);
                cmd.Parameters["val"].Value = expected;
                var result = (byte[])cmd.ExecuteScalar();
                Assert.That(result, Is.EqualTo(expected));
            }
        }

        [Test, Description("Tests that bytea values are truncated when the NpgsqlParameter's Size is set")]
        public void Truncate()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                byte[] data = { 1, 2, 3, 4, 5, 6 };
                var p = new NpgsqlParameter("p", data) { Size = 4 };
                cmd.Parameters.Add(p);
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));

                // NpgsqlParameter.Size needs to persist when value is changed
                byte[] data2 = { 11, 12, 13, 14, 15, 16 };
                p.Value = data2;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(new byte[] { 11, 12, 13, 14 }));

                // NpgsqlParameter.Size larger than the value size should mean the value size, as well as 0 and -1
                p.Size = data2.Length + 10;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));
                p.Size = 0;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));
                p.Size = -1;
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(data2));

                Assert.That(() => p.Size = -2, Throws.Exception.TypeOf<ArgumentException>());
            }
        }

        [Test]
        public void ByteaOverArrayOfBytes()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ByteaOverArrayOfBytes),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", new byte[3]);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo("bytea"));
                }
            }
        }

        [Test]
        public void ArrayOfBytea()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT :p1", conn))
            {
                var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
                var inVal = new[] { bytes, bytes };
                cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array, inVal);
                var retVal = (byte[][])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }


        // Older tests from here

        [Test]
        public void Prepared()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("select :p1", conn))
            {
                var bytes = new byte[] { 1, 2, 3, 4, 5, 34, 39, 48, 49, 50, 51, 52, 92, 127, 128, 255, 254, 253, 252, 251 };
                var inVal = new[] { bytes, bytes };
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Bytea | NpgsqlDbType.Array);
                parameter.Value = inVal;
                cmd.Parameters.Add(parameter);
                cmd.Prepare();

                var retVal = (byte[][])cmd.ExecuteScalar();
                Assert.AreEqual(inVal.Length, retVal.Length);
                Assert.AreEqual(inVal[0], retVal[0]);
                Assert.AreEqual(inVal[1], retVal[1]);
            }
        }

        [Test]
        public void Insert1()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @bytes", conn))
            {
                byte[] toStore = { 0, 1, 255, 254 };
                cmd.Parameters.AddWithValue("@bytes", toStore);
                var result = (byte[])cmd.ExecuteScalar();
                Assert.AreEqual(toStore, result);
            }
        }

        [Test]
        public void ArraySegment()
        {
            using (var conn = OpenConnection())
            {
                using (var cmd = new NpgsqlCommand("select :bytearr", conn))
                {
                    var arr = new byte[20000];
                    for (var i = 0; i < arr.Length; i++)
                    {
                        arr[i] = (byte) (i & 0xff);
                    }

                    // Big value, should go through "direct buffer"
                    var segment = new ArraySegment<byte>(arr, 17, 18000);
                    cmd.Parameters.Add(new NpgsqlParameter("bytearr", DbType.Binary) {Value = segment});
                    var returned = (byte[]) cmd.ExecuteScalar();
                    Assert.That(segment.SequenceEqual(returned));

                    cmd.Parameters[0].Size = 17000;
                    returned = (byte[]) cmd.ExecuteScalar();
                    Assert.That(returned.SequenceEqual(new ArraySegment<byte>(segment.Array, segment.Offset, 17000)));

                    // Small value, should be written normally through the NpgsqlBuffer
                    segment = new ArraySegment<byte>(arr, 6, 10);
                    cmd.Parameters[0].Value = segment;
                    returned = (byte[]) cmd.ExecuteScalar();
                    Assert.That(segment.SequenceEqual(returned));

                    cmd.Parameters[0].Size = 2;
                    returned = (byte[]) cmd.ExecuteScalar();
                    Assert.That(returned.SequenceEqual(new ArraySegment<byte>(segment.Array, segment.Offset, 2)));
                }

                using (var cmd = new NpgsqlCommand("select :bytearr", conn))
                {
                    var segment = new ArraySegment<byte>(new byte[] {1, 2, 3}, 1, 2);
                    cmd.Parameters.AddWithValue("bytearr", segment);
                    Assert.That(segment.SequenceEqual((byte[]) cmd.ExecuteScalar()));
                }
            }
        }

        [Test, Description("Writes a bytea that doesn't fit in a partially-full buffer, but does fit in an empty buffer")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/654")]
        public void WriteDoesntFitInitiallyButFitsLater()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field BYTEA)");

                var bytea = new byte[8180];
                for (var i = 0; i < bytea.Length; i++)
                {
                    bytea[i] = (byte) (i%256);
                }

                using (var cmd = new NpgsqlCommand("INSERT INTO data (field) VALUES (@p)", conn))
                {
                    cmd.Parameters.AddWithValue("@p", bytea);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
