using NodaTime;
using Npgsql.Internal;

namespace Npgsql.NodaTime.Internal;

sealed class OffsetTimeConverter : PgBufferedConverter<OffsetTime>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long) + sizeof(int)) };

    // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
    public override OffsetTime Read(PgReader reader)
        => new(LocalTime.FromTicksSinceMidnight(reader.ReadInt64() * 10), Offset.FromSeconds(-reader.ReadInt32()));

    public override void Write(PgWriter writer, OffsetTime value)
    {
        writer.WriteInt64(value.TickOfDay / 10);
        writer.WriteInt32(-(int)(value.Offset.Ticks / NodaConstants.TicksPerSecond));
    }
}
