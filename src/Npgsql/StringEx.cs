using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

#if EXPOSE_EVERYTHING || EXPOSE_STRINGEX
public
#endif
static partial class StringEx
{
#pragma warning disable 1591

    #region basic String methods

    public static bool IsNullOrEmpty(this string value)
        => string.IsNullOrEmpty(value);

    public static bool IsNullOrWhiteSpace(this string value)
        => string.IsNullOrWhiteSpace(value);

    public static bool IsWhiteSpace(this string value)
    {
        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c)) continue;

            return false;
        }
        return true;
    }

#if !PCL
    public static string IsInterned(this string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return string.IsInterned(value);
    }

    public static string Intern(this string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return string.Intern(value);
    }
#endif

#if UNSAFE
    public static unsafe string ToLowerForASCII(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        value = string.Copy(value);
        fixed (char* low = value)
        {
            var end = low + value.Length;
            for (var p = low; p < end; p++)
            {
                var c = *p;
                if (c < 'A' || c > 'Z')
                    continue;
                *p = (char)(c + 0x20);
            }
        }
        return value;
    }

    public static unsafe string ToUpperForASCII(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        value = string.Copy(value);
        fixed (char* low = value)
        {
            var end = low + value.Length;
            for (var p = low; p < end; p++)
            {
                var c = *p;
                if (c < 'a' || c > 'z')
                    continue;
                *p = (char)(c - 0x20);
            }
        }
        return value;
    }
#else
    public static string ToLowerForASCII(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c < 'A' || c > 'Z')
                sb.Append(c);
            else
                sb.Append((char)(c + 0x20));
        }
        return sb.ToString();
    }

    public static string ToUpperForASCII(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c < 'a' || c > 'z')
                sb.Append(c);
            else
                sb.Append((char)(c - 0x20));
        }
        return sb.ToString();
    }
#endif

    #endregion

}
