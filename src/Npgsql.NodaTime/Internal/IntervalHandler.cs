using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using BclIntervalHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.IntervalHandler;

#pragma warning disable 618 // NpgsqlTimeSpan is obsolete, remove in 7.0

namespace Npgsql.NodaTime.Internal
{
    sealed partial class IntervalHandler :
        NpgsqlSimpleTypeHandler<Period>,
        INpgsqlSimpleTypeHandler<Duration>,
        INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>,
        INpgsqlSimpleTypeHandler<TimeSpan>,
        INpgsqlSimpleTypeHandler<NpgsqlInterval>
    {
        readonly BclIntervalHandler _bclHandler;

        internal IntervalHandler(PostgresType postgresType)
            : base(postgresType)
            => _bclHandler = new BclIntervalHandler(postgresType);

        public override Period Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var microsecondsInDay = buf.ReadInt64();
            var days = buf.ReadInt32();
            var totalMonths = buf.ReadInt32();

            // NodaTime will normalize most things (i.e. nanoseconds to milliseconds, seconds...)
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
            // Note that the end result must be long
            // see #3438
            var microsecondsInDay =
                (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) * NodaConstants.MillisecondsPerSecond + value.Milliseconds) * 1000 +
                value.Nanoseconds / 1000; // Take the microseconds, discard the nanosecond remainder

            buf.WriteInt64(microsecondsInDay);
            buf.WriteInt32(value.Weeks * 7 + value.Days); // days
            buf.WriteInt32(value.Years * 12 + value.Months); // months
        }

        Duration INpgsqlSimpleTypeHandler<Duration>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            var microsecondsInDay = buf.ReadInt64();
            var days = buf.ReadInt32();
            var totalMonths = buf.ReadInt32();

            if (totalMonths != 0)
                throw new NpgsqlException("Cannot read PostgreSQL interval with non-zero months to NodaTime Duration. Try reading as a NodaTime Period instead.");

            return Duration.FromDays(days) + Duration.FromNanoseconds(microsecondsInDay * 1000);
        }

        public int ValidateAndGetLength(Duration value, NpgsqlParameter? parameter) => 16;

        public void Write(Duration value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            const long microsecondsPerSecond = 1_000_000;

            // Note that the end result must be long
            // see #3438
            var microsecondsInDay =
                (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) *
                    microsecondsPerSecond + value.SubsecondNanoseconds / 1000); // Take the microseconds, discard the nanosecond remainder

            buf.WriteInt64(microsecondsInDay);
            buf.WriteInt32(value.Days); // days
            buf.WriteInt32(0); // months
        }

        NpgsqlTimeSpan INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<NpgsqlTimeSpan>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>.ValidateAndGetLength(NpgsqlTimeSpan value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>.Write(NpgsqlTimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);

        TimeSpan INpgsqlSimpleTypeHandler<TimeSpan>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<TimeSpan>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<TimeSpan>.ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<TimeSpan>)_bclHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<TimeSpan>.Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<TimeSpan>)_bclHandler).Write(value, buf, parameter);

        NpgsqlInterval INpgsqlSimpleTypeHandler<NpgsqlInterval>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<NpgsqlInterval>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<NpgsqlInterval>.ValidateAndGetLength(NpgsqlInterval value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<NpgsqlInterval>)_bclHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<NpgsqlInterval>.Write(NpgsqlInterval value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<NpgsqlInterval>)_bclHandler).Write(value, buf, parameter);
    }
}
