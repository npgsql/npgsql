// NpgsqlTypes.NpgsqlTypesHelper.cs
//
// Author:
//	Glen Parker <glenebob@nwlink.com>
//
//	Copyright (C) 2004 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

// This file provides implementations of PostgreSQL specific data types that cannot
// be mapped to standard .NET classes.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

using Npgsql;

namespace NpgsqlTypes
{

    /// <summary>
    /// Represents a PostgreSQL Point type
    /// </summary>
    public struct NpgsqlPoint : IEquatable<NpgsqlPoint>
    {
        public Single X;
        public Single Y;

        public NpgsqlPoint(Single x, Single y)
        {
            X = x;
            Y = y;
        }
        public bool Equals(NpgsqlPoint other)
        {
            return X == other.X && Y == other.Y;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlPoint && Equals((NpgsqlPoint)obj);
        }
        public static bool operator==(NpgsqlPoint x, NpgsqlPoint y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlPoint x, NpgsqlPoint y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof(int) / 2);
        }
    }

    public struct NpgsqlBox : IEquatable<NpgsqlBox>
    {
        public NpgsqlPoint UpperRight;
        public NpgsqlPoint LowerLeft;

        public NpgsqlBox(NpgsqlPoint upperRight, NpgsqlPoint lowerLeft)
        {
            UpperRight = upperRight;
            LowerLeft = lowerLeft;
        }
        public NpgsqlBox(float Top, float Right, float Bottom, float Left)
            :this(new NpgsqlPoint(Right, Top), new NpgsqlPoint(Left, Bottom)){}
        public float Left
        {
            get
            {
                return LowerLeft.X;
            }
        }
        public float Right
        {
            get
            {
                return UpperRight.X;
            }
        }
        public float Bottom
        {
            get
            {
                return LowerLeft.Y;
            }
        }
        public float Top
        {
            get
            {
                return UpperRight.Y;
            }
        }
        public float Width
        {
            get
            {
                return Right - Left;
            }
        }
        public float Height
        {
            get
            {
                return Top - Bottom;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return Width == 0 || Height == 0;
            }
        }
        public bool Equals(NpgsqlBox other)
        {
            return UpperRight == other.UpperRight && LowerLeft == other.LowerLeft;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlBox && Equals((NpgsqlBox) obj);
        }
        public static bool operator==(NpgsqlBox x, NpgsqlBox y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlBox x, NpgsqlBox y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return Top.GetHashCode()
                ^ PGUtil.RotateShift(Right.GetHashCode(), sizeof(int) / 4)
                ^ PGUtil.RotateShift(Bottom.GetHashCode(), sizeof(int) / 2)
                ^ PGUtil.RotateShift(LowerLeft.GetHashCode(), sizeof(int) * 3 / 4);
        }
    }


    /// <summary>
    /// Represents a PostgreSQL Line Segment type.
    /// </summary>
    public struct NpgsqlLSeg : IEquatable<NpgsqlLSeg>
    {
        public NpgsqlPoint     Start;
        public NpgsqlPoint     End;

        public NpgsqlLSeg(NpgsqlPoint start, NpgsqlPoint end)
        {
            Start = start;
            End = end;
        }
        public override String ToString()
        {
            return String.Format("({0}, {1})", Start, End);
        }
        public override int GetHashCode()
        {
            return Start.X.GetHashCode()
                ^ PGUtil.RotateShift(Start.Y.GetHashCode(), sizeof(int) / 4)
                ^ PGUtil.RotateShift(End.X.GetHashCode(), sizeof(int) / 2)
                ^ PGUtil.RotateShift(End.Y.GetHashCode(), sizeof(int) * 3 / 4);
        }
        public bool Equals(NpgsqlLSeg other)
        {
            return Start == other.Start && End == other.End;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlLSeg && Equals((NpgsqlLSeg)obj);
        }
        public static bool operator==(NpgsqlLSeg x, NpgsqlLSeg y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlLSeg x, NpgsqlLSeg y)
        {
            return !(x == y);
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Path type.
    /// </summary>
    public struct NpgsqlPath : IList<NpgsqlPoint>, IEquatable<NpgsqlPath>
    {
        public bool Open;
        private readonly List<NpgsqlPoint> _points;
        public NpgsqlPath(IEnumerable<NpgsqlPoint> points, bool open)
        {
            _points = new List<NpgsqlPoint>(points);
            Open = open;
        }
        public NpgsqlPath(IEnumerable<NpgsqlPoint> points)
            :this(points, false){}
        public NpgsqlPath(bool open)
        {
            _points = new List<NpgsqlPoint>();
            Open = open;
        }
        public NpgsqlPath(int capacity, bool open)
        {
            _points = new List<NpgsqlPoint>(capacity);
            Open = open;
        }
        public NpgsqlPath(int capacity)
            :this(capacity, false){}
        public NpgsqlPoint this[int index]
        {
            get
            {
                return _points[index];
            }
            set
            {
                _points[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return _points.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public int IndexOf(NpgsqlPoint item)
        {
            return _points.IndexOf(item);
        }
        public void Insert(int index, NpgsqlPoint item)
        {
            _points.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);
        }
        public void Add(NpgsqlPoint item)
        {
            _points.Add(item);
        }
        public void Clear()
        {
            _points.Clear();
        }
        public bool Contains(NpgsqlPoint item)
        {
            return _points.Contains(item);
        }
        public void CopyTo(NpgsqlPoint[] array, int arrayIndex)
        {
            _points.CopyTo(array, arrayIndex);
        }
        public bool Remove(NpgsqlPoint item)
        {
            return _points.Remove(item);
        }
        public IEnumerator<NpgsqlPoint> GetEnumerator()
        {
            return _points.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public bool Equals(NpgsqlPath other)
        {
            if(Open != other.Open || Count != other.Count)
                return false;
            for(int i = 0; i != Count; ++i)
                if(this[i] != other[i])
                return false;
            return true;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlPath && Equals((NpgsqlPath)obj);
        }
        public static bool operator==(NpgsqlPath x, NpgsqlPath y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlPath x, NpgsqlPath y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            int ret = 0;
            foreach(NpgsqlPoint point in this)
                //The ideal amount to shift each value is one that would evenly spread it throughout
                //the resultant bytes. Using the current result % 32 is essentially using a random value
                //but one that will be the same on subsequent calls.
                ret ^= PGUtil.RotateShift(point.GetHashCode(), ret % sizeof(int));
            return Open ? ret : -ret;
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Polygon type.
    /// </summary>
    public struct NpgsqlPolygon : IList<NpgsqlPoint>, IEquatable<NpgsqlPolygon>
    {
        private readonly List<NpgsqlPoint> _points;
        public NpgsqlPolygon(IEnumerable<NpgsqlPoint> points)
        {
            _points = new List<NpgsqlPoint>(points);
        }
        public NpgsqlPoint this[int index]
        {
            get
            {
                return _points[index];
            }
            set
            {
                _points[index] = value;
            }
        }
        public int Count
        {
            get
            {
                return _points.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public int IndexOf(NpgsqlPoint item)
        {
            return _points.IndexOf(item);
        }
        public void Insert(int index, NpgsqlPoint item)
        {
            _points.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);
        }
        public void Add(NpgsqlPoint item)
        {
            _points.Add(item);
        }
        public void Clear()
        {
            _points.Clear();
        }
        public bool Contains(NpgsqlPoint item)
        {
            return _points.Contains(item);
        }
        public void CopyTo(NpgsqlPoint[] array, int arrayIndex)
        {
            _points.CopyTo(array, arrayIndex);
        }
        public bool Remove(NpgsqlPoint item)
        {
            return _points.Remove(item);
        }
        public IEnumerator<NpgsqlPoint> GetEnumerator()
        {
            return _points.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public bool Equals(NpgsqlPolygon other)
        {
            if(Count != other.Count)
                return false;
            for(int i = 0; i != Count; ++i)
                if(this[i] != other[i])
                return false;
            return true;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlPolygon && Equals((NpgsqlPolygon)obj);
        }
        public static bool operator==(NpgsqlPolygon x, NpgsqlPolygon y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlPolygon x, NpgsqlPolygon y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            int ret = 0;
            foreach(NpgsqlPoint point in this)
                //The ideal amount to shift each value is one that would evenly spread it throughout
                //the resultant bytes. Using the current result % 32 is essentially using a random value
                //but one that will be the same on subsequent calls.
                ret ^= PGUtil.RotateShift(point.GetHashCode(), ret % sizeof(int));
            return ret;
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Circle type.
    /// </summary>
    public struct NpgsqlCircle : IEquatable<NpgsqlCircle>
    {
        public NpgsqlPoint   Center;
        public Double        Radius;

        public NpgsqlCircle(NpgsqlPoint center, Double radius)
        {
            Center = center;
            Radius = radius;
        }
        public bool Equals(NpgsqlCircle other)
        {
            return Center == other.Center && Radius == other.Radius;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlCircle && Equals((NpgsqlCircle)obj);
        }
        public override String ToString()
        {
            return string.Format("({0}), {1}", Center, Radius);
        }
        public static bool operator==(NpgsqlCircle x, NpgsqlCircle y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlCircle x, NpgsqlCircle y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            return Center.X.GetHashCode()
                ^ PGUtil.RotateShift(Center.Y.GetHashCode(), sizeof(int) / 4)
                ^ PGUtil.RotateShift(Radius.GetHashCode(), sizeof(int) / 2);
        }
    }


    /// <summary>
    /// Represents a PostgreSQL inet type.
    /// </summary>
    public struct NpgsqlInet : IEquatable<NpgsqlInet>
    {
        public IPAddress addr;
        public int mask;

        public NpgsqlInet(IPAddress addr, int mask)
        {
            this.addr = addr;
            this.mask = mask;
        }

        public NpgsqlInet(IPAddress addr)
        {
            this.addr = addr;
            this.mask = 32;
        }

        public NpgsqlInet(string addr)
        {
            if (addr.IndexOf('/') > 0)
            {
                string[] addrbits = addr.Split('/');
                if (addrbits.GetUpperBound(0) != 1)
                    throw new FormatException("Invalid number of parts in CIDR specification");
                this.addr = IPAddress.Parse(addrbits[0]);
                this.mask = int.Parse(addrbits[1]);
            }
            else
            {
                this.addr = IPAddress.Parse(addr);
                this.mask = 32;
            }
        }

        public override String ToString()
        {
            if (mask != 32)
                return string.Format("{0}/{1}", addr.ToString(), mask);
            else
                return addr.ToString();
        }
        public static implicit operator IPAddress(NpgsqlInet x)
        {
            if (x.mask != 32)
                throw new InvalidCastException("Cannot cast CIDR network to address");
            else
                return x.addr;
        }
        public bool Equals(NpgsqlInet other)
        {
            return addr.Equals(other.addr) && mask == other.mask;
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlInet && Equals((NpgsqlInet)obj);
        }
        public override int GetHashCode()
        {
            return PGUtil.RotateShift(addr.GetHashCode(), mask % 32);
        }
        public static bool operator==(NpgsqlInet x, NpgsqlInet y)
        {
            return x.Equals(y);
        }
        public static bool operator!=(NpgsqlInet x, NpgsqlInet y)
        {
            return !(x == y);
        }
    }
}
/// <summary>
/// Represents the PostgreSQL interval datatype.
/// <remarks>PostgreSQL differs from .NET in how it's interval type doesn't assume 24 hours in a day
/// (to deal with 23- and 25-hour days caused by daylight savings adjustments) and has a concept
/// of months that doesn't exist in .NET's <see cref="TimeSpan"/> class. (Neither datatype
/// has any concessions for leap-seconds).
/// <para>For most uses just casting to and from TimeSpan will work correctly &#x2014; in particular,
/// the results of subtracting one <see cref="DateTime"/> or the PostgreSQL date, time and
/// timestamp types from another should be the same whether you do so in .NET or PostgreSQL &#x2014;
/// but if the handling of days and months in PostgreSQL is important to your application then you
/// should use this class instead of <see cref="TimeSpan"/>.</para>
/// <para>If you don't know whether these differences are important to your application, they
/// probably arent! Just use <see cref="TimeSpan"/> and do not use this class directly &#x263a;</para>
/// <para>To avoid forcing unnecessary provider-specific concerns on users who need not be concerned
/// with them a call to <see cref="IDataRecord.GetValue(int)"/> on a field containing an
/// <see cref="NpgsqlInterval"/> value will return a <see cref="TimeSpan"/> rather than an
/// <see cref="NpgsqlInterval"/>. If you need the extra functionality of <see cref="NpgsqlInterval"/>
/// then use <see cref="Npgsql.NpgsqlDataReader.GetInterval(Int32)"/>.</para>
/// </remarks>
/// <seealso cref="Ticks"/>
/// <seealso cref="JustifyDays"/>
/// <seealso cref="JustifyMonths"/>
/// <seealso cref="Canonicalize()"/>
/// </summary>
[Serializable]
public struct NpgsqlInterval : IComparable, IComparer, IEquatable<NpgsqlInterval>, IComparable<NpgsqlInterval>, IComparer<NpgsqlInterval>, ICloneable
{
    #region Constants
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one microsecond. This field is constant.
    /// </summary>
    public const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one millisecond. This field is constant.
    /// </summary>
    public const long TicksPerMillsecond = TimeSpan.TicksPerMillisecond;
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one second. This field is constant.
    /// </summary>
    public const long TicksPerSecond = TimeSpan.TicksPerSecond;
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one minute. This field is constant.
    /// </summary>
    public const long TicksPerMinute = TimeSpan.TicksPerMinute;
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one hour. This field is constant.
    /// </summary>
    public const long TicksPerHour = TimeSpan.TicksPerHour;
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one day. This field is constant.
    /// </summary>
    public const long TicksPerDay = TimeSpan.TicksPerDay;
    /// <summary>
    /// Represents the number of hours in one day (assuming no daylight savings adjustments). This field is constant.
    /// </summary>
    public const int HoursPerDay = 24;
    /// <summary>
    /// Represents the number of days assumed in one month if month justification or unjustifcation is performed.
    /// This is set to 30 for consistency with PostgreSQL. Note that this is means that month adjustments cause
    /// a year to be taken as 30 &#xd7; 12 = 360 rather than 356/366 days.
    /// </summary>
    public const int DaysPerMonth = 30;
    /// <summary>
    /// Represents the number of ticks (100ns periods) in one day, assuming 30 days per month. <seealso cref="DaysPerMonth"/>
    /// </summary>
    public const long TicksPerMonth = TicksPerDay * DaysPerMonth;
    /// <summary>
    /// Represents the number of months in a year. This field is constant.
    /// </summary>
    public const int MonthsPerYear = 12;
    /// <summary>
    /// Represents the maximum <see cref="NpgsqlInterval"/>. This field is read-only.
    /// </summary>
    public static readonly NpgsqlInterval MaxValue = new NpgsqlInterval(long.MaxValue);
    /// <summary>
    /// Represents the minimum <see cref="NpgsqlInterval"/>. This field is read-only.
    /// </summary>
    public static readonly NpgsqlInterval MinValue = new NpgsqlInterval(long.MinValue);
    /// <summary>
    /// Represents the zero <see cref="NpgsqlInterval"/>. This field is read-only.
    /// </summary>
    public static readonly NpgsqlInterval Zero = new NpgsqlInterval(0);
    #endregion

    private readonly int _months;
    private readonly int _days;
    private readonly long _ticks;
    #region Constructors
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to the specified number of ticks.
    /// </summary>
    /// <param name="ticks">A time period expressed in 100ns units.</param>
    public NpgsqlInterval(long ticks)
    {
        _months = 0;
        _days = 0;
        _ticks = ticks;
    }
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to hold the same time as a <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="timespan">A time period expressed in a <see cref="TimeSpan"/></param>
    public NpgsqlInterval(TimeSpan timespan)
        :this(timespan.Ticks){}
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to the specified number of months, days
    /// &amp; ticks.
    /// </summary>
    /// <param name="months">Number of months.</param>
    /// <param name="days">Number of days.</param>
    /// <param name="ticks">Number of 100ns units.</param>
    public NpgsqlInterval(int months, int days, long ticks)
    {
        _months = months;
        _days = days;
        _ticks = ticks;
    }
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to the specified number of
    /// days, hours, minutes &amp; seconds.
    /// </summary>
    /// <param name="days">Number of days.</param>
    /// <param name="hours">Number of hours.</param>
    /// <param name="minutes">Number of minutes.</param>
    /// <param name="seconds">Number of seconds.</param>
    public NpgsqlInterval(int days, int hours, int minutes, int seconds)
        :this(0, days, new TimeSpan(hours, minutes, seconds).Ticks){}
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to the specified number of
    /// days, hours, minutes, seconds &amp; milliseconds.
    /// </summary>
    /// <param name="days">Number of days.</param>
    /// <param name="hours">Number of hours.</param>
    /// <param name="minutes">Number of minutes.</param>
    /// <param name="seconds">Number of seconds.</param>
    /// <param name="milliseconds">Number of milliseconds.</param>
    public NpgsqlInterval(int days, int hours, int minutes, int seconds, int milliseconds)
        :this(0, days, new TimeSpan(hours, minutes, seconds, milliseconds).Ticks){}
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to the specified number of
    /// months, days, hours, minutes, seconds &amp; milliseconds.
    /// </summary>
    /// <param name="months">Number of months.</param>
    /// <param name="days">Number of days.</param>
    /// <param name="hours">Number of hours.</param>
    /// <param name="minutes">Number of minutes.</param>
    /// <param name="seconds">Number of seconds.</param>
    /// <param name="milliseconds">Number of milliseconds.</param>
    public NpgsqlInterval(int months, int days, int hours, int minutes, int seconds, int milliseconds)
        :this(months, days, new TimeSpan(hours, minutes, seconds, milliseconds).Ticks){}
    /// <summary>
    /// Initializes a new <see cref="NpgsqlInterval"/> to the specified number of
    /// years, months, days, hours, minutes, seconds &amp; milliseconds.
    /// <para>Years are calculated exactly equivalent to 12 months.</para>
    /// </summary>
    /// <param name="years">Number of years.</param>
    /// <param name="months">Number of months.</param>
    /// <param name="days">Number of days.</param>
    /// <param name="hours">Number of hours.</param>
    /// <param name="minutes">Number of minutes.</param>
    /// <param name="seconds">Number of seconds.</param>
    /// <param name="milliseconds">Number of milliseconds.</param>
    public NpgsqlInterval(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        :this(years * 12 + months, days, new TimeSpan(hours, minutes, seconds, milliseconds).Ticks){}
    #endregion
    #region Whole Parts
    /// <summary>
    /// The total number of ticks(100ns units) contained. This is the resolution of the
    /// <see cref="NpgsqlInterval"/>  type. This ignores the number of days and
    /// months held. If you want them included use <see cref="UnjustifyInterval()"/> first.
    /// <remarks>The resolution of the PostgreSQL
    /// interval type is by default 1&#xb5;s = 1,000 ns. It may be smaller as follows:
    /// <list type="number">
    /// <item>
    /// <term>interval(0)</term>
    /// <description>resolution of 1s (1 second)</description>
    /// </item>
    /// <item>
    /// <term>interval(1)</term>
    /// <description>resolution of 100ms = 0.1s (100 milliseconds)</description>
    /// </item>
    /// <item>
    /// <term>interval(2)</term>
    /// <description>resolution of 10ms = 0.01s (10 milliseconds)</description>
    /// </item>
    /// <item>
    /// <term>interval(3)</term>
    /// <description>resolution of 1ms = 0.001s (1 millisecond)</description>
    /// </item>
    /// <item>
    /// <term>interval(4)</term>
    /// <description>resolution of 100&#xb5;s = 0.0001s (100 microseconds)</description>
    /// </item>
    /// <item>
    /// <term>interval(5)</term>
    /// <description>resolution of 10&#xb5;s = 0.00001s (10 microseconds)</description>
    /// </item>
    /// <item>
    /// <term>interval(6) or interval</term>
    /// <description>resolution of 1&#xb5;s = 0.000001s (1 microsecond)</description>
    /// </item>
    /// </list>
    /// <para>As such, if the 100-nanosecond resolution is significant to an application, a PostgreSQL interval will
    /// not suffice for those purposes.</para>
    /// <para>In more frequent cases though, the resolution of the interval suffices.
    /// <see cref="NpgsqlInterval"/> will always suffice to handle the resolution of any interval value, and upon
    /// writing to the database, will be rounded to the resolution used.</para>
    /// </remarks>
    /// <returns>The number of ticks in the instance.</returns>
    /// </summary>
    public long Ticks
    {
        get
        {
            return _ticks;
        }
    }
    /// <summary>
    /// Gets the number of whole microseconds held in the instance.
    /// <returns>An  in the range [-999999, 999999].</returns>
    /// </summary>
    public int Microseconds
    {
        get
        {
            return (int)(_ticks / 10) % 1000000;
        }
    }
    /// <summary>
    /// Gets the number of whole milliseconds held in the instance.
    /// <returns>An  in the range [-999, 999].</returns>
    /// </summary>
    public int Milliseconds
    {
        get
        {
            return (int)((_ticks / TicksPerMillsecond) % 1000);
        }
    }
    /// <summary>
    /// Gets the number of whole seconds held in the instance.
    /// <returns>An  in the range [-59, 59].</returns>
    /// </summary>
    public int Seconds
    {
        get
        {
            return (int)((_ticks / TicksPerSecond) % 60);
        }
    }
    /// <summary>
    /// Gets the number of whole minutes held in the instance.
    /// <returns>An  in the range [-59, 59].</returns>
    /// </summary>
    public int Minutes
    {
        get
        {
            return (int)((_ticks / TicksPerMinute) % 60);
        }
    }
    /// <summary>
    /// Gets the number of whole hours held in the instance.
    /// <remarks>Note that this can be less than -23 or greater than 23 unless <see cref="JustifyDays()"/>
    /// has been used to produce this instance.</remarks>
    /// </summary>
    public int Hours
    {
        get
        {
            return (int)(_ticks / TicksPerHour);
        }
    }
    /// <summary>
    /// Gets the number of days held in the instance.
    /// <remarks>Note that this does not pay attention to a time component with -24 or less hours or
    /// 24 or more hours, unless <see cref="JustifyDays()"/> has been called to produce this instance.</remarks>
    /// </summary>
    public int Days
    {
        get
        {
            return _days;
        }
    }
    /// <summary>
    /// Gets the number of months held in the instance.
    /// <remarks>Note that this does not pay attention to a day component with -30 or less days or
    /// 30 or more days, unless <see cref="JustifyMonths()"/> has been called to produce this instance.</remarks>
    /// </summary>
    public int Months
    {
        get
        {
            return _months;
        }
    }
    /// <summary>
    /// Returns a <see cref="TimeSpan"/> representing the time component of the instance.
    /// <remarks>Note that this may have a value beyond the range &#xb1;23:59:59.9999999 unless
    /// <see cref="JustifyDays()"/> has been called to produce this instance.</remarks>
    /// </summary>
    public TimeSpan Time
    {
        get
        {
            return new TimeSpan(_ticks);
        }
    }
    #endregion
    #region Total Parts
    /// <summary>
    /// The total number of ticks (100ns units) in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public long TotalTicks
    {
        get
        {
            return Ticks + Days * TicksPerDay + Months * TicksPerMonth;
        }
    }
    /// <summary>
    /// The total number of microseconds in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalMicroseconds
    {
        get
        {
            return TotalTicks / 10d;
        }
    }
    /// <summary>
    /// The total number of milliseconds in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalMilliseconds
    {
        get
        {
            return TotalTicks / (double)TicksPerMillsecond;
        }
    }
    /// <summary>
    /// The total number of seconds in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalSeconds
    {
        get
        {
            return TotalTicks / (double)TicksPerSecond;
        }
    }
    /// <summary>
    /// The total number of minutes in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalMinutes
    {
        get
        {
            return TotalTicks / (double)TicksPerMinute;
        }
    }
    /// <summary>
    /// The total number of hours in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalHours
    {
        get
        {
            return TotalTicks / (double)TicksPerHour;
        }
    }
    /// <summary>
    /// The total number of days in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalDays
    {
        get
        {
            return TotalTicks / (double)TicksPerDay;
        }
    }
    /// <summary>
    /// The total number of months in the instance, assuming 24 hours in each day and
    /// 30 days in a month.
    /// </summary>
    public double TotalMonths
    {
        get
        {
            return TotalTicks / (double)TicksPerMonth;
        }
    }
    #endregion
    #region Create From Part
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of ticks.
    /// </summary>
    /// <param name="ticks">The number of ticks (100ns units) in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of ticks.</returns>
    public static NpgsqlInterval FromTicks(long ticks)
    {
        return new NpgsqlInterval(ticks).Canonicalize();
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of microseconds.
    /// </summary>
    /// <param name="ticks">The number of microseconds in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of microseconds.</returns>
    public static NpgsqlInterval FromMicroseconds(double micro)
    {
        return FromTicks((long)(micro * TicksPerMicrosecond));
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of milliseconds.
    /// </summary>
    /// <param name="ticks">The number of milliseconds in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of milliseconds.</returns>
    public static NpgsqlInterval FromMilliseconds(double milli)
    {
        return FromTicks((long)(milli * TicksPerMillsecond));
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of seconds.
    /// </summary>
    /// <param name="ticks">The number of seconds in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of seconds.</returns>
    public static NpgsqlInterval FromSeconds(double seconds)
    {
        return FromTicks((long)(seconds * TicksPerSecond));
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of minutes.
    /// </summary>
    /// <param name="ticks">The number of minutes in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of minutes.</returns>
    public static NpgsqlInterval FromMinutes(double minutes)
    {
        return FromTicks((long)(minutes * TicksPerMinute));
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of hours.
    /// </summary>
    /// <param name="ticks">The number of hours in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of hours.</returns>
    public static NpgsqlInterval FromHours(double hours)
    {
        return FromTicks((long)(hours * TicksPerHour));
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of days.
    /// </summary>
    /// <param name="ticks">The number of days in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of days.</returns>
    public static NpgsqlInterval FromDays(double days)
    {
        return FromTicks((long)(days * TicksPerDay));
    }
    /// <summary>
    /// Creates an <see cref="NpgsqlInterval"/> from a number of months.
    /// </summary>
    /// <param name="ticks">The number of months in the interval.</param>
    /// <returns>A <see cref="Canonicalize()"/>d <see cref="NpgsqlInterval"/> with the given number of months.</returns>
    public static NpgsqlInterval FromMonths(double months)
    {
        return FromTicks((long)(months * TicksPerMonth));
    }
    #endregion
    #region Arithmetic
    /// <summary>
    /// Adds another interval to this instance and returns the result.
    /// </summary>
    /// <param name="interval">An <see cref="NpgsqlInterval"/> to add to this instance.</param>
    /// <returns>An <see cref="NpgsqlInterval"></see> whose values are the sums of the two instances.</returns>
    public NpgsqlInterval Add(NpgsqlInterval interval)
    {
        return new NpgsqlInterval(Months + interval.Months, Days + interval.Days, Ticks + interval.Ticks);
    }
    /// <summary>
    /// Subtracts another interval from this instance and returns the result.
    /// </summary>
    /// <param name="interval">An <see cref="NpgsqlInterval"/> to subtract from this instance.</param>
    /// <returns>An <see cref="NpgsqlInterval"></see> whose values are the differences of the two instances.</returns>
    public NpgsqlInterval Subtract(NpgsqlInterval interval)
    {
        return new NpgsqlInterval(Months - interval.Months, Days - interval.Days, Ticks - interval.Ticks);
    }
    /// <summary>
    /// Returns an <see cref="NpgsqlInterval"/> whose value is the negated value of this instance.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> whose value is the negated value of this instance.</returns>
    public NpgsqlInterval Negate()
    {
        return new NpgsqlInterval(-Months, -Days, -Ticks);
    }
    /// <summary>
    /// This absolute value of this instance. In the case of some, but not all, components being negative,
    /// the rules used for justification are used to determine if the instance is positive or negative.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> whose value is the absolute value of this instance.</returns>
    public NpgsqlInterval Duration()
    {
        return UnjustifyInterval().Ticks < 0
            ? Negate()
            : this;
    }
    #endregion
    #region Justification
    /// <summary>
    /// Equivalent to PostgreSQL's justify_days function.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one, but with any hours outside of the range [-23, 23]
    /// converted into days.</returns>
    public NpgsqlInterval JustifyDays()
    {
        return new NpgsqlInterval(Months, Days + (int)(Ticks / TicksPerDay), Ticks % TicksPerDay);
    }
    /// <summary>
    /// Opposite to PostgreSQL's justify_days function.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one, but with any days converted to multiples of &#xB1;24hours.</returns>
    public NpgsqlInterval UnjustifyDays()
    {
        return new NpgsqlInterval(Months, 0, Ticks + Days * TicksPerDay);
    }
    /// <summary>
    /// Equivalent to PostgreSQL's justify_months function.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one, but with any days outside of the range [-30, 30]
    /// converted into months.</returns>
    public NpgsqlInterval JustifyMonths()
    {
        return new NpgsqlInterval(Months + Days / DaysPerMonth, Days % DaysPerMonth, Ticks);
    }
    /// <summary>
    /// Opposite to PostgreSQL's justify_months function.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one, but with any months converted to multiples of &#xB1;30days.</returns>
    public NpgsqlInterval UnjustifyMonths()
    {
        return new NpgsqlInterval(0, Days + Months * DaysPerMonth, Ticks);
    }
    /// <summary>
    /// Equivalent to PostgreSQL's justify_interval function.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one,
    /// but with any months converted to multiples of &#xB1;30days
    /// and then with any days converted to multiples of &#xB1;24hours</returns>
    public NpgsqlInterval JustifyInterval()
    {
        return JustifyMonths().JustifyDays();
    }
    /// <summary>
    /// Opposite to PostgreSQL's justify_interval function.
    /// </summary>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one, but with any months converted to multiples of &#xB1;30days and then any days converted to multiples of &#xB1;24hours;</returns>
    public NpgsqlInterval UnjustifyInterval()
    {
        return new NpgsqlInterval(Ticks + Days * TicksPerDay + Months * DaysPerMonth * TicksPerDay);
    }
    /// <summary>
    /// Produces a canonical NpgslInterval with 0 months and hours in the range of [-23, 23].
    /// <remarks>
    /// <para>
    /// While the fact that for many purposes, two different <see cref="NpgsqlInterval"/> instances could be considered
    /// equivalent (e.g. one with 2days, 3hours and one with 1day 27hours) there are different possible canonical forms.
    /// </para><para>
    /// E.g. we could move all excess hours into days and all excess days into months and have the most readable form,
    /// or we could move everything into the ticks and have the form that allows for the easiest arithmetic) the form
    /// chosen has two important properties that make it the best choice.
    /// </para><para>First, it is closest two how
    /// <see cref="TimeSpan"/> objects are most often represented. Second, it is compatible with results of many
    /// PostgreSQL functions, particularly with age() and the results of subtracting one date, time or timestamp from
    /// another.
    /// </para>
    /// <para>Note that the results of casting a <see cref="TimeSpan"/> to <see cref="NpgsqlInterval"/> is
    /// canonicalised.</para>
    /// </remarks>
    /// <returns>An <see cref="NpgsqlInterval"/> based on this one, but with months converted to multiples of &#xB1;30days and with any hours outside of the range [-23, 23]
    /// converted into days.</return>
    public NpgsqlInterval Canonicalize()
    {
        return new NpgsqlInterval(0, Days + Months * DaysPerMonth + (int)(Ticks / TicksPerDay), Ticks % TicksPerDay);
    }
    #endregion
    #region Casts
    /// <summary>
    /// Implicit cast of a <see cref="TimeSpan"/> to an <see cref="NpgsqlInterval"/>
    /// </summary>
    /// <param name="timespan">A <see cref="TimeSpan"/></param>
    /// <returns>An eqivalent, canonical, <see cref="NpgsqlInterval"/>.</returns>
    public static implicit operator NpgsqlInterval(TimeSpan timespan)
    {
        return new NpgsqlInterval(timespan).Canonicalize();
    }
    /// <summary>
    /// Implicit cast of an <see cref="NpgsqlInterval"/> to a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="interval">A <see cref="NpgsqlInterval"/>.</param>
    /// <returns>An equivalent <see cref="TimeSpan"/>.</returns>
    public static explicit operator TimeSpan(NpgsqlInterval interval)
    {
        return new TimeSpan(interval.Ticks + interval.Days * TicksPerDay + interval.Months * DaysPerMonth * TicksPerDay);
    }
    #endregion
    #region Comparison
    /// <summary>
    /// Returns true if another <see cref="NpgsqlInterval"/> is exactly the same as this instance.
    /// </summary>
    /// <param name="other">An <see cref="NpgsqlInterval"/> for comparison.</param>
    /// <returns>true if the two <see cref="NpgsqlInterval"/> instances are exactly the same,
    /// false otherwise.</returns>
    public bool Equals(NpgsqlInterval other)
    {
        return Ticks == other.Ticks && Days == other.Days && Months == other.Months;
    }
    /// <summary>
    /// Returns true if another object is an <see cref="NpgsqlInterval"/>, that is exactly the same as
    /// this instance
    /// </summary>
    /// <param name="obj">An <see cref="Object"/> for comparison.</param>
    /// <returns>true if the argument is an <see cref="NpgsqlInterval"/> and is exactly the same
    /// as this one, false otherwise.</returns>
    public override bool Equals(object obj)
    {
        if(obj == null)
            return false;
        if(obj is NpgsqlInterval)
            return Equals((NpgsqlInterval)obj);
        return false;
    }
    /// <summary>
    /// Compares two <see cref="NpgsqlInterval"/> instances.
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/>.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/>.</param>
    /// <returns>0 if the two are equal or equivalent. A value greater than zero if x is greater than y,
    /// a value less than zero if x is less than y.</returns>
    public static int Compare(NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.CompareTo(y);
    }
    int IComparer<NpgsqlInterval>.Compare(NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.CompareTo(y);
    }
    int IComparer.Compare(object x, object y)
    {
        if(x == null)
            return y == null ? 0 : 1;
        if(y == null)
            return -1;
        try
        {
            return ((IComparable)x).CompareTo(y);
        }
        catch(Exception)
        {
            throw new ArgumentException();
        }
    }
    /// <summary>
    /// A hash code suitable for uses with hashing algorithms.
    /// </summary>
    /// <returns>An signed integer.</returns>
    public override int GetHashCode()
    {
        return UnjustifyInterval().Ticks.GetHashCode();
    }
    /// <summary>
    /// Compares this instance with another/
    /// </summary>
    /// <param name="other">An <see cref="NpgsqlInterval"/> to compare this with.</param>
    /// <returns>0 if the instances are equal or equivalent. A value less than zero if
    /// this instance is less than the argument. A value greater than zero if this instance
    /// is greater than the instance.</returns>
    public int CompareTo(NpgsqlInterval other)
    {
        return UnjustifyInterval().Ticks.CompareTo(other.UnjustifyInterval().Ticks);
    }
    /// <summary>
    /// Compares this instance with another/
    /// </summary>
    /// <param name="other">An object to compare this with.</param>
    /// <returns>0 if the argument is an <see cref="NpgsqlInterval"/> and the instances are equal or equivalent.
    /// A value less than zero if the argument is an <see cref="NpgsqlInterval"/> and
    /// this instance is less than the argument.
    /// A value greater than zero if the argument is an <see cref="NpgsqlInterval"/> and this instance
    /// is greater than the instance.</returns>
    /// A value greater than zero if the argument is null.
    /// <exception cref="ArgumentException">The argument is not an <see cref="NpgsqlInterval"/>.</exception>
    public int CompareTo(object other)
    {
        if(other == null)
            return 1;
        else if(other is NpgsqlInterval)
            return CompareTo((NpgsqlInterval)other);
        else
            throw new ArgumentException();
    }
    public NpgsqlInterval Clone()
    {
        return new NpgsqlInterval(_months, _days, _ticks);
    }
    object ICloneable.Clone()
    {
        return Clone();
    }
    #endregion
    #region To And From Strings
    /// <summary>
    /// Parses a <see cref="String"/> and returns a <see cref="NpgsqlInterval"/> instance.
    /// Designed to use the formats generally returned by PostgreSQL.
    /// </summary>
    /// <param name="str">The <see cref="String"/> to parse.</param>
    /// <returns>An <see cref="NpgsqlInterval"/> represented by the argument.</returns>
    /// <exception cref="ArgumentNullException">The string was null.</exception>
    /// <exception cref="OverflowException">A value obtained from parsing the string exceeded the values allowed for the relevant component.</exception>
    /// <exception cref="FormatException">The string was not in a format that could be parsed to produce an <see cref="NpgsqlInterval"/>.</exception>
    public static NpgsqlInterval Parse(string str)
    {
        if(str == null)
            throw new ArgumentNullException("str");
        str = str.Replace('s', ' ');//Quick and easy way to catch plurals.
        try
        {
            int years = 0;
            int months = 0;
            int days = 0;
            int hours = 0;
            int minutes = 0;
            decimal seconds = 0m;
            int idx = str.IndexOf("year");
            if(idx > 0)
            {
                years = int.Parse(str.Substring(0, idx));
                str = str.Substring(idx + 5);
            }
            idx = str.IndexOf("mon");
            if(idx > 0)
            {
                months = int.Parse(str.Substring(0, idx));
                str = str.Substring(idx + 4);
            }
            idx = str.IndexOf("day");
            if(idx > 0)
            {
                days = int.Parse(str.Substring(0, idx));
                str = str.Substring(idx + 4).Trim();
            }
            if(str.Length > 0)
            {
                string[] parts = str.Split(':');
                switch(parts.Length)//One of those times that fall-through would actually be good.
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
                        seconds = decimal.Parse(parts[2]);
                        break;
                }
            }
            long ticks = hours * TicksPerHour + minutes * TicksPerMinute + (long)(seconds * TicksPerSecond);
            return new NpgsqlInterval(years * MonthsPerYear + months, days, ticks);
        }
        catch(OverflowException)
        {
            throw;
        }
        catch(Exception)
        {
            throw new FormatException();
        }
    }
    /// <summary>
    /// Attempt to parse a <see cref="String"/> to produce an <see cref="NpgsqlInterval"/>.
    /// </summary>
    /// <param name="str">The <see cref="String"/> to parse.</param>
    /// <param name="result">(out) The <see cref="NpgsqlInterval"/> produced, or <see cref="Zero"/> if the parsing failed.</param>
    /// <returns>true if the parsing succeeded, false otherwise.</returns>
    public static bool TryParse(string str, out NpgsqlInterval result)
    {
        try
        {
            result = Parse(str);
            return true;
        }
        catch(Exception)
        {
            result = Zero;
            return false;
        }
    }
    /// <summary>
    /// Create a <see cref="String"/> representation of the <see cref="NpgsqlInterval"/> instance.
    /// The format returned is of the form:
    /// [M mon[s]] [d day[s]] [HH:mm:ss[.f[f[f[f[f[f[f[f[f]]]]]]]]]]
    /// A zero <see cref="NpgsqlInterval"/> is represented as 00:00:00
    /// <remarks>
    /// Ticks are 100ns, Postgress resolution is only to 1&#xb5;s at most. Hence we lose 1 or more decimal
    /// precision in storing values in the database. Despite this, this method will output that extra
    /// digit of precision. It's forward-compatible with any future increases in resolution up to 100ns,
    /// and also makes this ToString() more applicable to any other use-case.
    /// </remarks>
    /// </summary>
    /// <returns>The <see cref="String"/> representation.</returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        if(Months != 0)
            sb.Append(Months).Append(Math.Abs(Months) == 1 ? " mon " : " mons ");
        if(Days != 0)
        {
            if(Months < 0 && Days > 0)
                sb.Append('+');
            sb.Append(Days).Append(Math.Abs(Days) == 1 ? " day " : " days ");
        }
        if(Ticks != 0 || sb.Length == 0)
        {
            if(Days < 0 || (Days == 0 && Months < 0))
                sb.Append('+');
            TimeSpan time = Time;
            sb.Append(time.Hours.ToString("D2")).Append(':').Append(time.Minutes.ToString("D2")).Append(':').Append(time.Seconds.ToString("D2"));
            long remainingTicks = Math.Abs(Ticks) % TicksPerSecond;
            if(remainingTicks != 0)
            {
                while(remainingTicks % 10 == 0)
                    remainingTicks /= 10;
                sb.Append('.').Append(remainingTicks);
            }
        }
        if(sb[sb.Length - 1] == ' ')
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
    #endregion
    #region Common Operators
    /// <summary>
    /// Adds two <see cref="NpgsqlInterval"/> together.
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to add.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to add.</param>
    /// <returns>An <see cref="NpgsqlInterval"/> whose values are the sum of the arguments.</returns>
    public static NpgsqlInterval operator + (NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.Add(y);
    }
    /// <summary>
    /// Subtracts one <see cref="NpgsqlInterval"/> from another.
    /// </summary>
    /// <param name="x">The <see cref="NpgsqlInterval"/> to subtract the other from.</param>
    /// <param name="y">The <see cref="NpgsqlInterval"/> to subtract from the other.</param>
    /// <returns>An <see cref="NpgsqlInterval"/> whose values are the difference of the arguments</returns>
    public static NpgsqlInterval operator - (NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.Subtract(y);
    }
    /// <summary>
    /// Returns true if two <see cref="NpgsqlInterval"/> are exactly the same.
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to compare.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to compare.</param>
    /// <returns>true if the two arguments are exactly the same, false otherwise.</returns>
    public static bool operator == (NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.Equals(y);
    }
    /// <summary>
    /// Returns false if two <see cref="NpgsqlInterval"/> are exactly the same.
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to compare.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to compare.</param>
    /// <returns>false if the two arguments are exactly the same, true otherwise.</returns>
    public static bool operator != (NpgsqlInterval x, NpgsqlInterval y)
    {
        return !(x == y);
    }
    /// <summary>
    /// Compares two <see cref="NpgsqlInterval"/> instances to see if the first is less than the second
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to compare.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to compare.</param>
    /// <returns>true if the first <see cref="NpgsqlInterval"/> is less than second, false otherwise.</returns>
    public static bool operator < (NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.UnjustifyInterval().Ticks < y.UnjustifyInterval().Ticks;
    }
    /// <summary>
    /// Compares two <see cref="NpgsqlInterval"/> instances to see if the first is less than or equivalent to the second
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to compare.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to compare.</param>
    /// <returns>true if the first <see cref="NpgsqlInterval"/> is less than or equivalent to second, false otherwise.</returns>
    public static bool operator <= (NpgsqlInterval x, NpgsqlInterval y)
    {
        return x.UnjustifyInterval().Ticks <= y.UnjustifyInterval().Ticks;
    }
    /// <summary>
    /// Compares two <see cref="NpgsqlInterval"/> instances to see if the first is greater than the second
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to compare.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to compare.</param>
    /// <returns>true if the first <see cref="NpgsqlInterval"/> is greater than second, false otherwise.</returns>
    public static bool operator > (NpgsqlInterval x, NpgsqlInterval y)
    {
        return !(x <= y);
    }
    /// <summary>
    /// Compares two <see cref="NpgsqlInterval"/> instances to see if the first is greater than or equivalent the second
    /// </summary>
    /// <param name="x">The first <see cref="NpgsqlInterval"/> to compare.</param>
    /// <param name="y">The second <see cref="NpgsqlInterval"/> to compare.</param>
    /// <returns>true if the first <see cref="NpgsqlInterval"/> is greater than or equivalent to the second, false otherwise.</returns>
    public static bool operator >= (NpgsqlInterval x, NpgsqlInterval y)
    {
        return !(x < y);
    }
    /// <summary>
    /// Returns the instance.
    /// </summary>
    /// <param name="x">An <see cref="NpgsqlInterval"/>.</param>
    /// <returns>The argument.</returns>
    public static NpgsqlInterval operator + (NpgsqlInterval x)
    {
        return x;
    }
    /// <summary>
    /// Negates an <see cref="NpgsqlInterval"/> instance.
    /// </summary>
    /// <param name="x">An <see cref="NpgsqlInterval"/>.</param>
    /// <returns>The negation of the argument.</returns>
    public static NpgsqlInterval operator - (NpgsqlInterval x)
    {
        return x.Negate();
    }
    #endregion
}
