using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using BclTimestampHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.TimestampHandler;
using static Npgsql.NodaTime.Internal.NodaTimeUtils;

namespace Npgsql.NodaTime.Internal
{
    sealed partial class TimestampHandler : NpgsqlSimpleTypeHandler<LocalDateTime>, INpgsqlSimpleTypeHandler<DateTime>
    {
        readonly BclTimestampHandler _bclHandler;

        internal TimestampHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
            => _bclHandler = new BclTimestampHandler(postgresType, convertInfinityDateTime);

        #region Read

        public override LocalDateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => ReadLocalDateTime(buf);

        internal static LocalDateTime ReadLocalDateTime(NpgsqlReadBuffer buf)
        {
            var value = buf.ReadInt64();

            if (value == long.MaxValue || value == long.MinValue)
                throw new NotSupportedException($"Infinity values not supported when reading {nameof(LocalDateTime)}");

            return DecodeInstant(value).InUtc().LocalDateTime;
        }

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(LocalDateTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(LocalDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => WriteLocalDateTime(value, buf);

        internal static void WriteLocalDateTime(LocalDateTime value, NpgsqlWriteBuffer buf)
            => buf.WriteInt64(EncodeInstant(value.InUtc().ToInstant()));

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).Write(value, buf, parameter);

        #endregion Write
    }
}
