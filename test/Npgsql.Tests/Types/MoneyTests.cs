using System;
using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

public class MoneyTests : TestBase
{
    static readonly object[] MoneyValues = new[]
    {
        new object[] { "$1.22", 1.22M },
        new object[] { "$1,000.22", 1000.22M },
        new object[] { "$1,000,000.22", 1000000.22M },
        new object[] { "$1,000,000,000.22", 1000000000.22M },
        new object[] { "$1,000,000,000,000.22", 1000000000000.22M },
        new object[] { "$1,000,000,000,000,000.22", 1000000000000000.22M },

        new object[] { "$92,233,720,368,547,758.07", +92233720368547758.07M },
        new object[] { "-$92,233,720,368,547,758.08", -92233720368547758.08M },
        new object[] { "-$92,233,720,368,547,758.08", -92233720368547758.08M },
    };

    [Test]
    [TestCaseSource(nameof(MoneyValues))]
    public async Task Money(string sqlLiteral, decimal money)
    {
        using var conn = await OpenConnectionAsync();
        await conn.ExecuteNonQueryAsync("SET lc_monetary='C'");
        await AssertType(conn, money, sqlLiteral, "money", NpgsqlDbType.Money, DbType.Currency, isDefault: false);
    }

    [Test]
    public async Task Non_decimal_types_are_not_supported()
    {
        await AssertTypeUnsupportedRead<byte>("8", "money");
        await AssertTypeUnsupportedRead<short>("8", "money");
        await AssertTypeUnsupportedRead<int>("8", "money");
        await AssertTypeUnsupportedRead<long>("8", "money");
        await AssertTypeUnsupportedRead<float>("8", "money");
        await AssertTypeUnsupportedRead<double>("8", "money");
    }

    static readonly object[] WriteWithLargeScaleCases = new[]
    {
        new object[] { "0.004::money", 0.004M, 0.00M },
        new object[] { "0.005::money", 0.005M, 0.01M },
    };

    [Test]
    [TestCaseSource(nameof(WriteWithLargeScaleCases))]
    public async Task Write_with_large_scale(string query, decimal parameter, decimal expected)
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p, @p = " + query, conn);
        cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Money) { Value = parameter });
        using var rdr = await cmd.ExecuteReaderAsync();
        rdr.Read();
        Assert.That(decimal.GetBits(rdr.GetFieldValue<decimal>(0)), Is.EqualTo(decimal.GetBits(expected)));
        Assert.That(rdr.GetFieldValue<bool>(1));
    }
}