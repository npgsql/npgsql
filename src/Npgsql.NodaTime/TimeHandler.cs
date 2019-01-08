using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimeHandlerFactory : NpgsqlTypeHandlerFactory<LocalTime>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<LocalTime> Create(NpgsqlConnection conn)
            => new TimeHandler(conn.HasIntegerDateTimes);
    }

    class TimeHandler : NpgsqlSimpleTypeHandler<LocalTime>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Some PostgreSQL-like databases (e.g. CrateDB) use floating-point representation by default and do not 
        /// provide the option of switching to integer format.
        /// </summary>
        readonly bool _integerFormat;

        public TimeHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        public override LocalTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (_integerFormat)
                // PostgreSQL time resolution == 1 microsecond == 10 ticks
                return LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10);
            else
                return LocalTime.FromTicksSinceMidnight((long)(buf.ReadDouble() * NodaConstants.TicksPerSecond));
        }

        public override int ValidateAndGetLength(LocalTime value, NpgsqlParameter parameter)
            => 8;

        public override void Write(LocalTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.TickOfDay / 10);
            else
                buf.WriteDouble((double)value.TickOfDay / NodaConstants.TicksPerSecond);
        }
    }
}
