using System;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timestamp", NpgsqlDbType.Timestamp, new[] { DbType.DateTime, DbType.DateTime2 }, typeof(NpgsqlTimeStamp))]
    internal class TimeStampHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStamp>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlTimeStamp>, ISimpleTypeWriter
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public TimeStampHandler(NpgsqlConnector connector)
        {
            _integerFormat = connector.BackendParams["integer_datetimes"] == "on";
        }

        public override bool SupportsBinaryWrite
        {
            get
            {
                return true;
            }
        }

        public DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ISimpleTypeReader<NpgsqlTimeStamp>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStamp ISimpleTypeReader<NpgsqlTimeStamp>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
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
            if ( value is DateTime )
            {
                var dtValue = (DateTime)value;
                var datePart = new NpgsqlDate(dtValue);
                var timePart = new NpgsqlTime(dtValue.Hour, dtValue.Minute, dtValue.Second, dtValue.Millisecond * 1000);
                value = new NpgsqlTimeStamp(datePart, timePart);
            }
            else if ( value is string )
            {
                throw new InvalidOperationException("String DateTimes are not allowed, use DateTime instead.");
            }

            var timestamp = (NpgsqlTimeStamp)value;
            var uSecsTime = timestamp.Time.Hours * 3600000000L + timestamp.Time.Minutes * 60000000L + timestamp.Time.Seconds * 1000000L + timestamp.Time.Microseconds;

            if ( timestamp >= new NpgsqlTimeStamp(2000, 1, 1, 0, 0, 0) )
            {
                var uSecsDate = ( timestamp.Date.DaysSinceEra - 730119 ) * 86400000000L;
                buf.WriteInt64(uSecsDate + uSecsTime);
            }
            else
            {
                var uSecsDate = ( 730119 - timestamp.Date.DaysSinceEra ) * 86400000000L;
                buf.WriteInt64(-( uSecsDate - uSecsTime ));
            }
        }
    }
}
