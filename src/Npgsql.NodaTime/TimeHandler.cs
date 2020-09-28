using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using BclTimeHandler = Npgsql.TypeHandlers.DateTimeHandlers.TimeHandler;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimeHandlerFactory : NpgsqlTypeHandlerFactory<LocalTime>
    {
        // Check for the legacy floating point timestamps feature
        public override NpgsqlTypeHandler<LocalTime> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new TimeHandler(postgresType)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    sealed class TimeHandler : NpgsqlSimpleTypeHandler<LocalTime>, INpgsqlSimpleTypeHandler<TimeSpan>
    {
        readonly BclTimeHandler _bclHandler;

        internal TimeHandler(PostgresType postgresType) : base(postgresType)
            => _bclHandler = new BclTimeHandler(postgresType);

        // PostgreSQL time resolution == 1 microsecond == 10 ticks
        public override LocalTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10);

        public override int ValidateAndGetLength(LocalTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(LocalTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteInt64(value.TickOfDay / 10);

        TimeSpan INpgsqlSimpleTypeHandler<TimeSpan>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
           => _bclHandler.Read<TimeSpan>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<TimeSpan>.ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<TimeSpan>.Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);
    }
}
