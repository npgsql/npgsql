using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimestampTzHandlerFactory : NpgsqlTypeHandlerFactory<Instant>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<Instant> Create(NpgsqlConnection conn)
            => new TimestampTzHandler(conn);
    }

    class TimestampTzHandler : NpgsqlSimpleTypeHandler<Instant>, INpgsqlSimpleTypeHandler<ZonedDateTime>,
        INpgsqlSimpleTypeHandler<OffsetDateTime>
    {
        readonly IDateTimeZoneProvider _dateTimeZoneProvider;

        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public TimestampTzHandler(NpgsqlConnection conn)
        {
            _integerFormat = conn.HasIntegerDateTimes;
            _dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
        }

        #region Read

        public override Instant Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (_integerFormat) {
                var value = buf.ReadInt64();
                if (value == long.MaxValue || value == long.MinValue)
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported for timestamp with time zone"));
                return TimestampHandler.Decode(value);
            }
            else
            {
                var value = buf.ReadDouble();
                if (double.IsPositiveInfinity(value) || double.IsNegativeInfinity(value))
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported for timestamp with time zone"));
                return TimestampHandler.Decode(value);
            }
        }

        ZonedDateTime INpgsqlSimpleTypeHandler<ZonedDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            try
            {
                if (_integerFormat)
                {
                    var value = buf.ReadInt64();
                    if (value == long.MaxValue || value == long.MinValue)
                        throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported for timestamp with time zone"));
                    return TimestampHandler.Decode(value).InZone(_dateTimeZoneProvider[buf.Connection.Timezone]);
                }
                else
                {
                    var value = buf.ReadDouble();
                    if (double.IsPositiveInfinity(value) || double.IsNegativeInfinity(value))
                        throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported for timestamp with time zone"));
                    return TimestampHandler.Decode(value).InZone(_dateTimeZoneProvider[buf.Connection.Timezone]);
                }
            }
            catch (TimeZoneNotFoundException) when (string.Equals(buf.Connection.Timezone, "localtime", StringComparison.OrdinalIgnoreCase))
            {
                throw new NpgsqlSafeReadException(
                    new TimeZoneNotFoundException(
                        "The special PostgreSQL timezone 'localtime' is not supported when reading values of type 'timestamp with time zone'. " +
                        "Please specify a real timezone in 'postgresql.conf' on the server, or set the 'PGTZ' environment variable on the client."));
            }
            catch (TimeZoneNotFoundException e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        OffsetDateTime INpgsqlSimpleTypeHandler<OffsetDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
            => ((INpgsqlSimpleTypeHandler<ZonedDateTime>)this).Read(buf, len, fieldDescription).ToOffsetDateTime();

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter parameter)
            => 8;

        int INpgsqlSimpleTypeHandler<ZonedDateTime>.ValidateAndGetLength(ZonedDateTime value, NpgsqlParameter parameter)
            => 8;

        public int ValidateAndGetLength(OffsetDateTime value, NpgsqlParameter parameter)
            => 8;

        public override void Write(Instant value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                TimestampHandler.WriteInteger(value, buf);
            else
                TimestampHandler.WriteDouble(value, buf);
        }

        void INpgsqlSimpleTypeHandler<ZonedDateTime>.Write(ZonedDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write(value.ToInstant(), buf, parameter);

        public void Write(OffsetDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write(value.ToInstant(), buf, parameter);

        #endregion Write
    }
}
