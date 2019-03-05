using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("timestamp", NpgsqlDbType.Timestamp, new[] { DbType.DateTime, DbType.DateTime2 }, new[] { typeof(NpgsqlDateTime), typeof(DateTime) }, DbType.DateTime)]
    class TimestampHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<DateTime> Create(NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new TimestampHandler(conn.Connector.ConvertInfinityDateTime)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimestampHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDateTime>
    {
        internal const uint TypeOID = 1114;

        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        protected readonly bool ConvertInfinityDateTime;

        internal TimestampHandler(bool convertInfinityDateTime)
            => ConvertInfinityDateTime = convertInfinityDateTime;

        #region Read

        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            try
            {
                if (ts.IsFinite)
                    return ts.ToDateTime();
                if (!ConvertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite timestamp values to DateTime");
                if (ts.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            }
            catch (Exception e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        protected override NpgsqlDateTime ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadTimeStamp(buf, len, fieldDescription);

        protected NpgsqlDateTime ReadTimeStamp(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var value = buf.ReadInt64();
            if (value == long.MaxValue)
                return NpgsqlDateTime.Infinity;
            if (value == long.MinValue)
                return NpgsqlDateTime.NegativeInfinity;
            if (value >= 0)
            {
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;

                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlDateTime(new NpgsqlDate(date), new TimeSpan(time));
            }
            else
            {
                value = -value;
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;
                if (time != 0)
                {
                    ++date;
                    time = 86400000000L - time;
                }

                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlDateTime(new NpgsqlDate(date), new TimeSpan(time));
            }
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter parameter)
            => 8;

        public override int ValidateAndGetLength(NpgsqlDateTime value, NpgsqlParameter parameter)
            => 8;

        public override void Write(NpgsqlDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (value.IsInfinity)
            {
                buf.WriteInt64(long.MaxValue);
                return;
            }

            if (value.IsNegativeInfinity)
            {
                buf.WriteInt64(long.MinValue);
                return;
            }

            var uSecsTime = value.Time.Ticks / 10;

            if (value >= new NpgsqlDateTime(2000, 1, 1, 0, 0, 0))
            {
                var uSecsDate = (value.Date.DaysSinceEra - 730119) * 86400000000L;
                buf.WriteInt64(uSecsDate + uSecsTime);
            }
            else
            {
                var uSecsDate = (730119 - value.Date.DaysSinceEra) * 86400000000L;
                buf.WriteInt64(-(uSecsDate - uSecsTime));
            }
        }

        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (ConvertInfinityDateTime)
            {
                if (value == DateTime.MaxValue)
                {
                    buf.WriteInt64(long.MaxValue);
                    return;
                }

                if (value == DateTime.MinValue)
                {
                    buf.WriteInt64(long.MinValue);
                    return;
                }
            }

            Write(new NpgsqlDateTime(value), buf, parameter);
        }

        #endregion Write
    }
}
