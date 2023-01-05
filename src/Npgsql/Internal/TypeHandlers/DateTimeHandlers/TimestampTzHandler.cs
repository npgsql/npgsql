using System;
using System.Diagnostics;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
using NpgsqlTypes;
using static Npgsql.Util.Statics;
using static Npgsql.Internal.TypeHandlers.DateTimeHandlers.DateTimeUtils;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers;

/// <summary>
/// A type handler for the PostgreSQL timestamptz data type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public partial class TimestampTzHandler : NpgsqlSimpleTypeHandler<DateTime>,
    INpgsqlSimpleTypeHandler<DateTimeOffset>, INpgsqlSimpleTypeHandler<long>
{
    /// <summary>
    /// Constructs an <see cref="TimestampTzHandler"/>.
    /// </summary>
    public TimestampTzHandler(PostgresType postgresType) : base(postgresType) {}

    /// <inheritdoc />
    public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
        => new RangeHandler<DateTime, DateTimeOffset>(pgRangeType, this);

    #region Read

    /// <inheritdoc />
    public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
    {
        var dateTime = ReadDateTime(buf, DateTimeKind.Utc);
        return LegacyTimestampBehavior && (DisableDateTimeInfinityConversions || dateTime != DateTime.MaxValue && dateTime != DateTime.MinValue)
            ? dateTime.ToLocalTime()
            : dateTime;
    }

    DateTimeOffset INpgsqlSimpleTypeHandler<DateTimeOffset>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
    {
        try
        {
            var value = buf.ReadInt64();
            switch (value)
            {
            case long.MaxValue:
                return DisableDateTimeInfinityConversions
                    ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                    : DateTimeOffset.MaxValue;
            case long.MinValue:
                return DisableDateTimeInfinityConversions
                    ? throw new InvalidCastException(NpgsqlStrings.CannotReadInfinityValue)
                    : DateTimeOffset.MinValue;
            default:
                var dateTime = DecodeTimestamp(value, DateTimeKind.Utc);
                return LegacyTimestampBehavior ? dateTime.ToLocalTime() : dateTime;
            }
        }
        catch (ArgumentOutOfRangeException e)
        {
            throw new InvalidCastException("Out of the range of DateTime (year must be between 1 and 9999)", e);
        }
    }

    long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        => buf.ReadInt64();

    #endregion Read

    #region Write

    /// <inheritdoc />
    public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
        => value.Kind == DateTimeKind.Utc ||
           value == DateTime.MinValue || // Allowed since this is default(DateTime) - sent without any timezone conversion.
           value == DateTime.MaxValue && !DisableDateTimeInfinityConversions ||
           LegacyTimestampBehavior
            ? 8
            : throw new InvalidCastException(
                $"Cannot write DateTime with Kind={value.Kind} to PostgreSQL type 'timestamp with time zone', only UTC is supported. " +
                "Note that it's not possible to mix DateTimes with different Kinds in an array/range. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");

    /// <inheritdoc />
    public int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter)
        => value.Offset == TimeSpan.Zero || LegacyTimestampBehavior
            ? 8
            : throw new InvalidCastException(
                $"Cannot write DateTimeOffset with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', " +
                "only offset 0 (UTC) is supported. " +
                "Note that it's not possible to mix DateTimes with different Kinds in an array/range. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");

    /// <inheritdoc />
    public int ValidateAndGetLength(long value, NpgsqlParameter? parameter) => 8;

    /// <inheritdoc />
    public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        if (LegacyTimestampBehavior)
        {
            switch (value.Kind)
            {
            case DateTimeKind.Unspecified:
            case DateTimeKind.Utc:
                break;
            case DateTimeKind.Local:
                value = value.ToUniversalTime();
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {value.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
            }
        }
        else
            Debug.Assert(value.Kind == DateTimeKind.Utc || value == DateTime.MinValue || value == DateTime.MaxValue);

        WriteTimestamp(value, buf);
    }

    /// <inheritdoc />
    public void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        if (LegacyTimestampBehavior)
            value = value.ToUniversalTime();

        Debug.Assert(value.Offset == TimeSpan.Zero);

        WriteTimestamp(value.DateTime, buf);
    }

    /// <inheritdoc />
    public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        => buf.WriteInt64(value);

    #endregion Write
}