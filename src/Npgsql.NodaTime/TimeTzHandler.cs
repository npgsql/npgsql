using System;
using System.Threading.Tasks;
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

    sealed class TimeTzHandler : NpgsqlSimpleTypeHandler<OffsetTime>
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

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value is DateTimeOffset
                ? _bclHandler.ValidateObjectAndGetLength(value, ref lengthCache, parameter)
                : base.ValidateObjectAndGetLength(value, ref lengthCache, parameter);

        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => value is DateTimeOffset
                ? _bclHandler.WriteObjectWithLength(value, buf, lengthCache, parameter, async)
                : base.WriteObjectWithLength(value, buf, lengthCache, parameter, async);

        internal override TAny Read<TAny>(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => typeof(TAny) == typeof(DateTimeOffset)
                ? _bclHandler.Read<TAny>(buf, len, fieldDescription)
                : base.Read<TAny>(buf, len, fieldDescription);
    }
}
