using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL timestamptz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TimestampTzHandlerFactory : NpgsqlTypeHandlerFactory<DateTimeOffset>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<DateTimeOffset> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new TimestampTzHandler(postgresType, conn.Connector!.ConvertInfinityDateTime)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL timestamptz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class TimestampTzHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTimeOffset, NpgsqlTimestamptz>
    {
        readonly bool _convertInfinityDateTime;

        /// <summary>
        /// Constructs a <see cref="TimestampHandler"/>.
        /// </summary>
        public TimestampTzHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType) => _convertInfinityDateTime = convertInfinityDateTime;

        /// <inheritdoc />
        public override DateTimeOffset Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var value = ReadPsv(buf, len, fieldDescription);
            return _convertInfinityDateTime && NpgsqlTimestamptz.IsInfinity(value)
                ? value == NpgsqlTimestamptz.PositiveInfinity
                    ? DateTimeOffset.MaxValue
                    : DateTimeOffset.MinValue
                : (DateTimeOffset)value;
        }

        /// <inheritdoc />
        protected override NpgsqlTimestamptz ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null) =>
            new NpgsqlTimestamptz(buf.ReadInt64());

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlTimestamptz value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) =>
            Write(
                _convertInfinityDateTime
                    ? value == DateTimeOffset.MinValue ? NpgsqlTimestamptz.NegativeInfinity
                    : value == DateTimeOffset.MaxValue ? NpgsqlTimestamptz.PositiveInfinity
                    : (NpgsqlTimestamptz)value
                    : (NpgsqlTimestamptz)value,
                buf,
                parameter);

        /// <inheritdoc />
        public override void Write(NpgsqlTimestamptz value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) =>
            buf.WriteInt64(value.Microseconds);
    }
}
