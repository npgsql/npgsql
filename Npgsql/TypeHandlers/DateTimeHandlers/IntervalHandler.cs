using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("interval", NpgsqlDbType.Interval, typeof(TimeSpan))]
    internal class IntervalHandler : TypeHandlerWithPsv<TimeSpan, NpgsqlTimeSpan>,
        ISimpleTypeReader<TimeSpan>, ISimpleTypeReader<NpgsqlTimeSpan>, ISimpleTypeWriter
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public IntervalHandler(TypeHandlerRegistry registry)
        {
            _integerFormat = registry.Connector.BackendParams["integer_datetimes"] == "on";
        }

        public TimeSpan Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (TimeSpan)((ISimpleTypeReader<NpgsqlTimeSpan>)this).Read(buf, len, fieldDescription);
        }

        NpgsqlTimeSpan ISimpleTypeReader<NpgsqlTimeSpan>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            if (!_integerFormat) {
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
            }
            var ticks = buf.ReadInt64();
            var day = buf.ReadInt32();
            var month = buf.ReadInt32();
            return new NpgsqlTimeSpan(month, day, ticks * 10);
        }

        public int ValidateAndGetLength(object value)
        {
            if (!_integerFormat) {
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
            }

            return 16;
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var interval = (value is TimeSpan)
                ? ((NpgsqlTimeSpan)(TimeSpan)value)
                : ((NpgsqlTimeSpan)value);

            buf.WriteInt64(interval.Ticks / 10); // TODO: round?
            buf.WriteInt32(interval.Days);
            buf.WriteInt32(interval.Months);
        }
    }
}
