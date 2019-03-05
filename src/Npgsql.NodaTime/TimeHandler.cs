﻿using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimeHandlerFactory : NpgsqlTypeHandlerFactory<LocalTime>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<LocalTime> Create(NpgsqlConnection conn)
            => conn.HasIntegerDateTimes
                ? new TimeHandler()
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    class TimeHandler : NpgsqlSimpleTypeHandler<LocalTime>
    {
        // PostgreSQL time resolution == 1 microsecond == 10 ticks
        public override LocalTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10);

        public override int ValidateAndGetLength(LocalTime value, NpgsqlParameter parameter)
            => 8;

        public override void Write(LocalTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value.TickOfDay / 10);
    }
}
