using System;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    internal class TimeStampTzHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStampTZ>, ITypeHandler<NpgsqlTimeStampTZ>
    {
        static readonly string[] _pgNames = { "timestamptz" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ITypeHandler<NpgsqlTimeStampTZ>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStampTZ ITypeHandler<NpgsqlTimeStampTZ>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return NpgsqlTimeStampTZ.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return ReadBinary(buf);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        NpgsqlTimeStampTZ ReadBinary(NpgsqlBuffer buf)
        {
            // The Int64 contains just the time in UTC, no time zone information
            var ts = NpgsqlTimeStamp.FromInt64(buf.ReadInt64());
            return new NpgsqlTimeStampTZ(ts.Date, new NpgsqlTimeTZ(ts.Time, NpgsqlTimeZone.UTC));
        }
    }
}
