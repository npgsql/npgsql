using System;
using Npgsql.Properties;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateTimeConverter : PgBufferedConverter<DateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly DateTimeKind _kind;

    // Kind validation runs in BindValue. Bind-time enforcement replaces the prior provider-level kind throw,
    // allowing decided-pgTypeId callers to erase the provider entirely.
    public DateTimeConverter(bool dateTimeInfinityConversions, DateTimeKind kind)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _kind = kind;
    }

    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        // optionalBind=false opts the fixed-size requirement out of the bind-skip optimization so kind validation fires.
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long), optionalBind: false) };

    protected override Size BindValue(in BindContext context, DateTime value, ref object? writeState)
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

    public override DateTime Read(PgReader reader)
        => PgTimestamp.Decode(reader.ReadInt64(), _kind, _dateTimeInfinityConversions);

    public override void Write(PgWriter writer, DateTime value)
        => writer.WriteInt64(PgTimestamp.Encode(value, _dateTimeInfinityConversions));
}

sealed class DateTimeOffsetConverter : PgBufferedConverter<DateTimeOffset>
{
    readonly bool _dateTimeInfinityConversions;

    public DateTimeOffsetConverter(bool dateTimeInfinityConversions)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
    }

    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        // optionalBind=false opts the fixed-size requirement out of the bind-skip optimization so offset validation fires.
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long), optionalBind: false) };

    protected override Size BindValue(in BindContext context, DateTimeOffset value, ref object? writeState)
    {
        if (value.Offset != TimeSpan.Zero)
            throw new ArgumentException($"Cannot write DateTimeOffset with Offset={value.Offset}, only offset 0 (UTC) is supported.", nameof(value));

        return context.BufferRequirement;
    }

    public override DateTimeOffset Read(PgReader reader)
        => new(PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, _dateTimeInfinityConversions), TimeSpan.Zero);

    public override void Write(PgWriter writer, DateTimeOffset value)
        => writer.WriteInt64(PgTimestamp.Encode(value.DateTime, _dateTimeInfinityConversions));
}
