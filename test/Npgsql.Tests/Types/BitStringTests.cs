using System;
using System.Collections;
using System.Collections.Specialized;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

/// <summary>
/// Tests on the PostgreSQL BitString type
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-bit.html
/// </remarks>
public class BitStringTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    [TestCase("10110110", TestName = "BitArray")]
    [TestCase("1011011000101111010110101101011011", TestName = "BitArray_with_34_bits")]
    [TestCase("", TestName = "BitArray_empty")]
    public async Task BitArray(string sqlLiteral)
    {
        var len = sqlLiteral.Length;

        var bitArray = new BitArray(len);
        for (var i = 0; i < sqlLiteral.Length; i++)
            bitArray[i] = sqlLiteral[i] == '1';

        await AssertType(bitArray, sqlLiteral, "bit varying", NpgsqlDbType.Varbit);

        if (len > 0)
            await AssertType(bitArray, sqlLiteral, $"bit({len})", NpgsqlDbType.Bit, isDefaultForWriting: false);
    }

    [Test]
    public async Task BitArray_long()
    {
        await using var conn = await OpenConnectionAsync();
        var bitLen = (conn.Settings.WriteBufferSize + 10) * 8;
        var chars = new char[bitLen];
        for (var i = 0; i < bitLen; i++)
            chars[i] = i % 2 == 0 ? '0' : '1';
        await BitArray(new string(chars));
    }

    [Test]
    public Task BitVector32()
        => AssertType(
            new BitVector32(4), "00000000000000000000000000000100", "bit varying", NpgsqlDbType.Varbit, isDefaultForReading: false);

    [Test]
    public Task BitVector32_too_long()
        => AssertTypeUnsupportedRead<BitVector32, InvalidCastException>(new string('0', 34), "bit varying");

    [Test]
    public Task Bool()
        => AssertType(true, "1", "bit(1)", NpgsqlDbType.Bit, isDefault: false);

    [Test]
    public async Task Bitstring_with_multiple_bits_as_bool_throws()
    {
        await AssertTypeUnsupportedRead<bool, InvalidCastException>("01", "varbit");
        await AssertTypeUnsupportedRead<bool, InvalidCastException>("01", "bit(2)");
    }

    [Test]
    public async Task Array()
    {
        using var conn = await OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var expected = new[] { new BitArray([true, false, true]), new BitArray([false]) };
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
    public async Task Array_of_single_bits_and_null()
    {
        var dataSource = CreateDataSource(builder => builder.ArrayNullabilityMode = ArrayNullabilityMode.Always);
        using var conn = await dataSource.OpenConnectionAsync();
        using var cmd = new NpgsqlCommand("SELECT @p::BIT(1)[]", conn);
        var expected = new bool?[] { true, false, null };
        var p = new NpgsqlParameter("p", NpgsqlDbType.Array | NpgsqlDbType.Bit) {Value = expected};
        cmd.Parameters.Add(p);
        p.Value = expected;
        using var reader = await cmd.ExecuteReaderAsync();
        reader.Read();
        var x = reader.GetValue(0);
        Assert.That(reader.GetValue(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldValue<bool?[]>(0), Is.EqualTo(expected));
        Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Array)));
    }

    [Test]
    public Task As_string()
        => AssertType("010101", "010101", "bit varying", NpgsqlDbType.Varbit, isDefault: false);

    [Test]
    public Task Write_as_string_validation()
        => AssertTypeUnsupportedWrite<string, ArgumentException>("001q0", "bit varying");
}
