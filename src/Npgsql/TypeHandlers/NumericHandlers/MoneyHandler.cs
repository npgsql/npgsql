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

using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-money.html
    /// </remarks>
    [TypeMapping("money", NpgsqlDbType.Money, dbType: DbType.Currency)]
    class MoneyHandler : NpgsqlSimpleTypeHandler<decimal>
    {
        const int MoneyScale = 2;

        public override decimal Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var result = new DecimalRaw(buf.ReadInt64()) { Scale = MoneyScale };
            return Unsafe.As<DecimalRaw, decimal>(ref result);
        }

        public override int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)
            => value < -92233720368547758.08M || value > 92233720368547758.07M
                ? throw new OverflowException($"The supplied value ({value}) is outside the range for a PostgreSQL money value.")
                : 8;

        public override void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var raw = Unsafe.As<decimal, DecimalRaw>(ref value);
            var scaleDifference = MoneyScale - raw.Scale;
            if (scaleDifference > 0)
                DecimalRaw.Multiply(ref raw, DecimalRaw.Powers10[scaleDifference]);
            else
                while (scaleDifference < 0)
                {
                    var scaleChunk = Math.Min(DecimalRaw.MaxUInt32Scale, -scaleDifference);
                    DecimalRaw.Divide(ref raw, DecimalRaw.Powers10[scaleChunk]);
                    scaleDifference -= scaleChunk;
                }

            var result = (long)raw.Mid << 32 | (long)raw.Low;
            if (raw.Negative) result = -result;
            buf.WriteInt64(result);
        }
    }
}
