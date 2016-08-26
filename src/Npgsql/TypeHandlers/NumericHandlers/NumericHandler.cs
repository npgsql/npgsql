﻿#region License
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
using System.Globalization;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using JetBrains.Annotations;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("numeric", NpgsqlDbType.Numeric, new[] { DbType.Decimal, DbType.VarNumeric }, typeof(decimal), DbType.Decimal)]
    class NumericHandler : SimpleTypeHandler<decimal>,
        ISimpleTypeHandler<byte>, ISimpleTypeHandler<short>, ISimpleTypeHandler<int>, ISimpleTypeHandler<long>,
        ISimpleTypeHandler<float>, ISimpleTypeHandler<double>,
        ISimpleTypeHandler<string>
    {
        static readonly decimal[] Decimals = new decimal[] {
            0.0000000000000000000000000001M,
            0.000000000000000000000001M,
            0.00000000000000000001M,
            0.0000000000000001M,
            0.000000000001M,
            0.00000001M,
            0.0001M,
            1M,
            10000M,
            100000000M,
            1000000000000M,
            10000000000000000M,
            100000000000000000000M,
            1000000000000000000000000M,
            10000000000000000000000000000M
        };

        internal NumericHandler(IBackendType backendType) : base(backendType) { }

        public override decimal Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var numGroups = (ushort)buf.ReadInt16();
            var weightFirstGroup = buf.ReadInt16(); // 10000^weight
            var sign = (ushort)buf.ReadInt16(); // 0x0000 = positive, 0x4000 = negative, 0xC000 = NaN
            buf.ReadInt16(); // dcsale. Number of digits (in base 10) to print after decimal separator

            var overflow = false;

            var result = 0M;
            for (int i = 0, weight = weightFirstGroup + 7; i < numGroups; i++, weight--)
            {
                var group = (ushort)buf.ReadInt16();
                if (weight < 0 || weight >= Decimals.Length)
                    overflow = true;
                else
                {
                    try
                    {
                        result += Decimals[weight] * group;
                    }
                    catch (OverflowException)
                    {
                        overflow = true;
                    }
                }
            }

            if (overflow)
                throw new SafeReadException(new OverflowException("Numeric value does not fit in a System.Decimal"));

            if (sign == 0xC000)
                throw new SafeReadException(new InvalidCastException("Numeric NaN not supported by System.Decimal"));

            return sign == 0x4000 ? -result : result;
        }

        byte ISimpleTypeHandler<byte>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (byte)Read(buf, len, fieldDescription);

        short ISimpleTypeHandler<short>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (short)Read(buf, len, fieldDescription);

        int ISimpleTypeHandler<int>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (int)Read(buf, len, fieldDescription);

        long ISimpleTypeHandler<long>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (long)Read(buf, len, fieldDescription);

        float ISimpleTypeHandler<float>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (float)Read(buf, len, fieldDescription);

        double ISimpleTypeHandler<double>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (double)Read(buf, len, fieldDescription);

        string ISimpleTypeHandler<string>.Read(ReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).ToString();

        void GetNumericHeader(decimal num, out int numGroups, out int weight, out int fractionDigits)
        {
            var integer = decimal.Truncate(num);
            var fraction = num - integer;
            int slot1;
            for (slot1 = 0; slot1 <= 13; slot1++)
            {
                if (num < Decimals[slot1 + 1])
                    break;
            }
            weight = slot1 - 7;
            fractionDigits = 0;
            var fractionGroups = 0;
            var integerGroups = weight >= 0 ? weight + 1 : 0;

            if (fraction != 0)
            {
                fractionDigits = fraction.ToString(CultureInfo.InvariantCulture).Length - 2;
                fractionGroups = (fractionDigits + 3) / 4;
                if (weight < -1)
                    fractionGroups += weight + 1;
            }

            numGroups = integerGroups + fractionGroups;
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            decimal num;
            if (value is decimal)
                num = (decimal)value;
            else
            {
                num = Convert.ToDecimal(value);
                if (parameter == null)
                {
                    throw CreateConversionButNoParamException(value.GetType());
                }
                parameter.ConvertedValue = num;
            }

            if (num == 0M)
                return 4 * sizeof(short) + 0;

            var negative = num < 0;
            if (negative)
                num = -num;

            int numGroups, weight, fractionDigits;
            GetNumericHeader(num, out numGroups, out weight, out fractionDigits);

            return 4 * sizeof(short) + numGroups * sizeof(short);
        }

        public override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            var num = (decimal) (parameter?.ConvertedValue ?? value);
            if (num == 0M)
                buf.WriteInt64(0);
            else
            {
                var negative = num < 0;
                if (negative)
                    num = -num;

                int numGroups, weight, fractionDigits;
                GetNumericHeader(num, out numGroups, out weight, out fractionDigits);

                buf.WriteInt16(numGroups);
                buf.WriteInt16(weight);
                buf.WriteInt16(negative ? 0x4000 : 0x0000);
                buf.WriteInt16(fractionDigits);
                for (int i = 0, pos = weight + 7; i < numGroups; i++, pos--)
                {
                    buf.WriteInt16((ushort)(num / Decimals[pos]));
                    num %= Decimals[pos];
                }
            }
        }
    }
}
