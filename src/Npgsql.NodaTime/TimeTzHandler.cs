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
            => new TimeTzHandler(conn.HasIntegerDateTimes);
    }

    class TimeTzHandler : NpgsqlSimpleTypeHandler<OffsetTime>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Some PostgreSQL-like databases (e.g. CrateDB) use floating-point representation by default and do not
        /// provide the option of switching to integer format.
        /// </summary>
        readonly bool _integerFormat;

        public TimeTzHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
        public override OffsetTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new OffsetTime(_integerFormat
                ? LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10)
                : LocalTime.FromTicksSinceMidnight((long)(buf.ReadDouble() * NodaConstants.TicksPerSecond)),
                Offset.FromSeconds(-buf.ReadInt32()));

        public override int ValidateAndGetLength(OffsetTime value, NpgsqlParameter parameter) => 12;

        public override void Write(OffsetTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.TickOfDay / 10);
            else
                buf.WriteDouble((double)value.TickOfDay / NodaConstants.TicksPerSecond);
            buf.WriteInt32(-(int)(value.Offset.Ticks / NodaConstants.TicksPerSecond));
        }
    }
}
