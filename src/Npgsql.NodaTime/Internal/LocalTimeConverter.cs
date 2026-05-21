using NodaTime;
using Npgsql.Internal;

namespace Npgsql.NodaTime.Internal;

sealed class LocalTimeConverter : PgBufferedConverter<LocalTime>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(long)) };

    // PostgreSQL time resolution == 1 microsecond == 10 ticks
    public override LocalTime Read(PgReader reader)
        => LocalTime.FromTicksSinceMidnight(reader.ReadInt64() * 10);

    public override void Write(PgWriter writer, LocalTime value)
        => writer.WriteInt64(value.TickOfDay / 10);
}
