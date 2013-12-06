/********************************************************
 * ADO.NET 2.0 Data Provider for SQLite Version 3.X
 * Written by Robert Simpson (robert@blackcastlesoft.com)
 *
 * Released to the public domain, use at your own risk!
 ********************************************************/

namespace Npgsql.Designer.Design
{
  using System;
  using System.Collections.Generic;
  using System.Text;

  internal static class SimpleTokenizer
  {
    public struct StringParts
    {
      internal string value;
      internal int position;
      internal string quote;
      internal bool sep;
      internal int depth;
      internal string keyword;
      internal char sepchar;

      public override string ToString()
      {
        return String.Format("{0} {1} at {2} {3} depth {4}", value, quote, position, sep == true ? "(sep)" : "", depth);
      }
    }

    public static StringParts[] BreakString(string source)
    {
      char[] opens = new char[] { '\"', '[', '\'', '(', ')', ',', ' ', ';', '\r', '\n', '\t' };
      char[] opens2 = new char[] { '\"', '[', '\'', '(', ')', ',', ' ', ';', '\r', '\n', '\t', '.' };
      char[] closes = new char[] { '\"', ']', '\'', };
      string sep = ";,";
      string opensstr = "\"['";

      if (String.IsNullOrEmpty(source) == true) return new StringParts[0];

      int n = 0;
      int x;
      int depth = 0;
      List<StringParts> ls = new List<StringParts>();
      int startat = 0;

      while (source.Length > 0)
      {
        if (source.Length > 1 && source[0] == '-' && source[1] == '-')
        {
          StringParts tok = new StringParts();
          tok.position = startat;
          x = source.IndexOf('\n');
          if (x == -1) tok.value = source;
          else tok.value = source.Substring(0, x + 1);

          //ls.Add(tok);
          source = source.Substring(tok.value.Length);
          startat += tok.value.Length;
          continue;
        }
        else if (source.Length > 1 && source[0] == '/' && source[1] == '*')
        {
          StringParts tok = new StringParts();
          tok.position = startat;
          x = source.IndexOf("*/");
          if (x == -1) tok.value = source;
          else tok.value = source.Substring(0, x + 2);

          //ls.Add(tok);
          source = source.Substring(tok.value.Length);
          startat += tok.value.Length;
          continue;
        }
        int comment = source.IndexOf("--", n);
        if (comment == -1) comment = source.IndexOf("/*", n);

        if (n > 0)
          n = source.IndexOfAny(opens2, n);
        else
          n = source.IndexOfAny(opens, n);

        if (comment > -1 && (n == -1 || comment < n))
          n = comment;

        if (n == -1) break;

        x = opensstr.IndexOf(source[n]);
        if (x != -1)
        {
          while (n != -1)
          {
            n = source.IndexOf(closes[x], n + 1);
            if (n == -1)
              break;

            if (n < source.Length - 1 && source[n + 1] == source[n])
            {
              startat++;
              source = source.Remove(n, 1);
            }
            else
            {
              n++;
              break;
            }
          }
          if (n == -1)
            break;
        }
        else
        {
          StringParts tok = new StringParts();

          int y = sep.IndexOf(source[n]);
          tok.sep = (y != -1);
          tok.sepchar = (y != -1) ? sep[y] : '\0';

          if (source[n] == '(') depth++;
          tok.depth = depth;
          if (source[n] == ')') depth--;

          tok.value = source.Substring(0, n);
          tok.position = startat;

          if (tok.value.Length > 1)
          {
            x = opensstr.IndexOf(tok.value[0]);
            if (x != -1 && tok.value[tok.value.Length - 1] == closes[x])
            {
              tok.quote = String.Format("{0}{1}", tok.value[0], tok.value[tok.value.Length - 1]);
              tok.value = tok.value.Substring(1, tok.value.Length - 2);
            }
            else
              tok.keyword = tok.value.ToUpperInvariant();
          }

          if (source.Length - n > 1 && ((source[n] == '-' && source[n + 1] == '-') || source[n] == '/' && source[n + 1] == '*'))
          {
            startat += n;
            source = source.Substring(n);
          }
          else
          {
            startat += (n + 1);
            source = source.Substring(n + 1);
          }
          if (tok.value.Length > 0)
            ls.Add(tok);
          else if (ls.Count > 0 && tok.sep)
          {
            StringParts prev = ls[ls.Count - 1];
            ls.RemoveAt(ls.Count - 1);
            prev.sep = tok.sep;
            prev.sepchar = tok.sepchar;
            ls.Add(prev);
          }
          n = 0;
        }
      }

      if (source.Length > 0)
      {
        StringParts tok = new StringParts();

        tok.value = source.Trim();
        tok.position = startat;

        if (tok.value.Length > 1)
        {
          x = opensstr.IndexOf(tok.value[0]);
          if (x != -1 && tok.value[tok.value.Length - 1] == closes[x])
          {
            tok.quote = String.Format("{0}{1}", tok.value[0], tok.value[tok.value.Length - 1]);
            tok.value = tok.value.Substring(1, tok.value.Length - 2);
          }
          else
            tok.keyword = tok.value.ToUpperInvariant();
        }
        if (tok.value.Length > 0) ls.Add(tok);
      }

      StringParts[] ar = new StringParts[ls.Count];
      ls.CopyTo(ar, 0);

      return ar;
    }
  }
}
