using System;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL timetz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("time with time zone", NpgsqlDbType.TimeTz)]
    public class TimeTzHandlerFactory : NpgsqlTypeHandlerFactory<DateTimeOffset>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<DateTimeOffset> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new TimeTzHandler(postgresType)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL timetz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TimeTzHandler : NpgsqlSimpleTypeHandler<DateTimeOffset>, INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<TimeSpan>
    {
        // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative

        /// <summary>
        /// Constructs an <see cref="TimeTzHandler"/>.
        /// </summary>
        public TimeTzHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override DateTimeOffset Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var ticks = buf.ReadInt64() * 10;
            var offset = new TimeSpan(0, 0, -buf.ReadInt32());
            return new DateTimeOffset(ticks + TimeSpan.TicksPerDay, offset);
        }

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription).LocalDateTime;

        TimeSpan INpgsqlSimpleTypeHandler<TimeSpan>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription).LocalDateTime.TimeOfDay;

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter) => 12;
        /// <inheritdoc />
        public int ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter)                => 12;
        /// <inheritdoc />
        public int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)                => 12;

        /// <inheritdoc />
        public override void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteInt64(value.TimeOfDay.Ticks / 10);
            buf.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
        }

        /// <inheritdoc />
        public void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteInt64(value.TimeOfDay.Ticks / 10);

            switch (value.Kind)
            {
            case DateTimeKind.Utc:
                buf.WriteInt32(0);
                break;
            case DateTimeKind.Unspecified:
            // Treat as local...
            case DateTimeKind.Local:
                buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {value.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
            }
        }

        /// <inheritdoc />
        public void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteInt64(value.Ticks / 10);
            buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
        }

        #endregion Write
    }
}
