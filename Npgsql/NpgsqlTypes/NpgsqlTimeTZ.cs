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
    public struct NpgsqlTimeTZ : IEquatable<NpgsqlTimeTZ>, IComparable<NpgsqlTimeTZ>, IComparable, IComparer<NpgsqlTimeTZ>,
                                 IComparer
    {
        public static readonly NpgsqlTimeTZ AllBalls = new NpgsqlTimeTZ(NpgsqlTime.AllBalls, NpgsqlTimeZone.UTC);

        public static NpgsqlTimeTZ Now
        {
            get { return new NpgsqlTimeTZ(NpgsqlTime.Now); }
        }

        public static NpgsqlTimeTZ LocalMidnight(NpgsqlDate date)
        {
            return new NpgsqlTimeTZ(NpgsqlTime.AllBalls, NpgsqlTimeZone.LocalTimeZone(date));
        }

        private readonly NpgsqlTime _localTime;
        private readonly NpgsqlTimeZone _timeZone;

        public NpgsqlTimeTZ(NpgsqlTime localTime, NpgsqlTimeZone timeZone)
        {
            _localTime = localTime;
            _timeZone = timeZone;
        }

        public NpgsqlTimeTZ(NpgsqlTime localTime)
            : this(localTime, NpgsqlTimeZone.CurrentTimeZone)
        {
        }

        public NpgsqlTimeTZ(long ticks)
            : this(new NpgsqlTime(ticks))
        {
        }

        public NpgsqlTimeTZ(TimeSpan time)
            : this(new NpgsqlTime(time))
        {
        }

        public NpgsqlTimeTZ(NpgsqlInterval time)
            : this(new NpgsqlTime(time))
        {
        }

        public NpgsqlTimeTZ(NpgsqlTimeTZ copyFrom)
            : this(copyFrom._localTime, copyFrom._timeZone)
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, int seconds)
            : this(new NpgsqlTime(hours, minutes, seconds))
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, int seconds, int microseconds)
            : this(new NpgsqlTime(hours, minutes, seconds, microseconds))
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, decimal seconds)
            : this(new NpgsqlTime(hours, minutes, seconds))
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, double seconds)
            : this(new NpgsqlTime(hours, minutes, seconds))
        {
        }

        public NpgsqlTimeTZ(long ticks, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(ticks), timeZone)
        {
        }

        public NpgsqlTimeTZ(TimeSpan time, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(time), timeZone)
        {
        }

        public NpgsqlTimeTZ(NpgsqlInterval time, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(time), timeZone)
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, int seconds, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(hours, minutes, seconds), timeZone)
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, int seconds, int microseconds, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(hours, minutes, seconds, microseconds), timeZone)
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, decimal seconds, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(hours, minutes, seconds), timeZone)
        {
        }

        public NpgsqlTimeTZ(int hours, int minutes, double seconds, NpgsqlTimeZone timeZone)
            : this(new NpgsqlTime(hours, minutes, seconds), timeZone)
        {
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", _localTime, _timeZone);
        }

        public static NpgsqlTimeTZ Parse(string str)
        {
            if (str == null) {
                throw new ArgumentNullException();
            }
            try {
                int idx = Math.Max(str.IndexOf('+'), str.IndexOf('-'));
                if (idx == -1) {
                    throw new FormatException();
                }
                return new NpgsqlTimeTZ(NpgsqlTime.Parse(str.Substring(0, idx)), NpgsqlTimeZone.Parse(str.Substring(idx)));
            } catch (OverflowException) {
                throw;
            } catch {
                throw new FormatException();
            }
        }

        public NpgsqlTime LocalTime
        {
            get { return _localTime; }
        }

        public NpgsqlTimeZone TimeZone
        {
            get { return _timeZone; }
        }

        public NpgsqlTime UTCTime
        {
            get { return AtTimeZone(NpgsqlTimeZone.UTC).LocalTime; }
        }

        public NpgsqlTimeTZ AtTimeZone(NpgsqlTimeZone timeZone)
        {
            return new NpgsqlTimeTZ(LocalTime - _timeZone + timeZone, timeZone);
        }

        internal NpgsqlTimeTZ AtTimeZone(NpgsqlTimeZone timeZone, out int overflow)
        {
            return
                new NpgsqlTimeTZ(LocalTime.Add(timeZone - (NpgsqlInterval)(_timeZone), out overflow), timeZone);
        }

        public long Ticks
        {
            get { return _localTime.Ticks; }
        }

        /// <summary>
        /// Gets the number of whole microseconds held in the instance.
        /// <returns>An integer in the range [0, 999999].</returns>
        /// </summary>
        public int Microseconds
        {
            get { return _localTime.Microseconds; }
        }

        /// <summary>
        /// Gets the number of whole milliseconds held in the instance.
        /// <returns>An integer in the range [0, 999].</returns>
        /// </summary>
        public int Milliseconds
        {
            get { return _localTime.Milliseconds; }
        }

        /// <summary>
        /// Gets the number of whole seconds held in the instance.
        /// <returns>An interger in the range [0, 59].</returns>
        /// </summary>
        public int Seconds
        {
            get { return _localTime.Seconds; }
        }

        /// <summary>
        /// Gets the number of whole minutes held in the instance.
        /// <returns>An integer in the range [0, 59].</returns>
        /// </summary>
        public int Minutes
        {
            get { return _localTime.Minutes; }
        }

        /// <summary>
        /// Gets the number of whole hours held in the instance.
        /// <remarks>Note that the time 24:00:00 can be stored for roundtrip compatibility. Any calculations on such a
        /// value will normalised it to 00:00:00.</remarks>
        /// </summary>
        public int Hours
        {
            get { return _localTime.Hours; }
        }

        /// <summary>
        /// Normalise this time; if it is 24:00:00, convert it to 00:00:00
        /// </summary>
        /// <returns>This time, normalised</returns>
        public NpgsqlTimeTZ Normalize()
        {
            return new NpgsqlTimeTZ(_localTime.Normalize(), _timeZone);
        }

        public bool Equals(NpgsqlTimeTZ other)
        {
            return _localTime.Equals(other._localTime) && _timeZone.Equals(other._timeZone);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlTimeTZ && Equals((NpgsqlTimeTZ)obj);
        }

        public override int GetHashCode()
        {
            return _localTime.GetHashCode() ^ PGUtil.RotateShift(_timeZone.GetHashCode(), 24);
        }

        /// <summary>
        /// Compares this with another <see cref="NpgsqlTimeTZ"/>. As per postgres' rules,
        /// first the times are compared as if they were both in the same timezone. If they are equal then
        /// then timezones are compared (+01:00 being "smaller" than -01:00).
        /// </summary>
        /// <param name="other">the <see cref="NpgsqlTimeTZ"/> to compare with.</param>
        /// <returns>An integer which is 0 if they are equal, &lt; 0 if this is the smaller and &gt; 0 if this is the larger.</returns>
        public int CompareTo(NpgsqlTimeTZ other)
        {
            int cmp = AtTimeZone(NpgsqlTimeZone.UTC).LocalTime.CompareTo(other.AtTimeZone(NpgsqlTimeZone.UTC).LocalTime);
            return cmp == 0 ? _timeZone.CompareTo(other._timeZone) : cmp;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) {
                return 1;
            }
            if (obj is NpgsqlTimeTZ) {
                return CompareTo((NpgsqlTimeTZ)obj);
            }
            throw new ArgumentException();
        }

        public int Compare(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
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

        public static bool operator ==(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
        {
            return !(x == y);
        }

        public static bool operator <(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <=(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >=(NpgsqlTimeTZ x, NpgsqlTimeTZ y)
        {
            return x.CompareTo(y) >= 0;
        }

        public NpgsqlTimeTZ Add(NpgsqlInterval interval)
        {
            return new NpgsqlTimeTZ(_localTime.Add(interval), _timeZone);
        }

        internal NpgsqlTimeTZ Add(NpgsqlInterval interval, out int overflow)
        {
            return new NpgsqlTimeTZ(_localTime.Add(interval, out overflow), _timeZone);
        }

        public NpgsqlTimeTZ Subtract(NpgsqlInterval interval)
        {
            return new NpgsqlTimeTZ(_localTime.Subtract(interval), _timeZone);
        }

        public NpgsqlInterval Subtract(NpgsqlTimeTZ earlier)
        {
            return _localTime.Subtract(earlier.AtTimeZone(_timeZone)._localTime);
        }

        public static NpgsqlTimeTZ operator +(NpgsqlTimeTZ time, NpgsqlInterval interval)
        {
            return time.Add(interval);
        }

        public static NpgsqlTimeTZ operator +(NpgsqlInterval interval, NpgsqlTimeTZ time)
        {
            return time + interval;
        }

        public static NpgsqlTimeTZ operator -(NpgsqlTimeTZ time, NpgsqlInterval interval)
        {
            return time.Subtract(interval);
        }

        public static NpgsqlInterval operator -(NpgsqlTimeTZ later, NpgsqlTimeTZ earlier)
        {
            return later.Subtract(earlier);
        }

        public static explicit operator NpgsqlTimeTZ(TimeSpan time)
        {
            return new NpgsqlTimeTZ(new NpgsqlTime(time));
        }

        public static explicit operator TimeSpan(NpgsqlTimeTZ time)
        {
            return (TimeSpan)time.LocalTime;
        }

        public static explicit operator DateTime(NpgsqlTimeTZ time)
        {
            // LocalTime property is actually time local to TimeZone
            return new DateTime(time.AtTimeZone(NpgsqlTimeZone.CurrentTimeZone).Ticks, DateTimeKind.Local);
        }
    }
}
