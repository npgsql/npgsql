#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    [TypeMapping("timestamptz", NpgsqlDbType.TimestampTZ, DbType.DateTimeOffset, typeof(DateTimeOffset))]
    class TimeStampTzHandler : TimeStampHandler, ISimpleTypeHandler<DateTimeOffset>
    {
        public TimeStampTzHandler(PostgresType postgresType, TypeHandlerRegistry registry)
            : base(postgresType, registry) {}

        public override DateTime Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // TODO: Convert directly to DateTime without passing through NpgsqlTimeStamp?
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            try
            {
                if (ts.IsFinite)
                    return ts.ToDateTime().ToLocalTime();
                if (!ConvertInfinityDateTime)
                    throw new InvalidCastException("Can't convert infinite timestamptz values to DateTime");
                if (ts.IsInfinity)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            } catch (Exception e) {
                throw new SafeReadException(e);
            }
        }

        internal override NpgsqlDateTime ReadPsv(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var ts = ReadTimeStamp(buf, len, fieldDescription);
            return new NpgsqlDateTime(ts.Date, ts.Time, DateTimeKind.Utc).ToLocalTime();
        }

        DateTimeOffset ISimpleTypeHandler<DateTimeOffset>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
        {
            try
            {
                return new DateTimeOffset(ReadTimeStamp(buf, len, fieldDescription).ToDateTime(), TimeSpan.Zero);
            } catch (Exception e) {
                throw new SafeReadException(e);
            }
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;

            if (value is NpgsqlDateTime)
            {
                var ts = (NpgsqlDateTime)value;
                switch (ts.Kind)
                {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Utc:
                    break;
                case DateTimeKind.Local:
                    ts = ts.ToUniversalTime();
                    break;
                default:
                    throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {ts.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
                }
                base.Write(ts, buf, parameter);
                return;
            }

            if (value is DateTime)
            {
                var dt = (DateTime)value;
                switch (dt.Kind)
                {
                case DateTimeKind.Unspecified:
                case DateTimeKind.Utc:
                    break;
                case DateTimeKind.Local:
                    dt = dt.ToUniversalTime();
                    break;
                default:
                    throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {dt.Kind} of enum {nameof(DateTimeKind)}. Please file a bug.");
                }
                base.Write(dt, buf, parameter);
                return;
            }

            if (value is DateTimeOffset)
            {
                base.Write(((DateTimeOffset)value).ToUniversalTime(), buf, parameter);
                return;
            }

            throw new InvalidOperationException("Internal Npgsql bug, please report.");
        }
    }
}
