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
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timetz", NpgsqlDbType.TimeTz)]
    class TimeTzHandler : NpgsqlSimpleTypeHandler<DateTimeOffset>, INpgsqlSimpleTypeHandler<DateTime>, INpgsqlSimpleTypeHandler<TimeSpan>
    {
        // Binary Format: int64 expressing microseconds, int32 expressing timezone in seconds, negative

        #region Read

        public override DateTimeOffset Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // Adjust from 1 microsecond to 100ns. Time zone (in seconds) is inverted.
            var ticks = buf.ReadInt64() * 10;
            var offset = new TimeSpan(0, 0, -buf.ReadInt32());
            return new DateTimeOffset(ticks, offset);
        }

        DateTime INpgsqlSimpleTypeHandler<DateTime>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).LocalDateTime;

        TimeSpan INpgsqlSimpleTypeHandler<TimeSpan>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).LocalDateTime.TimeOfDay;

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(DateTimeOffset value, NpgsqlParameter parameter)
            => 12;
        public int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 12;
        public int ValidateAndGetLength(DateTime value, NpgsqlParameter parameter)
            => 12;

        public override void Write(DateTimeOffset value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteInt64(value.TimeOfDay.Ticks / 10);
            buf.WriteInt32(-(int)(value.Offset.Ticks / TimeSpan.TicksPerSecond));
        }

        public void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteInt64(value.TimeOfDay.Ticks / 10);

            switch (value.Kind)
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
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {value.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
            }
        }

        public void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteInt64(value.Ticks / 10);
            buf.WriteInt32(-(int)(TimeZoneInfo.Local.BaseUtcOffset.Ticks / TimeSpan.TicksPerSecond));
        }

        #endregion Write
    }
}
