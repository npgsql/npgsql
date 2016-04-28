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

    public static string ToLowerForASCII(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value;

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            if (c.IsNotAsciiUpper())
                sb.Append(c);
            else
                sb.Append(c.ToAsciiLowerNoCheck());
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
            if (c.IsNotAsciiLower())
                sb.Append(c);
            else
                sb.Append(c.ToAsciiUpperNoCheck());
        }
        return sb.ToString();
    }

    #endregion

}
