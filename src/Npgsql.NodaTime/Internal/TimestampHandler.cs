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
    sealed partial class TimestampHandler : NpgsqlSimpleTypeHandler<LocalDateTime>,
        INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<long>
    {
        readonly BclTimestampHandler _bclHandler;

        const string InfinityExceptionMessage = "Can't read infinity value since Npgsql.DisableDateTimeInfinityConversions is enabled";

        internal TimestampHandler(PostgresType postgresType)
            : base(postgresType)
            => _bclHandler = new BclTimestampHandler(postgresType);

        #region Read

        public override LocalDateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => ReadLocalDateTime(buf);

        // TODO: Switch to use LocalDateTime.MinMaxValue when available (#4061)
        internal static LocalDateTime ReadLocalDateTime(NpgsqlReadBuffer buf)
            => buf.ReadInt64() switch
            {
                long.MaxValue => DisableDateTimeInfinityConversions
                    ? throw new InvalidCastException(InfinityExceptionMessage)
                    : LocalDate.MaxIsoValue + LocalTime.MaxValue,
                long.MinValue => DisableDateTimeInfinityConversions
                    ? throw new InvalidCastException(InfinityExceptionMessage)
                    : LocalDate.MinIsoValue + LocalTime.MinValue,
                var value => DecodeInstant(value).InUtc().LocalDateTime
            };

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read(buf, len, fieldDescription);

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => ((INpgsqlSimpleTypeHandler<long>)_bclHandler).Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(LocalDateTime value, NpgsqlParameter? parameter)
            => 8;

        public override void Write(LocalDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => WriteLocalDateTime(value, buf);

        internal static void WriteLocalDateTime(LocalDateTime value, NpgsqlWriteBuffer buf)
        {
            // TODO: Switch to use LocalDateTime.MinMaxValue when available (#4061)
            if (!DisableDateTimeInfinityConversions)
            {
                if (value == LocalDate.MaxIsoValue + LocalTime.MaxValue)
                {
                    buf.WriteInt64(long.MaxValue);
                    return;
                }

                if (value == LocalDate.MinIsoValue + LocalTime.MinValue)
                {
                    buf.WriteInt64(long.MinValue);
                    return;
                }
            }

            buf.WriteInt64(EncodeInstant(value.InUtc().ToInstant()));
        }

        public int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).ValidateAndGetLength(value, parameter);

        public int ValidateAndGetLength(long value, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<long>)_bclHandler).ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<DateTime>)_bclHandler).Write(value, buf, parameter);

        void INpgsqlSimpleTypeHandler<long>.Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => ((INpgsqlSimpleTypeHandler<long>)_bclHandler).Write(value, buf, parameter);

        #endregion Write
    }
}
