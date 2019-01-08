using System;
using System.Data;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("date", NpgsqlDbType.Date, DbType.Date, typeof(NpgsqlDate))]
    class DateHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        protected override NpgsqlTypeHandler<DateTime> Create(NpgsqlConnection conn)
            => new DateHandler(conn.Connector.ConvertInfinityDateTime);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class DateHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDate>
    {
        internal const int PostgresEpochJdate = 2451545; // == date2j(2000, 1, 1)
        internal const int MonthsPerYear = 12;

        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        public DateHandler(bool convertInfinityDateTime)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
        }

        #region Read

        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var npgsqlDate = ReadPsv(buf, len, fieldDescription);
            try {
                if (npgsqlDate.IsFinite)
                    return (DateTime)npgsqlDate;
                if (!_convertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite date values to DateTime");
                if (npgsqlDate.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            } catch (Exception e) {
                throw new NpgsqlSafeReadException(e);
            }
        }

        /// <remarks>
        /// Copied wholesale from Postgresql backend/utils/adt/datetime.c:j2date
        /// </remarks>
        protected override NpgsqlDate ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var binDate = buf.ReadInt32();

            switch (binDate)
            {
            case int.MaxValue:
                return NpgsqlDate.Infinity;
            case int.MinValue:
                return NpgsqlDate.NegativeInfinity;
            default:
                return new NpgsqlDate(binDate + 730119);
            }
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter parameter)
            => 4;

        public override int ValidateAndGetLength(NpgsqlDate value, NpgsqlParameter parameter)
            => 4;

        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            NpgsqlDate value2;
            if (_convertInfinityDateTime)
            {
                if (value == DateTime.MaxValue)
                    value2 = NpgsqlDate.Infinity;
                else if (value == DateTime.MinValue)
                    value2 = NpgsqlDate.NegativeInfinity;
                else
                    value2 = new NpgsqlDate(value);
            }
            else
                value2 = new NpgsqlDate(value);

            Write(value2, buf, parameter);
        }

        public override void Write(NpgsqlDate value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (value == NpgsqlDate.NegativeInfinity)
                buf.WriteInt32(int.MinValue);
            else if (value == NpgsqlDate.Infinity)
                buf.WriteInt32(int.MaxValue);
            else
                buf.WriteInt32(value.DaysSinceEra - 730119);
        }

        #endregion Write
    }
}
