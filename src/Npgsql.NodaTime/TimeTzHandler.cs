using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using BclTimeTzHandler = Npgsql.TypeHandlers.DateTimeHandlers.TimeTzHandler;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimeTzHandlerFactory : NpgsqlTypeHandlerFactory<OffsetTime>
    {
        // Check for the legacy floating point timestamps feature
        public override NpgsqlTypeHandler<OffsetTime> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new TimeTzHandler(postgresType)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    sealed class TimeTzHandler : NpgsqlSimpleTypeHandler<OffsetTime>, INpgsqlSimpleTypeHandler<DateTimeOffset>,
                                  INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<TimeSpan>
    {
        readonly BclTimeTzHandler _bclHandler;

        internal TimeTzHandler(PostgresType postgresType) : base(postgresType)
            => _bclHandler = new BclTimeTzHandler(postgresType);

        // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
        public override OffsetTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new OffsetTime(
                LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10),
                Offset.FromSeconds(-buf.ReadInt32()));

        public override int ValidateAndGetLength(OffsetTime value, NpgsqlParameter? parameter) => 12;

        public override void Write(OffsetTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteInt64(value.TickOfDay / 10);
            buf.WriteInt32(-(int)(value.Offset.Ticks / NodaConstants.TicksPerSecond));
        }

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

        TimeSpan INpgsqlSimpleTypeHandler<TimeSpan>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<TimeSpan>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<TimeSpan>.ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<TimeSpan>.Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);
    }
}
