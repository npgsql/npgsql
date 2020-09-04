using System;
using System.Threading.Tasks;
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

    internal class TimeHandler : NpgsqlSimpleTypeHandler<LocalTime>
    {
        readonly BclTimeHandler _bclTimeHandler;

        internal TimeHandler(PostgresType postgresType) : base(postgresType)
            => _bclTimeHandler = new BclTimeHandler(postgresType);

        // PostgreSQL time resolution == 1 microsecond == 10 ticks
        public override LocalTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10);

        public override int ValidateAndGetLength(LocalTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(LocalTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteInt64(value.TickOfDay / 10);

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value is TimeSpan
                ? _bclTimeHandler.ValidateObjectAndGetLength(value, ref lengthCache, parameter)
                : base.ValidateObjectAndGetLength(value, ref lengthCache, parameter);

        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => value is TimeSpan
                ? _bclTimeHandler.WriteObjectWithLength(value, buf, lengthCache, parameter, async)
                : base.WriteObjectWithLength(value, buf, lengthCache, parameter, async);

        internal override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => typeof(TAny) == typeof(TimeSpan)
                ? _bclTimeHandler.Read<TAny>(buf, len, fieldDescription)
                : base.Read<TAny>(buf, len, fieldDescription);
    }
}
