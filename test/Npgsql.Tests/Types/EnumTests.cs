using System.Data;
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

    [Test]
    public Task SByte_enum()
        => AssertType(SByteEnum.B, "100", "smallint", dataTypeInference: DataTypeInference.Nothing,
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

    // Nullable scalar tests skip the auto-derived array check: Nullable<TEnum>[] is layout-identical to
    // Nullable<TUnderlying>[] but the CLR's enum-array covariance doesn't extend to Nullable<>[], so the
    // ArrayConverter's exact-type assertion can't reinterpret IntEnum?[] as int?[] at runtime. The scalar
    // path is what's contracted here; nullable-enum-array support is a separate generalization.
    [Test]
    public Task Nullable_int_enum()
        => AssertType<IntEnum?>(IntEnum.FortyTwo, "42", "integer", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int32, DbType.Object), valueTypeEqualsFieldType: false, skipArrayCheck: true);

    [Test]
    public Task Nullable_short_enum()
        => AssertType<ShortEnum?>(ShortEnum.B, "2", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false, skipArrayCheck: true);

    [Test]
    public Task Nullable_long_enum()
        => AssertType<LongEnum?>(LongEnum.Big, "100000000000", "bigint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int64, DbType.Object), valueTypeEqualsFieldType: false, skipArrayCheck: true);

    [Test]
    public Task Nullable_byte_enum()
        => AssertType<ByteEnum?>(ByteEnum.X, "255", "smallint", dataTypeInference: DataTypeInference.Nothing,
            dbType: new DbTypes(DbType.Int16, DbType.Object), valueTypeEqualsFieldType: false, skipArrayCheck: true);

    [Test]
    public Task Enum_rejects_non_canonical_column()
        // The converter is fixed to the underlying's canonical wire format (int → integer); cross-converting to a
        // mismatched PG type would corrupt the wire. Both scalar and array paths reject it.
        => AssertTypeUnsupported(IntEnum.FortyTwo, "42", "bigint");
}
