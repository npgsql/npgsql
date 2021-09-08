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
    /// A type handler for the PostgreSQL time data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class TimeHandler : NpgsqlSimpleTypeHandler<TimeSpan>
#if NET6_0_OR_GREATER
        , INpgsqlSimpleTypeHandler<TimeOnly>
#endif
    {
        /// <summary>
        /// Constructs a <see cref="TimeHandler"/>.
        /// </summary>
        public TimeHandler(PostgresType postgresType) : base(postgresType) {}

        // PostgreSQL time resolution == 1 microsecond == 10 ticks
        /// <inheritdoc />
        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new(buf.ReadInt64() * 10);

        /// <inheritdoc />
        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteInt64(value.Ticks / 10);

#if NET6_0_OR_GREATER
        TimeOnly INpgsqlSimpleTypeHandler<TimeOnly>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => new(buf.ReadInt64() * 10);

        public int ValidateAndGetLength(TimeOnly value, NpgsqlParameter? parameter) => 8;

        public void Write(TimeOnly value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteInt64(value.Ticks / 10);
#endif
    }
}
