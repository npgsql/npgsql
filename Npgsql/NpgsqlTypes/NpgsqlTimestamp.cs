using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    [Serializable]
    public struct NpgsqlTimeStamp : IEquatable<NpgsqlTimeStamp>, IComparable<NpgsqlTimeStamp>, IComparable,
                                    IComparer<NpgsqlTimeStamp>, IComparer
    {
        private enum TimeType
        {
            Finite,
            Infinity,
            MinusInfinity
        }

        public static readonly NpgsqlTimeStamp Epoch = new NpgsqlTimeStamp(NpgsqlDate.Epoch);
        public static readonly NpgsqlTimeStamp Era = new NpgsqlTimeStamp(NpgsqlDate.Era);

        public static readonly NpgsqlTimeStamp Infinity =
            new NpgsqlTimeStamp(TimeType.Infinity, NpgsqlDate.Era, NpgsqlTime.AllBalls);

        public static readonly NpgsqlTimeStamp MinusInfinity =
            new NpgsqlTimeStamp(TimeType.MinusInfinity, NpgsqlDate.Era, NpgsqlTime.AllBalls);

        public static NpgsqlTimeStamp Now
        {
            get { return new NpgsqlTimeStamp(NpgsqlDate.Now, NpgsqlTime.Now); }
        }

        public static NpgsqlTimeStamp Today
        {
            get { return new NpgsqlTimeStamp(NpgsqlDate.Now); }
        }

        public static NpgsqlTimeStamp Yesterday
        {
            get { return new NpgsqlTimeStamp(NpgsqlDate.Yesterday); }
        }

        public static NpgsqlTimeStamp Tomorrow
        {
            get { return new NpgsqlTimeStamp(NpgsqlDate.Tomorrow); }
        }

        private readonly NpgsqlDate _date;
        private readonly NpgsqlTime _time;
        private readonly TimeType _type;

        private NpgsqlTimeStamp(TimeType type, NpgsqlDate date, NpgsqlTime time)
        {
            _type = type;
            _date = date;
            _time = time;
        }

        public NpgsqlTimeStamp(NpgsqlDate date, NpgsqlTime time)
            : this(TimeType.Finite, date, time)
        {
        }

        public NpgsqlTimeStamp(NpgsqlDate date)
            : this(date, NpgsqlTime.AllBalls)
        {
        }

        public NpgsqlTimeStamp(int year, int month, int day, int hours, int minutes, int seconds)
            : this(new NpgsqlDate(year, month, day), new NpgsqlTime(hours, minutes, seconds))
        {
        }

        public NpgsqlDate Date
        {
            get { return _date; }
        }

        public NpgsqlTime Time
        {
            get { return _time; }
        }

        public int DayOfYear
        {
            get { return _date.DayOfYear; }
        }

        public int Year
        {
            get { return _date.Year; }
        }

        public int Month
        {
            get { return _date.Month; }
        }

        public int Day
        {
            get { return _date.Day; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return _date.DayOfWeek; }
        }

        public bool IsLeapYear
        {
            get { return _date.IsLeapYear; }
        }

        public NpgsqlTimeStamp AddDays(int days)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                return new NpgsqlTimeStamp(_date.AddDays(days), _time);
            }
        }

        public NpgsqlTimeStamp AddYears(int years)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                return new NpgsqlTimeStamp(_date.AddYears(years), _time);
            }
        }

        public NpgsqlTimeStamp AddMonths(int months)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                return new NpgsqlTimeStamp(_date.AddMonths(months), _time);
            }
        }

        public long Ticks
        {
            get { return _date.DaysSinceEra * NpgsqlInterval.TicksPerDay + _time.Ticks; }
        }

        public int Microseconds
        {
            get { return _time.Microseconds; }
        }

        public int Milliseconds
        {
            get { return _time.Milliseconds; }
        }

        public int Seconds
        {
            get { return _time.Seconds; }
        }

        public int Minutes
        {
            get { return _time.Minutes; }
        }

        public int Hours
        {
            get { return _time.Hours; }
        }

        public bool IsFinite
        {
            get { return _type == TimeType.Finite; }
        }

        public bool IsInfinity
        {
            get { return _type == TimeType.Infinity; }
        }

        public bool IsMinusInfinity
        {
            get { return _type == TimeType.MinusInfinity; }
        }

        public NpgsqlTimeStamp Normalize()
        {
            return Add(NpgsqlInterval.Zero);
        }

        public override string ToString()
        {
            switch (_type) {
            case TimeType.Infinity:
                return "infinity";
            case TimeType.MinusInfinity:
                return "-infinity";
            default:
                return string.Format("{0} {1}", _date, _time);
            }
        }

        public static NpgsqlTimeStamp Parse(string str)
        {
            if (str == null) {
                throw new NullReferenceException();
            }
            switch (str = str.Trim().ToLowerInvariant()) {
            case "infinity":
                return Infinity;
            case "-infinity":
                return MinusInfinity;
            default:
                try {
                    int idxSpace = str.IndexOf(' ');
                    string datePart = str.Substring(0, idxSpace);
                    if (str.Contains("bc")) {
                        datePart += " BC";
                    }
                    int idxSecond = str.IndexOf(' ', idxSpace + 1);
                    if (idxSecond == -1) {
                        idxSecond = str.Length;
                    }
                    string timePart = str.Substring(idxSpace + 1, idxSecond - idxSpace - 1);
                    return new NpgsqlTimeStamp(NpgsqlDate.Parse(datePart), NpgsqlTime.Parse(timePart));
                } catch (OverflowException) {
                    throw;
                } catch {
                    throw new FormatException();
                }
            }
        }

        /// <summary>
        /// Creates a NpgsqlTimeStamp from backend binary format
        /// </summary>
        /// <param name="value">Number of microseconds since 2000-01-01 00:00:00</param>
        internal static NpgsqlTimeStamp FromInt64(long value)
        {
            if (value == long.MaxValue)
                return Infinity;
            if (value == long.MinValue)
                return MinusInfinity;
            if (value >= 0) {
                int date = (int)(value / 86400000000L);
                long time = value % 86400000000L;

                date += 730119; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlTimeStamp(new NpgsqlDate(date), new NpgsqlTime(time));
            } else {
                value = -value;
                int date = (int)(value / 86400000000L);
                long time = value % 86400000000L;
                if (time != 0) {
                    ++date;
                    time = 86400000000L - time;
                }
                date = 730119 - date; // 730119 = days since era (0001-01-01) for 2000-01-01
                time *= 10; // To 100ns

                return new NpgsqlTimeStamp(new NpgsqlDate(date), new NpgsqlTime(time));
            }
        }

        public bool Equals(NpgsqlTimeStamp other)
        {
            switch (_type) {
            case TimeType.Infinity:
                return other._type == TimeType.Infinity;
            case TimeType.MinusInfinity:
                return other._type == TimeType.MinusInfinity;
            default:
                return other._type == TimeType.Finite && _date.Equals(other._date) && _time.Equals(other._time);
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlTimeStamp && Equals((NpgsqlTimeStamp)obj);
        }

        public override int GetHashCode()
        {
            switch (_type) {
            case TimeType.Infinity:
                return int.MaxValue;
            case TimeType.MinusInfinity:
                return int.MinValue;
            default:
                return _date.GetHashCode() ^ PGUtil.RotateShift(_time.GetHashCode(), 16);
            }
        }

        public int CompareTo(NpgsqlTimeStamp other)
        {
            switch (_type) {
            case TimeType.Infinity:
                return other._type == TimeType.Infinity ? 0 : 1;
            case TimeType.MinusInfinity:
                return other._type == TimeType.MinusInfinity ? 0 : -1;
            default:
                switch (other._type) {
                case TimeType.Infinity:
                    return -1;
                case TimeType.MinusInfinity:
                    return 1;
                default:
                    int cmp = _date.CompareTo(other._date);
                    return cmp == 0 ? _time.CompareTo(_time) : cmp;
                }
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) {
                return 1;
            }
            if (obj is NpgsqlTimeStamp) {
                return CompareTo((NpgsqlTimeStamp)obj);
            }
            throw new ArgumentException();
        }

        public int Compare(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.CompareTo(y);
        }

        public int Compare(object x, object y)
        {
            if (x == null) {
                return y == null ? 0 : -1;
            }
            if (y == null) {
                return 1;
            }
            if (!(x is IComparable) || !(y is IComparable)) {
                throw new ArgumentException();
            }
            return ((IComparable)x).CompareTo(y);
        }

        public NpgsqlTimeStampTZ AtTimeZone(NpgsqlTimeZone timeZoneFrom, NpgsqlTimeZone timeZoneTo)
        {
            int overflow;
            NpgsqlTimeTZ adjusted = new NpgsqlTimeTZ(_time, timeZoneFrom).AtTimeZone(timeZoneTo, out overflow);
            return new NpgsqlTimeStampTZ(_date.AddDays(overflow), adjusted);
        }

        public NpgsqlTimeStampTZ AtTimeZone(NpgsqlTimeZone timeZone)
        {
            return AtTimeZone(timeZone, NpgsqlTimeZone.LocalTimeZone(_date));
        }

        public NpgsqlTimeStamp Add(NpgsqlInterval interval)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                int overflow;
                NpgsqlTime time = _time.Add(interval, out overflow);
                return new NpgsqlTimeStamp(_date.Add(interval, overflow), time);
            }
        }

        public NpgsqlTimeStamp Subtract(NpgsqlInterval interval)
        {
            return Add(-interval);
        }

        public NpgsqlInterval Subtract(NpgsqlTimeStamp timestamp)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                throw new ArgumentOutOfRangeException("this", "You cannot subtract infinity timestamps");
            }
            switch (timestamp._type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                throw new ArgumentOutOfRangeException("timestamp", "You cannot subtract infinity timestamps");
            }
            return new NpgsqlInterval(0, _date.DaysSinceEra - timestamp._date.DaysSinceEra, _time.Ticks - timestamp._time.Ticks);
        }

        public static implicit operator NpgsqlTimeStamp(DateTime datetime)
        {
            if (datetime == DateTime.MaxValue) {
                return Infinity;
            } else if (datetime == DateTime.MinValue) {
                return MinusInfinity;
            } else {
                return new NpgsqlTimeStamp(new NpgsqlDate(datetime), new NpgsqlTime(datetime.TimeOfDay));
            }
        }

        public static implicit operator DateTime(NpgsqlTimeStamp timestamp)
        {
            switch (timestamp._type) {
            case TimeType.Infinity:
                return DateTime.MaxValue;
            case TimeType.MinusInfinity:
                return DateTime.MinValue;
            default:
                try {
                    return
                        new DateTime(timestamp.Date.DaysSinceEra * NpgsqlInterval.TicksPerDay + timestamp._time.Ticks,
                                     DateTimeKind.Unspecified);
                } catch {
                    throw new InvalidCastException();
                }
            }
        }

        public static NpgsqlTimeStamp operator +(NpgsqlTimeStamp timestamp, NpgsqlInterval interval)
        {
            return timestamp.Add(interval);
        }

        public static NpgsqlTimeStamp operator +(NpgsqlInterval interval, NpgsqlTimeStamp timestamp)
        {
            return timestamp.Add(interval);
        }

        public static NpgsqlTimeStamp operator -(NpgsqlTimeStamp timestamp, NpgsqlInterval interval)
        {
            return timestamp.Subtract(interval);
        }

        public static NpgsqlInterval operator -(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.Subtract(y);
        }

        public static bool operator ==(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return !(x == y);
        }

        public static bool operator <(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <=(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >=(NpgsqlTimeStamp x, NpgsqlTimeStamp y)
        {
            return x.CompareTo(y) >= 0;
        }
    }
}
