#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Runtime.InteropServices;
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
        static readonly Instant Instant0 = Instant.FromUtc(1, 1, 1, 0, 0, 0);
        static readonly Instant Instant2000 = Instant.FromUtc(2000, 1, 1, 0, 0, 0);
        static readonly Duration Plus292Years = Duration.FromDays(292 * 365);
        static readonly Duration Minus292Years = -Plus292Years;

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

        #region Read

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
                return Decode(value);
            }
            else
            {
                var value = buf.ReadDouble();
                if (_convertInfinityDateTime)
                {
                    if (double.IsPositiveInfinity(value))
                        return Instant.MaxValue;
                    if (double.IsNegativeInfinity(value))
                        return Instant.MinValue;
                }
                return Decode(value);
            }
        }

        LocalDateTime INpgsqlSimpleTypeHandler<LocalDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            if (_integerFormat)
            {
                var value = buf.ReadInt64();
                if (value == long.MaxValue || value == long.MinValue)
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported when reading LocalDateTime, read as Instant instead"));
                return Decode(value).InUtc().LocalDateTime;
            }
            else
            {
                var value = buf.ReadDouble();
                if (double.IsPositiveInfinity(value) || double.IsNegativeInfinity(value))
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported when reading LocalDateTime, read as Instant instead"));
                return Decode(value).InUtc().LocalDateTime;
            }
        }

        // value is the number of microseconds from 2000-01-01T00:00:00.
        // Unfortunately NodaTime doesn't have Duration.FromMicroseconds(), so we decompose into milliseconds
        // and nanoseconds
        internal static Instant Decode(long value)
            => Instant2000 + Duration.FromMilliseconds(value / 1000) + Duration.FromNanoseconds(value % 1000 * 1000);

        // This is legacy support for PostgreSQL's old floating-point timestamp encoding - finally removed in PG 10 and not used for a long
        // time. Unfortunately CrateDB seems to use this for some reason.
        internal static Instant Decode(double value)
        {
            Debug.Assert(!double.IsPositiveInfinity(value) && !double.IsNegativeInfinity(value));

            if (value >= 0d)
            {
                var date = (int)value / 86400;
                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                var microsecondOfDay = (long)((value % 86400d) * 1000000d);

                return Instant0 + Duration.FromDays(date) + Duration.FromNanoseconds(microsecondOfDay*1000);
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

                return Instant0 + Duration.FromDays(date) + Duration.FromNanoseconds(microsecondOfDay*1000);
            }
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(Instant value, NpgsqlParameter parameter)
            => 8;

        int INpgsqlSimpleTypeHandler<LocalDateTime>.ValidateAndGetLength(LocalDateTime value, NpgsqlParameter parameter)
            => 8;

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
                WriteInteger(value, buf);
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
                WriteDouble(value, buf);
            }
        }

        void INpgsqlSimpleTypeHandler<LocalDateTime>.Write(LocalDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                WriteInteger(value.InUtc().ToInstant(), buf);
            else
                WriteDouble(value.InUtc().ToInstant(), buf);
        }

        // We need to write the number of microseconds from 2000-01-01T00:00:00.
        internal static void WriteInteger(Instant instant, NpgsqlWriteBuffer buf)
        {
            var since2000 = instant - Instant2000;

            // The nanoseconds may overflow, so fallback to BigInteger where necessary.
            var microseconds =
                since2000 >= Minus292Years &&
                since2000 <= Plus292Years
                    ? since2000.ToInt64Nanoseconds() / 1000
                    : (long)(since2000.ToBigIntegerNanoseconds() / 1000);

            buf.WriteInt64(microseconds);
        }

        // This is legacy support for PostgreSQL's old floating-point timestamp encoding - finally removed in PG 10 and not used for a long
        // time. Unfortunately CrateDB seems to use this for some reason.
        internal static void WriteDouble(Instant instant, NpgsqlWriteBuffer buf)
        {
            var localDateTime = instant.InUtc().LocalDateTime;
            var totalDaysSinceEra = Period.Between(default(LocalDateTime), localDateTime, PeriodUnits.Days).Days;
            var secondOfDay = localDateTime.NanosecondOfDay / 1000000000d;

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

        #endregion Write
    }
}
