using System;
using Npgsql.Internal.Converters.Types;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class DateTimeConverter : PgBufferedConverter<DateTime>
{
    readonly bool _dateTimeInfinityConversions;
    readonly DateTimeKind _kind;

    public DateTimeConverter(bool dateTimeInfinityConversions, DateTimeKind kind)
    {
        _dateTimeInfinityConversions = dateTimeInfinityConversions;
        _kind = kind;
    }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
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
        => _dateTimeInfinityConversions = dateTimeInfinityConversions;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long));
        return format is DataFormat.Binary;
    }

    protected override DateTimeOffset ReadCore(PgReader reader)
        => PgTimestamp.Decode(reader.ReadInt64(), DateTimeKind.Utc, _dateTimeInfinityConversions);

    protected override void WriteCore(PgWriter writer, DateTimeOffset value)
    {
        // TODO move back to a resolver.
        if (value.Offset != TimeSpan.Zero)
            throw new ArgumentException($"Cannot write DateTimeOffset with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', only offset 0 (UTC) is supported. ", nameof(value));

        writer.WriteInt64(PgTimestamp.Encode(value.DateTime, _dateTimeInfinityConversions));

    }
}
