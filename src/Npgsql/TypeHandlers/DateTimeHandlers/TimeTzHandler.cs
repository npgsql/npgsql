using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timetz", NpgsqlDbType.TimeTZ)]
    internal class TimeTzHandler : TypeHandler<DateTimeOffset>,
        ISimpleTypeReader<DateTimeOffset>, ISimpleTypeReader<DateTime>, ISimpleTypeReader<TimeSpan>,
        ISimpleTypeWriter
    {
        // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative

        DateTimeOffset ISimpleTypeReader<DateTimeOffset>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var ticks = buf.ReadInt64() * 10;
            var offset = new TimeSpan(0, 0, -buf.ReadInt32());
            return new DateTimeOffset(ticks, offset);
        }

        DateTime ISimpleTypeReader<DateTime>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read<DateTimeOffset>(buf, len, fieldDescription).LocalDateTime;
        }

        TimeSpan ISimpleTypeReader<TimeSpan>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read<DateTimeOffset>(buf, len, fieldDescription).LocalDateTime.TimeOfDay;
        }

        public int ValidateAndGetLength(object value)
        {
            return 12;
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            if (value is DateTimeOffset)
            {
                var dto = (DateTimeOffset) value;
                buf.WriteInt64(dto.TimeOfDay.Ticks / 10);
                buf.WriteInt32(-(int)(dto.Offset.Ticks / TimeSpan.TicksPerSecond));
                return;
            }

            if (value is DateTime)
            {
                var dt = (DateTime) value;

                buf.WriteInt64(dt.TimeOfDay.Ticks / 10);

                switch (dt.Kind)
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
                    throw PGUtil.ThrowIfReached();
                }

                return;
            }

            if (value is TimeSpan)
            {
                var ts = (TimeSpan)value;
                buf.WriteInt64(ts.Ticks / 10);
                buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
                return;
            }

            throw new InvalidCastException();
        }
    }
}
