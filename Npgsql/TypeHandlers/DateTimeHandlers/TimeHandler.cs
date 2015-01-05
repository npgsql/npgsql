using System;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeHandler : TypeHandlerWithPsv<DateTime, NpgsqlTime>, ITypeHandler<NpgsqlTime>
    {
        static readonly string[] _pgNames = { "time" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Time };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Time };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

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

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            if (value is DateTime)
            {
                writer.WriteString(((DateTime)value).ToString("HH:mm:ss.ffffff", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            }
            else
            {
                NpgsqlTime time;
                if (value is TimeSpan)
                {
                    time = (NpgsqlTime)(TimeSpan)value;
                }
                else
                {
                    time = (NpgsqlTime)value;
                }
                writer.WriteString(time.ToString());
            }
        }

        internal override int BinarySize(object value)
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
