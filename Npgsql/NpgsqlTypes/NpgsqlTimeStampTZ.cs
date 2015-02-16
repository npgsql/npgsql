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
    public struct NpgsqlTimeStampTZ : IEquatable<NpgsqlTimeStampTZ>, IComparable<NpgsqlTimeStampTZ>, IComparable,
                                      IComparer<NpgsqlTimeStampTZ>, IComparer
    {
        private enum TimeType
        {
            Finite,
            Infinity,
            MinusInfinity
        }

        public static readonly NpgsqlTimeStampTZ Epoch = new NpgsqlTimeStampTZ(NpgsqlDate.Epoch, NpgsqlTimeTZ.AllBalls);
        public static readonly NpgsqlTimeStampTZ Era = new NpgsqlTimeStampTZ(NpgsqlDate.Era, NpgsqlTimeTZ.AllBalls);

        public static readonly NpgsqlTimeStampTZ Infinity =
            new NpgsqlTimeStampTZ(TimeType.Infinity, NpgsqlDate.Era, NpgsqlTimeTZ.AllBalls);

        public static readonly NpgsqlTimeStampTZ MinusInfinity =
            new NpgsqlTimeStampTZ(TimeType.MinusInfinity, NpgsqlDate.Era, NpgsqlTimeTZ.AllBalls);

        public static NpgsqlTimeStampTZ Now
        {
            get { return new NpgsqlTimeStampTZ(NpgsqlDate.Now, NpgsqlTimeTZ.Now); }
        }

        public static NpgsqlTimeStampTZ Today
        {
            get { return new NpgsqlTimeStampTZ(NpgsqlDate.Now); }
        }

        public static NpgsqlTimeStampTZ Yesterday
        {
            get { return new NpgsqlTimeStampTZ(NpgsqlDate.Yesterday); }
        }

        public static NpgsqlTimeStampTZ Tomorrow
        {
            get { return new NpgsqlTimeStampTZ(NpgsqlDate.Tomorrow); }
        }

        private readonly NpgsqlDate _date;
        private readonly NpgsqlTimeTZ _time;
        private readonly TimeType _type;

        private NpgsqlTimeStampTZ(TimeType type, NpgsqlDate date, NpgsqlTimeTZ time)
        {
            _type = type;
            _date = date;
            _time = time;
        }

        public NpgsqlTimeStampTZ(NpgsqlDate date, NpgsqlTimeTZ time)
            : this(TimeType.Finite, date, time)
        {
        }

        public NpgsqlTimeStampTZ(NpgsqlDate date)
            : this(date, NpgsqlTimeTZ.LocalMidnight(date))
        {
        }

        public NpgsqlTimeStampTZ(int year, int month, int day, int hours, int minutes, int seconds, NpgsqlTimeZone? timezone)
            : this(
                new NpgsqlDate(year, month, day),
                new NpgsqlTimeTZ(hours, minutes, seconds,
                                 timezone.HasValue ? timezone.Value : NpgsqlTimeZone.LocalTimeZone(new NpgsqlDate(year, month, day)))
                )
        {
        }

        public NpgsqlDate Date
        {
            get { return _date; }
        }

        public NpgsqlTimeTZ Time
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

        public NpgsqlTimeStampTZ AddDays(int days)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                return new NpgsqlTimeStampTZ(_date.AddDays(days), _time);
            }
        }

        public NpgsqlTimeStampTZ AddYears(int years)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                return new NpgsqlTimeStampTZ(_date.AddYears(years), _time);
            }
        }

        public NpgsqlTimeStampTZ AddMonths(int months)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                return new NpgsqlTimeStampTZ(_date.AddMonths(months), _time);
            }
        }

        public NpgsqlTime LocalTime
        {
            get { return _time.LocalTime; }
        }

        public NpgsqlTimeZone TimeZone
        {
            get { return _time.TimeZone; }
        }

        public NpgsqlTime UTCTime
        {
            get { return _time.UTCTime; }
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

        public NpgsqlTimeStampTZ Normalize()
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

        public static NpgsqlTimeStampTZ Parse(string str)
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
                    return new NpgsqlTimeStampTZ(NpgsqlDate.Parse(datePart), NpgsqlTimeTZ.Parse(timePart));
                } catch (OverflowException) {
                    throw;
                } catch {
                    throw new FormatException();
                }
            }
        }

        public bool Equals(NpgsqlTimeStampTZ other)
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
            return obj != null && obj is NpgsqlTimeStamp && Equals((NpgsqlTimeStampTZ)obj);
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

        public int CompareTo(NpgsqlTimeStampTZ other)
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

        public int Compare(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
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

        public NpgsqlTimeStamp AtTimeZone(NpgsqlTimeZone timeZone)
        {
            int overflow;
            NpgsqlTimeTZ adjusted = _time.AtTimeZone(timeZone, out overflow);
            return new NpgsqlTimeStamp(_date.AddDays(overflow), adjusted.LocalTime);
        }

        public NpgsqlTimeStampTZ Add(NpgsqlInterval interval)
        {
            switch (_type) {
            case TimeType.Infinity:
            case TimeType.MinusInfinity:
                return this;
            default:
                int overflow;
                NpgsqlTimeTZ time = _time.Add(interval, out overflow);
                return new NpgsqlTimeStampTZ(_date.Add(interval, overflow), time);
            }
        }

        public NpgsqlTimeStampTZ Subtract(NpgsqlInterval interval)
        {
            return Add(-interval);
        }

        public NpgsqlInterval Subtract(NpgsqlTimeStampTZ timestamp)
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
            return new NpgsqlInterval(0, _date.DaysSinceEra - timestamp._date.DaysSinceEra, (_time - timestamp._time).Ticks);
        }

        public static implicit operator NpgsqlTimeStampTZ(DateTime datetime)
        {
            if (datetime == DateTime.MaxValue) {
                return Infinity;
            } else if (datetime == DateTime.MinValue) {
                return MinusInfinity;
            } else {
                NpgsqlDate newDate = new NpgsqlDate(datetime);
                return
                    new NpgsqlTimeStampTZ(newDate,
                                          new NpgsqlTimeTZ(datetime.TimeOfDay,
                                                           datetime.Kind == DateTimeKind.Utc
                                                               ? NpgsqlTimeZone.UTC
                                                               : NpgsqlTimeZone.LocalTimeZone(newDate)));
            }
        }

        public static explicit operator DateTime(NpgsqlTimeStampTZ timestamp)
        {
            switch (timestamp._type) {
            case TimeType.Infinity:
                return DateTime.MaxValue;
            case TimeType.MinusInfinity:
                return DateTime.MinValue;
            default:
                try {
                    NpgsqlTimeStamp utc = timestamp.AtTimeZone(NpgsqlTimeZone.UTC);
                    return new DateTime(utc.Date.DaysSinceEra * NpgsqlInterval.TicksPerDay + utc.Time.Ticks, DateTimeKind.Utc);
                } catch {
                    throw new InvalidCastException();
                }
            }
        }

        public static implicit operator NpgsqlTimeStampTZ(DateTimeOffset datetimeoffset)
        {
            if (datetimeoffset == DateTimeOffset.MaxValue) {
                return Infinity;
            } else if (datetimeoffset == DateTimeOffset.MinValue) {
                return MinusInfinity;
            } else {
                NpgsqlDate newDate = new NpgsqlDate(datetimeoffset.Year,
                    datetimeoffset.Month, datetimeoffset.Day);
                return
                    new NpgsqlTimeStampTZ(newDate, new NpgsqlTimeTZ(datetimeoffset.TimeOfDay,
                        new NpgsqlTimeZone(datetimeoffset.Offset)));
            }
        }
        public static explicit operator DateTimeOffset(NpgsqlTimeStampTZ timestamp)
        {
            switch (timestamp._type) {
            case TimeType.Infinity:
                return DateTimeOffset.MaxValue;
            case TimeType.MinusInfinity:
                return DateTimeOffset.MinValue;
            default:
                try {
                    return new DateTimeOffset(timestamp.Date.DaysSinceEra * NpgsqlInterval.TicksPerDay + timestamp.Time.Ticks, timestamp.TimeZone);
                } catch {
                    throw new InvalidCastException();
                }
            }
        }

        public static NpgsqlTimeStampTZ operator +(NpgsqlTimeStampTZ timestamp, NpgsqlInterval interval)
        {
            return timestamp.Add(interval);
        }

        public static NpgsqlTimeStampTZ operator +(NpgsqlInterval interval, NpgsqlTimeStampTZ timestamp)
        {
            return timestamp.Add(interval);
        }

        public static NpgsqlTimeStampTZ operator -(NpgsqlTimeStampTZ timestamp, NpgsqlInterval interval)
        {
            return timestamp.Subtract(interval);
        }

        public static NpgsqlInterval operator -(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return x.Subtract(y);
        }

        public static bool operator ==(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return !(x == y);
        }

        public static bool operator <(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <=(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >=(NpgsqlTimeStampTZ x, NpgsqlTimeStampTZ y)
        {
            return x.CompareTo(y) >= 0;
        }
    }
}
