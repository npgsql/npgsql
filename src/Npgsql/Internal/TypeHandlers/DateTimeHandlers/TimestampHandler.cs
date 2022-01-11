using System;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using static Npgsql.Util.Statics;
using static Npgsql.Internal.TypeHandlers.DateTimeHandlers.DateTimeUtils;

namespace Npgsql.Internal.TypeHandlers.DateTimeHandlers;

/// <summary>
/// A type handler for the PostgreSQL timestamp data type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-datetime.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public partial class TimestampHandler : NpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<long>
{
    /// <summary>
    /// Constructs a <see cref="TimestampHandler"/>.
    /// </summary>
    public TimestampHandler(PostgresType postgresType) : base(postgresType) {}

    #region Read

    /// <inheritdoc />
    public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        => ReadDateTime(buf, DateTimeKind.Unspecified);

    long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        => buf.ReadInt64();

    #endregion Read

    #region Write

    /// <inheritdoc />
    public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
        => value.Kind != DateTimeKind.Utc || LegacyTimestampBehavior
            ? 8
            : throw new InvalidCastException(
                "Cannot write DateTime with Kind=UTC to PostgreSQL type 'timestamp without time zone', " +
                "consider using 'timestamp with time zone'. " +
                "Note that it's not possible to mix DateTimes with different Kinds in an array/range. " +
                "See the Npgsql.EnableLegacyTimestampBehavior AppContext switch to enable legacy behavior.");

    /// <inheritdoc />
    public int ValidateAndGetLength(long value, NpgsqlParameter? parameter) => 8;

    /// <inheritdoc />
    public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        => WriteTimestamp(value, buf);

    /// <inheritdoc />
    public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        => buf.WriteInt64(value);

    #endregion Write
}