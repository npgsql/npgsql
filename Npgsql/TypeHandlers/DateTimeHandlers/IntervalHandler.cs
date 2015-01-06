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
    internal class IntervalHandler : TypeHandlerWithPsv<TimeSpan, NpgsqlInterval>, ITypeHandler<NpgsqlInterval>
    {
        public override TimeSpan Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (TimeSpan)((ITypeHandler<NpgsqlInterval>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlInterval ITypeHandler<NpgsqlInterval>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlInterval.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return ReadBinary(buf);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        NpgsqlInterval ReadBinary(NpgsqlBuffer buf)
        {
            var ticks = buf.ReadInt64();
            var day = buf.ReadInt32();
            var month = buf.ReadInt32();
            return new NpgsqlInterval(month, day, ticks * 10);
        }

        internal override int Length(object value)
        {
            return 16;
        }

        internal override void WriteBinary(object value, NpgsqlBuffer buf)
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
