using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using System.Runtime.CompilerServices;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL timestamp data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("timestamp without time zone", NpgsqlDbType.Timestamp, new[] { DbType.DateTime, DbType.DateTime2 }, new[] { typeof(NpgsqlDateTime), typeof(DateTime) }, DbType.DateTime)]
    public class TimestampHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<DateTime> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new TimestampHandler(postgresType, conn.Connector!.ConvertInfinityDateTime)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL timestamp data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TimestampHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDateTime>
    {
        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        protected readonly bool ConvertInfinityDateTime;

        /// <summary>
        /// Constructs a <see cref="TimestampHandler"/>.
        /// </summary>
        public TimestampHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType) => ConvertInfinityDateTime = convertInfinityDateTime;

        #region Read

        private protected const string InfinityExceptionMessage = "Can't convert infinite timestamp values to DateTime";
        private protected const string OutOfRangeExceptionMessage = "Out of the range of DateTime (year must be between 1 and 9999)";

        /// <inheritdoc />
        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {

            var postgresTimestamp = buf.ReadInt64();
            if (postgresTimestamp == long.MaxValue)
                return ConvertInfinityDateTime
                    ? DateTime.MaxValue
                    : throw new InvalidCastException(InfinityExceptionMessage);
            if (postgresTimestamp == long.MinValue)
                return ConvertInfinityDateTime
                    ? DateTime.MinValue
                    : throw new InvalidCastException(InfinityExceptionMessage);

            try
            {
                return FromPostgresTimestamp(postgresTimestamp);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidCastException(OutOfRangeExceptionMessage, e);
            }
        }

        /// <inheritdoc />
        protected override NpgsqlDateTime ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => ReadTimeStamp(buf, len, fieldDescription);

        /// <summary>
        /// Reads a timestamp from the buffer as an <see cref="NpgsqlDateTime"/>.
        /// </summary>
        protected NpgsqlDateTime ReadTimeStamp(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
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

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlDateTime value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override void Write(NpgsqlDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
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

        /// <inheritdoc />
        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
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

            var postgresTimestamp = ToPostgresTimestamp(value);
            buf.WriteInt64(postgresTimestamp);
        }

        #endregion Write

        const long PostgresTimestampOffsetTicks = 630822816000000000L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static long ToPostgresTimestamp(DateTime value)
            // Rounding here would cause problems because we would round up DateTime.MaxValue
            // which would make it impossible to retrieve it back from the database, so we just drop the additional precision
            => (value.Ticks - PostgresTimestampOffsetTicks) / 10;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DateTime FromPostgresTimestamp(long value)
            => new DateTime(value * 10 + PostgresTimestampOffsetTicks);
    }
}
