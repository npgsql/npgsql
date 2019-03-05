using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimeTzHandlerFactory : NpgsqlTypeHandlerFactory<OffsetTime>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<OffsetTime> Create(NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new TimeTzHandler()
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    class TimeTzHandler : NpgsqlSimpleTypeHandler<OffsetTime>
    {
        // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
        public override OffsetTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new OffsetTime(
                LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10),
                Offset.FromSeconds(-buf.ReadInt32()));

        public override int ValidateAndGetLength(OffsetTime value, NpgsqlParameter parameter) => 12;

        public override void Write(OffsetTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteInt64(value.TickOfDay / 10);
            buf.WriteInt32(-(int)(value.Offset.Ticks / NodaConstants.TicksPerSecond));
        }
    }
}
