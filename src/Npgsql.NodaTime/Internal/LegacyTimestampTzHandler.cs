using System;
using NodaTime;
using NodaTime.TimeZones;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using BclTimestampTzHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.TimestampTzHandler;

namespace Npgsql.NodaTime.Internal
{
    sealed partial class LegacyTimestampTzHandler : NpgsqlSimpleTypeHandler<Instant>, INpgsqlSimpleTypeHandler<ZonedDateTime>,
                              INpgsqlSimpleTypeHandler<OffsetDateTime>, INpgsqlSimpleTypeHandler<DateTimeOffset>, 
                              INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<long>
    {
        readonly IDateTimeZoneProvider _dateTimeZoneProvider;
        readonly TimestampTzHandler _wrappedHandler;

        public LegacyTimestampTzHandler(PostgresType postgresType)
            : base(postgresType)
        {
            _dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
            _wrappedHandler = new TimestampTzHandler(postgresType);
        }

        #region Read

        public override Instant Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => _wrappedHandler.Read(buf, len, fieldDescription);

        ZonedDateTime INpgsqlSimpleTypeHandler<ZonedDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            try
            {
                var zonedDateTime = ((INpgsqlSimpleTypeHandler<ZonedDateTime>)_wrappedHandler).Read(buf, len, fieldDescription);

                var value = buf.ReadInt64();
                if (value == long.MaxValue || value == long.MinValue)
                    throw new NotSupportedException("Infinity values not supported for timestamp with time zone");
                return zonedDateTime.WithZone(_dateTimeZoneProvider[buf.Connection.Timezone]);
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

        DateTimeOffset INpgsqlSimpleTypeHandler<DateTimeOffset>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _wrappedHandler.Read<DateTimeOffset>(buf, len, fieldDescription);

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _wrappedHandler.Read<DateTime>(buf, len, fieldDescription);

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _wrappedHandler.Read<long>(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter? parameter)
            => 8;

        int INpgsqlSimpleTypeHandler<ZonedDateTime>.ValidateAndGetLength(ZonedDateTime value, NpgsqlParameter? parameter)
            => 8;

        public int ValidateAndGetLength(OffsetDateTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(Instant value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _wrappedHandler.Write(value, buf, parameter);

        void INpgsqlSimpleTypeHandler<ZonedDateTime>.Write(ZonedDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _wrappedHandler.Write(value.ToInstant(), buf, parameter);

        public void Write(OffsetDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _wrappedHandler.Write(value.ToInstant(), buf, parameter);

        int INpgsqlSimpleTypeHandler<DateTimeOffset>.ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTimeOffset>)_wrappedHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTimeOffset>.Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTimeOffset>)_wrappedHandler).Write(value, buf, parameter);

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_wrappedHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_wrappedHandler).Write(value, buf, parameter);

        int INpgsqlSimpleTypeHandler<long>.ValidateAndGetLength(long value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<long>)_wrappedHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<long>.Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<long>)_wrappedHandler).Write(value, buf, parameter);

        #endregion Write
    }
}
