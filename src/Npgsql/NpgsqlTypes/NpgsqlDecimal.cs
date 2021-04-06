#define DIGITS8

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#pragma warning disable 1591
#nullable enable

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQL numeric. This struct is similar to <see cref="decimal"/> but capable of storing
    /// the full range of values of the PostgreSQL numeric type, including NaN, Infinity and -Infinity.
    /// </summary>
    public readonly struct NpgsqlDecimal : IEquatable<NpgsqlDecimal>, IComparable<NpgsqlDecimal>, IComparable,
        IComparer<NpgsqlDecimal>, IComparer
    {
        // For debugging purposes, we can change number of digits in each group
#if DIGITS8
        const int Nbase = 100000000;
        const int DecDigits = 8;
        const int MulGuardDigits = 2;
#elif DIGITS4
        const int Nbase = 10000;
        const int DecDigits = 4;
        const int MulGuardDigits = 2;
#elif DIGITS2
        const int Nbase = 100;
        const int DecDigits = 2;
        const int MulGuardDigits = 3;
#elif DIGITS1
        const int Nbase = 10;
        const int DecDigits = 1;
        const int MulGuardDigits = 4;
#endif

        const int HalfNbase = Nbase / 2;
        const int MinSigDigits = 16;
        const int MaxDisplayScale = 1000;
        const int MaxDigitsBeforeDecimalPoint = 131072;
        const int MaxDigitsAfterDecimalPoint = 16383;

        const int MaxWeight = MaxDigitsBeforeDecimalPoint / DecDigits - 1;
        const int MinWeight = -((MaxDigitsAfterDecimalPoint + DecDigits - 1) / DecDigits);

        /// <summary>
        /// Represents positive infinity. This field is constant.
        /// </summary>
        public static readonly NpgsqlDecimal PositiveInfinity = new NpgsqlDecimal(0, Sign.Pinf, 0, null);

        /// <summary>
        /// Represents negative infinity. This field is constant.
        /// </summary>
        public static readonly NpgsqlDecimal NegativeInfinity = new NpgsqlDecimal(0, Sign.Ninf, 0, null);

        /// <summary>
        /// Represents a value that is not a number (NaN). This field is constant.
        /// </summary>
        public static readonly NpgsqlDecimal NaN = new NpgsqlDecimal(0, Sign.Nan, 0, null);

        readonly short _weight;
        readonly Sign _sign;
        readonly ushort _dscale;
        readonly uint[]? _digits;

        /// <summary>
        /// The number of digits after the decimal separator. Will be 0 if non-finite.
        /// </summary>
        public int Scale => _dscale;

        internal short Weight => _weight;
        internal Sign Header => _sign;
        internal uint[]? Digits => _digits;

        /// <summary>
        /// Represents the number zero (0).
        /// </summary>
        public static NpgsqlDecimal Zero => new();


        /// <summary>
        /// Represents the number one (1).
        /// </summary>
        public static readonly NpgsqlDecimal One = new(1UL);

        internal NpgsqlDecimal(short weight, Sign sign, ushort dscale, uint[]? digits)
        {
            _weight = weight;
            _sign = sign;
            _dscale = dscale;
            _digits = digits;
        }

        NpgsqlDecimal(ref Var var)
        {
            if (var._weight < MinWeight || var._weight > MaxWeight || (uint)var._dscale > MaxDigitsAfterDecimalPoint)
            {
                throw new OverflowException("Overflow in numeric calculation");
            }
            var.Normalize();
            _weight = (short)var._weight;
            _sign = var._sign;
            _dscale = (ushort)var._dscale;
            _digits = var._digits;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(ulong value)
        {
            if (value == 0)
            {
                _weight = 0;
                _sign = Sign.Pos;
                _dscale = 0;
                _digits = null;
                return;
            }

            var ndigits = 0;
            var v = value;
            do
            {
                ndigits++;
                v /= Nbase;
            } while (v != 0);
            var digits = new uint[ndigits];
            do
            {
                digits[--ndigits] = (uint)(value % Nbase);
                value /= Nbase;
            } while (ndigits != 0);
            _weight = (short)(digits.Length - 1);
            _sign = Sign.Pos;
            _dscale = 0;
            _digits = digits;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified 64-bit signed integer.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(long value) : this((ulong)(value < 0 ? -value : value))
        {
            if (value < 0)
                _sign = Sign.Neg;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified 32-bit signed integer.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(int value) : this((long)value)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(uint value) : this((ulong)value)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified double-precision floating-point number.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(double value)
        {
            if (double.IsPositiveInfinity(value))
                this = PositiveInfinity;
            else if (double.IsNegativeInfinity(value))
                this = NegativeInfinity;
            else if (double.IsNaN(value))
                this = NaN;
            else
                this = Parse(value.ToString("G17", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified single-precision floating-point number.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(float value)
        {
            if (float.IsPositiveInfinity(value))
                this = PositiveInfinity;
            else if (float.IsNegativeInfinity(value))
                this = NegativeInfinity;
            else if (double.IsNaN(value))
                this = NaN;
            this = Parse(value.ToString("G9", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NpgsqlDecimal"/> to the value of the specified <see cref="decimal"/> number.
        /// </summary>
        /// <param name="value">The value to represent as a <see cref="NpgsqlDecimal"/>.</param>
        public NpgsqlDecimal(decimal value)
        {
            this = Parse(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="NpgsqlDecimal"/> equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">The string representation of the number to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="NpgsqlDecimal"/> number that is equivalent
        /// to the numeric value contained in <paramref name="value"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="value"/> parameter is <code>null</code>, is not a number in a valid format, or
        /// represents a number less than the allowed range specified by PostgreSQL.</param>
        /// <returns><code>true</code> if <paramref name="value"/> was converted successfully; otherwise, <code>false</code>.</returns>
        public static bool TryParse(string? value, out NpgsqlDecimal result)
        {
            if (value == null)
            {
                result = Zero;
                return false;
            }

            if (value.Equals("NaN", StringComparison.OrdinalIgnoreCase))
            {
                result = NaN;
            }
            else if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase) ||
                     value.Equals("inf", StringComparison.OrdinalIgnoreCase) ||
                     value.Equals("+Infinity", StringComparison.OrdinalIgnoreCase) ||
                     value.Equals("+inf", StringComparison.OrdinalIgnoreCase))
            {
                result = PositiveInfinity;
            }
            else if (value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase) ||
                     value.Equals("-inf", StringComparison.OrdinalIgnoreCase))
            {
                result = NegativeInfinity;
            }
            else
            {
                if (!ParseNumber(value, out result))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="NpgsqlDecimal"/> equivalent.
        /// </summary>
        /// <param name="value">The string representation of the number to convert.</param>
        /// <returns>The equivalent to the number contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not on the correct format, or lies outside the allowed range.</exception>
        public static NpgsqlDecimal Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!TryParse(value, out var result))
                throw new ArgumentException("Invalid numeric: " + value);
            return result;
        }

        static bool ParseNumber(string value, out NpgsqlDecimal result)
        {
            var dweight = -1;
            var dscale = 0;
            var p = 0;
            var haveDp = false;
            Sign sign = Sign.Pos;

            if (0 < value.Length && value[0] == '+')
            {
                sign = Sign.Pos;
                p++;
            }
            else if (0 < value.Length && value[0] == '-')
            {
                sign = Sign.Neg;
                p++;
            }

            if (p < value.Length && value[p] == '.')
            {
                haveDp = true;
                p++;
            }

            if (!IsDigit(value[p]))
            {
                result = Zero;
                return false;
            }

            var decDigits = new byte[value.Length - p + DecDigits * 2];
            var i = DecDigits;
            while (p < value.Length)
            {
                if (IsDigit(value[p]))
                {
                    decDigits[i++] = (byte)(value[p++] - '0');
                    if (!haveDp)
                    {
                        dweight++;
                    }
                    else
                    {
                        dscale++;
                    }
                }
                else if (value[p] == '.')
                {
                    if (haveDp)
                    {
                        result = Zero;
                        return false;
                    }

                    haveDp = true;
                    p++;
                }
                else
                {
                    break;
                }
            }

            var ddigits = i - DecDigits;

            if (p < value.Length)
            {
                if (!(value[p] == 'e' || value[p] == 'E') ||
                    !int.TryParse(value.Substring(p + 1), NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite,
                        CultureInfo.InvariantCulture, out var exponent) ||
                    exponent >= int.MaxValue / 2 || exponent <= -(int.MaxValue / 2))
                {
                    result = Zero;
                    return false;
                }

                dweight += exponent;
                dscale -= exponent;
                dscale = Math.Max(dscale, 0);
            }

            /*
             * Okay, convert pure-decimal representation to base Nbase.  First we need
             * to determine the converted weight and ndigits.  offset is the number of
             * decimal zeroes to insert before the first given digit to have a
             * correctly aligned first Nbase digit.
             */
            int weight;
            if (dweight >= 0)
                weight = (dweight + 1 + DecDigits - 1) / DecDigits - 1;
            else
                weight = -((-dweight - 1) / DecDigits + 1);
            var offset = (weight + 1) * DecDigits - (dweight + 1);
            var ndigits = (ddigits + offset + DecDigits - 1) / DecDigits;

            var digits = new uint[ndigits];

            i = DecDigits - offset;

            var pos = 0;
            while (ndigits-- > 0)
            {
                uint val = decDigits[i];
                for (var j = 1; j < DecDigits; j++)
                {
                    val *= 10;
                    val += decDigits[i + j];
                }
                digits[pos++] = val;
                i += DecDigits;
            }

            var var = new Var()
            {
                _weight = weight,
                _dscale = dscale,
                _sign = sign,
                _ndigits = digits.Length,
                _digits = digits,
                _digitStartPos = 0
            };

            var.Strip();

            if (var._weight < MinWeight || var._weight > MaxWeight || (uint)var._dscale > MaxDigitsAfterDecimalPoint)
            {
                result = Zero;
                return false;
            }

            result = new NpgsqlDecimal(ref var);
            return true;

            static bool IsDigit(char c) => c >= '0' && c <= '9';
        }

        /// <summary>
        /// Adds two specified values.
        /// </summary>
        /// <param name="n1">The first value to add.</param>
        /// <param name="n2">The second value to add.</param>
        /// <returns>The result of adding the two values.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static NpgsqlDecimal operator +(NpgsqlDecimal n1, NpgsqlDecimal n2)
        {
            if (n1._sign == Sign.Nan || n2._sign == Sign.Nan)
                return NaN;
            if (n1._sign == Sign.Pinf)
                return n2._sign == Sign.Ninf ? NaN : PositiveInfinity;
            if (n1._sign == Sign.Ninf)
                return n2._sign == Sign.Pinf ? NaN : NegativeInfinity;
            if (n2._sign == Sign.Pinf)
                return PositiveInfinity;
            if (n2._sign == Sign.Ninf)
                return NegativeInfinity;

            Var.Add(Var.FromNpgsqlDecimal(n1), Var.FromNpgsqlDecimal(n2), out Var result);
            return new NpgsqlDecimal(ref result);
        }

        /// <summary>
        /// Subtracts two specified values.
        /// </summary>
        /// <param name="n1">The minuend.</param>
        /// <param name="n2">The subtrahend.</param>
        /// <returns>The result of subtracting the subtrahend from the minuend.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static NpgsqlDecimal operator -(NpgsqlDecimal n1, NpgsqlDecimal n2)
        {
            if (n1._sign == Sign.Nan || n2._sign == Sign.Nan)
                return NaN;
            if (n1._sign == Sign.Pinf)
                return n2._sign == Sign.Pinf ? NaN : PositiveInfinity;
            if (n1._sign == Sign.Ninf)
                return n2._sign == Sign.Ninf ? NaN : NegativeInfinity;
            if (n2._sign == Sign.Pinf)
                return NegativeInfinity;
            if (n2._sign == Sign.Ninf)
                return PositiveInfinity;

            Var.Sub(Var.FromNpgsqlDecimal(n1), Var.FromNpgsqlDecimal(n2), out Var result);
            return new NpgsqlDecimal(ref result);
        }

        /// <summary>
        /// Multiplies two specified values.
        /// </summary>
        /// <param name="n1">The first value to multiply.</param>
        /// <param name="n2">The second value to multiply.</param>
        /// <returns>The result of multiplying the two values.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static NpgsqlDecimal operator *(NpgsqlDecimal n1, NpgsqlDecimal n2)
        {
            if (n1._sign == Sign.Nan || n2._sign == Sign.Nan)
                return NaN;
            if (n1._sign == Sign.Pinf || n1._sign == Sign.Ninf)
            {
                switch (n2.SignInternal())
                {
                case 0:
                    return NaN; // Inf * 0
                case 1:
                    return n1;
                case -1:
                    return n1._sign == Sign.Pinf ? NegativeInfinity : PositiveInfinity;
                }
            }
            if (n2._sign == Sign.Pinf || n2._sign == Sign.Ninf)
            {
                switch (n1.SignInternal())
                {
                case 0:
                    return NaN; // Inf * 0
                case 1:
                    return n2;
                case -1:
                    return n2._sign == Sign.Pinf ? NegativeInfinity : PositiveInfinity;
                }
            }

            Var.Mul(Var.FromNpgsqlDecimal(n1), Var.FromNpgsqlDecimal(n2), out Var result, n1._dscale + n2._dscale);
            return new NpgsqlDecimal(ref result);
        }

        /// <summary>
        /// Divides two specified values.
        /// </summary>
        /// <param name="n1">The dividend.</param>
        /// <param name="n2">The divisor.</param>
        /// <returns>The result of dividing the dividend by the divisor.</returns>
        /// <exception cref="DivideByZeroException"><paramref name="n2"/> is zero.</exception>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static NpgsqlDecimal operator /(NpgsqlDecimal n1, NpgsqlDecimal n2)
        {
            if (n1._sign == Sign.Nan || n2._sign == Sign.Nan)
                return NaN;
            if (n1._sign == Sign.Pinf || n1._sign == Sign.Ninf)
            {
                if (n2._sign == Sign.Pinf || n2._sign == Sign.Ninf)
                    return NaN;
                switch (n2.SignInternal())
                {
                case 0:
                    throw new DivideByZeroException();
                case 1:
                    return n1;
                case -1:
                    return n1._sign == Sign.Pinf ? NegativeInfinity : PositiveInfinity;
                }
            }
            if (n2._sign == Sign.Pinf || n2._sign == Sign.Ninf)
                return Zero;
            if (n2._digits == null)
                throw new DivideByZeroException();

            var rscale = SelectDivScale(ref n1, ref n2);
            Var.Div(Var.FromNpgsqlDecimal(n1), Var.FromNpgsqlDecimal(n2), out Var result, rscale, true);
            return new NpgsqlDecimal(ref result);
        }

        /// <summary>
        /// Returns the remainder resulting from dividing two specified <see cref="NpgsqlDecimal"/> values.
        /// </summary>
        /// <param name="n1">The dividend.</param>
        /// <param name="n2">The divisor.</param>
        /// <returns>The remainder resulting from dividing the dividend by the divisor.</returns>
        /// <exception cref="DivideByZeroException"><paramref name="n2"/> is zero.</exception>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static NpgsqlDecimal operator %(NpgsqlDecimal n1, NpgsqlDecimal n2)
        {
            if (n1._sign == Sign.Nan || n2._sign == Sign.Nan)
                return NaN;
            if (n1._sign == Sign.Pinf || n1._sign == Sign.Ninf)
            {
                if (n2.SignInternal() == 0)
                    throw new DivideByZeroException();
                return NaN;
            }
            if (n2._sign == Sign.Pinf || n2._sign == Sign.Ninf)
                return n1;

            Var.Mod(Var.FromNpgsqlDecimal(n1), Var.FromNpgsqlDecimal(n2), out Var result);
            return new NpgsqlDecimal(ref result);
        }

        /// <summary>
        /// Negates the value of the specified Decimal operand.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of <paramref name="value"/> multiplied by negative one (-1).</returns>
        public static NpgsqlDecimal operator -(NpgsqlDecimal value)
        {
            switch (value._sign)
            {
            case Sign.Pos:
                if (value._digits == null) // Keep zero positive
                    return value;
                return new NpgsqlDecimal(value._weight, Sign.Neg, value._dscale, value._digits);

            case Sign.Neg:
                return new NpgsqlDecimal(value._weight, Sign.Pos, value._dscale, value._digits);

            case Sign.Pinf:
                return NegativeInfinity;

            case Sign.Ninf:
                return PositiveInfinity;

            default:
                return value;
            }
        }

        /// <summary>
        /// Returns the value of the <see cref="NpgsqlDecimal"/> operand (the sign of the operand is unchanged).
        /// </summary>
        /// <param name="value">The operand to return.</param>
        /// <returns>The value of the operand, <paramref name="value"/>.</returns>
        public static NpgsqlDecimal operator +(NpgsqlDecimal value) => value;


        /// <summary>
        /// Increments the <see cref="NpgsqlDecimal"/> operand by 1.
        /// </summary>
        /// <param name="value">The value to increment</param>
        /// <returns>The value of <paramref name="value"/> incremented by 1.</returns>
        public static NpgsqlDecimal operator ++(NpgsqlDecimal value) => value + One;

        /// <summary>
        /// Decrements the <see cref="NpgsqlDecimal"/> operand by 1.
        /// </summary>
        /// <param name="value">The value to decrement</param>
        /// <returns>The value of <paramref name="value"/> decremented by 1.</returns>
        public static NpgsqlDecimal operator --(NpgsqlDecimal value) => value - One;

        /// <summary>
        /// Returns a value that indicates whether two <see cref="NpgsqlDecimal"/> values are equal.
        /// </summary>
        /// <param name="n1">The first value to compare.</param>
        /// <param name="n2">The second value to compare.</param>
        /// <returns><code>true</code> if the operands are equal; otherwise, <code>false</code>.</returns>
        /// <remarks>The <see cref="Scale"/> is not taken into account when comparing the operands.</remarks>
        public static bool operator ==(NpgsqlDecimal n1, NpgsqlDecimal n2) => n1.Equals(n2);

        /// <summary>
        /// Returns a value that indicates whether two <see cref="NpgsqlDecimal"/> objects have different values.
        /// </summary>
        /// <param name="n1">The first value to compare.</param>
        /// <param name="n2">The second value to compare.</param>
        /// <returns><code>true</code> if the operands are not equal; otherwise, <code>false</code>.</returns>
        /// <remarks>The <see cref="Scale"/> is not taken into account when comparing the operands.</remarks>
        public static bool operator !=(NpgsqlDecimal n1, NpgsqlDecimal n2) => !n1.Equals(n2);

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlDecimal"/> is less than or equal to another specified <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="n1">The first value to compare.</param>
        /// <param name="n2">The second value to compare.</param>
        /// <returns><code>true</code> if <paramref name="n1"/> is less than or equal to <paramref name="n2"/>; otherwise, <code>false</code>.</returns>
        public static bool operator <=(NpgsqlDecimal n1, NpgsqlDecimal n2) => n1.CompareTo(n2) <= 0;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlDecimal"/> is greater than or equal to another specified <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="n1">The first value to compare.</param>
        /// <param name="n2">The second value to compare.</param>
        /// <returns><code>true</code> if <paramref name="n1"/> is greater than or equal to <paramref name="n2"/>; otherwise, <code>false</code>.</returns>
        public static bool operator >=(NpgsqlDecimal n1, NpgsqlDecimal n2) => n1.CompareTo(n2) >= 0;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlDecimal"/> is less than another specified <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="n1">The first value to compare.</param>
        /// <param name="n2">The second value to compare.</param>
        /// <returns><code>true</code> if <paramref name="n1"/> is less than <paramref name="n2"/>; otherwise, <code>false</code>.</returns>
        public static bool operator <(NpgsqlDecimal n1, NpgsqlDecimal n2) => n1.CompareTo(n2) < 0;

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="NpgsqlDecimal"/> is greater than another specified <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="n1">The first value to compare.</param>
        /// <param name="n2">The second value to compare.</param>
        /// <returns><code>true</code> if <paramref name="n1"/> is greater than <paramref name="n2"/>; otherwise, <code>false</code>.</returns>
        public static bool operator >(NpgsqlDecimal n1, NpgsqlDecimal n2) => n1.CompareTo(n2) > 0;

#region Casts

        /// <summary>
        /// Defines an explicit conversion of a double-precision floating-point number to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The double-precision floating-point number to convert.</param>
        /// <returns>The converted double-precision floating-point number.</returns>
        public static explicit operator NpgsqlDecimal(double value) => new NpgsqlDecimal(value);

        /// <summary>
        /// Defines an explicit conversion of a single-precision floating-point number to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The single-precision floating-point number to convert.</param>
        /// <returns>The converted single-precision floating-point number.</returns>
        public static explicit operator NpgsqlDecimal(float value) => new NpgsqlDecimal(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="decimal"/> number to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The <see cref="decimal"/> number to convert.</param>
        /// <returns>The converted number.</returns>
        public static explicit operator NpgsqlDecimal(decimal value) => new NpgsqlDecimal(value);

        /// <summary>
        /// Defines an implicit conversion of a 64-bit unsigned integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 64-bit unsigned integer to convert.</param>
        /// <returns>The converted 64-bit unsigned integer.</returns>
        public static implicit operator NpgsqlDecimal(ulong value) => new NpgsqlDecimal(value);

        /// <summary>
        /// Defines an implicit conversion of a 64-bit signed integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 64-bit signed integer to convert.</param>
        /// <returns>The converted 64-bit signed integer.</returns>
        public static implicit operator NpgsqlDecimal(long value) => new NpgsqlDecimal(value);

        /// <summary>
        /// Defines an implicit conversion of a 32-bit unsigned integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer to convert.</param>
        /// <returns>The converted 32-bit unsigned integer.</returns>
        public static implicit operator NpgsqlDecimal(uint value) => new NpgsqlDecimal((ulong)value);

        /// <summary>
        /// Defines an implicit conversion of a 32-bit signed integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 32-bit signed integer to convert.</param>
        /// <returns>The converted 32-bit signed integer.</returns>
        public static implicit operator NpgsqlDecimal(int value) => new NpgsqlDecimal((long)value);

        /// <summary>
        /// Defines an implicit conversion of a 16-bit unsigned integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 16-bit unsigned integer to convert.</param>
        /// <returns>The converted 16-bit unsigned integer.</returns>
        public static implicit operator NpgsqlDecimal(ushort value) => new NpgsqlDecimal((ulong)value);

        /// <summary>
        /// Defines an implicit conversion of a 16-bit signed integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 16-bit signed integer to convert.</param>
        /// <returns>The converted 16-bit signed integer.</returns>
        public static implicit operator NpgsqlDecimal(short value) => new NpgsqlDecimal((long)value);

        /// <summary>
        /// Defines an implicit conversion of a 8-bit unsigned integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 8-bit unsigned integer to convert.</param>
        /// <returns>The converted 8-bit unsigned integer.</returns>
        public static implicit operator NpgsqlDecimal(byte value) => new NpgsqlDecimal((ulong)value);

        /// <summary>
        /// Defines an implicit conversion of a 8-bit signed integer to a <see cref="NpgsqlDecimal"/>.
        /// </summary>
        /// <param name="value">The 8-bit signed integer to convert.</param>
        /// <returns>The converted 8-bit signed integer.</returns>
        public static implicit operator NpgsqlDecimal(sbyte value) => new NpgsqlDecimal((long)value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a double-precision floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A double-precision floating-point number that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        public static explicit operator double(NpgsqlDecimal value) => ToDouble(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a single-precision floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A single-precision floating-point number that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        public static explicit operator float(NpgsqlDecimal value) => ToSingle(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a <see cref="decimal"/> number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="decimal"/> number that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        public static explicit operator decimal(NpgsqlDecimal value) => ToDecimal(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 64-bit unsigned integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator ulong(NpgsqlDecimal value) => ToUInt64(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 64-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 64-bit signed integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator long(NpgsqlDecimal value) => ToInt64(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 32-bit unsigned integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator uint(NpgsqlDecimal value) => ToUInt32(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 32-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 32-bit signed integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator int(NpgsqlDecimal value) => ToInt32(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 16-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 16-bit unsigned integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator ushort(NpgsqlDecimal value) => ToUInt16(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 16-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 16-bit signed integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator short(NpgsqlDecimal value) => ToInt16(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 8-bit unsigned integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 8-bit unsigned integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator byte(NpgsqlDecimal value) => ToByte(value);

        /// <summary>
        /// Defines an explicit conversion of a <see cref="NpgsqlDecimal"/> to a 8-bit signed integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A 8-bit signed integer that represents the converted <see cref="NpgsqlDecimal"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static explicit operator sbyte(NpgsqlDecimal value) => ToSByte(value);

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent single-precision floating-point number.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A single-precision floating-point number equivalent to the value of <paramref name="value"/>.</returns>
        public static float ToSingle(NpgsqlDecimal value)
            => value._sign switch
            {
                Sign.Pos or Sign.Neg => float.Parse(value.ToString(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                Sign.Pinf => float.PositiveInfinity,
                Sign.Ninf => float.NegativeInfinity,
                _ => float.NaN,
            };

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent double-precision floating-point number.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A double-precision floating-point number equivalent to the value of <paramref name="value"/>.</returns>
        public static double ToDouble(NpgsqlDecimal value)
            => value._sign switch
            {
                Sign.Pos or Sign.Neg => double.Parse(value.ToString(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                Sign.Pinf => double.PositiveInfinity,
                Sign.Ninf => double.NegativeInfinity,
                _ => double.NaN,
            };

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent <see cref="decimal"/> number.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A <see cref="decimal"/> number equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static decimal ToDecimal(NpgsqlDecimal value)
            => value._sign switch
            {
                Sign.Pos or Sign.Neg => decimal.Parse(value.ToString(), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                Sign.Pinf => throw new OverflowException("Cannot convert Infinity to decimal"),
                Sign.Ninf => throw new OverflowException("Cannot convert -Infinity to decimal"),
                _ => throw new OverflowException("Cannot convert NaN to decimal"),
            };

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 64-bit unsigned integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 64-bit unsigned integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static ulong ToUInt64(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, 64, out var result))
                throw new OverflowException("numeric does not fit in System.UInt64: " + value);

            return result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 32-bit unsigned integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 32-bit unsigned integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static uint ToUInt32(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, 32, out var result))
                throw new OverflowException("numeric does not fit in System.UInt32: " + value);

            return (uint)result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 16-bit unsigned integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 16-bit unsigned integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static ushort ToUInt16(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, 16, out var result))
                throw new OverflowException("numeric does not fit in System.UInt16: " + value);

            return (ushort)result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 8-bit unsigned integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 8-bit unsigned integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static byte ToByte(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, 8, out var result))
                throw new OverflowException("numeric does not fit in System.Byte: " + value);

            return (byte)result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 64-bit signed integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 64-bit signed integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static long ToInt64(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, -64, out var result))
                throw new OverflowException("numeric does not fit in System.Int64: " + value);

            return (long)result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 32-bit signed integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 32-bit signed integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static int ToInt32(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, -32, out var result))
                throw new OverflowException("numeric does not fit in System.Int32: " + value);

            return (int)result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 16-bit signed integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 16-bit signed integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static short ToInt16(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, -16, out var result))
                throw new OverflowException("numeric does not fit in System.Int16: " + value);

            return (short)result;
        }

        /// <summary>
        /// Converts the value of the specified <see cref="NpgsqlDecimal"/> to the equivalent 8-bit signed integer.
        /// </summary>
        /// <param name="value">The decimal number to convert.</param>
        /// <returns>A 8-bit signed integer equivalent to the value of <paramref name="value"/>.</returns>
        /// <exception cref="OverflowException">The return value lies outside the allowed range.</exception>
        public static sbyte ToSByte(NpgsqlDecimal value)
        {
            if (!ConvertToInt(value, -8, out var result))
                throw new OverflowException("numeric does not fit in System.SByte: " + value);

            return (sbyte)result;
        }

        static bool ConvertToInt(NpgsqlDecimal value, int bits, out ulong result)
        {
            result = 0;
            switch (value._sign)
            {
            default:
                return false;

            case Sign.Pos:
            case Sign.Neg:
                if (value._digits == null || value._weight < -1)
                    return true;

                if (value._weight >= (19 + DecDigits - 1) / DecDigits)
                    return false;

                var v = Var.FromNpgsqlDecimal(value);
                if (value._weight < 0)
                {
                    v._digits = v._digits!.ToArray();
                    v.Round(0);
                    v.Strip();
                }

                if (v._ndigits == 0)
                    return true;

                result = v._digits![0];
                for (var i = 1; i <= v._weight; i++)
                {
                    var resultTmp = result * Nbase;
                    if (resultTmp / Nbase != result)
                        return false; // overflow
                    if (i < v._ndigits)
                    {
                        result = resultTmp + v._digits[i];
                        if (result < resultTmp)
                            return false; // overflow
                    }
                }

                // The absolute value now fits in an ulong
                if (v._sign == Sign.Pos)
                {
                    switch (bits)
                    {
                    case 64:
                        return true;
                    case 32:
                        return result <= uint.MaxValue;
                    case 16:
                        return result <= ushort.MaxValue;
                    case 8:
                        return result <= byte.MaxValue;
                    case -64:
                        return result <= long.MaxValue;
                    case -32:
                        return result <= int.MaxValue;
                    case -16:
                        return result <= (ulong)short.MaxValue;
                    case -8:
                        return result <= (ulong)sbyte.MaxValue;
                    default:
                        return false; // unreachable
                    }
                }
                else
                {
                    if (result > (ulong)long.MaxValue + 1)
                        return false;
                    var signedResult = -(long)result;
                    result = (ulong)signedResult;
                    switch (bits)
                    {
                    case -64:
                        return true;
                    case -32:
                        return signedResult >= int.MinValue;
                    case -16:
                        return signedResult >= short.MinValue;
                    case -8:
                        return signedResult >= sbyte.MinValue;
                    default:
                        return false;
                    }
                }
            }
        }

#endregion

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified decimal number.
        /// </summary>
        /// <param name="value">A decimal number.</param>
        /// <returns>The smallest integral value that is greater than or equal to the <paramref name="value"/> parameter.
        /// Note that this method returns a <see cref="NpgsqlDecimal"/> instead of an integral type.</returns>
        public static NpgsqlDecimal Ceiling(NpgsqlDecimal value)
        {
            switch (value._sign)
            {
            default:
                return value;

            case Sign.Pos:
            case Sign.Neg:
                Var.Ceil(Var.FromNpgsqlDecimal(value), out Var result);
                return new NpgsqlDecimal(ref result);
            }
        }

        /// <summary>
        /// Rounds a specified <see cref="NpgsqlDecimal"/> number to the closest integer toward negative infinity.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <returns>If <paramref name="value"/> has a fractional part, the next whole <see cref="NpgsqlDecimal"/>
        /// number toward negative infinity that is less than <paramref name="value"/>.
        /// Or, if <paramref name="value"/> doesn't have a fractional part, <paramref name="value"/> is returned unchanged.
        /// Note that the method returns an integral value of type <see cref="NpgsqlDecimal"/>.</returns>
        public static NpgsqlDecimal Floor(NpgsqlDecimal value)
        {
            switch (value._sign)
            {
            default:
                return value;

            case Sign.Pos:
            case Sign.Neg:
                Var.Floor(Var.FromNpgsqlDecimal(value), out Var result);
                return new NpgsqlDecimal(ref result);
            }
        }

        /// <summary>
        /// Rounds a <see cref="NpgsqlDecimal"/> value to a specified number of decimal places.
        /// </summary>
        /// <param name="value">A decimal number to round.</param>
        /// <param name="scale">A value that specifies the number of decimal places to round to.
        /// If this is negative, rounds to the nearest corresponding power of 10.</param>
        /// <returns>The rounded number. The <see cref="Scale"/> of the result will be at most <paramref name="scale"/>, or 0 if negative.</returns>
        public static NpgsqlDecimal Round(NpgsqlDecimal value, int scale = 0)
            => RoundTruncateHelper(value, scale, true, false);

        /// <summary>
        /// Truncates a <see cref="NpgsqlDecimal"/> value to a specified number of decimal places.
        /// </summary>
        /// <param name="value">A decimal number to truncate.</param>
        /// <param name="scale">A value that specifies the number of decimal places to truncate to.
        /// If this is negative, truncates to the nearest corresponding power of 10.</param>
        /// <returns>The truncated number. The <see cref="Scale"/> of the result will be at most <paramref name="scale"/>, or 0 if negative.</returns>
        public static NpgsqlDecimal Truncate(NpgsqlDecimal value, int scale = 0)
            => RoundTruncateHelper(value, scale, false, false);

        /// <summary>
        /// Adjusts the <see cref="Scale"/> of a number to the one specified.
        /// </summary>
        /// <param name="value">A decimal number.</param>
        /// <param name="scale">The new scale (must be non-negative) of the number. If this is less than the current scale, the number will be rounded.</param>
        /// <returns>The new value, after rounding and adjusting the scale.</returns>
        public static NpgsqlDecimal AdjustScale(NpgsqlDecimal value, int scale)
        {
            if (scale < 0)
                throw new ArgumentException("scale must be non-negative", nameof(scale));

            return RoundTruncateHelper(value, scale, true, true);
        }

        static NpgsqlDecimal RoundTruncateHelper(NpgsqlDecimal value, int scale, bool round, bool allowIncreaseDscale)
        {
            switch (value._sign)
            {
            default:
                return value;

            case Sign.Pos:
            case Sign.Neg:
                // PostgreSQL uses [-2000, 2000] for some reason. We accept the full span.
                scale = Math.Min(scale, MaxDigitsAfterDecimalPoint);
                scale = Math.Max(scale, -MaxDigitsBeforeDecimalPoint);

                if (scale >= value._dscale)
                {
                    if (allowIncreaseDscale)
                        return new NpgsqlDecimal(value._weight, value._sign, (ushort)scale, value._digits);
                    return value;
                }

                var v = Var.FromNpgsqlDecimal(value);
                if (round)
                {
                    v._digits = v._digits!.ToArray(); // Be sure to make a copy since rounding changes the array contents
                    v.Round(scale);
                }
                else
                {
                    if (scale % DecDigits != 0)
                        v._digits = v._digits!.ToArray(); // Be sure to make a copy since truncating inside a digit changes the array contents
                    v.Trunc(scale);
                }
                v.Strip();
                v._dscale = Math.Max(scale, 0);
                return new NpgsqlDecimal(ref v);
            }
        }

        /// <summary>
        /// Returns the absolute value of a <see cref="NpgsqlDecimal"/> number.
        /// </summary>
        /// <param name="value">A decimal number</param>
        /// <returns>The absolute value of the number.</returns>
        public static NpgsqlDecimal Abs(NpgsqlDecimal value)
        {
            switch (value._sign)
            {
            case Sign.Ninf:
                return PositiveInfinity;

            case Sign.Pos:
            case Sign.Neg:
                return new NpgsqlDecimal(value._weight, Sign.Pos, value._dscale, value._digits);

            default:
                return value;
            }
        }

        int SignInternal()
        {
            if (_sign == Sign.Pinf)
                return 1;
            if (_sign == Sign.Ninf)
                return -1;
            if (_sign == Sign.Nan)
                throw new InvalidOperationException();
            if ((_digits?.Length ?? 0) == 0)
                return 0;
            return _sign == Sign.Pos ? 1 : -1;
        }

        static int SelectDivScale(ref NpgsqlDecimal n1, ref NpgsqlDecimal n2)
        {
            /*
	         * The result scale of a division isn't specified in any SQL standard. For
	         * PostgreSQL we select a result scale that will give at least
	         * NUMERIC_MIN_SIG_DIGITS significant digits, so that numeric gives a
	         * result no less accurate than float8; but use a scale not less than
	         * either input's display scale.
	         */

            (var weight1, var firstdigit1) = CalcWeightAndFirstDigit(ref n1);
            (var weight2, var firstdigit2) = CalcWeightAndFirstDigit(ref n2);

            /*
	         * Estimate weight of quotient.  If the two first digits are equal, we
	         * can't be sure, but assume that var1 is less than var2.
	         */
            var qweight = weight1 - weight2;
            if (firstdigit1 <= firstdigit2)
                qweight--;

            var rscale = MinSigDigits - qweight * DecDigits;
            rscale = Math.Max(rscale, n1._dscale);
            rscale = Math.Max(rscale, n2._dscale);
            rscale = Math.Min(rscale, MaxDisplayScale);

            return rscale;

            (int weight, uint firstdigit) CalcWeightAndFirstDigit(ref NpgsqlDecimal n)
            {
                var weight = 0;
                var firstdigit = 0U;
                for (var i = 0; i < n._digits!.Length; i++)
                {
                    firstdigit = n._digits![i];
                    if (firstdigit != 0)
                    {
                        weight = n._weight - 1;
                        break;
                    }
                }
                return (weight, firstdigit);
            }
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value.</returns>
        public override string ToString()
        {
            switch (_sign)
            {
            case Sign.Nan: return "NaN";
            case Sign.Pinf: return "Infinity";
            case Sign.Ninf: return "-Infinity";
            }

            var sb = new StringBuilder(Math.Max((_weight + 1) * DecDigits, 1) + _dscale + DecDigits + 1);

            if (_sign == Sign.Neg)
                sb.Append('-');

            var ndigits = _digits?.Length ?? 0;

            int d;
            if (_weight < 0 || ndigits == 0)
            {
                sb.Append('0');
                d = _weight + 1;
            }
            else
            {
                sb.Append(_digits![0]);
                for (d = 1; d <= _weight; d++)
                {
                    var dig = (d < ndigits) ? _digits![d] : 0;
                    uint d1;
                    if (DecDigits >= 8)
                    {
                        d1 = dig / 10000000;
                        dig -= d1 * 10000000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 1000000;
                        dig -= d1 * 1000000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 100000;
                        dig -= d1 * 100000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 10000;
                        dig -= d1 * 10000;
                        sb.Append((char)(d1 + '0'));
                    }
                    if (DecDigits >= 4)
                    {
                        d1 = dig / 1000;
                        dig -= d1 * 1000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 100;
                        dig -= d1 * 100;
                        sb.Append((char)(d1 + '0'));
                    }
                    if (DecDigits >= 2)
                    {
                        d1 = dig / 10;
                        dig -= d1 * 10;
                        sb.Append((char)(d1 + '0'));
                    }
                    sb.Append((char)(dig + '0'));

                }
            }

            if (_dscale > 0)
            {
                sb.Append('.');
                for (var i = 0; i < _dscale; d++, i += DecDigits)
                {
                    var dig = (d >= 0 && d < ndigits) ? _digits![d] : 0;
                    uint d1;
                    if (DecDigits >= 8)
                    {
                        d1 = dig / 10000000;
                        dig -= d1 * 10000000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 1000000;
                        dig -= d1 * 1000000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 100000;
                        dig -= d1 * 100000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 10000;
                        dig -= d1 * 10000;
                        sb.Append((char)(d1 + '0'));
                    }
                    if (DecDigits >= 4)
                    {
                        d1 = dig / 1000;
                        dig -= d1 * 1000;
                        sb.Append((char)(d1 + '0'));
                        d1 = dig / 100;
                        dig -= d1 * 100;
                        sb.Append((char)(d1 + '0'));
                    }
                    if (DecDigits >= 2)
                    {
                        d1 = dig / 10;
                        dig -= d1 * 10;
                        sb.Append((char)(d1 + '0'));
                    }
                    sb.Append((char)(dig + '0'));
                }
                // Truncate the last digits
                var last = _dscale % DecDigits;
                if (last > 0)
                {
                    sb.Length -= DecDigits - last;
                }
            }

            return sb.ToString();
        }

        public int CompareTo(NpgsqlDecimal o)
        {
            if (_sign == Sign.Nan)
                return o._sign == Sign.Nan ? 0 : 1;
            if (_sign == Sign.Pinf)
                return o._sign switch
                {
                    Sign.Nan => -1,
                    Sign.Pinf => 0,
                    _ => 1
                };
            if (_sign == Sign.Ninf)
                return o._sign == Sign.Ninf ? 0 : -1;
            if (o._sign == Sign.Ninf)
                return 1;
            if (o._sign == Sign.Pinf || o._sign == Sign.Nan)
                return -1;

            var v1 = Var.FromNpgsqlDecimal(this);
            var v2 = Var.FromNpgsqlDecimal(o);
            return Var.Compare(ref v1, ref v2);
        }

        public int CompareTo(object? o)
            => o == null
                ? 1
                : o is NpgsqlDecimal npgsqlNumeric
                    ? CompareTo(npgsqlNumeric)
                    : throw new ArgumentException();

        public int Compare(NpgsqlDecimal n1, NpgsqlDecimal n2) => n1.CompareTo(n2);

        public int Compare(object? x, object? y)
        {
            if (x == null)
                return y == null ? 0 : -1;
            if (y == null)
                return 1;
            if (!(x is IComparable) || !(y is IComparable))
                throw new ArgumentException();
            return ((IComparable)x).CompareTo(y);
        }

        public bool Equals(NpgsqlDecimal other)
        {
            return _weight == other._weight && _sign == other._sign &&
                (_digits?.Length ?? 0) == (other._digits?.Length ?? 0) && (_digits?.SequenceEqual(other._digits!) ?? true);
        }

        public override bool Equals(object? obj)
            => obj is NpgsqlDecimal npgsqlDecimal && Equals(npgsqlDecimal);

        public override int GetHashCode()
        {
            var hash = (byte)_sign + _weight * 23;
            if (_digits != null)
            {
                foreach (var d in _digits)
                {
                    hash = hash * 23 + (int)d;
                }
            }
            return hash;
        }

        struct Var
        {
            public int _weight;
            public Sign _sign;
            public int _dscale;
            public uint[]? _digits;
            public int _digitStartPos;
            public int _ndigits;

            static readonly uint[] RoundPowers = { 0, 10000000, 1000000, 100000, 10000, 1000, 100, 10 };
            static readonly Var One = new() { _sign = Sign.Pos, _digits = new uint[] { 1 }, _ndigits = 1 };

            public static Var FromNpgsqlDecimal(NpgsqlDecimal n)
            {
                return new Var()
                {
                    _weight = n._weight,
                    _sign = n._sign,
                    _dscale = n._dscale,
                    _digitStartPos = 0,
                    _ndigits = n._digits?.Length ?? 0,
                    _digits = n._digits
                };
            }

            public static int Compare(ref Var var1, ref Var var2)
            {
                if (var1._ndigits == 0)
                {
                    return var2._ndigits == 0 ? 0 : var2._sign == Sign.Neg ? 1 : -1;
                }
                if (var2._ndigits == 0)
                {
                    return var1._sign == Sign.Pos ? 1 : -1;
                }
                if (var1._sign == Sign.Pos)
                {
                    if (var2._sign == Sign.Neg)
                        return 1;
                    return CompareAbs(ref var1, ref var2);
                }
                else
                {
                    if (var2._sign == Sign.Pos)
                        return -1;
                    return CompareAbs(ref var2, ref var1);
                }
            }

            static int CompareAbs(ref Var var1, ref Var var2)
            {
                var var1ndigits = var1._ndigits;
                var var2ndigits = var2._ndigits;
                var var1digits = var1._digits!;
                var var2digits = var2._digits!;
                var var1digitStartPos = var1._digitStartPos;
                var var2digitStartPos = var2._digitStartPos;
                var var1weight = var1._weight;
                var var2weight = var2._weight;

                var i1 = 0;
                var i2 = 0;

                /* Check any digits before the first common digit */

                while (var1weight > var2weight && i1 < var1ndigits)
                {
                    if (var1digits[var1digitStartPos + i1++] != 0)
                        return 1;
                    var1weight--;
                }
                while (var2weight > var1weight && i2 < var2ndigits)
                {
                    if (var2digits[var2digitStartPos + i2++] != 0)
                        return -1;
                    var2weight--;
                }

                /* At this point, either w1 == w2 or we've run out of digits */

                if (var1weight == var2weight)
                {
                    while (i1 < var1ndigits && i2 < var2ndigits)
                    {
                        var stat = (int)var1digits[var1digitStartPos + i1++] - (int)var2digits[var2digitStartPos + i2++];

                        if (stat != 0)
                        {
                            if (stat > 0)
                                return 1;
                            return -1;
                        }
                    }
                }

                /*
                 * At this point, we've run out of digits on one side or the other; so any
                 * remaining nonzero digits imply that side is larger
                 */
                while (i1 < var1ndigits)
                {
                    if (var1digits[var1digitStartPos + i1++] != 0)
                        return 1;
                }
                while (i2 < var2ndigits)
                {
                    if (var2digits[var2digitStartPos + i2++] != 0)
                        return -1;
                }

                return 0;
            }

            public static void AddAbs(Var var1, Var var2, out Var result)
            {
                var var1ndigits = var1._ndigits;
                var var2ndigits = var2._ndigits;
                var var1digits = var1._digits!;
                var var2digits = var2._digits!;
                var var1digitStartPos = var1._digitStartPos;
                var var2digitStartPos = var2._digitStartPos;

                var resWeight = Math.Max(var1._weight, var2._weight) + 1;
                var resDscale = Math.Max(var1._dscale, var2._dscale);

                var rscale1 = var1ndigits - var1._weight - 1;
                var rscale2 = var2ndigits - var2._weight - 1;
                var resRscale = Math.Max(rscale1, rscale2);

                var resNdigits = resRscale + resWeight + 1;
                if (resNdigits <= 0)
                    resNdigits = 1;

                var resDigits = new uint[resNdigits];
                var i1 = resRscale + var1._weight + 1;
                var i2 = resRscale + var2._weight + 1;
                var carry = 0U;
                for (var i = resNdigits - 1; i >= 0; i--)
                {
                    i1--;
                    i2--;
                    if (i1 >= 0 && i1 < var1ndigits)
                        carry += var1digits[var1digitStartPos + i1];
                    if (i2 >= 0 && i2 < var2ndigits)
                        carry += var2digits[var2digitStartPos + i2];

                    if (carry >= Nbase)
                    {
                        resDigits[i] = carry - Nbase;
                        carry = 1;
                    }
                    else
                    {
                        resDigits[i] = carry;
                        carry = 0;
                    }
                }

                result = new Var
                {
                    _ndigits = resNdigits,
                    _digits = resDigits,
                    _digitStartPos = 0,
                    _weight = resWeight,
                    _dscale = resDscale
                };

                result.Strip();
            }

            public static void SubAbs(Var var1, Var var2, out Var result)
            {
                var var1ndigits = var1._ndigits;
                var var2ndigits = var2._ndigits;
                var var1digits = var1._digits!;
                var var2digits = var2._digits!;
                var var1digitStartPos = var1._digitStartPos;
                var var2digitStartPos = var2._digitStartPos;

                var resWeight = var1._weight;
                var resDscale = Math.Max(var1._dscale, var2._dscale);

                var rscale1 = var1ndigits - var1._weight - 1;
                var rscale2 = var2ndigits - var2._weight - 1;
                var resRscale = Math.Max(rscale1, rscale2);

                var resNdigits = resRscale + resWeight + 1;
                if (resNdigits <= 0)
                    resNdigits = 1;

                var resDigits = new uint[resNdigits];
                var i1 = resRscale + var1._weight + 1;
                var i2 = resRscale + var2._weight + 1;
                var borrow = 0;
                for (var i = resNdigits - 1; i >= 0; i--)
                {
                    i1--;
                    i2--;
                    if (i1 >= 0 && i1 < var1ndigits)
                        borrow += (int)var1digits[var1digitStartPos + i1];
                    if (i2 >= 0 && i2 < var2ndigits)
                        borrow -= (int)var2digits[var2digitStartPos + i2];

                    if (borrow < 0)
                    {
                        resDigits[i] = (uint)(borrow + Nbase);
                        borrow = -1;
                    }
                    else
                    {
                        resDigits[i] = (uint)borrow;
                        borrow = 0;
                    }
                }

                result = new Var
                {
                    _ndigits = resNdigits,
                    _digits = resDigits,
                    _digitStartPos = 0,
                    _weight = resWeight,
                    _dscale = resDscale
                };

                result.Strip();
            }

            public static void Add(Var var1, Var var2, out Var result)
            {
                if (var1._sign == Sign.Pos)
                {
                    if (var2._sign == Sign.Pos)
                    {
                        AddAbs(var1, var2, out result);
                        result._sign = Sign.Pos;
                    }
                    else
                    {
                        switch (CompareAbs(ref var1, ref var2))
                        {
                        default:
                            result = new Var()
                            {
                                _dscale = Math.Max(var1._dscale, var2._dscale)
                            };
                            break;

                        case 1:
                            SubAbs(var1, var2, out result);
                            result._sign = Sign.Pos;
                            break;

                        case -1:
                            SubAbs(var2, var1, out result);
                            result._sign = Sign.Neg;
                            break;
                        }
                    }
                }
                else
                {
                    if (var2._sign == Sign.Pos)
                    {
                        switch (CompareAbs(ref var1, ref var2))
                        {
                        default:
                            result = new Var()
                            {
                                _dscale = Math.Max(var1._dscale, var2._dscale)
                            };
                            break;

                        case 1:
                            SubAbs(var1, var2, out result);
                            result._sign = Sign.Neg;
                            break;

                        case -1:
                            SubAbs(var2, var1, out result);
                            result._sign = Sign.Pos;
                            break;
                        }
                    }
                    else
                    {
                        AddAbs(var1, var2, out result);
                        result._sign = Sign.Neg;
                    }
                }
            }

            public static void Sub(Var var1, Var var2, out Var result)
            {
                var2._sign = var2._sign == Sign.Pos ? Sign.Neg : Sign.Pos;
                Add(var1, var2, out result);
            }

            public static void Mul(Var var1, Var var2, out Var result, int rscale)
            {
                // Result is rounded to no more than rscale fractional digits.

                /*
	             * Arrange for var1 to be the shorter of the two numbers.  This improves
	             * performance because the inner multiplication loop is much simpler than
	             * the outer loop, so it's better to have a smaller number of iterations
	             * of the outer loop.  This also reduces the number of times that the
	             * accumulator array needs to be normalized.
	             */
                if (var1._ndigits > var2._ndigits)
                {
                    var tmp = var1;
                    var1 = var2;
                    var2 = tmp;
                }

                var var1ndigits = var1._ndigits;
                var var2ndigits = var2._ndigits;
                var var1digits = var1._digits!;
                var var2digits = var2._digits!;
                var var1digitStartPos = var1._digitStartPos;
                var var2digitStartPos = var2._digitStartPos;

                if (var1ndigits == 0 || var2ndigits == 0)
                {
                    result = new Var() { _dscale = rscale };
                    return;
                }

                // Determine result sign and (maximum possible) weight
                var resSign = var1._sign == var2._sign ? Sign.Pos : Sign.Neg;
                var resWeight = var1._weight + var2._weight + 2;

                // Determine the number of result digits to compute
                var resNdigits = var1ndigits + var2ndigits + 1;
                var maxdigits = resWeight + 1 + (rscale + DecDigits - 1) / DecDigits + MulGuardDigits;
                resNdigits = Math.Min(resNdigits, maxdigits);

                if (resNdigits < 3)
                {
                    // All input digits will be ignored; so result is zero
                    result = new Var() { _dscale = rscale };
                    return;
                }

                /*
	             * We do the arithmetic in an array "dig[]" of 64-bit integers.  Since
	             * long.MaxValue is noticeably larger than Nbase*Nbase, this gives us headroom
	             * to avoid normalizing carries immediately.
	             *
	             * maxdig tracks the maximum possible value of any dig[] entry; when this
	             * threatens to exceed long.MaxValue, we take the time to propagate carries.
	             * Furthermore, we need to ensure that overflow doesn't occur during the
	             * carry propagation passes either.  The carry values could be as much as
	             * long.MaxValue/Nbase, so really we must normalize when digits threaten to
	             * exceed long.MaxValue - long.MaxValue/Nbase.
	             *
	             * To avoid overflow in maxdig itself, it actually represents the max
	             * possible value divided by Nbase-1, ie, at the top of the loop it is
	             * known that no dig[] entry exceeds maxdig * (Nbase-1).
	             */

                ulong carry;

                var dig = new ulong[resNdigits];
                ulong maxdig = 0;

                /*
	             * The least significant digits of var1 should be ignored if they don't
	             * contribute directly to the first resNdigits digits of the result that
	             * we are computing.
	             *
	             * Digit i1 of var1 and digit i2 of var2 are multiplied and added to digit
	             * i1+i2+2 of the accumulator array, so we need only consider digits of
	             * var1 for which i1 <= resNdigits - 3.
	             */

                for (var i1 = Math.Min(var1ndigits - 1, resNdigits - 3); i1 >= 0; i1--)
                {
                    var var1digit = var1digits[var1digitStartPos + i1];
                    if (var1digit == 0)
                        continue;

                    // Time to normalize?
                    maxdig += var1digit;
                    if (maxdig > (long.MaxValue - long.MaxValue / Nbase) / (Nbase - 1))
                    {
                        // Yes, do it
                        carry = 0;
                        for (var i = resNdigits - 1; i >= 0; i--)
                        {
                            var newdig = dig[i] + carry;
                            if (newdig >= Nbase)
                            {
                                carry = newdig / Nbase;
                                newdig -= carry * Nbase;
                            }
                            else
                            {
                                carry = 0;
                            }
                            dig[i] = newdig;
                        }
                        // Reset maxdig to indicate new worst-case
                        maxdig = 1 + var1digit;
                    }

                    // Inner loop
                    var i2limit = Math.Min(var2ndigits, resNdigits - i1 - 2);
                    var resultPos = i1 + 2;
                    for (var i2 = 0; i2 < i2limit; i2++)
                        dig[resultPos + i2] += (ulong)var1digit * var2digits[var2digitStartPos + i2];
                }

                /*
	             * Now we do a final carry propagation pass to normalize the result, which
	             * we combine with storing the result digits into the output. Note that
	             * this is still done at full precision w/guard digits.
	             */
                var resDigits = new uint[resNdigits];
                carry = 0;
                for (var i = resNdigits - 1; i >= 0; i--)
                {
                    var newdig = dig[i] + carry;
                    if (newdig >= Nbase)
                    {
                        carry = newdig / Nbase;
                        newdig -= carry * Nbase;
                    }
                    else
                    {
                        carry = 0;
                    }
                    resDigits[i] = (uint)newdig;
                }

                result = new Var()
                {
                    _weight = resWeight,
                    _sign = resSign,
                    _digits = resDigits,
                    _digitStartPos = 0,
                    _ndigits = resNdigits
                };
                result.Round(rscale);
                result.Strip();
            }

            public static void Div(Var var1, Var var2, out Var result, int rscale, bool round)
            {
                /*
                 *	The quotient is figured to exactly rscale fractional digits.
                 *	If round is true, it is rounded at the rscale'th digit; if false, it
                 *	is truncated (towards zero) at that digit.
                 */

                var var1ndigits = var1._ndigits;
                var var2ndigits = var2._ndigits;

                if (var2ndigits == 0 || var2._digits![var2._digitStartPos] == 0)
                    throw new DivideByZeroException();

                if (var1ndigits == 0)
                {
                    result = new Var() { _dscale = rscale };
                    return;
                }

                /*
	             * Determine the result sign, weight and number of digits to calculate.
	             * The weight figured here is correct if the emitted quotient has no
	             * leading zero digits; otherwise Strip() will fix things up.
	             */
                var resSign = var1._sign == var2._sign ? Sign.Pos : Sign.Neg;
                var resWeight = var1._weight - var2._weight;
                // The number of accurate result digits we need to produce:
                var resNdigits = resWeight + 1 + (rscale + DecDigits - 1) / DecDigits;
                // ... but always at least 1
                resNdigits = Math.Max(resNdigits, 1);
                // If rounding needed, figure one more digit to ensure correct result
                if (round)
                    resNdigits++;

                /*
	             * The working dividend normally requires resNdigits + var2ndigits
	             * digits, but make it at least var1ndigits so we can load all of var1
	             * into it.  (There will be an additional digit dividend[0] in the
	             * dividend space, but for consistency with Knuth's notation we don't
	             * count that in divNdigits.)
	             */
                var divNdigits = Math.Max(resNdigits + var2ndigits, var1ndigits);

                /*
	             * We need a workspace with room for the working dividend (divNdigits+1
	             * digits) plus room for the possibly-normalized divisor (var2ndigits
	             * digits).  It is convenient also to have a zero at divisor[0] with the
	             * actual divisor data in divisor[1 .. var2ndigits].  Transferring the
	             * digits into the workspace also allows us to realloc the result (which
	             * might be the same as either input var) before we begin the main loop.
	             */
                var dividend = new uint[divNdigits + 1];
                var divisor = new uint[var2ndigits + 1];
                Array.Copy(var1._digits!, var1._digitStartPos, dividend, 1, var1ndigits);
                Array.Copy(var2._digits, var2._digitStartPos, divisor, 1, var2ndigits);

                // Now we can realloc the result to hold the generated quotient digits.
                var resDigits = new uint[resNdigits];

                if (var2ndigits == 1)
                {
                    //If there's only a single divisor digit, we can use a fast path (cf. Knuth section 4.3.1 exercise 16).
                    var divisor1 = divisor[1];
                    ulong carry = 0;
                    for (var i = 0; i < resNdigits; i++)
                    {
                        carry = carry * Nbase + dividend[i + 1];
                        resDigits[i] = (uint)(carry / divisor1);
                        carry %= divisor1;
                    }
                }
                else
                {
                    /*
		             * The full multiple-place algorithm is taken from Knuth volume 2,
		             * Algorithm 4.3.1D.
		             *
		             * We need the first divisor digit to be >= Nbase/2.  If it isn't,
		             * make it so by scaling up both the divisor and dividend by the
		             * factor "d".  (The reason for allocating dividend[0] above is to
		             * leave room for possible carry here.)
		             */
                    if (divisor[1] < HalfNbase)
                    {
                        long d = Nbase / (divisor[1] + 1);
                        long carry = 0;
                        for (var i = var2ndigits; i > 0; i--)
                        {
                            carry += divisor[i] * d;
                            divisor[i] = (uint)(carry % Nbase);
                            carry /= Nbase;
                        }
                        carry = 0;
                        // at this point only var1ndigits of dividend can be nonzero
                        for (var i = var1ndigits; i >= 0; i--)
                        {
                            carry += dividend[i] * d;
                            dividend[i] = (uint)(carry % Nbase);
                            carry /= Nbase;
                        }
                    }
                    var divisor1 = divisor[1];
                    var divisor2 = divisor[2];

                    /*
		             * Begin the main loop.  Each iteration of this loop produces the j'th
		             * quotient digit by dividing dividend[j .. j + var2ndigits] by the
		             * divisor; this is essentially the same as the common manual
		             * procedure for long division.
		             */
                    for (var j = 0; j < resNdigits; j++)
                    {
                        // Estimate quotient digit from the first two dividend digits
                        var next2digits = (ulong)dividend[j] * Nbase + dividend[j + 1];

                        /*
			             * If next2digits are 0, then quotient digit must be 0 and there's
			             * no need to adjust the working dividend.  It's worth testing
			             * here to fall out ASAP when processing trailing zeroes in a
			             * dividend.
			             */
                        if (next2digits == 0)
                        {
                            resDigits[j] = 0;
                            continue;
                        }

                        var qhat = dividend[j] == divisor1 ? Nbase - 1 : (uint)(next2digits / divisor1);

                        /*
			             * Adjust quotient digit if it's too large.  Knuth proves that
			             * after this step, the quotient digit will be either correct or
			             * just one too large.  (Note: it's OK to use dividend[j+2] here
			             * because we know the divisor length is at least 2.)
			             */
                        while (divisor2 * (ulong)qhat > (next2digits - (ulong)qhat * divisor1) * Nbase + dividend[j + 2])
                            qhat--;

                        // As above, need do nothing more when quotient digit is 0
                        if (qhat > 0)
                        {
                            /*
				             * Multiply the divisor by qhat, and subtract that from the
				             * working dividend.  "carry" tracks the multiplication,
				             * "borrow" the subtraction (could we fold these together?)
				             */
                            long carry = 0;
                            var borrow = 0;

                            for (var i = var2ndigits; i >= 0; i--)
                            {
                                carry += divisor[i] * (long)qhat;
                                borrow -= (int)(carry % Nbase);
                                carry /= Nbase;
                                borrow += (int)dividend[j + i];
                                if (borrow < 0)
                                {
                                    dividend[j + i] = (uint)(borrow + Nbase);
                                    borrow = -1;
                                }
                                else
                                {
                                    dividend[j + i] = (uint)borrow;
                                    borrow = 0;
                                }
                            }

                            /*
				             * If we got a borrow out of the top dividend digit, then
				             * indeed qhat was one too large.  Fix it, and add back the
				             * divisor to correct the working dividend.  (Knuth proves
				             * that this will occur only about 3/Nbase of the time; hence,
				             * it's a good idea to test this code with small Nbase to be
				             * sure this section gets exercised.)
				             */
                            if (borrow != 0)
                            {
                                qhat--;
                                var carry2 = 0U;
                                for (var i = var2ndigits; i >= 0; i--)
                                {
                                    carry2 += dividend[j + i] + divisor[i];
                                    if (carry2 >= Nbase)
                                    {
                                        dividend[j + i] = carry2 - Nbase;
                                        carry2 = 1;
                                    }
                                    else
                                    {
                                        dividend[j + i] = carry2;
                                        carry2 = 0;
                                    }
                                }
                            }
                        }
                        resDigits[j] = qhat;
                    }
                }

                result = new Var()
                {
                    _weight = resWeight,
                    _sign = resSign,
                    _digits = resDigits,
                    _digitStartPos = 0,
                    _ndigits = resNdigits
                };
                if (round)
                    result.Round(rscale);
                else
                    result.Trunc(rscale);
                result.Strip();
            }

            public static void Mod(Var var1, Var var2, out Var result)
            {
                Var tmp;
                Div(var1, var2, out tmp, 0, false);
                Mul(var2, tmp, out tmp, var2._dscale);
                Sub(var1, tmp, out result);
            }

            public static void Ceil(Var var, out Var result)
            {
                Var tmp = var;
                tmp.Trunc(0);
                if (var._sign == Sign.Pos && Compare(ref var, ref tmp) != 0)
                    Add(tmp, One, out tmp);
                result = tmp;
            }

            public static void Floor(Var var, out Var result)
            {
                Var tmp = var;
                tmp.Trunc(0);
                if (var._sign == Sign.Neg && Compare(ref var, ref tmp) != 0)
                    Sub(tmp, One, out tmp);
                result = tmp;
            }

            public void Round(int rscale)
            {
                _dscale = rscale;

                // decimal digits wanted
                var di = (_weight + 1) * DecDigits + rscale;

                /*
	             * If di = 0, the value loses all digits, but could round up to 1 if its
	             * first extra digit is >= 5.  If di < 0 the result must be 0.
	             */
                if (di < 0)
                {
                    _ndigits = 0;
                    _weight = 0;
                    _sign = Sign.Pos;
                    _digits = null;
                }
                else
                {
                    var ndigits = (di + DecDigits - 1) / DecDigits;

                    di %= DecDigits;

                    if (ndigits < _ndigits || (ndigits == _ndigits && di > 0))
                    {
                        _ndigits = ndigits;
                        var arrayPos = _digitStartPos + ndigits;

                        uint carry;
                        if (di == 0)
                        {
                            carry = (_digits![arrayPos] >= HalfNbase) ? 1U : 0U;
                        }
                        else
                        {
                            var pow = RoundPowers[di];
                            var extra = _digits![--arrayPos] % pow;
                            _digits[arrayPos] -= extra;
                            carry = 0;
                            if (extra >= pow / 2)
                            {
                                pow += _digits[arrayPos];
                                if (pow >= Nbase)
                                {
                                    pow -= Nbase;
                                    carry = 1;
                                }
                                _digits[arrayPos] = pow;
                            }
                        }

                        while (carry != 0 && arrayPos > 0)
                        {
                            carry += _digits[--arrayPos];
                            if (carry >= Nbase)
                            {
                                _ndigits--;
                                carry = 1;
                            }
                            else
                            {
                                _digits[arrayPos] = carry;
                                carry = 0;
                            }
                        }

                        if (carry != 0)
                        {
                            _digits[0] = carry;
                            _ndigits = 1;
                            _digitStartPos = 0;
                            _weight++;
                        }
                    }
                }
            }

            public void Trunc(int rscale)
            {
                _dscale = rscale;

                var di = (_weight + 1) * DecDigits + rscale;

                if (di <= 0)
                {
                    _ndigits = 0;
                    _weight = 0;
                    _sign = Sign.Pos;
                    _digits = null;
                }
                else
                {
                    var ndigits = (di + DecDigits - 1) / DecDigits;

                    if (ndigits <= _ndigits)
                    {
                        _ndigits = ndigits;
                        di %= DecDigits;
                        if (di > 0)
                        {
                            var arrayPos = _digitStartPos + ndigits - 1;
                            var pow = RoundPowers[di];
                            var extra = _digits![arrayPos] % pow;
                            _digits[arrayPos] -= extra;
                        }
                    }
                }
            }

            public void Strip()
            {
                var pos = _digitStartPos;
                while (_ndigits > 0 && _digits![pos] == 0)
                {
                    pos++;
                    _weight--;
                    _ndigits--;
                }

                while (_ndigits > 0 && _digits![pos + _ndigits - 1] == 0)
                    _ndigits--;

                if (_ndigits == 0)
                {
                    _sign = Sign.Pos;
                    _weight = 0;
                }

                _digitStartPos = pos;
            }

            public void Normalize()
            {
                if (_ndigits == 0)
                {
                    this = new Var() { _dscale = _dscale };
                    return;
                }
                if (_ndigits != _digits!.Length)
                {
                    var newDigits = new uint[_ndigits];
                    Array.Copy(_digits, _digitStartPos, newDigits, 0, _ndigits);
                    _digits = newDigits;
                }
            }
        }

        internal enum Sign : byte
        {
            Pos,
            Neg,
            Nan,
            Pinf,
            Ninf
        }
    }
}
