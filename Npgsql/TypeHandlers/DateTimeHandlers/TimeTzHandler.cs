using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeTzHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeTZ>, ITypeHandler<NpgsqlTimeTZ>
    {
        static readonly string[] _pgNames = { "timetz" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.TimeTZ };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }

        public override bool SupportsBinaryWrite
        {
            get
            {
                return false; // TODO: Implement
            }
        }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeTZ?
            return (DateTime)((ITypeHandler<NpgsqlTimeTZ>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeTZ ITypeHandler<NpgsqlTimeTZ>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTimeTZ.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    // Adjusting from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
                    return new NpgsqlTimeTZ(buf.ReadInt64() * 10, new NpgsqlTimeZone(0, 0, -buf.ReadInt32()));
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            if (value is DateTime)
            {
                writer.WriteString(((DateTime)value).ToString("HH:mm:ss.ffffff", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            }
            else
            {
                NpgsqlTimeTZ time;
                if (value is TimeSpan)
                {
                    time = (NpgsqlTimeTZ)(TimeSpan)value;
                }
                else
                {
                    time = (NpgsqlTimeTZ)value;
                }
                writer.WriteString(time.ToString());
            }
        }
    }
}
