using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;

namespace Npgsql.NodaTime.Internal;

sealed class LocalDateConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<LocalDate>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override LocalDate ReadCore(PgReader reader)
        => reader.ReadInt32() switch
        {
            int.MaxValue => dateTimeInfinityConversions
                ? LocalDate.MaxIsoValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue),
            int.MinValue => dateTimeInfinityConversions
                ? LocalDate.MinIsoValue
                : throw new InvalidCastException(NpgsqlNodaTimeStrings.CannotReadInfinityValue),
            var value => new LocalDate().PlusDays(value + 730119)
        };

    protected override void WriteCore(PgWriter writer, LocalDate value)
    {
        if (dateTimeInfinityConversions)
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
