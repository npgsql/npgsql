using System;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL timetz data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class TimeTzHandler : NpgsqlSimpleTypeHandler<DateTimeOffset>
    {
        // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative

        /// <summary>
        /// Constructs an <see cref="TimeTzHandler"/>.
        /// </summary>
        public TimeTzHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override DateTimeOffset Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var ticks = buf.ReadInt64() * 10;
            var offset = new TimeSpan(0, 0, -buf.ReadInt32());
            return new DateTimeOffset(ticks + TimeSpan.TicksPerDay, offset);
        }

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter? parameter) => 12;

        /// <inheritdoc />
        public override void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteInt64(value.TimeOfDay.Ticks / 10);
            buf.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
        }

        #endregion Write
    }
}
