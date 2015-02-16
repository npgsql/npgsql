using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("numeric", NpgsqlDbType.Numeric, new[] { DbType.Decimal, DbType.VarNumeric }, typeof(decimal))]
    internal class NumericHandler : TypeHandler<decimal>,
        ISimpleTypeReader<decimal>, ISimpleTypeWriter,
        ISimpleTypeReader<byte>, ISimpleTypeReader<short>, ISimpleTypeReader<int>, ISimpleTypeReader<long>,
        ISimpleTypeReader<float>, ISimpleTypeReader<double>,
        ISimpleTypeReader<string>
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

        public decimal Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var numGroups = (ushort)buf.ReadInt16();
            var weightFirstGroup = buf.ReadInt16(); // 10000^weight
            var sign = (ushort)buf.ReadInt16(); // 0x0000 = positive, 0x4000 = negative, 0xC000 = NaN
            var dscale = buf.ReadInt16(); // Number of digits (in base 10) to print after decimal separator

            bool overflow = false;

            var result = 0M;
            for (int i = 0, weight = weightFirstGroup + 7; i < numGroups; i++, weight--)
            {
                var group = (ushort)buf.ReadInt16();
                if (weight < 0 || weight >= Decimals.Length)
                {
                    overflow = true;
                }
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

        byte ISimpleTypeReader<byte>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (byte)Read(buf, fieldDescription, len);
        }

        short ISimpleTypeReader<short>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (short)Read(buf, fieldDescription, len);
        }

        int ISimpleTypeReader<int>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (int)Read(buf, fieldDescription, len);
        }

        long ISimpleTypeReader<long>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (long)Read(buf, fieldDescription, len);
        }

        float ISimpleTypeReader<float>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (float)Read(buf, fieldDescription, len);
        }

        double ISimpleTypeReader<double>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (double)Read(buf, fieldDescription, len);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
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
                fractionDigits = fraction.ToString().Length;
                fractionGroups = (fractionDigits + 3) / 4;
                if (weight < -1)
                    fractionGroups += weight + 1;
            }

            numGroups = integerGroups + fractionGroups;
        }

        public int ValidateAndGetLength(object value)
        {
            var num = GetIConvertibleValue<decimal>(value);
            
            if (num == 0M)
                return 4 * sizeof(short) + 0;

            bool negative = num < 0;
            if (negative)
                num = -num;

            int numGroups, weight, fractionDigits;
            GetNumericHeader(num, out numGroups, out weight, out fractionDigits);

            return 4 * sizeof(short) + numGroups * sizeof(short);
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var num = GetIConvertibleValue<decimal>(value);

            if (num == 0M)
            {
                buf.WriteInt64(0);
            }
            else
            {
                bool negative = num < 0;
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
