#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timetz", NpgsqlDbType.TimeTZ)]
    internal class TimeTzHandler : SimpleTypeHandler<DateTimeOffset>, ISimpleTypeHandler<DateTime>, ISimpleTypeHandler<TimeSpan>
    {
        // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative

        public override DateTimeOffset Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var ticks = buf.ReadInt64() * 10;
            var offset = new TimeSpan(0, 0, -buf.ReadInt32());
            return new DateTimeOffset(ticks, offset);
        }

        DateTime ISimpleTypeHandler<DateTime>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).LocalDateTime;
        }

        TimeSpan ISimpleTypeHandler<TimeSpan>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).LocalDateTime.TimeOfDay;
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is DateTimeOffset) && !(value is DateTime) && !(value is TimeSpan))
            {
                throw CreateConversionException(value.GetType());
            }
            return 12;
        }

        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            if (value is DateTimeOffset)
            {
                var dto = (DateTimeOffset) value;
                buf.WriteInt64(dto.TimeOfDay.Ticks / 10);
                buf.WriteInt32(-(int)(dto.Offset.Ticks / TimeSpan.TicksPerSecond));
                return;
            }

            if (value is DateTime)
            {
                var dt = (DateTime) value;

                buf.WriteInt64(dt.TimeOfDay.Ticks / 10);

                switch (dt.Kind)
                {
                case DateTimeKind.Utc:
                    buf.WriteInt32(0);
                    break;
                case DateTimeKind.Unspecified:
                    // Treat as local...
                case DateTimeKind.Local:
                    buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
                    break;
                default:
                    throw PGUtil.ThrowIfReached();
                }

                return;
            }

            if (value is TimeSpan)
            {
                var ts = (TimeSpan)value;
                buf.WriteInt64(ts.Ticks / 10);
                buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
                return;
            }

            throw PGUtil.ThrowIfReached();
        }
    }
}
