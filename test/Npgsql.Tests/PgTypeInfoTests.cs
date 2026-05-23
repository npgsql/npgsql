using System;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using NUnit.Framework;

namespace Npgsql.Tests;

public class PgTypeInfoTests
{
    enum IntEnum { Value = 42 }

    [Test]
    public void Enum_underlying_pair_cannot_enable_direction_without_marker(
        [Values] bool nullable, [Values] bool dualFormat, [Values] bool reading)
        => Assert.Throws<InvalidOperationException>(() => CreateEnumInfo(nullable, dualFormat, marked: false,
            supportsReading: reading, supportsWriting: !reading));

    [Test]
    public void Enum_underlying_pair_without_marker_defaults_to_unsupported(
        [Values] bool nullable, [Values] bool dualFormat, [Values(null, false)] bool? supported)
    {
        var info = CreateEnumInfo(nullable, dualFormat, marked: false,
            supportsReading: supported, supportsWriting: supported);

        Assert.That(info.SupportsReading, Is.False);
        Assert.That(info.SupportsWriting, Is.False);
    }

    [Test]
    public void Enum_underlying_marker_preserves_defaults_and_direction_overrides(
        [Values] bool nullable, [Values] bool dualFormat,
        [Values(null, false, true)] bool? supportsReading, [Values(null, false, true)] bool? supportsWriting)
    {
        var info = CreateEnumInfo(nullable, dualFormat, marked: true, supportsReading, supportsWriting);

        Assert.That(info.SupportsReading, Is.EqualTo(supportsReading ?? true));
        Assert.That(info.SupportsWriting, Is.EqualTo(supportsWriting ?? true));
    }

    static PgConcreteTypeInfo CreateEnumInfo(
        bool nullable, bool dualFormat, bool marked, bool? supportsReading, bool? supportsWriting)
    {
        var options = new PgSerializerOptions(PostgresMinimalDatabaseInfo.DefaultTypeCatalog);
        PgConverter converter = marked
            ? nullable
                ? new EnumUnderlyingNullableConverter<int>(typeof(IntEnum))
                : new EnumUnderlyingConverter<int>(typeof(IntEnum))
            : nullable
                ? new NullableConverter<int>(new Int4Converter<int>())
                : new Int4Converter<int>();
        var requestedType = nullable ? typeof(IntEnum?) : typeof(IntEnum);

        return dualFormat
            ? PgConcreteTypeInfo.Create(options, converter, converter, DataTypeNames.Int4,
                requestedType: requestedType, supportsReading: supportsReading, supportsWriting: supportsWriting)
            : PgConcreteTypeInfo.Create(options, converter, DataTypeNames.Int4,
                requestedType: requestedType, supportsReading: supportsReading, supportsWriting: supportsWriting);
    }
}
