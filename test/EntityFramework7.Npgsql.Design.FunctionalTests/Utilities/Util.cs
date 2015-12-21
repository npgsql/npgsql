using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EntityFramework7.Npgsql.Design.FunctionalTests.Utilities
{
    internal static class Util
    {
        internal static string ToUnixNewlines(this string s)
        {
            return s.Replace("\r\n", "\n");
        }
    }
}
