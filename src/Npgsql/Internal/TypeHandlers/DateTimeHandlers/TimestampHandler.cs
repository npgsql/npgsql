using System;
using System.Data;
using System.Runtime.CompilerServices;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL timestamp data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TimestampHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<DateTime> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new TimestampHandler(postgresType, conn.Connector!.ConvertInfinityDateTime)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL timestamp data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class TimestampHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlTimestamp>
    {
        readonly bool _convertInfinityDateTime;

        /// <summary>
        /// Constructs a <see cref="TimestampHandler"/>.
        /// </summary>
        public TimestampHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType) => _convertInfinityDateTime = convertInfinityDateTime;

        /// <inheritdoc />
        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var value = ReadPsv(buf, len, fieldDescription);
            return _convertInfinityDateTime && NpgsqlTimestamp.IsInfinity(value)
                ? value == NpgsqlTimestamp.PositiveInfinity
                    ? DateTime.MaxValue
                    : DateTime.MinValue
                : (DateTime)value;
        }

        /// <inheritdoc />
        protected override NpgsqlTimestamp ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null) =>
            new NpgsqlTimestamp(buf.ReadInt64());

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlTimestamp value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) =>
            Write(
                _convertInfinityDateTime
                    ? value == DateTime.MinValue ? NpgsqlTimestamp.NegativeInfinity
                    : value == DateTime.MaxValue ? NpgsqlTimestamp.PositiveInfinity
                    : new NpgsqlTimestamp(value.Ticks)
                    : new NpgsqlTimestamp(value.Ticks),
                buf,
                parameter);

        /// <inheritdoc />
        public override void Write(NpgsqlTimestamp value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) =>
            buf.WriteInt64(value.Microseconds);
    }
}
