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
using System.Diagnostics;
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimestampHandlerFactory : NpgsqlTypeHandlerFactory<Instant>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<Instant> Create(NpgsqlConnection conn)
        {
            var csb = new NpgsqlConnectionStringBuilder(conn.ConnectionString);
            return new TimestampHandler(conn.HasIntegerDateTimes, csb.ConvertInfinityDateTime);
        }
    }

    class TimestampHandler : NpgsqlSimpleTypeHandler<Instant>, INpgsqlSimpleTypeHandler<LocalDateTime>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        /// <summary>
        /// Whether to convert positive and negative infinity values to Instant.{Max,Min}Value when
        /// an Instant is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        internal TimestampHandler(bool integerFormat, bool convertInfinityDateTime)
        {
            _integerFormat = integerFormat;
            _convertInfinityDateTime = convertInfinityDateTime;
        }

        public override Instant Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (_integerFormat)
            {
                var value = buf.ReadInt64();
                if (_convertInfinityDateTime)
                {
                    if (value == long.MaxValue)
                        return Instant.MaxValue;
                    if (value == long.MinValue)
                        return Instant.MinValue;
                }
                return DecodeZonedDateTimeUsingIntegerFormat(value).ToInstant();
            }
            else
            {
                var value = buf.ReadDouble();
                if (_convertInfinityDateTime)
                {
                    if (value == double.PositiveInfinity)
                        return Instant.MaxValue;
                    if (value == double.NegativeInfinity)
                        return Instant.MinValue;
                }
                return DecodeZonedDateTimeUsingFloatingPointFormat(value).ToInstant();
            }
        }

        LocalDateTime INpgsqlSimpleTypeHandler<LocalDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            if (_integerFormat)
            {
                var value = buf.ReadInt64();
                if (value == long.MaxValue || value == long.MinValue)
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported when reading ZonedDateTime, read as Instant instead"));
                return DecodeZonedDateTimeUsingIntegerFormat(value).LocalDateTime;
            }
            else
            {
                var value = buf.ReadDouble();
                if (value == double.PositiveInfinity || value == double.NegativeInfinity)
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported when reading ZonedDateTime, read as Instant instead"));
                return DecodeZonedDateTimeUsingFloatingPointFormat(value).LocalDateTime;
            }
        }

        internal static ZonedDateTime DecodeZonedDateTimeUsingIntegerFormat(long value)
        {
            Debug.Assert(value != long.MaxValue && value != long.MinValue);

            if (value >= 0)
            {
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;

                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 1000;   // From microseconds to nanoseconds

                return new ZonedDateTime().Plus(Duration.FromDays(date) + Duration.FromNanoseconds(time));
            }
            else
            {
                value = -value;
                var date = (int)(value / 86400000000L);
                var time = value % 86400000000L;
                if (time != 0)
                {
                    ++date;
                    time = 86400000000L - time;
                }
                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 1000;           // From microseconds to nanoseconds

                return new ZonedDateTime().Plus(Duration.FromDays(date) + Duration.FromNanoseconds(time));
            }
        }
        
        internal static ZonedDateTime DecodeZonedDateTimeUsingFloatingPointFormat(double value)
        {
            Debug.Assert(value != double.PositiveInfinity && value != double.NegativeInfinity);

            if (value >= 0d)
            {
                var date = (int)value / 86400;
                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                var microsecondOfDay = (long)((value % 86400d) * 1000000d);
                
                return new ZonedDateTime().Plus(Duration.FromDays(date) + Duration.FromNanoseconds(microsecondOfDay*1000));
            }
            else
            {
                value = -value;
                var date = (int)value / 86400;
                var microsecondOfDay = (long)((value % 86400d) * 1000000d);
                if (microsecondOfDay != 0)
                {
                    ++date;
                    microsecondOfDay = 86400000000L - microsecondOfDay;
                }
                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01

                return new ZonedDateTime().Plus(Duration.FromDays(date) + Duration.FromNanoseconds(microsecondOfDay * 1000));
            }
        }

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter parameter)
        {
            return 8;
        }

        int INpgsqlSimpleTypeHandler<LocalDateTime>.ValidateAndGetLength(LocalDateTime value, NpgsqlParameter parameter)
        {
            return 8;
        }

        public override void Write(Instant value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
            {
                if (_convertInfinityDateTime)
                {
                    if (value == Instant.MaxValue)
                    {
                        buf.WriteInt64(long.MaxValue);
                        return;
                    }
                    if (value == Instant.MinValue)
                    {
                        buf.WriteInt64(long.MinValue);
                        return;
                    }
                }
                WriteDateTimeUsingIntegerFormat(value.InUtc().LocalDateTime, buf);
            }
            else
            {
                if (_convertInfinityDateTime)
                {
                    if (value == Instant.MaxValue)
                    {
                        buf.WriteDouble(double.PositiveInfinity);
                        return;
                    }
                    if (value == Instant.MinValue)
                    {
                        buf.WriteDouble(double.NegativeInfinity);
                        return;
                    }
                }
                WriteDateTimeUsingFloatingPointFormat(value.InUtc().LocalDateTime, buf);
            }
        }

        void INpgsqlSimpleTypeHandler<LocalDateTime>.Write(LocalDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                WriteDateTimeUsingIntegerFormat(value, buf);
            else
                WriteDateTimeUsingFloatingPointFormat(value, buf);
        }

        internal static void WriteDateTimeUsingIntegerFormat(LocalDateTime value, NpgsqlWriteBuffer buf)
        {
            var totalDaysSinceEra = Period.Between(default(LocalDateTime), value, PeriodUnits.Days).Days;
            var microsecondOfDay = value.NanosecondOfDay / 1000;

            if (totalDaysSinceEra >= 730119)   // 1/1/2000
            {
                var uSecsDate = (totalDaysSinceEra - 730119) * 86400000000L;
                buf.WriteInt64(uSecsDate + microsecondOfDay);
            }
            else
            {
                var uSecsDate = (730119 - totalDaysSinceEra) * 86400000000L;
                buf.WriteInt64(-(uSecsDate - microsecondOfDay));
            }
        }

        internal static void WriteDateTimeUsingFloatingPointFormat(LocalDateTime value, NpgsqlWriteBuffer buf)
        {
            var totalDaysSinceEra = Period.Between(default(LocalDateTime), value, PeriodUnits.Days).Days;
            var secondOfDay = value.NanosecondOfDay / 1000000000d;

            if (totalDaysSinceEra >= 730119)
            {
                var uSecsDate = (totalDaysSinceEra - 730119) * 86400d;
                buf.WriteDouble(uSecsDate + secondOfDay);
            }
            else
            {
                var uSecsDate = (730119 - totalDaysSinceEra) * 86400d;
                buf.WriteDouble(-(uSecsDate - secondOfDay));
            }
        }
    }
}
