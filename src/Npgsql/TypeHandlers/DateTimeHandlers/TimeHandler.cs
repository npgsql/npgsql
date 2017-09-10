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
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.DateTimeHandlers
{
    [TypeMapping("time", NpgsqlDbType.Time, new[] { DbType.Time })]
    class TimeHandlerFactory : NpgsqlTypeHandlerFactory<TimeSpan>
    {
        // Check for the legacy floating point timestamps feature
        protected override NpgsqlTypeHandler<TimeSpan> Create(NpgsqlConnection conn)
            => new TimeHandler();
    }

    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    class TimeHandler : NpgsqlSimpleTypeHandler<TimeSpan>
    {
        public TimeHandler()
        {
        }

        public override TimeSpan Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            // PostgreSQL time resolution == 1 microsecond == 10 ticks
            return new TimeSpan(buf.ReadInt64() * 10);
        }

        public override int ValidateAndGetLength(TimeSpan value, NpgsqlParameter parameter)
        {
            return 8;
        }

        public override void Write(TimeSpan value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value.Ticks / 10);
    }
}
