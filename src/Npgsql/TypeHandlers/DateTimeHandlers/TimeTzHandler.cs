using System;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("time with time zone", NpgsqlDbType.TimeTz)]
    class TimeTzHandlerFactory : NpgsqlTypeHandlerFactory<DateTimeOffset>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<DateTimeOffset> Create(NpgsqlConnection conn)
            => new TimeTzHandler(conn.HasIntegerDateTimes);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimeTzHandler : NpgsqlSimpleTypeHandler<DateTimeOffset>, INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<TimeSpan>
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

        // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative

        #region Read

        public override DateTimeOffset Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var ticks = _integerFormat ? buf.ReadInt64() * 10 : (long)(buf.ReadDouble() * TimeSpan.TicksPerSecond);
            var offset = new TimeSpan(0, 0, -buf.ReadInt32());
            return new DateTimeOffset(ticks + TimeSpan.TicksPerDay, offset);
        }

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).LocalDateTime;

        TimeSpan INpgsqlSimpleTypeHandler<TimeSpan>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).LocalDateTime.TimeOfDay;

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter parameter)
            => 12;
        public int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 12;
        public int ValidateAndGetLength(DateTime value, NpgsqlParameter parameter)
            => 12;

        public override void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.TimeOfDay.Ticks / 10);
            else
                buf.WriteDouble((double)value.TimeOfDay.Ticks / TimeSpan.TicksPerSecond);

            buf.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
        }

        public void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.TimeOfDay.Ticks / 10);
            else
                buf.WriteDouble((double)value.TimeOfDay.Ticks / TimeSpan.TicksPerSecond);

            switch (value.Kind)
            {
            case DateTimeKind.Utc:
                buf.WriteInt32(0);
                break;
            case DateTimeKind.Unspecified:
            // Treat as local...
            case DateTimeKind.Local:
                buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {value.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
            }
        }

        public void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.Ticks / 10);
            else
                buf.WriteDouble((double)value.Ticks / TimeSpan.TicksPerSecond);

            buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
        }

        #endregion Write
    }
}
