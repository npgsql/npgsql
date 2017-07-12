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
using System.Globalization;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using System.Diagnostics;
using JetBrains.Annotations;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("numeric", NpgsqlDbType.Numeric, new[] { DbType.Decimal, DbType.VarNumeric }, typeof(decimal), DbType.Decimal)]
    class NumericHandler : NpgsqlSimpleTypeHandler<decimal>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>,
        INpgsqlSimpleTypeHandler<string>
    {
        #region Read

        static readonly decimal[] Decimals = {
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

        public override decimal Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
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
                throw new NpgsqlSafeReadException(new OverflowException("Numeric value does not fit in a System.Decimal"));

            if (sign == 0xC000)
                throw new NpgsqlSafeReadException(new InvalidCastException("Numeric NaN not supported by System.Decimal"));

            return sign == 0x4000 ? -result : result;
        }

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (byte)Read(buf, len, fieldDescription);

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (short)Read(buf, len, fieldDescription);

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (int)Read(buf, len, fieldDescription);

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (long)Read(buf, len, fieldDescription);

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (float)Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => (double)Read(buf, len, fieldDescription);

        string INpgsqlSimpleTypeHandler<string>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription).ToString();

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)
        {
            if (value == 0M)
                return 4 * sizeof(short) + 0;

            var negative = value < 0;
            if (negative)
                value = -value;

            int numGroups, weight, fractionDigits;
            GetNumericHeader(value, out numGroups, out weight, out fractionDigits);

            return 4 * sizeof(short) + numGroups * sizeof(short);
        }

        public int ValidateAndGetLength(short value, NpgsqlParameter parameter)
            => ValidateAndGetLength((decimal)value, parameter);
        public int ValidateAndGetLength(int value, NpgsqlParameter parameter)
            => ValidateAndGetLength((decimal) value, parameter);
        public int ValidateAndGetLength(long value, NpgsqlParameter parameter)
            => ValidateAndGetLength((decimal) value, parameter);
        public int ValidateAndGetLength(float value, NpgsqlParameter parameter)
            => ValidateAndGetLength((decimal) value, parameter);
        public int ValidateAndGetLength(double value, NpgsqlParameter parameter)
            => ValidateAndGetLength((decimal) value, parameter);
        public int ValidateAndGetLength(byte value, NpgsqlParameter parameter)
            => ValidateAndGetLength((decimal) value, parameter);

        public int ValidateAndGetLength(string value, NpgsqlParameter parameter)
        {
            var converted = Convert.ToDecimal(value);
            if (parameter == null)
                throw CreateConversionButNoParamException(value.GetType());
            parameter.ConvertedValue = converted;
            return ValidateAndGetLength(converted, parameter);
        }

        public override void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            if (value == 0M)
            {
                buf.WriteInt64(0);
                return;
            }

            var negative = value < 0;
            if (negative)
                value = -value;

            int numGroups, weight, fractionDigits;
            GetNumericHeader(value, out numGroups, out weight, out fractionDigits);

            buf.WriteInt16(numGroups);
            buf.WriteInt16(weight);
            buf.WriteInt16(negative ? 0x4000 : 0x0000);
            buf.WriteInt16(fractionDigits);
            for (int i = 0, pos = weight + 7; i < numGroups; i++, pos--)
            {
                buf.WriteInt16((ushort)(value / Decimals[pos]));
                value %= Decimals[pos];
            }
        }

        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write((decimal)value, buf, parameter);
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write((decimal)value, buf, parameter);
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write((decimal)value, buf, parameter);
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write((decimal)value, buf, parameter);
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write((decimal)value, buf, parameter);
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => Write((decimal)value, buf, parameter);
        public void Write(string value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            Debug.Assert(parameter != null);
            Write((decimal)parameter.ConvertedValue, buf, parameter);
        }

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

        #endregion Write
    }
}
