using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

/// <summary>
/// Tests on PostgreSQL numeric types
/// </summary>
/// <summary>
/// https://www.postgresql.org/docs/current/static/datatype-numeric.html
/// </summary>
public class NumericTypeTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public async Task Int16()
    {
        await AssertType((short)8, "8", "smallint", dbType: DbType.Int16);
        // Clr byte/sbyte maps to 'int2' as there is no byte type in PostgreSQL, byte[] maps to bytea however.
        await AssertType((byte)8, "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: DbType.Int16, valueTypeEqualsFieldType: false, skipArrayCheck: true);
        await AssertType((sbyte)8, "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: DbType.Int16, valueTypeEqualsFieldType: false);

        await AssertType(8,       "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int16, DbType.Int32), valueTypeEqualsFieldType: false);
        await AssertType(8L,      "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int16, DbType.Int64), valueTypeEqualsFieldType: false);
        await AssertType(8F,      "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int16, DbType.Single), valueTypeEqualsFieldType: false);
        await AssertType(8D,      "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int16, DbType.Double), valueTypeEqualsFieldType: false);
        await AssertType(8M,      "8", "smallint", dataTypeInference: DataTypeInference.Mismatch,
                dbType: new(DbType.Int16, DbType.Decimal), valueTypeEqualsFieldType: false);
    }

    [Test]
    public async Task Int32()
    {
        await AssertType(8, "8", "integer", dbType: DbType.Int32);

        await AssertType((short)8, "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Int16), valueTypeEqualsFieldType: false);
        await AssertType(8L,       "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Int64), valueTypeEqualsFieldType: false);
        await AssertType((byte)8,  "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Int16), valueTypeEqualsFieldType: false, skipArrayCheck: true); // byte[] maps to bytea
        await AssertType((sbyte)8, "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Int16), valueTypeEqualsFieldType: false);
        await AssertType(8F,       "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Single), valueTypeEqualsFieldType: false);
        await AssertType(8D,       "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Double), valueTypeEqualsFieldType: false);
        await AssertType(8M,       "8", "integer", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int32, DbType.Decimal), valueTypeEqualsFieldType: false);
    }

    [Test, Description("Tests some types which are aliased to UInt32")]
    [TestCase("oid", TestName="OID")]
    [TestCase("xid", TestName="XID")]
    [TestCase("cid", TestName="CID")]
    public Task UInt32(string dataTypeName)
        => AssertType(8u, "8", dataTypeName, dataTypeInference: false);

    [Test]
    [TestCase("xid8", TestName="XID8")]
    public async Task UInt64(string dataTypeName)
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "13.0", "The xid8 type was introduced in PostgreSQL 13");

        await AssertType(8ul, "8", dataTypeName, dataTypeInference: false);
    }

    [Test]
    public async Task Int64()
    {
        await AssertType(8L, "8", "bigint", dbType: DbType.Int64);

        await AssertType((short)8, "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int64, DbType.Int16), valueTypeEqualsFieldType: false);
        await AssertType(8,        "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int64, DbType.Int32), valueTypeEqualsFieldType: false);
        await AssertType((byte)8,  "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int64, DbType.Int16), valueTypeEqualsFieldType: false, skipArrayCheck: true); // byte[] maps to bytea
        await AssertType((sbyte)8, "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int64, DbType.Int16), valueTypeEqualsFieldType: false);
        await AssertType(8F,       "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int64, DbType.Single), valueTypeEqualsFieldType: false);
        await AssertType(8D,       "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
            dbType: new(DbType.Int64, DbType.Double), valueTypeEqualsFieldType: false);
        await AssertType(8M,       "8", "bigint", dataTypeInference: DataTypeInference.Mismatch,
                dbType: new(DbType.Int64, DbType.Decimal), valueTypeEqualsFieldType: false);
    }

    [Test]
    [TestCase(4.123456789012345, "4.123456789012345", TestName = "Double")]
    [TestCase(double.NaN, "NaN", TestName = "Double_NaN")]
    [TestCase(double.PositiveInfinity, "Infinity", TestName = "Double_PositiveInfinity")]
    [TestCase(double.NegativeInfinity, "-Infinity", TestName = "Double_NegativeInfinity")]
    public async Task Double(double value, string sqlLiteral)
    {
        await using var conn = await OpenConnectionAsync();
        MinimumPgVersion(conn, "12.0");

        await AssertType(value, sqlLiteral, "double precision", dbType: DbType.Double);
    }

    [Test]
    [TestCase(0.123456F, "0.123456", TestName = "Float")]
    [TestCase(float.NaN, "NaN", TestName = "Float_NaN")]
    [TestCase(float.PositiveInfinity, "Infinity", TestName = "Float_PositiveInfinity")]
    [TestCase(float.NegativeInfinity, "-Infinity", TestName = "Float_NegativeInfinity")]
    public Task Float(float value, string sqlLiteral)
        => AssertType(value, sqlLiteral, "real", dbType: DbType.Single);

    [Test]
    [TestCase(short.MaxValue + 1, "smallint")]
    [TestCase(int.MaxValue + 1L, "integer")]
    [TestCase(long.MaxValue + 1D, "bigint")]
    public Task Write_overflow<T>(T value, string dataTypeName)
        => AssertTypeUnsupportedWrite<T, OverflowException>(value, dataTypeName);

    [Test]
    [TestCase((short)0, short.MaxValue + 1D, "int")]
    [TestCase(0, int.MaxValue + 1D, "bigint")]
    [TestCase(0L, long.MaxValue + 1D, "decimal")]
    public Task Read_overflow<T>(T _, double value, string dataTypeName)
        => AssertTypeUnsupportedRead<T, OverflowException>(value.ToString(CultureInfo.InvariantCulture), dataTypeName);
}
