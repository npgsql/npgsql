using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Npgsql;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    [Serializable]
    public struct NpgsqlDate : IEquatable<NpgsqlDate>, IComparable<NpgsqlDate>, IComparable, IComparer<NpgsqlDate>,
                               IComparer
    {
        private static readonly int[] CommonYearDays = new int[] {0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};
        private static readonly int[] LeapYearDays = new int[] {0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};
        private static readonly int[] CommonYearMaxes = new int[] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
        private static readonly int[] LeapYearMaxes = new int[] {31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
        public const int MaxYear = 5874897;
        public const int MinYear = -4714;
        public static readonly NpgsqlDate MaxCalculableValue = new NpgsqlDate(MaxYear, 12, 31);
        public static readonly NpgsqlDate MinCalculableValue = new NpgsqlDate(MinYear, 11, 24);

        /// <summary>
        /// Represents the date 1970-01-01
        /// </summary>
        public static readonly NpgsqlDate Epoch = new NpgsqlDate(1970, 1, 1);

        /// <summary>
        /// Represents the date 0001-01-01
        /// </summary>
        public static readonly NpgsqlDate Era = new NpgsqlDate(0);

        public static NpgsqlDate Now
        {
            get { return new NpgsqlDate(DateTime.Now); }
        }

        public static NpgsqlDate Today
        {
            get { return Now; }
        }

        public static NpgsqlDate Yesterday
        {
            get { return Now.AddDays(-1); }
        }

        public static NpgsqlDate Tomorrow
        {
            get { return Now.AddDays(1); }
        }

        public static NpgsqlDate Parse(string str)
        {

            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            // Handle -infinity and infinity special values.

            if (str == "-infinity")
                return new NpgsqlDate(DateTime.MinValue);

            if (str == "infinity")
                return new NpgsqlDate(DateTime.MaxValue);

            str = str.Trim();
            try
            {
                int idx = str.IndexOf('-');
                if (idx == -1)
                {
                    throw new FormatException();
                }
                int year = int.Parse(str.Substring(0, idx));
                int idxLast = idx + 1;
                if ((idx = str.IndexOf('-', idxLast)) == -1)
                {
                    throw new FormatException();
                }
                int month = int.Parse(str.Substring(idxLast, idx - idxLast));
                idxLast = idx + 1;
                if ((idx = str.IndexOf(' ', idxLast)) == -1)
                {
                    idx = str.Length;
                }
                int day = int.Parse(str.Substring(idxLast, idx - idxLast));
                if (str.Contains("BC"))
                {
                    year = -year;
                }
                return new NpgsqlDate(year, month, day);
            }
            catch (OverflowException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new FormatException();
            }
        }

        public static bool TryParse(string str, out NpgsqlDate date)
        {
            try
            {
                date = Parse(str);
                return true;
            }
            catch
            {
                date = Era;
                return false;
            }
        }

        //Number of days since January 1st CE (January 1st EV). 1 Jan 1 CE = 0, 2 Jan 1 CE = 1, 31 Dec 1 BCE = -1, etc.
        private readonly int _daysSinceEra;

        public NpgsqlDate(int days)
        {
            _daysSinceEra = days;
        }

        public NpgsqlDate(DateTime dateTime)
            : this((int) (dateTime.Ticks/TimeSpan.TicksPerDay))
        {
        }

        public NpgsqlDate(NpgsqlDate copyFrom)
            : this(copyFrom._daysSinceEra)
        {
        }

        public NpgsqlDate(int year, int month, int day)
        {
            if (year == 0 || year < MinYear || year > MaxYear || month < 1 || month > 12 || day < 1 ||
                (day > (IsLeap(year) ? 366 : 365)))
            {
                throw new ArgumentOutOfRangeException();
            }

            _daysSinceEra = DaysForYears(year) + (IsLeap(year) ? LeapYearDays : CommonYearDays)[month - 1] + day - 1;
        }

        private const int DaysInYear = 365; //Common years
        private const int DaysIn4Years = 4*DaysInYear + 1; //Leap year every 4 years.
        private const int DaysInCentury = 25*DaysIn4Years - 1; //Except no leap year every 100.
        private const int DaysIn4Centuries = 4*DaysInCentury + 1; //Except leap year every 400.

        private static int DaysForYears(int years)
        {
            //Number of years after 1CE (0 for 1CE, -1 for 1BCE, 1 for 2CE).
            int calcYear = years < 1 ? years : years - 1;

            return calcYear/400*DaysIn4Centuries //Blocks of 400 years with their leap and common years
                   + calcYear%400/100*DaysInCentury //Remaining blocks of 100 years with their leap and common years
                   + calcYear%100/4*DaysIn4Years //Remaining blocks of 4 years with their leap and common years
                   + calcYear%4*DaysInYear //Remaining years, all common
                   + (calcYear < 0 ? -1 : 0); //And 1BCE is leap.
        }

        public int DayOfYear
        {
            get { return _daysSinceEra - DaysForYears(Year) + 1; }
        }

        public int Year
        {
            get
            {
                int guess = (int) Math.Round(_daysSinceEra/365.2425);
                int test = guess - 1;
                while (DaysForYears(++test) <= _daysSinceEra)
                {
                    ;
                }
                return test - 1;
            }
        }

        public int Month
        {
            get
            {
                int i = 1;
                int target = DayOfYear;
                int[] array = IsLeapYear ? LeapYearDays : CommonYearDays;
                while (target > array[i])
                {
                    ++i;
                }
                return i;
            }
        }

        public int Day
        {
            get { return DayOfYear - (IsLeapYear ? LeapYearDays : CommonYearDays)[Month - 1]; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return (DayOfWeek) ((_daysSinceEra + 1)%7); }
        }

        internal int DaysSinceEra
        {
            get { return _daysSinceEra; }
        }

        public bool IsLeapYear
        {
            get { return IsLeap(Year); }
        }

        private static bool IsLeap(int year)
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

        public NpgsqlDate AddDays(int days)
        {
            return new NpgsqlDate(_daysSinceEra + days);
        }

        public NpgsqlDate AddYears(int years)
        {
            int newYear = Year + years;
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

        public NpgsqlDate AddMonths(int months)
        {
            int newYear = Year;
            int newMonth = Month + months;

            while (newMonth > 12)
            {
                newMonth -= 12;
                newYear += 1;
            };
            while (newMonth < 1)
            {
                newMonth += 12;
                newYear -= 1;
            };
            int maxDay = (IsLeap(newYear) ? LeapYearMaxes : CommonYearMaxes)[newMonth - 1];
            int newDay = Day > maxDay ? maxDay : Day;
            return new NpgsqlDate(newYear, newMonth, newDay);

        }

        public NpgsqlDate Add(NpgsqlInterval interval)
        {
            return AddMonths(interval.Months).AddDays(interval.Days);
        }

        internal NpgsqlDate Add(NpgsqlInterval interval, int carriedOverflow)
        {
            return AddMonths(interval.Months).AddDays(interval.Days + carriedOverflow);
        }

        public int Compare(NpgsqlDate x, NpgsqlDate y)
        {
            return x.CompareTo(y);
        }

        public int Compare(object x, object y)
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
            return _daysSinceEra == other._daysSinceEra;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlDate && Equals((NpgsqlDate) obj);
        }

        public int CompareTo(NpgsqlDate other)
        {
            return _daysSinceEra.CompareTo(other._daysSinceEra);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (obj is NpgsqlDate)
            {
                return CompareTo((NpgsqlDate) obj);
            }
            throw new ArgumentException();
        }

        public override int GetHashCode()
        {
            return _daysSinceEra;
        }

        public override string ToString()
        {
            //Format of yyyy-MM-dd with " BC" for BCE and optional " AD" for CE which we omit here.
            return
                new StringBuilder(Math.Abs(Year).ToString("D4")).Append('-').Append(Month.ToString("D2")).Append('-').Append(
                    Day.ToString("D2")).Append(_daysSinceEra < 0 ? " BC" : "").ToString();
        }

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
            return x._daysSinceEra < y._daysSinceEra;
        }

        public static bool operator >(NpgsqlDate x, NpgsqlDate y)
        {
            return x._daysSinceEra > y._daysSinceEra;
        }

        public static bool operator <=(NpgsqlDate x, NpgsqlDate y)
        {
            return x._daysSinceEra <= y._daysSinceEra;
        }

        public static bool operator >=(NpgsqlDate x, NpgsqlDate y)
        {
            return x._daysSinceEra >= y._daysSinceEra;
        }

        public static explicit operator DateTime(NpgsqlDate date)
        {
            try
            {
                return new DateTime(date._daysSinceEra*NpgsqlInterval.TicksPerDay);
            }
            catch
            {
                throw new InvalidCastException();
            }
        }

        public static explicit operator NpgsqlDate(DateTime date)
        {
            return new NpgsqlDate((int) (date.Ticks/NpgsqlInterval.TicksPerDay));
        }

        public static NpgsqlDate operator +(NpgsqlDate date, NpgsqlInterval interval)
        {
            return date.Add(interval);
        }

        public static NpgsqlDate operator +(NpgsqlInterval interval, NpgsqlDate date)
        {
            return date.Add(interval);
        }

        public static NpgsqlDate operator -(NpgsqlDate date, NpgsqlInterval interval)
        {
            return date.Add(-interval);
        }

        public static NpgsqlInterval operator -(NpgsqlDate dateX, NpgsqlDate dateY)
        {
            return new NpgsqlInterval(0, dateX._daysSinceEra - dateY._daysSinceEra, 0);
        }
    }

}
