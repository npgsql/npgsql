using NpgsqlTypes;
using NUnit.Framework;
using System.Data;

namespace Npgsql.Tests.Types
{
    public class NumericTests : TestBase
    {
        static readonly object[] ReadWriteCases = new[]
        {
            new object[] { "0.0000000000000000000000000001::numeric", 0.0000000000000000000000000001M },
            new object[] { "0.000000000000000000000001::numeric", 0.000000000000000000000001M },
            new object[] { "0.00000000000000000001::numeric", 0.00000000000000000001M },
            new object[] { "0.0000000000000001::numeric", 0.0000000000000001M },
            new object[] { "0.000000000001::numeric", 0.000000000001M },
            new object[] { "0.00000001::numeric", 0.00000001M },
            new object[] { "0.0001::numeric", 0.0001M },
            new object[] { "1::numeric", 1M },
            new object[] { "10000::numeric", 10000M },
            new object[] { "100000000::numeric", 100000000M },
            new object[] { "1000000000000::numeric", 1000000000000M },
            new object[] { "10000000000000000::numeric", 10000000000000000M },
            new object[] { "100000000000000000000::numeric", 100000000000000000000M },
            new object[] { "1000000000000000000000000::numeric", 1000000000000000000000000M },
            new object[] { "10000000000000000000000000000::numeric", 10000000000000000000000000000M },
            
            new object[] { "11.222233334444555566667777888::numeric", 11.222233334444555566667777888M },
            new object[] { "111.22223333444455556666777788::numeric", 111.22223333444455556666777788M },
            new object[] { "1111.2222333344445555666677778::numeric", 1111.2222333344445555666677778M },

            new object[] { "+79228162514264337593543950335::numeric", +79228162514264337593543950335M },
            new object[] { "-79228162514264337593543950335::numeric", -79228162514264337593543950335M },

            // It is important to test rounding on both even and odd
            // numbers to make sure midpoint rounding is away from zero.
            new object[] { "1::numeric(10,2)", 1.00M },
            new object[] { "2::numeric(10,2)", 2.00M },

            new object[] { "1.2::numeric(10,1)", 1.2M },
            new object[] { "1.2::numeric(10,2)", 1.20M },
            new object[] { "1.2::numeric(10,3)", 1.200M },
            new object[] { "1.2::numeric(10,4)", 1.2000M },
            new object[] { "1.2::numeric(10,5)", 1.20000M },

            new object[] { "1.4::numeric(10,0)", 1M },
            new object[] { "1.5::numeric(10,0)", 2M },
            new object[] { "2.4::numeric(10,0)", 2M },
            new object[] { "2.5::numeric(10,0)", 3M },

            new object[] { "-1.4::numeric(10,0)", -1M },
            new object[] { "-1.5::numeric(10,0)", -2M },
            new object[] { "-2.4::numeric(10,0)", -2M },
            new object[] { "-2.5::numeric(10,0)", -3M },
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
                cmd.Parameters.AddWithValue("p", expected);
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
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3, @p4", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.Numeric) { Value = 8M });
                cmd.Parameters.Add(new NpgsqlParameter("p2", DbType.Decimal) { Value = 8M });
                cmd.Parameters.Add(new NpgsqlParameter("p3", DbType.VarNumeric) { Value = 8M });
                cmd.Parameters.Add(new NpgsqlParameter("p4", 8M));

                using (var rdr = cmd.ExecuteRecord())
                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(rdr.GetFieldType(i), Is.EqualTo(typeof(decimal)));
                        Assert.That(rdr.GetDataTypeName(i), Is.EqualTo("numeric"));
                        Assert.That(rdr.GetValue(i), Is.EqualTo(8M));
                        Assert.That(rdr.GetProviderSpecificValue(i), Is.EqualTo(8M));
                        Assert.That(rdr.GetFieldValue<decimal>(i), Is.EqualTo(8M));
                        Assert.That(rdr.GetFieldValue<byte>(i), Is.EqualTo(8));
                        Assert.That(rdr.GetFieldValue<short>(i), Is.EqualTo(8));
                        Assert.That(rdr.GetFieldValue<int>(i), Is.EqualTo(8));
                        Assert.That(rdr.GetFieldValue<long>(i), Is.EqualTo(8));
                        Assert.That(rdr.GetFieldValue<float>(i), Is.EqualTo(8.0f));
                        Assert.That(rdr.GetFieldValue<double>(i), Is.EqualTo(8.0d));
                    }
            }
        }
    }
}
