using System;
using System.Diagnostics;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using BclTimestampHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.TimestampHandler;

namespace Npgsql.NodaTime.Internal
{
    sealed partial class LegacyTimestampHandler : NpgsqlSimpleTypeHandler<Instant>,
        INpgsqlSimpleTypeHandler<LocalDateTime>, INpgsqlSimpleTypeHandler<DateTime>
    {
        readonly bool _convertInfinityDateTime;
        readonly BclTimestampHandler _bclHandler;

        internal LegacyTimestampHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
            _bclHandler = new BclTimestampHandler(postgresType, convertInfinityDateTime);
        }

        #region Read

        public override Instant Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => TimestampTzHandler.ReadInstant(buf, _convertInfinityDateTime);

        LocalDateTime INpgsqlSimpleTypeHandler<LocalDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => TimestampHandler.ReadLocalDateTime(buf);

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter? parameter)
            => 8;

        int INpgsqlSimpleTypeHandler<LocalDateTime>.ValidateAndGetLength(LocalDateTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(Instant value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => TimestampTzHandler.WriteInstant(value, buf, _convertInfinityDateTime);

        void INpgsqlSimpleTypeHandler<LocalDateTime>.Write(LocalDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => TimestampHandler.WriteLocalDateTime(value, buf);

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).Write(value, buf, parameter);

        #endregion Write
    }
}
