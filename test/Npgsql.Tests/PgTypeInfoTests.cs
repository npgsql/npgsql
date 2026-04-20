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
        [Values] bool dualFormat, [Values] bool reading)
        => Assert.Throws<InvalidOperationException>(() => CreateEnumInfo(dualFormat, marked: false,
            supportsReading: reading, supportsWriting: !reading));

    [Test]
    public void Enum_underlying_pair_without_marker_defaults_to_unsupported(
        [Values] bool dualFormat, [Values(null, false)] bool? supported)
    {
        var info = CreateEnumInfo(dualFormat, marked: false,
            supportsReading: supported, supportsWriting: supported);

        Assert.That(info.SupportsReading, Is.False);
        Assert.That(info.SupportsWriting, Is.False);
    }

    [Test]
    public void Enum_underlying_marker_preserves_defaults_and_direction_overrides(
        [Values] bool dualFormat,
        [Values(null, false, true)] bool? supportsReading, [Values(null, false, true)] bool? supportsWriting)
    {
        var info = CreateEnumInfo(dualFormat, marked: true, supportsReading, supportsWriting);

        Assert.That(info.SupportsReading, Is.EqualTo(supportsReading ?? true));
        Assert.That(info.SupportsWriting, Is.EqualTo(supportsWriting ?? true));
    }

    static PgConcreteTypeInfo CreateEnumInfo(
        bool dualFormat, bool marked, bool? supportsReading, bool? supportsWriting)
    {
        var options = new PgSerializerOptions(PostgresMinimalDatabaseInfo.DefaultTypeCatalog);
        PgConverter converter = marked
            ? new EnumUnderlyingConverter<int>(typeof(IntEnum))
            : new Int4Converter<int>();
        var requestedType = typeof(IntEnum);

        return dualFormat
            ? PgConcreteTypeInfo.Create(options, converter, converter, DataTypeNames.Int4,
                requestedType: requestedType, supportsReading: supportsReading, supportsWriting: supportsWriting)
            : PgConcreteTypeInfo.Create(options, converter, DataTypeNames.Int4,
                requestedType: requestedType, supportsReading: supportsReading, supportsWriting: supportsWriting);
    }
}
