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
using System.Globalization;
using System.Data;
using System.Net;
using System.Text;
using System.IO;
using System.Resources;

namespace NpgsqlTypes
{

    /// <summary>
    /// Represents a PostgreSQL Point type
    /// </summary>

    public struct NpgsqlPoint
    {
        private Single _X;
        private Single _Y;

        public NpgsqlPoint(Single X, Single Y)
        {
            _X = X;
            _Y = Y;
        }

        public Single X
        {
            get
            {
                return _X;
            }

            set
            {
                _X = value;
            }
        }


        public Single Y
        {
            get
            {
                return _Y;
            }

            set
            {
                _Y = value;
            }
        }
    }

    public struct NpgsqlBox
    {
        private NpgsqlPoint _UpperRight;
        private NpgsqlPoint _LowerLeft;

        public NpgsqlBox(NpgsqlPoint UpperRight, NpgsqlPoint LowerLeft)
        {
            _UpperRight = UpperRight;
            _LowerLeft = LowerLeft;
        }


        public NpgsqlPoint UpperRight
        {
            get
            {
                return _UpperRight;
            }
            set
            {
                _UpperRight = value;
            }
        }

        public NpgsqlPoint LowerLeft
        {
            get
            {
                return _LowerLeft;
            }
            set
            {
                _LowerLeft = value;
            }
        }

    }


    /// <summary>
    /// Represents a PostgreSQL Line Segment type.
    /// </summary>
    public struct NpgsqlLSeg
    {
        public NpgsqlPoint     Start;
        public NpgsqlPoint     End;

        public NpgsqlLSeg(NpgsqlPoint Start, NpgsqlPoint End)
        {
            this.Start = Start;
            this.End = End;
        }

        public override String ToString()
        {
            return String.Format("({0}, {1})", Start, End);
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Path type.
    /// </summary>
    public struct NpgsqlPath
    {
        internal NpgsqlPoint[]	Points;

        internal Boolean 		IsOpen;

        public NpgsqlPath(NpgsqlPoint[] Points)
        {
            this.Points = Points;
            IsOpen = false;
        }

        public Int32 Count
        { get
          {
              return Points.Length;
          } }

        public NpgsqlPoint this [Int32 Index]
        { get
          {
              return Points[Index];
          } }

        public Boolean Open
        {
            get
            {
                return IsOpen;
            }
        }
    }

    /// <summary>
    /// Represents a PostgreSQL Polygon type.
    /// </summary>
    public struct NpgsqlPolygon
    {
        internal NpgsqlPoint[]     Points;

        public NpgsqlPolygon(NpgsqlPoint[] Points)
        {
            this.Points = Points;
        }

        public Int32 Count
        { get
          {
              return Points.Length;
          } }

        public NpgsqlPoint this [Int32 Index]
        { get
          {
              return Points[Index];
          } }
    }

    /// <summary>
    /// Represents a PostgreSQL Circle type.
    /// </summary>
    public struct NpgsqlCircle
    {
        public NpgsqlPoint   Center;
        public Double        Radius;

        public NpgsqlCircle(NpgsqlPoint Center, Double Radius)
        {
            this.Center = Center;
            this.Radius = Radius;
        }

        public override String ToString()
        {
            return string.Format("({0}), {1}", Center, Radius);
        }
    }


	/// <summary>
	/// Represents a PostgreSQL inet type.
	/// </summary>
	public struct NpgsqlInet
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
	}
}
