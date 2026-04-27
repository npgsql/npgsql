using System;
using Npgsql.Properties;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateTimeConverter : PgBufferedConverter<DateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly DateTimeKind _kind;

    // Kind validation runs in the size path so the GetSize-skip optimization is bypassed.
    // Bind-time enforcement replaces the prior provider-level kind throw, allowing decided-pgTypeId callers
    // to erase the provider entirely.
    public DateTimeConverter(bool dateTimeInfinityConversions, DateTimeKind kind)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _kind = kind;
        HandleFixedSizeBind = true;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override Size GetSize(SizeContext context, DateTime value, ref object? writeState)
    {
        if (_kind is DateTimeKind.Utc)
        {
            // timestamptz: accept Utc, plus Unspecified MinValue/MaxValue when infinity conversions are on.
            if (value.Kind is not DateTimeKind.Utc
                && !(_dateTimeInfinityConversions && (value == DateTime.MinValue || value == DateTime.MaxValue)))
            {
                throw new ArgumentException(string.Format(NpgsqlStrings.TimestampTzNoDateTimeUnspecified, value.Kind), nameof(value));
            }
        }
        else if (value.Kind is DateTimeKind.Utc)
        {
            // timestamp: reject Utc.
            throw new ArgumentException(NpgsqlStrings.TimestampNoDateTimeUtc, nameof(value));
        }

        return context.BufferRequirement;
    }

    protected override DateTime ReadCore(PgReader reader)
        => PgTimestamp.Decode(reader.ReadInt64(), _kind, _dateTimeInfinityConversions);

    protected override void WriteCore(PgWriter writer, DateTime value)
        => writer.WriteInt64(PgTimestamp.Encode(value, _dateTimeInfinityConversions));
}

sealed class DateTimeOffsetConverter : PgBufferedConverter<DateTimeOffset>
{
    readonly bool _dateTimeInfinityConversions;

    public DateTimeOffsetConverter(bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        HandleFixedSizeBind = true;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override Size GetSize(SizeContext context, DateTimeOffset value, ref object? writeState)
    {
        if (value.Offset != TimeSpan.Zero)
            throw new ArgumentException($"Cannot write DateTimeOffset with Offset={value.Offset}, only offset 0 (UTC) is supported.", nameof(value));

        return context.BufferRequirement;
    }

    protected override DateTimeOffset ReadCore(PgReader reader)
        => new(PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, _dateTimeInfinityConversions), TimeSpan.Zero);

    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
        => writer.WriteInt64(PgTimestamp.Encode(value.DateTime, _dateTimeInfinityConversions));
}
