using System;
using Npgsql.Properties;

namespace Npgsql.Internal.Converters;

sealed class DateTimeDateConverter : PgBufferedConverter<DateTime>
{
    readonly bool _dateTimeInfinityConversions;

    static readonly DateTime BaseValue = new(2000, 1, 1, 0, 0, 0);

    public DateTimeDateConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }

    public override Size GetSize(SizeContext context, DateTime value, ref object? writeState)
        => sizeof(int);

    protected override DateTime ReadCore(PgReader reader)
        => reader.ReadInt32() switch
        {
            int.MaxValue => !_dateTimeInfinityConversions
                ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                : DateTime.MaxValue,
            int.MinValue => !_dateTimeInfinityConversions
                ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                : DateTime.MinValue,
            var value => BaseValue + TimeSpan.FromDays(value)
        };

    protected override void WriteCore(PgWriter writer, DateTime value)
    {
        if (_dateTimeInfinityConversions)
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

#if NET6_0_OR_GREATER
sealed class DateOnlyDateConverter : PgBufferedConverter<DateOnly>
{
    readonly bool _dateTimeInfinityConversions;

    static readonly DateOnly BaseValue = new(2000, 1, 1);

    public DateOnlyDateConverter(bool dateTimeInfinityConversions)
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    protected override DateOnly ReadCore(PgReader reader)
        => reader.ReadInt32() switch
        {
            int.MaxValue => !_dateTimeInfinityConversions
                ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                : DateOnly.MaxValue,
            int.MinValue => !_dateTimeInfinityConversions
                ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                : DateOnly.MinValue,
            var value => BaseValue.AddDays(value)
        };

    public override Size GetSize(SizeContext context, DateOnly value, ref object? writeState)
        => sizeof(int);

    protected override void WriteCore(PgWriter writer, DateOnly value)
    {
        if (_dateTimeInfinityConversions)
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
#endif
