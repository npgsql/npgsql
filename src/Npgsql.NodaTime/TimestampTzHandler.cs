using System;
using NodaTime;
using NodaTime.TimeZones;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using BclTimestampTzHandler = Npgsql.TypeHandlers.DateTimeHandlers.TimestampTzHandler;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimestampTzHandlerFactory : NpgsqlTypeHandlerFactory<Instant>
    {
        // Check for the legacy floating point timestamps feature
        public override NpgsqlTypeHandler<Instant> Create(PostgresType postgresType, NpgsqlConnection conn)
        {
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            return conn.HasIntegerDateTimes
                ? new TimestampTzHandler(postgresType, csb.ConvertInfinityDateTime)
                : throw new NotSupportedException(
                    $"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
        }
    }

    sealed class TimestampTzHandler : NpgsqlSimpleTypeHandler<Instant>, INpgsqlSimpleTypeHandler<ZonedDateTime>,
                              INpgsqlSimpleTypeHandler<OffsetDateTime>, INpgsqlSimpleTypeHandler<DateTimeOffset>, 
                              INpgsqlSimpleTypeHandler<DateTime>
    {
        readonly IDateTimeZoneProvider _dateTimeZoneProvider;
        readonly BclTimestampTzHandler _bclHandler;

        /// <summary>
        /// Whether to convert positive and negative infinity values to Instant.{Max,Min}Value when
        /// an Instant is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        public TimestampTzHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
        {
            _dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
            _convertInfinityDateTime = convertInfinityDateTime;
            _bclHandler = new BclTimestampTzHandler(postgresType, convertInfinityDateTime);
        }

        #region Read

        public override Instant Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var value = buf.ReadInt64();
            if (_convertInfinityDateTime)
            {
                if (value == long.MaxValue)
                    return Instant.MaxValue;
                if (value == long.MinValue)
                    return Instant.MinValue;
            }
            return TimestampHandler.Decode(value);
        }

        ZonedDateTime INpgsqlSimpleTypeHandler<ZonedDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            try
            {
                var value = buf.ReadInt64();
                if (value == long.MaxValue || value == long.MinValue)
                    throw new NotSupportedException("Infinity values not supported for timestamp with time zone");
                return TimestampHandler.Decode(value).InZone(_dateTimeZoneProvider[buf.Connection.Timezone]);
            }
            catch (Exception e) when (
                string.Equals(buf.Connection.Timezone, "localtime", StringComparison.OrdinalIgnoreCase) &&
                (e is TimeZoneNotFoundException || e is DateTimeZoneNotFoundException))
            {
                throw new TimeZoneNotFoundException(
                    "The special PostgreSQL timezone 'localtime' is not supported when reading values of type 'timestamp with time zone'. " +
                    "Please specify a real timezone in 'postgresql.conf' on the server, or set the 'PGTZ' environment variable on the client.",
                    e);
            }
        }

        OffsetDateTime INpgsqlSimpleTypeHandler<OffsetDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => ((INpgsqlSimpleTypeHandler<ZonedDateTime>)this).Read(buf, len, fieldDescription).ToOffsetDateTime();

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter? parameter)
            => 8;

        int INpgsqlSimpleTypeHandler<ZonedDateTime>.ValidateAndGetLength(ZonedDateTime value, NpgsqlParameter? parameter)
            => 8;

        public int ValidateAndGetLength(OffsetDateTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(Instant value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (_convertInfinityDateTime)
            {
                if (value == Instant.MaxValue)
                {
                    buf.WriteInt64(long.MaxValue);
                    return;
                }

                if (value == Instant.MinValue)
                {
                    buf.WriteInt64(long.MinValue);
                    return;
                }
            }
            TimestampHandler.WriteInteger(value, buf);
        }

        void INpgsqlSimpleTypeHandler<ZonedDateTime>.Write(ZonedDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => Write(value.ToInstant(), buf, parameter);

        public void Write(OffsetDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => Write(value.ToInstant(), buf, parameter);

        #endregion Write

        DateTimeOffset INpgsqlSimpleTypeHandler<DateTimeOffset>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<DateTimeOffset>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<DateTimeOffset>.ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTimeOffset>.Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription) 
            => _bclHandler.Read<DateTime>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter) 
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) 
            => _bclHandler.Write(value, buf, parameter);
    }
}
