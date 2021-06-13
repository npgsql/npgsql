using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL timestamptz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("timestamp with time zone", NpgsqlDbType.TimestampTz, DbType.DateTimeOffset, typeof(DateTimeOffset))]
    public class TimestampTzHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<DateTime> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new TimestampTzHandler(postgresType, conn.Connector!.ConvertInfinityDateTime)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL timestamptz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TimestampTzHandler : TimestampHandler, INpgsqlSimpleTypeHandler<DateTimeOffset>
    {
        /// <summary>
        /// Constructs an <see cref="TimestampTzHandler"/>.
        /// </summary>
        public TimestampTzHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType, convertInfinityDateTime) {}

        /// <inheritdoc />
        public override IRangeHandler CreateRangeHandler(PostgresType rangeBackendType)
            => new RangeHandler<DateTime, DateTimeOffset>(rangeBackendType, this);

        #region Read

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
                return FromPostgresTimestamp(postgresTimestamp).ToLocalTime();
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidCastException(OutOfRangeExceptionMessage, e);
            }
        }

        /// <inheritdoc />
        protected override NpgsqlDateTime ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            return ts.IsFinite ? new NpgsqlDateTime(ts.Date, ts.Time, DateTimeKind.Utc).ToLocalTime() : ts;
        }

        DateTimeOffset INpgsqlSimpleTypeHandler<DateTimeOffset>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            var postgresTimestamp = buf.ReadInt64();
            if (postgresTimestamp == long.MaxValue)
                return ConvertInfinityDateTime
                    ? DateTimeOffset.MaxValue
                    : throw new InvalidCastException(InfinityExceptionMessage);
            if (postgresTimestamp == long.MinValue)
                return ConvertInfinityDateTime
                    ? DateTimeOffset.MinValue
                    : throw new InvalidCastException(InfinityExceptionMessage);
            try
            {
                return FromPostgresTimestamp(postgresTimestamp).ToLocalTime();
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidCastException(OutOfRangeExceptionMessage, e);
            }
        }

        #endregion Read

        #region Write

        /// <inheritdoc />
        public int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override void Write(NpgsqlDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
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

        /// <inheritdoc />
        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
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

            NpgsqlDateTime pgValue = value;
            if (ConvertInfinityDateTime)
            {
                if (value == DateTime.MinValue)
                {
                    pgValue = NpgsqlDateTime.NegativeInfinity;
                }
                else if (value == DateTime.MaxValue)
                {
                    pgValue = NpgsqlDateTime.Infinity;
                }
            }

            // We cannot pass the DateTime value due to it implicitly converting to the NpgsqlDateTime anyway
            base.Write(pgValue, buf, parameter);
        }

        /// <inheritdoc />
        public void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => base.Write(value.ToUniversalTime().DateTime, buf, parameter);

        #endregion Write
    }
}
