using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    [Serializable]
    public struct NpgsqlTime : IEquatable<NpgsqlTime>, IComparable<NpgsqlTime>, IComparable, IComparer<NpgsqlTime>,
                               IComparer
    {
        public static readonly NpgsqlTime AllBalls = new NpgsqlTime(0);

        public static NpgsqlTime Now
        {
            get { return new NpgsqlTime(DateTime.Now.TimeOfDay); }
        }

        private readonly long _ticks;

        public NpgsqlTime(long ticks)
        {
            if (ticks == NpgsqlInterval.TicksPerDay) {
                _ticks = ticks;
            } else {
                ticks %= NpgsqlInterval.TicksPerDay;
                _ticks = ticks < 0 ? ticks + NpgsqlInterval.TicksPerDay : ticks;
            }
        }

        public NpgsqlTime(TimeSpan time)
            : this(time.Ticks)
        {
        }

        public NpgsqlTime(NpgsqlInterval time)
            : this(time.Ticks)
        {
        }

        public NpgsqlTime(NpgsqlTime copyFrom)
            : this(copyFrom.Ticks)
        {
        }

        public NpgsqlTime(int hours, int minutes, int seconds)
            : this(hours, minutes, seconds, 0)
        {
        }

        public NpgsqlTime(int hours, int minutes, int seconds, int microseconds)
            : this(
                hours * NpgsqlInterval.TicksPerHour + minutes * NpgsqlInterval.TicksPerMinute + seconds * NpgsqlInterval.TicksPerSecond +
                microseconds * NpgsqlInterval.TicksPerMicrosecond)
        {
        }

        public NpgsqlTime(int hours, int minutes, decimal seconds)
            : this(
                hours * NpgsqlInterval.TicksPerHour + minutes * NpgsqlInterval.TicksPerMinute +
                (long)(seconds * NpgsqlInterval.TicksPerSecond))
        {
        }

        public NpgsqlTime(int hours, int minutes, double seconds)
            : this(hours, minutes, (decimal)seconds)
        {
        }

        /// <summary>
        /// The total number of ticks(100ns units) contained. This is the resolution of the
        /// <see cref="NpgsqlTime"/>  type.
        /// <remarks>The resolution of the PostgreSQL
        /// interval type is by default 1&#xb5;s = 1,000 ns. It may be smaller as follows:
        /// <list type="number">
        /// <item>
        /// <term>time(0)</term>
        /// <description>resolution of 1s (1 second)</description>
        /// </item>
        /// <item>
        /// <term>time(1)</term>
        /// <description>resolution of 100ms = 0.1s (100 milliseconds)</description>
        /// </item>
        /// <item>
        /// <term>time(2)</term>
        /// <description>resolution of 10ms = 0.01s (10 milliseconds)</description>
        /// </item>
        /// <item>
        /// <term>time(3)</term>
        /// <description>resolution of 1ms = 0.001s (1 millisecond)</description>
        /// </item>
        /// <item>
        /// <term>time(4)</term>
        /// <description>resolution of 100&#xb5;s = 0.0001s (100 microseconds)</description>
        /// </item>
        /// <item>
        /// <term>time(5)</term>
        /// <description>resolution of 10&#xb5;s = 0.00001s (10 microseconds)</description>
        /// </item>
        /// <item>
        /// <term>time(6) or interval</term>
        /// <description>resolution of 1&#xb5;s = 0.000001s (1 microsecond)</description>
        /// </item>
        /// </list>
        /// <para>As such, if the 100-nanosecond resolution is significant to an application, a PostgreSQL time will
        /// not suffice for those purposes.</para>
        /// <para>In more frequent cases though, the resolution of time suffices.
        /// <see cref="NpgsqlTime"/> will always suffice to handle the resolution of any time value, and upon
        /// writing to the database, will be rounded to the resolution used.</para>
        /// </remarks>
        /// <returns>The number of ticks in the instance.</returns>
        /// </summary>
        public long Ticks
        {
            get { return _ticks; }
        }

        /// <summary>
        /// Gets the number of whole microseconds held in the instance.
        /// <returns>An integer in the range [0, 999999].</returns>
        /// </summary>
        public int Microseconds
        {
            get { return (int)((_ticks / 10) % 1000000); }
        }

        /// <summary>
        /// Gets the number of whole milliseconds held in the instance.
        /// <returns>An integer in the range [0, 999].</returns>
        /// </summary>
        public int Milliseconds
        {
            get { return (int)((_ticks / NpgsqlInterval.TicksPerMillsecond) % 1000); }
        }

        /// <summary>
        /// Gets the number of whole seconds held in the instance.
        /// <returns>An interger in the range [0, 59].</returns>
        /// </summary>
        public int Seconds
        {
            get { return (int)((_ticks / NpgsqlInterval.TicksPerSecond) % 60); }
        }

        /// <summary>
        /// Gets the number of whole minutes held in the instance.
        /// <returns>An integer in the range [0, 59].</returns>
        /// </summary>
        public int Minutes
        {
            get { return (int)((_ticks / NpgsqlInterval.TicksPerMinute) % 60); }
        }

        /// <summary>
        /// Gets the number of whole hours held in the instance.
        /// <remarks>Note that the time 24:00:00 can be stored for roundtrip compatibility. Any calculations on such a
        /// value will normalised it to 00:00:00.</remarks>
        /// </summary>
        public int Hours
        {
            get { return (int)(_ticks / NpgsqlInterval.TicksPerHour); }
        }

        /// <summary>
        /// Normalise this time; if it is 24:00:00, convert it to 00:00:00
        /// </summary>
        /// <returns>This time, normalised</returns>
        public NpgsqlTime Normalize()
        {
            return new NpgsqlTime(_ticks % NpgsqlInterval.TicksPerDay);
        }

        public bool Equals(NpgsqlTime other)
        {
            return Ticks == other.Ticks;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlTime && Equals((NpgsqlTime)obj);
        }

        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }

        public override string ToString()
        {
            // calculate total seconds and then subtract total whole minutes in seconds to get just the seconds and fractional part
            decimal seconds = _ticks / (decimal)NpgsqlInterval.TicksPerSecond - (_ticks / NpgsqlInterval.TicksPerMinute) * 60;
            StringBuilder sb =
                new StringBuilder(Hours.ToString("D2")).Append(':').Append(Minutes.ToString("D2")).Append(':').Append(
                    seconds.ToString("0#.######", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
            return sb.ToString();
        }

        public static NpgsqlTime Parse(string str)
        {
            if (str == null) {
                throw new ArgumentNullException();
            }
            try {
                int hours = 0;
                int minutes = 0;
                decimal seconds = 0m;
                string[] parts = str.Split(':');
                switch (parts.Length) //One of those times that fall-through would actually be good.
                {
                case 1:
                    hours = int.Parse(parts[0]);
                    break;
                case 2:
                    hours = int.Parse(parts[0]);
                    minutes = int.Parse(parts[1]);
                    break;
                default:
                    hours = int.Parse(parts[0]);
                    minutes = int.Parse(parts[1]);
                    seconds = decimal.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    break;
                }
                if (hours < 0 || hours > 24 || minutes < 0 || minutes > 59 || seconds < 0m || seconds >= 60 ||
                    (hours == 24 && (minutes != 0 || seconds != 0m))) {
                    throw new OverflowException();
                }
                return new NpgsqlTime(hours, minutes, seconds);
            } catch (OverflowException) {
                throw;
            } catch {
                throw new FormatException();
            }
        }

        public static bool TryParse(string str, out NpgsqlTime time)
        {
            try {
                time = Parse(str);
                return true;
            } catch {
                time = AllBalls;
                return false;
            }
        }

        public int CompareTo(NpgsqlTime other)
        {
            return Ticks.CompareTo(other.Ticks);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) {
                return 1;
            }
            if (obj is NpgsqlTime) {
                return CompareTo((NpgsqlTime)obj);
            }
            throw new ArgumentException();
        }

        public int Compare(NpgsqlTime x, NpgsqlTime y)
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

        public static bool operator ==(NpgsqlTime x, NpgsqlTime y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlTime x, NpgsqlTime y)
        {
            return !(x == y);
        }

        public static bool operator <(NpgsqlTime x, NpgsqlTime y)
        {
            return x.Ticks < y.Ticks;
        }

        public static bool operator >(NpgsqlTime x, NpgsqlTime y)
        {
            return x.Ticks > y.Ticks;
        }

        public static bool operator <=(NpgsqlTime x, NpgsqlTime y)
        {
            return x.Ticks <= y.Ticks;
        }

        public static bool operator >=(NpgsqlTime x, NpgsqlTime y)
        {
            return x.Ticks >= y.Ticks;
        }

        public static explicit operator NpgsqlInterval(NpgsqlTime time)
        {
            return new NpgsqlInterval(time.Ticks);
        }

        public static explicit operator NpgsqlTime(NpgsqlInterval interval)
        {
            return new NpgsqlTime(interval);
        }

        public static explicit operator TimeSpan(NpgsqlTime time)
        {
            return new TimeSpan(time.Ticks);
        }

        public static explicit operator DateTime(NpgsqlTime time)
        {
            try {
                return new DateTime(time.Ticks, DateTimeKind.Unspecified);
            } catch {
                throw new InvalidCastException();
            }
        }

        public static explicit operator NpgsqlTime(TimeSpan interval)
        {
            return new NpgsqlTime(interval);
        }

        public NpgsqlTime AddTicks(long ticksAdded)
        {
            return new NpgsqlTime((Ticks + ticksAdded) % NpgsqlInterval.TicksPerDay);
        }

        private NpgsqlTime AddTicks(long ticksAdded, out int overflow)
        {
            long result = Ticks + ticksAdded;
            overflow = (int)(result / NpgsqlInterval.TicksPerDay);
            result %= NpgsqlInterval.TicksPerDay;
            if (result < 0) {
                --overflow; //"carry the one"
            }
            return new NpgsqlTime(result);
        }

        public NpgsqlTime Add(NpgsqlInterval interval)
        {
            return AddTicks(interval.Ticks);
        }

        internal NpgsqlTime Add(NpgsqlInterval interval, out int overflow)
        {
            return AddTicks(interval.Ticks, out overflow);
        }

        public NpgsqlTime Subtract(NpgsqlInterval interval)
        {
            return AddTicks(-interval.Ticks);
        }

        public NpgsqlInterval Subtract(NpgsqlTime earlier)
        {
            return new NpgsqlInterval(Ticks - earlier.Ticks);
        }

        public NpgsqlTimeTZ AtTimeZone(NpgsqlTimeZone timeZone)
        {
            return new NpgsqlTimeTZ(this).AtTimeZone(timeZone);
        }

        public static NpgsqlTime operator +(NpgsqlTime time, NpgsqlInterval interval)
        {
            return time.Add(interval);
        }

        public static NpgsqlTime operator +(NpgsqlInterval interval, NpgsqlTime time)
        {
            return time + interval;
        }

        public static NpgsqlTime operator -(NpgsqlTime time, NpgsqlInterval interval)
        {
            return time.Subtract(interval);
        }

        public static NpgsqlInterval operator -(NpgsqlTime later, NpgsqlTime earlier)
        {
            return later.Subtract(earlier);
        }
    }
}
