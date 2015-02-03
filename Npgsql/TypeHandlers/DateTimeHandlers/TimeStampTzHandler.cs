using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timestamptz", NpgsqlDbType.TimestampTZ, DbType.DateTimeOffset, typeof(NpgsqlTimeStampTZ))]
    internal class TimeStampTzHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStampTZ>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlTimeStampTZ>
    {
        public DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ISimpleTypeReader<NpgsqlTimeStampTZ>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStampTZ ISimpleTypeReader<NpgsqlTimeStampTZ>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // The Int64 contains just the time in UTC, no time zone information
            var ts = NpgsqlTimeStamp.FromInt64(buf.ReadInt64());
            return new NpgsqlTimeStampTZ(ts.Date, new NpgsqlTimeTZ(ts.Time, NpgsqlTimeZone.UTC));
        }
    }
}
