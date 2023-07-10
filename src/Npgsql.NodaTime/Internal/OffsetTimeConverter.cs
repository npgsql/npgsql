using NodaTime;
using Npgsql.Internal;

namespace Npgsql.NodaTime.Internal;

sealed class OffsetTimeConverter : PgBufferedConverter<OffsetTime>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int));
        return format is DataFormat.Binary;
    }

    // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
    protected override OffsetTime ReadCore(PgReader reader)
        => new(LocalTime.FromTicksSinceMidnight(reader.ReadInt64() * 10), Offset.FromSeconds(-reader.ReadInt32()));

    protected override void WriteCore(PgWriter writer, OffsetTime value)
    {
        writer.WriteInt64(value.TickOfDay / 10);
        writer.WriteInt32(-(int)(value.Offset.Ticks / NodaConstants.TicksPerSecond));
    }
}
