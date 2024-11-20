using System;
using Npgsql.Properties;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateOnlyDateConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<DateOnly>
{
    static readonly DateOnly BaseValue = new(2000, 1, 1);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override DateOnly ReadCore(PgReader reader)
        => reader.ReadInt32() switch
        {
            int.MaxValue => dateTimeInfinityConversions
                ? DateOnly.MaxValue
                : throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue),
            int.MinValue => dateTimeInfinityConversions
                ? DateOnly.MinValue
                : throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue),
            var value => BaseValue.AddDays(value)
        };

    protected override void WriteCore(PgWriter writer, DateOnly value)
    {
        if (dateTimeInfinityConversions)
        {
            if (value == DateOnly.MaxValue)
            {
                writer.WriteInt32(int.MaxValue);
                return;
            }

            if (value == DateOnly.MinValue)
            {
                writer.WriteInt32(int.MinValue);
                return;
            }
        }

        writer.WriteInt32(value.DayNumber - BaseValue.DayNumber);
    }
}

sealed class DateTimeDateConverter(bool dateTimeInfinityConversions) : PgBufferedConverter<DateTime>
{
    static readonly DateTime BaseValue = new(2000, 1, 1, 0, 0, 0);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(int));
        return format is DataFormat.Binary;
    }

    protected override DateTime ReadCore(PgReader reader)
        => reader.ReadInt32() switch
        {
            int.MaxValue => dateTimeInfinityConversions
                ? DateTime.MaxValue
                : throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue),
            int.MinValue => dateTimeInfinityConversions
                ? DateTime.MinValue
                : throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue),
            var value => BaseValue + TimeSpan.FromDays(value)
        };

    protected override void WriteCore(PgWriter writer, DateTime value)
    {
        if (dateTimeInfinityConversions)
        {
            if (value == DateTime.MaxValue)
            {
                writer.WriteInt32(int.MaxValue);
                return;
            }

            if (value == DateTime.MinValue)
            {
                writer.WriteInt32(int.MinValue);
                return;
            }
        }

        writer.WriteInt32((value.Date - BaseValue).Days);
    }
}
