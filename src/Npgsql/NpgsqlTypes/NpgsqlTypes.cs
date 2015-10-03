#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;

#pragma warning disable 1591

namespace NpgsqlTypes
{
    /// <summary>
    /// Represents a PostgreSQL point type.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    public struct NpgsqlPoint : IEquatable<NpgsqlPoint>
    {
        static readonly Regex Regex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\)");

        public double X { get; set; }
        public double Y { get; set; }

        public NpgsqlPoint(double x, double y)
            : this()
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
            return obj is NpgsqlPoint && Equals((NpgsqlPoint) obj);
        }

        public static bool operator ==(NpgsqlPoint x, NpgsqlPoint y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlPoint x, NpgsqlPoint y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ PGUtil.RotateShift(Y.GetHashCode(), sizeof (int)/2);
        }

        public static NpgsqlPoint Parse(string s)
        {
            var m = Regex.Match(s);
            if (!m.Success) {
                throw new FormatException("Not a valid point: " + s);
            }
            return new NpgsqlPoint(Double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                   Double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat));
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "({0},{1})", X, Y);
        }
    }

    /// <summary>
    /// Represents a PostgreSQL line type.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    public struct NpgsqlLine : IEquatable<NpgsqlLine>
    {
        static readonly Regex Regex = new Regex(@"\{(-?\d+.?\d*),(-?\d+.?\d*),(-?\d+.?\d*)\}");

        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public NpgsqlLine(double a, double b, double c)
            : this()
        {
            A = a;
            B = b;
            C = c;
        }

        public static NpgsqlLine Parse(string s)
        {
            var m = Regex.Match(s);
            if (!m.Success) {
                throw new FormatException("Not a valid line: " + s);
            }
            return new NpgsqlLine(
                Double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)
            );
        }

        public override String ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2}}}", A, B, C);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() * B.GetHashCode() * C.GetHashCode();
        }

        public bool Equals(NpgsqlLine other)
        {
            return A == other.A && B == other.B && C == other.C;
        }

        public override bool Equals(object obj)
        {
            return obj is NpgsqlLine && Equals((NpgsqlLine)obj);
        }

        public static bool operator ==(NpgsqlLine x, NpgsqlLine y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlLine x, NpgsqlLine y)
        {
            return !(x == y);
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Line Segment type.
    /// </summary>
    public struct NpgsqlLSeg : IEquatable<NpgsqlLSeg>
    {
        static readonly Regex Regex = new Regex(@"\[\((-?\d+.?\d*),(-?\d+.?\d*)\),\((-?\d+.?\d*),(-?\d+.?\d*)\)\]");

        public NpgsqlPoint Start { get; set; }
        public NpgsqlPoint End { get; set; }

        public NpgsqlLSeg(NpgsqlPoint start, NpgsqlPoint end)
            : this()
        {
            Start = start;
            End = end;
        }

        public NpgsqlLSeg(double startx, double starty, double endx, double endy) : this()
        {
            Start = new NpgsqlPoint(startx, starty);
            End   = new NpgsqlPoint(endx,   endy);
        }

        public static NpgsqlLSeg Parse(string s)
        {
            var m = Regex.Match(s);
            if (!m.Success) {
                throw new FormatException("Not a valid line: " + s);
            }
            return new NpgsqlLSeg(
                Double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)
            );

        }

        public override String ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "[{0},{1}]", Start, End);
        }

        public override int GetHashCode()
        {
            return
                Start.X.GetHashCode() ^ PGUtil.RotateShift(Start.Y.GetHashCode(), sizeof(int) / 4) ^
                PGUtil.RotateShift(End.X.GetHashCode(), sizeof(int) / 2) ^ PGUtil.RotateShift(End.Y.GetHashCode(), sizeof(int) * 3 / 4);
        }

        public bool Equals(NpgsqlLSeg other)
        {
            return Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj)
        {
            return obj is NpgsqlLSeg && Equals((NpgsqlLSeg)obj);
        }

        public static bool operator ==(NpgsqlLSeg x, NpgsqlLSeg y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlLSeg x, NpgsqlLSeg y)
        {
            return !(x == y);
        }
    }

    /// <summary>
    /// Represents a PostgreSQL box type.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    public struct NpgsqlBox : IEquatable<NpgsqlBox>
    {
        static readonly Regex Regex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\),\((-?\d+.?\d*),(-?\d+.?\d*)\)");

        public NpgsqlPoint UpperRight { get; set; }
        public NpgsqlPoint LowerLeft { get; set; }

        public NpgsqlBox(NpgsqlPoint upperRight, NpgsqlPoint lowerLeft) : this()
        {
            UpperRight = upperRight;
            LowerLeft = lowerLeft;
        }

        public NpgsqlBox(double top, double right, double bottom, double left)
            : this(new NpgsqlPoint(right, top), new NpgsqlPoint(left, bottom)) { }

        public double Left => LowerLeft.X;
        public double Right => UpperRight.X;
        public double Bottom => LowerLeft.Y;
        public double Top => UpperRight.Y;
        public double Width => Right - Left;
        public double Height => Top - Bottom;

        public bool IsEmpty => Width == 0 || Height == 0;

        public bool Equals(NpgsqlBox other)
        {
            return UpperRight == other.UpperRight && LowerLeft == other.LowerLeft;
        }

        public override bool Equals(object obj)
        {
            return obj is NpgsqlBox && Equals((NpgsqlBox) obj);
        }

        public static bool operator ==(NpgsqlBox x, NpgsqlBox y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlBox x, NpgsqlBox y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0},{1}", UpperRight, LowerLeft);
        }

        public static NpgsqlBox Parse(string s)
        {
            var m = Regex.Match(s);
            return new NpgsqlBox(
                new NpgsqlPoint(Double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                Double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
                new NpgsqlPoint(Double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                                Double.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat))
            );
        }

        public override int GetHashCode()
        {
            return
                Top.GetHashCode() ^ PGUtil.RotateShift(Right.GetHashCode(), sizeof (int)/4) ^
                PGUtil.RotateShift(Bottom.GetHashCode(), sizeof (int)/2) ^
                PGUtil.RotateShift(LowerLeft.GetHashCode(), sizeof (int)*3/4);
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Path type.
    /// </summary>
    public struct NpgsqlPath : IList<NpgsqlPoint>, IEquatable<NpgsqlPath>
    {
        readonly List<NpgsqlPoint> _points;
        public bool Open { get; set; }

        public NpgsqlPath(IEnumerable<NpgsqlPoint> points, bool open) : this()
        {
            _points = new List<NpgsqlPoint>(points);
            Open = open;
        }

        public NpgsqlPath(IEnumerable<NpgsqlPoint> points) : this(points, false) {}
        public NpgsqlPath(params NpgsqlPoint[] points) : this(points, false) {}

        public NpgsqlPath(bool open) : this()
        {
            _points = new List<NpgsqlPoint>();
            Open = open;
        }

        public NpgsqlPath(int capacity, bool open) : this()
        {
            _points = new List<NpgsqlPoint>(capacity);
            Open = open;
        }

        public NpgsqlPath(int capacity) : this(capacity, false) {}

        public NpgsqlPoint this[int index]
        {
            get { return _points[index]; }
            set { _points[index] = value; }
        }

        public int Capacity => _points.Capacity;
        public int Count => _points.Count;
        public bool IsReadOnly => false;

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
            if (Open != other.Open || Count != other.Count)
                return false;
            else if(ReferenceEquals(_points, other._points))//Short cut for shallow copies.
                return true;
            for (int i = 0; i != Count; ++i)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlPath && Equals((NpgsqlPath) obj);
        }

        public static bool operator ==(NpgsqlPath x, NpgsqlPath y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlPath x, NpgsqlPath y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            foreach (NpgsqlPoint point in this)
            {
                //The ideal amount to shift each value is one that would evenly spread it throughout
                //the resultant bytes. Using the current result % 32 is essentially using a random value
                //but one that will be the same on subsequent calls.
                ret ^= PGUtil.RotateShift(point.GetHashCode(), ret%sizeof (int));
            }
            return Open ? ret : -ret;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Open ? '[' : '(');
            int i;
            for (i = 0; i < _points.Count; i++)
            {
                var p = _points[i];
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0},{1})", p.X, p.Y);
                if (i < _points.Count - 1) {
                    sb.Append(",");
                }
            }
            sb.Append(Open ? ']' : ')');
            return sb.ToString();
        }

        public static NpgsqlPath Parse(string s)
        {
            bool open;
            switch (s[0])
            {
                case '[':
                    open = true;
                    break;
                case '(':
                    open = false;
                    break;
                default:
                    throw new Exception("Invalid path string: " + s);
            }
            Contract.Assume(s[s.Length - 1] == (open ? ']' : ')'));
            var result = new NpgsqlPath(open);
            var i = 1;
            while (true)
            {
                var i2 = s.IndexOf(')', i);
                result.Add(NpgsqlPoint.Parse(s.Substring(i, i2 - i + 1)));
                if (s[i2 + 1] != ',')
                    break;
                i = i2 + 2;
            }
            return result;
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

        public NpgsqlPolygon(params NpgsqlPoint[] points) : this ((IEnumerable<NpgsqlPoint>) points) {}

        public NpgsqlPolygon(int capacity)
        {
            _points = new List<NpgsqlPoint>(capacity);
        }

        public NpgsqlPoint this[int index]
        {
            get { return _points[index]; }
            set { _points[index] = value; }
        }

        public int Capacity => _points.Capacity;
        public int Count => _points.Count;
        public bool IsReadOnly => false;

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
            if (Count != other.Count)
                return false;
            if (ReferenceEquals(_points, other._points))
                return true;
            for (int i = 0; i != Count; ++i)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is NpgsqlPolygon && Equals((NpgsqlPolygon) obj);
        }

        public static bool operator ==(NpgsqlPolygon x, NpgsqlPolygon y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlPolygon x, NpgsqlPolygon y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            int ret = 266370105;//seed with something other than zero to make paths of all zeros hash differently.
            foreach (NpgsqlPoint point in this)
            {
                //The ideal amount to shift each value is one that would evenly spread it throughout
                //the resultant bytes. Using the current result % 32 is essentially using a random value
                //but one that will be the same on subsequent calls.
                ret ^= PGUtil.RotateShift(point.GetHashCode(), ret%sizeof (int));
            }
            return ret;
        }

        public static NpgsqlPolygon Parse(string s)
        {
            var points = new List<NpgsqlPoint>();
            var i = 1;
            while (true)
            {
                var i2 = s.IndexOf(')', i);
                points.Add(NpgsqlPoint.Parse(s.Substring(i, i2 - i + 1)));
                if (s[i2 + 1] != ',')
                    break;
                i = i2 + 2;
            }
            return new NpgsqlPolygon(points);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('(');
            int i;
            for (i = 0; i < _points.Count; i++)
            {
                var p = _points[i];
                sb.AppendFormat(CultureInfo.InvariantCulture, "({0},{1})", p.X, p.Y);
                if (i < _points.Count - 1) {
                    sb.Append(",");
                }
            }
            sb.Append(')');
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Circle type.
    /// </summary>
    public struct NpgsqlCircle : IEquatable<NpgsqlCircle>
    {
        static readonly Regex Regex = new Regex(@"<\((-?\d+.?\d*),(-?\d+.?\d*)\),(\d+.?\d*)>");

        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }

        public NpgsqlCircle(NpgsqlPoint center, double radius)
            : this()
        {
            X = center.X;
            Y = center.Y;
            Radius = radius;
        }

        public NpgsqlCircle(double x, double y, double radius) : this()
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public NpgsqlPoint Center
        {
            get { return new NpgsqlPoint(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public bool Equals(NpgsqlCircle other)
        {
            return X == other.X && Y == other.Y && Radius == other.Radius;
        }

        public override bool Equals(object obj)
        {
            return obj is NpgsqlCircle && Equals((NpgsqlCircle) obj);
        }

        public static NpgsqlCircle Parse(string s)
        {
            var m = Regex.Match(s);
            if (!m.Success) {
                throw new FormatException("Not a valid circle: " + s);
            }

            return new NpgsqlCircle(
                Double.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
                Double.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)
            );
        }

        public override String ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "<({0},{1}),{2}>", X, Y, Radius);
        }

        public static bool operator ==(NpgsqlCircle x, NpgsqlCircle y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlCircle x, NpgsqlCircle y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * Y.GetHashCode() * Radius.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a PostgreSQL inet type, which is a combination of an IPAddress and a
    /// subnet mask.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    public struct NpgsqlInet : IEquatable<NpgsqlInet>
    {
        public IPAddress Address;
        public int Netmask;

        public NpgsqlInet(IPAddress address, int netmask)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork && address.AddressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException("Only IPAddress of InterNetwork or InterNetworkV6 address families are accepted", nameof(address));
            Contract.EndContractBlock();

            Address = address;
            Netmask = netmask;
        }

        public NpgsqlInet(IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork && address.AddressFamily != AddressFamily.InterNetworkV6)
                throw new ArgumentException("Only IPAddress of InterNetwork or InterNetworkV6 address families are accepted", nameof(address));
            Contract.EndContractBlock();

            Address = address;
            Netmask = address.AddressFamily == AddressFamily.InterNetwork ? 32 : 128;
        }

        public NpgsqlInet(string addr)
        {
            if (addr.IndexOf('/') > 0)
            {
                var addrbits = addr.Split('/');
                if (addrbits.GetUpperBound(0) != 1) {
                    throw new FormatException("Invalid number of parts in CIDR specification");
                }
                Address = IPAddress.Parse(addrbits[0]);
                Netmask = int.Parse(addrbits[1]);
            }
            else
            {
                Address = IPAddress.Parse(addr);
                Netmask = 32;
            }
        }

        public override string ToString()
        {
            if ((Address.AddressFamily == AddressFamily.InterNetwork   && Netmask == 32) ||
                (Address.AddressFamily == AddressFamily.InterNetworkV6 && Netmask == 128))
            {
                return Address.ToString();
            }
            return $"{Address}/{Netmask}";
        }

        public static explicit operator IPAddress(NpgsqlInet x)
        {
            if (x.Netmask != 32) {
                throw new InvalidCastException("Cannot cast CIDR network to address");
            }
            return x.Address;
        }

        public static implicit operator NpgsqlInet(IPAddress ipaddress)
        {
            return ReferenceEquals(ipaddress, null)
                ? default(NpgsqlInet)
                : new NpgsqlInet(ipaddress);
        }

        public bool Equals(NpgsqlInet other)
        {
            return Address.Equals(other.Address) && Netmask == other.Netmask;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NpgsqlInet && Equals((NpgsqlInet) obj);
        }

        public override int GetHashCode()
        {
            return PGUtil.RotateShift(Address.GetHashCode(), Netmask%32);
        }

        public static bool operator ==(NpgsqlInet x, NpgsqlInet y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(NpgsqlInet x, NpgsqlInet y)
        {
            return !(x == y);
        }
    }
}

#pragma warning restore 1591
