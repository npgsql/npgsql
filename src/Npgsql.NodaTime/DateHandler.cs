using System;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using NpgsqlTypes;
using BclDateHandler = Npgsql.TypeHandlers.DateTimeHandlers.DateHandler;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class DateHandlerFactory : NpgsqlTypeHandlerFactory<LocalDate>
    {
        public override NpgsqlTypeHandler<LocalDate> Create(PostgresType postgresType, NpgsqlConnection conn)
        {
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            return new DateHandler(postgresType, csb.ConvertInfinityDateTime);
        }
    }

    sealed class DateHandler : NpgsqlSimpleTypeHandler<LocalDate>, INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<NpgsqlDate>
    {
        /// <summary>
        /// Whether to convert positive and negative infinity values to Instant.{Max,Min}Value when
        /// an Instant is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;
        readonly BclDateHandler _bclHandler;

        internal DateHandler(PostgresType postgresType, bool convertInfinityDateTime)
            : base(postgresType)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
            _bclHandler = new BclDateHandler(postgresType, convertInfinityDateTime);
        }

        public override LocalDate Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var value = buf.ReadInt32();
            if (_convertInfinityDateTime)
            {
                if (value == int.MaxValue)
                    return LocalDate.MaxIsoValue;
                if (value == int.MinValue)
                    return LocalDate.MinIsoValue;
            }
            return new LocalDate().PlusDays(value + 730119);
        }

        public override int ValidateAndGetLength(LocalDate value, NpgsqlParameter? parameter)
            => 4;

        public override void Write(LocalDate value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (_convertInfinityDateTime)
            {
                if (value == LocalDate.MaxIsoValue)
                {
                    buf.WriteInt32(int.MaxValue);
                    return;
                }
                if (value == LocalDate.MinIsoValue)
                {
                    buf.WriteInt32(int.MinValue);
                    return;
                }
            }

            var totalDaysSinceEra = Period.Between(default(LocalDate), value, PeriodUnits.Days).Days;
            buf.WriteInt32(totalDaysSinceEra - 730119);
        }

        NpgsqlDate INpgsqlSimpleTypeHandler<NpgsqlDate>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<NpgsqlDate>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<NpgsqlDate>.ValidateAndGetLength(NpgsqlDate value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<NpgsqlDate>.Write(NpgsqlDate value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => _bclHandler.Read<DateTime>(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<DateTime>.ValidateAndGetLength(DateTime value, NpgsqlParameter? parameter)
            => _bclHandler.ValidateAndGetLength(value, parameter);

        void INpgsqlSimpleTypeHandler<DateTime>.Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => _bclHandler.Write(value, buf, parameter);
    }
}
