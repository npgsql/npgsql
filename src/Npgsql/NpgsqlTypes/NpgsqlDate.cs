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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public struct NpgsqlDate : IEquatable<NpgsqlDate>, IComparable<NpgsqlDate>, IComparable, IComparer<NpgsqlDate>,
                               IComparer
    {
        //Number of days since January 1st CE (January 1st EV). 1 Jan 1 CE = 0, 2 Jan 1 CE = 1, 31 Dec 1 BCE = -1, etc.
        readonly int _daysSinceEra;
        readonly InternalType _type;

        #region Constants

        static readonly int[] CommonYearDays = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        static readonly int[] LeapYearDays = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
        static readonly int[] CommonYearMaxes = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static readonly int[] LeapYearMaxes = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// Represents the date 1970-01-01
        /// </summary>
        public static readonly NpgsqlDate Epoch = new NpgsqlDate(1970, 1, 1);

        /// <summary>
        /// Represents the date 0001-01-01
        /// </summary>
        public static readonly NpgsqlDate Era = new NpgsqlDate(0);

        [PublicAPI]
        public const int MaxYear = 5874897;
        [PublicAPI]
        public const int MinYear = -4714;
        [PublicAPI]
        public static readonly NpgsqlDate MaxCalculableValue = new NpgsqlDate(MaxYear, 12, 31);
        [PublicAPI]
        public static readonly NpgsqlDate MinCalculableValue = new NpgsqlDate(MinYear, 11, 24);

        public static readonly NpgsqlDate Infinity = new NpgsqlDate(InternalType.Infinity);
        public static readonly NpgsqlDate NegativeInfinity = new NpgsqlDate(InternalType.NegativeInfinity);

        const int DaysInYear = 365; //Common years
        const int DaysIn4Years = 4 * DaysInYear + 1; //Leap year every 4 years.
        const int DaysInCentury = 25 * DaysIn4Years - 1; //Except no leap year every 100.
        const int DaysIn4Centuries = 4 * DaysInCentury + 1; //Except leap year every 400.

        #endregion

        #region Constructors

        NpgsqlDate(InternalType type)
        {
            _type = type;
            _daysSinceEra = 0;
        }

        internal NpgsqlDate(int days)
        {
            _type = InternalType.Finite;
            _daysSinceEra = days;
        }

        public NpgsqlDate(DateTime dateTime) : this((int) (dateTime.Ticks/TimeSpan.TicksPerDay)) {}

        public NpgsqlDate(NpgsqlDate copyFrom) : this(copyFrom._daysSinceEra) {}

        public NpgsqlDate(int year, int month, int day)
        {
            _type = InternalType.Finite;
            if (year == 0 || year < MinYear || year > MaxYear || month < 1 || month > 12 || day < 1 ||
                (day > (IsLeap(year) ? 366 : 365)))
            {
                throw new ArgumentOutOfRangeException();
            }

            _daysSinceEra = DaysForYears(year) + (IsLeap(year) ? LeapYearDays : CommonYearDays)[month - 1] + day - 1;
        }

        #endregion

        #region String Conversions

        public override string ToString()
        {
            switch (_type)
            {
            case InternalType.Infinity:
                return "infinity";
            case InternalType.NegativeInfinity:
                return "-infinity";
            default:
                //Format of yyyy-MM-dd with " BC" for BCE and optional " AD" for CE which we omit here.
                return
                    new StringBuilder(Math.Abs(Year).ToString("D4")).Append('-').Append(Month.ToString("D2")).Append('-').Append(
                        Day.ToString("D2")).Append(_daysSinceEra < 0 ? " BC" : "").ToString();
            }
        }

        public static NpgsqlDate Parse(string str)
        {

            if (str == null) {
                throw new ArgumentNullException(nameof(str));
            }

            if (str == "infinity")
                return Infinity;

            if (str == "-infinity")
                return NegativeInfinity;

            str = str.Trim();
            try {
                var idx = str.IndexOf('-');
                if (idx == -1) {
                    throw new FormatException();
                }
                var year = int.Parse(str.Substring(0, idx));
                var idxLast = idx + 1;
                if ((idx = str.IndexOf('-', idxLast)) == -1) {
                    throw new FormatException();
                }
                var month = int.Parse(str.Substring(idxLast, idx - idxLast));
                idxLast = idx + 1;
                if ((idx = str.IndexOf(' ', idxLast)) == -1) {
                    idx = str.Length;
                }
                var day = int.Parse(str.Substring(idxLast, idx - idxLast));
                if (str.Contains("BC")) {
                    year = -year;
                }
                return new NpgsqlDate(year, month, day);
            } catch (OverflowException) {
                throw;
            } catch (Exception) {
                throw new FormatException();
            }
        }

        [PublicAPI]
        public static bool TryParse(string str, out NpgsqlDate date)
        {
            try {
                date = Parse(str);
                return true;
            } catch {
                date = Era;
                return false;
            }
        }

        #endregion

        #region Public Properties

        [PublicAPI] public static NpgsqlDate Now => new NpgsqlDate(DateTime.Now);
        [PublicAPI] public static NpgsqlDate Today => Now;
        [PublicAPI] public static NpgsqlDate Yesterday => Now.AddDays(-1);
        [PublicAPI] public static NpgsqlDate Tomorrow => Now.AddDays(1);

        [PublicAPI] public int DayOfYear => _daysSinceEra - DaysForYears(Year) + 1;

        [PublicAPI] public int Year
        {
            get
            {
                var guess = (int)Math.Round(_daysSinceEra/365.2425);
                var test = guess - 1;
                while (DaysForYears(++test) <= _daysSinceEra) {}
                return test - 1;
            }
        }

        [PublicAPI] public int Month
        {
            get
            {
                var i = 1;
                var target = DayOfYear;
                var array = IsLeapYear ? LeapYearDays : CommonYearDays;
                while (target > array[i])
                {
                    ++i;
                }
                return i;
            }
        }

        [PublicAPI] public int Day => DayOfYear - (IsLeapYear ? LeapYearDays : CommonYearDays)[Month - 1];

        [PublicAPI] public DayOfWeek DayOfWeek => (DayOfWeek) ((_daysSinceEra + 1)%7);

        internal int DaysSinceEra => _daysSinceEra;

        [PublicAPI] public bool IsLeapYear => IsLeap(Year);

        [PublicAPI] public bool IsInfinity => _type == InternalType.Infinity;
        [PublicAPI] public bool IsNegativeInfinity => _type == InternalType.NegativeInfinity;

        [PublicAPI] public bool IsFinite
        {
            get
            {
                switch (_type) {
                case InternalType.Finite:
                    return true;
                case InternalType.Infinity:
                case InternalType.NegativeInfinity:
                    return false;
                default:
                    throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
                }
            }
        }

        #endregion

        #region Internals

        static int DaysForYears(int years)
        {
            //Number of years after 1CE (0 for 1CE, -1 for 1BCE, 1 for 2CE).
            var calcYear = years < 1 ? years : years - 1;

            return calcYear / 400 * DaysIn4Centuries //Blocks of 400 years with their leap and common years
                   + calcYear % 400 / 100 * DaysInCentury //Remaining blocks of 100 years with their leap and common years
                   + calcYear % 100 / 4 * DaysIn4Years //Remaining blocks of 4 years with their leap and common years
                   + calcYear % 4 * DaysInYear //Remaining years, all common
                   + (calcYear < 0 ? -1 : 0); //And 1BCE is leap.
        }

        static bool IsLeap(int year)
        {
            //Every 4 years is a leap year
            //Except every 100 years isn't a leap year.
            //Except every 400 years is.
            if (year < 1)
            {
                year = year + 1;
            }
            return (year%4 == 0) && ((year%100 != 0) || (year%400 == 0));
        }

        #endregion

        #region Arithmetic

        [PublicAPI]
        public NpgsqlDate AddDays(int days)
        {
            switch (_type)
            {
            case InternalType.Infinity:
                return Infinity;
            case InternalType.NegativeInfinity:
                return NegativeInfinity;
            case InternalType.Finite:
                return new NpgsqlDate(_daysSinceEra + days);
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
            }
        }

        [PublicAPI]
        public NpgsqlDate AddYears(int years)
        {
            switch (_type) {
            case InternalType.Infinity:
                return Infinity;
            case InternalType.NegativeInfinity:
                return NegativeInfinity;
            case InternalType.Finite:
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
            }

            var newYear = Year + years;
            if (newYear >= 0 && _daysSinceEra < 0) //cross 1CE/1BCE divide going up
            {
                ++newYear;
            }
            else if (newYear <= 0 && _daysSinceEra >= 0) //cross 1CE/1BCE divide going down
            {
                --newYear;
            }
            return new NpgsqlDate(newYear, Month, Day);
        }

        [PublicAPI]
        public NpgsqlDate AddMonths(int months)
        {
            switch (_type) {
            case InternalType.Infinity:
                return Infinity;
            case InternalType.NegativeInfinity:
                return NegativeInfinity;
            case InternalType.Finite:
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
            }

            var newYear = Year;
            var newMonth = Month + months;

            while (newMonth > 12)
            {
                newMonth -= 12;
                newYear += 1;
            }
            while (newMonth < 1)
            {
                newMonth += 12;
                newYear -= 1;
            }
            var maxDay = (IsLeap(newYear) ? LeapYearMaxes : CommonYearMaxes)[newMonth - 1];
            var newDay = Day > maxDay ? maxDay : Day;
            return new NpgsqlDate(newYear, newMonth, newDay);

        }

        [PublicAPI]
        public NpgsqlDate Add(NpgsqlTimeSpan interval)
        {
            switch (_type) {
            case InternalType.Infinity:
                return Infinity;
            case InternalType.NegativeInfinity:
                return NegativeInfinity;
            case InternalType.Finite:
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
            }

            return AddMonths(interval.Months).AddDays(interval.Days);
        }

        [PublicAPI]
        internal NpgsqlDate Add(NpgsqlTimeSpan interval, int carriedOverflow)
        {
            switch (_type) {
            case InternalType.Infinity:
                return Infinity;
            case InternalType.NegativeInfinity:
                return NegativeInfinity;
            case InternalType.Finite:
                break;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
            }

            return AddMonths(interval.Months).AddDays(interval.Days + carriedOverflow);
        }

        #endregion

        #region Comparison

        public int Compare(NpgsqlDate x, NpgsqlDate y)
        {
            return x.CompareTo(y);
        }

        public int Compare([CanBeNull] object x, [CanBeNull] object y)
        {
            if (x == null)
            {
                return y == null ? 0 : -1;
            }
            if (y == null)
            {
                return 1;
            }
            if (!(x is IComparable) || !(y is IComparable))
            {
                throw new ArgumentException();
            }
            return ((IComparable) x).CompareTo(y);
        }

        public bool Equals(NpgsqlDate other)
        {
            switch (_type) {
            case InternalType.Infinity:
                return other._type == InternalType.Infinity;
            case InternalType.NegativeInfinity:
                return other._type == InternalType.NegativeInfinity;
            case InternalType.Finite:
                return other._type == InternalType.Finite && _daysSinceEra == other._daysSinceEra;
            default:
                return false;
            }
        }

        public override bool Equals([CanBeNull] object obj)
        {
            return obj is NpgsqlDate && Equals((NpgsqlDate) obj);
        }

        public int CompareTo(NpgsqlDate other)
        {
            switch (_type) {
            case InternalType.Infinity:
                return other._type == InternalType.Infinity ? 0 : 1;
            case InternalType.NegativeInfinity:
                return other._type == InternalType.NegativeInfinity ? 0 : -1;
            default:
                switch (other._type) {
                case InternalType.Infinity:
                    return -1;
                case InternalType.NegativeInfinity:
                    return 1;
                default:
                    return _daysSinceEra.CompareTo(other._daysSinceEra);
                }
            }
        }

        public int CompareTo([CanBeNull] object o)
        {
            if (o == null)
            {
                return 1;
            }
            if (o is NpgsqlDate)
            {
                return CompareTo((NpgsqlDate) o);
            }
            throw new ArgumentException();
        }

        public override int GetHashCode()
        {
            return _daysSinceEra;
        }

        #endregion

        #region Operators

        public static bool operator ==(NpgsqlDate x, NpgsqlDate y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlDate x, NpgsqlDate y)
        {
            return !(x == y);
        }

        public static bool operator <(NpgsqlDate x, NpgsqlDate y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(NpgsqlDate x, NpgsqlDate y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <=(NpgsqlDate x, NpgsqlDate y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >=(NpgsqlDate x, NpgsqlDate y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static DateTime ToDateTime(NpgsqlDate date)
        {
            switch (date._type)
            {
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                throw new InvalidCastException("Infinity values can't be cast to DateTime");
            case InternalType.Finite:
                try { return new DateTime(date._daysSinceEra * NpgsqlTimeSpan.TicksPerDay); }
                catch { throw new InvalidCastException(); }
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {date._type} of enum {nameof(NpgsqlDate)}.{nameof(InternalType)}. Please file a bug.");
            }
        }

        public static explicit operator DateTime(NpgsqlDate date) => ToDateTime(date);

        public static NpgsqlDate ToNpgsqlDate(DateTime date)
            => new NpgsqlDate((int)(date.Ticks / NpgsqlTimeSpan.TicksPerDay));

        public static explicit operator NpgsqlDate(DateTime date) => ToNpgsqlDate(date);

        public static NpgsqlDate operator +(NpgsqlDate date, NpgsqlTimeSpan interval)
            => date.Add(interval);

        public static NpgsqlDate operator +(NpgsqlTimeSpan interval, NpgsqlDate date)
            => date.Add(interval);

        public static NpgsqlDate operator -(NpgsqlDate date, NpgsqlTimeSpan interval)
            => date.Subtract(interval);

        public NpgsqlDate Subtract(NpgsqlTimeSpan interval) => Add(-interval);

        public static NpgsqlTimeSpan operator -(NpgsqlDate dateX, NpgsqlDate dateY)
        {
            if (dateX._type != InternalType.Finite || dateY._type != InternalType.Finite)
                throw new ArgumentException("Can't subtract infinity date values");

            return new NpgsqlTimeSpan(0, dateX._daysSinceEra - dateY._daysSinceEra, 0);
        }

        #endregion

        enum InternalType
        {
            Finite,
            Infinity,
            NegativeInfinity
        }
    }
}
