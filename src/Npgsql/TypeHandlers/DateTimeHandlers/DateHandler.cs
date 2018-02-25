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
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("date", NpgsqlDbType.Date, DbType.Date, typeof(NpgsqlDate))]
    class DateHandlerFactory : NpgsqlTypeHandlerFactory<DateTime>
    {
        protected override NpgsqlTypeHandler<DateTime> Create(NpgsqlConnection conn)
            => new DateHandler(conn.Connector.ConvertInfinityDateTime);
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class DateHandler : NpgsqlSimpleTypeHandlerWithPsv<DateTime, NpgsqlDate>
    {
        internal const int PostgresEpochJdate = 2451545; // == date2j(2000, 1, 1)
        internal const int MonthsPerYear = 12;

        /// <summary>
        /// Whether to convert positive and negative infinity values to DateTime.{Max,Min}Value when
        /// a DateTime is requested
        /// </summary>
        readonly bool _convertInfinityDateTime;

        public DateHandler(bool convertInfinityDateTime)
        {
            _convertInfinityDateTime = convertInfinityDateTime;
        }

        #region Read

        public override DateTime Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var npgsqlDate = ReadPsv(buf, len, fieldDescription);
            try {
                if (npgsqlDate.IsFinite)
                    return (DateTime)npgsqlDate;
                if (!_convertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite date values to DateTime");
                if (npgsqlDate.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            } catch (Exception e) {
                throw new NpgsqlSafeReadException(e);
            }
        }

        /// <remarks>
        /// Copied wholesale from Postgresql backend/utils/adt/datetime.c:j2date
        /// </remarks>
        protected override NpgsqlDate ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var binDate = buf.ReadInt32();

            switch (binDate)
            {
            case int.MaxValue:
                return NpgsqlDate.Infinity;
            case int.MinValue:
                return NpgsqlDate.NegativeInfinity;
            default:
                return new NpgsqlDate(binDate + 730119);
            }
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(DateTime value, NpgsqlParameter parameter)
            => 4;

        public override int ValidateAndGetLength(NpgsqlDate value, NpgsqlParameter parameter)
            => 4;

        public override void Write(DateTime value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            NpgsqlDate value2;
            if (_convertInfinityDateTime)
            {
                if (value == DateTime.MaxValue)
                    value2 = NpgsqlDate.Infinity;
                else if (value == DateTime.MinValue)
                    value2 = NpgsqlDate.NegativeInfinity;
                else
                    value2 = new NpgsqlDate(value);
            }
            else
                value2 = new NpgsqlDate(value);

            Write(value2, buf, parameter);
        }

        public override void Write(NpgsqlDate value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (value == NpgsqlDate.NegativeInfinity)
                buf.WriteInt32(int.MinValue);
            else if (value == NpgsqlDate.Infinity)
                buf.WriteInt32(int.MaxValue);
            else
                buf.WriteInt32(value.DaysSinceEra - 730119);
        }

        #endregion Write
    }
}
