using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL time data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("time without time zone", NpgsqlDbType.Time, new[] { DbType.Time })]
    public class TimeHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        /// <inheritdoc />
        public override NpgsqlTypeHandler<TimeSpan> Create(PostgresType postgresType, NpgsqlConnection conn)
            => conn.HasIntegerDateTimes  // Check for the legacy floating point timestamps feature
                ? new TimeHandler(postgresType)
                : throw new NotSupportedException($"The deprecated floating-point date/time format is not supported by {nameof(Npgsql)}.");
    }

    /// <summary>
    /// A type handler for the PostgreSQL time data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class TimeHandler : NpgsqlSimpleTypeHandler<TimeSpan>
    {
        /// <summary>
        /// Constructs a <see cref="TimeHandler"/>.
        /// </summary>
        public TimeHandler(PostgresType postgresType) : base(postgresType) {}

        // PostgreSQL time resolution == 1 microsecond == 10 ticks
        /// <inheritdoc />
        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new TimeSpan(buf.ReadInt64() * 10);

        /// <inheritdoc />
        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter)
            => 8;

        /// <inheritdoc />
        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteInt64(value.Ticks / 10);
    }
}
