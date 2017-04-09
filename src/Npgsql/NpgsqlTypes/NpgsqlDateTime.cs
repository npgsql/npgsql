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
using JetBrains.Annotations;
using Npgsql;
#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes
{
    /// <summary>
    /// A struct similar to .NET DateTime but capable of storing PostgreSQL's timestamp and timestamptz types.
    /// DateTime is capable of storing values from year 1 to 9999 at 100-nanosecond precision,
    /// while PostgreSQL's timestamps store values from 4713BC to 5874897AD with 1-microsecond precision.
    /// </summary>
#if NET45 || NET451
    [Serializable]
#endif
    public struct NpgsqlDateTime : IEquatable<NpgsqlDateTime>, IComparable<NpgsqlDateTime>, IComparable,
                                    IComparer<NpgsqlDateTime>, IComparer
    {
        #region Fields

        readonly NpgsqlDate _date;
        readonly TimeSpan _time;
        readonly InternalType _type;

        #endregion

        #region Constants

        public static readonly NpgsqlDateTime Epoch = new NpgsqlDateTime(NpgsqlDate.Epoch);
        public static readonly NpgsqlDateTime Era = new NpgsqlDateTime(NpgsqlDate.Era);

        public static readonly NpgsqlDateTime Infinity =
            new NpgsqlDateTime(InternalType.Infinity, NpgsqlDate.Era, TimeSpan.Zero);

        public static readonly NpgsqlDateTime NegativeInfinity =
            new NpgsqlDateTime(InternalType.NegativeInfinity, NpgsqlDate.Era, TimeSpan.Zero);

        // 9999-12-31
        private const int MaxDateTimeDay = 3652058;

        #endregion

        #region Constructors

        NpgsqlDateTime(InternalType type, NpgsqlDate date, TimeSpan time)
        {
            if (!date.IsFinite && type != InternalType.Infinity && type != InternalType.NegativeInfinity)
                throw new ArgumentException("Can't construct an NpgsqlDateTime with a non-finite date, use Infinity and NegativeInfinity instead", nameof(date));

            _type = type;
            _date = date;
            _time = time;
        }

        public NpgsqlDateTime(NpgsqlDate date, TimeSpan time, DateTimeKind kind = DateTimeKind.Unspecified)
            : this(KindToInternalType(kind), date, time) {}

        public NpgsqlDateTime(NpgsqlDate date)
            : this(date, TimeSpan.Zero) {}

        public NpgsqlDateTime(int year, int month, int day, int hours, int minutes, int seconds, DateTimeKind kind=DateTimeKind.Unspecified)
            : this(new NpgsqlDate(year, month, day), new TimeSpan(0, hours, minutes, seconds), kind) {}

        public NpgsqlDateTime(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds, DateTimeKind kind = DateTimeKind.Unspecified)
            : this(new NpgsqlDate(year, month, day), new TimeSpan(0, hours, minutes, seconds, milliseconds), kind) { }

        public NpgsqlDateTime(DateTime dateTime)
            : this(new NpgsqlDate(dateTime.Date), dateTime.TimeOfDay, dateTime.Kind) {}

        public NpgsqlDateTime(long ticks, DateTimeKind kind)
            : this(new DateTime(ticks, kind)) { }

        public NpgsqlDateTime(long ticks)
            : this(new DateTime(ticks, DateTimeKind.Unspecified)) { }

        #endregion

        #region Public Properties

        public NpgsqlDate Date => _date;
        public TimeSpan Time => _time;
        public int DayOfYear => _date.DayOfYear;
        public int Year => _date.Year;
        public int Month => _date.Month;
        public int Day => _date.Day;
        public DayOfWeek DayOfWeek => _date.DayOfWeek;
        public bool IsLeapYear => _date.IsLeapYear;

        public long Ticks => _date.DaysSinceEra * NpgsqlTimeSpan.TicksPerDay + _time.Ticks;
        public int Millisecond => _time.Milliseconds;
        public int Second => _time.Seconds;
        public int Minute => _time.Minutes;
        public int Hour => _time.Hours;
        public bool IsInfinity => _type == InternalType.Infinity;
        public bool IsNegativeInfinity => _type == InternalType.NegativeInfinity;

        public bool IsFinite
        {
            get
            {
                switch (_type) {
                case InternalType.FiniteUnspecified:
                case InternalType.FiniteUtc:
                case InternalType.FiniteLocal:
                    return true;
                case InternalType.Infinity:
                case InternalType.NegativeInfinity:
                    return false;
                default:
                    throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDateTime)}.{nameof(InternalType)}. Please file a bug.");
                }
            }
        }

        public DateTimeKind Kind
        {
            get
            {
                switch (_type)
                {
                case InternalType.FiniteUtc:
                    return DateTimeKind.Utc;
                case InternalType.FiniteLocal:
                    return DateTimeKind.Local;
                case InternalType.FiniteUnspecified:
                case InternalType.Infinity:
                case InternalType.NegativeInfinity:
                    return DateTimeKind.Unspecified;
                default:
                    throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(DateTimeKind)}. Please file a bug.");
                }
            }
        }

        /// <summary>
        /// Cast of an <see cref="NpgsqlDateTime"/> to a <see cref="DateTime"/>.
        /// </summary>
        /// <returns>An equivalent <see cref="DateTime"/>.</returns>
        public DateTime ToDateTime()
        {
            if (!IsFinite)
                throw new InvalidCastException("Can't convert infinite timestamp values to DateTime");

            if (_date.DaysSinceEra < 0 || _date.DaysSinceEra > MaxDateTimeDay)
                throw new InvalidCastException("Out of the range of DateTime (year must be between 1 and 9999)");

            return new DateTime(Ticks, Kind);
        }

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlDateTime"/> object to Coordinated Universal Time (UTC).
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for DateTime.ToUniversalTime().
        /// <b>Note:</b> this method <b>only</b> takes into account the time zone's base offset, and does
        /// <b>not</b> respect daylight savings. See https://github.com/npgsql/npgsql/pull/684 for more
        /// details.
        /// </remarks>
        public NpgsqlDateTime ToUniversalTime()
        {
            switch (_type)
            {
            case InternalType.FiniteUnspecified:
                // Treat as Local
            case InternalType.FiniteLocal:
                if (_date.DaysSinceEra >= 1 && _date.DaysSinceEra <= MaxDateTimeDay - 1)
                {
                    // Day between 0001-01-02 and 9999-12-30, so we can use DateTime and it will always succeed
                    return new NpgsqlDateTime(Subtract(TimeZoneInfo.Local.GetUtcOffset(new DateTime(ToDateTime().Ticks, DateTimeKind.Local))).Ticks, DateTimeKind.Utc);
                }
                // Else there are no DST rules available in the system for outside the DateTime range, so just use the base offset
                return new NpgsqlDateTime(Subtract(TimeZoneInfo.Local.BaseUtcOffset).Ticks, DateTimeKind.Utc);
            case InternalType.FiniteUtc:
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                return this;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDateTime)}.{nameof(InternalType)}. Please file a bug.");
            }
        }

        /// <summary>
        /// Converts the value of the current <see cref="NpgsqlDateTime"/> object to local time.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for DateTime.ToLocalTime().
        /// <b>Note:</b> this method <b>only</b> takes into account the time zone's base offset, and does
        /// <b>not</b> respect daylight savings. See https://github.com/npgsql/npgsql/pull/684 for more
        /// details.
        /// </remarks>
        public NpgsqlDateTime ToLocalTime()
        {
            switch (_type) {
            case InternalType.FiniteUnspecified:
                // Treat as UTC
            case InternalType.FiniteUtc:
                if (_date.DaysSinceEra >= 1 && _date.DaysSinceEra <= MaxDateTimeDay - 1)
                {
                    // Day between 0001-01-02 and 9999-12-30, so we can use DateTime and it will always succeed
                    return new NpgsqlDateTime(TimeZoneInfo.ConvertTime(new DateTime(ToDateTime().Ticks, DateTimeKind.Utc), TimeZoneInfo.Local));
                }
                // Else there are no DST rules available in the system for outside the DateTime range, so just use the base offset
                return new NpgsqlDateTime(Add(TimeZoneInfo.Local.BaseUtcOffset).Ticks, DateTimeKind.Local);
            case InternalType.FiniteLocal:
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                return this;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {_type} of enum {nameof(NpgsqlDateTime)}.{nameof(InternalType)}. Please file a bug.");
            }
        }

        public static NpgsqlDateTime Now => new NpgsqlDateTime(DateTime.Now);

        #endregion

        #region String Conversions

        public override string ToString()
        {
            switch (_type) {
            case InternalType.Infinity:
                return "infinity";
            case InternalType.NegativeInfinity:
                return "-infinity";
            default:
                return $"{_date} {_time}";
            }
        }

        public static NpgsqlDateTime Parse(string str)
        {
            if (str == null) {
                throw new NullReferenceException();
            }
            switch (str = str.Trim().ToLowerInvariant()) {
            case "infinity":
                return Infinity;
            case "-infinity":
                return NegativeInfinity;
            default:
                try {
                    var idxSpace = str.IndexOf(' ');
                    var datePart = str.Substring(0, idxSpace);
                    if (str.Contains("bc")) {
                        datePart += " BC";
                    }
                    var idxSecond = str.IndexOf(' ', idxSpace + 1);
                    if (idxSecond == -1) {
                        idxSecond = str.Length;
                    }
                    var timePart = str.Substring(idxSpace + 1, idxSecond - idxSpace - 1);
                    return new NpgsqlDateTime(NpgsqlDate.Parse(datePart), TimeSpan.Parse(timePart));
                } catch (OverflowException) {
                    throw;
                } catch {
                    throw new FormatException();
                }
            }
        }

        #endregion

        #region Comparisons

        public bool Equals(NpgsqlDateTime other)
        {
            switch (_type) {
            case InternalType.Infinity:
                return other._type == InternalType.Infinity;
            case InternalType.NegativeInfinity:
                return other._type == InternalType.NegativeInfinity;
            default:
                return other._type == _type && _date.Equals(other._date) && _time.Equals(other._time);
            }
        }

        public override bool Equals([CanBeNull] object obj)
            => obj is NpgsqlDateTime && Equals((NpgsqlDateTime)obj);

        public override int GetHashCode()
        {
            switch (_type) {
            case InternalType.Infinity:
                return int.MaxValue;
            case InternalType.NegativeInfinity:
                return int.MinValue;
            default:
                return _date.GetHashCode() ^ PGUtil.RotateShift(_time.GetHashCode(), 16);
            }
        }

        public int CompareTo(NpgsqlDateTime other)
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
                    var cmp = _date.CompareTo(other._date);
                    return cmp == 0 ? _time.CompareTo(_time) : cmp;
                }
            }
        }

        public int CompareTo([CanBeNull] object o)
        {
            if (o == null)
                return 1;
            if (o is NpgsqlDateTime)
                return CompareTo((NpgsqlDateTime)o);
            throw new ArgumentException();
        }

        public int Compare(NpgsqlDateTime x, NpgsqlDateTime y) => x.CompareTo(y);

        public int Compare([CanBeNull] object x, [CanBeNull] object y)
        {
            if (x == null)
                return y == null ? 0 : -1;
            if (y == null)
                return 1;
            if (!(x is IComparable) || !(y is IComparable))
                throw new ArgumentException();
            return ((IComparable)x).CompareTo(y);
        }

        #endregion

        #region Arithmetic

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the value of the specified TimeSpan to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time interval represented by value.</returns>
        public NpgsqlDateTime Add(NpgsqlTimeSpan value) { return AddTicks(value.Ticks); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the value of the specified <see cref="NpgsqlTimeSpan"/> to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time interval represented by value.</returns>
        public NpgsqlDateTime Add(TimeSpan value) { return AddTicks(value.Ticks); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of years represented by value.</returns>
        public NpgsqlDateTime AddYears(int value)
        {
            switch (_type) {
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                return this;
            default:
                return new NpgsqlDateTime(_type, _date.AddYears(value), _time);
            }
        }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="value">A number of months. The months parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and months.</returns>
        public NpgsqlDateTime AddMonths(int value)
        {
            switch (_type) {
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                return this;
            default:
                return new NpgsqlDateTime(_type, _date.AddMonths(value), _time);
            }
        }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional days. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of days represented by value.</returns>
        public NpgsqlDateTime AddDays(double value) { return Add(TimeSpan.FromDays(value)); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of hours represented by value.</returns>
        public NpgsqlDateTime AddHours(double value) { return Add(TimeSpan.FromHours(value)); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of minutes represented by value.</returns>
        public NpgsqlDateTime AddMinutes(double value) { return Add(TimeSpan.FromMinutes(value)); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of minutes represented by value.</returns>
        public NpgsqlDateTime AddSeconds(double value) { return Add(TimeSpan.FromSeconds(value)); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of milliseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional milliseconds. The value parameter can be negative or positive. Note that this value is rounded to the nearest integer.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of milliseconds represented by value.</returns>
        public NpgsqlDateTime AddMilliseconds(double value) { return Add(TimeSpan.FromMilliseconds(value)); }

        /// <summary>
        /// Returns a new <see cref="NpgsqlDateTime"/> that adds the specified number of ticks to the value of this instance.
        /// </summary>
        /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be positive or negative.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time represented by value.</returns>
        public NpgsqlDateTime AddTicks(long value)
        {
            switch (_type) {
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                return this;
            default:
                return new NpgsqlDateTime(Ticks + value, Kind);
            }
        }

        public NpgsqlDateTime Subtract(NpgsqlTimeSpan interval)
        {
            return Add(-interval);
        }

        public NpgsqlTimeSpan Subtract(NpgsqlDateTime timestamp)
        {
            switch (_type) {
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                throw new InvalidOperationException("You cannot subtract infinity timestamps");
            }
            switch (timestamp._type) {
            case InternalType.Infinity:
            case InternalType.NegativeInfinity:
                throw new InvalidOperationException("You cannot subtract infinity timestamps");
            }
            return new NpgsqlTimeSpan(0, _date.DaysSinceEra - timestamp._date.DaysSinceEra, _time.Ticks - timestamp._time.Ticks);
        }

        #endregion

        #region Operators

        public static NpgsqlDateTime operator +(NpgsqlDateTime timestamp, NpgsqlTimeSpan interval)
            => timestamp.Add(interval);

        public static NpgsqlDateTime operator +(NpgsqlTimeSpan interval, NpgsqlDateTime timestamp)
            => timestamp.Add(interval);

        public static NpgsqlDateTime operator -(NpgsqlDateTime timestamp, NpgsqlTimeSpan interval)
            => timestamp.Subtract(interval);

        public static NpgsqlTimeSpan operator -(NpgsqlDateTime x, NpgsqlDateTime y) => x.Subtract(y);
        public static bool operator ==(NpgsqlDateTime x, NpgsqlDateTime y) => x.Equals(y);
        public static bool operator !=(NpgsqlDateTime x, NpgsqlDateTime y) => !(x == y);
        public static bool operator <(NpgsqlDateTime x, NpgsqlDateTime y) => x.CompareTo(y) < 0;
        public static bool operator >(NpgsqlDateTime x, NpgsqlDateTime y) => x.CompareTo(y) > 0;
        public static bool operator <=(NpgsqlDateTime x, NpgsqlDateTime y) => x.CompareTo(y) <= 0;
        public static bool operator >=(NpgsqlDateTime x, NpgsqlDateTime y) => x.CompareTo(y) >= 0;

        #endregion

        #region Casts

        /// <summary>
        /// Implicit cast of a <see cref="DateTime"/> to an <see cref="NpgsqlDateTime"/>
        /// </summary>
        /// <param name="dateTime">A <see cref="DateTime"/></param>
        /// <returns>An equivalent <see cref="NpgsqlDateTime"/>.</returns>
        public static implicit operator NpgsqlDateTime(DateTime dateTime) => ToNpgsqlDateTime(dateTime);
        public static NpgsqlDateTime ToNpgsqlDateTime(DateTime dateTime) => new NpgsqlDateTime(dateTime);

        /// <summary>
        /// Explicit cast of an <see cref="NpgsqlDateTime"/> to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="npgsqlDateTime">An <see cref="NpgsqlDateTime"/>.</param>
        /// <returns>An equivalent <see cref="DateTime"/>.</returns>
        public static explicit operator DateTime(NpgsqlDateTime npgsqlDateTime)
            => npgsqlDateTime.ToDateTime();

        #endregion

        public NpgsqlDateTime Normalize() => Add(NpgsqlTimeSpan.Zero);

        static InternalType KindToInternalType(DateTimeKind kind)
        {
            switch (kind) {
            case DateTimeKind.Unspecified:
                return InternalType.FiniteUnspecified;
            case DateTimeKind.Utc:
                return InternalType.FiniteUtc;
            case DateTimeKind.Local:
                return InternalType.FiniteLocal;
            default:
                throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {kind} of enum {nameof(NpgsqlDateTime)}.{nameof(InternalType)}. Please file a bug.");
            }
        }

        enum InternalType
        {
            FiniteUnspecified,
            FiniteUtc,
            FiniteLocal,
            Infinity,
            NegativeInfinity
        }
    }
}
