using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Npgsql.Util;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    /// <summary>
    /// Tests on PostgreSQL numeric types
    /// </summary>
    /// <summary>
    /// https://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </summary>
    public class NumericTypeTests : MultiplexingTestBase
    {
        [Test]
        public async Task Int16()
        {
            using (var conn = await OpenConnectionAsync())
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
                using (var reader = await cmd.ExecuteReaderAsync())
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
        public async Task Int32()
        {
            using (var conn = await OpenConnectionAsync())
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
                using (var reader = await cmd.ExecuteReaderAsync())
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
        public async Task UInt32(NpgsqlDbType npgsqlDbType)
        {
            var expected = 8u;
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", npgsqlDbType) { Value = expected });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader[0], Is.EqualTo(expected));
                    Assert.That(reader.GetProviderSpecificValue(0), Is.EqualTo(expected));
                    Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(uint)));
                }
            }
        }

        [Test]
        public async Task Int64()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Bigint);
                var p2 = new NpgsqlParameter("p2", DbType.Int64);
                var p3 = new NpgsqlParameter { ParameterName = "p3", Value = (long)8 };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = (short)8;
                using (var reader = await cmd.ExecuteReaderAsync())
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
        public async Task Double()
        {
            using (var conn = await OpenConnectionAsync())
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
                using (var reader = await cmd.ExecuteReaderAsync())
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
        public async Task DoubleSpecial(double value)
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Double, value);
                var actual = await cmd.ExecuteScalarAsync();
                Assert.That(actual, Is.EqualTo(value));
            }
        }

        [Test]
        public async Task Float()
        {
            const float expected = .123456F;
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
            {
                var p1 = new NpgsqlParameter("p1", NpgsqlDbType.Real);
                var p2 = new NpgsqlParameter("p2", DbType.Single);
                var p3 = new NpgsqlParameter {ParameterName = "p3", Value = expected};
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                cmd.Parameters.Add(p3);
                p1.Value = p2.Value = expected;
                using (var reader = await cmd.ExecuteReaderAsync())
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
        public async Task DoubleFloat(double value)
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                cmd.Parameters.AddWithValue("p", NpgsqlDbType.Real, value);
                var actual = await cmd.ExecuteScalarAsync();
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
        [TestCase(NpgsqlDbType.InternalChar, 1 + byte.MaxValue)]
        public async Task WriteOverflow(NpgsqlDbType type, object value)
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = new NpgsqlCommand("SELECT @p1", conn);

            var p1 = new NpgsqlParameter("p1", type)
            {
                Value = value
            };
            cmd.Parameters.Add(p1);
            Assert.ThrowsAsync<OverflowException>(async () => await cmd.ExecuteScalarAsync());
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
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
        public async Task ReadOverflow<T>(T readingType, NpgsqlDbType type, double value)
        {
            var typeString = GetTypeAsString(type);
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand($"SELECT {value}::{typeString}", conn))
            {
                Assert.ThrowsAsync<OverflowException>(async() =>
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        Assert.True(reader.Read());
                        reader.GetFieldValue<T>(0);
                    }
                });
            }

            string GetTypeAsString(NpgsqlDbType dbType)
                => dbType switch
                {
                    NpgsqlDbType.Smallint => "int2",
                    NpgsqlDbType.Integer  => "int4",
                    NpgsqlDbType.Bigint   => "int8",
                    _                     => throw new NotSupportedException()
                };
        }

        // Older tests

        [Test]
        public async Task DoubleWithoutPrepared()
        {
            using (var conn = await OpenConnectionAsync())
            using (var command = new NpgsqlCommand("select :field_float8", conn))
            {
                command.Parameters.Add(new NpgsqlParameter(":field_float8", NpgsqlDbType.Double));
                var x = 1d/7d;
                command.Parameters[0].Value = x;
                var valueReturned = await command.ExecuteScalarAsync();
                Assert.That(valueReturned, Is.EqualTo(x).Within(100).Ulps);
            }
        }

        [Test]
        public async Task NumberConversionWithCulture()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("select :p1", conn))
            using (TestUtil.SetCurrentCulture(new CultureInfo("es-ES")))
            {
                var parameter = new NpgsqlParameter("p1", NpgsqlDbType.Double) { Value = 5.5 };
                cmd.Parameters.Add(parameter);
                var result = await cmd.ExecuteScalarAsync();
                Assert.AreEqual(5.5, result);
            }
        }

        [Test]
        public async Task Money()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select '1'::MONEY, '12345'::MONEY / 100, '123456789012345'::MONEY / 100";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.AreEqual(1M, reader.GetValue(0));
                    Assert.AreEqual(123.45M, reader.GetValue(1));
                    Assert.AreEqual(1234567890123.45M, reader.GetValue(2));
                }
            }
        }

        public NumericTypeTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
