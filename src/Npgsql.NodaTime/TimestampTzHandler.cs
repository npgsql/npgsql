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
using NodaTime.TimeZones;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.NodaTime
{
    public class TimestampTzHandlerFactory : NpgsqlTypeHandlerFactory<Instant>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<Instant> Create(NpgsqlConnection conn)
            => new TimestampTzHandler(conn.HasIntegerDateTimes);
    }

    class TimestampTzHandler : NpgsqlSimpleTypeHandler<Instant>, INpgsqlSimpleTypeHandler<ZonedDateTime>
    {
        readonly IDateTimeZoneProvider _dateTimeZoneProvider;

        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Npgsql (currently) does not support this mode.
        /// </summary>
        readonly bool _integerFormat;

        public TimestampTzHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
            _dateTimeZoneProvider = DateTimeZoneProviders.Tzdb;
        }

        public override Instant Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            CheckIntegerFormat();
            var value = buf.ReadInt64();
            if (value == long.MaxValue || value == long.MinValue)
                throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported for timestamp with time zone"));
            return TimestampHandler.DecodeZonedDateTime(value).ToInstant();
        }

        ZonedDateTime INpgsqlSimpleTypeHandler<ZonedDateTime>.Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            CheckIntegerFormat();
            try
            {
                var value = buf.ReadInt64();
                if (value == long.MaxValue || value == long.MinValue)
                    throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not supported for timestamp with time zone"));
                var inUtc = TimestampHandler.DecodeZonedDateTime(value);
                var timezone = _dateTimeZoneProvider[buf.Connection.Timezone];
                return inUtc.WithZone(timezone);
            }
            catch (DateTimeZoneNotFoundException e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        protected override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            CheckIntegerFormat();
            if (!(value is Instant) && !(value is ZonedDateTime))
                throw CreateConversionException(value.GetType());
            return 8;
        }

        protected override void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            switch (value)
            {
                case Instant i:
                    TimestampHandler.WriteDateTime(i.InUtc().LocalDateTime, buf);
                    return;
                case ZonedDateTime zdt:
                    TimestampHandler.WriteDateTime(zdt.WithZone(DateTimeZone.Utc).LocalDateTime, buf);
                    break;
                default:
                    throw new InvalidCastException();
            }
        }

        void CheckIntegerFormat()
        {
            if (!_integerFormat)
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
        }
    }
}
