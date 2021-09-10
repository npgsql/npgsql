using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using static Npgsql.Util.Statics;
using static Npgsql.Internal.TypeHandlers.DateTimeHandlers.DateTimeUtils;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers
{
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
    public partial class TimestampTzHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDateTime>, INpgsqlSimpleTypeHandler<DateTimeOffset>
    {
        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        protected readonly bool ConvertInfinityDateTime;

        /// <summary>
        /// Constructs an <see cref="TimestampTzHandler"/>.
        /// </summary>
        public TimestampTzHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
            => ConvertInfinityDateTime = convertInfinityDateTime;

        /// <inheritdoc />
        public override NpgsqlTypeHandler CreateRangeHandler(PostgresType pgRangeType)
            => new RangeHandler<DateTime, DateTimeOffset>(pgRangeType, this);

        #region Read

        private protected const string InfinityExceptionMessage = "Can't convert infinite timestamp values to DateTime";
        private protected const string OutOfRangeExceptionMessage = "Out of the range of DateTime (year must be between 1 and 9999)";

        /// <inheritdoc />
        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var dateTime = ReadDateTime(buf, ConvertInfinityDateTime, DateTimeKind.Utc);
            return LegacyTimestampBehavior && (!ConvertInfinityDateTime || dateTime != DateTime.MaxValue && dateTime != DateTime.MinValue)
                ? dateTime.ToLocalTime()
                : dateTime;
        }

        /// <inheritdoc />
        protected override NpgsqlDateTime ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var ts = ReadNpgsqlDateTime(buf, len, fieldDescription);

            if (!ts.IsFinite)
                return ts;

            var npgsqlDateTime = new NpgsqlDateTime(ts.Date, ts.Time, DateTimeKind.Utc);
            return LegacyTimestampBehavior ? npgsqlDateTime.ToLocalTime() : npgsqlDateTime;
        }

        DateTimeOffset INpgsqlSimpleTypeHandler<DateTimeOffset>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            try
            {
                var value = buf.ReadInt64();
                switch (value)
                {
                case long.MaxValue:
                    return ConvertInfinityDateTime
                        ? DateTimeOffset.MaxValue
                        : throw new InvalidCastException(InfinityExceptionMessage);
                case long.MinValue:
                    return ConvertInfinityDateTime
                        ? DateTimeOffset.MinValue
                        : throw new InvalidCastException(InfinityExceptionMessage);
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

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
        {
            if (LegacyTimestampBehavior)
                throw new Exception("Legacy mode is on in NodaTime");

            if (value.Kind != DateTimeKind.Utc)
                throw new Exception("Kind is: " + value.Kind);

            if (ConvertInfinityDateTime)
                throw new Exception("ConvertInfinityDateTime is true");

            if (!LegacyTimestampBehavior && value.Kind != DateTimeKind.Utc &&
                (!ConvertInfinityDateTime || value != DateTime.MinValue && value != DateTime.MaxValue))
            {
                throw new InvalidCastException(
                    $"Cannot write DateTime with Kind={value.Kind} to PostgreSQL type 'timestamp with time zone', only UTC is supported. " +
                    "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
            }

            return 8;
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlDateTime value, NpgsqlParameter? parameter)
        {
            if (!LegacyTimestampBehavior && value.Kind != DateTimeKind.Utc && value.IsFinite)
            {
                throw new InvalidCastException(
                    $"Cannot write DateTime with Kind={value.Kind} to PostgreSQL type 'timestamp with time zone', only UTC is supported. " +
                    "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
            }

            return 8;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter)
        {
            if (!LegacyTimestampBehavior && value.Offset != TimeSpan.Zero)
            {
                throw new InvalidCastException(
                    $"Cannot write DateTimeOffset with Offset={value.Offset} to PostgreSQL type 'timestamp with time zone', only offset 0 (UTC) is supported. " +
                    "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");
            }

            return 8;
        }

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
                Debug.Assert(value.Kind == DateTimeKind.Utc || (ConvertInfinityDateTime && (value == DateTime.MinValue || value == DateTime.MaxValue)));

            WriteTimestamp(value, buf, ConvertInfinityDateTime);
        }

        /// <inheritdoc />
        public override void Write(NpgsqlDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
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
                Debug.Assert(value.Kind == DateTimeKind.Utc || !value.IsFinite);

            WriteTimestamp(value, buf, ConvertInfinityDateTime);
        }

        /// <inheritdoc />
        public void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (LegacyTimestampBehavior)
                value = value.ToUniversalTime();

            Debug.Assert(value.Offset == TimeSpan.Zero);

            WriteTimestamp(value.DateTime, buf, ConvertInfinityDateTime);
        }

        #endregion Write
    }
}
