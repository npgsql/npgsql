using System;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timestamp", NpgsqlDbType.Timestamp, new[] { DbType.DateTime, DbType.DateTime2 }, typeof(NpgsqlTimeStamp))]
    internal class TimeStampHandler : TypeHandlerWithPsv<DateTime, NpgsqlTimeStamp>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlTimeStamp>
    {
        public override bool SupportsBinaryWrite
        {
            get
            {
                return false; // TODO: Implement
            }
        }

        public DateTime Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            return (DateTime)((ISimpleTypeReader<NpgsqlTimeStamp>)this).Read(buf, fieldDescription, len);
        }

        NpgsqlTimeStamp ISimpleTypeReader<NpgsqlTimeStamp>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return NpgsqlTimeStamp.FromInt64(buf.ReadInt64());
        }
    }
}
