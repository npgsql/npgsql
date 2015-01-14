using System;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("interval", NpgsqlDbType.Interval, typeof(TimeSpan))]
    internal class IntervalHandler : TypeHandlerWithPsv<TimeSpan, NpgsqlInterval>,
        ISimpleTypeReader<TimeSpan>, ISimpleTypeReader<NpgsqlInterval>, ISimpleTypeWriter
    {
        public TimeSpan Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (TimeSpan)((ISimpleTypeReader<NpgsqlInterval>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlInterval ISimpleTypeReader<NpgsqlInterval>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var ticks = buf.ReadInt64();
            var day = buf.ReadInt32();
            var month = buf.ReadInt32();
            return new NpgsqlInterval(month, day, ticks * 10);
        }

        public int GetLength(object value) { return 16; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var interval = (value is TimeSpan)
                ? ((NpgsqlInterval)(TimeSpan)value)
                : ((NpgsqlInterval)value);

            buf.WriteInt64(interval.Ticks / 10); // TODO: round?
            buf.WriteInt32(interval.Days);
            buf.WriteInt32(interval.Months);
        }
    }
}
