using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests.Types;

/// <summary>
/// Tests for CLR enum types read and written through their underlying integer PG types
/// (enum underlying-type converter support).
/// </summary>
public class EnumTests : TestBase
{
    enum IntEnum { Zero = 0, One = 1, FortyTwo = 42 }
    enum ShortEnum : short { A = 1, B = 2 }
    enum LongEnum : long { Big = 100_000_000_000 }
    enum ByteEnum : byte { X = 255 }
    enum SByteEnum : sbyte { A = 1, B = 100 }
    enum UShortEnum : ushort { A = 1, B = 30000 }
    enum UIntEnum : uint { A = 1, B = 100_000 }
    enum ULongEnum : ulong { Big = 100_000_000_000 }

    [Test]
    public Task Int_enum()
        => AssertType(IntEnum.FortyTwo, "42", "integer", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int32, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Short_enum()
        => AssertType(ShortEnum.B, "2", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Long_enum()
        => AssertType(LongEnum.Big, "100000000000", "bigint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int64, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Byte_enum()
        => AssertType(ByteEnum.X, "255", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [TestCase(-128)]
    [TestCase(-1)]
    [TestCase(100)]
    [TestCase(127)]
    public Task SByte_enum(sbyte value)
        => AssertType((SByteEnum)value, value.ToString(CultureInfo.InvariantCulture), "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task UShort_enum()
        => AssertType(UShortEnum.B, "30000", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task UInt_enum()
        => AssertType(UIntEnum.B, "100000", "integer", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int32, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task ULong_enum()
        => AssertType(ULongEnum.Big, "100000000000", "bigint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int64, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_int_enum()
        => AssertType<IntEnum?>(IntEnum.FortyTwo, "42", "integer", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int32, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_short_enum()
        => AssertType<ShortEnum?>(ShortEnum.B, "2", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_long_enum()
        => AssertType<LongEnum?>(LongEnum.Big, "100000000000", "bigint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int64, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_byte_enum()
        => AssertType<ByteEnum?>(ByteEnum.X, "255", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [TestCase(-128)]
    [TestCase(-1)]
    [TestCase(127)]
    public Task Nullable_sbyte_enum(sbyte value)
        => AssertType<SByteEnum?>((SByteEnum)value, value.ToString(CultureInfo.InvariantCulture), "smallint",
            dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_ushort_enum()
        => AssertType<UShortEnum?>((UShortEnum)ushort.MaxValue, "-1", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_uint_enum()
        => AssertType<UIntEnum?>((UIntEnum)uint.MaxValue, "-1", "integer", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int32, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Nullable_ulong_enum()
        => AssertType<ULongEnum?>((ULongEnum)ulong.MaxValue, "-1", "bigint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int64, DbType.Object), valueTypeEqualsFieldType: false);

    [Test]
    public Task Sbyte_enum_rejects_out_of_range_values([Values(-129, 128, 255)] short value, [Values] bool nullable)
        => nullable
            ? AssertTypeUnsupportedRead<SByteEnum?, OverflowException>(value.ToString(CultureInfo.InvariantCulture), "smallint")
            : AssertTypeUnsupportedRead<SByteEnum, OverflowException>(value.ToString(CultureInfo.InvariantCulture), "smallint");

    [Test]
    public Task Enum_rejects_non_canonical_column()
        // The converter is fixed to the underlying's canonical wire format (int → integer); cross-converting to a
        // mismatched PG type would corrupt the wire. Both scalar and array paths reject it.
        => AssertTypeUnsupported(IntEnum.FortyTwo, "42", "bigint");

    [Test]
    public Task Int_enum_max_dim_array()
    {
        var arr = new IntEnum[1, 1, 1, 1, 1, 2];
        arr[0, 0, 0, 0, 0, 0] = IntEnum.FortyTwo;
        arr[0, 0, 0, 0, 0, 1] = IntEnum.One;
        return AssertType(arr, "{{{{{{42,1}}}}}}", "integer[]",
            dataTypeInference: DataTypeInference.Nothing, valueTypeEqualsFieldType: false);
    }

    [Test]
    public Task Nullable_int_enum_max_dim_array()
    {
        var arr = new IntEnum?[1, 1, 1, 1, 1, 2];
        arr[0, 0, 0, 0, 0, 0] = IntEnum.FortyTwo;
        arr[0, 0, 0, 0, 0, 1] = null;
        return AssertType(arr, "{{{{{{42,NULL}}}}}}", "integer[]",
            dataTypeInference: DataTypeInference.Nothing, valueTypeEqualsFieldType: false);
    }
}
