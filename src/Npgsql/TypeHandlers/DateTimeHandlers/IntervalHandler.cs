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
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("interval", NpgsqlDbType.Interval, new[] { typeof(TimeSpan), typeof(NpgsqlTimeSpan) })]
    class IntervalHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<TimeSpan> Create(NpgsqlConnection conn)
            => new IntervalHandler(conn.HasIntegerDateTimes);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class IntervalHandler : NpgsqlSimpleTypeHandlerWithPsv<TimeSpan, NpgsqlTimeSpan>
    {
        /// <summary>
        /// A deprecated compile-time option of PostgreSQL switches to a floating-point representation of some date/time
        /// fields. Some PostgreSQL-like databases (e.g. CrateDB) use floating-point representation by default and do not
        /// provide the option of switching to integer format.
        /// </summary>
        readonly bool _integerFormat;

        public IntervalHandler(bool integerFormat)
        {
            _integerFormat = integerFormat;
        }

        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (TimeSpan)((INpgsqlSimpleTypeHandler<NpgsqlTimeSpan>)this).Read(buf, len, fieldDescription);

        protected override NpgsqlTimeSpan ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            if (_integerFormat)
            {
                var ticks = buf.ReadInt64();
                var day = buf.ReadInt32();
                var month = buf.ReadInt32();
                return new NpgsqlTimeSpan(month, day, ticks * 10);
            }
            else
            {
                var seconds = buf.ReadDouble();
                var day = buf.ReadInt32();
                var month = buf.ReadInt32();
                return new NpgsqlTimeSpan(month, day, (long)(seconds * TimeSpan.TicksPerSecond));
            }
        }

        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
            => 16;

        public override int ValidateAndGetLength(NpgsqlTimeSpan value, NpgsqlParameter parameter)
            => 16;

        public override void Write(NpgsqlTimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (_integerFormat)
                buf.WriteInt64(value.Ticks / 10); // TODO: round?
            else
                buf.WriteDouble(value.TotalSeconds - (value.Days * 86400) - (value.Months * NpgsqlTimeSpan.DaysPerMonth * 86400));

            buf.WriteInt32(value.Days);
            buf.WriteInt32(value.Months);
        }

        // TODO: Can write directly from TimeSpan
        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write(value, buf, parameter);
    }
}
