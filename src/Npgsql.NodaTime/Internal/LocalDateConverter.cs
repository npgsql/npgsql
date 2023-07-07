using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;

namespace Npgsql.NodaTime.Internal;

sealed class LocalDateConverter : PgBufferedConverter<LocalDate>
{
    readonly bool _dateTimeInfinityConversions;

    public LocalDateConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out bufferingRequirement);
    }

    public override Size GetSize(SizeContext context, LocalDate value, ref object? writeState) => sizeof(int);

    protected override LocalDate ReadCore(PgReader reader)
        => reader.ReadInt32() switch
        {
            int.MaxValue => _dateTimeInfinityConversions
                ? LocalDate.MaxIsoValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue),
            int.MinValue => _dateTimeInfinityConversions
                ? LocalDate.MinIsoValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue),
            var value => new LocalDate().PlusDays(value + 730119)
        };

    protected override void WriteCore(PgWriter writer, LocalDate value)
    {
        if (_dateTimeInfinityConversions)
        {
            if (value == LocalDate.MaxIsoValue)
            {
                writer.WriteInt32(int.MaxValue);
                return;
            }
            if (value == LocalDate.MinIsoValue)
            {
                writer.WriteInt32(int.MinValue);
                return;
            }
        }

        var totalDaysSinceEra = Period.Between(default, value, PeriodUnits.Days).Days;
        writer.WriteInt32(totalDaysSinceEra - 730119);
    }
}
