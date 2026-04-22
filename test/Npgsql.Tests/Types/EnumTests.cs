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

    [Test]
    public Task Int_enum()
        => AssertType(IntEnum.FortyTwo, "42", "integer", dbType: DbType.Int32, valueTypeEqualsFieldType: false);

    [Test]
    public Task Short_enum()
        => AssertType(ShortEnum.B, "2", "smallint", dbType: DbType.Int16, valueTypeEqualsFieldType: false);

    [Test]
    public Task Long_enum()
        => AssertType(LongEnum.Big, "100000000000", "bigint", dbType: DbType.Int64, valueTypeEqualsFieldType: false);

    [Test]
    public Task Byte_enum()
        => AssertType(ByteEnum.X, "255", "smallint", dbType: DbType.Int16, valueTypeEqualsFieldType: false);
}
