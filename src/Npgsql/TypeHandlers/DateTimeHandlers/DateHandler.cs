using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("date", NpgsqlDbType.Date, DbType.Date, typeof(NpgsqlDate))]
    internal class DateHandler : TypeHandlerWithPsv<DateTime, NpgsqlDate>,
        ISimpleTypeReader<DateTime>, ISimpleTypeReader<NpgsqlDate>, ISimpleTypeWriter
    {
        internal const int PostgresEpochJdate = 2451545; // == date2j(2000, 1, 1)
        internal const int MonthsPerYear = 12;

        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        public DateHandler(TypeHandlerRegistry registry)
        {
            _convertInfinityDateTime = registry.Connector.ConvertInfinityDateTime;
        }

        public DateTime Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlDate?
            var npgsqlDate = ((ISimpleTypeReader<NpgsqlDate>) this).Read(buf, len, fieldDescription);
            try {
                if (npgsqlDate.IsFinite)
                    return (DateTime)npgsqlDate;
                if (!_convertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite date values to DateTime");
                if (npgsqlDate.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            } catch (Exception e) {
                throw new SafeReadException(e);
            }
        }

        /// <remarks>
        /// Copied wholesale from Postgresql backend/utils/adt/datetime.c:j2date
        /// </remarks>
        NpgsqlDate ISimpleTypeReader<NpgsqlDate>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
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

        public int ValidateAndGetLength(object value) { return 4; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            NpgsqlDate date;
            if (value is NpgsqlDate)
            {
                date = (NpgsqlDate)value;
            }
            else if (value is DateTime)
            {
                var dt = (DateTime)value;
                if (_convertInfinityDateTime)
                {
                    if (dt == DateTime.MaxValue)
                    {
                        date = NpgsqlDate.Infinity;
                    }
                    else if (dt == DateTime.MinValue)
                    {
                        date = NpgsqlDate.NegativeInfinity;
                    }
                    else
                    {
                        date = new NpgsqlDate(dt);
                    }
                }
                else
                {
                    date = new NpgsqlDate(dt);
                }
            }
            else if (value is string)
            {
                date = NpgsqlDate.Parse((string) value);
            }
            else
            {
                throw new InvalidCastException();
            }

            if (date == NpgsqlDate.NegativeInfinity)
                buf.WriteInt32(int.MinValue);
            else if (date == NpgsqlDate.Infinity)
                buf.WriteInt32(int.MaxValue);
            else
                buf.WriteInt32(date.DaysSinceEra - 730119);
        }
    }
}
