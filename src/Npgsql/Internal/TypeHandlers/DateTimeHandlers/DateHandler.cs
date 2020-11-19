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
    /// A factory for type handlers for the PostgreSQL date data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class DateHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<DateTime> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new DateHandler(postgresType, conn.Connector!.ConvertInfinityDateTime);
    }

    /// <summary>
    /// A type handler for the PostgreSQL date data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class DateHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDate>
    {
        readonly bool _convertInfinityDateTime;

        /// <summary>
        /// Constructs a <see cref="DateHandler"/>
        /// </summary>
        public DateHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType) => _convertInfinityDateTime = convertInfinityDateTime;

        /// <inheritdoc />
        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var value = ReadPsv(buf, len, fieldDescription);
            return _convertInfinityDateTime && NpgsqlDate.IsInfinity(value)
                ? value == NpgsqlDate.PositiveInfinity
                    ? DateTime.MaxValue
                    : DateTime.MinValue
                : default;
        }

        /// <inheritdoc />
        protected override NpgsqlDate ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null) =>
            new NpgsqlDate(buf.ReadInt32());

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter) => 4;

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlDate value, NpgsqlParameter? parameter) => 4;

        /// <inheritdoc />
        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) =>
            Write(
                _convertInfinityDateTime
                    ? value == DateTime.MinValue ? NpgsqlDate.NegativeInfinity
                    : value == DateTime.MaxValue ? NpgsqlDate.PositiveInfinity
                    : default
                    : default,
                buf,
                parameter);

        /// <inheritdoc />
        public override void Write(NpgsqlDate value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) =>
            buf.WriteInt32(value.Days);
    }
}
