using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("timestamp with time zone", NpgsqlDbType.TimestampTz, DbType.DateTimeOffset, typeof(DateTimeOffset))]
    class TimestampTzHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<DateTime> Create(NpgsqlConnection conn)
            => new TimestampTzHandler(conn.HasIntegerDateTimes, conn.Connector.ConvertInfinityDateTime);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimestampTzHandler : TimestampHandler, INpgsqlSimpleTypeHandler<DateTimeOffset>
    {
        public TimestampTzHandler(bool integerFormat, bool convertInfinityDateTime)
            : base(integerFormat, convertInfinityDateTime) {}

        #region Read

        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            try
            {
                if (ts.IsFinite)
                    return ts.ToDateTime().ToLocalTime();
                if (!ConvertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite timestamptz values to DateTime");
                if (ts.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            } catch (Exception e) {
                throw new NpgsqlSafeReadException(e);
            }
        }

        protected override NpgsqlDateTime ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            return new NpgsqlDateTime(ts.Date, ts.Time, DateTimeKind.Utc).ToLocalTime();
        }

        DateTimeOffset INpgsqlSimpleTypeHandler<DateTimeOffset>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            try
            {
                if (ts.IsFinite)
                    return ts.ToDateTime().ToLocalTime();
                if (!ConvertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite timestamptz values to DateTime");
                if (ts.IsInfinity)
                    return DateTimeOffset.MaxValue;
                return DateTimeOffset.MinValue;
            } catch (Exception e) {
                throw new NpgsqlSafeReadException(e);
            }
        }

        #endregion Read

        #region Write

        public int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter parameter)
            => 8;

        public override void Write(NpgsqlDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            switch (value.Kind)
            {
            case DateTimeKind.Unspecified:
            case DateTimeKind.Utc:
                break;
            case DateTimeKind.Local:
                value = value.ToUniversalTime();
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {value.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
            }
            base.Write(value, buf, parameter);
        }

        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            switch (value.Kind)
            {
            case DateTimeKind.Unspecified:
            case DateTimeKind.Utc:
                break;
            case DateTimeKind.Local:
                value = value.ToUniversalTime();
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {value.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
            }
            base.Write(value, buf, parameter);
        }

        public void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => base.Write(value.ToUniversalTime().DateTime, buf, parameter);

        #endregion Write
    }
}
