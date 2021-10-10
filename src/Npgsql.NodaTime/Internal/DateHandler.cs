﻿using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using BclDateHandler = Npgsql.Internal.TypeHandlers.DateTimeHandlers.DateHandler;

#pragma warning disable 618 // NpgsqlDate is obsolete, remove in 7.0

namespace Npgsql.NodaTime.Internal
{
    sealed partial class DateHandler : NpgsqlSimpleTypeHandler<LocalDate>,
        INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<NpgsqlDate>, INpgsqlSimpleTypeHandler<int>
#if NET6_0_OR_GREATER
        , INpgsqlSimpleTypeHandler<DateOnly>
#endif
    {
        /// <summary>
        /// Whether to convert positive and negative infinity values to Instant.{Max,Min}Value when
        /// an Instant is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;
        readonly BclDateHandler _bclHandler;

        const string InfinityExceptionMessage = "Can't convert infinite timestamp values to DateTime";

        internal DateHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
            _bclHandler = new BclDateHandler(postgresType, convertInfinityDateTime);
        }

        public override LocalDate Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadInt32() switch
            {
                int.MaxValue => _convertInfinityDateTime ? LocalDate.MaxIsoValue : throw new InvalidCastException(InfinityExceptionMessage),
                int.MinValue => _convertInfinityDateTime ? LocalDate.MinIsoValue : throw new InvalidCastException(InfinityExceptionMessage),
                var value => new LocalDate().PlusDays(value + 730119)
            };

        public override int ValidateAndGetLength(LocalDate value, NpgsqlParameter? parameter)
            => 4;

        public override void Write(LocalDate value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (_convertInfinityDateTime)
            {
                if (value == LocalDate.MaxIsoValue)
                {
                    buf.WriteInt32(int.MaxValue);
                    return;
                }
                if (value == LocalDate.MinIsoValue)
                {
                    buf.WriteInt32(int.MinValue);
                    return;
                }
            }

            var totalDaysSinceEra = Period.Between(default, value, PeriodUnits.Days).Days;
            buf.WriteInt32(totalDaysSinceEra - 730119);
        }

        NpgsqlDate INpgsqlSimpleTypeHandler<NpgsqlDate>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<NpgsqlDate>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<NpgsqlDate>.ValidateAndGetLength(NpgsqlDate value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<NpgsqlDate>.Write(NpgsqlDate value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<DateTime>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<int>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<int>.ValidateAndGetLength(int value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<int>.Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);

#if NET6_0_OR_GREATER
        DateOnly INpgsqlSimpleTypeHandler<DateOnly>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<DateOnly>(buf, len, fieldDescription);

        public int ValidateAndGetLength(DateOnly value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        public void Write(DateOnly value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);
#endif

        public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
            => new DateRangeHandler(pgRangeType, this);
    }
}
