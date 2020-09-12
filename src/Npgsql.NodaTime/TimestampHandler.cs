using System;
using System.Diagnostics;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using BclTimestampHandler = Npgsql.TypeHandlers.DateTimeHandlers.TimestampHandler;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimestampHandlerFactory : NpgsqlTypeHandlerFactory<Instant>
    {
        public override NpgsqlTypeHandler<Instant> Create(PostgresType postgresType, NpgsqlConnection conn)
        {
            if (!conn.HasIntegerDateTimes)
                throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");

            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            return new TimestampHandler(postgresType, csb.ConvertInfinityDateTime);
        }
    }

    sealed class TimestampHandler : NpgsqlSimpleTypeHandler<Instant>, INpgsqlSimpleTypeHandler<LocalDateTime>, INpgsqlSimpleTypeHandler<DateTime>
    {
        static readonly Instant Instant0 = Instant.FromUtc(1, 1, 1, 0, 0, 0);
        static readonly Instant Instant2000 = Instant.FromUtc(2000, 1, 1, 0, 0, 0);
        static readonly Duration Plus292Years = Duration.FromDays(292 * 365);
        static readonly Duration Minus292Years = -Plus292Years;

        /// <summary>
        /// Whether to convert positive and negative infinity values to Instant.{Max,Min}Value when
        /// an Instant is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;
        readonly BclTimestampHandler _bclHandler;

        internal TimestampHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
            _bclHandler = new BclTimestampHandler(postgresType, convertInfinityDateTime);
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

            return Decode(value);
        }

        LocalDateTime INpgsqlSimpleTypeHandler<LocalDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            var value = buf.ReadInt64();
            if (value == long.MaxValue || value == long.MinValue)
                throw new NotSupportedException("Infinity values not supported when reading LocalDateTime, read as Instant instead");
            return Decode(value).InUtc().LocalDateTime;
        }

        // value is the number of microseconds from 2000-01-01T00:00:00.
        // Unfortunately NodaTime doesn't have Duration.FromMicroseconds(), so we decompose into milliseconds
        // and nanoseconds
        internal static Instant Decode(long value)
            => Instant2000 + Duration.FromMilliseconds(value / 1000) + Duration.FromNanoseconds(value % 1000 * 1000);

        // This is legacy support for PostgreSQL's old floating-point timestamp encoding - finally removed in PG 10 and not used for a long
        // time. Unfortunately CrateDB seems to use this for some reason.
        internal static Instant Decode(double value)
        {
            Debug.Assert(!double.IsPositiveInfinity(value) && !double.IsNegativeInfinity(value));

            if (value >= 0d)
            {
                var date = (int)value / 86400;
                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                var microsecondOfDay = (long)((value % 86400d) * 1000000d);

                return Instant0 + Duration.FromDays(date) + Duration.FromNanoseconds(microsecondOfDay * 1000);
            }
            else
            {
                value = -value;
                var date = (int)value / 86400;
                var microsecondOfDay = (long)((value % 86400d) * 1000000d);
                if (microsecondOfDay != 0)
                {
                    ++date;
                    microsecondOfDay = 86400000000L - microsecondOfDay;
                }

                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01

                return Instant0 + Duration.FromDays(date) + Duration.FromNanoseconds(microsecondOfDay * 1000);
            }
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter? parameter)
            => 8;

        int INpgsqlSimpleTypeHandler<LocalDateTime>.ValidateAndGetLength(LocalDateTime value, NpgsqlParameter? parameter)
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

            WriteInteger(value, buf);
        }

        void INpgsqlSimpleTypeHandler<LocalDateTime>.Write(LocalDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => WriteInteger(value.InUtc().ToInstant(), buf);

        // We need to write the number of microseconds from 2000-01-01T00:00:00.
        internal static void WriteInteger(Instant instant, NpgsqlWriteBuffer buf)
        {
            var since2000 = instant - Instant2000;

            // The nanoseconds may overflow, so fallback to BigInteger where necessary.
            var microseconds =
                since2000 >= Minus292Years &&
                since2000 <= Plus292Years
                    ? since2000.ToInt64Nanoseconds() / 1000
                    : (long)(since2000.ToBigIntegerNanoseconds() / 1000);

            buf.WriteInt64(microseconds);
        }

        // This is legacy support for PostgreSQL's old floating-point timestamp encoding - finally removed in PG 10 and not used for a long
        // time. Unfortunately CrateDB seems to use this for some reason.
        internal static void WriteDouble(Instant instant, NpgsqlWriteBuffer buf)
        {
            var localDateTime = instant.InUtc().LocalDateTime;
            var totalDaysSinceEra = Period.Between(default(LocalDateTime), localDateTime, PeriodUnits.Days).Days;
            var secondOfDay = localDateTime.NanosecondOfDay / 1000000000d;

            if (totalDaysSinceEra >= 730119)
            {
                var uSecsDate = (totalDaysSinceEra - 730119) * 86400d;
                buf.WriteDouble(uSecsDate + secondOfDay);
            }
            else
            {
                var uSecsDate = (730119 - totalDaysSinceEra) * 86400d;
                buf.WriteDouble(-(uSecsDate - secondOfDay));
            }
        }

        #endregion Write

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
           => _bclHandler.Read(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).Write(value, buf, parameter);
    }
}
