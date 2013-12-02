// NpgsqlTypes.ExpectedTypeConverter.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Text;
using System.IO;
using Npgsql;

namespace NpgsqlTypes
{

    internal static class ExpectedTypeConverter
    {
        internal static object ChangeType(object value, Type expectedType)
        {
            if (value == null)
                return null;
            Type currentType = value.GetType();
            if (value is DBNull || currentType == expectedType)
                return value;
#if NET35
            if (expectedType == typeof(DateTimeOffset))
            {
                if (currentType == typeof(NpgsqlDate))
                {
                    return new DateTimeOffset((DateTime)(NpgsqlDate)value);
                }
                else if (currentType == typeof(NpgsqlTime))
                {
                    return new DateTimeOffset((DateTime)(NpgsqlTime)value);
                }
                else if (currentType == typeof(NpgsqlTimeTZ))
                {
                    NpgsqlTimeTZ timetz = (NpgsqlTimeTZ)value;
                    return new DateTimeOffset(timetz.Ticks, new TimeSpan(timetz.TimeZone.Hours, timetz.TimeZone.Minutes, timetz.TimeZone.Seconds));
                }
                else if (currentType == typeof(NpgsqlTimeStamp))
                {
                    return new DateTimeOffset((DateTime)(NpgsqlTimeStamp)value);
                }
                else if (currentType == typeof(NpgsqlTimeStampTZ))
                {
                    NpgsqlTimeStampTZ timestamptz = (NpgsqlTimeStampTZ)value;
                    return new DateTimeOffset(timestamptz.Ticks, new TimeSpan(timestamptz.TimeZone.Hours, timestamptz.TimeZone.Minutes, timestamptz.TimeZone.Seconds));
                }
                else if (currentType == typeof(NpgsqlInterval))
                {
                    return new DateTimeOffset(((TimeSpan)(NpgsqlInterval)value).Ticks, TimeSpan.FromSeconds(0));
                }
                else if (currentType == typeof(DateTime))
                {
                    return new DateTimeOffset((DateTime)value);
                }
                else if (currentType == typeof(TimeSpan))
                {
                    return new DateTimeOffset(((TimeSpan)value).Ticks, TimeSpan.FromSeconds(0));
                }
                else
                {
                    return DateTimeOffset.Parse(value.ToString(), CultureInfo.InvariantCulture);
                }
            }
            else
#endif
            if (expectedType == typeof(TimeSpan))
            {
                if (currentType == typeof(NpgsqlDate))
                {
                    return new TimeSpan(((DateTime)(NpgsqlDate)value).Ticks);
                }
                else if (currentType == typeof(NpgsqlTime))
                {
                    return new TimeSpan(((NpgsqlTime)value).Ticks);
                }
                else if (currentType == typeof(NpgsqlTimeTZ))
                {
                    return new TimeSpan(((NpgsqlTimeTZ)value).UTCTime.Ticks);
                }
                else if (currentType == typeof(NpgsqlTimeStamp))
                {
                    return new TimeSpan(((NpgsqlTimeStamp)value).Ticks);
                }
                else if (currentType == typeof(NpgsqlTimeStampTZ))
                {
                    return new TimeSpan(((DateTime)(NpgsqlTimeStampTZ)value).ToUniversalTime().Ticks);
                }
                else if (currentType == typeof(NpgsqlInterval))
                {
                    return (TimeSpan)(NpgsqlInterval)value;
                }
                else if (currentType == typeof(DateTime))
                {
                    return new TimeSpan(((DateTime)value).ToUniversalTime().Ticks);
                }
                else if (currentType == typeof(DateTimeOffset))
                {
                    return new TimeSpan(((DateTimeOffset)value).Ticks);
                }
                else
                {
#if NET40
                    return TimeSpan.Parse(value.ToString(), CultureInfo.InvariantCulture);
#else
                    return TimeSpan.Parse(value.ToString());
#endif
                }
            }
            else if (expectedType == typeof(string))
            {
                return value.ToString();
            }
            else if (expectedType == typeof(Guid))
            {
                if (currentType == typeof(byte[]))
                {
                    return new Guid((byte[])value);
                }
                else
                {
                    return new Guid(value.ToString());
                }
            }
            else if (expectedType == typeof(DateTime))
            {
                if (currentType == typeof(NpgsqlDate))
                {
                    return (DateTime)(NpgsqlDate)value;
                }
                else if (currentType == typeof(NpgsqlTime))
                {
                    return (DateTime)(NpgsqlTime)value;
                }
                else if (currentType == typeof(NpgsqlTimeTZ))
                {
                    return (DateTime)(NpgsqlTimeTZ)value;
                }
                else if (currentType == typeof(NpgsqlTimeStamp))
                {
                    return (DateTime)(NpgsqlTimeStamp)value;
                }
                else if (currentType == typeof(NpgsqlTimeStampTZ))
                {
                    return (DateTime)(NpgsqlTimeStampTZ)value;
                }
                else if (currentType == typeof(NpgsqlInterval))
                {
                    return new DateTime(((TimeSpan)(NpgsqlInterval)value).Ticks);
                }
#if NET35
                else if (currentType == typeof(DateTimeOffset))
                {
                    return ((DateTimeOffset)value).LocalDateTime;
                }
#endif
                else if (currentType == typeof(TimeSpan))
                {
                    return new DateTime(((TimeSpan)value).Ticks);
                }
                else
                {
                    return DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture);
                }
            }
            else if (expectedType == typeof(byte[]))
            {
                if (currentType == typeof(Guid))
                {
                    return ((Guid)value).ToByteArray();
                }
                else if (value is Array)
                {
                    Array valueArray = (Array)value;
                    int byteLength = Buffer.ByteLength(valueArray);
                    byte[] bytes = new byte[byteLength];
                    Buffer.BlockCopy(valueArray, 0, bytes, 0, byteLength);
                    return bytes;
                }
                else
                {
                    // expect InvalidCastException from this call
                    return Convert.ChangeType(value, expectedType);
                }
            }
            else // long, int, short, double, float, decimal, byte, sbyte, bool, and other unspecified types
            {
                // ChangeType supports the conversions we want for above expected types
                return Convert.ChangeType(value, expectedType);
            }
        }

    }
}
