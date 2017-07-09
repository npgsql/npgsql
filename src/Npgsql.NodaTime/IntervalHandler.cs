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
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<Period>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<Period> Create(NpgsqlConnection conn)
            => new IntervalHandler(conn.HasIntegerDateTimes);
    }

    class IntervalHandler : NpgsqlSimpleTypeHandler<Period>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public IntervalHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        public override Period Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            CheckIntegerFormat();
            var microsecondsInDay = buf.ReadInt64();
            var days = buf.ReadInt32();
            var totalMonths = buf.ReadInt32();

            // Nodatime will normalize most things (i.e. nanoseconds to milliseconds, seconds...)
            // but it will not normalize months to years.
            var months = totalMonths % 12;
            var years = totalMonths / 12;

            return new PeriodBuilder
            {
                Nanoseconds = microsecondsInDay * 1000,
                Days = days,
                Months = months,
                Years = years
            }.Build().Normalize();
        }

        protected override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            CheckIntegerFormat();
            if (!(value is Period))
                throw CreateConversionException(value.GetType());
            return 16;
        }

        protected override void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            var period = (Period)value;

            var microsecondsInDay =
                (((period.Hours * NodaConstants.MinutesPerHour + period.Minutes) * NodaConstants.SecondsPerMinute + period.Seconds) * NodaConstants.MillisecondsPerSecond + period.Milliseconds) * 1000 +
                period.Nanoseconds / 1000;  // Take the microseconds, discard the nanosecond remainder
            buf.WriteInt64(microsecondsInDay);
            buf.WriteInt32(period.Weeks * 7 + period.Days);     // days
            buf.WriteInt32(period.Years * 12 + period.Months);  // months
        }

        void CheckIntegerFormat()
        {
            if (!_integerFormat)
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
        }
    }
}
