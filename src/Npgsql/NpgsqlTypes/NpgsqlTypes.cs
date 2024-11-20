using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

#pragma warning disable 1591

// ReSharper disable once CheckNamespace
namespace NpgsqlTypes;

/// <summary>
/// Represents a PostgreSQL point type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-geometric.html
/// </remarks>
public struct NpgsqlPoint(double x, double y) : IEquatable<NpgsqlPoint>
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;

    // ReSharper disable CompareOfFloatsByEqualityOperator
    public bool Equals(NpgsqlPoint other) => X == other.X && Y == other.Y;
    // ReSharper restore CompareOfFloatsByEqualityOperator

    public override bool Equals(object? obj)
        => obj is NpgsqlPoint point && Equals(point);

    public static bool operator ==(NpgsqlPoint x, NpgsqlPoint y) => x.Equals(y);

    public static bool operator !=(NpgsqlPoint x, NpgsqlPoint y) => !(x == y);

    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "({0},{1})", X, Y);
}

/// <summary>
/// Represents a PostgreSQL line type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-geometric.html
/// </remarks>
public struct NpgsqlLine(double a, double b, double c) : IEquatable<NpgsqlLine>
{
    public double A { get; set; } = a;
    public double B { get; set; } = b;
    public double C { get; set; } = c;

    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "{{{0},{1},{2}}}", A, B, C);

    public override int GetHashCode()
        => HashCode.Combine(A, B, C);

    public bool Equals(NpgsqlLine other)
        => A == other.A && B == other.B && C == other.C;

    public override bool Equals(object? obj)
        => obj is NpgsqlLine line && Equals(line);

    public static bool operator ==(NpgsqlLine x, NpgsqlLine y) => x.Equals(y);
    public static bool operator !=(NpgsqlLine x, NpgsqlLine y) => !(x == y);
}

/// <summary>
/// Represents a PostgreSQL Line Segment type.
/// </summary>
public struct NpgsqlLSeg : IEquatable<NpgsqlLSeg>
{
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
        End = new NpgsqlPoint(endx,   endy);
    }

    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "[{0},{1}]", Start, End);

    public override int GetHashCode()
        => HashCode.Combine(Start.X, Start.Y, End.X, End.Y);

    public bool Equals(NpgsqlLSeg other)
        => Start == other.Start && End == other.End;

    public override bool Equals(object? obj)
        => obj is NpgsqlLSeg seg && Equals(seg);

    public static bool operator ==(NpgsqlLSeg x, NpgsqlLSeg y) => x.Equals(y);
    public static bool operator !=(NpgsqlLSeg x, NpgsqlLSeg y) => !(x == y);
}

/// <summary>
/// Represents a PostgreSQL box type.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/datatype-geometric.html
/// </remarks>
public struct NpgsqlBox : IEquatable<NpgsqlBox>
{
    NpgsqlPoint _upperRight;
    public NpgsqlPoint UpperRight
    {
        get => _upperRight;
        set
        {
            _upperRight = value;
            NormalizeBox();
        }
    }

    NpgsqlPoint _lowerLeft;
    public NpgsqlPoint LowerLeft
    {
        get => _lowerLeft;
        set
        {
            _lowerLeft = value;
            NormalizeBox();
        }
    }

    public NpgsqlBox(NpgsqlPoint upperRight, NpgsqlPoint lowerLeft) : this()
    {
        _upperRight = upperRight;
        _lowerLeft = lowerLeft;
        NormalizeBox();
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
        => UpperRight == other.UpperRight && LowerLeft == other.LowerLeft;

    public override bool Equals(object? obj)
        => obj is NpgsqlBox box && Equals(box);

    public static bool operator ==(NpgsqlBox x, NpgsqlBox y) => x.Equals(y);
    public static bool operator !=(NpgsqlBox x, NpgsqlBox y) => !(x == y);
    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "{0},{1}", UpperRight, LowerLeft);

    public override int GetHashCode()
        => HashCode.Combine(Top, Right, Bottom, LowerLeft);

    // Swaps corners for isomorphic boxes, to mirror postgres behavior.
    // See: https://github.com/postgres/postgres/blob/af2324fabf0020e464b0268be9ef03e8f46ed84b/src/backend/utils/adt/geo_ops.c#L435-L447
    void NormalizeBox()
    {
        if (_upperRight.X < _lowerLeft.X)
            (_upperRight.X, _lowerLeft.X) = (_lowerLeft.X, _upperRight.X);

        if (_upperRight.Y < _lowerLeft.Y)
            (_upperRight.Y, _lowerLeft.Y) = (_lowerLeft.Y, _upperRight.Y);
    }
}

/// <summary>
/// Represents a PostgreSQL Path type.
/// </summary>
public struct NpgsqlPath : IList<NpgsqlPoint>, IEquatable<NpgsqlPath>
{
    List<NpgsqlPoint> _points;

    List<NpgsqlPoint> Points => _points ??= [];

    public bool Open { get; set; }

    public NpgsqlPath()
        => _points = [];

    public NpgsqlPath(IEnumerable<NpgsqlPoint> points, bool open)
    {
        _points = [..points];
        Open = open;
    }

    public NpgsqlPath(IEnumerable<NpgsqlPoint> points) : this(points, false) {}
    public NpgsqlPath(params NpgsqlPoint[] points) : this(points, false) {}

    public NpgsqlPath(bool open) : this()
    {
        _points = [];
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
        get => Points[index];
        set => Points[index] = value;
    }

    public int Capacity => Points.Capacity;
    public int Count => _points?.Count ?? 0;
    public bool IsReadOnly => false;

    public int IndexOf(NpgsqlPoint item) => Points.IndexOf(item);
    public void Insert(int index, NpgsqlPoint item) => Points.Insert(index, item);
    public void RemoveAt(int index) => Points.RemoveAt(index);
    public void Add(NpgsqlPoint item) => Points.Add(item);
    public void Clear() => Points.Clear();
    public bool Contains(NpgsqlPoint item) => Points.Contains(item);
    public void CopyTo(NpgsqlPoint[] array, int arrayIndex) => Points.CopyTo(array, arrayIndex);
    public bool Remove(NpgsqlPoint item) => Points.Remove(item);
    public IEnumerator<NpgsqlPoint> GetEnumerator() => Points.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(NpgsqlPath other)
    {
        if (Open != other.Open || Count != other.Count)
            return false;
        if (ReferenceEquals(_points, other._points))//Short cut for shallow copies.
            return true;
        for (var i = 0; i != Count; ++i)
            if (this[i] != other[i])
                return false;
        return true;
    }

    public override bool Equals(object? obj)
        => obj is NpgsqlPath path && Equals(path);

    public static bool operator ==(NpgsqlPath x, NpgsqlPath y) => x.Equals(y);
    public static bool operator !=(NpgsqlPath x, NpgsqlPath y) => !(x == y);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Open);

        foreach (var point in this)
        {
            hashCode.Add(point.X);
            hashCode.Add(point.Y);
        }

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Open ? '[' : '(');
        int i;
        for (i = 0; i < Count; i++)
        {
            var p = _points[i];
            sb.AppendFormat(CultureInfo.InvariantCulture, "({0},{1})", p.X, p.Y);
            if (i < _points.Count - 1)
                sb.Append(',');
        }
        sb.Append(Open ? ']' : ')');
        return sb.ToString();
    }
}

/// <summary>
/// Represents a PostgreSQL Polygon type.
/// </summary>
public struct NpgsqlPolygon : IList<NpgsqlPoint>, IEquatable<NpgsqlPolygon>
{
    List<NpgsqlPoint> _points;

    List<NpgsqlPoint> Points => _points ??= [];

    public NpgsqlPolygon()
        => _points = [];

    public NpgsqlPolygon(IEnumerable<NpgsqlPoint> points)
        => _points = [..points];

    public NpgsqlPolygon(params NpgsqlPoint[] points) : this((IEnumerable<NpgsqlPoint>) points) {}

    public NpgsqlPolygon(int capacity)
        => _points = new List<NpgsqlPoint>(capacity);

    public NpgsqlPoint this[int index]
    {
        get => Points[index];
        set => Points[index] = value;
    }

    public int Capacity => Points.Capacity;
    public int Count => _points?.Count ?? 0;
    public bool IsReadOnly => false;

    public int IndexOf(NpgsqlPoint item) => Points.IndexOf(item);
    public void Insert(int index, NpgsqlPoint item) => Points.Insert(index, item);
    public void RemoveAt(int index) => Points.RemoveAt(index);
    public void Add(NpgsqlPoint item) => Points.Add(item);
    public void Clear() => Points.Clear();
    public bool Contains(NpgsqlPoint item) => Points.Contains(item);
    public void CopyTo(NpgsqlPoint[] array, int arrayIndex) => Points.CopyTo(array, arrayIndex);
    public bool Remove(NpgsqlPoint item) => Points.Remove(item);
    public IEnumerator<NpgsqlPoint> GetEnumerator() => Points.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(NpgsqlPolygon other)
    {
        if (Count != other.Count)
            return false;
        if (ReferenceEquals(_points, other._points))
            return true;
        for (var i = 0; i != Count; ++i)
            if (this[i] != other[i])
                return false;
        return true;
    }

    public override bool Equals(object? obj)
        => obj is NpgsqlPolygon polygon && Equals(polygon);

    public static bool operator ==(NpgsqlPolygon x, NpgsqlPolygon y) => x.Equals(y);
    public static bool operator !=(NpgsqlPolygon x, NpgsqlPolygon y) => !(x == y);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var point in this)
        {
            hashCode.Add(point.X);
            hashCode.Add(point.Y);
        }

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('(');
        int i;
        for (i = 0; i < Count; i++)
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
public struct NpgsqlCircle(double x, double y, double radius) : IEquatable<NpgsqlCircle>
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Radius { get; set; } = radius;

    public NpgsqlCircle(NpgsqlPoint center, double radius)
        : this(center.X, center.Y, radius)
    {
    }

    public NpgsqlPoint Center
    {
        get => new(X, Y);
        set => (X, Y) = (value.X, value.Y);
    }

    // ReSharper disable CompareOfFloatsByEqualityOperator
    public bool Equals(NpgsqlCircle other)
        => X == other.X && Y == other.Y && Radius == other.Radius;
    // ReSharper restore CompareOfFloatsByEqualityOperator

    public override bool Equals(object? obj)
        => obj is NpgsqlCircle circle && Equals(circle);

    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "<({0},{1}),{2}>", X, Y, Radius);

    public static bool operator ==(NpgsqlCircle x, NpgsqlCircle y) => x.Equals(y);
    public static bool operator !=(NpgsqlCircle x, NpgsqlCircle y) => !(x == y);

    public override int GetHashCode()
        => HashCode.Combine(X, Y, Radius);
}

/// <summary>
/// Represents a PostgreSQL inet type, which is a combination of an IPAddress and a subnet mask.
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-net-types.html
/// </remarks>
public readonly record struct NpgsqlInet
{
    public IPAddress Address { get; }
    public byte Netmask { get; }

    public NpgsqlInet(IPAddress address, byte netmask)
    {
        CheckAddressFamily(address);
        Address = address;
        Netmask = netmask;
    }

    public NpgsqlInet(IPAddress address)
        : this(address, (byte)(address.AddressFamily == AddressFamily.InterNetwork ? 32 : 128))
    {
    }

    public NpgsqlInet(string addr)
    {
        switch (addr.Split('/'))
        {
        case { Length: 2 } segments:
            (Address, Netmask) = (IPAddress.Parse(segments[0]), byte.Parse(segments[1]));
            break;
        case { Length: 1 } segments:
            var ipAddr = IPAddress.Parse(segments[0]);
            CheckAddressFamily(ipAddr);
            (Address, Netmask) = (
                ipAddr,
                ipAddr.AddressFamily == AddressFamily.InterNetworkV6 ? (byte)128 : (byte)32);
            break;
        default:
            throw new FormatException("Invalid number of parts in CIDR specification");
        }
    }

    public override string ToString()
        => (Address?.AddressFamily == AddressFamily.InterNetwork && Netmask == 32) ||
           (Address?.AddressFamily == AddressFamily.InterNetworkV6 && Netmask == 128)
            ? Address.ToString()
            : $"{Address}/{Netmask}";

    public static explicit operator IPAddress(NpgsqlInet inet)
        => inet.Address;

    public static implicit operator NpgsqlInet(IPAddress ip)
        => new(ip);

    public void Deconstruct(out IPAddress address, out byte netmask)
    {
        address = Address;
        netmask = Netmask;
    }

    static void CheckAddressFamily(IPAddress address)
    {
        if (address.AddressFamily != AddressFamily.InterNetwork && address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException("Only IPAddress of InterNetwork or InterNetworkV6 address families are accepted", nameof(address));
    }
}

/// <summary>
/// Represents a PostgreSQL cidr type.
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-net-types.html
/// </remarks>
[Obsolete("Use .NET IPNetwork instead of NpgsqlCidr to map to PostgreSQL cidr")]
public readonly record struct NpgsqlCidr
{
    public IPAddress Address { get; }
    public byte Netmask { get; }

    public NpgsqlCidr(IPAddress address, byte netmask)
    {
        if (address.AddressFamily != AddressFamily.InterNetwork && address.AddressFamily != AddressFamily.InterNetworkV6)
            throw new ArgumentException("Only IPAddress of InterNetwork or InterNetworkV6 address families are accepted", nameof(address));

        Address = address;
        Netmask = netmask;
    }

    public NpgsqlCidr(string addr)
        => (Address, Netmask) = addr.Split('/') switch
        {
            { Length: 2 } segments => (IPAddress.Parse(segments[0]), byte.Parse(segments[1])),
            { Length: 1 } => throw new FormatException("Missing netmask"),
            _ => throw new FormatException("Invalid number of parts in CIDR specification")
        };

    public static implicit operator NpgsqlInet(NpgsqlCidr cidr)
        => new(cidr.Address, cidr.Netmask);

    public static explicit operator IPAddress(NpgsqlCidr cidr)
        => cidr.Address;

    public override string ToString()
        => $"{Address}/{Netmask}";

    public void Deconstruct(out IPAddress address, out byte netmask)
    {
        address = Address;
        netmask = Netmask;
    }
}

/// <summary>
/// Represents a PostgreSQL tid value
/// </summary>
/// <remarks>
/// https://www.postgresql.org/docs/current/static/datatype-oid.html
/// </remarks>
public readonly struct NpgsqlTid(uint blockNumber, ushort offsetNumber) : IEquatable<NpgsqlTid>
{
    /// <summary>
    /// Block number
    /// </summary>
    public uint BlockNumber { get; } = blockNumber;

    /// <summary>
    /// Tuple index within block
    /// </summary>
    public ushort OffsetNumber { get; } = offsetNumber;

    public bool Equals(NpgsqlTid other)
        => BlockNumber == other.BlockNumber && OffsetNumber == other.OffsetNumber;

    public override bool Equals(object? o)
        => o is NpgsqlTid tid && Equals(tid);

    public override int GetHashCode() => (int)BlockNumber ^ OffsetNumber;
    public static bool operator ==(NpgsqlTid left, NpgsqlTid right) => left.Equals(right);
    public static bool operator !=(NpgsqlTid left, NpgsqlTid right) => !(left == right);
    public override string ToString() => $"({BlockNumber},{OffsetNumber})";
}

#pragma warning restore 1591
