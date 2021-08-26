using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using BclTimeTzHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.TimeTzHandler;

namespace Npgsql.NodaTime.Internal
{
    sealed partial class TimeTzHandler : NpgsqlSimpleTypeHandler<OffsetTime>, INpgsqlSimpleTypeHandler<DateTimeOffset>
    {
        readonly BclTimeTzHandler _bclHandler;

        internal TimeTzHandler(PostgresType postgresType)
            : base(postgresType)
            => _bclHandler = new BclTimeTzHandler(postgresType);

        // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
        public override OffsetTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new(
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
    }
}
