using System;
using System.Diagnostics;

namespace NpgsqlTypes
{

    /// <summary>
    /// Class that implements PostgreSQL Numeric type with full precision and scale .
    /// This class can represent a PostgreSQL numeric value that allows up to 131072 digits before decimal point
    /// and up to 16383 digits after decimal point.
    /// Arithmetic operators (+,/,-,*) and comparison operators (&lt;,&lt;=,&gt;,&gt;=,==,!=) are implemented.
    /// It also provides specific constructor from well know types (double, long, int, decimal).
    /// Note: This class was based on the Microsoft <see cref="System.Data.SqlTypes.SqlDecimal"/> .Net type,
    /// but changed to support PostgreSQL precision and scale.
    /// <remarks>
    /// https://referencesource.microsoft.com/#System.Data/fx/src/data/System/Data/SQLTypes/SQLDecimal.cs
    /// https://www.postgresql.org/docs/9.1/datatype-numeric.html
    /// </remarks>
    /// </summary>
    public struct NpgsqlDecimal : IComparable, IComparable<NpgsqlDecimal>
    {
        #region Internal constants

        const int SignMask = unchecked((int)0x80000000);
        const int DecimalScaleMask = 0x00FF0000;
        const int DecimalScaleShift = 16;
        const uint InternalScaleMask = 0x7FFFFFFF;

        internal const int MaxDataLimbs = 18432; //131072+16383 digits requires ~ 18432 UI4s (uint) or limbs
        internal const int NUMERIC_MAX_PRECISION = 131072;  // Maximum precision of numeric
        internal const int NUMERIC_MAX_SCALE = 16383;  // Maximum scale of numeric
        private const double DUINT_BASE = (double)x_lInt32Base;     // 2**32
        private const uint DBL_DIG = 17;                       // Max decimal digits of double
        private const byte x_cNumeDivScaleMin = 6;     // Minimum result scale of numeric division

        private const double logOf10Base2 = 3.3219280948873626d;
        const int MaxDecimalScale = 28; //maximum scale that a decimal supports according to Scale decimal documentation.
                                        // The maximum power of 10 that a 32 bit unsigned integer can store
                                        // Fast access for 10^n where n is 0-9
        internal static readonly uint[] Powers10 = new uint[]
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000
        };
        internal static readonly int MaxUInt32Scale = Powers10.Length - 1;
        private const long x_lInt32Base = ((long)1) << 32;      // 2**32
        private const ulong x_ulInt32Base = ((ulong)1) << 32;     // 2**32
        private const ulong x_ulInt32BaseForMod = x_ulInt32Base - 1;    // 2**32 - 1 (0xFFF...FF)
        internal const ulong x_llMax = long.MaxValue;   // Max of Int64
        private const uint x_ulBase10 = 10;

        /// <summary>
        /// Zero NpgsqlDecimal representation.
        /// </summary>
        public static readonly NpgsqlDecimal Zero = new NpgsqlDecimal(0);

        #endregion

        #region Fields and Properties
        uint _flags;
        uint[] _data;


        /// <summary>
        /// Returns true if the number is negative.
        /// </summary>
        public bool Negative => (!Positive);

        /// <summary>
        /// Gets or sets if the number is positive.
        /// </summary>
        public bool Positive {
            get => (_flags & SignMask) > 0;
            set => _flags = value ? (uint)(_flags | SignMask) : (uint)(_flags & ~SignMask);
        }

        /// <summary>
        /// Gets or sets the Scale (number of decimal digits) for the current <see cref="NpgsqlDecimal"/> instance.
        /// </summary>
        public int Scale {
            get => (int)(_flags & InternalScaleMask);
            set => _flags = (uint)((_flags & SignMask) | (value & InternalScaleMask));
        }

        internal int LimbsLength => _data.Length;

        #endregion

        #region Constructors 
        /// <summary>
        /// Constructor for the decimal value.
        /// </summary>
        /// <param name="value"></param>
        public NpgsqlDecimal(decimal value) : this()
        {
            _data = new uint[3];
            var dBits = decimal.GetBits(value);
            _data[0] = (uint)dBits[0];
            _data[1] = (uint)dBits[1];
            _data[2] = (uint)dBits[2];

            var flags = dBits[3];
            Scale = (flags & DecimalScaleMask) >> DecimalScaleShift;
            Positive = (flags & SignMask) == 0;

            if (IsZero())
                Positive = true;

            FixPrecision();
        }

        /// <summary>
        /// Constructor for the integer (32bits) value.
        /// </summary>
        /// <param name="value"></param>
        public NpgsqlDecimal(int value)
        {
            _flags = 0;
            _data = new uint[0];

            var uiValue = (uint)value;
            Positive = true;

            // set the sign bit
            if (value < 0)
            {
                Positive = false;
                // The negative of -2147483648 doesn't fit into int32, directly cast to int should work.
                if (value != int.MinValue)
                    uiValue = (uint)(-value);
            }

            // set the data
            _data = new uint[] { uiValue };

        }

        /// <summary>
        /// Constructor for the long (64bit) value.
        /// </summary>
        /// <param name="value"></param>
        public NpgsqlDecimal(long value)
        {
            _flags = 0;
            _data = new uint[0];

            var dwl = (ulong)value;
            Positive = true;

            // set the sign bit
            if (value < 0)
            {
                Positive = false;
                // The negative of Int64.MinValue doesn't fit into int64, directly cast to ulong should work.
                if (value != long.MinValue)
                    dwl = (ulong)(-value);
            }

            // Copy DWL into bottom 2 UI4s of numeric
            _data = new uint[] { (uint)dwl, (uint)(dwl >> 32) };


            FixPrecision();
        }

        /// <summary>
        /// Constructor for the double precision floating point number value.
        /// </summary>
        /// <param name="dVal"></param>
        public NpgsqlDecimal(double dVal)
        {
            _flags = 0;
            _data = new uint[0];
            Positive = true;

            // Split double to sign, integer, and fractional parts
            if (dVal < 0)
            {
                dVal = -dVal;
                Positive = false;
            }

            var dInt = Math.Floor(dVal);
            var dFrac = dVal - dInt;
            _data = new uint[1];

            var i = 0;
            while (dInt > 0.0)
            {

                dVal = Math.Floor(dInt / DUINT_BASE);
                _data[i] = (uint)(dInt - dVal * DUINT_BASE);
                dInt = dVal;

                i++;
                if (dInt > 0.0 && i >= _data.Length)
                {
                    Array.Resize(ref _data, _data.Length + 1);
                }
            }

            uint ulLen, ulLenDelta;
            uint ulTemp;

            // Get size of the integer part
            ulLen = IsZero() ? 0 : (uint)GetPrecision();
            Debug.Assert(ulLen <= NUMERIC_MAX_PRECISION, "ulLen <= NUMERIC_MAX_PRECISION", "");

            // If we got more than 17 decimal digits, zero lower ones.
            if (ulLen > DBL_DIG)
            {
                // Divide number by 10 while there are more then 17 digits
                var ulWrk = (uint)(ulLen - DBL_DIG);
                do
                {
                    ulTemp = MpDivide(10);
                    ulWrk--;
                }
                while (ulWrk > 0);
                ulWrk = ulLen - DBL_DIG;

                // Round, if necessary. # of digits can change. Cannot be overflow.
                if (ulTemp >= 5)
                {
                    MpAdd(1);
                    ulLen = GetPrecision() + ulWrk;
                }

                // Multiply back
                do
                {
                    MpMultiply(10);
                    ulWrk--;
                }
                while (ulWrk > 0);
            }

            Scale = (int)(ulLen < DBL_DIG ? DBL_DIG - ulLen : 0);

            // Add meaningful fractional part - max 9 digits per iteration
            if (Scale > 0)
            {
                ulLen = (uint)Scale;
                do
                {
                    ulLenDelta = (ulLen >= 9) ? 9 : ulLen;

                    dFrac *= (double)Powers10[(int)ulLenDelta];
                    ulLen -= ulLenDelta;
                    MpMultiply(Powers10[(int)ulLenDelta]);
                    MpAdd((uint)dFrac);
                    dFrac -= Math.Floor(dFrac);
                }
                while (ulLen > 0);
            }

            // Round, if necessary
            if (dFrac >= 0.5)
            {
                MpAdd(1);
            }

            if (IsZero())
                Positive = true;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="value"></param>
        public NpgsqlDecimal(NpgsqlDecimal value) : this()
        {
            _data = new uint[value._data.Length];
            value._data.CopyTo(_data, 0);
            _flags = value._flags;
        }

        /// <summary>
        /// Constructor for the <see cref="NpgsqlDecimal"/> given its internal data representation, scale and sign.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scale"></param>
        /// <param name="isPositive"></param>
        public NpgsqlDecimal(uint[] data, int scale, bool isPositive)
        {
            _data = (uint[])data;
            _flags = 0;
            Positive = isPositive;
            Scale = scale;

        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets the internal representation of the <see cref="NpgsqlDecimal"/> without scale and sign information.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint[] GetBits(NpgsqlDecimal value)
        {
            var result = new uint[value._data.Length];
            Array.Copy(value._data, result, value._data.Length);
            return result;
        }

        /// <summary>
        /// Changes the sign of the <see cref="NpgsqlDecimal"/> value to opposite.
        /// </summary>
        /// <param name="value"></param>
        public static void Negate(ref NpgsqlDecimal value)
            => value.Positive = !value.Positive;

        /// <summary>
        /// Adjusts the size of the given <see cref="NpgsqlDecimal"/> data representation to accomodate higher precision.
        /// </summary>
        /// <param name="data">Internal <see cref="NpgsqlDecimal"/> data representation</param>
        /// <param name="limbs">Number of uint limbs that are added to the internal representation</param>
        private static uint[] ExtendPrecision(uint[] data, ushort limbs)
        {
            var len = data.Length;
            var newData = new uint[len + limbs];
            Array.Copy(data, 0, newData, 0, len);
            return newData;
        }

        /// <summary>
        /// Adds an integer multi precision value to the current internal representation
        /// </summary>
        /// <param name="addend"></param>
        internal void MpAdd(uint addend)
        {
            uint integer;
            uint sum;
            var len = _data.Length;

            for (var i = 0; i < len; i++)
            {
                integer = _data[i];

                if (i == 0)
                {
                    _data[i] = sum = integer + addend;

                    if (sum >= integer && sum >= addend)
                        return;
                }
                else
                {
                    _data[i] = sum = integer + 1;

                    if (sum >= integer && sum >= 1)
                        return;
                }

                if (i == (len - 1) && len >= MaxDataLimbs)
                {
                    throw new OverflowException("Numeric value does not fit in a NpgsqlDecimal");
                }

                if (i == (len - 1))
                {
                    var newData = new uint[len + 1];
                    Array.Copy(_data, 0, newData, 0, len);
                    _data = newData;
                    len++;
                    i++;
                }
            }

        }

        /// <summary>
        /// Multiplies a multi precision integer to the internal representation.
        /// </summary>
        /// <param name="multiplier"></param>
        internal void MpMultiply(uint multiplier)
        {
            ulong integer;
            uint remainder = 0;
            var len = _data.Length;

            for (var i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    integer = (ulong)_data[i] * multiplier;
                }
                else
                {
                    integer = (ulong)_data[i] * multiplier + remainder;
                }

                _data[i] = (uint)integer;
                remainder = (uint)(integer >> 32);

                if ((i == (len - 1) && len > MaxDataLimbs))
                    throw new OverflowException("Numeric value does not fit in a NpgsqlDecimal");


                if (remainder != 0 && i == (len - 1))
                {
                    var newData = new uint[len + 1];
                    Array.Copy(_data, 0, newData, 0, len);
                    _data = newData;
                    len++;
                }
            }
        }

        /// <summary>
        /// Multi-precision one super-digit multiply in place.
        /// D = D * X
        /// Length can increase
        /// </summary>
        /// <param name="piulD">(in/out) D - Divisor <see cref="NpgsqlDecimal"/> internal representation.</param>
        /// <param name="ciulD">(int/out) # of digits in D</param>
        /// <param name="iulX">(in) X</param>
        private static void MpMul1(uint[] piulD, ref int ciulD, uint iulX)
        {
            Debug.Assert(iulX > 0);
            uint ulCarry = 0;
            int iData;
            ulong dwlAccum;

            for (iData = 0; iData < ciulD; iData++)
            {
                var ulTemp = (ulong)piulD[iData];
                dwlAccum = ulCarry + ulTemp * (ulong)iulX;
                ulCarry = HI(dwlAccum);
                piulD[iData] = LO(dwlAccum);
            }

            // If overflow occurs, increase precision
            if (ulCarry != 0)
            {
                piulD[iData] = ulCarry;
                ciulD++;
            }
        }

        /// <summary>
        /// Divides the internal representation by a multi-precision integer.
        /// </summary>
        /// <param name="divisor"></param>
        /// <returns></returns>
        internal uint MpDivide(uint divisor)
        {
            ulong integer;
            uint remainder = 0;
            var len = _data.Length;

            for (var i = (len - 1); i >= 0; i--)
            {
                if (_data[i] != 0 && i == (len - 1))
                {
                    integer = _data[i];
                    _data[i] = (uint)(integer / divisor);
                    remainder = (uint)(integer % divisor);
                }
                else
                {
                    integer = ((ulong)remainder << 32) | _data[i];
                    _data[i] = (uint)(integer / divisor);
                    remainder = (uint)(integer % divisor);
                }
            }

            FixPrecision();

            return remainder;
        }

        /// <summary>
        /// Multi-precision divide.
        /// Q = U / D,
        /// R = U % D,
        /// U and D not changed.
        /// It is Ok for U and R to have the same location in memory,
        /// but then U will be changed.
        /// Assumes that there is enough room in Q and R for results.
        ///
        /// Drawbacks of this implementation:
        ///    1)    Need one extra super-digit (ULONG) in R
        ///    2)    As it modifies D during work, then it do (probably) unnecessary
        ///        work to restore it
        ///    3)    Always get Q and R - if R is unnecessary, can be slightly faster
        /// Most of this can be fixed if it'll be possible to have a working buffer. But
        /// then we'll use alloca() or there will be limit on the upper size of numbers
        /// (maybe not a problem in SQL Server).
        /// </summary>
        /// <param name="rgulU">(in) U - <see cref="NpgsqlDecimal"/> internal representation of the operand to be divided</param>
        /// <param name="ciulU">(in) # of digits in U</param>
        /// <param name="rgulD">(in) D - <see cref="NpgsqlDecimal"/> internal representation of the divisor</param>
        /// <param name="ciulD">(in) # of digits in D</param>
        /// <param name="rgulQ">(out) Q - <see cref="NpgsqlDecimal"/> internal representation of the Quotient</param>
        /// <param name="ciulQ">(out) # of digits in Q</param>
        /// <param name="rgulR">(out) R - <see cref="NpgsqlDecimal"/> internal representation of the Remainder</param>
        /// <param name="ciulR">(out) # of digits in R</param>
        private static void MpDiv(uint[] rgulU, int ciulU, uint[] rgulD, int ciulD, uint[] rgulQ, out int ciulQ, uint[] rgulR, out int ciulR)
        {
            Debug.Assert(ciulU > 0, "ciulU > 0", "In method MpDiv");
            Debug.Assert(ciulD > 0, "ciulD > 0", "In method MpDiv");

            // Division by zero?
            if (ciulD == 1 && rgulD[0] == 0)
            {
                ciulQ = ciulR = 0;
            }

            // Check for simplest case, so it'll be fast
            else if (ciulU == 1 && ciulD == 1)
            {
                MpSet(rgulQ, out ciulQ, rgulU[0] / rgulD[0]);
                MpSet(rgulR, out ciulR, rgulU[0] % rgulD[0]);
            }

            // If D > U then do not divide at all
            else if (ciulD > ciulU)
            {
                MpMove(rgulU, ciulU, rgulR, out ciulR);        // R = U
                MpSet(rgulQ, out ciulQ, 0);                    // Q = 0
            }

            // Try to divide faster - check for remaining good sizes (8 / 4, 8 / 8)
            else if (ciulU <= 2)
            {
                ulong dwlU, dwlD, dwlT;

                dwlU = DWL(rgulU[0], rgulU[1]);
                dwlD = rgulD[0];
                if (ciulD > 1)
                    dwlD += (((ulong)rgulD[1]) << 32);
                dwlT = dwlU / dwlD;
                rgulQ[0] = LO(dwlT);
                rgulQ[1] = HI(dwlT);
                ciulQ = (HI(dwlT) != 0) ? 2 : 1;
                dwlT = dwlU % dwlD;
                rgulR[0] = LO(dwlT);
                rgulR[1] = HI(dwlT);
                ciulR = (HI(dwlT) != 0) ? 2 : 1;
            }

            // If we are dividing by one digit - use simpler routine
            else if (ciulD == 1)
            {
                MpMove(rgulU, ciulU, rgulQ, out ciulQ);        // Q = U
                MpDiv1(rgulQ, ref ciulQ, rgulD[0], out var remainder);     // Q = Q / D, R = Q % D
                rgulR[0] = remainder;
                ciulR = 1;
            }
            //TODO: check GMP division and replace.
            // Worst case. Knuth, "The Art of Computer Programming", 3rd edition, vol.II, Alg.D, pg 272
            else
            {
                ciulQ = ciulR = 0;

                uint D1, ulDHigh, ulDSecond;
                int iulRindex;

                if (rgulU != rgulR)
                    MpMove(rgulU, ciulU, rgulR, out ciulR);        // R = U

                ciulQ = ciulU - ciulD + 1;
                ulDHigh = rgulD[ciulD - 1];

                // D1.    Normalize so high digit of D >= BASE/2 - that guarantee
                //        that QH will not be too far from the correct digit later in D3
                rgulR[ciulU] = 0;
                iulRindex = ciulU;
                D1 = (uint)(x_ulInt32Base / ((ulong)ulDHigh + 1));
                if (D1 > 1)
                {

                    MpMul1(rgulD, ref ciulD, D1);
                    ulDHigh = rgulD[ciulD - 1];
                    MpMul1(rgulR, ref ciulR, D1);
                }
                ulDSecond = rgulD[ciulD - 2];
                // D2 already done - iulRindex initialized before normalization of R.
                // D3-D7. Loop on iulRindex - obtaining digits one-by-one, as "in paper"
                do
                {
                    uint QH, RH;
                    int iulDindex, iulRwork;
                    ulong dwlAccum, dwlMulAccum;

                    // D3. Calculate Q hat - estimation of the next digit
                    dwlAccum = DWL(rgulR[iulRindex - 1], rgulR[iulRindex]);
                    if (ulDHigh == rgulR[iulRindex])
                        QH = (uint)(x_ulInt32Base - 1);
                    else
                        QH = (uint)(dwlAccum / ulDHigh);
                    var ulTemp = (ulong)QH;
                    RH = (uint)(dwlAccum - ulTemp * (ulong)ulDHigh);

                    while (ulDSecond * ulTemp > DWL(rgulR[iulRindex - 2], RH))
                    {
                        QH--;
                        if (RH >= (uint)-((int)ulDHigh))
                            break;
                        RH += ulDHigh;
                        ulTemp = (ulong)QH;
                    }

                    // D4. Multiply and subtract: (some digits of) R -= D * QH
                    for (dwlAccum = x_ulInt32Base, dwlMulAccum = 0, iulDindex = 0, iulRwork = iulRindex - ciulD;
                        iulDindex < ciulD; iulDindex++, iulRwork++)
                    {
                        var ulTemp2 = (ulong)rgulD[iulDindex];
                        dwlMulAccum += (ulong)QH * ulTemp2;
                        dwlAccum += (ulong)rgulR[iulRwork] - LO(dwlMulAccum);
                        dwlMulAccum = (ulong)(HI(dwlMulAccum));
                        rgulR[iulRwork] = LO(dwlAccum);
                        dwlAccum = HI(dwlAccum) + x_ulInt32Base - 1;
                    }
                    dwlAccum += (ulong)rgulR[iulRwork] - dwlMulAccum;
                    rgulR[iulRwork] = LO(dwlAccum);
                    rgulQ[iulRindex - ciulD] = QH;

                    // D5. Test remainder. Carry indicates result<0, therefore QH 1 too large
                    if (HI(dwlAccum) == 0)
                    {
                        // D6. Add back - probabilty is 2**(-31). R += D. Q[digit] -= 1
                        uint ulCarry;

                        rgulQ[iulRindex - ciulD] = QH - 1;
                        for (ulCarry = 0, iulDindex = 0, iulRwork = iulRindex - ciulD;
                            iulDindex < ciulD; iulDindex++, iulRwork++)
                        {
                            dwlAccum = (ulong)rgulD[iulDindex] + (ulong)rgulR[iulRwork] + (ulong)ulCarry;
                            ulCarry = HI(dwlAccum);
                            rgulR[iulRwork] = LO(dwlAccum);
                        }
                        rgulR[iulRwork] += ulCarry;
                    }
                    // D7. Loop on iulRindex
                    iulRindex--;
                }
                while (iulRindex >= ciulD);
                // Normalize results
                MpNormalize(rgulQ, ref ciulQ);
                ciulR = ciulD;
                MpNormalize(rgulR, ref ciulR);
                // D8. Unnormalize: Divide D and R to get result
                if (D1 > 1)
                {
                    MpDiv1(rgulD, ref ciulD, D1, out var ret);
                    MpDiv1(rgulR, ref ciulR, D1, out ret);
                }
            }
        }

        /// <summary>
        /// Merge low and high part uints into a ulong.
        /// </summary>
        /// <param name="lo">least significant part of the long (32 bits)</param>
        /// <param name="hi">highest significant part of the long (32 bits)</param>
        /// <returns></returns>
        internal static ulong DWL(uint lo, uint hi) => (ulong)lo + (((ulong)hi) << 32);

        /// <summary>
        /// Obtains the highest significant part (32 bits) of the given unsigned long number.
        /// </summary>
        /// <param name="x">ulong to obtain the highest significant part.</param>
        /// <returns></returns>
        private static uint HI(ulong x) => (uint)(x >> 32);

        /// <summary>
        /// Obtains the lowest significant part (32 bits) of the given unsigned long number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static uint LO(ulong x) => (uint)x;

        /// <summary>
        /// Move multi-precision number
        /// </summary>
        /// <param name="rgulS">(in) S - Source number internal <see cref="NpgsqlDecimal"/> representation</param>
        /// <param name="ciulS">(in) # of digits in S</param>
        /// <param name="rgulD">(out) D - Destination number internal <see cref="NpgsqlDecimal"/> representation</param>
        /// <param name="ciulD">(out) # of digits in D</param>
        private static void MpMove(uint[] rgulS, int ciulS, uint[] rgulD, out int ciulD)
        {
            ciulD = ciulS;

            Debug.Assert(rgulS.Length >= ciulS, "rgulS.Length >= ciulS", "Invalid array length");
            Debug.Assert(rgulD.Length >= ciulS, "rgulD.Length >= ciulS", "Invalid array length");

            for (var i = 0; i < ciulS; i++)
                rgulD[i] = rgulS[i];
        }

        /// <summary>
        /// Set multi-precision number to one super-digit
        /// </summary>
        /// <param name="rgulD">(out) D - <see cref="NpgsqlDecimal"/> internal representation of a Number </param>
        /// <param name="ciulD">(out) # of digits in D</param>
        /// <param name="iulN">(in) ULONG to set</param>
        private static void MpSet(uint[] rgulD, out int ciulD, uint iulN)
        {
            ciulD = 1;
            rgulD[0] = iulN;
        }

        /// <summary>
        /// Set all extra uints to zero.
        /// Given an index it will set all uint internal representation limbs to 0 til it reaches the array length.
        /// </summary>
        /// <param name="rgulData"><see cref="NpgsqlDecimal"/> internal representation to set extra uints to zero</param>
        /// <param name="cUI4sCur">Index of uint limb after which to set as 0</param>
        private static void ZeroToMaxLen(uint[] rgulData, int cUI4sCur)
        {
            Debug.Assert(rgulData.Length <= MaxDataLimbs, "rgulData.Length == MaxDataLimbs", "Invalid array length");

            for (var i = cUI4sCur; i < rgulData.Length; i++)
                rgulData[i] = 0;

        }

        private void FixPrecision()
        {
            var currLen = _data.Length;
            while (currLen > 1 && _data[currLen - 1] == 0)
                currLen--;
            if (currLen != _data.Length)
                Array.Resize(ref _data, currLen);
        }

        /// <summary>
        /// Converts the <see cref="NpgsqlDecimal"/> to decimal,
        /// It throws an OverflowException if the internal representation cannot be converted to decimal.
        /// </summary>
        /// <returns>decimal number</returns>
        public decimal ToDecimal()
        {
            if (Scale > MaxDecimalScale)
                throw new OverflowException();

            if (_data.Length > 3 && !(_data.Length == 4 && _data[3] == 0))
                throw new OverflowException();

            return new decimal((int)_data[0],
                               _data.Length >= 2 ? (int)_data[1] : 0,
                               _data.Length >= 3 ? (int)_data[2] : 0, Negative, (byte)Scale);
        }

        /// <summary>
        /// Converts the <see cref="NpgsqlDecimal"/> to double if possible otherwise raises an OverflowException.
        /// </summary>
        /// <returns>double number</returns>
        public double ToDouble()
        {
            var dRet = 0.0d;

            if (_data.Length > 32)
                throw new OverflowException("Conversion to double results in overflow");

            for (var i = _data.Length - 1; i >= 0; i--)
            {
                if (i != _data.Length - 1)
                    dRet = dRet * x_lInt32Base + _data[i];
                else dRet = (double)_data[i];
            }

            dRet /= System.Math.Pow(10.0, Scale);

            return Positive ? dRet : -dRet;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="NpgsqlDecimal"/> value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Make local copy of data to avoid modifying input.
            var rgulNumeric = new NpgsqlDecimal(this);
            var precision = GetPrecision() + (Scale > 0 ? 1 : 0) + (Positive ? 0 : 1); //2 from the decimal separator and sign

            var pszTmp = new char[precision];   //Local Character buffer to hold
                                                //the decimal digits, from the
                                                //lowest significant to highest significant

            var iDigits = 0;//Number of significant digits
            uint ulRem; //Remainder of a division by x_ulBase10, i.e.,least significant digit

            // Build the final numeric string by inserting the sign, reversing
            // the order and inserting the decimal number at the correct position

            //Retrieve each digit from the lowest significant digit
            while (rgulNumeric._data.Length > 1 || rgulNumeric._data[0] != 0)
            {
                ulRem = rgulNumeric.MpDivide(Powers10[1]);

                //modulo x_ulBase10 is the lowest significant digit
                pszTmp[iDigits++] = ChFromDigit(ulRem);
            }

            // if scale of the number has not been
            // reached pad remaining number with zeros.
            while (iDigits <= Scale)
            {
                pszTmp[iDigits++] = ChFromDigit(0);
            }


            var uiResultLen = 0;
            var iCurChar = 0;
            char[] szResult;

            // Increment the result length if scale > 0 (need to add '.')            
            if (Scale > 0)
            {
                uiResultLen = 1;
            }

            if (!Negative)
            {
                szResult = new char[uiResultLen + iDigits];
            }
            else
            {
                // Increment the result length if negative (need to add '-')            
                szResult = new char[uiResultLen + iDigits + 1];
                szResult[iCurChar++] = '-';
            }

            while (iDigits > 0)
            {
                if (iDigits-- == Scale)
                    szResult[iCurChar++] = '.';

                szResult[iCurChar++] = pszTmp[iDigits];
            }

            return new string(szResult);
        }

        /// <summary>
        /// Internal implementation of the Hashcode calculation.
        /// Returns the same hashcode taking into account only the scale and internal representation.
        /// Signal is not taken into consideration, e.g. same values of opposite sign will have the same signal in many situations.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (IsZero())
                return 0;

            NpgsqlDecimal ssnumTemp;
            int lActualPrec;

            // First, "normalize" numeric, so that values with different
            // scale/precision will have the same representation.
            ssnumTemp = this;
            lActualPrec = Scale;// GetPrecision(ssnumTemp._data);
            ssnumTemp = ssnumTemp.AdjustScale(NUMERIC_MAX_SCALE - lActualPrec, true);

            // Now evaluate the hash
            var cDwords = ssnumTemp._data.Length;
            var ulValue = (int)0;
            int ulHi;

            // Size of CRC window (hashing bytes, ssstr, sswstr, numeric)
            const int x_cbCrcWindow = 4;
            // const int iShiftVal = (sizeof ulValue) * (8*sizeof(char)) - x_cbCrcWindow;
            const int iShiftVal = 4 * 8 - x_cbCrcWindow;

            var rgiData = ssnumTemp._data;

            for (var i = 0; i < cDwords; i++)
            {
                ulHi = (ulValue >> iShiftVal) & 0xff;
                ulValue <<= x_cbCrcWindow;
                ulValue = (int)(ulValue ^ rgiData[i] ^ ulHi);
            }
            return ulValue;
        }

        /// <summary>
        ///  Compares this instance with a specified object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Equals(object? value)
        {
            if (value == null)
                return false;

            if (!(value is NpgsqlDecimal))
            {
                return false;
            }

            var i = (NpgsqlDecimal)value;

            return (this == i);
        }


        private static char ChFromDigit(uint uiDigit) => (char)(uiDigit + '0');

        /// <summary>
        /// Adjusts the scale of the current <see cref="NpgsqlDecimal"/> value to the given scale according to the given round rule,
        /// returning a new instance of <see cref="NpgsqlDecimal"/> adjusted to the given scale.
        /// </summary>
        /// <param name="newScale">New scale for the returned value</param>
        /// <param name="round">If the number should be rounded for the given scale</param>
        /// <returns></returns>
        public NpgsqlDecimal AdjustScale(int newScale, bool round = true)
        {
            if (NpgsqlDecimal.IsZero(ref this))
                return Zero;

            var ret = new NpgsqlDecimal(this);

            var lScaleAdjust = newScale - (int)Scale;//Adjustment to scale

            //Adjust scale
            ret.AdjustScaleWithDifference(lScaleAdjust, round);

            return ret;
        }

        private void AdjustScaleWithDifference(int digits, bool fRound)
        {
            uint ulRem;                  //Remainder when downshifting
            uint ulShiftBase;            //What to multiply by to effect scale adjust
            var fNeedRound = false;     //Do we really need to round?
            int bNewScale;
            var lAdjust = digits;

            //If downshifting causes truncation of data
            if (lAdjust + Scale < 0)
                throw new ArithmeticException("Npgsql value truncated");

            //If uphifting causes scale overflow
            if (lAdjust + Scale > NUMERIC_MAX_PRECISION)
                throw new OverflowException("Overflow");

            bNewScale = (lAdjust + Scale);

            if (lAdjust > 0)
            {
                Scale = bNewScale;

                while (lAdjust > 0)
                {
                    //if lAdjust>=9, downshift by 10^9 each time, otherwise by the full amount
                    if (lAdjust >= 9)
                    {
                        ulShiftBase = Powers10[8 + 1];
                        lAdjust -= 9;
                    }
                    else
                    {
                        ulShiftBase = Powers10[lAdjust];
                        lAdjust = 0;
                    }
                    MpMultiply(ulShiftBase);
                }
            }
            else if (lAdjust < 0)
            {
                do
                {
                    if (lAdjust <= -9)
                    {
                        ulShiftBase = Powers10[8 + 1];
                        lAdjust += 9;
                    }
                    else
                    {
                        ulShiftBase = Powers10[-lAdjust];
                        lAdjust = 0;
                    }
                    ulRem = MpDivide(ulShiftBase);
                }
                while (lAdjust < 0);

                // Do we really need to round?
                fNeedRound = (ulRem >= ulShiftBase / 2);

                Scale = bNewScale;
            }


            // After adjusting, if the result is 0 and remainder is less than 5,
            // set the sign to be positive and return.
            if (fNeedRound && fRound)
            {
                // If remainder is 5 or above, increment/decrement by 1.
                MpAdd(1);
            }
            else if (IsZero(ref this))
                Positive = true;
        }

        /// <summary>
        /// Rounds a specified <see cref="NpgsqlDecimal"/> number to the next lower whole number.
        /// </summary>
        /// <param name="value">The <see cref="NpgsqlDecimal"/> structure for which the floor value is to be calculated.</param>
        /// <returns>Returns a <see cref="NpgsqlDecimal"/> structure that contains the whole number part of
        /// this <see cref="NpgsqlDecimal"/> structure.</returns>
        public static NpgsqlDecimal Floor(NpgsqlDecimal value) => value.AdjustScale(0, false);

        /// <summary>
        /// Returns the smallest whole number greater than or equal to the specified <see cref="NpgsqlDecimal"/> structure.
        /// </summary>
        /// <param name="value">The <see cref="NpgsqlDecimal"/> structure for which the ceiling value is to be calculated.</param>
        /// <returns>Returns a <see cref="NpgsqlDecimal"/> structure representing the smallest whole number
        /// greater than or equal to the specified <see cref="NpgsqlDecimal"/> structure.</returns>
        public static NpgsqlDecimal Ceiling(NpgsqlDecimal value) => value.AdjustScale(0, true);

        /// <summary>
        /// The Abs method gets the absolute value of the <see cref="NpgsqlDecimal"/> parameter.
        /// </summary>
        /// <param name="value">A <see cref="NpgsqlDecimal"/> structure.</param>
        /// <returns>A <see cref="NpgsqlDecimal"/> structure whose value
        /// contains the unsigned number representing the absolute value of the <see cref="NpgsqlDecimal"/> parameter.</returns>
        public static NpgsqlDecimal Abs(NpgsqlDecimal value) => new NpgsqlDecimal(value._data, value.Scale, true);

        /// <summary>
        /// Compare the absolute value of two numerics without checking scale.
        /// Operand 1 is the current instance, Operand 2 is snumOp.
        /// </summary>
        /// <param name="snumOp"></param>
        /// <returns>
        ///        positive    - |this| &gt; |snumOp|
        ///        0            - |this| = |snumOp|
        ///        negative    - |this| &lt; |snumOp|
        /// </returns>
        private int LAbsCmp(NpgsqlDecimal snumOp)
        {

            int iData;  //which UI4 we are operating on
            int culOp;  //#of UI4s on operand
            int culThis; //# of UI4s in this

            // If one longer, it is larger
            culOp = snumOp._data.Length;
            culThis = _data.Length;
            if (culOp != culThis)
                return (culThis > culOp) ? 1 : -1;

            var rglData1 = _data;
            var rglData2 = snumOp._data;

            // Loop through numeric value checking each byte for differences.
            iData = culOp - 1;
            do
            {
                // 

                if (rglData1[iData] != rglData2[iData])
                    return ((rglData1[iData] > rglData2[iData]) ? 1 : -1);
                iData--;
            }
            while (iData >= 0);


            // All UI4s the same, return 0.
            return 0;
        }

        /// <summary>
        /// Parses a string that contains a number to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static NpgsqlDecimal Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(nameof(s));

            if (s == "0")
                return NpgsqlDecimal.Zero;

            var snResult = new NpgsqlDecimal(0);

            var rgwchStr = s.ToCharArray();
            var cwchStr = rgwchStr.Length;

            int iData;            //index to string
            char usChar;            //current value in string
            var lDecPnt = -1;    //position of decimal point in string
            var iCurChar = 0;

            //Must initialize precision and scale to valid values
            int precision;
            snResult.Scale = 0;

            // Trim trailing blanks.
            while (cwchStr != 0 && rgwchStr[cwchStr - 1] == ' ')
                cwchStr--;

            // If string contains only spaces, stop
            if (cwchStr == 0)
                throw new FormatException("Wrong NpgsqlDecimal format");

            // Trim leading blanks.
            while (rgwchStr[iCurChar] == ' ')
            {
                iCurChar++;
                cwchStr--;
            }

            // Get sign for numeric value.
            if (rgwchStr[iCurChar] == '-')
            {
                NpgsqlDecimal.Negate(ref snResult);
                iCurChar++;
                cwchStr--;
            }
            else
            {
                snResult.Positive = true;
                if (rgwchStr[iCurChar] == '+')
                {
                    iCurChar++;
                    cwchStr--;
                }
            }

            // Hack: Check for "0.". If so, replace by ".0".
            while ((cwchStr > 2) && (rgwchStr[iCurChar] == '0'))
            {
                iCurChar++;
                cwchStr--;
            }
            if (2 == cwchStr && '0' == rgwchStr[iCurChar] && '.' == rgwchStr[iCurChar + 1])
            {
                rgwchStr[iCurChar] = '.';
                rgwchStr[iCurChar + 1] = '0';
            }

            // Invalid string?
            if (cwchStr == 0 || cwchStr > (NUMERIC_MAX_PRECISION + NUMERIC_MAX_SCALE + 2))
                throw new FormatException("Wrong NpgsqlDecimal format");

            // Trim leading zeros.  (There shouldn't be any except for floats
            // less than 1.  e.g.  0.01)
            while ((cwchStr > 1) && (rgwchStr[iCurChar] == '0'))
            {
                iCurChar++;
                cwchStr--;
            }

            // Convert string to numeric value by looping through input string.
            for (iData = 0; iData < cwchStr; iData++)
            {
                usChar = rgwchStr[iCurChar];
                iCurChar++;

                if (usChar >= '0' && usChar <= '9')
                    usChar -= '0';
                else if (usChar == '.' && lDecPnt < 0)
                {
                    lDecPnt = iData;
                    continue;
                }
                else
                    throw new FormatException("Wrong NpgsqlDecimal format");

                snResult.MpMultiply(x_ulBase10);
                snResult.MpAdd(usChar);
            }

            // Save precision and scale.
            if (lDecPnt < 0)
            {
                precision = iData;
                snResult.Scale = 0;
            }
            else
            {
                precision = (iData - 1);
                snResult.Scale = (precision - lDecPnt);
            }

            //Check for overflow condition
            if ((precision - snResult.Scale) > NUMERIC_MAX_PRECISION)
                throw new FormatException($"Wrong NpgsqlDecimal format, number as more than {NUMERIC_MAX_PRECISION} digits before decimal point allowed by PostgreSQL Numeric type");

            // Check for invalid precision for numeric value.
            // e.g., when string is ".", precision will be 0
            if (precision == 0)
                throw new FormatException("Wrong NpgsqlDecimal format");

            if (snResult.Scale > NUMERIC_MAX_SCALE)
                throw new FormatException($"Wrong NpgsqlDecimal format, number has more than {NUMERIC_MAX_SCALE} decimal digits allowed by PostgreSQL Numeric type");

            // If result is -0, adjust sign to positive.
            if (snResult.IsZero())
                snResult.Positive = true;

            return snResult;
        }

        /// <summary>
        /// Checks if the <see cref="NpgsqlDecimal"/> is zero or not.
        /// </summary>
        /// <returns></returns>
        public bool IsZero() => IsZero(ref this);

        /// <summary>
        /// Returns true ifthe given <see cref="NpgsqlDecimal"/> value is zero or false otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsZero(ref NpgsqlDecimal value)
        {
            for (var i = 0; i < value._data.Length; i++)
            {
                if (value._data[i] != 0)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Implements Multi Precision division for single uint divisors (uint).
        /// Result is stored in the given U operand <see cref="NpgsqlDecimal"/> internal representation.
        /// Multi-precision one super-digit divide in place.
        /// U = U / D,
        /// R = U % D
        /// Length of U can decrease        
        /// </summary>
        /// <param name="rgulU">(in/out) U - <see cref="NpgsqlDecimal"/> internal representation to be divided</param>
        /// <param name="ciulU">(in/out) # of digits in U</param>
        /// <param name="iulD">(in) D - uint divisor</param>
        /// <param name="iulR">(out) R - result</param>
        private static void MpDiv1(uint[] rgulU, ref int ciulU, uint iulD, out uint iulR)
        {
            Debug.Assert(rgulU.Length <= MaxDataLimbs);

            uint ulCarry = 0;
            ulong dwlAccum;
            var ulD = (ulong)iulD;
            var idU = ciulU;

            Debug.Assert(iulD != 0, "iulD != 0", "Divided by zero!");
            Debug.Assert(iulD > 0, "iulD > 0", "Invalid data: less than zero");
            Debug.Assert(ciulU > 0, "ciulU > 0", "No data in the array");

            while (idU > 0)
            {
                idU--;
                dwlAccum = (((ulong)ulCarry) << 32) + (ulong)(rgulU[idU]);
                rgulU[idU] = (uint)(dwlAccum / ulD);
                ulCarry = (uint)(dwlAccum - (ulong)rgulU[idU] * ulD);  // (ULONG) (dwlAccum % iulD)
            }
            iulR = ulCarry;
            MpNormalize(rgulU, ref ciulU);
        }

        /// <summary>
        /// Normalize multi-precision number - remove leading zeroes
        /// </summary>
        /// <param name="rgulU">(in) <see cref="NpgsqlDecimal"/> internal representation to be normalized</param>
        /// <param name="ciulU">(in/out) Index for which the limb is not zero </param>
        private static void MpNormalize(uint[] rgulU, ref int ciulU)
        {
            while (ciulU > 1 && rgulU[ciulU - 1] == 0)
                ciulU--;
        }


        /// <summary>
        /// Gets the number of bits in an integer.
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        private static int BitSizeOfInt(uint bits)
        {
            var size = 0;
            while (bits != 0)
            {
                bits >>= 1;
                size++;
            }
            return size;
        }

        internal uint GetPrecision()
        {
            FixPrecision();

            return CalculatePrecision(_data);
        }

        private static uint CalculatePrecision(uint[] bitData)
        {
            var totalBits = BitSizeOfInt(bitData[bitData.Length - 1]);
            var len = bitData.Length - 1;
            totalBits = ((len * 4 * 8) + totalBits);

            //# of digits = ceiling(bits /log2(10))
            //https://www.exploringbinary.com/number-of-bits-in-a-decimal-integer/

            //TODO: verify issues with this calculation for NUMERIC_MAX_PRECISION and NUMERIC_MAX_SCALE
            var result = (uint)(Math.Ceiling(totalBits / logOf10Base2));
            return result == 0 ? 1 : result;
        }

        #endregion

        #region Implicit Explicit conversions

        /// <summary>
        /// Explicit conversion from Decimal to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(decimal x) => new NpgsqlDecimal(x);

        /// <summary>
        /// Explicit conversion from Double to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(double x) => new NpgsqlDecimal(x);

        /// <summary>
        /// Implicit conversion from SqlBoolean to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(bool x) => new NpgsqlDecimal(x ? 1 : 0);

        /// <summary>
        /// Implicit conversion from SqlByte to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(byte x) => new NpgsqlDecimal((int)(x));

        /// <summary>
        /// Implicit conversion from SqlInt16 to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(short x) => new NpgsqlDecimal((int)x);

        /// <summary>
        /// Implicit conversion from SqlInt32 to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(int x) => new NpgsqlDecimal(x);

        /// <summary>
        /// Implicit conversion from SqlInt64 to <see cref="NpgsqlDecimal"/>
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        public static explicit operator NpgsqlDecimal(long x) => new NpgsqlDecimal(x);

        #endregion

        #region Arithmetic operators

        /// <summary>
        /// Sum operator.
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand</param>
        /// <returns>Sum result of <see cref="NpgsqlDecimal"/> operands (x+y)</returns>
        public static NpgsqlDecimal operator +(NpgsqlDecimal x, NpgsqlDecimal y)
        {
            if (IsZero(ref x) || IsZero(ref y))
                return Zero;

            ulong dwlAccum;           //accumulated sum
            bool fMySignPos;         //sign of x was positive at start
            bool fOpSignPos;         // sign of y positive at start
            var fResSignPos = true; //sign of result should be positive
            int myScale;    //scale of x
            int opScale;    //scale of y
            int resScale;   //scale of result
            //int ResPrec;    //precision of result
            //int ResInteger; //number of digits for the integer part of result
            int culOp1;     //# of UI4s in x
            int culOp2;     //# of UI4s in y
            int iulData;    //which UI4 we are operating on in x, y

            fMySignPos = !x.Negative;
            fOpSignPos = !y.Negative;

            //result scale = max(s1,s2)
            //result precison = max(s1,s2) + max(p1-s1,p2-s2)
            myScale = x.Scale;
            opScale = y.Scale;

            // Calculate the scale of the result.
            resScale = Math.Max(myScale, opScale);
            Debug.Assert(resScale <= NUMERIC_MAX_SCALE);

            // Adjust both operands to be the same scale as ResScale.
            if (myScale != resScale)
                x = x.AdjustScale(resScale - myScale, true);

            if (opScale != resScale)
                y = y.AdjustScale(resScale - opScale, true);

            // When sign of first operand is negative
            // negate all operands including result.
            if (!fMySignPos)
            {
                fOpSignPos = !fOpSignPos;
                fResSignPos = !fResSignPos;
            }

            // Initialize operand lengths and pointer.
            culOp1 = x._data.Length;
            culOp2 = y._data.Length;

            var rglData1 = new uint[x._data.Length];
            var rglData2 = new uint[y._data.Length];
            Array.Copy(x._data, rglData1, x._data.Length);
            Array.Copy(y._data, rglData2, y._data.Length);

            if (fOpSignPos)
            {
                dwlAccum = 0;

                // Loop through UI4s adding operands and putting result in *this
                // of the operands and put result in *this
                for (iulData = 0; iulData < culOp1 || iulData < culOp2; iulData++)
                {
                    // None of these DWORDLONG additions can overflow, as dwlAccum comes in < x_lInt32Base
                    if (iulData < culOp1)
                        dwlAccum += rglData1[iulData];
                    if (iulData < culOp2)
                        dwlAccum += rglData2[iulData];

                    rglData1[iulData] = (uint)dwlAccum; // equiv to mod x_lInt32Base
                    dwlAccum >>= 32; // equiv to div x_lInt32Base
                }

                //If carry
                if (dwlAccum != 0)
                {
                    Debug.Assert(dwlAccum < x_ulInt32Base);

                    //Either overflowed
                    if (iulData == MaxDataLimbs)
                        throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");

                    // Or extended length
                    if (rglData1.Length < (iulData + 1))
                    {
                        rglData1 = ExtendPrecision(rglData1, 1);
                    }

                    rglData1[iulData] = (uint)dwlAccum;
                }
            }
            else
            {
                // When second operand is negative, switch operands
                // if operand2 is greater than operand1
                if (x.LAbsCmp(y) < 0)
                {
                    fResSignPos = !fResSignPos;
                    var rguiTemp = rglData2;
                    rglData2 = rglData1;
                    rglData1 = rguiTemp;
                    culOp1 = culOp2;
                    culOp2 = x._data.Length;
                }

                dwlAccum = x_ulInt32Base;
                for (iulData = 0; iulData < culOp1 || iulData < culOp2; iulData++)
                {
                    if (iulData < culOp1)
                        dwlAccum += rglData1[iulData];
                    if (iulData < culOp2)
                        dwlAccum -= rglData2[iulData];

                    rglData1[iulData] = (uint)dwlAccum; // equiv to mod BaseUI4

                    dwlAccum >>= 32; // equiv to /= BaseUI4
                    dwlAccum += x_ulInt32BaseForMod; // equiv to BaseUI4 - 1
                }
            }

            var ret = new NpgsqlDecimal(rglData1, resScale, fResSignPos);
            ret.FixPrecision();

            if (ret.Scale > NUMERIC_MAX_SCALE)
                ret = ret.AdjustScale(NUMERIC_MAX_SCALE);

            if ((ret.GetPrecision() - ret.Scale) > NUMERIC_MAX_PRECISION)
                throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");


            if (IsZero(ref ret))
                ret.Positive = true;

            return ret;
        }

        /// <summary>
        /// Implements the unary - operator (negate sign).
        /// </summary>
        /// <param name="x"></param>
        /// <returns>Result of the subtraction respecting scale of the operands.</returns>
        public static NpgsqlDecimal operator -(NpgsqlDecimal x)
        {
            var s = x;
            if (s.IsZero())
                s.Positive = true;
            else NpgsqlDecimal.Negate(ref s);
            return s;
        }

        /// <summary>
        /// Subtraction Operator implementation.
        /// </summary>
        /// <param name="x">Operand that will be substracted.</param>
        /// <param name="y">Operand to substract</param>
        /// <returns>Result of the addition respecting the scale of the operands</returns>
        public static NpgsqlDecimal operator -(NpgsqlDecimal x, NpgsqlDecimal y) => x + (-y);

        /// <summary>
        ///         
        ///
        ///   Multiply two numerics.
        ///
        /// Parameters:
        ///       x    - IN Multiplier
        ///       y    - IN Multiplicand
        ///
        ///   Result scale and precision(same as in SQL Server Manual and Hydra):
        ///       scale = s1 + s2
        ///       precison = s1 + s2 + (p1 - s1) + (p2 - s2) + 1
        ///
        ///   Overflow Rules:
        ///       If scale is greater than NUMERIC_MAX_SCALE it is set to
        ///   NUMERIC_MAX_SCALE.  If precision before decimal digit separator is greater than NUMERIC_MAX_PRECISION
        ///   Overflow exception is raised.
        ///
        ///   Algorithm:
        ///       Starting from the lowest significant uint limb, for each uint limb of the multiplier
        ///   iterate through the uint limbs of the multiplicand starting from
        ///   the least significant UI4s, multiply the multiplier uint with
        ///   multiplicand uint, update the result buffer with the product modulo
        ///   x_dwlBaseUI4 at the same index as the multiplicand, and carry the quotient to
        ///   add to the next multiplicand uint.  Until the end of the multiplier data
        ///   array is reached.
        ///
        /// </summary>
        /// <param name="x">Operand to be multipled.</param>
        /// <param name="y">Operand that is the multiplier</param>
        /// <returns>Result of the multiplication respecting scale of operands</returns>
        public static NpgsqlDecimal operator *(NpgsqlDecimal x, NpgsqlDecimal y)
        {
            //Implementation:
            //        I) Figure result scale,prec
            //        II) Perform mult.
            //        III) Adjust product to result scale,prec

            // Local variables for actual multiplication
            int iulPlier;           //index of UI4 in the Multiplier
            uint ulPlier;            //current mutiplier UI4
            ulong dwlAccum;           //accumulated sum
            ulong dwlNextAccum;       //overflow of accumulated sum
            var culCand = y._data.Length; //length of multiplicand in UI4s

            //Local variables to track scale,precision
            int actualScale;                    // Scale after mult done
            int resScale;                       // Final scale we will force result to
            int resPrec;                        // Final precision we will force result to
            int resInteger;                     // # of digits in integer part of result (prec-scale)
            int lScaleAdjust;   //How much result scale will be adjusted
            bool fResPositive;  // Result sign

            NpgsqlDecimal ret;

            //I) Figure result prec,scale
            actualScale = x.Scale + y.Scale;
            resScale = actualScale;
            resInteger = ((int)x.GetPrecision() - x.Scale) + ((int)y.GetPrecision() - y.Scale) + 1;

            //result precison = s1 + s2 + (p1 - s1) + (p2 - s2) + 1
            resPrec = resScale + resInteger;

            // Downward adjust res prec,scale if either larger than NUMERIC_MAX_PRECISION           
            if (resScale > NUMERIC_MAX_SCALE)
                resScale = NUMERIC_MAX_SCALE;
            if ((resPrec - resScale) > NUMERIC_MAX_PRECISION)
                resPrec = NUMERIC_MAX_PRECISION;

            //
            // It is possible when two large numbers are being multiplied the scale
            // can be reduced to 0 to keep data untruncated; the fix here is to
            // preserve a minimum scale of 6.
            //
            // If overflow, reduce the scale to avoid truncation of data
            resScale = Math.Min((resPrec - resInteger), resScale);
            // But keep a minimum scale of NUMERIC_MIN_DVSCALE
            resScale = Math.Max(resScale, Math.Min(actualScale, x_cNumeDivScaleMin));

            lScaleAdjust = resScale - actualScale;

            fResPositive = (x.Positive == y.Positive);//positive if both signs same.

            // II) Perform multiplication
            var rglData1 = new uint[x._data.Length];
            var rglData2 = new uint[y._data.Length];
            Array.Copy(x._data, rglData1, x._data.Length);
            Array.Copy(y._data, rglData2, y._data.Length);

            //Local buffer to hold the result of multiplication.
            //Longer than CReNumeBuf because full precision of multiplication is carried out
            var x_culNumeMultRes = x._data.Length + y._data.Length + 2;       // Maximum # UI4s in result buffer in multiplication
            var rgulRes = new uint[x_culNumeMultRes]; //new [] are already initialized to zero
            int culRes;             // # of UI4s in result
            var idRes = (int)0;

            //Iterate over the bytes of multiplier
            for (iulPlier = 0; iulPlier < x._data.Length; iulPlier++)
            {
                ulPlier = rglData1[iulPlier];
                dwlAccum = 0;

                //Multiply each UI4 of multiCand by ulPliear and accumulate into result buffer

                // Start on correct place in result
                idRes = iulPlier;

                for (var iulCand = 0; iulCand < culCand; iulCand++)
                {
                    // dwlAccum = dwlAccum + rgulRes[idRes] + ulPlier*rglData2[iulCand]
                    //        use dwlNextAccum to detect overflow of DWORDLONG
                    dwlNextAccum = dwlAccum + rgulRes[idRes];
                    var ulTemp = (ulong)rglData2[iulCand];
                    dwlAccum = (ulong)ulPlier * ulTemp;
                    dwlAccum += dwlNextAccum;
                    if (dwlAccum < dwlNextAccum) // indicates dwl addition overflowed
                        dwlNextAccum = x_ulInt32Base; // = maxUI64/x_dwlBaseUI4
                    else
                        dwlNextAccum = 0;

                    // Update result and accum
                    rgulRes[idRes++] = (uint)(dwlAccum);// & x_ulInt32BaseForMod); // equiv to mod x_lInt32Base
                    dwlAccum = (dwlAccum >> 32) + dwlNextAccum; // equiv to div BaseUI4 + dwlNAccum

                    // dwlNextAccum can't overflow next iteration
                    Debug.Assert(dwlAccum < x_ulInt32Base * 2, "can't overflow next iteration");
                }

                Debug.Assert(dwlAccum < x_ulInt32Base); // can never final accum > 1 more UI4
                if (dwlAccum != 0)
                    rgulRes[idRes++] = (uint)dwlAccum;
            }
            // Skip leading 0s (may exist if we are multiplying by 0)
            for (; (rgulRes[idRes] == 0) && (idRes > 0); idRes--)
                ;
            // Calculate actual result length
            culRes = idRes + 1;

            // III) Adjust precision,scale to result prec,scale
            if (lScaleAdjust != 0)
            {
                // If need to decrease scale
                if (lScaleAdjust < 0)
                {
                    Debug.Assert(NUMERIC_MAX_PRECISION > (resPrec + lScaleAdjust));

                    // have to adjust - might yet end up fitting.
                    // Cannot call AdjustScale - number cannot fit in a numeric, so
                    // have to duplicate code here

                    uint ulRem;          //Remainder when downshifting
                    uint ulShiftBase;    //What to multiply by to effect scale adjust

                    do
                    {
                        if (lScaleAdjust <= -9)
                        {
                            ulShiftBase = Powers10[8 + 1];
                            lScaleAdjust += 9;
                        }
                        else
                        {
                            ulShiftBase = Powers10[-lScaleAdjust];
                            lScaleAdjust = 0;
                        }
                        MpDiv1(rgulRes, ref culRes, ulShiftBase, out ulRem);
                    }
                    while (lScaleAdjust != 0);

                    // Still do not fit?
                    if (culRes > MaxDataLimbs)
                        throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");

                    for (idRes = culRes; idRes < rgulRes.Length; idRes++)
                        rgulRes[idRes] = 0;
                    ret = new NpgsqlDecimal(rgulRes, resScale, fResPositive);

                    // If remainder is 5 or above, increment/decrement by 1.
                    if (ulRem >= ulShiftBase / 2)
                        ret.MpAdd(1);
                    // After adjusting, if the result is 0 and remainder is less than 5,
                    // set the sign to be positive
                    if (ret.IsZero())
                        ret.Positive = true;

                    if ((ret.GetPrecision() - ret.Scale) > NUMERIC_MAX_PRECISION)
                        throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");

                    return ret;
                }

                // Otherwise call AdjustScale
                if (culRes > MaxDataLimbs)    // Do not fit now, so will not fit after asjustement
                    throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");
                // NOTE: Have not check for value in the range (10**38..2**128),
                // as we'll call AdjustScale with positive argument, and it'll
                // return "normal" overflow

                for (idRes = culRes; idRes < rgulRes.Length; idRes++)
                    rgulRes[idRes] = 0;
                ret = new NpgsqlDecimal(rgulRes, actualScale, fResPositive);

                if (ret.IsZero())
                    ret.Positive = true;


                ret.AdjustScale(lScaleAdjust, true);

                if ((ret.GetPrecision() - ret.Scale) > NUMERIC_MAX_PRECISION)
                    throw new OverflowException();

                return ret;
            }
            else
            {
                if (culRes > MaxDataLimbs)
                    throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");

                for (idRes = culRes; idRes < rgulRes.Length/*MaxDataLimbs*/; idRes++)
                    rgulRes[idRes] = 0;

                ret = new NpgsqlDecimal(rgulRes, resScale, fResPositive);

                if ((ret.GetPrecision() - ret.Scale) > NUMERIC_MAX_PRECISION)
                    throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");

                if (ret.IsZero())
                    ret.Positive = true;

                return ret;
            }
        }


        /// <summary>
        /// DivNm():
        ///   Divide numeric by numeric.
        ///     The Quotient will be returned in *this
        ///
        /// Result scale &amp; precision:
        ///     NOTE: 
        ///         scale = max(s1 + p2 + 1, x_cNumeDivScaleMin);
        ///         precision = max(s1 + p2 + 1, x_cNumeDivScaleMin) + p1 + p2 + 1;
        ///
        /// Overflow Rules:
        ///         If scale is greater than NUMERIC_MAX_SCALE it is set to NUMERIC_MAX_SCALE.
        ///         If # digits before decimal separator is greater than NUMERIC_MAX_PRECISION
        ///         OverflowException is throwed.
        ///         Resulting scale by default will be 6.       
        ///
        /// Algorithm
        ///   Call general purpose arbitrary precision division routine with scale = 0.
        ///     Scale,prec adjusted later.
        /// </summary>
        /// <param name="x">Operand to be divided.</param>
        /// <param name="y">Operand that is the divisor.</param>
        /// <returns>Quotient</returns>
        public static NpgsqlDecimal operator /(NpgsqlDecimal x, NpgsqlDecimal y)
        {

            // Variables for figuring prec,scale           
            int resScale;           // Final scale we will force quotient to
            int resPrec;            // Final precision we will force quotient to
            int resInteger;         // # of digits in integer part of result (prec-scale)
            int minScale;           // Temp to help compute ResScale
            int lScaleAdjust;       // How much result scale will be adjusted
            bool fResSignPos;       // sign of result

            // Steps:
            //    1) Figure result prec,scale; adjust scale of dividend
            //    2) Compute result remainder/quotient in 0 scale numbers
            //    3) Set result prec,scale and adjust as necessary

            // 0) Check for Div by 0
            if (y.IsZero())
                throw new DivideByZeroException("Divide by Zero");

            // 1) Figure out result prec,scale,sign..
            fResSignPos = (x.Positive == y.Positive);//sign of result

            //scale = max(s1 + p2 + 1, x_cNumeDivScaleMin);
            //precision = max(s1 + p2 + 1, x_cNumeDivScaleMin) + p1 + p2 + 1;
            //For backward compatibility, use exactly the same scheme as in Hydra
            var xPrec = (int)CalculatePrecision(x._data);
            var yPrec = (int)CalculatePrecision(y._data);
            resScale = Math.Max(x.Scale + yPrec + 1, x_cNumeDivScaleMin);
            resInteger = xPrec - x.Scale + y.Scale;

            minScale = Math.Min(resScale, x_cNumeDivScaleMin);

            resInteger = Math.Min(resInteger, NUMERIC_MAX_PRECISION + NUMERIC_MAX_SCALE);
            resPrec = resInteger + resScale;

            // If overflow, reduce the scale to avoid truncation of data
            resScale = Math.Min((resPrec - resInteger), resScale);
            resScale = Math.Max(resScale, minScale);
            resScale = Math.Min(resScale, NUMERIC_MAX_SCALE);

            //Adjust the scale of the dividend
            lScaleAdjust = resScale - (int)x.Scale + (int)y.Scale;
            x.AdjustScaleWithDifference(lScaleAdjust, true);

            // Step2: Actual Computation
            var rgulData1 = new uint[x._data.Length];
            var rgulData2 = new uint[y._data.Length];
            Array.Copy(x._data, rgulData1, x._data.Length);
            Array.Copy(y._data, rgulData2, y._data.Length);

            // Buffers for arbitrary precision divide
            var rgulR = new uint[x._data.Length + y._data.Length + 1];
            var rgulQ = new uint[x._data.Length + y._data.Length];

            // Divide mantissas. V is not zero - already checked.
            // Cannot overflow, as Q <= U, R <= V. (and both are positive)
            MpDiv(rgulData1, x._data.Length, rgulData2, y._data.Length, rgulQ, out var culQ /* # of ULONGs in result*/, rgulR, out var culR /* # of ULONGs in result*/);

            // Construct the result from Q
            ZeroToMaxLen(rgulQ, culQ);
            var ret = new NpgsqlDecimal(rgulQ, resScale, fResSignPos);

            //try to avoid this 2 blocks in the future (reuse the calculations made previously)
            var prec = ret.GetPrecision();
            if ((prec - ret.Scale) > NUMERIC_MAX_PRECISION)
                throw new OverflowException("Arithmetic overflow. Result requires more precision or scale than allowed by PostgreSQL numeric type.");

            if ((ret.Scale > NUMERIC_MAX_SCALE))
                ret = ret.AdjustScale(NUMERIC_MAX_SCALE, true);

            if (ret.IsZero())
                ret.Positive = true;

            return ret;
        }

        #endregion

        #region Comparison operators

        /// <summary>
        /// Overloading comparison operators
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <returns></returns>
        public static bool operator ==(NpgsqlDecimal x, NpgsqlDecimal y) => x.CompareNm(y) == 0 /*EComparison.EQ*/;

        /// <summary>
        /// Overloading operator for different !=.
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <returns></returns>
        public static bool operator !=(NpgsqlDecimal x, NpgsqlDecimal y) => !(x == y);

        /// <summary>
        /// Overloading operator for less than.
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <returns></returns>
        public static bool operator <(NpgsqlDecimal x, NpgsqlDecimal y) => x.CompareNm(y) < 0 /*EComparison.LT*/;

        /// <summary>
        /// Overloading operator for greater than.
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <returns></returns>
        public static bool operator >(NpgsqlDecimal x, NpgsqlDecimal y) => x.CompareNm(y) > 0 /*EComparison.GT*/;

        /// <summary>
        /// Overloading operator for less than or equal.
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <returns></returns>
        public static bool operator <=(NpgsqlDecimal x, NpgsqlDecimal y) => x.CompareNm(y) <= 0 /* EComparison.LT || result == EComparison.EQ*/;

        /// <summary>
        /// Overloading operator for greater than or equal.
        /// </summary>
        /// <param name="x"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <param name="y"><see cref="NpgsqlDecimal"/> operand to be compared with</param>
        /// <returns></returns>
        public static bool operator >=(NpgsqlDecimal x, NpgsqlDecimal y) => x.CompareNm(y) >= 0 /* EComparison.GT || result == EComparison.EQ*/;

        /// <summary>
        /// Compares the current NpgsqlDecimal with the given object.
        /// </summary>
        /// <param name="obj">The object to be compared with</param>
        /// <returns>
        /// If the given object is not of NpgsqlDecimal instance returns negative.
        /// If the given object is NpgsqlDecimal and lower than current returns negative.
        /// If the given object is NpgsqlDecimal and higher than current returns positive.
        /// If the given object is NpgsqlDecimal is equal returns 0.
        /// </returns>
        public int CompareTo(object? obj)
        {
            if (obj != null && obj is NpgsqlDecimal)
                return CompareNm((NpgsqlDecimal)obj);

            return -1;
        }

        /// <summary>
        /// Compares the current NpgsqlDecimal with the given object.
        /// </summary>
        /// <param name="obj">The NpgsqlDecimal to be compared with</param>
        /// <returns>
        /// If the given number is lower than current returns negative.
        /// If the given number is and higher than current returns positive.
        /// If the given number is is equal returns 0.
        /// </returns>
        public int CompareTo(NpgsqlDecimal obj) => CompareNm((NpgsqlDecimal)obj);

        private int CompareNm(NpgsqlDecimal snumOp)
        {
            //Signs of the two numeric operands
            int sign1;
            int sign2;

            int iFinalResult;   //Final result of comparision: positive = greater
                                //than, 0 = equal, negative = less than

            //Initialize the sign values to be 1(positive) or -1(negative)
            sign1 = Positive ? 1 : -1;
            sign2 = snumOp.Positive ? 1 : -1;

            if (sign1 != sign2) //If different sign, the positive one is greater
                return sign1 == 1 ? 1 /*EComparison.GT*/ : -1 /*EComparison.LT*/;

            else
            { //same sign, have to compare absolute values
                //Temporary memory to hold the operand since it is const
                //but its scale may get adjusted during comparison
                int scaleDiff;
                var snumArg1 = this;
                var snumArg2 = snumOp;

                //First make the two operands the same scale if necessary
                scaleDiff = ((int)Scale) - ((int)snumOp.Scale);

                if (scaleDiff < 0)
                {
                    //If increasing the scale of operand1 caused an overflow,
                    //then its absolute value is greater than that of operand2.
                    try
                    {
                        snumArg1.AdjustScaleWithDifference(-scaleDiff, true);
                    }
                    catch (OverflowException)
                    {
                        return (sign1 > 0) ? 1 /*EComparison.GT*/ : -1 /*EComparison.LT*/;
                    }
                }
                else if (scaleDiff > 0)
                {
                    //If increasing the scale of operand2 caused an overflow, then
                    //operand1's absolute value is less than that of operand2.
                    try
                    {

                        snumArg2.AdjustScaleWithDifference(scaleDiff, true);
                    }
                    catch (OverflowException)
                    {
                        return (sign1 > 0) ? -1 /*EComparison.LT*/ : 1 /*EComparison.GT*/;
                    }
                }

                //Compare the absolute value of the two numerics
                //Note: We are sure that scale of arguments is the same,
                //      so LAbsCmp() will not modify its argument.
                var lResult = snumArg1.LAbsCmp(snumArg2);
                if (0 == lResult)
                    return 0/*EComparison.EQ*/;

                //if both positive, result same as result from LAbsCmp;
                //if both negative, result reverse of result from LAbsCmp
                iFinalResult = sign1 * lResult;
                //iFinalResult < 0 => EComparison.LT
                //iFinalResult >= 0 => EComparison.GT

                return iFinalResult;
            }
        }


        #endregion
    }
}
