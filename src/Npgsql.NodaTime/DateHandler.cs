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
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.NodaTime
{
    class DateHandlerFactory : NpgsqlTypeHandlerFactory<LocalDate>
    {
        protected override NpgsqlTypeHandler<LocalDate> Create(NpgsqlConnection conn)
          => new DateHandler();
    }

    sealed class DateHandler : NpgsqlSimpleTypeHandler<LocalDate>
    {
        public override LocalDate Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var value = buf.ReadInt32();
            if (value == int.MaxValue || value == int.MinValue)
                throw new NpgsqlSafeReadException(new NotSupportedException("Infinity values not yet supported for date"));
            return new LocalDate().PlusDays(value + 730119);
        }

        protected override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is LocalDate))
                throw CreateConversionException(value.GetType());
            return 4;
        }

        protected override void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            var date = (LocalDate)value;
            var totalDaysSinceEra = Period.Between(default(LocalDate), date, PeriodUnits.Days).Days;
            buf.WriteInt32(totalDaysSinceEra - 730119);
        }
    }
}
