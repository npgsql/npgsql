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

// This file provides data type converters between PostgreSQL representations
// and .NET objects.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NpgsqlTypes
{
	/// <summary>
	/// Provide event handlers to convert all native supported basic data types from their backend
	/// text representation to a .NET object.
	/// </summary>
	internal abstract class BasicBackendToNativeTypeConverter
	{
		private static readonly String[] DateFormats = new String[] { "yyyy-MM-dd", };
		private static readonly Regex EXCLUDE_DIGITS = new Regex("[^0-9\\-]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly String[] TimeFormats =
			new String[]
				{
					"HH:mm:ss.ffffff", "HH:mm:ss", "HH:mm:ss.ffffffzz", "HH:mm:sszz", "HH:mm:ss.fffff", "HH:mm:ss.ffff", "HH:mm:ss.fff"
					, "HH:mm:ss.ff", "HH:mm:ss.f", "HH:mm:ss.fffffzz", "HH:mm:ss.ffffzz", "HH:mm:ss.fffzz", "HH:mm:ss.ffzz",
					"HH:mm:ss.fzz", 
                    "HH:mm:ss.fffffzzz", "HH:mm:ss.ffffzzz", "HH:mm:ss.fffzzz", "HH:mm:ss.ffzzz",
                    "HH:mm:ss.fzzz", "HH:mm:sszzz",
				};

		private static readonly String[] DateTimeFormats =
			new String[]
				{
					"yyyy-MM-dd HH:mm:ss.ffffff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.ffffffzz", "yyyy-MM-dd HH:mm:sszz",
					"yyyy-MM-dd HH:mm:ss.fffff", "yyyy-MM-dd HH:mm:ss.ffff", "yyyy-MM-dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss.ff",
					"yyyy-MM-dd HH:mm:ss.f", "yyyy-MM-dd HH:mm:ss.fffffzz", "yyyy-MM-dd HH:mm:ss.ffffzz", "yyyy-MM-dd HH:mm:ss.fffzz",
					"yyyy-MM-dd HH:mm:ss.ffzz", "yyyy-MM-dd HH:mm:ss.fzz",
                    "yyyy-MM-dd HH:mm:ss.fffffzzz", "yyyy-MM-dd HH:mm:ss.ffffzzz", "yyyy-MM-dd HH:mm:ss.fffzzz",
                    "yyyy-MM-dd HH:mm:ss.ffzzz", "yyyy-MM-dd HH:mm:ss.fzzz", "yyyy-MM-dd HH:mm:sszzz"
				};

		/// <summary>
		/// Binary data.
		/// </summary>
		internal static Object ToBinary(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			Int32 octalValue = 0;
			Int32 byteAPosition = 0;
			Int32 byteAStringLength = BackendData.Length;
			MemoryStream ms = new MemoryStream();

			if (BackendData.StartsWith("\\x"))
            {
                // PostgreSQL 8.5+'s bytea_output=hex format
                for (byteAPosition = 2; byteAPosition < byteAStringLength; byteAPosition += 2)
                {
                    byte value = Convert.ToByte(BackendData.Substring(byteAPosition, 2), 16);
                    ms.WriteByte(value);
                }
            }
            
            else
            {
            
                while (byteAPosition < byteAStringLength)
    			{
    				// The IsDigit is necessary in case we receive a \ as the octal value and not
    				// as the indicator of a following octal value in decimal format.
    				// i.e.: \201\301P\A
    				if (BackendData[byteAPosition] == '\\')
    				{
    					if (byteAPosition + 1 == byteAStringLength)
    					{
    						octalValue = '\\';
    						byteAPosition++;
    					}
    					else if (Char.IsDigit(BackendData[byteAPosition + 1]))
    					{
                            octalValue = Convert.ToByte(BackendData.Substring(byteAPosition + 1, 3), 8);
                            //octalValue = (Byte.Parse(BackendData[byteAPosition + 1].ToString()) << 6);
                            //octalValue |= (Byte.Parse(BackendData[byteAPosition + 2].ToString()) << 3);
                            //octalValue |= Byte.Parse(BackendData[byteAPosition + 3].ToString());
    						byteAPosition += 4;
    					}
    					else
    					{
    						octalValue = '\\';
    						byteAPosition += 2;
    					}
    				}
    				else
    				{
    					octalValue = (Byte)BackendData[byteAPosition];
    					byteAPosition++;
    				}
    
    
    				ms.WriteByte((Byte)octalValue);
    			}
            }
            
			return ms.ToArray();
		}

		/// <summary>
		/// Convert a postgresql boolean to a System.Boolean.
		/// </summary>
		internal static Object ToBoolean(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize,
										 Int32 TypeModifier)
		{
			return (BackendData.ToLower() == "t" ? true : false);
		}


		/// <summary>
		/// Convert a postgresql bit to a System.Boolean.
		/// </summary>
		internal static Object ToBit(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
		    /// <summary>
		    /// Current tests seem to expect single-bit bitstrings to behave as boolean (why?)
		    /// 
		    /// To ensure compatibility we return a bool if the bitstring is single-length.
		    /// Maybe we don't need to do this (why do we?) or maybe people used to some other,
		    /// but taking a conservative approach here.
		    /// 
		    /// It means that IDataReader.GetValue() can't be used safely for bitstrings that
		    /// may be single-bit, but NpgsqlDataReader.GetBitString() can deal with the conversion
		    /// below by reversing it, so if GetBitString() is used, no harm is done.
		    BitString bs = BitString.Parse(BackendData);
		    return bs.Length == 1 ? (object)bs[0] : bs;
		}

		/// <summary>
		/// Convert a postgresql datetime to a System.DateTime.
		/// </summary>
		internal static Object ToDateTime(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize,
										  Int32 TypeModifier)
		{
			// Get the date time parsed in all expected formats for timestamp.

			// First check for special values infinity and -infinity.

			if (BackendData == "infinity")
			{
				return DateTime.MaxValue;
			}

			if (BackendData == "-infinity")
			{
				return DateTime.MinValue;
			}

			return
				DateTime.ParseExact(BackendData, DateTimeFormats, DateTimeFormatInfo.InvariantInfo,
									DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces);
		}

		/// <summary>
		/// Convert a postgresql date to a System.DateTime.
		/// </summary>
		internal static Object ToDate(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
		    // First check for special values infinity and -infinity.

			if (BackendData == "infinity")
			{
				return DateTime.MaxValue;
			}

			if (BackendData == "-infinity")
			{
				return DateTime.MinValue;
			}
            
			return
				DateTime.ParseExact(BackendData, DateFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces);
		}

		/// <summary>
		/// Convert a postgresql time to a System.DateTime.
		/// </summary>
		internal static Object ToTime(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			return
				DateTime.ParseExact(BackendData, TimeFormats, DateTimeFormatInfo.InvariantInfo,
									DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces);
		}

		/// <summary>
		/// Convert a postgresql money to a System.Decimal.
		/// </summary>
		internal static Object ToMoney(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			// Will vary according to currency symbol, position of symbol and decimal and thousands separators, but will
			// alwasy be two-decimal precision number using Arabic (0-9) digits, so we can extract just those digits and
			// divide by 100.
			return Convert.ToDecimal(EXCLUDE_DIGITS.Replace(BackendData, string.Empty), CultureInfo.InvariantCulture) / 100m;
		}
	}

	/// <summary>
	/// Provide event handlers to convert the basic native supported data types from
	/// native form to backend representation.
	/// </summary>
	internal abstract class BasicNativeToBackendTypeConverter
	{
		/// <summary>
		/// Binary data.
		/// </summary>
		internal static String ToBinary(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			Byte[] byteArray = (Byte[])NativeData;
			StringBuilder res = new StringBuilder(byteArray.Length * 5);
			foreach(byte b in byteArray)
			    if(b >= 0x20 && b < 0x7F && b != 0x27 && b != 0x5C)
			        res.Append((char)b);
			    else
			        res.Append("\\\\")
			            .Append((char)('0' + (7 & (b >> 6))))
			            .Append((char)('0' + (7 & (b >> 3))))
			            .Append((char)('0' + (7 & b)));
            return res.ToString();
		}

		/// <summary>
		/// Convert to a postgresql boolean.
		/// </summary>
        internal static String ToBoolean(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			return ((bool)NativeData) ? "TRUE" : "FALSE";
		}

		/// <summary>
		/// Convert to a postgresql bit.
		/// </summary>
        internal static String ToBit(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
		    if(NativeData is bool)
		        return ((bool)NativeData) ? "1" : "0";
		    // It may seem more sensible to just convert an integer to a BitString here and pass it on.
		    // However behaviour varies in terms of how this is interpretted if being passed to a bitstring
		    // value smaller than the int.
		    // Prior to Postgres 8.0, the behaviour would be the same either way. E.g. if 10 were passed to
		    // a bit(1) then the bits (1010) would be extracted from the left, so resulting in the bitstring B'1'.
		    // From 8.0 onwards though, if we cast 10 straight to a bit(1) then the right-most bit is taken,
		    // resulting in B'0'. If we cast it to the "natural" bitstring for it's size first (which is what would
		    // happen if we did that work here) then it would become B'1010' which would then be cast to bit(1) by
		    // taking the left-most bit resulting in B'1' (the behaviour one would expect from Postgres 7.x).
		    // 
		    // Since we don't know what implicit casts (say by inserting into a table with a bitstring field of
		    // set size) may happen, we don't know how to ensure expected behaviour. While passing a bitstring
		    // literal would work as expected with Postgres before 8.0, it can fail with 8.0 and later.
		    else if(NativeData is int)
		        return NativeData.ToString();
		    else
		        return ((BitString)NativeData).ToString("E");
		}

		/// <summary>
		/// Convert to a postgresql timestamp.
		/// </summary>
        internal static String ToDateTime(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			if (!(NativeData is DateTime))
			{
				return ExtendedNativeToBackendTypeConverter.ToTimeStamp(TypeInfo, NativeData, ForExtendedQuery);
			}
			if (DateTime.MaxValue.Equals(NativeData))
			{
				return "infinity";
			}
			if (DateTime.MinValue.Equals(NativeData))
			{
				return "-infinity";
			}
			return ((DateTime)NativeData).ToString("yyyy-MM-dd HH:mm:ss.ffffff", DateTimeFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Convert to a postgresql date.
		/// </summary>
        internal static String ToDate(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			if (!(NativeData is DateTime))
			{
				return ExtendedNativeToBackendTypeConverter.ToDate(TypeInfo, NativeData, ForExtendedQuery);
			}
			return ((DateTime)NativeData).ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Convert to a postgresql time.
		/// </summary>
        internal static String ToTime(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			if (!(NativeData is DateTime))
			{
				return ExtendedNativeToBackendTypeConverter.ToTime(TypeInfo, NativeData, ForExtendedQuery);
			}
			else
			{
				return ((DateTime)NativeData).ToString("HH:mm:ss.ffffff", DateTimeFormatInfo.InvariantInfo);
			}
		}

		/// <summary>
		/// Convert to a postgres money.
		/// </summary>
        internal static String ToMoney(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
		    //Formats accepted vary according to locale, but it always accepts a plain number (no currency or
		    //grouping symbols) passed as a string (with the appropriate cast appended, as UseCast will cause
		    //to happen.
			return ((IFormattable)NativeData).ToString(null, CultureInfo.InvariantCulture.NumberFormat);
		}
		
		
		/// <summary>
		/// Convert to a postgres double with maximum precision.
		/// </summary>
        internal static String ToSingleDouble(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
		    //Formats accepted vary according to locale, but it always accepts a plain number (no currency or
		    //grouping symbols) passed as a string (with the appropriate cast appended, as UseCast will cause
		    //to happen.
			return ((IFormattable)NativeData).ToString("R", CultureInfo.InvariantCulture.NumberFormat);
		}
		
		


        internal static string ToBasicType<T>(NpgsqlNativeTypeInfo TypeInfo, object NativeData, Boolean ForExtendedQuery)
        {
            // This double cast is needed in order to get the enum type handled correctly (IConvertible)
            // and the decimal separator always as "." regardless of culture (IFormattable)
            return (((IFormattable)((IConvertible)NativeData).ToType(typeof(T), null)).ToString(null, CultureInfo.InvariantCulture.NumberFormat));
        }
	}


	/// <summary>
	/// Provide event handlers to convert extended native supported data types from their backend
	/// text representation to a .NET object.
	/// </summary>
	internal abstract class ExtendedBackendToNativeTypeConverter
	{
		private static readonly Regex pointRegex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\)");
		private static readonly Regex boxlsegRegex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\),\((-?\d+.?\d*),(-?\d+.?\d*)\)");
		private static readonly Regex pathpolygonRegex = new Regex(@"\((-?\d+.?\d*),(-?\d+.?\d*)\)");
		private static readonly Regex circleRegex = new Regex(@"<\((-?\d+.?\d*),(-?\d+.?\d*)\),(\d+.?\d*)>");


		/// <summary>
		/// Convert a postgresql point to a System.NpgsqlPoint.
		/// </summary>
		internal static Object ToPoint(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			Match m = pointRegex.Match(BackendData);

			return
				new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
								Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat));
		}

		/// <summary>
		/// Convert a postgresql point to a System.RectangleF.
		/// </summary>
		internal static Object ToBox(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			Match m = boxlsegRegex.Match(BackendData);

			return
				new NpgsqlBox(
					new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
									Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
					new NpgsqlPoint(Single.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
									Single.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
		}

		/// <summary>
		/// LDeg.
		/// </summary>
		internal static Object ToLSeg(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			Match m = boxlsegRegex.Match(BackendData);

			return
				new NpgsqlLSeg(
					new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
									Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
					new NpgsqlPoint(Single.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
									Single.Parse(m.Groups[4].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
		}

		/// <summary>
		/// Path.
		/// </summary>
		internal static Object ToPath(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			Match m = pathpolygonRegex.Match(BackendData);
			Boolean open = (BackendData[0] == '[');
			List<NpgsqlPoint> points = new List<NpgsqlPoint>();

			while (m.Success)
			{
				if (open)
				{
					points.Add(
						new NpgsqlPoint(
							Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
							Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
				}
				else
				{
					// Here we have to do a little hack, because as of 2004-08-11 mono cvs version, the last group is returned with
					// a trailling ')' only when the last character of the string is a ')' which is the case for closed paths
					// returned by backend. This gives parsing exception when converting to single. 
					// I still don't know if this is a bug in mono or in my regular expression.
					// Check if there is this character and remove it.

					String group2 = m.Groups[2].ToString();
					if (group2.EndsWith(")"))
					{
						group2 = group2.Remove(group2.Length - 1, 1);
					}

					points.Add(
						new NpgsqlPoint(
							Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
							Single.Parse(group2, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));
				}

				m = m.NextMatch();
			}

			NpgsqlPath result = new NpgsqlPath(points.ToArray());
			result.Open = open;
			return result;
		}

		/// <summary>
		/// Polygon.
		/// </summary>
		internal static Object ToPolygon(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize,
										 Int32 TypeModifier)
		{
			Match m = pathpolygonRegex.Match(BackendData);
			List<NpgsqlPoint> points = new List<NpgsqlPoint>();

			while (m.Success)
			{
				// Here we have to do a little hack, because as of 2004-08-11 mono cvs version, the last group is returned with
				// a trailling ')' only when the last character of the string is a ')' which is the case for closed paths
				// returned by backend. This gives parsing exception when converting to single. 
				// I still don't know if this is a bug in mono or in my regular expression.
				// Check if there is this character and remove it.

				String group2 = m.Groups[2].ToString();
				if (group2.EndsWith(")"))
				{
					group2 = group2.Remove(group2.Length - 1, 1);
				}

				points.Add(
					new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
									Single.Parse(group2, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)));


				m = m.NextMatch();
			}

			return new NpgsqlPolygon(points);
		}

		/// <summary>
		/// Circle.
		/// </summary>
		internal static Object ToCircle(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			Match m = circleRegex.Match(BackendData);
			return
				new NpgsqlCircle(
					new NpgsqlPoint(Single.Parse(m.Groups[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat),
									Single.Parse(m.Groups[2].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat)),
					Single.Parse(m.Groups[3].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat));
		}

		/// <summary>
		/// Inet.
		/// </summary>
		internal static Object ToInet(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
		{
			return new NpgsqlInet(BackendData);
		}

        /// <summary>
        /// MAC Address.
        /// </summary>
        internal static Object ToMacAddress(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            return new NpgsqlMacAddress(BackendData);
        }
		
        internal static Object ToGuid(NpgsqlBackendTypeInfo TypeInfo, String BackendData, Int16 TypeSize, Int32 TypeModifier)
        {
            return new Guid(BackendData);
        }

		/// <summary>
		/// interval
		/// </summary>
		internal static object ToInterval(NpgsqlBackendTypeInfo typeInfo, String backendData, Int16 typeSize,
										  Int32 typeModifier)
		{
			return NpgsqlInterval.Parse(backendData);
		}

		internal static object ToTime(NpgsqlBackendTypeInfo typeInfo, String backendData, Int16 typeSize, Int32 typeModifier)
		{
			return NpgsqlTime.Parse(backendData);
		}

		internal static object ToTimeTZ(NpgsqlBackendTypeInfo typeInfo, String backendData, Int16 typeSize, Int32 typeModifier)
		{
			return NpgsqlTimeTZ.Parse(backendData);
		}

		internal static object ToDate(NpgsqlBackendTypeInfo typeInfo, String backendData, Int16 typeSize, Int32 typeModifier)
		{
			return NpgsqlDate.Parse(backendData);
		}

		internal static object ToTimeStamp(NpgsqlBackendTypeInfo typeInfo, String backendData, Int16 typeSize,
										   Int32 typeModifier)
		{
			return NpgsqlTimeStamp.Parse(backendData);
		}

		internal static object ToTimeStampTZ(NpgsqlBackendTypeInfo typeInfo, String backendData, Int16 typeSize,
											 Int32 typeModifier)
		{
			return NpgsqlTimeStampTZ.Parse(backendData);
		}
	}

	/// <summary>
	/// Provide event handlers to convert extended native supported data types from
	/// native form to backend representation.
	/// </summary>
	internal abstract class ExtendedNativeToBackendTypeConverter
	{
		/// <summary>
		/// Point.
		/// </summary>
        internal static String ToPoint(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			if (NativeData is NpgsqlPoint)
			{
				NpgsqlPoint P = (NpgsqlPoint)NativeData;
				return String.Format(CultureInfo.InvariantCulture, "({0},{1})", P.X, P.Y);
			}
			else
			{
				throw new InvalidCastException("Unable to cast data to NpgsqlPoint type");
			}
		}

		/// <summary>
		/// Box.
		/// </summary>
        internal static String ToBox(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			/*if (NativeData.GetType() == typeof(Rectangle)) {
                Rectangle       R = (Rectangle)NativeData;
                return String.Format(CultureInfo.InvariantCulture, "({0},{1}),({2},{3})", R.Left, R.Top, R.Left + R.Width, R.Top + R.Height);
            } else if (NativeData.GetType() == typeof(RectangleF)) {
                RectangleF      R = (RectangleF)NativeData;
                return String.Format(CultureInfo.InvariantCulture, "({0},{1}),({2},{3})", R.Left, R.Top, R.Left + R.Width, R.Top + R.Height);*/

			if (NativeData is NpgsqlBox)
			{
				NpgsqlBox box = (NpgsqlBox)NativeData;
				return
					String.Format(CultureInfo.InvariantCulture, "({0},{1}),({2},{3})", box.LowerLeft.X, box.LowerLeft.Y,
								  box.UpperRight.X, box.UpperRight.Y);
			}
			else
			{
				throw new InvalidCastException("Unable to cast data to Rectangle type");
			}
		}

		/// <summary>
		/// LSeg.
		/// </summary>
        internal static String ToLSeg(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			NpgsqlLSeg S = (NpgsqlLSeg)NativeData;
			return String.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", S.Start.X, S.Start.Y, S.End.X, S.End.Y);
		}

		/// <summary>
		/// Open path.
		/// </summary>
        internal static String ToPath(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			StringBuilder B = null;
			try
			{
	B =new StringBuilder();

			foreach (NpgsqlPoint P in ((NpgsqlPath)NativeData))
			{
				B.AppendFormat(CultureInfo.InvariantCulture, "{0}({1},{2})", (B.Length > 0 ? "," : ""), P.X, P.Y);
			}

			return String.Format("[{0}]", B);
			}
			finally
			{
				B = null;

			}
		
		}

		/// <summary>
		/// Polygon.
		/// </summary>
        internal static String ToPolygon(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			StringBuilder B = new StringBuilder();

			foreach (NpgsqlPoint P in ((NpgsqlPolygon)NativeData))
			{
				B.AppendFormat(CultureInfo.InvariantCulture, "{0}({1},{2})", (B.Length > 0 ? "," : ""), P.X, P.Y);
			}

			return String.Format("({0})", B);
		}
  
        /// <summary>
        /// Convert to a postgres MAC Address.
        /// </summary>
        internal static String ToMacAddress(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
        {
            if (NativeData is NpgsqlMacAddress)
            {
                return ((NpgsqlMacAddress)NativeData).ToString();
            }
            return NativeData.ToString();
        }
        
		/// <summary>
		/// Circle.
		/// </summary>
        internal static String ToCircle(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			NpgsqlCircle C = (NpgsqlCircle)NativeData;
			return String.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", C.Center.X, C.Center.Y, C.Radius);
		}

		/// <summary>
		/// Convert to a postgres inet.
		/// </summary>
        internal static String ToIPAddress(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			if (NativeData is NpgsqlInet)
			{
				return ((NpgsqlInet)NativeData).ToString();
			}
			return NativeData.ToString();

		}

		/// <summary>
		/// Convert to a postgres interval
		/// </summary>
        internal static String ToInterval(NpgsqlNativeTypeInfo TypeInfo, Object NativeData, Boolean ForExtendedQuery)
		{
			return
				((NativeData is TimeSpan)
					? ((NpgsqlInterval)(TimeSpan)NativeData).ToString()
					: ((NpgsqlInterval)NativeData).ToString());
		}

        internal static string ToTime(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean ForExtendedQuery)
		{
			if (nativeData is DateTime)
			{
				return BasicNativeToBackendTypeConverter.ToTime(typeInfo, nativeData, ForExtendedQuery);
			}
			NpgsqlTime time;
			if (nativeData is TimeSpan)
			{
				time = (NpgsqlTime)(TimeSpan)nativeData;
			}
			else
			{
				time = (NpgsqlTime)nativeData;
			}
			return time.ToString();
		}

        internal static string ToTimeTZ(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean ForExtendedQuery)
		{
			if (nativeData is DateTime)
			{
				return BasicNativeToBackendTypeConverter.ToTime(typeInfo, nativeData, ForExtendedQuery);
			}
			NpgsqlTimeTZ time;
			if (nativeData is TimeSpan)
			{
				time = (NpgsqlTimeTZ)(TimeSpan)nativeData;
			}
			else
			{
				time = (NpgsqlTimeTZ)nativeData;
			}
			return time.ToString();
		}

        internal static string ToDate(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean ForExtendedQuery)
		{
			if (nativeData is DateTime)
			{
				return BasicNativeToBackendTypeConverter.ToDate(typeInfo, nativeData, ForExtendedQuery);
			}
			else
			{
				return nativeData.ToString();
			}
		}

        internal static string ToTimeStamp(NpgsqlNativeTypeInfo typeInfo, object nativeData, Boolean ForExtendedQuery)
		{
			if (nativeData is DateTime)
			{
				return BasicNativeToBackendTypeConverter.ToDateTime(typeInfo, nativeData, ForExtendedQuery);
			}
			else
			{
				return nativeData.ToString();
			}
		}
	}
}
