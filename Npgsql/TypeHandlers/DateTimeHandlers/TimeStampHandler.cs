using System;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeStampHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStamp>, ITypeHandler<NpgsqlTimeStamp>
    {
        static readonly string[] _pgNames = { "timestamp" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Timestamp };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.DateTime };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }
        static readonly DbType[][] _dbTypes2 = { new DbType[] { DbType.DateTime2 } };
        internal override DbType[][] DbTypeAliases { get { return _dbTypes2; } }

        public override bool SupportsBinaryWrite
        {
            get
            {
                return false; // TODO: Implement
            }
        }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ITypeHandler<NpgsqlTimeStamp>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStamp ITypeHandler<NpgsqlTimeStamp>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTimeStamp.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return NpgsqlTimeStamp.FromInt64(buf.ReadInt64());
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            if (value is DateTime)
            {
                if (DateTime.MaxValue.Equals(value))
                {
                    writer.WriteString("infinity");
                }
                else if (DateTime.MinValue.Equals(value))
                {
                    writer.WriteString("-infinity");
                }
                else
                {
                    writer.WriteString((((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.ffffff", System.Globalization.DateTimeFormatInfo.InvariantInfo)));
                }
            }
            else
            {
                // Handles NpgsqlTimeStamp
                writer.WriteString(value.ToString());
            }
        }
    }
}
