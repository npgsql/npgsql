using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    [Serializable]
    public struct NpgsqlTimeZone : IEquatable<NpgsqlTimeZone>, IComparable<NpgsqlTimeZone>, IComparable
    {
        public static NpgsqlTimeZone UTC = new NpgsqlTimeZone(0);
        private readonly int _totalSeconds;

        public NpgsqlTimeZone(TimeSpan ts)
            : this(ts.Ticks)
        {
        }

        private NpgsqlTimeZone(long ticks)
        {
            _totalSeconds = (int)(ticks / NpgsqlInterval.TicksPerSecond);
        }

        public NpgsqlTimeZone(NpgsqlInterval ni)
            : this(ni.Ticks)
        {
        }

        public NpgsqlTimeZone(NpgsqlTimeZone copyFrom)
        {
            _totalSeconds = copyFrom._totalSeconds;
        }

        public NpgsqlTimeZone(int hours, int minutes)
            : this(hours, minutes, 0)
        {
        }

        public NpgsqlTimeZone(int hours, int minutes, int seconds)
        {
            _totalSeconds = hours * 60 * 60 + minutes * 60 + seconds;
        }

        public static implicit operator NpgsqlTimeZone(NpgsqlInterval interval)
        {
            return new NpgsqlTimeZone(interval);
        }

        public static implicit operator NpgsqlInterval(NpgsqlTimeZone timeZone)
        {
            return new NpgsqlInterval(timeZone._totalSeconds * NpgsqlInterval.TicksPerSecond);
        }

        public static implicit operator NpgsqlTimeZone(TimeSpan interval)
        {
            return new NpgsqlTimeZone(interval);
        }

        public static implicit operator TimeSpan(NpgsqlTimeZone timeZone)
        {
            return new TimeSpan(timeZone._totalSeconds * NpgsqlInterval.TicksPerSecond);
        }

        public static NpgsqlTimeZone SolarTimeZone(decimal longitude)
        {
            return new NpgsqlTimeZone((long)(longitude / 15m * NpgsqlInterval.TicksPerHour));
        }

        public int Hours
        {
            get { return _totalSeconds / 60 / 60; }
        }

        public int Minutes
        {
            get { return (_totalSeconds / 60) % 60; }
        }

        public int Seconds
        {
            get { return _totalSeconds % 60; }
        }

        public static NpgsqlTimeZone CurrentTimeZone
        {
            get { return new NpgsqlTimeZone(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now)); }
        }

        public static NpgsqlTimeZone LocalTimeZone(NpgsqlDate date)
        {
            DateTime dt;
            if (date.Year >= 1902 && date.Year <= 2038) {
                dt = (DateTime)date;
            } else {
                dt = new DateTime(2000, date.Month, date.Day);
            }
            return new NpgsqlTimeZone(TimeZone.CurrentTimeZone.GetUtcOffset(dt));
        }

        public bool Equals(NpgsqlTimeZone other)
        {
            return _totalSeconds == other._totalSeconds;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlTimeZone && Equals((NpgsqlTimeZone)obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(_totalSeconds < 0 ? "-" : "+").Append(Math.Abs(Hours).ToString("D2"));
            if (Minutes != 0 || Seconds != 0) {
                sb.Append(':').Append(Math.Abs(Minutes).ToString("D2"));
                if (Seconds != 0) {
                    sb.Append(":").Append(Math.Abs(Seconds).ToString("D2"));
                }
            }
            return sb.ToString();
        }

        public static NpgsqlTimeZone Parse(string str)
        {
            if (str == null) {
                throw new ArgumentNullException();
            }
            try {
                str = str.Trim();
                bool neg;
                switch (str[0]) {
                case '+':
                    neg = false;
                    break;
                case '-':
                    neg = true;
                    break;
                default:
                    throw new FormatException();
                }
                int hours;
                int minutes;
                int seconds;
                string[] parts = str.Substring(1).Split(':');
                switch (parts.Length) //One of those times that fall-through would actually be good.
                {
                case 1:
                    hours = int.Parse(parts[0]);
                    minutes = seconds = 0;
                    break;
                case 2:
                    hours = int.Parse(parts[0]);
                    minutes = int.Parse(parts[1]);
                    seconds = 0;
                    break;
                default:
                    hours = int.Parse(parts[0]);
                    minutes = int.Parse(parts[1]);
                    seconds = int.Parse(parts[2]);
                    break;
                }
                int totalSeconds = (hours * 60 * 60 + minutes * 60 + seconds) * (neg ? -1 : 1);
                return new NpgsqlTimeZone(totalSeconds * NpgsqlInterval.TicksPerSecond);
            } catch (OverflowException) {
                throw;
            } catch {
                throw new FormatException();
            }
        }

        public static bool TryParse(string str, NpgsqlTimeZone tz)
        {
            try {
                tz = Parse(str);
                return true;
            } catch {
                tz = UTC;
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _totalSeconds;
        }

        //Note, +01:00 is less than -01:00
        public int CompareTo(NpgsqlTimeZone other)
        {
            return -(_totalSeconds.CompareTo(other._totalSeconds));
        }

        public int CompareTo(object obj)
        {
            if (obj == null) {
                return 1;
            }
            if (obj is NpgsqlTimeZone) {
                return CompareTo((NpgsqlTimeZone)obj);
            }
            throw new ArgumentException();
        }

        public static NpgsqlTimeZone operator -(NpgsqlTimeZone tz)
        {
            return new NpgsqlTimeZone(-tz._totalSeconds * NpgsqlInterval.TicksPerSecond);
        }

        public static NpgsqlTimeZone operator +(NpgsqlTimeZone tz)
        {
            return tz;
        }

        public static bool operator ==(NpgsqlTimeZone x, NpgsqlTimeZone y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlTimeZone x, NpgsqlTimeZone y)
        {
            return !(x == y);
        }

        public static bool operator <(NpgsqlTimeZone x, NpgsqlTimeZone y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(NpgsqlTimeZone x, NpgsqlTimeZone y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >(NpgsqlTimeZone x, NpgsqlTimeZone y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(NpgsqlTimeZone x, NpgsqlTimeZone y)
        {
            return x.CompareTo(y) >= 0;
        }
    }
}
