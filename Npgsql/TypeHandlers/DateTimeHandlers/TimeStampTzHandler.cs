using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using System.Runtime.Remoting.Channels;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timestamptz", NpgsqlDbType.TimestampTZ, DbType.DateTimeOffset, typeof(DateTimeOffset))]
    internal class TimeStampTzHandler : TimeStampHandler, ISimpleTypeReader<NpgsqlDateTime>, ISimpleTypeReader<DateTimeOffset>
    {
        public TimeStampTzHandler(TypeHandlerRegistry registry) : base(registry) {}

        public override DateTime Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            var ts = ((ISimpleTypeReader<NpgsqlDateTime>)this).Read(buf, len, fieldDescription);
            try
            {
                return ts.DateTime;
            } catch (Exception e) {
                throw new SafeReadException(e);
            }
        }

        NpgsqlDateTime ISimpleTypeReader<NpgsqlDateTime>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            return new NpgsqlDateTime(ts.Date, ts.Time, DateTimeKind.Utc);
        }

        DateTimeOffset ISimpleTypeReader<DateTimeOffset>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return new DateTimeOffset(ReadTimeStamp(buf, len, fieldDescription).DateTime, TimeSpan.Zero);
        }

        public override void Write(object value, NpgsqlBuffer buf)
        {
            if (value is NpgsqlDateTime)
            {
                var ts = (NpgsqlDateTime)value;
                switch (ts.Kind)
                {
                case DateTimeKind.Unspecified:
                    // Treat as Local
                case DateTimeKind.Utc:
                    break;
                case DateTimeKind.Local:
                    ts = ts.ToUniversalTime();
                    break;
                default:
                    throw PGUtil.ThrowIfReached();
                }
                base.Write(ts, buf);
                return;
            }

            if (value is DateTime)
            {
                var dt = (DateTime)value;
                switch (dt.Kind)
                {
                case DateTimeKind.Unspecified:
                // Treat as Local
                case DateTimeKind.Utc:
                    break;
                case DateTimeKind.Local:
                    dt = dt.ToUniversalTime();
                    break;
                default:
                    throw PGUtil.ThrowIfReached();
                }
                base.Write(dt, buf);
                return;
            }

            if (value is DateTimeOffset)
            {
                base.Write(((DateTimeOffset)value).ToUniversalTime(), buf);
                return;
            }

            throw new InvalidCastException();
        }
    }
}
