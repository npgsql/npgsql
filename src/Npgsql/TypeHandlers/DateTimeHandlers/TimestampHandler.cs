#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("timestamp", NpgsqlDbType.Timestamp, new[] { DbType.DateTime, DbType.DateTime2 }, new[] { typeof(NpgsqlDateTime), typeof(DateTime) }, DbType.DateTime)]
    class TimestampHandlerFactory : TypeHandlerFactory
    {
        // Check for the legacy floating point timestamps feature
        protected override TypeHandler Create(NpgsqlConnection conn)
            => new TimestampHandler(conn.HasIntegerDateTimes, conn.Connector.ConvertInfinityDateTime);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimestampHandler : SimpleTypeHandlerWithPsv<DateTime, NpgsqlDateTime>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        protected readonly bool ConvertInfinityDateTime;

        internal TimestampHandler(bool integerFormat, bool convertInfinityDateTime)
        {
            // Check for the legacy floating point timestamps feature, defaulting to integer timestamps
            _integerFormat = integerFormat;
            ConvertInfinityDateTime = convertInfinityDateTime;
        }

        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            try
            {
                if (ts.IsFinite)
                    return ts.ToDateTime();
                if (!ConvertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite timestamp values to DateTime");
                if (ts.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            }
            catch (Exception e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        protected override NpgsqlDateTime ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ReadTimeStamp(buf, len, fieldDescription);

        protected NpgsqlDateTime ReadTimeStamp(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            CheckIntegerFormat();

            var value = buf.ReadInt64();
            if (value == long.MaxValue)
                return NpgsqlDateTime.Infinity;
            if (value == long.MinValue)
                return NpgsqlDateTime.NegativeInfinity;
            if (value >= 0) {
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;

                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlDateTime(new NpgsqlDate(date), new TimeSpan(time));
            } else {
                value = -value;
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;
                if (time != 0) {
                    ++date;
                    time = 86400000000L - time;
                }
                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlDateTime(new NpgsqlDate(date), new TimeSpan(time));
            }
        }

        protected override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            CheckIntegerFormat();

            if (!(value is DateTime) && !(value is NpgsqlDateTime) && !(value is DateTimeOffset))
            {
                var converted = Convert.ToDateTime(value);
                if (parameter == null)
                    throw CreateConversionButNoParamException(value.GetType());
                parameter.ConvertedValue = converted;
            }
            return 8;
        }

        protected override void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;

            NpgsqlDateTime ts;
            if (value is NpgsqlDateTime) {
                ts = (NpgsqlDateTime)value;
                if (!ts.IsFinite)
                {
                    if (ts.IsInfinity)
                    {
                        buf.WriteInt64(long.MaxValue);
                        return;
                    }

                    if (ts.IsNegativeInfinity)
                    {
                        buf.WriteInt64(long.MinValue);
                        return;
                    }

                    throw new InvalidOperationException("Internal Npgsql bug, please report.");
                }
            }
            else if (value is DateTime)
            {
                var dt = (DateTime)value;
                if (ConvertInfinityDateTime)
                {
                    if (dt == DateTime.MaxValue)
                    {
                        buf.WriteInt64(long.MaxValue);
                        return;
                    }
                    if (dt == DateTime.MinValue)
                    {
                        buf.WriteInt64(long.MinValue);
                        return;
                    }
                }
                ts = new NpgsqlDateTime(dt);
            }
            else if (value is DateTimeOffset)
                ts = new NpgsqlDateTime(((DateTimeOffset)value).DateTime);
            else
                throw new InvalidOperationException("Internal Npgsql bug, please report.");

            var uSecsTime = ts.Time.Ticks / 10;

            if (ts >= new NpgsqlDateTime(2000, 1, 1, 0, 0, 0))
            {
                var uSecsDate = (ts.Date.DaysSinceEra - 730119) * 86400000000L;
                buf.WriteInt64(uSecsDate + uSecsTime);
            }
            else
            {
                var uSecsDate = (730119 - ts.Date.DaysSinceEra) * 86400000000L;
                buf.WriteInt64(-(uSecsDate - uSecsTime));
            }
        }

        void CheckIntegerFormat()
        {
            if (!_integerFormat)
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
        }
    }
}
