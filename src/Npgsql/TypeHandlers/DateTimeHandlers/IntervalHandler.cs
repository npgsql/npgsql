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
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("interval", NpgsqlDbType.Interval, new[] { typeof(TimeSpan), typeof(NpgsqlTimeSpan) })]
    class IntervalHandlerFactory : TypeHandlerFactory
    {
        // Check for the legacy floating point timestamps feature
        protected override TypeHandler Create(NpgsqlConnection conn)
            => new IntervalHandler(conn.HasIntegerDateTimes);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class IntervalHandler : SimpleTypeHandlerWithPsv<TimeSpan, NpgsqlTimeSpan>
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

        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (TimeSpan)((ISimpleTypeHandler<NpgsqlTimeSpan>)this).Read(buf, len, fieldDescription);

        protected override NpgsqlTimeSpan ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            CheckIntegerFormat();
            var ticks = buf.ReadInt64();
            var day = buf.ReadInt32();
            var month = buf.ReadInt32();
            return new NpgsqlTimeSpan(month, day, ticks * 10);
        }

        protected override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            CheckIntegerFormat();

            var asString = value as string;
            if (asString != null)
            {
                var converted = NpgsqlTimeSpan.Parse(asString);
                if (parameter == null)
                    throw CreateConversionButNoParamException(value.GetType());
                parameter.ConvertedValue = converted;
            }
            else if (!(value is TimeSpan) && !(value is NpgsqlTimeSpan))
                throw CreateConversionException(value.GetType());

            return 16;
        }

        protected override void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;

            var interval = value as TimeSpan? ?? (NpgsqlTimeSpan)value;

            buf.WriteInt64(interval.Ticks / 10); // TODO: round?
            buf.WriteInt32(interval.Days);
            buf.WriteInt32(interval.Months);
        }

        void CheckIntegerFormat()
        {
            if (!_integerFormat)
                throw new NotSupportedException("Old floating point representation for timestamps not supported");
        }
    }
}
