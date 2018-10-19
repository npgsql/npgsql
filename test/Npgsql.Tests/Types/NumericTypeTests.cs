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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL numeric types
    /// </summary>
    /// <summary>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </summary>
    public class NumericTypeTests : TestBase
    {
        [Test]
        public void Int16()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4, @p5", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Smallint);
                var p2 = new NpgsqlParameter("p2", DbType.Int16);
                var p3 = new NpgsqlParameter("p3", DbType.Byte);
                var p4 = new NpgsqlParameter { ParameterName = "p4", Value = (short)8 };
                var p5 = new NpgsqlParameter { ParameterName = "p5", Value = (byte)8  };
                Assert.That(p4.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Smallint));
                Assert.That(p4.DbType, Is.EqualTo(DbType.Int16));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                cmd.Parameters.Add(p4);
                cmd.Parameters.Add(p5);
                p1.Value = p2.Value = p3.Value = (long)8;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetInt16(i), Is.EqualTo(8));
                        Assert.That(reader.GetInt32(i), Is.EqualTo(8));
                        Assert.That(reader.GetInt64(i), Is.EqualTo(8));
                        Assert.That(reader.GetByte(i), Is.EqualTo(8));
                        Assert.That(reader.GetFloat(i), Is.EqualTo(8.0f));
                        Assert.That(reader.GetDouble(i), Is.EqualTo(8.0d));
                        Assert.That(reader.GetDecimal(i), Is.EqualTo(8.0m));
                        Assert.That(reader.GetValue(i), Is.EqualTo(8));
                        Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(short)));
                        Assert.That(reader.GetDataTypeName(i), Is.EqualTo("smallint"));
                    }
                }
            }
        }

        [Test]
        public void Int32()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Integer);
                var p2 = new NpgsqlParameter("p2", DbType.Int32);
                var p3 = new NpgsqlParameter { ParameterName = "p3", Value = 8 };
                Assert.That(p3.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
                Assert.That(p3.DbType, Is.EqualTo(DbType.Int32));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = (long)8;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetInt32(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetInt64(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetInt16(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetByte(i),                  Is.EqualTo(8));
                        Assert.That(reader.GetFloat(i),                 Is.EqualTo(8.0f));
                        Assert.That(reader.GetDouble(i),                Is.EqualTo(8.0d));
                        Assert.That(reader.GetDecimal(i),               Is.EqualTo(8.0m));
                        Assert.That(reader.GetValue(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                        Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof(int)));
                        Assert.That(reader.GetDataTypeName(i),          Is.EqualTo("integer"));
                    }
                }
            }
        }

        [Test, Description("Tests some types which are aliased to UInt32")]
        [TestCase(NpgsqlDbType.Oid, TestName="OID")]
        [TestCase(NpgsqlDbType.Xid, TestName="XID")]
        [TestCase(NpgsqlDbType.Cid, TestName="CID")]
        public void UInt32(NpgsqlDbType npgsqlDbType)
        {
            var expected = 8u;
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", npgsqlDbType) { Value = expected });
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader[0], Is.EqualTo(expected));
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(uint)));
                }
            }
        }

        [Test]
        public void Int64()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Bigint);
                var p2 = new NpgsqlParameter("p2", DbType.Int64);
                var p3 = new NpgsqlParameter { ParameterName = "p3", Value = (long)8 };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = (short)8;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetInt64(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetInt16(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetInt32(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetByte(i),                  Is.EqualTo(8));
                        Assert.That(reader.GetFloat(i),                 Is.EqualTo(8.0f));
                        Assert.That(reader.GetDouble(i),                Is.EqualTo(8.0d));
                        Assert.That(reader.GetDecimal(i),               Is.EqualTo(8.0m));
                        Assert.That(reader.GetValue(i),                 Is.EqualTo(8));
                        Assert.That(reader.GetProviderSpecificValue(i), Is.EqualTo(8));
                        Assert.That(reader.GetFieldType(i),             Is.EqualTo(typeof(long)));
                        Assert.That(reader.GetDataTypeName(i),          Is.EqualTo("bigint"));
                    }
                }
            }
        }

        [Test]
        public void Double()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                const double expected = 4.123456789012345;
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Double);
                var p2 = new NpgsqlParameter("p2", DbType.Double);
                var p3 = new NpgsqlParameter {ParameterName = "p3", Value = expected};
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetDouble(i), Is.EqualTo(expected).Within(10E-07));
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(double)));
                    }
                }
            }
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void DoubleSpecial(double value)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Double, value);
                var actual = cmd.ExecuteScalar();
                Assert.That(actual, Is.EqualTo(value));
            }
        }

        [Test]
        public void Float()
        {
            const float expected = .123456F;
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Real);
                var p2 = new NpgsqlParameter("p2", DbType.Single);
                var p3 = new NpgsqlParameter {ParameterName = "p3", Value = expected};
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = expected;
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFloat(i), Is.EqualTo(expected).Within(10E-07));
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(float)));
                    }
                }
            }
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void DoubleFloat(double value)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Real, value);
                var actual = cmd.ExecuteScalar();
                Assert.That(actual, Is.EqualTo(value));
            }
        }

        [Test, Description("Tests handling of numeric overflow when writing data")]
        [TestCase(NpgsqlDbType.Smallint, 1 + short.MaxValue)]
        [TestCase(NpgsqlDbType.Smallint, 1L + short.MaxValue)]
        [TestCase(NpgsqlDbType.Smallint, 1F + short.MaxValue)]
        [TestCase(NpgsqlDbType.Smallint, 1D + short.MaxValue)]
        [TestCase(NpgsqlDbType.Integer, 1L + int.MaxValue)]
        [TestCase(NpgsqlDbType.Integer, 1F + int.MaxValue)]
        [TestCase(NpgsqlDbType.Integer, 1D + int.MaxValue)]
        [TestCase(NpgsqlDbType.Bigint, 1F + long.MaxValue)]
        [TestCase(NpgsqlDbType.Bigint, 1D + long.MaxValue)]
        public void WriteOverflow(NpgsqlDbType type, object value)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1", conn))
            {
                var p1 = new NpgsqlParameter("p1", type)
                {
                    Value = value
                };
                cmd.Parameters.Add(p1);
                Assert.Throws<OverflowException>(() =>
                {
                    using (var reader = cmd.ExecuteReader()) { }
                });
            }
        }

        static IEnumerable<TestCaseData> ReadOverflowTestCases
        {
            get
            {
                yield return new TestCaseData(NpgsqlDbType.Smallint, 1D + byte.MaxValue){ };
            }
        }
        [Test, Description("Tests handling of numeric overflow when reading data")]
        [TestCase((byte)0, NpgsqlDbType.Smallint, 1D + byte.MaxValue)]
        [TestCase((sbyte)0, NpgsqlDbType.Smallint, 1D + sbyte.MaxValue)]
        [TestCase((byte)0, NpgsqlDbType.Integer, 1D + byte.MaxValue)]
        [TestCase((short)0, NpgsqlDbType.Integer, 1D + short.MaxValue)]
        [TestCase((byte)0, NpgsqlDbType.Bigint, 1D + byte.MaxValue)]
        [TestCase((short)0, NpgsqlDbType.Bigint, 1D + short.MaxValue)]
        [TestCase(0, NpgsqlDbType.Bigint, 1D + int.MaxValue)]
        public void ReadOverflow<T>(T readingType, NpgsqlDbType type, double value)
        {
            var typeString = GetTypeAsString(type);
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand($"SELECT {value}::{typeString}", conn))
            {
                Assert.Throws<OverflowException>(() =>
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Assert.True(reader.Read());
                        reader.GetFieldValue<T>(0);
                    }
                });
            }

            string GetTypeAsString(NpgsqlDbType dbType)
            {
                switch (dbType)
                {
                case NpgsqlDbType.Smallint:
                    return "int2";
                case NpgsqlDbType.Integer:
                    return "int4";
                case NpgsqlDbType.Bigint:
                    return "int8";
                default:
                    throw new NotSupportedException();
                }
            }
        }

        // Older tests

        [Test]
        public void DoubleWithoutPrepared()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("select :field_float8", conn))
            {
                command.Parameters.Add(new NpgsqlParameter(":field_float8", NpgsqlDbType.Double));
                var x = 1d/7d;
                command.Parameters[0].Value = x;
                var valueReturned = command.ExecuteScalar();
                Assert.That(valueReturned, Is.EqualTo(x).Within(100).Ulps);
            }
        }

        [Test]
        public void NumberConversionWithCulture()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("select :p1", conn))
            using (new CultureSetter(new CultureInfo("es-ES")))
            {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double) { Value = 5.5 };
                cmd.Parameters.Add(parameter);
                var result = cmd.ExecuteScalar();
                Assert.AreEqual(5.5, result);
            }
        }

        [Test]
        public void TestMoney([Values(PrepareOrNot.Prepared, PrepareOrNot.NotPrepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select '1'::MONEY, '12345'::MONEY / 100, '123456789012345'::MONEY / 100";
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(1M, reader.GetValue(0));
                    Assert.AreEqual(123.45M, reader.GetValue(1));
                    Assert.AreEqual(1234567890123.45M, reader.GetValue(2));
                }
            }
        }
    }
}
