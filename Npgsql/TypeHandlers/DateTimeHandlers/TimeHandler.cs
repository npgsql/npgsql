using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("time", NpgsqlDbType.Time, DbType.Time)]
    internal class TimeHandler : TypeHandlerWithPsv<DateTime, NpgsqlTime>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlTime>, ISimpleTypeWriter
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public TimeHandler(TypeHandlerRegistry registry)
        {
            _integerFormat = registry.Connector.BackendParams["integer_datetimes"] == "on";
        }

        public DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTime?
            return (DateTime)((ISimpleTypeReader<NpgsqlTime>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTime ISimpleTypeReader<NpgsqlTime>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            if (!_integerFormat) {
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
            }

            // Postgresql time resolution == 1 microsecond == 10 ticks
            return new NpgsqlTime(buf.ReadInt64() * 10);
        }

        public int ValidateAndGetLength(object value) { return 8; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            NpgsqlTime time;
            if (value is DateTime)
            {
                time = new NpgsqlTime(((DateTime)value).TimeOfDay);
            }
            else if (value is TimeSpan)
            {
                time = new NpgsqlTime((TimeSpan)value);
            }
            else
            {
                time = (NpgsqlTime)value;
            }
            buf.WriteInt64(time.Ticks / 10); // TODO: round?
        }
    }
}
