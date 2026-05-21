using System;
using NodaTime;
using Npgsql.Internal;
using Npgsql.NodaTime.Properties;

namespace Npgsql.NodaTime.Internal;

sealed class LocalDateConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<LocalDate>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int)) };

    public override LocalDate Read(PgReader reader)
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

    public override void Write(PgWriter writer, LocalDate value)
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
