using System;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL interval data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("interval", NpgsqlDbType.Interval, new[] { typeof(TimeSpan), typeof(NpgsqlTimeSpan) })]
    public class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<TimeSpan> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new IntervalHandler(postgresType)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL date interval type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class IntervalHandler : NpgsqlSimpleTypeHandlerWithPsv<TimeSpan, NpgsqlTimeSpan>
    {
        /// <summary>
        /// Constructs an <see cref="IntervalHandler"/>
        /// </summary>
        public IntervalHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => (TimeSpan)((INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>)this).Read(buf, len, fieldDescription);

        /// <inheritdoc />
        protected override NpgsqlTimeSpan ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var ticks = buf.ReadInt64();
            var day = buf.ReadInt32();
            var month = buf.ReadInt32();
            return new NpgsqlTimeSpan(month, day, ticks * 10);
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter) => 16;

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlTimeSpan value, NpgsqlParameter? parameter) => 16;

        /// <inheritdoc />
        public override void Write(NpgsqlTimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteInt64(value.Ticks / 10); // TODO: round?
            buf.WriteInt32(value.Days);
            buf.WriteInt32(value.Months);
        }

        // TODO: Can write directly from TimeSpan
        /// <inheritdoc />
        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => Write(value, buf, parameter);
    }
}
