﻿#region License
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

        public override int ValidateAndGetLength(Period value, NpgsqlParameter parameter)
        {
            CheckIntegerFormat();
            return 16;
        }

        public override void Write(Period value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var microsecondsInDay =
                (((value.Hours * NodaConstants.MinutesPerHour + value.Minutes) * NodaConstants.SecondsPerMinute + value.Seconds) * NodaConstants.MillisecondsPerSecond + value.Milliseconds) * 1000 +
                value.Nanoseconds / 1000;  // Take the microseconds, discard the nanosecond remainder
            buf.WriteInt64(microsecondsInDay);
            buf.WriteInt32(value.Weeks * 7 + value.Days);     // days
            buf.WriteInt32(value.Years * 12 + value.Months);  // months
        }

        void CheckIntegerFormat()
        {
            if (!_integerFormat)
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
        }
    }
}
