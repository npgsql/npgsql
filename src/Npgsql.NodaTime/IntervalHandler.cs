using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<Period>
    {
        // Check for the legacy floating point timestamps feature
        public override NpgsqlTypeHandler<Period> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new IntervalHandler(postgresType)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    class IntervalHandler : NpgsqlSimpleTypeHandler<Period>
    {
        public IntervalHandler(PostgresType postgresType) : base(postgresType) {}

        public override Period Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var microsecondsInDay = buf.ReadInt64();
            var days = buf.ReadInt32();
            var totalMonths = buf.ReadInt32();

            // Nodatime will normalize most things (i.e. nanoseconds to milliseconds, seconds...)
            // but it will not normalize months to years.
            var months = totalMonths % 12;
            var years = totalMonths / 12;

            return new PeriodBuilder
            {
                Nanoseconds = microsecondsInDay * 1000,
                Days = days,
                Months = months,
                Years = years
            }.Build().Normalize();
        }

        public override int ValidateAndGetLength(Period value, NpgsqlParameter? parameter)
            => 16;

        public override void Write(Period value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            var microsecondsInDay =
                (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) * NodaConstants.MillisecondsPerSecond + value.Milliseconds) * 1000 +
                value.Nanoseconds / 1000; // Take the microseconds, discard the nanosecond remainder

            buf.WriteInt64(microsecondsInDay);
            buf.WriteInt32(value.Weeks * 7 + value.Days); // days
            buf.WriteInt32(value.Years * 12 + value.Months); // months
        }
    }
}
