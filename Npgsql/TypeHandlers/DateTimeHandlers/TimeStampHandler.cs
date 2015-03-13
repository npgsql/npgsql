using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timestamp", NpgsqlDbType.Timestamp, new[] { DbType.DateTime, DbType.DateTime2 }, new [] { typeof(NpgsqlTimeStamp), typeof(DateTime) })]
    internal class TimeStampHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStamp>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlTimeStamp>, ISimpleTypeWriter
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public TimeStampHandler(TypeHandlerRegistry registry)
        {
            _integerFormat = registry.Connector.BackendParams["integer_datetimes"] == "on";
        }

        public DateTime Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ISimpleTypeReader<NpgsqlTimeStamp>)this).Read(buf, len, fieldDescription);
        }

        NpgsqlTimeStamp ISimpleTypeReader<NpgsqlTimeStamp>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            if (!_integerFormat) {
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
            }
            return NpgsqlTimeStamp.FromInt64(buf.ReadInt64());
        }
               
        public int ValidateAndGetLength(object value)
        {
            return 8;
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            NpgsqlTimeStamp ts;
            if (value is NpgsqlTimeStamp) {
                ts = (NpgsqlTimeStamp)value;
            }
            else if (value is DateTime)
            {
                var dtValue = (DateTime)value;
                var datePart = new NpgsqlDate(dtValue);
                var timePart = new NpgsqlTime(dtValue.Hour, dtValue.Minute, dtValue.Second, dtValue.Millisecond * 1000);
                ts = new NpgsqlTimeStamp(datePart, timePart);
            }
            else if (value is string)
            {
                // TODO: Decide what to do with this
                throw new InvalidOperationException("String DateTimes are not allowed, use DateTime instead.");
            }
            else
            {
                throw new InvalidCastException();
            }

            var uSecsTime = ts.Time.Hours * 3600000000L + ts.Time.Minutes * 60000000L + ts.Time.Seconds * 1000000L + ts.Time.Microseconds;

            if (ts >= new NpgsqlTimeStamp(2000, 1, 1, 0, 0, 0))
            {
                var uSecsDate = (ts.Date.DaysSinceEra - 730119) * 86400000000L;
                buf.WriteInt64(uSecsDate + uSecsTime);
            }
            else
            {
                var uSecsDate = (730119 - ts.Date.DaysSinceEra) * 86400000000L;
                buf.WriteInt64(-(uSecsDate - uSecsTime));
            }
        }
    }
}
