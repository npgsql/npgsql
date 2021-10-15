﻿using System;
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

        internal TimestampHandler(PostgresType postgresType)
            : base(postgresType)
            => _bclHandler = new BclTimestampHandler(postgresType);

        #region Read

        public override LocalDateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => ReadLocalDateTime(buf);

        internal static LocalDateTime ReadLocalDateTime(NpgsqlReadBuffer buf)
        {
            var value = buf.ReadInt64();

            // No {Min,Max}Value for LocalDateTime: https://github.com/nodatime/nodatime/issues/58
            // But infinity values can still be accessed as long.{Min,Max}Value
            if (value == long.MaxValue || value == long.MinValue)
                throw new NotSupportedException($"Infinity values not supported when reading {nameof(LocalDateTime)}");

            return DecodeInstant(value).InUtc().LocalDateTime;
        }

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
            => buf.WriteInt64(EncodeInstant(value.InUtc().ToInstant()));

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
