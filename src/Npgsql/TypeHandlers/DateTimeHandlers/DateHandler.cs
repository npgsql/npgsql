using System;
using System.Data;
using System.Runtime.CompilerServices;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <summary>
    /// A factory for type handlers for the PostgreSQL date data type.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("date", NpgsqlDbType.Date, DbType.Date
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
        , typeof(NpgsqlDate)
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes
        )]
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
    /// See http://www.postgresql.org/docs/current/static/datatype-datetime.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class DateHandler :
#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
        NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDate>
#pragma warning restore 618
#else
        NpgsqlSimpleTypeHandler<DateTime>
#endif // LegacyProviderSpecificDateTimeTypes
    {
        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        /// <summary>
        /// Constructs a <see cref="DateHandler"/>
        /// </summary>
        public DateHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
            => _convertInfinityDateTime = convertInfinityDateTime;

        #region Read

        /// <inheritdoc />
        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            const string infinityExceptionMessage = "Can't convert infinite date values to DateTime";
            const string outOfRangeExceptionMessage = "Out of the range of DateTime (year must be between 1 and 9999)";

            var postgresDate = buf.ReadInt32();

            switch (postgresDate)
            {
            case int.MaxValue:
                return _convertInfinityDateTime
                    ? DateTime.MaxValue
                    : throw new InvalidCastException(infinityExceptionMessage);
            case int.MinValue:
                return _convertInfinityDateTime
                    ? DateTime.MinValue
                    : throw new InvalidCastException(infinityExceptionMessage);
            default:
                try
                {
                    return FromPostgresDate(postgresDate);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    throw new InvalidCastException(outOfRangeExceptionMessage, e);
                }
            }
        }

#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
        /// <remarks>
        /// Copied wholesale from Postgresql backend/utils/adt/datetime.c:j2date
        /// </remarks>
        protected override NpgsqlDate ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var binDate = buf.ReadInt32();

            return binDate switch
            {
                int.MaxValue => NpgsqlDate.Infinity,
                int.MinValue => NpgsqlDate.NegativeInfinity,
                _            => new NpgsqlDate(binDate + 730119)
            };
        }
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter) => 4;

#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlDate value, NpgsqlParameter? parameter) => 4;
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

        /// <inheritdoc />
        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (_convertInfinityDateTime)
            {
                if (value == DateTime.MaxValue)
                    buf.WriteInt32(int.MaxValue);
                else if (value == DateTime.MinValue)
                    buf.WriteInt32(int.MinValue);
                else
                    buf.WriteInt32(ToPostgresDate(value));
            }
            else
                buf.WriteInt32(ToPostgresDate(value));
        }

#if LegacyProviderSpecificDateTimeTypes
#pragma warning disable 618
        /// <inheritdoc />
        public override void Write(NpgsqlDate value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (value == NpgsqlDate.NegativeInfinity)
                buf.WriteInt32(int.MinValue);
            else if (value == NpgsqlDate.Infinity)
                buf.WriteInt32(int.MaxValue);
            else
                buf.WriteInt32(value.DaysSinceEra - 730119);
        }
#pragma warning restore 618
#endif // LegacyProviderSpecificDateTimeTypes

        #endregion Write

        const int PostgresDateOffsetDays = 730119;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int ToPostgresDate(DateTime value)
            => (int)(value.Ticks / TimeSpan.TicksPerDay) - PostgresDateOffsetDays;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static DateTime FromPostgresDate(int value)
            => new DateTime((value + PostgresDateOffsetDays) * TimeSpan.TicksPerDay);
    }
}
