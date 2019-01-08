using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class DateHandlerFactory : NpgsqlTypeHandlerFactory<LocalDate>
    {
        protected override NpgsqlTypeHandler<LocalDate> Create(NpgsqlConnection conn)
        {
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            return new DateHandler(csb.ConvertInfinityDateTime);
        }
    }

    sealed class DateHandler : NpgsqlSimpleTypeHandler<LocalDate>
    {
        /// <summary>
        /// Whether to convert positive and negative infinity values to Instant.{Max,Min}Value when
        /// an Instant is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        internal DateHandler(bool convertInfinityDateTime)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
        }

        public override LocalDate Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
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

        public override int ValidateAndGetLength(LocalDate value, NpgsqlParameter parameter)
            => 4;

        public override void Write(LocalDate value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
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
    }
}
