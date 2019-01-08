using System;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("interval", NpgsqlDbType.Interval, new[] { typeof(TimeSpan), typeof(NpgsqlTimeSpan) })]
    class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<TimeSpan> Create(NpgsqlConnection conn)
            => new IntervalHandler(conn.HasIntegerDateTimes);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class IntervalHandler : NpgsqlSimpleTypeHandlerWithPsv<TimeSpan, NpgsqlTimeSpan>
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

        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (TimeSpan)((INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>)this).Read(buf, len, fieldDescription);

        protected override NpgsqlTimeSpan ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (_integerFormat)
            {
                var ticks = buf.ReadInt64();
                var day = buf.ReadInt32();
                var month = buf.ReadInt32();
                return new NpgsqlTimeSpan(month, day, ticks * 10);
            }
            else
            {
                var seconds = buf.ReadDouble();
                var day = buf.ReadInt32();
                var month = buf.ReadInt32();
                return new NpgsqlTimeSpan(month, day, (long)(seconds * TimeSpan.TicksPerSecond));
            }
        }

        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 16;

        public override int ValidateAndGetLength(NpgsqlTimeSpan value, NpgsqlParameter parameter)
            => 16;

        public override void Write(NpgsqlTimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.Ticks / 10); // TODO: round?
            else
                buf.WriteDouble(value.TotalSeconds - (value.Days * 86400) - (value.Months * NpgsqlTimeSpan.DaysPerMonth * 86400));

            buf.WriteInt32(value.Days);
            buf.WriteInt32(value.Months);
        }

        // TODO: Can write directly from TimeSpan
        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write(value, buf, parameter);
    }
}
