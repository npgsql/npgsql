using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Data;

namespace Npgsql.Tests.Types
{
    public class MoneyTests : TestBase
    {
        static readonly object[] ReadWriteCases = new[]
        {
            new object[] { "1.22::money", 1.22M },
            new object[] { "1000.22::money", 1000.22M },
            new object[] { "1000000.22::money", 1000000.22M },
            new object[] { "1000000000.22::money", 1000000000.22M },
            new object[] { "1000000000000.22::money", 1000000000000.22M },
            new object[] { "1000000000000000.22::money", 1000000000000000.22M },

            new object[] { "(+92233720368547758.07::numeric)::money", +92233720368547758.07M },
            new object[] { "(-92233720368547758.08::numeric)::money", -92233720368547758.08M },
        };

        [Test]
        [TestCaseSource(nameof(ReadWriteCases))]
        public void Read(string query, decimal expected)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT " + query, conn))
            {
                Assert.That(decimal.GetBits(cmd.ExecuteScalar<decimal>()), Is.EqualTo(decimal.GetBits(expected)));
            }
        }

        [Test]
        [TestCaseSource(nameof(ReadWriteCases))]
        public void Write(string query, decimal expected)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p, @p = " + query, conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Money) { Value = expected });
                using (var rdr = cmd.ExecuteRecord())
                {
                    Assert.That(decimal.GetBits(rdr.GetFieldValue<decimal>(0)), Is.EqualTo(decimal.GetBits(expected)));
                    Assert.That(rdr.GetFieldValue<bool>(1));
                }
            }
        }

        static readonly object[] WriteWithLargeScaleCases = new[]
        {
            new object[] { "0.004::money", 0.004M, 0.00M },
            new object[] { "0.005::money", 0.005M, 0.01M },
        };

        [Test]
        [TestCaseSource(nameof(WriteWithLargeScaleCases))]
        public void WriteWithLargeScale(string query, decimal parameter, decimal expected)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p, @p = " + query, conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Money) { Value = parameter });
                using (var rdr = cmd.ExecuteRecord())
                {
                    Assert.That(decimal.GetBits(rdr.GetFieldValue<decimal>(0)), Is.EqualTo(decimal.GetBits(expected)));
                    Assert.That(rdr.GetFieldValue<bool>(1));
                }
            }
        }

        [Test]
        public void Mapping()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Money) { Value = 8M });
                cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Currency) { Value = 8M });

                using (var rdr = cmd.ExecuteRecord())
                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(rdr.GetFieldType(i), Is.EqualTo(typeof(decimal)));
                        Assert.That(rdr.GetDataTypeName(i), Is.EqualTo("money"));
                        Assert.That(rdr.GetValue(i), Is.EqualTo(8M));
                        Assert.That(rdr.GetProviderSpecificValue(i), Is.EqualTo(8M));
                        Assert.That(rdr.GetFieldValue<decimal>(i), Is.EqualTo(8M));
                        Assert.That(() => rdr.GetFieldValue<byte>(i), Throws.InstanceOf<InvalidCastException>());
                        Assert.That(() => rdr.GetFieldValue<short>(i), Throws.InstanceOf<InvalidCastException>());
                        Assert.That(() => rdr.GetFieldValue<int>(i), Throws.InstanceOf<InvalidCastException>());
                        Assert.That(() => rdr.GetFieldValue<long>(i), Throws.InstanceOf<InvalidCastException>());
                        Assert.That(() => rdr.GetFieldValue<float>(i), Throws.InstanceOf<InvalidCastException>());
                        Assert.That(() => rdr.GetFieldValue<double>(i), Throws.InstanceOf<InvalidCastException>());
                    }
            }
        }
    }
}
