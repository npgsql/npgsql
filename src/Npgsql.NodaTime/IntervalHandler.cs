using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<Period>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<Period> Create(NpgsqlConnection conn)
            => new IntervalHandler(conn.HasIntegerDateTimes);
    }

    class IntervalHandler : NpgsqlSimpleTypeHandler<Period>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Some PostgreSQL-like databases (e.g. CrateDB) use floating-point representation by default and do not 
        /// provide the option of switching to integer format.
        /// </summary>
        readonly bool _integerFormat;

        public IntervalHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        public override Period Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var microsecondsInDay = _integerFormat ? buf.ReadInt64() : (long)(buf.ReadDouble() * 1000000);
            var days = buf.ReadInt32();
            var totalMonths = buf.ReadInt32();

            // Nodatime will normalize most things (i.e. nanoseconds to milliseconds, seconds...)
            // but it will not normalize months to years.
            var months = totalMonths % 12;
            var years = totalMonths / 12;

            return new PeriodBuilder
            {
                Nanoseconds = microsecondsInDay * 1000,
                Days = days,
                Months = months,
                Years = years
            }.Build().Normalize();
        }

        public override int ValidateAndGetLength(Period value, NpgsqlParameter parameter)
            => 16;

        public override void Write(Period value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var microsecondsInDay =
                (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) * NodaConstants.MillisecondsPerSecond + value.Milliseconds) * 1000 +
                value.Nanoseconds / 1000;  // Take the microseconds, discard the nanosecond remainder

            if (_integerFormat)
                buf.WriteInt64(microsecondsInDay);
            else
                buf.WriteDouble(microsecondsInDay / 1000000d);

            buf.WriteInt32(value.Weeks * 7 + value.Days);     // days
            buf.WriteInt32(value.Years * 12 + value.Months);  // months
        }
    }
}
