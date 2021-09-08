using System;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using BclTimeHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.TimeHandler;

namespace Npgsql.NodaTime.Internal
{
    sealed partial class TimeHandler : NpgsqlSimpleTypeHandler<LocalTime>, INpgsqlSimpleTypeHandler<TimeSpan>
#if NET6_0_OR_GREATER
        , INpgsqlSimpleTypeHandler<TimeOnly>
#endif
    {
        readonly BclTimeHandler _bclHandler;

        internal TimeHandler(PostgresType postgresType)
            : base(postgresType)
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

#if NET6_0_OR_GREATER
        TimeOnly INpgsqlSimpleTypeHandler<TimeOnly>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<TimeOnly>(buf, len, fieldDescription);

        public int ValidateAndGetLength(TimeOnly value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        public void Write(TimeOnly value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);
#endif
    }
}
