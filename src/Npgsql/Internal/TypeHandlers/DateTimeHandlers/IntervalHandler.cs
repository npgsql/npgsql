﻿using System;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers;

/// <summary>
/// A type handler for the PostgreSQL date interval type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public partial class IntervalHandler : NpgsqlSimpleTypeHandler<TimeSpan>, INpgsqlSimpleTypeHandler<NpgsqlInterval>
{
    /// <summary>
    /// Constructs an <see cref="IntervalHandler"/>
    /// </summary>
    public IntervalHandler(PostgresType postgresType) : base(postgresType) {}

    /// <inheritdoc />
    public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
    {
        var microseconds = buf.ReadInt64();
        var days = buf.ReadInt32();
        var months = buf.ReadInt32();

        if (months > 0)
            throw new InvalidCastException("Cannot convert interval value with non-zero months to TimeSpan");

        return new(microseconds * 10 + days * TimeSpan.TicksPerDay);
    }

    NpgsqlInterval INpgsqlSimpleTypeHandler<NpgsqlInterval>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
    {
        var ticks = buf.ReadInt64();
        var day = buf.ReadInt32();
        var month = buf.ReadInt32();
        return new NpgsqlInterval(month, day, ticks);
    }

    /// <inheritdoc />
    public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter) => 16;

    /// <inheritdoc />
    public int ValidateAndGetLength(NpgsqlInterval value, NpgsqlParameter? parameter) => 16;

    /// <inheritdoc />
    public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        var ticksInDay = value.Ticks - TimeSpan.TicksPerDay * value.Days;

        buf.WriteInt64(ticksInDay / 10);
        buf.WriteInt32(value.Days);
        buf.WriteInt32(0);
    }

    public void Write(NpgsqlInterval value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        buf.WriteInt64(value.Time);
        buf.WriteInt32(value.Days);
        buf.WriteInt32(value.Months);
    }
}