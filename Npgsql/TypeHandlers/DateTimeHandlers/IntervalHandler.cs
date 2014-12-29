using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class IntervalHandler : TypeHandlerWithPsv<TimeSpan, NpgsqlInterval>, ITypeHandler<NpgsqlInterval>
    {
        static readonly string[] _pgNames = { "interval" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

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
    }
}
