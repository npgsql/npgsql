using System;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("time", NpgsqlDbType.Time, DbType.Time)]
    internal class TimeHandler : TypeHandlerWithPsv<DateTime, NpgsqlTime>, ITypeHandler<NpgsqlTime>
    {
        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTime?
            return (DateTime)((ITypeHandler<NpgsqlTime>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTime ITypeHandler<NpgsqlTime>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTime.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    // Postgresql time resolution == 1 microsecond == 10 ticks
                    return new NpgsqlTime(buf.ReadInt64() * 10);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        internal override int Length(object value)
        {
            return 8;
        }

        internal override void WriteBinary(object value, NpgsqlBuffer buf)
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
