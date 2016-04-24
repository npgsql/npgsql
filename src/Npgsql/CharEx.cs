using System.Globalization;

#if EXPOSE_EVERYTHING || EXPOSE_CHAREX
public
#endif
static partial class CharEx
{
#pragma warning disable 1591

    public const int AsciiCaseGap = 'a' - 'A'; // 0x61 - 0x41 = 0x20

    #region IS

    public static bool IsControl(this char c)
        => char.IsControl(c);
    public static bool IsDigit(this char c)
        => char.IsDigit(c);
    public static bool IsLetter(this char c)
        => char.IsLetter(c);

    public static bool IsAsciiUpper(this char c)
        => (c >= 'A' && c <= 'Z');
    public static bool IsAsciiLower(this char c)
        => (c >= 'a' && c <= 'z');

    public static bool IsNotAsciiUpper(this char c)
        => (c < 'A' || c > 'Z');
    public static bool IsNotAsciiLower(this char c)
        => (c < 'a' || c > 'z');

    public static bool IsUpper(this char c)
        => char.IsUpper(c);
    public static bool IsLower(this char c)
        => char.IsLower(c);
    public static bool IsLetterOrDigit(this char c)
        => char.IsLetterOrDigit(c);
    public static bool IsNumber(this char c)
        => char.IsNumber(c);

    public static bool IsPunctuation(this char c)
        => char.IsPunctuation(c);
    public static bool IsSeparator(this char c)
        => char.IsSeparator(c);

    public static bool IsSurrogate(this char c)
        => char.IsSurrogate(c);
    public static bool IsHighSurrogate(this char c)
#if PCL
        => ((c >= '\uD800') && (c < '\uDC00'));
#else
        => char.IsHighSurrogate(c);
#endif
    public static bool IsLowSurrogate(this char c)
#if PCL
        => ((c >= '\uDC00') && (c < '\uDE00'));
#else
        => char.IsLowSurrogate(c);
#endif

    public static bool IsSymbol(this char c)
        => char.IsSymbol(c);
    public static bool IsWhiteSpace(this char c)
        => char.IsWhiteSpace(c);

    #endregion

    #region ToLower

    public static char ToLower(this char c)
        => char.ToLower(c);
    public static char ToLower(this char c, CultureInfo culture)
        => char.ToLower(c, culture);
    public static char ToLowerInvariant(this char c)
        => char.ToLowerInvariant(c);

    internal static char ToAsciiLowerNoCheck(this char c)
        => (char)(c + AsciiCaseGap);
    public static char ToLowerForAscii(this char c)
    {
        if (c.IsAsciiUpper())
            return c.ToAsciiLowerNoCheck();
        return c;
    }
    #endregion

    #region ToUpper

    public static char ToUpper(this char c)
        => char.ToUpper(c);
    public static char ToUpper(this char c, CultureInfo culture)
        => char.ToUpper(c, culture);
    public static char ToUpperInvariant(this char c)
        => char.ToUpperInvariant(c);

    internal static char ToAsciiUpperNoCheck(this char c)
        => (char)(c - AsciiCaseGap);
    public static char ToUpperForAscii(this char c)
    {
        if (c.IsAsciiLower())
            return c.ToAsciiUpperNoCheck();
        return c;
    }
    #endregion
}
