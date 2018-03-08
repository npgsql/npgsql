﻿#region License
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
using NodaTime;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NodaTime
{
    public class TimeTzHandlerFactory : NpgsqlTypeHandlerFactory<OffsetDateTime>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<OffsetDateTime> Create(NpgsqlConnection conn)
            => new TimeTzHandler(conn.HasIntegerDateTimes);
    }

    class TimeTzHandler : NpgsqlSimpleTypeHandler<OffsetDateTime>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Some PostgreSQL-like databases (e.g. CrateDB) use floating-point representation by default and do not 
        /// provide the option of switching to integer format.
        /// </summary>
        readonly bool _integerFormat;

        public TimeTzHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        public override OffsetDateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var dateTime = _integerFormat ?
                new LocalDate() + LocalTime.FromTicksSinceMidnight(buf.ReadInt64() * 10) :
                new LocalDate() + LocalTime.FromTicksSinceMidnight((long)(buf.ReadDouble() * NodaConstants.TicksPerSecond));
            var offset = Offset.FromSeconds(-buf.ReadInt32());
            return new OffsetDateTime(dateTime, offset);
        }

        public override int ValidateAndGetLength(OffsetDateTime value, NpgsqlParameter parameter)
        {
            if (value.Date != default(LocalDate))
                throw new InvalidCastException("Date component must be empty for timetz");
            return 12;
        }

        public override void Write(OffsetDateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.TickOfDay / 10);
            else
                buf.WriteDouble((double)value.TickOfDay / NodaConstants.TicksPerSecond);
            buf.WriteInt32(-(int)(value.Offset.Ticks / NodaConstants.TicksPerSecond));
        }
    }
}
